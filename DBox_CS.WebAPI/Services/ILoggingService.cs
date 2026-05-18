using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DBox_CS.WebAPI.Services
{
    public interface ILoggingService
    {
        void LogInformation(string message);
        void LogError(Exception ex, string message);
    }
}
