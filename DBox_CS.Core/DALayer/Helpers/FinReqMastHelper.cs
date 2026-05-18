using DBox_CS.Core.AppClass;
using DBox_CS.Core.HCMS.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBox_CS.Core.DALayer.Helpers
{
    public class FinReqMastHelper
    {
        #region Insert
        public static bool Insert(FinReqMast Object, ref SqlConnection myConn)
        {
            bool RowsAffected = false;
            //SqlConnection myConn = new SqlConnection();
            //myConn.ConnectionString = ConnectionFunctions.GetConnectionString();
            try
            {
                //myConn.Open();
                string sqry = "EAF_USP_Financial_InsertFinReqMast";
                SqlCommand myCmd = new SqlCommand(sqry, myConn);
                myCmd.CommandType = CommandType.StoredProcedure;
                myCmd.Parameters.AddWithValue("SrNo", Object.SrNo);
                myCmd.Parameters.AddWithValue("ReqDate", Object.ReqDate);
                myCmd.Parameters.AddWithValue("EffectiveDate", Object.EffectiveDate);
                myCmd.Parameters.AddWithValue("EmpID", Object.EmpID);
                myCmd.Parameters.AddWithValue("EmpCode", Object.EmpCode);
                myCmd.Parameters.AddWithValue("EmpNameE", Object.EmpNameE);
                myCmd.Parameters.AddWithValue("EmpNameA", Object.EmpNameA);
                myCmd.Parameters.AddWithValue("FinFlag", Object.FinFlag);
                myCmd.Parameters.AddWithValue("ModChangeFlag", Object.ModChangeFlag);
                myCmd.Parameters.AddWithValue("ReqID", Object.ReqID);
                myCmd.Parameters.AddWithValue("Status", Object.Status);
                myCmd.Parameters.AddWithValue("ActiveStatus", Object.ActiveStatus);
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

        #region getMastReqNo
        public static Int32 GetFinMastReqSrNo(ref SqlConnection myConn)
        {
            //SqlConnection myConn = new SqlConnection();
            //myConn.ConnectionString = ConnectionFunctions.GetConnectionString();
            try
            {
                //myConn.Open();
                string sqry = "EAF_USP_Financial_GetFinReqMastSrNo";
                SqlCommand myCmd = new SqlCommand(sqry, myConn);
                myCmd.CommandType = CommandType.StoredProcedure;

                var srNo = myCmd.ExecuteScalar();

                if (srNo != null && srNo != "")
                {
                    return Convert.ToInt32(srNo) + 1;
                }
                else
                {
                    // For First Time
                    return 1;
                }
                


            }
            catch (Exception ex)
            {
                return 0;
            }
            finally
            {
                //if(myConn.State!=ConnectionState.Closed)
                //    myConn.Close();
            }
        }
        #endregion
    }
}
