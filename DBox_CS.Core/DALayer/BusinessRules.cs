using System;
using System.Data;
using System.Text;
using System.Collections;
using System.Data.SqlClient;
using System.Web;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DBox_CS.Core.HCMS.Entity;
using DBox_CS.Core.AppClass;

namespace DBox_CS.Core.DALayer
{
    public class BusinessRules
    {

        //private static bool bFirstTime;
        private static string sBuf = string.Empty;
        private static bool bFirstTime;




        public static bool CheckBusinessRule(Int16 ViewNo, DataTable TranTable, string ModuleTableName, string MainColumnName, DataTable ErrorsTable, out Dictionary<String, String> Errors)
        {
            bool RetVal = true;
            Errors = new Dictionary<String, String>();
            try
            {
                string FieldPrefix = string.Empty;
                string FPrefixValue = string.Empty;
                string ValidExpr = string.Empty;
                string WhatToReplace = string.Empty;
                ArrayList DFields = default(ArrayList);
                ExtDDFEng ExtDDF = null;
                ExtDDFEng ExtDDFLnk = null;
                string[] FPrefix = null;
                string sQry = null;
                string sFormattedColValue = null;
                short Ctr = 0;
                short i = 0;
                short ErrorRecords = 0;
                short Result = 0;
                DFields = new ArrayList();

                DataView ErrorView = new DataView(ErrorsTable);

                ErrorView.RowFilter = "SysCoded in(0,3)";//DHanesh Modified to include  Civilsoft defined 18-12-2012
                //-----------ErrType = 0 AND 
                ErrorRecords = Convert.ToInt16(ErrorView.Count - 1);
                //(ErrorsTable.Rows.Count()) - 1 

                int TranCount = 0;
                string ErrorCodes = null;
                int ErrorCount = 0;

                for (TranCount = 0; TranCount <= TranTable.Rows.Count - 1; TranCount++)
                {

                    //=========================Loop for all rows in transaction table=================== 
                    ErrorCount = 0;
                    ErrorCodes = string.Empty;


                    //===Validate All Hard Coded Business Rules (CS0002d.dll in HRP)===

                    {
                        //Nishad Added 30052016
                        String ErrMsg = String.Empty;
                        switch (ViewNo)
                        {
                            //case 101:
                            //    RetVal = LeaveS(TranTable.Rows[TranCount], ref ErrorCodes, ref ErrMsg);
                            //    if (RetVal == false)
                            //    {
                            //        break; // TODO: might not be correct. Was : Exit Try
                            //    }
                            //    break;
                            //case 191:
                            //    LoanReqS(TranTable.Rows[TranCount], ref ErrorCodes, ref ErrMsg);
                            //    break;
                            case 116:
                                {
                                    //FinancialS(116, TranTable.Rows[TranCount], ref ErrorCodes, ref ErrMsg, "", "");

                                    //Seetha added 19122021 - Check business rule based on financial page tab rights changes
                                    String userID = String.Empty, modCode = string.Empty;
                                    int reqId = (TranTable.Rows[TranCount]["ReqID"] != null) ? Convert.ToInt32(TranTable.Rows[TranCount]["ReqID"]) : 0;
                                    RetVal = ConnectionFunctions.Connect_SQLScalar(ref userID, "SELECT UserId FROM Security WITH (NOLOCK) WHERE UserNo = " + reqId, ref ErrMsg);

                                    if (RetVal && !string.IsNullOrEmpty(userID))
                                    {
                                        if (ViewNo == 116)
                                            modCode = "CS0200";
                                        else if (ViewNo == 117)
                                            modCode = "CS0210";

                                        SecRights oSecRights = Common.GetSecRights(userID, modCode);

                                        string filterCode = string.Empty, filterCodeStart = string.Empty, filterCodeEnd = string.Empty;
                                        bool isFilterCodeApplicable = false;

                                        filterCodeStart = " Code in ( ";
                                        filterCodeEnd = " )";
                                        //Location tab rights
                                        if (oSecRights.PerRW)
                                        {
                                            isFilterCodeApplicable = true;
                                            filterCode = " 'FT0057','FT0058','FT0059','FT0065','FT0068','FT0072','FT0073' ";
                                        }

                                        //Ticket tab rights
                                        if (oSecRights.ErnRW)
                                        {
                                            isFilterCodeApplicable = true;
                                            if (!string.IsNullOrEmpty(filterCode))
                                            {
                                                filterCode = filterCode + " ,'FT0070' ";
                                            }
                                            else
                                            {
                                                filterCode = " 'FT0070' ";
                                            }

                                            //28-03-2023: Robin added code for YAS include ticket Rule "Only 0 , 12 or 24 months allowed for Employee/Family Ticket Every" as the rule was being not checked for new Fin req case
                                            string sCmpID = Common.GetCompanyProfile().ToUpper();
                                            if (sCmpID == "RADYAS")
                                            {
                                                //filterCode = filterCode + " ,'UFT002','UFT003' "; //Nishad Edited 22062023
                                                filterCode = filterCode + " ,'UFT002','UFT003','UFT006' ";
                                            }
                                            //End: Robin added code

                                        }

                                        //Nishad Added 04012024
                                        string sCMP = Common.GetCompanyProfile().ToUpper();
                                        if (sCMP == "SME")
                                        {
                                            filterCode = filterCode + " ,'UFT007' ";
                                        }
                                        //Nishad End 04012024

                                        if (isFilterCodeApplicable)
                                        {
                                            ErrorView.RowFilter = filterCodeStart + filterCode + filterCodeEnd;
                                            ErrorRecords = Convert.ToInt16(ErrorView.Count - 1);
                                        }

                                        FinancialS(116, TranTable.Rows[TranCount], ref ErrorCodes, ref ErrMsg, "", "", oSecRights.AdmRW, oSecRights.PerRW, oSecRights.ErnRW);
                                    }
                                    else
                                    {
                                        FinancialS(116, TranTable.Rows[TranCount], ref ErrorCodes, ref ErrMsg, "", "", true, true, true);
                                    }

                                    break;
                                }
                        }

                        if ((ErrorCodes.Trim().Length) > 0)
                        {
                            ErrorCount = 1;
                        }
                        //Nishad End 30052016
                    }

                    for (i = 0; i <= ErrorRecords; i++)
                    {

                        //Put All the Distinct Field Prefices in an array 
                        FieldPrefix = Common.HandleNullText(ErrorView[i]["DistinctFldPrefix"]);
                        FieldPrefix = FieldPrefix.Substring(0, FieldPrefix.Length - 1);
                        FPrefix = FieldPrefix.Split(Convert.ToChar("@"));
                        DFields.InsertRange(0, FPrefix);

                        //Get The Valid expression 
                        ValidExpr = Common.HandleNullText(ErrorView[i]["ValidExprn"]);

                        //Loop For Each Distinct Field Prefix in the Array [DFields] 
                        for (Ctr = 0; Ctr <= Convert.ToInt16(DFields.Count - 1); Ctr++)
                        {
                            ExtDDF = new ExtDDFEng();
                            ExtDDF = GetOneByTableNameAndFieldPrefix(ModuleTableName, DFields[Ctr].ToString());

                            if (ExtDDF.FieldType <= 7 || ExtDDF.FieldPrefix.Contains("LocLib"))
                            {

                                FPrefixValue = Common.HandleNullText(TranTable.Rows[TranCount][DFields[Ctr].ToString()]);
                                //WhatToReplace = ExtDDF.TableName + "." + ExtDDF.FieldPrefix;
                                //#NEWLOC
                                if (ExtDDF.FieldType <= 7)
                                {
                                    WhatToReplace = ExtDDF.TableName + "." + ExtDDF.FieldPrefix;
                                }
                                if (ExtDDF.FieldType >= 14 && ExtDDF.FieldType <= 21)
                                {
                                    WhatToReplace = ExtDDF.SecondaryTable + "." + ExtDDF.FieldPrefix;
                                }
                            }

                            else if (ExtDDF.FieldType >= 14 && ExtDDF.FieldType <= 21)
                            {

                                ExtDDFLnk = new ExtDDFEng();
                                string fieldpreifix = ExtDDF.SecondaryLink;

                                ExtDDFLnk = GetOneByTableNameAndFieldPrefix(ExtDDF.SecondaryTable, fieldpreifix);

                                string temp = TranTable.Rows[TranCount][ExtDDF.PrimaryTableLink].ToString();
                                byte temps = ExtDDFLnk.DataType;

                                string strQuery = string.Empty;
                                String ErrMsg = string.Empty;
                                //strQuery = HCMS.Common.Utility.General.GetJointVariant(ExtDDF, temp, temps, FPrefixValue);                                
                                strQuery = GetJointVariant(ExtDDF, temp, temps, FPrefixValue, "forIdentity");
                                //FPrefixValue = HCMS.Datalayer.CommonHelper.GetColValue(strQuery); //Nishad Commented 30112014
                                FPrefixValue = GetColValue(strQuery);   //Nishad Added 30112014

                                WhatToReplace = ExtDDF.SecondaryTable + "." + ExtDDF.FieldPrefix;
                            }

                            else
                            {
                                //ErrMsg = "The Field Type defined for the Field (" + ExtDDF.FieldPrefix + ") is Invalid or Inconsistent for Validation";
                            }

                            // done by benny on 22-07-2020 replace single quotes
                            if (!string.IsNullOrEmpty(FPrefixValue.ToString()))
                            {
                                FPrefixValue = FPrefixValue.ToString().Replace("'", "`");
                            }

                            sFormattedColValue = FPrefixValue.ToString();

                            if (sFormattedColValue == "NULL")
                            {
                                sFormattedColValue = "NULL";
                            }
                            //change back to orginal code
                            //if (string.IsNullOrEmpty(sFormattedColValue))
                            //{
                            //    sFormattedColValue = "NULL";
                            //}
                            else
                            {
                                switch (ExtDDF.DataType)
                                {
                                    case 0:
                                        sFormattedColValue = "'" + FPrefixValue.ToString() + "'";
                                        break;
                                    case 1:
                                        sFormattedColValue = "'" + FPrefixValue.ToString() + "'";
                                        break;
                                    case 2:
                                        sFormattedColValue = "'" + FPrefixValue.ToString() + "'";
                                        break;
                                    case 3:
                                        if (Convert.ToBoolean(FPrefixValue) == false)
                                        {
                                            sFormattedColValue = "0";
                                        }
                                        else
                                        {
                                            sFormattedColValue = "1";
                                        }

                                        break;
                                    case 4:
                                        if (!string.IsNullOrEmpty(FPrefixValue))
                                            sFormattedColValue = "CONVERT(DATETIME,'" + Convert.ToDateTime(FPrefixValue).ToString("yyyy/MM/dd H:mm:ss") + "')";
                                        else
                                            sFormattedColValue = "CONVERT(DATETIME,'" + (new DateTime(1900, 1, 1)).ToString("yyyy/MM/dd H:mm:ss") + "')"; //06-09-2022: robin added code to handle blank values
                                        break;
                                    case 5:
                                        sFormattedColValue = Convert.ToString(FPrefixValue);
                                        break;
                                    case 6:
                                        sFormattedColValue = Convert.ToString(FPrefixValue);
                                        break;
                                    case 7:
                                        sFormattedColValue = (Convert.ToString(FPrefixValue).Trim() == string.Empty ? "0" : Convert.ToString(FPrefixValue).Trim());
                                        break;
                                    case 8:
                                        sFormattedColValue = Convert.ToString(FPrefixValue);
                                        break;
                                    case 9:
                                        sFormattedColValue = Convert.ToString(FPrefixValue);
                                        break;
                                    case 10:
                                        sFormattedColValue = Convert.ToString(FPrefixValue);
                                        break;
                                }
                            }
                            ValidExpr = ValidExpr.ToUpper();
                            //String WTR = WhatToReplace.ToUpper();
                            String FCV = sFormattedColValue.ToUpper();
                            //ValidExpr = ValidExpr.Replace(WTR, FCV);
                            //Seetha Added - 04042021 - Replace with whole word.Above code createing issue with similar coulmn names like npresent and npresentsec
                            String WTR = String.Format(@"\b{0}\b", WhatToReplace.ToUpper());
                            ValidExpr = Regex.Replace(ValidExpr, WTR, FCV);
                            ValidExpr = "(" + ValidExpr + ")";
                        }

                        DFields.Clear();

                        sQry = "IF " + ValidExpr + " Select 1 As ColVal ELSE Select 2 As ColVal";
                        Result = Convert.ToInt16(GetColValue(sQry));
                        if (Result == 2)
                        {

                        }
                        else
                        {
                            ErrorCodes = ErrorCodes + Convert.ToString(ErrorView[i][1]) + "@";
                            ErrorCount = ErrorCount + 1;
                        }
                    }


                    if (ErrorCount != 0)
                    {
                        Errors.Add(TranTable.Rows[TranCount][MainColumnName].ToString(), ErrorCodes);
                    }
                }

            }

            catch (Exception Ex)
            {
                RetVal = false;
                //ErrMsg = "Function : CheckBusinessRules@Error : " + Ex.Message;
            }

            return RetVal;
        }

