
using HCMS.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using DBox_CS.Core.AppClass;
using DBox_CS.Core.DALayer;
using DBox_CS.Core.Utility;

namespace DBox_CS.Core.BL
{
    public class DailyHoursImportBL
    {

        private readonly SFTPService _sFTPService;

        public DailyHoursImportBL(SFTPService sFTPService)
        {
            _sFTPService = sFTPService;
        }


        public string downloadFolder;
        public string downloadArchiveFolder;
        public string tempfolder;
        public string archiveFolder;


        StringBuilder sbLineErrMsg;
        StringBuilder sbFileErrMsg;
        int errCount = 0;
        int errTotalCount = 0;
        private bool RetVal = false;
        string errmsg = "";
        String sQry = String.Empty;
        private int result = 0;
        string importfileName = "";
        StringBuilder sbSaveMsg;

        bool bHasEOSTranBRerror = false;
        bool bEOSTranSaved = false;

        int iCurrUFIProcessId = 0;
        string strCurrEmpCode = "";
        int iCurrRowNo = 0;
        bool bCurrHasLineErrors = true;

        Dictionary<string, string> dictTitleNames;
        DataTable dtLookUpFieldsDetails_Emp;
        DataTable dtLookUpFieldsDetails_EOSTran;

        DataTable mydt;
        DataTable dtXLData;

        const string xlcol_EmpCode = "EmpCode";
        const string xlcol_LastDayInService = "LastDayInService";
        const string xlcol_EOSReason = "EOSReason";
        const string xlcol_ResignationDate = "ResignationDate";
        const string xlcol_EOSType = "EndOfServiceType";

        const string xlcol_TerminationDate = "TerminationDate";

        static string[] EmployeeCols =
        {
            xlcol_EmpCode,
        };
        static string[] EOSCols =
       {
            xlcol_LastDayInService, xlcol_EOSReason, xlcol_ResignationDate,xlcol_EOSType
        };
        static string[] EOSColsForXlLookUp =
        {
            xlcol_EOSReason,xlcol_EOSType
        };

        DateTime dtEmptyDate = new DateTime(1900, 1, 1);
        DateTime dtminDate = new DateTime(1900, 1, 1);
        DateTime dtmaxDate = new DateTime(2079, 6, 6);
        DateTime currDate;

        enum enmXlImportTables
        {
            Employee = 0,
            EOSTran = 1,

        }
        List<string> lstXlImportTables = new List<string>() { "Employee", "EOSTran" };


        static string[] SystemMandatoryEOSTranCols =
        {
            xlcol_EmpCode, xlcol_LastDayInService, xlcol_EOSReason, xlcol_ResignationDate,

        };
        static string[] MandatoryEOSTranCols =
        {
            xlcol_EmpCode, xlcol_LastDayInService, xlcol_EOSReason, xlcol_ResignationDate,

        };






        private void ClearDirectory(string folderpath)
        {
            if (Directory.Exists(folderpath))
            {
                Directory.GetFiles(folderpath).ToList().ForEach(File.Delete);
            }
        }
        
