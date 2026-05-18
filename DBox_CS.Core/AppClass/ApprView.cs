using Microsoft.VisualBasic;

using System.Text;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web;
using System.Collections;
using DBox_CS.Core.DALayer;

namespace DBox_CS.Core.AppClass
{
    public class ApprView
    {

        public enum ApprBox : byte
        {

            In = 1,
            Out = 2,
            Returned = 3,
            Hold = 4
        }


        public enum Modules
        {
            Employee = 1,
            Visitor = 16,
            Family = 46,
            Sponsor = 61,
            Leave_Requistions = 101,
            Leave_Cancellation = 103,
            Leaving = 106,
            Rejoining = 107,
            Leave_Processing = 110,
            Financial_New = 116,
            Financial_Current = 117,
            Employee_Journal = 131,
            Payments_In_Advance = 191,
            Training = 200,
            Exception = 235,
            Payroll_Processing = 250,
            Addition_Deduction = 265,
            ConfirmPayments = 266,
            Over_Time = 280,
            End_Of_Service = 295,
            EOS_Processing = 300,
            UnFreezing_Employee = 310,
            PayMode_Details = 325,
            Ticket_Requisition = 341,
            EmpHold = 350,
            EmpHold_Retrieve = 351,
            Warning = 355,
            WorkAgreement_Details = 375,
            StandardRate_OverTime = 380,
            Budget = 403,
            Performance_Appraisal = 410,
            Deputation = 600,
            Employee_Modify_Details = 750,
            AuditTrail = 751,
            Location_Library = 1000,
            TimeKeeping = 1003,
            Loan_Rescheduling_Hold = 1006,
            Housing_Details = 1009,
            Bonus = 1010,
            Settlement_View = 2000,
            SettlementProcessing = 2005,
            TA = 5001,
            Cost_Center_Setup = 5002,
            AST = 5003,
            EmpBals = 5004,
            JV_Setup = 5005,
            Attendance = 5006,
            Salary_Slip = 5007,
            Ticket_Master = 6000,
            NotificationManager = 6002,
            NotificationServer = 6003,
            Get_Approval_Id = 6004,
            Staff_Travel_Request = 7000,
            Staff_Travel_Ticket_Confirmation = 7200,
            Staff_Travel_Setup = 7300,
            Duty_Travel = 7400,
            Staff_Travel_Deduction = 7500,
            Staff_Travel_Refund = 7600
        }


        public static bool GetModulesOnRights(ref string[] UserInfo, ref ArrayList arModules, ref ArrayList lstModules, ref SqlConnection sqlConn)
        {

            bool RetVal = false;
            string ErrMsg = string.Empty;
            SqlDataReader MyReader = null;
            try
            {

                DataTable astTable = new DataTable();
                //---------------------------------Checking how many ASTs Current User has got.------------------------------------------------------------ 
                string MySQL = "SELECT ApprAuth, ModuleCode FROM SecRights WHERE (ModuleCode = 'CS0300' AND UserID = '" + UserInfo.GetValue(Convert.ToInt16(Common.APPR.UserID)) + "') " + "And ( dbo.IsEmptyVarchar ( convert (varchar(8000) , LocLib" + UserInfo.GetValue(Convert.ToInt16(Common.APPR.HierarchyLevel)) + " )) IS NOT NULL ) And ( dbo.IsEmptyVarchar " + "( convert (varchar(8000) , SalProfile)) IS NOT NULL ) And ( dbo.IsEmptyVarchar ( ApprAuth ) IS NOT NULL )";

                RetVal = ConnectionFunctions.Connect_SQLDataTable(ref astTable, MySQL, ref sqlConn, null, ref ErrMsg);
                if (RetVal == false)
                {
                    return false; // TODO: might not be correct. Was : Exit Try 
                }

                string ModuleName = string.Empty;
                for (Int16 i = 0; i <= astTable.Rows.Count - 1; i++)
                {
                    //Checking how many ASTs Current User has got. 
                    MySQL = "SELECT DISTINCT ViewNo FROM WrkFlowMast WHERE ('@' + CONVERT(varchar(8000),AuthPerson) " + "LIKE '%%@" + astTable.Rows[i]["ApprAuth"] + "@%%') ORDER BY ViewNo";
                    DataTable viewTable = new DataTable();
                    RetVal = ConnectionFunctions.Connect_SQLDataTable(ref viewTable, MySQL, ref sqlConn, null, ref ErrMsg);
                    if (RetVal == false)
                    {
                        return false; // TODO: might not be correct. Was : Exit Try 
                    }

                    for (Int16 ctr = 0; ctr <= viewTable.Rows.Count - 1; ctr++)
                    {
                        ModuleName = GetModuleNamefromViewNo(Convert.ToInt16(viewTable.Rows[ctr]["ViewNo"]), ref sqlConn, (UserInfo.GetValue(Convert.ToInt16(Common.APPR.Language)).ToString()));
                        if (lstModules.Contains(ModuleName) == false)
                        {
                            lstModules.Add(ModuleName);
                            arModules.Add(viewTable.Rows[ctr]["ViewNo"] + "@" + ModuleName);
                        }
                    }
                    viewTable.Dispose();
                }
                astTable.Dispose();
            }
            catch (Exception ex)
            {
                RetVal = false;
            }

            return RetVal;

        }

