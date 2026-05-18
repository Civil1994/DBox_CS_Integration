using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Data.SqlClient;
using System.Collections;
using DBox_CS.Core.HCMS.Entity;
using DBox_CS.Core.DALayer;

namespace DBox_CS.Core.AppClass
{
     public class workflow
    {
        string glbEmpId = string.Empty;   //Added By Alagar for Employee as an Actor on 30/12/2021
        //Seetha Added 15/07/2020
        string mulSelWorkFlow = string.Empty;
        public bool GenerateWorkFlow(Hashtable ModuleData, DataTable TranTable, ref ArrayList Appr, string strConn, string ReqID, ref bool ByPassed, ref string ErrMsg, int EmpId, ref  String WfCode, string LocFilter = "NoFilter")
        {
            //End Added By Alagar for Employee as an Actor on 30/12/2021
            if(TranTable != null)
            {
                if(TranTable.Rows.Count > 0)
                {
                    if (TranTable.Columns.Contains("EmpID"))
                        glbEmpId = TranTable.Rows[0]["EmpID"].ToString().Trim();
                    else
                        glbEmpId = string.Empty;
                }
            }
            //End Added By Alagar for Employee as an Actor on 30/12/2021
            bool RetVal = true;
            SqlConnection sqlConnection = new SqlConnection(strConn);
            try
            {
                string Code = string.Empty;
                string _appr = string.Empty;
                //Nishad Added 29072013
                bool bRepHierarchy = false;
                int iNoofRepLevel = 0;
               
                //RetVal = GetWorkFlowCode(Convert.ToInt16(ModuleData["ViewNo"]), Convert.ToString(ModuleData["TranTableName"]), ref TranTable, ref Code, ref ByPassed, strConn, ref ErrMsg);
                RetVal = GetWorkFlowCode(Convert.ToInt16(ModuleData["ViewNo"]), Convert.ToString(ModuleData["TranTableName"]), ref TranTable, ref Code, ref ByPassed, strConn, ref ErrMsg, ref bRepHierarchy, ref iNoofRepLevel);
                //Nishad End 29072013
                if (RetVal == false)
                {
                    return false; // TODO: might not be correct. Was : Exit Try 
                }
                ArrayList AuthPersons = null;

                if (Code == "Error")
                {
                    ErrMsg = "The System Could Not determine the Approval Flow for your Request. Please contact the Software Administrator for further assistance..";
                    //ConnectionFunctions.LogError((int)ConnectionFunctions.ErrorMessageTypes.Error, methodName: "GenerateWorkFlow", fileName: "Workflow.cs", viewNo: Convert.ToInt32(ModuleData["ViewNo"]), empId: EmpId, userId: ReqID, errorMessage: "The System Could Not determine the Approval Flow for your Request. Please contact the Software Administrator for further assistance.@No WorkFlow Found / WorkFlow Conflict (Function:GetWorkFlowCode)");

                    RetVal = false;
                    return false; // TODO: might not be correct. Was : Exit Try 
                }
                else if (Code == "MultipleError")
                {
                    //Seetha Added 15/07/2020
                    mulSelWorkFlow = "Workflow Codes Are : " + mulSelWorkFlow;
                    ErrMsg = "There is more than one Work Flow defined for this transaction." + mulSelWorkFlow;
                    RetVal = false;
                    return false;
                }
                else
                {

                    WfCode = Code;
                    string App = string.Empty;
                    string AppDate = string.Empty;


                    //Chnage for DBS SFI Service 08-04-2025:TO MAKE THE APP VALUE SAME AS of a Normal Request commented the below by pass condition


                    ////Check for ByPass 
                    //if (ByPassed == true)
                    //{
                    //    Appr.Add("@1" + ReqID + "@");
                    //    Appr.Add("@1#" + DateTime.Now.Date.ToString("dd/MM/yyyy") + "@");
                    //    Appr.Add(1);
                    //    //No Of Approval Authorities 
                    //    Appr.Add("");
                    //    Appr.Add(Code);
                    //}
                    //else if (ByPassed == false && bRepHierarchy == false)
                    if (bRepHierarchy == false)
                    {
                        if (!string.IsNullOrEmpty(LocFilter))
                        {
                            if (LocFilter.ToUpper().Equals("N"))
                            {
                                RetVal = GetAuthPersonsWithLocFilter(Code,"N", ref AuthPersons, strConn, ref ErrMsg);
                                if (RetVal == false)
                                {
                                    return false; // TODO: might not be correct. Was : Exit Try 
                                }
                            }
                            else if (LocFilter.ToUpper().Equals("O"))
                            {
                                RetVal = GetAuthPersonsWithLocFilter(Code, "O", ref AuthPersons, strConn, ref ErrMsg);
                                if (RetVal == false)
                                {
                                    return false; // TODO: might not be correct. Was : Exit Try 
                                }
                            }
                            else
                            {
                                RetVal = GetAuthPersons(Code, ref AuthPersons, strConn, ref ErrMsg);
                                if (RetVal == false)
                                {
                                    return false; // TODO: might not be correct. Was : Exit Try 
                                }
                            }
                        }
                        else
                        {
                            RetVal = GetAuthPersons(Code, ref AuthPersons, strConn, ref ErrMsg);
                            if (RetVal == false)
                            {
                                return false; // TODO: might not be correct. Was : Exit Try 
                            }
                        }
                        

                        //If a Valid Approval Flow is found it returns arraylist 
                        ArrayList Loclib_Sp = new ArrayList();
                        if(TranTable.Columns.Contains("LastLoc"))
                            Loclib_Sp.Add(TranTable.Rows[0]["LastLoc"].ToString().Trim());
                        else
                            Loclib_Sp.Add(TranTable.Rows[0]["LocLibID"].ToString().Trim());
                        Loclib_Sp.Add(TranTable.Rows[0]["SalProfile"].ToString().Trim());

                        ArrayList Users = null;
                        RetVal = ValidateAuthPersons(AuthPersons, Loclib_Sp, Convert.ToByte(ModuleData["LocLibLevels"]), Convert.ToString(ModuleData["ModuleCode"]), ref Users, strConn, ref ErrMsg);
                        if (RetVal == false)
                        {
                            return false; // TODO: might not be correct. Was : Exit Try 
                        }
                        
                        Int16 Ctr = 0;
                        Int16 Ctr1 = 0;
                        for (Ctr = 1; Ctr <= Convert.ToInt16(Users.Count); Ctr++)
                        {
                            App = App + "@" + Ctr + Convert.ToString(Users[Ctr1]);

                            //Chnage for DBS SFI Service 08-04-2025:
                            if(ByPassed)
                            {
                                AppDate = AppDate + "@" + Ctr + "#" + DateTime.Now.Date.ToString("dd/MM/yyyy");
                            }
                            else
                            {
                                AppDate = AppDate + "@" + Ctr + "#";
                            }

                            _appr = _appr + "," + Convert.ToString(Users[Ctr1]);
                            Ctr1 += Convert.ToInt16(1);
                        }

                        Appr.Add(App + "@");
                        Appr.Add(AppDate + "@");
                        Appr.Add(Users.Count);
                        //No Of Approval Authorities 
                        Appr.Add(Users[0]);
                        //NextApprAuth 
                        Appr.Add(Code);
                        
                    }
                    else if(bRepHierarchy == true)
                    {
                        String sQry = String.Empty;
                        String sMessage = String.Empty;
                        Boolean bRetvalue = false;
                        string RptAppOrig = string.Empty;
                        string RptAppDateOrig = string.Empty;
                        string RptAuthPersonCntOrig = string.Empty;
                        ArrayList AuthPersonsOrig = null;

                        string SkipRepHirEnabled = GetSkipRepHirEnabled(WfCode, strConn);   //Added By Alagar for 1st level Hierachy Skip on 20/01/2022

                        //int EmpId = 6;
                        sQry = "EXEC GetEmployeeManagerForApproval " + EmpId + "," + iNoofRepLevel.ToString();
                        
                        SqlCommand MyCommand = new SqlCommand(sQry);
                        MyCommand.Connection = sqlConnection;
                        sqlConnection.Open();
                        SqlDataAdapter MyAdapter = new SqlDataAdapter();
                        MyAdapter.SelectCommand = MyCommand;
                        DataTable dtRepManagers = new DataTable();
                        MyAdapter.Fill(dtRepManagers);
                        MyCommand.Dispose();
                        MyAdapter.Dispose();

                        DataRow dRow = dtRepManagers.Rows[0];                        
                     
                        bRetvalue = Convert.ToBoolean(dRow["RetValue"]);
                        int noOfAppr = Convert.ToInt32(dRow["NoofAppr"]);

                        //Seetha commented the retvalue true check as discussed with aziz to allow the posting even if there is one manager assigned (Rep hierarnchy with multiple level setup) 
                        //if (bRetvalue == true)
                        if (noOfAppr > 0)
                        {
                            //RetVal = GetAuthPersons(Code, ref AuthPersons, strConn, ref ErrMsg);
                            //if (RetVal == false)
                            //{
                            //    return false; // TODO: might not be correct. Was : Exit Try 
                            //}
                            if (!string.IsNullOrEmpty(LocFilter))
                            {
                                if (LocFilter.ToUpper().Equals("N"))
                                {
                                    RetVal = GetAuthPersonsWithLocFilter(Code, "N", ref AuthPersons, strConn, ref ErrMsg);
                                    if (RetVal == false)
                                    {
                                        return false; // TODO: might not be correct. Was : Exit Try 
                                    }
                                }
                                else if (LocFilter.ToUpper().Equals("O"))
                                {
                                    RetVal = GetAuthPersonsWithLocFilter(Code, "O", ref AuthPersons, strConn, ref ErrMsg);
                                    if (RetVal == false)
                                    {
                                        return false; // TODO: might not be correct. Was : Exit Try 
                                    }
                                }
                                else
                                {
                                    RetVal = GetAuthPersons(Code, ref AuthPersons, strConn, ref ErrMsg);
                                    if (RetVal == false)
                                    {
                                        return false; // TODO: might not be correct. Was : Exit Try 
                                    }
                                }
                            }
                            else
                            {
                                RetVal = GetAuthPersons(Code, ref AuthPersons, strConn, ref ErrMsg);
                                if (RetVal == false)
                                {
                                    return false; // TODO: might not be correct. Was : Exit Try 
                                }
                            }
                            if (AuthPersons.Count > 1)
                            {
                                string RptApp = dtRepManagers.Rows[0]["APPROVALS"].ToString();
                                string RptAppDate = dtRepManagers.Rows[0]["APPDATES"].ToString();
                                string RptAuthPersonCnt = dtRepManagers.Rows[0]["NoofAppr"].ToString();
                                string RptNextAuthPerson = dtRepManagers.Rows[0]["NextApprauth"].ToString();
                                string RptPrevAuthPerson = dtRepManagers.Rows[0]["PrevApprauth"].ToString();

                                RptAppOrig = RptApp;
                                RptAppDateOrig = RptAppDate;
                                RptAuthPersonCntOrig = RptAuthPersonCnt;
                                AuthPersonsOrig = (ArrayList) AuthPersons.Clone();

                                //Seetha 08082021 - Added for Reporting hierarchy should stop in some level with job title as per workflow setup defined
                                RetVal = GetFinalizedAuthPersonForRepHierarchy(Code,ref RptApp, ref RptAppDate, ref RptAuthPersonCnt, strConn, ref ErrMsg);
                                if (RetVal == false)
                                {
                                    return false; // TODO: might not be correct. Was : Exit Try 
                                }

                                if(!string.IsNullOrEmpty(RptApp))
                                {
                                    //If a Valid Approval Flow is found it returns arraylist 
                                    ArrayList Loclib_Sp = new ArrayList();
                                    if (TranTable.Columns.Contains("LastLoc"))
                                        Loclib_Sp.Add(TranTable.Rows[0]["LastLoc"].ToString().Trim());
                                    else
                                        Loclib_Sp.Add(TranTable.Rows[0]["LocLibID"].ToString().Trim());
                                    Loclib_Sp.Add(TranTable.Rows[0]["SalProfile"].ToString().Trim());

                                    int isFirstReporting = 0;
                                    string firstAppr = AuthPersons[0].ToString();
                                    if ((firstAppr == "100"))
                                    {
                                        isFirstReporting = 1;
                                    }

                                    //Seetha 09112021 - Copied logic from HCMS
                                    //if ((isFirstReporting == 1))
                                    //{
                                    //    AuthPersons.RemoveAt(0);
                                    //    // Extra Added
                                    //}
                                    //else
                                    //{
                                    //    AuthPersons.RemoveAt((AuthPersons.Count - 1));
                                    //}

                                    int j = 0;
                                    int ReportLevel = 0;
                                    string ReportAppr = "";
                                    var loopTo = AuthPersons.Count - 1;
                                    for (j = 0; j <= loopTo; j++)
                                    {
                                        ReportAppr = AuthPersons[j].ToString();
                                        if (ReportAppr == "100")
                                        {
                                            ReportLevel = j;
                                            break;
                                        }
                                    }

                                    //Remove all Reporting hierarchy before AST validation
                                    if (AuthPersons.Count > 0)
                                    {
                                        while (AuthPersons.Contains("100"))
                                        {
                                            AuthPersons.Remove("100");
                                        }
                                    }

                                    ArrayList Users = null;
                                    RetVal = ValidateAuthPersons(AuthPersons, Loclib_Sp, Convert.ToByte(ModuleData["LocLibLevels"]), Convert.ToString(ModuleData["ModuleCode"]), ref Users, strConn, ref ErrMsg);
                                    if (RetVal == false)
                                    {
                                        return false; // TODO: might not be correct. Was : Exit Try 
                                    }

                                    string PreviousAuthPerson = "";
                                    Int32 AuthPersonCnt = 0;
                                    Int16 Ctr = 0;
                                    Int16 Ctr1 = 0;


                                    // Start Shyamjith Added 30/01/2018
                                    if (isFirstReporting == 1)
                                    {
                                        int _Status = 0;
                                        AuthPersonCnt = Convert.ToInt16(RptAuthPersonCnt);

                                        //Start Added By Alagar for 1st level Hierachy Skip on 20/01/2022
                                        string NextAuth = RptNextAuthPerson;
                                        if (SkipRepHirEnabled == "True" && Convert.ToInt16(RptAuthPersonCntOrig) > 1 && AuthPersonCnt > 1)
                                        {
                                            char[] Delimiter = { '@' };
                                            ArrayList arrApproval = new ArrayList();
                                            arrApproval.AddRange(RptApp.Split(Delimiter));
                                            arrApproval = FilterApprovalUsers(arrApproval);
                                            RptApp = "@";
                                            RptAppDate = "@";
                                            RptNextAuthPerson = "";
                                            for (j = 0; j < AuthPersonCnt; j++)
                                            {
                                                if (j > 0)
                                                {
                                                    if (j == 1)
                                                    {
                                                        RptNextAuthPerson = (String)arrApproval[j];
                                                    }
                                                    RptApp = RptApp + j + (String)arrApproval[j] + "@";
                                                    RptAppDate = RptAppDate + j + "#@";
                                                }
                                            }
                                            AuthPersonCnt = AuthPersonCnt - 1;
                                        }
                                        else if(SkipRepHirEnabled == "True" && Convert.ToInt16(RptAuthPersonCntOrig) > 1 && AuthPersonCnt <= 1)
                                        {
                                            ErrMsg = "Approval Hierarchy stopped in 1st level so can not skip the 1st level.";
                                            return false;
                                        }
                                        //End Added By Alagar for 1st level Hierachy Skip on 20/01/2022

                                        App = RptApp;
                                        AppDate = RptAppDate;
                                        PreviousAuthPerson = RptPrevAuthPerson;
                                        //End Shyamjith Added 30/01/2018
                                        for (Ctr = 1; Ctr <= Convert.ToInt16(Users.Count); Ctr++)
                                        {
                                        	//Start Added By Alagar for 1st level Hierachy Skip on 20/01/2022
                                            if(Ctr ==1 && RptNextAuthPerson == "")
                                            {
                                                RptNextAuthPerson = Convert.ToString(Users[Ctr1]);
                                            }
                                        	//End Added By Alagar for 1st level Hierachy Skip on 20/01/2022
                                            AuthPersonCnt++;
                                            App = App + AuthPersonCnt + Convert.ToString(Users[Ctr1]) + "@";
                                            AppDate = AppDate + AuthPersonCnt + "#" + "@";
                                            PreviousAuthPerson = Convert.ToString(Users[Ctr1]);

                                            Ctr1 += Convert.ToInt16(1);
                                        }
                                        //Start Added By Alagar for 1st level Hierachy Skip on 20/01/2022
                                        if(RptNextAuthPerson == "")
                                        {
                                            RptNextAuthPerson = NextAuth;
                                        }
                                        //End Added By Alagar for 1st level Hierachy Skip on 20/01/2022
                                        Appr.Add(App);
                                        Appr.Add(AppDate);
                                        Appr.Add(AuthPersonCnt);//No Of Approval Authorities 
                                        Appr.Add(RptNextAuthPerson);//NextApprAuth
                                        Appr.Add(Code);
                                        //Start Added By Alagar for 1st level Hierachy Skip on 20/01/2022
                                        if (SkipRepHirEnabled == "True" && Convert.ToInt16(RptAuthPersonCntOrig) > 1 && Convert.ToInt16(RptAuthPersonCnt) > 1)
                                            _Status = Convert.ToInt16(RptAuthPersonCntOrig) + Convert.ToInt16(Users.Count) - AuthPersonCnt;
                                        else
                                            _Status = 0;
                                        //End Added By Alagar for 1st level Hierachy Skip on 20/01/2022
                                        if (!SaveApprRepHierarchyAppDetails(Convert.ToInt32(ModuleData["ViewNo"]), Convert.ToInt32(TranTable.Rows[0]["ReqNo"].ToString().Trim()), EmpId, ModuleData, TranTable, iNoofRepLevel, RptAppOrig, RptAppDateOrig, RptAuthPersonCntOrig, AuthPersonsOrig, Code, strConn, ref ErrMsg, ReqID, _Status, RptNextAuthPerson))
                                        {
                                            ErrMsg = "Error Occured while inserting the approval details for direct managers";
                                        }
                                    }
                                    else
                                    {

                                        //End Shyamjith Added 30/01/2018
                                        for (Ctr = 1; Ctr <= Convert.ToInt16(Users.Count); Ctr++)
                                        {
                                            if ((Ctr - 1) == ReportLevel)
                                            {
                                                int i = 0;
                                                for (i = Convert.ToInt32(RptAuthPersonCnt); i >= 1; i--)
                                                {
                                                    RptApp = RptApp.Replace(("@" + i.ToString()), ("@"
                                                                    + ((i + AuthPersonCnt).ToString())));
                                                    RptAppDate = RptAppDate.Replace(("@"
                                                                    + (i.ToString() + "#")), ("@"
                                                                    + (((i + AuthPersonCnt).ToString())
                                                                    + "#")));
                                                }

                                                AuthPersonCnt = AuthPersonCnt + Convert.ToInt16(RptAuthPersonCnt);
                                                App = App + RptApp.Substring(0, RptApp.Length - 1);
                                                AppDate = AppDate + RptAppDate.Substring(0, RptAppDate.Length - 1);
                                            }

                                            AuthPersonCnt++;
                                            App = App + "@" + AuthPersonCnt + Convert.ToString(Users[Ctr1]);
                                            AppDate = AppDate + "@" + AuthPersonCnt + "#";
                                            Ctr1 += Convert.ToInt16(1);
                                        }

                                        if (ReportLevel == (Ctr - 1)) // Nishad added 04102021 
                                        {
                                            int i = 0;
                                            for (i = Convert.ToInt32(RptAuthPersonCnt); i >= 1; i--)
                                            {
                                                RptApp = RptApp.Replace(("@" + i.ToString()), ("@"
                                                                + ((i + AuthPersonCnt).ToString())));
                                                RptAppDate = RptAppDate.Replace(("@"
                                                                + (i.ToString() + "#")), ("@"
                                                                + (((i + AuthPersonCnt).ToString())
                                                                + "#")));
                                            }

                                            AuthPersonCnt = AuthPersonCnt + Convert.ToInt16(RptAuthPersonCnt);
                                            App = App + RptApp.Substring(0, RptApp.Length - 1);
                                            AppDate = AppDate + RptAppDate.Substring(0, RptAppDate.Length - 1);
                                        }

                                        App = App + "@";
                                        AppDate = AppDate + "@";

                                        //AuthPersonCnt = AuthPersonCnt + Convert.ToInt16(RptAuthPersonCnt);
                                        //App = App + RptApp;
                                        //AppDate = AppDate + RptAppDate;

                                        Appr.Add(App);
                                        Appr.Add(AppDate);
                                        Appr.Add(AuthPersonCnt);//No Of Approval Authorities 
                                        Appr.Add(Users[0]);//NextApprAuth
                                        Appr.Add(Code);

                                        if (!SaveApprRepHierarchyAppDetails(Convert.ToInt32(ModuleData["ViewNo"]), Convert.ToInt32(TranTable.Rows[0]["ReqNo"].ToString().Trim()), EmpId, ModuleData, TranTable, iNoofRepLevel, RptAppOrig, RptAppDateOrig, RptAuthPersonCntOrig, AuthPersonsOrig, Code, strConn, ref ErrMsg, ReqID, 0, ""))
                                        {
                                            ErrMsg = "Error Occured while inserting the approval details for direct managers";
                                        }
                                    }
                                }
                                else
                                {
                                    ByPassed = true;
                                    Appr.Add("@1" + ReqID + "@");
                                    Appr.Add("@1#" + DateTime.Now.Date.ToString("dd/MM/yyyy") + "@");
                                    Appr.Add(1);
                                    //No Of Approval Authorities 
                                    Appr.Add("");
                                    Appr.Add(Code);
                                }
                            }
                            else
                            {
                                string RptApp = dtRepManagers.Rows[0]["APPROVALS"].ToString();
                                string RptAppDate = dtRepManagers.Rows[0]["APPDATES"].ToString();
                                string RptAuthPersonCnt = dtRepManagers.Rows[0]["NoofAppr"].ToString();
                                string RptNextAuthPerson = dtRepManagers.Rows[0]["NextApprauth"].ToString();
                                string RptPrevAuthPerson = dtRepManagers.Rows[0]["PrevApprauth"].ToString();

                                RptAppOrig = RptApp;
                                RptAppDateOrig = RptAppDate;
                                RptAuthPersonCntOrig = RptAuthPersonCnt;
                                AuthPersonsOrig = (ArrayList)AuthPersons.Clone();

                                //Seetha 08082021 - Added for Reporting hierarchy should stop in some level with job title as per workflow setup defined
                                RetVal = GetFinalizedAuthPersonForRepHierarchy(Code,ref RptApp, ref RptAppDate, ref RptAuthPersonCnt, strConn, ref ErrMsg);
                                if (RetVal == false)
                                {
                                    return false; // TODO: might not be correct. Was : Exit Try 
                                }

                                Appr.Add(dRow["APPROVALS"].ToString());
                                Appr.Add(dRow["APPDATES"].ToString());
                                Appr.Add(dRow["NoofAppr"].ToString());
                                //Appr.Add(Users.Count) 'No Of Approval Authorities
                                Appr.Add(dRow["NextApprauth"].ToString()); //NextApprAuth
                                Appr.Add(Code);

                                if (!string.IsNullOrEmpty(RptApp))
                                {
                                    Appr.Add(RptApp);
                                    Appr.Add(RptAppDate);
                                    Appr.Add(RptAuthPersonCnt);
                                    //Appr.Add(Users.Count) 'No Of Approval Authorities
                                    Appr.Add(RptNextAuthPerson); //NextApprAuth
                                    Appr.Add(Code);

                                    if (!SaveApprRepHierarchyAppDetails(Convert.ToInt32(ModuleData["ViewNo"]), Convert.ToInt32(TranTable.Rows[0]["ReqNo"].ToString().Trim()), EmpId, ModuleData, TranTable, iNoofRepLevel, RptAppOrig, RptAppDateOrig, RptAuthPersonCntOrig, AuthPersonsOrig, Code, strConn, ref ErrMsg, ReqID, 0, ""))
                                    {
                                        ErrMsg = "Error Occured while inserting the approval details for direct managers";
                                    }
                                }
                                else //If rptapp empty then consider this as bypassed
                                {
                                    Appr.Add("@1" + ReqID + "@");
                                    Appr.Add("@1#" + DateTime.Now.Date.ToString("dd/MM/yyyy") + "@");
                                    Appr.Add(1);
                                    //No Of Approval Authorities 
                                    Appr.Add("");
                                    Appr.Add(Code);
                                    ByPassed = true;
                                }
                            }
                        }
                        else
                        {
                            sMessage = dRow["Message"].ToString();
                            RetVal = false;
                            //ErrMsg = "The System Could Not Generate the Approval Flow for your Request. <br>  Please contact the Software Administrator for further assistance. <br> " + sMessage;
                            ErrMsg = "Please contact the Software Administrator for further assistance. <br> " + sMessage;
                        }
                    }

                    Appr.Add(Code);
                    Appr.Add(_appr.Trim(','));
                    //Start Added By Alagar for ApprProcess TimeLine
                    if (Convert.ToInt32(ModuleData["ViewNo"]) != 854)
                    {
                        int _ReqNo = 0;
                        if (Convert.ToInt32(ModuleData["ViewNo"]) == 116)
                            _ReqNo = Convert.ToInt32(TranTable.Rows[0]["RecordNo"].ToString().Trim());
                        else if(TranTable.Columns.Contains("ReqNo"))
                            _ReqNo = Convert.ToInt32(TranTable.Rows[0]["ReqNo"].ToString().Trim());

                        if(_ReqNo > 0 && RetVal == true)
                        {
                            bool RetVall = ApprTimeLineSave(Convert.ToInt32(ModuleData["ViewNo"]), _ReqNo, DateTime.Now, "Posted", strConn);
                            if (RetVall == false)
                            {
                                RetVal = false;
                                ErrMsg = "System was unable to make an entry in the ApprProcessExpiryTimeLine table";
                            }
                        }
                    }
                    //End Added By Alagar for ApprProcess TimeLine
                }

            }

            catch (Exception Ex)
            {
                RetVal = false;
                ErrMsg = "The System Could Not Generate the Approval Flow for your Request. Please contact the Software Administrator for further assistance.";
                //ConnectionFunctions.LogError((int)ConnectionFunctions.ErrorMessageTypes.Error, methodName: "GenerateWorkFlow", fileName: "Workflow.cs", viewNo: Convert.ToInt32(ModuleData["ViewNo"]), empId: EmpId, userId: ReqID, errorMessage: "The System Could Not Generate the Approval Flow for your Request. Please contact the Software Administrator for further assistance.@" + Ex.Message + "",ex:Ex);
            }
            finally
            {
                if (sqlConnection.State != 0)
                    sqlConnection.Close();
            }

            return RetVal;

        }