        private string[] GetDailyHoursFilesFromFolder(string folderpath)
        {
            string prefix = "DailyHours_";
            if (Directory.Exists(folderpath))
            {
                var files = Directory.GetFiles(folderpath, "*.csv")
               .Where(file => Path.GetFileName(file).StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
               .ToArray();

                return files;
            }
            else
            {
                return null;
            }
        }
        private void MoveFiles(string[] filePaths, string destinationFolderPath)
        {
            foreach(string fp in filePaths)
            {
                MoveFile(fp, destinationFolderPath);
            }
            
        }
        private void MoveFile(string sourcefilePath, string destinationFolderPath)
        {
            if (File.Exists(sourcefilePath))
            {
                if (Directory.Exists(destinationFolderPath))
                {

                    File.Move(sourcefilePath, Path.Combine(destinationFolderPath, Path.GetFileName(sourcefilePath)));
                    File.Delete(sourcefilePath);
                }
            }
        }


        public int DownloadDailyHoursFromUF(ref int ufiProcessId)
        {
            bool hasProcessError = false;
            string strprocessRemarks = "";
            int totalreccount = 0;
            RetVal = false;
            errmsg = "";

            string message = "";


            ufiProcessId = Common.CreateUFIProcessLogEntry("Daily Hours download from UF", "");


            if (ufiProcessId == 0)
            {
                Common.LogAction("Error generating UFI Process ID for 'Daily Hours download from UF' process");
                return 0;
            }

            try
            {
                string[] files = GetDailyHoursFilesFromFolder(downloadFolder);
                MoveFiles(files, downloadArchiveFolder);

                if (!Directory.Exists(tempfolder))
                {
                    Directory.CreateDirectory(tempfolder); // Create temporary directory if it doesn't exist
                    Common.LogAction("Daily Hours Temporary directory created.");
                }
                else
                {
                    ClearDirectory(tempfolder);
                }


                RetVal = _sFTPService.DownloadFilesFromSFTP("UFIDailyHours", downloadFolder, ref errmsg);

                if (!RetVal)
                {
                    hasProcessError = true;
                    Common.LogAction("Daily Hours File SFTP Download Error. Details: " + errmsg);
                    Common.LogErrorToUFIImportProcessLogDetails(ufiProcessId, "", "DailyHours", "Daily Hours File SFTP Download Error. Details: " + errmsg);
                }



                string[] downloadedfiles = GetDailyHoursFilesFromFolder(downloadFolder);
                if(downloadedfiles != null && downloadedfiles.Length > 0)
                {
                    totalreccount = downloadedfiles.Length;
                }

                message = totalreccount > 0
                    ? $"Total { totalreccount} File(s) Downloaded:\n{string.Join("\n", totalreccount)}"
                    : "No files were downloaded.";

                //ShowMessage(message, Msgtype.Info);


                Common.LogAction(message);
                Common.UpdateRemarksToUFIImportProcessLogDetails(ufiProcessId, "", "DailyHours", message);

                return totalreccount;
            }
            catch (Exception ex)
            {
                hasProcessError = true;
                Common.LogAction("Error occured at DownloadDailyHoursFromUF. Details:" + ex.Message);
                Common.LogException(ex);
                Common.LogErrorToUFIImportProcessLogDetails(ufiProcessId, "", "DailyHours", "Error occured at DownloadDailyHoursFromUF. Details:" + ex.Message);
                return 0;
            }
            finally
            {

                if (hasProcessError)
                {
                    strprocessRemarks = "An error occured. Check Process Log Details";
                }
                else
                {
                    strprocessRemarks = message;
                }
                Common.LogUFIProcessCompletion(ufiProcessId, strprocessRemarks, hasProcessError);
            }

        }


        public void SaveToCSFromFile(ref int ufiProcessId)
        {

            bool hasProcessError = false;
            string strprocessRemarks = "";
            int totalreccount = 0;
            RetVal = false;
            errmsg = "";

            string message = "";


            ufiProcessId = Common.CreateUFIProcessLogEntry("Daily Hours Save to CS", "");

            if (ufiProcessId == 0)
            {
                Common.LogAction("Error generating UFI Process ID for 'Daily Hours Save to CS' process");
                return;
            }

            iCurrUFIProcessId = ufiProcessId;

            try
            {

                
                string[] downloadedfiles = GetDailyHoursFilesFromFolder(downloadFolder);
                if (downloadedfiles != null && downloadedfiles.Length > 0)
                {
                    totalreccount = downloadedfiles.Length;
                }

                string csvFilePath = downloadedfiles[0];
                importfileName = Path.GetFileName(csvFilePath);
                DataTable csvData = ReadCsvToDataTable(csvFilePath);


                if (csvData.Rows.Count > 0)
                {
                    int rowIndex = 0;  //To track the row number

                    DataTable stagingtbl = CreateDailyHoursStagingTableSchema();
                    foreach (DataRow row in csvData.Rows)
                    {
                        rowIndex++;
                        try
                        {
                            DataRow stagingrow = stagingtbl.NewRow();
                            MapDataRowToStagingRow(row, stagingrow);
                            stagingrow["RowNo"] = rowIndex;
                            stagingrow["FileName"] = importfileName;
                            stagingtbl.Rows.Add(stagingrow);

                        }
                        catch (Exception ex)
                        {
                            hasProcessError = true;
                            string errorMsg = $"Exception in row {rowIndex}: {ex.Message}";
                            Common.LogErrorToUFIImportProcessLogDetails(ufiProcessId, importfileName, "DailyHours", "Error occured at Importing file data to Staging table. Details:" + errorMsg, rowIndex);
                            Common.LogException(ex);
                            break;
                        }

                    }

                    if (hasProcessError)
                        return;

                    MoveDataToStaging(stagingtbl);

                    //If all records are processed, proceed to move data to the CS Table
                    hasProcessError=SaveToCSFromStaging();
  

                    //If all records are processed, proceed to move data to the StagingClosed
                    MoveDataToStagingClosed();



                }
                else
                {
                    string infoMessage = "The CSV file is empty.";
                    Common.LogAction(infoMessage);
                    Common.LogErrorToUFIImportProcessLogDetails(ufiProcessId, importfileName, "DailyHours", infoMessage);

                }

                
            }
            catch (Exception ex)
            {
                hasProcessError = true;
                Common.LogAction("Error occured at DownloadDailyHoursFromUF. Details:" + ex.Message);
                Common.LogException(ex);
                Common.LogErrorToUFIImportProcessLogDetails(ufiProcessId, "", "DailyHours", "Error occured at DownloadDailyHoursFromUF. Details:" + ex.Message);
                
            }
            finally
            {

                if (hasProcessError)
                {
                    strprocessRemarks = "An error occured. Check Process Log Details";
                }
                else
                {
                    strprocessRemarks = message;
                }
                Common.LogUFIProcessCompletion(ufiProcessId, strprocessRemarks, hasProcessError);
            }

        }

       
        private bool SaveToCSFromStaging()
        {
            return false;
        }

        private void MapDataRowToStagingRow(DataRow row, DataRow stagingrow)
        {
            
            stagingrow["EmpCode"] = row["EmpId"] == DBNull.Value ? "" : row["EmpId"].ToString();
            stagingrow["Shift_Date"] = row["ShiftDate"] == DBNull.Value ? "" : row["ShiftDate"].ToString(); 
            stagingrow["Shift_Hours"] = row["SHiftHours"] == DBNull.Value ? "" : row["SHiftHours"].ToString(); 
            stagingrow["Worked_Hours"] = row["Worked hours"] == DBNull.Value ? "" : row["Worked hours"].ToString();
            stagingrow["Overtime_Hours"] = row["OT Hours"] == DBNull.Value ? "" : row["OT Hours"].ToString();
            stagingrow["Overtime_Type"] = row["OT Type"] == DBNull.Value ? "" : row["OT Type"].ToString();
            stagingrow["Attendance"] = row["Attendance"] == DBNull.Value ? "" : row["Attendance"].ToString();
            stagingrow["Location_Code"] = row["Location"] == DBNull.Value ? "" : row["Location"].ToString();

        }

        private DataTable ReadCsvToDataTable(string filePath)
        {
            DataTable dataTable = new DataTable();
            try
            {
                using (var reader = new StreamReader(filePath))
                {
                    // Read the header line to get column names
                    string[] headers = reader.ReadLine().Split(',');

                    foreach (string header in headers)
                    {
                        dataTable.Columns.Add(header.Trim());
                    }

                    // Read the data lines and add to the DataTable
                    while (!reader.EndOfStream)
                    {
                        string[] rows = reader.ReadLine().Split(',');
                        dataTable.Rows.Add(rows);
                    }

                    string fileName = Path.GetFileName(filePath);
                    Common.UpdateRemarksToUFIImportProcessLogDetails(iCurrUFIProcessId,fileName,"DailyHours", "Csv File Reading Success.");
                }
            }
            catch (Exception ex)
            {
                string fileName = Path.GetFileName(filePath);
                Common.LogErrorToUFIImportProcessLogDetails(iCurrUFIProcessId, fileName, "DailyHours", "Csv File Reading error. ErrMsg:" + ex.Message);


                Common.LogAction("Error reading CSV file at " + filePath + ". Details: " + ex.Message);
                Common.LogException(ex);

                throw new Exception("An error occurred while reading the CSV file.", ex);
            }
            return dataTable;
        }

        public DataTable CreateDailyHoursStagingTableSchema()
        {
            SqlConnection SQLConn = new SqlConnection(ConnectionFunctions.GetConnectionString());
            DataSet ds = new DataSet();
            try
            {
                SqlDataAdapter MyDataAdapter = new SqlDataAdapter();
                MyDataAdapter.SelectCommand = new SqlCommand("SELECT Top 0 * FROM UFIDailyHoursInitialStaging WITH (NOLOCK)", SQLConn);
                SqlCommandBuilder cb = new SqlCommandBuilder(MyDataAdapter);
                SQLConn.Open();
                MyDataAdapter.Fill(ds, "UFIDailyHoursInitialStaging");
            }
            catch (Exception Ex)
            {
                if ((SQLConn != null))
                {
                    if (SQLConn.State != ConnectionState.Closed)
                        SQLConn.Close();
                }
            }
            finally
            {
                if ((SQLConn != null))
                {
                    if (SQLConn.State != ConnectionState.Closed)
                        SQLConn.Close();
                }
            }
            return ds.Tables["UFIDailyHoursInitialStaging"];
        }

        private void MoveDataToStaging(DataTable dt)
        {
            try
            {
                dt.Columns.Add("UFIProcessId", Type.GetType("System.Int32"));
                dt.Columns.Add("InsertedDate", Type.GetType("System.DateTime"));

                DateTime inserttime = DateTime.Now;

                foreach (DataRow row in dt.Rows)
                {
                    row["UFIProcessId"] = iCurrUFIProcessId;
                    row["InsertedDate"] = inserttime;
                }

                dt.Columns["UFIProcessId"].SetOrdinal(0);
                dt.Columns["InsertedDate"].SetOrdinal(1);

                SqlConnection conn = new SqlConnection(ConnectionFunctions.GetConnectionString());

                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
                {
                    bulkCopy.DestinationTableName = "dbo.UFIDailyHoursInitialStaging";

                    // Write from the source to the destination.
                    conn.Open();
                    bulkCopy.WriteToServer(dt);
                    conn.Close();

                }

            }
            catch (Exception ex)
            {
                Common.LogAction("Employee Upload data logging in LogExportData failed. Details:" + ex.Message);
                Common.LogException(ex);
            }


        }

        private void MoveDataToStagingClosed()
        {
            errmsg = "";
            sQry = "Insert into UFIDailyHoursStagingClosed Select * from UFIDailyHoursInitialStaging";
            RetVal = ConnectionFunctions.Connect_SQLNonQuery(ref result, sQry, ref errmsg);

            if(!RetVal)
            {
                Common.LogAction("MoveDataToStagingClosed failed. Details:" + errmsg);
            }

        }




        //public void SaveToCSFromStaging()
        //{

        //    try
        //    {
        //        Common.LogAction($"Save From EOS Staging Table to CSTable Started");
        //        sbLineErrMsg = new StringBuilder();
        //        sbFileErrMsg = new StringBuilder();
        //        sbSaveMsg = new StringBuilder();

        //        SqlConnection con = new SqlConnection(ConnectionFunctions.GetConnectionString());
        //        SqlConnection myConn = new SqlConnection();
        //        myConn.ConnectionString = ConnectionFunctions.GetConnectionString();
        //        SqlTransaction SqlTran = null;

        //        //For Progress bar
        //        StartProgress();
        //        //End:For Progress bar

        //        string sQry = "";
        //        string finishMessage = ""; //Progress finished message

        //        mydt = new DataTable();
        //        dtXLData = new DataTable();

        //        mydt.Columns.Add(new DataColumn("RowNo", System.Type.GetType("System.String")));
        //        mydt.Columns.Add(new DataColumn(xlcol_EmpCode, System.Type.GetType("System.String")));
        //        mydt.Columns.Add(new DataColumn(xlcol_LastDayInService, System.Type.GetType("System.String")));
        //        mydt.Columns.Add(new DataColumn(xlcol_EOSReason, System.Type.GetType("System.String")));
        //        mydt.Columns.Add(new DataColumn(xlcol_ResignationDate, System.Type.GetType("System.String")));



        //        // Step 1: Retrieve data from SFIEOSInitialStaging
        //        string selectQuery = $"SELECT * FROM SFIEOSInitialStaging";
        //        dtXLData = ConnectionFunctions.ExecuteQueryToDataTable(selectQuery);

        //        if (dtXLData == null || dtXLData.Rows.Count == 0)
        //        {
        //            Common.LogAction($"No data found in SFIEOSInitialStaging.");
        //            return;
        //        }

        //        //For Progress bar
        //        string fileName = dtXLData.Rows[0]["FileName"].ToString();
        //        Common.LogAction($"EOS file '{fileName}' processed successfully.");
        //        //End:For Progress bar

        //        importfileName = fileName;

        //        dictTitleNames = GetTitleNames();
        //        dtLookUpFieldsDetails_Emp = GetLookupFieldDetails(enmXlImportTables.Employee);
        //        dtLookUpFieldsDetails_EOSTran = GetLookupFieldDetails(enmXlImportTables.EOSTran);




        //        int xlrowno = 0;
        //        Boolean isRowHasData = false;
        //        int TotalRecords = 0, InsertedRecords = 0, SkippedRecords = 0;
        //        DataRow mydtrow;
        //        foreach (DataRow row in dtXLData.Rows)
        //        {
        //            xlrowno++;


        //            mydtrow = mydt.NewRow();
        //            for (int k = 0; k < mydt.Columns.Count; k++)
        //            {
        //                mydtrow[k] = GetColumnValue(row, GetXLColName(mydt.Columns[k].ColumnName));
        //                if (mydtrow[k] != null)
        //                    isRowHasData = true;
        //            }

        //            mydt.Rows.Add(mydtrow);

        //        }


        //        if (mydt.Rows.Count == 0)
        //        {

        //            //For Progress bar
        //            SetProgressError("No Record To Import");
        //            //End:For Progress bar

        //            return;
        //        }



        //        errTotalCount = 0;


        //        DataTable dtDefaultValues = new DataTable();
        //        GetDefaultSaveValues("", ref dtDefaultValues, ref errmsg);


        //        object emptyObj = null;
        //        object lookupCodeObj = null;
        //        string sQuery = "";
        //        string fieldErr = "";
        //        Boolean isUpdate = false;

        //        int nRowNo = 0;
        //        foreach (DataRow row in mydt.Rows)
        //        {
        //            try
        //            {
        //                nRowNo = Convert.ToInt32(row["RowNo"]);
        //                iCurrRowNo = nRowNo;

        //                AddProcessSummaryRow();

        //                #region Variable declaration

        //                errCount = 0;
        //                bHasEOSTranBRerror = false;
        //                bEOSTranSaved = false;
        //                strCurrEmpCode = "";
        //                bCurrHasLineErrors = false;
        //                sbLineErrMsg.Clear();
        //                sbSaveMsg.Clear();
        //                isUpdate = false;

        //                string StrEmpCode = "";
        //                string LocationLevelCode = "";
        //                string SalaryProfileCode = "";
        //                string JobTitleCode = "";
        //                string StrEOSType = "";
        //                string EOSTypeCode = "";
        //                string StrEOSReason = "";
        //                string EOSReasonCode = "";
        //                string StrLastDayInServiceDt = "";
        //                DateTime LastDayInServiceDtValue = new DateTime(1900, 1, 1);
        //                string StrResignationDt = "";
        //                DateTime ResignationDtValue = new DateTime(1900, 1, 1);
        //                string StrEOSOffTreat = "";
        //                string EOSOffTreatCode = "";
        //                string StrTerminationDt = "";
        //                DateTime TerminationDtValue = new DateTime(1900, 1, 1);



        //                bool bIsEOSEntered = false;


        //                StringBuilder strBuildrAudit = new StringBuilder();
        //                string strBuildrAuditText = "";

        //                string strEditMode = "ADD";
        //                bool bskipInsertUpdate = false;
        //                string rowErrInfo = "";
        //                #endregion

        //                #region Employee Fiedls Validations

        //                if (CheckIfColumnExists(mydt, xlcol_EmpCode))
        //                {
        //                    if (row[xlcol_EmpCode] == null || row[xlcol_EmpCode].ToString() == string.Empty)
        //                    {
        //                        AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_EmpCode] + " is empty : ");
        //                        bskipInsertUpdate = true;
        //                        goto skipInsertUpdateStep;
        //                    }
        //                    else
        //                    {
        //                        StrEmpCode = row[xlcol_EmpCode].ToString();
        //                        rowErrInfo = ", EmpCode: " + StrEmpCode;
        //                        strCurrEmpCode = StrEmpCode;
        //                        bIsEOSEntered = true;
        //                    }
        //                }
        //                else
        //                {
        //                    AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_EmpCode] + " is empty : ");
        //                    bskipInsertUpdate = true;
        //                    goto skipInsertUpdateStep;
        //                }



        //                //For Progress bar

        //                //End:For Progress bar




        //                //check the record is in edit mode.
        //                string sQueryManD = "SELECT TOP(1) EmpID FROM Employee WITH (NOLOCK) WHERE EmpCode = @ParmEmpCode";
        //                string strEmpID = "";
        //                int nEmpID = 0;

        //                SqlParameter[] Params2 = new SqlParameter[1];
        //                Params2[0] = new SqlParameter("@ParmEmpCode", SqlDbType.VarChar);
        //                Params2[0].Value = StrEmpCode;
        //                if (!ConnectionFunctions.Connect_SQLScalar(ref strEmpID, sQueryManD, ref Params2, ref errmsg))
        //                {

        //                    AppendLineError(nRowNo, rowErrInfo, "Error Occurred while retrieving the Employee Details from Database : ");
        //                    bskipInsertUpdate = true;
        //                    goto skipInsertUpdateStep;

        //                }
        //                if (string.IsNullOrEmpty(strEmpID))
        //                {
        //                    AppendLineError(nRowNo, rowErrInfo, "Employee not found in the Database : ");
        //                    bskipInsertUpdate = true;
        //                    goto skipInsertUpdateStep;
        //                }
        //                else
        //                {
        //                    nEmpID = Convert.ToInt32(strEmpID);

        //                }

        //                DataTable dtEmployee = new DataTable();
        //                DataTable dtFinMast = new DataTable();
        //                DataRow drowEmp = null;
        //                DataRow drowFinMast = null;


        //                if (!GetEmployeeData(StrEmpCode, ref dtEmployee, ref errmsg))
        //                {
        //                    AppendLineError(nRowNo, rowErrInfo, errmsg);
        //                    bskipInsertUpdate = true;
        //                    goto skipInsertUpdateStep;
        //                }
        //                else
        //                {
        //                    if (dtEmployee != null && dtEmployee.Rows.Count >= 1)
        //                        drowEmp = dtEmployee.Rows[0];
        //                }


        //                if (!GetEmployeeFinMastData(StrEmpCode, ref dtFinMast, ref errmsg))
        //                {
        //                    AppendLineError(nRowNo, rowErrInfo, errmsg);
        //                    bskipInsertUpdate = true;
        //                    goto skipInsertUpdateStep;
        //                }
        //                else
        //                {
        //                    if (dtFinMast != null && dtFinMast.Rows.Count >= 1)
        //                        drowFinMast = dtFinMast.Rows[0];
        //                }

        //                if (drowFinMast == null)
        //                {
        //                    LocationLevelCode = drowEmp["LocLib5"].ToString();
        //                    SalaryProfileCode = drowEmp["SalProfile"].ToString();
        //                }
        //                else
        //                {
        //                    LocationLevelCode = drowFinMast["LocLib5"].ToString();
        //                    SalaryProfileCode = drowFinMast["SalProfile"].ToString();
        //                    JobTitleCode = drowFinMast["JobTitle"].ToString();
        //                }


        //                //Employee Status Validation
        //                //if (drowEmp != null && drowEmp["EmployeeStatus"] != DBNull.Value)
        //                //{
        //                //    if (drowEmp["EmployeeStatus"].ToString() == "11")
        //                //    {
        //                //        AppendLineError(nRowNo, rowErrInfo, "End of Service Employee Work Agreement Details cannot be modified : ");
        //                //        UpdateImportLineStatusAndContinue(nRowNo, SharedImportBL.ImportDataStatus.HasErrors);
        //                //        continue;
        //                //    }
        //                //}



        //                //EOS check if already posted
        //                int nCountResult = 0;
        //                if(!IsEOSPosted(nEmpID, LocationLevelCode, 0, ref nCountResult, ref errmsg))
        //                {
        //                    AppendLineError(nRowNo, rowErrInfo, "Error Occurred while Checking the Employee End OF Service Details from Database : " + errmsg);
        //                    bskipInsertUpdate = true;
        //                    goto skipInsertUpdateStep;
        //                }
        //                else if (nCountResult > 0)
        //                {
        //                    AppendLineError(nRowNo, rowErrInfo, "EOS has been already posted at the same Working Company. End Of Service Request Could Not Be Saved");
        //                    bskipInsertUpdate = true;
        //                    goto skipInsertUpdateStep;
        //                }
        //                //=============================================




        //                #endregion

        //                #region EOS Validations


        //                if (row[xlcol_LastDayInService] == null || row[xlcol_LastDayInService].ToString() == string.Empty)
        //                {
        //                    if (CheckIfMandatory(xlcol_LastDayInService, enmXlImportTables.EOSTran))
        //                    {
        //                        AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_LastDayInService] + " is empty: ");
        //                        bskipInsertUpdate = true;
        //                    }
        //                }
        //                else
        //                {


        //                    bIsEOSEntered = true;
        //                    StrLastDayInServiceDt = row[xlcol_LastDayInService].ToString();

        //                    if (!ValidateField(xlcol_LastDayInService, StrLastDayInServiceDt, isUpdate, ref fieldErr, ref emptyObj))
        //                    {
        //                        AppendLineError(nRowNo, rowErrInfo, fieldErr);
        //                        bskipInsertUpdate = true;
        //                    }
        //                    else
        //                    {
        //                        LastDayInServiceDtValue = GetValidDateTime(StrLastDayInServiceDt);
        //                        strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_LastDayInService] + " " + StrLastDayInServiceDt + "]");
        //                    }


        //                }


        //                if (row[xlcol_EOSReason] == null || row[xlcol_EOSReason].ToString() == string.Empty)
        //                {
        //                    if (CheckIfMandatory(xlcol_EOSReason, enmXlImportTables.EOSTran))
        //                    {
        //                        AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_EOSReason] + " is empty: ");
        //                        bskipInsertUpdate = true;
        //                    }
        //                }
        //                else
        //                {
        //                    bIsEOSEntered = true;
        //                    StrEOSReason = row[xlcol_EOSReason].ToString();

        //                    if (!ValidateField(xlcol_EOSReason, StrEOSReason, isUpdate, ref fieldErr, ref lookupCodeObj))
        //                    {
        //                        AppendLineError(nRowNo, rowErrInfo, fieldErr);
        //                        bskipInsertUpdate = true;
        //                    }
        //                    else
        //                    {
        //                        EOSReasonCode = Convert.ToString(lookupCodeObj);

        //                        strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_EOSReason] + " " + StrEOSReason + "]");
        //                    }
        //                }


        //                if (row[xlcol_ResignationDate] == null || row[xlcol_ResignationDate].ToString() == string.Empty)
        //                {
        //                    if (CheckIfMandatory(xlcol_ResignationDate, enmXlImportTables.EOSTran))
        //                    {
        //                        AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_ResignationDate] + " is empty: ");
        //                        bskipInsertUpdate = true;
        //                    }

        //                }
        //                else
        //                {
        //                    bIsEOSEntered = true;
        //                    StrResignationDt = row[xlcol_ResignationDate].ToString();

        //                    if (!ValidateField(xlcol_ResignationDate, StrResignationDt, isUpdate, ref fieldErr, ref emptyObj))
        //                    {
        //                        AppendLineError(nRowNo, rowErrInfo, fieldErr);
        //                        bskipInsertUpdate = true;
        //                    }
        //                    else
        //                    {
        //                        ResignationDtValue = GetValidDateTime(StrResignationDt);
        //                        strBuildrAudit.Append(",[ " + dictTitleNames[xlcol_ResignationDate] + " " + StrResignationDt + "]");
        //                    }

        //                }

        //                if(!string.IsNullOrEmpty(EOSReasonCode))
        //                {
        //                    EOSTypeCode = GetEOSTypeByEosReasonCode(EOSReasonCode);
        //                    if (EOSTypeCode == "0")
        //                    {
        //                        StrTerminationDt = "";
        //                        TerminationDtValue = dtEmptyDate;
        //                    }
        //                    if (EOSTypeCode == "1")
        //                    {
        //                        StrTerminationDt = StrResignationDt;
        //                        TerminationDtValue = ResignationDtValue;
        //                        StrResignationDt = "";
        //                        ResignationDtValue = dtEmptyDate;
        //                    }
        //                }



        //                if (bIsEOSEntered == true)
        //                {
        //                    #region Default  Values filling
        //                    //StrEOSType = "Resignation"; //resignation;
        //                    //EOSTypeCode = "0"; //resignationcode;

        //                    if (dtDefaultValues!=null && dtDefaultValues.Rows.Count>0)
        //                    {
        //                        //if (string.IsNullOrEmpty(StrEOSType))
        //                        //{
        //                        //    EOSTypeCode = dtDefaultValues.Rows[0]["EOSType"].ToString();
        //                        //}

        //                        if (string.IsNullOrEmpty(StrEOSOffTreat))
        //                        {
        //                            EOSOffTreatCode = dtDefaultValues.Rows[0]["EOSOffTreat"].ToString();
        //                        }
        //                    }

        //                    #endregion

        //                    if (string.IsNullOrEmpty(EOSReasonCode))
        //                    {
        //                        AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_EOSReason] + " is Empty : ");
        //                        bskipInsertUpdate = true;
        //                    }
        //                    if (string.IsNullOrEmpty(EOSTypeCode))
        //                    {
        //                        AppendLineError(nRowNo, rowErrInfo, "Could not determine EOS Type from Eos Reason : ");
        //                        bskipInsertUpdate = true;
        //                    }


        //                    //Check Location Transer posted already
        //                    int trnsfrCount = 0;
        //                    if (EOSTypeCode == "5" || EOSTypeCode == "8")
        //                    {
        //                        if (!IsTransfered(nEmpID, 0, ref trnsfrCount, ref errmsg))
        //                        {
        //                            AppendLineError(nRowNo, rowErrInfo, "Error Occurred while Checking the Employee Transfer Status Details from Database : " + errmsg);
        //                            bskipInsertUpdate = true;
        //                        }
        //                        else if (trnsfrCount > 0)
        //                        {
        //                            AppendLineError(nRowNo, rowErrInfo, "There is an active Transfer Request already posted, cannot post more than 1 company transfer in a month.");
        //                            bskipInsertUpdate = true;

        //                        }

        //                    }


        //                    if (LastDayInServiceDtValue == dtEmptyDate)
        //                    {
        //                        AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_LastDayInService] + " is Empty : ");
        //                        bskipInsertUpdate = true;

        //                    }
        //                    if (EOSTypeCode == "0")
        //                    {
        //                        if (ResignationDtValue == dtEmptyDate)
        //                        {
        //                            AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_ResignationDate] + " is Empty : ");
        //                            bskipInsertUpdate = true;
        //                        }
        //                    }
        //                    if (EOSTypeCode == "1")
        //                    {
        //                        if (TerminationDtValue == dtEmptyDate)
        //                        {
        //                            AppendLineError(nRowNo, rowErrInfo, dictTitleNames[xlcol_TerminationDate] + " is Empty : ");
        //                            bskipInsertUpdate = true;
        //                        }
        //                    }


        //                }

        //                #endregion

        //                if (bskipInsertUpdate == true)
        //                {
        //                    goto skipInsertUpdateStep;
        //                }


        //                bool skipEOSSave = false;


        //                //---------------------------Begin:Check Work Agreement Business Rules Validation-----------------------
        //                EOSTran oEOSTran = new EOSTran();
        //                DataTable dtEOSTranTable = new DataTable();
        //                short ViewNo_EOS = 295;
        //                string eosTranReqNo = "0";

        //                if (bIsEOSEntered == true)
        //                {
        //                    currDate = DateTime.Now;

        //                    eosTranReqNo = Common.GetRequestNo(ViewNo_EOS).ToString();
        //                    oEOSTran.ReqNo = Convert.ToInt32(eosTranReqNo);

        //                    oEOSTran.EmpID = Convert.ToInt32(nEmpID);
        //                    oEOSTran.LocLib5 = LocationLevelCode;
        //                    oEOSTran.SalProfile = SalaryProfileCode;
        //                    oEOSTran.JobTitle = JobTitleCode;

        //                    oEOSTran.ReqDate = currDate;
        //                    oEOSTran.LastDayInService = LastDayInServiceDtValue;
        //                    oEOSTran.EndOfServiceType = Convert.ToByte(EOSTypeCode);
        //                    oEOSTran.EosTypeChar = EOSTypeCode;
        //                    oEOSTran.ResignationDate = ResignationDtValue;
        //                    oEOSTran.EOSReason = EOSReasonCode;
        //                    oEOSTran.TerminationDate = TerminationDtValue;
        //                    oEOSTran.EOSRemarks = "";
        //                    oEOSTran.Officialtreatment = "";
        //                    oEOSTran.EOSOffTreat = EOSOffTreatCode;
        //                    oEOSTran.LOCancDate = new DateTime(1900, 01, 01);
        //                    oEOSTran.LOBan = 1;//default
        //                    oEOSTran.LOMonths = 0;
        //                    oEOSTran.ICancDate = new DateTime(1900, 01, 01);
        //                    oEOSTran.IBan = 0;
        //                    oEOSTran.IMonths = 0;
        //                    oEOSTran.LeavingDate = new DateTime(1900, 01, 01);
        //                    oEOSTran.SCReducingDate = new DateTime(1900, 01, 01);
        //                    oEOSTran.SCRegNo = string.Empty;
        //                    oEOSTran.LastActLeavingDate = new DateTime(1900, 01, 01);
        //                    oEOSTran.EstRejoiningDate = new DateTime(1900, 01, 01);
        //                    oEOSTran.DiffTillReqDate = 0;
        //                    oEOSTran.LONotifyDate = new DateTime(1900, 01, 01);
        //                    oEOSTran.LONotifySrNo = string.Empty;
        //                    oEOSTran.BankAmt = 0.0M;
        //                    oEOSTran.BankDetails = string.Empty;
        //                    //Nishad Added 28082014
        //                    //oEOSTran.NoticeWrk = chkNotice.Checked;
        //                    oEOSTran.NoticeWrkDate = new DateTime(1900, 01, 01);

        //                    // General info.
        //                    oEOSTran.ReqID = Common.nSvcUserNo;
        //                    oEOSTran.LastModDateTime = currDate;
        //                    oEOSTran.Status = 0;
        //                    oEOSTran.ActiveStatus = 0;




        //                    dtEOSTranTable = CreateEOSTransactionTable(ref oEOSTran);

        //                    DataTable ErrTable_EOS = new DataTable();
        //                    ErrTable_EOS = Common.GetErrMast(ViewNo_EOS);
        //                    Dictionary<String, String> Errors_EOS = null;

        //                    RetVal = BusinessRules.CheckBusinessRule(ViewNo_EOS, dtEOSTranTable, "EOSTran", "ReqNo", ErrTable_EOS, out Errors_EOS);
        //                    if (RetVal == false)
        //                    {
        //                        AppendLineError(nRowNo, rowErrInfo, "An unexpected error occured while verifying End Of Service business rule");
        //                        bHasEOSTranBRerror = true;
        //                        skipEOSSave = true;
        //                    }
        //                    string Codes_EOS = string.Empty;
        //                    Int16 Ctr1_EOS = 0;
        //                    if (Errors_EOS.Count > 0)
        //                    {
        //                        string[] arCodes;
        //                        arCodes = Errors_EOS[eosTranReqNo.ToString()].ToString().Split('@');
        //                        for (Ctr1_EOS = 0; Ctr1_EOS <= arCodes.Length - 1; Ctr1_EOS++)
        //                        {
        //                            Codes_EOS = Codes_EOS + "'" + arCodes.GetValue(Ctr1_EOS) + "',";
        //                        }
        //                    }
        //                    if (Codes_EOS.Trim().Length != 0)
        //                    {
        //                        DataView dvErrors = new DataView(ErrTable_EOS);
        //                        dvErrors.RowFilter = "Code IN(" + Codes_EOS + ") AND Severity NOT IN(2)";

        //                        //'Sort the Errors_WrkAgrmnt by Severity 
        //                        dvErrors.Sort = "ShowInList";

        //                        if (dvErrors.Count > 0)
        //                        {
        //                            int nErrno = 0;
        //                            foreach (DataRowView rowDv in dvErrors)
        //                            {
        //                                if (rowDv[5].ToString() == "0" || rowDv[5].ToString() == "3")
        //                                {
        //                                    nErrno++;

        //                                    AppendLineError(nRowNo, rowErrInfo, "EOS Business rule Error#" + nErrno + " - " + rowDv["Message"].ToString());
        //                                    bHasEOSTranBRerror = true;
        //                                    skipEOSSave = true;

        //                                }
        //                            }

        //                        }
        //                    }

        //                }
        //                if (skipEOSSave)
        //                {
        //                    goto skipInsertUpdateStep;
        //                }
        //                //---------------------------End: Check Agreement Business Rules Validation-----------------------

        //                //---------------------------Begin: Check WorkAgreement Workflow-----------------------
        //                workflow WrkFlow_EOS = new workflow();
        //                ArrayList ApprF_EOS = new ArrayList();
        //                ArrayList AuthPersons_EOS = new ArrayList();
        //                ArrayList Users_EOS = new ArrayList();
        //                Hashtable ModuleData_EOS = new Hashtable();
        //                bool byPassed_EOS = false;
        //                string wfc_EOS = string.Empty;

        //                if (skipEOSSave == false && bIsEOSEntered)
        //                {
        //                    ModuleData_EOS.Add("ViewNo", ViewNo_EOS);
        //                    ModuleData_EOS.Add("TranTableName", "EOSTran");
        //                    ModuleData_EOS.Add("Levels", "5");
        //                    ModuleData_EOS.Add("ModuleCode", "CS0300"); //Time keeper

        //                    RetVal = WrkFlow_EOS.GenerateWorkFlow(ModuleData_EOS, dtEOSTranTable, ref ApprF_EOS, ConnectionFunctions.GetConnectionString(), Common.strSvcUserId, ref byPassed_EOS, ref errmsg, oEOSTran.EmpID, ref wfc_EOS);
        //                    if (RetVal == false)
        //                    {
        //                        AppendLineError(nRowNo, rowErrInfo, "Error in EOS Work Flow. " + errmsg);
        //                        skipEOSSave = true;
        //                        goto skipInsertUpdateStep;
        //                    }
        //                }

        //                //---------------------------End: Check WorkAgreement Workflow-----------------------




        //                if (skipEOSSave == false)
        //                {

        //                    try
        //                    {
        //                        ApprProcess oApprProcess = new ApprProcess();
        //                        oApprProcess.EmpID = oEOSTran.EmpID;
        //                        oApprProcess.DocAttach = "";
        //                        oApprProcess.App = ApprF_EOS[0].ToString();
        //                        oApprProcess.AppDate = ApprF_EOS[1].ToString();
        //                        oApprProcess.NoOfAppr = Convert.ToByte(ApprF_EOS[2]);
        //                        oApprProcess.ReqID = Common.nSvcUserNo;
        //                        oApprProcess.ViewNo = 295;

        //                        oApprProcess.Remarks = "Imported From SFI Import Service"; 

        //                        string ISL = string.Empty;
        //                        string ISLA = string.Empty;

        //                        Common.GetISLAndISLA("EosRequestHCMS", "EosReq00", 295, ref ISL, ref ISLA);
        //                        string sTypeDescE = "DescE"; string sTypeDescA = "DescA";
        //                        string sReasonDescE = "DescE"; string sReasonDescA = "DescA";

        //                        //Common.GetDescEAFromCode(ddlEOSType.SelectedValue, "EosType", ref sTypeDescE, ref sTypeDescA);
        //                        //Common.GetDescEAFromCode(ddlEOSReason.SelectedValue, "EOSReason", ref sReasonDescE, ref sReasonDescA);

        //                        sTypeDescE=GetLookupDescByCode(enmXlImportTables.EOSTran, xlcol_EOSType, oEOSTran.EndOfServiceType.ToString());
        //                        sTypeDescA = sTypeDescE;

        //                        sReasonDescE = GetLookupDescByCode(enmXlImportTables.EOSTran, xlcol_EOSReason, oEOSTran.EOSReason.ToString()); ;
        //                        sReasonDescA = sReasonDescE;

        //                        ISL = string.Format(ISL, sTypeDescE + " -> " + sReasonDescE, StrLastDayInServiceDt);
        //                        ISLA = string.Format(ISLA, sTypeDescA + " -> " + sReasonDescA, StrLastDayInServiceDt);

        //                        oApprProcess.Isl = ISL;
        //                        oApprProcess.Isla = ISLA;
        //                        oApprProcess.WFCode = wfc_EOS;
        //                        if (byPassed_EOS == true)
        //                        {
        //                            //Shyamjith Added This Code For Hokair Portal Integration on 07/07/2019
        //                            if (Common.GetCompanyProfile().ToUpper() == "HOKAIRHOTEL")
        //                            {
        //                                //if (IsHokairClearanceCheckEnabled(295))
        //                                //{
        //                                //    if (newcommon.HokairClearanceCheck(oEOSTran.EmpID) != 0)
        //                                //    {
        //                                //        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "", "CallNotification('" + Resources.WebResource.EmployeeClearenceIsPending + "','Warning');", true);
        //                                //        return;
        //                                //    }
        //                                //}
        //                            }
        //                            // End Shyamjith Added This Code For Hokair Portal Integration on 07/07/2019
        //                            oApprProcess.NextApprAuth = "";
        //                            oApprProcess.Bypassed = 1;
        //                            oApprProcess.Status = 20;
        //                            oEOSTran.Status = 20;
        //                            oEOSTran.ActiveStatus = 20;
        //                            //Since Record is added in the EOSPaytran table 
        //                            //through the trigger  ADD_TO_EOSPAY on EosTran table
        //                        }
        //                        else
        //                        {
        //                            oApprProcess.NextApprAuth = ApprF_EOS[3].ToString();
        //                            oApprProcess.Bypassed = 0;
        //                            oApprProcess.Status = 0;
        //                            oEOSTran.Status = 0;
        //                            oEOSTran.ActiveStatus = 0;
        //                        }
        //                        oApprProcess.RequestDate = currDate;
        //                        oApprProcess.LastModDateTime = currDate;
        //                        oApprProcess.Priority = 0;

        //                        InsertEOSTran(oEOSTran, oApprProcess);

        //                        bEOSTranSaved = true;

        //                    }
        //                    catch (Exception ex)
        //                    {


        //                        errmsg = "An Error Occurred while Saving the Employee EOS Record to Database. TryCatch Exception. Details :" + ex.Message;
        //                        AppendLineError(nRowNo, rowErrInfo, errmsg);
        //                        Common.LogException(ex);

        //                    }

        //                }

        //            skipInsertUpdateStep:;

        //                if (bIsEOSEntered == true)
        //                {
        //                    if (bEOSTranSaved == false)
        //                    {
        //                        sbSaveMsg.Append("Employee EOS Posting failed.");
        //                    }
        //                    else
        //                    {
        //                        sbSaveMsg.Append("Employee EOS Posted Successfully.");
        //                        Update_LASTUPDDTTM(StrEmpCode, DateTime.Now, "eos");

        //                    }
        //                }
        //                else
        //                {
        //                    sbSaveMsg.Append("No EOS found to import.");
        //                }

        //            }
        //            catch (Exception ex)
        //            {

        //                string errorMessage = $" Unexpected Error occured while processing row with EmpCode {row["EmpCode"]}: {ex.Message}";

        //                AppendLineError(nRowNo, "", errorMessage);

        //                Common.LogAction(errorMessage);
        //                Common.LogException(ex);
        //            }
        //            finally
        //            {

        //                if (errCount > 0)
        //                {
        //                    bCurrHasLineErrors = true;

        //                    string strFullErrorText = sbLineErrMsg.ToString().TrimEnd('\r', '\n');
        //                    Common.LogErrorToSFIErrorLog(importfileName, iCurrRowNo, strCurrEmpCode, strFullErrorText, "EOS");
        //                }

        //                UpdateProcessSummaryRow();

        //            }
        //        }



        //        //For Progress bar
        //        FinishProgress(finishMessage);
        //        //End:For Progress bar
        //    }
        //    catch (Exception ex)
        //    {
        //        errmsg = ex.Message;

        //        //For Progress bar
        //        SetProgressError(errmsg);
        //        //End:For Progress bar

        //        //string errmsg = ex.Message;
        //        // ScriptManager.RegisterStartupScript(this, this.GetType(), "", "console.log('" + ex.Message.Replace('\'', ' ') + "');", true);

        //        Common.LogAction("An unexpected error occured in function EOS SaveToCSFromStaging. Details :" + ex.Message);
        //        Common.LogException(ex);
        //    }



        //}




        //#region ProgressBar Functions
        //private void StartProgress(string progressTitle = "")
        //{
        //    //objprog.name = "";
        //    //if (!String.IsNullOrEmpty(progressTitle))
        //    //    objprog.Title = progressTitle;
        //    //objprog.percn = 9999;
        //    //objprog.Progress = "Started";
        //    //Session["Progressbar"] = objprog;

        //    //Session["EmplImpErrFile"] = "";
        //    //Session["EmployeeImpErrorYN"] = "";
        //}
        //private bool IsProgressStopped()
        //{
        //    //if (objprog.Progress == "Stop")
        //    //    return true;
        //    //else
        //    //    return false;

        //    return false;
        //}
        //private void SetProgressStartVariables()
        //{
        //    //objprog.Starttime = DateTime.Now;
        //    //Session["Progressbar"] = objprog;
        //}
        //private void UpdateProgress(string progressEmpname, int nprccedd, int totcnt, double percn, string strnoofemp)
        //{
        //    ////TO DISPLAY THE NAME OF THE EMPLOYEE IN THE SPLASH SCREEN
        //    //objprog.name = progressEmpname;

        //    //if (percn < 1 && percn > 0)
        //    //{
        //    //    objprog.percn = 1;
        //    //}
        //    //else
        //    //{
        //    //    objprog.percn = Convert.ToInt32(percn);
        //    //}

        //    //objprog.noofemp = strnoofemp;

        //    ////time statistics calc denson added 20042015
        //    //DateTime CurrTime = DateTime.Now;
        //    //if (nprccedd > 1)
        //    //{
        //    //    TimeSpan TimeFor1 = (CurrTime - objprog.Starttime);
        //    //    double secforone = TimeFor1.TotalMilliseconds;
        //    //    secforone = (secforone / nprccedd);
        //    //    double multifactor = totcnt - nprccedd + 1;
        //    //    double sectoadd = secforone * multifactor;
        //    //    objprog.Exptime = CurrTime.AddMilliseconds(sectoadd);
        //    //    objprog.Remtime = objprog.Exptime - CurrTime;
        //    //}

        //    //Session["Progressbar"] = objprog;
        //}
        //private void SetProgressFinalErrors(string strErrFile, string ErrorYN)
        //{
        //    //Session["EmplImpErrFile"] = strErrFile;
        //    //Session["EmployeeImpErrorYN"] = ErrorYN;
        //}
        //private void FinishProgress(string finishMessage = "")
        //{
        //    //if (!String.IsNullOrEmpty(finishMessage))
        //    //    objprog.Xmsg = finishMessage;
        //    //objprog.percn = 101;
        //    //objprog.Progress = "finished";
        //    //Session["Progressbar"] = objprog;
        //}
        //private void SetProgressError(string Errmsg)
        //{
        //    //objprog.errmsg = Errmsg;
        //    //objprog.percn = -1;
        //    //Session["Progressbar"] = objprog;
        //}

        //#endregion

        //private Dictionary<string, string> GetTitleNames()
        //{
        //    //dictionry key is case sensitive

        //    string squery = "SELECT NameE,NameA,FieldPrefixE,FieldPrefixA FROM dbo.AuxFields  WITH (NOLOCK) WHERE [View] in (295)";
        //    string ErrMsg = "";
        //    DataTable dt = new DataTable();
        //    SqlConnection con = new SqlConnection(ConnectionFunctions.GetConnectionString());
        //    if (!ConnectionFunctions.Connect_SQLDataTable(ref dt, squery, ref con, ref ErrMsg))
        //    {
        //        throw new Exception(ErrMsg);
        //    }
        //    var comparer = StringComparer.OrdinalIgnoreCase;
        //    Dictionary<string, string> EngTitles = new Dictionary<string, string>(comparer);

        //    DataRow[] drows = null;
        //    string strTitle = "";
        //    foreach (DataColumn dcol in mydt.Columns)
        //    {
        //        strTitle = "";

        //        if (Common.buseColNameAsTitleName == false)
        //        {
        //            if (EmployeeCols.Contains(dcol.ColumnName, StringComparer.OrdinalIgnoreCase))
        //            {
        //                drows = dt.Select("FieldPrefixE='" + dcol.ColumnName + "' OR FieldPrefixA='" + dcol.ColumnName + "'");
        //                if (drows != null && drows.Length > 0)
        //                {
        //                    strTitle = drows[0]["NameE"].ToString();
        //                }

        //            }
        //            else if (EOSCols.Contains(dcol.ColumnName, StringComparer.OrdinalIgnoreCase))
        //            {
        //                switch (dcol.ColumnName)
        //                {
        //                    case xlcol_EOSReason:
        //                        strTitle = "End Of Service Reason";
        //                        break;
        //                    case xlcol_LastDayInService:
        //                        strTitle = "Last Day In Service";
        //                        break;
        //                    case xlcol_ResignationDate:
        //                        strTitle = "Resignation Date";
        //                        break;

        //                }
        //            }

        //        }

        //        if (string.IsNullOrEmpty(strTitle))
        //        {
        //            strTitle = dcol.ColumnName;
        //        }
        //        else
        //        {
        //            strTitle = strTitle + " (" + dcol.ColumnName + ")";
        //        }

        //        strTitle = GetValidString(strTitle);

        //        EngTitles.Add(dcol.ColumnName, strTitle);



        //    }


        //    return EngTitles;
        //}

        //public object GetColumnValue(DataRow row, string column, bool columnexist = true)
        //{
        //    if (row.Table.Columns.Contains(column))
        //    {
        //        columnexist = true;
        //        return GetValidString(row[column]);
        //    }
        //    else
        //    {
        //        columnexist = false;
        //        return null;
        //    }

        //}
        //public string GetValidString(object inputObj)
        //{
        //    string rsltstring = "";
        //    if (inputObj == null)
        //        return "";

        //    rsltstring = inputObj.ToString().Replace("\r\n", "");
        //    rsltstring = rsltstring.Replace("\n", "");
        //    rsltstring = rsltstring.Replace("\t", "");
        //    rsltstring = rsltstring.Trim();
        //    return rsltstring;

        //}

        //private DataTable GetLookupFieldDetails(enmXlImportTables tableindx)
        //{
        //    DataTable dt = new DataTable();
        //    string tableName = lstXlImportTables[Convert.ToInt32(tableindx)];


        //    string squery = "select FieldPrefix,FieldType,LookTableName,DisplayFieldPrefix,LinkingFieldPrefix from ExtDDFEng  WITH (NOLOCK) where TableName = '" + tableName + "' and FieldType in (2,3,4,5)";
        //    string ErrMsg = "";

        //    SqlConnection con = new SqlConnection(ConnectionFunctions.GetConnectionString());
        //    if (!ConnectionFunctions.Connect_SQLDataTable(ref dt, squery, ref con, ref ErrMsg))
        //    {
        //        throw new Exception(ErrMsg);
        //    }

        //    return dt;

        //}

        //private void AppendLineError(int iLineNo, string lineInfo, string errMsg)
        //{
        //    if (!string.IsNullOrEmpty(lineInfo))
        //    {
        //        lineInfo = ", " + lineInfo;
        //    }
        //    errCount++; errTotalCount++;
        //    //sbLineErrMsg.Append("LineNo: " + iLineNo.ToString() + lineInfo + ", Error : " + errMsg + Environment.NewLine);
        //    sbLineErrMsg.Append("EOS Error#" + errCount + " : " + errMsg + ";" + Environment.NewLine);
        //    //sbPersonAllLineErrMsg.Append("LineNo: " + iLineNo.ToString() + lineInfo + ", Error : " + errMsg + Environment.NewLine);
        //    sbFileErrMsg.Append("LineNo: " + iLineNo.ToString() + lineInfo + ", Error : " + errMsg + Environment.NewLine);
        //}
        //private void UpdateImportLineStatusAndContinue(int iLineNo, SharedImportBL.ImportDataStatus sts)
        //{
        //    errmsg = "";
        //    try
        //    {
        //        if (!UpdateImportLineStatus(iLineNo, sts, ref errmsg))
        //        {
        //            AppendLineError(iLineNo, "", errmsg);
        //        }
        //        Common.LogAction("Employee Import SaveToCS: " + sbLineErrMsg.ToString().TrimEnd('\r', '\n'));

        //    }
        //    catch (Exception ex)
        //    {
        //        Common.LogAction("Employee Import SaveToCS: Try Catch error, Details: " + ex.Message);
        //        Common.LogException(ex);
        //    }
        //    finally
        //    {
        //        sbLineErrMsg.Clear();
        //        errCount = 0;
        //    }
        //}

        //private bool UpdateImportLineStatus(int iLineNo, SharedImportBL.ImportDataStatus sts, ref string errmsg)
        //{
        //    errmsg = "";
        //    //Update LineStatus to processed
        //    sQry = "Update SFIEOSInitialStaging  set [SFIstatus] = " + ((int)sts).ToString() + " Where FileName='" + importfileName + "' and [RowNo]=" + iLineNo.ToString();
        //    RetVal = ConnectionFunctions.Connect_SQLNonQuery(ref result, sQry, ref errmsg);


        //    return RetVal;
        //}



        //private bool GetEmployeeData(string strEmpCode, ref DataTable dt, ref string ErrMsg)
        //{


        //    string sQuery = "Select * FROM Employee  WITH (NOLOCK) WHERE EmpCode = @EmpCode";

        //    SqlParameter[] Params = new SqlParameter[1];
        //    Params[0] = new SqlParameter("@EmpCode", SqlDbType.VarChar);
        //    Params[0].Value = strEmpCode;


        //    SqlConnection con = new SqlConnection(ConnectionFunctions.GetConnectionString());
        //    if (!ConnectionFunctions.Connect_SQLDataTable(ref dt, sQuery, ref con, Params, ref ErrMsg))
        //    {
        //        ErrMsg = "Error Occurred while retrieving the Employee Details from Database";
        //        return false;
        //    }
        //    if (dt.Rows.Count == 0)
        //    {
        //        ErrMsg = "Employee EmpCode not found in the Database";
        //        return false;
        //    }

        //    return true;

        //}

        //private bool GetEmployeeFinMastData(string strEmpCode, ref DataTable dt, ref string ErrMsg)
        //{


        //    string sQuery = "Select TOp(1) * FROM FinMast  WITH (NOLOCK) WHERE EmpId = (Select Top(1) EmpID from Employee where EmpCode=@EmpCode)";

        //    //if (IsDataMigrationEnabled == true)
        //    //{
        //    //    sQuery = "Select TOp(1) * FROM FinMast  WITH (NOLOCK) WHERE EmpId = (Select Top(1) EmpID from Employee where EmpCode=@EmpCode) and [status] = 20";
        //    //}

        //    SqlParameter[] Params = new SqlParameter[1];
        //    Params[0] = new SqlParameter("@EmpCode", SqlDbType.VarChar);
        //    Params[0].Value = strEmpCode;


        //    SqlConnection con = new SqlConnection(ConnectionFunctions.GetConnectionString());
        //    if (!ConnectionFunctions.Connect_SQLDataTable(ref dt, sQuery, ref con, Params, ref ErrMsg))
        //    {
        //        ErrMsg = "Error Occurred while retrieving the Employee Financial Details from Database";
        //        return false;
        //    }

        //    return true;

        //}

        //private bool CheckIfColumnExists(DataTable mydt, string colName)
        //{
        //    bool columnExists = mydt.Columns.Cast<DataColumn>()
        //    .Any(c => string.Equals(c.ColumnName, colName, StringComparison.OrdinalIgnoreCase));

        //    return columnExists;
        //}
        //private bool CheckIfMandatory(string xlcolName, enmXlImportTables tableindex)
        //{
        //    bool ismandatory = false;

        //    switch (tableindex)
        //    {
        //        case enmXlImportTables.EOSTran:
        //            ismandatory = MandatoryEOSTranCols.Contains(xlcolName) || SystemMandatoryEOSTranCols.Contains(xlcolName);
        //            break;
        //    }

        //    return ismandatory;
        //}

        //public static bool hasSpecialChar(string input)
        //{
        //    string specialChar = @"[~!@#$%^&*()+=|\\{}':;.,<>/?[\]""_-]";
        //    //string specialChar = @"[~`!@#$%^&*()+=|\\{}':;.,<>/?[\]""_-]";
        //    //object item;
        //    foreach (var item in specialChar)
        //    {
        //        if (input.Contains(item))
        //            return true;
        //    }
        //    return false;
        //}

        //private bool ValidateLookUpCode(DataTable dtLookUpFD, string FieldName, object value, ref string fieldErr, ref object LookUpCode)
        //{
        //    LookUpCode = null;

        //    if (dtLookUpFD == null)
        //    {
        //        return true;
        //    }
        //    if (dtLookUpFD.Rows.Count == 0)
        //    {
        //        return true;
        //    }
        //    DataRow[] drows = dtLookUpFD.Select("FieldPrefix='" + FieldName + "'");
        //    if (drows == null || drows.Length == 0)
        //    {
        //        return true;
        //    }

        //    string sQuery = "";
        //    string lookupDispcolName = "";
        //    string lookupcodecolName = "";
        //    string lookuptable = "";
        //    SqlDataReader dr = null;

        //    if (Convert.ToInt32(drows[0]["FieldType"]) == 4 || Convert.ToInt32(drows[0]["FieldType"]) == 5)
        //    {
        //        //fixed lookup
        //        lookupDispcolName = "DescE";
        //        lookupcodecolName = "Code";
        //        lookuptable = "FixedLookup";
        //        string lookupname = drows[0]["LookTableName"].ToString();

        //        sQuery = "SELECT TOP(1) " + lookupcodecolName + " from " + lookuptable + " with (nolock) where " + lookupcodecolName + " = @CodeFieldValue and LookupName='" + lookupname + "'";
        //    }
        //    else
        //    {

        //        lookupDispcolName = drows[0]["DisplayFieldPrefix"].ToString();
        //        lookupcodecolName = drows[0]["LinkingFieldPrefix"].ToString();
        //        lookuptable = drows[0]["LookTableName"].ToString();

        //        sQuery = "SELECT TOP(1) " + lookupcodecolName + " from " + lookuptable + " with (nolock) where " + lookupcodecolName + " = @CodeFieldValue";

        //    }


        //    SqlParameter[] Params = new SqlParameter[1];
        //    Params[0] = new SqlParameter("@CodeFieldValue", SqlDbType.VarChar);
        //    Params[0].Value = value.ToString();

        //    if (!ConnectionFunctions.Connect_SQLDataReader(ref dr, sQuery, ref errmsg, Params, CommandType.Text))
        //    {
        //        fieldErr = "Error Occurred while retrieving the " + dictTitleNames[FieldName] + " Details from Database";
        //        return false;
        //    }
        //    else
        //    {
        //        if (!dr.HasRows)
        //        {
        //            dr.Close();
        //            fieldErr = dictTitleNames[FieldName] + " which you have entered is not found in the Database";
        //            return false;
        //        }

        //        dr.Read();

        //        if (!dr.IsDBNull(dr.GetOrdinal(lookupcodecolName)))
        //            LookUpCode = dr[lookupcodecolName];
        //    }
        //    dr.Close();

        //    return true;
        //}

        //private bool ValidateLookUp(DataTable dtLookUpFD, string FieldName, object value, ref string fieldErr, ref object LookUpCode)
        //{
        //    LookUpCode = null;

        //    if (dtLookUpFD == null)
        //    {
        //        return true;
        //    }
        //    if (dtLookUpFD.Rows.Count == 0)
        //    {
        //        return true;
        //    }
        //    DataRow[] drows = dtLookUpFD.Select("FieldPrefix='" + FieldName + "'");
        //    if (drows == null || drows.Length == 0)
        //    {
        //        return true;
        //    }

        //    string sQuery = "";
        //    string lookupDispcolName = "";
        //    string lookupcodecolName = "";
        //    string lookuptable = "";
        //    SqlDataReader dr = null;

        //    if (Convert.ToInt32(drows[0]["FieldType"]) == 4 || Convert.ToInt32(drows[0]["FieldType"]) == 5)
        //    {
        //        //fixed lookup
        //        lookupDispcolName = "DescE";
        //        lookupcodecolName = "Code";
        //        lookuptable = "FixedLookup";
        //        string lookupname = drows[0]["LookTableName"].ToString();

        //        sQuery = "SELECT TOP(1) " + lookupcodecolName + " from " + lookuptable + " with (nolock) where " + lookupDispcolName + " = @DisplayFieldValue and LookupName='" + lookupname + "'";
        //    }
        //    else
        //    {

        //        lookupDispcolName = drows[0]["DisplayFieldPrefix"].ToString();
        //        lookupcodecolName = drows[0]["LinkingFieldPrefix"].ToString();
        //        lookuptable = drows[0]["LookTableName"].ToString();

        //        sQuery = "SELECT TOP(1) " + lookupcodecolName + " from " + lookuptable + " with (nolock) where " + lookupDispcolName + " = @DisplayFieldValue";

        //    }


        //    SqlParameter[] Params = new SqlParameter[1];
        //    Params[0] = new SqlParameter("@DisplayFieldValue", SqlDbType.VarChar);
        //    Params[0].Value = value.ToString();

        //    if (!ConnectionFunctions.Connect_SQLDataReader(ref dr, sQuery, ref errmsg, Params, CommandType.Text))
        //    {
        //        fieldErr = "Error Occurred while retrieving the " + dictTitleNames[FieldName] + " Details from Database";
        //        return false;
        //    }
        //    else
        //    {
        //        if (!dr.HasRows)
        //        {
        //            dr.Close();
        //            fieldErr = dictTitleNames[FieldName] + " which you have entered is not found in the Database";
        //            return false;
        //        }

        //        dr.Read();

        //        if (!dr.IsDBNull(dr.GetOrdinal(lookupcodecolName)))
        //            LookUpCode = dr[lookupcodecolName];
        //    }
        //    dr.Close();

        //    return true;
        //}

        //private Boolean ValidateField(string FieldName, object Value, bool isUpdate, ref string fieldErr, ref object LookUpCode, params object[] paraData)
        //{
        //    bool isValid = true;

        //    bool hasSpecial;
        //    DateTime dateValue;
        //    int intValue;
        //    Int16 int16Value;
        //    double dblValue;
        //    decimal decValue;
        //    switch (FieldName)
        //    {
        //        case xlcol_EmpCode:

        //            break;

        //        case xlcol_EOSReason:
        //            if (!ValidateLookUpCode(dtLookUpFieldsDetails_EOSTran, FieldName, Value, ref fieldErr, ref LookUpCode))
        //            {
        //                isValid = false; break;
        //            }
        //            break;

        //        case xlcol_LastDayInService:
        //            if (!ValidateDateTime(Value.ToString(), out dateValue, ref errmsg))
        //            {
        //                fieldErr = "Invalid " + dictTitleNames[FieldName];
        //                isValid = false; break;

        //            }
        //            break;

        //        case xlcol_ResignationDate:
        //            if (!ValidateDateTime(Value.ToString(), out dateValue, ref errmsg))
        //            {
        //                fieldErr = "Invalid " + dictTitleNames[FieldName];
        //                isValid = false; break;

        //            }
        //            break;



        //    }
        //    return isValid;
        //}

        //private bool ValidateDateTime(string strdtToCheck, out DateTime dateValue, ref string errmsg)
        //{
        //    if (!DateTime.TryParseExact(strdtToCheck.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateValue))
        //    {
        //        errmsg = "";
        //        return false;
        //    }
        //    if (dateValue < dtminDate || dateValue > dtmaxDate)
        //    {
        //        errmsg = "Value out of valid date range";
        //        return false;
        //    }
        //    return true;
        //}

        //private DateTime GetValidDateTime(string strDateTime)
        //{

        //    DateTime dtrsltDate = new DateTime(1900, 1, 1);
        //    DateTime.TryParseExact(strDateTime, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtrsltDate);

        //    return dtrsltDate;
        //}




        //private DataTable CreateEOSTransactionTable(ref EOSTran oEOSTran)
        //{
        //    try
        //    {
        //        DataTable dtTranTable = new DataTable("WrkAgrmntDet");
        //        dtTranTable = CreateEOSTranTableSchema();
        //        DataRow rRow = default(DataRow);
        //        rRow = dtTranTable.NewRow();

        //        //Set Row Data
        //        rRow["ReqNo"] = oEOSTran.ReqNo;
        //        rRow["ReqDate"] = oEOSTran.ReqDate;
        //        rRow["EmpID"] = oEOSTran.EmpID;
        //        rRow["LocLib1"] = oEOSTran.LocLib1;
        //        rRow["LocLib2"] = oEOSTran.LocLib2;
        //        rRow["LocLib3"] = oEOSTran.LocLib3;
        //        rRow["LocLib4"] = oEOSTran.LocLib4;
        //        rRow["LocLib5"] = oEOSTran.LocLib5;
        //        rRow["SalProfile"] = oEOSTran.SalProfile;
        //        rRow["JobTitle"] = oEOSTran.JobTitle;
        //        rRow["LastDayInService"] = oEOSTran.LastDayInService;
        //        rRow["EndofServiceType"] = oEOSTran.EndOfServiceType;
        //        rRow["ResignationDate"] = oEOSTran.ResignationDate;
        //        rRow["TerminationDate"] = oEOSTran.TerminationDate;
        //        rRow["EosTypeChar"] = oEOSTran.EosTypeChar;
        //        rRow["EOSRemarks"] = oEOSTran.EOSRemarks;
        //        rRow["Officialtreatment"] = oEOSTran.Officialtreatment;
        //        rRow["EOSReason"] = oEOSTran.EOSReason;
        //        rRow["EOSOffTreat"] = oEOSTran.EOSOffTreat;
        //        rRow["LOCancDate"] = oEOSTran.LOCancDate;
        //        rRow["LOBan"] = oEOSTran.LOBan;
        //        rRow["LOMonths"] = oEOSTran.LOMonths;
        //        rRow["ICancDate"] = oEOSTran.ICancDate;
        //        rRow["IBan"] = oEOSTran.IBan;
        //        rRow["IMonths"] = oEOSTran.IMonths;
        //        rRow["LeavingDate"] = oEOSTran.LeavingDate;
        //        rRow["SCReducingDate"] = oEOSTran.SCReducingDate;
        //        rRow["SCRegNo"] = oEOSTran.SCRegNo;
        //        rRow["LastActLeavingDate"] = oEOSTran.LastActLeavingDate;
        //        rRow["EstRejoiningDate"] = oEOSTran.EstRejoiningDate;
        //        rRow["DiffTillReqDate"] = oEOSTran.DiffTillReqDate;
        //        rRow["LONotifyDate"] = oEOSTran.LONotifyDate;
        //        rRow["LONotifySrNo"] = oEOSTran.LONotifySrNo;
        //        rRow["BankAmt"] = oEOSTran.BankAmt;
        //        rRow["BankDetails"] = oEOSTran.BankDetails;
        //        rRow["ActiveStatus"] = oEOSTran.ActiveStatus;
        //        rRow["Status"] = oEOSTran.Status;
        //        rRow["LastModDateTime"] = oEOSTran.LastModDateTime;
        //        rRow["ReqID"] = oEOSTran.ReqID;
        //        //Denson Added 03/04/2018
        //        rRow["SettleEnt"] = oEOSTran.SettleEnt;
        //        rRow["WrkNPYN"] = oEOSTran.NoticeWrk;
        //        rRow["NPStDt"] = oEOSTran.NoticeWrkDate;


        //        dtTranTable.Rows.Add(rRow);
        //        dtTranTable.AcceptChanges();

        //        Common.HandleLastLoc(oEOSTran.LocLib5, ref dtTranTable);

        //        return dtTranTable;
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }

