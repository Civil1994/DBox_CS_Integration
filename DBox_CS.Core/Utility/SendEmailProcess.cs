//using MailKit.Security;
//using MimeKit;
using DBox_CS.Core.AppClass;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
//using MailKit.Net.Pop3;
//using MailKit.Security;
using System;
using System.Net;
using DBox_CS.Core.DALayer;

namespace DBox_CS.Core.Utility
{
    public class SendEmailProcess
    {
        #region Email
        public static bool SendEmailWithCSVAttachment(int ufiProcessId, ref string error)
        {
            try
            {
                Common.LogAction($"Send Error Email Process Started");
                int failedMailCount = 0;
                //Flag For Send Email Process
                int emailFlag = GetEmailFlagFromDatabase();
                if (emailFlag == 0)
                {
                    error = "Email sending is disabled.";
                    Common.LogAction(error);
                    return true;
                }

                error = "Email sending is Available.";
                Common.LogAction(error);
                //Error details and save them to a CSV file
                List<FileInfo> errfileInfo=new List<FileInfo>();
                DataTable errorTable=GetErrorDetailsToEmail(ufiProcessId);
                if (errorTable == null || errorTable.Rows.Count == 0)
                {
                    error = "No error details found to send.";
                    Common.LogAction(error);
                    return false;
                }

                Common.LogAction($"Fetched {errorTable.Rows.Count} error records for today's date.");

                var distinctMailRecId = errorTable.AsEnumerable()
                                      .Select(row => row.Field<int>("MailRecipientID"))
                                      .Distinct();

                Dictionary<string, List<string>> emailAddresses = new Dictionary<string, List<string>>();

                string fromEmail=GetSenderEmailAddress();

                if (string.IsNullOrEmpty(fromEmail))
                {
                    error = "Unable to fetch EMail Sender Id.";
                    Common.LogAction(error);
                    return false;
                }

                emailAddresses["From"] = new List<string> { fromEmail };

                foreach (var mailRecId in distinctMailRecId)
                {

                    List<string> tolist=GetRecipientEmailAddresses(mailRecId);
                    if (tolist==null || tolist.Count==0)
                    {
                        error = "No receipient Email Id set for EmailRecipientId " + mailRecId.ToString() ;
                        Common.LogAction(error);
                        return false;
                    }
                    emailAddresses["To"] = tolist;

                    DataRow[] selectederrorRows = errorTable.Select("MailRecipientID = " + mailRecId);
                    DataTable selectederrorTbl  = selectederrorRows.CopyToDataTable();
                    selectederrorTbl.Columns.Remove("MailRecipientID");

                    errfileInfo = new List<FileInfo>();

                    string fileprefix = $"ERR_{mailRecId}_{DateTime.Now:yyyyMMdd_HHmmss}-";
                    SaveErrorDetailsToCSV(selectederrorTbl, fileprefix, ref errfileInfo);

                    List<string> latestFiles = null;
                    if (errfileInfo != null && errfileInfo.Count > 0)
                    {
                        latestFiles = errfileInfo.Select(fi => fi.FullName).ToList();
                    }

                    if (!latestFiles.Any())
                    {
                        error = "No matching CSV files found.";
                        Common.LogAction(error);
                        return false;
                    }

                    Common.LogAction("error file count:" + latestFiles.Count.ToString());

 
                    fromEmail = emailAddresses.ContainsKey("From") ? string.Join(",", emailAddresses["From"]) : string.Empty;
                    string toEmail = emailAddresses.ContainsKey("To") ? string.Join(",", emailAddresses["To"]) : string.Empty;



                    // 5. Get SMTP configuration from database
                    DataTable smtpConfig = GetSmtpConfigFromDatabase();
                    if (smtpConfig == null || smtpConfig.Rows.Count == 0)
                    {
                        error = "SMTP configuration not found in the database.";
                        Common.LogAction(error);
                        return false;
                    }

                    string smtpHost = smtpConfig.Rows[0]["Host"].ToString();
                    int smtpPort = Convert.ToInt32(smtpConfig.Rows[0]["Port"]);
                    bool enableSsl = Convert.ToBoolean(smtpConfig.Rows[0]["IsEnableSSL"]);
                    string smtpUsername = smtpConfig.Rows[0]["Username"].ToString();
                    string encryptedPassword = smtpConfig.Rows[0]["Password"].ToString();
                    string password = DecryptPassword(encryptedPassword);

                    // 6. Send the email with the CSV file as attachment
                    bool emailSent = SendEmailWithAttachments(fromEmail, toEmail, "UNIFOCUS Integration Error Notification",
                                                     "Please find the error details attached.", ref error,
                                                     latestFiles, smtpHost, smtpPort, enableSsl, smtpUsername, password);
                    if (emailSent)
                    {
                        Common.LogAction($"From Email: {fromEmail}, To Email: {toEmail} Succesfully");
                    }
                    else
                    {
                        failedMailCount++;
                        Common.LogAction($"From Email: {fromEmail}, To Email: {toEmail} Failed");
                    }


                }


                return failedMailCount>0?false:true;
            }
            catch (Exception ex)
            {
                error = $"Error occurred while sending email: {ex.Message}";
                Common.LogAction(error);
                //Logger.LogException(ex);
                return false;
            }
        }

