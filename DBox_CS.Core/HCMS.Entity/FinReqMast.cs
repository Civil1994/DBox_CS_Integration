using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DBox_CS.Core.HCMS.Entity
{
    [DataContract]
    [Serializable]
    public class FinReqMast
    {
        [DataMember]
        public Int32 SrNo { get; set; }
        [DataMember]
        public DateTime ReqDate { get; set; }
        [DataMember]
        public DateTime EffectiveDate { get; set; }
        [DataMember]
        public Int32 EmpID { get; set; }
        [DataMember]
        public String EmpCode { get; set; }
        [DataMember]
        public String EmpNameE { get; set; }
        [DataMember]
        public String EmpNameA { get; set; }
        [DataMember]
        public Int64 FinFlag { get; set; }
        [DataMember]
        public Int16 ModChangeFlag { get; set; }
        [DataMember]
        public String ReqID { get; set; }
        [DataMember]
        public Byte Status { get; set; }
        [DataMember]
        public Int16 ActiveStatus { get; set; }

    }
}
