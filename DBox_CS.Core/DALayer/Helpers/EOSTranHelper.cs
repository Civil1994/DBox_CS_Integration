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
    public class EOSTranHelper
    {

        #region Insert
        public static bool Insert(EOSTran oEOSTran, ref SqlConnection myConn)
        {
            bool RowsAffected = false;
            //SqlConnection myConn = new SqlConnection();
            //myConn.ConnectionString = ConnectionFunctions.GetConnectionString();
            try
            {
                //myConn.Open();
                string sqry = "EAF_USP_EOSTranInsert";
                SqlCommand myCmd = new SqlCommand(sqry, myConn);
                myCmd.CommandType = CommandType.StoredProcedure;

                myCmd.Parameters.AddWithValue("ReqNo", oEOSTran.ReqNo);
                myCmd.Parameters.AddWithValue("ReqDate", oEOSTran.ReqDate);
                myCmd.Parameters.AddWithValue("EmpID", oEOSTran.EmpID);
                myCmd.Parameters.AddWithValue("LocLib5", oEOSTran.LocLib5);
                myCmd.Parameters.AddWithValue("SalProfile", oEOSTran.SalProfile);
                myCmd.Parameters.AddWithValue("JobTitle", oEOSTran.JobTitle);
                myCmd.Parameters.AddWithValue("LastDayInService", oEOSTran.LastDayInService);
                myCmd.Parameters.AddWithValue("EndOfServiceType", oEOSTran.EndOfServiceType);
                myCmd.Parameters.AddWithValue("ResignationDate", oEOSTran.ResignationDate);
                myCmd.Parameters.AddWithValue("TerminationDate", oEOSTran.TerminationDate);
                myCmd.Parameters.AddWithValue("EOSRemarks", oEOSTran.EOSRemarks);
                myCmd.Parameters.AddWithValue("Officialtreatment", oEOSTran.Officialtreatment);
                myCmd.Parameters.AddWithValue("LOCancDate", oEOSTran.LOCancDate);
                myCmd.Parameters.AddWithValue("LOBan", oEOSTran.LOBan);
                myCmd.Parameters.AddWithValue("LOMonths", oEOSTran.LOMonths);
                myCmd.Parameters.AddWithValue("ICancDate", oEOSTran.ICancDate);
                myCmd.Parameters.AddWithValue("IBan", oEOSTran.IBan);
                myCmd.Parameters.AddWithValue("IMonths", oEOSTran.IMonths);
                myCmd.Parameters.AddWithValue("LeavingDate", oEOSTran.LeavingDate);
                myCmd.Parameters.AddWithValue("SCReducingDate", oEOSTran.SCReducingDate);
                myCmd.Parameters.AddWithValue("SCRegNo", oEOSTran.SCRegNo);
                myCmd.Parameters.AddWithValue("LastActLeavingDate", oEOSTran.LastActLeavingDate);
                myCmd.Parameters.AddWithValue("EstRejoiningDate", oEOSTran.EstRejoiningDate);
                myCmd.Parameters.AddWithValue("DiffTillReqDate", oEOSTran.DiffTillReqDate);
                myCmd.Parameters.AddWithValue("LONotifyDate", oEOSTran.LONotifyDate);
                myCmd.Parameters.AddWithValue("LONotifySrNo", oEOSTran.LONotifySrNo);
                myCmd.Parameters.AddWithValue("BankAmt", oEOSTran.BankAmt);
                myCmd.Parameters.AddWithValue("BankDetails", oEOSTran.BankDetails);
                myCmd.Parameters.AddWithValue("ActiveStatus", oEOSTran.ActiveStatus);
                myCmd.Parameters.AddWithValue("Status", oEOSTran.Status);
                myCmd.Parameters.AddWithValue("LastModDateTime", oEOSTran.LastModDateTime);
                myCmd.Parameters.AddWithValue("ReqID", oEOSTran.ReqID);
                myCmd.Parameters.AddWithValue("EOSReason", oEOSTran.EOSReason);
                myCmd.Parameters.AddWithValue("EOSOffTreat", oEOSTran.EOSOffTreat);
                myCmd.Parameters.AddWithValue("NoticeWorkYN", oEOSTran.NoticeWrk);
                myCmd.Parameters.AddWithValue("NoticeWorkDate", oEOSTran.NoticeWrkDate);
                myCmd.Parameters.AddWithValue("WaiveNP", oEOSTran.SettleEnt);

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
