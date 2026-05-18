using DBox_CS.Core.BL;
using DBox_CS.Core.DALayer;
using DBox_CS.Core.Enums;
using DBox_CS.Core.Models;
using DBox_CS.Core.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DBox_CS.Core.APIClient;
using Newtonsoft.Json;
using DBox_CS.Core.Configuration;

namespace DBox_CS.Core.AppClass
{
    public class Main
    {
        //private List<LocalFile> LocalFiles = new List<LocalFile>();
        public Dictionary<string, string> tempFolders;// List of folders to process files from
        public Dictionary<string, string> tempOutputFolders;// List of folders for output
        public string appFilesPath;
        public string tempDirectory;



        ProcessIntitator _caller;

        //EmployeeExportBL empexportbl;
        //LeaveExportBL leaveexportbl;
        //DailyHoursImportBL dhimportbl;



        public enum Msgtype
        {
            Error = 1,
            Success = 2,
            Info = 3,
            Warning = 4,
        }
        public enum ProcessIntitator
        {
            BackroudWindowService = 1,
            WindowsForm = 2,
            WebAPI = 3,
        }

        public Main()
        {
        }
        public Main(ProcessIntitator pi, IAppSettings appSettings)
        {
            _caller = pi;
            //empexportbl = new EmployeeExportBL();
            //SFTPService sFTPService = new SFTPService();
            //leaveexportbl = new LeaveExportBL(sFTPService);
            //dhimportbl = new DailyHoursImportBL(sFTPService);

            ConnectionFunctions.Initialize(appSettings.ConnectionString);

        }



        public void SaveEmployee(EmployeeModel empmodel, out int dboxiProcessId)
        {

            dboxiProcessId = 0;
            bool hasProcessError = false;
            string strprocessRemarks = "";
            EmployeeImportBL empimportbl = new EmployeeImportBL();
            EmployeeStagingBL empstgbl = new EmployeeStagingBL();

            try
            {
                dboxiProcessId = Common.CreateDBoxIProcessLogEntry("Employee Import from DBox");

                string ErrMsg = string.Empty;
                Common.LogAction($"Dbox Integration Started.");

                if (dboxiProcessId == 0)
                {
                    throw new ManualException("", "Failed to generate process ID");
                }

                empstgbl.SaveModelToStaging(empmodel, dboxiProcessId);

                empimportbl.SaveToCSFromStaging(dboxiProcessId, ref strprocessRemarks, ref hasProcessError);

                if (string.IsNullOrEmpty(strprocessRemarks))
                {
                    if (hasProcessError)
                        strprocessRemarks = "Employee Saving failed. Check error log for details";
                    else
                        strprocessRemarks = "Employee Saving Process completed successfully";
                }

            }
            catch (ManualException ex)
            {
                strprocessRemarks = "An error occured. Check error log for details";
                hasProcessError = true;
                Common.LogErrorToDBOXIErrorLog(dboxiProcessId, 0, "", "SaveEmployee trycatch block", ex.Message + "," + ex.InnerErrorDetails);
                throw ex;
            }
            catch (Exception ex)
            {
                strprocessRemarks = "An error occured. Check error log for details";
                hasProcessError = true;
                Common.LogErrorToDBOXIErrorLog(dboxiProcessId, 0, "", "SaveEmployee trycatch block", ex.Message);
                throw ex;
            }
            finally
            {
                if (dboxiProcessId != 0)
                {
                    empstgbl.MoveDataToEmpStagingClosed(dboxiProcessId);

                    Common.LogUFIProcessCompletion(dboxiProcessId, strprocessRemarks, hasProcessError);
                }


                Common.LogAction($"Dbox Integration Completed.");
            }
        }

        public void SaveEmployeeExit(EmpExitInboundModel empExitModel)
        {
            try
            {
                ExitMethods exitModel = new ExitMethods();
                exitModel.PostEmployeeExit(empExitModel);

                Common.LogAction($"Dbox Exit Integration Completed.");
            }

            catch (Exception ex)
            {
                Common.LogAction(ex.Message);
                throw ex;
            }
        }


