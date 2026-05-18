using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DBox_CS.Core.HCMS.Entity
{
    [DataContract]
    [Serializable]
    public class ExtDDFWeb
    {
        [DataMember]
        public String TableName { get; set; }
        [DataMember]
        public String FieldPrefix { get; set; }
        [DataMember]
        public String FieldTitleE { get; set; }
        [DataMember]
        public String FieldTitleA { get; set; }

    }
}