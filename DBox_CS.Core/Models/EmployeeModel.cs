using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace DBox_CS.Core.Models
{
    public class EmployeeModel
    {
        [DataMember(Name = "EmployeeID")]
        public string EmployeeID { get; set; }

        [DataMember(Name = "WorkingCompany")]
        public string WorkingCompany { get; set; }

        [DataMember(Name = "EmployeeTitle")]
        public string EmployeeTitle { get; set; }

        [DataMember(Name = "FullNameE")]
        public string FullNameE { get; set; }

        [DataMember(Name = "FirstName")]
        public string FirstName { get; set; }

        [DataMember(Name = "MiddleName")]
        public string MiddleName { get; set; }

        [DataMember(Name = "ThirdName")]
        public string ThirdName { get; set; }

        [DataMember(Name = "FourthName")]
        public string FourthName { get; set; }

        [DataMember(Name = "FamilyName")]
        public string FamilyName { get; set; }

        [DataMember(Name = "MotherNameE")]
        public string MotherNameE { get; set; }

        [DataMember(Name = "Gender")]
        public string Gender { get; set; }

        [DataMember(Name = "Religion")]
        public string Religion { get; set; }

        [DataMember(Name = "Faith")]
        public string Faith { get; set; }

        [DataMember(Name = "DateOfBirth")]
        public string DateOfBirth { get; set; }

        [DataMember(Name = "MaritalStatus")]
        public string MaritalStatus { get; set; }

        [DataMember(Name = "CountryOfBirth")]
        public string CountryOfBirth { get; set; }

        [DataMember(Name = "BirthPlaceE")]
        public string BirthPlaceE { get; set; }

        [DataMember(Name = "PassportNo")]
        public string PassportNo { get; set; }

        [DataMember(Name = "PassportCategory")]
        public string PassportCategory { get; set; }

        [DataMember(Name = "PassportIssueDate")]
        public string PassportIssueDate { get; set; }

        [DataMember(Name = "PassportExpiryDate")]
        public string PassportExpiryDate { get; set; }

        [DataMember(Name = "PassportIssueCountry")]
        public string PassportIssueCountry { get; set; }

        [DataMember(Name = "PassportIssuePlace")]
        public string PassportIssuePlace { get; set; }

        [DataMember(Name = "PresentNationality")]
        public string PresentNationality { get; set; }

        [DataMember(Name = "PreviousNationality")]
        public string PreviousNationality { get; set; }

        [DataMember(Name = "MOHREVISAProfession")]
        public string MOHREVISAProfession { get; set; }

        [DataMember(Name = "VisaQualification")]
        public string VisaQualification { get; set; }

        [DataMember(Name = "Language1")]
        public string Language1 { get; set; }

        [DataMember(Name = "Language2")]
        public string Language2 { get; set; }

        [DataMember(Name = "Language3")]
        public string Language3 { get; set; }

        [DataMember(Name = "EduCertissuedFrom")]
        public string EduCertissuedFrom { get; set; }

        [DataMember(Name = "MOFAAttestationNo")]
        public string MOFAAttestationNo { get; set; }

        [DataMember(Name = "MOFAAttestationLabel")]
        public string MOFAAttestationLabel { get; set; }

        [DataMember(Name = "CertAttestationNo")]
        public string CertAttestationNo { get; set; }

        [DataMember(Name = "UnifiedIdentityNumber")]
        public string UnifiedIdentityNumber { get; set; }

        [DataMember(Name = "EmirateState")]
        public string EmirateState { get; set; }

        [DataMember(Name = "Area")]
        public string Area { get; set; }

        [DataMember(Name = "City")]
        public string City { get; set; }

        [DataMember(Name = "Building")]
        public string Building { get; set; }

        [DataMember(Name = "Street")]
        public string Street { get; set; }

        [DataMember(Name = "FlatNo")]
        public string FlatNo { get; set; }

        [DataMember(Name = "POBox")]
        public string POBox { get; set; }

        [DataMember(Name = "OfficeTelNo")]
        public string OfficeTelNo { get; set; }

        [DataMember(Name = "LandlineNo")]
        public string LandlineNo { get; set; }

        [DataMember(Name = "MobileNo")]
        public string MobileNo { get; set; }

        [DataMember(Name = "TeleNoAbroad")]
        public string TeleNoAbroad { get; set; }

        [DataMember(Name = "PersonalEmail")]
        public string PersonalEmail { get; set; }

        [DataMember(Name = "Address")]
        public string Address { get; set; }

        [DataMember(Name = "AddressAbroad")]
        public string AddressAbroad { get; set; }

        [DataMember(Name = "Email")]
        public string Email { get; set; }

        [DataMember(Name = "Sponsor")]
        public string Sponsor { get; set; }

        [DataMember(Name = "CandidateLocationCurrently")]
        public string CandidateLocationCurrently { get; set; }

        [DataMember(Name = "NoticePeriod")]
        public string NoticePeriod { get; set; }

        [DataMember(Name = "Probation")]
        public string Probation { get; set; }

        [DataMember(Name = "WeeklyHolidays")]
        public string WeeklyHolidays { get; set; }

        [DataMember(Name = "WorkType")]
        public string WorkType { get; set; }

        [DataMember(Name = "Remuneration")]
        public string Remuneration { get; set; }

        [DataMember(Name = "BasicSalary")]
        public string BasicSalary { get; set; }

        [DataMember(Name = "HousingAmount")]
        public string HousingAmount { get; set; }

        [DataMember(Name = "TransportingAmount")]
        public string TransportingAmount { get; set; }

        [DataMember(Name = "FoodAllowance")]
        public string FoodAllowance { get; set; }

        [DataMember(Name = "MobileConnectivityAllowance")]
        public string MobileConnectivityAllowance { get; set; }

        [DataMember(Name = "CostOfLivingAllowance")]
        public string CostOfLivingAllowance { get; set; }

        [DataMember(Name = "OtherAllowance")]
        public string OtherAllowance { get; set; }

        [DataMember(Name = "EmployeeStatus")]
        public string EmployeeStatus { get; set; }

        [DataMember(Name = "HRJobTitle")]
        public string HRJobTitle { get; set; }

        [DataMember(Name = "UniversityName")]
        public string UniversityName { get; set; }

        [DataMember(Name = "Faculty")]
        public string Faculty { get; set; }

        [DataMember(Name = "StudyMajors")]
        public string StudyMajors { get; set; }

        [DataMember(Name = "DegreeType")]
        public string DegreeType { get; set; }

        [DataMember(Name = "DegreeStartDate")]
        public string DegreeStartDate { get; set; }

        [DataMember(Name = "DegreeEndDate")]
        public string DegreeEndDate { get; set; }

        [DataMember(Name = "GraduationYear")]
        public string GraduationYear { get; set; }

        [DataMember(Name = "ActualYearsofDegree")]
        public string ActualYearsofDegree { get; set; }

        [DataMember(Name = "JoiningDate")]
        public string JoiningDate { get; set; }

        [DataMember(Name = "Doc_WBPhoto")]
        public string Doc_WBPhoto { get; set; }
        [DataMember(Name = "Doc_Pass1")]
        public string Doc_Pass1 { get; set; }
        [DataMember(Name = "Doc_Pass2")]
        public string Doc_Pass2 { get; set; }
        [DataMember(Name = "Doc_ECP1")]
        public string Doc_ECP1 { get; set; }
        [DataMember(Name = "Doc_ECP2")]
        public string Doc_ECP2 { get; set; }
        [DataMember(Name = "Doc_CTR")]
        public string Doc_CTR { get; set; }


    }

    public class SponsorchangeModel
    {
        [DataMember(Name = "EmployeeID")]
        public string EmployeeID { get; set; }

        [DataMember(Name = "PrevSponsorCompany")]
        public string PrevSponsorCompany { get; set; }

        [DataMember(Name = "CurrSponsorCompany")]
        public string CurrSponsorCompany { get; set; }

  

        [DataMember(Name = "TransferDate")]
        public string TransferDate { get; set; }

        [DataMember(Name = "EmployeeHasFamilySponsored")]
        public string EmployeeHasFamilySponsored { get; set; }

        [DataMember(Name = "Doc_PassCpy1")]
        public string Doc_PassCpy1 { get; set; }
        [DataMember(Name = "Doc_PassCpy2")]
        public string Doc_PassCpy2 { get; set; }

        [DataMember(Name = "Doc_WBPhoto")]
        public string Doc_WBPhoto { get; set; }
        [DataMember(Name = "Doc_Res")]
        public string Doc_Res { get; set; }

    }
}