        public static bool CheckForReturnedReq(ref string[] UserInfo, ref ArrayList arModules, ref ArrayList lstModules, ref SqlConnection sqlConn)
        {

            bool RetVal = false;
            string ErrMsg = string.Empty;
            try
            {

                DataTable astTable = new DataTable();
                string MySQL = " SELECT CSModules.ModuleName, CSModules.ViewNo FROM ApprProcess INNER JOIN CSModules ON ApprProcess.ViewNo = CSModules.ViewNo ";
                MySQL += "INNER JOIN Security ON ApprProcess.ReqID = Security.UserNo WHERE (ApprProcess.Returned <> 0) AND (ApprProcess.Deleted = 0) AND ";
                MySQL += "(ApprProcess.OnHold = 0) AND (Security.UserID = '" + UserInfo.GetValue(Convert.ToInt16(Common.APPR.UserID)) + "') Group By CSModules.ModuleName, CSModules.ViewNo";
                RetVal = ConnectionFunctions.Connect_SQLDataTable(ref astTable, MySQL, ref sqlConn, null, ref ErrMsg);
                if (RetVal == false)
                {
                    return false; // TODO: might not be correct. Was : Exit Try 
                }

                string ModuleName = string.Empty;
                for (Int16 i = 0; i <= astTable.Rows.Count - 1; i++)
                {
                    ModuleName = astTable.Rows[i]["ModuleName"].ToString();
                    if (lstModules.Contains(ModuleName) == false)
                    {
                        lstModules.Add(ModuleName);
                        arModules.Add(astTable.Rows[i]["ViewNo"] + "@" + ModuleName);
                    }
                }
                astTable.Dispose();
            }
            catch (Exception ex)
            {
                RetVal = false;
            }

            return RetVal;

        }

        public static bool CheckForOnHoldReq(ref string[] UserInfo, ref ArrayList arModules, ref ArrayList lstModules, ref SqlConnection sqlConn)
        {

            bool RetVal = false;
            string ErrMsg = string.Empty;
            try
            {

                DataTable astTable = new DataTable();
                string MySQL = " SELECT CSModules.ModuleName, CSModules.ViewNo FROM ApprProcess INNER JOIN CSModules ON ApprProcess.ViewNo = CSModules.ViewNo ";
                MySQL += "WHERE (ApprProcess.Returned = 0) AND (ApprProcess.Deleted = 0) AND (ApprProcess.OnHold <> 0) AND (ApprProcess.Status = 0) AND ";
                MySQL += "(ApprProcess.HoldUserNo = " + UserInfo.GetValue(Convert.ToInt16(Common.APPR.UserNo)) + ") Group By CSModules.ModuleName, CSModules.ViewNo";
                RetVal = ConnectionFunctions.Connect_SQLDataTable(ref astTable, MySQL, ref sqlConn, null, ref ErrMsg);
                if (RetVal == false)
                {
                    return false; // TODO: might not be correct. Was : Exit Try 
                }

                string ModuleName = string.Empty;
                for (Int16 i = 0; i <= astTable.Rows.Count - 1; i++)
                {
                    ModuleName = astTable.Rows[i]["ModuleName"].ToString();
                    if (lstModules.Contains(ModuleName) == false)
                    {
                        lstModules.Add(ModuleName);
                        arModules.Add(astTable.Rows[i]["ViewNo"] + "@" + ModuleName);
                    }
                }
                astTable.Dispose();
            }
            catch (Exception ex)
            {
                RetVal = false;
            }

            return RetVal;

        }