        //}

        //private DataTable CreateEOSTranTableSchema()
        //{
        //    SqlConnection SQLConn = new SqlConnection(ConnectionFunctions.GetConnectionString());
        //    DataSet EOSTranDS = new DataSet();
        //    try
        //    {
        //        SqlDataAdapter MyDataAdapter = new SqlDataAdapter();
        //        MyDataAdapter.SelectCommand = new SqlCommand("SELECT Top 0 * FROM EOSTran", SQLConn);
        //        SqlCommandBuilder cb = new SqlCommandBuilder(MyDataAdapter);
        //        SQLConn.Open();
        //        MyDataAdapter.Fill(EOSTranDS, "EOSTran");
        //    }
        //    catch (Exception Ex)
        //    {
        //    }
        //    finally
        //    {
        //        if ((SQLConn != null))
        //        {
        //            if (SQLConn.State != ConnectionState.Closed)
        //                SQLConn.Close();
        //        }
        //    }
        //    return EOSTranDS.Tables["EOSTran"];

        //}

        //private bool SaveWrkAgrAppProcess(ref SqlConnection sqlConn, ref SqlTransaction SqlTran, string StrEmpCode, int nEmpID, WrkAgrmntDet oWrkAgrmntDet, ArrayList ApprF, bool byPassed, short ViewNo, string wfc, ref string refErrMsg) //JRR
        //{
        //    bool bError = false;
        //    string strErrorLog = "";
        //    bool RetVal = false;
        //    string ErrMsg = string.Empty;
        //    try
        //    {