        private static bool SendEmailWithAttachments(string fromEmail, string toEmail, string subject,
                                                      string body, ref string error, List<string> attachmentFiles,
                                                      string smtpHost, int smtpPort, bool enableSsl,
                                                      string smtpUsername, string smtpPassword)
        {
            try
            {
                // Create MailMessage
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(fromEmail);
                mail.To.Add(toEmail);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true; // Adjust if necessary


                // Get the latest files for each filePrefixes
                //var latestFiles = attachmentFiles
                //    .Where(file => File.Exists(file) && Common.validFiles.Any(prefix => Path.GetFileName(file).StartsWith(prefix, StringComparison.OrdinalIgnoreCase)))
                //    .GroupBy(file => Common.validFiles.FirstOrDefault(prefix => Path.GetFileName(file).StartsWith(prefix, StringComparison.OrdinalIgnoreCase)))
                //    .Select(group => group.OrderByDescending(file => File.GetLastWriteTime(file)).FirstOrDefault())
                //    .ToList();

                var latestFiles = attachmentFiles;

                Common.LogAction("error file count2:" + latestFiles.Count.ToString());

                // Attach only the filtered latest files
                foreach (var file in latestFiles)
                {
                    Common.LogAction("File.Exists check:" + file);
                    if (File.Exists(file))
                    {
                        Attachment attachment = new Attachment(file);
                        mail.Attachments.Add(attachment);
                    }
                }

                //// Add attachments
                //foreach (var file in attachmentFiles)
                //{
                //    if (File.Exists(file))
                //    {
                //        Attachment attachment = new Attachment(file);
                //        mail.Attachments.Add(attachment);
                //    }
                //}

                // Configure SMTP client
                using (SmtpClient smtpClient = new SmtpClient(smtpHost, smtpPort))
                {
                    smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                    smtpClient.EnableSsl = enableSsl;

                    // Send email
                    smtpClient.Send(mail);
                }

                //Common.LogAction($"From Email: {fromEmail}, To Email: {toEmail} Succesfully");
                return true;
            }
            catch (Exception ex)
            {
                error = $"Error occurred while sending email: {ex.Message}";
                Common.LogAction(error);
                if (ex.InnerException != null)
                {
                    Common.LogAction($"Inner Exception: {ex.InnerException.Message}");
                }
                return false;
            }
        }

        private static string DecryptPassword(string encryptedPassword)
        {
            encryptedPassword = cryptoutil.Decrypt(encryptedPassword);
            return encryptedPassword;
        }

