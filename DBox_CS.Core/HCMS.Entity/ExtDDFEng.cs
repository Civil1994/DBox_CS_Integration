using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DBox_CS.Core.HCMS.Entity
{
    [DataContract]
    [Serializable]
    public class ExtDDFEng
    {
        [DataMember]
        public String TableName { get; set; }
        [DataMember]
        public String FieldPrefix { get; set; }
        [DataMember]
        public String FieldTitle { get; set; }
        [DataMember]
        public Byte FieldType { get; set; }
        [DataMember]
        public Byte DataType { get; set; }
        [DataMember]
        public String LookTableName { get; set; }
        [DataMember]
        public String DisplayFieldPrefix { get; set; }
        [DataMember]
        public String LinkingFieldPrefix { get; set; }
        [DataMember]
        public Int16 DisplayWidth { get; set; }
        [DataMember]
        public String SecondaryTable { get; set; }
        [DataMember]
        public String SecondaryLink { get; set; }
        [DataMember]
        public String PrimaryTableLink { get; set; }
        [DataMember]
        public Byte ForWorkFlow { get; set; }
        [DataMember]
        public Boolean ForBR { get; set; }
        [DataMember]
        //10-05-2022:Robin added field for GroupID where condition
        public String LookupWhereCond { get; set; }

    }
}