        //        ApprProcess oApprProcess = new ApprProcess();

        //        oApprProcess.EmpID = oWrkAgrmntDet.EmpID;
        //        oApprProcess.DocAttach = "";
        //        oApprProcess.App = ApprF[0].ToString();
        //        oApprProcess.AppDate = ApprF[1].ToString();
        //        oApprProcess.NoOfAppr = Convert.ToByte(ApprF[2]);
        //        oApprProcess.ReqNo = oWrkAgrmntDet.ReqNo;
        //        oApprProcess.ReqID = Common.nSvcUserNo; //Convert.ToInt16(UserInfo[Convert.ToInt16(Common.APPR.UserNo)]);
        //        oApprProcess.ViewNo = ViewNo; // WrkAgrmntDet
        //        oApprProcess.Remarks = String.Empty;
        //        //"Limited Contract for Duration 01/01/1900 to 01/01/1900";
        //        switch (oWrkAgrmntDet.WrkAgreeType.ToString())
        //        {
        //            case "1":
        //                //Limited
        //                oApprProcess.Isl = "Limited Contract for Duration [" + oWrkAgrmntDet.WrkAgrStartDt.ToString("dd/MM/yyyy") + "] To [" + oWrkAgrmntDet.WrkAgrExpDt.ToString("dd/MM/yyyy") + "].";
        //                oApprProcess.Isla = "" + oWrkAgrmntDet.WrkAgrStartDt.ToString("dd/MM/yyyy") + ", " + oWrkAgrmntDet.WrkAgrExpDt.ToString("dd/MM/yyyy") + " عقد عمل محدود المدة بين ";
        //                break;
        //            case "2":
        //                //Unlimited
        //                oApprProcess.Isl = "UnLimited Contract with Start Date as [" + oWrkAgrmntDet.WrkAgrStartDt.ToString("dd/MM/yyyy") + "].";
        //                oApprProcess.Isla = "" + oWrkAgrmntDet.WrkAgrStartDt.ToString("dd/MM/yyyy") + " عقد عمل غير محدود المدة يبدأ من ";
        //                break;
        //            default:
        //                oApprProcess.Isl = "Work agreement Request ";
        //                oApprProcess.Isla = "Work agreement Request ";
        //                break;
        //        }
        //        oApprProcess.WFCode = wfc;
        //        if (byPassed == true)
        //        {
        //            oApprProcess.NextApprAuth = "";
        //            oApprProcess.Bypassed = 1;
        //            oApprProcess.Status = 20;
        //            //oWrkAgrmntDet.Status = 20;
        //            //oWrkAgrmntDet.ActiveStatus = 20;
        //        }
        //        else
        //        {
        //            oApprProcess.NextApprAuth = ApprF[3].ToString();
        //            oApprProcess.Bypassed = 0;
        //            oApprProcess.Status = 0;
        //            //oWrkAgrmntDet.Status = 0;
        //            //oWrkAgrmntDet.ActiveStatus = 0;
        //        }
        //        oApprProcess.RequestDate = System.DateTime.Now;
        //        oApprProcess.LastModDateTime = System.DateTime.Now;