        public static bool CheckForBypassedReq(ref string[] UserInfo, ref ArrayList arModules, ref ArrayList lstModules, ref SqlConnection sqlConn)
        {

            bool RetVal = false;
            string ErrMsg = string.Empty;
            try
            {

                DataTable astTable = new DataTable();
                string MySQL = " SELECT CSModules.ModuleName, CSModules.ViewNo FROM ApprProcess INNER JOIN CSModules ON ApprProcess.ViewNo = CSModules.ViewNo ";
                MySQL += "INNER JOIN Security ON ApprProcess.ReqID = Security.UserNo WHERE (ApprProcess.Returned = 0) AND (ApprProcess.Deleted = 0) AND ";
                MySQL += "(ApprProcess.OnHold = 0) AND (ApprProcess.Status >= 0) AND (ApprProcess.ByPassed <> 0) AND (Security.UserID = '" + UserInfo.GetValue(Convert.ToInt16(Common.APPR.UserID)) + "') Group By CSModules.ModuleName, CSModules.ViewNo";
                RetVal = ConnectionFunctions.Connect_SQLDataTable(ref astTable, MySQL, ref sqlConn, null, ref  ErrMsg);
                if (RetVal == false)
                {
                    return false; // TODO: might not be correct. Was : Exit Try 
                }

                string ModuleName = string.Empty;
                for (Int16 i = 0; i <= astTable.Rows.Count - 1; i++)
                {
                    ModuleName = astTable.Rows[i]["ModuleName"].ToString();
                    if (lstModules.Contains(ModuleName) == false)
                    {
                        lstModules.Add(ModuleName);
                        arModules.Add(astTable.Rows[i]["ViewNo"] + "@" + ModuleName);
                    }
                }
                astTable.Dispose();
            }
            catch (Exception ex)
            {
                RetVal = false;
            }

            return RetVal;

        }

        public static bool CheckForDeputedModules(ref string[] UserInfo, ref ArrayList arModules, ref ArrayList lstModules, ref ArrayList DeputedModules, ref SqlConnection sqlConn)
        {

            bool RetVal = false;
            string ErrMsg = string.Empty;
            SqlDataReader MyReader = null;
            try
            {
                //Query to check whether the current date lies between the two deputation dates 
                DataTable DepTable = new DataTable();
                string UserID = string.Empty;
                Int16 ReqNo = 0;
                Int16 ReqID = 0;

                string MySQL = null;
                MySQL = "SELECT ReqNo, ReqID FROM DeputationMast ";
                MySQL += "WHERE (StartDate <= '" + Convert.ToDateTime(DateTime.Now.Date).ToString("yyyy/MM/dd") + "' AND EndDate";
                MySQL += " >= '" + DateTime.Now.Date.ToString("yyyy/MM/dd") + "') AND (Status >= 20 AND Status < 40)";
                RetVal = ConnectionFunctions.Connect_SQLDataTable(ref DepTable, MySQL, ref sqlConn, null, ref ErrMsg);

                string ModuleName = string.Empty;
                for (Int16 i = 0; i <= DepTable.Rows.Count - 1; i++)
                {
                    ReqNo = Convert.ToInt16(DepTable.Rows[i]["ReqNo"]);
                    ReqID = Convert.ToInt16(DepTable.Rows[i]["ReqID"]);
                    MySQL = "SELECT DISTINCT ReqNo, CSModules.ModuleName, CSModules.ViewNo, SelectedByCode FROM DeputationSec INNER JOIN CSModules ON DeputationSec.ViewNo = CSModules.ViewNo ";
                    MySQL += "Where ReqNo = " + ReqNo + " AND SelectedByCode = '" + UserInfo.GetValue(Convert.ToInt16(Common.APPR.UserID)) + "'";
                    RetVal = ConnectionFunctions.Connect_SQLDataReader(ref MyReader, MySQL, ref ErrMsg, ref sqlConn);
                    while (MyReader.Read())
                    {
                        ModuleName = Convert.ToString(MyReader["ModuleName"]);
                        DeputedModules.Add(ModuleName);
                        if (lstModules.Contains(ModuleName) == false)
                        {
                            lstModules.Add(ModuleName);
                            arModules.Add(Convert.ToString(MyReader["ViewNo"]) + "@" + ModuleName);
                        }
                    }
                    MyReader.Close();
                }

                DepTable.Dispose();
            }

            catch (Exception ex)
            {
                RetVal = false;
            }

            return RetVal;

        }

