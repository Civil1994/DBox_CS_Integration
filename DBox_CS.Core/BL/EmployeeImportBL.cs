using DBox_CS.Core.AppClass;
using DBox_CS.Core.DALayer;
using DBox_CS.Core.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Web;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;

namespace DBox_CS.Core.BL
{
    public class EmployeeImportBL
    {
        //DBOXI Employee Import notes:
        //1. Sponsor chnage not allowed from Employee Update
        //2. 



        StringBuilder sbLineErrMsg;
        StringBuilder sbFileErrMsg;
        int errCount = 0;
        int errTotalCount = 0;
        private bool RetVal = false;
        string errmsg = "";
        String sQry = String.Empty;
        private int result = 0;
        string importProcessId = "";
        StringBuilder sbSaveMsg;

        bool bHasEmpBRerror = false;
        bool bEmpSaved = false;
        bool bEmpSkipped = false;

        string strCurrEmpCode = "";
        int iCurrRowNo = 0;
        bool bCurrHasLineErrors = true;
        int iCurrPLDSrNo = 0;


        Dictionary<string, string> dictTitleNames;
        DataTable dtLookUpFieldsDetails_Emp;

        DataTable mydt;
        //DataTable dtXLData;

        const string xlcol_EmpCode = "EmpCode";
        const string xlcol_EmpNameE = "EmpNameE";
        const string xlcol_FNameE = "FNameE";
        const string xlcol_SNameE = "SNameE";
        const string xlcol_FamilyNameE = "FamilyNameE";
        const string xlcol_Sex = "Sex";
        const string xlcol_MaritalStat = "MaritalStat";
        const string xlcol_NPresent = "NPresent";
        const string xlcol_NPrevious = "NPrevious";
        const string xlcol_Religion = "Religion";
        const string xlcol_DateOfBirth = "DateOfBirth";
        const string xlcol_BirthPlaceE = "BirthPlaceE";
        const string xlcol_CountryOfBirth = "CountryOfBirth";
        const string xlcol_PassportNoE = "PassportNoE";
        const string xlcol_PIssuePlaceE = "PIssuePlaceE";
        const string xlcol_PIssueCountry = "PIssueCountry";
        const string xlcol_PIssueDate = "PIssueDate";
        const string xlcol_PExpiryDate = "PExpiryDate";
        const string xlcol_RelNameE = "RelNameE";
        const string xlcol_NextofKinE = "NextofKinE";
        const string xlcol_MotherNameE = "MotherNameE";
        const string xlcol_MobileNo = "MobileNo";
        const string xlcol_TeleNoAbroad = "TeleNoAbroad";
        const string xlcol_PerAddressE = "PerAddressE";
        const string xlcol_Email = "Email";
        const string xlcol_PassportProf = "PassportProf";
        const string xlcol_StartDtofIndemnity = "StartDtofIndemnity";
        const string xlcol_AuxString3 = "AuxString3";
        const string xlcol_Education = "Education";
        const string xlcol_AuxString4 = "AuxString4";
        const string xlcol_AuxInt3 = "AuxInt3";
        const string xlcol_LocLib5 = "LocLib5";
        const string xlcol_SalProfile = "SalProfile";
        const string xlcol_HealthInsurCmp = "HealthInsurCmp";
        const string xlcol_NextofKinAddrE = "NextofKinAddrE";
        const string xlcol_SponsorCode = "SponsorCode";
        const string xlcol_Title = "Title";
        const string xlcol_EmpNameA = "EmpNameA";
        const string xlcol_NickNameE = "NickNameE";
        const string xlcol_NickNameA = "NickNameA";
        const string xlcol_RelType = "RelType";
        const string xlcol_RelNameA = "RelNameA";
        const string xlcol_MotherNameA = "MotherNameA";
        const string xlcol_FamilyNameA = "FamilyNameA";
        const string xlcol_BirthPlaceA = "BirthPlaceA";
        const string xlcol_PassportNoA = "PassportNoA";
        const string xlcol_PCategory = "PCategory";
        const string xlcol_PIssuePlaceA = "PIssuePlaceA";
        const string xlcol_PerAddressA = "PerAddressA";
        const string xlcol_Skill1 = "Skill1";
        const string xlcol_Skill2 = "Skill2";
        const string xlcol_Skill3 = "Skill3";
        const string xlcol_Language1 = "Language1";
        const string xlcol_Language2 = "Language2";
        const string xlcol_Language3 = "Language3";
        const string xlcol_VisaType = "VisaType";
        const string xlcol_VisaNo = "VisaNo";
        const string xlcol_VisaIssueDate = "VisaIssueDate";
        const string xlcol_ImmigFileNo = "ImmigFileNo";
        const string xlcol_EntryPlace = "EntryPlace";
        const string xlcol_EntryDate = "EntryDate";
        const string xlcol_ResidenceNo = "ResidenceNo";
        const string xlcol_ResIssueDate = "ResIssueDate";
        const string xlcol_ResExpDate = "ResExpDate";
        const string xlcol_ResIssuePlace = "ResIssuePlace";
        const string xlcol_LabCardNo = "LabCardNo";
        const string xlcol_LCIssueDate = "LCIssueDate";
        const string xlcol_LCExpDate = "LCExpDate";
        const string xlcol_HlthCardNo = "HlthCardNo";
        const string xlcol_HCIssuePlace = "HCIssuePlace";
        const string xlcol_HCIssueDate = "HCIssueDate";
        const string xlcol_HCExpiryDate = "HCExpiryDate";
        const string xlcol_DrvLicNo = "DrvLicNo";
        const string xlcol_DLCategory = "DLCategory";
        const string xlcol_DLIssuePlace = "DLIssuePlace";
        const string xlcol_DLIssueDate = "DLIssueDate";
        const string xlcol_DLExpiryDate = "DLExpiryDate";
        const string xlcol_SponByOther = "SponByOther";
        const string xlcol_OSponNameE = "OSponNameE";
        const string xlcol_OSponNameA = "OSponNameA";
        const string xlcol_OSponRel = "OSponRel";
        const string xlcol_OSponNation = "OSponNation";
        const string xlcol_OSponVisaNo = "OSponVisaNo";
        const string xlcol_OSponVExpDt = "OSponVExpDt";
        const string xlcol_OSponPsprtNoE = "OSponPsprtNoE";
        const string xlcol_OSponPsprtNoA = "OSponPsprtNoA";
        const string xlcol_ExperienceE = "ExperienceE";
        const string xlcol_ExperienceA = "ExperienceA";
        const string xlcol_Emirates = "Emirates";
        const string xlcol_City = "City";
        const string xlcol_Area = "Area";
        const string xlcol_StreetE = "StreetE";
        const string xlcol_StreetA = "StreetA";
        const string xlcol_BuildingE = "BuildingE";
        const string xlcol_BuildingA = "BuildingA";
        const string xlcol_FlatE = "FlatE";
        const string xlcol_FlatA = "FlatA";
        const string xlcol_OffPhoneNo = "OffPhoneNo";
        const string xlcol_Ext = "Ext";
        const string xlcol_ResPhoneNo = "ResPhoneNo";
        const string xlcol_POBox = "POBox";
        const string xlcol_PagerNo = "PagerNo";
        const string xlcol_BloodGroup = "BloodGroup";
        const string xlcol_FaxNo = "FaxNo";
        const string xlcol_NextofKinA = "NextofKinA";
        const string xlcol_NextofKinAddrA = "NextofKinAddrA";
        const string xlcol_AddressE = "AddressE";
        const string xlcol_AddressA = "AddressA";
        const string xlcol_AuxString1 = "AuxString1";
        const string xlcol_AuxString2 = "AuxString2";
        const string xlcol_AuxString5 = "AuxString5";
        const string xlcol_AuxString6 = "AuxString6";
        const string xlcol_AuxString7 = "AuxString7";
        const string xlcol_AuxString8 = "AuxString8";
        const string xlcol_AuxString9 = "AuxString9";
        const string xlcol_AuxString10 = "AuxString10";
        const string xlcol_AuxAString1 = "AuxAString1";
        const string xlcol_AuxAString2 = "AuxAString2";
        const string xlcol_AuxAString3 = "AuxAString3";
        const string xlcol_AuxAString4 = "AuxAString4";
        const string xlcol_AuxAString5 = "AuxAString5";
        const string xlcol_AuxAString6 = "AuxAString6";
        const string xlcol_AuxAString7 = "AuxAString7";
        const string xlcol_AuxAString8 = "AuxAString8";
        const string xlcol_AuxAString9 = "AuxAString9";
        const string xlcol_AuxAString10 = "AuxAString10";
        const string xlcol_AuxInt1 = "AuxInt1";
        const string xlcol_AuxInt2 = "AuxInt2";
        const string xlcol_AuxCurrency1 = "AuxCurrency1";
        const string xlcol_AuxCurrency2 = "AuxCurrency2";
        const string xlcol_AuxDate1 = "AuxDate1";
        const string xlcol_AuxDate2 = "AuxDate2";
        const string xlcol_AuxDate3 = "AuxDate3";
        const string xlcol_AuxDate4 = "AuxDate4";
        const string xlcol_AuxDate5 = "AuxDate5";
        const string xlcol_AuxLib1 = "AuxLib1";
        const string xlcol_AuxLib2 = "AuxLib2";
        const string xlcol_AuxLib3 = "AuxLib3";
        const string xlcol_AuxLib4 = "AuxLib4";
        const string xlcol_IntlJoiningDate = "IntlJoiningDate";
        const string xlcol_PointOfHireE = "PointOfHireE";
        const string xlcol_PointOfHireA = "PointOfHireA";
        const string xlcol_SourceOfHire = "SourceOfHire";
        const string xlcol_SLReInitDate = "SLReInitDate";
        const string xlcol_LCIssuePlace = "LCIssuePlace";
        const string xlcol_FNameA = "FNameA";
        const string xlcol_SNameA = "SNameA";
        const string xlcol_GrandFatherE = "GrandFatherE";
        const string xlcol_SponTypeExtnl = "SponTypeExtnl";
        const string xlcol_GrandFatherA = "GrandFatherA";
        const string xlcol_VisaExpDate = "VisaExpDate";
        const string xlcol_AuxLib5 = "AuxLib5";
        const string xlcol_AuxLib6 = "AuxLib6";
        const string xlcol_PersEmail = "PersEmail";
        const string xlcol_NPresentSec = "NPresentSec";
        const string xlcol_ReligionSubSet = "ReligionSubSet";

        const string xlcol_UIDNo = "UIDNo";
        const string xlcol_Disability = "Disability";
        const string xlcol_DisabilityDescE = "DisabilityDescE";
        const string xlcol_DisabilityDescA = "DisabilityDescA";
        const string xlcol_NationalID = "NationalID";

        const string xlcol_WPS = "WPS";

        static string[] EmployeeCols =
        {
            xlcol_EmpCode, xlcol_EmpNameE, xlcol_FNameE, xlcol_SNameE, xlcol_FamilyNameE, xlcol_Sex, xlcol_MaritalStat,
            xlcol_NPresent, xlcol_NPrevious, xlcol_Religion, xlcol_DateOfBirth, xlcol_BirthPlaceE, xlcol_CountryOfBirth,
            xlcol_PassportNoE, xlcol_PIssuePlaceE, xlcol_PIssueCountry, xlcol_PIssueDate, xlcol_PExpiryDate, xlcol_RelNameE,
            xlcol_NextofKinE, xlcol_MotherNameE, xlcol_MobileNo, xlcol_TeleNoAbroad, xlcol_PerAddressE, xlcol_Email,
            xlcol_PassportProf, xlcol_StartDtofIndemnity, xlcol_AuxString3, xlcol_Education, xlcol_AuxString4, xlcol_AuxInt3,
            xlcol_LocLib5, xlcol_HealthInsurCmp, xlcol_NextofKinAddrE, xlcol_SponsorCode, xlcol_Title,
            xlcol_EmpNameA, xlcol_NickNameE, xlcol_NickNameA, xlcol_RelType, xlcol_RelNameA, xlcol_MotherNameA,
            xlcol_FamilyNameA, xlcol_BirthPlaceA, xlcol_PassportNoA, xlcol_PCategory, xlcol_PIssuePlaceA,
            xlcol_PerAddressA, xlcol_Skill1, xlcol_Skill2, xlcol_Skill3, xlcol_Language1, xlcol_Language2,
            xlcol_Language3, xlcol_VisaType, xlcol_VisaNo, xlcol_VisaIssueDate, xlcol_ImmigFileNo,
            xlcol_EntryPlace, xlcol_EntryDate, xlcol_ResidenceNo, xlcol_ResIssueDate, xlcol_ResExpDate,
            xlcol_ResIssuePlace, xlcol_LabCardNo, xlcol_LCIssueDate, xlcol_LCExpDate, xlcol_HlthCardNo,
            xlcol_HCIssuePlace, xlcol_HCIssueDate, xlcol_HCExpiryDate, xlcol_DrvLicNo, xlcol_DLCategory,
            xlcol_DLIssuePlace, xlcol_DLIssueDate, xlcol_DLExpiryDate, xlcol_SponByOther, xlcol_OSponNameE,
            xlcol_OSponNameA, xlcol_OSponRel, xlcol_OSponNation, xlcol_OSponVisaNo, xlcol_OSponVExpDt, xlcol_OSponPsprtNoE,
            xlcol_OSponPsprtNoA, xlcol_ExperienceE, xlcol_ExperienceA, xlcol_Emirates, xlcol_City, xlcol_Area, xlcol_StreetE,
            xlcol_StreetA, xlcol_BuildingE, xlcol_BuildingA, xlcol_FlatE, xlcol_FlatA, xlcol_OffPhoneNo, xlcol_Ext, xlcol_ResPhoneNo,
            xlcol_POBox, xlcol_PagerNo, xlcol_BloodGroup, xlcol_FaxNo, xlcol_NextofKinA, xlcol_NextofKinAddrA, xlcol_AddressE, xlcol_AddressA,
            xlcol_AuxString1, xlcol_AuxString2, xlcol_AuxString5, xlcol_AuxString6, xlcol_AuxString7, xlcol_AuxString8, xlcol_AuxString9,
            xlcol_AuxString10, xlcol_AuxAString1, xlcol_AuxAString2, xlcol_AuxAString3, xlcol_AuxAString4, xlcol_AuxAString5, xlcol_AuxAString6,
            xlcol_AuxAString7, xlcol_AuxAString8, xlcol_AuxAString9, xlcol_AuxAString10, xlcol_AuxInt1, xlcol_AuxInt2, xlcol_AuxCurrency1,
            xlcol_AuxCurrency2, xlcol_AuxDate1, xlcol_AuxDate2, xlcol_AuxDate3, xlcol_AuxDate4, xlcol_AuxDate5, xlcol_AuxLib1, xlcol_AuxLib2,
            xlcol_AuxLib3, xlcol_AuxLib4, xlcol_IntlJoiningDate, xlcol_PointOfHireE, xlcol_PointOfHireA, xlcol_SourceOfHire, xlcol_SLReInitDate,
            xlcol_LCIssuePlace, xlcol_FNameA, xlcol_SNameA, xlcol_GrandFatherE, xlcol_SponTypeExtnl, xlcol_GrandFatherA,
            xlcol_VisaExpDate, xlcol_AuxLib5, xlcol_AuxLib6, xlcol_PersEmail, xlcol_NPresentSec, xlcol_ReligionSubSet,
            xlcol_UIDNo,xlcol_Disability,xlcol_DisabilityDescE,xlcol_DisabilityDescA,xlcol_NationalID,xlcol_WPS
        };

        DateTime dtEmptyDate = new DateTime(1900, 1, 1);
        DateTime dtminDate = new DateTime(1900, 1, 1);
        DateTime dtmaxDate = new DateTime(2079, 6, 6);
        DateTime currDate;

        const string clearFieldMarker = "<blank>";

        enum enmXlImportTables
        {
            Employee = 0,
            //FinMast = 1,
            //PayDetails = 2,
            WrkAgrmntDet = 3,
            //TKTMASTER = 4
        }
        //List<string> lstXlImportTables = new List<string>() { "Employee", "FinMast", "PayDetails", "WrkAgrmntDet", "TKTMASTER" };
        List<string> lstXlImportTables = new List<string>() { "Employee" };



        static string[] SystemMandatoryEmployeeCols =
        {
            xlcol_EmpCode,xlcol_EmpNameE,xlcol_Sex,xlcol_NPresent,
            xlcol_DateOfBirth,xlcol_LocLib5,xlcol_SalProfile,

        };
        static string[] MandatoryEmployeeCols =
        {
            xlcol_EmpCode, xlcol_Title, xlcol_EmpNameE, xlcol_FNameE, xlcol_FamilyNameE, xlcol_MotherNameE,xlcol_Sex, xlcol_Religion, xlcol_DateOfBirth,xlcol_MaritalStat,xlcol_CountryOfBirth, 
            xlcol_BirthPlaceE, xlcol_PassportNoE, xlcol_PCategory, xlcol_PIssueDate,xlcol_PExpiryDate,
                //, xlcol_PIssueCountry, xlcol_PIssuePlaceE,
            xlcol_NPresent, xlcol_NPrevious, xlcol_PassportProf,xlcol_Education, //xlcol_Language1, xlcol_Language2,
            xlcol_Emirates,
            //xlcol_Area, xlcol_City,xlcol_BuildingE, xlcol_StreetE, xlcol_FlatE,xlcol_POBox,
            //xlcol_OffPhoneNo,xlcol_ResPhoneNo, xlcol_MobileNo, xlcol_TeleNoAbroad, 
           // xlcol_PersEmail, xlcol_AddressE, xlcol_PerAddressE, xlcol_Email,
            xlcol_SponsorCode,
            xlcol_LocLib5,
            xlcol_IntlJoiningDate

        };