        public bool GenerateWorkFlow(Hashtable ModuleData, DataTable TranTable, ref ArrayList Appr, string strConn, string ReqID, ref bool ByPassed, ref string ErrMsg, int EmpId, string LocFilter = "NoFilter")
        {
            string wfCode = "";
            return this.GenerateWorkFlow(ModuleData, TranTable, ref Appr, strConn, ReqID, ref ByPassed, ref ErrMsg, EmpId, ref wfCode, LocFilter);
        }
        //Nishad Commented 29072013
        //public bool GetWorkFlowCode(Int16 ViewNo, string strTranTableName, ref DataTable TranTable, ref string Code, ref bool byPassed, string strConn, ref string ErrMsg)
        //{

        //    bool RetVal = true;
        //    try
        //    {
        //        short iNoOfWorkFlows = 0;
        //        short Ctr = 0;
        //        string[] arFPrefix = null;
        //        string ValidExpr = "";
        //        string FieldPrefix = "";
        //        string sFormattedColValue = "";
        //        string FPrefixValue = "";
        //        string WhatToReplace = "";
        //        Hashtable ExtDDF = default(Hashtable);
        //        Hashtable ExtDDFLnk = default(Hashtable);
        //        SqlDataReader MyReader = null;
        //        Common CommonFn = new Common();

