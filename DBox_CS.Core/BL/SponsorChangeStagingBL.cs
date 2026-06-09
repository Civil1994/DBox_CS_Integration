using DBox_CS.Core.AppClass;
using DBox_CS.Core.DALayer;
using DBox_CS.Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBox_CS.Core.BL
{
    class SponsorChangeStagingBL
    {

        #region MappingDataRow

        public static SponsorchangeStaging MapModelToStagingModel(SponsorchangeModel spchangeModel, int processid)
        {
            try
            {
                return new SponsorchangeStaging
                {
                    //ProbPeriod = Common.ParseNullableInt(row["ProbPeriod"]),
                    InsertedDate = DateTime.Now,
                    DBOXIProcessId = processid,
                    EmployeeID = spchangeModel.EmployeeID,
                    PrevSponsorCompany = spchangeModel.PrevSponsorCompany,
                    CurrSponsorCompany = spchangeModel.CurrSponsorCompany,
                    TransferDate = spchangeModel.TransferDate,
                    EmployeeHasFamilySponsored = spchangeModel.EmployeeHasFamilySponsored,
                    Doc_PassCpy1 = spchangeModel.Doc_PassCpy1,
                    Doc_PassCpy2 = spchangeModel.Doc_PassCpy2,
                    Doc_WBPhoto = spchangeModel.Doc_WBPhoto,
                    Doc_Res = spchangeModel.Doc_Res,
                };
            }
            catch (Exception ex)
            {
                // Log error details
                string errorMessage = $"Error occurred while mapping to SponsorchangeStaging. Details: {ex.Message}";
                Common.LogAction(errorMessage);  // Log the error action
                Common.LogException(ex);  // Log the exception details

                return null;  // Return null to indicate failure in mapping
            }
        }


        #endregion

        #region MappingParameters

        public static Dictionary<string, object> MapToParameters(SponsorchangeStaging sponsorchange)
        {
            return new Dictionary<string, object>
            {
                {"@InsertedDate", sponsorchange.InsertedDate},
                {"@DBOXIProcessId", sponsorchange.DBOXIProcessId},
                {"@EmployeeID", sponsorchange.EmployeeID},
                {"@PrevSponsorCompany", sponsorchange.PrevSponsorCompany},
                {"@CurrSponsorCompany", sponsorchange.CurrSponsorCompany},
                {"@TransferDate", sponsorchange.TransferDate},
                 {"@EmployeeHasFamilySponsored", sponsorchange.EmployeeHasFamilySponsored},
                {"@Doc_PassCpy1", sponsorchange.Doc_PassCpy1},
                {"@Doc_PassCpy2", sponsorchange.Doc_PassCpy2},
                 {"@Doc_WBPhoto", sponsorchange.Doc_WBPhoto},
                {"@Doc_Res", sponsorchange.Doc_Res}

            };
        }



        #endregion


        public void SaveModelToSponsorStaging(SponsorchangeModel spchangeModel, int processid)
        {
            Common.LogAction($"Dbox Integration SaveModelToSponsorStaging started.");
            SponsorchangeStaging sponsorstgmodel = SponsorChangeStagingBL.MapModelToStagingModel(spchangeModel, processid);

            string sQry = @"
                Declare @rowno int=(Select count(1) from DBOXI_SponsorchangeInitialStaging where  DBOXIProcessId=@DBOXIProcessId)+1;

                INSERT INTO [dbo].[DBOXI_SponsorchangeInitialStaging]
               ([InsertedDate],[DBOXIProcessId],[RowNo],[EmployeeID],[PrevSponsorCompany],
              [CurrSponsorCompany],[TransferDate],[EmployeeHasFamilySponsored],[Doc_PassCpy1],[Doc_PassCpy2],[Doc_WBPhoto],[Doc_Res])VALUES
              (@InsertedDate,@DBOXIProcessId,@rowno,@EmployeeID,@PrevSponsorCompany,@CurrSponsorCompany,@TransferDate,
               @EmployeeHasFamilySponsored, @Doc_PassCpy1,@Doc_PassCpy2,@Doc_WBPhoto,@Doc_Res
             );";


            Dictionary<string, object> parameters = SponsorChangeStagingBL.MapToParameters(sponsorstgmodel);


            string errMsg = string.Empty;
            if (!ConnectionFunctions.ExecuteQuery(sQry, parameters, ref errMsg))
            {
                string errorMsg = $"Error inserting row FOR PROCESSID " + processid.ToString() + " : {errMsg}";
                //Common.LogErrorToSFIErrorLog(fileName, rowIndex, employee.EmpCode, errorMsg, "Employee");
                Common.LogAction(errorMsg);

                throw new ManualException("", errorMsg);
            }
            Common.LogAction($"Dbox Integration SaveModelToSponsorStaging completed.");
        }


        public void MoveDataToEmpStagingClosed(int processid)
        {

            string moveDataQuery = @" DELETE FROM DBOXI_SponsorchangeStagingClosed  WHERE DBOXIProcessId = @DBOXIProcessId;

               INSERT INTO [dbo].[DBOXI_SponsorchangeStagingClosed]([Id],[DBOXIProcessId],[InsertedDate],[RowNo],
             [EmployeeID],[PrevSponsorCompany],[CurrSponsorCompany],[TransferDate],
             [EmployeeHasFamilySponsored],[Doc_PassCpy1],[Doc_PassCpy2],[Doc_WBPhoto],[Doc_Res])
            SELECT [Id],[DBOXIProcessId],[InsertedDate],
            [RowNo],[EmployeeID],[PrevSponsorCompany],[CurrSponsorCompany],
            [TransferDate],[EmployeeHasFamilySponsored],[Doc_PassCpy1],[Doc_PassCpy2],[Doc_WBPhoto],[Doc_Res] FROM DBOXI_SponsorchangeInitialStaging
          WHERE DBOXIProcessId = @DBOXIProcessId;

        DELETE FROM DBOXI_SponsorchangeInitialStaging  
        WHERE DBOXIProcessId = @DBOXIProcessId;";


            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                {"@DBOXIProcessId", processid}
            };

            string errMsg = string.Empty;
            if (!ConnectionFunctions.ExecuteQuery(moveDataQuery, parameters, ref errMsg))
            {
                string errorMsg = $"Error deleting staging data FOR PROCESSID " + processid.ToString() + " : {errMsg}";
                //Common.LogErrorToSFIErrorLog(fileName, rowIndex, employee.EmpCode, errorMsg, "Employee");
                Common.LogAction(errorMsg);

                throw new ManualException("", errorMsg);
            }
        }
    }
}