        public void SaveToCSFromStaging(int processID, ref string strprocessRemarks, ref bool hasProcessError)
        {
            try
            {
                Common.LogAction($"Save From Employee Staging Table to CSTable Started");
                sbLineErrMsg = new StringBuilder();
                sbFileErrMsg = new StringBuilder();
                sbSaveMsg = new StringBuilder();


                //For Progress bar
                StartProgress();
                //End:For Progress bar

                string sQry = "";
                string finishMessage = ""; //Progress finished message

                mydt = new DataTable();
                // Step 1: Retrieve data from DBOXI_EmployeeInitialStaging
                //string selectQuery = "SELECT * FROM DBOXI_EmployeeInitialStaging where DBOXIProcessID="+ processID.ToString();

                string selectQuery = "EXEC USP_DBOXI_GetPreProcessedStagingData " + processID.ToString();
                DataTable mydt_staging = ConnectionFunctions.ExecuteQueryToDataTable(selectQuery);

                if (mydt_staging == null || mydt_staging.Rows.Count == 0)
                {
                    Common.LogAction("No data found in DBOXI_EmployeeInitialStaging table.");
                    throw new ManualException("", "No data found in DBOXI_EmployeeInitialStaging table.");
                    
                }

                CreateEmployeeImportDataTable();
                MapStagingDataToImportTable(mydt_staging, ref mydt);



                //For Progress bar
                //string fileName = mydt.Rows[0]["DBOXIProcessID"].ToString();
                //Common.LogAction($"Employee file '{fileName}' processed successfully.");
                //End:For Progress bar

                importProcessId = processID.ToString();

                dictTitleNames = GetTitleNames();
                AssignStagingTitleNames(ref dictTitleNames);

                dtLookUpFieldsDetails_Emp = GetLookupFieldDetails(enmXlImportTables.Employee);



                int TotalRecords = 0, InsertedRecords = 0, UpdatedRecords = 0, SkippedRecords = 0;




                errTotalCount = 0;

                object emptyObj = null;
                object lookupCodeObj = null;
                string sQuery = "";
                string fieldErr = "";
                Boolean isUpdate = false;
                string ErrorFileStr = "";
                string ErrorFileHeader = "";
                ErrorFileHeader = "Unable to Import the some of the Transactions due to  below error." + Environment.NewLine;
                ErrorFileHeader += "---------------------------------------------------------------------------------------------" + Environment.NewLine + Environment.NewLine;


                var totalRecord = mydt.Rows.Count;


                //For Progress bar
                int nprccedd = 0;
                int totcnt = mydt.Rows.Count;
                double percn = 0;
                SetProgressStartVariables();
                DateTime LastprcsdTime = new DateTime(1900, 1, 1);
                //End:For Progress bar

                int nRowNo = 0;

                int nEmpID = 0;
                foreach (DataRow row in mydt.Rows)
                {
                    nEmpID = 0;
                    try
                    {
                        iCurrRowNo = 0;
                        iCurrPLDSrNo = 0;

                        nRowNo = Convert.ToInt32(row["RowNo"]);
                        iCurrRowNo = nRowNo;

                        SkippedRecords += 1;

                        iCurrPLDSrNo = AddProcessLogDetails();

                        #region Variable declaration
                        string strEmpLocationData = "";
                        errCount = 0;
                        bHasEmpBRerror = false;
                        bEmpSaved = false;
                        bEmpSkipped = false;
                        strCurrEmpCode = "";
                        bCurrHasLineErrors = false;
                        sbSaveMsg.Clear();
                        sbLineErrMsg.Clear();

                        // Variable declaration
                        string StrEmpCode = "";
                        string StrTitle = "";
                        int TitleCode = 0;
                        string StrEmpNameE = "";
                        string StrEmpNameA = "";
                        string StrNickNameE = "";
                        string StrNickNameA = "";
                        string StrRelType = "";
                        string RelTypeCode = "";
                        string StrRelNameE = "";
                        string StrRelNameA = "";
                        string StrMotherNameE = "";
                        string StrMotherNameA = "";
                        string StrFamilyNameE = "";
                        string StrFamilyNameA = "";
                        string StrGender = "";
                        int GenderCode = 0;
                        string StrPresentNationality = "";
                        string PresentNationalityCode = "";
                        string StrPreviousNationality = "";
                        string PreviousNationalityCode = "";
                        string StrMaritalStat = "";
                        int MaritalStatusCode = 0;
                        string StrDateOfBirth = "";
                        DateTime DateOfBirthDateValue = new DateTime(1900, 1, 1);
                        int DOBDAYVal = 0, DOBMONTHVal = 0, DOBYEARVal = 0;
                        string StrBirthPlaceE = "";
                        string StrBirthPlaceA = "";
                        string StrCountryOfBirth = "";
                        string CountryOfBirthCode = "";
                        string StrPassportNoE = "";
                        string StrPassportNoA = "";
                        string StrPCategory = "";
                        int PCategoryCode = 0;
                        string StrPIssuePlaceE = "";
                        string StrPIssuePlaceA = "";
                        string StrPassportIssueCountry = "";
                        string PassportIssueCountryCode = "";
                        string StrPIssueDate = "";
                        DateTime PassportIssueDateValue = new DateTime(1900, 1, 1);
                        string StrPExpiryDate = "";
                        DateTime PassportExpiryDateValue = new DateTime(1900, 1, 1);
                        string StrReligion = "";
                        string ReligionCode = "";
                        string StrPassportProf = "";
                        string VisaProfessionCode = "";
                        string StrEducation = "";
                        string VisaQualificationCode = "";
                        string StrPerAddressE = "";
                        string StrPerAddressA = "";
                        string StrSkill1 = "";
                        string StrSkill2 = "";
                        string StrSkill3 = "";
                        string Skill1Code = "";
                        string Skill2Code = "";
                        string Skill3Code = "";
                        string StrLanguage1 = "";
                        string StrLanguage2 = "";
                        string StrLanguage3 = "";
                        string Language1Code = "";
                        string Language2Code = "";
                        string Language3Code = "";
                        string StrVisaType = "";
                        string VisaTypeCode = "";
                        string StrVisaNo = "";
                        string StrVisaIssueDate = "";
                        DateTime VisaIssueDateValue = new DateTime(1900, 1, 1);
                        string StrImmigFileNo = "";
                        string StrEntryPlace = "";
                        string EntryPlaceCode = "";
                        string StrEntryDate = "";
                        DateTime EntryDateValue = new DateTime(1900, 1, 1);
                        string StrResidenceNo = "";
                        string StrResIssueDate = "";
                        DateTime ResIssueDateValue = new DateTime(1900, 1, 1);
                        string StrResExpDate = "";
                        DateTime ResExpDateValue = new DateTime(1900, 1, 1);
                        string StrResIssuePlace = "";
                        string ResIssuePlaceCode = "";
                        string StrLabCardNo = "";
                        string StrLCIssueDate = "";
                        DateTime LCIssueDateValue = new DateTime(1900, 1, 1);
                        string StrLCExpDate = "";
                        DateTime LCExpDateValue = new DateTime(1900, 1, 1);
                        string StrHlthCardNo = "";
                        string StrHCIssuePlace = "";
                        string HCIssuePlaceCode = "";
                        string StrHCIssueDate = "";
                        DateTime HCIssueDateValue = new DateTime(1900, 1, 1);
                        string StrHCExpiryDate = "";
                        DateTime HCExpiryDateValue = new DateTime(1900, 1, 1);
                        string StrDrvLicNo = "";
                        string StrDLCategory = "";
                        int DLCategoryCode = 0;
                        string StrDLIssuePlace = "";
                        string StrDLIssueDate = "";
                        string StrDLExpiryDate = "";
                        DateTime DLIssueDateValue = new DateTime(1900, 1, 1);
                        DateTime DLExpiryDateValue = new DateTime(1900, 1, 1);
                        string StrSponsorCode = "";
                        string StrSponsorName = "";
                        string StrSponByOther = "";
                        int SponByOtherCode = 0;
                        string StrOSponNameE = "";
                        string StrOSponNameA = "";
                        string StrOSponRel = "";
                        string OSponRelCode = "";
                        string StrOSponNation = "";
                        string OSponNationCode = "";
                        string StrOSponVisaNo = "";
                        string StrOSponVExpDt = "";
                        DateTime OSponVExpDtValue = new DateTime(1900, 1, 1);
                        string StrOSponPsprtNoE = "";
                        string StrOSponPsprtNoA = "";
                        string StrExperienceE = "";
                        string StrExperienceA = "";
                        string StrEmirates = "";
                        string EmiratesCode = "";
                        string StrCity = "";
                        string CityCode = "";
                        string StrArea = "";
                        string AreaCode = "";
                        string StrStreetE = "";
                        string StrStreetA = "";
                        string StrBuildingE = "";
                        string StrBuildingA = "";
                        string StrFlatE = "";
                        string StrFlatA = "";
                        string StrOffPhoneNo = "";
                        string StrExt = "";
                        string StrResPhoneNo = "";
                        string StrPOBox = "";
                        string StrMobileNo = "";
                        string StrPagerNo = "";
                        string StrTeleNoAbroad = "";
                        string StrEmail = "";
                        string StrBloodGroup = "";
                        string BloodGroupCode = "";
                        string StrFaxNo = "";
                        string StrNextofKinE = "";
                        string StrNextofKinA = "";
                        string StrNextofKinAddrE = "";
                        string StrNextofKinAddrA = "";
                        string StrAddressE = "";
                        string StrAddressA = "";
                        string StrAuxString1 = "";
                        string StrAuxString2 = "";
                        string StrAuxString3 = "";
                        string StrAuxString4 = "";
                        string StrAuxString5 = "";
                        string StrAuxString6 = "";
                        string StrAuxString7 = "";
                        string StrAuxString8 = "";
                        string StrAuxString9 = "";
                        string StrAuxString10 = "";
                        string StrAuxAString1 = "";
                        string StrAuxAString2 = "";
                        string StrAuxAString3 = "";
                        string StrAuxAString4 = "";
                        string StrAuxAString5 = "";
                        string StrAuxAString6 = "";
                        string StrAuxAString7 = "";
                        string StrAuxAString8 = "";
                        string StrAuxAString9 = "";
                        string StrAuxAString10 = "";
                        string StrAuxInt1 = "";
                        int nAuxInt1 = 0;
                        string StrAuxInt2 = "";
                        int nAuxInt2 = 0;
                        string StrAuxInt3 = "";
                        int nAuxInt3 = 0;
                        string StrAuxCurrency1 = "";
                        double nAuxCurrency1 = 0;
                        string StrAuxCurrency2 = "";
                        double nAuxCurrency2 = 0;
                        string StrAuxDate1 = "";
                        string StrAuxDate2 = "";
                        string StrAuxDate3 = "";
                        string StrAuxDate4 = "";
                        string StrAuxDate5 = "";
                        DateTime AuxDate1Value = new DateTime(1900, 1, 1);
                        DateTime AuxDate2Value = new DateTime(1900, 1, 1);
                        DateTime AuxDate3Value = new DateTime(1900, 1, 1);
                        DateTime AuxDate4Value = new DateTime(1900, 1, 1);
                        DateTime AuxDate5Value = new DateTime(1900, 1, 1);
                        string StrAuxLib1 = "";
                        string StrAuxLib2 = "";
                        string StrAuxLib3 = "";
                        string StrAuxLib4 = "";
                        string AuxLib1Code = "";
                        string AuxLib2Code = "";
                        string AuxLib3Code = "";
                        string AuxLib4Code = "";
                        string StrIntlJoiningDate = "";
                        DateTime IntlJoiningDateValue = new DateTime(1900, 1, 1);
                        string StrPointOfHireE = "";
                        string PointOfHireECode = "";
                        string StrPointOfHireA = "";
                        string StrSourceOfHire = "";
                        string SourceOfHireCode = "";
                        string StrSLReInitDate = "";
                        DateTime SLReInitDateValue = new DateTime(1900, 1, 1);
                        string StrStartDtofIndemnity = "";
                        DateTime StartDtofIndemnityValue = new DateTime(1900, 1, 1);
                        string LocationLevelCode = "";
                        string StrSalProfile = "";
                        string SalaryProfileCode = "";
                        string StrSalaryProfileText = "";
                        string StrHealthInsurCmp = "";
                        string HealthInsurCmpCode = "";
                        string StrLCIssuePlace = "";
                        string LCIssuePlaceCode = "";
                        string StrFNameE = "";
                        string StrFNameA = "";
                        string StrSNameE = "";
                        string StrSNameA = "";
                        string StrGrandFatherE = "";
                        string StrSponTypeExtnl = "";
                        int SponTypeExtnlCode = 0;
                        string StrGrandFatherA = "";
                        string StrVisaExpDate = "";
                        DateTime VisaExpDateValue = new DateTime(1900, 1, 1);
                        string StrAuxLib5 = "";
                        string StrAuxLib6 = "";
                        string AuxLib5Code = "";
                        string AuxLib6Code = "";
                        string StrPersEmail = "";
                        string StrNPresentSec = "";
                        string NPresentSecCode = "";
                        string StrReligionSubSet = "";
                        string ReligionSubSetCode = "";
                        string StrUIDNo = "";
                        string StrDisability = "";
                        string DisabilityCode = "";
                        string StrDisabilityDescE = "";
                        string StrDisabilityDescA = "";
                        string StrNationalID = "";
                        string StrWPS = "";
                        string WPSCode = "";



                        //Begin:derived variables
                        string StrWrkAgreeNo = "";
                        //End:derived



                        string strEditMode = "ADD";


                        SqlDataReader dr = null;
                        StringBuilder strBuildrAudit = new StringBuilder();
                        StringBuilder strBuildrAuditFin = new StringBuilder();
                        StringBuilder strBuildUpdtAud = new StringBuilder();
                        StringBuilder strBuildUpdtAudFin = new StringBuilder();
                        string strBuildrAuditText = "";
                        string strBuildrUpdtAudText = "";


                        bool bskipInsertUpdate = false;
                        string rowErrInfo = "";

                        #endregion


                        #region Employee Fiedls Validations

                        if (CheckIfColumnExists(mydt, xlcol_EmpCode))
                        {

                            if (row[xlcol_EmpCode] == null || row[xlcol_EmpCode].ToString() == string.Empty)
                            {
                                AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_EmpCode] + " is empty : ");
                                bskipInsertUpdate = true;
                                goto skipInsertUpdateStep;
                            }
                            else
                            {
                                StrEmpCode = row[xlcol_EmpCode].ToString();
                                rowErrInfo = ", EmpCode: " + StrEmpCode;
                                strCurrEmpCode = StrEmpCode;

                                int StrEmpCodelength = 0;

                                StrEmpCodelength = StrEmpCode.Length;

                                if (StrEmpCodelength > 9)
                                {
                                    AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_EmpCode] + " doesn't allow more than 9 characters : ");
                                    bskipInsertUpdate = true;
                                    goto skipInsertUpdateStep;
                                }
                            }
                        }
                        else
                        {
                            AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_EmpCode] + " is empty : ");
                            bskipInsertUpdate = true;
                            goto skipInsertUpdateStep;
                        }

                        //For Progress bar
                        string StrEmpNameProgrss = "";
                        string progressEmpname = "";

                        if (row[xlcol_EmpNameE] == null || row[xlcol_EmpNameE].ToString() == string.Empty)
                        {
                        }
                        else
                        {
                            StrEmpNameProgrss = row[xlcol_EmpNameE].ToString();
                        }

                        string CodeFirst = "EmpNameE";

                        string strqry = "SELECT ISNULL(LTRIM(RTRIM(EmpCode)) + SPACE(1)  + '[' + LTRIM(RTRIM(" + CodeFirst + ")) + ']',EmpCode) AS CodeEFirst FROM Employee WITH (NOLOCK) WHERE EmpCode = @ParmEmpCode";
                        string sResult2 = "";

                        SqlParameter[] Params1 = new SqlParameter[1];
                        Params1[0] = new SqlParameter("@ParmEmpCode", SqlDbType.VarChar);
                        Params1[0].Value = StrEmpCode;
                        if (!ConnectionFunctions.Connect_SQLScalar(ref sResult2, strqry, ref Params1, ref errmsg))
                        {
                            progressEmpname = "Importing Employee " + StrEmpCode;
                        }
                        if (sResult2 != "")
                        {
                            progressEmpname = "Importing Employee " + sResult2;
                        }
                        else if (StrEmpNameProgrss != "")
                        {
                            progressEmpname = "Importing Employee " + StrEmpCode + " [" + StrEmpNameProgrss + "]";
                        }
                        else
                        {
                            progressEmpname = "Importing Employee " + StrEmpCode;
                        }


                        nprccedd = nprccedd + 1;
                        percn = ((double)nprccedd / (double)totcnt) * (double)100;

                        string strnoofemp = " " + nprccedd + " of " + totcnt;

                        UpdateProgress(progressEmpname, nprccedd, totcnt, percn, strnoofemp);

                        LastprcsdTime = DateTime.Now;

                        //End:For Progress bar



                        //check the record is in edit mode.
                        string sQueryManD = "SELECT TOP(1) EmpID FROM Employee WITH (NOLOCK) WHERE EmpCode = @ParmEmpCode";
                        string strEmpID = "";
                        //int nEmpID = 0;
                        int nRecordNo_FinMast = 0;
                        int nRecordNo_EmpBals = 0;
                        SqlParameter[] Params2 = new SqlParameter[1];
                        Params2[0] = new SqlParameter("@ParmEmpCode", SqlDbType.VarChar);
                        Params2[0].Value = StrEmpCode;

                        if (!ConnectionFunctions.Connect_SQLScalar(ref strEmpID, sQueryManD, ref Params2, ref errmsg))
                        {
                            AppendLineError(nRowNo, rowErrInfo, "Error Occurred while retrieving the Employee Details from Database : ");
                            bskipInsertUpdate = true;
                            goto skipInsertUpdateStep;
                        }
                        if (string.IsNullOrEmpty(strEmpID))
                        {
                            strEditMode = "ADD";
                            isUpdate = false;
                        }
                        else
                        {
                            strEditMode = "EDIT";
                            isUpdate = true;
                            nEmpID = Convert.ToInt32(strEmpID);
                        }


                        DataTable dtEmployeeOld = new DataTable();
                        DataTable dtFinMastOld = new DataTable();
                        DataRow drowEmpOld = null;
                        DataRow drowFinMastOld = null;

                        if (strEditMode == "EDIT")
                        {

                            if (!GetEmployeeData(StrEmpCode, ref dtEmployeeOld, ref errmsg))
                            {
                                AppendLineError(nRowNo, rowErrInfo, errmsg);
                                bskipInsertUpdate = true;
                                goto skipInsertUpdateStep;
                            }
                            else
                            {
                                drowEmpOld = dtEmployeeOld.Rows[0];
                            }

                            if (!GetEmployeeFinMastData(StrEmpCode, ref dtFinMastOld, ref errmsg))
                            {
                                AppendLineError(nRowNo, rowErrInfo, "Employee Financial Details fetch error. Details: " + errmsg);
                                bskipInsertUpdate = true;
                                goto skipInsertUpdateStep;
                            }
                            else
                            {
                                if (dtFinMastOld != null && dtFinMastOld.Rows.Count > 0)
                                {
                                    drowFinMastOld = dtFinMastOld.Rows[0];
                                }
                            }

                        }



                        //Employee Status Validation
                        if (strEditMode == "EDIT" && drowEmpOld != null && drowEmpOld["EmployeeStatus"] != DBNull.Value)
                        {
                            if (drowEmpOld["EmployeeStatus"].ToString() == "11")
                            {

                                AppendLineError(nRowNo, rowErrInfo, "End of Service Employee Details cannot be modified : ");
                                bskipInsertUpdate = true;
                                goto skipInsertUpdateStep;
                            }
                        }





                        if (CheckIfColumnExists(mydt, xlcol_Title))
                        {
                            if (row[xlcol_Title] == null || row[xlcol_Title].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                    if (CheckIfMandatory(xlcol_Title, enmXlImportTables.Employee))
                                    {
                                        AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_Title] + " cannot be blank");
                                        bskipInsertUpdate = true;
                                    }
                                }
                                else if (drowEmpOld != null && drowEmpOld["Title"] != DBNull.Value)
                                {
                                    TitleCode = Convert.ToInt32(drowEmpOld["Title"]);
                                }
                            }
                            else
                            {
                                StrTitle = row[xlcol_Title].ToString();


                                if (!ValidateField(xlcol_Title, StrTitle, isUpdate, ref fieldErr, ref lookupCodeObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);
                                }
                                else
                                {
                                    TitleCode = Convert.ToInt32(lookupCodeObj);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_Title] + " " + StrTitle + "]");
                                }
                            }
                        }

                        if (CheckIfColumnExists(mydt, xlcol_EmpNameE))
                        {
                            if (row[xlcol_EmpNameE] == null || row[xlcol_EmpNameE].ToString() == string.Empty)
                            {
                                //mandatory
                                if (strEditMode != "EDIT")
                                {
                                    AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_EmpNameE] + " is Empty : ");
                                    bskipInsertUpdate = true;
                                }
                                else if (drowEmpOld != null && drowEmpOld["EmpNameE"] != DBNull.Value)
                                {
                                    StrEmpNameE = Convert.ToString(drowEmpOld["EmpNameE"]);
                                }
                            }
                            else
                            {
                                StrEmpNameE = row[xlcol_EmpNameE].ToString();

                                if (!ValidateField(xlcol_EmpNameE, StrEmpNameE, isUpdate, ref fieldErr, ref emptyObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);
                                }
                                else
                                {
                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_EmpNameE] + " " + StrEmpNameE + "]");
                                }


                            }
                        }

                        if (CheckIfColumnExists(mydt, xlcol_EmpNameA))
                        {
                            if (row[xlcol_EmpNameA] == null || row[xlcol_EmpNameA].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["EmpNameA"] != DBNull.Value)
                                {
                                    StrEmpNameA = Convert.ToString(drowEmpOld["EmpNameA"]);
                                }
                            }
                            else
                            {
                                StrEmpNameA = row[xlcol_EmpNameA].ToString();

                                if (!ValidateField(xlcol_EmpNameA, StrEmpNameA, isUpdate, ref fieldErr, ref emptyObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);
                                }
                                else
                                {
                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_EmpNameA] + " " + StrEmpNameA + "]");
                                }


                            }
                        }


                        if (CheckIfColumnExists(mydt, xlcol_FNameE))
                        {
                            row[xlcol_FNameE] = Common.RemoveSpecialCharacters(row[xlcol_FNameE].ToString());

                            if (row[xlcol_FNameE] == null || row[xlcol_FNameE].ToString() == string.Empty)
                            {

                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                    //mandatory if new
                                    AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_FNameE] + " is empty: ");
                                    bskipInsertUpdate = true;
                                }
                                else if (drowEmpOld != null && drowEmpOld["FNameE"] != DBNull.Value)
                                {
                                    StrFNameE = Convert.ToString(drowEmpOld["FNameE"]);
                                }
                            }
                            else
                            {
                                StrFNameE = row[xlcol_FNameE].ToString();

                                if (!ValidateField(xlcol_FNameE, StrFNameE, isUpdate, ref fieldErr, ref emptyObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);
                                }
                                else
                                {
                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_FNameE] + " " + StrFNameE + "]");
                                }
                            }
                        }

                        if (CheckIfColumnExists(mydt, xlcol_SNameE))
                        {
                            row[xlcol_SNameE] = Common.RemoveSpecialCharacters(row[xlcol_SNameE].ToString());

                            if (row[xlcol_SNameE] == null || row[xlcol_SNameE].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                    if (CheckIfMandatory(xlcol_SNameE, enmXlImportTables.Employee))
                                    {
                                        AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_SNameE] + " cannot be blank");
                                        bskipInsertUpdate = true;
                                    }
                                }
                                else if (drowEmpOld != null && drowEmpOld["SNameE"] != DBNull.Value)
                                {
                                    StrSNameE = Convert.ToString(drowEmpOld["SNameE"]);
                                }
                            }
                            else
                            {
                                StrSNameE = row[xlcol_SNameE].ToString();

                                if (!ValidateField(xlcol_SNameE, StrSNameE, isUpdate, ref fieldErr, ref emptyObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);
                                }
                                else
                                {
                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_SNameE] + " " + StrSNameE + "]");
                                }
                            }
                        }

                        if (CheckIfColumnExists(mydt, xlcol_NickNameE))
                        {
                            row[xlcol_NickNameE] = Common.RemoveSpecialCharacters(row[xlcol_NickNameE].ToString());

                            if (row[xlcol_NickNameE] == null || row[xlcol_NickNameE].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["NickNameE"] != DBNull.Value)
                                {
                                    StrNickNameE = Convert.ToString(drowEmpOld["NickNameE"]);
                                }
                            }
                            else
                            {
                                StrNickNameE = row[xlcol_NickNameE].ToString();

                                if (!ValidateField(xlcol_NickNameE, StrNickNameE, isUpdate, ref fieldErr, ref emptyObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {
                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_NickNameE] + " " + StrNickNameE + "]");
                                }


                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_FamilyNameE))
                        {
                            row[xlcol_FamilyNameE] = Common.RemoveSpecialCharacters(row[xlcol_FamilyNameE].ToString());

                            if (row[xlcol_FamilyNameE] == null || row[xlcol_FamilyNameE].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                    //if (CheckIfMandatory(xlcol_FamilyNameE, enmXlImportTables.Employee))
                                    //{
                                    //    AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_FamilyNameE] + " cannot be blank");
                                    //    bskipInsertUpdate = true;
                                    //}
                                }
                                else if (drowEmpOld != null && drowEmpOld["FamilyNameE"] != DBNull.Value)
                                {
                                    StrFamilyNameE = Convert.ToString(drowEmpOld["FamilyNameE"]);
                                }
                            }
                            else
                            {
                                StrFamilyNameE = row[xlcol_FamilyNameE].ToString();

                                if (!ValidateField(xlcol_FamilyNameE, StrFamilyNameE, isUpdate, ref fieldErr, ref emptyObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);
                                }
                                else
                                {
                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_FamilyNameE] + " " + StrFamilyNameE + "]");
                                }
                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_GrandFatherE))
                        {
                            if (row[xlcol_GrandFatherE] == null || row[xlcol_GrandFatherE].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["GrandFatherE"] != DBNull.Value)
                                {
                                    StrGrandFatherE = Convert.ToString(drowEmpOld["GrandFatherE"]);
                                }
                            }
                            else
                            {
                                StrGrandFatherE = row[xlcol_GrandFatherE].ToString();

                                if (!ValidateField(xlcol_GrandFatherE, StrGrandFatherE, isUpdate, ref fieldErr, ref emptyObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);
                                }
                                else
                                {
                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_GrandFatherE] + " " + StrGrandFatherE + "]");
                                }
                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_FNameA))
                        {
                            if (row[xlcol_FNameA] == null || row[xlcol_FNameA].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["FNameA"] != DBNull.Value)
                                {
                                    StrFNameA = Convert.ToString(drowEmpOld["FNameA"]);
                                }
                            }
                            else
                            {
                                StrFNameA = row[xlcol_FNameA].ToString();

                                if (!ValidateField(xlcol_FNameA, StrFNameA, isUpdate, ref fieldErr, ref emptyObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);
                                }
                                else
                                {
                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_FNameA] + " " + StrFNameA + "]");
                                }
                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_SNameA))
                        {
                            if (row[xlcol_SNameA] == null || row[xlcol_SNameA].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["SNameA"] != DBNull.Value)
                                {
                                    StrSNameA = Convert.ToString(drowEmpOld["SNameA"]);
                                }
                            }
                            else
                            {
                                StrSNameA = row[xlcol_SNameA].ToString();

                                if (!ValidateField(xlcol_SNameA, StrSNameA, isUpdate, ref fieldErr, ref emptyObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);
                                }
                                else
                                {
                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_SNameA] + " " + StrSNameA + "]");
                                }
                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_NickNameA))
                        {
                            if (row[xlcol_NickNameA] == null || row[xlcol_NickNameA].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["NickNameA"] != DBNull.Value)
                                {
                                    StrNickNameA = Convert.ToString(drowEmpOld["NickNameA"]);
                                }
                            }
                            else
                            {
                                StrNickNameA = row[xlcol_NickNameA].ToString();

                                if (!ValidateField(xlcol_NickNameA, StrNickNameA, isUpdate, ref fieldErr, ref emptyObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);
                                }
                                else
                                {
                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_NickNameA] + " " + StrNickNameA + "]");
                                }


                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_FamilyNameA))
                        {
                            if (row[xlcol_FamilyNameA] == null || row[xlcol_FamilyNameA].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["FamilyNameA"] != DBNull.Value)
                                {
                                    StrFamilyNameA = Convert.ToString(drowEmpOld["FamilyNameA"]);
                                }
                            }
                            else
                            {
                                StrFamilyNameA = row[xlcol_FamilyNameA].ToString();

                                if (!ValidateField(xlcol_FamilyNameA, StrFamilyNameA, isUpdate, ref fieldErr, ref emptyObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);
                                }
                                else
                                {
                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_FamilyNameA] + " " + StrFamilyNameA + "]");
                                }
                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_GrandFatherA))
                        {
                            if (row[xlcol_GrandFatherA] == null || row[xlcol_GrandFatherA].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["GrandFatherA"] != DBNull.Value)
                                {
                                    StrGrandFatherA = Convert.ToString(drowEmpOld["GrandFatherA"]);
                                }
                            }
                            else
                            {
                                StrGrandFatherA = row[xlcol_GrandFatherA].ToString();

                                if (!ValidateField(xlcol_GrandFatherA, StrGrandFatherA, isUpdate, ref fieldErr, ref emptyObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);
                                }
                                else
                                {
                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_GrandFatherA] + " " + StrGrandFatherA + "]");
                                }
                            }
                        }


                        if (CheckIfColumnExists(mydt, xlcol_RelType))
                        {
                            if (row[xlcol_RelType] == null || row[xlcol_RelType].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["RelType"] != DBNull.Value)
                                {
                                    RelTypeCode = Convert.ToString(drowEmpOld["RelType"]);
                                }
                            }
                            else
                            {
                                StrRelType = row[xlcol_RelType].ToString();


                                if (!ValidateField(xlcol_RelType, StrRelType, isUpdate, ref fieldErr, ref lookupCodeObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);
                                }
                                else
                                {
                                    RelTypeCode = Convert.ToString(lookupCodeObj);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_RelType] + " " + StrRelType + "]");
                                }


                            }
                        }

                        if (CheckIfColumnExists(mydt, xlcol_RelNameE))
                        {
                            if (row[xlcol_RelNameE] == null || row[xlcol_RelNameE].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["RelNameE"] != DBNull.Value)
                                {
                                    StrRelNameE = Convert.ToString(drowEmpOld["RelNameE"]);
                                }
                            }
                            else
                            {
                                StrRelNameE = row[xlcol_RelNameE].ToString();

                                if (!ValidateField(xlcol_RelNameE, StrRelNameE, isUpdate, ref fieldErr, ref emptyObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);
                                }
                                else
                                {
                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_RelNameE] + " " + StrRelNameE + "]");
                                }
                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_RelNameA))
                        {
                            if (row[xlcol_RelNameA] == null || row[xlcol_RelNameA].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["RelNameA"] != DBNull.Value)
                                {
                                    StrRelNameA = Convert.ToString(drowEmpOld["RelNameA"]);
                                }
                            }
                            else
                            {
                                StrRelNameA = row[xlcol_RelNameA].ToString();

                                if (!ValidateField(xlcol_RelNameA, StrRelNameA, isUpdate, ref fieldErr, ref emptyObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);
                                }
                                else
                                {
                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_RelNameA] + " " + StrRelNameA + "]");
                                }
                            }

                        }


                        if (CheckIfColumnExists(mydt, xlcol_MotherNameE))
                        {
                            if (row[xlcol_MotherNameE] == null || row[xlcol_MotherNameE].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                    if (CheckIfMandatory(xlcol_MotherNameE, enmXlImportTables.Employee))
                                    {
                                        AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_MotherNameE] + " cannot be blank");
                                        bskipInsertUpdate = true;
                                    }
                                }
                                else if (drowEmpOld != null && drowEmpOld["MotherNameE"] != DBNull.Value)
                                {
                                    StrMotherNameE = Convert.ToString(drowEmpOld["MotherNameE"]);
                                }
                            }
                            else
                            {

                                StrMotherNameE = row[xlcol_MotherNameE].ToString();

                                if (!ValidateField(xlcol_MotherNameE, StrMotherNameE, isUpdate, ref fieldErr, ref emptyObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);
                                }
                                else
                                {
                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_MotherNameE] + " " + StrMotherNameE + "]");
                                }
                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_MotherNameA))
                        {
                            if (row[xlcol_MotherNameA] == null || row[xlcol_MotherNameA].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["MotherNameA"] != DBNull.Value)
                                {
                                    StrMotherNameA = Convert.ToString(drowEmpOld["MotherNameA"]);
                                }
                            }
                            else
                            {

                                StrMotherNameA = row[xlcol_MotherNameA].ToString();

                                if (!ValidateField(xlcol_MotherNameA, StrMotherNameA, isUpdate, ref fieldErr, ref emptyObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);
                                }
                                else
                                {
                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_MotherNameA] + " " + StrMotherNameA + "]");
                                }
                            }

                        }


                        if (CheckIfColumnExists(mydt, xlcol_Sex))
                        {
                            if (row[xlcol_Sex] == null || row[xlcol_Sex].ToString() == string.Empty)
                            {
                                if (strEditMode != "EDIT")
                                {
                                    //mandatory if new
                                    AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_Sex] + " is empty: ");
                                    bskipInsertUpdate = true;
                                }
                                else if (drowEmpOld != null && drowEmpOld["Sex"] != DBNull.Value)
                                {
                                    GenderCode = Convert.ToInt32(drowEmpOld["Sex"]);
                                }
                            }
                            else
                            {
                                StrGender = row[xlcol_Sex].ToString();

                                if (!ValidateField(xlcol_Sex, StrGender, isUpdate, ref fieldErr, ref lookupCodeObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);
                                }
                                else
                                {
                                    GenderCode = Convert.ToInt32(lookupCodeObj);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_Sex] + " " + StrGender + "]");
                                }


                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_NPresent))
                        {
                            if (row[xlcol_NPresent] == null || row[xlcol_NPresent].ToString() == string.Empty)
                            {
                                
                                if (strEditMode != "EDIT")
                                {
                                    //mandatory if new
                                    AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_NPresent] + " is empty: ");
                                    bskipInsertUpdate = true;
                                }
                                else if (drowEmpOld != null && drowEmpOld["NPresent"] != DBNull.Value)
                                {
                                    PresentNationalityCode = Convert.ToString(drowEmpOld["NPresent"]);
                                }
                            }
                            else
                            {

                                StrPresentNationality = row[xlcol_NPresent].ToString();



                                if (!ValidateField(xlcol_NPresent, StrPresentNationality, isUpdate, ref fieldErr, ref lookupCodeObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);
                                }
                                else
                                {
                                    PresentNationalityCode = Convert.ToString(lookupCodeObj);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_NPresent] + " " + StrPresentNationality + "]");
                                }
                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_NPrevious))
                        {
                            if (row[xlcol_NPrevious] == null || row[xlcol_NPrevious].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                    if (CheckIfMandatory(xlcol_NPrevious, enmXlImportTables.Employee))
                                    {
                                        AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_NPrevious] + " cannot be blank");
                                        bskipInsertUpdate = true;
                                    }
                                }
                                else if (drowEmpOld != null && drowEmpOld["NPrevious"] != DBNull.Value)
                                {
                                    PreviousNationalityCode = Convert.ToString(drowEmpOld["NPrevious"]);
                                }
                            }
                            else
                            {

                                StrPreviousNationality = row[xlcol_NPrevious].ToString();

                                if (!ValidateField(xlcol_NPrevious, StrPreviousNationality, isUpdate, ref fieldErr, ref lookupCodeObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {
                                    PreviousNationalityCode = Convert.ToString(lookupCodeObj);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_NPrevious] + " " + StrPreviousNationality + "]");
                                }
                            }

                        }


                        if (CheckIfColumnExists(mydt, xlcol_MaritalStat))
                        {
                            if (row[xlcol_MaritalStat] == null || row[xlcol_MaritalStat].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                    //if (CheckIfMandatory(xlcol_MaritalStat, enmXlImportTables.Employee))
                                    //{
                                    //    AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_MaritalStat] + " cannot be blank");
                                    //    bskipInsertUpdate = true;
                                    //}
                                }
                                else if (drowEmpOld != null && drowEmpOld["MaritalStat"] != DBNull.Value)
                                {
                                    MaritalStatusCode = Convert.ToInt32(drowEmpOld["MaritalStat"]);
                                }
                            }
                            else
                            {
                                StrMaritalStat = row[xlcol_MaritalStat].ToString();

                                //if (!ValidateField(xlcol_MaritalStat, StrMaritalStat, isUpdate, ref fieldErr, ref lookupCodeObj))
                                //{
                                ////    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                //}
                                //else
                                //{
                                    MaritalStatusCode = Convert.ToInt32(lookupCodeObj);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_MaritalStat] + " " + StrMaritalStat + "]");

                                //}

                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_DateOfBirth))
                        {
                            if (row[xlcol_DateOfBirth] == null || row[xlcol_DateOfBirth].ToString() == string.Empty)
                            {
                                if (strEditMode != "EDIT")
                                {
                                    //mandatory if new
                                    AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_DateOfBirth] + " is empty: ");
                                    bskipInsertUpdate = true;

                                }
                                else if (drowEmpOld != null && drowEmpOld["DateOfBirth"] != DBNull.Value)
                                {
                                    DateOfBirthDateValue = GetValidDateTime(drowEmpOld["DateOfBirth"].ToString());

                                    DOBDAYVal = DateOfBirthDateValue.Day;
                                    DOBMONTHVal = DateOfBirthDateValue.Month;
                                    DOBYEARVal = DateOfBirthDateValue.Year;
                                }
                            }
                            else
                            {

                                StrDateOfBirth = row[xlcol_DateOfBirth].ToString();

                                if (!ValidateField(xlcol_DateOfBirth, StrDateOfBirth, isUpdate, ref fieldErr, ref emptyObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);
                                }
                                else
                                {

                                    DateOfBirthDateValue = GetValidDateTime(StrDateOfBirth);

                                    DOBDAYVal = DateOfBirthDateValue.Day;
                                    DOBMONTHVal = DateOfBirthDateValue.Month;
                                    DOBYEARVal = DateOfBirthDateValue.Year;

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_DateOfBirth] + " " + DateOfBirthDateValue.ToString("dd/MM/yyyy") + "]");


                                }



                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_BirthPlaceE))
                        {
                            if (row[xlcol_BirthPlaceE] == null || row[xlcol_BirthPlaceE].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                    if (CheckIfMandatory(xlcol_BirthPlaceE, enmXlImportTables.Employee))
                                    {
                                        AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_BirthPlaceE] + " cannot be blank");
                                        bskipInsertUpdate = true;
                                    }
                                }
                                else if (drowEmpOld != null && drowEmpOld["BirthPlaceE"] != DBNull.Value)
                                {
                                    StrBirthPlaceE = Convert.ToString(drowEmpOld["BirthPlaceE"]);
                                }
                            }
                            else
                            {
                                StrBirthPlaceE = row[xlcol_BirthPlaceE].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_BirthPlaceE] + " " + StrBirthPlaceE + "]");

                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_BirthPlaceA))
                        {
                            if (row[xlcol_BirthPlaceA] == null || row[xlcol_BirthPlaceA].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["BirthPlaceA"] != DBNull.Value)
                                {
                                    StrBirthPlaceA = Convert.ToString(drowEmpOld["BirthPlaceA"]);
                                }
                            }
                            else
                            {
                                StrBirthPlaceA = row[xlcol_BirthPlaceA].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_BirthPlaceA] + " " + StrBirthPlaceA + "]");

                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_CountryOfBirth))
                        {
                            if (row[xlcol_CountryOfBirth] == null || row[xlcol_CountryOfBirth].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                    if (CheckIfMandatory(xlcol_CountryOfBirth, enmXlImportTables.Employee))
                                    {
                                        AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_CountryOfBirth] + " cannot be blank");
                                        bskipInsertUpdate = true;
                                    }
                                }
                                else if (drowEmpOld != null && drowEmpOld["CountryOfBirth"] != DBNull.Value)
                                {
                                    CountryOfBirthCode = Convert.ToString(drowEmpOld["CountryOfBirth"]);
                                }
                            }
                            else
                            {

                                StrCountryOfBirth = row[xlcol_CountryOfBirth].ToString();

                                if (!ValidateField(xlcol_CountryOfBirth, StrCountryOfBirth, isUpdate, ref fieldErr, ref lookupCodeObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {
                                    CountryOfBirthCode = Convert.ToString(lookupCodeObj);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_CountryOfBirth] + " " + StrCountryOfBirth + "]");
                                }
                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_PassportNoE))
                        {
                            if (row[xlcol_PassportNoE] == null || row[xlcol_PassportNoE].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                    if (CheckIfMandatory(xlcol_PassportNoE, enmXlImportTables.Employee))
                                    {
                                        AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_PassportNoE] + " cannot be blank");
                                        bskipInsertUpdate = true;
                                    }
                                }
                                else if (drowEmpOld != null && drowEmpOld["PassportNoE"] != DBNull.Value)
                                {
                                    StrPassportNoE = Convert.ToString(drowEmpOld["PassportNoE"]);
                                }
                            }
                            else
                            {

                                StrPassportNoE = row[xlcol_PassportNoE].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_PassportNoE] + " " + StrPassportNoE + "]");

                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_PassportNoA))
                        {
                            if (row[xlcol_PassportNoA] == null || row[xlcol_PassportNoA].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["PassportNoA"] != DBNull.Value)
                                {
                                    StrPassportNoA = Convert.ToString(drowEmpOld["PassportNoA"]);
                                }
                            }
                            else
                            {

                                StrPassportNoA = row[xlcol_PassportNoA].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_PassportNoA] + " " + StrPassportNoA + "]");

                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_PCategory))
                        {
                            if (row[xlcol_PCategory] == null || row[xlcol_PCategory].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                    if (CheckIfMandatory(xlcol_PCategory, enmXlImportTables.Employee))
                                    {
                                        AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_PCategory] + " cannot be blank");
                                        bskipInsertUpdate = true;
                                    }
                                }
                                else if (drowEmpOld != null && drowEmpOld["PCategory"] != DBNull.Value)
                                {
                                    PCategoryCode = Convert.ToInt32(drowEmpOld["PCategory"]);
                                }
                            }
                            else
                            {

                                StrPCategory = row[xlcol_PCategory].ToString();

                                if (!ValidateField(xlcol_PCategory, StrPCategory, isUpdate, ref fieldErr, ref lookupCodeObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {
                                    PCategoryCode = Convert.ToInt32(lookupCodeObj);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_PCategory] + " " + StrPCategory + "]");
                                }
                            }
                        }

                        if (CheckIfColumnExists(mydt, xlcol_PIssuePlaceE))
                        {
                            if (row[xlcol_PIssuePlaceE] == null || row[xlcol_PIssuePlaceE].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                    //if (CheckIfMandatory(xlcol_PIssuePlaceE, enmXlImportTables.Employee))
                                    //{
                                    //    AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_PIssuePlaceE] + " cannot be blank");
                                    //    bskipInsertUpdate = true;
                                    //}
                                }
                                else if (drowEmpOld != null && drowEmpOld["PIssuePlaceE"] != DBNull.Value)
                                {
                                    StrPIssuePlaceE = Convert.ToString(drowEmpOld["PIssuePlaceE"]);
                                }
                            }
                            else
                            {
                                StrPIssuePlaceE = row[xlcol_PIssuePlaceE].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_PIssuePlaceE] + " " + StrPIssuePlaceE + "]");

                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_PIssueCountry))
                        {
                            if (row[xlcol_PIssueCountry] == null || row[xlcol_PIssueCountry].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                    //if (CheckIfMandatory(xlcol_PIssueCountry, enmXlImportTables.Employee))
                                    //{
                                    //    AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_PIssueCountry] + " cannot be blank");
                                    //    bskipInsertUpdate = true;
                                    //}
                                }
                                else if (drowEmpOld != null && drowEmpOld["PIssueCountry"] != DBNull.Value)
                                {
                                    PassportIssueCountryCode = Convert.ToString(drowEmpOld["PIssueCountry"]);
                                }
                            }
                            else
                            {

                                StrPassportIssueCountry = row[xlcol_PIssueCountry].ToString();

                                //if (!ValidateField(xlcol_PIssueCountry, StrPassportIssueCountry, isUpdate, ref fieldErr, ref lookupCodeObj))
                                //{
                                //    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                //}
                                //else
                                //{
                                    PassportIssueCountryCode = Convert.ToString(lookupCodeObj);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_PIssueCountry] + " " + StrPassportIssueCountry + "]");

                                //}

                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_PIssueDate))
                        {
                            if (row[xlcol_PIssueDate] == null || row[xlcol_PIssueDate].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                    if (CheckIfMandatory(xlcol_PIssueDate, enmXlImportTables.Employee))
                                    {
                                        AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_PIssueDate] + " cannot be blank");
                                        bskipInsertUpdate = true;
                                    }
                                }
                                else if (drowEmpOld != null && drowEmpOld["PIssueDate"] != DBNull.Value)
                                {
                                    PassportIssueDateValue = GetValidDateTime(drowEmpOld["PIssueDate"].ToString());
                                }
                            }
                            else
                            {
                                StrPIssueDate = row[xlcol_PIssueDate].ToString();

                                if (!ValidateField(xlcol_PIssueDate, StrPIssueDate, isUpdate, ref fieldErr, ref emptyObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {
                                    PassportIssueDateValue = GetValidDateTime(StrPIssueDate);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_PIssueDate] + " " + PassportIssueDateValue.ToString("dd/MM/yyyy") + "]");

                                }
                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_PExpiryDate))
                        {
                            if (row[xlcol_PExpiryDate] == null || row[xlcol_PExpiryDate].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                    if (CheckIfMandatory(xlcol_PExpiryDate, enmXlImportTables.Employee))
                                    {
                                        AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_PExpiryDate] + " cannot be blank");
                                        bskipInsertUpdate = true;
                                    }
                                }
                                else if (drowEmpOld != null && drowEmpOld["PExpiryDate"] != DBNull.Value)
                                {
                                    PassportExpiryDateValue = GetValidDateTime(drowEmpOld["PExpiryDate"].ToString());
                                }
                            }
                            else
                            {

                                StrPExpiryDate = row[xlcol_PExpiryDate].ToString();

                                if (!ValidateField(xlcol_PExpiryDate, StrPExpiryDate, isUpdate, ref fieldErr, ref emptyObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {


                                    PassportExpiryDateValue = GetValidDateTime(StrPExpiryDate);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_PExpiryDate] + " " + PassportExpiryDateValue.ToString("dd/MM/yyyy") + "]");

                                }


                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_Religion))
                        {
                            if (row[xlcol_Religion] == null || row[xlcol_Religion].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                    //if (CheckIfMandatory(xlcol_Religion, enmXlImportTables.Employee))
                                    //{
                                    //    AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_Religion] + " cannot be blank");
                                    //    bskipInsertUpdate = true;
                                    //}
                                }
                                else if (drowEmpOld != null && drowEmpOld["Religion"] != DBNull.Value)
                                {
                                    ReligionCode = Convert.ToString(drowEmpOld["Religion"].ToString());
                                }
                            }
                            else
                            {
                                StrReligion = row[xlcol_Religion].ToString();

                                //if (!ValidateField(xlcol_Religion, StrReligion, isUpdate, ref fieldErr, ref lookupCodeObj))
                                //{
                                   // AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                //}
                                //else
                                //{
                                    ReligionCode = Convert.ToString(lookupCodeObj);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_Religion] + " " + StrReligion + "]");
                                //}
                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_PassportProf))
                        {
                            if (row[xlcol_PassportProf] == null || row[xlcol_PassportProf].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                    if (CheckIfMandatory(xlcol_PassportProf, enmXlImportTables.Employee))
                                    {
                                        AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_PassportProf] + " cannot be blank");
                                        bskipInsertUpdate = true;
                                    }
                                }
                                else if (drowEmpOld != null && drowEmpOld["PassportProf"] != DBNull.Value)
                                {
                                    VisaProfessionCode = Convert.ToString(drowEmpOld["PassportProf"].ToString());
                                }
                            }
                            else
                            {

                                StrPassportProf = row[xlcol_PassportProf].ToString();

                                if (!ValidateField(xlcol_PassportProf, StrPassportProf, isUpdate, ref fieldErr, ref lookupCodeObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {
                                    VisaProfessionCode = Convert.ToString(lookupCodeObj);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_PassportProf] + " " + StrPassportProf + "]");

                                }
                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_Education))
                        {
                            if (row[xlcol_Education] == null || row[xlcol_Education].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                    if (CheckIfMandatory(xlcol_Education, enmXlImportTables.Employee))
                                    {
                                        AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_Education] + " cannot be blank");
                                        bskipInsertUpdate = true;
                                    }
                                }
                                else if (drowEmpOld != null && drowEmpOld["Education"] != DBNull.Value)
                                {
                                    VisaQualificationCode = Convert.ToString(drowEmpOld["Education"].ToString());
                                }
                            }
                            else
                            {

                                StrEducation = row[xlcol_Education].ToString();

                                if (!ValidateField(xlcol_Education, StrEducation, isUpdate, ref fieldErr, ref lookupCodeObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {
                                    VisaQualificationCode = Convert.ToString(lookupCodeObj);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_Education] + " " + StrEducation + "]");

                                }
                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_PerAddressE))
                        {
                            if (row[xlcol_PerAddressE] == null || row[xlcol_PerAddressE].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                    if (CheckIfMandatory(xlcol_PerAddressE, enmXlImportTables.Employee))
                                    {
                                        AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_PerAddressE] + " cannot be blank");
                                        bskipInsertUpdate = true;
                                    }
                                }
                                else if (drowEmpOld != null && drowEmpOld["PerAddressE"] != DBNull.Value)
                                {
                                    StrPerAddressE = Convert.ToString(drowEmpOld["PerAddressE"]);
                                }
                            }
                            else
                            {
                                StrPerAddressE = row[xlcol_PerAddressE].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_PerAddressE] + " " + StrPerAddressE + "]");
                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_PerAddressA))
                        {
                            if (row[xlcol_PerAddressA] == null || row[xlcol_PerAddressA].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["PerAddressA"] != DBNull.Value)
                                {
                                    StrPerAddressA = Convert.ToString(drowEmpOld["PerAddressA"]);
                                }
                            }
                            else
                            {
                                StrPerAddressA = row[xlcol_PerAddressA].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_PerAddressA] + " " + StrPerAddressA + "]");
                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_Skill1))
                        {
                            if (row[xlcol_Skill1] == null || row[xlcol_Skill1].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["Skill1"] != DBNull.Value)
                                {
                                    Skill1Code = Convert.ToString(drowEmpOld["Skill1"]);
                                }
                            }
                            else
                            {
                                StrSkill1 = row[xlcol_Skill1].ToString();

                                if (!ValidateField(xlcol_Skill1, StrSkill1, isUpdate, ref fieldErr, ref lookupCodeObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {
                                    Skill1Code = Convert.ToString(lookupCodeObj);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_Skill1] + " " + StrSkill1 + "]");
                                }
                            }
                        }

                        if (CheckIfColumnExists(mydt, xlcol_Skill2))
                        {
                            if (row[xlcol_Skill2] == null || row[xlcol_Skill2].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["Skill2"] != DBNull.Value)
                                {
                                    Skill2Code = Convert.ToString(drowEmpOld["Skill2"]);
                                }
                            }
                            else
                            {
                                StrSkill2 = row[xlcol_Skill2].ToString();

                                if (!ValidateField(xlcol_Skill2, StrSkill2, isUpdate, ref fieldErr, ref lookupCodeObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {
                                    Skill2Code = Convert.ToString(lookupCodeObj);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_Skill2] + " " + StrSkill2 + "]");
                                }
                            }
                        }

                        if (CheckIfColumnExists(mydt, xlcol_Skill3))
                        {
                            if (row[xlcol_Skill3] == null || row[xlcol_Skill3].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["Skill3"] != DBNull.Value)
                                {
                                    Skill3Code = Convert.ToString(drowEmpOld["Skill3"]);
                                }
                            }
                            else
                            {
                                StrSkill3 = row[xlcol_Skill3].ToString();

                                if (!ValidateField(xlcol_Skill3, StrSkill3, isUpdate, ref fieldErr, ref lookupCodeObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {
                                    Skill3Code = Convert.ToString(lookupCodeObj);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_Skill3] + " " + StrSkill3 + "]");
                                }
                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_Language1))
                        {
                            if (row[xlcol_Language1] == null || row[xlcol_Language1].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                    if (CheckIfMandatory(xlcol_Language1, enmXlImportTables.Employee))
                                    {
                                        AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_Language1] + " cannot be blank");
                                        bskipInsertUpdate = true;
                                    }
                                }
                                else if (drowEmpOld != null && drowEmpOld["Language1"] != DBNull.Value)
                                {
                                    Language1Code = Convert.ToString(drowEmpOld["Language1"]);
                                }
                            }
                            else
                            {
                                StrLanguage1 = row[xlcol_Language1].ToString();

                                if (!ValidateField(xlcol_Language1, StrLanguage1, isUpdate, ref fieldErr, ref lookupCodeObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {
                                    Language1Code = Convert.ToString(lookupCodeObj);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_Language1] + " " + StrLanguage1 + "]");
                                }
                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_Language2))
                        {
                            if (row[xlcol_Language2] == null || row[xlcol_Language2].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                    if (CheckIfMandatory(xlcol_Language2, enmXlImportTables.Employee))
                                    {
                                        AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_Language2] + " cannot be blank");
                                        bskipInsertUpdate = true;
                                    }
                                }
                                else if (drowEmpOld != null && drowEmpOld["Language2"] != DBNull.Value)
                                {
                                    Language2Code = Convert.ToString(drowEmpOld["Language2"]);
                                }
                            }
                            else
                            {
                                StrLanguage2 = row[xlcol_Language2].ToString();

                                if (!ValidateField(xlcol_Language2, StrLanguage2, isUpdate, ref fieldErr, ref lookupCodeObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {
                                    Language2Code = Convert.ToString(lookupCodeObj);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_Language2] + " " + StrLanguage2 + "]");
                                }
                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_Language3))
                        {
                            if (row[xlcol_Language3] == null || row[xlcol_Language3].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["Language3"] != DBNull.Value)
                                {
                                    Language3Code = Convert.ToString(drowEmpOld["Language3"]);
                                }
                            }
                            else
                            {
                                StrLanguage3 = row[xlcol_Language3].ToString();

                                if (!ValidateField(xlcol_Language3, StrLanguage3, isUpdate, ref fieldErr, ref lookupCodeObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {
                                    Language3Code = Convert.ToString(lookupCodeObj);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_Language3] + " " + StrLanguage3 + "]");
                                }
                            }
                        }

                        if (CheckIfColumnExists(mydt, xlcol_VisaType))
                        {
                            if (row[xlcol_VisaType] == null || row[xlcol_VisaType].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["VisaType"] != DBNull.Value)
                                {
                                    VisaTypeCode = Convert.ToString(drowEmpOld["VisaType"]);
                                }
                            }
                            else
                            {
                                StrVisaType = row[xlcol_VisaType].ToString();

                                if (!ValidateField(xlcol_VisaType, StrVisaType, isUpdate, ref fieldErr, ref lookupCodeObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {
                                    VisaTypeCode = Convert.ToString(lookupCodeObj);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_VisaType] + " " + StrVisaType + "]");
                                }
                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_VisaNo))
                        {
                            if (row[xlcol_VisaNo] == null || row[xlcol_VisaNo].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["VisaNo"] != DBNull.Value)
                                {
                                    StrVisaNo = Convert.ToString(drowEmpOld["VisaNo"]);
                                }
                            }
                            else
                            {
                                StrVisaNo = row[xlcol_VisaNo].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_VisaNo] + " " + StrVisaNo + "]");
                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_VisaIssueDate))
                        {
                            if (row[xlcol_VisaIssueDate] == null || row[xlcol_VisaIssueDate].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["VisaIssueDate"] != DBNull.Value)
                                {
                                    VisaIssueDateValue = GetValidDateTime(drowEmpOld["VisaIssueDate"].ToString());
                                }
                            }
                            else
                            {

                                StrVisaIssueDate = row[xlcol_VisaIssueDate].ToString();

                                if (!ValidateField(xlcol_VisaIssueDate, StrVisaIssueDate, isUpdate, ref fieldErr, ref emptyObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {


                                    VisaIssueDateValue = GetValidDateTime(StrVisaIssueDate);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_VisaIssueDate] + " " + VisaIssueDateValue.ToString("dd/MM/yyyy") + "]");

                                }


                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_ImmigFileNo))
                        {
                            if (row[xlcol_ImmigFileNo] == null || row[xlcol_ImmigFileNo].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["ImmigFileNo"] != DBNull.Value)
                                {
                                    StrImmigFileNo = Convert.ToString(drowEmpOld["ImmigFileNo"]);
                                }
                            }
                            else
                            {
                                StrImmigFileNo = row[xlcol_ImmigFileNo].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_ImmigFileNo] + " " + StrImmigFileNo + "]");
                            }

                        }


                        if (CheckIfColumnExists(mydt, xlcol_EntryPlace))
                        {
                            if (row[xlcol_EntryPlace] == null || row[xlcol_EntryPlace].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["EntryPlace"] != DBNull.Value)
                                {
                                    EntryPlaceCode = Convert.ToString(drowEmpOld["EntryPlace"]);
                                }
                            }
                            else
                            {
                                StrEntryPlace = row[xlcol_EntryPlace].ToString();

                                if (!ValidateField(xlcol_EntryPlace, StrEntryPlace, isUpdate, ref fieldErr, ref lookupCodeObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {
                                    EntryPlaceCode = Convert.ToString(lookupCodeObj);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_EntryPlace] + " " + StrEntryPlace + "]");
                                }
                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_EntryDate))
                        {
                            if (row[xlcol_EntryDate] == null || row[xlcol_EntryDate].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                    if (CheckIfMandatory(xlcol_EntryDate, enmXlImportTables.Employee))
                                    {
                                        AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_EntryDate] + " cannot be blank");
                                        bskipInsertUpdate = true;
                                    }
                                }
                                else if (drowEmpOld != null && drowEmpOld["EntryDate"] != DBNull.Value)
                                {
                                    EntryDateValue = GetValidDateTime(drowEmpOld["EntryDate"].ToString());
                                }
                            }
                            else
                            {

                                StrEntryDate = row[xlcol_EntryDate].ToString();

                                if (!ValidateField(xlcol_EntryDate, StrEntryDate, isUpdate, ref fieldErr, ref emptyObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {


                                    EntryDateValue = GetValidDateTime(StrEntryDate);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_EntryDate] + " " + EntryDateValue.ToString("dd/MM/yyyy") + "]");

                                }


                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_ResidenceNo))
                        {
                            if (row[xlcol_ResidenceNo] == null || row[xlcol_ResidenceNo].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["ResidenceNo"] != DBNull.Value)
                                {
                                    StrResidenceNo = Convert.ToString(drowEmpOld["ResidenceNo"]);
                                }
                            }
                            else
                            {
                                StrResidenceNo = row[xlcol_ResidenceNo].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_ResidenceNo] + " " + StrResidenceNo + "]");

                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_ResIssueDate))
                        {
                            if (row[xlcol_ResIssueDate] == null || row[xlcol_ResIssueDate].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["ResIssueDate"] != DBNull.Value)
                                {
                                    ResIssueDateValue = GetValidDateTime(drowEmpOld["ResIssueDate"].ToString());
                                }
                            }
                            else
                            {

                                StrResIssueDate = row[xlcol_ResIssueDate].ToString();

                                if (!ValidateField(xlcol_ResIssueDate, StrResIssueDate, isUpdate, ref fieldErr, ref emptyObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {


                                    ResIssueDateValue = GetValidDateTime(StrResIssueDate);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_ResIssueDate] + " " + ResIssueDateValue.ToString("dd/MM/yyyy") + "]");

                                }


                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_ResExpDate))
                        {
                            if (row[xlcol_ResExpDate] == null || row[xlcol_ResExpDate].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["ResExpDate"] != DBNull.Value)
                                {
                                    ResExpDateValue = GetValidDateTime(drowEmpOld["ResExpDate"].ToString());
                                }
                            }
                            else
                            {

                                StrResExpDate = row[xlcol_ResExpDate].ToString();

                                if (!ValidateField(xlcol_ResExpDate, StrResExpDate, isUpdate, ref fieldErr, ref emptyObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {


                                    ResExpDateValue = GetValidDateTime(StrResExpDate);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_ResExpDate] + " " + ResExpDateValue.ToString("dd/MM/yyyy") + "]");

                                }


                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_ResIssuePlace))
                        {

                            if (row[xlcol_ResIssuePlace] == null || row[xlcol_ResIssuePlace].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["ResIssuePlace"] != DBNull.Value)
                                {
                                    ResIssuePlaceCode = Convert.ToString(drowEmpOld["ResIssuePlace"]);
                                }
                            }
                            else
                            {
                                StrResIssuePlace = row[xlcol_ResIssuePlace].ToString();

                                if (!ValidateField(xlcol_ResIssuePlace, StrResIssuePlace, isUpdate, ref fieldErr, ref lookupCodeObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {
                                    ResIssuePlaceCode = Convert.ToString(lookupCodeObj);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_ResIssuePlace] + " " + StrResIssuePlace + "]");
                                }
                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_LabCardNo))
                        {
                            if (row[xlcol_LabCardNo] == null || row[xlcol_LabCardNo].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["LabCardNo"] != DBNull.Value)
                                {
                                    StrLabCardNo = Convert.ToString(drowEmpOld["LabCardNo"]);
                                }

                            }
                            else
                            {
                                StrLabCardNo = row[xlcol_LabCardNo].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_LabCardNo] + " " + StrLabCardNo + "]");

                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_LCIssueDate))
                        {
                            if (row[xlcol_LCIssueDate] == null || row[xlcol_LCIssueDate].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["LCIssueDate"] != DBNull.Value)
                                {
                                    LCIssueDateValue = GetValidDateTime(drowEmpOld["LCIssueDate"].ToString());
                                }
                            }
                            else
                            {

                                StrLCIssueDate = row[xlcol_LCIssueDate].ToString();

                                if (!ValidateField(xlcol_LCIssueDate, StrLCIssueDate, isUpdate, ref fieldErr, ref emptyObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {


                                    LCIssueDateValue = GetValidDateTime(StrLCIssueDate);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_LCIssueDate] + " " + LCIssueDateValue.ToString("dd/MM/yyyy") + "]");

                                }


                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_LCExpDate))
                        {
                            if (row[xlcol_LCExpDate] == null || row[xlcol_LCExpDate].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["LCExpDate"] != DBNull.Value)
                                {
                                    LCExpDateValue = GetValidDateTime(drowEmpOld["LCExpDate"].ToString());
                                }
                            }
                            else
                            {

                                StrLCExpDate = row[xlcol_LCExpDate].ToString();

                                if (!ValidateField(xlcol_LCExpDate, StrLCExpDate, isUpdate, ref fieldErr, ref emptyObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {


                                    LCExpDateValue = GetValidDateTime(StrLCExpDate);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_LCExpDate] + " " + LCExpDateValue.ToString("dd/MM/yyyy") + "]");

                                }


                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_HlthCardNo))
                        {
                            if (row[xlcol_HlthCardNo] == null || row[xlcol_HlthCardNo].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["HlthCardNo"] != DBNull.Value)
                                {
                                    StrHlthCardNo = Convert.ToString(drowEmpOld["HlthCardNo"]);
                                }
                            }
                            else
                            {
                                StrHlthCardNo = row[xlcol_HlthCardNo].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_HlthCardNo] + " " + StrHlthCardNo + "]");

                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_HCIssueDate))
                        {
                            if (row[xlcol_HCIssueDate] == null || row[xlcol_HCIssueDate].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["HCIssueDate"] != DBNull.Value)
                                {
                                    HCIssueDateValue = GetValidDateTime(drowEmpOld["HCIssueDate"].ToString());
                                }
                            }
                            else
                            {

                                StrHCIssueDate = row[xlcol_HCIssueDate].ToString();

                                if (!ValidateField(xlcol_HCIssueDate, StrHCIssueDate, isUpdate, ref fieldErr, ref emptyObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {


                                    HCIssueDateValue = GetValidDateTime(StrHCIssueDate);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_HCIssueDate] + " " + HCIssueDateValue.ToString("dd/MM/yyyy") + "]");

                                }


                            }
                        }

                        if (CheckIfColumnExists(mydt, xlcol_HCExpiryDate))
                        {
                            if (row[xlcol_HCExpiryDate] == null || row[xlcol_HCExpiryDate].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["HCExpiryDate"] != DBNull.Value)
                                {
                                    HCExpiryDateValue = GetValidDateTime(drowEmpOld["HCExpiryDate"].ToString());
                                }
                            }
                            else
                            {

                                StrHCExpiryDate = row[xlcol_HCExpiryDate].ToString();

                                if (!ValidateField(xlcol_HCExpiryDate, StrHCExpiryDate, isUpdate, ref fieldErr, ref emptyObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {


                                    HCExpiryDateValue = GetValidDateTime(StrHCExpiryDate);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_HCExpiryDate] + " " + HCExpiryDateValue.ToString("dd/MM/yyyy") + "]");

                                }


                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_HCIssuePlace))
                        {
                            if (row[xlcol_HCIssuePlace] == null || row[xlcol_HCIssuePlace].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["HCIssuePlace"] != DBNull.Value)
                                {
                                    HCIssuePlaceCode = Convert.ToString(drowEmpOld["HCIssuePlace"]);
                                }
                            }
                            else
                            {
                                StrHCIssuePlace = row[xlcol_HCIssuePlace].ToString();

                                if (!ValidateField(xlcol_HCIssuePlace, StrHCIssuePlace, isUpdate, ref fieldErr, ref lookupCodeObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {
                                    HCIssuePlaceCode = Convert.ToString(lookupCodeObj);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_HCIssuePlace] + " " + StrHCIssuePlace + "]");
                                }
                            }

                        }


                        if (CheckIfColumnExists(mydt, xlcol_DrvLicNo))
                        {
                            if (row[xlcol_DrvLicNo] == null || row[xlcol_DrvLicNo].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["DrvLicNo"] != DBNull.Value)
                                {
                                    StrDrvLicNo = Convert.ToString(drowEmpOld["DrvLicNo"]);
                                }
                            }
                            else
                            {

                                StrDrvLicNo = row[xlcol_DrvLicNo].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_DrvLicNo] + " " + StrDrvLicNo + "]");

                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_DLCategory))
                        {
                            if (row[xlcol_DLCategory] == null || row[xlcol_DLCategory].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["DLCategory"] != DBNull.Value)
                                {
                                    DLCategoryCode = Convert.ToInt32(drowEmpOld["DLCategory"]);
                                }
                            }
                            else
                            {

                                StrDLCategory = row[xlcol_DLCategory].ToString();

                                if (!ValidateField(xlcol_DLCategory, StrDLCategory, isUpdate, ref fieldErr, ref lookupCodeObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {
                                    DLCategoryCode = Convert.ToInt32(lookupCodeObj);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_DLCategory] + " " + StrDLCategory + "]");
                                }
                            }
                        }

                        if (CheckIfColumnExists(mydt, xlcol_DLIssuePlace))
                        {
                            if (row[xlcol_DLIssuePlace] == null || row[xlcol_DLIssuePlace].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["DLIssuePlace"] != DBNull.Value)
                                {
                                    StrDLIssuePlace = Convert.ToString(drowEmpOld["DLIssuePlace"]);
                                }
                            }
                            else
                            {
                                StrDLIssuePlace = row[xlcol_DLIssuePlace].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_DLIssuePlace] + " " + StrDLIssuePlace + "]");

                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_DLIssueDate))
                        {
                            if (row[xlcol_DLIssueDate] == null || row[xlcol_DLIssueDate].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["DLIssueDate"] != DBNull.Value)
                                {
                                    DLIssueDateValue = GetValidDateTime(drowEmpOld["DLIssueDate"].ToString());
                                }
                            }
                            else
                            {
                                StrDLIssueDate = row[xlcol_DLIssueDate].ToString();

                                if (!ValidateField(xlcol_DLIssueDate, StrDLIssueDate, isUpdate, ref fieldErr, ref emptyObj))
                                {
                                    ErrorFileStr += StrEmpCode + " :- " + fieldErr + " : " + Environment.NewLine;
                                    errCount++; errTotalCount++;

                                }
                                else
                                {


                                    DLIssueDateValue = GetValidDateTime(StrDLIssueDate);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_DLIssueDate] + " " + DLIssueDateValue.ToString("dd/MM/yyyy") + "]");

                                }
                            }
                        }

                        if (CheckIfColumnExists(mydt, xlcol_DLExpiryDate))
                        {
                            if (row[xlcol_DLExpiryDate] == null || row[xlcol_DLExpiryDate].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["DLExpiryDate"] != DBNull.Value)
                                {
                                    DLExpiryDateValue = GetValidDateTime(drowEmpOld["DLExpiryDate"].ToString());
                                }
                            }
                            else
                            {

                                StrDLExpiryDate = row[xlcol_DLExpiryDate].ToString();

                                if (!ValidateField(xlcol_DLExpiryDate, StrDLExpiryDate, isUpdate, ref fieldErr, ref emptyObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {


                                    DLExpiryDateValue = GetValidDateTime(StrDLExpiryDate);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_DLExpiryDate] + " " + DLExpiryDateValue.ToString("dd/MM/yyyy") + "]");

                                }


                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_SponsorCode))
                        {
                            if (row[xlcol_SponsorCode] == null || row[xlcol_SponsorCode].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                    if (CheckIfMandatory(xlcol_SponsorCode, enmXlImportTables.Employee))
                                    {
                                        AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_SponsorCode] + " cannot be blank");
                                        bskipInsertUpdate = true;
                                    }
                                }
                                else if (drowEmpOld != null && drowEmpOld["SponsorCode"] != DBNull.Value)
                                {
                                    StrSponsorCode = Convert.ToString(drowEmpOld["SponsorCode"]);
                                }
                            }
                            else
                            {

                                StrSponsorName = row[xlcol_SponsorCode].ToString();

                                if (!ValidateField(xlcol_SponsorCode, StrSponsorName, isUpdate, ref fieldErr, ref lookupCodeObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {
                                    StrSponsorCode = Convert.ToString(lookupCodeObj);
                                    
                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_SponsorCode] + " " + StrSponsorName + "]");
                                    
                                }
                            }
                        }

                        if (CheckIfColumnExists(mydt, xlcol_SponByOther))
                        {
                            if (row[xlcol_SponByOther] == null || row[xlcol_SponByOther].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["SponByOther"] != DBNull.Value)
                                {
                                    SponByOtherCode = Convert.ToInt32(drowEmpOld["SponByOther"]);
                                }
                            }
                            else
                            {

                                StrSponByOther = row[xlcol_SponByOther].ToString();

                                if (!ValidateField(xlcol_SponByOther, StrSponByOther, isUpdate, ref fieldErr, ref lookupCodeObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);
                                }
                                else
                                {
                                    SponByOtherCode = Convert.ToInt32(lookupCodeObj);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_SponByOther] + " " + StrSponByOther + "]");
                                }
                            }
                        }

                        if (CheckIfColumnExists(mydt, xlcol_OSponNameE))
                        {
                            if (row[xlcol_OSponNameE] == null || row[xlcol_OSponNameE].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["OSponNameE"] != DBNull.Value)
                                {
                                    StrOSponNameE = Convert.ToString(drowEmpOld["OSponNameE"]);
                                }
                            }
                            else
                            {
                                StrOSponNameE = row[xlcol_OSponNameE].ToString();

                                if (!ValidateField(xlcol_OSponNameE, StrOSponNameE, isUpdate, ref fieldErr, ref emptyObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {
                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_OSponNameE] + " " + StrOSponNameE + "]");
                                }
                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_OSponNameA))
                        {
                            if (row[xlcol_OSponNameA] == null || row[xlcol_OSponNameA].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["OSponNameA"] != DBNull.Value)
                                {
                                    StrOSponNameA = Convert.ToString(drowEmpOld["OSponNameA"]);
                                }
                            }
                            else
                            {
                                StrOSponNameA = row[xlcol_OSponNameA].ToString();

                                if (!ValidateField(xlcol_OSponNameA, StrOSponNameA, isUpdate, ref fieldErr, ref emptyObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {
                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_OSponNameA] + " " + StrOSponNameA + "]");
                                }
                            }
                        }

                        if (CheckIfColumnExists(mydt, xlcol_OSponRel))
                        {
                            if (row[xlcol_OSponRel] == null || row[xlcol_OSponRel].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["OSponRel"] != DBNull.Value)
                                {
                                    OSponRelCode = Convert.ToString(drowEmpOld["OSponRel"]);
                                }
                            }
                            else
                            {

                                StrOSponRel = row[xlcol_OSponRel].ToString();

                                if (!ValidateField(xlcol_OSponRel, StrOSponRel, isUpdate, ref fieldErr, ref lookupCodeObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {
                                    OSponRelCode = Convert.ToString(lookupCodeObj);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_OSponRel] + " " + StrOSponRel + "]");
                                }
                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_OSponNation))
                        {

                            if (row[xlcol_OSponNation] == null || row[xlcol_OSponNation].ToString() == string.Empty)
                            {
                                //Empty check
                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["OSponNation"] != DBNull.Value)
                                {
                                    OSponNationCode = Convert.ToString(drowEmpOld["OSponNation"]);
                                }
                            }
                            else
                            {

                                StrOSponNation = row[xlcol_OSponNation].ToString();

                                if (!ValidateField(xlcol_OSponNation, StrOSponNation, isUpdate, ref fieldErr, ref lookupCodeObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {
                                    OSponNationCode = Convert.ToString(lookupCodeObj);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_OSponNation] + " " + StrOSponNation + "]");
                                }
                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_OSponVisaNo))
                        {
                            if (row[xlcol_OSponVisaNo] == null || row[xlcol_OSponVisaNo].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["OSponVisaNo"] != DBNull.Value)
                                {
                                    StrOSponVisaNo = Convert.ToString(drowEmpOld["OSponVisaNo"]);
                                }
                            }
                            else
                            {
                                StrOSponVisaNo = row[xlcol_OSponVisaNo].ToString();

                                if (!ValidateField(xlcol_OSponVisaNo, StrOSponVisaNo, isUpdate, ref fieldErr, ref emptyObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {
                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_OSponVisaNo] + " " + StrOSponVisaNo + "]");
                                }
                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_OSponVExpDt))
                        {
                            if (row[xlcol_OSponVExpDt] == null || row[xlcol_OSponVExpDt].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["OSponVExpDt"] != DBNull.Value)
                                {
                                    OSponVExpDtValue = GetValidDateTime(drowEmpOld["OSponVExpDt"].ToString());
                                }
                            }
                            else
                            {
                                StrOSponVExpDt = row[xlcol_OSponVExpDt].ToString();

                                if (!ValidateField(xlcol_OSponVExpDt, StrOSponVExpDt, isUpdate, ref fieldErr, ref emptyObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {


                                    OSponVExpDtValue = GetValidDateTime(StrOSponVExpDt);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_OSponVExpDt] + " " + OSponVExpDtValue.ToString("dd/MM/yyyy") + "]");

                                }
                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_OSponPsprtNoE))
                        {
                            if (row[xlcol_OSponPsprtNoE] == null || row[xlcol_OSponPsprtNoE].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["OSponPsprtNoE"] != DBNull.Value)
                                {
                                    StrOSponPsprtNoE = Convert.ToString(drowEmpOld["OSponPsprtNoE"]);
                                }
                            }
                            else
                            {
                                StrOSponPsprtNoE = row[xlcol_OSponPsprtNoE].ToString();

                                if (!ValidateField(xlcol_OSponPsprtNoE, StrOSponPsprtNoE, isUpdate, ref fieldErr, ref emptyObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {
                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_OSponPsprtNoE] + " " + StrOSponPsprtNoE + "]");
                                }
                            }
                        }

                        if (CheckIfColumnExists(mydt, xlcol_OSponPsprtNoA))
                        {
                            if (row[xlcol_OSponPsprtNoA] == null || row[xlcol_OSponPsprtNoA].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["OSponPsprtNoA"] != DBNull.Value)
                                {
                                    StrOSponPsprtNoA = Convert.ToString(drowEmpOld["OSponPsprtNoA"]);
                                }
                            }
                            else
                            {
                                StrOSponPsprtNoA = row[xlcol_OSponPsprtNoA].ToString();

                                if (!ValidateField(xlcol_OSponPsprtNoA, StrOSponPsprtNoA, isUpdate, ref fieldErr, ref emptyObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {
                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_OSponPsprtNoA] + " " + StrOSponPsprtNoA + "]");
                                }
                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_Emirates))
                        {
                            if (row[xlcol_Emirates] == null || row[xlcol_Emirates].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                    if (CheckIfMandatory(xlcol_Emirates, enmXlImportTables.Employee))
                                    {
                                        AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_Emirates] + " cannot be blank");
                                        bskipInsertUpdate = true;
                                    }
                                }
                                else if (drowEmpOld != null && drowEmpOld["Emirates"] != DBNull.Value)
                                {
                                    EmiratesCode = Convert.ToString(drowEmpOld["Emirates"]);
                                }
                            }
                            else
                            {

                                StrEmirates = row[xlcol_Emirates].ToString();

                                if (!ValidateField(xlcol_Emirates, StrEmirates, isUpdate, ref fieldErr, ref lookupCodeObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {
                                    EmiratesCode = Convert.ToString(lookupCodeObj);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_Emirates] + " " + StrEmirates + "]");
                                }
                            }
                        }

                        if (CheckIfColumnExists(mydt, xlcol_City))
                        {
                            if (row[xlcol_City] == null || row[xlcol_City].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                    if (CheckIfMandatory(xlcol_City, enmXlImportTables.Employee))
                                    {
                                        AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_City] + " cannot be blank");
                                        bskipInsertUpdate = true;
                                    }
                                }
                                else if (drowEmpOld != null && drowEmpOld["City"] != DBNull.Value)
                                {
                                    CityCode = Convert.ToString(drowEmpOld["City"]);
                                }
                            }
                            else
                            {

                                StrCity = row[xlcol_City].ToString();

                                //if (!ValidateField(xlcol_City, StrCity, isUpdate, ref fieldErr, ref lookupCodeObj))
                                //{
                                //    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                //}
                                //else
                                //{
                                    CityCode = Convert.ToString(lookupCodeObj);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_City] + " " + StrCity + "]");
                                //}
                            }
                        }

                        if (CheckIfColumnExists(mydt, xlcol_Area))
                        {
                            if (row[xlcol_Area] == null || row[xlcol_Area].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                    if (CheckIfMandatory(xlcol_Area, enmXlImportTables.Employee))
                                    {
                                        AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_Area] + " cannot be blank");
                                        bskipInsertUpdate = true;
                                    }
                                }
                                else if (drowEmpOld != null && drowEmpOld["Area"] != DBNull.Value)
                                {
                                    StrArea = Convert.ToString(drowEmpOld["Area"]);
                                }
                            }
                            else
                            {

                                StrArea = row[xlcol_Area].ToString();

                                //if (!ValidateField(xlcol_Area, StrArea, isUpdate, ref fieldErr, ref lookupCodeObj))
                                //{
                                //    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                //}
                                //else
                                //{
                                    AreaCode = Convert.ToString(lookupCodeObj);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_Area] + " " + StrArea + "]");
                                //}
                            }
                        }

                        if (CheckIfColumnExists(mydt, xlcol_StreetE))
                        {
                            if (row[xlcol_StreetE] == null || row[xlcol_StreetE].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                    if (CheckIfMandatory(xlcol_StreetE, enmXlImportTables.Employee))
                                    {
                                        AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_StreetE] + " cannot be blank");
                                        bskipInsertUpdate = true;
                                    }
                                }
                                else if (drowEmpOld != null && drowEmpOld["StreetE"] != DBNull.Value)
                                {
                                    StrStreetE = Convert.ToString(drowEmpOld["StreetE"]);
                                }
                            }
                            else
                            {
                                StrStreetE = row[xlcol_StreetE].ToString();

                                //if (!ValidateField(xlcol_StreetE, StrStreetE, isUpdate, ref fieldErr, ref emptyObj))
                                //{
                                //    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                //}
                                //else
                                //{
                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_StreetE] + " " + StrStreetE + "]");
                                //}
                            }
                        }

                        if (CheckIfColumnExists(mydt, xlcol_StreetA))
                        {
                            if (row[xlcol_StreetA] == null || row[xlcol_StreetA].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["StreetA"] != DBNull.Value)
                                {
                                    StrStreetA = Convert.ToString(drowEmpOld["StreetA"]);
                                }
                            }
                            else
                            {
                                StrStreetA = row[xlcol_StreetA].ToString();

                                //if (!ValidateField(xlcol_StreetA, StrStreetA, isUpdate, ref fieldErr, ref emptyObj))
                                //{
                                //    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                //}
                                //else
                                //{
                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_StreetA] + " " + StrStreetA + "]");
                               // }
                            }
                        }

                        if (CheckIfColumnExists(mydt, xlcol_BuildingE))
                        {
                            if (row[xlcol_BuildingE] == null || row[xlcol_BuildingE].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                    if (CheckIfMandatory(xlcol_BuildingE, enmXlImportTables.Employee))
                                    {
                                        AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_BuildingE] + " cannot be blank");
                                        bskipInsertUpdate = true;
                                    }
                                }
                                else if (drowEmpOld != null && drowEmpOld["BuildingE"] != DBNull.Value)
                                {
                                    StrBuildingE = Convert.ToString(drowEmpOld["BuildingE"]);
                                }
                            }
                            else
                            {
                                StrBuildingE = row[xlcol_BuildingE].ToString();

                                //if (!ValidateField(xlcol_BuildingE, StrBuildingE, isUpdate, ref fieldErr, ref emptyObj))
                                //{
                                //    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                //}
                                //else
                                //{
                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_BuildingE] + " " + StrBuildingE + "]");
                                //}
                            }
                        }

                        if (CheckIfColumnExists(mydt, xlcol_BuildingA))
                        {
                            if (row[xlcol_BuildingA] == null || row[xlcol_BuildingA].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["BuildingA"] != DBNull.Value)
                                {
                                    StrBuildingA = Convert.ToString(drowEmpOld["BuildingA"]);
                                }
                            }
                            else
                            {
                                StrBuildingA = row[xlcol_BuildingA].ToString();

                                //if (!ValidateField(xlcol_BuildingA, StrBuildingA, isUpdate, ref fieldErr, ref emptyObj))
                                //{
                                //    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                //}
                                //else
                                //{
                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_BuildingA] + " " + StrBuildingA + "]");
                               // }
                            }
                        }

                        if (CheckIfColumnExists(mydt, xlcol_FlatE))
                        {
                            if (row[xlcol_FlatE] == null || row[xlcol_FlatE].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                    if (CheckIfMandatory(xlcol_FlatE, enmXlImportTables.Employee))
                                    {
                                        AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_FlatE] + " cannot be blank");
                                        bskipInsertUpdate = true;
                                    }
                                }
                                else if (drowEmpOld != null && drowEmpOld["FlatE"] != DBNull.Value)
                                {
                                    StrFlatE = Convert.ToString(drowEmpOld["FlatE"]);
                                }
                            }
                            else
                            {
                                StrFlatE = row[xlcol_FlatE].ToString();

                                //if (!ValidateField(xlcol_FlatE, StrFlatE, isUpdate, ref fieldErr, ref emptyObj))
                                //{
                                //    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                //}
                                //else
                                //{
                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_FlatE] + " " + StrFlatE + "]");
                                //}
                            }
                        }

                        if (CheckIfColumnExists(mydt, xlcol_FlatA))
                        {
                            if (row[xlcol_FlatA] == null || row[xlcol_FlatA].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["FlatA"] != DBNull.Value)
                                {
                                    StrFlatA = Convert.ToString(drowEmpOld["FlatA"]);
                                }
                            }
                            else
                            {
                                StrFlatA = row[xlcol_FlatA].ToString();

                                //if (!ValidateField(xlcol_FlatA, StrFlatA, isUpdate, ref fieldErr, ref emptyObj))
                                //{
                                //    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                //}
                                //else
                                //{
                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_FlatA] + " " + StrFlatA + "]");
                                //}
                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_OffPhoneNo))
                        {
                            if (row[xlcol_OffPhoneNo] == null || row[xlcol_OffPhoneNo].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                    if (CheckIfMandatory(xlcol_OffPhoneNo, enmXlImportTables.Employee))
                                    {
                                        AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_OffPhoneNo] + " cannot be blank");
                                        bskipInsertUpdate = true;
                                    }
                                }
                                else if (drowEmpOld != null && drowEmpOld["OffPhoneNo"] != DBNull.Value)
                                {
                                    StrOffPhoneNo = Convert.ToString(drowEmpOld["OffPhoneNo"]);
                                }
                            }
                            else
                            {

                                StrOffPhoneNo = row[xlcol_OffPhoneNo].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_OffPhoneNo] + " " + StrOffPhoneNo + "]");
                            }
                        }

                        if (CheckIfColumnExists(mydt, xlcol_Ext))
                        {
                            if (row[xlcol_Ext] == null || row[xlcol_Ext].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["Ext"] != DBNull.Value)
                                {
                                    StrExt = Convert.ToString(drowEmpOld["Ext"]);
                                }
                            }
                            else
                            {

                                StrExt = row[xlcol_Ext].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_Ext] + " " + StrExt + "]");
                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_ResPhoneNo))
                        {
                            if (row[xlcol_ResPhoneNo] == null || row[xlcol_ResPhoneNo].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                    if (CheckIfMandatory(xlcol_ResPhoneNo, enmXlImportTables.Employee))
                                    {
                                        AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_ResPhoneNo] + " cannot be blank");
                                        bskipInsertUpdate = true;
                                    }
                                }
                                else if (drowEmpOld != null && drowEmpOld["ResPhoneNo"] != DBNull.Value)
                                {
                                    StrResPhoneNo = Convert.ToString(drowEmpOld["ResPhoneNo"]);
                                }
                            }
                            else
                            {

                                StrResPhoneNo = row[xlcol_ResPhoneNo].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_ResPhoneNo] + " " + StrResPhoneNo + "]");
                            }
                        }

                        if (CheckIfColumnExists(mydt, xlcol_POBox))
                        {
                            if (row[xlcol_POBox] == null || row[xlcol_POBox].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                    if (CheckIfMandatory(xlcol_POBox, enmXlImportTables.Employee))
                                    {
                                        AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_POBox] + " cannot be blank");
                                        bskipInsertUpdate = true;
                                    }
                                }
                                else if (drowEmpOld != null && drowEmpOld["POBox"] != DBNull.Value)
                                {
                                    StrPOBox = Convert.ToString(drowEmpOld["POBox"]);
                                }


                            }
                            else
                            {

                                StrPOBox = row[xlcol_POBox].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_POBox] + " " + StrPOBox + "]");
                            }
                        }

                        if (CheckIfColumnExists(mydt, xlcol_MobileNo))
                        {
                            if (row[xlcol_MobileNo] == null || row[xlcol_MobileNo].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                    if (CheckIfMandatory(xlcol_MobileNo, enmXlImportTables.Employee))
                                    {
                                        AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_MobileNo] + " cannot be blank");
                                        bskipInsertUpdate = true;
                                    }
                                }
                                else if (drowEmpOld != null && drowEmpOld["MobileNo"] != DBNull.Value)
                                {
                                    StrMobileNo = Convert.ToString(drowEmpOld["MobileNo"]);
                                }
                            }
                            else
                            {

                                StrMobileNo = row[xlcol_MobileNo].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_MobileNo] + " " + StrMobileNo + "]");
                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_PagerNo))
                        {
                            if (row[xlcol_PagerNo] == null || row[xlcol_PagerNo].ToString() == string.Empty)
                            {
                                //Empty check
                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["PagerNo"] != DBNull.Value)
                                {
                                    StrPagerNo = Convert.ToString(drowEmpOld["PagerNo"]);
                                }
                            }
                            else
                            {

                                StrPagerNo = row[xlcol_PagerNo].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_PagerNo] + " " + StrPagerNo + "]");
                            }
                        }

                        if (CheckIfColumnExists(mydt, xlcol_TeleNoAbroad))
                        {
                            if (row[xlcol_TeleNoAbroad] == null || row[xlcol_TeleNoAbroad].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                    if (CheckIfMandatory(xlcol_TeleNoAbroad, enmXlImportTables.Employee))
                                    {
                                        AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_TeleNoAbroad] + " cannot be blank");
                                        bskipInsertUpdate = true;
                                    }
                                }
                                else if (drowEmpOld != null && drowEmpOld["TeleNoAbroad"] != DBNull.Value)
                                {
                                    StrTeleNoAbroad = Convert.ToString(drowEmpOld["TeleNoAbroad"]);
                                }
                            }
                            else
                            {

                                StrTeleNoAbroad = row[xlcol_TeleNoAbroad].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_TeleNoAbroad] + " " + StrTeleNoAbroad + "]");
                            }
                        }

                        if (CheckIfColumnExists(mydt, xlcol_Email))
                        {
                            if (row[xlcol_Email] == null || row[xlcol_Email].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                    if (CheckIfMandatory(xlcol_Email, enmXlImportTables.Employee))
                                    {
                                        AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_Email] + " cannot be blank");
                                        bskipInsertUpdate = true;
                                    }
                                }
                                else if (drowEmpOld != null && drowEmpOld["Email"] != DBNull.Value)
                                {
                                    StrEmail = Convert.ToString(drowEmpOld["Email"]);
                                }
                            }
                            else
                            {
                                StrEmail = row[xlcol_Email].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_Email] + " " + StrEmail + "]");
                            }
                        }

                        if (CheckIfColumnExists(mydt, xlcol_BloodGroup))
                        {
                            if (row[xlcol_BloodGroup] == null || row[xlcol_BloodGroup].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["BloodGroup"] != DBNull.Value)
                                {
                                    BloodGroupCode = Convert.ToString(drowEmpOld["BloodGroup"]);
                                }
                            }
                            else
                            {

                                StrBloodGroup = row[xlcol_BloodGroup].ToString();

                                if (!ValidateField(xlcol_BloodGroup, StrBloodGroup, isUpdate, ref fieldErr, ref lookupCodeObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {
                                    BloodGroupCode = Convert.ToString(lookupCodeObj);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_BloodGroup] + " " + StrBloodGroup + "]");
                                }
                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_FaxNo))
                        {
                            if (row[xlcol_FaxNo] == null || row[xlcol_FaxNo].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["FaxNo"] != DBNull.Value)
                                {
                                    StrFaxNo = Convert.ToString(drowEmpOld["FaxNo"]);
                                }
                            }
                            else
                            {
                                StrFaxNo = row[xlcol_FaxNo].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_FaxNo] + " " + StrFaxNo + "]");
                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_NextofKinE))
                        {
                            if (row[xlcol_NextofKinE] == null || row[xlcol_NextofKinE].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["NextofKinE"] != DBNull.Value)
                                {
                                    StrNextofKinE = Convert.ToString(drowEmpOld["NextofKinE"]);
                                }
                            }
                            else
                            {
                                StrNextofKinE = row[xlcol_NextofKinE].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_NextofKinE] + " " + StrNextofKinE + "]");
                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_NextofKinA))
                        {
                            if (row[xlcol_NextofKinA] == null || row[xlcol_NextofKinA].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["NextofKinA"] != DBNull.Value)
                                {
                                    StrNextofKinA = Convert.ToString(drowEmpOld["NextofKinA"]);
                                }
                            }
                            else
                            {
                                StrNextofKinA = row[xlcol_NextofKinA].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_NextofKinA] + " " + StrNextofKinA + "]");
                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_NextofKinAddrE))
                        {
                            if (row[xlcol_NextofKinAddrE] == null || row[xlcol_NextofKinAddrE].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["NextofKinAddrE"] != DBNull.Value)
                                {
                                    StrNextofKinAddrE = Convert.ToString(drowEmpOld["NextofKinAddrE"]);
                                }
                            }
                            else
                            {
                                StrNextofKinAddrE = row[xlcol_NextofKinAddrE].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_NextofKinAddrE] + " " + StrNextofKinAddrE + "]");
                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_NextofKinAddrA))
                        {
                            if (row[xlcol_NextofKinAddrA] == null || row[xlcol_NextofKinAddrA].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["NextofKinAddrA"] != DBNull.Value)
                                {
                                    StrNextofKinAddrA = Convert.ToString(drowEmpOld["NextofKinAddrA"]);
                                }
                            }
                            else
                            {
                                StrNextofKinAddrA = row[xlcol_NextofKinAddrA].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_NextofKinAddrA] + " " + StrNextofKinAddrA + "]");
                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_AddressE))
                        {
                            if (row[xlcol_AddressE] == null || row[xlcol_AddressE].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                    if (CheckIfMandatory(xlcol_AddressE, enmXlImportTables.Employee))
                                    {
                                        AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_AddressE] + " cannot be blank");
                                        bskipInsertUpdate = true;
                                    }
                                }
                                else if (drowEmpOld != null && drowEmpOld["AddressE"] != DBNull.Value)
                                {
                                    StrAddressE = Convert.ToString(drowEmpOld["AddressE"]);
                                }
                            }
                            else
                            {
                                StrAddressE = row[xlcol_AddressE].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_AddressE] + " " + StrAddressE + "]");
                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_AddressA))
                        {
                            if (row[xlcol_AddressA] == null || row[xlcol_AddressA].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["AddressA"] != DBNull.Value)
                                {
                                    StrAddressA = Convert.ToString(drowEmpOld["AddressA"]);
                                }
                            }
                            else
                            {
                                StrAddressA = row[xlcol_AddressA].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_AddressA] + " " + StrAddressA + "]");
                            }

                        }

            
                        if (CheckIfColumnExists(mydt, xlcol_AuxString1))
                        {
                            if (row[xlcol_AuxString1] == null || row[xlcol_AuxString1].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["AuxString1"] != DBNull.Value)
                                {
                                    StrAuxString1 = Convert.ToString(drowEmpOld["AuxString1"]);
                                }
                            }
                            else
                            {
                                StrAuxString1 = row[xlcol_AuxString1].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_AuxString1] + " " + StrAuxString1 + "]");
                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_AuxString2))
                        {
                            if (row[xlcol_AuxString2] == null || row[xlcol_AuxString2].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["AuxString2"] != DBNull.Value)
                                {
                                    StrAuxString2 = Convert.ToString(drowEmpOld["AuxString2"]);
                                }
                            }
                            else
                            {
                                StrAuxString2 = row[xlcol_AuxString2].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_AuxString2] + " " + StrAuxString2 + "]");
                            }

                        }

                       

                   

                        if (CheckIfColumnExists(mydt, xlcol_AuxString3))
                        {
                            if (row[xlcol_AuxString3] == null || row[xlcol_AuxString3].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["AuxString3"] != DBNull.Value)
                                {
                                    StrAuxString3 = Convert.ToString(drowEmpOld["AuxString3"]);
                                }
                            }
                            else
                            {
                                StrAuxString3 = row[xlcol_AuxString3].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_AuxString3] + " " + StrAuxString3 + "]");
                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_AuxString4))
                        {
                            if (row[xlcol_AuxString4] == null || row[xlcol_AuxString4].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["AuxString4"] != DBNull.Value)
                                {
                                    StrAuxString4 = Convert.ToString(drowEmpOld["AuxString4"]);
                                }
                            }
                            else
                            {
                                StrAuxString4 = row[xlcol_AuxString4].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_AuxString4] + " " + StrAuxString4 + "]");
                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_AuxString5))
                        {
                            if (row[xlcol_AuxString5] == null || row[xlcol_AuxString5].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["AuxString5"] != DBNull.Value)
                                {
                                    StrAuxString5 = Convert.ToString(drowEmpOld["AuxString5"]);
                                }
                            }
                            else
                            {
                                StrAuxString5 = row[xlcol_AuxString5].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_AuxString5] + " " + StrAuxString5 + "]");
                            }
                        }

                        if (CheckIfColumnExists(mydt, xlcol_AuxString6))
                        {
                            if (row[xlcol_AuxString6] == null || row[xlcol_AuxString6].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["AuxString6"] != DBNull.Value)
                                {
                                    StrAuxString6 = Convert.ToString(drowEmpOld["AuxString6"]);
                                }
                            }
                            else
                            {

                                StrAuxString6 = row[xlcol_AuxString6].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_AuxString6] + " " + StrAuxString6 + "]");

                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_AuxString7))
                        {
                            if (row[xlcol_AuxString7] == null || row[xlcol_AuxString7].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["AuxString7"] != DBNull.Value)
                                {
                                    StrAuxString7 = Convert.ToString(drowEmpOld["AuxString7"]);
                                }
                            }
                            else
                            {
                                StrAuxString7 = row[xlcol_AuxString7].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_AuxString7] + " " + StrAuxString7 + "]");
                            }
                        }

                        if (CheckIfColumnExists(mydt, xlcol_AuxString8))
                        {
                            if (row[xlcol_AuxString8] == null || row[xlcol_AuxString8].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["AuxString8"] != DBNull.Value)
                                {
                                    StrAuxString8 = Convert.ToString(drowEmpOld["AuxString8"]);
                                }
                            }
                            else
                            {
                                StrAuxString8 = row[xlcol_AuxString8].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_AuxString8] + " " + StrAuxString8 + "]");
                            }
                        }

                        if (CheckIfColumnExists(mydt, xlcol_AuxString9))
                        {
                            if (row[xlcol_AuxString9] == null || row[xlcol_AuxString9].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["AuxString9"] != DBNull.Value)
                                {
                                    StrAuxString9 = Convert.ToString(drowEmpOld["AuxString9"]);
                                }
                            }
                            else
                            {
                                StrAuxString9 = row[xlcol_AuxString9].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_AuxString9] + " " + StrAuxString9 + "]");
                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_AuxString10))
                        {
                            if (row[xlcol_AuxString10] == null || row[xlcol_AuxString10].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["AuxString10"] != DBNull.Value)
                                {
                                    StrAuxString10 = Convert.ToString(drowEmpOld["AuxString10"]);
                                }
                            }
                            else
                            {
                                StrAuxString10 = row[xlcol_AuxString10].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_AuxString10] + " " + StrAuxString10 + "]");
                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_AuxAString1))
                        {
                            if (row[xlcol_AuxAString1] == null || row[xlcol_AuxAString1].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["AuxAString1"] != DBNull.Value)
                                {
                                    StrAuxAString1 = Convert.ToString(drowEmpOld["AuxAString1"]);
                                }
                            }
                            else
                            {
                                StrAuxAString1 = row[xlcol_AuxAString1].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_AuxAString1] + " " + StrAuxAString1 + "]");
                            }
                        }

                        if (CheckIfColumnExists(mydt, xlcol_AuxAString2))
                        {
                            if (row[xlcol_AuxAString2] == null || row[xlcol_AuxAString2].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["AuxAString2"] != DBNull.Value)
                                {
                                    StrAuxAString2 = Convert.ToString(drowEmpOld["AuxAString2"]);
                                }
                            }
                            else
                            {
                                StrAuxAString2 = row[xlcol_AuxAString2].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_AuxAString2] + " " + StrAuxAString2 + "]");
                            }
                        }

                        if (CheckIfColumnExists(mydt, xlcol_AuxAString3))
                        {
                            if (row[xlcol_AuxAString3] == null || row[xlcol_AuxAString3].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["AuxAString3"] != DBNull.Value)
                                {
                                    StrAuxAString3 = Convert.ToString(drowEmpOld["AuxAString3"]);
                                }
                            }
                            else
                            {
                                StrAuxAString3 = row[xlcol_AuxAString3].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_AuxAString3] + " " + StrAuxAString3 + "]");
                            }
                        }

                        if (CheckIfColumnExists(mydt, xlcol_AuxAString4))
                        {
                            if (row[xlcol_AuxAString4] == null || row[xlcol_AuxAString4].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["AuxAString4"] != DBNull.Value)
                                {
                                    StrAuxAString4 = Convert.ToString(drowEmpOld["AuxAString4"]);
                                }
                            }
                            else
                            {
                                StrAuxAString4 = row[xlcol_AuxAString4].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_AuxAString4] + " " + StrAuxAString4 + "]");
                            }
                        }

                        if (CheckIfColumnExists(mydt, xlcol_AuxAString5))
                        {
                            if (row[xlcol_AuxAString5] == null || row[xlcol_AuxAString5].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["AuxAString5"] != DBNull.Value)
                                {
                                    StrAuxAString5 = Convert.ToString(drowEmpOld["AuxAString5"]);
                                }
                            }
                            else
                            {
                                StrAuxAString5 = row[xlcol_AuxAString5].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_AuxAString5] + " " + StrAuxAString5 + "]");
                            }
                        }
                        if (CheckIfColumnExists(mydt, xlcol_AuxAString6))
                        {
                            if (row[xlcol_AuxAString6] == null || row[xlcol_AuxAString6].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["AuxAString6"] != DBNull.Value)
                                {
                                    StrAuxAString6 = Convert.ToString(drowEmpOld["AuxAString6"]);
                                }
                            }
                            else
                            {
                                StrAuxAString6 = row[xlcol_AuxAString6].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_AuxAString6] + " " + StrAuxAString6 + "]");
                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_AuxAString7))
                        {
                            if (row[xlcol_AuxAString7] == null || row[xlcol_AuxAString7].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["AuxAString7"] != DBNull.Value)
                                {
                                    StrAuxAString7 = Convert.ToString(drowEmpOld["AuxAString7"]);
                                }
                            }
                            else
                            {
                                StrAuxAString7 = row[xlcol_AuxAString7].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_AuxAString7] + " " + StrAuxAString7 + "]");
                            }
                        }

                        if (CheckIfColumnExists(mydt, xlcol_AuxAString8))
                        {
                            if (row[xlcol_AuxAString8] == null || row[xlcol_AuxAString8].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["AuxAString8"] != DBNull.Value)
                                {
                                    StrAuxAString8 = Convert.ToString(drowEmpOld["AuxAString8"]);
                                }
                            }
                            else
                            {
                                StrAuxAString8 = row[xlcol_AuxAString8].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_AuxAString8] + " " + StrAuxAString8 + "]");
                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_AuxAString9))
                        {
                            if (row[xlcol_AuxAString9] == null || row[xlcol_AuxAString9].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["AuxAString9"] != DBNull.Value)
                                {
                                    StrAuxAString9 = Convert.ToString(drowEmpOld["AuxAString9"]);
                                }
                            }
                            else
                            {
                                StrAuxAString9 = row[xlcol_AuxAString9].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_AuxAString9] + " " + StrAuxAString9 + "]");
                            }

                        }

                        if (CheckIfColumnExists(mydt, xlcol_AuxAString10))
                        {
                            if (row[xlcol_AuxAString10] == null || row[xlcol_AuxAString10].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["AuxAString10"] != DBNull.Value)
                                {
                                    StrAuxAString10 = Convert.ToString(drowEmpOld["AuxAString10"]);
                                }
                            }
                            else
                            {
                                StrAuxAString10 = row[xlcol_AuxAString10].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_AuxAString10] + " " + StrAuxAString10 + "]");
                            }
                        }

                        if (CheckIfColumnExists(mydt, xlcol_AuxInt1))
                        {
                            if (row[xlcol_AuxInt1] == null || row[xlcol_AuxInt1].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["AuxInt1"] != DBNull.Value)
                                {
                                    nAuxInt1 = Convert.ToInt32(drowEmpOld["AuxInt1"]);
                                }
                            }
                            else
                            {
                                StrAuxInt1 = row[xlcol_AuxInt1].ToString();

                                //if (!ValidateField(xlcol_AuxInt1, StrAuxInt1, isUpdate, ref fieldErr, ref emptyObj))
                                //{
                                    //AppendLineError(nRowNo, rowErrInfo, fieldErr);

                               // }
                                //else
                                //{
                                    nAuxInt1 = Convert.ToInt32(StrAuxInt1);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_AuxInt1] + " " + StrAuxInt1 + "]");
                               // }

                            }
                        }
                        if (CheckIfColumnExists(mydt, xlcol_AuxInt2))
                        {
                            if (row[xlcol_AuxInt2] == null || row[xlcol_AuxInt2].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["AuxInt2"] != DBNull.Value)
                                {
                                    nAuxInt2 = Convert.ToInt32(drowEmpOld["AuxInt2"]);
                                }
                            }
                            else
                            {
                                StrAuxInt2 = row[xlcol_AuxInt2].ToString();

                                //if (!ValidateField(xlcol_AuxInt2, StrAuxInt2, isUpdate, ref fieldErr, ref emptyObj))
                                //{
                                    //AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                //}
                                //else
                                //{
                                    nAuxInt2 = Convert.ToInt32(StrAuxInt2);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_AuxInt2] + " " + StrAuxInt2 + "]");
                                //}

                            }
                        }
                        if (CheckIfColumnExists(mydt, xlcol_AuxInt3))
                        {
                            if (row[xlcol_AuxInt3] == null || row[xlcol_AuxInt3].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["AuxInt3"] != DBNull.Value)
                                {
                                    nAuxInt3 = Convert.ToInt32(drowEmpOld["AuxInt3"]);
                                }
                            }
                            else
                            {
                                StrAuxInt3 = row[xlcol_AuxInt3].ToString();

                                if (!ValidateField(xlcol_AuxInt3, StrAuxInt3, isUpdate, ref fieldErr, ref emptyObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {
                                    nAuxInt3 = Convert.ToInt32(StrAuxInt3);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_AuxInt3] + " " + StrAuxInt3 + "]");
                                }

                            }
                        }
                        if (CheckIfColumnExists(mydt, xlcol_AuxCurrency1))
                        {
                            if (row[xlcol_AuxCurrency1] == null || row[xlcol_AuxCurrency1].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["AuxCurrency1"] != DBNull.Value)
                                {
                                    nAuxCurrency1 = Convert.ToDouble(drowEmpOld["AuxCurrency1"]);
                                }
                            }
                            else
                            {
                                StrAuxCurrency1 = row[xlcol_AuxCurrency1].ToString();

                                if (!ValidateField(xlcol_AuxCurrency1, StrAuxCurrency1, isUpdate, ref fieldErr, ref emptyObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {
                                    nAuxCurrency1 = Convert.ToDouble(StrAuxCurrency1);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_AuxCurrency1] + " " + StrAuxCurrency1 + "]");
                                }

                            }
                        }
                        if (CheckIfColumnExists(mydt, xlcol_AuxCurrency2))
                        {
                            if (row[xlcol_AuxCurrency2] == null || row[xlcol_AuxCurrency2].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["AuxCurrency2"] != DBNull.Value)
                                {
                                    nAuxCurrency2 = Convert.ToDouble(drowEmpOld["AuxCurrency2"]);
                                }
                            }
                            else
                            {
                                StrAuxCurrency2 = row[xlcol_AuxCurrency2].ToString();

                                if (!ValidateField(xlcol_AuxCurrency2, StrAuxCurrency2, isUpdate, ref fieldErr, ref emptyObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {
                                    nAuxCurrency2 = Convert.ToDouble(StrAuxCurrency2);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_AuxCurrency2] + " " + StrAuxCurrency2 + "]");
                                }

                            }
                        }

                        if (CheckIfColumnExists(mydt, xlcol_AuxDate1))
                        {
                            if (row[xlcol_AuxDate1] == null || row[xlcol_AuxDate1].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["AuxDate1"] != DBNull.Value)
                                {
                                    AuxDate1Value = GetValidDateTime(drowEmpOld["AuxDate1"].ToString());
                                }
                            }
                            else
                            {
                                StrAuxDate1 = row[xlcol_AuxDate1].ToString();

                                //if (!ValidateField(xlcol_AuxDate1, StrAuxDate1, isUpdate, ref fieldErr, ref emptyObj))
                                //{
                                //    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                //}
                                //else
                                //{


                                    AuxDate1Value = GetValidDateTime(StrAuxDate1);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_AuxDate1] + " " + AuxDate1Value.ToString("dd/MM/yyyy") + "]");

                                //}
                            }
                        }
                        if (CheckIfColumnExists(mydt, xlcol_AuxDate2))
                        {
                            if (row[xlcol_AuxDate2] == null || row[xlcol_AuxDate2].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["AuxDate2"] != DBNull.Value)
                                {
                                    AuxDate2Value = GetValidDateTime(drowEmpOld["AuxDate2"].ToString());
                                }
                            }
                            else
                            {
                                StrAuxDate2 = row[xlcol_AuxDate2].ToString();

                                //if (!ValidateField(xlcol_AuxDate2, StrAuxDate2, isUpdate, ref fieldErr, ref emptyObj))
                                //{
                                //    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                //}
                                //else
                                //{


                                    AuxDate2Value = GetValidDateTime(StrAuxDate2);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_AuxDate2] + " " + AuxDate2Value.ToString("dd/MM/yyyy") + "]");

                               // }
                            }
                        }
                        if (CheckIfColumnExists(mydt, xlcol_AuxDate3))
                        {
                            if (row[xlcol_AuxDate3] == null || row[xlcol_AuxDate3].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["AuxDate3"] != DBNull.Value)
                                {
                                    AuxDate3Value = GetValidDateTime(drowEmpOld["AuxDate3"].ToString());
                                }
                            }
                            else
                            {
                                StrAuxDate3 = row[xlcol_AuxDate3].ToString();

                                if (!ValidateField(xlcol_AuxDate3, StrAuxDate3, isUpdate, ref fieldErr, ref emptyObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {


                                    AuxDate3Value = GetValidDateTime(StrAuxDate3);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_AuxDate3] + " " + AuxDate3Value.ToString("dd/MM/yyyy") + "]");

                                }
                            }
                        }
                        if (CheckIfColumnExists(mydt, xlcol_AuxDate4))
                        {
                            if (row[xlcol_AuxDate4] == null || row[xlcol_AuxDate4].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["AuxDate4"] != DBNull.Value)
                                {
                                    AuxDate4Value = GetValidDateTime(drowEmpOld["AuxDate4"].ToString());
                                }
                            }
                            else
                            {
                                StrAuxDate4 = row[xlcol_AuxDate4].ToString();

                                if (!ValidateField(xlcol_AuxDate4, StrAuxDate4, isUpdate, ref fieldErr, ref emptyObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {


                                    AuxDate4Value = GetValidDateTime(StrAuxDate4);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_AuxDate4] + " " + AuxDate4Value.ToString("dd/MM/yyyy") + "]");

                                }
                            }
                        }
                        if (CheckIfColumnExists(mydt, xlcol_AuxDate5))
                        {
                            if (row[xlcol_AuxDate5] == null || row[xlcol_AuxDate5].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["AuxDate5"] != DBNull.Value)
                                {
                                    AuxDate5Value = GetValidDateTime(drowEmpOld["AuxDate5"].ToString());
                                }
                            }
                            else
                            {
                                StrAuxDate5 = row[xlcol_AuxDate5].ToString();

                                if (!ValidateField(xlcol_AuxDate5, StrAuxDate5, isUpdate, ref fieldErr, ref emptyObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {

                                    AuxDate5Value = GetValidDateTime(StrAuxDate5);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_AuxDate5] + " " + AuxDate5Value.ToString("dd/MM/yyyy") + "]");

                                }
                            }
                        }


                        if (CheckIfColumnExists(mydt, xlcol_AuxLib1))
                        {
                            if (row[xlcol_AuxLib1] == null || row[xlcol_AuxLib1].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["AuxLib1"] != DBNull.Value)
                                {
                                    AuxLib1Code = Convert.ToString(drowEmpOld["AuxLib1"]);
                                }
                            }
                            else
                            {

                                StrAuxLib1 = row[xlcol_AuxLib1].ToString();

                                if (!ValidateField(xlcol_AuxLib1, StrAuxLib1, isUpdate, ref fieldErr, ref lookupCodeObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {
                                    AuxLib1Code = Convert.ToString(lookupCodeObj);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_AuxLib1] + " " + StrAuxLib1 + "]");
                                }
                            }
                        }
                        if (CheckIfColumnExists(mydt, xlcol_AuxLib2))
                        {
                            if (row[xlcol_AuxLib2] == null || row[xlcol_AuxLib2].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["AuxLib2"] != DBNull.Value)
                                {
                                    AuxLib2Code = Convert.ToString(drowEmpOld["AuxLib2"]);
                                }
                            }
                            else
                            {

                                StrAuxLib2 = row[xlcol_AuxLib2].ToString();

                                //if (!ValidateField(xlcol_AuxLib2, StrAuxLib2, isUpdate, ref fieldErr, ref lookupCodeObj))
                                //{
                                //    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                //}
                                //else
                                //{
                                    AuxLib2Code = Convert.ToString(lookupCodeObj);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_AuxLib2] + " " + StrAuxLib2 + "]");
                                //}
                            }
                        }
                        if (CheckIfColumnExists(mydt, xlcol_AuxLib3))
                        {
                            if (row[xlcol_AuxLib3] == null || row[xlcol_AuxLib3].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["AuxLib3"] != DBNull.Value)
                                {
                                    AuxLib3Code = Convert.ToString(drowEmpOld["AuxLib3"]);
                                }
                            }
                            else
                            {

                                StrAuxLib3 = row[xlcol_AuxLib3].ToString();

                                //if (!ValidateField(xlcol_AuxLib3, StrAuxLib3, isUpdate, ref fieldErr, ref lookupCodeObj))
                                //{
                                //    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                //}
                                //else
                                //{
                                    AuxLib3Code = Convert.ToString(lookupCodeObj);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_AuxLib3] + " " + StrAuxLib3 + "]");
                                //}
                            }
                        }
                        if (CheckIfColumnExists(mydt, xlcol_AuxLib4))
                        {
                            if (row[xlcol_AuxLib4] == null || row[xlcol_AuxLib4].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["AuxLib4"] != DBNull.Value)
                                {
                                    AuxLib4Code = Convert.ToString(drowEmpOld["AuxLib4"]);
                                }
                            }
                            else
                            {

                                StrAuxLib4 = row[xlcol_AuxLib4].ToString();

                                if (!ValidateField(xlcol_AuxLib4, StrAuxLib4, isUpdate, ref fieldErr, ref lookupCodeObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {
                                    AuxLib4Code = Convert.ToString(lookupCodeObj);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_AuxLib4] + " " + StrAuxLib4 + "]");
                                }
                            }
                        }
                        if (CheckIfColumnExists(mydt, xlcol_AuxLib5))
                        {
                            if (row[xlcol_AuxLib5] == null || row[xlcol_AuxLib5].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["AuxLib5"] != DBNull.Value)
                                {
                                    AuxLib5Code = Convert.ToString(drowEmpOld["AuxLib5"]);
                                }
                            }
                            else
                            {

                                StrAuxLib5 = row[xlcol_AuxLib5].ToString();

                                if (!ValidateField(xlcol_AuxLib5, StrAuxLib5, isUpdate, ref fieldErr, ref lookupCodeObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {
                                    AuxLib5Code = Convert.ToString(lookupCodeObj);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_AuxLib5] + " " + StrAuxLib5 + "]");
                                }
                            }
                        }
                        if (CheckIfColumnExists(mydt, xlcol_AuxLib6))
                        {
                            if (row[xlcol_AuxLib6] == null || row[xlcol_AuxLib6].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["AuxLib6"] != DBNull.Value)
                                {
                                    AuxLib6Code = Convert.ToString(drowEmpOld["AuxLib6"]);
                                }
                            }
                            else
                            {

                                StrAuxLib6 = row[xlcol_AuxLib6].ToString();

                                if (!ValidateField(xlcol_AuxLib6, StrAuxLib6, isUpdate, ref fieldErr, ref lookupCodeObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {
                                    AuxLib6Code = Convert.ToString(lookupCodeObj);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_AuxLib6] + " " + StrAuxLib6 + "]");
                                }
                            }
                        }

                        if (CheckIfColumnExists(mydt, xlcol_IntlJoiningDate))
                        {
                            if (row[xlcol_IntlJoiningDate] == null || row[xlcol_IntlJoiningDate].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                    if (CheckIfMandatory(xlcol_IntlJoiningDate, enmXlImportTables.Employee))
                                    {
                                        AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_IntlJoiningDate] + " cannot be blank");
                                        bskipInsertUpdate = true;
                                    }
                                }
                                else if (drowEmpOld != null && drowEmpOld["IntlJoiningDate"] != DBNull.Value)
                                {
                                    IntlJoiningDateValue = GetValidDateTime(drowEmpOld["IntlJoiningDate"].ToString());
                                }
                            }
                            else
                            {
                                StrIntlJoiningDate = row[xlcol_IntlJoiningDate].ToString();

                                if (!ValidateField(xlcol_IntlJoiningDate, StrIntlJoiningDate, isUpdate, ref fieldErr, ref emptyObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {

                                    IntlJoiningDateValue = GetValidDateTime(StrIntlJoiningDate);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_IntlJoiningDate] + " " + IntlJoiningDateValue.ToString("dd/MM/yyyy") + "]");

                                }
                            }
                        }

                        if (CheckIfColumnExists(mydt, xlcol_PointOfHireE))
                        {
                            if (row[xlcol_PointOfHireE] == null || row[xlcol_PointOfHireE].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["PointOfHireE"] != DBNull.Value)
                                {
                                    PointOfHireECode = Convert.ToString(drowEmpOld["PointOfHireE"]);
                                }
                            }
                            else
                            {

                                StrPointOfHireE = row[xlcol_PointOfHireE].ToString();

                                if (!ValidateField(xlcol_PointOfHireE, StrPointOfHireE, isUpdate, ref fieldErr, ref lookupCodeObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {
                                    PointOfHireECode = Convert.ToString(lookupCodeObj);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_PointOfHireE] + " " + StrPointOfHireE + "]");
                                }
                            }
                        }
                        if (CheckIfColumnExists(mydt, xlcol_PointOfHireA))
                        {
                            if (row[xlcol_PointOfHireA] == null || row[xlcol_PointOfHireA].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["PointOfHireA"] != DBNull.Value)
                                {
                                    StrPointOfHireA = Convert.ToString(drowEmpOld["PointOfHireA"]);
                                }
                            }
                            else
                            {
                                StrPointOfHireA = row[xlcol_PointOfHireA].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_PointOfHireA] + " " + StrPointOfHireA + "]");
                            }
                        }
                        if (CheckIfColumnExists(mydt, xlcol_SourceOfHire))
                        {
                            if (row[xlcol_SourceOfHire] == null || row[xlcol_SourceOfHire].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["SourceOfHire"] != DBNull.Value)
                                {
                                    SourceOfHireCode = Convert.ToString(drowEmpOld["SourceOfHire"]);
                                }
                            }
                            else
                            {

                                StrSourceOfHire = row[xlcol_SourceOfHire].ToString();

                                if (!ValidateField(xlcol_SourceOfHire, StrSourceOfHire, isUpdate, ref fieldErr, ref lookupCodeObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {
                                    SourceOfHireCode = Convert.ToString(lookupCodeObj);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_SourceOfHire] + " " + StrSourceOfHire + "]");
                                }
                            }
                        }

                        if (CheckIfColumnExists(mydt, xlcol_SLReInitDate))
                        {
                            if (row[xlcol_SLReInitDate] == null || row[xlcol_SLReInitDate].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["SLReInitDate"] != DBNull.Value)
                                {
                                    SLReInitDateValue = GetValidDateTime(drowEmpOld["SLReInitDate"].ToString());
                                }
                            }
                            else
                            {


                                StrSLReInitDate = row[xlcol_SLReInitDate].ToString();

                                if (!ValidateField(xlcol_SLReInitDate, StrSLReInitDate, isUpdate, ref fieldErr, ref emptyObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {


                                    SLReInitDateValue = GetValidDateTime(StrSLReInitDate);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_SLReInitDate] + " " + SLReInitDateValue.ToString("dd/MM/yyyy") + "]");

                                }
                            }
                        }


                        if (CheckIfColumnExists(mydt, xlcol_StartDtofIndemnity))
                        {
                            if (row[xlcol_StartDtofIndemnity] == null || row[xlcol_StartDtofIndemnity].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                    if (CheckIfMandatory(xlcol_StartDtofIndemnity, enmXlImportTables.Employee))
                                    {
                                        AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_StartDtofIndemnity] + " cannot be blank");
                                        bskipInsertUpdate = true;
                                    }
                                }
                                else if (drowEmpOld != null && drowEmpOld["StartDtofIndemnity"] != DBNull.Value)
                                {
                                    StartDtofIndemnityValue = GetValidDateTime(drowEmpOld["StartDtofIndemnity"].ToString());
                                }
                            }
                            else
                            {
                                //if financial is approved then StartDtofIndemnity will not be Updated in Employee table - as per Aziz 26-02-2025
                                if (drowFinMastOld != null && drowFinMastOld["Status"] != DBNull.Value && Convert.ToInt32(drowFinMastOld["Status"]) >= 20)
                                {

                                    if (drowEmpOld != null && drowEmpOld["StartDtofIndemnity"] != DBNull.Value)
                                    {
                                        StartDtofIndemnityValue = GetValidDateTime(drowEmpOld["StartDtofIndemnity"].ToString());
                                        StrStartDtofIndemnity = StartDtofIndemnityValue.ToString("dd/MM/yyyy");
                                    }
                                    
                                }
                                else
                                {

                                    StrStartDtofIndemnity = row[xlcol_StartDtofIndemnity].ToString();

                                    if (!ValidateField(xlcol_StartDtofIndemnity, StrStartDtofIndemnity, isUpdate, ref fieldErr, ref emptyObj))
                                    {
                                        AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                    }
                                    else
                                    {

                                        StartDtofIndemnityValue = GetValidDateTime(StrStartDtofIndemnity);

                                        strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_StartDtofIndemnity] + " " + StartDtofIndemnityValue.ToString("dd/MM/yyyy") + "]");

                                    }
                                }
                            }
                        }

                        if (CheckIfColumnExists(mydt, xlcol_HealthInsurCmp))
                        {
                            if (row[xlcol_HealthInsurCmp] == null || row[xlcol_HealthInsurCmp].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["HealthInsurCmp"] != DBNull.Value)
                                {
                                    HealthInsurCmpCode = Convert.ToString(drowEmpOld["HealthInsurCmp"]);
                                }
                            }
                            else
                            {

                                StrHealthInsurCmp = row[xlcol_HealthInsurCmp].ToString();

                                if (!ValidateField(xlcol_HealthInsurCmp, StrHealthInsurCmp, isUpdate, ref fieldErr, ref lookupCodeObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {
                                    HealthInsurCmpCode = Convert.ToString(lookupCodeObj);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_HealthInsurCmp] + " " + StrHealthInsurCmp + "]");
                                }
                            }
                        }
                        if (CheckIfColumnExists(mydt, xlcol_LCIssuePlace))
                        {
                            if (row[xlcol_LCIssuePlace] == null || row[xlcol_LCIssuePlace].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["LCIssuePlace"] != DBNull.Value)
                                {
                                    LCIssuePlaceCode = Convert.ToString(drowEmpOld["LCIssuePlace"]);
                                }
                            }
                            else
                            {

                                StrLCIssuePlace = row[xlcol_LCIssuePlace].ToString();

                                if (!ValidateField(xlcol_LCIssuePlace, StrLCIssuePlace, isUpdate, ref fieldErr, ref lookupCodeObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {
                                    LCIssuePlaceCode = Convert.ToString(lookupCodeObj);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_LCIssuePlace] + " " + StrLCIssuePlace + "]");
                                }
                            }
                        }
                        if (CheckIfColumnExists(mydt, xlcol_SponTypeExtnl))
                        {
                            if (row[xlcol_SponTypeExtnl] == null || row[xlcol_SponTypeExtnl].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["SponTypeExtnl"] != DBNull.Value)
                                {
                                    SponTypeExtnlCode = Convert.ToInt32(drowEmpOld["SponTypeExtnl"]);
                                }
                            }
                            else
                            {

                                StrSponTypeExtnl = row[xlcol_SponTypeExtnl].ToString();

                                if (!ValidateField(xlcol_SponTypeExtnl, StrSponTypeExtnl, isUpdate, ref fieldErr, ref lookupCodeObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {
                                    SponTypeExtnlCode = Convert.ToInt32(lookupCodeObj);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_SponTypeExtnl] + " " + StrSponTypeExtnl + "]");
                                }
                            }
                        }


                        if (CheckIfColumnExists(mydt, xlcol_VisaExpDate))
                        {
                            if (row[xlcol_VisaExpDate] == null || row[xlcol_VisaExpDate].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["VisaExpDate"] != DBNull.Value)
                                {
                                    VisaExpDateValue = GetValidDateTime(drowEmpOld["VisaExpDate"].ToString());
                                }
                            }
                            else
                            {
                                StrVisaExpDate = row[xlcol_VisaExpDate].ToString();

                                if (!ValidateField(xlcol_VisaExpDate, StrVisaExpDate, isUpdate, ref fieldErr, ref emptyObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {


                                    VisaExpDateValue = GetValidDateTime(StrVisaExpDate);


                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_VisaExpDate] + " " + VisaExpDateValue.ToString("dd/MM/yyyy") + "]");

                                }
                            }
                        }

                        if (CheckIfColumnExists(mydt, xlcol_PersEmail))
                        {
                            if (row[xlcol_PersEmail] == null || row[xlcol_PersEmail].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                    if (CheckIfMandatory(xlcol_PersEmail, enmXlImportTables.Employee))
                                    {
                                        AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_PersEmail] + " cannot be blank");
                                        bskipInsertUpdate = true;
                                    }
                                }
                                else if (drowEmpOld != null && drowEmpOld["PersEmail"] != DBNull.Value)
                                {
                                    StrPersEmail = Convert.ToString(drowEmpOld["PersEmail"]);
                                }
                            }
                            else
                            {
                                StrPersEmail = row[xlcol_PersEmail].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_PersEmail] + " " + StrPersEmail + "]");
                            }
                        }


                        if (CheckIfColumnExists(mydt, xlcol_NPresentSec))
                        {
                            if (row[xlcol_NPresentSec] == null || row[xlcol_NPresentSec].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["NPresentSec"] != DBNull.Value)
                                {
                                    NPresentSecCode = Convert.ToString(drowEmpOld["NPresentSec"]);
                                }
                            }
                            else
                            {

                                StrNPresentSec = row[xlcol_NPresentSec].ToString();

                                if (!ValidateField(xlcol_NPresentSec, StrNPresentSec, isUpdate, ref fieldErr, ref lookupCodeObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {
                                    NPresentSecCode = Convert.ToString(lookupCodeObj);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_NPresentSec] + " " + StrNPresentSec + "]");
                                }
                            }
                        }



                        if (CheckIfColumnExists(mydt, xlcol_ReligionSubSet))
                        {
                            if (row[xlcol_ReligionSubSet] == null || row[xlcol_ReligionSubSet].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["ReligionSubSet"] != DBNull.Value)
                                {
                                    ReligionSubSetCode = Convert.ToString(drowEmpOld["ReligionSubSet"]);
                                }
                            }
                            else
                            {

                                StrReligionSubSet = row[xlcol_ReligionSubSet].ToString();

                                //if (!ValidateField(xlcol_ReligionSubSet, StrReligionSubSet, isUpdate, ref fieldErr, ref lookupCodeObj))
                                //{
                                //    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                //}
                                //else
                                //{
                                    ReligionSubSetCode = Convert.ToString(lookupCodeObj);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_ReligionSubSet] + " " + StrReligionSubSet + "]");
                                //}
                            }
                        }



                        if (CheckIfColumnExists(mydt, xlcol_UIDNo))
                        {
                            if (row[xlcol_UIDNo] == null || row[xlcol_UIDNo].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["UIDNo"] != DBNull.Value)
                                {
                                    StrUIDNo = Convert.ToString(drowEmpOld["UIDNo"]);
                                }
                            }
                            else
                            {
                                StrUIDNo = row[xlcol_UIDNo].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_UIDNo] + " " + StrUIDNo + "]");

                            }
                        }
                        if (CheckIfColumnExists(mydt, xlcol_Disability))
                        {
                            if (row[xlcol_Disability] == null || row[xlcol_Disability].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["Disability"] != DBNull.Value)
                                {
                                    DisabilityCode = drowEmpOld["Disability"].ToString();
                                }
                            }
                            else
                            {
                                StrDisability = row[xlcol_Disability].ToString();


                                if (!ValidateField(xlcol_Disability, StrDisability, isUpdate, ref fieldErr, ref lookupCodeObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {
                                    DisabilityCode = Convert.ToString(lookupCodeObj);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_Disability] + " " + StrDisability + "]");
                                }
                            }
                        }
                        if (CheckIfColumnExists(mydt, xlcol_DisabilityDescE))
                        {
                            if (row[xlcol_DisabilityDescE] == null || row[xlcol_DisabilityDescE].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["DisabilityDescE"] != DBNull.Value)
                                {
                                    StrDisabilityDescE = Convert.ToString(drowEmpOld["DisabilityDescE"]);
                                }
                            }
                            else
                            {
                                StrDisabilityDescE = row[xlcol_DisabilityDescE].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_DisabilityDescE] + " " + StrDisabilityDescE + "]");
                            }
                        }
                        if (CheckIfColumnExists(mydt, xlcol_DisabilityDescA))
                        {
                            if (row[xlcol_DisabilityDescA] == null || row[xlcol_DisabilityDescA].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["DisabilityDescA"] != DBNull.Value)
                                {
                                    StrDisabilityDescA = Convert.ToString(drowEmpOld["DisabilityDescA"]);
                                }
                            }
                            else
                            {
                                StrDisabilityDescA = row[xlcol_DisabilityDescA].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_DisabilityDescA] + " " + StrDisabilityDescA + "]");
                            }
                        }

                        if (CheckIfColumnExists(mydt, xlcol_NationalID))
                        {
                            //NationalID validation check -- added by farook 21/NOV/2023
                            if (row[xlcol_NationalID] == null || row[xlcol_NationalID].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["NationalID"] != DBNull.Value)
                                {
                                    StrNationalID = Convert.ToString(drowEmpOld["NationalID"]);
                                }
                            }
                            else
                            {
                                StrNationalID = row[xlcol_NationalID].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_NationalID] + " " + StrNationalID + "]");
                            }
                        }

                        if (!isUpdate)
                        {

                            if (CheckIfColumnExists(mydt, xlcol_LocLib5))
                            {
                                if (row[xlcol_LocLib5] == null || row[xlcol_LocLib5].ToString() == string.Empty)
                                {
                                    //mandatory if new
                                    AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_LocLib5] + " is empty: ");
                                    bskipInsertUpdate = true;

                                }
                                else
                                {
                                    strEmpLocationData = row[xlcol_LocLib5].ToString();

                                    sQuery = "Select [dbo].[fn_DBOXI_GetLoclib5FromDBoxText](@DBoxText)";
                                    List<SqlParameter> Paramslst = new List<SqlParameter>();
                                    Paramslst.Add(new SqlParameter("@DBoxText", strEmpLocationData));
                                    SqlParameter[] Params13 = Paramslst.ToArray();

                                    if (!ConnectionFunctions.Connect_SQLDataReader(ref dr, sQuery, ref errmsg, Params13, CommandType.Text))
                                    {

                                        AppendLineError(nRowNo, rowErrInfo, "Error Occurred while retrieving the Location Details from Database");
                                        bskipInsertUpdate = true;
                                    }
                                    else
                                    {
                                        if (dr.HasRows)
                                        {
                                            dr.Read();
                                            if (!dr.IsDBNull(0))
                                                LocationLevelCode = dr[0].ToString();
                                            
                                            if (!string.IsNullOrEmpty(LocationLevelCode) && LocationLevelCode != "-1")
                                            {
                                                strBuildrAudit.Append(",[ Location " + LocationLevelCode + "]");
                                            }
                                            else
                                            {
                                                AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_LocLib5] + " which you have entered is not found in the Database");
                                                bskipInsertUpdate = true;

                                            }
                                            dr.Close();
                                        }
                                        else
                                        {
                                            AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_LocLib5] + " which you have entered is not found in the Database");
                                            bskipInsertUpdate = true;

                                        }
                                    }
                                }
                            }

                            if (CheckIfColumnExists(mydt, xlcol_SalProfile))
                            {
                                if (row[xlcol_SalProfile] == null || row[xlcol_SalProfile].ToString() == string.Empty)
                                {


                                    if (strEditMode != "EDIT")
                                    {
                                        //mandatory if new
                                        AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_SalProfile] + " is empty: ");
                                        bskipInsertUpdate = true;
                                    }
                                    else if (drowEmpOld != null && drowEmpOld[xlcol_SalProfile] != DBNull.Value)
                                    {
                                        SalaryProfileCode = Convert.ToString(drowEmpOld[xlcol_SalProfile]);
                                    }

                                }
                                else
                                {
                                    //bIsFinMastEntered = true; //will not be considered as finmast part, it will be employee part thats why this line commented
                                    StrSalProfile = row[xlcol_SalProfile].ToString();


                                    if (!ValidateField(xlcol_SalProfile, StrSalProfile, isUpdate, ref fieldErr, ref lookupCodeObj))
                                    {
                                        AppendLineError(nRowNo, rowErrInfo, fieldErr);
                                    }
                                    else
                                    {
                                        SalaryProfileCode = Convert.ToString(lookupCodeObj);

                                        DataTable dtLookUp = new DataTable();
                                        if (!GetLookupTableByCode(enmXlImportTables.Employee, xlcol_SalProfile, StrSalProfile, ref dtLookUp, ref errmsg))
                                        {
                                            AppendLineError(nRowNo, rowErrInfo, errmsg);

                                        }
                                        else
                                        {
                                            StrSalaryProfileText = dtLookUp.Rows[0][1].ToString();

                                            strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_SalProfile] + " " + StrSalaryProfileText + "]");
                                        }


                                    }


                                }
                            }

                        }


                        if (CheckIfColumnExists(mydt, xlcol_WPS))
                        {
                            if (row[xlcol_WPS] == null || row[xlcol_WPS].ToString() == string.Empty)
                            {
                                //Empty check

                                if (strEditMode != "EDIT")
                                {
                                }
                                else if (drowEmpOld != null && drowEmpOld["WPS"] != DBNull.Value)
                                {
                                    WPSCode = Convert.ToString(drowEmpOld["WPS"]);
                                }
                            }
                            else
                            {
                                StrWPS = row[xlcol_WPS].ToString();

                                strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_WPS] + " " + xlcol_WPS + "]");

                                if (!ValidateField(xlcol_WPS, StrWPS, isUpdate, ref fieldErr, ref lookupCodeObj))
                                {
                                    AppendLineError(nRowNo, rowErrInfo, fieldErr);

                                }
                                else
                                {
                                    WPSCode = Convert.ToString(lookupCodeObj);

                                    strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_WPS] + " " + StrWPS + "]");

                                    if(WPSCode=="0")// when wps is 0 i.e "NO" WPS number will be made blank
                                    {
                                        StrImmigFileNo = "";
                                        strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_WPS] + " " + StrImmigFileNo + "]");
                                    }
                                }

                            }
                        }



                        #endregion

                        if (isUpdate)
                        {
                            // if existing finmast record then location and salprofile will be used from existing record
                            if (drowEmpOld != null)
                            {
                                LocationLevelCode = drowEmpOld["loclib5"] == DBNull.Value ? "" : drowEmpOld["loclib5"].ToString();
                                SalaryProfileCode = drowEmpOld["SalProfile"] == DBNull.Value ? "" : drowEmpOld["SalProfile"].ToString();
                                StrSponsorCode = drowEmpOld["SponsorCode"] == DBNull.Value ? "" : drowEmpOld["SponsorCode"].ToString();
                            }
                        }
                        if (!isUpdate)
                        {
                            //if(string.IsNullOrEmpty(StrAddressE))//assign Address from (flat+building+street+area+city) if address is empty for NEW JOIN
                            //{
                            //    if (!string.IsNullOrEmpty(StrFlatE))
                            //    {
                            //        StrAddressE += StrFlatE +" ";
                            //    }
                            //    if (!string.IsNullOrEmpty(StrBuildingE))
                            //    {
                            //        StrAddressE += StrBuildingE + " ";
                            //    }
                            //    if (!string.IsNullOrEmpty(StrStreetE))
                            //    {
                            //        StrAddressE += StrStreetE + " ";
                            //    }
                            //    if (!string.IsNullOrEmpty(AreaCode))
                            //    {
                            //        StrAddressE += StrArea + " ";
                            //    }
                            //    if (!string.IsNullOrEmpty(EmiratesCode))
                            //    {
                            //        StrAddressE += StrEmirates + " ";
                            //    }
                            //    StrAddressE = StrAddressE.Trim();
                            //    row[xlcol_AddressE] = StrAddressE;
                            //}

                        }

                        //============================================================================================


                        if (string.IsNullOrEmpty(LocationLevelCode))
                        {

                            //AppendLineError(nRowNo, rowErrInfo, "Could not verify Location and Salary Profile rights.");
                            AppendLineError(nRowNo, rowErrInfo, "Could not verify Location.");
                            bskipInsertUpdate = true;

                        }
                        //else
                        //{
                        //    if (!CheckUserRights_LocationAndSalaryProfile(LocationLevelCode, SalaryProfileCode, ref errmsg))
                        //    {
                        //        AppendLineError(nRowNo, rowErrInfo, errmsg);
                        //        bskipInsertUpdate;

                        //    }
                        //}

                        if (bskipInsertUpdate == true)
                        {
                            goto skipInsertUpdateStep;
                        }

                       




                        #region Insert/Update TO CS


                        bool skipEmpSave = false;

                        DataTable dtEmployee = new DataTable("Employee");
                        dtEmployee = this.CreateEmployeeTableSchema();
                        DataRow rRow = default(DataRow);
                        rRow = dtEmployee.NewRow();

                        int il = 1;
                        String lastLocForWF = "";
                        if (!string.IsNullOrEmpty(LocationLevelCode))
                        {
                            // #NewLoc Location BR/WF
                            String locFullCode = String.Empty;
                            String ErrMsgBR = String.Empty;
                            bool bRetVal;
                            sQry = "Select dbo.IL_fun_GetLocationFullCode(" + LocationLevelCode + ")";
                            bRetVal = ConnectionFunctions.Connect_SQLScalar(ref locFullCode, sQry, ref ErrMsgBR);



                            if (bRetVal == false)
                            {

                                AppendLineError(nRowNo, rowErrInfo, "UnabletoRetriveLocationHierarchy");
                                skipEmpSave = true;
                                goto skipEmpSaveStep;
                            }

                            string[] fullCode = locFullCode.Split('>');

                            String cLoc = "";
                            foreach (string word in fullCode)
                            {
                                cLoc = "LocLib" + il.ToString();
                                if (il > 5)
                                {
                                    dtEmployee.Columns.Add(cLoc, typeof(string));
                                }
                                rRow[cLoc] = word;
                                il++;

                                lastLocForWF = word;
                            }
                        }






                        if (il <= 5)
                        {
                            rRow["LocLib5"] = lastLocForWF;
                        }
                        //#NewLoc Location BR/WF


                        if(isUpdate)
                            rRow["EmpId"] = nEmpID;

                        rRow["EmpCode"] = StrEmpCode;
                        rRow["Title"] = TitleCode;
                        rRow["EmpNameE"] = StrEmpNameE;
                        rRow["EmpNameA"] = StrEmpNameA;
                        rRow["NickNameE"] = StrNickNameE;
                        rRow["NickNameA"] = StrNickNameA;
                        rRow["RelType"] = RelTypeCode;
                        rRow["RelNameE"] = StrRelNameE;
                        rRow["RelNameA"] = StrRelNameA;
                        rRow["MotherNameE"] = StrMotherNameE;
                        rRow["MotherNameA"] = StrMotherNameA;
                        rRow["FamilyNameE"] = StrFamilyNameE;
                        rRow["FamilyNameA"] = StrFamilyNameA;
                        rRow["Sex"] = GenderCode;
                        rRow["NPresent"] = PresentNationalityCode;
                        rRow["NPrevious"] = PreviousNationalityCode;
                        rRow["MaritalStat"] = MaritalStatusCode;
                        rRow["DateOfBirth"] = DateOfBirthDateValue.ToString("dd/MM/yyyy"); //because DateOfBirth is varchar
                        rRow["DOBDAY"] = DOBDAYVal;
                        rRow["DOBMONTH"] = DOBMONTHVal;
                        rRow["DOBYEAR"] = DOBYEARVal;
                        rRow["BirthPlaceE"] = StrBirthPlaceE;
                        rRow["BirthPlaceA"] = StrBirthPlaceA;
                        rRow["CountryOfBirth"] = CountryOfBirthCode;
                        rRow["PassportNoE"] = StrPassportNoE;
                        rRow["PassportNoA"] = StrPassportNoA;
                        rRow["PCategory"] = PCategoryCode;
                        rRow["PIssuePlaceE"] = StrPIssuePlaceE;
                        rRow["PIssuePlaceA"] = StrPIssuePlaceA;
                        rRow["PIssueCountry"] = PassportIssueCountryCode;
                        rRow["PIssueDate"] = PassportIssueDateValue.ToString("yyyy/MM/dd");
                        rRow["PExpiryDate"] = PassportExpiryDateValue.ToString("yyyy/MM/dd");
                        rRow["Religion"] = ReligionCode;
                        rRow["PassportProf"] = VisaProfessionCode;
                        rRow["Education"] = StrEducation;
                        rRow["PerAddressE"] = StrPerAddressE;
                        rRow["PerAddressA"] = StrPerAddressA;
                        rRow["Skill1"] = Skill1Code;
                        rRow["Skill2"] = Skill2Code;
                        rRow["Skill3"] = Skill3Code;
                        rRow["Language1"] = Language1Code;
                        rRow["Language2"] = Language2Code;
                        rRow["Language3"] = Language3Code;
                        rRow["VisaType"] = VisaTypeCode;
                        rRow["VisaNo"] = StrVisaNo;
                        rRow["VisaIssueDate"] = VisaIssueDateValue.ToString("yyyy/MM/dd");
                        rRow["ImmigFileNo"] = StrImmigFileNo;
                        rRow["EntryPlace"] = StrEntryPlace;
                        rRow["EntryDate"] = EntryDateValue.ToString("yyyy/MM/dd");
                        rRow["ResidenceNo"] = StrResidenceNo;
                        rRow["ResIssueDate"] = ResIssueDateValue.ToString("yyyy/MM/dd");
                        rRow["ResExpDate"] = ResExpDateValue.ToString("yyyy/MM/dd");
                        rRow["ResIssuePlace"] = ResIssuePlaceCode;
                        rRow["LabCardNo"] = StrLabCardNo;
                        rRow["LCIssueDate"] = LCIssueDateValue.ToString("yyyy/MM/dd");
                        rRow["LCExpDate"] = LCExpDateValue.ToString("yyyy/MM/dd");
                        rRow["HlthCardNo"] = StrHlthCardNo;
                        rRow["HCIssuePlace"] = HCIssuePlaceCode;
                        rRow["HCIssueDate"] = HCIssueDateValue.ToString("yyyy/MM/dd");
                        rRow["HCExpiryDate"] = HCExpiryDateValue.ToString("yyyy/MM/dd");
                        rRow["DrvLicNo"] = StrDrvLicNo;
                        rRow["DLCategory"] = DLCategoryCode;
                        rRow["DLIssuePlace"] = StrDLIssuePlace;
                        rRow["DLIssueDate"] = DLIssueDateValue.ToString("yyyy/MM/dd");
                        rRow["DLExpiryDate"] = DLExpiryDateValue.ToString("yyyy/MM/dd");
                        rRow["SponsorCode"] = StrSponsorCode;
                        rRow["SponByOther"] = SponByOtherCode;
                        rRow["OSponNameE"] = StrOSponNameE;
                        rRow["OSponNameA"] = StrOSponNameA;
                        rRow["OSponRel"] = OSponRelCode;
                        rRow["OSponNation"] = OSponNationCode;
                        rRow["OSponVisaNo"] = StrOSponVisaNo;
                        rRow["OSponVExpDt"] = OSponVExpDtValue.ToString("yyyy/MM/dd");
                        rRow["OSponPsprtNoE"] = StrOSponPsprtNoE;
                        rRow["OSponPsprtNoA"] = StrOSponPsprtNoA;
                        rRow["ExperienceE"] = StrExperienceE;
                        rRow["ExperienceA"] = StrExperienceA;
                        rRow["Emirates"] = EmiratesCode;
                        rRow["City"] = CityCode;
                        rRow["Area"] = AreaCode;
                        rRow["StreetE"] = StrStreetE;
                        rRow["StreetA"] = StrStreetA;
                        rRow["BuildingE"] = StrBuildingE;
                        rRow["BuildingA"] = StrBuildingA;
                        rRow["FlatE"] = StrFlatE;
                        rRow["FlatA"] = StrFlatA;
                        rRow["OffPhoneNo"] = StrOffPhoneNo;
                        rRow["Ext"] = StrExt;
                        rRow["ResPhoneNo"] = StrResPhoneNo;
                        rRow["POBox"] = StrPOBox;
                        rRow["MobileNo"] = StrMobileNo;
                        rRow["PagerNo"] = StrPagerNo;
                        rRow["TeleNoAbroad"] = StrTeleNoAbroad;
                        rRow["Email"] = StrEmail;
                        rRow["BloodGroup"] = BloodGroupCode;
                        rRow["FaxNo"] = StrFaxNo;
                        rRow["NextofKinE"] = StrNextofKinE;
                        rRow["NextofKinA"] = StrNextofKinA;
                        rRow["NextofKinAddrE"] = StrNextofKinAddrE;
                        rRow["NextofKinAddrA"] = StrNextofKinAddrA;
                        rRow["AddressE"] = StrAddressE;
                        rRow["AddressA"] = StrAddressA;
                        rRow["AuxString1"] = StrAuxString1;
                        rRow["AuxString2"] = StrAuxString2;
                        rRow["AuxString3"] = StrAuxString3;
                        rRow["AuxString4"] = StrAuxString4;
                        rRow["AuxString5"] = StrAuxString5;
                        rRow["AuxString6"] = StrAuxString6;
                        rRow["AuxString7"] = StrAuxString7;
                        rRow["AuxString8"] = StrAuxString8;
                        rRow["AuxString9"] = StrAuxString9;
                        rRow["AuxString10"] = StrAuxString10;
                        rRow["AuxAString1"] = StrAuxAString1;
                        rRow["AuxAString2"] = StrAuxAString2;
                        rRow["AuxAString3"] = StrAuxAString3;
                        rRow["AuxAString4"] = StrAuxAString4;
                        rRow["AuxAString5"] = StrAuxAString5;
                        rRow["AuxAString6"] = StrAuxAString6;
                        rRow["AuxAString7"] = StrAuxAString7;
                        rRow["AuxAString8"] = StrAuxAString8;
                        rRow["AuxAString9"] = StrAuxAString9;
                        rRow["AuxAString10"] = StrAuxAString10;
                        rRow["AuxInt1"] = nAuxInt1;
                        rRow["AuxInt2"] = nAuxInt2;
                        rRow["AuxInt3"] = nAuxInt3;
                        rRow["AuxCurrency1"] = nAuxCurrency1;
                        rRow["AuxCurrency2"] = nAuxCurrency2;
                        rRow["AuxDate1"] = AuxDate1Value.ToString("yyyy/MM/dd");
                        rRow["AuxDate2"] = AuxDate2Value.ToString("yyyy/MM/dd");
                        rRow["AuxDate3"] = AuxDate3Value.ToString("yyyy/MM/dd");
                        rRow["AuxDate4"] = AuxDate4Value.ToString("yyyy/MM/dd");
                        rRow["AuxDate5"] = AuxDate5Value.ToString("yyyy/MM/dd");
                        rRow["AuxLib1"] = AuxLib1Code;
                        rRow["AuxLib2"] = AuxLib2Code;
                        rRow["AuxLib3"] = AuxLib3Code;
                        rRow["AuxLib4"] = AuxLib4Code;
                        rRow["IntlJoiningDate"] = IntlJoiningDateValue.ToString("yyyy/MM/dd");
                        rRow["PointOfHireE"] = PointOfHireECode;
                        rRow["PointOfHireA"] = StrPointOfHireA;
                        rRow["SourceOfHire"] = SourceOfHireCode;
                        rRow["SLReInitDate"] = SLReInitDateValue.ToString("yyyy/MM/dd");
                        rRow["StartDtofIndemnity"] = StartDtofIndemnityValue.ToString("yyyy/MM/dd");
                        rRow["LocLib5"] = LocationLevelCode;
                        rRow["WrkAgreeNo"] = StrWrkAgreeNo;
                        rRow["SalProfile"] = SalaryProfileCode;
                        rRow["HealthInsurCmp"] = HealthInsurCmpCode;
                        rRow["LCIssuePlace"] = LCIssuePlaceCode;
                        rRow["FNameE"] = StrFNameE;
                        rRow["FNameA"] = StrFNameA;
                        rRow["SNameE"] = StrSNameE;
                        rRow["SNameA"] = StrSNameA;
                        rRow["GrandFatherE"] = StrGrandFatherE;
                        rRow["SponTypeExtnl"] = SponTypeExtnlCode;
                        rRow["GrandFatherA"] = StrGrandFatherA;
                        rRow["VisaExpDate"] = VisaExpDateValue.ToString("yyyy/MM/dd");
                        rRow["AuxLib5"] = AuxLib5Code;
                        rRow["AuxLib6"] = AuxLib6Code;
                        rRow["PersEmail"] = StrPersEmail;
                        rRow["NPresentSec"] = NPresentSecCode;
                        rRow["ReligionSubSet"] = ReligionSubSetCode;
                        rRow["UIDNo"] = StrUIDNo;
                        rRow["Disability"] = DisabilityCode;
                        rRow["DisabilityDescE"] = StrDisabilityDescE;
                        rRow["DisabilityDescA"] = StrDisabilityDescA;
                        rRow["NationalID"] = StrNationalID;
                        rRow["WPS"] = WPSCode;







                        dtEmployee.Rows.Add(rRow);
                        dtEmployee.AcceptChanges();

                        Boolean RetVal = false;
                        string Codes = string.Empty;
                        //Employee Business rule validation.
                        DataTable ErrTable = new DataTable();
                        ErrTable = Common.GetErrMast(1);
                        Dictionary<String, String> Errors = null;

                        Common.UpdateErrorSeverityForService(ref ErrTable,1);

                        //RetVal = BusinessRules.CheckBusinessRule(1, dtEmployee, "Employee", "EmpCode", ErrTable, out Errors);

                        //if (RetVal == false)
                        //{
                        //    AppendLineError(nRowNo, rowErrInfo, "An unexpected error occured while verifying Employee business rule");
                        //    bHasEmpBRerror = true;
                        //    skipEmpSave = true;
                        //}

                        Int16 Ctr1 = 0;
                        //if (Errors.Count > 0)
                        //{
                        //    string[] arCodes;
                        //    arCodes = Errors[StrEmpCode.ToString()].ToString().Split('@');
                        //    for (Ctr1 = 0; Ctr1 <= arCodes.Length - 1; Ctr1++)
                        //    {
                        //        Codes = Codes + "'" + arCodes.GetValue(Ctr1) + "',";
                        //    }
                        //}
                        if (Codes.Trim().Length != 0)
                        {
                            DataView dvErrors = new DataView(ErrTable);
                            dvErrors.RowFilter = "Code IN(" + Codes + ") AND Severity NOT IN (2,1)";

                            //if (dvErrors.Count > 0)
                            //{
                            //    int nErrno = 0;
                            //    foreach (DataRowView rowDv in dvErrors)
                            //    {
                            //        if (rowDv[5].ToString() == "0" || rowDv[5].ToString() == "3")
                            //        {
                            //            nErrno++;
                            //            AppendLineError(nRowNo, rowErrInfo, "Employee Business rule Error#" + nErrno + " - " + rowDv["Message"].ToString());
                            //            bHasEmpBRerror = true;
                            //            skipEmpSave = true;

                            //        }
                            //    }

                            //}
                        }

                        //// Employee business rule code end 

                        if (skipEmpSave)
                        {
                            goto skipEmpSaveStep;
                        }



                        SqlConnection myConn = new SqlConnection();
                        myConn.ConnectionString = ConnectionFunctions.GetConnectionString();
                        SqlTransaction SqlTran = null;

                        try
                        {
                            myConn.Open();

                            SqlTran = myConn.BeginTransaction();

                            SqlCommand myCmd = new SqlCommand();
                            myCmd.Connection = myConn;
                            myCmd.Transaction = SqlTran;

                            #region Employee DB Save


                            myCmd.Parameters.Clear();
                            myCmd.CommandText = "USP_DBoxToCSEmpImport_Emp_InsertUpdate";
                            myCmd.CommandType = CommandType.StoredProcedure;


                            myCmd.Parameters.AddWithValue("@UserID", Common.strSvcUserId);
                            myCmd.Parameters.AddWithValue("@IPAddress", "");

                            myCmd.Parameters.AddWithValue("@EmpCode", StrEmpCode);

                            #region Procedure Parameters
                            if (CheckIfColumnExists(mydt, xlcol_Title) && !StringIsNullOrEmpty(row[xlcol_Title].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@Title", TitleCode);
                                Build_UpdateAudErrors(isUpdate, "Title", TitleCode, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@Title", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_EmpNameE) && !StringIsNullOrEmpty(row[xlcol_EmpNameE].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@EmpNameE", StrEmpNameE);
                                Build_UpdateAudErrors(isUpdate, "EmpNameE", StrEmpNameE, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@EmpNameE", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_EmpNameA) && !StringIsNullOrEmpty(row[xlcol_EmpNameA].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@EmpNameA", StrEmpNameA);
                                Build_UpdateAudErrors(isUpdate, "EmpNameA", StrEmpNameA, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else if (!string.IsNullOrEmpty(StrEmpNameE))
                            {
                                //    // Translate English name → Arabic
                                //    var translator = new DBox_CS.Core.AppClass.TranslationHelperV2();
                                //    string jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cs-translationapiproj-321d0950989b.json");
                                //    string empNameA = DBox_CS.Core.AppClass.TranslationHelperV2.TranslateFromEnglishToArabic(StrEmpNameE, jsonPath);
                                //    myCmd.Parameters.AddWithValue("@EmpNameA", empNameA);
                                AddTranslatedParam(myCmd, "@EmpNameA", StrEmpNameE, StrEmpNameA, isUpdate, "EmpNameA", ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@EmpNameA", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_NickNameE) && !StringIsNullOrEmpty(row[xlcol_NickNameE].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@NickNameE", StrNickNameE);
                                Build_UpdateAudErrors(isUpdate, "NickNameE", StrNickNameE, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@NickNameE", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_NickNameA) && !StringIsNullOrEmpty(row[xlcol_NickNameA].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@NickNameA", StrNickNameA);
                                Build_UpdateAudErrors(isUpdate, "NickNameA", StrNickNameA, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else if (!string.IsNullOrEmpty(StrNickNameE))
                            {
                                AddTranslatedParam(myCmd, "@NickNameA", StrNickNameE, StrNickNameA, isUpdate, "NickNameA", ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@NickNameA", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_RelType) && !StringIsNullOrEmpty(row[xlcol_RelType].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@RelType", RelTypeCode);
                                Build_UpdateAudErrors(isUpdate, "RelType", RelTypeCode, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@RelType", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_RelNameE) && !StringIsNullOrEmpty(row[xlcol_RelNameE].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@RelNameE", StrRelNameE);
                                Build_UpdateAudErrors(isUpdate, "RelNameE", StrRelNameE, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@RelNameE", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_RelNameA) && !StringIsNullOrEmpty(row[xlcol_RelNameA].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@RelNameA", StrRelNameA);
                                Build_UpdateAudErrors(isUpdate, "RelNameA", StrRelNameA, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else if (!string.IsNullOrEmpty(StrRelNameE))
                            {
                                AddTranslatedParam(myCmd, "@RelNameA", StrRelNameE, StrNickNameA, isUpdate, "RelNameA", ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@RelNameA", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_MotherNameE) && !StringIsNullOrEmpty(row[xlcol_MotherNameE].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@MotherNameE", StrMotherNameE);
                                Build_UpdateAudErrors(isUpdate, "MotherNameE", StrMotherNameE, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@MotherNameE", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_MotherNameA) && !StringIsNullOrEmpty(row[xlcol_MotherNameA].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@MotherNameA", StrMotherNameA);
                                Build_UpdateAudErrors(isUpdate, "MotherNameA", StrMotherNameA, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else if (!string.IsNullOrEmpty(StrMotherNameE))
                            {
                                AddTranslatedParam(myCmd, "@MotherNameA", StrMotherNameE, StrMotherNameA, isUpdate, "MotherNameA", ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@MotherNameA", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_FamilyNameE) && !StringIsNullOrEmpty(row[xlcol_FamilyNameE].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@FamilyNameE", StrFamilyNameE);
                                Build_UpdateAudErrors(isUpdate, "FamilyNameE", StrFamilyNameE, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@FamilyNameE", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_FamilyNameA) && !StringIsNullOrEmpty(row[xlcol_FamilyNameA].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@FamilyNameA", StrFamilyNameA);
                                Build_UpdateAudErrors(isUpdate, "FamilyNameA", StrFamilyNameA, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else if (!string.IsNullOrEmpty(StrFamilyNameE))
                            {
                                AddTranslatedParam(myCmd, "@FamilyNameA", StrFamilyNameE, StrFamilyNameA, isUpdate, "FamilyNameA", ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@FamilyNameA", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_Sex) && !StringIsNullOrEmpty(row[xlcol_Sex].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@Sex", GenderCode);
                                Build_UpdateAudErrors(isUpdate, "Sex", GenderCode, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@Sex", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_NPresent) && !StringIsNullOrEmpty(row[xlcol_NPresent].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@NPresent", PresentNationalityCode);
                                Build_UpdateAudErrors(isUpdate, "NPresent", PresentNationalityCode, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@NPresent", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_NPrevious) && !StringIsNullOrEmpty(row[xlcol_NPrevious].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@NPrevious", PreviousNationalityCode);
                                Build_UpdateAudErrors(isUpdate, "NPrevious", PreviousNationalityCode, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@NPrevious", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_MaritalStat) && !StringIsNullOrEmpty(row[xlcol_MaritalStat].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@MaritalStat", MaritalStatusCode);
                                Build_UpdateAudErrors(isUpdate, "MaritalStat", MaritalStatusCode, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@MaritalStat", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_DateOfBirth) && !StringIsNullOrEmpty(row[xlcol_DateOfBirth].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@DateOfBirth", DateOfBirthDateValue.ToString("dd/MM/yyyy"));
                                Build_UpdateAudErrors(isUpdate, "DateOfBirth", DateOfBirthDateValue.ToString("dd/MM/yyyy"), ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@DateOfBirth", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_DateOfBirth) && !StringIsNullOrEmpty(row[xlcol_DateOfBirth].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@DOBDAY", DOBDAYVal);
                                Build_UpdateAudErrors(isUpdate, "DOBDAY", DOBDAYVal, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@DOBDAY", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_DateOfBirth) && !StringIsNullOrEmpty(row[xlcol_DateOfBirth].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@DOBMONTH", DOBMONTHVal);
                                Build_UpdateAudErrors(isUpdate, "DOBMONTH", DOBMONTHVal, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@DOBMONTH", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_DateOfBirth) && !StringIsNullOrEmpty(row[xlcol_DateOfBirth].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@DOBYEAR", DOBYEARVal);
                                Build_UpdateAudErrors(isUpdate, "DOBYEAR", DOBYEARVal, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@DOBYEAR", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_BirthPlaceE) && !StringIsNullOrEmpty(row[xlcol_BirthPlaceE].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@BirthPlaceE", StrBirthPlaceE);
                                Build_UpdateAudErrors(isUpdate, "BirthPlaceE", StrBirthPlaceE, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@BirthPlaceE", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_BirthPlaceA) && !StringIsNullOrEmpty(row[xlcol_BirthPlaceA].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@BirthPlaceA", StrBirthPlaceA);
                                Build_UpdateAudErrors(isUpdate, "BirthPlaceA", StrBirthPlaceA, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else if (!string.IsNullOrEmpty(StrBirthPlaceE))
                            {
                                AddTranslatedParam(myCmd, "@BirthPlaceA", StrBirthPlaceE, StrBirthPlaceA, isUpdate, "BirthPlaceA", ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@BirthPlaceA", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_CountryOfBirth) && !StringIsNullOrEmpty(row[xlcol_CountryOfBirth].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@CountryOfBirth", CountryOfBirthCode);
                                Build_UpdateAudErrors(isUpdate, "CountryOfBirth", CountryOfBirthCode, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@CountryOfBirth", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_PassportNoE) && !StringIsNullOrEmpty(row[xlcol_PassportNoE].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@PassportNoE", StrPassportNoE);
                                Build_UpdateAudErrors(isUpdate, "PassportNoE", StrPassportNoE, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@PassportNoE", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_PassportNoA) && !StringIsNullOrEmpty(row[xlcol_PassportNoA].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@PassportNoA", StrPassportNoA);
                                Build_UpdateAudErrors(isUpdate, "PassportNoA", StrPassportNoA, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else if (!string.IsNullOrEmpty(StrPassportNoE))
                            {
                                AddTranslatedParam(myCmd, "@PassportNoA", StrPassportNoE, StrPassportNoA, isUpdate, "PassportNoA", ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@PassportNoA", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_PCategory) && !StringIsNullOrEmpty(row[xlcol_PCategory].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@PCategory", PCategoryCode);
                                Build_UpdateAudErrors(isUpdate, "PCategory", PCategoryCode, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@PCategory", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_PIssuePlaceE) && !StringIsNullOrEmpty(row[xlcol_PIssuePlaceE].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@PIssuePlaceE", StrPIssuePlaceE);
                                Build_UpdateAudErrors(isUpdate, "PIssuePlaceE", StrPIssuePlaceE, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@PIssuePlaceE", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_PIssuePlaceA) && !StringIsNullOrEmpty(row[xlcol_PIssuePlaceA].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@PIssuePlaceA", StrPIssuePlaceA);
                                Build_UpdateAudErrors(isUpdate, "PIssuePlaceA", StrPIssuePlaceA, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else if (!string.IsNullOrEmpty(StrPIssuePlaceE))
                            {
                                AddTranslatedParam(myCmd, "@PIssuePlaceA", StrPIssuePlaceE, StrPIssuePlaceA, isUpdate, "PIssuePlaceA", ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@PIssuePlaceA", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_PIssueCountry) && !StringIsNullOrEmpty(row[xlcol_PIssueCountry].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@PIssueCountry", PassportIssueCountryCode);
                                Build_UpdateAudErrors(isUpdate, "PIssueCountry", PassportIssueCountryCode, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@PIssueCountry", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_PIssueDate) && !StringIsNullOrEmpty(row[xlcol_PIssueDate].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@PIssueDate", PassportIssueDateValue.ToString("yyyy-MM-dd")).SqlDbType = SqlDbType.SmallDateTime;
                                Build_UpdateAudErrors(isUpdate, "PIssueDate", PassportIssueDateValue, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@PIssueDate", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_PExpiryDate) && !StringIsNullOrEmpty(row[xlcol_PExpiryDate].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@PExpiryDate", PassportExpiryDateValue.ToString("yyyy-MM-dd")).SqlDbType = SqlDbType.SmallDateTime;
                                Build_UpdateAudErrors(isUpdate, "PExpiryDate", PassportExpiryDateValue, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@PExpiryDate", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_Religion) && !StringIsNullOrEmpty(row[xlcol_Religion].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@Religion", ReligionCode);
                                Build_UpdateAudErrors(isUpdate, "Religion", ReligionCode, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@Religion", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_PassportProf) && !StringIsNullOrEmpty(row[xlcol_PassportProf].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@PassportProf", VisaProfessionCode);
                                Build_UpdateAudErrors(isUpdate, "PassportProf", VisaProfessionCode, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@PassportProf", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_Education) && !StringIsNullOrEmpty(row[xlcol_Education].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@Education", VisaQualificationCode);
                                Build_UpdateAudErrors(isUpdate, "Education", VisaQualificationCode, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@Education", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_PerAddressE) && !StringIsNullOrEmpty(row[xlcol_PerAddressE].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@PerAddressE", StrPerAddressE);
                                Build_UpdateAudErrors(isUpdate, "PerAddressE", StrPerAddressE, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@PerAddressE", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_PerAddressA) && !StringIsNullOrEmpty(row[xlcol_PerAddressA].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@PerAddressA", StrPerAddressA);
                                Build_UpdateAudErrors(isUpdate, "PerAddressA", StrPerAddressA, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else if (!string.IsNullOrEmpty(StrPerAddressE))
                            {
                                AddTranslatedParam(myCmd, "@PerAddressA", StrPerAddressE, StrPerAddressA, isUpdate, "PerAddressA", ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@PerAddressA", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_Skill1) && !StringIsNullOrEmpty(row[xlcol_Skill1].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@Skill1", Skill1Code);
                                Build_UpdateAudErrors(isUpdate, "Skill1", Skill1Code, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@Skill1", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_Skill2) && !StringIsNullOrEmpty(row[xlcol_Skill2].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@Skill2", Skill2Code);
                                Build_UpdateAudErrors(isUpdate, "Skill2", Skill2Code, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@Skill2", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_Skill3) && !StringIsNullOrEmpty(row[xlcol_Skill3].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@Skill3", Skill3Code);
                                Build_UpdateAudErrors(isUpdate, "Skill3", Skill3Code, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@Skill3", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_Language1) && !StringIsNullOrEmpty(row[xlcol_Language1].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@Language1", Language1Code);
                                Build_UpdateAudErrors(isUpdate, "Language1", Language1Code, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@Language1", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_Language2) && !StringIsNullOrEmpty(row[xlcol_Language2].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@Language2", Language2Code);
                                Build_UpdateAudErrors(isUpdate, "Language2", Language2Code, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@Language2", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_Language3) && !StringIsNullOrEmpty(row[xlcol_Language3].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@Language3", Language3Code);
                                Build_UpdateAudErrors(isUpdate, "Language3", Language3Code, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@Language3", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_VisaType) && !StringIsNullOrEmpty(row[xlcol_VisaType].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@VisaType", VisaTypeCode);
                                Build_UpdateAudErrors(isUpdate, "VisaType", VisaTypeCode, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@VisaType", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_VisaNo) && !StringIsNullOrEmpty(row[xlcol_VisaNo].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@VisaNo", StrVisaNo);
                                Build_UpdateAudErrors(isUpdate, "VisaNo", StrVisaNo, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@VisaNo", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_VisaIssueDate) && !StringIsNullOrEmpty(row[xlcol_VisaIssueDate].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@VisaIssueDate", VisaIssueDateValue.ToString("yyyy-MM-dd")).SqlDbType = SqlDbType.SmallDateTime;
                                Build_UpdateAudErrors(isUpdate, "VisaIssueDate", VisaIssueDateValue, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@VisaIssueDate", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_ImmigFileNo) && !StringIsNullOrEmpty(row[xlcol_ImmigFileNo].ToString()))
                            {
                                if (WPSCode == "0"|| row[xlcol_ImmigFileNo].ToString()==clearFieldMarker)// when wps is 0 i.e "NO" WPS number will be made blank  OR <blank> is passed in cell
                                {
                                    myCmd.Parameters.AddWithValue("@ImmigFileNo", clearFieldMarker);
                                    Build_UpdateAudErrors(isUpdate, "ImmigFileNo", "", ref strBuildUpdtAud, ref drowEmpOld);
                                }
                                else 
                                {
                                    myCmd.Parameters.AddWithValue("@ImmigFileNo", StrImmigFileNo);
                                    Build_UpdateAudErrors(isUpdate, "ImmigFileNo", StrImmigFileNo, ref strBuildUpdtAud, ref drowEmpOld);
                                }
                            }                            
                            else
                                myCmd.Parameters.AddWithValue("@ImmigFileNo", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_EntryPlace) && !StringIsNullOrEmpty(row[xlcol_EntryPlace].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@EntryPlace", StrEntryPlace);
                                Build_UpdateAudErrors(isUpdate, "EntryPlace", StrEntryPlace, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@EntryPlace", DBNull.Value);

                            if (CheckIfColumnExists(mydt, xlcol_EntryDate) && !StringIsNullOrEmpty(row[xlcol_EntryDate].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@EntryDate", EntryDateValue.ToString("yyyy-MM-dd")).SqlDbType = SqlDbType.SmallDateTime;
                                Build_UpdateAudErrors(isUpdate, "EntryDate", EntryDateValue, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@EntryDate", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_ResidenceNo) && !StringIsNullOrEmpty(row[xlcol_ResidenceNo].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@ResidenceNo", StrResidenceNo);
                                Build_UpdateAudErrors(isUpdate, "ResidenceNo", StrResidenceNo, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@ResidenceNo", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_ResIssueDate) && !StringIsNullOrEmpty(row[xlcol_ResIssueDate].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@ResIssueDate", ResIssueDateValue.ToString("yyyy-MM-dd")).SqlDbType = SqlDbType.SmallDateTime;
                                Build_UpdateAudErrors(isUpdate, "ResIssueDate", ResIssueDateValue, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@ResIssueDate", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_ResExpDate) && !StringIsNullOrEmpty(row[xlcol_ResExpDate].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@ResExpDate", ResExpDateValue.ToString("yyyy-MM-dd")).SqlDbType = SqlDbType.SmallDateTime;
                                Build_UpdateAudErrors(isUpdate, "ResExpDate", ResExpDateValue, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@ResExpDate", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_ResIssuePlace) && !StringIsNullOrEmpty(row[xlcol_ResIssuePlace].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@ResIssuePlace", ResIssuePlaceCode);
                                Build_UpdateAudErrors(isUpdate, "ResIssuePlace", ResIssuePlaceCode, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@ResIssuePlace", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_LabCardNo) && !StringIsNullOrEmpty(row[xlcol_LabCardNo].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@LabCardNo", StrLabCardNo);
                                Build_UpdateAudErrors(isUpdate, "LabCardNo", StrLabCardNo, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@LabCardNo", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_LCIssueDate) && !StringIsNullOrEmpty(row[xlcol_LCIssueDate].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@LCIssueDate", LCIssueDateValue.ToString("yyyy-MM-dd")).SqlDbType = SqlDbType.SmallDateTime;
                                Build_UpdateAudErrors(isUpdate, "LCIssueDate", LCIssueDateValue, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@LCIssueDate", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_LCExpDate) && !StringIsNullOrEmpty(row[xlcol_LCExpDate].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@LCExpDate", LCExpDateValue.ToString("yyyy-MM-dd")).SqlDbType = SqlDbType.SmallDateTime;
                                Build_UpdateAudErrors(isUpdate, "LCExpDate", LCExpDateValue, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@LCExpDate", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_HlthCardNo) && !StringIsNullOrEmpty(row[xlcol_HlthCardNo].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@HlthCardNo", StrHlthCardNo);
                                Build_UpdateAudErrors(isUpdate, "HlthCardNo", StrHlthCardNo, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@HlthCardNo", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_HCIssuePlace) && !StringIsNullOrEmpty(row[xlcol_HCIssuePlace].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@HCIssuePlace", HCIssuePlaceCode);
                                Build_UpdateAudErrors(isUpdate, "HCIssuePlace", HCIssuePlaceCode, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@HCIssuePlace", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_HCIssueDate) && !StringIsNullOrEmpty(row[xlcol_HCIssueDate].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@HCIssueDate", HCIssueDateValue.ToString("yyyy-MM-dd")).SqlDbType = SqlDbType.SmallDateTime;
                                Build_UpdateAudErrors(isUpdate, "HCIssueDate", HCIssueDateValue, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@HCIssueDate", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_HCExpiryDate) && !StringIsNullOrEmpty(row[xlcol_HCExpiryDate].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@HCExpiryDate", HCExpiryDateValue.ToString("yyyy-MM-dd")).SqlDbType = SqlDbType.SmallDateTime;
                                Build_UpdateAudErrors(isUpdate, "HCExpiryDate", HCExpiryDateValue, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@HCExpiryDate", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_DrvLicNo) && !StringIsNullOrEmpty(row[xlcol_DrvLicNo].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@DrvLicNo", StrDrvLicNo);
                                Build_UpdateAudErrors(isUpdate, "DrvLicNo", StrDrvLicNo, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@DrvLicNo", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_DLCategory) && !StringIsNullOrEmpty(row[xlcol_DLCategory].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@DLCategory", DLCategoryCode);
                                Build_UpdateAudErrors(isUpdate, "DLCategory", DLCategoryCode, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@DLCategory", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_DLIssuePlace) && !StringIsNullOrEmpty(row[xlcol_DLIssuePlace].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@DLIssuePlace", StrDLIssuePlace);
                                Build_UpdateAudErrors(isUpdate, "DLIssuePlace", StrDLIssuePlace, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@DLIssuePlace", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_DLIssueDate) && !StringIsNullOrEmpty(row[xlcol_DLIssueDate].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@DLIssueDate", DLIssueDateValue.ToString("yyyy-MM-dd")).SqlDbType = SqlDbType.SmallDateTime;
                                Build_UpdateAudErrors(isUpdate, "DLIssueDate", DLIssueDateValue, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@DLIssueDate", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_DLExpiryDate) && !StringIsNullOrEmpty(row[xlcol_DLExpiryDate].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@DLExpiryDate", DLExpiryDateValue.ToString("yyyy-MM-dd")).SqlDbType = SqlDbType.SmallDateTime;
                                Build_UpdateAudErrors(isUpdate, "DLExpiryDate", DLExpiryDateValue, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@DLExpiryDate", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_SponsorCode) && !StringIsNullOrEmpty(row[xlcol_SponsorCode].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@SponsorCode", StrSponsorCode);
                                Build_UpdateAudErrors(isUpdate, "SponsorCode", StrSponsorCode, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@SponsorCode", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_SponByOther) && !StringIsNullOrEmpty(row[xlcol_SponByOther].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@SponByOther", SponByOtherCode);
                                Build_UpdateAudErrors(isUpdate, "SponByOther", SponByOtherCode, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@SponByOther", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_OSponNameE) && !StringIsNullOrEmpty(row[xlcol_OSponNameE].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@OSponNameE", StrOSponNameE);
                                Build_UpdateAudErrors(isUpdate, "OSponNameE", StrOSponNameE, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@OSponNameE", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_OSponNameA) && !StringIsNullOrEmpty(row[xlcol_OSponNameA].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@OSponNameA", StrOSponNameA);
                                Build_UpdateAudErrors(isUpdate, "OSponNameA", StrOSponNameA, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else if (!string.IsNullOrEmpty(StrOSponNameE))
                            {
                                AddTranslatedParam(myCmd, "@OSponNameA", StrOSponNameE, StrOSponNameA, isUpdate, "OSponNameA", ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@OSponNameA", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_OSponRel) && !StringIsNullOrEmpty(row[xlcol_OSponRel].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@OSponRel", OSponRelCode);
                                Build_UpdateAudErrors(isUpdate, "OSponRel", OSponRelCode, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@OSponRel", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_OSponNation) && !StringIsNullOrEmpty(row[xlcol_OSponNation].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@OSponNation", OSponNationCode);
                                Build_UpdateAudErrors(isUpdate, "OSponNation", OSponNationCode, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@OSponNation", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_OSponVisaNo) && !StringIsNullOrEmpty(row[xlcol_OSponVisaNo].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@OSponVisaNo", StrOSponVisaNo);
                                Build_UpdateAudErrors(isUpdate, "OSponVisaNo", StrOSponVisaNo, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@OSponVisaNo", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_OSponVExpDt) && !StringIsNullOrEmpty(row[xlcol_OSponVExpDt].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@OSponVExpDt", OSponVExpDtValue.ToString("yyyy-MM-dd")).SqlDbType = SqlDbType.SmallDateTime;
                                Build_UpdateAudErrors(isUpdate, "OSponVExpDt", OSponVExpDtValue, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@OSponVExpDt", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_OSponPsprtNoE) && !StringIsNullOrEmpty(row[xlcol_OSponPsprtNoE].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@OSponPsprtNoE", StrOSponPsprtNoE);
                                Build_UpdateAudErrors(isUpdate, "OSponPsprtNoE", StrOSponPsprtNoE, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@OSponPsprtNoE", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_OSponPsprtNoA) && !StringIsNullOrEmpty(row[xlcol_OSponPsprtNoA].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@OSponPsprtNoA", StrOSponPsprtNoA);
                                Build_UpdateAudErrors(isUpdate, "OSponPsprtNoA", StrOSponPsprtNoA, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else if (!string.IsNullOrEmpty(StrOSponPsprtNoE))
                            {
                                AddTranslatedParam(myCmd, "@OSponPsprtNoA", StrOSponPsprtNoE, StrOSponPsprtNoA, isUpdate, "OSponPsprtNoA", ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@OSponPsprtNoA", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_ExperienceE) && !StringIsNullOrEmpty(row[xlcol_ExperienceE].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@ExperienceE", StrExperienceE);
                                Build_UpdateAudErrors(isUpdate, "ExperienceE", StrExperienceE, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@ExperienceE", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_ExperienceA) && !StringIsNullOrEmpty(row[xlcol_ExperienceA].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@ExperienceA", StrExperienceA);
                                Build_UpdateAudErrors(isUpdate, "ExperienceA", StrExperienceA, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else if (!string.IsNullOrEmpty(StrExperienceE))
                            {
                                AddTranslatedParam(myCmd, "@ExperienceA", StrExperienceE, StrExperienceA, isUpdate, "ExperienceA", ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@ExperienceA", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_Emirates) && !StringIsNullOrEmpty(row[xlcol_Emirates].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@Emirates", EmiratesCode);
                                Build_UpdateAudErrors(isUpdate, "Emirates", EmiratesCode, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@Emirates", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_City) && !StringIsNullOrEmpty(row[xlcol_City].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@City", CityCode);
                                Build_UpdateAudErrors(isUpdate, "City", CityCode, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@City", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_Area) && !StringIsNullOrEmpty(row[xlcol_Area].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@Area", AreaCode);
                                Build_UpdateAudErrors(isUpdate, "Area", AreaCode, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@Area", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_StreetE) && !StringIsNullOrEmpty(row[xlcol_StreetE].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@StreetE", StrStreetE);
                                Build_UpdateAudErrors(isUpdate, "StreetE", StrStreetE, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@StreetE", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_StreetA) && !StringIsNullOrEmpty(row[xlcol_StreetA].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@StreetA", StrStreetA);
                                Build_UpdateAudErrors(isUpdate, "StreetA", StrStreetA, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else if (!string.IsNullOrEmpty(StrStreetE))
                            {
                                AddTranslatedParam(myCmd, "@StreetA", StrStreetE, StrStreetA, isUpdate, "StreetA", ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@StreetA", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_BuildingE) && !StringIsNullOrEmpty(row[xlcol_BuildingE].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@BuildingE", StrBuildingE);
                                Build_UpdateAudErrors(isUpdate, "BuildingE", StrBuildingE, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@BuildingE", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_BuildingA) && !StringIsNullOrEmpty(row[xlcol_BuildingA].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@BuildingA", StrBuildingA);
                                Build_UpdateAudErrors(isUpdate, "BuildingA", StrBuildingA, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else if (!string.IsNullOrEmpty(StrBuildingE))
                            {
                                AddTranslatedParam(myCmd, "@BuildingA", StrBuildingE, StrBuildingA, isUpdate, "BuildingA", ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@BuildingA", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_FlatE) && !StringIsNullOrEmpty(row[xlcol_FlatE].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@FlatE", StrFlatE);
                                Build_UpdateAudErrors(isUpdate, "FlatE", StrFlatE, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@FlatE", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_FlatA) && !StringIsNullOrEmpty(row[xlcol_FlatA].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@FlatA", StrFlatA);
                                Build_UpdateAudErrors(isUpdate, "FlatA", StrFlatA, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else if (!string.IsNullOrEmpty(StrFlatE))
                            {
                                AddTranslatedParam(myCmd, "@FlatA", StrFlatE, StrFlatA, isUpdate, "FlatA", ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@FlatA", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_OffPhoneNo) && !StringIsNullOrEmpty(row[xlcol_OffPhoneNo].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@OffPhoneNo", StrOffPhoneNo);
                                Build_UpdateAudErrors(isUpdate, "OffPhoneNo", StrOffPhoneNo, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@OffPhoneNo", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_Ext) && !StringIsNullOrEmpty(row[xlcol_Ext].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@Ext", StrExt);
                                Build_UpdateAudErrors(isUpdate, "Ext", StrExt, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@Ext", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_ResPhoneNo) && !StringIsNullOrEmpty(row[xlcol_ResPhoneNo].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@ResPhoneNo", StrResPhoneNo);
                                Build_UpdateAudErrors(isUpdate, "ResPhoneNo", StrResPhoneNo, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@ResPhoneNo", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_POBox) && !StringIsNullOrEmpty(row[xlcol_POBox].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@POBox", StrPOBox);
                                Build_UpdateAudErrors(isUpdate, "POBox", StrPOBox, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@POBox", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_MobileNo) && !StringIsNullOrEmpty(row[xlcol_MobileNo].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@MobileNo", StrMobileNo);
                                Build_UpdateAudErrors(isUpdate, "MobileNo", StrMobileNo, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@MobileNo", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_PagerNo) && !StringIsNullOrEmpty(row[xlcol_PagerNo].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@PagerNo", StrPagerNo);
                                Build_UpdateAudErrors(isUpdate, "PagerNo", StrPagerNo, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@PagerNo", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_TeleNoAbroad) && !StringIsNullOrEmpty(row[xlcol_TeleNoAbroad].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@TeleNoAbroad", StrTeleNoAbroad);
                                Build_UpdateAudErrors(isUpdate, "TeleNoAbroad", StrTeleNoAbroad, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@TeleNoAbroad", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_Email) && !StringIsNullOrEmpty(row[xlcol_Email].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@Email", StrEmail);
                                Build_UpdateAudErrors(isUpdate, "Email", StrEmail, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@Email", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_BloodGroup) && !StringIsNullOrEmpty(row[xlcol_BloodGroup].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@BloodGroup", BloodGroupCode);
                                Build_UpdateAudErrors(isUpdate, "BloodGroup", BloodGroupCode, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@BloodGroup", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_FaxNo) && !StringIsNullOrEmpty(row[xlcol_FaxNo].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@FaxNo", StrFaxNo);
                                Build_UpdateAudErrors(isUpdate, "FaxNo", StrFaxNo, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@FaxNo", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_NextofKinE) && !StringIsNullOrEmpty(row[xlcol_NextofKinE].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@NextofKinE", StrNextofKinE);
                                Build_UpdateAudErrors(isUpdate, "NextofKinE", StrNextofKinE, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@NextofKinE", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_NextofKinA) && !StringIsNullOrEmpty(row[xlcol_NextofKinA].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@NextofKinA", StrNextofKinA);
                                Build_UpdateAudErrors(isUpdate, "NextofKinA", StrNextofKinA, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else if (!string.IsNullOrEmpty(StrNextofKinE))
                            {
                                AddTranslatedParam(myCmd, "@NextofKinA", StrNextofKinE, StrNextofKinA, isUpdate, "NextofKinA", ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@NextofKinA", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_NextofKinAddrE) && !StringIsNullOrEmpty(row[xlcol_NextofKinAddrE].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@NextofKinAddrE", StrNextofKinAddrE);
                                Build_UpdateAudErrors(isUpdate, "NextofKinAddrE", StrNextofKinAddrE, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@NextofKinAddrE", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_NextofKinAddrA) && !StringIsNullOrEmpty(row[xlcol_NextofKinAddrA].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@NextofKinAddrA", StrNextofKinAddrA);
                                Build_UpdateAudErrors(isUpdate, "NextofKinAddrA", StrNextofKinAddrA, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@NextofKinAddrA", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_AddressE) && !StringIsNullOrEmpty(row[xlcol_AddressE].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@AddressE", StrAddressE);
                                Build_UpdateAudErrors(isUpdate, "AddressE", StrAddressE, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@AddressE", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_AddressA) && !StringIsNullOrEmpty(row[xlcol_AddressA].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@AddressA", StrAddressA);
                                Build_UpdateAudErrors(isUpdate, "AddressA", StrAddressA, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else if (!string.IsNullOrEmpty(StrAddressE))
                            {
                                AddTranslatedParam(myCmd, "@AddressA", StrAddressE, StrAddressA, isUpdate, "AddressA", ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@AddressA", DBNull.Value);

                    
                            if (CheckIfColumnExists(mydt, xlcol_AuxString1) && !StringIsNullOrEmpty(row[xlcol_AuxString1].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@AuxString1", StrAuxString1);
                                Build_UpdateAudErrors(isUpdate, "AuxString1", StrAuxString1, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@AuxString1", DBNull.Value);
                         

                            if (CheckIfColumnExists(mydt, xlcol_AuxString2) && !StringIsNullOrEmpty(row[xlcol_AuxString2].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@AuxString2", StrAuxString2);
                                Build_UpdateAudErrors(isUpdate, "AuxString2", StrAuxString2, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@AuxString2", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_AuxString3) && !StringIsNullOrEmpty(row[xlcol_AuxString3].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@AuxString3", StrAuxString3);
                                Build_UpdateAudErrors(isUpdate, "AuxString3", StrAuxString3, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@AuxString3", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_AuxString4) && !StringIsNullOrEmpty(row[xlcol_AuxString4].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@AuxString4", StrAuxString4);
                                Build_UpdateAudErrors(isUpdate, "AuxString4", StrAuxString4, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@AuxString4", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_AuxString5) && !StringIsNullOrEmpty(row[xlcol_AuxString5].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@AuxString5", StrAuxString5);
                                Build_UpdateAudErrors(isUpdate, "AuxString5", StrAuxString5, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@AuxString5", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_AuxString6) && !StringIsNullOrEmpty(row[xlcol_AuxString6].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@AuxString6", StrAuxString6);
                                Build_UpdateAudErrors(isUpdate, "AuxString6", StrAuxString6, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@AuxString6", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_AuxString7) && !StringIsNullOrEmpty(row[xlcol_AuxString7].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@AuxString7", StrAuxString7);
                                Build_UpdateAudErrors(isUpdate, "AuxString7", StrAuxString7, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@AuxString7", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_AuxString8) && !StringIsNullOrEmpty(row[xlcol_AuxString8].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@AuxString8", StrAuxString8);
                                Build_UpdateAudErrors(isUpdate, "AuxString8", StrAuxString8, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@AuxString8", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_AuxString9) && !StringIsNullOrEmpty(row[xlcol_AuxString9].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@AuxString9", StrAuxString9);
                                Build_UpdateAudErrors(isUpdate, "AuxString9", StrAuxString9, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@AuxString9", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_AuxString10) && !StringIsNullOrEmpty(row[xlcol_AuxString10].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@AuxString10", StrAuxString10);
                                Build_UpdateAudErrors(isUpdate, "AuxString10", StrAuxString10, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@AuxString10", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_AuxAString1) && !StringIsNullOrEmpty(row[xlcol_AuxAString1].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@AuxAString1", StrAuxAString1);
                                Build_UpdateAudErrors(isUpdate, "AuxAString1", StrAuxAString1, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@AuxAString1", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_AuxAString2) && !StringIsNullOrEmpty(row[xlcol_AuxAString2].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@AuxAString2", StrAuxAString2);
                                Build_UpdateAudErrors(isUpdate, "AuxAString2", StrAuxAString2, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@AuxAString2", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_AuxAString3) && !StringIsNullOrEmpty(row[xlcol_AuxAString3].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@AuxAString3", StrAuxAString3);
                                Build_UpdateAudErrors(isUpdate, "AuxAString3", StrAuxAString3, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@AuxAString3", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_AuxAString4) && !StringIsNullOrEmpty(row[xlcol_AuxAString4].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@AuxAString4", StrAuxAString4);
                                Build_UpdateAudErrors(isUpdate, "AuxAString4", StrAuxAString4, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@AuxAString4", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_AuxAString5) && !StringIsNullOrEmpty(row[xlcol_AuxAString5].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@AuxAString5", StrAuxAString5);
                                Build_UpdateAudErrors(isUpdate, "AuxAString5", StrAuxAString5, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@AuxAString5", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_AuxAString6) && !StringIsNullOrEmpty(row[xlcol_AuxAString6].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@AuxAString6", StrAuxAString6);
                                Build_UpdateAudErrors(isUpdate, "AuxAString6", StrAuxAString6, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@AuxAString6", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_AuxAString7) && !StringIsNullOrEmpty(row[xlcol_AuxAString7].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@AuxAString7", StrAuxAString7);
                                Build_UpdateAudErrors(isUpdate, "AuxAString7", StrAuxAString7, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@AuxAString7", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_AuxAString8) && !StringIsNullOrEmpty(row[xlcol_AuxAString8].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@AuxAString8", StrAuxAString8);
                                Build_UpdateAudErrors(isUpdate, "AuxAString8", StrAuxAString8, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@AuxAString8", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_AuxAString9) && !StringIsNullOrEmpty(row[xlcol_AuxAString9].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@AuxAString9", StrAuxAString9);
                                Build_UpdateAudErrors(isUpdate, "AuxAString9", StrAuxAString9, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@AuxAString9", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_AuxAString10) && !StringIsNullOrEmpty(row[xlcol_AuxAString10].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@AuxAString10", StrAuxAString10);
                                Build_UpdateAudErrors(isUpdate, "AuxAString10", StrAuxAString10, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@AuxAString10", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_AuxInt1) && !StringIsNullOrEmpty(row[xlcol_AuxInt1].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@AuxInt1", nAuxInt1);
                                Build_UpdateAudErrors(isUpdate, "AuxInt1", nAuxInt1, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@AuxInt1", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_AuxInt2) && !StringIsNullOrEmpty(row[xlcol_AuxInt2].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@AuxInt2", nAuxInt2);
                                Build_UpdateAudErrors(isUpdate, "AuxInt2", nAuxInt2, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@AuxInt2", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_AuxInt3) && !StringIsNullOrEmpty(row[xlcol_AuxInt3].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@AuxInt3", nAuxInt3);
                                Build_UpdateAudErrors(isUpdate, "AuxInt3", nAuxInt3, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@AuxInt3", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_AuxCurrency1) && !StringIsNullOrEmpty(row[xlcol_AuxCurrency1].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@AuxCurrency1", nAuxCurrency1);
                                Build_UpdateAudErrors(isUpdate, "AuxCurrency1", nAuxCurrency1, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@AuxCurrency1", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_AuxCurrency2) && !StringIsNullOrEmpty(row[xlcol_AuxCurrency2].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@AuxCurrency2", nAuxCurrency2);
                                Build_UpdateAudErrors(isUpdate, "AuxCurrency2", nAuxCurrency2, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@AuxCurrency2", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_AuxDate1) && !StringIsNullOrEmpty(row[xlcol_AuxDate1].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@AuxDate1", AuxDate1Value.ToString("yyyy-MM-dd")).SqlDbType = SqlDbType.SmallDateTime;
                                Build_UpdateAudErrors(isUpdate, "AuxDate1", AuxDate1Value, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@AuxDate1", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_AuxDate2) && !StringIsNullOrEmpty(row[xlcol_AuxDate2].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@AuxDate2", AuxDate2Value.ToString("yyyy-MM-dd")).SqlDbType = SqlDbType.SmallDateTime;
                                Build_UpdateAudErrors(isUpdate, "AuxDate2", AuxDate2Value, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@AuxDate2", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_AuxDate3) && !StringIsNullOrEmpty(row[xlcol_AuxDate3].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@AuxDate3", AuxDate3Value.ToString("yyyy-MM-dd")).SqlDbType = SqlDbType.SmallDateTime;
                                Build_UpdateAudErrors(isUpdate, "AuxDate3", AuxDate3Value, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@AuxDate3", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_AuxDate4) && !StringIsNullOrEmpty(row[xlcol_AuxDate4].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@AuxDate4", AuxDate4Value.ToString("yyyy-MM-dd")).SqlDbType = SqlDbType.SmallDateTime;
                                Build_UpdateAudErrors(isUpdate, "AuxDate4", AuxDate4Value, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@AuxDate4", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_AuxDate5) && !StringIsNullOrEmpty(row[xlcol_AuxDate5].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@AuxDate5", AuxDate5Value.ToString("yyyy-MM-dd")).SqlDbType = SqlDbType.SmallDateTime;
                                Build_UpdateAudErrors(isUpdate, "AuxDate5", AuxDate5Value, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@AuxDate5", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_AuxLib1) && !StringIsNullOrEmpty(row[xlcol_AuxLib1].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@AuxLib1", AuxLib1Code);
                                Build_UpdateAudErrors(isUpdate, "AuxLib1", AuxLib1Code, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@AuxLib1", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_AuxLib2) && !StringIsNullOrEmpty(row[xlcol_AuxLib2].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@AuxLib2", AuxLib2Code);
                                Build_UpdateAudErrors(isUpdate, "AuxLib2", AuxLib2Code, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@AuxLib2", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_AuxLib3) && !StringIsNullOrEmpty(row[xlcol_AuxLib3].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@AuxLib3", AuxLib3Code);
                                Build_UpdateAudErrors(isUpdate, "AuxLib3", AuxLib3Code, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@AuxLib3", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_AuxLib4) && !StringIsNullOrEmpty(row[xlcol_AuxLib4].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@AuxLib4", AuxLib4Code);
                                Build_UpdateAudErrors(isUpdate, "AuxLib4", AuxLib4Code, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@AuxLib4", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_IntlJoiningDate) && !StringIsNullOrEmpty(row[xlcol_IntlJoiningDate].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@IntlJoiningDate", IntlJoiningDateValue.ToString("yyyy-MM-dd")).SqlDbType = SqlDbType.SmallDateTime;
                                Build_UpdateAudErrors(isUpdate, "IntlJoiningDate", IntlJoiningDateValue, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@IntlJoiningDate", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_PointOfHireE) && !StringIsNullOrEmpty(row[xlcol_PointOfHireE].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@PointOfHireE", PointOfHireECode);
                                Build_UpdateAudErrors(isUpdate, "PointOfHireE", PointOfHireECode, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@PointOfHireE", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_PointOfHireA) && !StringIsNullOrEmpty(row[xlcol_PointOfHireA].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@PointOfHireA", StrPointOfHireA);
                                Build_UpdateAudErrors(isUpdate, "PointOfHireA", StrPointOfHireA, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@PointOfHireA", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_SourceOfHire) && !StringIsNullOrEmpty(row[xlcol_SourceOfHire].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@SourceOfHire", SourceOfHireCode);
                                Build_UpdateAudErrors(isUpdate, "SourceOfHire", SourceOfHireCode, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@SourceOfHire", DBNull.Value);

                            if (CheckIfColumnExists(mydt, xlcol_SLReInitDate) && !StringIsNullOrEmpty(row[xlcol_SLReInitDate].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@SLReInitDate", SLReInitDateValue.ToString("yyyy-MM-dd")).SqlDbType = SqlDbType.SmallDateTime;
                                Build_UpdateAudErrors(isUpdate, "SLReInitDate", SLReInitDateValue, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@SLReInitDate", DBNull.Value);

                            if (CheckIfColumnExists(mydt, xlcol_StartDtofIndemnity) && !StringIsNullOrEmpty(row[xlcol_StartDtofIndemnity].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@StartDtofIndemnity", StartDtofIndemnityValue.ToString("yyyy-MM-dd")).SqlDbType = SqlDbType.SmallDateTime;
                                Build_UpdateAudErrors(isUpdate, "StartDtofIndemnity", StartDtofIndemnityValue, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@StartDtofIndemnity", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_LocLib5) && !StringIsNullOrEmpty(row[xlcol_LocLib5].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@LocLib5", LocationLevelCode);
                                Build_UpdateAudErrors(isUpdate, "LocLib5", LocationLevelCode, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@LocLib5", DBNull.Value);


                            //if (!StringIsNullOrEmpty(StrWrkAgreeNo))
                            //{
                            //    myCmd.Parameters.AddWithValue("@WrkAgreeNo", StrWrkAgreeNo);
                            //    Build_UpdateAudErrors(isUpdate,"WrkAgreeNo", StrWrkAgreeNo, ref strBuildUpdtAud, ref drowEmpOld);
                            //}
                            //else
                            myCmd.Parameters.AddWithValue("@WrkAgreeNo", DBNull.Value);



                            if (CheckIfColumnExists(mydt, xlcol_HealthInsurCmp) && !StringIsNullOrEmpty(row[xlcol_HealthInsurCmp].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@HealthInsurCmp", HealthInsurCmpCode);
                                Build_UpdateAudErrors(isUpdate, "HealthInsurCmp", HealthInsurCmpCode, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@HealthInsurCmp", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_LCIssuePlace) && !StringIsNullOrEmpty(row[xlcol_LCIssuePlace].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@LCIssuePlace", LCIssuePlaceCode);
                                Build_UpdateAudErrors(isUpdate, "LCIssuePlace", LCIssuePlaceCode, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@LCIssuePlace", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_FNameE) && !StringIsNullOrEmpty(row[xlcol_FNameE].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@FNameE", StrFNameE);
                                Build_UpdateAudErrors(isUpdate, "FNameE", StrFNameE, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@FNameE", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_FNameA) && !StringIsNullOrEmpty(row[xlcol_FNameA].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@FNameA", StrFNameA);
                                Build_UpdateAudErrors(isUpdate, "FNameA", StrFNameA, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else if (!string.IsNullOrEmpty(StrFNameE))
                            {
                                AddTranslatedParam(myCmd, "@FNameA", StrFNameE, StrFNameA, isUpdate, "FNameA", ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@FNameA", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_SNameE) && !StringIsNullOrEmpty(row[xlcol_SNameE].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@SNameE", StrSNameE);
                                Build_UpdateAudErrors(isUpdate, "SNameE", StrSNameE, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@SNameE", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_SNameA) && !StringIsNullOrEmpty(row[xlcol_SNameA].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@SNameA", StrSNameA);
                                Build_UpdateAudErrors(isUpdate, "SNameA", StrSNameA, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else if (!string.IsNullOrEmpty(StrSNameE))
                            {
                                AddTranslatedParam(myCmd, "@SNameA", StrSNameE, StrSNameA, isUpdate, "SNameA", ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@SNameA", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_GrandFatherE) && !StringIsNullOrEmpty(row[xlcol_GrandFatherE].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@GrandFatherE", StrGrandFatherE);
                                Build_UpdateAudErrors(isUpdate, "GrandFatherE", StrGrandFatherE, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@GrandFatherE", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_SponTypeExtnl) && !StringIsNullOrEmpty(row[xlcol_SponTypeExtnl].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@SponTypeExtnl", SponTypeExtnlCode);
                                Build_UpdateAudErrors(isUpdate, "SponTypeExtnl", SponTypeExtnlCode, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@SponTypeExtnl", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_GrandFatherA) && !StringIsNullOrEmpty(row[xlcol_GrandFatherA].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@GrandFatherA", StrGrandFatherA);
                                Build_UpdateAudErrors(isUpdate, "GrandFatherA", StrGrandFatherA, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else if (!string.IsNullOrEmpty(StrGrandFatherE))
                            {
                                AddTranslatedParam(myCmd, "@GrandFatherA", StrGrandFatherE, StrGrandFatherA, isUpdate, "GrandFatherA", ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@GrandFatherA", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_VisaExpDate) && !StringIsNullOrEmpty(row[xlcol_VisaExpDate].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@VisaExpDate", VisaExpDateValue.ToString("yyyy-MM-dd")).SqlDbType = SqlDbType.SmallDateTime;
                                Build_UpdateAudErrors(isUpdate, "VisaExpDate", VisaExpDateValue, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@VisaExpDate", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_AuxLib5) && !StringIsNullOrEmpty(row[xlcol_AuxLib5].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@AuxLib5", AuxLib5Code);
                                Build_UpdateAudErrors(isUpdate, "AuxLib5", AuxLib5Code, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@AuxLib5", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_AuxLib6) && !StringIsNullOrEmpty(row[xlcol_AuxLib6].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@AuxLib6", AuxLib6Code);
                                Build_UpdateAudErrors(isUpdate, "AuxLib6", AuxLib6Code, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@AuxLib6", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_PersEmail) && !StringIsNullOrEmpty(row[xlcol_PersEmail].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@PersEmail", StrPersEmail);
                                Build_UpdateAudErrors(isUpdate, "PersEmail", StrPersEmail, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@PersEmail", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_NPresentSec) && !StringIsNullOrEmpty(row[xlcol_NPresentSec].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@NPresentSec", NPresentSecCode);
                                Build_UpdateAudErrors(isUpdate, "NPresentSec", NPresentSecCode, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@NPresentSec", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_ReligionSubSet) && !StringIsNullOrEmpty(row[xlcol_ReligionSubSet].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@ReligionSubSet", ReligionSubSetCode);
                                Build_UpdateAudErrors(isUpdate, "ReligionSubSet", ReligionSubSetCode, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@ReligionSubSet", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_UIDNo) && !StringIsNullOrEmpty(row[xlcol_UIDNo].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@UIDNo", StrUIDNo);
                                Build_UpdateAudErrors(isUpdate, "UIDNo", StrUIDNo, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@UIDNo", DBNull.Value);

                            if (CheckIfColumnExists(mydt, xlcol_Disability) && !StringIsNullOrEmpty(row[xlcol_Disability].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@Disability", DisabilityCode);
                                Build_UpdateAudErrors(isUpdate, "Disability", DisabilityCode, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@Disability", DBNull.Value);

                            if (CheckIfColumnExists(mydt, xlcol_DisabilityDescE) && !StringIsNullOrEmpty(row[xlcol_DisabilityDescE].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@DisabilityDescE", StrDisabilityDescE);
                                Build_UpdateAudErrors(isUpdate, "DisabilityDescE", StrDisabilityDescE, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@DisabilityDescE", DBNull.Value);

                            if (CheckIfColumnExists(mydt, xlcol_DisabilityDescA) && !StringIsNullOrEmpty(row[xlcol_DisabilityDescA].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@DisabilityDescA", StrDisabilityDescA);
                                Build_UpdateAudErrors(isUpdate, "DisabilityDescA", StrDisabilityDescA, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else if (!string.IsNullOrEmpty(StrDisabilityDescE))
                            {
                                AddTranslatedParam(myCmd, "@DisabilityDescA", StrDisabilityDescE, StrDisabilityDescA, isUpdate, "DisabilityDescA", ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@DisabilityDescA", DBNull.Value);

                            //NationalID -- added by farook 21/NOV/2023
                            if (CheckIfColumnExists(mydt, xlcol_NationalID) && !StringIsNullOrEmpty(row[xlcol_NationalID].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@NationalID", StrNationalID);
                                Build_UpdateAudErrors(isUpdate, "NationalID", StrNationalID, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@NationalID", DBNull.Value);


                            if (!isUpdate && CheckIfColumnExists(mydt, xlcol_SalProfile) && !StringIsNullOrEmpty(row[xlcol_SalProfile].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@SalProfile", SalaryProfileCode);
                                Build_UpdateAudErrors(isUpdate, "SalProfile", SalaryProfileCode, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@SalProfile", DBNull.Value);


                            if (CheckIfColumnExists(mydt, xlcol_WPS) && !StringIsNullOrEmpty_AllowZero(row[xlcol_WPS].ToString()))
                            {
                                myCmd.Parameters.AddWithValue("@WPS", WPSCode);
                                Build_UpdateAudErrors(isUpdate, "WPS", WPSCode, ref strBuildUpdtAud, ref drowEmpOld);
                            }
                            else
                                myCmd.Parameters.AddWithValue("@WPS", DBNull.Value);

                            //Audit Mesaage parameters



                            //if (!string.IsNullOrEmpty(strBuildrAudit.ToString()))
                            //{
                            //    strBuildrAuditText = strBuildrAudit.ToString();
                            //}
                            //myCmd.Parameters.AddWithValue("@AudErrors", strBuildrAuditText.ToString());


                            if (!string.IsNullOrEmpty(strBuildUpdtAud.ToString()))
                            {
                                strBuildrUpdtAudText = "Data Changed for [EmpCode = " + drowEmpOld["EmpCode"].ToString() + " And EmpID = " + drowEmpOld["EmpID"].ToString() + " : ";
                                strBuildrUpdtAudText += strBuildUpdtAud.ToString();

                                if (isUpdate == false)
                                {
                                    strBuildrAuditText = "Data Created for [EmpCode = " + drowEmpOld["EmpCode"].ToString() + "] : ";
                                    strBuildrAuditText += strBuildUpdtAud.ToString();
                                }
                            }
                            myCmd.Parameters.AddWithValue("@UpdtAudErrors", strBuildrUpdtAudText.ToString());
                            myCmd.Parameters.AddWithValue("@AudErrors", strBuildrAuditText);



                            myCmd.Parameters.Add("@EmpID_Out", SqlDbType.Int).Direction = ParameterDirection.Output;

                            #endregion

                            int k = myCmd.ExecuteNonQuery();

                            nEmpID = Convert.ToInt32(myCmd.Parameters["@EmpID_Out"].Value);



                            if (nEmpID > 0)
                            {
                                bEmpSaved = true;
                            }



                            #endregion




                            SqlTran.Commit();
                            //SqlTran.Rollback();//testing

                            SkippedRecords -= 1;

                            if (strEditMode != "EDIT")
                                InsertedRecords += 1;
                            else
                                UpdatedRecords += 1;
                        }
                        catch (Exception ex)
                        {

                            if ((myConn != null))
                            {
                                if (myConn.State != ConnectionState.Closed)
                                {
                                    SqlTran.Rollback();
                                }
                            }

                            AppendLineError(nRowNo, rowErrInfo, "An Error Occurred while Saving the Employee Record : " + ex.Message);

                        }
                        finally
                        {
                            if ((myConn != null))
                            {
                                if (myConn.State != ConnectionState.Closed)
                                {
                                    myConn.Close();
                                }
                            }
                        }



                        try
                        {
                            WorkAgreementImportBL importToCSWrkAgr = new WorkAgreementImportBL();
                            importToCSWrkAgr.SaveWorkAgreementFromStaging(mydt_staging);
                        }
                        catch (Exception ex_wrk)
                        {
                            AppendLineError(nRowNo, "Work Agreement Save Error", "An Error Occurred while Saving the Employee Contract Details : " + ex_wrk.Message);

                        }


                    skipEmpSaveStep:;


                    #endregion

                    skipInsertUpdateStep:;


                        if (bEmpSaved == false)
                        {
                            sbSaveMsg.Append("Employee Saving failed. Please check the error log for details.");
                        }
                        else
                        {
                            if (errCount == 0)
                                sbSaveMsg.Append("Employee Record successfully " + (isUpdate ? "Updated" : "Created"));
                            else
                                sbSaveMsg.Append("Employee Record " + (isUpdate ? "Updated" : "Created") +" but with errors.");
                            
                            Update_LASTUPDDTTM(StrEmpCode, DateTime.Now, "employee");

                        }




                    }
                    catch (Exception ex)
                    {
                        string errorMessage = $"Error processing row with EmpCode {row["EmpCode"]}: {ex.Message}";

                        AppendLineError(nRowNo, "", errorMessage);

                        Common.LogAction(errorMessage);
                        Common.LogException(ex);
                    }
                    finally
                    {

                        if (errCount > 0)
                        {
                            bCurrHasLineErrors = true;

                            string strFullErrorText = sbLineErrMsg.ToString().TrimEnd('\r', '\n');
                            //Common.LogErrorToSFIErrorLog(importfileName, iCurrRowNo, strCurrEmpCode, strFullErrorText, "Employee");
                        }

                        UpdateProcessLogDetails();

                    }

                    #region Document Save

                    

                    // Get EmpID from the employee table
                    

                    if (nEmpID==0)
                        continue; // skip if employee not found

                    foreach (DataRow stgRow in mydt_staging.Rows)
                    {
                        foreach (DataColumn col in mydt_staging.Columns)
                        {
                            if (col.ColumnName.StartsWith("doc_", StringComparison.OrdinalIgnoreCase))
                            {
                                try
                                {
                                    //string base64 = mydt_staging.Rows[iCurrRowNo][col.ColumnName]?.ToString();
                                    string base64 = stgRow[col.ColumnName]?.ToString();

                                    if (!string.IsNullOrEmpty(base64))
                                    {
                                        string docCode = GetDocCode(col.ColumnName);
                                        if (string.IsNullOrEmpty(docCode))
                                            continue;

                                        string contentType = ".pdf"; // default

                                        // Remove data prefix if present
                                        if (base64.Contains(","))
                                        {
                                            var prefix = base64.Substring(0, base64.IndexOf(","));
                                            base64 = base64.Substring(base64.IndexOf(",") + 1);

                                            if (prefix.Contains("image/jpeg")) contentType = ".jpg";
                                            else if (prefix.Contains("image/png")) contentType = ".png";
                                            else if (prefix.Contains("application/pdf")) contentType = ".pdf";
                                        }

                                        byte[] fileBytes = Convert.FromBase64String(base64);

                                        // Detect file type from signature if prefix not available
                                        contentType = GetFileExtension(fileBytes);

                                        // VALIDATION STEP
                                        string validationMsg;
                                        //if (!ValidateDocument(docCode, fileBytes, contentType, out validationMsg))
                                        //{
                                        //    AppendLineError(nRowNo, $"Validation failed {col.ColumnName}", validationMsg);
                                        //    continue; 
                                        //}

                                        // Save document with content type
                                        SaveDocument(nEmpID.ToString(), docCode, fileBytes, contentType);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    AppendLineError(nRowNo, "Insert/Update Document block " + col.ColumnName, "Document Insert/Update failed. Details: " + ex.Message);
                                    Common.LogAction("Document Insert/Update failed. Details : " + ex.Message);
                                    Common.LogException(ex);

                                }
                            }
                        }

                    }


                    #endregion
                   
                   
                }

                if (errTotalCount > 0)
                {
                    ErrorFileHeader += "Total Records:" + nRowNo + " , Inserted : " + InsertedRecords + " , Updated : " + UpdatedRecords + " , Skipped : " + SkippedRecords + Environment.NewLine;
                    ErrorFileHeader += "---------------------------------------------------------------------------------------------" + Environment.NewLine + Environment.NewLine;
                    ErrorFileStr = ErrorFileHeader + ErrorFileStr;

                }


                if (SkippedRecords == 0)
                {
                    if (nRowNo == 1)
                    {
                        if (errTotalCount == 0)
                        {
                             finishMessage = "Employee Record successfully " + (UpdatedRecords == 1 ? "Updated" : "Created");
                        }
                        else
                        {                           
                            finishMessage = "Employee Record " + (UpdatedRecords == 1 ? "Updated" : "Created") + " but with errors."; 
                        }

                    }
                    else
                    {
                        finishMessage = "Employee Records successfully Imported." + "</br>" + " Total Records:" + nRowNo + " , Inserted : " + InsertedRecords + " , Updated : " + UpdatedRecords + " , Skipped : " + SkippedRecords;
                    }
                }
                else
                {
                    if (nRowNo == 1)
                    {
                         finishMessage = "Employee Saving failed. Please check the error log for details.";
                    }
                    else
                    {

                        finishMessage = "Some records could not imported. Please check the error log for details." + "</br>" + " Total Records:" + nRowNo + " , Inserted : " + InsertedRecords + " , Updated : " + UpdatedRecords + " , Skipped : " + SkippedRecords;
                    }

                }

                //For Progress bar
                FinishProgress(finishMessage);
                //End:For Progress bar

                //setting PRCOESS variables
                strprocessRemarks = finishMessage;
                if (SkippedRecords != 0 || errTotalCount>0)
                    hasProcessError = true;

            }
            catch (Exception ex)
            {
                Common.LogAction($"An error occurred: {ex.Message}");
                Common.LogException(ex);

                hasProcessError = true;

                throw ex;
            }
        }

        private string GetFileExtension(byte[] fileBytes)
        {
            if (fileBytes.Length < 4) return ".bin";

            // PDF: %PDF
            if (fileBytes[0] == 0x25 && fileBytes[1] == 0x50 &&
                fileBytes[2] == 0x44 && fileBytes[3] == 0x46) return ".pdf";

            // JPEG: FF D8 FF
            if (fileBytes[0] == 0xFF && fileBytes[1] == 0xD8 &&
                fileBytes[2] == 0xFF) return ".jpg";

            // PNG: 89 50 4E 47
            if (fileBytes[0] == 0x89 && fileBytes[1] == 0x50 &&
                fileBytes[2] == 0x4E && fileBytes[3] == 0x47) return ".png";

            return ".bin"; // unknown
        }

        private void CreateEmployeeImportDataTable()
        {
            mydt = new DataTable();

            mydt.Columns.Add(new DataColumn("RowNo", System.Type.GetType("System.Int32")));
            mydt.Columns.Add(new DataColumn(xlcol_EmpCode, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_Title, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_EmpNameE, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_EmpNameA, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_NickNameE, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_NickNameA, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_RelType, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_RelNameE, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_RelNameA, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_MotherNameE, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_MotherNameA, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_FamilyNameE, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_FamilyNameA, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_Sex, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_NPresent, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_NPrevious, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_MaritalStat, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_DateOfBirth, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_BirthPlaceE, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_BirthPlaceA, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_CountryOfBirth, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_PassportNoE, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_PassportNoA, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_PCategory, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_PIssuePlaceE, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_PIssuePlaceA, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_PIssueCountry, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_PIssueDate, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_PExpiryDate, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_Religion, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_PassportProf, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_Education, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_PerAddressE, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_PerAddressA, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_Skill1, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_Skill2, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_Skill3, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_Language1, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_Language2, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_Language3, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_VisaType, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_VisaNo, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_VisaIssueDate, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_ImmigFileNo, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_EntryPlace, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_EntryDate, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_ResidenceNo, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_ResIssueDate, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_ResExpDate, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_ResIssuePlace, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_LabCardNo, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_LCIssueDate, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_LCExpDate, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_HlthCardNo, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_HCIssuePlace, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_HCIssueDate, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_HCExpiryDate, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_DrvLicNo, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_DLCategory, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_DLIssuePlace, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_DLIssueDate, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_DLExpiryDate, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_SponsorCode, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_SponByOther, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_OSponNameE, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_OSponNameA, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_OSponRel, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_OSponNation, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_OSponVisaNo, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_OSponVExpDt, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_OSponPsprtNoE, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_OSponPsprtNoA, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_ExperienceE, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_ExperienceA, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_Emirates, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_City, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_Area, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_StreetE, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_StreetA, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_BuildingE, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_BuildingA, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_FlatE, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_FlatA, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_OffPhoneNo, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_Ext, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_ResPhoneNo, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_POBox, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_MobileNo, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_PagerNo, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_TeleNoAbroad, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_Email, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_BloodGroup, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_FaxNo, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_NextofKinE, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_NextofKinA, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_NextofKinAddrE, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_NextofKinAddrA, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_AddressE, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_AddressA, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_AuxString1, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_AuxString2, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_AuxString3, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_AuxString4, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_AuxString5, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_AuxString6, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_AuxString7, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_AuxString8, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_AuxString9, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_AuxString10, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_AuxAString1, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_AuxAString2, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_AuxAString3, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_AuxAString4, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_AuxAString5, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_AuxAString6, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_AuxAString7, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_AuxAString8, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_AuxAString9, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_AuxAString10, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_AuxInt1, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_AuxInt2, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_AuxInt3, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_AuxCurrency1, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_AuxCurrency2, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_AuxDate1, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_AuxDate2, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_AuxDate3, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_AuxDate4, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_AuxDate5, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_AuxLib1, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_AuxLib2, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_AuxLib3, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_AuxLib4, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_IntlJoiningDate, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_PointOfHireE, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_PointOfHireA, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_SourceOfHire, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_SLReInitDate, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_StartDtofIndemnity, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_LocLib5, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_SalProfile, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_HealthInsurCmp, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_LCIssuePlace, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_FNameE, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_FNameA, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_SNameE, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_SNameA, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_GrandFatherE, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_SponTypeExtnl, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_GrandFatherA, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_VisaExpDate, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_AuxLib5, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_AuxLib6, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_PersEmail, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_NPresentSec, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_ReligionSubSet, System.Type.GetType("System.String")));
            mydt.Columns.Add(new DataColumn(xlcol_UIDNo, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_Disability, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_DisabilityDescE, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_DisabilityDescA, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_NationalID, System.Type.GetType("System.String"))); //NationalID -- added by farook 21/NOV/2023
            //mydt.Columns.Add(new DataColumn(xlcol_WPS, System.Type.GetType("System.String"))); //NationalID -- added by farook 21/NOV/2023

            ////finmaster columns
            //mydt.Columns.Add(new DataColumn(xlcol_BSalaryAmt, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_FoodAmt, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_HRAAmt, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_TranAmt, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_AuxAll2Amt, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_JobTitle, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_JoiningDate, System.Type.GetType("System.String")));


            //mydt.Columns.Add(new DataColumn(xlcol_CategMast, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_SalGrade, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_WrkAgreeType, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_WrkAgrStartDt, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_WrkAgrExpDt, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_ProbationPeriod, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_NoticeDayByEmp, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_NoticeDayByComp, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_BSalaryCurr, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_AuxAll1Amt, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_AuxAll3Amt, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_AuxAll4Amt, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_AuxAll5Amt, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_AuxAll6Amt, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_AuxAll7Amt, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_AuxAll8Amt, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_ALCode, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_ETicketEvery, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_FTicketYN, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_FTicketEvery, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_NoOfFullTickets, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_NoOfChildTickets, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_NoOfInfantTickets, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_RouteEmp, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_RouteFam, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_LastPaidDate, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_ALBal, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_NonServDays, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_ExtraBal, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_SLTaken, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_PymntMode, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_BankName1, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_BankAcctNo1, System.Type.GetType("System.String")));
            //mydt.Columns.Add(new DataColumn(xlcol_TktDueDate, System.Type.GetType("System.String")));
        }

  
        private void MapStagingDataToImportTable(DataTable mydt_staging, ref DataTable mydt)
        {


            foreach (DataRow drow in mydt_staging.Rows)
            {
                DataRow drownew=mydt.NewRow();

                drownew["RowNo"] = drow["RowNo"];

                drownew[xlcol_EmpCode] = drow["EmployeeID"];
                drownew[xlcol_LocLib5] = drow["WorkingCompany"];
                drownew[xlcol_Title] = drow["EmployeeTitle"];
                drownew[xlcol_EmpNameE] = drow["FullNameE"];
                drownew[xlcol_FNameE] = drow["FirstName"];
                drownew[xlcol_SNameE] = drow["MiddleName"];
                drownew[xlcol_NickNameE] = drow["ThirdName"];
                drownew[xlcol_GrandFatherE] = drow["FourthName"];
                drownew[xlcol_FamilyNameE] = drow["FamilyName"];
                drownew[xlcol_MotherNameE] = drow["MotherNameE"];
                drownew[xlcol_Sex] = drow["Gender"];
                drownew[xlcol_Religion] = drow["Religion"];
                drownew[xlcol_ReligionSubSet] = drow["Faith"];
                drownew[xlcol_DateOfBirth] = drow["DateOfBirth"];
                drownew[xlcol_MaritalStat] = drow["MaritalStatus"];
                drownew[xlcol_CountryOfBirth] = drow["CountryOfBirth"];
                drownew[xlcol_BirthPlaceE] = drow["BirthPlaceE"];
                drownew[xlcol_PassportNoE] = drow["PassportNo"];
                drownew[xlcol_PCategory] = drow["PassportCategory"];
                drownew[xlcol_PIssueDate] = drow["PassportIssueDate"];
                drownew[xlcol_PExpiryDate] = drow["PassportExpiryDate"];
                drownew[xlcol_PIssueCountry] = drow["PassportIssueCountry"];
                drownew[xlcol_PIssuePlaceE] = drow["PassportIssuePlace"];
                drownew[xlcol_NPresent] = drow["PresentNationality"];
                drownew[xlcol_NPrevious] = drow["PreviousNationality"];
                drownew[xlcol_PassportProf] = drow["MOHREVISAProfession"];
                drownew[xlcol_Education] = drow["VisaQualification"];
                drownew[xlcol_Language1] = drow["Language1"];
                drownew[xlcol_Language2] = drow["Language2"];
                drownew[xlcol_Language3] = drow["Language3"];
                drownew[xlcol_AuxLib1] = drow["EduCertissuedFrom"];
                drownew[xlcol_AuxString3] = drow["MOFAAttestationNo"];
                drownew[xlcol_AuxString4] = drow["MOFAAttestationLabel"];
                drownew[xlcol_AuxString5] = drow["CertAttestationNo"];
                drownew[xlcol_UIDNo] = drow["UnifiedIdentityNumber"];
                drownew[xlcol_Emirates] = drow["EmirateState"];
                drownew[xlcol_Area] = drow["Area"];
                drownew[xlcol_City] = drow["City"];
                drownew[xlcol_BuildingE] = drow["Building"];
                drownew[xlcol_StreetE] = drow["Street"];
                drownew[xlcol_FlatE] = drow["FlatNo"];
                drownew[xlcol_POBox] = drow["POBox"];
                drownew[xlcol_OffPhoneNo] = drow["OfficeTelNo"];
                drownew[xlcol_ResPhoneNo] = drow["LandlineNo"];
                drownew[xlcol_MobileNo] = drow["MobileNo"];
                drownew[xlcol_TeleNoAbroad] = drow["TeleNoAbroad"];
                drownew[xlcol_PersEmail] = drow["PersonalEmail"];
                drownew[xlcol_AddressE] = drow["Address"];
                drownew[xlcol_PerAddressE] = drow["AddressAbroad"];
                drownew[xlcol_Email] = drow["Email"];
                drownew[xlcol_SponsorCode] = drow["Sponsor"];
                //drownew[xlcol_CandidateLocationCurrently] = drow["CandidateLocationCurrently"];
                //drownew[xlcol_NoticePeriod] = drow["NoticePeriod"];
                //drownew[xlcol_Probation] = drow["Probation"];
                //drownew[xlcol_WeeklyHolidays] = drow["WeeklyHolidays"];
                //drownew[xlcol_WorkType] = drow["WorkType"];
                //drownew[xlcol_Remuneration] = drow["Remuneration"];
                //drownew[xlcol_BasicSalary] = drow["BasicSalary"];
                //drownew[xlcol_HousingAmount] = drow["HousingAmount"];
                //drownew[xlcol_TransportingAmount] = drow["TransportingAmount"];
                //drownew[xlcol_FoodAllowance] = drow["FoodAllowance"];
                //drownew[xlcol_MobileConnectivityAllowance] = drow["MobileConnectivityAllowance"];
                //drownew[xlcol_CostOfLivingAllowance] = drow["CostOfLivingAllowance"];
                //drownew[xlcol_OtherAllowance] = drow["OtherAllowance"];
                //drownew[xlcol_EmployeeStatus] = drow["EmployeeStatus"];
                drownew[xlcol_AuxLib3] = drow["HRJobTitle"];
                drownew[xlcol_AuxString1] = drow["UniversityName"];
                drownew[xlcol_AuxString2] = drow["Faculty"];
                drownew[xlcol_AuxString6] = drow["StudyMajors"];
                drownew[xlcol_AuxLib2] = drow["DegreeType"];
                drownew[xlcol_AuxDate1] = drow["DegreeStartDate"];
                drownew[xlcol_AuxDate2] = drow["DegreeEndDate"];
                drownew[xlcol_AuxInt1] = drow["GraduationYear"];
                drownew[xlcol_AuxInt2] = drow["ActualYearsofDegree"];
                drownew[xlcol_IntlJoiningDate] = drow["JoiningDate"];
                //drownew[xlcol_Doc_WBPhoto] = drow["Doc_WBPhoto"];
                //drownew[xlcol_Doc_Pass1] = drow["Doc_Pass1"];
                //drownew[xlcol_Doc_Pass2] = drow["Doc_Pass2"];
                //drownew[xlcol_Doc_ECP1] = drow["Doc_ECP1"];
                //drownew[xlcol_Doc_ECP2] = drow["Doc_ECP2"];
                //drownew[xlcol_Doc_CTR] = drow["Doc_CTR"];

                mydt.Rows.Add(drownew);
            }

            //EmployeeID
            //WorkingCompany
            //EmployeeTitle
            //FullNameE
            //FirstName
            //MiddleName
            //ThirdName
            //FourthName
            //FamilyName
            //MotherNameE
            //Gender
            //Religion
            //Faith
            //DateOfBirth
            //MaritalStatus
            //CountryOfBirth
            //BirthPlaceE
            //PassportNo
            //PassportCategory
            //PassportIssueDate
            //PassportExpiryDate
            //PassportIssueCountry
            //PassportIssuePlace
            //PresentNationality
            //PreviousNationality
            //MOHREVISAProfession
            //VisaQualification
            //Language1
            //Language2
            //Language3
            //EduCertissuedFrom
            //MOFAAttestationNo
            //MOFAAttestationLabel
            //CertAttestationNo
            //UnifiedIdentityNumber
            //EmirateState
            //Area
            //City
            //Building
            //Street
            //FlatNo
            //POBox
            //OfficeTelNo
            //LandlineNo
            //MobileNo
            //TeleNoAbroad
            //PersonalEmail
            //Address
            //AddressAbroad
            //Email
            //Sponsor
            //CandidateLocationCurrently
            //NoticePeriod
            //Probation
            //WeeklyHolidays
            //WorkType
            //Remuneration
            //BasicSalary
            //HousingAmount
            //TransportingAmount
            //FoodAllowance
            //MobileConnectivityAllowance
            //CostOfLivingAllowance
            //OtherAllowance
            //EmployeeStatus
            //HRJobTitle
            //UniversityName
            //Faculty
            //StudyMajors
            //DegreeType
            //DegreeStartDate
            //DegreeEndDate
            //GraduationYear
            //ActualYearsofDegree
            //JoiningDate
            //Doc_WBPhoto
            //Doc_Pass1
            //Doc_Pass2
            //Doc_ECP1
            //Doc_ECP2
            //Doc_CTR
        }
        private string GetDocCode(string columnName)
        {
            switch (columnName)
            {
                case "Doc_WBPhoto":
                    return "31";

                case "Doc_Pass1":
                    return "29";

                case "Doc_Pass2":
                    return "30";

                case "Doc_ECP1":
                    return "ECP1";

                case "Doc_ECP2":
                    return "68";
                case "Doc_CTR":
                    return "CTR";

                default:
                    return "";
            }
        }

        private void SaveDocument(string empId, string docCode, byte[] fileBytes, string contentType)
        {
            string conStr = ConnectionFunctions.GetConnectionString();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                using (SqlCommand cmd = new SqlCommand("SP_SaveEmployeeDocument", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@EmpID", SqlDbType.Int).Value = Convert.ToInt32(empId);
                    cmd.Parameters.Add("@DocCode", SqlDbType.VarChar, 20).Value = docCode;
                    cmd.Parameters.Add("@Datas", SqlDbType.VarBinary).Value = fileBytes;
                    cmd.Parameters.Add("@ContentType", SqlDbType.VarChar, 10).Value = contentType;

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private bool CheckIfMandatory(string xlcolName, enmXlImportTables tableindex)
        {
            bool ismandatory = false;

            switch (tableindex)
            {
                case enmXlImportTables.Employee:
                    ismandatory = MandatoryEmployeeCols.Contains(xlcolName) || SystemMandatoryEmployeeCols.Contains(xlcolName);
                    break;
            }

            return ismandatory;
        }




        #region ProgressBar Functions
        private void StartProgress(string progressTitle = "")
        {
            //objprog.name = "";
            //if (!String.IsNullOrEmpty(progressTitle))
            //    objprog.Title = progressTitle;
            //objprog.percn = 9999;
            //objprog.Progress = "Started";
            //Session["Progressbar"] = objprog;

            //Session["EmplImpErrFile"] = "";
            //Session["EmployeeImpErrorYN"] = "";
        }
        private bool IsProgressStopped()
        {
            //if (objprog.Progress == "Stop")
            //    return true;
            //else
            //    return false;

            return false;
        }
        private void SetProgressStartVariables()
        {
            //objprog.Starttime = DateTime.Now;
            //Session["Progressbar"] = objprog;
        }
        private void UpdateProgress( string progressEmpname, int nprccedd, int totcnt, double percn, string strnoofemp)
        {
            ////TO DISPLAY THE NAME OF THE EMPLOYEE IN THE SPLASH SCREEN
            //objprog.name = progressEmpname;

            //if (percn < 1 && percn > 0)
            //{
            //    objprog.percn = 1;
            //}
            //else
            //{
            //    objprog.percn = Convert.ToInt32(percn);
            //}

            //objprog.noofemp = strnoofemp;

            ////time statistics calc denson added 20042015
            //DateTime CurrTime = DateTime.Now;
            //if (nprccedd > 1)
            //{
            //    TimeSpan TimeFor1 = (CurrTime - objprog.Starttime);
            //    double secforone = TimeFor1.TotalMilliseconds;
            //    secforone = (secforone / nprccedd);
            //    double multifactor = totcnt - nprccedd + 1;
            //    double sectoadd = secforone * multifactor;
            //    objprog.Exptime = CurrTime.AddMilliseconds(sectoadd);
            //    objprog.Remtime = objprog.Exptime - CurrTime;
            //}

            //Session["Progressbar"] = objprog;
        }
        private void SetProgressFinalErrors(string strErrFile, string ErrorYN)
        {
            //Session["EmplImpErrFile"] = strErrFile;
            //Session["EmployeeImpErrorYN"] = ErrorYN;
        }
        private void FinishProgress(string finishMessage = "")
        {
            //if (!String.IsNullOrEmpty(finishMessage))
            //    objprog.Xmsg = finishMessage;
            //objprog.percn = 101;
            //objprog.Progress = "finished";
            //Session["Progressbar"] = objprog;
        }
        private void SetProgressError(string Errmsg)
        {
            //objprog.errmsg = Errmsg;
            //objprog.percn = -1;
            //Session["Progressbar"] = objprog;
        }

        #endregion  










        private object smallDateTime(object value)
        {
            if (value == null || value == DBNull.Value || string.IsNullOrWhiteSpace(value.ToString()))
                return DBNull.Value;

            DateTime dateValue;
            if (DateTime.TryParse(value.ToString(), out dateValue))
            {
                // Ensure the date is within SQL smalldatetime range
                if (dateValue < new DateTime(1900, 1, 1) || dateValue > new DateTime(2079, 6, 6))
                    return DBNull.Value;
                return dateValue;
            }

            return DBNull.Value;
        }

        private string TruncateValue(object value, int maxLength)
        {
            if (value == null || value == DBNull.Value)
                return null;

            var strValue = value.ToString();
            return strValue.Length > maxLength ? strValue.Substring(0, maxLength) : strValue;
        }





        private Dictionary<string, string> GetTitleNames()
        {
            //dictionry key is case sensitive

            string squery = "SELECT NameE,NameA,FieldPrefixE,FieldPrefixA FROM dbo.AuxFields  WITH (NOLOCK) WHERE [View] in (1)";
            string ErrMsg = "";
            DataTable dt = new DataTable();
            SqlConnection con = new SqlConnection(ConnectionFunctions.GetConnectionString());
            if (!ConnectionFunctions.Connect_SQLDataTable(ref dt, squery, ref con, ref ErrMsg))
            {
                throw new Exception(ErrMsg);
            }
            var comparer = StringComparer.OrdinalIgnoreCase;
            Dictionary<string, string> EngTitles = new Dictionary<string, string>(comparer);

            DataRow[] drows = null;
            string strTitle = "";
            foreach (DataColumn dcol in mydt.Columns)
            {
                strTitle = "";

                //if (Common.buseColNameAsTitleName == false)
                //{
                //    //if (EmployeeCols.Contains(dcol.ColumnName, StringComparer.OrdinalIgnoreCase))
                //    //{
                //        drows = dt.Select("FieldPrefixE='" + dcol.ColumnName + "' OR FieldPrefixA='" + dcol.ColumnName + "'");
                //        if (drows != null && drows.Length > 0)
                //        {
                //            strTitle = drows[0]["NameE"].ToString();
                //        }

                //    //}
                //}

                if (string.IsNullOrEmpty(strTitle))
                {
                    strTitle = dcol.ColumnName;
                }
                else
                {
                    strTitle = strTitle + " (" + dcol.ColumnName + ")";
                }

                strTitle = GetValidString(strTitle);

                EngTitles.Add(dcol.ColumnName, strTitle);
            }


            return EngTitles;
        }


        /// <summary>
        /// Gets the rows from ExtDDFEng table for the specified Tablename
        /// </summary>
        /// <param name="tableindx"></param>
        /// <returns></returns>
        private DataTable GetLookupFieldDetails(enmXlImportTables tableindx)
        {
            DataTable dt = new DataTable();
            string tableName = lstXlImportTables[Convert.ToInt32(tableindx)];


            string squery = "select FieldPrefix,FieldType,LookTableName,DisplayFieldPrefix,LinkingFieldPrefix from ExtDDFEng  WITH (NOLOCK) where TableName = '" + tableName + "' and FieldType in (2,3,4,5)";
            string ErrMsg = "";

            SqlConnection con = new SqlConnection(ConnectionFunctions.GetConnectionString());
            if (!ConnectionFunctions.Connect_SQLDataTable(ref dt, squery, ref con, ref ErrMsg))
            {
                throw new Exception(ErrMsg);
            }

            return dt;

        }


        public string GetValidString(object inputObj)
        {
            string rsltstring = "";
            if (inputObj == null)
                return "";

            rsltstring = inputObj.ToString().Replace("\r\n", "");
            rsltstring = rsltstring.Replace("\n", "");
            rsltstring = rsltstring.Replace("\t", "");
            rsltstring = rsltstring.Trim();
            return rsltstring;

        }







        private bool GetEmployeeData(string strEmpCode, ref DataTable dt, ref string ErrMsg)
        {


            string sQuery = "Select * FROM Employee  WITH (NOLOCK) WHERE EmpCode = @EmpCode";

            SqlParameter[] Params = new SqlParameter[1];
            Params[0] = new SqlParameter("@EmpCode", SqlDbType.VarChar);
            Params[0].Value = strEmpCode;


            SqlConnection con = new SqlConnection(ConnectionFunctions.GetConnectionString());
            if (!ConnectionFunctions.Connect_SQLDataTable(ref dt, sQuery, ref con, Params, ref ErrMsg))
            {
                ErrMsg = "Error Occurred while retrieving the Employee Details from Database";
                return false;
            }
            if (dt.Rows.Count == 0)
            {
                ErrMsg = "Employee EmpCode not found in the Database";
                return false;
            }

            return true;

        }

        private bool GetEmployeeFinMastData(string strEmpCode, ref DataTable dt, ref string ErrMsg)
        {


            string sQuery = "Select TOp(1) * FROM FinMast  WITH (NOLOCK) WHERE EmpId = (Select Top(1) EmpID from Employee where EmpCode=@EmpCode)";

            //if (IsDataMigrationEnabled == true)
            //{
            //    sQuery = "Select TOp(1) * FROM FinMast  WITH (NOLOCK) WHERE EmpId = (Select Top(1) EmpID from Employee where EmpCode=@EmpCode) and [status] = 20";
            //}

            SqlParameter[] Params = new SqlParameter[1];
            Params[0] = new SqlParameter("@EmpCode", SqlDbType.VarChar);
            Params[0].Value = strEmpCode;


            SqlConnection con = new SqlConnection(ConnectionFunctions.GetConnectionString());
            if (!ConnectionFunctions.Connect_SQLDataTable(ref dt, sQuery, ref con, Params, ref ErrMsg))
            {
                ErrMsg = "Error Occurred while retrieving the Employee Financial Details from Database";
                return false;
            }

            return true;

        }


        /// <summary>
        /// Gets the full row of the Lookuptable having the specified description
        /// <br>Corresponding dtLookUpFieldsDetails must be filled before calling this function</br>
        /// </summary>
        /// <param name="table"></param>
        /// <param name="FieldName"></param>
        /// <param name="value"></param>
        /// <param name="dt"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        private bool GetLookupTable(enmXlImportTables table, string FieldName, string value, ref DataTable dt, ref string ErrMsg)
        {
            DataTable dtLookUpFD = new DataTable();

            switch (table)
            {
                case enmXlImportTables.Employee:
                    dtLookUpFD = dtLookUpFieldsDetails_Emp;
                    break;
                //case enmXlImportTables.FinMast:
                //    dtLookUpFD = dtLookUpFieldsDetails_FinMast;
                //    break;
                //case enmXlImportTables.PayDetails:
                //    dtLookUpFD = dtLookUpFieldsDetails_PayDetails;
                //    break;
                //case enmXlImportTables.WrkAgrmntDet:
                //    dtLookUpFD = dtLookUpFieldsDetails_WrkAgrmntDet;
                //    break;

            }

            DataRow[] drows = dtLookUpFD.Select("FieldPrefix='" + FieldName + "'");
            if (drows == null || drows.Length == 0)
            {
                ErrMsg = "Error Occurred while retrieving the " + dictTitleNames[FieldName] + " Details from Database"; ;
                return false;
            }
            string sQuery = "";
            string lookupDispcolName = "";
            string lookupcodecolName = "";
            string lookuptable = "";

            if (Convert.ToInt32(drows[0]["FieldType"]) == 4 || Convert.ToInt32(drows[0]["FieldType"]) == 5)
            {
                //fixed lookup
                lookupDispcolName = "DescE";
                lookupcodecolName = "Code";
                lookuptable = "FixedLookup";
                string lookupname = drows[0]["LookTableName"].ToString();

                sQuery = "SELECT TOP(1) " + lookupcodecolName + "," + lookupDispcolName + ",* from " + lookuptable + " with (nolock) where " + lookupDispcolName + " = @DisplayFieldValue and LookupName='" + lookupname + "'";
            }
            else
            {

                lookupDispcolName = drows[0]["DisplayFieldPrefix"].ToString();
                lookupcodecolName = drows[0]["LinkingFieldPrefix"].ToString();
                lookuptable = drows[0]["LookTableName"].ToString();

                sQuery = "SELECT TOP(1) " + lookupcodecolName + "," + lookupDispcolName + ",* from " + lookuptable + " with (nolock) where " + lookupDispcolName + " = @DisplayFieldValue";

            }


            SqlParameter[] Params = new SqlParameter[1];
            Params[0] = new SqlParameter("@DisplayFieldValue", SqlDbType.VarChar);
            Params[0].Value = value.ToString();



            SqlConnection con = new SqlConnection(ConnectionFunctions.GetConnectionString());
            if (!ConnectionFunctions.Connect_SQLDataTable(ref dt, sQuery, ref con, Params, ref ErrMsg))
            {
                ErrMsg = "Error Occurred while retrieving the " + dictTitleNames[FieldName] + " Details from Database";
                return false;
            }
            if (dt.Rows.Count == 0)
            {
                ErrMsg = dictTitleNames[FieldName] + "which you have entered is not found in the Database";
                return false;
            }

            return true;

        }

        private bool GetLookupTableByCode(enmXlImportTables table, string FieldName, string value, ref DataTable dt, ref string ErrMsg)
        {
            DataTable dtLookUpFD = new DataTable();

            switch (table)
            {
                case enmXlImportTables.Employee:
                    dtLookUpFD = dtLookUpFieldsDetails_Emp;
                    break;
                    //case enmXlImportTables.FinMast:
                    //    dtLookUpFD = dtLookUpFieldsDetails_FinMast;
                    //    break;
                    //case enmXlImportTables.PayDetails:
                    //    dtLookUpFD = dtLookUpFieldsDetails_PayDetails;
                    //    break;
                    //case enmXlImportTables.WrkAgrmntDet:
                    //    dtLookUpFD = dtLookUpFieldsDetails_WrkAgrmntDet;
                    //    break;

            }

            DataRow[] drows = dtLookUpFD.Select("FieldPrefix='" + FieldName + "'");
            if (drows == null || drows.Length == 0)
            {
                ErrMsg = "Error Occurred while retrieving the " + dictTitleNames[FieldName] + " Details from Database"; ;
                return false;
            }
            string sQuery = "";
            string lookupDispcolName = "";
            string lookupcodecolName = "";
            string lookuptable = "";

            if (Convert.ToInt32(drows[0]["FieldType"]) == 4 || Convert.ToInt32(drows[0]["FieldType"]) == 5)
            {
                //fixed lookup
                lookupDispcolName = "DescE";
                lookupcodecolName = "Code";
                lookuptable = "FixedLookup";
                string lookupname = drows[0]["LookTableName"].ToString();

                sQuery = "SELECT TOP(1) " + lookupcodecolName + "," + lookupDispcolName + ",* from " + lookuptable + " with (nolock) where " + lookupcodecolName + " = @CodeFieldValue and LookupName='" + lookupname + "'";
            }
            else
            {

                lookupDispcolName = drows[0]["DisplayFieldPrefix"].ToString();
                lookupcodecolName = drows[0]["LinkingFieldPrefix"].ToString();
                lookuptable = drows[0]["LookTableName"].ToString();

                sQuery = "SELECT TOP(1) " + lookupcodecolName + "," + lookupDispcolName + ",* from " + lookuptable + " with (nolock) where " + lookupcodecolName + " = @CodeFieldValue";

            }


            SqlParameter[] Params = new SqlParameter[1];
            Params[0] = new SqlParameter("@CodeFieldValue", SqlDbType.VarChar);
            Params[0].Value = value.ToString();



            SqlConnection con = new SqlConnection(ConnectionFunctions.GetConnectionString());
            if (!ConnectionFunctions.Connect_SQLDataTable(ref dt, sQuery, ref con, Params, ref ErrMsg))
            {
                ErrMsg = "Error Occurred while retrieving the " + dictTitleNames[FieldName] + " Details from Database";
                return false;
            }
            if (dt.Rows.Count == 0)
            {
                ErrMsg = dictTitleNames[FieldName] + "which you have entered is not found in the Database";
                return false;
            }

            return true;

        }

        private Boolean ValidateField(string FieldName, object Value, bool isUpdate, ref string fieldErr, ref object LookUpCode, params object[] paraData)
        {
            bool isValid = true;

            bool hasSpecial;
            DateTime dateValue;
            int intValue;
            Int16 int16Value;
            double dblValue;
            decimal decValue;
            switch (FieldName)
            {
                case xlcol_EmpCode:

                    break;

                case xlcol_EmpNameE:
                    hasSpecial = hasSpecialChar(Value.ToString());
                    if (hasSpecial == true)
                    {
                        fieldErr = "Special characters are not allowed in " + dictTitleNames[FieldName];
                        isValid = false; break;
                    }
                    break;
                case xlcol_FNameE:
                    hasSpecial = hasSpecialChar(Value.ToString());
                    if (hasSpecial == true)
                    {
                        fieldErr = "Special characters are not allowed in " + dictTitleNames[FieldName];
                        isValid = false; break;
                    }
                    break;
                case xlcol_SNameE:
                    hasSpecial = hasSpecialChar(Value.ToString());
                    if (hasSpecial == true)
                    {
                        fieldErr = "Special characters are not allowed in " + dictTitleNames[FieldName];
                        isValid = false; break;
                    }
                    break;
                case xlcol_FamilyNameE:
                    hasSpecial = hasSpecialChar(Value.ToString());
                    if (hasSpecial == true)
                    {
                        fieldErr = "Special characters are not allowed in " + dictTitleNames[FieldName];
                        isValid = false; break;
                    }
                    break;
                case xlcol_Sex:

                    if (!ValidateLookUp(dtLookUpFieldsDetails_Emp, FieldName, Value, ref fieldErr, ref LookUpCode))
                    {
                        isValid = false; break;
                    }
                    break;

                case xlcol_MaritalStat:

                    if (!ValidateLookUp(dtLookUpFieldsDetails_Emp, FieldName, Value, ref fieldErr, ref LookUpCode))
                    {
                        isValid = false; break;
                    }
                    break;

                case xlcol_NPresent:
                    if (!ValidateLookUp(dtLookUpFieldsDetails_Emp, FieldName, Value, ref fieldErr, ref LookUpCode))
                    {
                        isValid = false; break;
                    }
                    break;
                case xlcol_NPrevious:
                    if (!ValidateLookUp(dtLookUpFieldsDetails_Emp, FieldName, Value, ref fieldErr, ref LookUpCode))
                    {
                        isValid = false; break;
                    }
                    break;
                case xlcol_Religion:
                    //if (!ValidateLookUp(dtLookUpFieldsDetails_Emp, FieldName, Value, ref fieldErr, ref LookUpCode))
                    //{
                    //    isValid = false; break;
                    //}
                    break;
                case xlcol_DateOfBirth:

                    if (!ValidateDateTime(Value.ToString(), out dateValue, ref errmsg))
                    {
                        fieldErr = "Invalid " + dictTitleNames[FieldName];
                        isValid = false; break;
                    }

                    int DOBYEARCurrVal = 0;
                    DateTime currDateT = DateTime.Now;
                    DOBYEARCurrVal = currDateT.Year;

                    if (dateValue.Year > DOBYEARCurrVal)
                    {
                        fieldErr = dictTitleNames[FieldName] + "Can't be greater than current year : " + Environment.NewLine;
                        isValid = false; break;
                    }

                    break;
                case xlcol_BirthPlaceE: break;
                case xlcol_CountryOfBirth:
                    if (!ValidateLookUp(dtLookUpFieldsDetails_Emp, FieldName, Value, ref fieldErr, ref LookUpCode))
                    {
                        isValid = false; break;
                    }
                    break;
                case xlcol_PassportNoE: break;
                case xlcol_PIssuePlaceE: break;
                case xlcol_PIssueCountry:
                    //if (!ValidateLookUp(dtLookUpFieldsDetails_Emp, FieldName, Value, ref fieldErr, ref LookUpCode))
                    //{
                    //    isValid = false; break;
                    //}
                    break;
                case xlcol_PIssueDate:
                    if (!ValidateDateTime(Value.ToString(), out dateValue, ref errmsg))
                    {
                        fieldErr = "Invalid " + dictTitleNames[FieldName] + ". " + errmsg;
                        isValid = false; break;
                    }
                    break;
                case xlcol_PExpiryDate:
                    if (!ValidateDateTime(Value.ToString(), out dateValue, ref errmsg))
                    {
                        fieldErr = "Invalid " + dictTitleNames[FieldName];
                        isValid = false; break;
                    }
                    break;
                case xlcol_RelNameE:
                    hasSpecial = hasSpecialChar(Value.ToString());
                    if (hasSpecial == true)
                    {
                        fieldErr = "Special characters are not allowed in " + dictTitleNames[FieldName];
                        isValid = false; break;
                    }
                    break;
                case xlcol_NextofKinE:
                    hasSpecial = hasSpecialChar(Value.ToString());
                    if (hasSpecial == true)
                    {
                        fieldErr = "Special characters are not allowed in " + dictTitleNames[FieldName];
                        isValid = false; break;
                    }
                    break;
                case xlcol_MotherNameE:
                    hasSpecial = hasSpecialChar(Value.ToString());
                    if (hasSpecial == true)
                    {
                        fieldErr = "Special characters are not allowed in " + dictTitleNames[FieldName];
                        isValid = false; break;
                    }
                    break;
                case xlcol_MobileNo: break;
                case xlcol_TeleNoAbroad: break;
                case xlcol_PerAddressE: break;
                case xlcol_Email: break;
                case xlcol_PassportProf:

                    if (!ValidateLookUp(dtLookUpFieldsDetails_Emp, FieldName, Value, ref fieldErr, ref LookUpCode))
                    {
                        isValid = false; break;
                    }
                    break;
                case xlcol_SLReInitDate:
                    if (!ValidateDateTime(Value.ToString(), out dateValue, ref errmsg))
                    {
                        fieldErr = "Invalid " + dictTitleNames[FieldName];
                        isValid = false; break;
                    }
                    break;
                case xlcol_StartDtofIndemnity:
                    if (!ValidateDateTime(Value.ToString(), out dateValue, ref errmsg))
                    {
                        fieldErr = "Invalid " + dictTitleNames[FieldName];
                        isValid = false; break;
                    }
                    break;
                case xlcol_AuxString3: break;
                case xlcol_Education:
                    if (!ValidateLookUp(dtLookUpFieldsDetails_Emp, FieldName, Value, ref fieldErr, ref LookUpCode))
                    {
                        isValid = false; break;
                    }
                    break;
                case xlcol_AuxString4: break;
                case xlcol_AuxInt3:
                    if (!int.TryParse(Value.ToString(), out intValue))
                    {
                        fieldErr = "Invalid " + dictTitleNames[FieldName];
                        isValid = false; break;
                    }
                    break;
                case xlcol_LocLib5: break;
                case xlcol_SalProfile:

                    if (!ValidateLookUp(dtLookUpFieldsDetails_Emp, FieldName, Value, ref fieldErr, ref LookUpCode))
                    {
                        isValid = false; break;
                    }
                    break;

                case xlcol_HealthInsurCmp:
                    if (!ValidateLookUp(dtLookUpFieldsDetails_Emp, FieldName, Value, ref fieldErr, ref LookUpCode))
                    {
                        isValid = false; break;
                    }
                    break;

                case xlcol_NextofKinAddrE: break;
                case xlcol_SponsorCode:
                    if (!ValidateLookUp(dtLookUpFieldsDetails_Emp, FieldName, Value, ref fieldErr, ref LookUpCode))
                    {
                        isValid = false; break;
                    }
                    break;
                case xlcol_Title:
                    if (!ValidateLookUp(dtLookUpFieldsDetails_Emp, FieldName, Value, ref fieldErr, ref LookUpCode))
                    {
                        isValid = false; break;
                    }
                    break;
                case xlcol_EmpNameA: break;
                case xlcol_NickNameE:
                    //hasSpecial = hasSpecialChar(Value.ToString());
                    //if (hasSpecial == true)
                    //{
                    //    fieldErr = "Special characters are not allowed in " + dictTitleNames[FieldName];
                    //    isValid = false; break;
                    //}
                    break;
                case xlcol_NickNameA: break;
                case xlcol_RelType:
                    if (!ValidateLookUp(dtLookUpFieldsDetails_Emp, FieldName, Value, ref fieldErr, ref LookUpCode))
                    {
                        isValid = false; break;
                    }
                    break;
                case xlcol_RelNameA: break;
                case xlcol_MotherNameA: break;
                case xlcol_FamilyNameA: break;
                case xlcol_BirthPlaceA: break;
                case xlcol_PassportNoA: break;
                case xlcol_PCategory:
                    if (!ValidateLookUp(dtLookUpFieldsDetails_Emp, FieldName, Value, ref fieldErr, ref LookUpCode))
                    {
                        isValid = false; break;
                    }
                    break;
                case xlcol_PIssuePlaceA: break;
                case xlcol_PerAddressA: break;
                case xlcol_Skill1:
                    //if (!ValidateLookUp(dtLookUpFieldsDetails_Emp, FieldName, Value, ref fieldErr, ref LookUpCode))
                    //{
                    //    isValid = false; break;
                    //}
                    break;
                case xlcol_Skill2:
                    //if (!ValidateLookUp(dtLookUpFieldsDetails_Emp, FieldName, Value, ref fieldErr, ref LookUpCode))
                    //{
                    //    isValid = false; break;
                    //}
                    break;
                case xlcol_Skill3:
                    //if (!ValidateLookUp(dtLookUpFieldsDetails_Emp, FieldName, Value, ref fieldErr, ref LookUpCode))
                    //{
                    //    isValid = false; break;
                    //}
                    break;
                case xlcol_Language1:
                    //if (!ValidateLookUp(dtLookUpFieldsDetails_Emp, FieldName, Value, ref fieldErr, ref LookUpCode))
                    //{
                    //    isValid = false; break;
                    //}
                    break;
                case xlcol_Language2:
                    //if (!ValidateLookUp(dtLookUpFieldsDetails_Emp, FieldName, Value, ref fieldErr, ref LookUpCode))
                    //{
                    //    isValid = false; break;
                    //}
                    break;
                case xlcol_Language3:
                    //if (!ValidateLookUp(dtLookUpFieldsDetails_Emp, FieldName, Value, ref fieldErr, ref LookUpCode))
                    //{
                    //    isValid = false; break;
                    //}
                    break;
                case xlcol_VisaType:
                    if (!ValidateLookUp(dtLookUpFieldsDetails_Emp, FieldName, Value, ref fieldErr, ref LookUpCode))
                    {
                        isValid = false; break;
                    }
                    break;
                case xlcol_VisaNo: break;
                case xlcol_VisaIssueDate:
                    if (!ValidateDateTime(Value.ToString(), out dateValue, ref errmsg))
                    {
                        fieldErr = "Invalid " + dictTitleNames[FieldName];
                    }
                    break;
                case xlcol_ImmigFileNo: break;
                case xlcol_EntryPlace:
                    if (!ValidateLookUp(dtLookUpFieldsDetails_Emp, FieldName, Value, ref fieldErr, ref LookUpCode))
                    {
                        isValid = false; break;
                    }
                    break;
                case xlcol_EntryDate:
                    if (!ValidateDateTime(Value.ToString(), out dateValue, ref errmsg))
                    {
                        fieldErr = "Invalid " + dictTitleNames[FieldName];
                        isValid = false; break;
                    }
                    break;
                case xlcol_ResidenceNo: break;
                case xlcol_ResIssueDate:
                    if (!ValidateDateTime(Value.ToString(), out dateValue, ref errmsg))
                    {
                        fieldErr = "Invalid " + dictTitleNames[FieldName];
                        isValid = false; break;
                    }
                    break;
                case xlcol_ResExpDate:
                    if (!ValidateDateTime(Value.ToString(), out dateValue, ref errmsg))
                    {
                        fieldErr = "Invalid " + dictTitleNames[FieldName];
                        isValid = false; break;
                    }
                    break;
                case xlcol_ResIssuePlace:
                    if (!ValidateLookUp(dtLookUpFieldsDetails_Emp, FieldName, Value, ref fieldErr, ref LookUpCode))
                    {
                        isValid = false; break;
                    }
                    break;
                case xlcol_LabCardNo: break;
                case xlcol_LCIssueDate:
                    if (!ValidateDateTime(Value.ToString(), out dateValue, ref errmsg))
                    {
                        fieldErr = "Invalid " + dictTitleNames[FieldName];
                        isValid = false; break;
                    }
                    break;
                case xlcol_LCExpDate:
                    if (!ValidateDateTime(Value.ToString(), out dateValue, ref errmsg))
                    {
                        fieldErr = "Invalid " + dictTitleNames[FieldName];
                        isValid = false; break;
                    }
                    break;
                case xlcol_HlthCardNo: break;
                case xlcol_HCIssuePlace:
                    if (!ValidateLookUp(dtLookUpFieldsDetails_Emp, FieldName, Value, ref fieldErr, ref LookUpCode))
                    {
                        isValid = false; break;
                    }
                    break;
                case xlcol_HCIssueDate:
                    if (!ValidateDateTime(Value.ToString(), out dateValue, ref errmsg))
                    {
                        fieldErr = "Invalid " + dictTitleNames[FieldName];
                        isValid = false; break;
                    }
                    break;
                case xlcol_HCExpiryDate:
                    if (!ValidateDateTime(Value.ToString(), out dateValue, ref errmsg))
                    {
                        fieldErr = "Invalid " + dictTitleNames[FieldName];
                        isValid = false; break;
                    }
                    break;
                case xlcol_DrvLicNo: break;
                case xlcol_DLCategory:
                    if (!ValidateLookUp(dtLookUpFieldsDetails_Emp, FieldName, Value, ref fieldErr, ref LookUpCode))
                    {
                        isValid = false; break;
                    }
                    break;
                case xlcol_DLIssuePlace:
                    if (!ValidateLookUp(dtLookUpFieldsDetails_Emp, FieldName, Value, ref fieldErr, ref LookUpCode))
                    {
                        isValid = false; break;
                    }
                    break;
                case xlcol_DLIssueDate:
                    if (!ValidateDateTime(Value.ToString(), out dateValue, ref errmsg))
                    {
                        fieldErr = "Invalid " + dictTitleNames[FieldName];
                        isValid = false; break;
                    }
                    break;
                case xlcol_DLExpiryDate:
                    if (!ValidateDateTime(Value.ToString(), out dateValue, ref errmsg))
                    {
                        fieldErr = "Invalid " + dictTitleNames[FieldName];
                        isValid = false; break;
                    }
                    break;
                case xlcol_SponByOther:
                    if (!ValidateLookUp(dtLookUpFieldsDetails_Emp, FieldName, Value, ref fieldErr, ref LookUpCode))
                    {
                        isValid = false; break;
                    }
                    break;
                case xlcol_OSponNameE: break;
                case xlcol_OSponNameA: break;
                case xlcol_OSponRel:
                    if (!ValidateLookUp(dtLookUpFieldsDetails_Emp, FieldName, Value, ref fieldErr, ref LookUpCode))
                    {
                        isValid = false; break;
                    }
                    break;
                case xlcol_OSponNation:
                    if (!ValidateLookUp(dtLookUpFieldsDetails_Emp, FieldName, Value, ref fieldErr, ref LookUpCode))
                    {
                        isValid = false; break;
                    }
                    break;
                case xlcol_OSponVisaNo: break;
                case xlcol_OSponVExpDt:
                    if (!ValidateDateTime(Value.ToString(), out dateValue, ref errmsg))
                    {
                        fieldErr = "Invalid " + dictTitleNames[FieldName];
                        isValid = false; break;
                    }
                    break;
                case xlcol_OSponPsprtNoE: break;
                case xlcol_OSponPsprtNoA: break;
                case xlcol_ExperienceE: break;
                case xlcol_ExperienceA: break;
                case xlcol_Emirates:
                    if (!ValidateLookUp(dtLookUpFieldsDetails_Emp, FieldName, Value, ref fieldErr, ref LookUpCode))
                    {
                        isValid = false; break;
                    }
                    break;
                case xlcol_City:
                    if (!ValidateLookUp(dtLookUpFieldsDetails_Emp, FieldName, Value, ref fieldErr, ref LookUpCode))
                    {
                        isValid = false; break;
                    }
                    break;
                case xlcol_Area:
                    if (!ValidateLookUp(dtLookUpFieldsDetails_Emp, FieldName, Value, ref fieldErr, ref LookUpCode))
                    {
                        isValid = false; break;
                    }
                    break;
                case xlcol_StreetE: break;
                case xlcol_StreetA: break;
                case xlcol_BuildingE: break;
                case xlcol_BuildingA: break;
                case xlcol_FlatE: break;
                case xlcol_FlatA: break;
                case xlcol_OffPhoneNo: break;
                case xlcol_Ext: break;
                case xlcol_ResPhoneNo: break;
                case xlcol_POBox: break;
                case xlcol_PagerNo: break;
                case xlcol_BloodGroup:
                    //if (!ValidateLookUp(dtLookUpFieldsDetails_Emp, FieldName, Value, ref fieldErr, ref LookUpCode))
                    //{
                    //    isValid = false; break;
                    //}
                    break;
                case xlcol_FaxNo: break;
                case xlcol_NextofKinA: break;
                case xlcol_NextofKinAddrA: break;
                case xlcol_AddressE: break;
                case xlcol_AddressA: break;
                case xlcol_AuxString1: break;
                case xlcol_AuxString2: break;
                case xlcol_AuxString5: break;
                case xlcol_AuxString6:

                case xlcol_AuxString7: break;
                case xlcol_AuxString8: break;
                case xlcol_AuxString9: break;
                case xlcol_AuxString10: break;
                case xlcol_AuxAString1: break;
                case xlcol_AuxAString2: break;
                case xlcol_AuxAString3: break;
                case xlcol_AuxAString4: break;
                case xlcol_AuxAString5: break;
                case xlcol_AuxAString6: break;
                case xlcol_AuxAString7: break;
                case xlcol_AuxAString8: break;
                case xlcol_AuxAString9: break;
                case xlcol_AuxAString10: break;
                case xlcol_AuxInt1:
                    if (!int.TryParse(Value.ToString(), out intValue))
                    {
                        fieldErr = "Invalid " + dictTitleNames[FieldName];
                        isValid = false; break;
                    }
                    break;
                case xlcol_AuxInt2:
                    if (!int.TryParse(Value.ToString(), out intValue))
                    {
                        fieldErr = "Invalid " + dictTitleNames[FieldName];
                        isValid = false; break;
                    }
                    break;
                case xlcol_AuxCurrency1:
                    if (!Double.TryParse(Value.ToString(), out dblValue))
                    {
                        fieldErr = "Invalid " + dictTitleNames[FieldName];
                        isValid = false; break;
                    }
                    break;
                case xlcol_AuxCurrency2:
                    if (!Double.TryParse(Value.ToString(), out dblValue))
                    {
                        fieldErr = "Invalid " + dictTitleNames[FieldName];
                        isValid = false; break;
                    }
                    break;
                case xlcol_AuxDate1:
                    if (!ValidateDateTime(Value.ToString(), out dateValue, ref errmsg))
                    {
                        fieldErr = "Invalid " + dictTitleNames[FieldName];
                        isValid = false; break;
                    }
                    break;
                case xlcol_AuxDate2:
                    if (!ValidateDateTime(Value.ToString(), out dateValue, ref errmsg))
                    {
                        fieldErr = "Invalid " + dictTitleNames[FieldName];
                        isValid = false; break;
                    }
                    break;
                case xlcol_AuxDate3:
                    if (!ValidateDateTime(Value.ToString(), out dateValue, ref errmsg))
                    {
                        fieldErr = "Invalid " + dictTitleNames[FieldName];
                        isValid = false; break;
                    }
                    break;
                case xlcol_AuxDate4:
                    if (!ValidateDateTime(Value.ToString(), out dateValue, ref errmsg))
                    {
                        fieldErr = "Invalid " + dictTitleNames[FieldName];
                        isValid = false; break;
                    }
                    break;
                case xlcol_AuxDate5:
                    if (!ValidateDateTime(Value.ToString(), out dateValue, ref errmsg))
                    {
                        fieldErr = "Invalid " + dictTitleNames[FieldName];
                        isValid = false; break;
                    }
                    break;
                case xlcol_AuxLib1:
                    if (!ValidateLookUp(dtLookUpFieldsDetails_Emp, FieldName, Value, ref fieldErr, ref LookUpCode))
                    {
                        isValid = false; break;
                    }
                    break;
                case xlcol_AuxLib2:
                    if (!ValidateLookUp(dtLookUpFieldsDetails_Emp, FieldName, Value, ref fieldErr, ref LookUpCode))
                    {
                        isValid = false; break;
                    }
                    break;
                case xlcol_AuxLib3:
                    //if (!ValidateLookUp(dtLookUpFieldsDetails_Emp, FieldName, Value, ref fieldErr, ref LookUpCode))
                    //{
                    //    isValid = false; break;
                    //}
                    break;
                case xlcol_AuxLib4:
                    if (!ValidateLookUp(dtLookUpFieldsDetails_Emp, FieldName, Value, ref fieldErr, ref LookUpCode))
                    {
                        isValid = false; break;
                    }
                    break;
                case xlcol_IntlJoiningDate:
                    if (!ValidateDateTime(Value.ToString(), out dateValue, ref errmsg))
                    {
                        fieldErr = "Invalid " + dictTitleNames[FieldName];
                        isValid = false; break;
                    }
                    break;
                case xlcol_PointOfHireE:
                    if (!ValidateLookUp(dtLookUpFieldsDetails_Emp, FieldName, Value, ref fieldErr, ref LookUpCode))
                    {
                        isValid = false; break;
                    }
                    break;
                case xlcol_PointOfHireA: break;
                case xlcol_SourceOfHire:
                    if (!ValidateLookUp(dtLookUpFieldsDetails_Emp, FieldName, Value, ref fieldErr, ref LookUpCode))
                    {
                        isValid = false; break;
                    }
                    break;
                case xlcol_LCIssuePlace:
                    if (!ValidateLookUp(dtLookUpFieldsDetails_Emp, FieldName, Value, ref fieldErr, ref LookUpCode))
                    {
                        isValid = false; break;
                    }
                    break;
                case xlcol_FNameA: break;
                case xlcol_SNameA: break;
                case xlcol_GrandFatherE: break;
                case xlcol_SponTypeExtnl:
                    if (!ValidateLookUp(dtLookUpFieldsDetails_Emp, FieldName, Value, ref fieldErr, ref LookUpCode))
                    {
                        isValid = false; break;
                    }
                    break;
                case xlcol_GrandFatherA: break;
                case xlcol_VisaExpDate:
                    if (!ValidateDateTime(Value.ToString(), out dateValue, ref errmsg))
                    {
                        fieldErr = "Invalid " + dictTitleNames[FieldName];
                        isValid = false; break;
                    }
                    break;
                case xlcol_AuxLib5:
                    if (!ValidateLookUp(dtLookUpFieldsDetails_Emp, FieldName, Value, ref fieldErr, ref LookUpCode))
                    {
                        isValid = false; break;
                    }
                    break;
                case xlcol_AuxLib6:
                    if (!ValidateLookUp(dtLookUpFieldsDetails_Emp, FieldName, Value, ref fieldErr, ref LookUpCode))
                    {
                        isValid = false; break;
                    }
                    break;
                case xlcol_PersEmail: break;
                case xlcol_NPresentSec:
                    if (!ValidateLookUp(dtLookUpFieldsDetails_Emp, FieldName, Value, ref fieldErr, ref LookUpCode))
                    {
                        isValid = false; break;
                    }
                    break;
                case xlcol_ReligionSubSet:
                    if (!ValidateLookUp(dtLookUpFieldsDetails_Emp, FieldName, Value, ref fieldErr, ref LookUpCode))
                    {
                        isValid = false; break;
                    }
                    break;
                case xlcol_UIDNo: break;
                case xlcol_Disability:
                    if (!ValidateLookUp(dtLookUpFieldsDetails_Emp, FieldName, Value, ref fieldErr, ref LookUpCode))
                    {
                        isValid = false; break;
                    }
                    break;
                case xlcol_DisabilityDescE: break;
                case xlcol_DisabilityDescA: break;
                case xlcol_NationalID:
                    hasSpecial = hasSpecialChar(Value.ToString());
                    if (hasSpecial == true)
                    {
                        fieldErr = "Special characters are not allowed in " + dictTitleNames[FieldName];
                        isValid = false; break;
                    }
                    break;
                case xlcol_WPS:
                    if (!ValidateLookUp(dtLookUpFieldsDetails_Emp, FieldName, Value, ref fieldErr, ref LookUpCode))
                    {
                        isValid = false; break;
                    }
                    break;


            }
            return isValid;
        }


        public static bool hasSpecialChar(string input)
        {
            string specialChar = @"[~!@#$%^&*()+=|\\{}':;.,<>/?[\]""_-]";
            //string specialChar = @"[~`!@#$%^&*()+=|\\{}':;.,<>/?[\]""_-]";
            //object item;
            foreach (var item in specialChar)
            {
                if (input.Contains(item))
                    return true;
            }
            return false;
        }

        private bool ValidateLookUpCode(DataTable dtLookUpFD, string FieldName, object value, ref string fieldErr, ref object LookUpCode)
        {
            LookUpCode = null;

            if (dtLookUpFD == null)
            {
                return true;
            }
            if (dtLookUpFD.Rows.Count == 0)
            {
                return true;
            }
            DataRow[] drows = dtLookUpFD.Select("FieldPrefix='" + FieldName + "'");
            if (drows == null || drows.Length == 0)
            {
                return true;
            }

            string sQuery = "";
            string lookupDispcolName = "";
            string lookupcodecolName = "";
            string lookuptable = "";
            SqlDataReader dr = null;

            if (Convert.ToInt32(drows[0]["FieldType"]) == 4 || Convert.ToInt32(drows[0]["FieldType"]) == 5)
            {
                //fixed lookup
                lookupDispcolName = "DescE";
                lookupcodecolName = "Code";
                lookuptable = "FixedLookup";
                string lookupname = drows[0]["LookTableName"].ToString();

                sQuery = "SELECT TOP(1) " + lookupcodecolName + " from " + lookuptable + " with (nolock) where " + lookupcodecolName + " = @CodeFieldValue and LookupName='" + lookupname + "'";
            }
            else
            {

                lookupDispcolName = drows[0]["DisplayFieldPrefix"].ToString();
                lookupcodecolName = drows[0]["LinkingFieldPrefix"].ToString();
                lookuptable = drows[0]["LookTableName"].ToString();

                sQuery = "SELECT TOP(1) " + lookupcodecolName + " from " + lookuptable + " with (nolock) where " + lookupcodecolName + " = @CodeFieldValue";

            }


            SqlParameter[] Params = new SqlParameter[1];
            Params[0] = new SqlParameter("@CodeFieldValue", SqlDbType.VarChar);
            Params[0].Value = value.ToString();

            if (!ConnectionFunctions.Connect_SQLDataReader(ref dr, sQuery, ref errmsg, Params, CommandType.Text))
            {
                fieldErr = "Error Occurred while retrieving the " + dictTitleNames[FieldName] + " Details from Database";
                return false;
            }
            else
            {
                if (!dr.HasRows)
                {
                    dr.Close();
                    fieldErr = dictTitleNames[FieldName] + " which you have entered is not found in the Database";
                    return false;
                }

                dr.Read();

                if (!dr.IsDBNull(dr.GetOrdinal(lookupcodecolName)))
                    LookUpCode = dr[lookupcodecolName];
            }
            dr.Close();

            return true;
        }

        private bool ValidateLookUp(DataTable dtLookUpFD, string FieldName, object value, ref string fieldErr, ref object LookUpCode)
        {
            LookUpCode = null;

            if (dtLookUpFD == null)
            {
                return true;
            }
            if (dtLookUpFD.Rows.Count == 0)
            {
                return true;
            }
            DataRow[] drows = dtLookUpFD.Select("FieldPrefix='" + FieldName + "'");
            if (drows == null || drows.Length == 0)
            {
                return true;
            }

            string sQuery = "";
            string lookupDispcolName = "";
            string lookupcodecolName = "";
            string lookuptable = "";
            SqlDataReader dr = null;

            if (Convert.ToInt32(drows[0]["FieldType"]) == 4 || Convert.ToInt32(drows[0]["FieldType"]) == 5)
            {
                //fixed lookup
                lookupDispcolName = "DescE";
                lookupcodecolName = "Code";
                lookuptable = "FixedLookup";
                string lookupname = drows[0]["LookTableName"].ToString();

                sQuery = "SELECT TOP(1) " + lookupcodecolName + " from " + lookuptable + " with (nolock) where " + lookupDispcolName + " = @DisplayFieldValue and LookupName='" + lookupname + "'";
            }
            else
            {

                lookupDispcolName = drows[0]["DisplayFieldPrefix"].ToString();
                lookupcodecolName = drows[0]["LinkingFieldPrefix"].ToString();
                lookuptable = drows[0]["LookTableName"].ToString();

                sQuery = "SELECT TOP(1) " + lookupcodecolName + " from " + lookuptable + " with (nolock) where " + lookupDispcolName + " = @DisplayFieldValue";

            }


            SqlParameter[] Params = new SqlParameter[1];
            Params[0] = new SqlParameter("@DisplayFieldValue", SqlDbType.VarChar);
            Params[0].Value = value.ToString();

            if (!ConnectionFunctions.Connect_SQLDataReader(ref dr, sQuery, ref errmsg, Params, CommandType.Text))
            {
                fieldErr = "Error Occurred while retrieving the " + dictTitleNames[FieldName] + " Details from Database";
                return false;
            }
            else
            {
                if (!dr.HasRows)
                {
                    dr.Close();
                    fieldErr = dictTitleNames[FieldName] + " which you have entered is not found in the Database";
                    return false;
                }

                dr.Read();

                if (!dr.IsDBNull(dr.GetOrdinal(lookupcodecolName)))
                    LookUpCode = dr[lookupcodecolName];
            }
            dr.Close();

            return true;
        }

        private bool ValidateDateTime(string strdtToCheck, out DateTime dateValue, ref string errmsg)
        {
            if (!DateTime.TryParseExact(strdtToCheck.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateValue))
            {
                if (!DateTime.TryParseExact(strdtToCheck.ToString(), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateValue))
                {
                    if (!DateTime.TryParseExact(strdtToCheck.ToString(), "dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateValue))
                    {
                        errmsg = "";
                        return false;
                    }
                }
            }
            if (dateValue < dtminDate || dateValue > dtmaxDate)
            {
                errmsg = "Value out of valid date range";
                return false;
            }
            return true;
        }
        private DateTime GetValidDateTime(string strDateTime)
        {

            DateTime dtrsltDate = new DateTime(1900, 1, 1);
            if(!DateTime.TryParseExact(strDateTime, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtrsltDate))
            {
                if (!DateTime.TryParseExact(strDateTime, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtrsltDate))
                {
                    DateTime.TryParseExact(strDateTime, "dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtrsltDate);
                }
            }


            if(dtrsltDate.ToString("dd/MM/yyyy") == "01/01/0001")
                dtrsltDate = new DateTime(1900, 1, 1);

            return dtrsltDate;
        }

        /// <summary>
        /// Always returs true. Not Checking rights currently, for future we may create dummy user SFIntegrationService_DummyUser for rights checking
        /// </summary>
        /// <param name="LocationLevelCode"></param>
        /// <param name="SalaryProfileCode"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        private bool CheckUserRights_LocationAndSalaryProfile(string LocationLevelCode, string SalaryProfileCode, ref string ErrMsg)
        {
            //string sQueryLoc = "SELECT ISNULL(count(1),0) from SecRights with (nolock) where ModuleCode='CS0001' AND UserID= @ParmUserID";
            //sQueryLoc += " AND (CAST('@' as varchar(MAX)) + CAST (LocLib5 as varchar(MAX)) like '%@' +  @ParmLocationLevelCode + '@%' OR  CAST (LocLib5 as varchar(MAX))='999@')";
            //sQueryLoc += " AND (CAST('@' as varchar(MAX)) + CAST (SalProfile as varchar(MAX)) like '%@' + @ParmSalaryProfileCode + '@%' OR  CAST (SalProfile as varchar(MAX))='999@')";

            //SqlParameter[] Params = new SqlParameter[3];
            //Params[0] = new SqlParameter("@ParmUserID", SqlDbType.VarChar);
            //Params[0].Value = "SFIntegrationService_DummyUser";
            //Params[1] = new SqlParameter("@ParmLocationLevelCode", SqlDbType.VarChar);
            //Params[1].Value = LocationLevelCode;
            //Params[2] = new SqlParameter("@ParmSalaryProfileCode", SqlDbType.VarChar);
            //Params[2].Value = SalaryProfileCode;

            //string cntLoc = "0";
            //if (!ConnectionFunctions.Connect_SQLScalar(ref cntLoc, sQueryLoc, ref Params, ref errmsg))
            //{
            //    ErrMsg = "An error occured while verifying Location and Salary Profile rights.";
            //    return false;
            //}
            //if (Convert.ToInt32(cntLoc) == 0)
            //{
            //    ErrMsg = "You Don't have necessary Location and Salary Profile rights.";
            //    return false;
            //}

            return true;
        }


        public DataTable CreateEmployeeTableSchema()
        {
            SqlConnection SQLConn = new SqlConnection(ConnectionFunctions.GetConnectionString());
            DataSet EmployeeDS = new DataSet();
            try
            {
                SqlDataAdapter MyDataAdapter = new SqlDataAdapter();
                MyDataAdapter.SelectCommand = new SqlCommand("SELECT Top 0 * FROM Employee WITH (NOLOCK)", SQLConn);
                SqlCommandBuilder cb = new SqlCommandBuilder(MyDataAdapter);
                SQLConn.Open();
                MyDataAdapter.Fill(EmployeeDS, "Employee");
            }
            catch (Exception Ex)
            {
                if ((SQLConn != null))
                {
                    if (SQLConn.State != ConnectionState.Closed)
                        SQLConn.Close();
                }
            }
            finally
            {
                if ((SQLConn != null))
                {
                    if (SQLConn.State != ConnectionState.Closed)
                        SQLConn.Close();
                }
            }
            return EmployeeDS.Tables["Employee"];
        }

        private bool StringIsNullOrEmpty(string val)
        {

            if (val == "0" || string.IsNullOrEmpty(val) || val == "1900/01/01" || val == "01/01/1900" || val == "1900-01-01 00:00:00" || val == "1/1/1900 12:00:00 AM")
                return true;
            else
                return false;
        }
        private bool StringIsNullOrEmpty_AllowZero(string val)
        {

            if (string.IsNullOrEmpty(val) || val == "1900/01/01" || val == "01/01/1900" || val == "1900-01-01 00:00:00" || val == "1/1/1900 12:00:00 AM")
                return true;
            else
                return false;
        }

        private void Build_UpdateAudErrors(bool isUpdate, string FieldName, object newValue, ref StringBuilder strBuildrAud, ref DataRow drowDataBeforeUpdate)
        {
            if (isUpdate == false)
            {
                return;
            }
            if (drowDataBeforeUpdate == null)
            {
                return;
            }

            try
            {
                object oldValue = drowDataBeforeUpdate[FieldName];


                if (oldValue != newValue)
                {

                    string stroldval = "";
                    string strnewval = "";
                    if (oldValue != null)
                        stroldval = oldValue.ToString();
                    if (newValue != null)
                        strnewval = newValue.ToString();

                    if (IsNumeric(stroldval))
                    {
                        stroldval = String.Format("{0:#0.0#}", Convert.ToDouble(stroldval));
                    }
                    else if (IsDateTime(stroldval))
                    {
                        stroldval = String.Format("{0:yyyy-MM-dd HH:mm:ss}", Convert.ToDateTime(stroldval));
                    }
                    if (IsNumeric(strnewval))
                    {
                        strnewval = String.Format("{0:#0.0#}", Convert.ToDouble(strnewval));
                    }
                    else if (IsDateTime(strnewval))
                    {
                        strnewval = String.Format("{0:yyyy-MM-dd HH:mm:ss}", Convert.ToDateTime(strnewval));
                    }

                    stroldval = stroldval.Trim().ToLower();
                    strnewval = strnewval.Trim().ToLower();

                    if (stroldval != strnewval)
                    {
                        strBuildrAud.Append(" [ " + stroldval + " " + dictTitleNames[FieldName] + " " + strnewval + " ] ");

                    }


                    //switch (Type.GetTypeCode(oldValue.GetType()))
                    //{
                    //    case TypeCode.Decimal:

                    //        break;

                    //    case TypeCode.Int32:

                    //        break;

                    //}


                }


            }
            catch (Exception)
            {

            }


        }

        private bool IsDateTime(string strval)
        {
            DateTime output = new DateTime();
            return DateTime.TryParse(strval, out output);
        }
        private bool IsNumeric(string strval)
        {
            double output = 0;
            return double.TryParse(strval, out output);
        }


        private int AddProcessLogDetails()
        {


            errmsg = "";


            int pldsrno = 0;

            string sQry = " Insert into [DBOXIImportProcessLogDetails]  ([DBOXIProcessId],[RowNo])values ('" + importProcessId + "'," + iCurrRowNo + ");";
            sQry += " SELECT SCOPE_IDENTITY() AS LastInsertedId;";



            ConnectionFunctions.Connect_SQLScalar(ref pldsrno, sQry, ref errmsg);
           

            return pldsrno;
        }


        private bool UpdateProcessLogDetails()
        {

            errmsg = "";

            sQry = "Update [DBOXIImportProcessLogDetails]  set EmpCode=@EmpCode," +
                "[HasError]=@HasError," +
                "[Data_Saved]=@Data_Saved,[ImportDataType]=@ImportDataType,[HasBRError]=@HasBRError," +
                "[LoggedDate]=getdate(),[Remarks]=@Remarks" +
                " where [DBOXIProcessId]=@processID and [RowNo]=@RowNo; ";

            SqlParameter[] Params = new SqlParameter[8];
            Params[0] = new SqlParameter("@EmpCode", SqlDbType.VarChar);
            Params[0].Value = strCurrEmpCode;
            Params[1] = new SqlParameter("@HasError", SqlDbType.VarChar);
            Params[1].Value = (bCurrHasLineErrors ? "1" : "0");
            Params[2] = new SqlParameter("@Data_Saved", SqlDbType.VarChar);
            Params[2].Value = (bEmpSaved ? "1" : "0");
            Params[3] = new SqlParameter("@ImportDataType", SqlDbType.VarChar);
            Params[3].Value = "Employee";
            Params[4] = new SqlParameter("@HasBRError", SqlDbType.VarChar);
            Params[4].Value = (bHasEmpBRerror ? "1" : "0");
            Params[5] = new SqlParameter("@processID", SqlDbType.VarChar);
            Params[5].Value = importProcessId;
            Params[6] = new SqlParameter("@Remarks", SqlDbType.VarChar);
            Params[6].Value = sbSaveMsg.ToString();
            Params[7] = new SqlParameter("@RowNo", SqlDbType.VarChar);
            Params[7].Value = iCurrRowNo;


            RetVal = ConnectionFunctions.Connect_SQLNonQuery(ref result, sQry, ref errmsg, Params, CommandType.Text);

            if (!RetVal)
            {
                Common.LogAction("ProcessLogDetails Update failed. Details : " + errmsg);
            }

            return RetVal;
        }


        private void AppendLineError(int iLineNo, string lineInfo, string errMsg)
        {
            if (!string.IsNullOrEmpty(lineInfo))
            {
                lineInfo = ", " + lineInfo;
            }
            errCount++; errTotalCount++;
            //sbLineErrMsg.Append("LineNo: " + iLineNo.ToString() + lineInfo + ", Error : " + errMsg + Environment.NewLine);
            sbLineErrMsg.Append("EMP Error#" + errCount + " : " + errMsg + ";" + Environment.NewLine);
            //sbPersonAllLineErrMsg.Append("LineNo: " + iLineNo.ToString() + lineInfo + ", Error : " + errMsg + Environment.NewLine);
            sbFileErrMsg.Append("LineNo: " + iLineNo.ToString() + lineInfo + ", Error : " + errMsg + Environment.NewLine);

            Common.LogErrorToDBOXIErrorLog(Convert.ToInt32(importProcessId), iCurrPLDSrNo, strCurrEmpCode, lineInfo, errMsg);

        }


        //private void UpdateImportLineStatusAndContinue(int iLineNo, int sts)
        //{

        //    //ImportDataStatus sts = null;

        //    errmsg = "";
        //    try
        //    {
        //        if (!UpdateImportLineStatus(iLineNo, sts, ref errmsg))
        //        {
        //            AppendLineError(iLineNo, "", errmsg);
        //        }
        //        Common.LogAction("Employee Import SaveToCS: " + sbLineErrMsg.ToString().TrimEnd('\r', '\n'));

        //    }
        //    catch (Exception ex)
        //    {
        //        Common.LogAction("Employee Import SaveToCS: Try Catch error, Details: " + ex.Message);
        //        Common.LogException(ex);
        //    }
        //    finally
        //    {
        //        sbLineErrMsg.Clear();
        //        errCount = 0;
        //    }
        //}

        //private bool UpdateImportLineStatus(int iLineNo, int sts, ref string errmsg)
        //{
        //    //ImportDataStatus sts = null;

        //    errmsg = "";
        //    //Update LineStatus to processed
        //    sQry = "Update SFIEmployeeInitialStaging  set [SFIstatus] = " + ((int)sts).ToString() + " Where FileName='" + importProcessId + "' and [RowNo]=" + iLineNo.ToString();
        //    RetVal = ConnectionFunctions.Connect_SQLNonQuery(ref result, sQry, ref errmsg);


        //    return RetVal;
        //}



        private bool CheckIfColumnExists(DataTable mydt, string colName)
        {
            bool columnExists = mydt.Columns.Cast<DataColumn>()
            .Any(c => string.Equals(c.ColumnName, colName, StringComparison.OrdinalIgnoreCase));

            return columnExists;
        }


        private bool Update_LASTUPDDTTM(string strEmpCode, DateTime lastUpdateDate, string updateModule)
        {
            errmsg = "";

            if (string.IsNullOrEmpty(strEmpCode))
            {
                return false;
            }
            string updateModulecolname = "";
            switch (updateModule.ToLower())
            {
                case "employee":
                    updateModulecolname = "Employee_LASTUPDDTTM";
                    break;
                case "wrk":
                    updateModulecolname = "WrkAgreement_LASTUPDDTTM";
                    break;
                case "fin":
                    updateModulecolname = "Financial_LASTUPDDTTM";
                    break;
                case "empTransfer":
                    updateModulecolname = "EmpTransfer_LASTUPDDTTM";
                    break;
                case "empexit":
                    updateModulecolname = "EmpExit_LASTUPDDTTM";
                    break;
            }

            sQry = "IF NOT EXISTS (SELECT * FROM DBOXI_EmpLastUpdateDate WHERE empcode='" + strEmpCode + "')" +
                "BEGIN " +
                "   Insert into DBOXI_EmpLastUpdateDate ([EmpCode]," + updateModulecolname + ") values ('" + strEmpCode + "','" + lastUpdateDate.ToString("yyyyMMdd HH:mm") + "');" +
                "End " +
                "Else " +
                "BEGIN " +
                "   UPDATE DBOXI_EmpLastUpdateDate set " + updateModulecolname + " = '" + lastUpdateDate.ToString("yyyyMMdd HH:mm") + "' Where [EmpCode]='" + strEmpCode + "';" +
                "End "; 

            RetVal = ConnectionFunctions.Connect_SQLNonQuery(ref result, sQry, ref errmsg);

            if (!RetVal)
            {
                Common.LogAction("Update_LASTUPDDTTM failed. Details: " + errmsg);
            }

            return RetVal;
        }

        private void AssignStagingTitleNames(ref Dictionary<string, string> EngTitles)
        {
            //dictionry key is case sensitive


            EngTitles[xlcol_EmpCode] = "EmployeeID";
            EngTitles[xlcol_LocLib5] = "WorkingCompany";
            EngTitles[xlcol_Title] = "EmployeeTitle";
            EngTitles[xlcol_EmpNameE] = "FullNameE";
            EngTitles[xlcol_FNameE] = "FirstName";
            EngTitles[xlcol_SNameE] = "MiddleName";
            EngTitles[xlcol_NickNameE] = "ThirdName";
            EngTitles[xlcol_GrandFatherE] = "FourthName";
            EngTitles[xlcol_FamilyNameE] = "FamilyName";
            EngTitles[xlcol_MotherNameE] = "MotherNameE";
            EngTitles[xlcol_Sex] = "Gender";
            EngTitles[xlcol_Religion] = "Religion";
            EngTitles[xlcol_ReligionSubSet] = "Faith";
            EngTitles[xlcol_DateOfBirth] = "DateOfBirth";
            EngTitles[xlcol_MaritalStat] = "MaritalStatus";
            EngTitles[xlcol_CountryOfBirth] = "CountryOfBirth";
            EngTitles[xlcol_BirthPlaceE] = "BirthPlaceE";
            EngTitles[xlcol_PassportNoE] = "PassportNo";
            EngTitles[xlcol_PCategory] = "PassportCategory";
            EngTitles[xlcol_PIssueDate] = "PassportIssueDate";
            EngTitles[xlcol_PExpiryDate] = "PassportExpiryDate";
            EngTitles[xlcol_PIssueCountry] = "PassportIssueCountry";
            EngTitles[xlcol_PIssuePlaceE] = "PassportIssuePlace";
            EngTitles[xlcol_NPresent] = "PresentNationality";
            EngTitles[xlcol_NPrevious] = "PreviousNationality";
            EngTitles[xlcol_PassportProf] = "MOHREVISAProfession";
            EngTitles[xlcol_Education] = "VisaQualification";
            EngTitles[xlcol_Language1] = "Language1";
            EngTitles[xlcol_Language2] = "Language2";
            EngTitles[xlcol_Language3] = "Language3";
            EngTitles[xlcol_AuxLib1] = "EduCertissuedFrom";
            EngTitles[xlcol_AuxString3] = "MOFAAttestationNo";
            EngTitles[xlcol_AuxString4] = "MOFAAttestationLabel";
            EngTitles[xlcol_AuxString5] = "CertAttestationNo";
            EngTitles[xlcol_UIDNo] = "UnifiedIdentityNumber";
            EngTitles[xlcol_Emirates] = "EmirateState";
            EngTitles[xlcol_Area] = "Area";
            EngTitles[xlcol_City] = "City";
            EngTitles[xlcol_BuildingE] = "Building";
            EngTitles[xlcol_StreetE] = "Street";
            EngTitles[xlcol_FlatE] = "FlatNo";
            EngTitles[xlcol_POBox] = "POBox";
            EngTitles[xlcol_OffPhoneNo] = "OfficeTelNo";
            EngTitles[xlcol_ResPhoneNo] = "LandlineNo";
            EngTitles[xlcol_MobileNo] = "MobileNo";
            EngTitles[xlcol_TeleNoAbroad] = "TeleNoAbroad";
            EngTitles[xlcol_PersEmail] = "PersonalEmail";
            EngTitles[xlcol_AddressE] = "Address";
            EngTitles[xlcol_PerAddressE] = "AddressAbroad";
            EngTitles[xlcol_Email] = "Email";
            EngTitles[xlcol_SponsorCode] = "Sponsor";

            EngTitles.Add("CandidateLocationCurrently","CandidateLocationCurrently");
            EngTitles.Add("NoticePeriod","NoticePeriod");
            EngTitles.Add("Probation","Probation");
            EngTitles.Add("WeeklyHolidays","WeeklyHolidays");
            EngTitles.Add("WorkType","WorkType");
            EngTitles.Add("Remuneration","Remuneration");
            EngTitles.Add("BasicSalary","BasicSalary");
            EngTitles.Add("HousingAmount","HousingAmount");
            EngTitles.Add("TransportingAmount","TransportingAmount");
            EngTitles.Add("FoodAllowance","FoodAllowance");
            EngTitles.Add("MobileConnectivityAllowance","MobileConnectivityAllowance");
            EngTitles.Add("CostOfLivingAllowance","CostOfLivingAllowance");
            EngTitles.Add("OtherAllowance","OtherAllowance");
            EngTitles.Add("EmployeeStatus","EmployeeStatus");

            EngTitles[xlcol_AuxLib3] = "HRJobTitle";
            EngTitles[xlcol_AuxString1] = "UniversityName";
            EngTitles[xlcol_AuxString2] = "Faculty";
            EngTitles[xlcol_AuxString6] = "StudyMajors";
            EngTitles[xlcol_AuxLib2] = "DegreeType";
            EngTitles[xlcol_AuxDate1] = "DegreeStartDate";
            EngTitles[xlcol_AuxDate2] = "DegreeEndDate";
            EngTitles[xlcol_AuxInt1] = "GraduationYear";
            EngTitles[xlcol_AuxInt2] = "ActualYearsofDegree";
            EngTitles[xlcol_IntlJoiningDate] = "JoiningDate";

            EngTitles.Add("Doc_WBPhoto", "Doc_WBPhoto");
            EngTitles.Add("Doc_Pass1","Doc_Pass1");
            EngTitles.Add("Doc_Pass2","Doc_Pass2");
            EngTitles.Add("Doc_ECP1","Doc_ECP1");
            EngTitles.Add("Doc_ECP2","Doc_ECP2");
            EngTitles.Add("Doc_CTR","Doc_CTR");


        }

        private bool ValidateDocument(string docCode, byte[] fileBytes, string contentType, out string errorMsg)
        {
            errorMsg = "";

            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionFunctions.GetConnectionString()))
                {
                    con.Open();

                    string query = @"SELECT dt.MaxFileSize, aft.AllowedFileExtns FROM DocumentTypes dt LEFT JOIN AttachFileType aft ON dt.AttachFileType = aft.AttachFileTypeId WHERE dt.Code = @DocCode";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@DocCode", docCode);

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                int maxFileSize = dr["MaxFileSize"] != DBNull.Value ? Convert.ToInt32(dr["MaxFileSize"]) : 0;
                                string allowedExt = dr["AllowedFileExtns"]?.ToString();
                               
                                if (!string.IsNullOrEmpty(allowedExt))
                                {
                                    var allowedList = allowedExt.Split(',');

                                    string ext = contentType.Replace(".", "").ToLower();

                                    if (!allowedList.Any(a => a.Trim().ToLower() == ext))
                                    {
                                        errorMsg = $"Invalid file type. Allowed: {allowedExt}";
                                        return false;
                                    }
                                }

                               
                                if (maxFileSize > 0)
                                {
                                    double fileSizeMB = fileBytes.Length / (1024.0 * 1024.0);

                                    if (fileSizeMB > maxFileSize)
                                    {
                                        errorMsg = $"File size exceeded. Max allowed: {maxFileSize} MB";
                                        return false;
                                    }
                                }
                            }
                            else
                            {
                                errorMsg = "Document type not found";
                                return false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return false;
            }

            return true;
        }

        [HttpPost]
        public JsonResult getTranslation(string TextToTranslate, string language)
        {
            var response = new GeneralResponse();
            try
            {
                // Read the web.config switch
                bool v2Enabled = true;
                //bool.TryParse(ConfigurationManager.AppSettings["TranslationV2Enabled"], out v2Enabled);

                if (v2Enabled)
                {
                    // === V2 path (Google Translation client) ===
                    string basePath = AppDomain.CurrentDomain.BaseDirectory;
                    string addBin = Path.Combine(basePath, "bin");
                    string jsonFilePath = Path.Combine(addBin, "cs-translationapiproj-321d0950989b.json");

                    string fromLang = (string.Equals(language, "en", StringComparison.OrdinalIgnoreCase)) ? "ar" : "en";
                    string toLang = (fromLang == "ar") ? "en" : "ar";

                    var v2 = TranslationHelperV2.TranslateFromTo(TextToTranslate, fromLang, toLang, jsonFilePath);

                    if (!string.IsNullOrWhiteSpace(v2))
                    {
                        response.Status = GeneralResponse.ResponseStatus.Success;
                        response.Result = v2;
                    }
                    else
                    {
                        response.Status = GeneralResponse.ResponseStatus.Error;
                    }

                    return Json(response, JsonRequestBehavior.AllowGet);
                }

             
                string result = null;

                if (language == "ar") // EN → AR
                {
                    result = TranslationHelper.IsIdLike(TextToTranslate)
                        ? TextToTranslate
                        : TranslationHelper.TranslateTextV3(TextToTranslate, "en", "ar");
                }
                else if (language == "en") // AR → EN
                {
                    if (TranslationHelper.IsIdLike(TextToTranslate))
                    {
                        result = TextToTranslate;
                    }
                    else
                    {
                        string normalized = TranslationHelper.NormalizeArabic(TextToTranslate);
                        var tokens = TranslationHelper.SplitTokens(normalized);

                        var arabicIdx = new List<int>();
                        var arabicTks = new List<string>();
                        for (int i = 0; i < tokens.Count; i++)
                            if (TranslationHelper.ContainsArabicLetter(tokens[i])) { arabicIdx.Add(i); arabicTks.Add(tokens[i]); }

                        var outputs = new List<string>(tokens);
                        if (arabicTks.Count > 0)
                        {
                            string apiErr;
                            var romanized = TranslationHelper.RomanizeBatch(arabicTks.ToArray(), out apiErr);

                            for (int k = 0; k < arabicTks.Count; k++)
                            {
                                int idx = arabicIdx[k];
                                string cand = (romanized != null && k < romanized.Length) ? romanized[k] : null;
                                outputs[idx] = !string.IsNullOrEmpty(cand)
                                    ? cand
                                    : TranslationHelper.FallbackRomanizeToken(tokens[idx]);
                            }
                        }
                        result = string.Join(" ", outputs);
                    }
                }
                else
                {
                    response.Status = GeneralResponse.ResponseStatus.Error;
                    return Json(response, JsonRequestBehavior.AllowGet);
                }

                if (!string.IsNullOrEmpty(result))
                {
                    response.Status = GeneralResponse.ResponseStatus.Success;
                    response.Result = result;
                }
                else
                {
                    response.Status = GeneralResponse.ResponseStatus.Error;
                }
            }
            catch (Exception ex)
            {
                response.Status = GeneralResponse.ResponseStatus.Error;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        private JsonResult Json(GeneralResponse response, JsonRequestBehavior allowGet)
        {
            throw new NotImplementedException();
        }

        private void AddTranslatedParam(SqlCommand myCmd,string paramName,string englishValue,string arabicValue,bool isUpdate,string fieldName,ref StringBuilder strBuildUpdtAud,ref DataRow drowDataBeforeUpdate)
        {
            string valueToInsert;

            if (!string.IsNullOrEmpty(arabicValue))
            {
                valueToInsert = arabicValue;
            }
            else if (!string.IsNullOrEmpty(englishValue))
            {
                // Translate English → Arabic
                string jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cs-translationapiproj-321d0950989b.json");
                valueToInsert = DBox_CS.Core.AppClass.TranslationHelperV2.TranslateFromEnglishToArabic(englishValue, jsonPath);
            }
            else
            {
                valueToInsert = null;
            }

            // Add parameter to command
            if (string.IsNullOrEmpty(valueToInsert))
                myCmd.Parameters.AddWithValue(paramName, DBNull.Value);
            else
                myCmd.Parameters.AddWithValue(paramName, valueToInsert);

            // Use Build_UpdateAudErrors without breaking old code
            if (isUpdate)
            {
                // Temporary StringBuilder for this field
                StringBuilder tempAud = new StringBuilder();

                // Call Build_UpdateAudErrors
                Build_UpdateAudErrors(isUpdate, fieldName, valueToInsert, ref tempAud, ref drowDataBeforeUpdate);

                // Append to main StringBuilder
                strBuildUpdtAud.Append(tempAud);
            }
        }
    }


}
