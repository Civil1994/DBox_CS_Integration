using SF_CS.Core.AppClass;
using SF_CS.Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UF_CS.Core.BL
{
    public class DailyHoursStagingBL
    {
        #region MappingDataRow


        public static EOSStaging MapDataRowToModel(DataRow row, string fileName, string status)
        {
            try
            {
                return new EOSStaging
                {
                    EmpCode = row["EmpCode"].ToString(),
                    LastDayInService = row["LastDayInService"].ToString(),
                    EOSReason = row["EOSReason"].ToString(),
                    ResignationDate = row["ResignationDate"].ToString(),
                    
                    InsertedDate = DateTime.Now,
                    FileName = fileName,
                    SFIStatus = status

                };
            }
            catch (Exception ex)
            {
                // Log error details
                string errorMessage = $"Error occurred while mapping DataRow to EOSStaging. Details: {ex.Message}";
                Common.LogAction(errorMessage);  // Log the error action
                Common.LogException(ex);  // Log the exception details

                return null;  // Return null to indicate failure in mapping
            }
        }

        #endregion

        #region MappingParameters

        public static Dictionary<string, object> MapToParameters(EOSStaging eosStaging)
        {
            return new Dictionary<string, object>
            {
                {"@RowNo", eosStaging.RowNo},
                {"@EmpCode", eosStaging.EmpCode},
                {"@LastDayInService", eosStaging.LastDayInService},
                {"@EOSReason", eosStaging.EOSReason},
                {"@ResignationDate", eosStaging.ResignationDate},

                {"@InsertedDate", eosStaging.InsertedDate},
                {"@FileName", eosStaging.FileName},
                {"@SFIStatus", eosStaging.SFIStatus},
            };
        }


        //private Dictionary<string, object> MapWorkAgreementToParameters(EmployeeStaging employee)
        //{
        //    // TODO:  Need to implement data Mapping for work agreement files
        //    Common.LogAction($"WorkAgreement data from '{fileName}' has been inserted into the database.");
        //}
        #endregion
    }
}
