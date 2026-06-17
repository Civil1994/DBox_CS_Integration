using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DBox_CS.Core.APIClient;
using DBox_CS.Core.AppClass;
using DBox_CS.Core.DALayer;
using DBox_CS.Core.Models;

namespace DBox_CS.Core.BL
{
    public class EmployeeExportBL
    {
        private ApiClient _apiClient;
        private HttpClient _httpClient;

        private bool RetVal = false;
        string errmsg = "";
        String sQry = String.Empty;
        private int result = 0;
        
        internal EmployeePushDTO ConvertToEmployeePushDto(DataRow drow)
        {
            EmployeePushDTO empDTO = new EmployeePushDTO();

            empDTO.EmpCode = drow["EmpCode"] == DBNull.Value ? "" : drow["EmpCode"].ToString();
            empDTO.EmpNameA = drow["EmpNameA"] == DBNull.Value ? "" : drow["EmpNameA"].ToString();

            // English Name
            empDTO.FNameE = drow["FNameE"] == DBNull.Value ? "" : drow["FNameE"].ToString();
            empDTO.SNameE = drow["SNameE"] == DBNull.Value ? "" : drow["SNameE"].ToString();
            empDTO.NickNameE = drow["NickNameE"] == DBNull.Value ? "" : drow["NickNameE"].ToString();
            empDTO.GrandFatherE = drow["GrandFatherE"] == DBNull.Value ? "" : drow["GrandFatherE"].ToString();
            empDTO.FamilyNameE = drow["FamilyNameE"] == DBNull.Value ? "" : drow["FamilyNameE"].ToString();

            // Arabic Name
            empDTO.FNameA = drow["FNameA"] == DBNull.Value ? "" : drow["FNameA"].ToString();
            empDTO.SNameA = drow["SNameA"] == DBNull.Value ? "" : drow["SNameA"].ToString();
            empDTO.NickNameA = drow["NickNameA"] == DBNull.Value ? "" : drow["NickNameA"].ToString();
            empDTO.GrandFatherA = drow["GrandFatherA"] == DBNull.Value ? "" : drow["GrandFatherA"].ToString();
            empDTO.FamilyNameA = drow["FamilyNameA"] == DBNull.Value ? "" : drow["FamilyNameA"].ToString();
            empDTO.MotherNameA = drow["MotherNameA"] == DBNull.Value ? "" : drow["MotherNameA"].ToString();

            // Location / Address
            empDTO.BirthPlaceA = drow["BirthPlaceA"] == DBNull.Value ? "" : drow["BirthPlaceA"].ToString();
            empDTO.PIssuePlaceA = drow["PIssuePlaceA"] == DBNull.Value ? "" : drow["PIssuePlaceA"].ToString();
            empDTO.BuildingA = drow["BuildingA"] == DBNull.Value ? "" : drow["BuildingA"].ToString();
            empDTO.AddressA = drow["AddressA"] == DBNull.Value ? "" : drow["AddressA"].ToString();
            empDTO.PerAddressA = drow["PerAddressA"] == DBNull.Value ? "" : drow["PerAddressA"].ToString();

            // Identity
            empDTO.UIDNo = drow["UIDNo"] == DBNull.Value ? "" : drow["UIDNo"].ToString();
            empDTO.NationalID = drow["NationalID"] == DBNull.Value ? "" : drow["NationalID"].ToString();

            // Residence / Labour
            empDTO.ResidenceNo = drow["ResidenceNo"] == DBNull.Value ? "" : drow["ResidenceNo"].ToString();
            empDTO.ResIssuePlace = drow["ResIssuePlace"] == DBNull.Value ? "" : drow["ResIssuePlace"].ToString();
            empDTO.ResIssueDate = drow["ResIssueDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(drow["ResIssueDate"]);
            empDTO.ResExpDate = drow["ResExpDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(drow["ResExpDate"]);

            empDTO.LabCardNo = drow["LabCardNo"] == DBNull.Value ? "" : drow["LabCardNo"].ToString();
            empDTO.LCIssuePlace = drow["LCIssuePlace"] == DBNull.Value ? "" : drow["LCIssuePlace"].ToString();
            empDTO.LCExpDate = drow["LCExpDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(drow["LCExpDate"]);

            // Visa
            empDTO.VisaNo = drow["VisaNo"] == DBNull.Value ? "" : drow["VisaNo"].ToString();
            empDTO.VisaIssueDate = drow["VisaIssueDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(drow["VisaIssueDate"]);
            empDTO.VisaExpDate = drow["VisaExpDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(drow["VisaExpDate"]);

            // Auxiliary
            empDTO.AuxDate3 = drow["AuxDate3"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(drow["AuxDate3"]);
            empDTO.AuxDate4 = drow["AuxDate4"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(drow["AuxDate4"]);
            empDTO.AuxDate5 = drow["AuxDate5"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(drow["AuxDate5"]);

            empDTO.AuxString7 = drow["AuxString7"] == DBNull.Value ? "" : drow["AuxString7"].ToString();
            empDTO.AuxLib5 = drow["AuxLib5"] == DBNull.Value ? "" : drow["AuxLib5"].ToString();
            empDTO.AuxLib6 = drow["AuxLib6"] == DBNull.Value ? "" : drow["AuxLib6"].ToString();

            return empDTO;
        }

        private DocumentPushDTO ConvertToDocumentPushDto(DataRow row)
        {
            return new DocumentPushDTO
            {
                employee_no = "112591",// row["employee_no"].ToString(),
                section = row["section"].ToString(),
                section_attribute = row["section_attribute"].ToString(),
                attachment_file_name = row["attachment_file_name"].ToString(),
                attachment_file_extension = Convert.ToString(row["attachment_file_extension"]).Trim().TrimStart('.'),
                //AttachmentFileExtension = row["attachment_file_extension"].ToString(),
                attachment = row["attachment"] != DBNull.Value
                    ? Convert.ToBase64String((byte[])row["attachment"])
                    : string.Empty
            };
        }

        public void UploadeEmployeeToDBOX()
        {
            int DBOXIProcessId;
            bool hasProcessError = false;

            DBOXIProcessId = Common.CreateDBoxIProcessLogEntry("Employee Push to DBOX");


            if (DBOXIProcessId == 0)
            {
                Common.LogAction("Error generating DBOXI Process ID");
                return;
            }


            try
            {
                string apikeyheader = ConfigurationManager.AppSettings["UFApiSettings.APIKeyHeader"].ToString();
                string apikey = ConfigurationManager.AppSettings["UFApiSettings.APIKey"].ToString();

                string clientId = ConfigurationManager.AppSettings["DBOXApiSettings.ClientId"].ToString();
                string clientSecret = ConfigurationManager.AppSettings["DBOXApiSettings.ClientSecret"].ToString();


                if (string.IsNullOrEmpty(clientId))
                {
                    throw new Exception("Cliend ID is missing.");
                }
                if (string.IsNullOrEmpty(clientSecret))
                {
                    throw new Exception("Client Secret missing.");
                }


                _httpClient = new HttpClient();
                _apiClient = new ApiClient(_httpClient, apikey, apikeyheader);

                DataTable empdt = GetEmployeeForExportToDBox();

                if (empdt == null || empdt.Rows.Count == 0)
                {
                    Common.LogAction("No Employee data to Upload.");
                    Common.UpdateRemarksToDBOXIExportProcessLogDetails(DBOXIProcessId, "", "", "No Employee data to Upload.");
                    return;
                }

                LogDBOXIExportData(empdt, DBOXIProcessId);

                EmployeePushModel employeesData = new EmployeePushModel();
                employeesData.apiKey = apikey;
                employeesData.importId = "employeesdata";
                employeesData.groupCompany = "";

                List<EmployeePushDTO> empList = new List<EmployeePushDTO>();
                EmployeePushDTO empDTO = new EmployeePushDTO();

                string EmpId = string.Empty;
                foreach (DataRow drow in empdt.Rows)
                {
                    empDTO = new EmployeePushDTO();
                    empDTO = ConvertToEmployeePushDto(drow);
                    empList.Add(empDTO);
                    EmpId = drow["EmpId"].ToString();
                    try
                    {
                        employeesData.importData = empList;
                        var response = _apiClient.PostEmployeeData(employeesData);

                        if (response == null)
                        {
                            hasProcessError = true;
                            Common.LogAction("api response is null");
                            Common.UpdateRemarksToDBOXIExportProcessLogDetails(DBOXIProcessId, empDTO.EmpCode, "", "api response is null", true);
                        }
                        else
                        {
                            if (response.IsSuccessStatusCode)
                            {

                                Common.LogAction("Posted " + empDTO.EmpCode);
                                Common.UpdateRemarksToDBOXIExportProcessLogDetails(DBOXIProcessId, empDTO.EmpCode, "", "Posted Successfully", false);
                                Update_EmpLastExportDateTime(empDTO.EmpCode, EmpId);
                            }
                            else
                            {
                                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                                {
                                    hasProcessError = true;
                                    Common.LogAction("Failure StatusCode " + ((int)response.StatusCode).ToString() + " bad request -Incorrect values.");
                                    Common.UpdateRemarksToDBOXIExportProcessLogDetails(DBOXIProcessId, empDTO.EmpCode, "", "Failure StatusCode " + ((int)response.StatusCode).ToString() + " bad request -Incorrect values.");
                                }
                                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                                {
                                    hasProcessError = true;
                                    Common.LogAction("Failure StatusCode " + ((int)response.StatusCode).ToString() + " forbidden request -Invalid 'dbox - api - secret'.");
                                    Common.UpdateRemarksToDBOXIExportProcessLogDetails(DBOXIProcessId, empDTO.EmpCode, "", "Failure StatusCode " + ((int)response.StatusCode).ToString() + " forbidden request -Invalid 'dbox - api - secret'.");
                                }
                                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                                {
                                    hasProcessError = true;
                                    Common.LogAction("Failure StatusCode " + ((int)response.StatusCode).ToString() + " internal server error - location Code does not exist");
                                    Common.UpdateRemarksToDBOXIExportProcessLogDetails(DBOXIProcessId, empDTO.EmpCode, "", "Failure StatusCode " + ((int)response.StatusCode).ToString() + " internal server error - location Code does not exist.");
                                }
                                else
                                {
                                    hasProcessError = true;
                                    Common.LogAction("Failure StatusCode " + ((int)response.StatusCode).ToString());
                                    Common.UpdateRemarksToDBOXIExportProcessLogDetails(DBOXIProcessId, empDTO.EmpCode, "", "Failure StatusCode " + ((int)response.StatusCode).ToString());

                                }

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        hasProcessError = true;
                        Common.LogAction("Error occured at Posting for Empcode " + drow["EmpCode"].ToString() + ". Error:" + ex.Message);
                        Common.LogException(ex);
                        Common.UpdateRemarksToDBOXIExportProcessLogDetails(DBOXIProcessId, drow["EmpCode"].ToString(), "", "Error occured at Posting. Error:" + ex.Message);
                    }
           

                }

                
            }
            catch (Exception ex)
            {
                hasProcessError = true;
                Common.LogAction("Error occured at Posting. Error:" + ex.Message);
                Common.LogException(ex);
                Common.UpdateRemarksToDBOXIExportProcessLogDetails(DBOXIProcessId, "", "", "Error occured at Posting. Error:" + ex.Message);
            }

           
            Common.LogUFIProcessCompletion(DBOXIProcessId, "Successfully pushed employee data",hasProcessError);

        }

        public DataTable GetEmployeeForExportToDBox()
        {
            DataTable dtEmpUpload = new DataTable();
            string ErrMsg = "";

            string sql = @"
                SELECT e.EmpId, e.EmpCode, e.EmpNameA, e.FNameE, e.SNameE, e.NickNameE, e.GrandFatherE, e.FamilyNameE,
                       e.FNameA, e.SNameA, e.NickNameA, e.GrandFatherA, e.FamilyNameA, e.MotherNameA,
                       e.BirthPlaceA, e.PIssuePlaceA, e.UIDNo, e.BuildingA, e.AddressA, e.PerAddressA,
                       e.ResidenceNo, e.ResIssuePlace, e.ResIssueDate, e.ResExpDate,
                       e.NationalID, e.LabCardNo, e.LCIssuePlace, e.LCExpDate,
                       e.VisaNo, e.VisaIssueDate, e.VisaExpDate,
                       e.AuxDate3, e.AuxDate4, e.AuxDate5,
                       e.AuxString7, e.AuxLib5, e.AuxLib6
                FROM Employee e
                INNER JOIN EmployeeSyncTracker_DBOXI t
                    ON e.EmpID = t.EmpID
                WHERE t.Employee_LastModifiedDateTime > t.Employee_LASTPUSHEDDTTM
            ";

            bool RetVal = ConnectionFunctions.Connect_SQLDataTable(ref dtEmpUpload, sql, ref ErrMsg);
            if (RetVal == false)
            {
                return null;
            }

            return dtEmpUpload;
        }

        private void LogDBOXIExportData(DataTable dt, int dboxiProcessId)
        {
            try
            {
                // Add required columns if not present
                if (!dt.Columns.Contains("DBOXIProcessId"))
                    dt.Columns.Add("DBOXIProcessId", typeof(int));

                if (!dt.Columns.Contains("InsertedDate"))
                    dt.Columns.Add("InsertedDate", typeof(DateTime));

                if (!dt.Columns.Contains("ExportRawData"))
                    dt.Columns.Add("ExportRawData", typeof(string));

                if (!dt.Columns.Contains("ExportResponseData"))
                    dt.Columns.Add("ExportResponseData", typeof(string));

                DateTime insertTime = DateTime.Now;

                foreach (DataRow row in dt.Rows)
                {
                    row["DBOXIProcessId"] = dboxiProcessId;
                    row["InsertedDate"] = insertTime;

                    // Optional: if not already filled
                    if (row["ExportRawData"] == DBNull.Value)
                        row["ExportRawData"] = "";

                    if (row["ExportResponseData"] == DBNull.Value)
                        row["ExportResponseData"] = "";
                }

                // Optional: Set column order (not mandatory but good practice)
                dt.Columns["DBOXIProcessId"].SetOrdinal(0);
                dt.Columns["InsertedDate"].SetOrdinal(1);

                using (SqlConnection conn = new SqlConnection(ConnectionFunctions.GetConnectionString()))
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
                {
                    bulkCopy.DestinationTableName = "dbo.DBOXI_EmpUpdateExportLog";

                    foreach (DataColumn col in dt.Columns)
                    {
                        bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                    }

                    conn.Open();
                    bulkCopy.WriteToServer(dt);
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Common.LogAction("DBOXI Export data logging failed. Details: " + ex.Message);
                Common.LogException(ex);
            }
        }

        private bool Update_EmpLastExportDateTime(string strEmpCode,string empId)
        {
            errmsg = "";

            if (string.IsNullOrEmpty(strEmpCode))
            {
                return false;
            }
            string actualEmpCode = strEmpCode;



            sQry = "IF NOT EXISTS (SELECT * FROM EmployeeSyncTracker_DBOXI WHERE empcode='" + actualEmpCode + "')" +
                "BEGIN " +
                "   Insert into EmployeeSyncTracker_DBOXI ([EmpId],[EmpCode],[Employee_LastModifiedDateTime],[Employee_LASTPUSHEDDTTM],) values (" + empId + ",'" + actualEmpCode + "',GETDATE(),GETDATE());" +
                "End " +
                "Else " +
                "BEGIN " +
                "   UPDATE EmployeeSyncTracker_DBOXI set [Employee_LASTPUSHEDDTTM] = GETDATE() Where [EmpCode]='" + actualEmpCode + "' And EmpId = " + empId +" ;"+
                "End ";

            RetVal = ConnectionFunctions.Connect_SQLNonQuery(ref result, sQry, ref errmsg);

            if (!RetVal)
            {
                Common.LogAction("Update_EmployeeSyncTracker_DBOXI failed. Details: " + errmsg);
            }

            return RetVal;
        }

        private bool Update_EmpDocumentLastExportDateTime(string strEmpCode, string empId)
        {
            errmsg = "";

            if (string.IsNullOrEmpty(strEmpCode))
            {
                return false;
            }
            string actualEmpCode = strEmpCode;



            sQry = "IF NOT EXISTS (SELECT * FROM EmployeeSyncTracker_DBOXI WHERE empcode='" + actualEmpCode + "')" +
                "BEGIN " +
                "   Insert into EmployeeSyncTracker_DBOXI ([EmpId],[EmpCode],[Employee_LastModifiedDateTime],[EmpDocument_LASTPUSHEDDTTM],) values (" + empId + ",'" + actualEmpCode + "',GETDATE(),GETDATE());" +
                "End " +
                "Else " +
                "BEGIN " +
                "   UPDATE EmployeeSyncTracker_DBOXI set [EmpDocument_LASTPUSHEDDTTM] = GETDATE() Where [EmpCode]='" + actualEmpCode + "' And EmpId = " + empId + " ;" +
                "End ";

            RetVal = ConnectionFunctions.Connect_SQLNonQuery(ref result, sQry, ref errmsg);

            if (!RetVal)
            {
                Common.LogAction("Update_EmployeeSyncTracker_DBOXI failed. Details: " + errmsg);
            }

            return RetVal;
        }
        public void UploadeEmployeeDocumentToDBOX()
        {
            int DBOXIProcessId;
            bool hasProcessError = false;
            Common.LogAction("Employee Document export started");
            DBOXIProcessId = Common.CreateDBoxIProcessLogEntry("Employee Document Push to DBOX");


            if (DBOXIProcessId == 0)
            {
                Common.LogAction("Error generating DBOXI Process ID");
                return;
            }


            try
            {
               
                string apikeyheader = ConfigurationManager.AppSettings["DBOXApiSettings.APIKeyHeader"].ToString();
                string apikey = ConfigurationManager.AppSettings["DBOXApiSettings.APIKey"].ToString();

                string clientId = ConfigurationManager.AppSettings["DBOXApiSettings.ClientId"].ToString();
                string clientSecret = ConfigurationManager.AppSettings["DBOXApiSettings.ClientSecret"].ToString();


                if (string.IsNullOrEmpty(clientId))
                {
                    throw new Exception("Cliend ID is missing.");
                }
                if (string.IsNullOrEmpty(clientSecret))
                {
                    throw new Exception("Client Secret missing.");
                }


                _httpClient = new HttpClient();
                _apiClient = new ApiClient(_httpClient, apikey, apikeyheader);

               

                DataTable employeeList = GetEmployeeIdForDocumentExportToDBox();

                if (employeeList == null || employeeList.Rows.Count == 0)
                {
                    Common.LogAction("No Employees found for document export.");
                    return;
                }

                foreach (DataRow empRow in employeeList.Rows)
                {
                    int empId = Convert.ToInt32(empRow["EmpID"]);
                    string empCode = empRow["EmpCode"].ToString();
                    DataTable empdt = GetDocumentForExportToDBox(empId);

                    if (empdt == null || empdt.Rows.Count == 0)
                    {
                        Common.LogAction("No documents found for Employee ID : " + empId);
                        continue;
                    }

                    LogDBOXIExportData(empdt, DBOXIProcessId);

                    EmployeePushModel employeesData = new EmployeePushModel();
                    //employeesData.apiKey = apikey;
                    //employeesData.importId = "employeesdata";
                    //employeesData.groupCompany = "";

                    List<EmployeePushDTO> empList = new List<EmployeePushDTO>();

                    foreach (DataRow drow in empdt.Rows)
                    {
                        DocumentPushDTO document = ConvertToDocumentPushDto(drow);


                        try
                        {
                        

                            var response = _apiClient.PostEmployeeDocument(document);

                            if (response == null)
                            {
                                hasProcessError = true;
                                Common.LogAction("api response is null");
                                Common.UpdateRemarksToDBOXIExportProcessLogDetails(DBOXIProcessId, document.employee_no, "", "api response is null", true);
                            }
                            else
                            {
                                if (response.IsSuccessStatusCode)
                                {

                                    Common.LogAction("Posted " + document.employee_no);
                                    Common.UpdateRemarksToDBOXIExportProcessLogDetails(DBOXIProcessId, document.employee_no, "", "Posted Successfully", false);
                                    Update_EmpDocumentLastExportDateTime(document.employee_no, empId.ToString());
                                }
                                else
                                {
                                    if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                                    {
                                        hasProcessError = true;
                                        Common.LogAction("Failure StatusCode " + ((int)response.StatusCode).ToString() + " bad request -Incorrect values.");
                                        Common.UpdateRemarksToDBOXIExportProcessLogDetails(DBOXIProcessId, document.employee_no, "", "Failure StatusCode " + ((int)response.StatusCode).ToString() + " bad request -Incorrect values.");
                                    }
                                    else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                                    {
                                        hasProcessError = true;
                                        Common.LogAction("Failure StatusCode " + ((int)response.StatusCode).ToString() + " forbidden request -Invalid 'dbox - api - secret'.");
                                        Common.UpdateRemarksToDBOXIExportProcessLogDetails(DBOXIProcessId, document.employee_no, "", "Failure StatusCode " + ((int)response.StatusCode).ToString() + " forbidden request -Invalid 'dbox - api - secret'.");
                                    }
                                    else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                                    {
                                        hasProcessError = true;
                                        Common.LogAction("Failure StatusCode " + ((int)response.StatusCode).ToString() + " internal server error - location Code does not exist");
                                        Common.UpdateRemarksToDBOXIExportProcessLogDetails(DBOXIProcessId, document.employee_no, "", "Failure StatusCode " + ((int)response.StatusCode).ToString() + " internal server error - location Code does not exist.");
                                    }
                                    else
                                    {
                                        hasProcessError = true;
                                        Common.LogAction("Failure StatusCode " + ((int)response.StatusCode).ToString());
                                        Common.UpdateRemarksToDBOXIExportProcessLogDetails(DBOXIProcessId, document.employee_no, "", "Failure StatusCode " + ((int)response.StatusCode).ToString());

                                    }

                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            hasProcessError = true;
                            Common.LogAction("Error occured at Posting. Error:" + ex.Message);
                            Common.LogException(ex);
                        }
                    }
                }


              

               
               


            }
            catch (Exception ex)
            {
                hasProcessError = true;
                Common.LogAction("Error occured at Posting. Error:" + ex.Message);
                Common.LogException(ex);
                Common.UpdateRemarksToDBOXIExportProcessLogDetails(DBOXIProcessId, "", "", "Error occured at Posting. Error:" + ex.Message);
            }


            Common.LogUFIProcessCompletion(DBOXIProcessId, "Successfully pushed employee data", hasProcessError);

        }


        public DataTable GetDocumentForExportToDBox(int empId)
        {
            DataTable dtEmpUpload = new DataTable();


          

            string connStr = ConnectionFunctions.GetConnectionString();

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand("dbo.GetEmployeeDocumentsExport", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EmpID", empId);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    conn.Open();
                    da.Fill(dtEmpUpload);
                }
            }

            return dtEmpUpload.Rows.Count > 0 ? dtEmpUpload : null;
        }
        public DataTable GetEmployeeIdForDocumentExportToDBox()
        {
            DataTable dtEmpUpload = new DataTable();
            string ErrMsg = "";

            string sql = @"
                SELECT e.EmpId, e.EmpCode
                FROM Employee e
                INNER JOIN EmployeeSyncTracker_DBOXI t
                    ON e.EmpID = t.EmpID
                WHERE t.Employee_LastModifiedDateTime <> t.Employee_LASTPUSHEDDTTM
            ";

            bool RetVal = ConnectionFunctions.Connect_SQLDataTable(ref dtEmpUpload, sql, ref ErrMsg);
            if (RetVal == false)
            {
                return null;
            }

            return dtEmpUpload;
        }
    }

}
