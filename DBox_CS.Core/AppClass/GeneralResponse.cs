using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBox_CS.Core.AppClass
{
    public class GeneralResponse
    {
        #region ' D A T A '

        public enum ResponseStatus
        {
            Success = 0,
            Error = 1,
            Warning = 2,
            SuccessWithErrors = 4,
        }

        public Error[] Errors;
        public object Result;
        public ResponseStatus Status;
        public string[] Message;

        #endregion

        public class Error
        {
            public int ID;
            public string Code;
            public string Message;
            public string Resolution;

        }
    }
}