        //        SqlConnection sqlConnection = new SqlConnection(strConn);
        //        SqlCommand MyCommand = new SqlCommand("EXEC WEB_GetWorkFlows " + ViewNo);
        //        MyCommand.Connection = sqlConnection;
        //        sqlConnection.Open();
        //        SqlDataAdapter MyAdapter = new SqlDataAdapter();
        //        MyAdapter.SelectCommand = MyCommand;
        //        DataTable WorkFlowTable = new DataTable();
        //        MyAdapter.Fill(WorkFlowTable);
        //        MyCommand.Dispose();
        //        MyAdapter.Dispose();

        //        Int16 i = default(Int16);
        //        //---------------------------------------------------------------------- 
        //        //Get Number of Workflows defined for the Module 
        //        for (i = 0; i <= Convert.ToInt16(WorkFlowTable.Rows.Count - 1); i++)
        //        {
        //            arFPrefix = Convert.ToString(WorkFlowTable.Rows[i]["DistinctFldPrefix"]).Substring(0, (WorkFlowTable.Rows[i]["DistinctFldPrefix"].ToString().Length - 1)).Split(Convert.ToChar("@"));
        //            ValidExpr = Convert.ToString(WorkFlowTable.Rows[i]["WrkFlowQry"]);

        //            //------------------------------------------------------------ 
        //            //Replace the values in the query with the values from the database 
        //            for (Ctr = 0; Ctr <= Convert.ToInt16(arFPrefix.GetLength(0) - 1); Ctr++)
        //            {
        //                //Get the Extended Data for the Field 
        //                ExtDDF = new Hashtable();
        //                RetVal = CommonFn.GetExtDDFEng(strTranTableName, Convert.ToString(arFPrefix.GetValue(Ctr)), ref ExtDDF, strConn, ref ErrMsg);
        //                if (RetVal == false)
        //                {
        //                    return false; // TODO: might not be correct. Was : Exit Try 
        //                }

        //                //Find the Replacement values depending on the DataType 
        //                if (Convert.ToInt16(ExtDDF["FieldType"]) <= 7)
        //                {

