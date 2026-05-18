using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DBox_CS.Core.Configuration
{
    public interface IAppSettings
    {
        string ConnectionString { get; }
    }
}
