using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HCMS.Entity
{
    [DataContract]
    [Serializable]
    public class AuditTrail
    {
        [DataMember]
        public Int32 AuditNo { get; set; }
        [DataMember]
        public String Table { get; set; }
        [DataMember]
        public String Transaction { get; set; }
        [DataMember]
        public Int32 TransactionNo { get; set; }
        [DataMember]
        public String EmpCode { get; set; }
        [DataMember]
        public String UserID { get; set; }
        [DataMember]
        public DateTime Date { get; set; }
        [DataMember]
        public String Errors { get; set; }
        [DataMember]
        public Int16 Flag { get; set; }
        [DataMember]
        public String WComp { get; set; }
        [DataMember]
        public String MachineName { get; set; }

    }

    [DataContract]
    [Serializable]
    public class AuditTrailView
    {
        public String MachineName { get; set; }
        [DataMember]
        public String WCompDesc { get; set; }
        [DataMember]
        public String EmpName { get; set; }
        [DataMember]
        public String EmpCode { get; set; }
        [DataMember]
        public String ModuleDesc { get; set; }
        [DataMember]
        public String UserID { get; set; }
        [DataMember]
        public DateTime Date { get; set; }
        [DataMember]
        public String Errors { get; set; }
        [DataMember]
        public String Transaction { get; set; }

    }

    public class AuditOpt
    {
       public String Errors { get; set; }
       public String Transaction { get; set; }
    }
}
