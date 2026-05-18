using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBox_CS.Core.Models
{

    public class UFApiResponse<T>
    {
        public Links _links { get; set; }
        public Embedded<T> _embedded { get; set; }
    }

    public class Links
    {
        public List<Curie> curies { get; set; }
    }

    public class Curie
    {
        public string href { get; set; }
        public string name { get; set; }
        public bool templated { get; set; }
    }

    public class Embedded<T>
    {
        public List<T> collection { get; set; }
    }

    public class UFLocationDTO
    {
        public string name { get; set; }
        public string code { get; set; }
        public string addressLine1 { get; set; }
        public string addressLine2 { get; set; }
        public string addressLine3 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string zipcode { get; set; }
        public string telephone { get; set; }
        public string scheduleMode { get; set; }
        public string region { get; set; }
        public string brand { get; set; }
    }
    public class UFEmployeeDTO
    {
        public string employeeId { get; set; }           // Required
        public string altId { get; set; }
        public string badgeNo { get; set; }
        public string firstName { get; set; }            // Required
        public string middleName { get; set; }
        public string lastName { get; set; }             // Required
        public string employeeType { get; set; }         // Required: Enum [P, R, S, U, V]
        public string hireDate { get; set; }           // Required
        public string seniorityDate { get; set; }
        public string statusTypeCode { get; set; }       // Required: Enum [A, T, L, R, F]
        public string hoursAvailable { get; set; }
        public string statusChangeReasonCode { get; set; }
        public string workClassCode { get; set; }        // Required
        public string primaryClassCode { get; set; }
        public string secondaryClassCode { get; set; }
        public string birthDate { get; set; }
        public string terminationDate { get; set; }
        public string leaveOfAbsenceDate { get; set; }
        public string leaveOfAbsenceReturnDate { get; set; }
        public string reHireDate { get; set; }
        public string gender { get; set; }
        public string tipped { get; set; }               // Enum [Y, N, I]

        public string address { get; set; }
        public string address2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string homePhone { get; set; }
        public string mobilePhone { get; set; }
        public string email { get; set; }
        public string emergencyContact { get; set; }
        public string emergencyPhone { get; set; }

        public string propertyCode { get; set; }         // Required: Home property code

        public List<UFReconcileJobDetailsDTO> jobs { get; set; }
        //public List<UFEmployeeCustomDataDTO> customDataList { get; set; }
    }
    public class UFReconcileJobDetailsDTO
    {
        public string jobCode { get; set; }              // Required
        public string jobDate { get; set; }            // Required
        //public string jobRank { get; set; }
        public string rateType { get; set; }             // Required: Enum [H, P, S, N, C]
        //public string hourlyRate { get; set; }
        public string annualRate { get; set; }
        //public string pieceRate { get; set; }
        //public string contractHours { get; set; }
        //public string contractDays { get; set; }
        public string rateDate { get; set; }           // Required
        public string jobOrder { get; set; }                // Required (1 for Home, 2 for Secondary)
        //public string deactivationDate { get; set; }
    }
    public class UFEmployeeCustomDataDTO
    {
        public string key { get; set; }
        public string value { get; set; }
    }
}
