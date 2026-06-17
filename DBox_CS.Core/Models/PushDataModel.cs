using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DBox_CS.Core.Models
{
    class PushDataModel
    {
    }

    public class EmployeePushModel
    {
        [DataMember(Name = "api_key")]
        public string apiKey { get; set; }

        [DataMember(Name = "import_id")]
        public string importId { get; set; }

        [DataMember(Name = "group_company")]
        public string groupCompany { get; set; }

        [DataMember(Name = "import_data")]
        public List<EmployeePushDTO> importData { get; set; }
    }


    public class EmployeePushDTO
    {
        public string EmpCode { get; set; }
        public string EmpNameA { get; set; }
        public string FNameE { get; set; }
        public string SNameE { get; set; }
        public string NickNameE { get; set; }
        public string GrandFatherE { get; set; }
        public string FamilyNameE { get; set; }
        public string FNameA { get; set; }
        public string SNameA { get; set; }
        public string NickNameA { get; set; }
        public string GrandFatherA { get; set; }
        public string FamilyNameA { get; set; }
        public string MotherNameA { get; set; }
        public string BirthPlaceA { get; set; }
        public string PIssuePlaceA { get; set; }
        public string UIDNo { get; set; }
        public string BuildingA { get; set; }
        public string AddressA { get; set; }
        public string PerAddressA { get; set; }
        public string ResidenceNo { get; set; }
        public string ResIssuePlace { get; set; }
        public DateTime? ResIssueDate { get; set; }
        public DateTime? ResExpDate { get; set; }
        public string NationalID { get; set; }
        public string LabCardNo { get; set; }
        public string LCIssuePlace { get; set; }
        public DateTime? LCExpDate { get; set; }
        public string VisaNo { get; set; }
        public DateTime? VisaIssueDate { get; set; }
        public DateTime? VisaExpDate { get; set; }
        public DateTime? AuxDate3 { get; set; }
        public DateTime? AuxDate4 { get; set; }
        public DateTime? AuxDate5 { get; set; }
        public string AuxString7 { get; set; }
        public string AuxLib5 { get; set; }
        public string AuxLib6 { get; set; }
    }


    public class DocumentPushDTO
    {
        [DataMember(Name = "employee_no")]
        public string employee_no { get; set; }

        [DataMember(Name = "section")]
        public string section { get; set; }

        [DataMember(Name = "section_attribute")]
        public string section_attribute { get; set; }

        [DataMember(Name = "attachment")]
        public string attachment { get; set; }

        [DataMember(Name = "attachment_file_name")]
        public string attachment_file_name { get; set; }

        [DataMember(Name = "attachment_file_extension")]
        public string attachment_file_extension { get; set; }
    }
}
