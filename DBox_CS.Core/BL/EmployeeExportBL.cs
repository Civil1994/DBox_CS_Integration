using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DBox_CS.Core.APIClient;
using DBox_CS.Core.AppClass;
using DBox_CS.Core.DALayer;
using DBox_CS.Core.Models;

namespace DBox_CS.Core.BL
{
    public class EmployeeExportBL
    {
        private ApiClient _apiClient;
        private HttpClient _httpClient;

        private bool RetVal = false;
        string errmsg = "";
        String sQry = String.Empty;
        private int result = 0;
        internal DataTable GetEmployeeForExportToUniFocus()
        {
            string errmsg = "";
            DataTable dt = new DataTable();
            string sQuery = "USP_UFIExport_GetEmployeeForPosting";

            SqlParameter[] Params = null;

            if (!ConnectionFunctions.Connect_SQLDataTable(ref dt, sQuery, ref errmsg, Params, CommandType.StoredProcedure))
            {
                Common.LogAction(" GetEmployeeForExportToUniFocus error. Details:" + errmsg);
            }

            return dt;
        }

        internal UFEmployeeDTO ConvertToEmployeeDto(DataRow drow)
        {
            UFEmployeeDTO empDTO = new UFEmployeeDTO();

            empDTO.employeeId = drow["EmployeeId"] == DBNull.Value ? "" : drow["EmployeeId"].ToString();
            empDTO.altId = drow["AltId"] == DBNull.Value ? "" : drow["AltId"].ToString();
            empDTO.badgeNo = drow["BadgeNo"] == DBNull.Value ? "" : drow["BadgeNo"].ToString();
            empDTO.firstName = drow["FirstName"] == DBNull.Value ? "" : drow["FirstName"].ToString();
            empDTO.middleName = drow["MiddleName"] == DBNull.Value ? "" : drow["MiddleName"].ToString();
            empDTO.lastName = drow["LastName"] == DBNull.Value ? "" : drow["LastName"].ToString();
            empDTO.employeeType = drow["EmployeeType"] == DBNull.Value ? "" : drow["EmployeeType"].ToString();
            empDTO.hireDate = drow["HireDate"] == DBNull.Value ? "" : drow["HireDate"].ToString();
            empDTO.seniorityDate = drow["SeniorityDate"] == DBNull.Value ? "" : drow["SeniorityDate"].ToString();
            empDTO.statusTypeCode = drow["StatusTypeCode"] == DBNull.Value ? "" : drow["StatusTypeCode"].ToString();
            empDTO.hoursAvailable = drow["HoursAvailable"] == DBNull.Value ? "0" : drow["HoursAvailable"].ToString();
            empDTO.statusChangeReasonCode = drow["StatusChangeReasonCode"] == DBNull.Value ? "" : drow["StatusChangeReasonCode"].ToString();
            empDTO.workClassCode = drow["WorkClassCode"] == DBNull.Value ? "" : drow["WorkClassCode"].ToString();
            empDTO.primaryClassCode = drow["PrimaryClassCode"] == DBNull.Value ? "" : drow["PrimaryClassCode"].ToString();
            empDTO.secondaryClassCode = drow["SecondaryClassCode"] == DBNull.Value ? "" : drow["SecondaryClassCode"].ToString();
            empDTO.birthDate = drow["BirthDate"] == DBNull.Value ? "" : drow["BirthDate"].ToString();
            empDTO.terminationDate = drow["TerminationDate"] == DBNull.Value ? "" : drow["TerminationDate"].ToString();
            empDTO.leaveOfAbsenceDate = drow["LeaveOfAbsenceDate"] == DBNull.Value ? "" : drow["LeaveOfAbsenceDate"].ToString();
            empDTO.leaveOfAbsenceReturnDate = drow["LeaveOfAbsenceReturnDate"] == DBNull.Value ? "" : drow["LeaveOfAbsenceReturnDate"].ToString();
            empDTO.reHireDate = drow["ReHireDate"] == DBNull.Value ? "" : drow["ReHireDate"].ToString();
            empDTO.gender = drow["Gender"] == DBNull.Value ? "" : drow["Gender"].ToString();
            empDTO.tipped = drow["Tipped"] == DBNull.Value ? "" : drow["Tipped"].ToString();
            empDTO.address = drow["Address"] == DBNull.Value ? "" : drow["Address"].ToString();
            empDTO.address2 = drow["Address2"] == DBNull.Value ? "" : drow["Address2"].ToString();
            empDTO.city = drow["City"] == DBNull.Value ? "" : drow["City"].ToString();
            empDTO.state = drow["State"] == DBNull.Value ? "" : drow["State"].ToString();
            empDTO.zip = drow["Zip"] == DBNull.Value ? "" : drow["Zip"].ToString();
            empDTO.homePhone = drow["HomePhone"] == DBNull.Value ? "" : drow["HomePhone"].ToString();
            empDTO.mobilePhone = drow["MobilePhone"] == DBNull.Value ? "" : drow["MobilePhone"].ToString();
            empDTO.email = drow["Email"] == DBNull.Value ? "" : drow["Email"].ToString();
            empDTO.emergencyContact = drow["EmergencyContact"] == DBNull.Value ? "" : drow["EmergencyContact"].ToString();
            empDTO.emergencyPhone = drow["EmergencyPhone"] == DBNull.Value ? "" : drow["EmergencyPhone"].ToString();
            empDTO.propertyCode = drow["PropertyCode"] == DBNull.Value ? "" : drow["PropertyCode"].ToString();


            UFReconcileJobDetailsDTO jobDetailsDTO = new UFReconcileJobDetailsDTO();

            jobDetailsDTO.jobCode = drow["JobCode"] == DBNull.Value ? "" : drow["JobCode"].ToString();
            jobDetailsDTO.jobDate = drow["JobDate"] == DBNull.Value ? "" : drow["JobDate"].ToString();
            //jobDetailsDTO.jobRank = drow["JobRank"] == DBNull.Value ? "0" : drow["JobRank"].ToString();
            jobDetailsDTO.rateType = drow["RateType"] == DBNull.Value ? "" : drow["RateType"].ToString();
            //jobDetailsDTO.hourlyRate = drow["HourlyRate"] == DBNull.Value ? "0" : drow["HourlyRate"].ToString();
            jobDetailsDTO.annualRate = drow["AnnualRate"] == DBNull.Value ? "" : drow["AnnualRate"].ToString();
            //jobDetailsDTO.pieceRate = drow["PieceRate"] == DBNull.Value ? "0" : drow["PieceRate"].ToString();
            //jobDetailsDTO.contractHours = drow["ContractHours"] == DBNull.Value ? "0" : drow["ContractHours"].ToString();
            //jobDetailsDTO.contractDays = drow["ContractDays"] == DBNull.Value ? "0" : drow["ContractDays"].ToString();
            jobDetailsDTO.rateDate = drow["RateDate"] == DBNull.Value ? "" : drow["RateDate"].ToString();
            jobDetailsDTO.jobOrder = drow["JobOrder"] == DBNull.Value ? "" : drow["JobOrder"].ToString();
            //jobDetailsDTO.deactivationDate = drow["DeactivationDate"] == DBNull.Value ? "" : drow["DeactivationDate"].ToString();


            empDTO.jobs= new List<UFReconcileJobDetailsDTO>();
            empDTO.jobs.Add(jobDetailsDTO);

            //UFEmployeeCustomDataDTO empcustomDataDTO = new UFEmployeeCustomDataDTO();
            //empcustomDataDTO.key = "";
            //empcustomDataDTO.value = "";

            //empDTO.customDataList = new List<UFEmployeeCustomDataDTO>();
            //empDTO.customDataList.Add(empcustomDataDTO);


            return empDTO;

        }