        //        oApprProcess.Priority = 0;
        //        //switch (chkUrgent.Checked)
        //        //{
        //        //    case true:
        //        //        oApprProcess.Priority = 1;
        //        //        break;
        //        //    case false:
        //        //        oApprProcess.Priority = 0;
        //        //        break;
        //        //}

        //        RetVal = Common.ApprProcessInsertOrUpdate(oApprProcess, ref sqlConn, ref SqlTran);
        //        if (RetVal == false)
        //        {
        //            refErrMsg = "Could not save Approval for the Transaction.Please contact Administrator. ";
        //            return false;
        //        }

        //        return true;

        //    }
        //    catch (Exception ex)
        //    {

        //        refErrMsg = "An Unexpected error occured while Saving Approval for the Work Agreement Transaction. ";
        //        return false;
        //    }

        //}

        //private bool Update_LASTUPDDTTM(string strEmpCode, DateTime lastUpdateDate, string updateModule)
        //{
        //    errmsg = "";

        //    if (string.IsNullOrEmpty(strEmpCode))
        //    {
        //        return false;
        //    }
        //    string updateModulecolname = "";
        //    switch (updateModule.ToLower())
        //    {
        //        case "employee":
        //            updateModulecolname = "Employee_LASTUPDDTTM";
        //            break;
        //        case "wrk":
        //            updateModulecolname = "WrkAgreement_LASTUPDDTTM";
        //            break;
        //        case "fin":
        //            updateModulecolname = "Financial_LASTUPDDTTM";
        //            break;
        //        case "finC":
        //            updateModulecolname = "FinancialChanges_LASTUPDDTTM";
        //            break;
        //        case "eos":
        //            updateModulecolname = "EOS_LASTUPDDTTM";
        //            break;
        //    }