        public static ExtDDFEng GetOneByTableNameAndFieldPrefix(string TableName, string FieldPrefix)
        {
            string sqlCommand = "EXEC EAF_USP_GetExtDDFEngByFieldPrefix '" + TableName + "','" + FieldPrefix + "'";
            SqlDataReader dataReader = null;
            String ErrMsg = string.Empty;
            ExtDDFEng oExtDDFEng = new ExtDDFEng();
            ConnectionFunctions.Connect_SQLDataReader(ref dataReader, sqlCommand, ref ErrMsg);            
            while (dataReader.Read())
            {
                if (!dataReader.IsDBNull(dataReader.GetOrdinal("TableName")))
                    oExtDDFEng.TableName = (String)dataReader["TableName"];
                if (!dataReader.IsDBNull(dataReader.GetOrdinal("FieldPrefix")))
                    oExtDDFEng.FieldPrefix = (String)dataReader["FieldPrefix"];
                if (!dataReader.IsDBNull(dataReader.GetOrdinal("FieldTitle")))
                    oExtDDFEng.FieldTitle = (String)dataReader["FieldTitle"];
                if (!dataReader.IsDBNull(dataReader.GetOrdinal("FieldType")))
                    oExtDDFEng.FieldType = (Byte)dataReader["FieldType"];
                if (!dataReader.IsDBNull(dataReader.GetOrdinal("DataType")))
                    oExtDDFEng.DataType = (Byte)dataReader["DataType"];
                if (!dataReader.IsDBNull(dataReader.GetOrdinal("LookTableName")))
                    oExtDDFEng.LookTableName = (String)dataReader["LookTableName"];
                if (!dataReader.IsDBNull(dataReader.GetOrdinal("DisplayFieldPrefix")))
                    oExtDDFEng.DisplayFieldPrefix = (String)dataReader["DisplayFieldPrefix"];
                if (!dataReader.IsDBNull(dataReader.GetOrdinal("LinkingFieldPrefix")))
                    oExtDDFEng.LinkingFieldPrefix = (String)dataReader["LinkingFieldPrefix"];
                if (!dataReader.IsDBNull(dataReader.GetOrdinal("DisplayWidth")))
                    oExtDDFEng.DisplayWidth = (Int16)dataReader["DisplayWidth"];
                if (!dataReader.IsDBNull(dataReader.GetOrdinal("SecondaryTable")))
                    oExtDDFEng.SecondaryTable = (String)dataReader["SecondaryTable"];
                if (!dataReader.IsDBNull(dataReader.GetOrdinal("SecondaryLink")))
                    oExtDDFEng.SecondaryLink = (String)dataReader["SecondaryLink"];
                if (!dataReader.IsDBNull(dataReader.GetOrdinal("PrimaryTableLink")))
                    oExtDDFEng.PrimaryTableLink = (String)dataReader["PrimaryTableLink"];
                if (!dataReader.IsDBNull(dataReader.GetOrdinal("ForWorkFlow")))
                    oExtDDFEng.ForWorkFlow = (Byte)dataReader["ForWorkFlow"];
                if (!dataReader.IsDBNull(dataReader.GetOrdinal("ForBR")))
                    oExtDDFEng.ForBR = (Boolean)dataReader["ForBR"];
            }
            
            return oExtDDFEng;
        }

