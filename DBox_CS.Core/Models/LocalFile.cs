using DBox_CS.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBox_CS.Core.Models
{
    public class LocalFile
    {
        public string LocalPath { get; set; } 
        public ImportProcessStatus Status { get; set; } 
    }
}