        protected static string GetModuleNamefromViewNo(Int16 ViewNo, ref SqlConnection sqlConn, string LanguageType = "0")
        {

            bool RetVal = false;
            string ErrMsg = string.Empty;
            string ModuleName = string.Empty;
            try
            {
                if (Common.ModulesTable == null || Common.ModulesTable.Count==0)
                {

                    Hashtable ModulesTable = new Hashtable();
                    SqlDataReader MyReader = null;
                    string SqlModuleName = string.Empty;                    
                    if(LanguageType == "1")
                        SqlModuleName = " CSModules.ModuleNameA";
                    else
                    SqlModuleName = " CSModules.ModuleName ";

                    RetVal = ConnectionFunctions.Connect_SQLDataReader(ref MyReader, "SELECT ViewNo, " + SqlModuleName +" As ModuleName FROM CSModules", ref ErrMsg, null, ref sqlConn, CommandType.Text);
                    while (MyReader.Read())
                    {
                        ModulesTable.Add(MyReader["ViewNo"], MyReader["ModuleName"]);
                    }
                    MyReader.Close();

                   
                    Common.ModulesTable = ModulesTable;
                }
                ModuleName = Common.ModulesTable[ViewNo].ToString();
            }
            catch (Exception Ex)
            {
            }

            return ModuleName;

        }

        public static string GetModuleTablefromViewNo(int ViewNo, ref SqlConnection sqlConn)
        {

            bool RetVal = false;
            string ErrMsg = string.Empty;
            string ModuleTable = string.Empty;
            try
            {
                SqlParameter[] P = null;
                RetVal = ConnectionFunctions.Connect_SQLScalar(ref ModuleTable, "SELECT ModuleTable FROM CSModules WHERE ViewNo = " + ViewNo.ToString(), ref P, ref sqlConn, ref ErrMsg);
            }
            catch (Exception Ex)
            {
            }

            return ModuleTable;

        }

        //29-06-2022: Robin added Code , copied from HCMS\Areas\EApproval\Old_App_Code\ApprView.vb
        public static string ShowErrorMessage(string LangType, string ErrCode)
        {
            string ErrMsg = string.Empty;
            string ErrorMessage = string.Empty;
            bool RetVal = false;
            SqlDataReader MyReader = null/* TODO Change to default(_) if this is not a reference type */;

            string lblCaption;
            if (LangType == "1")
                lblCaption = "DescA as DescE";
            else
                lblCaption = "DescE";
            if (ConnectionFunctions.Connect_SQLDataReader(ref MyReader, "SELECT " + lblCaption + " FROM ErrorMessage  WITH (NOLOCK)  WHERE ErrCode = '" + ErrCode + "'", ref ErrMsg))
            {
                if (MyReader.HasRows)
                {
                    while ((MyReader.Read()))
                        ErrorMessage = MyReader[0].ToString();
                }
            }
            return ErrorMessage;
        }
        public static string GetLanguageType()
        {
            string[] Userinfo = Common.UserInfo;
            string LanguageType = string.Empty;
            if (Userinfo.Length < (int)Common.APPR.Language)
                LanguageType = "0";
            else
                LanguageType = Userinfo.GetValue((int)Common.APPR.Language).ToString();
            return LanguageType;
        }

        public static string GetCompanyProfile()
        {
            string[] Userinfo = Common.UserInfo;
            string sCmpID = string.Empty;
            if (Userinfo.Length >= (int)Common.APPR.CompanyName)
                sCmpID = Userinfo.GetValue((int)Common.APPR.CompanyName).ToString();
            
            if(string.IsNullOrEmpty(sCmpID))
                sCmpID = Common.GetCompanyProfile();

            return sCmpID;
        }

        //End: Robin added Code , copied from HCMS\Areas\EApproval\Old_App_Code\ApprView.vb



    }
}