        //    sQry = "IF NOT EXISTS (SELECT * FROM SFI_EmpLastUpdateDate WHERE empcode='" + strEmpCode + "')" +
        //        "BEGIN " +
        //        "   Insert into SFI_EmpLastUpdateDate ([EmpCode]," + updateModulecolname + ") values ('" + strEmpCode + "','" + lastUpdateDate.ToString("yyyyMMdd HH:mm") + "');" +
        //        "End " +
        //        "Else " +
        //        "BEGIN " +
        //        "   UPDATE SFI_EmpLastUpdateDate set " + updateModulecolname + " = '" + lastUpdateDate.ToString("yyyyMMdd HH:mm") + "' Where [EmpCode]='" + strEmpCode + "';" +
        //        "End ";

        //    RetVal = ConnectionFunctions.Connect_SQLNonQuery(ref result, sQry, ref errmsg);

        //    if (!RetVal)
        //    {
        //        Common.LogAction("Update_LASTUPDDTTM failed. Details: " + errmsg);
        //    }

        //    return RetVal;
        //}

        //private bool AddProcessSummaryRow()
        //{


        //    errmsg = "";

        //    //sQry = "DELETE From [SFIToCSProcessSummary] where [FileName]='" + importfileName + "' and [RowNo]='" + iCurrRowNo + "'; " +
        //    //    "Insert into [SFIToCSProcessSummary]  ([FileName],[RowNo])values ('" + importfileName + "','" + iCurrRowNo + "');";

