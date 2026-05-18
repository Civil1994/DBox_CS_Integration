using DBox_CS.Core.DALayer;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace DBox_CS.Core.AppClass
{
    public class CApprProcess
    {
        //26-06-2022: Robin Added class, copy of HCMS\Areas\EApproval\Old_App_Code\CApprProcess.vb
        //Created for resolving financial bypass case 

        public CApprProcess()
        {
            m_ApprProcessData = new CApprProcessData();
            Empty();
        }

        public class CApprProcessData
        {
            public byte Priority;
            public int ViewNo;
            public long ReqNo;
            public DateTime RequestDate;
            public long EmpID;
            public string ISL;
            public string App;
            public string AppDate;
            public byte NoOfAppr;
            public byte Status;
            public byte OnHold;
            public byte Returned;
            public byte Deleted;
            public string Remarks;
            public string DocAttach;
            public DateTime LastModDateTime;
            public long ReqID;
            public byte AsGroup;
            public byte Bypassed;
            public string WfCode;
        }

        public CApprProcessData m_ApprProcessData;

        public void Empty()
        {
            m_ApprProcessData.Priority = 0;
            m_ApprProcessData.ViewNo = 0;
            m_ApprProcessData.ReqNo = 0;
            m_ApprProcessData.RequestDate = new DateTime(1900, 1, 1);
            m_ApprProcessData.EmpID = 0;
            m_ApprProcessData.ISL = string.Empty;
            m_ApprProcessData.App = string.Empty;
            m_ApprProcessData.AppDate = string.Empty;
            m_ApprProcessData.NoOfAppr = 0;
            m_ApprProcessData.Status = 0;

            m_ApprProcessData.OnHold = 0;
            m_ApprProcessData.Returned = 0;
            m_ApprProcessData.Deleted = 0;

            m_ApprProcessData.Remarks = string.Empty;

            m_ApprProcessData.DocAttach = string.Empty;
            m_ApprProcessData.LastModDateTime = new DateTime(1900, 1, 1);
            m_ApprProcessData.ReqID = 0;
            m_ApprProcessData.AsGroup = 0;
            m_ApprProcessData.Bypassed = 0;
        }

        public bool GetValueApprProcess(ref int nViewNo, ref long lReqNo, ref CApprProcessData ApprProcessData, ref SqlConnection Conn, ref string ErrMsg)
        {
            bool RetVal = true;
            SqlDataReader MyReader = null/* TODO Change to default(_) if this is not a reference type */;
            try
            {
                RetVal = ConnectionFunctions.Connect_SQLDataReader(ref MyReader, "SELECT * FROM ApprProcess WITH (NOLOCK) WHERE ViewNo = " + nViewNo + " And ReqNo = " + lReqNo, ref ErrMsg, ref Conn);
                if (RetVal == false)
                    goto ExitTry;

                if (MyReader.HasRows)
                {
                    MyReader.Read();
                    ApprProcessData.ViewNo = Convert.ToInt32(MyReader["ViewNo"]);
                    ApprProcessData.ReqNo = Convert.ToInt64(MyReader["ReqNo"]);
                    ApprProcessData.ReqID = Convert.ToInt64(MyReader["ReqID"]);
                    ApprProcessData.Priority = Convert.ToByte(MyReader["Priority"]);
                    ApprProcessData.EmpID = Convert.ToInt64(MyReader["EmpID"]);
                    ApprProcessData.RequestDate = (MyReader["RequestDate"]==DBNull.Value? new DateTime(1900, 1, 1): Convert.ToDateTime(MyReader["RequestDate"]));
                    ApprProcessData.ISL = (MyReader["ISL"] == DBNull.Value ? "": MyReader["ISL"].ToString());
                    ApprProcessData.App =  (MyReader["App"] == DBNull.Value ? "" : MyReader["App"].ToString());
                    ApprProcessData.AppDate =  (MyReader["AppDate"] == DBNull.Value ? "" : MyReader["AppDate"].ToString());
                    ApprProcessData.NoOfAppr = Convert.ToByte(MyReader["NoOfAppr"]);
                    ApprProcessData.Status = Convert.ToByte(MyReader["Status"]);
                    ApprProcessData.OnHold = Convert.ToByte(MyReader["OnHold"]);
                    ApprProcessData.Returned = Convert.ToByte(MyReader["Returned"]);
                    ApprProcessData.Deleted = Convert.ToByte(MyReader["Deleted"]);
                    ApprProcessData.Remarks =  (MyReader["Remarks"] == DBNull.Value ? "" : MyReader["Remarks"].ToString());
                    ApprProcessData.DocAttach =  (MyReader["DocAttach"] == DBNull.Value ? "" : MyReader["DocAttach"].ToString());
                    ApprProcessData.LastModDateTime = (MyReader["LastModDateTime"] == DBNull.Value ? new DateTime(1900, 1, 1) : Convert.ToDateTime(MyReader["LastModDateTime"]));
                    ApprProcessData.AsGroup = Convert.ToByte(MyReader["AsGroup"] == DBNull.Value ? 0 : MyReader["AsGroup"]);
                    ApprProcessData.Bypassed = Convert.ToByte(MyReader["Bypassed"] == DBNull.Value ? 0 : MyReader["Bypassed"]);
                    ApprProcessData.WfCode =  (MyReader["WFCode"] == DBNull.Value ? "" : MyReader["WFCode"].ToString());// added by srini
                }
                else
                {
                    RetVal = false;
                    // Rahul Start Edit 26-04-2011
                    ErrMsg = ApprView.ShowErrorMessage(ApprView.GetLanguageType(), "APP001");
                    // Rahul End Edit 26-04-2011
                    goto ExitTry;
                }

                ExitTry:;
            }
            catch (Exception Ex)
            {
                RetVal = false;
            }
            finally
            {
                if (MyReader != null)
                {
                    if (!MyReader.IsClosed)
                        MyReader.Close();
                }
            }

            return RetVal;
        }
    }
}