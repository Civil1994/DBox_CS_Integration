using DBox_CS.Core.HCMS.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBox_CS.Core.DALayer.Helpers
{
    public class EmployeeHelper
    {

        #region GetEmployeeDetailsByEmpId
        public static Employee GetEmployeeDetailsByEmpId(int p_iEmpId, ref SqlConnection myConn)
        {
            Employee oEmployee = new Employee();
            //SqlConnection myConn = new SqlConnection();
            //myConn.ConnectionString = ConnectionFunctions.GetConnectionString();
            try
            {
                //myConn.Open();
                string sqry = "EAF_USP_GetEmployeeDetailsByEmpId";
                SqlCommand myCmd = new SqlCommand(sqry, myConn);
                myCmd.CommandType = CommandType.StoredProcedure;
                string sqlCommand = "EAF_USP_GetEmployeeDetailsByEmpId";

                myCmd.Parameters.AddWithValue("EmpId", p_iEmpId);

                using (SqlDataReader dataReader = myCmd.ExecuteReader())
                {

                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("EmpID")))
                                oEmployee.EmpID = (Int32)dataReader["EmpID"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("EmpCode")))
                                oEmployee.EmpCode = (String)dataReader["EmpCode"];

                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("EmpNameE")))
                                oEmployee.EmpNameE = (String)dataReader["EmpNameE"];

                            //if (!dataReader.IsDBNull(dataReader.GetOrdinal("LocLib1")))
                            //    oEmployee.LocLib1 = String.Empty;
                            //if (!dataReader.IsDBNull(dataReader.GetOrdinal("LocLib2")))
                            //    oEmployee.LocLib2 = String.Empty;
                            //if (!dataReader.IsDBNull(dataReader.GetOrdinal("LocLib3")))
                            //    oEmployee.LocLib3 = String.Empty;
                            //if (!dataReader.IsDBNull(dataReader.GetOrdinal("LocLib4")))
                            //    oEmployee.LocLib4 = String.Empty;
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("LocLib5")))
                                oEmployee.LocLib5 = (String)dataReader["LocLib5"];
                            //if (!dataReader.IsDBNull(dataReader.GetOrdinal("LocLibID")))
                            //    oEmployee.LocLibId = (String)dataReader["LocLibID"];
                            //if (!dataReader.IsDBNull(dataReader.GetOrdinal("LocLib1E")))
                            //    oEmployee.LocLib1E = (String)dataReader["LocLib1E"];
                            //if (!dataReader.IsDBNull(dataReader.GetOrdinal("LocLib2E")))
                            //    oEmployee.LocLib2E = (String)dataReader["LocLib2E"];
                            //if (!dataReader.IsDBNull(dataReader.GetOrdinal("LocLib3E")))
                            //    oEmployee.LocLib3E = (String)dataReader["LocLib3E"];
                            //if (!dataReader.IsDBNull(dataReader.GetOrdinal("LocLib4E")))
                            //    oEmployee.LocLib4E = (String)dataReader["LocLib4E"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("LocLib5E")))
                                oEmployee.LocLib5E = (String)dataReader["LocLib5E"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("SalaryProfile")))
                                oEmployee.SalaryProfile = (String)dataReader["SalaryProfile"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("salprofile")))
                                oEmployee.SalProfile = (String)dataReader["salprofile"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("JobTitleDesc")))
                                oEmployee.JobTitleDesc = (String)dataReader["JobTitleDesc"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("ResidenceNo")))
                                oEmployee.ResidenceNo = (String)dataReader["ResidenceNo"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("Sex")))
                                oEmployee.Sex = (Byte)dataReader["Sex"];
                        }
                    }

                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                //if (myConn.State != ConnectionState.Closed)
                //    myConn.Close();
            }
            return oEmployee;
        }
        #endregion

        public static Employee GetEmployee(Int32 EmpId)
        {
            SqlConnection myConn = new SqlConnection();
            myConn.ConnectionString = ConnectionFunctions.GetConnectionString();

            Employee oEmployee = new Employee();

            try
            {
                myConn.Open();
                string sqry = "EAF_USP_GetEmployee";
                SqlCommand myCmd = new SqlCommand(sqry, myConn);
                myCmd.CommandType = CommandType.StoredProcedure;
                myCmd.Parameters.AddWithValue("EmpId", EmpId);

                using (SqlDataReader dataReader = myCmd.ExecuteReader())
                {
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("EmpID")))
                                oEmployee.EmpID = (Int32)dataReader["EmpID"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("EmpCode")))
                                oEmployee.EmpCode = (String)dataReader["EmpCode"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("Title")))
                                oEmployee.Title = (String)dataReader["Title"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("EmpNameE")))
                                oEmployee.EmpNameE = (String)dataReader["EmpNameE"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("EmpNameA")))
                                oEmployee.EmpNameA = (String)dataReader["EmpNameA"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("NickNameE")))
                                oEmployee.NickNameE = (String)dataReader["NickNameE"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("NickNameA")))
                                oEmployee.NickNameA = (String)dataReader["NickNameA"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("RelType")))
                                oEmployee.RelType = (String)dataReader["RelType"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("RelNameE")))
                                oEmployee.RelNameE = (String)dataReader["RelNameE"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("RelNameA")))
                                oEmployee.RelNameA = (String)dataReader["RelNameA"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("MotherNameE")))
                                oEmployee.MotherNameE = (String)dataReader["MotherNameE"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("MotherNameA")))
                                oEmployee.MotherNameA = (String)dataReader["MotherNameA"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("FamilyNameE")))
                                oEmployee.FamilyNameE = (String)dataReader["FamilyNameE"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("FamilyNameA")))
                                oEmployee.FamilyNameA = (String)dataReader["FamilyNameA"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("Sex")))
                                oEmployee.Sex = (Byte)dataReader["Sex"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("NPresent")))
                                oEmployee.NPresent = (String)dataReader["NPresent"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("NPrevious")))
                                oEmployee.NPrevious = (String)dataReader["NPrevious"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("MaritalStat")))
                                oEmployee.MaritalStat = (Byte)dataReader["MaritalStat"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("DateOfBirth")))
                                oEmployee.DateOfBirth = (String)dataReader["DateOfBirth"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("Dobday")))
                                oEmployee.Dobday = (Byte)dataReader["Dobday"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("Dobmonth")))
                                oEmployee.Dobmonth = (Byte)dataReader["Dobmonth"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("Dobyear")))
                                oEmployee.Dobyear = (Int16)dataReader["Dobyear"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("BirthPlaceE")))
                                oEmployee.BirthPlaceE = (String)dataReader["BirthPlaceE"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("BirthPlaceA")))
                                oEmployee.BirthPlaceA = (String)dataReader["BirthPlaceA"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("CountryOfBirth")))
                                oEmployee.CountryOfBirth = (String)dataReader["CountryOfBirth"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("PassportNoE")))
                                oEmployee.PassportNoE = (String)dataReader["PassportNoE"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("PassportNoA")))
                                oEmployee.PassportNoA = (String)dataReader["PassportNoA"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("PCategory")))
                                oEmployee.PCategory = (Byte)dataReader["PCategory"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("PIssuePlaceE")))
                                oEmployee.PIssuePlaceE = (String)dataReader["PIssuePlaceE"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("PIssuePlaceA")))
                                oEmployee.PIssuePlaceA = (String)dataReader["PIssuePlaceA"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("PIssueCountry")))
                                oEmployee.PIssueCountry = (String)dataReader["PIssueCountry"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("PIssueDate")))
                                oEmployee.PIssueDate = (DateTime)dataReader["PIssueDate"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("PExpiryDate")))
                                oEmployee.PExpiryDate = (DateTime)dataReader["PExpiryDate"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("Religion")))
                                oEmployee.Religion = (String)dataReader["Religion"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("PassportProf")))
                                oEmployee.PassportProf = (String)dataReader["PassportProf"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("Education")))
                                oEmployee.Education = (String)dataReader["Education"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("PerAddressE")))
                                oEmployee.PerAddressE = (String)dataReader["PerAddressE"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("PerAddressA")))
                                oEmployee.PerAddressA = (String)dataReader["PerAddressA"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("Skill1")))
                                oEmployee.Skill1 = (String)dataReader["Skill1"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("Skill2")))
                                oEmployee.Skill2 = (String)dataReader["Skill2"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("Skill3")))
                                oEmployee.Skill3 = (String)dataReader["Skill3"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("Language1")))
                                oEmployee.Language1 = (String)dataReader["Language1"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("Language2")))
                                oEmployee.Language2 = (String)dataReader["Language2"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("Language3")))
                                oEmployee.Language3 = (String)dataReader["Language3"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("VisaType")))
                                oEmployee.VisaType = (String)dataReader["VisaType"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("VisaNo")))
                                oEmployee.VisaNo = (String)dataReader["VisaNo"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("VisaIssueDate")))
                                oEmployee.VisaIssueDate = (DateTime)dataReader["VisaIssueDate"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("ImmigFileNo")))
                                oEmployee.ImmigFileNo = (String)dataReader["ImmigFileNo"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("EntryPlace")))
                                oEmployee.EntryPlace = (String)dataReader["EntryPlace"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("EntryDate")))
                                oEmployee.EntryDate = (DateTime)dataReader["EntryDate"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("ResidenceNo")))
                                oEmployee.ResidenceNo = (String)dataReader["ResidenceNo"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("ResIssueDate")))
                                oEmployee.ResIssueDate = (DateTime)dataReader["ResIssueDate"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("ResExpDate")))
                                oEmployee.ResExpDate = (DateTime)dataReader["ResExpDate"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("ResIssuePlace")))
                                oEmployee.ResIssuePlace = (String)dataReader["ResIssuePlace"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("LabCardNo")))
                                oEmployee.LabCardNo = (String)dataReader["LabCardNo"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("LCIssueDate")))
                                oEmployee.LCIssueDate = (DateTime)dataReader["LCIssueDate"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("LCExpDate")))
                                oEmployee.LCExpDate = (DateTime)dataReader["LCExpDate"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("HlthCardNo")))
                                oEmployee.HlthCardNo = (String)dataReader["HlthCardNo"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("HCIssuePlace")))
                                oEmployee.HCIssuePlace = (String)dataReader["HCIssuePlace"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("HCIssueDate")))
                                oEmployee.HCIssueDate = (DateTime)dataReader["HCIssueDate"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("HCExpiryDate")))
                                oEmployee.HCExpiryDate = (DateTime)dataReader["HCExpiryDate"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("DrvLicNo")))
                                oEmployee.DrvLicNo = (String)dataReader["DrvLicNo"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("DLCategory")))
                                oEmployee.DLCategory = (Byte)dataReader["DLCategory"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("DLIssuePlace")))
                                oEmployee.DLIssuePlace = (String)dataReader["DLIssuePlace"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("DLIssueDate")))
                                oEmployee.DLIssueDate = (DateTime)dataReader["DLIssueDate"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("DLExpiryDate")))
                                oEmployee.DLExpiryDate = (DateTime)dataReader["DLExpiryDate"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("SponsorCode")))
                                oEmployee.SponsorCode = (String)dataReader["SponsorCode"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("SponByOther")))
                                oEmployee.SponByOther = (Byte)dataReader["SponByOther"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("OSponNameE")))
                                oEmployee.OSponNameE = (String)dataReader["OSponNameE"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("OSponNameA")))
                                oEmployee.OSponNameA = (String)dataReader["OSponNameA"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("OSponRel")))
                                oEmployee.OSponRel = (String)dataReader["OSponRel"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("OSponNation")))
                                oEmployee.OSponNation = (String)dataReader["OSponNation"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("OSponVisaNo")))
                                oEmployee.OSponVisaNo = (String)dataReader["OSponVisaNo"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("OSponVExpDt")))
                                oEmployee.OSponVExpDt = (DateTime)dataReader["OSponVExpDt"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("OSponPsprtNoE")))
                                oEmployee.OSponPsprtNoE = (String)dataReader["OSponPsprtNoE"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("OSponPsprtNoA")))
                                oEmployee.OSponPsprtNoA = (String)dataReader["OSponPsprtNoA"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("ExperienceE")))
                                oEmployee.ExperienceE = (String)dataReader["ExperienceE"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("ExperienceA")))
                                oEmployee.ExperienceA = (String)dataReader["ExperienceA"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("Emirates")))
                                oEmployee.Emirates = (String)dataReader["Emirates"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("City")))
                                oEmployee.City = (String)dataReader["City"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("Area")))
                                oEmployee.Area = (String)dataReader["Area"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("StreetE")))
                                oEmployee.StreetE = (String)dataReader["StreetE"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("StreetA")))
                                oEmployee.StreetA = (String)dataReader["StreetA"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("BuildingE")))
                                oEmployee.BuildingE = (String)dataReader["BuildingE"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("BuildingA")))
                                oEmployee.BuildingA = (String)dataReader["BuildingA"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("FlatE")))
                                oEmployee.FlatE = (String)dataReader["FlatE"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("FlatA")))
                                oEmployee.FlatA = (String)dataReader["FlatA"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("OffPhoneNo")))
                                oEmployee.OffPhoneNo = (String)dataReader["OffPhoneNo"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("Ext")))
                                oEmployee.Ext = (String)dataReader["Ext"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("ResPhoneNo")))
                                oEmployee.ResPhoneNo = (String)dataReader["ResPhoneNo"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("POBox")))
                                oEmployee.POBox = (String)dataReader["POBox"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("MobileNo")))
                                oEmployee.MobileNo = (String)dataReader["MobileNo"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("PagerNo")))
                                oEmployee.PagerNo = (String)dataReader["PagerNo"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("TeleNoAbroad")))
                                oEmployee.TeleNoAbroad = (String)dataReader["TeleNoAbroad"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("Email")))
                                oEmployee.Email = (String)dataReader["Email"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("BloodGroup")))
                                oEmployee.BloodGroup = (String)dataReader["BloodGroup"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("FaxNo")))
                                oEmployee.FaxNo = (String)dataReader["FaxNo"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("NextofKinE")))
                                oEmployee.NextofKinE = (String)dataReader["NextofKinE"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("NextofKinA")))
                                oEmployee.NextofKinA = (String)dataReader["NextofKinA"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("NextofKinAddrE")))
                                oEmployee.NextofKinAddrE = (String)dataReader["NextofKinAddrE"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("NextofKinAddrA")))
                                oEmployee.NextofKinAddrA = (String)dataReader["NextofKinAddrA"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("AddressE")))
                                oEmployee.AddressE = (String)dataReader["AddressE"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("AddressA")))
                                oEmployee.AddressA = (String)dataReader["AddressA"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("EOSDate")))
                                oEmployee.EOSDate = (DateTime)dataReader["EOSDate"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("ReasonForEOSE")))
                                oEmployee.ReasonForEOSE = (String)dataReader["ReasonForEOSE"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("ReasonForEOSA")))
                                oEmployee.ReasonForEOSA = (String)dataReader["ReasonForEOSA"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("AuxString1")))
                                oEmployee.AuxString1 = (String)dataReader["AuxString1"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("AuxString2")))
                                oEmployee.AuxString2 = (String)dataReader["AuxString2"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("AuxString3")))
                                oEmployee.AuxString3 = (String)dataReader["AuxString3"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("AuxString4")))
                                oEmployee.AuxString4 = (String)dataReader["AuxString4"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("AuxString5")))
                                oEmployee.AuxString5 = (String)dataReader["AuxString5"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("AuxString6")))
                                oEmployee.AuxString6 = (String)dataReader["AuxString6"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("AuxString7")))
                                oEmployee.AuxString7 = (String)dataReader["AuxString7"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("AuxString8")))
                                oEmployee.AuxString8 = (String)dataReader["AuxString8"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("AuxString9")))
                                oEmployee.AuxString9 = (String)dataReader["AuxString9"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("AuxString10")))
                                oEmployee.AuxString10 = (String)dataReader["AuxString10"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("AuxAString1")))
                                oEmployee.AuxAString1 = (String)dataReader["AuxAString1"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("AuxAString2")))
                                oEmployee.AuxAString2 = (String)dataReader["AuxAString2"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("AuxAString3")))
                                oEmployee.AuxAString3 = (String)dataReader["AuxAString3"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("AuxAString4")))
                                oEmployee.AuxAString4 = (String)dataReader["AuxAString4"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("AuxAString5")))
                                oEmployee.AuxAString5 = (String)dataReader["AuxAString5"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("AuxAString6")))
                                oEmployee.AuxAString6 = (String)dataReader["AuxAString6"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("AuxAString7")))
                                oEmployee.AuxAString7 = (String)dataReader["AuxAString7"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("AuxAString8")))
                                oEmployee.AuxAString8 = (String)dataReader["AuxAString8"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("AuxAString9")))
                                oEmployee.AuxAString9 = (String)dataReader["AuxAString9"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("AuxAString10")))
                                oEmployee.AuxAString10 = (String)dataReader["AuxAString10"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("AuxInt1")))
                                oEmployee.AuxInt1 = (Int16)dataReader["AuxInt1"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("AuxInt2")))
                                oEmployee.AuxInt2 = (Int16)dataReader["AuxInt2"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("AuxInt3")))
                                oEmployee.AuxInt3 = (Int16)dataReader["AuxInt3"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("AuxCurrency1")))
                                oEmployee.AuxCurrency1 = (Decimal)dataReader["AuxCurrency1"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("AuxCurrency2")))
                                oEmployee.AuxCurrency2 = Convert.ToDecimal(dataReader["AuxCurrency2"]);
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("AuxDate1")))
                                oEmployee.AuxDate1 = (DateTime)dataReader["AuxDate1"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("AuxDate2")))
                                oEmployee.AuxDate2 = (DateTime)dataReader["AuxDate2"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("AuxDate3")))
                                oEmployee.AuxDate3 = (DateTime)dataReader["AuxDate3"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("AuxDate4")))
                                oEmployee.AuxDate4 = (DateTime)dataReader["AuxDate4"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("AuxDate5")))
                                oEmployee.AuxDate5 = (DateTime)dataReader["AuxDate5"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("AuxLib1")))
                                oEmployee.AuxLib1 = (String)dataReader["AuxLib1"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("AuxLib2")))
                                oEmployee.AuxLib2 = (String)dataReader["AuxLib2"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("AuxLib3")))
                                oEmployee.AuxLib3 = (String)dataReader["AuxLib3"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("AuxLib4")))
                                oEmployee.AuxLib4 = (String)dataReader["AuxLib4"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("IntlJoiningDate")))
                                oEmployee.IntlJoiningDate = (DateTime)dataReader["IntlJoiningDate"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("PointOfHireE")))
                                oEmployee.PointOfHireE = (String)dataReader["PointOfHireE"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("PointOfHireA")))
                                oEmployee.PointOfHireA = (String)dataReader["PointOfHireA"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("SourceOfHire")))
                                oEmployee.SourceOfHire = (String)dataReader["SourceOfHire"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("SLReInitDate")))
                                oEmployee.SLReInitDate = (DateTime)dataReader["SLReInitDate"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("StartDtofIndemnity")))
                                oEmployee.StartDtofIndemnity = (DateTime)dataReader["StartDtofIndemnity"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("LastAppraisalDate")))
                                oEmployee.LastAppraisalDate = (DateTime)dataReader["LastAppraisalDate"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("NextAppraisalDate")))
                                oEmployee.NextAppraisalDate = (DateTime)dataReader["NextAppraisalDate"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("SalaryStatus")))
                                oEmployee.SalaryStatus = (Int16)dataReader["SalaryStatus"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("EmployeeStatus")))
                                oEmployee.EmployeeStatus = (Byte)dataReader["EmployeeStatus"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("LocLib5")))
                                oEmployee.LocLib5 = (String)dataReader["LocLib5"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("WrkAgreeNo")))
                                oEmployee.WrkAgreeNo = (String)dataReader["WrkAgreeNo"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("TransferredFromVisitor")))
                                oEmployee.TransferredFromVisitor = (Boolean)dataReader["TransferredFromVisitor"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("LastModUID")))
                                oEmployee.LastModUID = (String)dataReader["LastModUID"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("LastModDateTime")))
                                oEmployee.LastModDateTime = (DateTime)dataReader["LastModDateTime"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("LeaveDate")))
                                oEmployee.LeaveDate = (DateTime)dataReader["LeaveDate"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("LockedByUser")))
                                oEmployee.LockedByUser = (Int32)dataReader["LockedByUser"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("SalProfile")))
                                oEmployee.SalProfile = (String)dataReader["SalProfile"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("RecCreatedUID")))
                                oEmployee.RecCreatedUID = (String)dataReader["RecCreatedUID"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("RecCreatedDateTime")))
                                oEmployee.RecCreatedDateTime = (DateTime)dataReader["RecCreatedDateTime"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("HealthInsurCmp")))
                                oEmployee.HealthInsurCmp = (String)dataReader["HealthInsurCmp"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("LCIssuePlace")))
                                oEmployee.LCIssuePlace = (String)dataReader["LCIssuePlace"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("ConfirmedYN")))
                                oEmployee.ConfirmedYN = (Boolean)dataReader["ConfirmedYN"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("FNameE")))
                                oEmployee.FNameE = (String)dataReader["FNameE"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("FNameA")))
                                oEmployee.FNameA = (String)dataReader["FNameA"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("SNameE")))
                                oEmployee.SNameE = (String)dataReader["SNameE"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("SNameA")))
                                oEmployee.SNameA = (String)dataReader["SNameA"];
                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("PersEmail")))   //Nishad Added 17052021
                                oEmployee.PersEmail = (String)dataReader["PersEmail"];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (myConn.State != ConnectionState.Closed)
                    myConn.Close();
            }



            return oEmployee;
        }
    }
}