        //    sQry = "IF NOT EXISTS (Select * FROM SFIToCSProcessSummary Where  [FileName]='" + importfileName + "' and [RowNo]='" + iCurrRowNo + "') " +
        //        " BEGIN" +
        //        " Insert into [SFIToCSProcessSummary]  ([FileName],[RowNo])values ('" + importfileName + "','" + iCurrRowNo + "');" +
        //        " END";

        //    RetVal = ConnectionFunctions.Connect_SQLNonQuery(ref result, sQry, ref errmsg);


        //    return RetVal;
        //}
        //private bool UpdateProcessSummaryRow()
        //{

        //    errmsg = "";

        //    sQry = "Update [SFIToCSProcessSummary]  set EmpCode=@EmpCode," +
        //         "[HasLineErrors]=@HasLineErrors," +
        //         "[Data_Saved]=@Data_Saved,[FileType]=@FileType,[File_HasBRError]=@File_HasBRError," +
        //         "[LoggedDate]=getdate(),[Remarks]=@Remarks" +
        //         " where [FileName]=@fileName and [RowNo]=@RowNo; ";

        //    SqlParameter[] Params = new SqlParameter[8];
        //    Params[0] = new SqlParameter("@EmpCode", SqlDbType.VarChar);
        //    Params[0].Value = strCurrEmpCode;
        //    Params[1] = new SqlParameter("@HasLineErrors", SqlDbType.VarChar);
        //    Params[1].Value = (bCurrHasLineErrors ? "1" : "0");
        //    Params[2] = new SqlParameter("@Data_Saved", SqlDbType.VarChar);
        //    Params[2].Value = (bEOSTranSaved ? "1" : "0");
        //    Params[3] = new SqlParameter("@FileType", SqlDbType.VarChar);
        //    Params[3].Value = "EOS";
        //    Params[4] = new SqlParameter("@File_HasBRError", SqlDbType.VarChar);
        //    Params[4].Value = (bHasEOSTranBRerror ? "1" : "0");
        //    Params[5] = new SqlParameter("@fileName", SqlDbType.VarChar);
        //    Params[5].Value = importfileName;
        //    Params[6] = new SqlParameter("@Remarks", SqlDbType.VarChar);
        //    Params[6].Value = sbSaveMsg.ToString();
        //    Params[7] = new SqlParameter("@RowNo", SqlDbType.VarChar);
        //    Params[7].Value = iCurrRowNo;

