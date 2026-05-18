using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DBox_CS.WebAPI.Models;
using DBox_CS.WebAPI.DAL;
using DBox_CS.WebAPI.Services;
using DBox_CS.Core.AppClass;
using DBox_CS.Core.Models;
using DBox_CS.Core.Configuration;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.Globalization;

namespace DBox_CS.WebAPI.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class DboxCSIController : ControllerBase
    {
        private readonly Main _objMain; // Variable for Main class
        private readonly ILoggingService _logger;
        public DboxCSIController(ILoggingService logger, IAppSettings appsettings)
        {
            _objMain = new Main(Main.ProcessIntitator.WebAPI, appsettings);
            _logger = logger;
        }




        [HttpPost("SaveEmployee")]
        public IActionResult SaveEmployee([FromBody] EmployeeModel empmodel)
        {
            if (string.IsNullOrWhiteSpace(empmodel?.EmployeeID))
            {
                var error = new Response
                {
                    Status = 0,
                    Message = "employee_id is required",
                    Data = new ResponseData { ErrorData = { "employee_id cannot be empty" } }
                };
                return BadRequest(error);
            }



            try
            {
                string responseMsg = "Employee Saved Successfully.";
                ResponseData rd = new ResponseData();
                int sts = 1;
                int dboxiProcessId = 0;

                _objMain.SaveEmployee(empmodel, out dboxiProcessId);

                if (dboxiProcessId != 0)
                {
                    DataTable dtlog = _objMain.GetProcessLog(dboxiProcessId);
                    DataTable dtlogdet = _objMain.GetProcessLogDetails(dboxiProcessId);
                    DataTable dterrors = _objMain.GetProcessError(dboxiProcessId);

                    if (dtlog.Rows.Count > 0)
                    {
                        responseMsg = dtlog.Rows[0]["Remarks"].ToString();
                    }
                    if (dtlogdet.Rows.Count > 0)
                    {
                        bool empsaved = dtlogdet.AsEnumerable().Any(row => row.Field<bool>("Data_Saved") == true);
                        if (empsaved)
                            sts = 1;
                        else
                            sts = 0;
                    }

                    if (dterrors.Rows.Count > 0)
                    {
                        foreach (DataRow drow in dterrors.Rows)
                        {
                            rd.ErrorData.Add(drow["ErrorText"].ToString());
                        }
                    }
                }


                //Verify Data.
                var success = new Response
                {
                    Status = sts,
                    Message = responseMsg,
                    Data = rd
                };

                return Ok(success);
            }
            catch (ManualException ex)
            {
                var error = new Response
                {
                    Status = 0,
                    Message = "Error saving employee",
                    Data = new ResponseData { ErrorData = { string.IsNullOrEmpty(ex.Message) ? "internal server error" : ex.Message } }
                };
                return StatusCode(500, error);
            }
            catch (Exception ex)
            {
                var error = new Response
                {
                    Status = 0,
                    Message = "Error saving employee",
                    Data = new ResponseData { ErrorData = { "unhandled error occured" } }
                };
                return StatusCode(500, error);
            }
        }


        [HttpPost("ExitEmployee")]
        public IActionResult ExitEmployee([FromBody] EmpExitInboundModel exitModel)
        {
            if (string.IsNullOrWhiteSpace(exitModel?.EmployeeID))
            {
                var error = new Response
                {
                    Status = 0,
                    Message = "Employee ID is required",
                    Data = new ResponseData { ErrorData = { "Employee ID cannot be empty" } }
                };
                return BadRequest(error);
            }
            else
            {
                bool exist = _objMain.IsExist("Employee", exitModel.EmployeeID, "EmpCode");
                if (!exist)
                {
                    var error = new Response
                    {
                        Status = 0,
                        Message = "Invalid EmployeeID",
                        Data = new ResponseData { ErrorData = { "Employee does not exist" } }
                    };
                    return BadRequest(error);
                }
            }

            if (string.IsNullOrWhiteSpace(exitModel?.EmployeeCurrentLocation))
            {
                var error = new Response
                {
                    Status = 0,
                    Message = "Employee's current location is required",
                    Data = new ResponseData { ErrorData = { "Employee's current location cannot be empty" } }
                };
                return BadRequest(error);
            }


            if (string.IsNullOrWhiteSpace(exitModel?.ReasonofCancellation))
            {
                var error = new Response
                {
                    Status = 0,
                    Message = "Reason of cancellation is required",
                    Data = new ResponseData { ErrorData = { "Reason of cancellation cannot be empty" } }
                };
                return BadRequest(error);
            }

            if (string.IsNullOrWhiteSpace(exitModel?.LastWorkingDate))
            {
                var error = new Response
                {
                    Status = 0,
                    Message = "Last working date is required",
                    Data = new ResponseData { ErrorData = { "Last working date cannot be empty" } }
                };
                return BadRequest(error);
            }

            try
            {


                _objMain.SaveEmployeeExit(exitModel);

                //Verify Data.
                var success = new Response
                {
                    Status = 1,
                    Message = "Employee exit posted successfully",
                    Data = new ResponseData()
                };

                return Ok(success);
            }
            catch (ManualException ex)
            {
                var errorMessage = !string.IsNullOrEmpty(ex.InnerErrorDetails)
                                    ? ex.InnerErrorDetails
                                    : ex.Message;

                var error = new Response
                {
                    Status = 0,
                    Message = "Error posting employee exit",
                    Data = new ResponseData { ErrorData = { errorMessage } }
                };

                return StatusCode(500, error);
            }
            catch (Exception ex)
            {
                var error = new Response
                {
                    Status = 0,
                    Message = "Error posting employee exit",
                    Data = new ResponseData { ErrorData = { "unhandled error occured" } }
                };
                return StatusCode(500, error);
            }
        }


        [HttpPost("SaveSponsorchange")]
        public IActionResult SaveSponsorchange([FromBody] SponsorchangeModel spchangemodel)
        {
            var validationResult = ValidateSponsorChange(spchangemodel);
            if (validationResult != null)
                return BadRequest(validationResult);


            try
            {
                string responseMsg = "Sponsor Change Details Saved Successfully.";
                ResponseData rd = new ResponseData();
                int sts = 1;
                int dboxiProcessId = 0;

                _objMain.SaveSponsorchange(spchangemodel, out dboxiProcessId);

                if (dboxiProcessId != 0)
                {

                    DataTable dtlog = _objMain.GetProcessLog(dboxiProcessId);
                    DataTable dtlogdet = _objMain.GetProcessLogDetails(dboxiProcessId);
                    DataTable dterrors = _objMain.GetProcessError(dboxiProcessId);

                    if (dtlog.Rows.Count > 0)
                    {
                        responseMsg = dtlog.Rows[0]["Remarks"].ToString();
                    }
                    if (dtlogdet.Rows.Count > 0)
                    {
                        bool empsaved = dtlogdet.AsEnumerable().Any(row => row.Field<bool>("Data_Saved") == true);
                        if (empsaved)
                            sts = 1;
                        else
                            sts = 0;
                    }

                    if (dterrors.Rows.Count > 0)
                    {
                        foreach (DataRow drow in dterrors.Rows)
                        {
                            rd.ErrorData.Add(drow["ErrorText"].ToString());
                        }
                    }
                }


                //Verify Data.
                var success = new Response
                {
                    Status = sts,
                    Message = responseMsg,
                    Data = rd
                };

                return Ok(success);
            }
            catch (ManualException ex)
            {
                var error = new Response
                {
                    Status = 0,
                    Message = "Error saving sponsor change details",
                    Data = new ResponseData { ErrorData = { string.IsNullOrEmpty(ex.Message) ? "internal server error" : ex.Message } }
                };
                return StatusCode(500, error);
            }
            catch (Exception ex)
            {
                var error = new Response
                {
                    Status = 0,
                    Message = "Error saving sponsor change details",
                    Data = new ResponseData { ErrorData = { "unhandled error occured" } }
                };
                return StatusCode(500, error);
            }
        }

        private Response ValidateSponsorChange(SponsorchangeModel model)
        {
            bool exist = false;


            if (string.IsNullOrWhiteSpace(model?.EmployeeID))
            {
                return new Response
                {
                    Status = 0,
                    Message = "employee_id is required",
                    Data = new ResponseData { ErrorData = { "employee_id cannot be empty" } }
                };
            }

            exist = _objMain.IsExist("Employee", model.EmployeeID, "EmpCode");
            if (!exist)
            {
                return new Response
                {
                    Status = 0,
                    Message = "Invalid EmployeeID",
                    Data = new ResponseData { ErrorData = { "Employee does not exist" } }
                };
            }


            if (string.IsNullOrWhiteSpace(model?.PrevSponsorCompany))
            {
                return new Response
                {
                    Status = 0,
                    Message = "PrevSponsorCompany is required",
                    Data = new ResponseData { ErrorData = { "PrevSponsorCompany cannot be empty" } }
                };
            }

            exist = _objMain.IsExist("Sponsor", model.PrevSponsorCompany, "NameE");
            if (!exist)
            {
                return new Response
                {
                    Status = 0,
                    Message = "Invalid PrevSponsorCompany",
                    Data = new ResponseData { ErrorData = { "PrevSponsorCompany does not exist" } }
                };
            }


            if (string.IsNullOrWhiteSpace(model?.CurrSponsorCompany))
            {
                return new Response
                {
                    Status = 0,
                    Message = "CurrSponsorCompany is required",
                    Data = new ResponseData { ErrorData = { "CurrSponsorCompany cannot be empty" } }
                };
            }

            exist = _objMain.IsExist("Sponsor", model.CurrSponsorCompany, "NameE");
            if (!exist)
            {
                return new Response
                {
                    Status = 0,
                    Message = "Invalid CurrSponsorCompany",
                    Data = new ResponseData { ErrorData = { "CurrSponsorCompany does not exist" } }
                };
            }


            if (string.IsNullOrWhiteSpace(model?.TransferDate))
            {
                return new Response
                {
                    Status = 0,
                    Message = "TransferDate is required",
                    Data = new ResponseData { ErrorData = { "TransferDate cannot be empty" } }
                };
            }

            if (!DateTime.TryParseExact(
                model.TransferDate,
                "dd/MM/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out _))
            {
                return new Response
                {
                    Status = 0,
                    Message = "Invalid TransferDate format",
                    Data = new ResponseData { ErrorData = { "Expected format: dd/MM/yyyy" } }
                };
            }

            return null;
        }
    }
}
