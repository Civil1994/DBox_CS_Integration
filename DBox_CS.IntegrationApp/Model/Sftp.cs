using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF_CS.IntegrationApp.Model
{
    public class SftpFile
    {
        public string SftpUrl { get; set; }
        public string LocalPath { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Status { get; set; }
        public int Port { get; set; }
    }
}
