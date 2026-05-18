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
    public class EmployeeStagingBL
    {
        #region MappingDataRow

        public static EmployeeStaging MapModelToStagingModel(EmployeeModel empModel, int processid)
        {
            try
            {
                return new EmployeeStaging
                {
                    //ProbPeriod = Common.ParseNullableInt(row["ProbPeriod"]),
                    InsertedDate = DateTime.Now,
                    DBOXIProcessId = processid,
                    EmployeeID = empModel.EmployeeID,
                    WorkingCompany = empModel.WorkingCompany,
                    EmployeeTitle = empModel.EmployeeTitle,
                    FullNameE = empModel.FullNameE,
                    FirstName = empModel.FirstName,
                    MiddleName = empModel.MiddleName,
                    ThirdName = empModel.ThirdName,
                    FourthName = empModel.FourthName,
                    FamilyName = empModel.FamilyName,
                    MotherNameE = empModel.MotherNameE,
                    Gender = empModel.Gender,
                    Religion = empModel.Religion,
                    Faith = empModel.Faith,
                    DateOfBirth = empModel.DateOfBirth,
                    MaritalStatus = empModel.MaritalStatus,
                    CountryOfBirth = empModel.CountryOfBirth,
                    BirthPlaceE = empModel.BirthPlaceE,
                    PassportNo = empModel.PassportNo,
                    PassportCategory = empModel.PassportCategory,
                    PassportIssueDate = empModel.PassportIssueDate,
                    PassportExpiryDate = empModel.PassportExpiryDate,
                    PassportIssueCountry = empModel.PassportIssueCountry,
                    PassportIssuePlace = empModel.PassportIssuePlace,
                    PresentNationality = empModel.PresentNationality,
                    PreviousNationality = empModel.PreviousNationality,
                    MOHREVISAProfession = empModel.MOHREVISAProfession,
                    VisaQualification = empModel.VisaQualification,
                    Language1 = empModel.Language1,
                    Language2 = empModel.Language2,
                    Language3 = empModel.Language3,
                    EduCertissuedFrom = empModel.EduCertissuedFrom,
                    MOFAAttestationNo = empModel.MOFAAttestationNo,
                    MOFAAttestationLabel = empModel.MOFAAttestationLabel,
                    CertAttestationNo = empModel.CertAttestationNo,
                    UnifiedIdentityNumber = empModel.UnifiedIdentityNumber,
                    EmirateState = empModel.EmirateState,
                    Area = empModel.Area,
                    City = empModel.City,
                    Building = empModel.Building,
                    Street = empModel.Street,
                    FlatNo = empModel.FlatNo,
                    POBox = empModel.POBox,
                    OfficeTelNo = empModel.OfficeTelNo,
                    LandlineNo = empModel.LandlineNo,
                    MobileNo = empModel.MobileNo,
                    TeleNoAbroad = empModel.TeleNoAbroad,
                    PersonalEmail = empModel.PersonalEmail,
                    Address = empModel.Address,
                    AddressAbroad = empModel.AddressAbroad,
                    Email = empModel.Email,
                    Sponsor = empModel.Sponsor,
                    CandidateLocationCurrently = empModel.CandidateLocationCurrently,
                    NoticePeriod = empModel.NoticePeriod,
                    Probation = empModel.Probation,
                    WeeklyHolidays = empModel.WeeklyHolidays,
                    WorkType = empModel.WorkType,
                    Remuneration = empModel.Remuneration,
                    BasicSalary = empModel.BasicSalary,
                    HousingAmount = empModel.HousingAmount,
                    TransportingAmount = empModel.TransportingAmount,
                    FoodAllowance = empModel.FoodAllowance,
                    MobileConnectivityAllowance = empModel.MobileConnectivityAllowance,
                    CostOfLivingAllowance = empModel.CostOfLivingAllowance,
                    OtherAllowance = empModel.OtherAllowance,
                    EmployeeStatus = empModel.EmployeeStatus,
                    HRJobTitle = empModel.HRJobTitle,
                    UniversityName = empModel.UniversityName,
                    Faculty = empModel.Faculty,
                    StudyMajors = empModel.StudyMajors,
                    DegreeType = empModel.DegreeType,
                    DegreeStartDate = empModel.DegreeStartDate,
                    DegreeEndDate = empModel.DegreeEndDate,
                    GraduationYear = empModel.GraduationYear,
                    ActualYearsofDegree = empModel.ActualYearsofDegree,
                    JoiningDate = empModel.JoiningDate,

                    Doc_WBPhoto = empModel.Doc_WBPhoto,
                    Doc_Pass1 = empModel.Doc_Pass1,
                    Doc_Pass2 = empModel.Doc_Pass2,
                    Doc_ECP1 = empModel.Doc_ECP1,
                    Doc_ECP2 = empModel.Doc_ECP2,
                    Doc_CTR = empModel.Doc_CTR,
                };
            }
            catch (Exception ex)
            {
                // Log error details
                string errorMessage = $"Error occurred while mapping to EmployeeStaging. Details: {ex.Message}";
                Common.LogAction(errorMessage);  // Log the error action
                Common.LogException(ex);  // Log the exception details

                return null;  // Return null to indicate failure in mapping
            }
        }


        #endregion

        #region MappingParameters

        public static Dictionary<string, object> MapToParameters(EmployeeStaging employee)
        {
            return new Dictionary<string, object>
            {
                {"@InsertedDate", employee.InsertedDate},
                {"@DBOXIProcessId", employee.DBOXIProcessId},
                {"@EmployeeID", employee.EmployeeID},
                {"@WorkingCompany", employee.WorkingCompany},
                {"@EmployeeTitle", employee.EmployeeTitle},
                {"@FullNameE", employee.FullNameE},
                {"@FirstName", employee.FirstName},
                {"@MiddleName", employee.MiddleName},
                {"@ThirdName", employee.ThirdName},
                {"@FourthName", employee.FourthName},
                {"@FamilyName", employee.FamilyName},
                {"@MotherNameE", employee.MotherNameE},
                {"@Gender", employee.Gender},
                {"@Religion", employee.Religion},
                {"@Faith", employee.Faith},
                {"@DateOfBirth", employee.DateOfBirth},
                {"@MaritalStatus", employee.MaritalStatus},
                {"@CountryOfBirth", employee.CountryOfBirth},
                {"@BirthPlaceE", employee.BirthPlaceE},
                {"@PassportNo", employee.PassportNo},
                {"@PassportCategory", employee.PassportCategory},
                {"@PassportIssueDate", employee.PassportIssueDate},
                {"@PassportExpiryDate", employee.PassportExpiryDate},
                {"@PassportIssueCountry", employee.PassportIssueCountry},
                {"@PassportIssuePlace", employee.PassportIssuePlace},
                {"@PresentNationality", employee.PresentNationality},
                {"@PreviousNationality", employee.PreviousNationality},
                {"@MOHREVISAProfession", employee.MOHREVISAProfession},
                {"@VisaQualification", employee.VisaQualification},
                {"@Language1", employee.Language1},
                {"@Language2", employee.Language2},
                {"@Language3", employee.Language3},
                {"@EduCertissuedFrom", employee.EduCertissuedFrom},
                {"@MOFAAttestationNo", employee.MOFAAttestationNo},
                {"@MOFAAttestationLabel", employee.MOFAAttestationLabel},
                {"@CertAttestationNo", employee.CertAttestationNo},
                {"@UnifiedIdentityNumber", employee.UnifiedIdentityNumber},
                {"@EmirateState", employee.EmirateState},
                {"@Area", employee.Area},
                {"@City", employee.City},
                {"@Building", employee.Building},
                {"@Street", employee.Street},
                {"@FlatNo", employee.FlatNo},
                {"@POBox", employee.POBox},
                {"@OfficeTelNo", employee.OfficeTelNo},
                {"@LandlineNo", employee.LandlineNo},
                {"@MobileNo", employee.MobileNo},
                {"@TeleNoAbroad", employee.TeleNoAbroad},
                {"@PersonalEmail", employee.PersonalEmail},
                {"@Address", employee.Address},
                {"@AddressAbroad", employee.AddressAbroad},
                {"@Email", employee.Email},
                {"@Sponsor", employee.Sponsor},
                {"@CandidateLocationCurrently", employee.CandidateLocationCurrently},
                {"@NoticePeriod", employee.NoticePeriod},
                {"@Probation", employee.Probation},
                {"@WeeklyHolidays", employee.WeeklyHolidays},
                {"@WorkType", employee.WorkType},
                {"@Remuneration", employee.Remuneration},
                {"@BasicSalary", employee.BasicSalary},
                {"@HousingAmount", employee.HousingAmount},
                {"@TransportingAmount", employee.TransportingAmount},
                {"@FoodAllowance", employee.FoodAllowance},
                {"@MobileConnectivityAllowance", employee.MobileConnectivityAllowance},
                {"@CostOfLivingAllowance", employee.CostOfLivingAllowance},
                {"@OtherAllowance", employee.OtherAllowance},
                {"@EmployeeStatus", employee.EmployeeStatus},
                {"@HRJobTitle", employee.HRJobTitle},
                {"@UniversityName", employee.UniversityName},
                {"@Faculty", employee.Faculty},
                {"@StudyMajors", employee.StudyMajors},
                {"@DegreeType", employee.DegreeType},
                {"@DegreeStartDate", employee.DegreeStartDate},
                {"@DegreeEndDate", employee.DegreeEndDate},
                {"@GraduationYear", employee.GraduationYear},
                {"@ActualYearsofDegree", employee.ActualYearsofDegree},
                {"@JoiningDate", employee.JoiningDate},

                {"@Doc_WBPhoto", employee.Doc_WBPhoto},
                {"@Doc_Pass1", employee.Doc_Pass1},
                {"@Doc_Pass2", employee.Doc_Pass2},
                {"@Doc_ECP1", employee.Doc_ECP1},
                {"@Doc_ECP2", employee.Doc_ECP2},
                {"@Doc_CTR", employee.Doc_CTR},


            };
        }



        #endregion


        public void SaveModelToStaging(EmployeeModel empModel, int processid)
        {
            EmployeeStaging empstgmodel = EmployeeStagingBL.MapModelToStagingModel(empModel, processid);

            string sQry = @"
                Declare @rowno int=(Select count(1) from DBOXI_EmployeeInitialStaging where  DBOXIProcessId=@DBOXIProcessId)+1;

                INSERT INTO [dbo].[DBOXI_EmployeeInitialStaging]
               ([DBOXIProcessId],[InsertedDate],[RowNo],[EmployeeID],[WorkingCompany],[EmployeeTitle],[FullNameE],[FirstName],[MiddleName]
		       ,[ThirdName],[FourthName],[FamilyName],[MotherNameE],[Gender],[Religion],[Faith],[DateOfBirth],[MaritalStatus],[CountryOfBirth],[BirthPlaceE]
		       ,[PassportNo],[PassportCategory],[PassportIssueDate],[PassportExpiryDate],[PassportIssueCountry],[PassportIssuePlace],[PresentNationality]
		       ,[PreviousNationality],[MOHREVISAProfession],[VisaQualification],[Language1],[Language2],[Language3],[EduCertissuedFrom],[MOFAAttestationNo]
		       ,[MOFAAttestationLabel],[CertAttestationNo],[UnifiedIdentityNumber],[EmirateState],[Area],[City],[Building],[Street],[FlatNo],[POBox],[OfficeTelNo]
		       ,[LandlineNo],[MobileNo],[TeleNoAbroad],[PersonalEmail],[Address],[AddressAbroad],[Email],[Sponsor],[CandidateLocationCurrently],[NoticePeriod]
		       ,[Probation],[WeeklyHolidays],[WorkType],[Remuneration],[BasicSalary],[HousingAmount],[TransportingAmount],[FoodAllowance],[MobileConnectivityAllowance]
		       ,[CostOfLivingAllowance],[OtherAllowance],[EmployeeStatus],[HRJobTitle],[UniversityName],[Faculty],[StudyMajors],[DegreeType],[DegreeStartDate]
		       ,[DegreeEndDate],[GraduationYear],[ActualYearsofDegree],[JoiningDate],[Doc_WBPhoto],[Doc_Pass1],[Doc_Pass2],[Doc_ECP1],[Doc_ECP2],[Doc_CTR]) 
                VALUES
               (@DBOXIProcessId,@InsertedDate,@rowno,@EmployeeID,@WorkingCompany,@EmployeeTitle,@FullNameE,@FirstName,@MiddleName
		       ,@ThirdName,@FourthName,@FamilyName,@MotherNameE,@Gender,@Religion,@Faith,@DateOfBirth,@MaritalStatus,@CountryOfBirth,@BirthPlaceE
		       ,@PassportNo,@PassportCategory,@PassportIssueDate,@PassportExpiryDate,@PassportIssueCountry,@PassportIssuePlace,@PresentNationality
		       ,@PreviousNationality,@MOHREVISAProfession,@VisaQualification,@Language1,@Language2,@Language3,@EduCertissuedFrom,@MOFAAttestationNo
		       ,@MOFAAttestationLabel,@CertAttestationNo,@UnifiedIdentityNumber,@EmirateState,@Area,@City,@Building,@Street,@FlatNo,@POBox,@OfficeTelNo
		       ,@LandlineNo,@MobileNo,@TeleNoAbroad,@PersonalEmail,@Address,@AddressAbroad,@Email,@Sponsor,@CandidateLocationCurrently,@NoticePeriod
		       ,@Probation,@WeeklyHolidays,@WorkType,@Remuneration,@BasicSalary,@HousingAmount,@TransportingAmount,@FoodAllowance,@MobileConnectivityAllowance
		       ,@CostOfLivingAllowance,@OtherAllowance,@EmployeeStatus,@HRJobTitle,@UniversityName,@Faculty,@StudyMajors,@DegreeType,@DegreeStartDate
		       ,@DegreeEndDate,@GraduationYear,@ActualYearsofDegree,@JoiningDate,@Doc_WBPhoto,@Doc_Pass1,@Doc_Pass2,@Doc_ECP1,@Doc_ECP2,@Doc_CTR);";


            Dictionary<string, object> parameters = EmployeeStagingBL.MapToParameters(empstgmodel);


            string errMsg = string.Empty;
            if (!ConnectionFunctions.ExecuteQuery(sQry, parameters, ref errMsg))
            {
                string errorMsg = $"Error inserting row FOR PROCESSID " + processid.ToString() + " : {errMsg}";
                //Common.LogErrorToSFIErrorLog(fileName, rowIndex, employee.EmpCode, errorMsg, "Employee");
                Common.LogAction(errorMsg);

                throw new ManualException("", errorMsg);
            }
        }


        public void MoveDataToEmpStagingClosed(int processid)
        {

            string moveDataQuery = @"

            DELETE FROM DBOXI_EmployeeStagingClosed  where DBOXIProcessId=@DBOXIProcessId



            INSERT INTO [dbo].[DBOXI_EmployeeStagingClosed]
                       ([Id],[DBOXIProcessId],[InsertedDate],[RowNo],[EmployeeID],[WorkingCompany],[EmployeeTitle],[FullNameE],[FirstName],[MiddleName]
		               ,[ThirdName],[FourthName],[FamilyName],[MotherNameE],[Gender],[Religion],[Faith],[DateOfBirth],[MaritalStatus],[CountryOfBirth],[BirthPlaceE]
		               ,[PassportNo],[PassportCategory],[PassportIssueDate],[PassportExpiryDate],[PassportIssueCountry],[PassportIssuePlace],[PresentNationality]
		               ,[PreviousNationality],[MOHREVISAProfession],[VisaQualification],[Language1],[Language2],[Language3],[EduCertissuedFrom],[MOFAAttestationNo]
		               ,[MOFAAttestationLabel],[CertAttestationNo],[UnifiedIdentityNumber],[EmirateState],[Area],[City],[Building],[Street],[FlatNo],[POBox],[OfficeTelNo]
		               ,[LandlineNo],[MobileNo],[TeleNoAbroad],[PersonalEmail],[Address],[AddressAbroad],[Email],[Sponsor],[CandidateLocationCurrently],[NoticePeriod]
		               ,[Probation],[WeeklyHolidays],[WorkType],[Remuneration],[BasicSalary],[HousingAmount],[TransportingAmount],[FoodAllowance],[MobileConnectivityAllowance]
		               ,[CostOfLivingAllowance],[OtherAllowance],[EmployeeStatus],[HRJobTitle],[UniversityName],[Faculty],[StudyMajors],[DegreeType],[DegreeStartDate]
		               ,[DegreeEndDate],[GraduationYear],[ActualYearsofDegree],[JoiningDate],[Doc_WBPhoto],[Doc_Pass1],[Doc_Pass2],[Doc_ECP1],[Doc_ECP2],[Doc_CTR]) 
                Select [Id],[DBOXIProcessId],[InsertedDate],[RowNo],[EmployeeID],[WorkingCompany],[EmployeeTitle],[FullNameE],[FirstName],[MiddleName]
		               ,[ThirdName],[FourthName],[FamilyName],[MotherNameE],[Gender],[Religion],[Faith],[DateOfBirth],[MaritalStatus],[CountryOfBirth],[BirthPlaceE]
		               ,[PassportNo],[PassportCategory],[PassportIssueDate],[PassportExpiryDate],[PassportIssueCountry],[PassportIssuePlace],[PresentNationality]
		               ,[PreviousNationality],[MOHREVISAProfession],[VisaQualification],[Language1],[Language2],[Language3],[EduCertissuedFrom],[MOFAAttestationNo]
		               ,[MOFAAttestationLabel],[CertAttestationNo],[UnifiedIdentityNumber],[EmirateState],[Area],[City],[Building],[Street],[FlatNo],[POBox],[OfficeTelNo]
		               ,[LandlineNo],[MobileNo],[TeleNoAbroad],[PersonalEmail],[Address],[AddressAbroad],[Email],[Sponsor],[CandidateLocationCurrently],[NoticePeriod]
		               ,[Probation],[WeeklyHolidays],[WorkType],[Remuneration],[BasicSalary],[HousingAmount],[TransportingAmount],[FoodAllowance],[MobileConnectivityAllowance]
		               ,[CostOfLivingAllowance],[OtherAllowance],[EmployeeStatus],[HRJobTitle],[UniversityName],[Faculty],[StudyMajors],[DegreeType],[DegreeStartDate]
		               ,[DegreeEndDate],[GraduationYear],[ActualYearsofDegree],[JoiningDate],[Doc_WBPhoto],[Doc_Pass1],[Doc_Pass2],[Doc_ECP1],[Doc_ECP2],[Doc_CTR]
                FROM DBOXI_EmployeeInitialStaging where DBOXIProcessId=@DBOXIProcessId;


                DELETE FROM DBOXI_EmployeeInitialStaging  where DBOXIProcessId=@DBOXIProcessId;

                ";


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