        public static String GetColValue(String sQry)
        {            
            string sqlCommand = "SET DATEFORMAT ymd " + sQry;
            //SqlCommand sqlCmd = new SqlCommand("SET DATEFORMAT ymd " + sQry, sqlConn);
            String sRetValue = string.Empty;
            String ErrMsg = string.Empty;
            SqlDataReader dataReader = null;
            ConnectionFunctions.Connect_SQLDataReader(ref dataReader, sqlCommand, ref ErrMsg);
            while (dataReader.Read())
            {
                if (!dataReader.IsDBNull(dataReader.GetOrdinal("ColVal")))
                    sRetValue = Convert.ToString(dataReader["ColVal"]);
            }
            return sRetValue;            
        }



        #region "Common Functions"

        private static bool AddCustomErrors(int ViewNo, ref DataTable ErrorsTable, SqlConnection Conn, ref string ErrMsg)
        {

            bool RetVal = true;
            SqlDataReader MyReader = null;
            try
            {
                RetVal = ConnectionFunctions.Connect_SQLDataReader(ref MyReader, "EXEC WEB_GetErrors " + ViewNo, ref ErrMsg, null, ref Conn, CommandType.Text);
                if (RetVal == true)
                {
                    ErrorsTable = CreateCustomErrorTableSchema();
                    DataRow LTRRow = default(DataRow);
                    while (MyReader.Read())
                    {
                        LTRRow = ErrorsTable.NewRow();
                        LTRRow[0] = MyReader[0];
                        LTRRow[1] = MyReader[1];
                        LTRRow[2] = MyReader[2];
                        LTRRow[3] = MyReader[3];
                        LTRRow[4] = MyReader[4];
                        LTRRow[5] = MyReader[5];
                        LTRRow[6] = MyReader[6];
                        LTRRow[7] = MyReader[7];
                        LTRRow[8] = MyReader[8];
                        LTRRow[9] = MyReader[9];
                        LTRRow[10] = MyReader[10];
                        ErrorsTable.Rows.Add(LTRRow);
                    }

                    if (ViewNo == 101)
                    {
                        LTRRow = ErrorsTable.NewRow();
                        LTRRow[0] = "2";
                        LTRRow[1] = "xxxxxx";
                        LTRRow[2] = "Leave Application is Overlapping with Another Application";
                        LTRRow[3] = "Leave Application is Overlapping with Another Application";
                        LTRRow[4] = "";
                        LTRRow[5] = "3";
                        LTRRow[6] = "101";
                        LTRRow[7] = "";
                        LTRRow[8] = "0";
                        LTRRow[9] = "1";
                        LTRRow[10] = "";
                        ErrorsTable.Rows.Add(LTRRow);
                    }
                }

            }
            catch (Exception Ex)
            {
                RetVal = false;
                ErrMsg = "Function : AddCustomErrors@Error: " + Ex.Message;
            }
            finally
            {
                if ((MyReader != null)) MyReader.Close();
            }

            return RetVal;

        }

        public static DataTable CreateCustomErrorTableSchema()
        {

            DataTable CustomErrorTable = new DataTable("CustomErrors");
            CustomErrorTable.Columns.Add("ErrType", Type.GetType("System.Byte"));
            CustomErrorTable.Columns.Add("Code", Type.GetType("System.String"));
            CustomErrorTable.Columns.Add("SystemMessage", Type.GetType("System.String"));
            CustomErrorTable.Columns.Add("Message", Type.GetType("System.String"));
            CustomErrorTable.Columns.Add("MessageA", Type.GetType("System.String"));
            CustomErrorTable.Columns.Add("Severity", Type.GetType("System.Byte"));
            CustomErrorTable.Columns.Add("TabID", Type.GetType("System.Int16"));
            CustomErrorTable.Columns.Add("ValidExprn", Type.GetType("System.String"));
            CustomErrorTable.Columns.Add("ShowInList", Type.GetType("System.Byte"));
            CustomErrorTable.Columns.Add("SysCoded", Type.GetType("System.Byte"));
            CustomErrorTable.Columns.Add("DistinctFldPrefix", Type.GetType("System.String"));
            return CustomErrorTable;

        }

        private static bool GetExtDDFEng(string ModuleTableName, string FieldPrefix, ref Hashtable ExtDDF, SqlConnection Conn, ref string ErrMsg)
        {

            bool RetVal = false;
            SqlDataReader MyReader = null;
            try
            {

                RetVal = ConnectionFunctions.Connect_SQLDataReader(ref MyReader, "EXEC WEB_GetExtDDFEng '" + ModuleTableName + "', '" + FieldPrefix + "'", ref ErrMsg, null, ref Conn, CommandType.Text);
                if (RetVal == true)
                {
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
                    }
                }
            }

            catch (Exception Ex)
            {
                RetVal = false;
                ErrMsg = "Function Name : GetExtDDFEng, Error : " + Ex.Message;
            }
            finally
            {
                if ((MyReader != null)) MyReader.Close();
            }

            return RetVal;

        }