        //                    FPrefixValue = Convert.ToString(TranTable.Rows[0][(Convert.ToString(arFPrefix.GetValue(Ctr)))]);
        //                    WhatToReplace = Convert.ToString(ExtDDF["TableName"]) + "." + Convert.ToString(ExtDDF["FieldPrefix"]);
        //                }

        //                else if (Convert.ToInt16(ExtDDF["FieldType"]) >= 14 & Convert.ToInt16(ExtDDF["FieldType"]) <= 21)
        //                {

        //                    ExtDDFLnk = new Hashtable();
        //                    RetVal = CommonFn.GetExtDDFEng(ExtDDF["SecondaryTable"].ToString(), ExtDDF["SecondaryLink"].ToString(), ref ExtDDFLnk, strConn, ref ErrMsg);
        //                    if (RetVal == false)
        //                    {
        //                        return false; // TODO: might not be correct. Was : Exit Try 
        //                    }

        //                    string str1 = Convert.ToString(TranTable.Rows[0][Convert.ToString(ExtDDF["PrimaryTableLink"])]);
        //                    byte b1 = Convert.ToByte(ExtDDFLnk["DataType"]);

        //                    RetVal = CommonFn.GetJointVariant(ref ExtDDF, ref str1, ref b1, ref FPrefixValue, strConn, ref ErrMsg);

        //                    //===================================old code======================== 
        //                    //RetVal = CommonFn.GetJointVariant(ExtDDF, CStr(WorkFlowTable.Rows(i).Item(CStr(ExtDDF.Item("PrimaryTableLink")))), _ 
        //                    // CByte(ExtDDFLnk.Item("DataType")), FPrefixValue, strConn, ErrMsg) 
        //                    //================================================================= 
        //                    if (RetVal == false)
        //                    {
        //                        break; // TODO: might not be correct. Was : Exit Try 
        //                    }

        //                    WhatToReplace = Convert.ToString(ExtDDF["SecondaryTable"]) + "." + Convert.ToString(ExtDDF["FieldPrefix"]);
        //                    ExtDDFLnk.Clear();
        //                }


        //                sFormattedColValue = FPrefixValue;

        //                //Format the Value Depending upon the DataType 
        //                if (string.IsNullOrEmpty(sFormattedColValue) | sFormattedColValue == "NULL")
        //                {
        //                    sFormattedColValue = "NULL";
        //                }
        //                else
        //                {
        //                    switch (Convert.ToInt16(ExtDDF["DataType"]))
        //                    {
        //                        case 0:
        //                            sFormattedColValue = "'" + FPrefixValue + "'";
        //                            break;
        //                        case 1:
        //                            sFormattedColValue = "'" + FPrefixValue + "'";
        //                            break;
        //                        case 2:
        //                            sFormattedColValue = "'" + FPrefixValue + "'";
        //                            break;
        //                        case 3:
        //                            if (Convert.ToBoolean(FPrefixValue) == false)
        //                            {
        //                                sFormattedColValue = "0";
        //                            }
        //                            else
        //                            {
        //                                sFormattedColValue = "1";
        //                            }

        //                            break;
        //                        case 4:
        //                            sFormattedColValue = "CONVERT(DATETIME,'" + Convert.ToDateTime(FPrefixValue).ToString("yyyy/MM/dd H:mm:ss") + "')";
        //                            break;
        //                        case 5:
        //                            break;
        //                        //Value Same 
        //                        case 6:
        //                            break;
        //                        //Value Same 
        //                        case 7:
        //                            break;
        //                        //Value Same 
        //                        case 8:
        //                            break;
        //                        //Value Same 
        //                        case 9:
        //                            break;
        //                        //Value Same 
        //                        case 10:
        //                            break;

        //                    }
        //                }

        //                ValidExpr = ValidExpr.ToUpper();
        //                String WTR = WhatToReplace.ToUpper();
        //                String FCV = sFormattedColValue.ToUpper();
        //                ValidExpr = ValidExpr.Replace(WTR, FCV);
        //                //ValidExpr = Strings.UCase(ValidExpr).Replace(Strings.UCase(WhatToReplace), sFormattedColValue))
        //                ExtDDF.Clear();
        //            }

        //            //----------------------------------------------------- 

        //            Int16 Result = 0;
        //            RetVal = CommonFn.GetResult("IF (" + ValidExpr + ") Select 1 As ColVal ELSE Select 2 As ColVal", ref Result, strConn, ref ErrMsg);
        //            if (RetVal == false)
        //            {
        //                break; // TODO: might not be correct. Was : Exit Try 
        //            }

        //            if (Result == 1)
        //            {
        //                Code = Convert.ToString(WorkFlowTable.Rows[i]["Code"]);
        //                byPassed = Convert.ToBoolean(WorkFlowTable.Rows[i]["Bypassed"]);
        //                iNoOfWorkFlows += Convert.ToInt16(1);
        //            }
        //        }

        //        //--------------------------------------------------------------------- 
        //        WorkFlowTable.Dispose();

        //        if (iNoOfWorkFlows != 1)
        //        {
        //            Code = "Error";
        //        }
        //    }

        //    catch (Exception Ex)
        //    {
        //        ErrMsg = "The System Could Not determine the Approval Flow for your Request. Please contact your HR Department for further assistance.@" + Ex.Message + "(Function:GetWorkFlowCode)";
        //        RetVal = false;
        //    }

        //    return RetVal;

        //}
        //Nishad End Comment 29072013

        //Nishad Added 29072013 (WorkFlow based on Report Line Hierarchy)

