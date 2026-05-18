using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DBox_CS.Core.HCMS.Entity
{
    [DataContract]
    [Serializable]
    public class Employee
    {
        [DataMember]
        public Int32 EmpID { get; set; }
        [DataMember]
        public String EmpCode { get; set; }
        [DataMember]
        public String Title { get; set; }
        [DataMember]
        public String EmpNameE { get; set; }
        [DataMember]
        public String EmpNameA { get; set; }
        [DataMember]
        public String NickNameE { get; set; }
        [DataMember]
        public String NickNameA { get; set; }
        [DataMember]
        public String RelType { get; set; }
        [DataMember]
        public String RelNameE { get; set; }
        [DataMember]
        public String RelNameA { get; set; }
        [DataMember]
        public String MotherNameE { get; set; }
        [DataMember]
        public String MotherNameA { get; set; }
        [DataMember]
        public String FamilyNameE { get; set; }
        [DataMember]
        public String FamilyNameA { get; set; }
        [DataMember]
        public Byte Sex { get; set; }
        [DataMember]
        public String NPresent { get; set; }
        [DataMember]
        public String NPrevious { get; set; }
        [DataMember]
        public Byte MaritalStat { get; set; }
        [DataMember]
        public String DateOfBirth { get; set; }
        [DataMember]
        public Byte Dobday { get; set; }
        [DataMember]
        public Byte Dobmonth { get; set; }
        [DataMember]
        public Int16 Dobyear { get; set; }
        [DataMember]
        public String BirthPlaceE { get; set; }
        [DataMember]
        public String BirthPlaceA { get; set; }
        [DataMember]
        public String CountryOfBirth { get; set; }
        [DataMember]
        public String PassportNoE { get; set; }
        [DataMember]
        public String PassportNoA { get; set; }
        [DataMember]
        public Byte PCategory { get; set; }
        [DataMember]
        public String PIssuePlaceE { get; set; }
        [DataMember]
        public String PIssuePlaceA { get; set; }
        [DataMember]
        public String PIssueCountry { get; set; }
        [DataMember]
        public DateTime PIssueDate { get; set; }
        [DataMember]
        public DateTime PExpiryDate { get; set; }
        [DataMember]
        public String Religion { get; set; }
        [DataMember]
        public String PassportProf { get; set; }
        [DataMember]
        public String Education { get; set; }
        [DataMember]
        public String PerAddressE { get; set; }
        [DataMember]
        public String PerAddressA { get; set; }
        [DataMember]
        public String Skill1 { get; set; }
        [DataMember]
        public String Skill2 { get; set; }
        [DataMember]
        public String Skill3 { get; set; }
        [DataMember]
        public String Language1 { get; set; }
        [DataMember]
        public String Language2 { get; set; }
        [DataMember]
        public String Language3 { get; set; }
        [DataMember]
        public String VisaType { get; set; }
        [DataMember]
        public String VisaNo { get; set; }
        [DataMember]
        public DateTime VisaIssueDate { get; set; }
        [DataMember]
        public String ImmigFileNo { get; set; }
        [DataMember]
        public String EntryPlace { get; set; }
        [DataMember]
        public DateTime EntryDate { get; set; }
        [DataMember]
        public String ResidenceNo { get; set; }
        [DataMember]
        public DateTime ResIssueDate { get; set; }
        [DataMember]
        public DateTime ResExpDate { get; set; }
        [DataMember]
        public String ResIssuePlace { get; set; }
        [DataMember]
        public String LabCardNo { get; set; }
        [DataMember]
        public DateTime LCIssueDate { get; set; }
        [DataMember]
        public DateTime LCExpDate { get; set; }
        [DataMember]
        public String HlthCardNo { get; set; }
        [DataMember]
        public String HCIssuePlace { get; set; }
        [DataMember]
        public DateTime HCIssueDate { get; set; }
        [DataMember]
        public DateTime HCExpiryDate { get; set; }
        [DataMember]
        public String DrvLicNo { get; set; }
        [DataMember]
        public Byte DLCategory { get; set; }
        [DataMember]
        public String DLIssuePlace { get; set; }
        [DataMember]
        public DateTime DLIssueDate { get; set; }
        [DataMember]
        public DateTime DLExpiryDate { get; set; }
        [DataMember]
        public String SponsorCode { get; set; }
        [DataMember]
        public Byte SponByOther { get; set; }
        [DataMember]
        public String OSponNameE { get; set; }
        [DataMember]
        public String OSponNameA { get; set; }
        [DataMember]
        public String OSponRel { get; set; }
        [DataMember]
        public String OSponNation { get; set; }
        [DataMember]
        public String OSponVisaNo { get; set; }
        [DataMember]
        public DateTime OSponVExpDt { get; set; }
        [DataMember]
        public String OSponPsprtNoE { get; set; }
        [DataMember]
        public String OSponPsprtNoA { get; set; }
        [DataMember]
        public String ExperienceE { get; set; }
        [DataMember]
        public String ExperienceA { get; set; }
        [DataMember]
        public String Emirates { get; set; }
        [DataMember]
        public String City { get; set; }
        [DataMember]
        public String Area { get; set; }
        [DataMember]
        public String StreetE { get; set; }
        [DataMember]
        public String StreetA { get; set; }
        [DataMember]
        public String BuildingE { get; set; }
        [DataMember]
        public String BuildingA { get; set; }
        [DataMember]
        public String FlatE { get; set; }
        [DataMember]
        public String FlatA { get; set; }
        [DataMember]
        public String OffPhoneNo { get; set; }
        [DataMember]
        public String Ext { get; set; }
        [DataMember]
        public String ResPhoneNo { get; set; }
        [DataMember]
        public String POBox { get; set; }
        [DataMember]
        public String MobileNo { get; set; }
        [DataMember]
        public String PagerNo { get; set; }
        [DataMember]
        public String TeleNoAbroad { get; set; }
        [DataMember]
        public String Email { get; set; }
        [DataMember]
        public String BloodGroup { get; set; }
        [DataMember]
        public String FaxNo { get; set; }
        [DataMember]
        public String NextofKinE { get; set; }
        [DataMember]
        public String NextofKinA { get; set; }
        [DataMember]
        public String NextofKinAddrE { get; set; }
        [DataMember]
        public String NextofKinAddrA { get; set; }
        [DataMember]
        public String AddressE { get; set; }
        [DataMember]
        public String AddressA { get; set; }
        [DataMember]
        public DateTime EOSDate { get; set; }
        [DataMember]
        public String ReasonForEOSE { get; set; }
        [DataMember]
        public String ReasonForEOSA { get; set; }
        [DataMember]
        public String AuxString1 { get; set; }
        [DataMember]
        public String AuxString2 { get; set; }
        [DataMember]
        public String AuxString3 { get; set; }
        [DataMember]
        public String AuxString4 { get; set; }
        [DataMember]
        public String AuxString5 { get; set; }
        [DataMember]
        public String AuxString6 { get; set; }
        [DataMember]
        public String AuxString7 { get; set; }
        [DataMember]
        public String AuxString8 { get; set; }
        [DataMember]
        public String AuxString9 { get; set; }
        [DataMember]
        public String AuxString10 { get; set; }
        [DataMember]
        public String AuxAString1 { get; set; }
        [DataMember]
        public String AuxAString2 { get; set; }
        [DataMember]
        public String AuxAString3 { get; set; }
        [DataMember]
        public String AuxAString4 { get; set; }
        [DataMember]
        public String AuxAString5 { get; set; }
        [DataMember]
        public String AuxAString6 { get; set; }
        [DataMember]
        public String AuxAString7 { get; set; }
        [DataMember]
        public String AuxAString8 { get; set; }
        [DataMember]
        public String AuxAString9 { get; set; }
        [DataMember]
        public String AuxAString10 { get; set; }
        [DataMember]
        public Int16 AuxInt1 { get; set; }
        [DataMember]
        public Int16 AuxInt2 { get; set; }
        [DataMember]
        public Int16 AuxInt3 { get; set; }
        [DataMember]
        public Decimal AuxCurrency1 { get; set; }
        [DataMember]
        public Decimal AuxCurrency2 { get; set; }
        [DataMember]
        public DateTime AuxDate1 { get; set; }
        [DataMember]
        public DateTime AuxDate2 { get; set; }
        [DataMember]
        public DateTime AuxDate3 { get; set; }
        [DataMember]
        public DateTime AuxDate4 { get; set; }
        [DataMember]
        public DateTime AuxDate5 { get; set; }
        [DataMember]
        public String AuxLib1 { get; set; }
        [DataMember]
        public String AuxLib2 { get; set; }
        [DataMember]
        public String AuxLib3 { get; set; }
        [DataMember]
        public String AuxLib4 { get; set; }
        [DataMember]
        public DateTime IntlJoiningDate { get; set; }
        [DataMember]
        public String PointOfHireE { get; set; }
        [DataMember]
        public String PointOfHireA { get; set; }
        [DataMember]
        public String SourceOfHire { get; set; }
        [DataMember]
        public DateTime SLReInitDate { get; set; }
        [DataMember]
        public DateTime StartDtofIndemnity { get; set; }
        [DataMember]
        public DateTime LastAppraisalDate { get; set; }
        [DataMember]
        public DateTime NextAppraisalDate { get; set; }
        [DataMember]
        public Int16 SalaryStatus { get; set; }
        [DataMember]
        public Byte EmployeeStatus { get; set; }
        [DataMember]
        public String LocLibId { get; set; }
        [DataMember]
        public String LocLib1 { get; set; }
        [DataMember]
        public String LocLib2 { get; set; }
        [DataMember]
        public String LocLib3 { get; set; }
        [DataMember]
        public String LocLib4 { get; set; }
        [DataMember]
        public String LocLib5 { get; set; }
        [DataMember]
        public String WrkAgreeNo { get; set; }
        [DataMember]
        public Boolean TransferredFromVisitor { get; set; }
        [DataMember]
        public String LastModUID { get; set; }
        [DataMember]
        public DateTime LastModDateTime { get; set; }
        [DataMember]
        public DateTime LeaveDate { get; set; }
        [DataMember]
        public Int32 LockedByUser { get; set; }
        [DataMember]
        public String SalProfile { get; set; }
        [DataMember]
        public String RecCreatedUID { get; set; }
        [DataMember]
        public DateTime RecCreatedDateTime { get; set; }
        [DataMember]
        public String HealthInsurCmp { get; set; }
        [DataMember]
        public String LCIssuePlace { get; set; }
        [DataMember]
        public Boolean ConfirmedYN { get; set; }
        [DataMember]
        public String FNameE { get; set; }
        [DataMember]
        public String FNameA { get; set; }
        [DataMember]
        public String SNameE { get; set; }
        [DataMember]
        public String SNameA { get; set; }
        [DataMember]
        public Int16 SponTypeExtnl { get; set; }
        [DataMember]
        public String CodeNameE { get; set; }
        [DataMember]
        public String CodeNameA { get; set; }

        #region "Leave Screen Columns"

        [DataMember]
        public string JobTitleId { get; set; }
        [DataMember]
        public string JobTitleDesc { get; set; }
        [DataMember]
        public DateTime LastPaidDate { get; set; }
        [DataMember]
        public DateTime JoiningDate { get; set; }
        [DataMember]
        public float LeaveBals { get; set; }
        [DataMember]
        public float SickLeave { get; set; }
        [DataMember]
        public float UpdAvailBal { get; set; }
        [DataMember]
        public float ExtraBal { get; set; }
        [DataMember]
        public float ExtraHrs { get; set; }
        [DataMember]
        public float AlCurrYr { get; set; }
        [DataMember]
        public string AnnualEnt { get; set; }
        [DataMember]
        public int ExludeOFFsFromLVPeriod { get; set; }
        [DataMember]
        public int ExludePHFromLVPeriod { get; set; }

        [DataMember]
        public string LocLib1E { get; set; }
        [DataMember]
        public string LocLib2E { get; set; }
        [DataMember]
        public string LocLib3E { get; set; }
        [DataMember]
        public string LocLib4E { get; set; }
        [DataMember]
        public string LocLib5E { get; set; }

        [DataMember]
        public string LocLib1A { get; set; }
        [DataMember]
        public string LocLib2A { get; set; }
        [DataMember]
        public string LocLib3A { get; set; }
        [DataMember]
        public string LocLib4A { get; set; }
        [DataMember]
        public string LocLib5A { get; set; }

        [DataMember]
        public string STHRS { get; set; }



        #endregion

        #region ExitReentry Coloumns
        [DataMember]
        public String SalaryProfile { get; set; }
        #endregion

        [DataMember]
        public Single ProbPeriod { get; set; }

        [DataMember]
        public Int16 PymntMode { get; set; }
        [DataMember]
        public String PersEmail { get; set; }
        [DataMember]
        public String SP { get; set; }  //Nishad Added 10012024
    }

    [DataContract]
    [Serializable]
    public class EmployeeViewAll
    {
        [DataMember]
        public long RowNo { get; set; }
        [DataMember]
        public Int32 EmpID { get; set; }
        [DataMember]
        public string EncryptedEmpId { get; set; }
        [DataMember]
        public String EmpCode { get; set; }
        [DataMember]
        public String Title { get; set; }
        [DataMember]
        public String EmpNameE { get; set; }
        [DataMember]
        public String EmpNameA { get; set; }
        [DataMember]
        public String NickNameE { get; set; }
        [DataMember]
        public String NickNameA { get; set; }
        [DataMember]
        public String RelType { get; set; }
        [DataMember]
        public String RelNameE { get; set; }
        [DataMember]
        public String RelNameA { get; set; }
        [DataMember]
        public String MotherNameE { get; set; }
        [DataMember]
        public String MotherNameA { get; set; }
        [DataMember]
        public String FamilyNameE { get; set; }
        [DataMember]
        public String FamilyNameA { get; set; }
        [DataMember]
        public Byte Sex { get; set; }
        [DataMember]
        public String NPresent { get; set; }
        [DataMember]
        public String NPrevious { get; set; }
        [DataMember]
        public Byte MaritalStat { get; set; }
        [DataMember]
        public String DateOfBirth { get; set; }
        [DataMember]
        public Byte Dobday { get; set; }
        [DataMember]
        public Byte Dobmonth { get; set; }
        [DataMember]
        public Int16 Dobyear { get; set; }
        [DataMember]
        public String BirthPlaceE { get; set; }
        [DataMember]
        public String BirthPlaceA { get; set; }
        [DataMember]
        public String CountryOfBirth { get; set; }
        [DataMember]
        public String PassportNoE { get; set; }
        [DataMember]
        public String PassportNoA { get; set; }
        [DataMember]
        public Byte PCategory { get; set; }
        [DataMember]
        public String PIssuePlaceE { get; set; }
        [DataMember]
        public String PIssuePlaceA { get; set; }
        [DataMember]
        public String PIssueCountry { get; set; }
        [DataMember]
        public DateTime PIssueDate { get; set; }
        [DataMember]
        public DateTime PExpiryDate { get; set; }
        [DataMember]
        public String Religion { get; set; }
        [DataMember]
        public String ReligionA { get; set; }
        [DataMember]
        public String PassportProf { get; set; }
        [DataMember]
        public String PassportProfA { get; set; }
        [DataMember]
        public String Education { get; set; }
        [DataMember]
        public String PerAddressE { get; set; }
        [DataMember]
        public String PerAddressA { get; set; }
        [DataMember]
        public String Skill1 { get; set; }
        [DataMember]
        public String Skill2 { get; set; }
        [DataMember]
        public String Skill3 { get; set; }
        [DataMember]
        public String Language1 { get; set; }
        [DataMember]
        public String Language2 { get; set; }
        [DataMember]
        public String Language3 { get; set; }
        [DataMember]
        public String VisaType { get; set; }

        [DataMember]
        public String VisaTypeA { get; set; }
        [DataMember]
        public String VisaNo { get; set; }
        [DataMember]
        public DateTime VisaIssueDate { get; set; }
        [DataMember]
        public String ImmigFileNo { get; set; }
        [DataMember]
        public String EntryPlace { get; set; }
        [DataMember]
        public DateTime EntryDate { get; set; }
        [DataMember]
        public String ResidenceNo { get; set; }
        [DataMember]
        public DateTime ResIssueDate { get; set; }
        [DataMember]
        public DateTime ResExpDate { get; set; }
        [DataMember]
        public String ResIssuePlace { get; set; }
        [DataMember]
        public String LabCardNo { get; set; }
        [DataMember]
        public DateTime LCIssueDate { get; set; }
        [DataMember]
        public DateTime LCExpDate { get; set; }
        [DataMember]
        public String HlthCardNo { get; set; }
        [DataMember]
        public String HCIssuePlace { get; set; }
        [DataMember]
        public DateTime HCIssueDate { get; set; }
        [DataMember]
        public DateTime HCExpiryDate { get; set; }
        [DataMember]
        public String DrvLicNo { get; set; }
        [DataMember]
        public Byte DLCategory { get; set; }
        [DataMember]
        public String DLIssuePlace { get; set; }
        [DataMember]
        public DateTime DLIssueDate { get; set; }
        [DataMember]
        public DateTime DLExpiryDate { get; set; }
        [DataMember]
        public String SponsorCode { get; set; }
        [DataMember]
        public String SponByOther { get; set; }
        [DataMember]
        public String OSponNameE { get; set; }
        [DataMember]
        public String OSponNameA { get; set; }
        [DataMember]
        public String OSponRel { get; set; }
        [DataMember]
        public String OSponNation { get; set; }
        [DataMember]
        public String OSponVisaNo { get; set; }
        [DataMember]
        public DateTime OSponVExpDt { get; set; }
        [DataMember]
        public String OSponPsprtNoE { get; set; }
        [DataMember]
        public String OSponPsprtNoA { get; set; }
        [DataMember]
        public String ExperienceE { get; set; }
        [DataMember]
        public String ExperienceA { get; set; }
        [DataMember]
        public String Emirates { get; set; }
        [DataMember]
        public String City { get; set; }
        [DataMember]
        public String Area { get; set; }
        [DataMember]
        public String StreetE { get; set; }
        [DataMember]
        public String StreetA { get; set; }
        [DataMember]
        public String BuildingE { get; set; }
        [DataMember]
        public String BuildingA { get; set; }
        [DataMember]
        public String FlatE { get; set; }
        [DataMember]
        public String FlatA { get; set; }
        [DataMember]
        public String OffPhoneNo { get; set; }
        [DataMember]
        public String Ext { get; set; }
        [DataMember]
        public String ResPhoneNo { get; set; }
        [DataMember]
        public String POBox { get; set; }
        [DataMember]
        public String MobileNo { get; set; }
        [DataMember]
        public String PagerNo { get; set; }
        [DataMember]
        public String TeleNoAbroad { get; set; }
        [DataMember]
        public String Email { get; set; }
        [DataMember]
        public String BloodGroup { get; set; }
        [DataMember]
        public String FaxNo { get; set; }
        [DataMember]
        public String NextofKinE { get; set; }
        [DataMember]
        public String NextofKinA { get; set; }
        [DataMember]
        public String NextofKinAddrE { get; set; }
        [DataMember]
        public String NextofKinAddrA { get; set; }
        [DataMember]
        public String AddressE { get; set; }
        [DataMember]
        public String AddressA { get; set; }
        [DataMember]
        public DateTime EOSDate { get; set; }
        [DataMember]
        public String ReasonForEOSE { get; set; }
        [DataMember]
        public String ReasonForEOSA { get; set; }
        [DataMember]
        public DateTime IntlJoiningDate { get; set; }
        [DataMember]
        public String PointOfHireE { get; set; }
        [DataMember]
        public String PointOfHireA { get; set; }
        [DataMember]
        public String SourceOfHire { get; set; }
        [DataMember]
        public DateTime SLReInitDate { get; set; }
        [DataMember]
        public DateTime StartDtofIndemnity { get; set; }
        [DataMember]
        public DateTime LastAppraisalDate { get; set; }
        [DataMember]
        public DateTime NextAppraisalDate { get; set; }
        [DataMember]
        public Int16 SalaryStatus { get; set; }
        [DataMember]
        public String EmployeeStatus { get; set; }

        [DataMember]
        public String EmployeeStatusA { get; set; }

        [DataMember]
        public String LocLibId { get; set; }
        [DataMember]
        public String LocLib1 { get; set; }
        [DataMember]
        public String LocLib2 { get; set; }
        [DataMember]
        public String LocLib3 { get; set; }
        [DataMember]
        public String LocLib4 { get; set; }
        [DataMember]
        public String LocLib5 { get; set; }
        [DataMember]
        public String WrkAgreeNo { get; set; }
        [DataMember]
        public Boolean TransferredFromVisitor { get; set; }
        [DataMember]
        public String LastModUID { get; set; }
        [DataMember]
        public DateTime LastModDateTime { get; set; }
        [DataMember]
        public DateTime LeaveDate { get; set; }
        [DataMember]
        public Int32 LockedByUser { get; set; }
        [DataMember]
        public String SalProfile { get; set; }
        [DataMember]
        public String RecCreatedUID { get; set; }
        [DataMember]
        public DateTime RecCreatedDateTime { get; set; }
        [DataMember]
        public String HealthInsurCmp { get; set; }
        [DataMember]
        public String LCIssuePlace { get; set; }
        [DataMember]
        public Boolean ConfirmedYN { get; set; }
        [DataMember]
        public String FNameE { get; set; }
        [DataMember]
        public String FNameA { get; set; }
        [DataMember]
        public String SNameE { get; set; }
        [DataMember]
        public String SNameA { get; set; }
        [DataMember]
        public string LocLib1E { get; set; }
        [DataMember]
        public string LocLib2E { get; set; }
        [DataMember]
        public string LocLib3E { get; set; }
        [DataMember]
        public string LocLib4E { get; set; }
        [DataMember]
        public string LocLib5E { get; set; }
        [DataMember]
        public string LocLib1A { get; set; }
        [DataMember]
        public string LocLib2A { get; set; }
        [DataMember]
        public string LocLib3A { get; set; }
        [DataMember]
        public string LocLib4A { get; set; }
        [DataMember]
        public string LocLib5A { get; set; }
        [DataMember]
        public String SalaryProfile { get; set; }

        [DataMember]
        public String SalaryProfileA { get; set; }
        [DataMember]
        public String PresentNationality { get; set; }

        [DataMember]
        public String PresentNationalityA { get; set; }

        [DataMember]
        public String EmpPhoto { get; set; }
        [DataMember]
        public Byte[] bEmpPhoto { get; set; }       
    }



}
