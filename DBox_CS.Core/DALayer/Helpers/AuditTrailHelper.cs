using HCMS.Entity;
using DBox_CS.Core.AppClass;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace DBox_CS.Core.DALayer.Helpers
{
    public static class AuditTrailHelper
    {
        #region Insert
        public static bool Insert(AuditTrail oAuditTrail, ref SqlConnection myConn)
        {
            bool RowsAffected = false;
            //SqlConnection myConn = new SqlConnection();
            //myConn.ConnectionString = ConnectionFunctions.GetConnectionString();
            try
            {
                //myConn.Open();
                string sqry = "EAF_USP_AuditTrailInsert";
                SqlCommand myCmd = new SqlCommand(sqry, myConn);
                myCmd.CommandType = CommandType.StoredProcedure;

                myCmd.Parameters.AddWithValue("Table", oAuditTrail.Table);
                myCmd.Parameters.AddWithValue("Transaction", oAuditTrail.Transaction);
                myCmd.Parameters.AddWithValue("TransactionNo", oAuditTrail.TransactionNo);
                myCmd.Parameters.AddWithValue("EmpCode", oAuditTrail.EmpCode);
                myCmd.Parameters.AddWithValue("UserID", oAuditTrail.UserID);
                myCmd.Parameters.AddWithValue("Date", oAuditTrail.Date);
                myCmd.Parameters.AddWithValue("Errors", oAuditTrail.Errors);
                myCmd.Parameters.AddWithValue("Flag", oAuditTrail.Flag);
                myCmd.Parameters.AddWithValue("WComp", oAuditTrail.WComp);
                if (string.IsNullOrEmpty(oAuditTrail.MachineName))
                {
                    //myCmd.Parameters.AddWithValue( "MachineName",  DBNull.Value);
                    try { myCmd.Parameters.AddWithValue("MachineName", Common.GetIPAddress); }
                    catch { myCmd.Parameters.AddWithValue("MachineName", DBNull.Value); }
                }
                else
                {
                    myCmd.Parameters.AddWithValue("MachineName", oAuditTrail.MachineName);
                }
                int val = myCmd.ExecuteNonQuery();
                
                if (val == 0) { RowsAffected = false; } else { RowsAffected = true; }
            }
            catch (Exception ex)
            {
                Common.LogException(ex);
            }
            finally
            {
                //if (myConn.State != ConnectionState.Closed)
                //    myConn.Close();
            }
            return RowsAffected;
        }
        #endregion


    }
}