        public bool GetWorkFlowCode(Int16 ViewNo, string strTranTableName, ref DataTable TranTable, ref string Code, ref bool byPassed, string strConn, ref string ErrMsg, ref bool bisRepHierarchy, ref int iRepLevel)
        {

            bool RetVal = true;
            try
            {
                short iNoOfWorkFlows = 0;
                short Ctr = 0;
                string[] arFPrefix = null;
                string ValidExpr = "";
                string FieldPrefix = "";
                string sFormattedColValue = "";
                string FPrefixValue = "";
                string WhatToReplace = "";
                string currWFCode = "";
                Hashtable ExtDDF = default(Hashtable);
                Hashtable ExtDDFLnk = default(Hashtable);
                SqlDataReader MyReader = null;
                Common CommonFn = new Common();

                SqlConnection sqlConnection = new SqlConnection(strConn);
                SqlCommand MyCommand = new SqlCommand("EXEC WEB_GetWorkFlows " + ViewNo);
                MyCommand.Connection = sqlConnection;
                sqlConnection.Open();
                SqlDataAdapter MyAdapter = new SqlDataAdapter();
                MyAdapter.SelectCommand = MyCommand;
                DataTable WorkFlowTable = new DataTable();
                MyAdapter.Fill(WorkFlowTable);
                MyCommand.Dispose();
                MyAdapter.Dispose();

                Int16 i = default(Int16);
                //---------------------------------------------------------------------- 
                //Get Number of Workflows defined for the Module 
                for (i = 0; i <= Convert.ToInt16(WorkFlowTable.Rows.Count - 1); i++)
                {
                    arFPrefix = Convert.ToString(WorkFlowTable.Rows[i]["DistinctFldPrefix"]).Substring(0, (WorkFlowTable.Rows[i]["DistinctFldPrefix"].ToString().Length - 1)).Split(Convert.ToChar("@"));
                    ValidExpr = Convert.ToString(WorkFlowTable.Rows[i]["WrkFlowQry"]);
                    currWFCode = Convert.ToString(WorkFlowTable.Rows[i]["Code"]);
                    //------------------------------------------------------------ 
                    //Replace the values in the query with the values from the database 
                    for (Ctr = 0; Ctr <= Convert.ToInt16(arFPrefix.GetLength(0) - 1); Ctr++)
                    {
                        //Get the Extended Data for the Field 
                        ExtDDF = new Hashtable();
                        RetVal = CommonFn.GetExtDDFEng(strTranTableName, Convert.ToString(arFPrefix.GetValue(Ctr)), ref ExtDDF, strConn, ref ErrMsg);
                        if (RetVal == false)
                        {
                            return false; // TODO: might not be correct. Was : Exit Try 
                        }

                        //Find the Replacement values depending on the DataType 
                        if (Convert.ToInt16(ExtDDF["FieldType"]) <= 7 || Convert.ToString(ExtDDF["FieldPrefix"]).Contains("LocLib"))
                        {

                            FPrefixValue = Convert.ToString(TranTable.Rows[0][(Convert.ToString(arFPrefix.GetValue(Ctr)))]);
                            //WhatToReplace = Convert.ToString(ExtDDF["TableName"]) + "." + Convert.ToString(ExtDDF["FieldPrefix"]);
                            //#NEWLOC
                            if (Convert.ToInt16(ExtDDF["FieldType"]) <= 7)
                            {
                                WhatToReplace = Convert.ToString(ExtDDF["TableName"]) + "." + Convert.ToString(ExtDDF["FieldPrefix"]);
                            }
                            if (Convert.ToInt16(ExtDDF["FieldType"]) >= 14 & Convert.ToInt16(ExtDDF["FieldType"]) <= 21)
                            {
                                WhatToReplace = Convert.ToString(ExtDDF["SecondaryTable"]) + "." + Convert.ToString(ExtDDF["FieldPrefix"]);
                            }
                        }

                        else if (Convert.ToInt16(ExtDDF["FieldType"]) >= 14 & Convert.ToInt16(ExtDDF["FieldType"]) <= 21)
                        {

                            ExtDDFLnk = new Hashtable();
                            RetVal = CommonFn.GetExtDDFEng(ExtDDF["SecondaryTable"].ToString(), ExtDDF["SecondaryLink"].ToString(), ref ExtDDFLnk, strConn, ref ErrMsg);
                            if (RetVal == false)
                            {
                                return false; // TODO: might not be correct. Was : Exit Try 
                            }

                            string str1 = Convert.ToString(TranTable.Rows[0][Convert.ToString(ExtDDF["PrimaryTableLink"])]);
                            byte b1 = Convert.ToByte(ExtDDFLnk["DataType"]);

                            RetVal = CommonFn.GetJointVariant(ref ExtDDF, ref str1, ref b1, ref FPrefixValue, strConn, ref ErrMsg);

                            //===================================old code======================== 
                            //RetVal = CommonFn.GetJointVariant(ExtDDF, CStr(WorkFlowTable.Rows(i).Item(CStr(ExtDDF.Item("PrimaryTableLink")))), _ 
                            // CByte(ExtDDFLnk.Item("DataType")), FPrefixValue, strConn, ErrMsg) 
                            //================================================================= 
                            if (RetVal == false)
                            {
                                break; // TODO: might not be correct. Was : Exit Try 
                            }

                            WhatToReplace = Convert.ToString(ExtDDF["SecondaryTable"]) + "." + Convert.ToString(ExtDDF["FieldPrefix"]);
                            ExtDDFLnk.Clear();
                        }


                        sFormattedColValue = FPrefixValue;

                        //Format the Value Depending upon the DataType 
                        if (string.IsNullOrEmpty(sFormattedColValue) | sFormattedColValue == "NULL")
                        {
                            //sFormattedColValue = "NULL";  //Nishad Edited 14012024
                            sFormattedColValue = "''";
                        }
                        else
                        {
                            switch (Convert.ToInt16(ExtDDF["DataType"]))
                            {
                                case 0:
                                    sFormattedColValue = "'" + FPrefixValue + "'";
                                    break;
                                case 1:
                                    sFormattedColValue = "'" + FPrefixValue + "'";
                                    break;
                                case 2:
                                    sFormattedColValue = "'" + FPrefixValue + "'";
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
                                    sFormattedColValue = "CONVERT(DATETIME,'" + Convert.ToDateTime(FPrefixValue).ToString("yyyy/MM/dd H:mm:ss") + "')";
                                    break;
                                case 5:
                                    break;
                                //Value Same 
                                case 6:
                                    break;
                                //Value Same 
                                case 7:
                                    break;
                                //Value Same 
                                case 8:
                                    break;
                                //Value Same 
                                case 9:
                                    break;
                                //Value Same 
                                case 10:
                                    break;

                            }
                        }

                        ValidExpr = ValidExpr.ToUpper();
                        String WTR = WhatToReplace.ToUpper();
                        String FCV = sFormattedColValue.ToUpper();
                        ValidExpr = ValidExpr.Replace(WTR, FCV);
                        //ValidExpr = Strings.UCase(ValidExpr).Replace(Strings.UCase(WhatToReplace), sFormattedColValue))
                        ExtDDF.Clear();
                    }

                    //----------------------------------------------------- 

                    Int16 Result = 0;
                    RetVal = CommonFn.GetResult("IF (" + ValidExpr + ") Select 1 As ColVal ELSE Select 2 As ColVal", ref Result, strConn, ref ErrMsg);
                    if (RetVal == false)
                    {
                        ErrMsg = "WorkFlow condition Not defined properly for : " + currWFCode + ", Please contact the Software Administrator for further assistance. " + ErrMsg;
                        break; // TODO: might not be correct. Was : Exit Try 
                    }

                    if (Result == 1)
                    {
                        Code = Convert.ToString(WorkFlowTable.Rows[i]["Code"]);
                        byPassed = Convert.ToBoolean(WorkFlowTable.Rows[i]["Bypassed"]);

                        bisRepHierarchy = Convert.ToBoolean(WorkFlowTable.Rows[i]["RePHiRchyEnabled"]);
                        iRepLevel = Convert.ToInt32(WorkFlowTable.Rows[i]["RePlevel"]);
                        iNoOfWorkFlows += Convert.ToInt16(1);
                        //Seetha Added 15/07/2020
                        if (!string.IsNullOrEmpty(mulSelWorkFlow))
                        {
                            mulSelWorkFlow = mulSelWorkFlow + " , " + Code;
                        }
                        else
                        {
                            mulSelWorkFlow = Code;
                        }

                    }
                }

                //--------------------------------------------------------------------- 
                WorkFlowTable.Dispose();

                if (iNoOfWorkFlows != 1)
                {
                    if (iNoOfWorkFlows > 1)
                    {
                        Code = "MultipleError";
                    }
                    else
                    {
                        Code = "Error";
                    }
                }
            }

            catch (Exception Ex)
            {
                ErrMsg = "The System Could Not determine the Approval Flow for your Request. Please contact the Software Administrator for further assistance..";
                //ConnectionFunctions.LogError((int)ConnectionFunctions.ErrorMessageTypes.Error, methodName: "GenerateWorkFlow", fileName: "Workflow.cs", viewNo: ViewNo, errorMessage: "The System Could Not Generate the Approval Flow for your Request. Please contact the Software Administrator for further assistance.@" + Ex.Message + "", ex: Ex);
                RetVal = false;
            }

            return RetVal;

        }
        //Nishad End 29072013

        public bool GetAuthPersons(string Code, ref ArrayList AuthPersons, string strConn, ref string ErrMsg)
        {

            bool RetVal = true;
            SqlConnection sqlConn = new SqlConnection(strConn);
            try
            {
                string[] strAuthPersons = null;
                SqlCommand MyCommand = new SqlCommand("EXEC WEB_GetAuthPerson '" + Code + "'", sqlConn);
                sqlConn.Open();
                string Temp = Convert.ToString(MyCommand.ExecuteScalar());
                strAuthPersons = Temp.Substring(0, (Temp.Length) - 1).Split(Convert.ToChar("@"));
                AuthPersons = new ArrayList();
                AuthPersons.InsertRange(0, strAuthPersons);
                MyCommand.Dispose();
                sqlConn.Close();
            }

            catch (Exception Ex)
            {
                RetVal = false;
                ErrMsg = "The System Could Not Retrieve Authorized Approval Authorities for your Request. Please contact the Software Administrator for further assistance.";
                //ConnectionFunctions.LogError((int)ConnectionFunctions.ErrorMessageTypes.Error, methodName: "GetAuthPersons", fileName: "Workflow.cs", errorMessage: "The System Could Not Retrieve Authorized Approval Authorities for your Request. Please contact the Software Administrator for further assistance.@" + Ex.Message + "(Function:GetAuthPersons)", ex: Ex);
            }
            finally
            {
                if (sqlConn.State != 0)
                    sqlConn.Close();
            }
            return RetVal;

        }

