using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Collections;
using System.Data.SqlClient;
using DBox_CS.Core.HCMS.Entity;
using DBox_CS.Core.AppClass;

namespace DBox_CS.Core.DALayer.Helpers
{
    public static class ApprProcessHelper
    {

        #region Insert
        public static bool Insert(ApprProcess Object, ref SqlConnection myConn)
        {
            bool RowsAffected = false;
            //SqlConnection myConn = new SqlConnection();
            //myConn.ConnectionString = ConnectionFunctions.GetConnectionString();
            try
            {

                //myConn.Open();
                string sqry = "EAF_USP_ApprProcessInsert";
                SqlCommand myCmd = new SqlCommand(sqry, myConn);
                myCmd.CommandType = CommandType.StoredProcedure;

                myCmd.Parameters.AddWithValue("Priority", Object.Priority);
                myCmd.Parameters.AddWithValue("ViewNo", Object.ViewNo);
                myCmd.Parameters.AddWithValue("ReqNo", Object.ReqNo);
                myCmd.Parameters.AddWithValue("RequestDate", Object.RequestDate);
                myCmd.Parameters.AddWithValue("EmpID", Object.EmpID);
                myCmd.Parameters.AddWithValue("Isl", Object.Isl);
                myCmd.Parameters.AddWithValue("App", Object.App);
                myCmd.Parameters.AddWithValue("AppDate", Object.AppDate);
                myCmd.Parameters.AddWithValue("NoOfAppr", Object.NoOfAppr);
                myCmd.Parameters.AddWithValue("Status", Object.Status);
                myCmd.Parameters.AddWithValue("Remarks", Object.Remarks);
                myCmd.Parameters.AddWithValue("DocAttach", Object.DocAttach);
                myCmd.Parameters.AddWithValue("OnHold", Object.OnHold);
                myCmd.Parameters.AddWithValue("HoldUserNo", Object.HoldUserNo);
                myCmd.Parameters.AddWithValue("Deleted", Object.Deleted);
                myCmd.Parameters.AddWithValue("Returned", Object.Returned);
                myCmd.Parameters.AddWithValue("LastModDateTime", Object.LastModDateTime);
                myCmd.Parameters.AddWithValue("LockedByUser", Object.LockedByUser);
                myCmd.Parameters.AddWithValue("ReqID", Object.ReqID);
                myCmd.Parameters.AddWithValue("NextApprAuth", Object.NextApprAuth);
                myCmd.Parameters.AddWithValue("AsGroup", Object.AsGroup);
                myCmd.Parameters.AddWithValue("GroupNo", Object.GroupNo);
                myCmd.Parameters.AddWithValue("Selected", Object.Selected);
                myCmd.Parameters.AddWithValue("Bypassed", Object.Bypassed);
                myCmd.Parameters.AddWithValue("ReturnedUserNo", Object.ReturnedUserNo);
                myCmd.Parameters.AddWithValue("Isla", Object.Isla);
                myCmd.Parameters.AddWithValue("WFCode", Object.WFCode);

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