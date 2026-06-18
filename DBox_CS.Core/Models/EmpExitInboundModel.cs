using DBox_CS.Core.AppClass;
using DBox_CS.Core.DALayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DBox_CS.Core.Models
{
    public class EmpExitInboundModel
    {
        [DataMember(Name = "EmployeeID")]
        public string EmployeeID { get; set; }

        [DataMember(Name = "LastWorkingDate")]
        public string LastWorkingDate { get; set; }

        [DataMember(Name = "EmployeeCurrentLocation")]
        public string EmployeeCurrentLocation { get; set; }

        [DataMember(Name = "ReasonofCancellation")]
        public string ReasonofCancellation
        { get; set; }

        [DataMember(Name = "EmployeeHasFamilySponsored")]
        public string EmployeeHasFamilySponsored { get; set; }

        [DataMember(Name = "coloured_passport_copy_page_1")]
        public string coloured_passport_copy_page_1 { get; set; }

        [DataMember(Name = "coloured_passport_copy_page_2")]
        public string coloured_passport_copy_page_2 { get; set; }
    }

    public class ExitMethods
    {

        public void PostEmployeeExit(EmpExitInboundModel exitObj, bool Issponsorchange=false,string wbPhoto="",string ResCopy="")
        {
            int processId = 0, processdetailsId = 0;
            try
            {
                Common.LogAction($"Dbox Exit Integration PostEmployeeExit started.");
                string errMsg = "";

                string reason = exitObj.ReasonofCancellation;
             

                if (reason.Equals("Involuntary", StringComparison.OrdinalIgnoreCase))
                {
                    exitObj.ReasonofCancellation = "termination";
                   
                }
                else if (reason.Equals("Voluntary", StringComparison.OrdinalIgnoreCase) ||
                         reason.Equals("Retired", StringComparison.OrdinalIgnoreCase) ||
                         reason.Equals("Death", StringComparison.OrdinalIgnoreCase))
                {
                    exitObj.ReasonofCancellation = "Resignation";
                  
                }
                

                string qry = @"INSERT INTO DBOXIProcessLog (ProcessName, StartTime, EndTime, HasErrors, Remarks) VALUES ('Exit Employee from DBox', GETDATE(), NULL, 0, 'Exit Process Started');SELECT SCOPE_IDENTITY();";
                bool procres = ConnectionFunctions.Connect_SQLInsertWithID(ref processId, qry, ref errMsg);

                qry = @"INSERT INTO DBOXIImportProcessLogDetails (DBOXIProcessId, RowNo, EmpCode, HasError, Data_Saved,ImportDataType, HasBRError, LoggedDate, Remarks) 
                    VALUES (" + processId + ", 1, '" + exitObj.EmployeeID + "', 0, 1,'EmployeeExit', 0, GETDATE(), Employee Exit Processed successfully)";
                procres = ConnectionFunctions.Connect_SQLInsertWithID(ref processdetailsId, qry, ref errMsg);
                //InsertImportLogDetails(    processId: processId,    rowNo: 1,    empCode: exitObj.EmployeeID,    hasError: false,    dataSaved: true,    importDataType: "EmployeeExit",    hasBRError: false,    remarks: "Employee Exit Processed successfully");

                //Insert staging entry for logging
                int rowNo = 1;

                InsertEmpExitStaging(exitObj, processId, rowNo);

                string EmpNameE = "", EmpNameA = "", PassportNoE = "", PassportNoA = "", NPresent = "", PExpiryDate = "", LabCardNo = "", EmpId = "";
                string field15061 = "", field2 = "", field15008 = "", field15010 = "", field15012 = "";
                int result = 0, count = 0;
                string empCode = exitObj.EmployeeID;
                //int newReqID = 0, ID44 = 0, ID47 = 0, ID112 = 0, ID113 = 0, ID114 = 0, IDEosType = 0, IDYesNo = 0, IDOption27 = 0, IDOption28 = 0, IDOption31 = 0, IDOption32 = 0 ;
                string empDetailsQry = "select EmpId, EmpNameE,EmpNameA, PassportNoE, PassportNoA, NPresent,Convert(varchar,PExpiryDate,103)PExpiryDate, LabCardNo from employee where EmpCode = '" + empCode + "'";
                DataTable dt = new DataTable();
                bool res = ConnectionFunctions.Connect_SQLDataTable(ref dt, empDetailsQry, ref errMsg);
                if (dt.Rows.Count > 0)
                {
                    EmpId = dt.Rows[0]["EmpId"].ToString();
                    bool isRecordExist = CheckRequestExists(EmpId, out count, ref errMsg);
                    if (isRecordExist)
                    {
                        if (Issponsorchange == false)
                        throw new ManualException("There is already an Exit request in progress");
                        else
                            throw new ManualException("There is already a Service request for Sponsor change in progress");
                    }

                    EmpNameE = dt.Rows[0]["EmpNameE"].ToString();
                    EmpNameA = dt.Rows[0]["EmpNameA"].ToString();
                    PassportNoE = dt.Rows[0]["PassportNoE"].ToString();
                    PassportNoA = dt.Rows[0]["PassportNoA"].ToString();
                    NPresent = dt.Rows[0]["NPresent"].ToString();
                    PExpiryDate = dt.Rows[0]["PExpiryDate"].ToString();
                    LabCardNo = dt.Rows[0]["LabCardNo"].ToString();

                    field15061 = LabCardNo;
                    field2 = EmpNameE + "[" + EmpNameA + "]";
                    field15008 = NPresent;
                    field15010 = PassportNoE + "[" + PassportNoA + "]";
                    field15012 = PExpiryDate;

                    #region without transaction
                    //string insQry = @"INSERT INTO AuthorityServiceRequest 
                    //                (AuthorityServiceID,ServiceTypeID,RequestedFor,ActiveStatus,Status,UserEmpId,RequestDate,IsForCandidate,IsDraft,SubmissionDate,CreatedDate,IsSelfReq,ServiceCost)
                    //                SELECT AuthorityServiceID, ServiceTypeID, " + EmpId + @", ActiveStatus, Status, 0, GETDATE(), IsForCandidate, IsDraft, SubmissionDate, CreatedDate, IsSelfReq, ServiceCost 
                    //                FROM AuthorityServiceRequest_CncelTemplate;

                    //                SELECT SCOPE_IDENTITY();";


                    // res = ConnectionFunctions.Connect_SQLInsertWithID(ref newReqID, insQry, ref errMsg);

                    //insQry = "Insert Into AuthorityServiceRequestField (FieldID,FieldValue,AuthorityServiceRequestID,ViewNo,IsChangedFromMasterFile,SortOrder)" +
                    //         " Select FieldID, '" + field15061 + "', " + newReqID + ", ViewNo, IsChangedFromMasterFile, SortOrder from AuthorityServiceRequestField_CncelTemplate Where FieldID = 15061; ";
                    //res = ConnectionFunctions.Connect_SQLNonQuery(ref result, insQry, ref errMsg);

                    //insQry = "Insert Into AuthorityServiceRequestField (FieldID,FieldValue,AuthorityServiceRequestID,ViewNo,IsChangedFromMasterFile,SortOrder)" +
                    //         " Select FieldID, '" + field2 + "', " + newReqID + ", ViewNo, IsChangedFromMasterFile, SortOrder from AuthorityServiceRequestField_CncelTemplate Where FieldID = 2; ";
                    //res = ConnectionFunctions.Connect_SQLNonQuery(ref result, insQry, ref errMsg);

                    //insQry = "Insert Into AuthorityServiceRequestField (FieldID,FieldValue,AuthorityServiceRequestID,ViewNo,IsChangedFromMasterFile,SortOrder)" +
                    //         " Select FieldID, '" + field15008 + "', " + newReqID + ", ViewNo, IsChangedFromMasterFile, SortOrder from AuthorityServiceRequestField_CncelTemplate Where FieldID = 15008; ";
                    //res = ConnectionFunctions.Connect_SQLNonQuery(ref result, insQry, ref errMsg);

                    //insQry = "Insert Into AuthorityServiceRequestField (FieldID,FieldValue,AuthorityServiceRequestID,ViewNo,IsChangedFromMasterFile,SortOrder)" +
                    //         " Select FieldID, '" + field15010 + "', " + newReqID + ", ViewNo, IsChangedFromMasterFile, SortOrder from AuthorityServiceRequestField_CncelTemplate Where FieldID = 15010; ";
                    //res = ConnectionFunctions.Connect_SQLNonQuery(ref result, insQry, ref errMsg);

                    //insQry = "Insert Into AuthorityServiceRequestField (FieldID,FieldValue,AuthorityServiceRequestID,ViewNo,IsChangedFromMasterFile,SortOrder)" +
                    //         " Select FieldID, '" + field15012 + "', " + newReqID + ", ViewNo, IsChangedFromMasterFile, SortOrder from AuthorityServiceRequestField_CncelTemplate Where FieldID = 15012; ";
                    //res = ConnectionFunctions.Connect_SQLNonQuery(ref result, insQry, ref errMsg);

                    ////Audit for add record
                    //insQry = "Insert Into AuditTrail ([Table],[Transaction],TransactionNo,EmpCode,UserID,Date,Errors,Flag,WComp,MachineName)" +
                    //         " Select [Table],'Add Record From DarwinBoxIntegration Service',TransactionNo,'" + empCode + "','AUTO',GETDATE(),'This is new  request for AuthorityServiceRequest for employee  with id : " + EmpId + "',Flag,'','' from AuditTrail_CncelTemplate";
                    //res = ConnectionFunctions.Connect_SQLNonQuery(ref result, insQry, ref errMsg);

                    ////AuthorityServiceRequestQuestionAnswers
                    //insQry = "Insert Into AuthorityServiceRequestQuestionAnswers (QuestionID,QuestionAnswer,AuthorityServiceRequestID,IsOptional)" +
                    //         " Select QuestionID,QuestionAnswer," + newReqID + ",IsOptional from AuthorityServiceRequestQuestionAnswers_CncelTemplate Where QuestionId = 44; SELECT SCOPE_IDENTITY();";
                    //res = ConnectionFunctions.Connect_SQLInsertWithID(ref ID44, insQry, ref errMsg);

                    //insQry = "Insert Into AuthorityServiceRequestQuestionAnswers (QuestionID,QuestionAnswer,AuthorityServiceRequestID,IsOptional)" +
                    //         " Select QuestionID,QuestionAnswer," + newReqID + ",IsOptional from AuthorityServiceRequestQuestionAnswers_CncelTemplate Where QuestionId = 47; SELECT SCOPE_IDENTITY();";
                    //res = ConnectionFunctions.Connect_SQLInsertWithID(ref ID47, insQry, ref errMsg);

                    //insQry = "Insert Into AuthorityServiceRequestQuestionAnswers (QuestionID,QuestionAnswer,AuthorityServiceRequestID,IsOptional)" +
                    //         " Select QuestionID,'" + (exitObj.EmployeeCurrentLocation.ToLower().Contains("inside") ? "31" : "32") + "'," + newReqID + ",IsOptional from AuthorityServiceRequestQuestionAnswers_CncelTemplate Where QuestionId = 112; SELECT SCOPE_IDENTITY();";
                    //res = ConnectionFunctions.Connect_SQLInsertWithID(ref ID112, insQry, ref errMsg);

                    //insQry = "Insert Into AuthorityServiceRequestQuestionAnswers (QuestionID,QuestionAnswer,AuthorityServiceRequestID,IsOptional)" +
                    //         " Select QuestionID,QuestionAnswer," + newReqID + ",IsOptional from AuthorityServiceRequestQuestionAnswers_CncelTemplate Where QuestionId = 113; SELECT SCOPE_IDENTITY();";
                    //res = ConnectionFunctions.Connect_SQLInsertWithID(ref ID113, insQry, ref errMsg);

                    //insQry = "Insert Into AuthorityServiceRequestQuestionAnswers (QuestionID,QuestionAnswer,AuthorityServiceRequestID,IsOptional)" +
                    //         " Select QuestionID,QuestionAnswer," + newReqID + ",IsOptional from AuthorityServiceRequestQuestionAnswers_CncelTemplate Where QuestionId = 114; SELECT SCOPE_IDENTITY();";
                    //res = ConnectionFunctions.Connect_SQLInsertWithID(ref ID114, insQry, ref errMsg);


                    ////AuthorityServiceRequestQuestionAnswersTr
                    //insQry = "Insert Into AuthorityServiceRequestQuestionAnswersTr (AuthSerReqQuestionAnswersId,LangCode,QuestionBody)" +
                    //         " Select " + ID44 + ",LangCode,QuestionBody from AuthorityServiceRequestQuestionAnswersTr_CncelTemplate Where QuestionBody = 'Processing System'; ";
                    //res = ConnectionFunctions.Connect_SQLNonQuery(ref result, insQry, ref errMsg);

                    //insQry = "Insert Into AuthorityServiceRequestQuestionAnswersTr (AuthSerReqQuestionAnswersId,LangCode,QuestionBody)" +
                    //         " Select " + ID47 + ",LangCode,QuestionBody from AuthorityServiceRequestQuestionAnswersTr_CncelTemplate Where QuestionBody = 'Cancel Reason'; ";
                    //res = ConnectionFunctions.Connect_SQLNonQuery(ref result, insQry, ref errMsg);

                    //insQry = "Insert Into AuthorityServiceRequestQuestionAnswersTr (AuthSerReqQuestionAnswersId,LangCode,QuestionBody)" +
                    //         " Select " + ID112 + ",LangCode,QuestionBody from AuthorityServiceRequestQuestionAnswersTr_CncelTemplate Where QuestionBody = 'Employee Location'; ";
                    //res = ConnectionFunctions.Connect_SQLNonQuery(ref result, insQry, ref errMsg);

                    //insQry = "Insert Into AuthorityServiceRequestQuestionAnswersTr (AuthSerReqQuestionAnswersId,LangCode,QuestionBody)" +
                    //         " Select " + ID113 + ",LangCode,QuestionBody from AuthorityServiceRequestQuestionAnswersTr_CncelTemplate Where QuestionBody = 'Last Working Date'; ";
                    //res = ConnectionFunctions.Connect_SQLNonQuery(ref result, insQry, ref errMsg);

                    //insQry = "Insert Into AuthorityServiceRequestQuestionAnswersTr (AuthSerReqQuestionAnswersId,LangCode,QuestionBody)" +
                    //         " Select " + ID114 + ",LangCode,QuestionBody from AuthorityServiceRequestQuestionAnswersTr_CncelTemplate Where QuestionBody = 'Employee Has Family Sponsored'; ";
                    //res = ConnectionFunctions.Connect_SQLNonQuery(ref result, insQry, ref errMsg);


                    ////AuthorityServiceRequestQuestionDate
                    //insQry = "Insert Into AuthorityServiceRequestQuestionDate (AuthorityServiceRequestAnswerId,IsDate,IsDateTime,PreventSelectionOlderThan,PreventSelectionOlderValue,PreventSelectionOlderValueType," +
                    //         " PreventSelectionGreaterThan,PreventSelectionGreaterValue,PreventSelectionGreaterValueType,QuestionAnswer)" +
                    //         " Select " + ID113 + ",IsDate,IsDateTime,PreventSelectionOlderThan,PreventSelectionOlderValue,PreventSelectionOlderValueType,PreventSelectionGreaterThan,PreventSelectionGreaterValue,PreventSelectionGreaterValueType,'" + Convert.ToDateTime(exitObj.LastWorkingDate).ToString("yyyy-MM-dd") + "'" +
                    //         " from AuthorityServiceRequestQuestionDate_CncelTemplate; ";
                    //res = ConnectionFunctions.Connect_SQLNonQuery(ref result, insQry, ref errMsg);

                    ////AuthorityServiceRequestQuestionLibrary
                    //insQry = "Insert Into AuthorityServiceRequestQuestionLibrary (AuthorityServiceRequestAnswerId,IsSingleSelection,LibraryTypeId)" +
                    //         " Select " + ID47 + ",IsSingleSelection,LibraryTypeId from AuthorityServiceRequestQuestionLibrary_CncelTemplate where LibraryTypeId = 'EosType'; SELECT SCOPE_IDENTITY();";
                    //res = ConnectionFunctions.Connect_SQLInsertWithID(ref IDEosType, insQry, ref errMsg);

                    //insQry = "Insert Into AuthorityServiceRequestQuestionLibrary (AuthorityServiceRequestAnswerId,IsSingleSelection,LibraryTypeId)" +
                    //         " Select " + ID114 + ",IsSingleSelection,LibraryTypeId from AuthorityServiceRequestQuestionLibrary_CncelTemplate where LibraryTypeId = 'YESNO'; SELECT SCOPE_IDENTITY();";
                    //res = ConnectionFunctions.Connect_SQLInsertWithID(ref IDYesNo, insQry, ref errMsg);

                    ////AuthorityServiceRequestQuestionLibraryAnswer
                    //insQry = "Insert Into AuthorityServiceRequestQuestionLibraryAnswer (AuthorityServiceRequestLibraryId,LibraryAnswerId,DescEn,DescAr,SortOrder)" +
                    //         " Select top 1 " + IDEosType + ",'" + (exitObj.ReasonofCancellation.ToLower().Contains("termination") ? 1 : 0) + "','" + exitObj.ReasonofCancellation + "',DescAr,SortOrder from AuthorityServiceRequestQuestionLibraryAnswer_CncelTemplate; ";
                    //res = ConnectionFunctions.Connect_SQLNonQuery(ref result, insQry, ref errMsg);

                    //insQry = "Insert Into AuthorityServiceRequestQuestionLibraryAnswer (AuthorityServiceRequestLibraryId,LibraryAnswerId,DescEn,DescAr,SortOrder)" +
                    //         " Select " + IDYesNo + ",'" + (exitObj.EmployeeHasFamilySponsored.ToLower().Contains("yes") ? 1 : 0) + "','" + exitObj.EmployeeHasFamilySponsored + "',DescAr,SortOrder from AuthorityServiceRequestQuestionLibraryAnswer_CncelTemplate; ";
                    //res = ConnectionFunctions.Connect_SQLNonQuery(ref result, insQry, ref errMsg);


                    ////AuthorityServiceRequestQuestionOption
                    //insQry = "Insert Into AuthorityServiceRequestQuestionOption (AuthorityServiceRequestQuestionAnswerId,QuestionOptionId,IsSelected)" +
                    //         " Select " + ID44 + ",QuestionOptionId,IsSelected from AuthorityServiceRequestQuestionOption_CncelTemplate where QuestionOptionId in (27); SELECT SCOPE_IDENTITY();";
                    //res = ConnectionFunctions.Connect_SQLInsertWithID(ref IDOption27, insQry, ref errMsg);

                    //insQry = "Insert Into AuthorityServiceRequestQuestionOption (AuthorityServiceRequestQuestionAnswerId,QuestionOptionId,IsSelected)" +
                    //         " Select " + ID44 + ",QuestionOptionId,IsSelected from AuthorityServiceRequestQuestionOption_CncelTemplate where QuestionOptionId in (28); SELECT SCOPE_IDENTITY();";
                    //res = ConnectionFunctions.Connect_SQLInsertWithID(ref IDOption28, insQry, ref errMsg);


                    //insQry = "Insert Into AuthorityServiceRequestQuestionOption (AuthorityServiceRequestQuestionAnswerId,QuestionOptionId,IsSelected)" +
                    //         " Select " + ID112 + ",QuestionOptionId," + (exitObj.EmployeeCurrentLocation.ToLower().Contains("inside") ? "1" : "0") + " from AuthorityServiceRequestQuestionOption_CncelTemplate where QuestionOptionId in (31); SELECT SCOPE_IDENTITY();";
                    //res = ConnectionFunctions.Connect_SQLInsertWithID(ref IDOption31, insQry, ref errMsg);

                    //insQry = "Insert Into AuthorityServiceRequestQuestionOption (AuthorityServiceRequestQuestionAnswerId,QuestionOptionId,IsSelected)" +
                    //         " Select " + ID112 + ",QuestionOptionId," + (exitObj.EmployeeCurrentLocation.ToLower().Contains("outside") ? "1" : "0") + " from AuthorityServiceRequestQuestionOption_CncelTemplate where QuestionOptionId in (32); SELECT SCOPE_IDENTITY();";
                    //res = ConnectionFunctions.Connect_SQLInsertWithID(ref IDOption32, insQry, ref errMsg);


                    ////AuthorityServiceRequestQuestionOptionTr
                    //insQry = "Insert Into AuthorityServiceRequestQuestionOptionTr (OptionId,LangCode,OptionDescription,LibraryOptionDescriptionId)" +
                    //         " Select " + IDOption27 + ",LangCode,OptionDescription,LibraryOptionDescriptionId from AuthorityServiceRequestQuestionOptionTr_CncelTemplate where OptionDescription = 'Bundle'; ";
                    //res = ConnectionFunctions.Connect_SQLNonQuery(ref result, insQry, ref errMsg);

                    //insQry = "Insert Into AuthorityServiceRequestQuestionOptionTr (OptionId,LangCode,OptionDescription,LibraryOptionDescriptionId)" +
                    //         " Select " + IDOption28 + ",LangCode,OptionDescription,LibraryOptionDescriptionId from AuthorityServiceRequestQuestionOptionTr_CncelTemplate where OptionDescription = 'Normal'; ";
                    //res = ConnectionFunctions.Connect_SQLNonQuery(ref result, insQry, ref errMsg);

                    //insQry = "Insert Into AuthorityServiceRequestQuestionOptionTr (OptionId,LangCode,OptionDescription,LibraryOptionDescriptionId)" +
                    //         " Select " + IDOption31 + ",LangCode,OptionDescription,LibraryOptionDescriptionId from AuthorityServiceRequestQuestionOptionTr_CncelTemplate where OptionDescription = 'Inside UAE'; ";
                    //res = ConnectionFunctions.Connect_SQLNonQuery(ref result, insQry, ref errMsg);

                    //insQry = "Insert Into AuthorityServiceRequestQuestionOptionTr (OptionId,LangCode,OptionDescription,LibraryOptionDescriptionId)" +
                    //         " Select " + IDOption32 + ",LangCode,OptionDescription,LibraryOptionDescriptionId from AuthorityServiceRequestQuestionOptionTr_CncelTemplate where OptionDescription = 'Outside UAE'; ";
                    //res = ConnectionFunctions.Connect_SQLNonQuery(ref result, insQry, ref errMsg);


                    ////Audit for edit record
                    //insQry = "Insert Into AuditTrail ([Table],[Transaction],TransactionNo,EmpCode,UserID,Date,Errors,Flag,WComp,MachineName)" +
                    //         " Select [Table],'Edit Record From DarwinBoxIntegration Service'," + newReqID + ",'" + empCode + "','AUTO',GETDATE(),'This is update for AuthorityServiceRequest for employee  with id : " + EmpId + "',Flag,'','' from AuditTrail_CncelTemplate";
                    //res = ConnectionFunctions.Connect_SQLNonQuery(ref result, insQry, ref errMsg);
                    #endregion

                    using (SqlConnection conn = new SqlConnection(ConnectionFunctions.GetConnectionString()))
                    {
                        conn.Open();
                        SqlTransaction tran = conn.BeginTransaction();

                        try
                        {
                            SqlCommand cmd = new SqlCommand();
                            cmd.Connection = conn;
                            cmd.Transaction = tran;

                            int newReqID = 0, ID44 = 0, ID47 = 0, ID112 = 0, ID113 = 0, ID114 = 0;
                            int IDEosType = 0, IDYesNo = 0, IDOption27 = 0, IDOption28 = 0, IDOption31 = 0, IDOption32 = 0;

                            // ✅ 1. Insert AuthorityServiceRequest
                            cmd.CommandText = @"
                            INSERT INTO AuthorityServiceRequest 
                            (AuthorityServiceID,ServiceTypeID,RequestedFor,ActiveStatus,Status,UserEmpId,RequestDate,IsForCandidate,IsDraft,SubmissionDate,CreatedDate,IsSelfReq,ServiceCost)
                            SELECT AuthorityServiceID, ServiceTypeID, @EmpId, ActiveStatus, Status, 0, GETDATE(), IsForCandidate, IsDraft, SubmissionDate, CreatedDate, IsSelfReq, ServiceCost 
                            FROM AuthorityServiceRequest_CncelTemplate;
                            SELECT SCOPE_IDENTITY();";

                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@EmpId", EmpId);

                            newReqID = Convert.ToInt32(cmd.ExecuteScalar());

                            // ✅ 2. Insert Fields
                            void InsertField(int fieldId, string value)
                            {
                                cmd.CommandText = @"
                                INSERT INTO AuthorityServiceRequestField 
                                (FieldID,FieldValue,AuthorityServiceRequestID,ViewNo,IsChangedFromMasterFile,SortOrder)
                                SELECT FieldID, @Value, @ReqID, ViewNo, IsChangedFromMasterFile, SortOrder 
                                FROM AuthorityServiceRequestField_CncelTemplate 
                                WHERE FieldID = @FieldID;";

                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("@Value", value ?? (object)DBNull.Value);
                                cmd.Parameters.AddWithValue("@ReqID", newReqID);
                                cmd.Parameters.AddWithValue("@FieldID", fieldId);

                                cmd.ExecuteNonQuery();
                            }

                            InsertField(15061, field15061);
                            InsertField(2, field2);
                            InsertField(15008, field15008);
                            InsertField(15010, field15010);
                            InsertField(15012, field15012);

                            // ✅ 3. Audit Insert
                            cmd.CommandText = @"
                            INSERT INTO AuditTrail ([Table],[Transaction],TransactionNo,EmpCode,UserID,Date,Errors,Flag,WComp,MachineName)
                            SELECT [Table],'Add Record From DarwinBoxIntegration Service',TransactionNo,@EmpCode,'AUTO',GETDATE(),
                                   'New request for employee id : ' + @EmpId,Flag,'',''
                            FROM AuditTrail_CncelTemplate";

                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@EmpCode", empCode);
                            cmd.Parameters.AddWithValue("@EmpId", EmpId);

                            cmd.ExecuteNonQuery();

                            // =========================
                            // QUESTION ANSWERS
                            // =========================
                            //int ID44, ID47, ID112, ID113, ID114;

                            cmd.CommandText = "Insert Into AuthorityServiceRequestQuestionAnswers Select QuestionID,QuestionAnswer," + newReqID + ",IsOptional from AuthorityServiceRequestQuestionAnswers_CncelTemplate Where QuestionId=44; SELECT SCOPE_IDENTITY();";
                            ID44 = Convert.ToInt32(cmd.ExecuteScalar());

                            cmd.CommandText = "Insert Into AuthorityServiceRequestQuestionAnswers Select QuestionID,QuestionAnswer," + newReqID + ",IsOptional from AuthorityServiceRequestQuestionAnswers_CncelTemplate Where QuestionId=47; SELECT SCOPE_IDENTITY();";
                            ID47 = Convert.ToInt32(cmd.ExecuteScalar());

                            cmd.CommandText = "Insert Into AuthorityServiceRequestQuestionAnswers Select QuestionID,'" + (exitObj.EmployeeCurrentLocation.ToLower().Contains("inside") ? "31" : "32") + "'," + newReqID + ",IsOptional from AuthorityServiceRequestQuestionAnswers_CncelTemplate Where QuestionId=112; SELECT SCOPE_IDENTITY();";
                            ID112 = Convert.ToInt32(cmd.ExecuteScalar());

                            cmd.CommandText = "Insert Into AuthorityServiceRequestQuestionAnswers Select QuestionID,QuestionAnswer," + newReqID + ",IsOptional from AuthorityServiceRequestQuestionAnswers_CncelTemplate Where QuestionId=113; SELECT SCOPE_IDENTITY();";
                            ID113 = Convert.ToInt32(cmd.ExecuteScalar());

                            cmd.CommandText = "Insert Into AuthorityServiceRequestQuestionAnswers Select QuestionID,QuestionAnswer," + newReqID + ",IsOptional from AuthorityServiceRequestQuestionAnswers_CncelTemplate Where QuestionId=114; SELECT SCOPE_IDENTITY();";
                            ID114 = Convert.ToInt32(cmd.ExecuteScalar());

                            // =========================
                            // QUESTION ANSWERS TR (ADDED)
                            // =========================
                            cmd.CommandText = "Insert Into AuthorityServiceRequestQuestionAnswersTr Select " + ID44 + ",LangCode,QuestionBody from AuthorityServiceRequestQuestionAnswersTr_CncelTemplate Where QuestionBody='Processing System'";
                            cmd.ExecuteNonQuery();

                            cmd.CommandText = "Insert Into AuthorityServiceRequestQuestionAnswersTr Select " + ID47 + ",LangCode,QuestionBody from AuthorityServiceRequestQuestionAnswersTr_CncelTemplate Where QuestionBody='Cancel Reason'";
                            cmd.ExecuteNonQuery();

                            cmd.CommandText = "Insert Into AuthorityServiceRequestQuestionAnswersTr Select " + ID112 + ",LangCode,QuestionBody from AuthorityServiceRequestQuestionAnswersTr_CncelTemplate Where QuestionBody='Employee Location'";
                            cmd.ExecuteNonQuery();

                            cmd.CommandText = "Insert Into AuthorityServiceRequestQuestionAnswersTr Select " + ID113 + ",LangCode,QuestionBody from AuthorityServiceRequestQuestionAnswersTr_CncelTemplate Where QuestionBody='Last Working Date'";
                            cmd.ExecuteNonQuery();

                            cmd.CommandText = "Insert Into AuthorityServiceRequestQuestionAnswersTr Select " + ID114 + ",LangCode,QuestionBody from AuthorityServiceRequestQuestionAnswersTr_CncelTemplate Where QuestionBody='Employee Has Family Sponsored'";
                            cmd.ExecuteNonQuery();

                            // ✅ 5. Question Date
                            string lastWorkingDate = DateTime.ParseExact(exitObj.LastWorkingDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");

                            cmd.CommandText = @"
                            INSERT INTO AuthorityServiceRequestQuestionDate
                            (AuthorityServiceRequestAnswerId,IsDate,IsDateTime,PreventSelectionOlderThan,
                             PreventSelectionOlderValue,PreventSelectionOlderValueType,PreventSelectionGreaterThan,
                             PreventSelectionGreaterValue,PreventSelectionGreaterValueType,QuestionAnswer)
                            SELECT @ID,IsDate,IsDateTime,PreventSelectionOlderThan,PreventSelectionOlderValue,
                                   PreventSelectionOlderValueType,PreventSelectionGreaterThan,
                                   PreventSelectionGreaterValue,PreventSelectionGreaterValueType,@Date
                            FROM AuthorityServiceRequestQuestionDate_CncelTemplate";

                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@ID", ID113);
                            cmd.Parameters.AddWithValue("@Date", lastWorkingDate);

                            cmd.ExecuteNonQuery();

                            // ✅ 6. Library Insert
                            int InsertLibrary(int ansId, string type)
                            {
                                cmd.CommandText = @"
                                INSERT INTO AuthorityServiceRequestQuestionLibrary
                                (AuthorityServiceRequestAnswerId,IsSingleSelection,LibraryTypeId)
                                SELECT @AnsID,IsSingleSelection,LibraryTypeId 
                                FROM AuthorityServiceRequestQuestionLibrary_CncelTemplate 
                                WHERE LibraryTypeId = @Type;
                                SELECT SCOPE_IDENTITY();";

                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("@AnsID", ansId);
                                cmd.Parameters.AddWithValue("@Type", type);

                                return Convert.ToInt32(cmd.ExecuteScalar());
                            }

                            IDEosType = InsertLibrary(ID47, "EosType");
                            IDYesNo = InsertLibrary(ID114, "YESNO");
                            
                            // ✅ 7. Library Answers
                            cmd.CommandText = @"
                            INSERT INTO AuthorityServiceRequestQuestionLibraryAnswer
                            (AuthorityServiceRequestLibraryId,LibraryAnswerId,DescEn,DescAr,SortOrder)
                            SELECT TOP 1 @LibID,@Ans,@Desc,DescAr,SortOrder
                            FROM AuthorityServiceRequestQuestionLibraryAnswer_CncelTemplate";

                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@LibID", IDEosType);
                            cmd.Parameters.AddWithValue("@Ans", exitObj.ReasonofCancellation.ToLower().Contains("termination") ? 1 : 0);
                            cmd.Parameters.AddWithValue("@Desc", exitObj.ReasonofCancellation);

                            cmd.ExecuteNonQuery();

                            cmd.CommandText = @"
                            INSERT INTO AuthorityServiceRequestQuestionLibraryAnswer
                            (AuthorityServiceRequestLibraryId,LibraryAnswerId,DescEn,DescAr,SortOrder)
                            SELECT TOP 1 @LibID,@Ans,@Desc,DescAr,SortOrder
                            FROM AuthorityServiceRequestQuestionLibraryAnswer_CncelTemplate";

                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@LibID", IDYesNo);
                            cmd.Parameters.AddWithValue("@Ans", exitObj.EmployeeHasFamilySponsored.ToLower().Contains("yes") ? 1 : 0);
                            cmd.Parameters.AddWithValue("@Desc", exitObj.EmployeeHasFamilySponsored);

                            cmd.ExecuteNonQuery();

                            // =========================
                            // QUESTION OPTIONS (ADDED)
                            // =========================
                            //int IDOption27, IDOption28, IDOption31, IDOption32;

                            cmd.CommandText = "Insert Into AuthorityServiceRequestQuestionOption Select " + ID44 + ",QuestionOptionId,IsSelected from AuthorityServiceRequestQuestionOption_CncelTemplate where QuestionOptionId=27; SELECT SCOPE_IDENTITY();";
                            IDOption27 = Convert.ToInt32(cmd.ExecuteScalar());

                            cmd.CommandText = "Insert Into AuthorityServiceRequestQuestionOption Select " + ID44 + ",QuestionOptionId,IsSelected from AuthorityServiceRequestQuestionOption_CncelTemplate where QuestionOptionId=28; SELECT SCOPE_IDENTITY();";
                            IDOption28 = Convert.ToInt32(cmd.ExecuteScalar());

                            cmd.CommandText = "Insert Into AuthorityServiceRequestQuestionOption Select " + ID112 + ",QuestionOptionId," + (exitObj.EmployeeCurrentLocation.ToLower().Contains("inside") ? "1" : "0") + " from AuthorityServiceRequestQuestionOption_CncelTemplate where QuestionOptionId=31; SELECT SCOPE_IDENTITY();";
                            IDOption31 = Convert.ToInt32(cmd.ExecuteScalar());

                            cmd.CommandText = "Insert Into AuthorityServiceRequestQuestionOption Select " + ID112 + ",QuestionOptionId," + (exitObj.EmployeeCurrentLocation.ToLower().Contains("outside") ? "1" : "0") + " from AuthorityServiceRequestQuestionOption_CncelTemplate where QuestionOptionId=32; SELECT SCOPE_IDENTITY();";
                            IDOption32 = Convert.ToInt32(cmd.ExecuteScalar());

                            // =========================
                            // QUESTION OPTION TR (ADDED)
                            // =========================
                            cmd.CommandText = "Insert Into AuthorityServiceRequestQuestionOptionTr Select " + IDOption27 + ",LangCode,OptionDescription,LibraryOptionDescriptionId from AuthorityServiceRequestQuestionOptionTr_CncelTemplate where OptionDescription='Bundle'";
                            cmd.ExecuteNonQuery();

                            cmd.CommandText = "Insert Into AuthorityServiceRequestQuestionOptionTr Select " + IDOption28 + ",LangCode,OptionDescription,LibraryOptionDescriptionId from AuthorityServiceRequestQuestionOptionTr_CncelTemplate where OptionDescription='Normal'";
                            cmd.ExecuteNonQuery();

                            cmd.CommandText = "Insert Into AuthorityServiceRequestQuestionOptionTr Select " + IDOption31 + ",LangCode,OptionDescription,LibraryOptionDescriptionId from AuthorityServiceRequestQuestionOptionTr_CncelTemplate where OptionDescription='Inside UAE'";
                            cmd.ExecuteNonQuery();

                            cmd.CommandText = "Insert Into AuthorityServiceRequestQuestionOptionTr Select " + IDOption32 + ",LangCode,OptionDescription,LibraryOptionDescriptionId from AuthorityServiceRequestQuestionOptionTr_CncelTemplate where OptionDescription='Outside UAE'";
                            cmd.ExecuteNonQuery();

                            // ✅ 8. Save Document

                            string ppContentType1;
                            byte[] ppBytes1 = ConvertBase64ToBytes(exitObj.coloured_passport_copy_page_1, out ppContentType1);

                            string ppContentType2;
                            byte[] ppBytes2 = ConvertBase64ToBytes(exitObj.coloured_passport_copy_page_2, out ppContentType2);

                            SaveExitDocument(conn, tran, EmpId, newReqID, "29", ppBytes1, "ColorPassPortCpy1", ppContentType1);

                            SaveExitDocument(conn, tran, EmpId, newReqID, "30", ppBytes2, "ColorPassPortCpy2", ppContentType2);
                            if (Issponsorchange == true)
                            {
                                string wbContentType;
                                byte[] wbBytes = ConvertBase64ToBytes(wbPhoto, out wbContentType);

                                string resContentType;
                                byte[] resBytes = ConvertBase64ToBytes(ResCopy, out resContentType);

                                SaveExitDocument(conn, tran, EmpId, newReqID, "31", wbBytes, "WBPhoto", wbContentType);

                                SaveExitDocument(conn, tran, EmpId, newReqID, "RSV", resBytes, "Resignation", resContentType);
                               
                            }

                            // ✅ 9. Final Audit (Edit)
                            cmd.CommandText = @"
                            INSERT INTO AuditTrail ([Table],[Transaction],TransactionNo,EmpCode,UserID,Date,Errors,Flag,WComp,MachineName)
                            SELECT [Table],'Edit Record From DarwinBoxIntegration Service',@ReqID,@EmpCode,'AUTO',GETDATE(),
                                   'This is update for AuthorityServiceRequest for employee with id : ' + @EmpId,Flag,'',''
                            FROM AuditTrail_CncelTemplate";

                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@ReqID", newReqID);
                            cmd.Parameters.AddWithValue("@EmpCode", empCode);
                            cmd.Parameters.AddWithValue("@EmpId", EmpId);

                            cmd.ExecuteNonQuery();
          

                            // ✅ COMMIT
                            tran.Commit();
                            Common.LogAction($"Dbox Exit Integration PostEmployeeExit completed.");
                        }
                        catch (Exception ex)
                        {
                            Common.LogErrorToDBOXIErrorLog(Convert.ToInt32(processId), processdetailsId, exitObj.EmployeeID, ex.Message, ex.InnerException?.Message);
                            tran.Rollback();
                            Common.LogAction("Transaction Failed: " + ex.Message);
                            throw;
                        }
                    }

                    try
                    {
                        string updateQry = @"UPDATE DBOXIProcessLog
                             SET EndTime = GETDATE(),
                                 Remarks = 'Exit Process Completed'
                             WHERE DBOXIProcessId = " + processId;

                        ConnectionFunctions.Connect_SQLNonQuery(ref result, updateQry, ref errMsg);
                        string moveQry = @"INSERT INTO DBOXI_EmpExitStagingClosed
                           (DBOXIProcessId, InsertedDate, RowNo, EmployeeID, LastWorkingDate,
                            CurrentLocation, ReasonofCancellation, EmployeeHasFamilySponsored,
                            Doc_Pass1, Doc_Pass2)
                           SELECT 
                                DBOXIProcessId,
                                GETDATE(),
                                RowNo,
                                EmployeeID,
                                LastWorkingDate,
                                CurrentLocation,
                                ReasonofCancellation,
                                EmployeeHasFamilySponsored,
                                Doc_Pass1,
                                Doc_Pass2
                           FROM DBOXI_EmpExitInitialStaging
                           WHERE DBOXIProcessId = " + processId;

                        ConnectionFunctions.Connect_SQLNonQuery(ref result, moveQry, ref errMsg);


                        // ✅ 3. Delete from Staging
                        string deleteQry = @"DELETE FROM DBOXI_EmpExitInitialStaging
                             WHERE DBOXIProcessId = " + processId;

                        ConnectionFunctions.Connect_SQLNonQuery(ref result, deleteQry, ref errMsg);
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }

                }
            }
            catch (Exception ex)
            {
                Common.LogErrorToDBOXIErrorLog(Convert.ToInt32(processId), processdetailsId, exitObj.EmployeeID, ex.Message, ex.InnerException?.Message);
                Common.LogAction("Transaction Failed: " + ex.Message);
                // tran.Rollback();
                throw ;
            }
            finally
            {
                try
                {
                   
                }
                catch (Exception ex)
                {
                    // ⚠️ Avoid throwing from finally — just log
                    Common.LogAction("Finally Block Error: " + ex.Message);
                }
            }
        }

        public void InsertEmpExitStaging(EmpExitInboundModel exitObj, int processId, int rowNo)
        {
            string errMsg = "";
            int result = 0;
            Common.LogAction("InsertEmpExitStaging started ");
            string qry = @"INSERT INTO DBOXI_EmpExitInitialStaging
                   (DBOXIProcessId, RowNo, EmployeeID, LastWorkingDate, CurrentLocation,
                    ReasonofCancellation, EmployeeHasFamilySponsored, Doc_Pass1, Doc_Pass2)
                   VALUES
                   (@ProcessId, @RowNo, @EmployeeID, @LastWorkingDate, @CurrentLocation,
                    @ReasonofCancellation, @EmployeeHasFamilySponsored, @DocPass1, @DocPass2)";

            SqlParameter[] param = new SqlParameter[]
            {
                new SqlParameter("@ProcessId", processId),
                new SqlParameter("@RowNo", rowNo),
                new SqlParameter("@EmployeeID", exitObj.EmployeeID ?? (object)DBNull.Value),
                new SqlParameter("@LastWorkingDate", exitObj.LastWorkingDate ?? (object)DBNull.Value),
                new SqlParameter("@CurrentLocation", exitObj.EmployeeCurrentLocation ?? (object)DBNull.Value),
                new SqlParameter("@ReasonofCancellation", exitObj.ReasonofCancellation ?? (object)DBNull.Value),
                new SqlParameter("@EmployeeHasFamilySponsored", exitObj.EmployeeHasFamilySponsored ?? (object)DBNull.Value),
                new SqlParameter("@DocPass1", exitObj.coloured_passport_copy_page_1 ?? (object)DBNull.Value),
                new SqlParameter("@DocPass2", exitObj.coloured_passport_copy_page_2 ?? (object)DBNull.Value)
            };

            ConnectionFunctions.Connect_SQLNonQuery(ref result, qry, ref errMsg, param);
        }

        public void InsertImportLogDetails(int processId, int rowNo, string empCode, bool hasError, bool dataSaved, string importDataType, bool hasBRError, string remarks)
        {
            string errMsg = "";
            int result = 0;

            string qry = @"INSERT INTO DBOXIImportProcessLogDetails
                   (DBOXIProcessId, RowNo, EmpCode, HasError, Data_Saved,
                    ImportDataType, HasBRError, LoggedDate, Remarks)
                   VALUES
                   (@ProcessId, @RowNo, @EmpCode, @HasError, @DataSaved,
                    @ImportDataType, @HasBRError, GETDATE(), @Remarks)";

            SqlParameter[] param = new SqlParameter[]
            {
                new SqlParameter("@ProcessId", processId),
                new SqlParameter("@RowNo", rowNo),
                new SqlParameter("@EmpCode", empCode ?? (object)DBNull.Value),
                new SqlParameter("@HasError", hasError),
                new SqlParameter("@DataSaved", dataSaved),
                new SqlParameter("@ImportDataType", importDataType ?? (object)DBNull.Value),
                new SqlParameter("@HasBRError", hasBRError),
                new SqlParameter("@Remarks", remarks ?? (object)DBNull.Value)
            };

            ConnectionFunctions.Connect_SQLNonQuery(ref result, qry, ref errMsg, param);
        }

        public static bool CheckRequestExists(string requestedFor, out int count, ref string errMsg)
        {
            bool res = false;
            string result = "";
            count = 0;


            try
            {
                string query = "SELECT COUNT(1) FROM [dbo].[AuthorityServiceRequest] WHERE [RequestedFor] = " + requestedFor + " AND [Status] < 20";
                ConnectionFunctions.Connect_SQLScalar(ref result, query, ref errMsg);
                if (!string.IsNullOrEmpty(result))
                    count = Convert.ToInt32(result);
                if (count > 0)
                    res = true;

            }
            catch (Exception ex)
            {
                errMsg = "Error: " + ex.Message;
                res = true;
            }


            return res;
        }
        private byte[] ConvertBase64ToBytes(string base64, out string contentType)
        {
            contentType = "";

            if (string.IsNullOrEmpty(base64))
                return null;
            try
            {
                if (base64.Contains(","))
                {
                    var prefix = base64.Substring(0, base64.IndexOf(","));
                    base64 = base64.Substring(base64.IndexOf(",") + 1);

                    if (prefix.Contains("image/jpeg")) contentType = "image/jpeg";
                    else if (prefix.Contains("image/png")) contentType = "image/png";
                    else if (prefix.Contains("application/pdf")) contentType = "application/pdf";
                    
                }
            }
            catch (Exception ex)
            {
                Common.LogAction("Base64 Conversion Error: " + ex.Message);
                throw;
            }
            return Convert.FromBase64String(base64);
            // Remove base64 prefix if exists



        }

        void SaveExitDocument(SqlConnection conn, SqlTransaction tran,string empId, int reqId, string docCode, byte[] fileData, string fileName, string contentType)
        {
            SqlCommand cmdDoc = new SqlCommand();
            cmdDoc.Connection = conn;
            cmdDoc.Transaction = tran;

            int authDocId = 0;
            Common.LogAction("SaveExitDocument started ");
            // =========================
            // CASE 1: FILE FROM API
            // =========================
            if (fileData != null && fileData.Length > 0)
            {
                // 1️ Insert Document header
                cmdDoc.CommandText = @"
            INSERT INTO AuthorityServiceRequestDocument(AuthorityServiceRequestID, DocumentTypeCode, DocID, UploadCount, Status, IsOptional, CreateDate)VALUES (@ReqID, @DocCode, @DocID, @UploadCount, @Status, @IsOptional, GETDATE());SELECT SCOPE_IDENTITY();";

                cmdDoc.Parameters.Clear();
                cmdDoc.Parameters.AddWithValue("@ReqID", reqId);
                cmdDoc.Parameters.AddWithValue("@DocCode", docCode);
                cmdDoc.Parameters.AddWithValue("@DocID", 0);
                cmdDoc.Parameters.AddWithValue("@UploadCount", 1);
                cmdDoc.Parameters.AddWithValue("@Status", 5);
                cmdDoc.Parameters.AddWithValue("@IsOptional", docCode == "31" ? 1 : 0);

                authDocId = Convert.ToInt32(cmdDoc.ExecuteScalar());

                // 2️ Insert Attachment (IMPORTANT FIX HERE)
                cmdDoc.CommandText = @"
            INSERT INTO AuthorityServiceRequestDocumentAttatchment
            (AuthSerReqtDocID, DocID, DocumentName, ContentType, Datas)
            VALUES (@AuthDocID, @DocID, @Name, @Type, @Data)";

                cmdDoc.Parameters.Clear();
                cmdDoc.Parameters.AddWithValue("@AuthDocID", authDocId);   
                cmdDoc.Parameters.AddWithValue("@DocID", 0);
                cmdDoc.Parameters.AddWithValue("@Name", fileName);
                cmdDoc.Parameters.AddWithValue("@Type", contentType);
                cmdDoc.Parameters.AddWithValue("@Data", fileData);

                cmdDoc.ExecuteNonQuery();

                string locLib5 = null;
                string salProfile = null;

                cmdDoc.CommandText = @"SELECT LocLib5, SalProfile FROM Employee WHERE EmpID = @EmpID";
                cmdDoc.Parameters.Clear();
                cmdDoc.Parameters.AddWithValue("@EmpID", empId);

                using (SqlDataReader dr = cmdDoc.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        locLib5 = dr["LocLib5"] == DBNull.Value ? null : dr["LocLib5"].ToString();
                        salProfile = dr["SalProfile"] == DBNull.Value ? null : dr["SalProfile"].ToString();
                    }
                }
                cmdDoc.CommandText = @"UPDATE DocumentExp
                       SET ActiveStatus = 35,Status = 35,LastModDateTime = GETDATE()
                       WHERE EmpID = @EmpID AND DocType = @DocType AND ActiveStatus = 20";

                cmdDoc.Parameters.Clear();
                cmdDoc.Parameters.AddWithValue("@EmpID", empId);
                cmdDoc.Parameters.AddWithValue("@DocType", docCode);

                cmdDoc.ExecuteNonQuery();

                cmdDoc.CommandText = @"INSERT INTO DocumentExp 
            (ReqNo, DocType, EmpID, ReqDate, LastModDateTime, ActiveStatus, Status, LocLib5, SalProfile, ReqID, CreateDate, ViewNo, UploadCount) 
            VALUES (@ReqNo, @DocType, @EmpID, GETDATE(), GETDATE(), @ActiveStatus, @Status, @LocLib5, @SalProfile,
            @ReqID, GETDATE(), @ViewNo, @UploadCount)";

                cmdDoc.Parameters.Clear();

                cmdDoc.Parameters.AddWithValue("@ReqNo", reqId); // or actual ReqNo
                cmdDoc.Parameters.AddWithValue("@DocType", docCode);
                cmdDoc.Parameters.AddWithValue("@EmpID", empId);

                cmdDoc.Parameters.AddWithValue("@ActiveStatus", 20);
                cmdDoc.Parameters.AddWithValue("@Status", 20);

                cmdDoc.Parameters.AddWithValue("@LocLib5", locLib5);      // if unknown
                cmdDoc.Parameters.AddWithValue("@SalProfile", salProfile);   // if unknown

                cmdDoc.Parameters.AddWithValue("@ReqID", 1);

                cmdDoc.Parameters.AddWithValue("@ViewNo", 1);
                cmdDoc.Parameters.AddWithValue("@UploadCount", 1);

                cmdDoc.ExecuteNonQuery();
            }

            // =========================
            // CASE 2: FETCH FROM GR DOCS
            // =========================
            else
            {
                string docTable = null;

                switch (docCode)
                {
                    case "31":
                        docTable = "Documents_31";
                        break;

                    case "RSV":
                        docTable = "Documents_RSV";
                        break;

                    case "29":
                        docTable = "Documents_29";
                        break;
                    case "30":
                        docTable = "Documents_30";
                        break;

                    default:
                        docTable = null;
                        break;
                }

                if (string.IsNullOrEmpty(docTable))
                    return;

                cmdDoc.CommandText = $@"SELECT TOP 1 DocID, DocumentName, ContentType, Datas FROM AlGurg_GR_Docs.dbo.{docTable} WHERE EmpID = @EmpID";

                cmdDoc.Parameters.Clear();
                cmdDoc.Parameters.AddWithValue("@EmpID", empId);

                using (SqlDataReader dr = cmdDoc.ExecuteReader())
                {
                    if (!dr.Read()) return;

                    int docID = Convert.ToInt32(dr["DocID"]);
                    string docName = dr["DocumentName"].ToString();
                    string type = dr["ContentType"].ToString();
                    byte[] data = (byte[])dr["Datas"];
                    dr.Close();

                    // 1️ Insert Document header
                    cmdDoc.CommandText = @"INSERT INTO AuthorityServiceRequestDocument(AuthorityServiceRequestID, DocumentTypeCode, DocID, UploadCount, Status, IsOptional, CreateDate)VALUES (@ReqID, @DocCode, @DocID, @UploadCount, @Status, @IsOptional, GETDATE());SELECT SCOPE_IDENTITY();";
                    cmdDoc.Parameters.Clear();
                    cmdDoc.Parameters.AddWithValue("@ReqID", reqId);
                    cmdDoc.Parameters.AddWithValue("@DocCode", docCode);
                    cmdDoc.Parameters.AddWithValue("@DocID", 0);
                    cmdDoc.Parameters.AddWithValue("@UploadCount", 1);
                    cmdDoc.Parameters.AddWithValue("@Status", 5);
                    cmdDoc.Parameters.AddWithValue("@IsOptional", docCode == "31" ? 1 : 0);

                    authDocId = Convert.ToInt32(cmdDoc.ExecuteScalar());

                    // 2️⃣ Insert Attachment
                    cmdDoc.CommandText = @"INSERT INTO AuthorityServiceRequestDocumentAttatchment(AuthSerReqtDocID, DocID, DocumentName, ContentType, Datas)VALUES (@AuthDocID, @DocID, @Name, @Type, @Data)";
                    cmdDoc.Parameters.Clear();
                    cmdDoc.Parameters.AddWithValue("@AuthDocID", authDocId);
                    cmdDoc.Parameters.AddWithValue("@DocID", docID);
                    cmdDoc.Parameters.AddWithValue("@Name", docName);
                    cmdDoc.Parameters.AddWithValue("@Type", type);
                    cmdDoc.Parameters.AddWithValue("@Data", data);
                    cmdDoc.ExecuteNonQuery();

                    Common.LogAction("SaveExitDocument completed ");
                }
            }
        }
   
    }
}