        /******************************** Start Added By Alagar for 1st level Hierachy Skip on 20/01/2022 ************************************************/
        public string GetSkipRepHirEnabled(string _WFcode, string strConn)
        {
            string SkipRepHirEnabled = "False";
            SqlConnection sqlConn = new SqlConnection(strConn);
            try
            {
                sqlConn.Open();
                SqlCommand myCmdGrd = new SqlCommand("Pr_GetWrkFlowMastOtherSettings", sqlConn);
                myCmdGrd.CommandType = CommandType.StoredProcedure;
                myCmdGrd.Parameters.AddWithValue("@Code", _WFcode);

                using (SqlDataReader read = myCmdGrd.ExecuteReader())
                {
                    while (read.Read())
                    {
                        SkipRepHirEnabled = read["SkipRepHirEnabled"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (sqlConn.State != 0)
                    sqlConn.Close();
            }
            return SkipRepHirEnabled;
        }

        public static ArrayList FilterApprovalUsers(ArrayList fnUsers)
        {
            ArrayList fnFilteredUsers = new ArrayList();
            try
            {
                string User = string.Empty;
                for (short i = 0; i <= fnUsers.Count - 1; i++)
                {
                    if (i <= 9)
                    {
                        User = fnUsers[i].ToString();
                        if (!string.IsNullOrEmpty(User))
                        {
                            User = Utility.General.Mid(User, 2);
                        }
                    }
                    else if (i > 9)
                    {
                        User = fnUsers[i].ToString();
                        if (!string.IsNullOrEmpty(User))
                        {
                            User = Utility.General.Mid(User, 3);
                        }
                    }
                    if (!string.IsNullOrEmpty(User))
                    {
                        fnFilteredUsers.Add(User);
                    }
                }
                return fnFilteredUsers;
            }

            catch (Exception Ex)
            {
                //ErrMsg = "Could Not Filter Approval Authorities [FilterApprovalUsers Failed]@" + Ex.Message;
                //RetVal = false;
            }

            return fnFilteredUsers;

        }
        /******************************** End Added By Alagar for 1st level Hierachy Skip on 20/01/2022 ************************************************/

        //Seetha added 15122021 - Handle auth persons with location filter
        public bool GetAuthPersonsWithLocFilter(string Code, string LocFilter, ref ArrayList AuthPersons, string strConn, ref string ErrMsg)
        {

            bool RetVal = true;
            SqlConnection sqlConn = new SqlConnection(strConn);
            try
            {
                string[] strAuthPersons = null;
                SqlCommand MyCommand = new SqlCommand("EXEC WEB_GetAuthPersonWithLocFilter '" + Code + "','" + LocFilter + "'", sqlConn);
                sqlConn.Open();
                string Temp = Convert.ToString(MyCommand.ExecuteScalar());
                strAuthPersons = Temp.Substring(0, (Temp.Length) - 1).Split(Convert.ToChar("@"));
                AuthPersons = new ArrayList();
                AuthPersons.InsertRange(0, strAuthPersons);
                MyCommand.Dispose();
                sqlConn.Close();
            }

            catch (Exception Ex)
            {
                RetVal = false;
                ErrMsg = "The System Could Not Retrieve Authorized Approval Authorities for your Request. Please contact the Software Administrator for further assistance.";
                //ConnectionFunctions.LogError((int)ConnectionFunctions.ErrorMessageTypes.Error, methodName: "GetAuthPersonsWithLocFilter", fileName: "Workflow.cs", errorMessage: "The System Could Not Retrieve Authorized Approval Authorities for your Request. Please contact the Software Administrator for further assistance.@" + Ex.Message + "(Function:GetAuthPersonsWithLocFilter)", ex: Ex);
            }
            finally
            {
                if (sqlConn.State != 0)
                    sqlConn.Close();
            }
            return RetVal;

        }

        public bool ValidateAuthPersons(ArrayList arAuthPersons, ArrayList Loclib_Sp, byte LocLibLevels, string ModuleCode, ref ArrayList Users, string strConn, ref string ErrMsg)
        {

            bool RetVal = true;
            SqlCommand MyCommand = default(SqlCommand);
            SqlCommand MyCommand1 = default(SqlCommand);    //Added By Alagar for Brief explained message if Rights not assighed properly at 29-11-2022
            SqlConnection sqlConn = new SqlConnection(strConn);
            try
            {
                bool boolVerifyAuthPerson = false;
                //Start Added By Alagar for Brief explained message if Rights not assighed properly at 29-11-2022
                ArrayList strNotAssignedAuthPersons = new ArrayList();
                ArrayList strNotAssignedAuthLocations = new ArrayList();
                ArrayList strNotAssignedSalProfile = new ArrayList();
                //End Added By Alagar for Brief explained message if Rights not assighed properly at 29-11-2022
                Int16 Ctr2 = 0;
                Int16 Ctr = 0;
                //Start Added By Alagar for Employee as Actor on 28/12/2021
                string sQry = string.Empty;
                string AuditString = string.Empty;
                int iResult = 0;
                //End Added By Alagar for Employee as Actor on 28/12/2021
                Users = new ArrayList();

                for (Ctr = 0; Ctr <= Convert.ToInt16(arAuthPersons.Count - 1); Ctr++)
                {
                    boolVerifyAuthPerson = false;
                    //Start Added By Alagar for Employee as Actor on 28/12/2021
                    if (Convert.ToString(arAuthPersons[Ctr]) == "1000" && glbEmpId != string.Empty)
                    {
                        if (glbEmpId != string.Empty)
                        {
                            int EmpId = int.Parse(glbEmpId);
                            string UserId = string.Empty, EISUserID = string.Empty, _ErrMsg = string.Empty;
                            ConnectionFunctions.Connect_SQLScalar(ref UserId, "SELECT Top 1 UserID FROM Security WHERE EmpId = " + EmpId, ref _ErrMsg);
                            if (UserId == "")
                            {
                                ConnectionFunctions.Connect_SQLScalar(ref EISUserID, "SELECT UserID FROM EIS_Security WHERE EmpId = " + EmpId, ref _ErrMsg);
                            }
                            if (UserId == "" && EISUserID == "")
                            {
                                Ctr2 += Convert.ToInt16(1);
                            }
                            else if (UserId == "" && EISUserID != "")
                            {
                                Users.Add(EISUserID);
                                sQry = "SELECT IsNull(EmpID,0) EmpID,IsNull(UserID,'') UserID ,Isnull(Password,'') Password,Isnull(GroupID,'') GroupID ,Isnull(SystemPassword,'') SystemPassword  FROM EIS_SECURITY WHERE EMPID = " + EmpId;
                                SqlDataReader MyReader = null;
                                ConnectionFunctions.Connect_SQLDataReader(ref MyReader, sQry, ref _ErrMsg);
                                if (MyReader.HasRows)
                                {
                                    MyReader.Read();
                                    //Create HCMS user
                                    SqlConnection SQLConn = new SqlConnection(ConnectionFunctions.GetConnectionString());
                                    SqlCommand HCommand = new SqlCommand();
                                    SQLConn.Open();
                                    HCommand.Connection = SQLConn;
                                    HCommand.CommandText = "HCMS_AddUserSave"; // SystemPassword 0
                                    HCommand.CommandType = CommandType.StoredProcedure;
                                    HCommand.Parameters.AddWithValue("@UserID", MyReader["UserID"].ToString());
                                    HCommand.Parameters.AddWithValue("@Password", MyReader["Password"].ToString());
                                    HCommand.Parameters.AddWithValue("@EmpID", MyReader["EmpID"].ToString());
                                    HCommand.Parameters.AddWithValue("@GroupID", MyReader["GroupID"].ToString());
                                    HCommand.Parameters.AddWithValue("@Role", "1");
                                    HCommand.Parameters.AddWithValue("@Status", "0");
                                    HCommand.Parameters.AddWithValue("@new_user", "1");
                                    HCommand.Parameters.AddWithValue("@user_type", "0");
                                    HCommand.Parameters.AddWithValue("@SystemPassword", MyReader["SystemPassword"].ToString());
                                    iResult = HCommand.ExecuteNonQuery();
                                    if (iResult == 0)
                                    {
                                        Ctr2 += Convert.ToInt16(1);
                                    }
                                    else
                                    {
                                        AuditString = "HCMS User Account (" + MyReader["UserID"].ToString() + ") Got Created Reporting Line Hierarchy Module";
                                        Common.AuditSave("Security", "Add Record", EmpId.ToString(), UserId, AuditString, "", 0);
                                        sQry = "Update a set a.new_user = b.new_user,a.secret_question = b.secret_question,a.secret_answer = b.secret_answer,a.UserBlocked =b.UserBlocked  from Security a, EIS_Security b where a.EmpID =b.EmpID   and a.empid =  '" + EmpId + "' ";
                                        ConnectionFunctions.Connect_SQLNonQuery(ref iResult, sQry, ref _ErrMsg);
                                    }
                                    MyReader.Close();
                                }
                                else
                                {
                                    MyReader.Close();
                                    Ctr2 += Convert.ToInt16(1);
                                }
                            }
                            else if (UserId != "")
                            {
                                Users.Add(UserId);
                            }
                        }
                        else
                        {
                            Ctr2 += Convert.ToInt16(1);
                        }
                    }
                    else
                    {   //End Added By Alagar for Employee as Actor on 28/12/2021

                        //Old Location
                        //MyCommand = new SqlCommand("WEB_GetSecRights", sqlConn);
                        //MyCommand.CommandType = CommandType.StoredProcedure;
                        //MyCommand.Parameters.Add(new SqlParameter("@ModuleCode", ModuleCode)).SqlDbType = SqlDbType.VarChar;
                        //MyCommand.Parameters.Add(new SqlParameter("@ApprAuth", Convert.ToString(arAuthPersons[Ctr]))).SqlDbType = SqlDbType.VarChar;
                        //MyCommand.Parameters.Add(new SqlParameter("@LocLibLevels", LocLibLevels)).SqlDbType = SqlDbType.Int;
                        //MyCommand.Parameters.Add(new SqlParameter("@SalProfile", Convert.ToString(Loclib_Sp[5]))).SqlDbType = SqlDbType.VarChar;
                        //MyCommand.Parameters.Add(new SqlParameter("@LocLib1", Convert.ToString(Loclib_Sp[0]))).SqlDbType = SqlDbType.VarChar;
                        //MyCommand.Parameters.Add(new SqlParameter("@LocLib2", Convert.ToString(Loclib_Sp[1]))).SqlDbType = SqlDbType.VarChar;

                        //if (LocLibLevels == 3)
                        //{
                        //    MyCommand.Parameters.Add(new SqlParameter("@LocLib3", Convert.ToString(Loclib_Sp[2]))).SqlDbType = SqlDbType.VarChar;
                        //}
                        //else if (LocLibLevels == 4)
                        //{
                        //    MyCommand.Parameters.Add(new SqlParameter("@LocLib3", Convert.ToString(Loclib_Sp[2]))).SqlDbType = SqlDbType.VarChar;
                        //    MyCommand.Parameters.Add(new SqlParameter("@LocLib4", Convert.ToString(Loclib_Sp[3]))).SqlDbType = SqlDbType.VarChar;
                        //}
                        //else if (LocLibLevels == 5)
                        //{
                        //    MyCommand.Parameters.Add(new SqlParameter("@LocLib3", Convert.ToString(Loclib_Sp[2]))).SqlDbType = SqlDbType.VarChar;
                        //    MyCommand.Parameters.Add(new SqlParameter("@LocLib4", Convert.ToString(Loclib_Sp[3]))).SqlDbType = SqlDbType.VarChar;
                        //    MyCommand.Parameters.Add(new SqlParameter("@LocLib5", Convert.ToString(Loclib_Sp[4]))).SqlDbType = SqlDbType.VarChar;
                        //}

                        //New Location
                        MyCommand = new SqlCommand("Loc_WEB_GetSecRights", sqlConn);
                        MyCommand.CommandType = CommandType.StoredProcedure;
                        MyCommand.Parameters.Add(new SqlParameter("@ModuleCode", ModuleCode)).SqlDbType = SqlDbType.VarChar;
                        MyCommand.Parameters.Add(new SqlParameter("@ApprAuth", Convert.ToString(arAuthPersons[Ctr]))).SqlDbType = SqlDbType.VarChar;
                        MyCommand.Parameters.Add(new SqlParameter("@SalProfile", Convert.ToString(Loclib_Sp[1]))).SqlDbType = SqlDbType.VarChar;
                        MyCommand.Parameters.Add(new SqlParameter("@LocLib5", Convert.ToString(Loclib_Sp[0]))).SqlDbType = SqlDbType.VarChar;

                        if (sqlConn.State == 0) sqlConn.Open();
                        SqlDataReader MyReader = MyCommand.ExecuteReader();

                        Int16 Ctr1 = 0;
                        while (MyReader.Read())
                        {
                            Ctr1 += Convert.ToInt16(1);
                            Users.Add(MyReader[0]);
                        }
                        if (Ctr1 != 1) boolVerifyAuthPerson = false; else boolVerifyAuthPerson = true;
                        //if (boolVerifyAuthPerson == false) Ctr2 += Convert.ToInt16(1);
                        MyReader.Close();
                        MyCommand.Parameters.Clear();
                        //Start Added By Alagar for Brief explained message if Rights not assighed properly at 29-11-2022
                        if (boolVerifyAuthPerson == false)
                        {
                            MyCommand1 = new SqlCommand("SP_GetASTNotAssignedInfo", sqlConn);
                            MyCommand1.CommandType = CommandType.StoredProcedure;
                            MyCommand1.Parameters.Add(new SqlParameter("@AuthCode", Convert.ToString(arAuthPersons[Ctr]))).SqlDbType = SqlDbType.VarChar;
                            MyCommand1.Parameters.Add(new SqlParameter("@LocCode", Convert.ToString(Loclib_Sp[0]))).SqlDbType = SqlDbType.VarChar;
                            MyCommand1.Parameters.Add(new SqlParameter("@SalProfCode", Convert.ToString(Loclib_Sp[1]))).SqlDbType = SqlDbType.VarChar;

                            SqlDataReader MyReader1 = MyCommand1.ExecuteReader();
                            while (MyReader1.Read())
                            {
                                strNotAssignedAuthPersons.Add(MyReader1[0]);
                                strNotAssignedAuthLocations.Add(MyReader1[1]);
                                strNotAssignedSalProfile.Add(MyReader1[2]);
                            }

                            Ctr2 += Convert.ToInt16(1);
                            MyReader1.Close();
                            MyCommand1.Parameters.Clear();
                        }
                        //End Added By Alagar for Brief explained message if Rights not assighed properly at 29-11-2022
                    }
                }

                glbEmpId = string.Empty;


                if (Ctr2 != 0)
                {
                    Users.Clear();
                    Users.Add("Error");
                    //Start Added By Alagar for Brief explained message if Rights not assighed properly at 29-11-2022
                    string MsgAuthorities = "", MsgLocation = "", MsgSalProfile = "";
                    if (strNotAssignedAuthPersons.Count > 0)
                    {
                        for (int ii = 0; ii < strNotAssignedAuthPersons.Count; ii++)
                        {
                            if (ii == 0)
                                MsgAuthorities = "♦ " + strNotAssignedAuthPersons[ii].ToString();
                            else
                                MsgAuthorities = MsgAuthorities + ", " + "♦ " + strNotAssignedAuthPersons[ii].ToString();
                        }
                        MsgLocation = strNotAssignedAuthLocations[0].ToString();
                        MsgSalProfile = strNotAssignedSalProfile[0].ToString();
                    }
                    //End Added By Alagar for Brief explained message if Rights not assighed properly at 29-11-2022
                    ErrMsg = "The Authorities in the Approval Flow " + MsgAuthorities + " do not have the necessary rights to approve your Request for Location " + MsgLocation + " and Salary Profile " + MsgSalProfile
                        + ". Please contact the Software Administrator for further assistance.";
                    //ConnectionFunctions.LogError((int)ConnectionFunctions.ErrorMessageTypes.Error, methodName: "ValidateAuthPersons", fileName: "Workflow.cs", errorMessage: "The Authorities in the Approval Flow do not have the necessary rights to approve your Request. Please contact the Software Administrator for further assistance.@SecRights Missing for AST (Function:ValidateAuthPersons)");
                    RetVal = false;
                    return false;
                }
            }

            catch (Exception Ex)
            {
                RetVal = false;
                ErrMsg = "The System Could Not Retrieve Authorized Approval Authorities for your Request. Please contact the Software Administrator for further assistance.";
                //ConnectionFunctions.LogError((int)ConnectionFunctions.ErrorMessageTypes.Error, methodName: "ValidateAuthPersons", fileName: "Workflow.cs", errorMessage: "The System Could Not Retrieve Authorized Approval Authorities for your Request. Please contact the Software Administrator for further assistance.@" + Ex.Message + "(Function:ValidateAuthPersons)", ex: Ex);
            }
            finally
            {
                if (sqlConn.State != ConnectionState.Closed) sqlConn.Close();
            }

            return RetVal;

        }

        //Seetha 08082021 - Added for Reporting hierarchy should stop in some level with job title as per workflow setup defined
        public bool GetFinalizedAuthPersonForRepHierarchy(string wfCode, ref string RptApp, ref string RptAppDate, ref string RptAuthPersonCnt, string strConn, ref string ErrMsg)
        {
            bool RetVal = true;
            SqlConnection sqlConn = new SqlConnection(strConn);
            try
            {
                string RHJobTitleDef = string.Empty;
                //string sQry = "SELECT RHFinalApprJobTitle FROM ApprFormFields WHERE Wfcode = '" + wfCode + "' and AuthPerson = 100";  //Nishad Edited 04092022
                string sQry = "SELECT RHFinalApprJobTitle FROM ApprFormFields WHERE Wfcode = '" + wfCode + "' and AuthPerson = '100'";

                SqlCommand MyCommand = new SqlCommand(sQry);
                MyCommand.Connection = sqlConn;
                MyCommand.CommandType = CommandType.Text;

                if (sqlConn.State != ConnectionState.Open)
                    sqlConn.Open();

                var tmpJT = MyCommand.ExecuteScalar();
                MyCommand.Dispose();

                if (tmpJT != null && !DBNull.Value.Equals(tmpJT))
                {
                    RHJobTitleDef = tmpJT.ToString();
                    if (!string.IsNullOrEmpty(RHJobTitleDef))
                    {
                        ArrayList saApprLevel = new ArrayList();
                        ArrayList saCodeTmp = new ArrayList();
                        ArrayList newUsers = new ArrayList();

                        if (!Common.DecodeAppr(RptApp, ref saCodeTmp, ref saApprLevel))
                            return false;

                        if (saCodeTmp != null && saCodeTmp.Count > 0)
                        {
                            bool exit = false;
                            foreach (string tmpUsr in saCodeTmp)
                            {
                                if (!exit)
                                {
                                    if (!string.IsNullOrEmpty(tmpUsr))
                                    {
                                        sQry = " SELECT f.JobTitle  FROM security S With (Nolock)  " +
                                               " LEFT OUTER JOIN finmast f With (Nolock) on f.EmpID = s.EmpID " +
                                               " WHERE s.UserID = '" + tmpUsr + "'";

                                        MyCommand = new SqlCommand(sQry);
                                        MyCommand.Connection = sqlConn;
                                        MyCommand.CommandType = CommandType.Text;
                                        var userJT = MyCommand.ExecuteScalar();
                                        MyCommand.Dispose();

                                        string[] strJTList = RHJobTitleDef.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries);

                                        if (strJTList != null && strJTList.Length > 0)
                                        {
                                            foreach (string strJT in strJTList)
                                            {
                                                if (!exit)
                                                {
                                                    if (userJT != null && !DBNull.Value.Equals(userJT))
                                                    {
                                                        if (strJT.Equals(userJT.ToString()))
                                                        {
                                                            //newUsers.Add(tmpUsr);
                                                            exit = true;
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            //newUsers.Add(tmpUsr);
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        if (exit)
                                        {
                                            newUsers.Add(tmpUsr);
                                            break;
                                        }
                                        else
                                        {
                                            newUsers.Add(tmpUsr);
                                        }
                                    }
                                }
                            }

                            Int16 Ctr = 0;
                            Int16 Ctr1 = 0;

                            RptApp = "";
                            RptAppDate = "";

                            if (newUsers != null && newUsers.Count > 0)
                            {
                                for (Ctr = 1; Ctr <= Convert.ToInt16(newUsers.Count); Ctr++)
                                {
                                    RptApp = RptApp + "@" + Ctr + Convert.ToString(newUsers[Ctr1]);
                                    RptAppDate = RptAppDate + "@" + Ctr + "#";
                                    Ctr1 += Convert.ToInt16(1);
                                }

                                RptAuthPersonCnt = newUsers.Count.ToString();

                                RptApp = RptApp + "@";
                                RptAppDate = RptAppDate + "@";
                            }

                        }

                    }
                }
            }

            catch (Exception Ex)
            {
                RetVal = false;
                ErrMsg = "Error Occured while validating the authority person with their job title";
            }
            finally
            {
                if (sqlConn.State != 0)
                    sqlConn.Close();
            }
            return RetVal;
        }

        public bool SaveApprRepHierarchyAppDetails(int ViewNo, int ReqNo, int EmpId, Hashtable ModuleData, DataTable TranTable, int noOfRepLevel, string RptApp, string RptAppDate, string RptAuthPersonCnt, ArrayList AuthPersons, string Code, string strConn, ref string ErrMsg, string reqID, int _Status, string _NextApprAuth)
        {
            string App = string.Empty;
            string AppDate = string.Empty;
            String sQry = String.Empty;
            String sMessage = String.Empty;
            bool bSaveRepHieApp = false;
            SqlConnection sqlConnection = new SqlConnection(strConn);
            ArrayList Appr = new ArrayList();
            bool RetVal = true;

            try
            {

                int iRptAuthPersonCnt = 0;

                if (!string.IsNullOrEmpty(RptAuthPersonCnt))
                {
                    iRptAuthPersonCnt = Convert.ToInt32(RptAuthPersonCnt);
                }

                if (noOfRepLevel != iRptAuthPersonCnt)
                {
                    ArrayList saApprLevel = new ArrayList();
                    ArrayList saCodeTmp = new ArrayList();
                    ArrayList newUsers = new ArrayList();
                    string slastMgr = string.Empty;

                    if (!Common.DecodeAppr(RptApp, ref saCodeTmp, ref saApprLevel))
                        return false;

                    for (int i = 0; i < noOfRepLevel; i++)
                    {
                        if (saCodeTmp.Count > i)
                        {
                            slastMgr = saCodeTmp[i].ToString();
                        }
                        else
                        {
                            iRptAuthPersonCnt++;
                            RptAuthPersonCnt = iRptAuthPersonCnt.ToString();
                            RptApp = RptApp + iRptAuthPersonCnt + slastMgr + "@";
                            RptAppDate = RptAppDate + iRptAuthPersonCnt + "#" + "@";
                        }
                    }
                }

                if (AuthPersons.Count > 1)
                {
                    if (!string.IsNullOrEmpty(RptApp))
                    {
                        //If a Valid Approval Flow is found it returns arraylist 
                        ArrayList Loclib_Sp = new ArrayList();
                        if (TranTable.Columns.Contains("LastLoc"))
                            Loclib_Sp.Add(TranTable.Rows[0]["LastLoc"].ToString().Trim());
                        else
                            Loclib_Sp.Add(TranTable.Rows[0]["LocLibID"].ToString().Trim());
                        Loclib_Sp.Add(TranTable.Rows[0]["SalProfile"].ToString().Trim());

                        int isFirstReporting = 0;
                        string firstAppr = AuthPersons[0].ToString();
                        if ((firstAppr == "100"))
                        {
                            isFirstReporting = 1;
                        }

                        int j = 0;
                        int ReportLevel = 0;
                        string ReportAppr = "";
                        var loopTo = AuthPersons.Count - 1;
                        for (j = 0; j <= loopTo; j++)
                        {
                            ReportAppr = AuthPersons[j].ToString();
                            if (ReportAppr == "100")
                            {
                                ReportLevel = j;
                                break;
                            }
                        }

                        //Remove all Reporting hierarchy before AST validation
                        if (AuthPersons.Count > 0)
                        {
                            while (AuthPersons.Contains("100"))
                            {
                                AuthPersons.Remove("100");
                            }
                        }

                        ArrayList Users = null;
                        RetVal = ValidateAuthPersons(AuthPersons, Loclib_Sp, Convert.ToByte(ModuleData["LocLibLevels"]), Convert.ToString(ModuleData["ModuleCode"]), ref Users, strConn, ref ErrMsg);
                        if (RetVal == false)
                        {
                            return false; // TODO: might not be correct. Was : Exit Try 
                        }

                        string PreviousAuthPerson = "";
                        Int32 AuthPersonCnt = 0;
                        Int16 Ctr = 0;
                        Int16 Ctr1 = 0;


                        // Start Shyamjith Added 30/01/2018
                        if (isFirstReporting == 1)
                        {
                            AuthPersonCnt = Convert.ToInt16(RptAuthPersonCnt);
                            App = RptApp;
                            AppDate = RptAppDate;

                            //End Shyamjith Added 30/01/2018
                            for (Ctr = 1; Ctr <= Convert.ToInt16(Users.Count); Ctr++)
                            {
                                AuthPersonCnt++;
                                App = App + AuthPersonCnt + Convert.ToString(Users[Ctr1]) + "@";
                                AppDate = AppDate + AuthPersonCnt + "#" + "@";
                                PreviousAuthPerson = Convert.ToString(Users[Ctr1]);

                                Ctr1 += Convert.ToInt16(1);
                            }

                            Appr.Add(App);
                            Appr.Add(AppDate);
                            Appr.Add(AuthPersonCnt);//No Of Approval Authorities
                            Appr.Add(Code);
                            bSaveRepHieApp = true;
                        }
                        else
                        {

                            //End Shyamjith Added 30/01/2018
                            for (Ctr = 1; Ctr <= Convert.ToInt16(Users.Count); Ctr++)
                            {
                                if ((Ctr - 1) == ReportLevel)
                                {
                                    int i = 0;
                                    for (i = Convert.ToInt32(RptAuthPersonCnt); i >= 1; i--)
                                    {
                                        RptApp = RptApp.Replace(("@" + i.ToString()), ("@"
                                                        + ((i + AuthPersonCnt).ToString())));
                                        RptAppDate = RptAppDate.Replace(("@"
                                                        + (i.ToString() + "#")), ("@"
                                                        + (((i + AuthPersonCnt).ToString())
                                                        + "#")));
                                    }

                                    AuthPersonCnt = AuthPersonCnt + Convert.ToInt16(RptAuthPersonCnt);
                                    App = App + RptApp.Substring(0, RptApp.Length - 1);
                                    AppDate = AppDate + RptAppDate.Substring(0, RptAppDate.Length - 1);
                                }

                                AuthPersonCnt++;
                                App = App + "@" + AuthPersonCnt + Convert.ToString(Users[Ctr1]);
                                AppDate = AppDate + "@" + AuthPersonCnt + "#";
                                Ctr1 += Convert.ToInt16(1);
                            }

                            if (ReportLevel == (Ctr - 1)) // Nishad added 04102021 
                            {
                                int i = 0;
                                for (i = Convert.ToInt32(RptAuthPersonCnt); i >= 1; i--)
                                {
                                    RptApp = RptApp.Replace(("@" + i.ToString()), ("@"
                                                    + ((i + AuthPersonCnt).ToString())));
                                    RptAppDate = RptAppDate.Replace(("@"
                                                    + (i.ToString() + "#")), ("@"
                                                    + (((i + AuthPersonCnt).ToString())
                                                    + "#")));
                                }

                                AuthPersonCnt = AuthPersonCnt + Convert.ToInt16(RptAuthPersonCnt);
                                App = App + RptApp.Substring(0, RptApp.Length - 1);
                                AppDate = AppDate + RptAppDate.Substring(0, RptAppDate.Length - 1);
                            }

                            App = App + "@";
                            AppDate = AppDate + "@";

                            Appr.Add(App);
                            Appr.Add(AppDate);
                            Appr.Add(AuthPersonCnt);//No Of Approval Authorities 
                            Appr.Add(Users[0]);//NextApprAuth
                            Appr.Add(Code);
                            bSaveRepHieApp = true;
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(RptApp))
                    {
                        Appr.Add(RptApp);
                        Appr.Add(RptAppDate);
                        Appr.Add(RptAuthPersonCnt);
                        Appr.Add(Code);
                        bSaveRepHieApp = true;
                    }
                }

                //Save the details to new table
                if (bSaveRepHieApp)
                {
                    //Start Added By Alagar for Status update
                    if (Appr[0].ToString() != "" && _NextApprAuth != "")
                    {
                        string _Appr = Appr[0].ToString();
                        if (!_Appr.Contains(_Status + 1 + _NextApprAuth))
                        {
                            string[] strApprList = _Appr.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries);
                            int _count = 0;
                            if (strApprList != null && strApprList.Length > 0)
                            {
                                foreach (string strJT in strApprList)
                                {
                                    _count++;
                                    if (strJT != null && !DBNull.Value.Equals(strJT))
                                    {
                                        if (strJT.Contains(_NextApprAuth))
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                            _Status = _count - 1;
                        }
                    }
                    else
                    {
                        _Status = 0;
                    }
                    //End Added By Alagar for Status update

                    ApprRepHierarchyAppDetails oApprRepHierarchyAppDetails = new ApprRepHierarchyAppDetails();
                    oApprRepHierarchyAppDetails.ViewNo = Convert.ToInt16(ViewNo);
                    oApprRepHierarchyAppDetails.ReqNo = ReqNo;
                    oApprRepHierarchyAppDetails.EmpID = EmpId;
                    oApprRepHierarchyAppDetails.App = Appr[0].ToString();
                    oApprRepHierarchyAppDetails.AppDate = Appr[1].ToString();
                    oApprRepHierarchyAppDetails.NoOfAppr = Convert.ToByte(Appr[2]);
                    oApprRepHierarchyAppDetails.Status = Convert.ToByte(_Status);
                    oApprRepHierarchyAppDetails.ReqID = Convert.ToInt32(TranTable.Rows[0]["ReqID"].ToString().Trim());
                    if (TranTable.Columns.Contains("ReqDate"))
                    {
                        if (TranTable.Rows[0]["ReqDate"].ToString() == "")
                            oApprRepHierarchyAppDetails.RequestDate = DateTime.Now;
                        else
                            oApprRepHierarchyAppDetails.RequestDate = Convert.ToDateTime(TranTable.Rows[0]["ReqDate"].ToString());
                    }
                    else
                    {
                        oApprRepHierarchyAppDetails.RequestDate = DateTime.Now;
                    }
                    oApprRepHierarchyAppDetails.GroupNo = 0;

                    RetVal = InsertUpdateApprRepHierarchyAppDetails(oApprRepHierarchyAppDetails, strConn);
                }

            }
            catch (Exception ex)
            {
                RetVal = false;
                ErrMsg = "Error Occured while inserting the approval details for direct managers";
            }

            return RetVal;
        }




        //copied from HCMS.Datalayer.ApprProcessHelper

        public bool ApprTimeLineSave(int iViewNo, int iReqNo, DateTime dtCurr, string strAction, string strConn)
        {
            bool RowsAffected = false;


            SqlConnection sqlConn = new SqlConnection(strConn);
            try
            {
                sqlConn.Open();
                SqlCommand myCmd = new SqlCommand("SP_InsertUpdateTimeLine", sqlConn);
                myCmd.CommandType = CommandType.StoredProcedure;
                myCmd.Parameters.AddWithValue("@ViewNo",  iViewNo);
                myCmd.Parameters.AddWithValue("@ReqNo", iReqNo);
                myCmd.Parameters.AddWithValue("@ModifiedDateTime", dtCurr.ToString("yyyy/MM/dd HH:mm:ss"));
                myCmd.Parameters.AddWithValue("@ActionTaken", strAction);

                int val = myCmd.ExecuteNonQuery();
                if (val == 0) { RowsAffected = false; } else { RowsAffected = true; }
            }
            catch (Exception ex)
            {
                RowsAffected = true;
            }
            finally
            {
                if (sqlConn.State != 0)
                    sqlConn.Close();
            }



            return RowsAffected;
        }


        // copied from HCMS.Datalayer.ApprProcessHelper
        public static bool InsertUpdateApprRepHierarchyAppDetails(ApprRepHierarchyAppDetails Object, string strConn)
        {
            bool RowsAffected = false;

            SqlConnection sqlConn = new SqlConnection(strConn);
            try
            {
                sqlConn.Open();
                SqlCommand myCmd = new SqlCommand("EAF_USP_ApprRepHierarchyAppDetailsInsertUpdate", sqlConn);
                myCmd.CommandType = CommandType.StoredProcedure;

                myCmd.Parameters.AddWithValue("@ViewNo", Object.ViewNo);
                myCmd.Parameters.AddWithValue("@ReqNo", Object.ReqNo);
                myCmd.Parameters.AddWithValue("@RequestDate", Object.RequestDate);
                myCmd.Parameters.AddWithValue("@EmpID", Object.EmpID);
                myCmd.Parameters.AddWithValue("@App", Object.App);
                myCmd.Parameters.AddWithValue("@AppDate", Object.AppDate);
                myCmd.Parameters.AddWithValue("@NoOfAppr", Object.NoOfAppr);
                myCmd.Parameters.AddWithValue("@Status", Object.Status);
                myCmd.Parameters.AddWithValue("@ReqID", Object.ReqID);
                myCmd.Parameters.AddWithValue("@GroupNo", Object.GroupNo);

                int val = myCmd.ExecuteNonQuery();
                if (val == 0) { RowsAffected = false; } else { RowsAffected = true; }
                
            }
            catch (Exception ex)
            {
                RowsAffected = true;
            }
            finally
            {
                if (sqlConn.State != 0)
                    sqlConn.Close();
            }



            return RowsAffected;
        }
    }
}