        private static Dictionary<string, List<string>> GetEmailAddresses(string fromEmailQuery, string toEmailQuery)
        {
            //Dictionary<string, string> emailAddresses = new Dictionary<string, string>();
            Dictionary<string, List<string>> emailAddresses = new Dictionary<string, List<string>>();

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionFunctions.GetConnectionString()))
                {
                    conn.Open();

                    // Fetch "From" email address
                    using (SqlCommand cmd = new SqlCommand(fromEmailQuery, conn))
                    {
                        //object fromResult = cmd.ExecuteScalar();
                        //emailAddresses["From"] = fromResult != null ? fromResult.ToString() : string.Empty;

                        object fromResult = cmd.ExecuteScalar();
                        string fromEmail = fromResult != null ? fromResult.ToString() : string.Empty;

                        if (!string.IsNullOrEmpty(fromEmail))
                        {
                            emailAddresses["From"] = new List<string> { fromEmail }; // Add the single "From" email
                        }
                    }

                    // Fetch "To" email address
                    using (SqlCommand cmd = new SqlCommand(toEmailQuery, conn))
                    {
                        //object toResult = cmd.ExecuteScalar();
                        //emailAddresses["To"] = toResult != null ? toResult.ToString() : string.Empty;
                        object toResult = cmd.ExecuteScalar();
                        string toEmails = toResult != null ? toResult.ToString() : string.Empty;

                        if (!string.IsNullOrEmpty(toEmails))
                        {
                            // Split "To" email addresses by delimiter (comma in this case) and trim each address
                            emailAddresses["To"] = toEmails
                                .Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(email => email.Trim())
                                .ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Common.LogAction($"Error occurred while fetching email addresses. Details: {ex.Message}");
                Common.LogException(ex);
            }

            return emailAddresses;
        }

        private static string GetSenderEmailAddress()
        {

            string fromEmail = "";
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionFunctions.GetConnectionString()))
                {
                    conn.Open();

                    string fromEmailQuery = "SELECT userName FROM MailConfig2 WHERE Id = 2";
                    // Fetch "From" email address
                    using (SqlCommand cmd = new SqlCommand(fromEmailQuery, conn))
                    {

                        object fromResult = cmd.ExecuteScalar();
                        fromEmail = fromResult != null ? fromResult.ToString() : string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                Common.LogAction($"Error occurred while fetching email addresses. Details: {ex.Message}");
                Common.LogException(ex);
            }

            return fromEmail;
        }
        private static List<string> GetRecipientEmailAddresses(int EmailRecipientId)
        {
            //Dictionary<string, string> emailAddresses = new Dictionary<string, string>();
            List<string> emailAddresses = new  List<string>();

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionFunctions.GetConnectionString()))
                {
                    conn.Open();

                    string toEmailQuery = "Select ISNULL([EmailIds],'') from [UFI_EmailRecipients] where EmailRecipientId=" + EmailRecipientId;

                    // Fetch "To" email address
                    using (SqlCommand cmd = new SqlCommand(toEmailQuery, conn))
                    {
                        //object toResult = cmd.ExecuteScalar();
                        //emailAddresses["To"] = toResult != null ? toResult.ToString() : string.Empty;
                        object toResult = cmd.ExecuteScalar();
                        string toEmails = toResult != null ? toResult.ToString() : string.Empty;

                        if (!string.IsNullOrEmpty(toEmails))
                        {
                            // Split "To" email addresses by delimiter (comma in this case) and trim each address
                            emailAddresses= toEmails
                                .Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(email => email.Trim())
                                .ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Common.LogAction($"Error occurred while fetching email addresses. Details: {ex.Message}");
                Common.LogException(ex);
            }

            return emailAddresses;
        }
        private static DataTable GetSmtpConfigFromDatabase()
        {
            DataTable dt = new DataTable();
            string error = string.Empty;

            try
            {
                string query = "SELECT * FROM MailConfig2 WHERE Id = 2";

                string connectionString = ConnectionFunctions.GetConnectionString();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    adapter.Fill(dt);
                }

                if (dt.Rows.Count == 0)
                {
                    error = "SMTP configuration not found in the database.";
                    Common.LogAction(error);
                }
            }
            catch (Exception ex)
            {
                Common.LogException(ex);
            }

            return dt;
        }

        #region Error DataTable
        public static DataTable GetErrorDetailsToEmail(int ufiProcessId)
        {
            string appFilesPath = ConfigurationManager.AppSettings["AppFilesPath"].ToString();
            string decryptDirectory = Path.Combine(appFilesPath, "DownloadedFile\\Processed\\");
            try
            {

                if (ufiProcessId == 0)
                {
                    Common.LogAction("invalid ufiProcessId.");
                    return new DataTable();
                }

                //Query to fetch error details for the file name and current date

                string query = @"
                                Select pl.UFIProcessId,pl.ProcessName, pld.EmpCode,  dbo.GetNameE(dbo.getempid(pld.EmpCode)) 'EmpName', pld.ErrorText, FORMAT(pld.LoggedDate,'dd/MM/yyyy HH:mm:ss') as LoggedDate, dbo.fn_UFI_GetErrorMailRecipientID(pld.EmpCode) as MailRecipientID
                                From UFIExportProcessLogDetails pld INNER JOIN UFIProcessLog pl on pl.UFIProcessId=pld.UFIProcessId
                                where isnull(pld.HasErrors,0)=1 and pl.UFIProcessId=@UFIProcessId";


                query += @" UNION Select pl.UFIProcessId,pl.ProcessName, pld.EmpCode,  dbo.GetNameE(dbo.getempid(pld.EmpCode)) 'EmpName', pld.Remarks as ErrorText, FORMAT(pld.LoggedDate,'dd/MM/yyyy HH:mm:ss') as LoggedDate, dbo.fn_UFI_GetErrorMailRecipientID(pld.EmpCode) as MailRecipientID
                                From UFIImportProcessLogDetails pld INNER JOIN UFIProcessLog pl on pl.UFIProcessId = pld.UFIProcessId
                                where isnull(pld.HasLineErrors,0)= 1 and pl.UFIProcessId = @UFIProcessId";


                DataTable errorTable = new DataTable();

                using (SqlConnection conn = new SqlConnection(ConnectionFunctions.GetConnectionString()))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UFIProcessId", ufiProcessId);
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(errorTable);
                        }
                    }


                }
               
   
                return errorTable;
            }
            catch (Exception ex)
            {
                string errorMessage = $"Error occurred while fetching error details for the UFIProcessID '{ufiProcessId .ToString()}'. Details: {ex.Message}";
                Common.LogAction(errorMessage);
                Common.LogException(ex);
                return new DataTable();
            }
        }
        #endregion

        #region Datatable to CSV
        public static void SaveErrorDetailsToCSV(DataTable errorTable, string fileprefix, ref List<FileInfo> errfileInfo)
        {
            errfileInfo = new List<FileInfo>();
            if (errorTable == null || errorTable.Rows.Count == 0)
            {
                Common.LogAction("No data available to create Error files.");
                return;
            }

            //string outputFolderPath = ConfigurationManager.AppSettings["OutputFilePath"].ToString();
            string appFilesPath = ConfigurationManager.AppSettings["AppFilesPath"].ToString();
            string outputFolderPath = Path.Combine(appFilesPath, "ErrorsFile\\");
            // Ensure the output folder exists
            if (!Directory.Exists(outputFolderPath))
            {
                Directory.CreateDirectory(outputFolderPath);
            }

            var groupedData = errorTable.AsEnumerable()
                                        .GroupBy(row => row["UFIProcessId"].ToString())
                                        .ToList();
            foreach (var group in groupedData)
            {
                string fileName = group.Key.ToString();

                string csvFilePath = Path.Combine(outputFolderPath, $"{fileprefix}{ fileName.Replace(".csv", "")}.csv");

                //Create a StringBuilder to build the CSV content
                StringBuilder csvContent = new StringBuilder();

                //Write the header row to CSV
                var columnNames = errorTable.Columns.Cast<DataColumn>()
                                                  .Select(column => EscapeCsvValue(column.ColumnName))
                                                  .ToArray();
                csvContent.AppendLine(string.Join(",", columnNames));

                //Write the data rows for each FileName
                foreach (var row in group)
                {
                    var values = row.ItemArray.Select(field => EscapeCsvValue(field.ToString()))
                                              .ToArray();
                    csvContent.AppendLine(string.Join(",", values));
                }
                //foreach (var row in group)
                //{
                //    var values = row.ItemArray.Select(field => field.ToString().Replace(",", " "))
                //                              .ToArray();
                //    csvContent.AppendLine(string.Join(",", values));
                //}

                File.WriteAllText(csvFilePath, csvContent.ToString());

                errfileInfo.Add(new FileInfo(csvFilePath));

                Common.LogAction($"Error Details CSV file created for '{fileName}' at {csvFilePath}");
            }
        }

        private static string EscapeCsvValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "\"\"";

            value = value.Replace("\"", "\"\"");
            if (value.Contains(",") || value.Contains("\n") || value.Contains("\r"))
            {
                value = $"\"{value}\"";
            }
            return value;
        }
        #endregion

        #region Email Start/Stop Flag
        private static int GetEmailFlagFromDatabase()
        {
            try
            {
                // Query the database to fetch the flag value (Val) for Code = 'EMLFLG'
                string query = "SELECT Val FROM UFI_IntegrationSettings WHERE Code = 'EMLFLG'";

                // Execute the query and return the flag value
                using (SqlConnection conn = new SqlConnection(ConnectionFunctions.GetConnectionString()))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        return result != null ? Convert.ToInt32(result) : 0; // Default to 0 if no value is found
                    }
                }
            }
            catch (Exception ex)
            {
                Common.LogException(ex);
                return 0; // Default to 0 in case of error
            }
        }
        #endregion
        #endregion
    }
}