        //    RetVal = ConnectionFunctions.Connect_SQLNonQuery(ref result, sQry, ref errmsg, Params, CommandType.Text);

        //    if (!RetVal)
        //    {
        //        Common.LogAction("ProcessSummaryRow Update failed. Details : " + errmsg);
        //    }

        //    return RetVal;
        //}

        //private bool GetDefaultSaveValues(string LocLib5Code, ref DataTable dt, ref string ErrMsg)
        //{


        //    ErrMsg = "";



        //    //string sQuery = "Select LocalCurr from Prgdefault1 where ProfileCode = (Select TOP(1) ProfileCode from LocLib1 where Code= dbo.fun_GetLocLib1WithLocLib5(@LocLib5Code));";
        //    string sQuery = "USP_SFIToCSImport_GetDefaultSaveValues";

        //    SqlParameter[] Params = new SqlParameter[1];
        //    Params[0] = new SqlParameter("@LocLib5", SqlDbType.VarChar);
        //    Params[0].Value = LocLib5Code;


        //    if (!ConnectionFunctions.Connect_SQLDataTable(ref dt, sQuery, ref errmsg, Params, CommandType.StoredProcedure))
        //    {
        //        ErrMsg = errmsg;
        //        return false;
        //    }

        //    return true;


        //}

        //private string GetXLColName(string columnName)
        //{
        //    string xlcolumnname = columnName;
        //    //switch (columnName)
        //    //{
        //    //    case xlcol_ProbationPeriod:
        //    //        xlcolumnname = "ProbPeriod";
        //    //        break;
        //    //    case xlcol_ResignationDate:
        //    //        xlcolumnname = "WrkAgrEndDt";
        //    //        break;
        //    //}
        //    return xlcolumnname;
        //}

        //private bool IsEOSPosted(int sEmpId, string lastLoc, int reqNo, ref int eosCount, ref string ErrMsg)
        //{
        //    ErrMsg = "";
        //    eosCount = 0;
        //    SqlDataReader MyReader = null;
        //    try
        //    {
        //        string qry = "SELECT COUNT(1) AS CNT FROM EOSTRAN WHERE empid = " + sEmpId + "AND LocLib5 = '" + lastLoc + "' AND ReqNo <> " + reqNo + " and Status <= 20";
        //        Boolean RetVal = true;
        //        RetVal = ConnectionFunctions.Connect_SQLDataReader(ref MyReader, qry, ref ErrMsg);
        //        if (MyReader.Read())
        //        {
        //            eosCount = Convert.ToInt32(MyReader[0].ToString());
        //        }

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        if (MyReader.IsClosed == false)
        //            MyReader.Close();

        //        ErrMsg = ex.Message;
        //        return false;
        //    }

        //}


        //private bool IsTransfered(int sEmpId, int reqNo, ref int tnsfrCount, ref string ErrMsg)
        //{
        //    ErrMsg = "";
        //    tnsfrCount = 0;
        //    SqlDataReader MyReader = null;
        //    try
        //    {
        //        string qry = "SELECT CASE WHEN (Endofservicetype IN ('5','8') AND status <> 40) THEN 1 ELSE 0 END 'Val' FROM EOSTran WHERE EmpID = " + sEmpId + " AND ReqNo <> " + reqNo;
        //        Boolean RetVal = true;
        //        RetVal = ConnectionFunctions.Connect_SQLDataReader(ref MyReader, qry, ref ErrMsg);
        //        if (MyReader.Read())
        //        {
        //            tnsfrCount = Convert.ToInt32(MyReader[0].ToString());
        //        }
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        if (MyReader.IsClosed == false)
        //            MyReader.Close();

        //        ErrMsg = ex.Message;
        //        return false;
        //    }

        //}


        //public bool InsertEOSTran(EOSTran oEosTran, ApprProcess oApprProcess)
        //{
        //    try
        //    {
        //        TransactionOptions tso = new TransactionOptions();
        //        tso.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
        //        bool bResult = false;
        //        using (TransactionScope sc = new TransactionScope(TransactionScopeOption.Required, tso))
        //        {
        //            SqlConnection myConn = new SqlConnection();
        //            myConn.ConnectionString = ConnectionFunctions.GetConnectionString();
        //            myConn.Open();

        //            Employee oEmployee = new Employee();
        //            oEmployee = EmployeeHelper.GetEmployeeDetailsByEmpId(oEosTran.EmpID, ref myConn);

        //            // Insert FinReqMast Record
        //            bResult = EOSTranHelper.Insert(oEosTran, ref myConn);

        //            if (bResult)
        //            {
        //                // Audit Trail Insert For FinReqMast
        //                AuditTrail oAuditTrail = new AuditTrail();
        //                oAuditTrail.Table = "EOSTran";
        //                oAuditTrail.Transaction = "Insert (WEB)";
        //                oAuditTrail.EmpCode = oEmployee.EmpCode;
        //                oAuditTrail.UserID = Common.strSvcUserId;
        //                oAuditTrail.Errors = "EOS Insert";
        //                oAuditTrail.TransactionNo = oEosTran.ReqNo;
        //                oAuditTrail.Date = oEosTran.ReqDate;
        //                oAuditTrail.Flag = 0;
        //                oAuditTrail.WComp = string.Empty;
        //                bResult = AuditTrailHelper.Insert(oAuditTrail, ref myConn);


        //                // Insert ApprProcess Record
        //                oApprProcess.ReqNo = oEosTran.ReqNo;
        //                bResult = ApprProcessHelper.Insert(oApprProcess, ref myConn);

        //            }


        //            sc.Complete();
        //        }
        //        return bResult;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //    finally
        //    {
        //    }
        //}

        //private string GetLookupDescByCode(enmXlImportTables table, string FieldName, string value)
        //{
        //    DataTable dt = new DataTable();
        //    string ErrMsg = "";
        //    string desc = "";
        //    if (GetLookupTableByCode(table, FieldName, value, ref dt, ref ErrMsg))
        //    {
        //        if (dt != null && dt.Rows.Count > 0)
        //        {
        //            desc = dt.Rows[0][1] == DBNull.Value ? "" : dt.Rows[0][1].ToString();
        //        }
        //    }
        //    return desc;
        //}
        //private bool GetLookupTableByCode(enmXlImportTables table, string FieldName, string value, ref DataTable dt, ref string ErrMsg)
        //{
        //    DataTable dtLookUpFD = new DataTable();

        //    switch (table)
        //    {
        //        case enmXlImportTables.Employee:
        //            dtLookUpFD = dtLookUpFieldsDetails_Emp;
        //            break;
        //        case enmXlImportTables.EOSTran:
        //            dtLookUpFD = dtLookUpFieldsDetails_EOSTran;
        //            break;
        //            //case enmXlImportTables.PayDetails:
        //            //    dtLookUpFD = dtLookUpFieldsDetails_PayDetails;
        //            //    break;
        //            //case enmXlImportTables.WrkAgrmntDet:
        //            //    dtLookUpFD = dtLookUpFieldsDetails_WrkAgrmntDet;
        //            //    break;

        //    }

        //    DataRow[] drows = dtLookUpFD.Select("FieldPrefix='" + FieldName + "'");
        //    if (drows == null || drows.Length == 0)
        //    {
        //        ErrMsg = "Error Occurred while retrieving the " + dictTitleNames[FieldName] + " Details from Database"; ;
        //        return false;
        //    }
        //    string sQuery = "";
        //    string lookupDispcolName = "";
        //    string lookupcodecolName = "";
        //    string lookuptable = "";

        //    if (Convert.ToInt32(drows[0]["FieldType"]) == 4 || Convert.ToInt32(drows[0]["FieldType"]) == 5)
        //    {
        //        //fixed lookup
        //        lookupDispcolName = "DescE";
        //        lookupcodecolName = "Code";
        //        lookuptable = "FixedLookup";
        //        string lookupname = drows[0]["LookTableName"].ToString();

        //        sQuery = "SELECT TOP(1) " + lookupcodecolName + "," + lookupDispcolName + ",* from " + lookuptable + " with (nolock) where " + lookupcodecolName + " = @CodeFieldValue and LookupName='" + lookupname + "'";
        //    }
        //    else
        //    {

        //        lookupDispcolName = drows[0]["DisplayFieldPrefix"].ToString();
        //        lookupcodecolName = drows[0]["LinkingFieldPrefix"].ToString();
        //        lookuptable = drows[0]["LookTableName"].ToString();

        //        sQuery = "SELECT TOP(1) " + lookupcodecolName + "," + lookupDispcolName + ",* from " + lookuptable + " with (nolock) where " + lookupcodecolName + " = @CodeFieldValue";

        //    }


        //    SqlParameter[] Params = new SqlParameter[1];
        //    Params[0] = new SqlParameter("@CodeFieldValue", SqlDbType.VarChar);
        //    Params[0].Value = value.ToString();



        //    SqlConnection con = new SqlConnection(ConnectionFunctions.GetConnectionString());
        //    if (!ConnectionFunctions.Connect_SQLDataTable(ref dt, sQuery, ref con, Params, ref ErrMsg))
        //    {
        //        ErrMsg = "Error Occurred while retrieving the " + dictTitleNames[FieldName] + " Details from Database";
        //        return false;
        //    }
        //    if (dt.Rows.Count == 0)
        //    {
        //        ErrMsg = dictTitleNames[FieldName] + "which you have entered is not found in the Database";
        //        return false;
        //    }

        //    return true;

        //}


        //private string GetEOSTypeByEosReasonCode(string strEOSreasonCode)
        //{
        //    DataTable dt = new DataTable();
        //    string ErrMsg = "";
        //    string eostype = "";
        //    if (GetLookupTableByCode(enmXlImportTables.EOSTran, xlcol_EOSReason, strEOSreasonCode, ref dt, ref ErrMsg))
        //    {
        //        if (dt != null && dt.Rows.Count > 0)
        //        {
        //            eostype = dt.Rows[0]["EosType"] == DBNull.Value ? "" : dt.Rows[0]["EosType"].ToString();
        //        }
        //    }
        //    return eostype;
        //}

    }
}
