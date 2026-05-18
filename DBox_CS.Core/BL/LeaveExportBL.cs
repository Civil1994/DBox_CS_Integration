using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBox_CS.Core.AppClass;
using DBox_CS.Core.DALayer;
using DBox_CS.Core.Utility;

namespace DBox_CS.Core.BL
{
    public class LeaveExportBL
    {
        private readonly SFTPService _sFTPService;
        private bool RetVal = false;
        string errmsg = "";
        String sQry = String.Empty;
        private int result = 0;
        string outputfilformat_stringpart = "CSLeaveInfo_{0}";
        string outputfilformat_datepart = "yyyyMMdd_HHmm";
        public string outputtempfolder = "";


        public LeaveExportBL(SFTPService sFTPService)
        {
            _sFTPService = sFTPService;
        }

        internal DataTable GetLeaveDetailsForExportToUniFocus()
        {
            string errmsg = "";
            DataTable dt = new DataTable();
            string sQuery = "USP_UFIExport_GetLeaveForPosting";

            SqlParameter[] Params = new SqlParameter[1];
            Params[0] = new SqlParameter("@OutputWithExportColumnsOnly", "1");

            if (!ConnectionFunctions.Connect_SQLDataTable(ref dt, sQuery, ref errmsg, Params, CommandType.StoredProcedure))
            {
                Common.LogAction("GetLeaveDetailsForExportToUniFocus error. Details:" + errmsg);
            }

            return dt;
        }

        internal void UploadeLeaveToUF(ref int ufiProcessId)
        {


            //int ufiProcessId = 0;
            bool hasProcessError = false;
            string strprocessRemarks = "";
            int totalreccount = 0;
            RetVal = false;
            errmsg = "";

            ufiProcessId = Common.CreateUFIProcessLogEntry("Leave Upload to UF", "");


            if (ufiProcessId == 0)
            {
                Common.LogAction("Error generating UFI Process ID for 'Leave Upload to UF' process");
                return;
            }


            try
            {
               

                DataTable leavedt = GetLeaveDetailsForExportToUniFocus();

                if (leavedt == null || leavedt.Rows.Count == 0)
                {
                    Common.LogAction("No Leave data to Upload.");
                    Common.UpdateRemarksToUFIExportProcessLogDetails(ufiProcessId, "", "", "No Leave data to Upload.");
                    return;
                }

                bool isfileCreated = false;
                string outputFolderPath = outputtempfolder;
                string outputfilename = string.Format(outputfilformat_stringpart, DateTime.Now.ToString(outputfilformat_datepart));
                string csvFilePath = Path.Combine(outputFolderPath, $"{outputfilename}.csv");

                // Ensure the output folder exists
                if (!Directory.Exists(outputFolderPath))
                {
                    Directory.CreateDirectory(outputFolderPath);
                }

                try
                {
                    //Create a StringBuilder to build the CSV content
                    StringBuilder csvContent = new StringBuilder();

                    //Write the header row to CSV
                    var columnNames = leavedt.Columns.Cast<DataColumn>()
                        .Select(column => Common.EscapeCsvValue(column.ColumnName))
                        .ToArray();

                    csvContent.AppendLine(string.Join(",", columnNames));

                    //Write the data rows for each FileName
                    foreach (DataRow row in leavedt.Rows)
                    {
                        var values = row.ItemArray
                            .Select(field => Common.EscapeCsvValue(field.ToString()))
                            .ToArray();

                        csvContent.AppendLine(string.Join(",", values));
                    }

                    totalreccount = leavedt.Rows.Count;

                    File.WriteAllText(csvFilePath, csvContent.ToString());

                    isfileCreated = true;
                    Common.LogAction("Leave CSV File generated");
                }
                catch (Exception ex)
                {

                    hasProcessError = true;
                    Common.LogAction("Leave CSV File Generation Error. Details:" + ex.Message);
                    Common.LogErrorToUFIExportProcessLogDetails(ufiProcessId, "", "", "CSV File Generation Error. Details:"+ ex.Message);
                }
               


                if (isfileCreated)
                {

                    ////RetVal=UploadFileToSFTP(csvFilePath, ref errmsg);
                    RetVal = _sFTPService.UploadFileToSFTP("UFILEAVE", csvFilePath, ref errmsg);
                    if (!RetVal)
                    {
                        hasProcessError = true;
                        Common.LogAction("Leave File SFTP Upload Error. Details: " + errmsg);
                        Common.LogErrorToUFIExportProcessLogDetails(ufiProcessId, "", "", "Leave File SFTP Upload Error. Details: "+errmsg);
                    }
                   
                }




            }
            catch (Exception ex)
            {
                
                hasProcessError = true;
                Common.LogAction("Error occured at UploadLeaveToUF. Details:" + ex.Message);
                Common.LogException(ex);
                Common.LogErrorToUFIExportProcessLogDetails(ufiProcessId, "", "", "Error occured at UploadLeaveToUF. Details:" + ex.Message);
            }

            if(hasProcessError)
            {
                strprocessRemarks = "An error occured. Check Process Log Details";
            }
            else
            {
                strprocessRemarks = "Total Uploaded records:"+ totalreccount.ToString();
            }
            Common.LogUFIProcessCompletion(ufiProcessId, strprocessRemarks, hasProcessError);

        }
        
    }
}
