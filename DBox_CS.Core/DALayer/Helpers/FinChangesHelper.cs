using DBox_CS.Core.HCMS.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace DBox_CS.Core.DALayer.Helpers
{
    public static class FinChangesHelper
    {
        #region Insert
        public static bool Insert(FinChanges Object, ref SqlConnection myConn)
        {
            bool RowsAffected = false;
            //SqlConnection myConn = new SqlConnection();
            //myConn.ConnectionString = ConnectionFunctions.GetConnectionString();
            try
            {
                //myConn.Open();
                string sqry = "EAF_USP_Financial_InsertFinChanges";
                SqlCommand myCmd = new SqlCommand(sqry, myConn);
                myCmd.CommandType = CommandType.StoredProcedure;

                myCmd.Parameters.AddWithValue("ReqNo", Object.ReqNo);
                myCmd.Parameters.AddWithValue("SrNo", Object.SrNo);
                myCmd.Parameters.AddWithValue("Element", Object.Element);
                myCmd.Parameters.AddWithValue("FromVal", Object.FromVal);
                myCmd.Parameters.AddWithValue("ToVal", Object.ToVal);
                myCmd.Parameters.AddWithValue("CodeFrom", Object.CodeFrom);
                myCmd.Parameters.AddWithValue("CodeTo", Object.CodeTo);
                myCmd.Parameters.AddWithValue("RemarksE", Object.RemarksE);
                myCmd.Parameters.AddWithValue("Attachment", Object.Attachment);
                myCmd.Parameters.AddWithValue("Status", Object.Status);
                myCmd.Parameters.AddWithValue("ActiveStatus", Object.ActiveStatus);
                myCmd.Parameters.AddWithValue("SalUpgrade", Object.SalUpgrade);
                int val = myCmd.ExecuteNonQuery();
                if (val == 0) { RowsAffected = false; } else { RowsAffected = true; }
            }
            catch (Exception ex)
            {
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


