using DBox_CS.Core.AppClass;
using DBox_CS.Core.DALayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBox_CS.Core.BL
{
    class WorkAgreementImportBL
    {
        public void SaveWorkAgreementFromStaging(DataTable mydt_staging)
        {
            bool hasProcessError = false;

            foreach (DataRow rowWT in mydt_staging.Rows)
            {
                try
                {
                    decimal basicSalary = ValidateDecimalField(rowWT, "basicSalary");
                    decimal housingAmt = ValidateDecimalField(rowWT, "housingAmount");
                    decimal transportAmt = ValidateDecimalField(rowWT, "transportingAmount");
                    decimal foodAmt = ValidateDecimalField(rowWT, "foodAllowance");

                    decimal mobileAmt = ValidateDecimalField(rowWT, "mobileConnectivityAllowance");
                    decimal costLivingAmt = ValidateDecimalField(rowWT, "costOfLivingAllowance");
                    decimal otherAmt = ValidateDecimalField(rowWT, "otherAllowance");

                    int noticeByEmp = ValidateMonthField(rowWT, "noticePeriod");
                    int noticeByComp = noticeByEmp;

                    short probation = Convert.ToInt16(ValidateMonthField(rowWT, "probation"));

                    string empcode = string.Empty;

                    if (rowWT["EmployeeID"] != DBNull.Value)
                        empcode = rowWT["EmployeeID"].ToString();

                    SaveWorkAgreement(empcode, basicSalary, housingAmt, transportAmt, foodAmt, mobileAmt, costLivingAmt, otherAmt, noticeByEmp, noticeByComp, probation);
                }
                catch (Exception ex)
                {
                    Common.LogAction($"An error occurred: {ex.Message}");
                    Common.LogException(ex);
                    hasProcessError = true;
                    throw;
                }
            }
        }
        private void SaveWorkAgreement(string empcode, decimal basicSalary, decimal housingAmt, decimal transportAmt, decimal foodAmt, decimal mobileAmt, decimal costLivingAmt, decimal otherAmt, int noticeByEmp, int noticeByComp, short probation)
        {
            using (SqlConnection con = new SqlConnection(ConnectionFunctions.GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GR_SaveWrkAgrmnt_FromAPI", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@EmpCode", empcode);
                    cmd.Parameters.AddWithValue("@BSalaryAmt", basicSalary);
                    cmd.Parameters.AddWithValue("@HRAAmt", housingAmt);
                    cmd.Parameters.AddWithValue("@TranAmt", transportAmt);
                    cmd.Parameters.AddWithValue("@FoodAmt", foodAmt);

                    //cmd.Parameters.AddWithValue("@AuxAll1Amt", mobileAmt);
                    //cmd.Parameters.AddWithValue("@AuxAll2Amt", costLivingAmt);
                    //cmd.Parameters.AddWithValue("@AuxAll3Amt", otherAmt);

                    cmd.Parameters.AddWithValue("@AuxAll1Amt", otherAmt);
                    cmd.Parameters.AddWithValue("@AuxAll2Amt", mobileAmt);
                    cmd.Parameters.AddWithValue("@AuxAll3Amt", costLivingAmt);

                    cmd.Parameters.AddWithValue("@AuxAll4Amt", 0);
                    cmd.Parameters.AddWithValue("@AuxAll5Amt", 0);
                    cmd.Parameters.AddWithValue("@AuxAll6Amt", 0);
                    cmd.Parameters.AddWithValue("@AuxAll7Amt", 0);
                    cmd.Parameters.AddWithValue("@AuxAll8Amt", 0);

                    cmd.Parameters.AddWithValue("@NoticeByEmp", noticeByEmp);
                    cmd.Parameters.AddWithValue("@NoticeByComp", noticeByComp);
                    cmd.Parameters.AddWithValue("@UserID", Common.strSvcUserId);
                    cmd.Parameters.AddWithValue("@ProbationPeriod", probation);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
        private decimal ValidateDecimalField(DataRow row, string columnName)
        {
            if (row[columnName] == DBNull.Value || string.IsNullOrWhiteSpace(row[columnName].ToString()))
                throw new Exception(columnName + " is mandatory.");

            decimal result;
            if (!decimal.TryParse(row[columnName].ToString(), out result))
                throw new Exception(columnName + " must be a numeric value.");

            return result;
        }
        private int ValidateMonthField(DataRow row, string columnName)
        {
            if (row[columnName] == DBNull.Value || string.IsNullOrWhiteSpace(row[columnName].ToString()))
                throw new Exception(columnName + " is mandatory.");

            string text = row[columnName].ToString();
            string numberPart = text.Split(' ')[0];

            int result;
            if (!int.TryParse(numberPart, out result))
                throw new Exception(columnName + " must contain numeric month value.");

            return result;
        }
    }
}
