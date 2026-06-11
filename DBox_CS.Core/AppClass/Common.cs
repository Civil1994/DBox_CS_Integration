using DBox_CS.Core.DALayer;
using DBox_CS.Core.HCMS.Entity;
using DBox_CS.Core.Models;
using DBox_CS.Core.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DBox_CS.Core.AppClass
{
    public class Common
    {
        //Note: some constant values specific to DBS, need to make it general
        public const string strSvcUserId = "AUTO";
        public const int nSvcUserNo = 3;
        public const string Language = "0"; //english
        public const string AllowFuturePosting_TA = "0"; //SELECT ISNULL(CONVERT(SMALLINT,Val),0) 'Val' FROM MasterSetup WHERE Code = 23
        public const string CompanyName = "SME";//SELECT ISNULL(Val,'') AS Vals FROM MasterSetup WHERE Code = 15
        public const string HierarchyLevel = "5";//SELECT ISNULL(Val,'') AS Vals FROM MasterSetup WHERE Code = 15
        public static readonly string[] UserInfo = new string[] { nSvcUserNo.ToString(), strSvcUserId, "", "", HierarchyLevel, "", "", "", "", "", "", "", AllowFuturePosting_TA, Language, CompanyName, "", "", "", "" };

        static int iResult = 0;
        static string errmsg = "";
        static string appFilesPath { get => ConfigurationManager.AppSettings["AppFilesPath"]; }

        public const string logFileName = "Log.txt";
        public const string exceptionFilePath = "ExceptionLog.txt";

        public static Hashtable ModulesTable;


        public static void LogTestAction(string logMessage)
        {
            //if (Directory.Exists(appFilesPath + "Log"))
            //{
            //    return;
            //}
            try
            {
                string logDirPath = Path.Combine(appFilesPath, "Log");
                string logFilePath = Path.Combine(logDirPath, logFileName);

                if (!Directory.Exists(logDirPath))
                {
                    Directory.CreateDirectory(logDirPath);
                }

                // set max log size
                int maxLogFileSize = 1024 * 512;
                // prepare exception details
                // string logFilePath = appFilesPath + logFileName;
                logMessage = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\t" + logMessage;

                // Read previous log content
                string lastLogString = "";
                if (File.Exists(logFilePath))
                {
                    lastLogString = Environment.NewLine + File.ReadAllText(logFilePath);
                }

                // Truncate log if necessary
                int currentLogSize = lastLogString.Length;
                if (currentLogSize > maxLogFileSize)
                {
                    lastLogString = lastLogString.Substring(currentLogSize - maxLogFileSize);
                }

                // Write current log message
                File.WriteAllText(logFilePath, logMessage, Encoding.UTF8);

                // Append truncated previous log
                File.AppendAllText(logFilePath, lastLogString, Encoding.UTF8);

            }
            catch (Exception ex)
            {
                ShowMessage($"Error: {ex.Message}\nStack Trace: {ex.StackTrace}", 1);
            }

        }
        public static void LogAction(string logMessage)
        {
            //if (Directory.Exists(appFilesPath + "Log"))
            //{
            //    return;
            //}
            try
            {
                string logDirPath = Path.Combine(appFilesPath, "Log");
                string logFilePath = Path.Combine(logDirPath, logFileName);

                if (!Directory.Exists(logDirPath))
                {
                    Directory.CreateDirectory(logDirPath);
                }

                // set max log size
                int maxLogFileSize = 1024 * 512;
                // prepare exception details
                // string logFilePath = appFilesPath + logFileName;
                logMessage = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\t" + logMessage;

                // Read previous log content
                string lastLogString = "";
                if (File.Exists(logFilePath))
                {
                    lastLogString = Environment.NewLine + File.ReadAllText(logFilePath);
                }

                // Truncate log if necessary
                int currentLogSize = lastLogString.Length;
                if (currentLogSize > maxLogFileSize)
                {
                    lastLogString = lastLogString.Substring(currentLogSize - maxLogFileSize);
                }

                // Write current log message
                File.WriteAllText(logFilePath, logMessage, Encoding.UTF8);

                // Append truncated previous log
                File.AppendAllText(logFilePath, lastLogString, Encoding.UTF8);

            }
            catch (Exception ex)
            {
                ShowMessage($"Error: {ex.Message}\nStack Trace: {ex.StackTrace}", 1);
            }

        }



        public static void LogException(Exception ex)
        {
            try
            {
                // Combine paths safely
                string logDirPath = Path.Combine(appFilesPath, "Log");
                string exceptionLogFilePath = Path.Combine(logDirPath, exceptionFilePath);

                // Ensure the log directory exists
                if (!Directory.Exists(logDirPath))
                {
                    Directory.CreateDirectory(logDirPath);
                }

                // Get the innermost exception
                Exception innermostException = ex;
                while (innermostException.InnerException != null)
                {
                    innermostException = innermostException.InnerException;
                }

                // Get stack trace details
                System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace(innermostException, true);
                System.Diagnostics.StackFrame frame = stackTrace.GetFrame(0);
                int lineNumber = frame?.GetFileLineNumber() ?? 0;

                // Prepare exception log details
                string exceptionString = $"Exception: L.no: {lineNumber}, On: {DateTime.Now:yyyy-MM-dd hh:mm tt}{Environment.NewLine}";
                exceptionString += innermostException.Message + Environment.NewLine;
                exceptionString += innermostException.StackTrace + Environment.NewLine;
                exceptionString += "==============================================================================" + Environment.NewLine;

                // Set maximum log file size
                int maxLogFileSize = 1024 * 512;

                // Read previous log content
                string lastLogString = "";
                if (File.Exists(exceptionLogFilePath))
                {
                    lastLogString = File.ReadAllText(exceptionLogFilePath);
                }

                // Truncate the previous log if necessary
                int currentLogSize = lastLogString.Length;
                if (currentLogSize > maxLogFileSize)
                {
                    lastLogString = lastLogString.Substring(currentLogSize - maxLogFileSize);
                }

                // Write the current exception details
                File.WriteAllText(exceptionLogFilePath, exceptionString, Encoding.UTF8);

                // Append the truncated previous log
                File.AppendAllText(exceptionLogFilePath, lastLogString, Encoding.UTF8);
            }
            catch (Exception loggingException)
            {
                // Handle any issues during logging
                ShowMessage($"Error: {loggingException.Message}\nStack Trace: {loggingException.StackTrace}", 1);
            }
        }

        public enum APPR : byte
        {
            UserNo = 0,
            UserID = 1,
            FullNameE_blank = 2,
            EmpID_blank = 3,
            HierarchyLevel = 4,
            LocLib1E_blank = 5,
            LocLib2E_blank = 6,
            LocLib3E_blank = 7,
            LocLib4E_blank = 8,
            LocLib5E_blank = 9,
            RoundOff_blank = 10,
            AtchDocPath_blank = 11,
            AllowFuturePosting_TA = 12,
            Language = 13,
            Arabic = 13,
            //FullNameA = 14,
            CompanyName = 14,    //Denson Added 1205015
            //Empcode = 13,

            ErrTxtPath_blank = 15,
            EFTFilePath_blank = 16,
            EFTPathYN_blank = 17,
            RPTPath_blank = 18,

        }

        public static SecRights GetSecRights(string UserId, string ModuleCode)
        {
            SqlConnection myConnection = new SqlConnection(ConnectionFunctions.GetConnectionString());
            SecRights oSecRights = new SecRights();
            try
            {

                myConnection.Open();
                SqlCommand MyCommand = new SqlCommand("EAF_USP_GetSecRights", myConnection);
                MyCommand.CommandType = CommandType.StoredProcedure;
                MyCommand.Parameters.AddWithValue("@UserId", UserId).SqlDbType = SqlDbType.VarChar;
                MyCommand.Parameters.AddWithValue("@ModuleCode", ModuleCode).SqlDbType = SqlDbType.VarChar;

                SqlDataReader dataReader = MyCommand.ExecuteReader(CommandBehavior.CloseConnection);

                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        if (!dataReader.IsDBNull(dataReader.GetOrdinal("UserID")))
                            oSecRights.UserID = (String)dataReader["UserID"];
                        if (!dataReader.IsDBNull(dataReader.GetOrdinal("ModuleCode")))
                            oSecRights.ModuleCode = (String)dataReader["ModuleCode"];
                        if (!dataReader.IsDBNull(dataReader.GetOrdinal("Read")))
                            oSecRights.Read = (Boolean)dataReader["Read"];
                        if (!dataReader.IsDBNull(dataReader.GetOrdinal("ReadWrite")))
                            oSecRights.ReadWrite = (Boolean)dataReader["ReadWrite"];
                        if (!dataReader.IsDBNull(dataReader.GetOrdinal("Delete")))
                            oSecRights.Delete = (Boolean)dataReader["Delete"];
                        if (!dataReader.IsDBNull(dataReader.GetOrdinal("RepAcc")))
                            oSecRights.RepAcc = (Boolean)dataReader["RepAcc"];
                        if (!dataReader.IsDBNull(dataReader.GetOrdinal("RepInp")))
                            oSecRights.RepInp = (Boolean)dataReader["RepInp"];
                        if (!dataReader.IsDBNull(dataReader.GetOrdinal("RepLF")))
                            oSecRights.RepLF = (Boolean)dataReader["RepLF"];
                        if (!dataReader.IsDBNull(dataReader.GetOrdinal("PrnFrm")))
                            oSecRights.PrnFrm = (Boolean)dataReader["PrnFrm"];
                        if (!dataReader.IsDBNull(dataReader.GetOrdinal("LtrPad")))
                            oSecRights.LtrPad = (Boolean)dataReader["LtrPad"];
                        if (!dataReader.IsDBNull(dataReader.GetOrdinal("AdmRD")))
                            oSecRights.AdmRD = (Boolean)dataReader["AdmRD"];
                        if (!dataReader.IsDBNull(dataReader.GetOrdinal("AdmRW")))
                            oSecRights.AdmRW = (Boolean)dataReader["AdmRW"];
                        if (!dataReader.IsDBNull(dataReader.GetOrdinal("PerRD")))
                            oSecRights.PerRD = (Boolean)dataReader["PerRD"];
                        if (!dataReader.IsDBNull(dataReader.GetOrdinal("PerRW")))
                            oSecRights.PerRW = (Boolean)dataReader["PerRW"];
                        if (!dataReader.IsDBNull(dataReader.GetOrdinal("ErnRD")))
                            oSecRights.ErnRD = (Boolean)dataReader["ErnRD"];
                        if (!dataReader.IsDBNull(dataReader.GetOrdinal("ErnRW")))
                            oSecRights.ErnRW = (Boolean)dataReader["ErnRW"];
                        if (!dataReader.IsDBNull(dataReader.GetOrdinal("LocLib1")))
                            oSecRights.LocLib1 = (String)dataReader["LocLib1"];
                        if (!dataReader.IsDBNull(dataReader.GetOrdinal("LocLib2")))
                            oSecRights.LocLib2 = (String)dataReader["LocLib2"];
                        if (!dataReader.IsDBNull(dataReader.GetOrdinal("LocLib3")))
                            oSecRights.LocLib3 = (String)dataReader["LocLib3"];
                        if (!dataReader.IsDBNull(dataReader.GetOrdinal("LocLib4")))
                            oSecRights.LocLib4 = (String)dataReader["LocLib4"];
                        if (!dataReader.IsDBNull(dataReader.GetOrdinal("LocLib5")))
                            oSecRights.LocLib5 = (String)dataReader["LocLib5"];
                        if (!dataReader.IsDBNull(dataReader.GetOrdinal("SalProfile")))
                            oSecRights.SalProfile = (String)dataReader["SalProfile"];
                        if (!dataReader.IsDBNull(dataReader.GetOrdinal("ApprAuth")))
                            oSecRights.ApprAuth = (String)dataReader["ApprAuth"];
                        if (!dataReader.IsDBNull(dataReader.GetOrdinal("Confirmation")))
                            oSecRights.Confirmation = (Boolean)dataReader["Confirmation"];
                    }
                }
            }
            catch (Exception ex)
            {

                //exception handling
            }
            finally
            {
                if ((myConnection != null))
                {
                    if (myConnection.State != ConnectionState.Closed)
                    {
                        myConnection.Close();
                    }
                }
            }
            return oSecRights;
        }



        public static string HandleNullText(object HashTableItem)
        {
            if (Convert.IsDBNull(HashTableItem))
            {
                //return "0";
                return "NULL";
            }
            else
            {
                if (HashTableItem.ToString().Trim().Length == 0)
                {
                    //return "0";
                    return "";
                }
                else
                {
                    return HashTableItem.ToString().Trim();
                }
            }
        }



        //public static SecRights GetSecRights(string UserId, string ModuleCode)
        //{
        //    SqlConnection myConnection = new SqlConnection(ConnectionFunctions.GetConnectionString());
        //    SecRights oSecRights = new SecRights();
        //    try
        //    {

        //        myConnection.Open();
        //        SqlCommand MyCommand = new SqlCommand("EAF_USP_GetSecRights", myConnection);
        //        MyCommand.CommandType = CommandType.StoredProcedure;
        //        MyCommand.Parameters.AddWithValue("@UserId", UserId).SqlDbType = SqlDbType.VarChar;
        //        MyCommand.Parameters.AddWithValue("@ModuleCode", ModuleCode).SqlDbType = SqlDbType.VarChar;

        //        SqlDataReader dataReader = MyCommand.ExecuteReader(CommandBehavior.CloseConnection);

        //        if (dataReader.HasRows)
        //        {
        //            while (dataReader.Read())
        //            {
        //                if (!dataReader.IsDBNull(dataReader.GetOrdinal("UserID")))
        //                    oSecRights.UserID = (String)dataReader["UserID"];
        //                if (!dataReader.IsDBNull(dataReader.GetOrdinal("ModuleCode")))
        //                    oSecRights.ModuleCode = (String)dataReader["ModuleCode"];
        //                if (!dataReader.IsDBNull(dataReader.GetOrdinal("Read")))
        //                    oSecRights.Read = (Boolean)dataReader["Read"];
        //                if (!dataReader.IsDBNull(dataReader.GetOrdinal("ReadWrite")))
        //                    oSecRights.ReadWrite = (Boolean)dataReader["ReadWrite"];
        //                if (!dataReader.IsDBNull(dataReader.GetOrdinal("Delete")))
        //                    oSecRights.Delete = (Boolean)dataReader["Delete"];
        //                if (!dataReader.IsDBNull(dataReader.GetOrdinal("RepAcc")))
        //                    oSecRights.RepAcc = (Boolean)dataReader["RepAcc"];
        //                if (!dataReader.IsDBNull(dataReader.GetOrdinal("RepInp")))
        //                    oSecRights.RepInp = (Boolean)dataReader["RepInp"];
        //                if (!dataReader.IsDBNull(dataReader.GetOrdinal("RepLF")))
        //                    oSecRights.RepLF = (Boolean)dataReader["RepLF"];
        //                if (!dataReader.IsDBNull(dataReader.GetOrdinal("PrnFrm")))
        //                    oSecRights.PrnFrm = (Boolean)dataReader["PrnFrm"];
        //                if (!dataReader.IsDBNull(dataReader.GetOrdinal("LtrPad")))
        //                    oSecRights.LtrPad = (Boolean)dataReader["LtrPad"];
        //                if (!dataReader.IsDBNull(dataReader.GetOrdinal("AdmRD")))
        //                    oSecRights.AdmRD = (Boolean)dataReader["AdmRD"];
        //                if (!dataReader.IsDBNull(dataReader.GetOrdinal("AdmRW")))
        //                    oSecRights.AdmRW = (Boolean)dataReader["AdmRW"];
        //                if (!dataReader.IsDBNull(dataReader.GetOrdinal("PerRD")))
        //                    oSecRights.PerRD = (Boolean)dataReader["PerRD"];
        //                if (!dataReader.IsDBNull(dataReader.GetOrdinal("PerRW")))
        //                    oSecRights.PerRW = (Boolean)dataReader["PerRW"];
        //                if (!dataReader.IsDBNull(dataReader.GetOrdinal("ErnRD")))
        //                    oSecRights.ErnRD = (Boolean)dataReader["ErnRD"];
        //                if (!dataReader.IsDBNull(dataReader.GetOrdinal("ErnRW")))
        //                    oSecRights.ErnRW = (Boolean)dataReader["ErnRW"];
        //                if (!dataReader.IsDBNull(dataReader.GetOrdinal("LocLib1")))
        //                    oSecRights.LocLib1 = (String)dataReader["LocLib1"];
        //                if (!dataReader.IsDBNull(dataReader.GetOrdinal("LocLib2")))
        //                    oSecRights.LocLib2 = (String)dataReader["LocLib2"];
        //                if (!dataReader.IsDBNull(dataReader.GetOrdinal("LocLib3")))
        //                    oSecRights.LocLib3 = (String)dataReader["LocLib3"];
        //                if (!dataReader.IsDBNull(dataReader.GetOrdinal("LocLib4")))
        //                    oSecRights.LocLib4 = (String)dataReader["LocLib4"];
        //                if (!dataReader.IsDBNull(dataReader.GetOrdinal("LocLib5")))
        //                    oSecRights.LocLib5 = (String)dataReader["LocLib5"];
        //                if (!dataReader.IsDBNull(dataReader.GetOrdinal("SalProfile")))
        //                    oSecRights.SalProfile = (String)dataReader["SalProfile"];
        //                if (!dataReader.IsDBNull(dataReader.GetOrdinal("ApprAuth")))
        //                    oSecRights.ApprAuth = (String)dataReader["ApprAuth"];
        //                if (!dataReader.IsDBNull(dataReader.GetOrdinal("Confirmation")))
        //                    oSecRights.Confirmation = (Boolean)dataReader["Confirmation"];
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        //exception handling
        //    }
        //    finally
        //    {
        //        if ((myConnection != null))
        //        {
        //            if (myConnection.State != ConnectionState.Closed)
        //            {
        //                myConnection.Close();
        //            }
        //        }
        //    }
        //    return oSecRights;
        //}

        public static string GetCompanyProfile()
        {
            string sCmpID = string.Empty;
            string SQry = string.Empty;
            string ErrMsg = string.Empty;


            try
            {
                SQry = "SELECT Val FROM dbo.MasterSetup  WITH (NOLOCK) WHERE code = '15' ";
                if (ConnectionFunctions.Connect_SQLScalar(ref sCmpID, SQry, ref ErrMsg))
                {
                    return sCmpID;
                }

            }
            catch (Exception ex)
            {
            }
            return sCmpID;
        }


        public static DataTable GetErrMast(Int16 TabID)
        {
            string errmsg = "";
            bool RetVal = false;
            DataTable dt = new DataTable();
            if (ConnectionFunctions.Connect_SQLDataTable(ref dt, " Exec EAF_USP_GetErrMast " + TabID, ref errmsg))
            {
                return dt;
            }
            else
            {
                return null;
            }

        }


        public static int GetRequestNo(int dummyValue)
        {
            int l_result = 0;
            DataTable dt = new DataTable();
            string errmsg = "";

            string sqlCommand = "HCMS_GenReqNo";
            SqlParameter[] Params = new SqlParameter[1];
            Params[0] = new SqlParameter("@ViewNo", SqlDbType.Int);
            Params[0].Value = dummyValue;

            if (ConnectionFunctions.Connect_SQLDataTable(ref dt, sqlCommand, ref errmsg, Params, CommandType.StoredProcedure))
            {
                l_result = Convert.ToInt32(dt.Rows[0]["ReqNo"]);
            }

            return l_result;


        }


        public bool GetExtDDFEng(string ModuleTableName, string FieldPrefix, ref Hashtable ExtDDF, string strConn, ref string ErrMsg)
        {

            bool RetVal = true;
            SqlConnection sqlConn = new SqlConnection(strConn);
            try
            {
                SqlDataReader MyReader = null;
                sqlConn.Open();
                SqlCommand MyCommand = new SqlCommand("EXEC WEB_GetExtDDFEng '" + ModuleTableName + "', '" + FieldPrefix + "'", sqlConn);
                MyReader = MyCommand.ExecuteReader();
                if (MyReader.HasRows)
                {
                    MyReader.Read();

                    ExtDDF.Add("TableName", MyReader[0]);
                    ExtDDF.Add("FieldPrefix", MyReader[1]);
                    ExtDDF.Add("FieldTitle", MyReader[2]);
                    ExtDDF.Add("FieldType", MyReader[3]);
                    ExtDDF.Add("DataType", MyReader[4]);
                    ExtDDF.Add("SecondaryTable", MyReader[5]);
                    ExtDDF.Add("SecondaryLink", MyReader[6]);
                    ExtDDF.Add("PrimaryTableLink", MyReader[7]);

                    MyReader.Close();
                }

                MyCommand.Dispose();
                sqlConn.Close();
            }

            catch (Exception Ex)
            {
                RetVal = false;
                ErrMsg = "Function Name : GetExtDDFEng, Error : " + Ex.Message;
            }
            finally
            {
                if (sqlConn.State != 0)
                    sqlConn.Close();
            }

            return RetVal;

        }

        public bool GetJointVariant(ref Hashtable ExtDDF, ref string PlinksValue, ref byte DataType, ref string FPrefixValue, string strConn, ref string ErrMsg)
        {

            bool RetVal = true;
            try
            {
                StringBuilder sQry = new StringBuilder();
                sQry.Append("Select " + (string)ExtDDF["SecondaryTable"] + "." + (string)ExtDDF["FieldPrefix"]);
                sQry.Append(" From " + (string)ExtDDF["SecondaryTable"]);
                sQry.Append(" Where (" + (string)ExtDDF["SecondaryTable"] + "." + (string)ExtDDF["SecondaryLink"] + " = ");

                //Dim Temp As String = sQry.ToString 
                string sFormattedPlinkValue = "";

                switch ((DataType))
                {
                    case 0:
                        sFormattedPlinkValue = Convert.ToString("'" + PlinksValue + "'");
                        break;
                    case 1:
                        sFormattedPlinkValue = Convert.ToString("'" + PlinksValue + "'");
                        break;
                    case 2:
                        sFormattedPlinkValue = Convert.ToString("'" + PlinksValue + "'");
                        break;
                    case 3:
                        if (Convert.ToBoolean(PlinksValue) == false)
                        {
                            sFormattedPlinkValue = "0";
                        }
                        else
                        {
                            sFormattedPlinkValue = "1";
                        }

                        break;
                    case 4:
                        sFormattedPlinkValue = "CONVERT(DATETIME,'" + Convert.ToDateTime(PlinksValue).ToString("yyyy/MM/dd H:mm:ss") + "')";
                        break;
                    case 5:
                        sFormattedPlinkValue = PlinksValue;
                        break;
                    case 6:
                        sFormattedPlinkValue = PlinksValue;
                        break;
                    case 7:
                        sFormattedPlinkValue = PlinksValue;
                        break;
                    case 8:
                        sFormattedPlinkValue = PlinksValue;
                        break;
                    case 9:
                        sFormattedPlinkValue = PlinksValue;
                        break;
                    case 10:
                        sFormattedPlinkValue = PlinksValue;
                        break;
                }

                sQry.Append(sFormattedPlinkValue + ")");

                RetVal = GetColValue("Select Count(Expr1) As Noc From TableStructs Where Expr1 = 'LastModDateTime' And [name] = '" + (string)ExtDDF["SecondaryTable"] + "'", ref FPrefixValue, strConn, ref ErrMsg);
                //Check If LastModDateTime Exosts in the Table 
                if (RetVal == false)
                {
                    return false; // TODO: might not be correct. Was : Exit Try 
                }
                if (Convert.ToInt16(FPrefixValue) == 1)
                {
                    sQry.Append(" AND (" + (string)ExtDDF["SecondaryTable"] + ".LastModDateTime = (Select Max(LastModDateTime) From " + (string)ExtDDF["SecondaryTable"] + " WHERE " + (string)ExtDDF["SecondaryTable"] + "." + (string)ExtDDF["SecondaryLink"] + "=" + sFormattedPlinkValue + "))");
                }

                RetVal = GetColValue(sQry.ToString(), ref FPrefixValue, strConn, ref ErrMsg);
                if (RetVal == false)
                {
                    return false; // TODO: might not be correct. Was : Exit Try 
                }
            }
            catch (Exception Ex)
            {
                RetVal = false;
                ErrMsg = "Function Name : GetJointVariant() , Error : " + Ex.Message;
            }
            return RetVal;
        }


        private bool GetColValue(string sQry, ref string FPrefixValue, string strConn, ref string ErrMsg)
        {
            bool RetVal = true;
            SqlConnection sqlConn = new SqlConnection(strConn);
            try
            {
                SqlCommand sqlCmd = new SqlCommand(sQry, sqlConn);
                sqlConn.Open();
                FPrefixValue = Convert.ToString(sqlCmd.ExecuteScalar());
                sqlCmd.Dispose();
                sqlConn.Close();
            }
            catch (Exception Ex)
            {
                RetVal = false;
                ErrMsg = "Function Name : GetColValue() Error : " + Ex.Message;
            }
            finally
            {
                if (sqlConn.State != 0)
                    sqlConn.Close();
            }
            return RetVal;
        }

        public bool GetResult(string sQry, ref Int16 Result, string strConn, ref string ErrMsg)
        {
            bool RetVal = true;
            SqlConnection sqlConn = new SqlConnection(strConn);
            try
            {
                SqlCommand sqlCmd = new SqlCommand("SET DATEFORMAT ymd " + sQry, sqlConn);
                sqlConn.Open();
                Result = Convert.ToInt16(sqlCmd.ExecuteScalar());
                sqlCmd.Dispose();
                sqlConn.Close();
            }
            catch (Exception Ex)
            {
                RetVal = false;
                ErrMsg = "Function : GetResult, Error : " + Ex.Message;
            }
            finally
            {
                if (sqlConn.State != 0)
                    sqlConn.Close();
            }
            return RetVal;
        }




        public static bool AuditSave(string strTable, string strTrans, string strEC, string strUser, string strRetErr, string strWC, long lTranNo)//Glen New
        {
            bool bRet = true;
            string sMachineName = "", szCompName = "";
            DateTime dtCurr = DateTime.Now;


            try
            {
                szCompName = GetIPAddress; //+ GetBrowserDet();
                string strLocQuery = "";
                string wLoc = "";
                wLoc = strWC;
                string strQuery = "INSERT INTO  AuditTrail ([Table],[Transaction],TransactionNo,EmpCode,UserID,Date,Errors,Flag,WComp,MachineName) VALUES ('" + strTable + "','" + strTrans + "'," + lTranNo + ",'" + strEC + "','" + strUser + "',getdate(),'" + strRetErr + "',0,'" + wLoc + "','" + szCompName + "')";
                if (!ConnectionFunctions.Connect_SQLNonQuery(ref iResult, strQuery, ref errmsg))
                    return false;
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            return true;
        }

        public static bool AuditSave_1(string strTable, string strTrans, string strEC, string strUser, string strRetErr, string strIPAdd, string strWC, String[] userinfo, long lTranNo)//Glen New
        {
            bool bRet = true;
            string sMachineName = "", szCompName = "";
            DateTime dtCurr = DateTime.Now;
            //String[] userinfo = HCMS.Web.AppClass.Common.GetCookie();
            if (userinfo.Length > 4)
            {
                string userno = userinfo[Convert.ToInt16(Common.APPR.UserNo)];
                string userid = userinfo[Convert.ToInt16(Common.APPR.UserID)];
                int m_nLevels = Convert.ToInt16(userinfo[Convert.ToInt16(Common.APPR.HierarchyLevel)]);
            }
            try
            {
                szCompName = strIPAdd + GetBrowserDet();
                string strLocQuery = "";
                string wLoc = "";
                //wLoc = strWC;
                //if (string.IsNullOrEmpty(strWC))
                //{
                //    string lEmpid = "";
                //    ConnectionFunctions.Connect_SQLScalar(ref lEmpid, "Select dbo.GetEmpId('" + strEC + "')", ref errmsg);
                //    wLoc = AppClass.Common.GetLocLib1UsingEmpID(Convert.ToInt32(lEmpid)).ToString();
                //}
                //string strQuery = "INSERT INTO  AuditTrail ([Table],[Transaction],TransactionNo,EmpCode,UserID,Date,Errors,Flag,WComp,MachineName) VALUES ('" + strTable + "','" + strTrans + "'," + lTranNo + ",'" + strEC + "','" + strUser + "','" + dtCurr.ToString("yyyy/MM/dd HH:mm:ss") + "','" + strRetErr + "',0,'" + wLoc + "','" + szCompName + "')";
                string strQuery = "INSERT INTO  AuditTrail ([Table],[Transaction],TransactionNo,EmpCode,UserID,Date,Errors,Flag,WComp,MachineName) VALUES ('" + strTable + "','" + strTrans + "'," + lTranNo + ",'" + strEC + "','" + strUser + "',getdate(),'" + strRetErr + "',0,'" + wLoc + "','" + szCompName + "')";
                if (!ConnectionFunctions.Connect_SQLNonQuery(ref iResult, strQuery, ref errmsg))
                    return false;
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            return true;
        }

        public static string GetIPAddress
        {

            get
            {

                string userIP = null;
                try
                {

                    //16-01-2024 - To Show IPAddress from local system instead of ::1
                    try
                    {
                        var _IPHostEntry = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
                        foreach (System.Net.IPAddress _IPAddress in _IPHostEntry.AddressList)
                        {
                            if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
                            {
                                userIP = _IPAddress.ToString();
                            }
                        }
                    }
                    catch (Exception ex)
                    { }
                    //16-01-2024 - To Show IPAddress from local system instead of ::1

                }
                catch { }
                return userIP;
            }
        }

        public static string GetBrowserDet()
        {
            string browserNV = "";
            try
            {
                ////public HttpRequestBase Request ;
                //System.Web.HttpBrowserCapabilities browser = HttpContext.Current.Request.Browser;
                ////System.Web.HttpBrowserCapabilitiesBase browser = HttpRequestBase.
                //var _checkEdge = HttpContext.Current.Request.UserAgent.IndexOf("Edge");
                //string version = browser.Version;
                //string browserName = System.Text.RegularExpressions.Regex.Replace(browser.Type, @"[0-9\-]", string.Empty);//browser.Type;
                //if (_checkEdge > -1)
                //{
                //    browserName = "Edge";
                //    string[] edgVer = HttpContext.Current.Request.UserAgent.ToString().Split(new string[] { "Edge/" }, StringSplitOptions.None);
                //    if (edgVer.Length >= 1)
                //    {
                //        string[] vers = edgVer[1].Split('.');
                //        version = vers[0];
                //    }
                //}
                //browserNV = "Login From " + browserName + " - " + version;
            }
            catch (Exception ex)
            {
                browserNV = "";
            }

            return browserNV;
        }

        public static bool DecodeAppr(string strApp, ref ArrayList astrApp, ref ArrayList astrAppNo)
        {//Decodes the Approval string passed (ex."@1A1@2A2@3A3@4A4@5A5@6A6@7A7@8A8@9A9@10A10@") and 
            //fills the approval persons in the astrApp array and the level in the astrAppNo array
            Int32 lLen = strApp.Length;
            int nofAppr = 1;
            for (Int32 lLoop = 1; lLoop < lLen; lLoop++)
            {
                string strAppNo = "";
                string strAppCode = "";
                Int32 lCnt = lLoop;
                int lstatusPos = 0;
                for (lCnt = lLoop; lCnt < lLen; lCnt++, lLoop++)
                {
                    char cTmp = strApp[lCnt];
                    if (nofAppr < 10)
                    {
                        if (lstatusPos >= 1)
                        {//not a number, break and pick approval name till next '@'
                            lstatusPos = 0;
                            break;
                        }
                        strAppNo += cTmp;
                        lstatusPos += 1;
                    }
                    else
                    {
                        if (lstatusPos >= 2)
                        {//not a number, break and pick approval name till next '@'
                            lstatusPos = 0;
                            break;
                        }
                        strAppNo += cTmp;
                        lstatusPos += 1;
                    }

                    //if (cTmp < 48 || cTmp > 57)

                }
                for (; lCnt < lLen; lCnt++, lLoop++)
                {
                    char cTmp = strApp[lCnt];
                    if (cTmp == '@')
                    {
                        break;
                    }
                    strAppCode += cTmp;
                }
                astrApp.Add(strAppCode);
                astrAppNo.Add(strAppNo);
            }
            return true;
        }



        public static void GetISLAndISLA(string moduleName, string ISLACode, int viewNo, ref string ISL, ref string ISLA)
        {
            SqlConnection sqlConn = new SqlConnection(ConnectionFunctions.GetConnectionString());
            try
            {
                SqlDataReader MyReader = null;
                sqlConn.Open();

                DataTable dt = new DataTable();
                List<SqlParameter> Params = new List<SqlParameter>();
                Params.Add(new SqlParameter("ModuleName", moduleName));
                Params.Add(new SqlParameter("ISLACode", ISLACode));
                Params.Add(new SqlParameter("ViewNo", viewNo));
                string query = "select DescE ISL, DescA ISLA FROM dbo.ErrorMessage WHERE ModuleName = @ModuleName AND ErrCode = @ISLACode AND FormNo = @ViewNo ";
                SqlCommand myCmd = new SqlCommand(query, sqlConn);
                myCmd.CommandType = CommandType.Text;
                myCmd.Parameters.AddRange(Params.ToArray());

                MyReader = myCmd.ExecuteReader();
                if (MyReader.HasRows)
                {
                    MyReader.Read();

                    ISL = MyReader["ISL"] != DBNull.Value ? MyReader["ISL"].ToString() : "";
                    ISLA = MyReader["ISLA"] != DBNull.Value ? MyReader["ISLA"].ToString() : "";

                    MyReader.Close();
                }

                myCmd.Dispose();

            }
            catch (Exception)
            {

            }
            finally
            {

                if (sqlConn.State != 0)
                    sqlConn.Close();
            }
        }



        public static bool ApprProcessInsertOrUpdate(ApprProcess Object, ref SqlConnection sqlConn, ref SqlTransaction SqlTran)
        {
            bool RowsAffected = false;


            //SqlConnection sqlConn = new SqlConnection(strConn);
            try
            {
                //sqlConn.Open();
                SqlCommand myCmd = new SqlCommand("EAF_USP_ApprProcess_InsertUpdate", sqlConn);
                myCmd.CommandType = CommandType.StoredProcedure;
                myCmd.Transaction = SqlTran;

                myCmd.Parameters.AddWithValue("@Priority", Object.Priority);
                myCmd.Parameters.AddWithValue("@ViewNo", Object.ViewNo);
                myCmd.Parameters.AddWithValue("@ReqNo", Object.ReqNo);
                myCmd.Parameters.AddWithValue("@RequestDate", Object.RequestDate);
                myCmd.Parameters.AddWithValue("@EmpID", Object.EmpID);
                myCmd.Parameters.AddWithValue("@Isl", Object.Isl);
                myCmd.Parameters.AddWithValue("@App", Object.App);
                myCmd.Parameters.AddWithValue("@AppDate", Object.AppDate);
                myCmd.Parameters.AddWithValue("@NoOfAppr", Object.NoOfAppr);
                myCmd.Parameters.AddWithValue("@Status", Object.Status);
                myCmd.Parameters.AddWithValue("@Remarks", Object.Remarks);
                myCmd.Parameters.AddWithValue("@DocAttach", Object.DocAttach);
                myCmd.Parameters.AddWithValue("@OnHold", Object.OnHold);
                myCmd.Parameters.AddWithValue("@HoldUserNo", Object.HoldUserNo);
                myCmd.Parameters.AddWithValue("@Deleted", Object.Deleted);
                myCmd.Parameters.AddWithValue("@Returned", Object.Returned);
                myCmd.Parameters.AddWithValue("@LastModDateTime", Object.LastModDateTime);
                myCmd.Parameters.AddWithValue("@LockedByUser", Object.LockedByUser);
                myCmd.Parameters.AddWithValue("@ReqID", Object.ReqID);
                myCmd.Parameters.AddWithValue("@NextApprAuth", Object.NextApprAuth);
                myCmd.Parameters.AddWithValue("@AsGroup", Object.AsGroup);
                myCmd.Parameters.AddWithValue("@GroupNo", Object.GroupNo);
                myCmd.Parameters.AddWithValue("@Selected", Object.Selected);
                myCmd.Parameters.AddWithValue("@Bypassed", Object.Bypassed);
                myCmd.Parameters.AddWithValue("@ReturnedUserNo", Object.ReturnedUserNo);
                myCmd.Parameters.AddWithValue("@Isla", Object.Isla);
                myCmd.Parameters.AddWithValue("@WFCode", Object.WFCode);
                int val = myCmd.ExecuteNonQuery();
                if (val == 0) { RowsAffected = false; } else { RowsAffected = true; }


            }
            catch (Exception ex)
            {
                RowsAffected = true;
            }
            finally
            {
                //if (sqlConn.State != 0)
                //    sqlConn.Close();
            }



            return RowsAffected;
        }

        public static void ShowMessage(string ErrMsg, int errortype)
        {
            switch (errortype)
            {
                case 1:

                    //ScriptManager.RegisterStartupScript(this, this.GetType(), "error", "alertify.error('" + ErrMsg + "');", true);
                    //MessageBox.Show($"Error: {ex.Message}\nStack Trace: {ex.StackTrace}",
                    //   "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Console.WriteLine(ErrMsg);
                    break;

                case 2:

                    //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "succ", "alertify.success('" + ErrMsg + "');", true);
                    Console.WriteLine(ErrMsg);
                    break;

                case 3:

                    //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "warn", "alertify.warning('" + ErrMsg + "');", true);
                    Console.WriteLine(ErrMsg);
                    break;

            }

        }


        public static decimal? ParseNullableDecimal(object value)
        {
            return decimal.TryParse(value?.ToString(), out var result) ? result : (decimal?)null;
        }

        public static int? ParseNullableInt(object value)
        {
            return int.TryParse(value?.ToString(), out var result) ? result : (int?)null;
        }



        public static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == ' ')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }


        /// <summary>
        /// given  locLib5 get all loclibIds hirearcy and overwrite the loclibID columns for entityRow
        /// add column LastLoc to the entityRow
        ///  
        /// </summary>        
        /// <param name="locLib5"></param>
        /// <param name="entityTable"></param>        
        /// <returns></returns>
        public static string HandleLastLoc(string locLib5, ref DataTable entityTable, int RowNo = 0)
        {
            string errMsg = string.Empty;

            DataRow entityRow = null;
            if (entityTable != null && entityTable.Rows.Count > RowNo)
                entityRow = entityTable.Rows[RowNo];
            else
                return "Error : Empty DataTable";

            String locFullCode = String.Empty;
            bool bRetVal;
            String sQry = "Select dbo.IL_fun_GetLocationFullCode(" + locLib5 + ")";
            bRetVal = ConnectionFunctions.Connect_SQLScalar(ref locFullCode, sQry, ref errMsg);

            String lastLocForWF = "";

            if (bRetVal == false)
            {
                errMsg = "Unable to Retrive Location Hierarchy.";
                return errMsg;
            }

            string[] fullCode = locFullCode.Split('>');
            int il = 1;
            String cLoc = "";
            foreach (string word in fullCode)
            {
                cLoc = "LocLib" + il.ToString();
                if (il > 5)
                {
                    if (!entityTable.Columns.Contains(cLoc))
                        entityTable.Columns.Add(cLoc, typeof(string));
                }
                entityRow[cLoc] = word;
                il++;

                lastLocForWF = word;
            }
            if (!entityTable.Columns.Contains("LastLoc"))
                entityTable.Columns.Add("LastLoc", typeof(string));
            entityRow["LastLoc"] = lastLocForWF;

            if (il <= 5)
            {
                entityRow["LocLib5"] = lastLocForWF;
            }
            return errMsg;
        }

        public static int findLocationNameFirstDiffIndex(string FullName1, string FullName2, string langcode = "en")
        {


            //findFirstDiffIndex function logic not working and giving error for case
            //OCEAN VIEW HOTEL>FOOD & BEVERAGE>F&B SERVICE>OVH>OFFSIDE FOOD TRUCK
            //OCEAN VIEW HOTEL>FOOD & BEVERAGE>F&B SERVICE>OVH>OFFSIDE
            //public static int findFirstDiffIndex(string s1, string s2)
            //{
            //    for (int i = 0; i < Math.Min(s1.Length, s2.Length); i++)
            //        if (s1[i] != s2[i])
            //            return i;

            //    return -1;
            //}



            int diffindx = 0;

            if (langcode == "ar")
            {
                diffindx = findLocationNameFirstDiffIndex_Arabic(FullName1, FullName2);
            }
            else
            {
                diffindx = findLocationNameFirstDiffIndex_English(FullName1, FullName2);
            }


            return diffindx;


        }


        public static int findLocationNameFirstDiffIndex_English(string FullName1, string FullName2)
        {


            //findFirstDiffIndex function logic not working and giving error for case
            //OCEAN VIEW HOTEL>FOOD & BEVERAGE>F&B SERVICE>OVH>OFFSIDE FOOD TRUCK
            //OCEAN VIEW HOTEL>FOOD & BEVERAGE>F&B SERVICE>OVH>OFFSIDE
            //public static int findFirstDiffIndex(string s1, string s2)
            //{
            //    for (int i = 0; i < Math.Min(s1.Length, s2.Length); i++)
            //        if (s1[i] != s2[i])
            //            return i;

            //    return -1;
            //}



            int diffindx = 0;
            int diffatSplitterNo = 0;

            int currindx = -1;
            int occurno = 0;

            string locnamesplitter = ">";
            string errmsg = "";



            try
            {
                string[] Fn1Arry = FullName1.Split(new string[] { locnamesplitter }, StringSplitOptions.None);
                string[] Fn2Arry = FullName2.Split(new string[] { locnamesplitter }, StringSplitOptions.None);

                int minArryLen = Math.Min(Fn1Arry.Length, Fn2Arry.Length);

                //to find the difference start at which location level
                for (int i = 0; i < minArryLen; i++)
                {
                    if (Fn1Arry[i] != Fn2Arry[i])
                    {
                        diffatSplitterNo = i;
                        break;
                    }
                }

                //to find the index of nth ocurrence of splitter (in this case 'n' is 'diffstartfrom')
                while (occurno < diffatSplitterNo)
                {
                    currindx = currindx + 1;
                    currindx = FullName1.IndexOf(locnamesplitter, currindx);
                    occurno = occurno + 1;
                }

                if (currindx != -1)//if current index is -1 
                {
                    diffindx = currindx + 1;
                }


            }
            catch (Exception ex)
            {

                errmsg = ex.Message;
            }



            return diffindx;


        }

        public static int findLocationNameFirstDiffIndex_Arabic(string FullName1, string FullName2)
        {

            //arabic need to be handled, not checked yet

            int diffindx = 0;
            int diffatSplitterNo = 0;

            int currindx = -1;
            int occurno = 0;

            string locnamesplitter = "<";
            string errmsg = "";



            try
            {
                string[] Fn1Arry = FullName1.Split(new string[] { locnamesplitter }, StringSplitOptions.None);
                string[] Fn2Arry = FullName2.Split(new string[] { locnamesplitter }, StringSplitOptions.None);

                int minArryLen = Math.Min(Fn1Arry.Length, Fn2Arry.Length);

                //to find the difference start at which location level
                for (int i = 0; i < minArryLen; i++)
                {
                    if (Fn1Arry[i] != Fn2Arry[i])
                    {
                        diffatSplitterNo = i;
                        break;
                    }
                }

                //to find the index of nth ocurrence of splitter (in this case 'n' is 'diffstartfrom')
                while (occurno < diffatSplitterNo)
                {
                    currindx = currindx + 1;
                    currindx = FullName1.IndexOf(locnamesplitter, currindx);
                    occurno = occurno + 1;
                }

                if (currindx != -1)//if current index is -1 
                {
                    diffindx = currindx + 1;
                }


            }
            catch (Exception ex)
            {

                errmsg = ex.Message;
            }



            return diffindx;


        }



        #region commonTA class functions
        public static bool CheckForAttendanceStatus(int lEmpID, ref SqlConnection Conn, ref string ErrMsg)
        {
            System.DateTime Temp = new System.DateTime(1900, 1, 1);
            return CheckForAttendanceStatus(lEmpID, Temp, ref Conn, ref ErrMsg);
        }

        public static bool CheckForAttendanceStatus(int lEmpID, System.DateTime dt_AttCloseDt, ref SqlConnection Conn, ref string ErrMsg)
        {

            bool RetVal = true;
            SqlDataReader MyReader = null;
            try
            {
                System.DateTime dt_LastPaid = new System.DateTime(1900, 1, 1);
                System.DateTime dt_AttClose = new System.DateTime(1900, 1, 1);
                RetVal = ConnectionFunctions.Connect_SQLDataReader(ref MyReader, "SELECT LastPaidDate,AttCloseDt FROM FinMast WHERE EmpID = " + lEmpID, ref ErrMsg, ref Conn);
                if (RetVal == true)
                {
                    if (MyReader.HasRows)
                    {
                        MyReader.Read();
                        dt_LastPaid = (DateTime)MyReader[0];
                        dt_AttClose = (DateTime)MyReader[1];
                        dt_AttCloseDt = dt_AttClose;

                        if (dt_LastPaid == dt_AttClose)
                        {
                            RetVal = true;
                        }
                        else
                        {
                            RetVal = false;
                        }
                    }
                }

                else
                {

                    RetVal = true;
                }

            }

            catch (Exception ex)
            {
                RetVal = true;
            }
            finally
            {

                if ((MyReader != null))
                {
                    if (!MyReader.IsClosed) MyReader.Close();
                }
            }


            return RetVal;

        }

        #endregion

        #region newcommon class functions
        public static string GetTableStatus(ref string ErrMsg)
        {
            string sQry = string.Empty;
            string sVal = string.Empty;
            bool RetVal = false;

            sQry = "Select IsNull(Val,0) As Val From MasterSetup WHERE Code = '26'";
            RetVal = ConnectionFunctions.Connect_SQLScalar(ref sVal, sQry, ref ErrMsg);
            if (!RetVal)
                ErrMsg = "Warning...Error while retrieving Value from Master Setup \nAborting approval of record";

            return sVal;
        }
        public static string GetEmp_CoProfileCode(long lEmpid)
        {
            string cs_Qry, coprof = "";
            string profcode = "";
            SqlDataReader dr = null;
            Int32 sLoclib1 = GetLocLib1UsingEmpID(Convert.ToInt32(lEmpid));
            profcode = GetCompanyProfileCode(sLoclib1.ToString(), 1);
            return profcode;
            //return GetCompanyProfileCode(cs_Qry, 1);
        }

        public static Int32 GetLocLib1UsingEmpID(Int32 EmpID)
        {
            Int32 fstLocID = 0;
            String ErrMsg = "";
            try
            {
                String sLoc1ID = "0";
                String sQry = "Select  dbo.fun_GetLocLib1WithEmpID(" + EmpID + ")";
                ConnectionFunctions.Connect_SQLScalar(ref sLoc1ID, sQry, ref ErrMsg);

                fstLocID = Convert.ToInt32(sLoc1ID);
            }
            catch (Exception ex)
            {
            }

            return fstLocID;
        }

        public static string GetCompanyProfileCode(string cs_Loclib, short nLevel)
        {
            string sResult = "";
            string ProfCode, sQry;
            sQry = "SELECT ISNULL(ProfileCode,'')  ProfileCode FROM Loclib1 WITH (NOLOCK) WHERE Code ='" + cs_Loclib + "'";
            ConnectionFunctions.Connect_SQLScalar(ref sResult, sQry, ref errmsg);

            ProfCode = sResult.ToString();
            return ProfCode;

        }

        #endregion

        #region HCMS.Common.Utility.General functions
        public static bool IsSLReinintilalization3Years()
        {
            SqlConnection sql = new SqlConnection();
            bool bEnabled = false;
            string sVal = "";
            try
            {
                string sQuery = "";
                sql.ConnectionString = ConnectionFunctions.GetConnectionString();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = sql;
                sql.Open();
                sQuery = "Select IsNull(Val,'') Val from MasterSetup With(nolock) where Code = '83'";
                cmd.CommandText = sQuery;
                sVal = Convert.ToString(cmd.ExecuteScalar());
                if (sVal == "1")
                    bEnabled = true;
            }
            catch (Exception ex)
            {
                return bEnabled;
            }
            finally
            {
                sql.Close();
            }
            return bEnabled;
        }

        #endregion

        public static bool IsUserRightsTemplateEnabled()
        {
            SqlConnection sql = new SqlConnection();
            bool RetVal = false;
            try
            {
                string sQuery = "";
                string sResult = "";
                string ErrMsg = "";
                sQuery = "Select IsNull(Val,'') FROM MasterSetup with(nolock) WHERE code = 69";
                if (!ConnectionFunctions.Connect_SQLScalar(ref sResult, sQuery, ref ErrMsg))
                    return false;
                if (sResult == "1")
                {
                    RetVal = true;
                }
            }
            catch (Exception ex)
            {
                RetVal = false;
            }
            finally
            {
                sql.Close();
            }
            return RetVal;
        }

        public static string RetDefAtt(string strSalProfile, short nAttType)
        {
            string result = "P";
            string strtemp = string.Empty;
            switch (nAttType)
            {
                case 0:
                    strtemp = "SELECT DefAtt FROM SalaryProfile  WITH (NOLOCK) WHERE Code ='" + strSalProfile + "'";
                    break;
                case 1:
                    strtemp = "SELECT NoExTreatAs FROM SalaryProfile  WITH (NOLOCK) WHERE Code = '" + strSalProfile + "'";
                    break;
            }
            ConnectionFunctions.Connect_SQLScalar(ref result, strtemp, ref errmsg);
            if (result == string.Empty)
            {
                result = "P";

            }
            return result;
        }



        public static string GetEmpTicketEveryType(int nEmpId)
        {
            string TEtype = "";
            bool RetVal = false;
            String ErrMsg = "";

            String sQry = "Select [dbo].[fun_GetEmpTicketEveryType](" + nEmpId + ")";
            RetVal = ConnectionFunctions.Connect_SQLScalar(ref TEtype, sQry, ref ErrMsg);

            if (!RetVal)
                TEtype = FixedMembers.DefaultTicketEveryType;//default value month

            return TEtype;
        }

        public static string GetEmpCodeFromEmpID(long nEmpID)
        {
            string sResult = "";
            string stemp = "Select EmpCode from EmpConc with (Nolock) where EmpId = " + nEmpID;
            sResult = "";
            string retval = "";
            if (ConnectionFunctions.Connect_SQLScalar(ref sResult, stemp, ref errmsg))
            {

                if (sResult != "")
                {
                    retval = sResult;
                }
            }
            return retval;
        }


        public static bool AddTktMaster(DateTime dtTktDueDt, DateTime dtLastTktIssDt, long m_lEmpID, DateTime dtRepTktDueDate, string[] userinfo)
        {
            try
            {
                SqlDataReader myreader = null;
                string sQry = "", sEmpnameE = "", m_EmpCode = "", sMessage = "";
                DateTime dtDOB = new DateTime();
                sQry = "SELECT DateOfBirth ,EMpnameE,Empcode FROM Employee  WITH (NOLOCK) WHERE EmpID = " + m_lEmpID;
                if (!ConnectionFunctions.Connect_SQLDataReader(ref myreader, sQry, ref errmsg))
                    return false;
                if (myreader.HasRows)
                {
                    myreader.Read();
                    dtDOB = Convert.ToDateTime(myreader["DateOfBirth"]);
                    sEmpnameE = myreader["EMpnameE"].ToString();
                    m_EmpCode = myreader["Empcode"].ToString();
                    myreader.Close();
                }

                long lRecordNo = 0;
                //Seetha Added 28102021 - To fix add tkt master case
                GenReqNoTkt("TktMaster", ref lRecordNo);

                sQry = "Insert TktMaster Values(" + lRecordNo + "," + m_lEmpID + ",NULL,'" + sEmpnameE + "','" + dtDOB.ToString("MM/dd/yyyy") + "',1,1,'" + dtTktDueDt.ToString("MM/dd/yyyy") + "','" + dtLastTktIssDt.ToString("MM/dd/yyyy") + "',1,0,NULL,10,0,0,NULL,'" + dtRepTktDueDate.ToString("MM/dd/yyyy") + "')";
                if (!ConnectionFunctions.Connect_SQLNonQuery(ref iResult, sQry, ref errmsg))
                    return false;
                sMessage = "Ticket Master Record Added while approving ( Ticket every change as the record was not there in Ticket Master )for Employee with due date  " + dtTktDueDt.ToString("dd/MM/yyyy");
                //if (!ConnectionFunctions.Connect_SQLNonQuery(ref iResult, sQry, ref errmsg))
                //    return false;
                if (!Common.AuditSave_1("TktMaster", "Add Record", m_EmpCode, userinfo[Convert.ToInt16(Common.APPR.UserID)].ToString(), sMessage, "", "", userinfo, 0))
                {

                }

                //return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public static bool GenReqNoTkt(string strTable, ref long lRecNo)
        {
            string strQuery;
            SqlDataReader myreader = null;
            strQuery = "SELECT RecordNo FROM " + strTable + " WHERE RecordNo < 10000000 ORDER BY RecordNo ASC";
            if (!ConnectionFunctions.Connect_SQLDataReader(ref myreader, strQuery, ref errmsg))
                return false;
            if (myreader.HasRows)
            {
                while (myreader.Read())
                {
                    lRecNo = Convert.ToInt32(myreader["RecordNo"]);
                    lRecNo++;
                }
                myreader.Close();

            }
            else
                lRecNo = 1;
            return true;
        }

        public static bool SecurityTemplateUpdate(int TemplID, string Userid, ref SqlCommand MyCommand, ref string errMsg)
        {
            //SqlConnection myConnection = new SqlConnection(ConnectionFunctions.GetConnectionString());
            //SqlTransaction SqlTran = null;
            //myConnection.Open();

            try
            {
                if (Userid.ToUpper() == "ADMIN")
                {
                    errMsg = "1";
                    return true;
                }

                //SqlTran = myConnection.BeginTransaction();
                SqlDataReader dr = null;
                string sQry = "";
                string errmsg = "";

                int LocationTypeVal = 0;
                string StrSalProfileVal = "";
                bool LocTypeSel = false;
                string LocLib5Sel = "";

                sQry = "select TemplateNameE,TemplateNameA,JobTitleCode,LocationType,SalProfile,FilterVal,LocTypeSel,LocLib5 from SecTempMast WITH (NOLOCK) where ID = " + TemplID + "";
                dr = null;
                if (ConnectionFunctions.Connect_SQLDataReader(ref dr, sQry, ref errMsg))
                {
                    if (dr.HasRows)
                    {
                        dr.Read();

                        if (!dr.IsDBNull(dr.GetOrdinal("LocationType")))
                        {
                            LocationTypeVal = Convert.ToInt32(dr["LocationType"].ToString());
                        }
                        if (!dr.IsDBNull(dr.GetOrdinal("SalProfile")))
                        {
                            StrSalProfileVal = dr["SalProfile"].ToString();
                        }
                        if (!dr.IsDBNull(dr.GetOrdinal("LocTypeSel")))
                        {
                            LocTypeSel = Convert.ToBoolean(dr["LocTypeSel"].ToString());
                        }
                        if (!dr.IsDBNull(dr.GetOrdinal("LocLib5")))
                        {
                            LocLib5Sel = dr["LocLib5"].ToString();
                        }

                        dr.Close();
                    }
                }

                string sLocFilter = "";

                if (!LocTypeSel)
                {
                    dr = null;
                    sQry = "";
                    errmsg = "";
                    int Empid = 0;
                    string strLocType = "";
                    sQry = "select Empid from security WITH (NOLOCK) where Userid = '" + Userid + "'";
                    ConnectionFunctions.Connect_SQLDataReader(ref dr, sQry, ref errmsg);
                    if (dr.HasRows)
                    {
                        dr.Read();

                        if (dr.HasRows)
                        {
                            if (!dr.IsDBNull(dr.GetOrdinal("Empid")))
                            {
                                Empid = Convert.ToInt32(dr["Empid"].ToString());
                            }
                        }

                        dr.Close();
                    }

                    sQry = "";
                    dr = null;
                    errmsg = "";
                    sQry = "select dbo.IL_fun_GetLocationFullCodeUsingEmpID(" + Empid + ") as LocType";
                    ConnectionFunctions.Connect_SQLDataReader(ref dr, sQry, ref errmsg);
                    if (dr.HasRows)
                    {
                        dr.Read();
                        if (!dr.IsDBNull(dr.GetOrdinal("LocType")))
                        {
                            strLocType = dr["LocType"].ToString();
                        }
                        dr.Close();
                    }
                    if (string.IsNullOrEmpty(strLocType))
                    {
                        errMsg = "1";
                        return true;
                    }

                    bool LocSelVal = false;


                    string[] ArrLocType = strLocType.Split('>');
                    foreach (string EmpLoc in ArrLocType)
                    {
                        int LocationTypeID = 0;
                        sQry = "";
                        dr = null;
                        errmsg = "";
                        sQry = "select LocationTypeID from Loc_Locations WITH (NOLOCK) where LocLibID = " + EmpLoc + "";
                        ConnectionFunctions.Connect_SQLDataReader(ref dr, sQry, ref errmsg);
                        if (dr.HasRows)
                        {
                            dr.Read();
                            if (!dr.IsDBNull(dr.GetOrdinal("LocationTypeID")))
                            {
                                LocationTypeID = Convert.ToInt32(dr["LocationTypeID"].ToString());
                            }
                            dr.Close();
                        }
                        if (LocationTypeVal.ToString() == LocationTypeID.ToString())
                        {
                            bool RetValLocLV = false;
                            string sQryLocLV = "";
                            DataTable dtLocLV = new DataTable();
                            sQryLocLV = "select LocLibId from [dbo].[GetLeafLocLibId](" + EmpLoc + ")";
                            RetValLocLV = ConnectionFunctions.Connect_SQLDataTable(ref dtLocLV, sQryLocLV, ref errmsg);
                            if (dtLocLV != null && dtLocLV.Rows.Count > 0)
                            {
                                foreach (DataRow row in dtLocLV.Rows)
                                {
                                    //duplicate will oaccure need toc check// benny
                                    sLocFilter += "" + row["LocLibId"].ToString() + "@";
                                }
                            }

                            LocSelVal = true;
                            break;
                        }
                    }

                    if (!LocSelVal)
                    {
                        errMsg = "1";
                        return true;
                    }
                    if (string.IsNullOrEmpty(sLocFilter))
                    {
                        errMsg = "1";
                        return true;
                    }
                }
                else
                {
                    sLocFilter = LocLib5Sel;
                }

                //sLocLibIDList += dr["LocLibID"].ToString();
                int iResult = 0;
                //sQry = "update S set S.[Read] = 0 ,S.[ReadWrite]= 0, S.[Delete] = 0,S.RepAcc = 0, S.RepInp = 0, S.RepLF = 0, S.AdmRD = 0, S.AdmRW = 0, S.PerRD = 0, S.PerRW = 0,S.ErnRD = 0, S.ErnRW = 0,";
                //sQry += " S.LtrPad = 0, S.PrnFrm = 0,S.loclib5 = '' ,S.SalProfile = '' from Secrights S, CSModules CS where  S.ModuleCode = CS.Code And CS.ViewNo Not In (107, 116) AND";
                //sQry += " (Modulecode not between 'CS0005' And 'CS0199' And Modulecode NOT IN ('CS0300', 'CS1000') AND ModuleCode IN (SELECT code FROM dbo.CSModules WHERE MFRYN = 1)) AND S.UserID ='" + Userid + "'";
                sQry = "update S set S.[Read] = 0 ,S.[ReadWrite]= 0, S.[Delete] = 0,S.RepAcc = 0, S.RepInp = 0, S.RepLF = 0, S.AdmRD = 0, S.AdmRW = 0, S.PerRD = 0, S.PerRW = 0,S.ErnRD = 0, S.ErnRW = 0,";
                sQry += "S.LtrPad = 0, S.PrnFrm = 0,S.loclib5 = '' ,S.SalProfile = '' from Secrights S, SecTempRights T where S.ModuleCode =T.ModuleCode and T.TemplID = " + Convert.ToInt32(TemplID) + "";
                sQry += " and S.UserID = '" + Userid + "' and S.Modulecode <> 'CS0300'";
                MyCommand.CommandText = sQry;
                MyCommand.ExecuteNonQuery();

                iResult = 0;
                sQry = "update S set S.[Read] = T.[Read],S.[ReadWrite]= T.[ReadWrite],S.[Delete] = T.[Delete],S.RepAcc = T.RepAcc, S.RepInp = T.RepInp, S.RepLF = T.RepLF,S.AdmRD = T.AdmRD,";
                sQry += " S.AdmRW = T.AdmRW, S.PerRD = T.PerRD, S.PerRW = T.PerRW, S.ErnRD = T.ErnRD, S.ErnRW = T.ErnRW, S.LtrPad = T.LtrPad, S.PrnFrm = T.PrnFrm,S.loclib5 = '" + sLocFilter + "'";
                sQry += " ,S.SalProfile = '" + StrSalProfileVal + "' from Secrights S, SecTempRights T where S.ModuleCode =T.ModuleCode and T.TemplID = " + Convert.ToInt32(TemplID) + "";
                sQry += " and S.UserID = '" + Userid + "' and T.[Read] = 1 and S.Modulecode <> 'CS0300'";
                MyCommand.CommandText = sQry;
                MyCommand.ExecuteNonQuery();

                //Other Libraries

                //AdddedTypeRights
                sQry = "update A set A.[Display] = 0";
                sQry += " from AdddedTypeRights A, SecTempLibRights T where A.AdddedType = T.LibType and T.TemplID = " + Convert.ToInt32(TemplID) + " and T.LibTableName = 'AdddedTypeRights'";
                sQry += " and A.UserID = '" + Userid + "'";
                MyCommand.CommandText = sQry;
                MyCommand.ExecuteNonQuery();

                sQry = "update A set A.[Display] = T.[Display]";
                sQry += " from AdddedTypeRights A, SecTempLibRights T where A.AdddedType = T.LibType and T.TemplID = " + Convert.ToInt32(TemplID) + " and T.LibTableName = 'AdddedTypeRights'";
                sQry += " and A.UserID = '" + Userid + "' and T.[Display] = 1";
                MyCommand.CommandText = sQry;
                MyCommand.ExecuteNonQuery();

                //LoanTypeRights
                sQry = "update A set A.[Display] = 0";
                sQry += " from LoanTypeRights A, SecTempLibRights T where A.LoanType = T.LibType and T.TemplID = " + Convert.ToInt32(TemplID) + " and T.LibTableName = 'LoanTypeRights'";
                sQry += " and A.UserID = '" + Userid + "'";
                MyCommand.CommandText = sQry;
                MyCommand.ExecuteNonQuery();

                sQry = "update A set A.[Display] = T.[Display]";
                sQry += " from LoanTypeRights A, SecTempLibRights T where A.LoanType = T.LibType and T.TemplID = " + Convert.ToInt32(TemplID) + " and T.LibTableName = 'LoanTypeRights'";
                sQry += " and A.UserID = '" + Userid + "' and T.[Display] = 1";
                MyCommand.CommandText = sQry;
                MyCommand.ExecuteNonQuery();

                //LoanReschRights
                sQry = "update A set A.[Display] = 0";
                sQry += " from LoanReschRights A, SecTempLibRights T where A.ReschType = T.LibType and T.TemplID = " + Convert.ToInt32(TemplID) + " and T.LibTableName = 'LoanReschRights'";
                sQry += " and A.UserID = '" + Userid + "'";
                MyCommand.CommandText = sQry;
                MyCommand.ExecuteNonQuery();

                sQry = "update A set A.[Display] = T.[Display]";
                sQry += " from LoanReschRights A, SecTempLibRights T where A.ReschType = T.LibType and T.TemplID = " + Convert.ToInt32(TemplID) + " and T.LibTableName = 'LoanReschRights'";
                sQry += " and A.UserID = '" + Userid + "' and T.[Display] = 1";
                MyCommand.CommandText = sQry;
                MyCommand.ExecuteNonQuery();

                //BankRights
                sQry = "update A set A.[Display] = 0";
                sQry += " from BankRights A, SecTempLibRights T where A.BankType = T.LibType and T.TemplID = " + Convert.ToInt32(TemplID) + " and T.LibTableName = 'BankRights'";
                sQry += " and A.UserID = '" + Userid + "'";
                MyCommand.CommandText = sQry;
                MyCommand.ExecuteNonQuery();

                sQry = "update A set A.[Display] = T.[Display]";
                sQry += " from BankRights A, SecTempLibRights T where A.BankType = T.LibType and T.TemplID = " + Convert.ToInt32(TemplID) + " and T.LibTableName = 'BankRights'";
                sQry += " and A.UserID = '" + Userid + "' and T.[Display] = 1";
                MyCommand.CommandText = sQry;
                MyCommand.ExecuteNonQuery();

                //CmpAcctDetRights
                sQry = "update A set A.[Display] = 0";
                sQry += " from CmpAcctDetRights A, SecTempLibRights T where A.BankCode = T.LibType and T.TemplID = " + Convert.ToInt32(TemplID) + " and T.LibTableName = 'CmpAcctDetRights'";
                sQry += " and A.UserID = '" + Userid + "'";
                MyCommand.CommandText = sQry;
                MyCommand.ExecuteNonQuery();

                sQry = "update A set A.[Display] = T.[Display]";
                sQry += " from CmpAcctDetRights A, SecTempLibRights T where A.BankCode = T.LibType and T.TemplID = " + Convert.ToInt32(TemplID) + " and T.LibTableName = 'CmpAcctDetRights'";
                sQry += " and A.UserID = '" + Userid + "' and T.[Display] = 1";
                MyCommand.CommandText = sQry;
                MyCommand.ExecuteNonQuery();

                //JournalTypeRights
                sQry = "update A set A.[Display] = 0";
                sQry += " from JournalTypeRights A, SecTempLibRights T where A.JournalType = T.LibType and T.TemplID = " + Convert.ToInt32(TemplID) + " and T.LibTableName = 'JournalTypeRights'";
                sQry += " and A.UserID = '" + Userid + "'";
                MyCommand.CommandText = sQry;
                MyCommand.ExecuteNonQuery();

                sQry = "update A set A.[Display] = T.[Display]";
                sQry += " from JournalTypeRights A, SecTempLibRights T where A.JournalType = T.LibType and T.TemplID = " + Convert.ToInt32(TemplID) + " and T.LibTableName = 'JournalTypeRights'";
                sQry += " and A.UserID = '" + Userid + "' and T.[Display] = 1";
                MyCommand.CommandText = sQry;
                MyCommand.ExecuteNonQuery();

                //RecAddDedTypeRights
                sQry = "update A set A.[Display] = 0";
                sQry += " from RecAddDedTypeRights A, SecTempLibRights T where A.RecMast = T.LibType and A.RecSec = T.LibTypeSec and T.TemplID = " + Convert.ToInt32(TemplID) + " and T.LibTableName = 'RecAddDedTypeRights'";
                sQry += " and A.UserID = '" + Userid + "'";
                MyCommand.CommandText = sQry;
                MyCommand.ExecuteNonQuery();

                sQry = "update A set A.[Display] = T.[Display]";
                sQry += " from RecAddDedTypeRights A, SecTempLibRights T where A.RecMast = T.LibType and A.RecSec = T.LibTypeSec and T.TemplID = " + Convert.ToInt32(TemplID) + " and T.LibTableName = 'RecAddDedTypeRights'";
                sQry += " and A.UserID = '" + Userid + "' and T.[Display] = 1";
                MyCommand.CommandText = sQry;
                MyCommand.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                errMsg = AppResources.ErrorOccurredWhileUpdatingTheUserRights;
                return false;
            }
        }



        #region IntegrationSettings functions
        public static DataTable GetIntegrationSetting()
        {
            string errmsg = "";
            bool RetVal = false;
            DataTable dt = new DataTable();

            if (ConnectionFunctions.Connect_SQLDataTable(ref dt, "SELECT * FROM dbo.UFI_IntegrationSettings  WITH (NOLOCK)", ref errmsg))
            {
                return dt;
            }
            else
            {
                LogAction("Intergation Setting Fetch Error, Error:" + errmsg);
                return null;
            }

        }
        public static string GetIntegrationSetting(string code)
        {
            string sVal = string.Empty;
            string SQry = string.Empty;
            string ErrMsg = string.Empty;


            try
            {
                SQry = "SELECT ISNULL(Val,'') as Val FROM dbo.UFI_IntegrationSettings  WITH (NOLOCK) WHERE code = '" + code + "' ";
                if (ConnectionFunctions.Connect_SQLScalar(ref sVal, SQry, ref ErrMsg))
                {
                    return sVal;
                }
                else
                {
                    LogAction("Intergation Setting Fetch Error for Code:" + code + ", Error:" + ErrMsg);
                }

            }
            catch (Exception ex)
            {
                LogAction("Intergation Setting Fetch Error for Code:" + code + ", Error:" + ex.Message);
            }
            return sVal;
        }


        #endregion


        internal static int CreateDBoxIProcessLogEntry(string processName)
        {

            int dboxiprocessid = 0;

            string errorQuery = " INSERT INTO DBOXIProcessLog ([ProcessName],[StartTime]) VALUES (@ProcessName, GETDATE());    ";
            errorQuery += " SELECT SCOPE_IDENTITY() AS LastInsertedId;";

            Dictionary<string, object> parameters = new Dictionary<string, object>    {
                { "@ProcessName", processName },
            };

            string errorMsg = string.Empty;
            object result = ConnectionFunctions.ExecuteScalar(errorQuery, parameters);

            if (result != null)
            {
                dboxiprocessid = Convert.ToInt32(result);
            }

            return dboxiprocessid;


        }
        internal static void LogUFIProcessCompletion(int dboxiprocessid, string strprocessRemarks, bool hasProcessError)
        {
            string errorQuery = " UPDATE DBOXIProcessLog SET Remarks = @Remarks, HasErrors=@HasErrors, EndTime=getdate()  where DBOXIProcessId=@ProcessId;    ";

            Dictionary<string, object> parameters = new Dictionary<string, object>    {
                { "@ProcessId", dboxiprocessid },
                { "@Remarks", strprocessRemarks},
                { "@HasErrors", hasProcessError?"1":"0"}
            };

            string errorMsg = string.Empty;
            bool result = ConnectionFunctions.ExecuteQuery(errorQuery, parameters, ref errorMsg);

            if (!result)
            {
                Common.LogAction($"Failed to Save Process End Info for processID " + dboxiprocessid.ToString() + ". Details: {errorMsg}");
            }
        }
        internal static string EscapeCsvValue(string value)
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

        public static void UpdateRemarksToDBOXIExportProcessLogDetails(int dboxiProcessId, string empCode, string csTransactionNo, string remarks, bool hasErrors = false, string errorText = null)
        {
            string query = @"IF NOT EXISTS (SELECT 1 FROM DBOXIExportProcessLogDetails WHERE DBOXIProcessId = @DBOXIProcessId AND EmpCode = @EmpCode AND CSTransactionNo = @CSTransactionNo)
                BEGIN
                    INSERT INTO DBOXIExportProcessLogDetails (DBOXIProcessId, EmpCode, CSTransactionNo, Remarks, LoggedDate, HasErrors, ErrorText)
                    VALUES (@DBOXIProcessId, @EmpCode, @CSTransactionNo, @Remarks, GETDATE(), @HasErrors, @ErrorText)
                END
                ELSE
                BEGIN
                    UPDATE DBOXIExportProcessLogDetails SET Remarks = ISNULL(Remarks, '') + CHAR(13) + CHAR(10) + @Remarks, HasErrors = @HasErrors,ErrorText = CASE WHEN @ErrorText IS NOT NULL 
                    THEN ISNULL(ErrorText, '') + CHAR(13) + CHAR(10) + @ErrorText ELSE ErrorText END, LoggedDate = GETDATE() WHERE DBOXIProcessId = @DBOXIProcessId AND EmpCode = @EmpCode 
                        AND CSTransactionNo = @CSTransactionNo
                END
                ";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@DBOXIProcessId", dboxiProcessId },
                { "@EmpCode", empCode },
                { "@CSTransactionNo", csTransactionNo },
                { "@Remarks", remarks },
                { "@HasErrors", hasErrors },
                { "@ErrorText", (object)errorText ?? DBNull.Value }
            };

            string errorMsg = string.Empty;
            bool result = ConnectionFunctions.ExecuteQuery(query, parameters, ref errorMsg);

            if (!result)
            {
                Common.LogAction($"Failed to Save Remarks to DBOXIExportProcessLogDetails. Details: {errorMsg}");
            }
        }

        public static void UpdateRemarksToUFIImportProcessLogDetails(int ufiprocessid, string strFileName, string strFileType, string remarks, int nRowNo = 0)
        {
            string errorQuery = " IF NOT EXISTS (Select * from UFIImportProcessLogDetails where UFIProcessId=@UFIProcessId and ISNULL(RowNo,0)=@RowNo)";
            errorQuery += " BEGIN ";
            errorQuery += " INSERT INTO UFIImportProcessLogDetails ([UFIProcessId],[FileName],[RowNo],[FileType],[LoggedDate],[Remarks]) VALUES (@UFIProcessId,@FileName,@RowNo,@FileType, GETDATE(),@Remarks);    ";
            errorQuery += " End ";
            errorQuery += " Else ";
            errorQuery += " BEGIN ";
            errorQuery += " UPDATE UFIImportProcessLogDetails SET  Remarks = Remarks + CHAR(13) + CHAR(10) + @Remarks where UFIProcessId=@UFIProcessId and ISNULL(RowNo,0)=@RowNo;";
            errorQuery += " End ";

            Dictionary<string, object> parameters = new Dictionary<string, object>    {
                { "@UFIProcessId", ufiprocessid },
                { "@FileName", strFileName},
                { "@RowNo", nRowNo },
                { "@FileType", strFileType},
                { "@Remarks", remarks }
            };

            string errorMsg = string.Empty;
            bool result = ConnectionFunctions.ExecuteQuery(errorQuery, parameters, ref errorMsg);

            if (!result)
            {
                Common.LogAction($"Failed to Save Remarks to UFIImportProcessLogDetails. Details: {errorMsg}");
            }
        }


        public static void LogErrorToDBOXIErrorLog(int processid, int processdetailId, string empCode, string errorInfo, string errorText)
        {
            string errorQuery = @"
        INSERT INTO DBOXIErrorLog (DBOXIProcessId, PLD_SrNo, EmpCode, ErrorInfo, ErrorText, LoggedDate)
        VALUES (@DBOXIProcessId, @PLD_SrNo, @EmpCode,@ErrorInfo, @ErrorText, GETDATE());
    ";

            Dictionary<string, object> parameters = new Dictionary<string, object>
    {
        { "@DBOXIProcessId", processid },
        { "@PLD_SrNo", processdetailId },
        { "@EmpCode", empCode },
        { "@Errorinfo", errorInfo },
        { "@ErrorText", errorText }
    };

            string errorMsg = string.Empty;
            bool result = ConnectionFunctions.ExecuteQuery(errorQuery, parameters, ref errorMsg);

            if (!result)
            {
                Common.LogAction($"Failed to log error to DBOXIErrorLog. Details: {errorMsg}");
            }
        }

        public static void UpdateErrorSeverityForService(ref DataTable errTable, short tabid)
        {
            string sCmpID = string.Empty;
            string SQry = string.Empty;
            string ErrMsg = string.Empty;


            try
            {
                DataTable dt = new DataTable();
                SQry = "SELECT * FROM dbo.DBOXI_ErrMastSeverity  WITH (NOLOCK) WHERE TabId='" + tabid + "'";
                if (!ConnectionFunctions.Connect_SQLDataTable(ref dt, SQry, ref ErrMsg))
                {
                    throw new Exception(ErrMsg);
                }

                foreach (DataRow drow in errTable.Rows)
                {
                    string errcode = drow["Code"].ToString().ToUpper();
                    DataRow[] filterrows = dt.Select("Code='" + errcode + "'");
                    if (filterrows != null && filterrows.Length > 0)
                    {
                        drow["Severity"] = filterrows[0]["Severity"];
                    }
                }

            }
            catch (Exception ex)
            {
                LogAction("UpdateErrorSeverityForService Error:" + ex.Message);
            }

        }


        public static DataTable GetProcessError(int dboxiProcessId)
        {
            string errmsg = "";
            bool RetVal = false;
            DataTable dt = new DataTable();
            if (ConnectionFunctions.Connect_SQLDataTable(ref dt, "select * from DBOXIErrorLog where  DBOXIProcessId=" + dboxiProcessId, ref errmsg))
            {
                return dt;
            }
            else
            {
                return null;
            }

        }
        public static DataTable GetProcessLog(int dboxiProcessId)
        {
            string errmsg = "";
            bool RetVal = false;
            DataTable dt = new DataTable();
            if (ConnectionFunctions.Connect_SQLDataTable(ref dt, "select * from [DBOXIProcessLog] where  DBOXIProcessId=" + dboxiProcessId, ref errmsg))
            {
                return dt;
            }
            else
            {
                return null;
            }

        }
        public static DataTable GetProcessLogDetails(int dboxiProcessId)
        {
            string errmsg = "";
            bool RetVal = false;
            DataTable dt = new DataTable();
            if (ConnectionFunctions.Connect_SQLDataTable(ref dt, "select * from [DBOXIImportProcessLogDetails] where  DBOXIProcessId=" + dboxiProcessId, ref errmsg))
            {
                return dt;
            }
            else
            {
                return null;
            }

        }
        public static bool IsExist(string TableName, string Value, string Filter)
        {
            string errmsg = "";
            bool RetVal = false;
            string strCode = "";
            int fil = 0;
            if (ConnectionFunctions.Connect_SQLScalar(ref fil, "select 1 from " + TableName + " where  " + Filter + "= '" + Value + "'", ref errmsg))
            {
                if (fil > 0)
                {

                    return true;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return false;
            }
        }

    }
}
