using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DBox_CS.Core.HCMS.Entity
{
    [DataContract]
    [Serializable]
    public class FinChanges
    {
        [DataMember]
        public Int32 ReqNo { get; set; }
        [DataMember]
        public Int32 SrNo { get; set; }
        [DataMember]
        public String Element { get; set; }
        [DataMember]
        public String FromVal { get; set; }
        [DataMember]
        public String ToVal { get; set; }
        [DataMember]
        public String CodeFrom { get; set; }
        [DataMember]
        public String CodeTo { get; set; }
        [DataMember]
        public String RemarksE { get; set; }
        [DataMember]
        public String Attachment { get; set; }
        [DataMember]
        public Int16 Status { get; set; }
        [DataMember]
        public Int16 ActiveStatus { get; set; }
        [DataMember]
        public Byte SalUpgrade { get; set; }

    }
}