        private void ShowMessage(string message, Msgtype msgtype)
        {
            if (_caller == ProcessIntitator.BackroudWindowService)
            {
                switch (msgtype)
                {
                    case Msgtype.Error:
                        Console.WriteLine("Error : " + message);
                        break;

                    case Msgtype.Success:

                        Console.WriteLine("Success : " + message);
                        break;

                    case Msgtype.Info:

                        Console.WriteLine("Info : " + message);
                        break;

                    case Msgtype.Warning:

                        Console.WriteLine("Warning : " + message);
                        break;

                }
            }
            else
            {
                switch (msgtype)
                {
                    case Msgtype.Error:

                        MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;

                    case Msgtype.Success:

                        MessageBox.Show(message, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;

                    case Msgtype.Info:

                        MessageBox.Show(message, "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;

                    case Msgtype.Warning:

                        MessageBox.Show(message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        break;

                }
            }

        }


        public DataTable GetProcessError(int dboxiProcessId)
        {
            return Common.GetProcessError(dboxiProcessId);
        }
        public DataTable GetProcessLog(int dboxiProcessId)
        {
            return Common.GetProcessLog(dboxiProcessId);
        }
        public DataTable GetProcessLogDetails(int dboxiProcessId)
        {
            return Common.GetProcessLogDetails(dboxiProcessId);
        }

        public void SaveSponsorchange(SponsorchangeModel spchangemodel, out int dboxiProcessId)
        {

            dboxiProcessId = 0;
            bool hasProcessError = false;
            string strprocessRemarks = "";
            EmployeeImportBL empimportbl = new EmployeeImportBL();
            SponsorChangeStagingBL spchangestgbl = new SponsorChangeStagingBL();

            try
            {
                dboxiProcessId = Common.CreateDBoxIProcessLogEntry("Sponsor Change Import from DBox");

                string ErrMsg = string.Empty;
                Common.LogAction($"Dbox Integration Started.");

                if (dboxiProcessId == 0)
                {
                    throw new ManualException("", "Failed to generate process ID");
                }

                spchangestgbl.SaveModelToSponsorStaging(spchangemodel, dboxiProcessId);

                //empimportbl.SaveToCSFromStaging(dboxiProcessId, ref strprocessRemarks, ref hasProcessError);

                if (string.IsNullOrEmpty(strprocessRemarks))
                {
                    if (hasProcessError)
                        strprocessRemarks = "Sponsor Change Saving failed. Check error log for details";
                    else
                        strprocessRemarks = "Sponsor Change Saving Process completed successfully";
                }
                if (dboxiProcessId != 0)
                {
                    var exitModel = new EmpExitInboundModel
                    {
                        EmployeeID = spchangemodel.EmployeeID,
                        LastWorkingDate = spchangemodel.TransferDate,
                        EmployeeCurrentLocation = "InsideUAE", // if available
                        ReasonofCancellation = "Resignation",
                        EmployeeHasFamilySponsored = spchangemodel.EmployeeHasFamilySponsored,
                        doc_WBPhoto = spchangemodel.Doc_WBPhoto, // if available
                        doc_Res = spchangemodel.Doc_Res // if available
                    };
                    SaveEmployeeExit(exitModel);
                }


            }
            catch (ManualException ex)
            {
                strprocessRemarks = "An error occured. Check error log for details";
                hasProcessError = true;
                Common.LogErrorToDBOXIErrorLog(dboxiProcessId, 0, "", "SaveSponsorchange trycatch block", ex.Message + "," + ex.InnerErrorDetails);
                throw ex;
            }
            catch (Exception ex)
            {
                strprocessRemarks = "An error occured. Check error log for details";
                hasProcessError = true;
                Common.LogErrorToDBOXIErrorLog(dboxiProcessId, 0, "", "SaveSponsorchange trycatch block", ex.Message);
                throw ex;
            }
            finally
            {
                if (dboxiProcessId != 0)
                {
                    spchangestgbl.MoveDataToEmpStagingClosed(dboxiProcessId);

                    Common.LogUFIProcessCompletion(dboxiProcessId, strprocessRemarks, hasProcessError);
                }


                Common.LogAction($"Dbox Integration Completed.");
            }
        }

        public bool IsExist(string TableName, string Value, string filter)
        {
            bool Exist = Common.IsExist(TableName, Value, filter);
            return Exist;
        }


    }
}