        private static bool GetJointVariant(ref Hashtable ExtDDF, ref string PlinksValue, ref byte DataType, ref string FPrefixValue, SqlConnection Conn, ref string ErrMsg)
        {

            bool RetVal = false;
            try
            {
                StringBuilder sQry = new StringBuilder();
                sQry.Append("Select " + ExtDDF["SecondaryTable"].ToString() + "." + ExtDDF["FieldPrefix"].ToString());
                sQry.Append(" From " + ExtDDF["SecondaryTable"].ToString());
                sQry.Append(" Where (" + ExtDDF["SecondaryTable"].ToString() + "." + ExtDDF["SecondaryLink"].ToString() + " = ");

                //Dim Temp As String = sQry.ToString 
                string sFormattedPlinkValue = string.Empty;

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

                RetVal = GetColValue("Select Count(Expr1) As Noc From TableStructs Where Expr1 = 'LastModDateTime' And [name] = '" + ExtDDF["SecondaryTable"].ToString() + "'", ref FPrefixValue, Conn, ref ErrMsg);
                //Check If LastModDateTime Exosts in the Table 
                if (RetVal == false)
                {
                    return false; // TODO: might not be correct. Was : Exit Try 
                }

                if (Convert.ToInt16(FPrefixValue) == 1)
                {
                    sQry.Append(" AND (" + ExtDDF["SecondaryTable"].ToString() + ".LastModDateTime = (Select Max(LastModDateTime) From " + ExtDDF["SecondaryTable"].ToString() + " WHERE " + ExtDDF["SecondaryTable"].ToString() + "." + ExtDDF["SecondaryLink"].ToString() + "=" + sFormattedPlinkValue + "))");
                }

                RetVal = GetColValue(sQry.ToString(), ref FPrefixValue, Conn, ref ErrMsg);
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

        public static string GetJointVariant(ExtDDFEng ExtDDF, string PlinksValue, byte DataType, String FPrefixValue)
        {
            bool RetVal = false;
            String ErrMsg = "";
            StringBuilder sQry = new StringBuilder();
            try
            {
                sQry.Append("Select " + ExtDDF.SecondaryTable + "." + ExtDDF.FieldPrefix + " As ColVal ");
                sQry.Append(" From " + ExtDDF.SecondaryTable);
                sQry.Append(" Where (" + ExtDDF.SecondaryTable + "." + ExtDDF.SecondaryLink + " = ");

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

                sQry.Append(sFormattedPlinkValue + ")");


                RetVal = GetColValue("Select Count(Expr1) As Noc From TableStructs Where Expr1 = 'LastModDateTime' And [name] = '" + ExtDDF.SecondaryTable + "'", ref FPrefixValue, "", ref ErrMsg);
                //Check If LastModDateTime Exosts in the Table 
                if (RetVal == false)
                {
                    return "false"; // TODO: might not be correct. Was : Exit Try 
                }

                if (Convert.ToInt16(FPrefixValue) == 1)
                {
                    sQry.Append(" AND (" + ExtDDF.SecondaryTable + ".LastModDateTime = (Select Max(LastModDateTime) From " + ExtDDF.SecondaryTable + " WHERE " + ExtDDF.SecondaryTable + "." + ExtDDF.SecondaryLink + "=" + sFormattedPlinkValue + "))");
                }

                //RetVal = GetColValue(sQry.ToString(), ref FPrefixValue, strConn, ref ErrMsg);
                //if (RetVal == false)
                //{
                //    return false; // TODO: might not be correct. Was : Exit Try 
                //}
                return sQry.ToString();
            }
            catch (Exception Ex)
            {
                sQry.Append(string.Empty);
            }
            return sQry.ToString();
        }

        private static bool GetColValue(string sQry, ref string FPrefixValue, SqlConnection Conn, ref string ErrMsg)
        {

            bool RetVal = false;
            try
            {
                SqlParameter[] p = null;
                RetVal = ConnectionFunctions.Connect_SQLScalar(ref FPrefixValue, sQry, ref p, ref Conn, ref ErrMsg);
            }
            catch (Exception Ex)
            {
                RetVal = false;
                ErrMsg = "Function Name : GetColValue() Error : " + Ex.Message;
            }

            return RetVal;

        }

        public static bool GetResult(string sQry, ref Int16 Result, string strConn, ref string ErrMsg)
        {

            bool RetVal = true;
            SqlConnection sqlConn = new SqlConnection(ConnectionFunctions.GetConnectionString());
            try
            {
                //String 
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

        private static string HandleNullText(object HashTableItem)
        {

            if (Convert.IsDBNull(HashTableItem))
            {
                return "NULL";
            }
            else
            {
                if (Convert.ToString(HashTableItem).Trim().Length == 0)
                {
                    return "";
                }
                else
                {
                    return Convert.ToString(HashTableItem).Trim();
                }
            }

        }

        public static bool GetExtDDFEng(string ModuleTableName, string FieldPrefix, ref Hashtable ExtDDF, string strConn, ref string ErrMsg)
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

        public static bool GetJointVariant(ref Hashtable ExtDDF, ref string PlinksValue, ref byte DataType, ref string FPrefixValue, string strConn, ref string ErrMsg)
        {

            bool RetVal = true;
            try
            {
                StringBuilder sQry = new StringBuilder();
                sQry.Append("Select " + ExtDDF["SecondaryTable"].ToString() + "." + ExtDDF["FieldPrefix"].ToString());
                sQry.Append(" From " + ExtDDF["SecondaryTable"].ToString());
                sQry.Append(" Where (" + ExtDDF["SecondaryTable"].ToString() + "." + ExtDDF["SecondaryLink"].ToString() + " = ");

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

                RetVal = GetColValue("Select Count(Expr1) As Noc From TableStructs Where Expr1 = 'LastModDateTime' And [name] = '" + ExtDDF["SecondaryTable"].ToString() + "'", ref FPrefixValue, strConn, ref ErrMsg);
                //Check If LastModDateTime Exosts in the Table 
                if (RetVal == false)
                {
                    return false; // TODO: might not be correct. Was : Exit Try 
                }

                if (Convert.ToInt16(FPrefixValue) == 1)
                {
                    sQry.Append(" AND (" + ExtDDF["SecondaryTable"].ToString() + ".LastModDateTime = (Select Max(LastModDateTime) From " + ExtDDF["SecondaryTable"].ToString() + " WHERE " + ExtDDF["SecondaryTable"].ToString() + "." + ExtDDF["SecondaryLink"].ToString() + "=" + sFormattedPlinkValue + "))");
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

        private static bool GetColValue(string sQry, ref string FPrefixValue, string strConn, ref string ErrMsg)
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

        public static bool CalculateAlBal(int EmpID, DateTime ExStartdt, ref decimal AlDays, ref SqlConnection Conn, ref string ErrMsg)
        {

            bool RetVal = false;
            SqlDataReader MyReader = null;
            try
            {
                string strQry = string.Empty;
                string AlCode = string.Empty;
                ArrayList temp = new ArrayList();

                //Get the AlCode 
                SqlParameter[] Params = new SqlParameter[2];
                Params[0] = new SqlParameter("@EmpID", EmpID);
                Params[1] = new SqlParameter("@EffDate", ExStartdt);
                RetVal = ConnectionFunctions.Connect_SQLScalar(ref AlCode, "Eff_ALBalEntitle", ref Params, ref  Conn, ref ErrMsg);
                if (RetVal == false)
                {
                    return false; // TODO: might not be correct. Was : Exit Try 
                }

                //This Procedure Returns AlBal from EmpBals & LastPaid,JoiningDate from FinMast 
                SqlParameter[] Params2 = new SqlParameter[2];
                Params2[0] = new SqlParameter("@EmpID", EmpID);
                Params2[1] = new SqlParameter("@AlCode", AlCode);
                RetVal = ConnectionFunctions.Connect_SQLDataReader(ref MyReader, "APPR_GetALBalances", ref ErrMsg, Params2, ref Conn, CommandType.StoredProcedure);
                if (RetVal == false)
                {
                    return false; // TODO: might not be correct. Was : Exit Try 
                }

                MyReader.Read();
                decimal AlBal = Math.Round(Convert.ToDecimal(MyReader[0]), 2);
                temp.Add(AlBal);

                MyReader.NextResult();
                MyReader.Read();

                System.DateTime JoiningDt = default(System.DateTime);
                System.DateTime LastPaidDt = default(System.DateTime);
                LastPaidDt = Convert.ToDateTime(MyReader[0]);
                JoiningDt = Convert.ToDateTime(MyReader[1]);

                temp.Add(LastPaidDt);
                temp.Add(JoiningDt);

                MyReader.NextResult();
                MyReader.Read();
                Int16 AlType = Convert.ToInt16(MyReader[0]);
                MyReader.Close();

                TimeSpan t1 = new TimeSpan();

                t1 = LastPaidDt.Subtract(ExStartdt);


                decimal BalanceDays = t1.Days; //DateDiff(DateInterval.Day, LastPaidDt, ExStartdt);
                //Extra Days Since the Last Paid Date 
                System.DateTime dtTemp = default(System.DateTime);
                System.DateTime dtTemp2 = default(System.DateTime);
                //Dim AlType As Int16 = AlCode 'Convert.ToInt16(ProcResult(0)) 
                float NoOfDays = 0;
                //CDec(ProcResult(1)) 
                float EWrkDays = 0;
                //CDec(ProcResult(2)) 

                temp.Add(BalanceDays);
                temp.Add(AlType);
                temp.Add(NoOfDays);
                temp.Add(EWrkDays);

                switch (AlType)
                {

                    case 2:
                        //Std No. of Days 
                        strQry = "SELECT col1, col2 FROM AlEntitlementsSec WHERE Code= '" + AlCode + "'";
                        RetVal = ConnectionFunctions.Connect_SQLDataReader(ref MyReader, strQry, ref  ErrMsg, null, ref Conn, CommandType.Text);
                        MyReader.Read();
                        NoOfDays = (float)(MyReader[0]);
                        EWrkDays = (float)(MyReader[1]);

                        MyReader.Close();

                        if ((NoOfDays == 0.0f) | (EWrkDays == 0.0f))
                        {
                            AlDays = 0;
                        }
                        else
                        {
                            AlDays = ((Convert.ToDecimal(NoOfDays) / Convert.ToDecimal(EWrkDays)) * BalanceDays);
                        }


                        temp.Add(NoOfDays);
                        temp.Add(EWrkDays);
                        break;

                    case 3:
                        //Alternate Years 
                        decimal nDays1 = default(decimal);
                        decimal nDays2 = default(decimal);


                        strQry = "SELECT ISNULL(col2,0) FROM AlEntitlementsSec WHERE Code= '" + AlCode + "' AND Slab = 1; ";
                        strQry += "SELECT ISNULL(col2,0) FROM AlEntitlementsSec WHERE Code= '" + AlCode + "' AND Slab = 2";
                        RetVal = ConnectionFunctions.Connect_SQLDataReader(ref MyReader, strQry, ref ErrMsg, null, ref Conn, CommandType.Text);

                        MyReader.Read();
                        nDays1 = Convert.ToDecimal(MyReader[0]);
                        MyReader.NextResult();
                        MyReader.Read();
                        nDays2 = Convert.ToDecimal(MyReader[0]);
                        MyReader.Close();

                        if (JoiningDt.Day == 1)
                        {
                            if (JoiningDt.Month == 1)
                            {
                                dtTemp = Convert.ToDateTime((JoiningDt.Year) + "-12-31");
                            }
                            else
                            {
                                if (JoiningDt.Month == 2 | JoiningDt.Month == 4 | JoiningDt.Month == 6 | JoiningDt.Month == 8 | JoiningDt.Month == 9 | JoiningDt.Month == 11)
                                {
                                    dtTemp = Convert.ToDateTime((JoiningDt.Year + 1) + "-" + (JoiningDt.Month - 1) + "-31");
                                }
                                else if (JoiningDt.Month == 3)
                                {
                                    if ((JoiningDt.Year + 1) % 4 == 0)
                                    {
                                        dtTemp = Convert.ToDateTime(((JoiningDt.Year + 1) + "-" + (JoiningDt.Month - 1) + "-29"));
                                    }
                                    else
                                    {
                                        dtTemp = Convert.ToDateTime((JoiningDt.Year + 1) + "-" + (JoiningDt.Month - 1) + "-28");
                                    }
                                }
                                else
                                {
                                    dtTemp = Convert.ToDateTime((JoiningDt.Year + 1) + "-" + (JoiningDt.Month - 1) + "-30");
                                }
                            }
                        }
                        else
                        {
                            dtTemp = Convert.ToDateTime((JoiningDt.Year + 1) + "-" + (JoiningDt.Month) + "-" + (JoiningDt.Day - 1));
                        }


                        if (ExStartdt <= dtTemp)
                        {
                            AlDays = ((nDays1 / Convert.ToDecimal(365.0)) * BalanceDays);
                            temp.Add(nDays1);
                        }
                        else
                        {
                            double ftotdays = 0;
                            double fTotYears = 0;
                            double fDiffMod = 0;
                            ftotdays = 0.0f;
                            fTotYears = 0.0f;
                            fDiffMod = 0.0f;
                            TimeSpan T1 = new TimeSpan();
                            T1 = JoiningDt.Subtract(ExStartdt);
                            ftotdays = T1.Days; //DateDiff(DateInterval.Day, JoiningDt, ExStartdt);
                            fTotYears = ftotdays / 365.0;
                            fDiffMod = fTotYears % 2.0;
                            if (fDiffMod <= 1.0)
                            {
                                AlDays = ((nDays1 / Convert.ToDecimal(365.0)) * BalanceDays);
                                temp.Add(nDays1);
                            }
                            else
                            {
                                AlDays = ((nDays2 / Convert.ToDecimal(365.0)) * BalanceDays);
                                temp.Add(nDays2);
                            }
                        }

                        temp.Add(365);
                        break;

                    case 4:
                        //Slabs 
                        Double years = 0;
                        Double years2 = 0;
                        //decimal nDays1 = default(decimal);
                        //decimal nDays2 = default(decimal);
                        nDays1 = 0;
                        nDays2 = 0;
                        Decimal nDays3 = 0;
                        strQry = "SELECT col1, col2 FROM AlEntitlementsSec WHERE Code = '" + AlCode + "' And Slab = 1; ";
                        strQry += "SELECT col1, col2  FROM AlEntitlementsSec WHERE Code = '" + AlCode + "' And Slab = 3";
                        strQry += "SELECT col2 FROM AlEntitlementsSec WHERE Code = '" + AlCode + "' And Slab = 2";
                        RetVal = ConnectionFunctions.Connect_SQLDataReader(ref MyReader, strQry, ref ErrMsg, null, ref Conn, CommandType.Text);

                        MyReader.Read();
                        years = Convert.ToDouble(MyReader[0]);
                        nDays1 = Convert.ToInt16(MyReader[1]);
                        MyReader.NextResult();
                        MyReader.Read();
                        years2 = Convert.ToDouble(MyReader[0]);
                        nDays2 = Convert.ToInt16(MyReader[1]);

                        MyReader.NextResult();
                        MyReader.Read();
                        nDays3 = Convert.ToDecimal(MyReader[0]);
                        MyReader.Close();
                        if (years == 0.5 && years2 == 0.5)
                        {
                            dtTemp = Convert.ToDateTime((JoiningDt.AddMonths(6)).AddDays(-1));
                            dtTemp2 = Convert.ToDateTime((JoiningDt.AddYears(1)).AddDays(-1));
                        }
                        else
                        {
                            if (JoiningDt.Day == 1)
                            {
                                if (JoiningDt.Month == 1)
                                {
                                    dtTemp = Convert.ToDateTime((JoiningDt.Year + (years - 1)) + "-12-31");
                                    if (years2 > 0)
                                        dtTemp2 = Convert.ToDateTime((JoiningDt.Year + (years + years2 - 1)) + "-12-31");
                                }
                                else
                                {
                                    if (JoiningDt.Month == 1)
                                    {
                                        dtTemp = Convert.ToDateTime((JoiningDt.Year + (years - 1)) + "-12-31");
                                        if (years2 > 0)
                                            dtTemp2 = Convert.ToDateTime((JoiningDt.Year + (years + years2 - 1)) + "-12-31");
                                    }
                                    else
                                    {
                                        if ((JoiningDt.Month == 2 | JoiningDt.Month == 4 | JoiningDt.Month == 6 | JoiningDt.Month == 8 | JoiningDt.Month == 9 | JoiningDt.Month == 11))
                                        {
                                            dtTemp = Convert.ToDateTime((JoiningDt.Year + years) + "-" + (JoiningDt.Month - 1) + "-31");

                                            if (years2 > 0)
                                                dtTemp2 = Convert.ToDateTime((JoiningDt.Year + years + years2) + "-" + (JoiningDt.Month - 1) + "-31");
                                        }
                                        else if (JoiningDt.Month == 3)
                                        {
                                            if ((JoiningDt.Year + years) % 4 == 0)
                                            {
                                                dtTemp = Convert.ToDateTime((JoiningDt.Year + years) + "-" + (JoiningDt.Month - 1) + "-29");
                                            }
                                            else
                                            {
                                                dtTemp = Convert.ToDateTime((JoiningDt.Year + years) + "-" + (JoiningDt.Month - 1) + "-28");
                                            }
                                            if (years2 > 0)
                                            {
                                                if ((JoiningDt.Year + years + years2) % 4 == 0)
                                                {
                                                    dtTemp2 = Convert.ToDateTime((JoiningDt.Year + years + years2) + "-" + (JoiningDt.Month - 1) + "-29");
                                                }
                                                else
                                                {
                                                    dtTemp2 = Convert.ToDateTime((JoiningDt.Year + years + years2) + "-" + (JoiningDt.Month - 1) + "-28");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            dtTemp = Convert.ToDateTime((JoiningDt.Year + years) + "-" + (JoiningDt.Month - 1) + "-30");
                                            if (years2 > 0)
                                            {
                                                dtTemp2 = Convert.ToDateTime((JoiningDt.Year + years + years2) + "-" + (JoiningDt.Month - 1) + "-30");
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                dtTemp = Convert.ToDateTime((JoiningDt.Year + years) + "-" + (JoiningDt.Month) + "-" + (JoiningDt.Day - 1));
                                if (years2 > 0)
                                {
                                    dtTemp2 = Convert.ToDateTime((JoiningDt.Year + years + years2) + "-" + (JoiningDt.Month) + "-" + (JoiningDt.Day - 1));
                                }
                            }

                        }
                        
                        if ((ExStartdt <= dtTemp))
                        {
                            AlDays = ((nDays1 / Convert.ToDecimal(365.0)) * BalanceDays);
                            temp.Add(nDays1);
                        }
                        else if (ExStartdt > dtTemp && ExStartdt <= dtTemp2)
                        {
                            AlDays = ((nDays2 / 365) * BalanceDays);
                            temp.Add(nDays2);
                        }
                        else
                        {
                            AlDays = (((nDays3) / Convert.ToDecimal(365.0)) * BalanceDays);
                            temp.Add(nDays3);
                        }
                        temp.Add(365);
                        break;

                }
            }

            catch (Exception Ex)
            {
                RetVal = false;
                ErrMsg = "There was a Problem Calculating Your Entitled Leave Balances." + Ex.Message;
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

        public static void ReturnProbationDate(ref int EmpID, ref System.DateTime ProbDate, ref SqlConnection Conn, ref string ErrMsg)
        {

            System.DateTime JoinDate = new System.DateTime(1900, 1, 1);
            bool RetVal = false;
            SqlDataReader MyReader = null;
            try
            {
                string MySQL = "Select JoiningDate FROM FinMast WHERE EmpId = " + EmpID + "; ";
                MySQL += "SELECT ProbPeriod FROM SalaryProfile WHERE Code IN (SELECT SalProfile FROM FinMast WHERE EmpId = " + EmpID + ")";
                RetVal = ConnectionFunctions.Connect_SQLDataReader(ref MyReader, MySQL, ref ErrMsg, null, ref Conn, CommandType.Text);
                if (RetVal != false)
                {
                    // TODO: might not be correct. Was : Exit Try 


                    MyReader.Read();
                    JoinDate = MyReader.GetDateTime(0);
                    MyReader.NextResult();
                    MyReader.Read();
                    ProbDate = JoinDate.AddDays(Convert.ToDouble(MyReader[0]));
                }
            }

            catch (Exception ex)
            {
                ProbDate = new DateTime(1900, 1, 1);
                RetVal = false;
                ErrMsg = ex.Message;
            }
            finally
            {
                if ((MyReader != null))
                {
                    if (!MyReader.IsClosed)
                    {
                        MyReader.Close();
                    }
                }
            }

        }

        public static string GetJointVariant(ExtDDFEng ExtDDF, string PlinksValue, byte DataType, String FPrefixValue, string forIdentity)
        {
            bool RetVal = false;
            String ErrMsg = "";
            StringBuilder sQry = new StringBuilder();
            SqlConnection strConn = new SqlConnection(ConnectionFunctions.GetConnectionString());
            try
            {
                sQry.Append("Select " + ExtDDF.SecondaryTable + "." + ExtDDF.FieldPrefix + " As ColVal ");
                sQry.Append(" From " + ExtDDF.SecondaryTable);
                sQry.Append(" Where (" + ExtDDF.SecondaryTable + "." + ExtDDF.SecondaryLink + " = ");

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

                strConn.Open();
                RetVal = GetColValue("Select Count(Expr1) As Noc From TableStructs Where Expr1 = 'LastModDateTime' And [name] = '" + ExtDDF.SecondaryTable + "'", ref FPrefixValue, strConn, ref ErrMsg);
                strConn.Close();
                //Check If LastModDateTime Exosts in the Table 
                if (RetVal == false)
                {
                    return ""; // TODO: might not be correct. Was : Exit Try 
                }

                if (Convert.ToInt16(FPrefixValue) == 1)
                {
                    sQry.Append(" AND (" + ExtDDF.SecondaryTable + ".LastModDateTime = (Select Max(LastModDateTime) From " + ExtDDF.SecondaryTable + " WHERE " + ExtDDF.SecondaryTable + "." + ExtDDF.SecondaryLink + "=" + sFormattedPlinkValue + "))");
                }

                //RetVal = GetColValue(sQry.ToString(), ref FPrefixValue, strConn, ref ErrMsg);
                //if (RetVal == false)
                //{
                //    return false; // TODO: might not be correct. Was : Exit Try 
                //}
                return sQry.ToString();
            }
            catch (Exception Ex)
            {
                sQry.Append(string.Empty);
            }
            finally
            {
                if (strConn.State != 0)
                    strConn.Close();
            }
            return sQry.ToString();
        }

        #endregion


        public static void FinancialS(int nViewNo, DataRow FinMast, ref string ErrorCodes, ref string ErrMsg, string sEffectiveDate, string sChangedElements, bool AdmRW, bool PerRW, bool ErnRW)
        {
            bool RetVal = false;
            SqlDataReader MyReader = null;
            try
            {
                string strEC = "";
                string strEN = "";
                string sTemp = "";
                string sTemp2 = "";
                decimal cyTemp = 0;
                decimal BlankCurr = 0;
                int lEmpID = 0;
                short nStatus = Convert.ToInt16(FinMast["Status"]);
                short nTemp = 0;
                short nTemp2 = 0;
                System.DateTime dtTemp = new System.DateTime(1900, 1, 1);
                System.DateTime EmptyDate = new System.DateTime(1900, 1, 1);

                if (nStatus < 20)
                {
                    //For new joined employees, check Joining Date validations
                    if (PerRW)
                    {
                        dtTemp = Convert.ToDateTime(FinMast["JoiningDate"]);
                        if (dtTemp <= EmptyDate)
                        {
                            ErrorCodes += "FT0009@";
                        }

                        //For new joined employees, check Last Paid Date validations
                        if (nViewNo != 456)
                        {
                            dtTemp = Convert.ToDateTime(FinMast["LastPaidDate"]);
                            if (dtTemp <= EmptyDate)
                            {
                                ErrorCodes += "FT0032@";
                            }
                        }
                    }
                }
                else
                {
                    //For current employees, check Effective Date validations.
                    if (nViewNo != 456)
                    {
                        //Nishad Commented 01062016
                        //dtTemp = (string.IsNullOrEmpty(sEffectiveDate) ? new System.DateTime(1900, 1, 1) : Convert.ToDateTime(sEffectiveDate));
                        //if (dtTemp <= EmptyDate)
                        //{
                        //    ErrorCodes += "FT0007@";
                        //}
                        //Nishad End Comment 01062016
                    }
                }

                //Checking for location not blank. Even if we check for LocLib1 its ok..
                //sTemp = ((FinMast.IsNull("LocLib1") ? string.Empty : FinMast["LocLib1"])).ToString().Trim();
                //if (string.IsNullOrEmpty(sTemp))
                //{
                //    ErrorCodes += "FT0010@";
                //}

                //Category Check
                if (PerRW)
                {
                    sTemp = ((FinMast.IsNull("CategMast") ? string.Empty : FinMast["CategMast"])).ToString().Trim();
                    sTemp2 = ((FinMast.IsNull("CategSec") ? string.Empty : FinMast["CategSec"])).ToString().Trim();
                    if (string.IsNullOrEmpty(sTemp) | string.IsNullOrEmpty(sTemp2))
                    {
                        ErrorCodes += "FT0011@";
                    }
                    String sCatCount = "0";
                    Int16 CatCount = 0;
                    ConnectionFunctions.Connect_SQLScalar(ref sCatCount, "SELECT COUNT(Code) FROM CategorySecondary WHERE (Code = '" + sTemp2 + "' AND CodeMast = '" + sTemp + "')", ref ErrMsg);
                    CatCount = Convert.ToInt16(sCatCount);
                    if (CatCount <= 0)
                    {
                        ErrorCodes += "FT0012@";
                    }

                    sTemp = string.Empty;
                    sTemp2 = string.Empty;

                    sTemp = ((FinMast.IsNull("JobTitle") ? string.Empty : FinMast["JobTitle"])).ToString().Trim();
                    if (string.IsNullOrEmpty(sTemp))
                    {
                        ErrorCodes += "FT0013@";
                    }

                    sTemp = ((FinMast.IsNull("SalProfile") ? string.Empty : FinMast["SalProfile"])).ToString().Trim();
                    if (string.IsNullOrEmpty(sTemp))
                    {
                        ErrorCodes += "FT0014@";
                    }

                    sTemp = ((FinMast.IsNull("SalGrade") ? string.Empty : FinMast["SalGrade"])).ToString().Trim();
                    if (string.IsNullOrEmpty(sTemp))
                    {
                        ErrorCodes += "FT0015@";
                    }
                }

                if (AdmRW)
                {
                    sTemp = ((FinMast.IsNull("BSalaryCurr") ? string.Empty : FinMast["BSalaryCurr"])).ToString().Trim();
                    cyTemp = Convert.ToDecimal((FinMast.IsNull("BSalaryAmt") ? 0 : FinMast["BSalaryAmt"]));
                    //Seetha Commented 12012022 - Zero basic salary check
                    //if (string.IsNullOrEmpty(sTemp) | cyTemp == BlankCurr)
                    if (string.IsNullOrEmpty(sTemp))
                    {
                        ErrorCodes += "FT0016@";
                    }

                    string sTem = ((FinMast.IsNull("HRACurr") ? string.Empty : FinMast["HRACurr"])).ToString().Trim();
                    int nCnt = 0;

                    if (!string.IsNullOrEmpty(sTem))
                    {
                        bFirstTime = true;
                        if (OnCheck(sTemp, sTem))
                            nCnt += 1;
                    }

                    sTem = ((FinMast.IsNull("TranCurr") ? string.Empty : FinMast["TranCurr"])).ToString().Trim();
                    if (!string.IsNullOrEmpty(sTem))
                    {
                        if (OnCheck(sTemp, sTem))
                            nCnt += 1;
                    }

                    sTem = ((FinMast.IsNull("FoodCurr") ? string.Empty : FinMast["FoodCurr"])).ToString().Trim();
                    if (!string.IsNullOrEmpty(sTem))
                    {
                        if (OnCheck(sTemp, sTem))
                            nCnt += 1;
                    }

                    for (int i = 1; i <= 8; i++)
                    {
                        string sElem = "AuxAll" + i.ToString() + "Curr";
                        sTem = ((FinMast.IsNull(sElem) ? string.Empty : FinMast[sElem])).ToString().Trim();
                        if (!string.IsNullOrEmpty(sTem))
                        {
                            if (OnCheck(sTemp, sTem))
                                nCnt += 1;
                        }
                    }

                    if (nCnt > 1)
                    {
                        ErrorCodes += "FT0054@";
                    }
                }

                if (ErnRW)
                {
                    nTemp = nTemp2 = 0;
                    //If the person has just selected the family and not selected employee give him warning.
                    nTemp = Convert.ToInt16(((FinMast.IsNull("ETicketEvery") ? 0 : FinMast["ETicketEvery"])).ToString().Trim());
                    nTemp2 = Convert.ToInt16(((FinMast.IsNull("FTicketYN") ? 0 : FinMast["FTicketYN"])).ToString().Trim());
                    if (nTemp2 > 0)
                    {
                        if (nTemp <= 0)
                            ErrorCodes += "FT0019@";
                    }

                    //If the employee is entitled to a family ticket, but Full, Child, Infant Tkts have not been entered. 
                    Int16 nFull = 0;
                    Int16 nChild = 0;
                    Int16 nInfant = 0;
                    nFull = Convert.ToInt16(((FinMast.IsNull("NoOfFullTickets") ? 0 : FinMast["NoOfFullTickets"])).ToString().Trim());
                    nChild = Convert.ToInt16(((FinMast.IsNull("NoOfChildTickets") ? 0 : FinMast["NoOfChildTickets"])).ToString().Trim());
                    nInfant = Convert.ToInt16(((FinMast.IsNull("NoOfInfantTickets") ? 0 : FinMast["NoOfInfantTickets"])).ToString().Trim());
                    //Family Ticket = YES
                    if (nTemp2 > 0)
                    {
                        if (nFull <= 0 & nChild <= 0 & nInfant <= 0)
                            ErrorCodes += "FT0034@";
                    }

                    //Here we are checking to see if the Employee is entitled to a Family Ticket, and if the Family Ticket Every XXX Months has been set as 0
                    nFull = Convert.ToInt16(((FinMast.IsNull("FTicketEvery") ? 0 : FinMast["FTicketEvery"])).ToString().Trim());
                    //Family Ticket = YES
                    if (nTemp2 > 0)
                    {
                        if (nFull <= 0)
                            ErrorCodes += "FT0035@";
                    }

                    //If the employee is entitled to a ticket, but his destination has not been entered. 
                    sTemp = ((FinMast.IsNull("RouteEmp") ? string.Empty : FinMast["RouteEmp"])).ToString().Trim();
                    //Employee Ticket Every > 0
                    if (nTemp > 0)
                    {
                        if (string.IsNullOrEmpty(sTemp))
                            ErrorCodes += "FT0020@";
                    }

                    //If the employee is entitled to a family ticket, but family destination has not been entered. 
                    sTemp = ((FinMast.IsNull("RouteFam") ? string.Empty : FinMast["RouteFam"])).ToString().Trim();
                    //If Family Ticket Every > 0
                    if (nTemp2 > 0)
                    {
                        if (string.IsNullOrEmpty(sTemp))
                            ErrorCodes += "FT0033@";
                    }
                }

                //Retreiving the last closed Payroll date.
                //Checking for "System locked by Payroll." error.
                System.DateTime dtPyrlClose = new System.DateTime(1900, 1, 1);
                bool bSiteModule = false;
                short nPyrlLock = 1;
                RetVal = ConnectionFunctions.Connect_SQLDataReader(ref MyReader, "SELECT * FROM PrgDefault", ref ErrMsg);
                if (RetVal == true & MyReader.HasRows == true)
                {
                    MyReader.Read();
                    //dtPyrlClose = (Information.IsDBNull(MyReader["PayrollDate"]) ? new System.DateTime(1900, 1, 1) : MyReader["PayrollDate"]);
                    //nPyrlLock = (Information.IsDBNull(MyReader["PayrollLock"]) ? 0 : MyReader["PayrollLock"]);
                    //bSiteModule = (Information.IsDBNull(MyReader["SiteModule"]) ? false : MyReader["SiteModule"]);

                    dtPyrlClose = Convert.ToDateTime((MyReader["PayrollDate"].Equals(DBNull.Value) ? new System.DateTime(1900, 1, 1) : MyReader["PayrollDate"]));
                    nPyrlLock = Convert.ToInt16((MyReader["PayrollLock"].Equals(DBNull.Value)) ? 0 : MyReader["PayrollLock"]);
                    bSiteModule = Convert.ToBoolean((MyReader["SiteModule"].Equals(DBNull.Value)) ? false : MyReader["SiteModule"]);

                    //Nishad Commented 01062016
                    //if (nPyrlLock == 1)
                    //    ErrorCodes += "FT0051@";
                    //Nishad End Comment 01062016
                }
                //Nishad Commented 01062016
                //else
                //{
                //    ErrorCodes += "FT0050@";
                //}
                //Nishad End Comment 01062016
                if (!MyReader.IsClosed)
                    MyReader.Close();

                if (nViewNo == 456)
                {
                    //This Part is Not Coded because its not required for Approvals
                    //....Candidate Financial....'
                }

                //Validations for the Changed Elements if Status >= 20

                if (nStatus >= 20)
                {
                    string sElemCopy = string.Empty;
                    string[] saElem;
                    //Parsing sChangedElements to saElem string array for changed elements
                    int b = 0;
                    int c = 0;
                    string s = string.Empty;
                    //sChangedElements = HttpContext.Current.Session["sChangedElements"].ToString(); //24-12-2024:commented as no session in SF_CS app
                    sElemCopy = sChangedElements;

                    if (sChangedElements != "" || sChangedElements != String.Empty)
                    {
                        sChangedElements = sChangedElements.Substring(0, sChangedElements.Length - 1);
                    }

                    saElem = sChangedElements.Split('@');

                    string[] saElem2 = sChangedElements.Split('@');
                    ArrayList saElem3 = new ArrayList();
                    foreach (string id in saElem2)
                    {
                        saElem3.Add(id);
                    }

                    int nTotElem1 = saElem3.Count;
                    for (int nd = 0; nd <= nTotElem1 - 1; nd++)
                    {
                        String s1 = saElem2[nd].ToString();
                    }


                    //for (b = 0; b <= sElemCopy.Length - 1; b++)
                    //{
                    //    if (sElemCopy.Substring(b, 1) == "@")
                    //    {
                    //        if (b == 0)
                    //            break; // TODO: might not be correct. Was : Exit For
                    //        s = sElemCopy.Substring(c, b - c);
                    //        saElem.Add(s);
                    //        c = b + 1;
                    //    }
                    //    if (b + 1 == sElemCopy.Length)
                    //    {
                    //        s = sElemCopy.Substring(c, (b + 1) - c);
                    //        saElem.Add(s);
                    //    }
                    //}

                    //If user clicks on Save without changing any element.
                    int nTotElem = saElem3.Count;
                    //Nishad Commented 01062016
                    //if (string.IsNullOrEmpty(sElemCopy) | nTotElem == -1)
                    //    ErrorCodes += "FT0021@";
                    //Nishad End Comment 01062016

                    //Used to store multiple occurances of Salary Profile, Location, Salary Grade, Category.
                    string sSPLOCStr = string.Empty;

                    dtTemp = Convert.ToDateTime(sEffectiveDate); //Convert.ToDateTime(HttpContext.Current.Session["UpgradeEffDate"]); //24-12-2024:commented as no session in SF_CS app
                    if (dtTemp > EmptyDate)
                    {
                        //Appending the Cut off date to the last closed Payroll Date this is done based on the new Salary Profile.
                        dtPyrlClose = Convert.ToDateTime((FinMast.IsNull("LastPaidDate") ? new System.DateTime(1900, 1, 1) : FinMast["LastPaidDate"]));
                        if (dtPyrlClose <= EmptyDate)
                            ErrorCodes += "FT0052@";

                        //SP and Financial Upgrades not allowed to be of effective dates < last processed payroll months cutoff date.
                        //Seetha comment below on 18072021 - As per aziz instruction removing the BR FT0022
                        //bool bChk = false;
                        //for (int nd = 0; nd <= nTotElem - 1; nd++)
                        //{
                        //    if (saElem[nd] == "Salary Profile")
                        //    {
                        //        if (dtTemp < dtPyrlClose)
                        //        {
                        //            ErrorCodes += "FT0022@";
                        //            bChk = true;
                        //            break; // TODO: might not be correct. Was : Exit For
                        //        }
                        //    }
                        //}

                        //bool bChk = false;
                        for (int nd = 0; nd <= nTotElem - 1; nd++)
                        {
                            //Denson comment below part 16/08/2020 Aziz instructed to remove this Business rule.
                            //if (saElem[nd] == "Location")
                            //{
                            //    if (dtTemp < dtPyrlClose.AddDays(-11))
                            //    {
                            //        ErrorCodes += "FT0036@";
                            //        bChk = true;
                            //        break; // TODO: might not be correct. Was : Exit For
                            //    }
                            //}

                            /////////////////////////////////////////////////////////////////////////////////////
                            //For CLients like SBG where there is attendance posted in Site Module
                            ////////////////////////////////////////////////////////////////////////////////////

                            if (saElem[nd] == "Location" & bSiteModule)
                            {
                                string cs_Qry = string.Empty;
                                sTemp = string.Empty;
                                String snVal = "0";
                                short nVal = 0;
                                cs_Qry = "IF EXISTS (SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'Hrs_Cust' AND COLUMN_NAME = 'EmpGrp') SELECT 1 Val ELSE SELECT 0 AS VAL";
                                RetVal = ConnectionFunctions.Connect_SQLScalar(ref snVal, cs_Qry, ref ErrMsg);
                                nVal = Convert.ToInt16(snVal);
                                if (nVal == 1)
                                {
                                    nVal = 0;
                                    cs_Qry = "SELECT EmpGrp AS Val FROM Hrs_Cust";
                                    RetVal = ConnectionFunctions.Connect_SQLScalar(ref snVal, cs_Qry, ref ErrMsg);
                                    nVal = Convert.ToInt16(snVal);
                                    if (nVal == 1)
                                    {
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        //return false
                    }

                    //Checking for internal validations in the FinReqMast and FinChange table.
                    lEmpID = Convert.ToInt32(FinMast["EmpID"]);
                    int lSerialNo = 0;

                    sTemp = string.Empty;
                    sTemp2 = string.Empty;

                    Int64 _SrNo = Convert.ToInt64(FinMast["SrNo"]);   //Nitha 09/07/2020 added for financial changes resubmit 

                    // sTemp = "SELECT SrNo FROM FinReqMast WHERE (EmpID = " + lEmpID + " AND (Status < 20 OR Status = 45)) ORDER BY SrNo"; //commented Nitha 09/07/2020 
                    sTemp = "SELECT SrNo FROM FinReqMast WHERE (EmpID = " + lEmpID + " AND (Status < 20 OR Status = 45) and SrNo != " + _SrNo + " ) ORDER BY SrNo";
                    RetVal = ConnectionFunctions.Connect_SQLDataReader(ref MyReader, sTemp, ref ErrMsg);
                    if (RetVal == true & MyReader.HasRows == true)
                    {
                        while (MyReader.Read())
                        {
                            lSerialNo = Convert.ToInt32(MyReader["SrNo"]);
                            if (string.IsNullOrEmpty(sTemp2))
                            {
                                sTemp2 = "SrNo = " + lSerialNo.ToString();
                            }
                            else
                            {
                                string cs_temp = sTemp2;
                                sTemp2 = sTemp2 + " OR SrNo = " + lSerialNo.ToString();
                            }
                        }
                    }

                    if (!MyReader.IsClosed)
                        MyReader.Close();

                    //We should not empty sTemp2, since this contains the where clause from above
                    sTemp = string.Empty;

                    if (string.IsNullOrEmpty(sTemp2))
                        sTemp2 = "SrNo = 0";

                    sTemp = "SELECT * FROM FinChanges WHERE ((" + sTemp2 + ") AND (Status < 20 OR Status = 45)) ORDER BY ReqNo";

                    RetVal = ConnectionFunctions.Connect_SQLDataReader(ref MyReader, sTemp, ref ErrMsg);
                    if (RetVal == true & MyReader.HasRows == true)
                    {
                        while (MyReader.Read())
                        {
                            //Now we check to see if the same component already exists in the Financial
                            //Change Table waiting for approval, in which case we cannot save the new 
                            //element, till the first component is completely approved.
                            sTemp2 = MyReader["Element"].ToString();
                            nTemp = Convert.ToInt16(MyReader["Status"]);
                            if (!string.IsNullOrEmpty(sTemp2))
                            {
                                for (int e = 0; e <= nTotElem - 1; e++)
                                {
                                    if (saElem[e] == sTemp2)
                                        ErrorCodes += "FT0025@";
                                }
                            }

                            //Next we check to see if Sal Profile, Sal Grade, Locations, Category
                            //components already exists in the Financial Change Table waiting for approval
                            //in which case we cannot save the new element, till these components are completely approved.
                            sTemp2 = MyReader["Element"].ToString();
                            nTemp = 0;

                            // Shyamjith Commented on 16/12/1019  as per Aziz's Instruction as Babtain was not able to post ticket entitlment change request because there were salary grade and Category change under approval
                            //if (sTemp2 == "Salary Profile")
                            //{
                            //    ErrorCodes += "FT0028@";
                            //}
                            //else if (sTemp2 == "Location")
                            //{
                            //    ErrorCodes += "FT0029@";
                            //}
                            //else if (sTemp2 == "Salary Grade")
                            //{
                            //    ErrorCodes += "FT0030@";
                            //}
                            //else if (sTemp2 == "Category")
                            //{
                            //    ErrorCodes += "FT0031@";
                            //}
                            // End Shyamjith Commented on 16/12/1019  as per Aziz's Instruction as Babtain was not able to post ticket entitlment change request because there were salary grade and Category change under approval
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
            }
        }

        public static bool OnCheck(string sTemp, string sTem)
        {
            sTem = sTem.Trim();
            if (bFirstTime)
            {
                bFirstTime = false;
                sBuf = sTem;
                if (sTem != sTemp)
                    return true;
                else
                    return false;
            }
            else
            {
                if (sBuf == sTem)
                {
                    return false;
                }
                else
                {
                    if (sTem != sTemp)
                        return true;
                    else
                        return false;
                }
            }
        }


   
    }
}
