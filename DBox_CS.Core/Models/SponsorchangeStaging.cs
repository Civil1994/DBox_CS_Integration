using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBox_CS.Core.Models
{
    class SponsorchangeStaging
    {
        public int DBOXIProcessId { get; set; }
        public DateTime InsertedDate { get; set; }
        public int RowNo { get; set; }
        public string EmployeeID { get; set; }
        public string PrevSponsorCompany { get; set; }
        public string CurrSponsorCompany { get; set; }
        public string TransferDate { get; set; }
        public string EmployeeHasFamilySponsored { get; set; }
        public string Doc_PassCpy1 { get; set; }
        public string Doc_PassCpy2 { get; set; }
        public string Doc_WBPhoto { get; set; }
        public string Doc_Res { get; set; }

    }
}