        internal void UploadeEmployeeToUF(ref int ufiProcessId)
        {


            //int ufiProcessId = 0;
            bool hasProcessError = false;
            string strprocessRemarks = "";

            ufiProcessId = Common.CreateUFIProcessLogEntry("Employee Upload to UF", "");


            if (ufiProcessId == 0)
            {
                Common.LogAction("Error generating UFI Process ID");
                return;
            }


            try
            {
                string apikeyheader = ConfigurationManager.AppSettings["UFApiSettings.APIKeyHeader"].ToString();
                string apikey = ConfigurationManager.AppSettings["UFApiSettings.APIKey"].ToString();


                if (string.IsNullOrEmpty(apikeyheader))
                {
                    throw new Exception("API Key Header missing.");
                }
                if (string.IsNullOrEmpty(apikey))
                {
                    throw new Exception("API Key missing.");
                }


                _httpClient = new HttpClient();
                _apiClient = new ApiClient(_httpClient, apikey, apikeyheader);



                bool continuePosting = false;
                DataTable empdt = GetEmployeeForExportToUniFocus();

                if (empdt == null || empdt.Rows.Count == 0)
                {
                    Common.LogAction("No Employee data to Upload.");
                    Common.UpdateRemarksToUFIExportProcessLogDetails(ufiProcessId, "", "", "No Employee data to Upload.");
                    return;
                }

                LogExportData(empdt, ufiProcessId);


                var distinctUFPropertyCodes = empdt.AsEnumerable().Select(row => row.Field<string>("PropertyCode")).Distinct().ToList();

                string empcode = "";
                UFEmployeeDTO empDTO = new UFEmployeeDTO();
                foreach (string ufpc in distinctUFPropertyCodes)
                {
                    continuePosting = true;

                    if (string.IsNullOrEmpty(ufpc))
                    {
                        DataRow[] drows = empdt.Select("ISNULL(PropertyCode,'')= ''");

                        foreach (DataRow drow in drows)
                        {
                            hasProcessError = true;
                            Common.LogErrorToUFIExportProcessLogDetails(ufiProcessId, drow["EmployeeId"].ToString(), "", "No Valid Property Code found for Employee " + drow["EmployeeId"].ToString());
                        }
                    }
                    else
                    {


                        Common.LogAction("Posting Employees for PropertyCode " + ufpc);

                        DataRow[] drows = empdt.Select("PropertyCode= '" + ufpc + "'");

                        foreach (DataRow drow in drows)
                        {
                            Common.LogAction("Posting Employee " + drow["EmployeeId"].ToString());

                            try
                            {
                                empDTO = ConvertToEmployeeDto(drow);
                                empcode = empDTO.employeeId.ToString();

                                var response = _apiClient.PostEmployeeData(empDTO, ufpc, empcode);

                                if (response == null)
                                {
                                    hasProcessError = true;
                                    Common.LogAction("api response is null");
                                    Common.LogErrorToUFIExportProcessLogDetails(ufiProcessId, empDTO.employeeId.ToString(), "", "api response is null");
                                }
                                else
                                {
                                    if (response.IsSuccessStatusCode)
                                    {

                                        Common.LogAction("Posted " + empDTO.employeeId.ToString());
                                        Common.UpdateRemarksToUFIExportProcessLogDetails(ufiProcessId, empDTO.employeeId.ToString(), "", "Posted Successfully");
                                        Update_EmpLastExportDateTime(empDTO.employeeId.ToString(), DateTime.Now);
                                    }
                                    else
                                    {


                                        //var resultContent = response.Content.ReadAsStringAsync().Result;
                                        //if (resultContent != null)
                                        //{
                                        //    var respMsg = JsonConvert.DeserializeObject<Message<string>>(resultContent);

                                        //}

                                        //200   successful operation
                                        //400   bad request -Incorrect values.
                                        //403   forbidden request -Invalid "unifocus-api-secret".
                                        //500   internal server error - location Code does not exist.


                                        if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                                        {
                                            hasProcessError = true;
                                            Common.LogAction("Failure StatusCode " + ((int)response.StatusCode).ToString() + " bad request -Incorrect values.");
                                            Common.LogErrorToUFIExportProcessLogDetails(ufiProcessId, empDTO.employeeId.ToString(), "", "Failure StatusCode " + ((int)response.StatusCode).ToString() + " bad request -Incorrect values.");
                                        }
                                        else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                                        {
                                            hasProcessError = true;
                                            Common.LogAction("Failure StatusCode " + ((int)response.StatusCode).ToString() + " forbidden request -Invalid 'unifocus - api - secret'.");
                                            Common.LogErrorToUFIExportProcessLogDetails(ufiProcessId, empDTO.employeeId.ToString(), "", "Failure StatusCode " + ((int)response.StatusCode).ToString() + " forbidden request -Invalid 'unifocus - api - secret'. Further Employee Posting for Property Code '" + ufpc + "' Skipped due to Critical error. ");
                                            continuePosting = false;
                                        }
                                        else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                                        {
                                            hasProcessError = true;
                                            Common.LogAction("Failure StatusCode " + ((int)response.StatusCode).ToString() + " internal server error - location Code does not exist");
                                            Common.LogErrorToUFIExportProcessLogDetails(ufiProcessId, empDTO.employeeId.ToString(), "", "Failure StatusCode " + ((int)response.StatusCode).ToString() + " internal server error - location Code does not exist. Further Employee Posting for Property Code '" + ufpc + "' Skipped due to Critical error. ");
                                            continuePosting = false;
                                        }
                                        else
                                        {
                                            hasProcessError = true;
                                            Common.LogAction("Failure StatusCode " + ((int)response.StatusCode).ToString());
                                            Common.LogErrorToUFIExportProcessLogDetails(ufiProcessId, empDTO.employeeId.ToString(), "", "Failure StatusCode " + ((int)response.StatusCode).ToString());

                                        }

                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                hasProcessError = true;
                                Common.LogAction("Error occured at Posting for Empcode "+ drow["EmployeeId"].ToString() + ". Error:" + ex.Message);
                                Common.LogException(ex);
                                Common.LogErrorToUFIExportProcessLogDetails(ufiProcessId, drow["EmployeeId"].ToString(), "", "Error occured at Posting. Error:" + ex.Message);
                            }


                            if (continuePosting == false)
                            {
                                break;
                            }


                        }
                    }

                }
            }
            catch (Exception ex)
            {
                strprocessRemarks = "An error occured. Check Process Log Details";
                hasProcessError = true;
                Common.LogAction("Error occured at Posting. Error:" + ex.Message);
                Common.LogException(ex);
                Common.LogErrorToUFIExportProcessLogDetails(ufiProcessId, "", "", "Error occured at Posting. Error:" + ex.Message);
            }

           
            Common.LogUFIProcessCompletion(ufiProcessId, strprocessRemarks,hasProcessError);

        }

        private void LogExportData(DataTable dt,int ufiProcessId)
        {
            try
            {
                dt.Columns.Add("UFIProcessId", Type.GetType("System.Int32"));
                dt.Columns.Add("InsertedDate", Type.GetType("System.DateTime"));

                DateTime inserttime = DateTime.Now;

                foreach (DataRow row in dt.Rows)
                {
                    row["UFIProcessId"] = ufiProcessId;
                    row["InsertedDate"] = inserttime;
                }

                dt.Columns["UFIProcessId"].SetOrdinal(0);
                dt.Columns["InsertedDate"].SetOrdinal(1);

                SqlConnection conn = new SqlConnection(ConnectionFunctions.GetConnectionString());

                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
                {
                    bulkCopy.DestinationTableName = "dbo.UFI_EmployeeUploadDataLog";

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

        private bool Update_EmpLastExportDateTime(string strEmpCode, DateTime lastUpdateDate)
        {
            errmsg = "";

            if (string.IsNullOrEmpty(strEmpCode))
            {
                return false;
            }
            string actualEmpCode = strEmpCode;

            if (!actualEmpCode.StartsWith("J"))
            {
                actualEmpCode = "J" + actualEmpCode; //appending J for JA employees Empcode which was removed the data fetch procecure
            }
           

            sQry = "IF NOT EXISTS (SELECT * FROM UFI_EmpLastExportDate WHERE empcode='" + actualEmpCode + "')" +
                "BEGIN " +
                "   Insert into UFI_EmpLastExportDate ([EmpCode],[LastExportDateTime]) values ('" + actualEmpCode + "','" + lastUpdateDate.ToString("yyyyMMdd HH:mm") + "');" +
                "End " +
                "Else " +
                "BEGIN " +
                "   UPDATE UFI_EmpLastExportDate set [LastExportDateTime] = '" + lastUpdateDate.ToString("yyyyMMdd HH:mm") + "' Where [EmpCode]='" + actualEmpCode + "';" +
                "End ";

            RetVal = ConnectionFunctions.Connect_SQLNonQuery(ref result, sQry, ref errmsg);

            if (!RetVal)
            {
                Common.LogAction("Update_EmpLastExportDateTime failed. Details: " + errmsg);
            }

            return RetVal;
        }
    }
}
