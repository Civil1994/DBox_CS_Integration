using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;


namespace DBox_CS.Core.HCMS.Entity
{
    [DataContract]
    [Serializable]
    public class SecRights
    {
        [DataMember]
        public String UserID { get; set; }
        [DataMember]
        public String ModuleCode { get; set; }
        [DataMember]
        public Boolean Read { get; set; }
        [DataMember]
        public Boolean ReadWrite { get; set; }
        [DataMember]
        public Boolean Delete { get; set; }
        [DataMember]
        public Boolean RepAcc { get; set; }
        [DataMember]
        public Boolean RepInp { get; set; }
        [DataMember]
        public Boolean RepLF { get; set; }
        [DataMember]
        public Boolean PrnFrm { get; set; }
        [DataMember]
        public Boolean LtrPad { get; set; }
        [DataMember]
        public Boolean AdmRD { get; set; }
        [DataMember]
        public Boolean AdmRW { get; set; }
        [DataMember]
        public Boolean PerRD { get; set; }
        [DataMember]
        public Boolean PerRW { get; set; }
        [DataMember]
        public Boolean ErnRD { get; set; }
        [DataMember]
        public Boolean ErnRW { get; set; }
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
        public String SalProfile { get; set; }
        [DataMember]
        public String ApprAuth { get; set; }
        [DataMember]
        public Boolean Confirmation { get; set; }

    }
}