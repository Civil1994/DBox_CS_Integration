using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Collections;
using System.Data.SqlClient;
using System.Collections.Generic;
using DBox_CS.Core.DALayer;
using DBox_CS.Core.DALayer.Helpers;
using DBox_CS.Core.Properties;
using DBox_CS.Core.Utility;

namespace DBox_CS.Core.AppClass
{
    public class ApprovalFlowFunctions
    {
        public static DataTable CreateAppTranSchema()
        {
            DataTable ApprovalData = new DataTable(" ApprovalData");
            ApprovalData.Columns.Add("ReqNo", Type.GetType("System.String"));
            ApprovalData.Columns.Add("EmpID", Type.GetType("System.String"));
            ApprovalData.Columns.Add("ISL", Type.GetType("System.String"));
            ApprovalData.Columns.Add("App", Type.GetType("System.String"));
            ApprovalData.Columns.Add("AppDate", Type.GetType("System.String"));
            ApprovalData.Columns.Add("NoOfAppr", Type.GetType("System.String"));
            ApprovalData.Columns.Add("Status", Type.GetType("System.String"));
            ApprovalData.Columns.Add("ByPassed", Type.GetType("System.String"));
            ApprovalData.Columns.Add("NextApprAuth", Type.GetType("System.String"));
            ApprovalData.Columns.Add("ISLA", Type.GetType("System.String"));
            ApprovalData.Columns.Add("WFCode", Type.GetType("System.String"));
            ApprovalData.Columns.Add("ActiveStatus", Type.GetType("System.String"));

            return ApprovalData;
        }

        public DataTable CreateApprStatusTableSchema()
        {
            DataTable ApprStatusTable = new DataTable("ApprStatus");
            ApprStatusTable.Columns.Add("Icon", Type.GetType("System.String"));
            ApprStatusTable.Columns.Add("ApprAuth", Type.GetType("System.String"));
            ApprStatusTable.Columns.Add("ApprDate", Type.GetType("System.String"));
            ApprStatusTable.Columns.Add("IconHand", Type.GetType("System.String"));
            ApprStatusTable.Columns.Add("Status", Type.GetType("System.String"));
            return ApprStatusTable;
        }

        public DataTable CreateLeaveTypesSchema()
        {
            DataTable LeaveTypeTable = new DataTable("LeaveType");
            LeaveTypeTable.Columns.Add("Icon", Type.GetType("System.String"));
            LeaveTypeTable.Columns.Add("LeaveType", Type.GetType("System.String"));
            LeaveTypeTable.Columns.Add("Days", Type.GetType("System.String"));

            return LeaveTypeTable;
        }
        public DataTable CreateBankSchema()
        {

            DataTable BankTable = new DataTable("Bank");
            BankTable.Columns.Add("Bank Details of Voucher", Type.GetType("System.String"));
            BankTable.Columns.Add(" ", Type.GetType("System.String"));
            return BankTable;
        }
        public DataTable CreateFinancialSchema()
        {

            DataTable LeavePaySlipTable = new DataTable("LeavePaySlip");
            LeavePaySlipTable.Columns.Add("xx", Type.GetType("System.String"));
            LeavePaySlipTable.Columns.Add("components", Type.GetType("System.String"));
            //<p align='left'>Salary Component</p>
            LeavePaySlipTable.Columns.Add("curr", Type.GetType("System.String"));
            //<p align='left'>Curr.</p>
            LeavePaySlipTable.Columns.Add("actual", Type.GetType("System.String"));
            //<p align='Right'>Actual</p>
            LeavePaySlipTable.Columns.Add("wrkdays", Type.GetType("System.String"));
            //<p align='Right'>Work Days</p>
            LeavePaySlipTable.Columns.Add("lvdays", Type.GetType("System.String"));
            //<p align='Right'>Leave Days</p>
            LeavePaySlipTable.Columns.Add("total", Type.GetType("System.String"));
            //<p align='Right'>Total</p>
            LeavePaySlipTable.Columns.Add("EntType", Type.GetType("System.String"));
            //<p align='left'>Type</p>

            return LeavePaySlipTable;
        }
        public bool GetLeavePaySlipData(ref short RoundOff, ref DataTable LeavePaySlip, ref DataTable BankTable, int ReqNo, int EmpID, System.DateTime StartDate, ref ArrayList arDays, ref ArrayList arVoucher, ref string ErrMsg)
        {
            bool RetVal = false;
            SqlDataReader MyReader = null;
            try
            {
                //-------------Set The Round Off Limit-----------------
                //string strRoundOff = "";
                int strRoundOff = 0;
                if (RoundOff == 4)
                {
                    //strRoundOff = "00.0000";
                    strRoundOff = 4;
                }
                else if (RoundOff == 3)
                {
                    //strRoundOff = "00.000";
                    strRoundOff = 3;
                }
                else if (RoundOff == 2)
                {
                    //strRoundOff = "00.00";
                    strRoundOff = 2;
                }
                else
                {
                    //strRoundOff = "00.00";
                    strRoundOff = 2;
                }
                //------------------------------------------------------------
                RetVal = ConnectionFunctions.Connect_SQLDataReader(ref MyReader, "SET DATEFORMAT YMD; EXEC HCMS_GetLeavePaySlip " + EmpID + ", " + ReqNo + ", '" + StartDate.ToString("yyyy-MM-dd") + "'", ref  ErrMsg);
                if (RetVal == false)
                {
                    return false;
                }
                if (MyReader.Read())
                {
                    if (MyReader[0].ToString() != "1")
                    {
                        MyReader.Close();
                        return false;
                    }
                }
                MyReader.NextResult();
                Hashtable Forex = new Hashtable();
                while (MyReader.Read())
                {

                    Forex.Add(MyReader[0].ToString(), MyReader[1].ToString());
                }
                MyReader.NextResult();
                if (MyReader.Read())
                {
                    short i = 0;
                    short PaymentMode = 0;
                    PaymentMode = Convert.ToInt16(MyReader["PymtMode"]);
                    ArrayList arLPayTran = new ArrayList();
                    for (i = 0; i <= 23; i++)
                    {
                        arLPayTran.Add(MyReader[i].ToString());
                    }
                    ArrayList arLPayCurr = new ArrayList();
                    for (i = 27; i <= 42; i++)
                    {
                        arLPayCurr.Add(MyReader[i].ToString());
                    }
                    ArrayList arAddDed = new ArrayList();
                    for (i = 43; i <= 50; i++)
                    {
                        arAddDed.Add(MyReader[i].ToString());
                    }
                    //Work Days, Leave Days, & Overtime Total Hours.
                    ArrayList arOtherData = new ArrayList();
                    arOtherData.Add(MyReader["TotalWorkDays"].ToString());
                    arOtherData.Add(MyReader["TotalLeaveDays"].ToString());
                    arOtherData.Add(Convert.ToDouble(MyReader["OTHours"]));
                    arDays = arOtherData;//'To Set the Header in the Grid

                    double FinalAmt = Convert.ToDouble(MyReader[26].ToString());
                    MyReader.NextResult();
                    //Fill All the Allowance Descriptions
                    ArrayList AuxAllSetup = new ArrayList();
                    AuxAllSetup.Add("AuxAllDescE");
                    while (MyReader.Read())
                    {
                        AuxAllSetup.Add(MyReader[0].ToString());
                    }
                    MyReader.NextResult();

                    // 'Fill The Current Financial Record to show in 'Actual' Column
                    MyReader.Read();
                    ArrayList FinMast = new ArrayList();
                    for (i = 0; i <= 34; i++)
                    {
                        FinMast.Add(MyReader[i].ToString());
                    }
                    MyReader.NextResult();

                    //Fill the HousingDetails Library(HRA By)
                    Hashtable HRABy = new Hashtable();
                    while (MyReader.Read())
                    {
                        HRABy.Add(MyReader[0].ToString(), MyReader[1].ToString());
                    }
                    MyReader.NextResult();

                    //'Fill All Loans
                    ArrayList Loans = new ArrayList();

                    while (MyReader.Read())
                    {
                        Loans.Add(MyReader[0].ToString() + "@" + MyReader[1].ToString());
                    }
                    MyReader.NextResult();
                    // 'Fill All Additions
                    ArrayList Additions = new ArrayList();
                    while (MyReader.Read())
                    {
                        Additions.Add(MyReader[0].ToString() + "@" + MyReader[1].ToString());
                    }

                    //Fill All Deductions
                    ArrayList Deductions = new ArrayList();
                    while (MyReader.Read())
                    {
                        Deductions.Add(MyReader[0].ToString() + "@" + MyReader[1].ToString());
                    }
                    MyReader.NextResult();
                    //'Fill Voucher Details
                    if (MyReader.Read())
                    {
                        if (MyReader["VoucherNo"].ToString() == "XXX")
                        {
                            arVoucher.Add("--");
                            arVoucher.Add("-- / -- / ----");
                        }
                        else
                        {
                            arVoucher.Add(MyReader["VoucherNo"].ToString());
                            arVoucher.Add(Convert.ToDateTime(MyReader[1]).ToString("dddd, dd MMMM yyyy"));
                        }
                        if (MyReader["PymntMode"].ToString() == "1")
                        {
                            arVoucher.Add("Bank Transfer");
                        }
                        else if (MyReader["PymntMode"].ToString() == "2")
                        {
                            arVoucher.Add("Cash"); ;
                        }
                        else if (MyReader["PymntMode"].ToString() == "3")
                        {
                            arVoucher.Add("Cheque"); ;
                        }
                        else
                        {
                            arVoucher.Add("C3Card Transfer"); ;
                        }
                        if (int.Parse(MyReader["Status"].ToString()) == 20)
                        {
                            arVoucher.Add("Pending (Approved)");
                        }
                        else if (int.Parse(MyReader["Status"].ToString()) > 20 & int.Parse(MyReader["Status"].ToString()) < 40)
                        {
                            arVoucher.Add("Done");
                        }
                        else if (int.Parse(MyReader["Status"].ToString()) == 40)
                        {
                            arVoucher.Add("Done (Closed)");
                        }
                        arVoucher.Add(int.Parse(MyReader["PayDetailReqNo"].ToString()));
                        arVoucher.Add(int.Parse(MyReader["PymntMode"].ToString()));
                    }
                    else
                    {
                        arVoucher.Add("--");//'Voucher Number
                        arVoucher.Add("--");//'Voucher Date
                        arVoucher.Add("--");//'Payment Method
                        arVoucher.Add("--");//'Status
                        arVoucher.Add(0);// 'Paydetails Number
                        arVoucher.Add(0);//'Payment Mode
                    }
                    MyReader.Close();
                    DataRow LeavePaySlipRow = null;
                    string Currency = String.Empty;

                    //'Breakup Of Days
                    LeavePaySlipRow = LeavePaySlip.NewRow();
                    LeavePaySlipRow[0] = "<FONT FACE=\"Verdana, Arial, Helvetica, sans-serif\" color=\"#336699\" size=\"1\">•</FONT>";
                    LeavePaySlipRow[1] = "<FONT COLOR=\"MAROON\">Breakup Of Days</FONT>";
                    LeavePaySlipRow[2] = "";
                    LeavePaySlipRow[3] = "";
                    LeavePaySlipRow[4] = arOtherData[0].ToString() + " Day(s)";
                    LeavePaySlipRow[5] = arOtherData[1].ToString() + " Day(s)";
                    LeavePaySlipRow[6] = Convert.ToString(int.Parse(arOtherData[0].ToString()) + int.Parse(arOtherData[1].ToString())) + " Day(s)";
                    LeavePaySlipRow[7] = "";
                    LeavePaySlip.Rows.Add(LeavePaySlipRow);

                    //'Empty Row
                    LeavePaySlipRow = LeavePaySlip.NewRow();
                    LeavePaySlipRow[0] = "";
                    LeavePaySlipRow[1] = "";
                    LeavePaySlipRow[2] = "";
                    LeavePaySlipRow[3] = "";
                    LeavePaySlipRow[4] = "";
                    LeavePaySlipRow[5] = "";
                    LeavePaySlipRow[6] = "";
                    LeavePaySlipRow[7] = "";
                    LeavePaySlip.Rows.Add(LeavePaySlipRow);

                    //'Earnings Header
                    LeavePaySlipRow = LeavePaySlip.NewRow();
                    LeavePaySlipRow[0] = "<IMG src='../../images/summary.gif'>";
                    LeavePaySlipRow[1] = "<FONT COLOR='Red'><b>Earnings:</b></FONT>";
                    LeavePaySlipRow[2] = "";
                    LeavePaySlipRow[3] = "";
                    LeavePaySlipRow[4] = "";
                    LeavePaySlipRow[5] = "";
                    LeavePaySlipRow[6] = "";
                    LeavePaySlipRow[7] = "";
                    LeavePaySlip.Rows.Add(LeavePaySlipRow);

                    //B/F Row
                    if (Convert.ToInt16(arAddDed[7]) != 0)
                    {
                        if (arLPayCurr[14] != DBNull.Value)
                        {
                            RetVal = GetCurrency(arLPayCurr[14].ToString(), ref Currency, Forex, ref ErrMsg);
                            if (RetVal == false)
                            {
                                return false;
                            }
                        }
                        LeavePaySlipRow = LeavePaySlip.NewRow();
                        LeavePaySlipRow[0] = "<FONT FACE=\"Verdana, Arial, Helvetica, sans-serif\" color=\"#336699\" size=\"1\">•</FONT>";
                        LeavePaySlipRow[1] = "&nbsp;<FONT COLOR=\"MAROON\">Last Month Adjustment</FONT>";
                        LeavePaySlipRow[2] = "&nbsp;<FONT COLOR=\"MAROON\">" + Currency + "</FONT>";
                        LeavePaySlipRow[3] = "";
                        LeavePaySlipRow[4] = "";
                        LeavePaySlipRow[5] = "";
                        LeavePaySlipRow[6] = "&nbsp;<FONT COLOR=\"#004040\">" + Math.Round(Convert.ToDouble(arAddDed[7]), strRoundOff).ToString() + "</FONT>";
                        LeavePaySlipRow[7] = "";
                        LeavePaySlip.Rows.Add(LeavePaySlipRow);

                    }
                    //Basic
                    if (arLPayCurr[0] != DBNull.Value)
                    {
                        RetVal = GetCurrency(arLPayCurr[0].ToString(), ref Currency, Forex, ref ErrMsg);
                        if (RetVal == false)
                        {
                            return false;
                        }
                    }
                    LeavePaySlipRow = LeavePaySlip.NewRow();
                    LeavePaySlipRow[0] = "<FONT FACE=\"Verdana, Arial, Helvetica, sans-serif\" color=\"#336699\" size=\"1\">•</FONT>";
                    LeavePaySlipRow[1] = "<FONT COLOR=\"MAROON\">" + AuxAllSetup[1].ToString() + "</FONT>";
                    LeavePaySlipRow[2] = "&nbsp;<FONT COLOR=\"MAROON\">" + Currency + "</FONT>";
                    LeavePaySlipRow[3] = "<FONT COLOR=\"BLUE\">" + Math.Round(Convert.ToDouble(FinMast[1]), strRoundOff).ToString() + "</FONT>";
                    LeavePaySlipRow[4] = "<FONT COLOR=\"#004040\">" + Math.Round(Convert.ToDouble(arLPayTran[0]), strRoundOff).ToString() + "</FONT>";
                    LeavePaySlipRow[5] = "<FONT COLOR=\"#004040\">" + Math.Round(Convert.ToDouble(arLPayTran[12]), strRoundOff).ToString() + "</FONT>";
                    LeavePaySlipRow[6] = "<FONT COLOR=\"#004040\">" + Math.Round(Convert.ToDouble(Convert.ToDouble(arLPayTran[0]) + Convert.ToDouble(arLPayTran[12])), strRoundOff).ToString() + "</FONT>";
                    LeavePaySlipRow[7] = "";
                    LeavePaySlip.Rows.Add(LeavePaySlipRow);

                    //HRA
                    if (arLPayCurr[1] != DBNull.Value)
                    {
                        RetVal = GetCurrency(arLPayCurr[1].ToString(), ref Currency, Forex, ref ErrMsg);
                        if (RetVal == false)
                        {
                            return false;
                        }
                    }
                    LeavePaySlipRow = LeavePaySlip.NewRow();
                    LeavePaySlipRow[0] = "<FONT FACE=\"Verdana, Arial, Helvetica, sans-serif\" color=\"#336699\" size=\"1\">•</FONT>";
                    LeavePaySlipRow[1] = "<FONT COLOR=\"MAROON\">" + AuxAllSetup[2].ToString() + "</FONT>";
                    LeavePaySlipRow[2] = "<FONT COLOR=\"MAROON\">" + Currency + "</FONT>";
                    LeavePaySlipRow[3] = "<FONT COLOR=\"BLUE\">" + Math.Round(Convert.ToDouble(FinMast[4]), strRoundOff).ToString() + "</FONT>";
                    LeavePaySlipRow[4] = "<FONT COLOR=\"#004040\">" + Math.Round(Convert.ToDouble(arLPayTran[1]), strRoundOff).ToString() + "</FONT>";
                    LeavePaySlipRow[5] = "<FONT COLOR=\"#004040\">" + Math.Round(Convert.ToDouble(arLPayTran[13]), strRoundOff).ToString() + "</FONT>";
                    LeavePaySlipRow[6] = "<FONT COLOR=\"#004040\">" + Math.Round(Convert.ToDouble(Convert.ToDouble(arLPayTran[1]) + Convert.ToDouble(arLPayTran[13])), strRoundOff).ToString() + "</FONT>";
                    LeavePaySlipRow[7] = HRABy[int.Parse(FinMast[2].ToString())].ToString();
                    LeavePaySlip.Rows.Add(LeavePaySlipRow);
                    //Transport
                    if (arLPayCurr[2] != DBNull.Value)
                    {
                        RetVal = GetCurrency(arLPayCurr[2].ToString(), ref Currency, Forex, ref ErrMsg);
                        if (RetVal == false)
                        {
                            return false;
                        }
                    }
                    LeavePaySlipRow = LeavePaySlip.NewRow();
                    LeavePaySlipRow[0] = "<FONT FACE=\"Verdana, Arial, Helvetica, sans-serif\" color=\"#336699\" size=\"1\">•</FONT>";
                    LeavePaySlipRow[1] = "<FONT COLOR=\"MAROON\">" + AuxAllSetup[3].ToString() + "</FONT>";
                    LeavePaySlipRow[2] = "<FONT COLOR=\"MAROON\">" + Currency + "</FONT>";
                    LeavePaySlipRow[3] = "<FONT COLOR=\"BLUE\">" + Math.Round(Convert.ToDouble(FinMast[7]), strRoundOff).ToString() + "</FONT>";
                    LeavePaySlipRow[4] = "<FONT COLOR=\"#004040\">" + Math.Round(Convert.ToDouble(arLPayTran[2]), strRoundOff).ToString() + "</FONT>";
                    LeavePaySlipRow[5] = "<FONT COLOR=\"#004040\">" + Math.Round(Convert.ToDouble(arLPayTran[4]), strRoundOff).ToString() + "</FONT>";
                    LeavePaySlipRow[6] = "<FONT COLOR=\"#004040\">" + Math.Round(Convert.ToDouble(Convert.ToDouble(arLPayTran[2]) + Convert.ToDouble(arLPayTran[14])), strRoundOff).ToString() + "</FONT>";
                    LeavePaySlipRow[7] = GetEntType(Convert.ToInt16(FinMast[5]));
                    LeavePaySlip.Rows.Add(LeavePaySlipRow);
                    //food
                    if (arLPayCurr[3] != DBNull.Value)
                    {
                        RetVal = GetCurrency(arLPayCurr[3].ToString(), ref Currency, Forex, ref ErrMsg);
                        if (RetVal == false)
                        {
                            return false;
                        }
                    }
                    LeavePaySlipRow = LeavePaySlip.NewRow();
                    LeavePaySlipRow[0] = "<FONT FACE=\"Verdana, Arial, Helvetica, sans-serif\" color=\"#336699\" size=\"1\">•</FONT>";
                    LeavePaySlipRow[1] = "<FONT COLOR=\"MAROON\">" + AuxAllSetup[4].ToString() + "</FONT>";
                    LeavePaySlipRow[2] = "<FONT COLOR=\"MAROON\">" + Currency + "</FONT>";
                    LeavePaySlipRow[3] = "<FONT COLOR=\"BLUE\">" + Math.Round(Convert.ToDouble(FinMast[10]), strRoundOff).ToString() + "</FONT>";
                    LeavePaySlipRow[4] = "<FONT COLOR=\"#004040\">" + Math.Round(Convert.ToDouble(arLPayTran[3]), strRoundOff).ToString() + "</FONT>";
                    LeavePaySlipRow[5] = "<FONT COLOR=\"#004040\">" + Math.Round(Convert.ToDouble(arLPayTran[15]), strRoundOff).ToString() + "</FONT>";
                    LeavePaySlipRow[6] = "<FONT COLOR=\"#004040\">" + Math.Round(Convert.ToDouble(Convert.ToDouble(arLPayTran[3]) + Convert.ToDouble(arLPayTran[15])), strRoundOff).ToString() + "</FONT>";
                    LeavePaySlipRow[7] = GetEntType(Convert.ToInt16(FinMast[8]));
                    LeavePaySlip.Rows.Add(LeavePaySlipRow);

                    //Auxall1
                    if (int.Parse(FinMast[13].ToString()) != 0)
                    {
                        if (arLPayCurr[4] != DBNull.Value)
                        {
                            RetVal = GetCurrency(arLPayCurr[4].ToString(), ref Currency, Forex, ref ErrMsg);
                            if (RetVal == false)
                            {
                                return false;
                            }
                        }
                        LeavePaySlipRow = LeavePaySlip.NewRow();
                        LeavePaySlipRow[0] = "<FONT FACE=\"Verdana, Arial, Helvetica, sans-serif\" color=\"#336699\" size=\"1\">•</FONT>";
                        LeavePaySlipRow[1] = "<FONT COLOR=\"MAROON\">" + AuxAllSetup[5].ToString() + "</FONT>";
                        LeavePaySlipRow[2] = "<FONT COLOR=\"MAROON\">" + Currency + "</FONT>";
                        LeavePaySlipRow[3] = "<FONT COLOR=\"BLUE\">" + Math.Round(Convert.ToDouble(FinMast[13]), strRoundOff).ToString() + "</FONT>";
                        LeavePaySlipRow[4] = "<FONT COLOR=\"#004040\">" + Math.Round(Convert.ToDouble(arLPayTran[4]), strRoundOff).ToString() + "</FONT>";
                        LeavePaySlipRow[5] = "<FONT COLOR=\"#004040\">" + Math.Round(Convert.ToDouble(arLPayTran[16]), strRoundOff).ToString() + "</FONT>";
                        LeavePaySlipRow[6] = "<FONT COLOR=\"#004040\">" + Math.Round(Convert.ToDouble(Convert.ToDouble(arLPayTran[4]) + Convert.ToDouble(arLPayTran[16])), strRoundOff).ToString() + "</FONT>";
                        LeavePaySlipRow[7] = GetEntType(Convert.ToInt16(FinMast[11]));
                        LeavePaySlip.Rows.Add(LeavePaySlipRow);

                    }
                    //Auxall2
                    if (int.Parse(FinMast[16].ToString()) != 0)
                    {
                        if (arLPayCurr[5] != DBNull.Value)
                        {
                            RetVal = GetCurrency(arLPayCurr[5].ToString(), ref Currency, Forex, ref ErrMsg);
                            if (RetVal == false)
                            {
                                return false;
                            }
                        }
                        LeavePaySlipRow = LeavePaySlip.NewRow();
                        LeavePaySlipRow[0] = "<FONT FACE=\"Verdana, Arial, Helvetica, sans-serif\" color=\"#336699\" size=\"1\">•</FONT>";
                        LeavePaySlipRow[1] = "<FONT COLOR=\"MAROON\">" + AuxAllSetup[6].ToString() + "</FONT>";
                        LeavePaySlipRow[2] = "<FONT COLOR=\"MAROON\">" + Currency + "</FONT>";
                        LeavePaySlipRow[3] = "<FONT COLOR=\"BLUE\">" + Math.Round(Convert.ToDouble(FinMast[16]), strRoundOff).ToString() + "</FONT>";
                        LeavePaySlipRow[4] = "<FONT COLOR=\"#004040\">" + Math.Round(Convert.ToDouble(arLPayTran[5]), strRoundOff).ToString() + "</FONT>";
                        LeavePaySlipRow[5] = "<FONT COLOR=\"#004040\">" + Math.Round(Convert.ToDouble(arLPayTran[17]), strRoundOff).ToString() + "</FONT>";
                        LeavePaySlipRow[6] = "<FONT COLOR=\"#004040\">" + Math.Round(Convert.ToDouble(Convert.ToDouble(arLPayTran[5]) + Convert.ToDouble(arLPayTran[17])), strRoundOff).ToString() + "</FONT>";
                        LeavePaySlipRow[7] = GetEntType(Convert.ToInt16(FinMast[14]));
                        LeavePaySlip.Rows.Add(LeavePaySlipRow);

                    }
                    //Auxall3
                    if (int.Parse(FinMast[19].ToString()) != 0)
                    {
                        if (arLPayCurr[6] != DBNull.Value)
                        {
                            RetVal = GetCurrency(arLPayCurr[6].ToString(), ref Currency, Forex, ref ErrMsg);
                            if (RetVal == false)
                            {
                                return false;
                            }
                        }
                        LeavePaySlipRow = LeavePaySlip.NewRow();
                        LeavePaySlipRow[0] = "<FONT FACE=\"Verdana, Arial, Helvetica, sans-serif\" color=\"#336699\" size=\"1\">•</FONT>";
                        LeavePaySlipRow[1] = "<FONT COLOR=\"MAROON\">" + AuxAllSetup[7].ToString() + "</FONT>";
                        LeavePaySlipRow[2] = "<FONT COLOR=\"MAROON\">" + Currency + "</FONT>";
                        LeavePaySlipRow[3] = "<FONT COLOR=\"BLUE\">" + Math.Round(Convert.ToDouble(FinMast[16]), strRoundOff).ToString() + "</FONT>";
                        LeavePaySlipRow[4] = "<FONT COLOR=\"#004040\">" + Math.Round(Convert.ToDouble(arLPayTran[6]), strRoundOff).ToString() + "</FONT>";
                        LeavePaySlipRow[5] = "<FONT COLOR=\"#004040\">" + Math.Round(Convert.ToDouble(arLPayTran[18]), strRoundOff).ToString() + "</FONT>";
                        LeavePaySlipRow[6] = "<FONT COLOR=\"#004040\">" + Math.Round(Convert.ToDouble(Convert.ToDouble(arLPayTran[6]) + Convert.ToDouble(arLPayTran[18])), strRoundOff).ToString() + "</FONT>";
                        LeavePaySlipRow[7] = GetEntType(Convert.ToInt16(FinMast[17]));
                        LeavePaySlip.Rows.Add(LeavePaySlipRow);

                    }
                    //Auxall4
                    if (int.Parse(FinMast[22].ToString()) != 0)
                    {
                        if (arLPayCurr[7] != DBNull.Value)
                        {
                            RetVal = GetCurrency(arLPayCurr[7].ToString(), ref Currency, Forex, ref ErrMsg);
                            if (RetVal == false)
                            {
                                return false;
                            }
                        }
                        LeavePaySlipRow = LeavePaySlip.NewRow();
                        LeavePaySlipRow[0] = "<FONT FACE=\"Verdana, Arial, Helvetica, sans-serif\" color=\"#336699\" size=\"1\">•</FONT>";
                        LeavePaySlipRow[1] = "<FONT COLOR=\"MAROON\">" + AuxAllSetup[8].ToString() + "</FONT>";
                        LeavePaySlipRow[2] = "<FONT COLOR=\"MAROON\">" + Currency + "</FONT>";
                        LeavePaySlipRow[3] = "<FONT COLOR=\"BLUE\">" + Math.Round(Convert.ToDouble(FinMast[22]), strRoundOff).ToString() + "</FONT>";
                        LeavePaySlipRow[4] = "<FONT COLOR=\"#004040\">" + Math.Round(Convert.ToDouble(arLPayTran[7]), strRoundOff).ToString() + "</FONT>";
                        LeavePaySlipRow[5] = "<FONT COLOR=\"#004040\">" + Math.Round(Convert.ToDouble(arLPayTran[19]), strRoundOff).ToString() + "</FONT>";
                        LeavePaySlipRow[6] = "<FONT COLOR=\"#004040\">" + Math.Round(Convert.ToDouble(Convert.ToDouble(arLPayTran[7]) + Convert.ToDouble(arLPayTran[19])), strRoundOff).ToString() + "</FONT>";
                        LeavePaySlipRow[7] = GetEntType(Convert.ToInt16(FinMast[20]));
                        LeavePaySlip.Rows.Add(LeavePaySlipRow);

                    }
                    //Auxall5
                    if (int.Parse(FinMast[25].ToString()) != 0)
                    {
                        if (arLPayCurr[8] != DBNull.Value)
                        {
                            RetVal = GetCurrency(arLPayCurr[8].ToString(), ref Currency, Forex, ref ErrMsg);
                            if (RetVal == false)
                            {
                                return false;
                            }
                        }
                        LeavePaySlipRow = LeavePaySlip.NewRow();
                        LeavePaySlipRow[0] = "<FONT FACE=\"Verdana, Arial, Helvetica, sans-serif\" color=\"#336699\" size=\"1\">•</FONT>";
                        LeavePaySlipRow[1] = "<FONT COLOR=\"MAROON\">" + AuxAllSetup[9].ToString() + "</FONT>";
                        LeavePaySlipRow[2] = "<FONT COLOR=\"MAROON\">" + Currency + "</FONT>";
                        LeavePaySlipRow[3] = "<FONT COLOR=\"BLUE\">" + Math.Round(Convert.ToDouble(FinMast[25]), strRoundOff).ToString() + "</FONT>";
                        LeavePaySlipRow[4] = "<FONT COLOR=\"#004040\">" + Math.Round(Convert.ToDouble(arLPayTran[8]), strRoundOff).ToString() + "</FONT>";
                        LeavePaySlipRow[5] = "<FONT COLOR=\"#004040\">" + Math.Round(Convert.ToDouble(arLPayTran[20]), strRoundOff).ToString() + "</FONT>";
                        LeavePaySlipRow[6] = "<FONT COLOR=\"#004040\">" + Math.Round(Convert.ToDouble(Convert.ToDouble(arLPayTran[8]) + Convert.ToDouble(arLPayTran[20])), strRoundOff).ToString() + "</FONT>";
                        LeavePaySlipRow[7] = GetEntType(Convert.ToInt16(FinMast[23]));
                        LeavePaySlip.Rows.Add(LeavePaySlipRow);

                    }
                    //Auxall6
                    if (int.Parse(FinMast[28].ToString()) != 0)
                    {
                        if (arLPayCurr[9] != DBNull.Value)
                        {
                            RetVal = GetCurrency(arLPayCurr[9].ToString(), ref Currency, Forex, ref ErrMsg);
                            if (RetVal == false)
                            {
                                return false;
                            }
                        }
                        LeavePaySlipRow = LeavePaySlip.NewRow();
                        LeavePaySlipRow[0] = "<FONT FACE=\"Verdana, Arial, Helvetica, sans-serif\" color=\"#336699\" size=\"1\">•</FONT>";
                        LeavePaySlipRow[1] = "<FONT COLOR=\"MAROON\">" + AuxAllSetup[10].ToString() + "</FONT>";
                        LeavePaySlipRow[2] = "<FONT COLOR=\"MAROON\">" + Currency + "</FONT>";
                        LeavePaySlipRow[3] = "<FONT COLOR=\"BLUE\">" + Math.Round(Convert.ToDouble(FinMast[28]), strRoundOff).ToString() + "</FONT>";
                        LeavePaySlipRow[4] = "<FONT COLOR=\"#004040\">" + Math.Round(Convert.ToDouble(arLPayTran[9]), strRoundOff).ToString() + "</FONT>";
                        LeavePaySlipRow[5] = "<FONT COLOR=\"#004040\">" + Math.Round(Convert.ToDouble(arLPayTran[21]), strRoundOff).ToString() + "</FONT>";
                        LeavePaySlipRow[6] = "<FONT COLOR=\"#004040\">" + Math.Round(Convert.ToDouble(Convert.ToDouble(arLPayTran[9]) + Convert.ToDouble(arLPayTran[21])), strRoundOff).ToString() + "</FONT>";
                        LeavePaySlipRow[7] = GetEntType(Convert.ToInt16(FinMast[26]));
                        LeavePaySlip.Rows.Add(LeavePaySlipRow);

                    }
                    //Auxall7
                    if (int.Parse(FinMast[31].ToString()) != 0)
                    {
                        if (arLPayCurr[10] != DBNull.Value)
                        {
                            RetVal = GetCurrency(arLPayCurr[10].ToString(), ref Currency, Forex, ref ErrMsg);
                            if (RetVal == false)
                            {
                                return false;
                            }
                        }
                        LeavePaySlipRow = LeavePaySlip.NewRow();
                        LeavePaySlipRow[0] = "<FONT FACE=\"Verdana, Arial, Helvetica, sans-serif\" color=\"#336699\" size=\"1\">•</FONT>";
                        LeavePaySlipRow[1] = "<FONT COLOR=\"MAROON\">" + AuxAllSetup[11].ToString() + "</FONT>";
                        LeavePaySlipRow[2] = "<FONT COLOR=\"MAROON\">" + Currency + "</FONT>";
                        LeavePaySlipRow[3] = "<FONT COLOR=\"BLUE\">" + Math.Round(Convert.ToDouble(FinMast[31]), strRoundOff).ToString() + "</FONT>";
                        LeavePaySlipRow[4] = "<FONT COLOR=\"#004040\">" + Math.Round(Convert.ToDouble(arLPayTran[10]), strRoundOff).ToString() + "</FONT>";
                        LeavePaySlipRow[5] = "<FONT COLOR=\"#004040\">" + Math.Round(Convert.ToDouble(arLPayTran[22]), strRoundOff).ToString() + "</FONT>";
                        LeavePaySlipRow[6] = "<FONT COLOR=\"#004040\">" + Math.Round(Convert.ToDouble(Convert.ToDouble(arLPayTran[10]) + Convert.ToDouble(arLPayTran[22])), strRoundOff).ToString() + "</FONT>";
                        LeavePaySlipRow[7] = GetEntType(Convert.ToInt16(FinMast[29]));
                        LeavePaySlip.Rows.Add(LeavePaySlipRow);

                    }
                    //Auxall8
                    if (int.Parse(FinMast[34].ToString()) != 0)
                    {
                        if (arLPayCurr[11] != DBNull.Value)
                        {
                            RetVal = GetCurrency(arLPayCurr[11].ToString(), ref Currency, Forex, ref ErrMsg);
                            if (RetVal == false)
                            {
                                return false;
                            }
                        }
                        LeavePaySlipRow = LeavePaySlip.NewRow();
                        LeavePaySlipRow[0] = "<FONT FACE=\"Verdana, Arial, Helvetica, sans-serif\" color=\"#336699\" size=\"1\">•</FONT>";
                        LeavePaySlipRow[1] = "<FONT COLOR=\"MAROON\">" + AuxAllSetup[12].ToString() + "</FONT>";
                        LeavePaySlipRow[2] = "<FONT COLOR=\"MAROON\">" + Currency + "</FONT>";
                        LeavePaySlipRow[3] = "<FONT COLOR=\"BLUE\">" + Math.Round(Convert.ToDouble(FinMast[34]), strRoundOff).ToString() + "</FONT>";
                        LeavePaySlipRow[4] = "<FONT COLOR=\"#004040\">" + Math.Round(Convert.ToDouble(arLPayTran[11]), strRoundOff).ToString() + "</FONT>";
                        LeavePaySlipRow[5] = "<FONT COLOR=\"#004040\">" + Math.Round(Convert.ToDouble(arLPayTran[23]), strRoundOff).ToString() + "</FONT>";
                        LeavePaySlipRow[6] = "<FONT COLOR=\"#004040\">" + Math.Round(Convert.ToDouble(Convert.ToDouble(arLPayTran[11]) + Convert.ToDouble(arLPayTran[23])), strRoundOff).ToString() + "</FONT>";
                        LeavePaySlipRow[7] = GetEntType(Convert.ToInt16(FinMast[32]));
                        LeavePaySlip.Rows.Add(LeavePaySlipRow);

                    }
                    //--------------------------------Additions--------------------------------------------------------
                    double AdditionAmt = Convert.ToDouble(Convert.ToDouble(arAddDed[2]) + Convert.ToDouble(arAddDed[3]));
                    if (AdditionAmt > 0)
                    {
                        //'Empty Row
                        LeavePaySlipRow = LeavePaySlip.NewRow();
                        LeavePaySlipRow[0] = "";
                        LeavePaySlipRow[1] = "";
                        LeavePaySlipRow[2] = "";
                        LeavePaySlipRow[3] = "";
                        LeavePaySlipRow[4] = "";
                        LeavePaySlipRow[5] = "";
                        LeavePaySlipRow[6] = "";
                        LeavePaySlipRow[7] = "";
                        LeavePaySlip.Rows.Add(LeavePaySlipRow);

                        //'Empty Row
                        LeavePaySlipRow = LeavePaySlip.NewRow();
                        LeavePaySlipRow[0] = "<IMG src='../../images/summary.gif'>";
                        LeavePaySlipRow[1] = "<FONT COLOR='Red'><b>Additions:</b></FONT>";
                        LeavePaySlipRow[2] = "";
                        LeavePaySlipRow[3] = "";
                        LeavePaySlipRow[4] = "";
                        LeavePaySlipRow[5] = "";
                        LeavePaySlipRow[6] = "";
                        LeavePaySlipRow[7] = "";
                        LeavePaySlip.Rows.Add(LeavePaySlipRow);

                        //All Additions
                        short Ctr1 = 0;
                        for (Ctr1 = 0; Ctr1 <= Additions.Count - 1; Ctr1++)
                        {
                            string[] strAdditions;
                            strAdditions = Additions[Ctr1].ToString().Split('@');
                            LeavePaySlipRow = LeavePaySlip.NewRow();
                            LeavePaySlipRow[0] = "<FONT FACE=\"Verdana, Arial, Helvetica, sans-serif\" color=\"#336699\" size=\"1\">•</FONT>";
                            LeavePaySlipRow[1] = "<FONT COLOR=\"MAROON\">" + strAdditions.GetValue(0) + "</FONT>";
                            LeavePaySlipRow[2] = "";
                            LeavePaySlipRow[3] = "";
                            LeavePaySlipRow[4] = "";
                            LeavePaySlipRow[5] = "";
                            LeavePaySlipRow[6] = "<FONT COLOR=\"#004040\">" + Math.Round(Convert.ToDouble(strAdditions.GetValue(1)), strRoundOff).ToString() + "</FONT>";
                            LeavePaySlipRow[7] = "";
                            LeavePaySlip.Rows.Add(LeavePaySlipRow);
                        }

                        // 'Overtime
                        if (int.Parse(arAddDed[4].ToString()) != 0)
                        {
                            LeavePaySlipRow = LeavePaySlip.NewRow();
                            LeavePaySlipRow[0] = "<FONT FACE=\"Verdana, Arial, Helvetica, sans-serif\" color=\"#336699\" size=\"1\">•</FONT>";
                            LeavePaySlipRow[1] = "<FONT COLOR=\"MAROON\">Overtime&nbsp;(" + arOtherData[2].ToString() + " Hrs.)</FONT>";
                            LeavePaySlipRow[2] = "";
                            LeavePaySlipRow[3] = "";
                            LeavePaySlipRow[4] = "";
                            LeavePaySlipRow[5] = "";
                            LeavePaySlipRow[6] = "<FONT COLOR=\"#004040\">" + Math.Round(Convert.ToDouble(arAddDed[4]), strRoundOff).ToString() + "</FONT>";
                            LeavePaySlipRow[7] = "";
                            LeavePaySlip.Rows.Add(LeavePaySlipRow);
                        }

                    }
                    //---------------------------------------------Deductions------------------------------------------------
                    double DeductionAmt = Convert.ToDouble(Convert.ToDouble(arAddDed[5]) + Convert.ToDouble(arAddDed[0]) + Convert.ToDouble(arAddDed[1]) + Convert.ToDouble(arAddDed[3]));
                    if (DeductionAmt > 0)
                    {
                        decimal DedAmt = Convert.ToDecimal(DeductionAmt);

                        //'Empty Row
                        LeavePaySlipRow = LeavePaySlip.NewRow();
                        LeavePaySlipRow[0] = "<IMG src='../../images/summary.gif'>";
                        LeavePaySlipRow[1] = "<FONT COLOR='Red'><b>Additions:</b></FONT>";
                        LeavePaySlipRow[2] = "";
                        LeavePaySlipRow[3] = "";
                        LeavePaySlipRow[4] = "";
                        LeavePaySlipRow[5] = "";
                        LeavePaySlipRow[6] = "";
                        LeavePaySlipRow[7] = "";
                        LeavePaySlip.Rows.Add(LeavePaySlipRow);

                        //All Deductions
                        short Ctr2 = 0;
                        for (Ctr2 = 0; Ctr2 <= Deductions.Count - 1; Ctr2++)
                        {
                            string[] strDeductions = null;
                            strDeductions = Additions[Ctr2].ToString().Split('@');
                            LeavePaySlipRow = LeavePaySlip.NewRow();
                            LeavePaySlipRow[0] = "<FONT FACE=\"Verdana, Arial, Helvetica, sans-serif\" color=\"#336699\" size=\"1\">•</FONT>";
                            LeavePaySlipRow[1] = "<FONT COLOR=\"MAROON\">" + strDeductions.GetValue(0) + "</FONT>";
                            LeavePaySlipRow[2] = "";
                            LeavePaySlipRow[3] = "";
                            LeavePaySlipRow[4] = "";
                            LeavePaySlipRow[5] = "";
                            LeavePaySlipRow[6] = "<FONT COLOR=\"#004040\">" + Math.Round(Convert.ToDouble(strDeductions.GetValue(1)), strRoundOff).ToString() + "</FONT>";
                            LeavePaySlipRow[7] = "";
                            LeavePaySlip.Rows.Add(LeavePaySlipRow);
                        }
                        //Loan Amount
                        short Ctr = 0;

                        for (Ctr = 0; Ctr <= Loans.Count - 1; Ctr++)
                        {
                            string[] strLoans;
                            strLoans = Loans[Ctr].ToString().Split('@');
                            LeavePaySlipRow = LeavePaySlip.NewRow();
                            LeavePaySlipRow[0] = "<FONT FACE=\"Verdana, Arial, Helvetica, sans-serif\" color=\"#336699\" size=\"1\">•</FONT>";
                            LeavePaySlipRow[1] = "<FONT COLOR=\"MAROON\">" + strLoans.GetValue(0) + "</FONT>";
                            LeavePaySlipRow[2] = "";
                            LeavePaySlipRow[3] = "";
                            LeavePaySlipRow[4] = "";
                            LeavePaySlipRow[5] = "";
                            LeavePaySlipRow[6] = "<FONT COLOR=\"#004040\">" + Math.Round(Convert.ToDouble(strLoans.GetValue(1)), strRoundOff).ToString() + "</FONT>";
                            LeavePaySlipRow[7] = "";
                            DedAmt = DedAmt - Convert.ToDecimal(strLoans.GetValue(1));
                            LeavePaySlip.Rows.Add(LeavePaySlipRow);
                        }
                        //Pension Amount

                        if (int.Parse(arAddDed[0].ToString()) != 0)
                        {
                            LeavePaySlipRow = LeavePaySlip.NewRow();
                            LeavePaySlipRow[0] = "<FONT FACE=\"Verdana, Arial, Helvetica, sans-serif\" color=\"#336699\" size=\"1\">•</FONT>";
                            LeavePaySlipRow[1] = "<FONT COLOR=\"MAROON\">GOSI</FONT>";
                            LeavePaySlipRow[2] = "";
                            LeavePaySlipRow[3] = "";
                            LeavePaySlipRow[4] = "";
                            LeavePaySlipRow[5] = "";
                            LeavePaySlipRow[6] = "<FONT COLOR=\"#004040\">" + Math.Round(Convert.ToDouble(arAddDed[0]), strRoundOff).ToString() + "</FONT>";
                            LeavePaySlipRow[7] = "";
                            DedAmt = DedAmt - Convert.ToDecimal(arAddDed[0]);
                            LeavePaySlip.Rows.Add(LeavePaySlipRow);
                        }
                        //Warnings Amount
                        if (int.Parse(arAddDed[1].ToString()) != 0)
                        {
                            LeavePaySlipRow = LeavePaySlip.NewRow();
                            LeavePaySlipRow[0] = "<FONT FACE=\"Verdana, Arial, Helvetica, sans-serif\" color=\"#336699\" size=\"1\">•</FONT>";
                            LeavePaySlipRow[1] = "<FONT COLOR=\"MAROON\">Warnings</FONT>";
                            LeavePaySlipRow[2] = "";
                            LeavePaySlipRow[3] = "";
                            LeavePaySlipRow[4] = "";
                            LeavePaySlipRow[5] = "";
                            LeavePaySlipRow[6] = "<FONT COLOR=\"#004040\">" + Math.Round(Convert.ToDouble(arAddDed[1]), strRoundOff).ToString() + "</FONT>";
                            LeavePaySlipRow[7] = "";
                            DedAmt = DedAmt - Convert.ToDecimal(arAddDed[1]);
                            LeavePaySlip.Rows.Add(LeavePaySlipRow);
                        }
                        if (DedAmt > 1)
                        {
                            LeavePaySlipRow = LeavePaySlip.NewRow();
                            LeavePaySlipRow[0] = "<FONT FACE=\"Verdana, Arial, Helvetica, sans-serif\" color=\"#336699\" size=\"1\">•</FONT>";
                            LeavePaySlipRow[1] = "<FONT COLOR=\"MAROON\">   </FONT>";
                            LeavePaySlipRow[2] = "";
                            LeavePaySlipRow[3] = "";
                            LeavePaySlipRow[4] = "";
                            LeavePaySlipRow[5] = "";
                            LeavePaySlipRow[6] = "<FONT COLOR=\"#004040\">" + Math.Round(DedAmt, strRoundOff).ToString() + "</FONT>";
                            LeavePaySlipRow[7] = "";
                            LeavePaySlip.Rows.Add(LeavePaySlipRow);
                        }
                    }
                    //'Empty Row
                    LeavePaySlipRow = LeavePaySlip.NewRow();
                    LeavePaySlipRow[0] = "<IMG src='../../images/summary.gif'>";
                    LeavePaySlipRow[1] = "<FONT COLOR='Red'><b>Additions:</b></FONT>";
                    LeavePaySlipRow[2] = "";
                    LeavePaySlipRow[3] = "";
                    LeavePaySlipRow[4] = "";
                    LeavePaySlipRow[5] = "";
                    LeavePaySlipRow[6] = "";
                    LeavePaySlipRow[7] = "";
                    LeavePaySlip.Rows.Add(LeavePaySlipRow);

                    //Net Payment Amount
                    if (arLPayCurr[15] != DBNull.Value)
                    {
                        RetVal = GetCurrency(arLPayCurr[15].ToString(), ref Currency, Forex, ref  ErrMsg);
                        if (RetVal == false)
                        {
                            return false;
                        }
                    }
                    LeavePaySlipRow = LeavePaySlip.NewRow();
                    LeavePaySlipRow[0] = "<IMG src='../../images/arrow.gif'>";
                    LeavePaySlipRow[1] = "<p align='left'><FONT COLOR='RED'><b>Net Payment</b></FONT>";
                    LeavePaySlipRow[2] = "<FONT COLOR=\"MAROON\">" + Currency + "</FONT>";
                    LeavePaySlipRow[3] = "";
                    LeavePaySlipRow[4] = "";
                    LeavePaySlipRow[5] = "";
                    LeavePaySlipRow[6] = "<p align='right'><FONT COLOR='RED'><b>" + Currency + " " + Math.Round(Convert.ToDouble(FinalAmt), strRoundOff).ToString() + "</b></FONT>";
                    LeavePaySlipRow[7] = "";
                    LeavePaySlip.Rows.Add(LeavePaySlipRow);

                    //'Empty Row
                    LeavePaySlipRow = LeavePaySlip.NewRow();
                    LeavePaySlipRow[0] = "<IMG src='../../images/summary.gif'>";
                    LeavePaySlipRow[1] = "<FONT COLOR='Red'><b>Additions:</b></FONT>";
                    LeavePaySlipRow[2] = "";
                    LeavePaySlipRow[3] = "";
                    LeavePaySlipRow[4] = "";
                    LeavePaySlipRow[5] = "";
                    LeavePaySlipRow[6] = "";
                    LeavePaySlipRow[7] = "";
                    LeavePaySlip.Rows.Add(LeavePaySlipRow);

                    //'Susp. Amt. C/F
                    if (arLPayCurr[13] != DBNull.Value)
                    {
                        RetVal = GetCurrency(arLPayCurr[13].ToString(), ref Currency, Forex, ref  ErrMsg);
                        if (RetVal == false)
                        {
                            return false;
                        }
                    }
                    if (int.Parse(arAddDed[6].ToString()) != 0)
                    {
                        LeavePaySlipRow = LeavePaySlip.NewRow();
                        LeavePaySlipRow[0] = "<IMG src='../../images/summary.gif'>";
                        LeavePaySlipRow[1] = "<FONT COLOR='Red'>Next Month Adjustment</FONT>";
                        LeavePaySlipRow[2] = "<FONT COLOR=\"MAROON\">" + Currency + "</FONT>";
                        LeavePaySlipRow[3] = "";
                        LeavePaySlipRow[4] = "";
                        LeavePaySlipRow[5] = "";
                        LeavePaySlipRow[6] = "<FONT COLOR='RED'>" + Math.Round(Convert.ToDouble(arAddDed[6]), strRoundOff).ToString() + "</FONT>";
                        LeavePaySlipRow[7] = "";
                        LeavePaySlip.Rows.Add(LeavePaySlipRow);

                    }


                }
                RetVal = true;
            }
            catch (Exception ex)
            {
                RetVal = false;
                ErrMsg = "Could Not Retrieve Fiancial Details of Transaction [GetLeavePaySlipData Failed]@" + ex.Message.ToString();
            }
            return RetVal;
        }
        public bool GetCurrency(string Code, ref string SignE, Hashtable Forex, ref string ErrMsg)
        {
            bool RetVal = true;
            try
            {
                if (Forex.ContainsKey(Code))
                {
                    SignE = Forex[Code].ToString();
                }
                else
                {
                    SignE = "";
                }
            }
            catch (Exception ex)
            {
                RetVal = false;
                ErrMsg = "Could Not Retrieve Currency Description [GetCurrency Failed]@" + ex.Message.ToString();

            }
            return RetVal;
        }
        public bool GetUserFullName(string fnUsers, ref ArrayList UserFullName, ref string ErrMsg)
        {

            bool RetVal = true;
            try
            {
                short Ctr = 0;
                string[] strUsers = null;
                ArrayList Users = new ArrayList();
                ArrayList FilteredUsers = new ArrayList();
                strUsers = fnUsers.Split('@');

                for (Ctr = 0; Ctr <= strUsers.GetUpperBound(0); Ctr++)
                {
                    if (!string.IsNullOrEmpty(strUsers[Ctr]))
                    {
                        Users.Add(Convert.ToString(strUsers[Ctr]));
                    }
                }

                RetVal = FilterApprovalUsers(Users, ref FilteredUsers, ref ErrMsg);
                if (RetVal == false)
                {
                    return false; // TODO: might not be correct. Was : Exit Try 
                }

                string FullNameE = string.Empty;
                for (Ctr = 0; Ctr <= Users.Count - 1; Ctr++)
                {
                    RetVal = GetFullNameE(FilteredUsers[Ctr].ToString(), ref FullNameE, ref ErrMsg);
                    if (RetVal == false)
                    {
                        return false; // TODO: might not be correct. Was : Exit Try 
                    }
                    UserFullName.Add(FullNameE);
                }
            }

            catch (Exception Ex)
            {
                ErrMsg = "Could Not Retrieve Names of Approval Authorities [GetUserFullName Failed]@" + Ex.Message;
                RetVal = false;
            }

            return RetVal;

        }

        public bool GetFullNameE(string UserID, ref string FullNameE, ref string ErrMsg)
        {

            bool RetVal = true;
            SqlDataReader MyReader = null;
            try
            {
                RetVal = ConnectionFunctions.Connect_SQLDataReader(ref MyReader, "Select FullNameE from Security where UserID = '" + UserID + "'", ref ErrMsg);
                if (RetVal == false)
                {
                    return false; // TODO: might not be correct. Was : Exit Try 
                }

                if (MyReader.Read())
                {
                    FullNameE = MyReader[0].ToString();
                }
                else
                {
                    FullNameE = UserID;
                }
            }

            catch (Exception Ex)
            {
                ErrMsg = "Could Not Retrieve Full Name of Approval Authority [GetFullNameE Failed]@" + Ex.Message;
                RetVal = false;
            }
            finally
            {
                if ((MyReader != null))
                {
                    if (!MyReader.IsClosed)
                    {
                        MyReader.Close();
                    }
                }
            }

            return RetVal;

        }

        public bool FilterApprovalUsers(ArrayList fnUsers, ref ArrayList fnFilteredUsers, ref string ErrMsg)
        {

            bool RetVal = true;
            try
            {
                string User = string.Empty;
                for (short i = 0; i <= fnUsers.Count - 1; i++)
                {
                    if (i <= 9)
                    {
                        User = fnUsers[i].ToString();
                        User = Utility.General.Mid(User, 2);
                    }
                    else if (i > 9)
                    {
                        User = fnUsers[i].ToString();
                        User = Utility.General.Mid(User, 3);
                    }
                    fnFilteredUsers.Add(User);
                }
            }

            catch (Exception Ex)
            {
                ErrMsg = "Could Not Filter Approval Authorities [FilterApprovalUsers Failed]@" + Ex.Message;
                RetVal = false;
            }

            return RetVal;

        }

        public bool GetApprovalDates(string fnApprDate, ref ArrayList FilteredDates, ref string ErrMsg)
        {

            bool RetVal = true;
            try
            {
                string[] strDates = null;
                strDates = fnApprDate.Split('@');
                ArrayList ApprDates = new ArrayList();

                for (short Ctr = 0; Ctr <= strDates.GetUpperBound(0); Ctr++)
                {
                    if (!string.IsNullOrEmpty(strDates[Ctr].ToString()))
                    {
                        ApprDates.Add(strDates[Ctr].ToString());
                    }
                }

                RetVal = FilterApprovalDates(ApprDates, ref FilteredDates, ref ErrMsg);
                if (RetVal == false)
                {
                    return false; // TODO: might not be correct. Was : Exit Try 
                }
            }

            catch (Exception Ex)
            {
                ErrMsg = "Unable to Retrieve Approval Dates for the Transaction [GetApprovalDates Failed]@" + Ex.Message;
                RetVal = false;
            }

            return RetVal;

        }

        public bool FilterApprovalDates(ArrayList fnApprDates, ref ArrayList fnFilteredDates, ref string ErrMsg)
        {

            bool RetVal = true;
            try
            {
                int i = 0;
                int a = 0;
                string ApprDate = string.Empty;

                if (!(fnApprDates.Count == 1))
                {
                    //----------------------------------- 
                    for (i = 0; i <= fnApprDates.Count - 2; i++)
                    {
                        if (i <= 9)
                        {
                            ApprDate = fnApprDates[i].ToString();
                            ApprDate = Utility.General.Mid(ApprDate, 3);
                            a = ApprDate.Length - 1;
                        }
                        else if (i > 9)
                        {
                            ApprDate = fnApprDates[i].ToString();
                            ApprDate = Utility.General.Mid(ApprDate, 4);
                            a = ApprDate.Length - 1;
                            if (a >= 0)
                            {
                                ApprDate = Utility.General.Mid(ApprDate, a);
                            }
                            else
                            {
                                ApprDate = "";
                            }
                        }
                        if (string.IsNullOrEmpty(ApprDate))
                        {
                            ApprDate = "--------------";
                        }
                        fnFilteredDates.Add(ApprDate);
                    }


                    if (i <= 9)
                    {
                        ApprDate = fnApprDates[i].ToString();
                        ApprDate = Utility.General.Mid(ApprDate, 3);
                        a = ApprDate.Length - 1;//new
                        if (a >= 0)
                        {
                            ApprDate = Utility.General.Mid(ApprDate, 1, a);
                        }
                        else
                        {
                            ApprDate = "";
                        }
                    }
                    else if (i > 9)
                    {
                        ApprDate = fnApprDates[i].ToString();
                        ApprDate = Utility.General.Mid(ApprDate, 4);
                        a = ApprDate.Length - 1;//new
                        if (a >= 0)
                        {
                            ApprDate = Utility.General.Mid(ApprDate, a);
                        }
                        else
                        {
                            ApprDate = "";
                        }
                    }
                    if (string.IsNullOrEmpty(ApprDate))
                    {
                        ApprDate = "--------------";
                    }
                    fnFilteredDates.Add(ApprDate);
                }
                //----------------------------------------------- 

                else
                {
                    ApprDate = fnApprDates[0].ToString();
                    ApprDate = Utility.General.Mid(ApprDate, 3);
                    a = ApprDate.Length - 1;
                    if (a >= 0)
                    {
                    }
                    //ApprDate = Mid(ApprDate, 1, a) 
                    else
                    {
                        ApprDate = "";
                    }
                    if (string.IsNullOrEmpty(ApprDate))
                    {
                        ApprDate = "--------------";
                    }
                    fnFilteredDates.Add(ApprDate);
                }

            }

            catch (Exception Ex)
            {
                ErrMsg = "Could Not Filter Approval Dates of Transaction [FilterApprovalDates Failed]@" + Ex.Message;
                RetVal = false;
            }

            return RetVal;

        }

        public bool GetApprovalStatus(short fnStatus, byte fnApprlevels, ref ArrayList arStatus, ref string ErrMsg)
        {

            bool RetVal = true;
            try
            {
                //----Status Counters:---- 
                // 0 = Not Yet Reached This Level 
                // 1 = Currently At This Level 
                // 2 = Approved At This Level 

                for (short i = 0; i <= fnApprlevels - 1; i++)
                {
                    if (i < fnStatus)
                    {
                        arStatus.Add("2");
                    }
                    else if (i == fnStatus)
                    {
                        arStatus.Add("1");
                    }
                    else if (i > fnStatus)
                    {
                        arStatus.Add("0");
                    }
                }
            }

            catch (Exception Ex)
            {
                ErrMsg = "Could Not Retrieve Approval Status for Current Transaction [GetApprovalStatus Failed]@" + Ex.Message;
                RetVal = false;
            }

            return RetVal;

        }
        public string GetEntType(short Code)
        {
            if (Code == 0)
            {
                return "Not Entitled";
            }
            else if (Code == 1)
            {
                return "By Company";
            }
            else if (Code == 2)
            {
                return "Monthly";
            }
            else if (Code == 3)
            {
                return "Not Entitled";
            }
            else if (Code == 4)
            {
                return "Annually";
            }
            else if (Code == 5)
            {
                return "Yearly Bonus";
            }
            else if (Code == 6)
            {
                return "Paid By Company";
            }
            else
            {
                return "Not Entitled";
            }
        }

        public bool UpdateReturnReqNoOnChangeApproval(int reqNo, int viewNo, int empID, string newApprId)
        {
            bool retVal = true;

            try
            {
                string sQry = string.Empty, ErrMsg = string.Empty, strReturned = string.Empty, strUserNo = string.Empty;

                sQry = "SELECT Returned FROM ApprProcess WITH(NOLOCK) WHERE Viewno = " + viewNo + " AND ReqNo = " + reqNo;

                if(ConnectionFunctions.Connect_SQLScalar(ref strReturned,sQry,ref ErrMsg))
                {
                    if(!string.IsNullOrEmpty(strReturned))
                    {
                        bool isReturned = Convert.ToBoolean(strReturned);

                        if(isReturned)
                        {
                            if(!string.IsNullOrEmpty(newApprId))
                            {
                                sQry = " SELECT UserNo FROM Security WITH(NOLOCK) WHERE UserID = '" + newApprId + "' ";

                                if (ConnectionFunctions.Connect_SQLScalar(ref strUserNo, sQry, ref ErrMsg))
                                {
                                    if(!string.IsNullOrEmpty(strUserNo))
                                    {
                                        int iUserNo = Convert.ToInt32(strUserNo);
                                        int res = 0;

                                        sQry = "UPDATE ApprProcess SET ReturnedUserNo=" + iUserNo + " WHERE Returned = 1 AND ViewNo=" + viewNo + " AND ReqNo=" + reqNo;
                                        if (!ConnectionFunctions.Connect_SQLNonQuery(ref res, sQry, ref ErrMsg))
                                        {
                                            return false;
                                        }
                                        else
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                return false;
            }

            return retVal;
        }



        //09-06-2022: Robin added code for Financial Request bypass
        public static bool ByPassRequestApproval(int reqNo, int viewNo, ref string ErrMsg, ref string ErrDet)
        {

            //Function Code derived from HCMS\Areas\eApprovalPortal\BL\ActionsOperations.cs call of Approval.HelperFunctions.CSApprovalData.TasksOnFinalApp

            string[] UserInfo = Common.UserInfo;
            CSApprovalData m_apprData = new CSApprovalData();
            CSApprovalData tempapprData = new CSApprovalData();
            DateTime currDate = DateTime.Now;

            string langcode = UserInfo[Convert.ToInt32(Common.APPR.Language)].ToString();


            HCMS.Entity.Employee objEmp = null;


            //Get Approval Data
            tempapprData = GetApprData(viewNo, reqNo);


            #region Get Request Related Data

            if (viewNo == 116 || viewNo == 117)
            {
                objEmp = EmployeeHelper.GetEmployee(tempapprData.m_lEmpID);
            }
            else
            {
                //for unhandled view cases return true
                return true;
            }
            #endregion


            SqlConnection Conn = new SqlConnection(ConnectionFunctions.GetConnectionString());
            if (Conn.State != ConnectionState.Open)
                Conn.Open();
            SqlCommand MyCommand = Conn.CreateCommand();


            #region Comment: Session variables SQLTran and  bTranStarted is used in eApprovalPortal\BL\ActionOperation.cs 
            //but in HCSM.Web Project ConnectionFunctions.SQLTran and ConnectionFunctions.bTranStarted is being used so commented the Session use

            //HttpContext.Current.Session["SQLTran"] = Conn.BeginTransaction(IsolationLevel.ReadUncommitted, "T1");
            //HttpContext.Current.Session["bTranStarted"] = true;
            //MyCommand.Connection = Conn;
            //SqlTransaction SQLTran = null/* TODO Change to default(_) if this is not a reference type */;
            //if ((HttpContext.Current.Session["SQLTran"] != null))
            //    SQLTran = (SqlTransaction)HttpContext.Current.Session["SQLTran"];

            //bool bTranStarted = true;
            //if ((HttpContext.Current.Session["bTranStarted"] != null))
            //    bTranStarted = Convert.ToBoolean(HttpContext.Current.Session["bTranStarted"]);
            #endregion



            ConnectionFunctions.SQLTran = Conn.BeginTransaction(IsolationLevel.ReadUncommitted, "T1");
            ConnectionFunctions.bTranStarted = true;
            MyCommand.Connection = Conn;
            MyCommand.Transaction = ConnectionFunctions.SQLTran;


            try
            {
               

                //filling only details that will be used in TasksOnFinalApp
                m_apprData.m_lEmpID = tempapprData.m_lEmpID;
                m_apprData.m_sModuleTable = tempapprData.m_sModuleTable;
                m_apprData.m_sEmpCode = tempapprData.m_sEmpCode;
                m_apprData.m_lReqNo = tempapprData.m_lReqNo;
                m_apprData.m_nViewNo = tempapprData.m_nViewNo;
                m_apprData.m_sEmpName = langcode == "1" ?  objEmp.EmpNameA:objEmp.EmpNameE;
                m_apprData.m_sCodeName = tempapprData.m_sCodeName;
                //============================================


                bool RetVal = TasksOnFinalApp(ref m_apprData, ref Conn, ref MyCommand, ref UserInfo, ref currDate, ref ErrMsg);
                if (!RetVal)
                {
                    throw new Exception(ErrMsg);
                }


                ConnectionFunctions.SQLTran.Commit();
                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = AppResources.RequestBypassApprovalTaskfailed;
                ErrDet = ex.Message;

                if (ConnectionFunctions.bTranStarted)
                {
                    ConnectionFunctions.SQLTran.Rollback();
                    ConnectionFunctions.bTranStarted = false;
                    
                }

                return false;

            }
            finally
            {
                ConnectionFunctions.bTranStarted = false;

                if (MyCommand != null)
                    MyCommand.Dispose();

                if (Conn != null)
                {
                    if (Conn.State != ConnectionState.Closed)
                    {
                        Conn.Close();
                    }
                }
                //HttpContext.Current.Session["SQLTran"] = null;
                //HttpContext.Current.Session["bTranStarted"] = null;
                if(ConnectionFunctions.SQLTran!=null)
                {
                    ConnectionFunctions.SQLTran.Dispose();
                    ConnectionFunctions.SQLTran = null;
                }
                    
            }
        }

        public static bool TasksOnFinalApp(ref CSApprovalData ApprData, ref SqlConnection Conn, ref SqlCommand MyCommand, ref string[] UserInfo, ref DateTime dtCurr, ref string ErrMsg, CApprProcess paraApprProcess = null)
        {

            //Handled Views in this Function
            //As of 28-06-2022: views 116 and 117 (Financial)
            

            string qryRslt = "";
            SqlParameter[] Params = null;
            bool RetVal = true;
            string sQry = string.Empty;
            SqlDataReader MyReader = null;
            string hfLanguage = UserInfo.GetValue(Convert.ToInt16(Common.APPR.Language)).ToString();

            try
            {
                string ScMP = string.Empty;


                if (ApprData.m_nViewNo != 300 && ApprData.m_nViewNo != 310 && ApprData.m_nViewNo != 305) 
                {
                    if (CheckVoucherCreated("EosPayTran", ApprData.m_lEmpID, ref Conn, ref ErrMsg))
                    {
                        //Nishad Edited 26012017
                        string lastDayinSrvc  = "";
                        string ldQry  = "Select CONVERT(VARCHAR(10),LastDayInService,103) AS LastDayInService from EosTran WITH (NOLOCK) where EmpID = " + ApprData.m_lEmpID;
                        ConnectionFunctions.Connect_SQLScalar(ref qryRslt, ldQry, ref ErrMsg);
                        lastDayinSrvc = qryRslt;
                        ErrMsg = ApprView.ShowErrorMessage(hfLanguage, "DEF020") + " " + ApprData.m_sCodeName + ", Last Day in Service " + lastDayinSrvc + " " + ApprView.ShowErrorMessage(hfLanguage, "DEF057") + Environment.NewLine + ApprData.m_sEmpName + Environment.NewLine + ApprView.ShowErrorMessage(hfLanguage, "DEF061");
                        //Nishad End Edit 26012017
                        RetVal = false;
                        goto ExitTry;
                    }
                }

                // Retrieving all the Approval Process data into appropriate variable.
                CApprProcess ApprProcess = new CApprProcess();

                if (paraApprProcess != null)
                {
                    ApprProcess = paraApprProcess;
                }
                else
                {
                    long m_lReqNo = (long)ApprData.m_lReqNo;
                    RetVal = ApprProcess.GetValueApprProcess(ref ApprData.m_nViewNo, ref m_lReqNo, ref ApprProcess.m_ApprProcessData, ref Conn, ref ErrMsg);
                    if (RetVal == false)
                        goto ExitTry;

                }


                ScMP = ApprView.GetCompanyProfile();


                int iRejType = 0;


                // Seetha 17112020 - Remove Leaving view no from below check since attendance closing check not needed for leaving
                if (ApprData.m_nViewNo == 117 | ApprData.m_nViewNo == 325 | ApprData.m_nViewNo == 350 | ApprData.m_nViewNo == 351 | (ApprData.m_nViewNo == 107 & iRejType != 1))
                {
                    if (Common.AllowFuturePosting_TA=="0")
                    {
                        if (ApprData.m_nViewNo == 117)
                        {

                            DateTime dt_EffDate = new DateTime(1900, 1, 1), dt_AttCloseDt = new DateTime(1900, 1, 1);
                            RetVal = ConnectionFunctions.Connect_SQLScalar(ref qryRslt, "SELECT EffectiveDate 'EffDate' FROM FinReqMast  WITH (NOLOCK)  WHERE SrNo = " + ApprData.m_lReqNo, ref Params, ref Conn, ref ErrMsg);
                            DateTime.TryParse(qryRslt, out dt_EffDate);
                            if (Common.CheckForAttendanceStatus(ApprData.m_lEmpID, dt_AttCloseDt, ref Conn, ref ErrMsg) == false)
                            {
                                if (dt_EffDate <= dt_AttCloseDt)
                                {
                                    // Rahul Start Edit 26-04-2011
                                    ErrMsg = ApprView.ShowErrorMessage(hfLanguage, "DEF034");
                                    // Rahul End Edit 26-04-2011
                                    RetVal = false;
                                    goto ExitTry;
                                }
                            }
                        }
                        else if (Common.CheckForAttendanceStatus(ApprData.m_lEmpID, ref Conn, ref ErrMsg) == false)
                        {
                            // Rahul Start Edit 26-04-2011
                            ErrMsg = ApprView.ShowErrorMessage(hfLanguage, "DEF034");
                            // Rahul End Edit 26-04-2011
                            RetVal = false;
                            goto ExitTry;
                        }
                    }
                    else if (Common.CheckForAttendanceStatus(ApprData.m_lEmpID, ref Conn, ref ErrMsg) == false)
                    {
                        // Rahul Start Edit 26-04-2011
                        ErrMsg = ApprView.ShowErrorMessage(hfLanguage, "DEF034");
                        // Rahul End Edit 26-04-2011
                        RetVal = false;
                        goto ExitTry;
                    }
                }

                //Nishad Added 29122013
                //Check for Loans, Addetion/Deduction, Financial New, Lifetime Flex
                if (ApprData.m_nViewNo == 191 || ApprData.m_nViewNo == 265 || ApprData.m_nViewNo == 116 || ApprData.m_nViewNo == 6015) 
                {
                    RetVal = Common.CheckForAttendanceStatus(ApprData.m_lEmpID, ref Conn, ref ErrMsg);
                    if (RetVal == false) 
                    {
                        //Rahul Start Edit 26-04-2011
                        ErrMsg = ApprView.ShowErrorMessage(hfLanguage, "DEF036");
                        //Rahul End Edit 26-04-2011
                        goto ExitTry;
                    }
                }
                //Nishad End 29122013


                // On Final Approval of New Joined Employee Financial Master
                if (ApprData.m_nViewNo == 116)
                {

                    // Shyamjith Added for 08/08/2019 for checking the licence in case of distributed licence
                    long nLic = 0;
                    long totLicCount = 0;
                    long nEmpCount;
                    string licenceQry = string.Empty;
                    licenceQry = "SELECT Count(License) License FROM LicenseDist";
                    if ((ConnectionFunctions.Connect_SQLScalar(ref qryRslt, licenceQry, ref Params, ref Conn, ref ErrMsg)))
                    {
                        long.TryParse(qryRslt, out nLic);
                        if ((nLic > 0))
                        {
                            licenceQry = "SELECT ISNULL(License,0) AS LicenseCnt FROM LicenseDist WHERE WComp in (SELECT dbo.fun_GetLocLib1WithEmpID(EmpId) FROM Finmast WITH (NOLOCK) WHERE ISNULL(Transferdate,'01/01/1900') = '01/01/1900' AND EmpID = " + ApprData.m_lEmpID.ToString() + ")";
                            if ((ConnectionFunctions.Connect_SQLScalar(ref qryRslt, licenceQry, ref Params, ref Conn, ref ErrMsg)))
                            {
                                long.TryParse(qryRslt, out totLicCount);
                                if ((totLicCount > 0))
                                {
                                    licenceQry = "SELECT ISNULL(COUNT(DISTINCT (EMPID)),0) as Cnt FROM FinMast WITH (NOLOCK) WHERE ISNULL(Transferdate,'01/01/1900') = '01/01/1900' AND Status IN (20,21,30) AND EmpID <> " + ApprData.m_lEmpID.ToString() + "  AND dbo.fun_GetLocLib1WithEmpID(EmpId) in (SELECT dbo.fun_GetLocLib1WithEmpID(EmpId) FROM Finmast WITH (NOLOCK) WHERE ISNULL(Transferdate,'01/01/1900') = '01/01/1900' AND EmpID = " + ApprData.m_lEmpID.ToString() + " )";
                                    if ((ConnectionFunctions.Connect_SQLScalar(ref qryRslt, licenceQry, ref Params, ref Conn, ref ErrMsg)))
                                    {
                                        long.TryParse(qryRslt, out nEmpCount);
                                        nEmpCount = nEmpCount + 1;
                                        if ((nEmpCount > totLicCount))
                                        {
                                            ErrMsg = "Maximum License exceeded for the Working Company. Employee cannot be approved.";
                                            RetVal = false;

                                            goto ExitTry;
                                        }
                                    }
                                    else
                                    {
                                        ErrMsg = "Maximum License exceeded for the Working Company. Employee cannot be approved.";
                                        RetVal = false;
                                        goto ExitTry;
                                    }
                                }
                                else
                                {
                                    ErrMsg = "Working Company / License not defined in License Distribution Setup. Employee cannot be approved.";
                                    RetVal = false;
                                    goto ExitTry;
                                }
                            }
                            else
                            {
                                ErrMsg = "Working Company / License not defined in License Distribution Setup.Employee cannot be approved.";
                                RetVal = false;
                                goto ExitTry;
                            }
                        }
                        else
                        {
                            RetVal = CheckLicenseStatus(ref Conn, ref ErrMsg);
                            if (RetVal == false)
                                goto ExitTry;
                        }
                    }
                    else
                    {
                        RetVal = CheckLicenseStatus(ref Conn, ref ErrMsg);
                        if (RetVal == false)
                            goto ExitTry;
                    }
                    // End Shyamjith Added for 08/08/2019 for checking the licence in case of distributed licence




                    // Check Employee License
                    // RetVal = newcommon.CheckLicenseStatus(Conn, ErrMsg)
                    // If RetVal = False Then
                    // Exit Try
                    // End If

                    // Add a record to the EmpBals table.
                    RetVal = AddToEmpBals(ref ApprData, ref Conn, ref MyCommand, ref ErrMsg);
                    if (RetVal == false)
                        goto ExitTry;
                    // Updated as per Aziz 30/03/2020 Update AlStart Year and Al Curr Year on update on Financial New Join Approval
                    RetVal = UpdateALStartYr(ref ApprData, ref Conn, ref MyCommand, ref ErrMsg);
                    if (RetVal == false)
                        goto ExitTry;

                    // Denson Added 27/01/2021
                    RetVal =UpdateNewEmpRPDHAJLeaveBal(ref ApprData, 1, ref Conn, ref MyCommand, ref ErrMsg);
                    if (RetVal == false)
                        goto ExitTry;

                    // Update Appraisal Date For New Joined Employee
                    RetVal = UpdateNewEmpNextApprDueDate(ref ApprData, 1, ref Conn, ref MyCommand, ref ErrMsg);
                    if (RetVal == false)
                        goto ExitTry;

                    if (ScMP.ToUpper() == "SME" | ScMP.ToUpper() == "BABTAIN" | ScMP.ToUpper() == "VOCO")
                    {
                        RetVal = UpdateBonusFormulaAmount(ref ApprData, ref Conn, ref MyCommand, ref ErrMsg);
                        if (RetVal == false)
                            goto ExitTry;
                    }
                    // SRINI NOTE BELOW CODE added by srini(1/05/2010)

                    string TableColumnStatus = "0";
                    string cs_CoProfileCode = "";
                    TableColumnStatus = Common.GetTableStatus(ref ErrMsg);

                    // Nishad Edited 26092016
                    // If TableColumnStatus = "1" Then
                    // cs_CoProfileCode = GetEmp_CoProfileCode(ApprData.m_lEmpID)
                    // End If
                    cs_CoProfileCode = Common.GetEmp_CoProfileCode(ApprData.m_lEmpID);
                    DateTime dt_SLReInitDtCurr = new DateTime(1900, 1, 1);
                    DateTime dt_SLReInitDtPrev = new DateTime(1900, 1, 1);
                    // Nishad End Edit 26092016

                    // SRINI NOTE Transfer Date  added by srini(1/05/2010)
                    DateTime dt_LastPaidDt = new DateTime(1900, 1, 1);

                    // Nishad Edited 28052014  --Should check Transferdate
                    // If TableColumnStatus = "1" Then
                    // sQry = "SELECT LastPaidDate FROM Finmast Where EmpId = " & ApprData.m_lEmpID & " AND IsNULL(TransferDate,'01/01/1900') = '01/01/1900'"
                    // Else
                    // sQry = "SELECT LastPaidDate FROM Finmast Where EmpId = " & ApprData.m_lEmpID
                    // End If
                    sQry = "SELECT LastPaidDate FROM Finmast WITH (NOLOCK) Where EmpId = " + ApprData.m_lEmpID + " AND IsNULL(TransferDate,'01/01/1900') = '01/01/1900'";
                    // Nishad End Edit 28052014

                    RetVal = ConnectionFunctions.Connect_SQLScalar(ref qryRslt, sQry, ref Params, ref Conn, ref ErrMsg);
                    if (RetVal == false)
                        goto ExitTry;

                    DateTime.TryParse(qryRslt, out dt_LastPaidDt);

                    DateTime dtTranDate = dt_LastPaidDt.AddDays(1);
                    RetVal = PayrollProcessFunction.ReturnSLReInitDate(ref ApprData.m_lEmpID, ref dtTranDate, ref dt_SLReInitDtCurr, ref dt_SLReInitDtPrev, ref cs_CoProfileCode, ref Conn, ref ErrMsg);
                    if (RetVal == false)
                        goto ExitTry;

                    // Seetha 22122020 - Commented below function call as per aziz Suggestion.Reinit date should be update as per above logic.
                    // RetDateSince(dt_SLReInitDtCurr, 12) 

                    // Modify Salstatus in Employee table making it 2, EmployeeStatus = 1, Start date of indemnity to joining date.
                    long lRAffected = 0L; // for getting the rows affected... s
                                          // #NEWLOC -- Removing Loc 1,2,3,4 from update query
                    if ((ApprView.GetCompanyProfile() == "GOVT"))
                    {
                        sQry = "UPDATE e SET e.SalaryStatus = 2,e.EmployeeStatus = 1,e.StartDtofIndemnity = f.Joiningdate, e.IntlJoiningDate=f.Joiningdate, ";
                        sQry += "e.Loclib5 = f.Loclib5,e.Salprofile = f.SalProfile, e.SLReInitDate = '" + dt_SLReInitDtCurr.ToString("yyyy/MM/dd") + "' ";
                        sQry += "FROM Employee e INNER JOIN Finmast f ON e.Empid = f.Empid WHERE e.EmpID = " + ApprData.m_lEmpID + " AND IsNull(f.TransferDate,'01/01/1900') = '01/01/1900'";
                    }
                    else
                    {
                        sQry = "UPDATE e SET e.SalaryStatus = 2,e.EmployeeStatus = 1,e.StartDtofIndemnity = f.Joiningdate, ";
                        sQry += "e.Loclib5 = f.Loclib5,e.Salprofile = f.SalProfile, e.SLReInitDate = '" + dt_SLReInitDtCurr.ToString("yyyy/MM/dd") + "' ";
                        sQry += "FROM Employee e INNER JOIN Finmast f ON e.Empid = f.Empid WHERE e.EmpID = " + ApprData.m_lEmpID + " AND IsNull(f.TransferDate,'01/01/1900') = '01/01/1900'";

                        sQry += " ; UPDATE e SET e.Loclib5 = f.Loclib5,e.Salprofile = f.SalProfile ";
                        sQry += "FROM Family e INNER JOIN Finmast f ON e.SponEmpCode = f.Empid WHERE e.SponEmpCode = " + ApprData.m_lEmpID + " AND IsNull(f.TransferDate,'01/01/1900') = '01/01/1900'";
                    }
                    MyCommand.CommandText = sQry;
                    lRAffected = MyCommand.ExecuteNonQuery();

                    if (lRAffected < 1)
                    {
                        // Rahul Start Edit 26-04-2011
                        ErrMsg = ApprView.ShowErrorMessage(hfLanguage, "DEF035") + Environment.NewLine + ApprView.ShowErrorMessage(hfLanguage, "DEF051");
                        // Rahul End Edit 26-04-2011
                        RetVal = false;
                        goto ExitTry;
                    }

                    // Denson added 23/02/2021
                    sQry = "delete from FirstSLDet where EmpID = " + ApprData.m_lEmpID + ";  insert into FirstSLDet SELECT Empid,JoiningDate,0 from finmast with(nolock) where  ISNULL(Transferdate,'01/01/1900') = '01/01/1900' and EmpID = " + ApprData.m_lEmpID + " ";
                    MyCommand.CommandText = sQry;
                    lRAffected = MyCommand.ExecuteNonQuery();
                    // Denson added 23/02/2021

                    // Nishad Added 08032014
                    if (ApprData.m_nViewNo == 116)
                    {
                        ScMP = ApprView.GetCompanyProfile();
                        if (ScMP.ToUpper() == "AIR ARABIA" | ScMP.ToUpper() == "BANAJA" | ScMP.ToUpper() == "BABTAIN" | ScMP.ToUpper() == "PPMDC")
                        {
                            string sLoc = "";
                            SqlCommand MyCommand1 = Conn.CreateCommand();
                            // #NEWLOC
                            sQry = "Select dbo.fun_GetLocLib1WithEmpID(" + ApprData.m_lEmpID + ")";
                            RetVal = ConnectionFunctions.Connect_SQLScalar(ref qryRslt, sQry, ref ErrMsg);
                            if (RetVal == false)
                                ErrMsg = ApprView.ShowErrorMessage(ApprView.GetLanguageType(), "GEN067");

                            sLoc = qryRslt;
                            string iCnt = "0";
                            sQry = "Select Count (Loclib1Code) AS CNT FROM PmsCompnyTransfer WHERE  Loclib1Code = '" + sLoc + "'";
                            RetVal = ConnectionFunctions.Connect_SQLScalar(ref qryRslt, sQry, ref ErrMsg);
                            if (RetVal == false)
                                ErrMsg = ApprView.ShowErrorMessage(ApprView.GetLanguageType(), "GEN067");

                            iCnt = qryRslt;

                            if (Convert.ToInt32(iCnt) > 0)
                            {
                                if (UpdatePMSOrgnsnHierarchy(false, ApprData.m_lEmpID, ApprData.m_lReqNo, false, "", ref MyCommand1, ref Conn, ref ErrMsg) == false)
                                {
                                    ErrMsg = ApprView.ShowErrorMessage(ApprView.GetLanguageType(), "PMS04");
                                    RetVal = false;
                                    goto ExitTry;
                                }
                            }
                        }
                    }

                    // Nishad End 08032013

                    // Nishad Added 27082017
                    if (ScMP.ToUpper() == "CHANEL")
                    {
                        string sQry1 = "";
                        long empRAffected = 0;
                        string salProfil = "";
                        sQry1 = "Update EmpBals Set ExtraDaysBal = 0 WHERE EmpID = " + ApprData.m_lEmpID;

                        MyCommand.CommandText = sQry1;
                        empRAffected = MyCommand.ExecuteNonQuery();

                        sQry1 = "Select SalProfile from Finmast WITH (NOLOCK) where EmpID = " + ApprData.m_lEmpID + " AND IsNull(TransferDate,'01/01/1900') = '01/01/1900'";
                        RetVal = ConnectionFunctions.Connect_SQLScalar(ref qryRslt, sQry1, ref ErrMsg);

                        salProfil = qryRslt;

                        if (salProfil.ToUpper() == "BOUT")
                        {
                            int lPHBal = 0;

                            // sQry1 = "SELECT 13 - DATEPART(mm,FirstFinEffdt) As PHBal FROM FinMast WITH (NOLOCK) WHERE EmpID = " & ApprData.m_lEmpID    'Nishad Commented 26122017
                            sQry1 = "SELECT case when datepart(dd,FirstFinEffdt) = 1 Then 13 - DATEPART(mm,FirstFinEffdt) ELSE 12 - DATEPART(mm,FirstFinEffdt) End As PHBal FROM FinMast WITH (NOLOCK) WHERE EmpID = " + ApprData.m_lEmpID;  // 'Nishad Added 26122017 --As per Aziz Mail on same date
                            RetVal = ConnectionFunctions.Connect_SQLScalar(ref qryRslt, sQry1, ref ErrMsg);

                            int.TryParse(qryRslt, out lPHBal);

                            sQry1 = "Update EmpBals Set ExtraDaysBal = " + lPHBal + " WHERE EmpID = " + ApprData.m_lEmpID;
                            MyCommand.CommandText = sQry1;
                            empRAffected = MyCommand.ExecuteNonQuery();
                        }
                    }
                    // Nishad End 27082017


                    // Nishad Added 22022021
                    string sRecordNo = "0";
                    string sQry2 = "";
                    sQry2 = "Select RecordNo from Finmast WITH (NOLOCK) where EmpID = " + ApprData.m_lEmpID + " AND IsNull(TransferDate,'01/01/1900') = '01/01/1900'";
                    RetVal = ConnectionFunctions.Connect_SQLScalar(ref qryRslt, sQry2, ref ErrMsg);

                    sRecordNo = qryRslt;

                    if (!string.IsNullOrEmpty(sRecordNo))
                    {
                        sQry2 = "Update SocialSecurity Set RecordNo = " + sRecordNo + " WHERE EmpId = " + ApprData.m_lEmpID;

                        MyCommand.CommandText = sQry2;
                        lRAffected = MyCommand.ExecuteNonQuery();
                    }
                    // Nishad End 22022021

                    // Seetha - 24052021 - Insert to tktmaster if there is no entry
                    string tktRecCount = "0";
                    sQry = "select count(*) from tktmaster with(nolock) where empid = " + ApprData.m_lEmpID + " AND ISNULL(Famcode,'') = ''";
                    RetVal = ConnectionFunctions.Connect_SQLScalar(ref qryRslt, sQry, ref ErrMsg);
                    if (RetVal == false)
                        ErrMsg = ApprView.ShowErrorMessage(ApprView.GetLanguageType(), "GEN018");

                    tktRecCount = qryRslt;

                    if (Convert.ToInt32(tktRecCount) == 0)
                    {
                        sQry = " INSERT INTO TktMaster (RecordNo,EmpID,FamCode,TktName,DateOfBirth,RelType,TktType,TktDueDate,LstTktIssueDt,EntitledYN,Status,IssuedTktAmt,RelName,TktAccTill,FamTktAccTill) " + " SELECT (SELECT MAX(RECORDNO) FROM TktMaster) + ROW_NUMBER() OVER (ORDER BY t.empid),f.EmpID,NULL,e.EmpNameE,CONVERT(DATETiME,e.DateOfBirth,103),1,1,DATEADD(MONTH,f.ETicketEvery,f.JoiningDate),f.JoiningDate,1,0,0,10,0,0 " + " FROM FinMast f " + " LEFT OUTER JOIN employee e on e.EmpID = f.EmpID " + " LEFT OUTER JOIN TktMaster t on t.empid = f.EmpID " + " where f.status < 40 and ETicketEvery > 0 and f.empid = " + ApprData.m_lEmpID.ToString() + " group by t.empid,f.EmpID,e.EmpNameE,e.DateOfBirth,f.ETicketEvery,f.JoiningDate,f.JoiningDate ";

                        MyCommand.CommandText = sQry;
                        lRAffected = MyCommand.ExecuteNonQuery();

                        string sMessage = "Ticket Master Record added from approval since record is not available";
                        sQry = "INSERT INTO dbo.Audit_FortktLV( TranDate, TranType,EMPID,Famcode, Fromval, Toval,TranFrom,UserID ) " + " SELECT  GETDATE(), 1,EMPID,Famcode,'', CONVERT(VARCHAR(15),TktDueDate,103) ,'" + sMessage + "','" + UserInfo.GetValue((int)Common.APPR.UserID) + "' FROM Tktmaster WHERE Empid = " + ApprData.m_lEmpID.ToString() + " AND ISNULL(Famcode,'') = ''";

                        MyCommand.CommandText = sQry;
                        lRAffected = MyCommand.ExecuteNonQuery();
                    }
                }

                // On Final Approval of Upgraded Financial Element, we update the FinMast table with the value from the FinChange.
                if (ApprData.m_nViewNo == 117)
                {
                    RetVal = UpdFinMastFrmFinChng(0, ref ApprData, ref UserInfo, ref Conn, ref MyCommand, ref ErrMsg);
                    if (RetVal == false)
                        goto ExitTry;
                    if (ScMP.ToUpper() == "SME" | ScMP.ToUpper() == "BABTAIN" | ScMP.ToUpper() == "VOCO")
                    {
                        RetVal = UpdateBonusFormulaAmountOnFinancialChange(ref ApprData, ref Conn, ref MyCommand, ref ErrMsg);
                        if (RetVal == false)
                            goto ExitTry;
                    }
                }



                // Updating locations and Salary profile of the employee in Pay Details and PayDetails History
                if (ApprData.m_nViewNo == 116 | ApprData.m_nViewNo == 117)
                {
                    string m_sLocLib1 = "";
                    string m_sLocLib2 = "";
                    string m_sLocLib3 = "";
                    string m_sLocLib4 = "";
                    string m_sLocLib5 = "";
                    string m_sSalProfile = "";
                    // #NEWLOC
                    sQry = "SELECT LocLib5, SalProfile FROM [FinMast] WITH (NOLOCK) WHERE EmpID = " + ApprData.m_lEmpID + " AND IsNull(TransferDate,'01/01/1900') = '01/01/1900'";

                    RetVal = ConnectionFunctions.Connect_SQLDataReader(ref MyReader, sQry, ref ErrMsg, ref Conn);
                    if (RetVal == false)
                        goto ExitTry;

                    if (MyReader.HasRows == true)
                    {
                        MyReader.Read();
                        m_sLocLib5 = (MyReader[0]==DBNull.Value? "": MyReader[0].ToString());
                        m_sSalProfile = (MyReader[1] == DBNull.Value ? "" : MyReader[1].ToString());
                    }
                    else
                    {
                        // Rahul Start Edit 26-04-2011
                        ErrMsg = ApprView.ShowErrorMessage(hfLanguage, "DEF038") + Environment.NewLine + ApprView.ShowErrorMessage(hfLanguage, "DEF052");
                        // Rahul End Edit 26-04-2011
                        RetVal = false;
                        goto ExitTry;
                    }
                    MyReader.Close();

                    long lEmployeeID = 0L;
                    string strEmployeeID = "";
                    sQry = "SELECT EmpID from PayDetails  WITH (NOLOCK) WHERE EmpID = " + ApprData.m_lEmpID;
                    RetVal = ConnectionFunctions.Connect_SQLScalar(ref qryRslt, sQry, ref Params, ref Conn,ref ErrMsg );

                    strEmployeeID = qryRslt;

                    if (RetVal == true)
                    {
                        sQry = "UPDATE PayDetails SET ";
                        sQry += "LocLib5 = '" + m_sLocLib5 + "', SalProfile = '" + m_sSalProfile + "' WHERE EmpID = " + ApprData.m_lEmpID;

                        MyCommand.CommandText = sQry;
                        MyCommand.ExecuteNonQuery();

                        sQry = "UPDATE WrkAgrmntDet SET ";
                        sQry += "LocLib5 = '" + m_sLocLib5 + "', SalProfile = '" + m_sSalProfile + "' WHERE (ActiveStatus < 35) AND EmpID = " + ApprData.m_lEmpID;

                        MyCommand.CommandText = sQry;
                        MyCommand.ExecuteNonQuery();
                    }
                }



                RetVal = true;

            ExitTry:;

            }
            catch (Exception Ex)
            {
                RetVal = false;
                // Rahul Start Edit 26-04-2011
                ErrMsg = ApprView.ShowErrorMessage(hfLanguage, "DEF039") + Environment.NewLine + ApprView.ShowErrorMessage(hfLanguage, "DEF052") + Ex.Message;
            }
            // Rahul End Edit 26-04-2011
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

        #region Functions Copied from HCMS\Areas\EApproval\Old_App_Code\CGeneral.vb
        public static bool CheckLicenseStatus(ref SqlConnection Conn, ref string ErrMsg)
        {
            bool RetVal = false;
            try
            {
                string cs_Qry = string.Empty;
                string cs_EmpCount = string.Empty;
                int nEmpCount = 0;
                string strEmpCount = "";
                int nLicCount = 0;
                string strLicCount = "";
                SqlParameter[] Params = null;
                cs_Qry = "SELECT TC FROM PrgDefault";
                RetVal = ConnectionFunctions.Connect_SQLScalar(ref cs_EmpCount, cs_Qry, ref Params, ref Conn, ref ErrMsg);
                if (RetVal == false)
                    goto ExitTry;

                // Rahul Start Edit 13-10-2011 Encrypt/Decrypt
                BlowFish objBlowFish = new BlowFish("FEDCBA9876543210");
                strLicCount = objBlowFish.Decrypt_ECB(cs_EmpCount);
                int.TryParse(strLicCount, out nLicCount);
                // Rahul Start Edit 13-10-2011 Encrypt/Decrypt

                cs_Qry = "SELECT COUNT(1) as Cnt FROM FinMast WITH (NOLOCK) WHERE Status IN (20,21,30) AND IsNull(TransferDate,'01/01/1900') = '01/01/1900'"
                        +" and dbo.fun_GetLocLib1WithEmpID(empid) in (select code from LocLib1 with(nolock) where ISNULL(Closed,'0') = '0') ";
                
                RetVal = ConnectionFunctions.Connect_SQLScalar(ref strEmpCount, cs_Qry, ref Params, ref Conn, ref ErrMsg);
                if (RetVal == false)
                    goto ExitTry;

                if (nEmpCount >= nLicCount & nLicCount != 0)
                {
                    // Rahul Start Edit 25-04-2011
                    ErrMsg = ApprView.ShowErrorMessage(ApprView.GetLanguageType(), "GEN008") + Environment.NewLine + ApprView.ShowErrorMessage(ApprView.GetLanguageType(), "GEN049");
                    // Rahul Start Edit 25-04-2011
                    RetVal = false;
                    goto ExitTry;
                }

                RetVal = true;

            ExitTry:;
            }
            catch (Exception ex)
            {
                ErrMsg = AppResources.LicenseDecryptError;
                // ErrMsg = "Error in CS0003.dll Please Contact CivilSoft." & ex.Message
                RetVal = false;
            }

            return RetVal;
        }

        public static bool AddToEmpBals(ref CSApprovalData ApprData, ref SqlConnection Conn, ref SqlCommand MyCommand, ref string ErrMsg)
        {
            bool RetVal = false;
            try
            {
                string sQry = string.Empty;
                int lRAffected = 0;
                string qryRslt = "";
                SqlParameter[] Params = null;

                // Checking whether the record is alredy there or not if it is then run the currency proc else the add the new one...
                sQry = "Select Count(EmpID) As Noc From EmpBals WITH (NOLOCK) Where EmpID = " + ApprData.m_lEmpID;
                RetVal = ConnectionFunctions.Connect_SQLScalar(ref qryRslt, sQry, ref Params, ref Conn,ref ErrMsg);
                if (RetVal == false)
                    goto ExitTry;

                int.TryParse(qryRslt, out lRAffected);

                if (lRAffected == 0)
                {
                    long lRecordno;



                    string sVal = string.Empty;

                    sQry = "Select IsNull(Val,0) As Val From MasterSetup WITH (NOLOCK) WHERE Code = '26'";
                    RetVal = ConnectionFunctions.Connect_SQLScalar(ref sVal, sQry, ref ErrMsg);
                    if (RetVal == false)
                        // Rahul Start Edit 26-04-2011
                        ErrMsg = ApprView.ShowErrorMessage(ApprView.GetLanguageType(), "GEN009");

                    // If sVal = "1" Then

                    lRecordno = GetCurrRecordNo(ApprData.m_lEmpID); // ADDED BY SRINI(30 APR)
                    sQry = "Insert into EmpBals (EmpID,RecordNo) Values (" + ApprData.m_lEmpID + "," + lRecordno + ")";

                    // Else
                    // sQry = "Insert into EmpBals (EmpID) Values (" & ApprData.m_lEmpID & ")"
                    // End If


                    // sQry = "Insert into EmpBals (EmpID) Values (" & ApprData.m_lEmpID & ")"
                    // Executing the predefined query, to put the values into the EmpBals table.
                    MyCommand.CommandText = sQry;
                    lRAffected = MyCommand.ExecuteNonQuery();

                    if (lRAffected != 1)
                    {
                        // Rahul Start Edit 26-04-2011
                        ErrMsg = ApprView.ShowErrorMessage(ApprView.GetLanguageType(), "GEN010") + Environment.NewLine + ApprView.ShowErrorMessage(ApprView.GetLanguageType(), "GEN019");
                        // Rahul End Edit 26-04-2011
                        RetVal = false;
                        goto ExitTry ;
                    }

                    // Denson added 09/01/2022
                    sQry = "Delete from EmpLeaveBal where empid = " + ApprData.m_lEmpID + " and LeaveType not in('NOTED','POTED','WOTED','OOTED') and LeaveType in (select Code from [event] with(nolock) where MaxTakenDays > 0 ) ; " + " Insert into EmpLeaveBal (EmpID,LeaveType,Bal,RecordNo) " + " Select " + ApprData.m_lEmpID + ",Code,isNull(MaxTakenDays,0)," + lRecordno + " from [event] with(nolock) where MaxTakenDays > 0  ";
                    MyCommand.CommandText = sQry;
                    lRAffected = MyCommand.ExecuteNonQuery();
                }

                RetVal = true;

            ExitTry:;
            }
            catch (Exception Ex)
            {
                RetVal = false;
                ErrMsg = Ex.Message;
            }

            return RetVal;
        }

        public static long GetCurrRecordNo(long nEmpID)
        {
            string cs_Temp;
            long nRec = 0;
            string qryReslt = "";
            bool RetVal = false;
            string ErrMsg = "";
            cs_Temp = "SELECT RecordNo FROM Finmast WITH (NOLOCK) WHERE EmpID = " + nEmpID + " AND IsNull(TransferDate,'01/01/1900') = '01/01/1900' AND ActiveStatus <=30 ";

            RetVal = ConnectionFunctions.Connect_SQLScalar(ref qryReslt, cs_Temp, ref ErrMsg);
            long.TryParse(qryReslt, out nRec);
            if (RetVal == false)
                // Rahul Start Edit 27-04-2011
                ErrMsg = ApprView.ShowErrorMessage(ApprView.GetLanguageType(), "GEN032") + Environment.NewLine + ApprView.ShowErrorMessage(ApprView.GetLanguageType(), "GEN045");

            return nRec;
        }

        public static bool UpdateALStartYr(ref CSApprovalData ApprData, ref SqlConnection Conn, ref SqlCommand MyCommand, ref string ErrMsg)
        {
            bool RetVal = false;
            try
            {
                string sQry = string.Empty;
                int lRAffected = 0;
                DateTime dt_DecDate = new DateTime();
                double fNonServLeave;
                double fAlTillDec;
                string qryRslt = "";
                SqlParameter[] Params = null;

                fNonServLeave = 0.0;
                fAlTillDec = 0.0;
                // Checking whether the record is alredy there or not if it is then run the currency proc else the add the new one...
                sQry = "Select Count(EmpID) As Noc From EmpBals Where EmpID = " + ApprData.m_lEmpID;
                RetVal = ConnectionFunctions.Connect_SQLScalar(ref qryRslt, sQry, ref Params, ref Conn, ref ErrMsg);
                
                if (RetVal == false)
                    goto ExitTry;

                int.TryParse(qryRslt, out lRAffected);

                if (lRAffected > 0)
                {
                    long lRecordno;
                    DateTime dt_JoiningDt;
                    //sQry = "SELECT JoiningDate FROM Finmast WITH (NOLOCK) Where EmpId = " + ApprData.m_lEmpID + " AND IsNULL(TransferDate,'01/01/1900') = '01/01/1900'";
                    sQry = "SELECT CASE WHEN ISNULL(FinFlag,0) = 1 THEN JoiningDate ELSE FirstFinEffdt END JoiningDate FROM Finmast WITH (NOLOCK) Where EmpId = " + ApprData.m_lEmpID + 
                        " AND IsNULL(TransferDate,'01/01/1900') = '01/01/1900'";
                    // Nishad End Edit 28052014

                    RetVal = ConnectionFunctions.Connect_SQLScalar(ref qryRslt, sQry, ref Params, ref Conn, ref ErrMsg);
                    
                    if (RetVal == false)
                        goto ExitTry;

                    DateTime.TryParse(qryRslt, out dt_JoiningDt);

                    dt_DecDate = new DateTime(dt_JoiningDt.Year, 12, 31);
                    if (!PayrollProcessFunction.CalculateLeaveBal(ApprData.m_lEmpID, ref fNonServLeave, ref dt_DecDate, ref fAlTillDec, 0.0, ref ErrMsg))
                        return false;

                    lRecordno = GetCurrRecordNo(ApprData.m_lEmpID); // ADDED BY SRINI(30 APR)
                    fAlTillDec = (fAlTillDec < 0? 0: fAlTillDec); // Seetha added 25032021 - Handled fAlTillDec shouldn't be negative  
                    sQry = "UPDATE EmpBals SET AlPrevYr = 0.0,AlCurrYr = " + fAlTillDec + ",AlStartYr = " + fAlTillDec + " WHERE EmpID = " + ApprData.m_lEmpID + " AND RecordNo = " + lRecordno;

                    // Else
                    // sQry = "Insert into EmpBals (EmpID) Values (" & ApprData.m_lEmpID & ")"
                    // End If


                    // sQry = "Insert into EmpBals (EmpID) Values (" & ApprData.m_lEmpID & ")"
                    // Executing the predefined query, to put the values into the EmpBals table.
                    MyCommand.CommandText = sQry;
                    lRAffected = MyCommand.ExecuteNonQuery();

                    if (lRAffected != 1)
                    {
                        // Rahul Start Edit 26-04-2011
                        ErrMsg = ApprView.ShowErrorMessage(ApprView.GetLanguageType(), "GEN010") + Environment.NewLine + ApprView.ShowErrorMessage(ApprView.GetLanguageType(), "GEN019");
                        // Rahul End Edit 26-04-2011
                        RetVal = false;
                        goto ExitTry;
                    }
                }

                RetVal = true;

            ExitTry:;
            }
            catch (Exception Ex)
            {
                RetVal = false;
                ErrMsg = Ex.Message;
            }

            return RetVal;
        }

        private static double GetNewJoinRPDBal(DateTime JoiningDate, DateTime EventSTDate, double dBal)
        {
            double RetVal;
            DateTime EventEndDate = new DateTime(1900, 1, 1);
            EventEndDate = EventSTDate.AddDays(Convert.ToInt32(dBal));
            RetVal = 0;
            if (JoiningDate <= EventEndDate)
                RetVal = EventEndDate.Subtract(JoiningDate).Days;
            if (RetVal >= dBal)
                RetVal = dBal;
            return RetVal;
        }
        public static bool UpdateNewEmpRPDHAJLeaveBal(ref CSApprovalData ApprData, byte byCallType, ref SqlConnection Conn, ref SqlCommand MyCommand, ref string ErrMsg)
        {
            bool RetVal = false;
            string sQry = string.Empty;
            string sSalProfile = string.Empty;
            try
            {
                DateTime HAJSTDT = new DateTime(1900, 1, 1);
                DateTime NATSTDT = new DateTime(1900, 1, 1);
                DateTime RMNSTDT = new DateTime(1900, 1, 1);
                DateTime FDSTDT = new DateTime(1900, 1, 1);
                DateTime dtJoinDate = new DateTime(1900, 1, 1);
                double dMaxHAJ, dMaxRMN, dMaxNAT,dMaxFD;
                dMaxHAJ = 0.0; dMaxNAT = 0.0; dMaxRMN = 0.0;dMaxFD = 0.0;
                long lRecordno= 0L;
                SqlDataReader MyReader;
                MyReader = null/* TODO Change to default(_) if this is not a reference type */;

                bool nSuccess = false;
                sQry = "select IsNull(joiningdate,'01/01/1900') joiningdate,RecordNo,IsNull(SalProfile,'') SalProfile   from FinMast with(nolock) where EmpID =" + ApprData.m_lEmpID + " AND IsNull(FinMast.TransferDate,'01/01/1900') = '01/01/1900';";
                sQry += "select IsNull(a.EventStart,'01/01/1900') EventStart,IsNull(a.Bal,'0') Bal,Isnull(SalProfile,'') as SalProfile  from RPDLeaves a with(nolock), LocLib1 b with(nolock) where a.ProfileCode = b.ProfileCode and b.Code = dbo.fun_GetLocLib1WithEmpID(" + ApprData.m_lEmpID + ") and EventCode ='RPDHAJ';";
                sQry += "select IsNull(a.EventStart,'01/01/1900') EventStart,IsNull(a.Bal,'0') Bal,Isnull(SalProfile,'') as SalProfile  from RPDLeaves a with(nolock), LocLib1 b with(nolock) where a.ProfileCode = b.ProfileCode and b.Code = dbo.fun_GetLocLib1WithEmpID(" + ApprData.m_lEmpID + ") and EventCode ='RPDNAT';";
                sQry += "select IsNull(a.EventStart,'01/01/1900') EventStart,IsNull(a.Bal,'0') Bal,Isnull(SalProfile,'') as SalProfile  from RPDLeaves a with(nolock), LocLib1 b with(nolock) where a.ProfileCode = b.ProfileCode and b.Code = dbo.fun_GetLocLib1WithEmpID(" + ApprData.m_lEmpID + ") and EventCode ='RPDRAM';";
                sQry += "select IsNull(a.EventStart,'01/01/1900') EventStart,IsNull(a.Bal,'0') Bal,Isnull(SalProfile,'') as SalProfile  from RPDLeaves a with(nolock), LocLib1 b with(nolock) where a.ProfileCode = b.ProfileCode and b.Code = dbo.fun_GetLocLib1WithEmpID(" + ApprData.m_lEmpID + ") and EventCode ='RPDFD';";
                RetVal = ConnectionFunctions.Connect_SQLDataReader(ref MyReader, sQry,ref ErrMsg, ref Conn);
                if (RetVal == false)
                {
                    ErrMsg = "Error in Retreiving balance";
                    RetVal = false;
                    goto ExitTry;
                }
                if (MyReader.HasRows)
                {
                    MyReader.Read();
                    dtJoinDate = Convert.ToDateTime(MyReader["joiningdate"]);
                    lRecordno = Convert.ToInt64(MyReader["RecordNo"]);
                    sSalProfile = Convert.ToString(MyReader["SalProfile"]);
                }
                MyReader.NextResult();
                if (MyReader.HasRows)
                {
                    MyReader.Read();
                    HAJSTDT = Convert.ToDateTime(MyReader["EventStart"]);
                    //if ((MyReader["SalProfile"].ToString().Contains("@" + sSalProfile + "@") || MyReader["SalProfile"].ToString() =="999@") && dtJoinDate <= HAJSTDT) 
                    //    dMaxHAJ = Convert.ToDouble(MyReader["Bal"]);
                    //Denson Commented above code and rewrite as below 20/09/2023
                    if ((MyReader["SalProfile"].ToString().Contains("@" + sSalProfile + "@") || MyReader["SalProfile"].ToString() == "999@"))
                        dMaxHAJ = GetNewJoinRPDBal(dtJoinDate, HAJSTDT, Convert.ToDouble(MyReader["Bal"]));
                }
                MyReader.NextResult();
                if (MyReader.HasRows)
                {
                    MyReader.Read();
                    NATSTDT = Convert.ToDateTime(MyReader["EventStart"]);
                    if ((MyReader["SalProfile"].ToString().Contains("@" + sSalProfile + "@") || MyReader["SalProfile"].ToString() == "999@"))
                        dMaxNAT = GetNewJoinRPDBal(dtJoinDate, NATSTDT, Convert.ToDouble(MyReader["Bal"]));
                }
                MyReader.NextResult();
                if (MyReader.HasRows)
                {
                    MyReader.Read();
                    RMNSTDT = Convert.ToDateTime(MyReader["EventStart"]);
                    if ((MyReader["SalProfile"].ToString().Contains("@" + sSalProfile + "@") || MyReader["SalProfile"].ToString() == "999@"))
                        dMaxRMN = GetNewJoinRPDBal(dtJoinDate, RMNSTDT, Convert.ToDouble(MyReader["Bal"]));
                }
                MyReader.NextResult();
                if (MyReader.HasRows)
                {
                    FDSTDT = Convert.ToDateTime(MyReader["EventStart"]);
                    if ((MyReader["SalProfile"].ToString().Contains("@" + sSalProfile + "@") || MyReader["SalProfile"].ToString() == "999@"))
                        dMaxFD = GetNewJoinRPDBal(dtJoinDate, FDSTDT, Convert.ToDouble(MyReader["Bal"]));
                }
                MyReader.Close();

                sQry = "Delete from EmpLeaveBal  where Empid = " + ApprData.m_lEmpID + " And  LeaveType = 'RPDHAJ' and RecordNo = " + lRecordno + ";";
                sQry += "Insert EmpLeaveBal(EmpID,LeaveType,Bal,RecordNo) Values(" + ApprData.m_lEmpID + ",'RPDHAJ'," + dMaxHAJ + "," + lRecordno + ")";

                MyCommand.CommandText = sQry;
                MyCommand.ExecuteNonQuery();
                sQry = "Delete from EmpLeaveBal  where Empid = " + ApprData.m_lEmpID + " And  LeaveType = 'RPDNAT' and RecordNo = " + lRecordno + ";";
                sQry += "Insert EmpLeaveBal(EmpID,LeaveType,Bal,RecordNo) Values(" + ApprData.m_lEmpID + ",'RPDNAT'," + dMaxNAT + "," + lRecordno + ")";

                MyCommand.CommandText = sQry;
                MyCommand.ExecuteNonQuery();
                sQry = "Delete from EmpLeaveBal  where Empid = " + ApprData.m_lEmpID + " And  LeaveType = 'RPDRAM' and RecordNo = " + lRecordno + ";";
                sQry += "Insert EmpLeaveBal(EmpID,LeaveType,Bal,RecordNo) Values(" + ApprData.m_lEmpID + ",'RPDRAM'," + dMaxRMN + "," + lRecordno + ")";
                MyCommand.CommandText = sQry;
                MyCommand.ExecuteNonQuery();
                sQry = "Delete from EmpLeaveBal  where Empid = " + ApprData.m_lEmpID + " And  LeaveType = 'RPDFD' and RecordNo = " + lRecordno + ";";
                sQry += "Insert EmpLeaveBal(EmpID,LeaveType,Bal,RecordNo) Values(" + ApprData.m_lEmpID + ",'RPDFD'," + dMaxFD + "," + lRecordno + ")";

                MyCommand.CommandText = sQry;
                MyCommand.ExecuteNonQuery();
                RetVal = true;

            ExitTry:;
            }
            catch (Exception Ex)
            {
                RetVal = false;
                ErrMsg = Ex.Message;
            }
            return RetVal;
        }

        public static bool UpdateNewEmpNextApprDueDate(ref CSApprovalData ApprData, byte byCallType, ref SqlConnection Conn, ref SqlCommand MyCommand, ref string ErrMsg)
        {
            bool RetVal = false;
            string sQry = string.Empty;
            try
            {

                // sQry = "Select Enabled From CSModules Where ViewNo = 410"
                // Dim bEnbl2 As String = String.Empty
                // Dim bEnbl As Boolean = False
                // RetVal = Connect_SQLScalar(bEnbl2, sQry, Nothing, Conn, ErrMsg)
                // 'If no PA module then return true from here itself. So that new joins financial can be approved / disapproved
                // 'If Boolean.TryParse(bEnbl2, bEnbl) = False Then
                // '    RetVal = True
                // '    Exit Try
                // 'End If
                // If bEnbl2 = Nothing Then 'srinivasan Added 25/06/2010
                // RetVal = True
                // Exit Try
                // End If
                // If bEnbl2 = "False" Then 'Dhanesh Added 13/04/2010
                // RetVal = True
                // Exit Try
                // End If

                DateTime ApprDueDate = new DateTime(1900, 1, 1);
                bool nSuccess = false;

                if (byCallType == 1)
                {
                    sQry = "Update Employee Set LocLIb1 = dbo.fun_GetLocLib1WithEmpID(Empid) where Empid = " + ApprData.m_lEmpID;
                    MyCommand.CommandText = sQry;
                    MyCommand.ExecuteNonQuery();

                    nSuccess = GetDueApprDate(ref ApprData.m_lEmpID, ref ApprDueDate, 1, ref Conn, ref ErrMsg); // 1 for Ist due date when Ist financial is approved....
                    if (nSuccess == false)
                    {
                        RetVal = false;
                        goto ExitTry;
                    }

                    sQry = "UPDATE Employee SET LastAppraisalDate = NextAppraisalDate WHERE EmpID = " + ApprData.m_lEmpID;
                    MyCommand.CommandText = sQry;
                    MyCommand.ExecuteNonQuery();

                    sQry = "UPDATE Employee SET NextAppraisalDate = '" + ApprDueDate.ToString("yyyy/MM/dd") + "' WHERE EmpID = " + ApprData.m_lEmpID;
                    MyCommand.CommandText = sQry;
                    MyCommand.ExecuteNonQuery();
                }
                else if (byCallType == 2)
                {
                    sQry = "UPDATE Employee SET LastAppraisalDate = '1/1/1900',  NextAppraisalDate = '1/1/1900' WHERE EmpID = " + ApprData.m_lEmpID;
                    MyCommand.CommandText = sQry;
                    MyCommand.ExecuteNonQuery();
                }

                RetVal = true;

            ExitTry:;
            }
            catch (Exception Ex)
            {
                RetVal = false;
                ErrMsg = Ex.Message;
            }

            return RetVal;
        }


        protected static bool GetDueApprDate(ref int lEmpID, ref DateTime ApprDueDate, byte nFlag, ref SqlConnection Conn, ref string ErrMsg)
        {
            bool RetVal = false;
            SqlDataReader MyReader = null/* TODO Change to default(_) if this is not a reference type */;
            try
            {
                string strQuery = string.Empty;
                string LocLib1 = string.Empty;
                DateTime JoiningDate = new DateTime(1900, 1, 1);
                DateTime LastAppraisalDate = new DateTime(1900, 1, 1);
                DateTime NextAppraisalDate = new DateTime(1900, 1, 1);
                DateTime ProbEndDt = new DateTime(1900, 1, 1);
                short NoOfApprPROB = 0;
                short SMonth = 0;
                short NoOfApprPerYear = 0;
                short DiffRegProbAppr = 0;
                bool RegEmpFlag = false;
                long lProbationPeriod = 0L;
                short month = 0;
                short year = 0;
                int nNextApprMon = 0;
                string qryRslt = "";
                SqlParameter[] Params = null;

                strQuery = "SELECT Employee.EmpID, Employee.LastAppraisalDate, Employee.NextAppraisalDate, Employee.LocLib1, ";
                strQuery += "FinMast.JoiningDate,HRPADueDateSetup.NoOfApprPROB,HRPADueDateSetup.RegEmpFlag,HRPADueDateSetup.SMonth,";
                strQuery += "HRPADueDateSetup.NoOfApprPerYear, HRPADueDateSetup.DiffRegProbAppr ";
                strQuery += "FROM Employee WITH (NOLOCK) INNER JOIN FinMast WITH (NOLOCK)  ON Employee.EmpID = FinMast.EmpID INNER JOIN ";
                strQuery += "HRPADueDateSetup WITH (NOLOCK)  ON dbo.fun_GetFirstLocation(Employee.LocLib5) = HRPADueDateSetup.LocLib1 WHERE (Employee.EmpID = " + lEmpID + " AND IsNull(FinMast.TransferDate,'01/01/1900') = '01/01/1900')";
                RetVal = ConnectionFunctions.Connect_SQLDataReader(ref MyReader, strQuery, ref ErrMsg, ref Conn);
                if (RetVal == false)
                    goto ExitTry;

                if (MyReader.HasRows)
                {
                    MyReader.Read();
                    LastAppraisalDate = (MyReader[1]==DBNull.Value? new DateTime(1900, 1, 1): Convert.ToDateTime(MyReader[1])); // Last Date When the Employee was Given Appairsal or Evaluated
                    NextAppraisalDate = (MyReader[2] == DBNull.Value ? new DateTime(1900, 1, 1) : Convert.ToDateTime(MyReader[2])); // Next Date When the employee has to be evaluated (No Used here)
                    LocLib1 = (MyReader[3] == DBNull.Value ? "" : MyReader[3].ToString()); // Comapny Code
                    JoiningDate = Convert.ToDateTime(MyReader[4]); // Joining Data
                    NoOfApprPROB = Convert.ToInt16(MyReader[5]); // No Of Appraisals in Prabation period
                    RegEmpFlag = Convert.ToBoolean(MyReader[6]);  // 1 - Fixed   0 - As per Jpoinig date
                    SMonth = Convert.ToInt16(MyReader[7]); // Starting month for Appraisal
                    if (SMonth == 5)
                        SMonth += 1;
                    NoOfApprPerYear = Convert.ToInt16(MyReader[8]); // No of Appraisals in a year for a regular employee (1,2,3,4,6)
                    DiffRegProbAppr = (MyReader[9] == DBNull.Value ? (short)0 : Convert.ToInt16(MyReader[9])); // Semi-Regular status of employee....
                }
                else
                {
                    // Rahul Start Edit 26-04-2011
                    ErrMsg = ApprView.ShowErrorMessage(ApprView.GetLanguageType(), "GEN012");
                    // Rahul End Edit 26-04-2011
                    RetVal = false;
                    goto ExitTry;
                }
                MyReader.Close();

                lProbationPeriod = 0;
                strQuery = "SELECT ProbPeriod FROM SalaryProfile WHERE (Code = (SELECT  SalProfile FROM FinMast WITH (NOLOCK) WHERE EmpID =  " + lEmpID + " AND IsNull(TransferDate,'01/01/1900') = '01/01/1900'))";
                RetVal = ConnectionFunctions.Connect_SQLScalar(ref qryRslt, strQuery, ref Params, ref Conn, ref ErrMsg);
                
                if (RetVal == false)
                    goto ExitTry;

                long.TryParse(qryRslt, out lProbationPeriod);

                // Last Appraisal for the regular employee should not be '1/1/1900' as next Appraisal Due date is calculated on the basis of Last appraisal date 
                // It should be >= (Joining Date + Probation Period) for new joined or under probation there is no problem as it is calculated on the basis of joining date
                if (nFlag == 1)
                {
                    nNextApprMon = Convert.ToInt32(lProbationPeriod / (double)NoOfApprPROB);
                    // strQuery = "SELECT DATEADD(dd," & nNextApprMon & ",'" & JoiningDate.ToString("yyyy/MM/dd") & "') AS FirstDueDate"
                    ApprDueDate = JoiningDate.AddDays(nNextApprMon);
                    RetVal = true;
                    goto ExitTry;
                }

                // Here after getting 1st date, lastapprdate will still have the 1/1/1900...now if nextapprdate is still under prbn means employee is in probn...
                decimal diff = NextAppraisalDate.Subtract(JoiningDate).Days;
                DateTime dtSemiRegDate = NextAppraisalDate.AddDays(DiffRegProbAppr);

                month = (short)NextAppraisalDate.Month;
                year = (short)NextAppraisalDate.Year;

                if (System.Convert.ToInt64(diff) >= lProbationPeriod)
                {
                    nNextApprMon = 0;
                    if (RegEmpFlag)
                    {
                        int counter = 0;
                        if (year <= 1900)
                        {
                            // Rahul Start Edit 26-04-2011
                            ErrMsg = ApprView.ShowErrorMessage(ApprView.GetLanguageType(), "GEN013");
                            // Rahul End Edit 26-04-2011
                            RetVal = false;
                            goto ExitTry;
                        }
                        while (true)
                        {
                            nNextApprMon = Convert.ToInt32(((12 / (double)NoOfApprPerYear) * counter) + SMonth);
                            if (nNextApprMon >= 12)
                            {
                                nNextApprMon -= 12;
                                year += 1;
                            }
                            ApprDueDate = new DateTime(year, nNextApprMon, 1);
                            if (ApprDueDate > dtSemiRegDate)
                                break;
                            counter += 1;
                        }
                    }
                    else
                    {
                        short counter = 1;
                        while (true)
                        {
                            nNextApprMon = Convert.ToInt32(((12 / (double)NoOfApprPerYear) * counter));
                            ApprDueDate = JoiningDate.AddDays(nNextApprMon);
                            if (ApprDueDate > dtSemiRegDate)
                                break;
                            counter += 1;
                        }
                    }
                }
                else
                {
                    int counter = 1;
                    while (true)
                    {
                        nNextApprMon = Convert.ToInt32(((lProbationPeriod / (double)NoOfApprPROB) * counter));
                        ApprDueDate = JoiningDate.AddDays(nNextApprMon);
                        if (ApprDueDate > NextAppraisalDate)
                            break;
                        counter += 1;
                    }
                }

                RetVal = true;

                ExitTry:;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
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


        public static bool UpdateBonusFormulaAmount(ref CSApprovalData ApprData, ref SqlConnection Conn, ref SqlCommand MyCommand, ref string ErrMsg)
        {
            bool RetVal = false;
            try
            {
                string sQry = string.Empty;
                int lRAffected = 0;
                int lCount = 0;
                string qryrslt = "";
                SqlParameter[] Params = null;

                SqlDataReader MyReader = null/* TODO Change to default(_) if this is not a reference type */;
                sQry = "SELECT ISNULL(Formula,'') Formula,ReqNo  FROM BonusSetup WHERE STATUS<=20 AND BonusPayType = 3 AND EmpId = " + ApprData.m_lEmpID.ToString();
                if ((ConnectionFunctions.Connect_SQLDataReader(ref MyReader, sQry, ref ErrMsg)))
                {
                    if ((MyReader.HasRows))
                    {
                        while ((MyReader.Read()))
                        {
                            int reqNo = 0;
                            string formula = "";
                            decimal amount = 0;
                            reqNo = Convert.ToInt32(MyReader["ReqNo"]);
                            formula = MyReader["Formula"].ToString();

                            if ((string.IsNullOrEmpty(formula)))
                            {
                                ErrMsg = "Unable to Calculate Bonus amount as formula is blank";
                                RetVal = false;
                                break;
                            }
                            sQry = "Select (" + formula + ") as CalculatedAmount FROM FinMast WITH (NOLOCK) WHERE EmpId = " + ApprData.m_lEmpID;
                            if ((!ConnectionFunctions.Connect_SQLScalar(ref qryrslt, sQry, ref Params, ref Conn, ref ErrMsg)))
                            {
                                ErrMsg = "Error Occured While Calculating Bonus Amount Details" + ErrMsg;
                                RetVal = false;
                                break;
                            }
                            decimal.TryParse(qryrslt, out amount);
                            if ((amount > 0))
                            {
                                string amountStr = "";
                                amountStr = Utility.General.Round(ApprData.m_lEmpID, amount, Conn.ConnectionString).ToString();
                                sQry = "UPDATE BonusSetup SET  FixAmount = '" + amountStr + "' WHERE STATUS<=20 AND BonusPayType = 3 AND EmpId = " + ApprData.m_lEmpID.ToString() + " AND  ReqNo = " + reqNo.ToString();
                                if ((!ConnectionFunctions.Connect_SQLNonQuery(ref lRAffected, sQry, ref ErrMsg)))
                                {
                                    ErrMsg = "Error Occured updating  calculated Bonus Amount. Details" + ErrMsg;
                                    RetVal = false;
                                    break;
                                }
                            }
                        }

                        MyReader.Close();
                    }
                }
                MyReader = null/* TODO Change to default(_) if this is not a reference type */;
                sQry = "SELECT ISNULL(Element,'') Element,ReqNo  FROM BonusSetup WHERE STATUS<=20 AND BonusPayType = 5 AND EmpId = " + ApprData.m_lEmpID.ToString();
                if ((ConnectionFunctions.Connect_SQLDataReader(ref MyReader, sQry, ref ErrMsg)))
                {
                    if ((MyReader.HasRows))
                    {
                        while ((MyReader.Read()))
                        {
                            int reqNo = 0;
                            string element = "";
                            decimal amount = 0;
                            reqNo = Convert.ToInt32(MyReader["ReqNo"]);
                            element = MyReader["Element"].ToString();
                            if ((element.ToUpper() == "GROSSSALARY"))
                                element = "TotalInBasicCurr";
                            if ((string.IsNullOrEmpty(element)))
                            {
                                ErrMsg = "Unable to Calculate Bonus amount as element is blank";
                                RetVal = false;
                                break;
                            }
                            sQry = "Select " + element + " as CalculatedAmount FROM FinMast WITH (NOLOCK) WHERE EmpId = " + ApprData.m_lEmpID;
                            if ((!ConnectionFunctions.Connect_SQLScalar(ref qryrslt, sQry, ref Params, ref Conn, ref ErrMsg)))
                            {
                                ErrMsg = "Error Occured While Calculating Bonus Amount Details" + ErrMsg;
                                RetVal = false;
                                break;
                            }
                            decimal.TryParse(qryrslt, out amount);
                            if ((amount > 0))
                            {
                                string amountStr = "";
                                amountStr = Utility.General.Round(ApprData.m_lEmpID, amount, Conn.ConnectionString).ToString();
                                sQry = "UPDATE BonusSetup SET  FixAmount = '" + amountStr + "' WHERE STATUS<=20 AND BonusPayType = 5 AND EmpId = " + ApprData.m_lEmpID.ToString() + " AND  ReqNo = " + reqNo.ToString();
                                if ((!ConnectionFunctions.Connect_SQLNonQuery(ref lRAffected, sQry, ref ErrMsg)))
                                {
                                    ErrMsg = "Error Occured updating  calculated Bonus Amount. Details" + ErrMsg;
                                    RetVal = false;
                                    break;
                                }
                            }
                        }

                        MyReader.Close();
                    }
                }
                RetVal = true;
            }

            catch (Exception ex)
            {
                RetVal = false;
                ErrMsg = ex.Message;
            }
            return RetVal;
        }

        public static bool UpdatePMSOrgnsnHierarchy(bool bAppFlag, int empid, int reqNo, bool isFromLocChng, string sLocQry, ref SqlCommand MyCommand, ref SqlConnection Conn, ref string ErrMsg)
        {
            string sQry;
            bool RetVal = false;
            string s;
            int iOldOpt = 0;
            int iNewOpt = 0;
            int iNewMgrPosID = 0;
            int iEmpPosID = 0;
            int iOldMgrPosID = 0;
            bool isMovePos = false;
            DateTime dtptodate;
            int iActOldMgrPosID = 0;
            SqlDataReader myReader = null/* TODO Change to default(_) if this is not a reference type */;
            int lRAffected;

            string qryRslt = "";
            SqlParameter[] Params = null;
            try
            {
                if (bAppFlag == false)
                {
                    sQry = "";
                    sQry = "Select SrNo,EmpID,ReqDate,OldOpt,NewOpt,MgrPosID,ReqID,EmpPosID,OldMgrPosID,PtoDate,IsMovePos,LastModDateTime  FROM  PMS_CmpnyHrchySetup WHERE  SrNo = " + reqNo + " AND EmpID =" + empid;
                    RetVal = ConnectionFunctions.Connect_SQLDataReader(ref myReader, sQry, ref ErrMsg, ref Conn);
                    if (RetVal == false)
                        goto ExitTry;

                    if (myReader.HasRows == true)
                    {
                        myReader.Read();
                        iOldOpt = Convert.ToInt32(myReader["OldOpt"]);
                        iNewOpt = Convert.ToInt32(myReader["NewOpt"]);
                        iNewMgrPosID = Convert.ToInt32(myReader["MgrPosID"]);
                        iEmpPosID = Convert.ToInt32(myReader["EmpPosID"]);
                        iOldMgrPosID = Convert.ToInt32(myReader["OldMgrPosID"]);
                        dtptodate = Convert.ToDateTime(myReader["PtoDate"]);
                        isMovePos = Convert.ToBoolean(myReader["IsMovePos"]);
                    }
                    else
                    {
                        ErrMsg = "No record found in the table PMS_CmpnyHrchySetup";
                        goto ExitTry;
                    }
                    myReader.Close();

                    if (isFromLocChng == true)
                    {
                        if (isMovePos == true)
                        {
                            sQry = "";
                            sQry = "Update PMS_OrgStruct SET " + sLocQry + ", MgrID = " + iNewMgrPosID + " WHERE posID IN(SELECT posID FROM dbo.PMS_OrgHist WHERE Empid = " + empid + " AND '" + dtptodate.ToString("yyyy/MM/dd") + "' BETWEEN FROMDATE AND TODATE)";
                            MyCommand.CommandText = sQry;
                            lRAffected = MyCommand.ExecuteNonQuery();

                            if (lRAffected <= 0)
                            {
                                RetVal = false;
                                ErrMsg = ApprView.ShowErrorMessage(ApprView.GetLanguageType(), "GEN038");
                                goto ExitTry;
                            }
                            return true;
                        }
                    }

                    if (iEmpPosID <= 0)
                    {
                        // ErrMsg = "Error While updating  PMS Organisation  Hierarchy.!! \n Position doesnt exists"
                        ErrMsg = ApprView.ShowErrorMessage(ApprView.GetLanguageType(), "PMS03");
                        RetVal = false;
                        return false;
                        goto ExitTry;
                    }

                    int iCnt = 0;
                    sQry = "";
                    sQry = "SELECT Count(EmpID) AS CNT FROM PMS_ORGHIST WHERE   PosID = " + iEmpPosID + " AND '" + dtptodate.ToString("yyyy/MM/dd") + "' BETWEEN  FromDate AND ToDate";
                    RetVal = ConnectionFunctions.Connect_SQLScalar(ref qryRslt, sQry, ref Params, ref Conn, ref ErrMsg);

                    int.TryParse(qryRslt, out iCnt);

                    if (iCnt > 0)
                    {
                        string locLevel = "";
                        sQry = "";
                        // #NEWLOC
                        // sQry = "Select HierarchyLevel FROM LocLibSetUp"
                        // RetVal = Connect_SQLScalar(locLevel, sQry, Nothing, Conn, ErrMsg)

                        locLevel = "5";
                        sQry = "";
                        sQry = "SELECT  Loclib5,JobTitle from Finmast WITH (NOLOCK) where empid = " + empid;

                        string sLastLoc = "";
                        string sJobtitle = "";

                        RetVal = ConnectionFunctions.Connect_SQLDataReader(ref myReader, sQry, ref ErrMsg, ref Conn);
                        if (RetVal == false)
                            goto ExitTry;

                        if (myReader.HasRows == true)
                        {
                            myReader.Read();
                            sLastLoc = myReader[0].ToString();
                            sJobtitle = myReader[1].ToString();
                            myReader.Close();
                        }

                        sQry = "";

                        sQry = "SELECT TOP 1 POSID  FROM PMS_OrgStruct WHERE Loclib" + locLevel + " = '" + sLastLoc + "' AND JobTitle ='" + sJobtitle + "' AND POSID NOT IN " + " (SELECT EmpPosID FROM dbo.PMS_CmpnyHrchySetup p JOIN  PMS_OrgHist H On p.EmpPosid= h.PosID WHERE '" + dtptodate.ToString("yyyy/MM/dd") + "' BETWEEN FromDate AND ToDate)" + " AND PosID not in (SELECT EmpPosID FROM dbo.PMS_CmpnyHrchySetup where" + " EmpPosID not  in (SELECT  PosID From PMS_OrgHist)) AND Closed = 0 ORDER BY PosID";

                        RetVal = ConnectionFunctions.Connect_SQLDataReader(ref myReader, sQry, ref ErrMsg, ref Conn);
                        if (RetVal == false)
                            goto ExitTry;

                        if (myReader.HasRows == true)
                        {
                            myReader.Read();
                            iEmpPosID = Convert.ToInt32(myReader[0]);
                            sQry = "";
                            sQry = "Update PMS_CmpnyHrchySetup set EmpPosid = " + iEmpPosID + " WHERE SrNo = " + reqNo + " AND EmpID = " + empid;
                            MyCommand.CommandText = sQry;
                            lRAffected = MyCommand.ExecuteNonQuery();

                            if (lRAffected != 1)
                            {
                                RetVal = false;
                                ErrMsg = ApprView.ShowErrorMessage(ApprView.GetLanguageType(), "GEN068");
                                goto ExitTry;
                            }
                            myReader.Close();
                        }
                        else
                        {
                            sQry = "";
                            sQry = "SELECT  dbo.GetCodeName(p.EmpID) As Name ,PosID As POSITION ,JobTitle,ManagerId AS Manager ,Fromdate,ToDate FROM PMS_ORGHIST P JOIN " + "PMS_Employee_Manager PM  ON pm.EmpID = p.Empid WHERE PosID = " + iEmpPosID + " AND '" + dtptodate.ToString("yyyy/MM/dd") + "' BETWEEN  FromDate AND ToDate";

                            myReader.Close();
                            RetVal = ConnectionFunctions.Connect_SQLDataReader(ref myReader, sQry, ref ErrMsg, ref Conn);
                            if (RetVal == false)
                                goto ExitTry;

                            string Name = "";
                            string position = "" ;
                            if (myReader.HasRows == true)
                            {
                                myReader.Read();
                                Name = myReader["Name"].ToString();
                                position = myReader["POSITION"].ToString();
                            }
                            myReader.Close();

                            ErrMsg = "Error While updating  PMS Organisation  Hierarchy.!! " + Environment.NewLine + "There is already Employee(s) Assigned to this Position " + position;
                            RetVal = false;
                            return false;
                            goto ExitTry;
                        }
                    }

                    sQry = "";
                    sQry = "SELECT COUNT(1) AS CNT FROM PMS_OrgStruct WHERE PosID = " + iEmpPosID + " AND Closed = 0";
                    RetVal = ConnectionFunctions.Connect_SQLScalar(ref qryRslt, sQry, ref Params, ref Conn, ref ErrMsg);

                    int.TryParse(qryRslt, out iCnt);

                    if (iCnt == 0)
                    {
                        ErrMsg = "NOT FOUND ..Please Disapprove the Record and post again..!";
                        RetVal = false;
                        return false;
                        goto ExitTry; 
                    }

                    DateTime dtFromdate = dtptodate;
                    DateTime dtTodate;
                    dtTodate = new DateTime(2099,12,31);

                    sQry = "";
                    sQry = "INSERT INTO PMS_ORGHIST( PosID,EmpID,FromDate,ToDate,Acting) VALUES (" + iEmpPosID + "," + empid + ",'" + dtFromdate.ToString("yyyy/MM/dd") + "','" + dtTodate.ToString("yyyy/MM/dd") + "', 0)";
                    MyCommand.CommandText = sQry;
                    lRAffected = MyCommand.ExecuteNonQuery();
                    if (lRAffected <= 0)
                    {
                        RetVal = false;
                        ErrMsg = ApprView.ShowErrorMessage(ApprView.GetLanguageType(), "GEN068");
                        goto ExitTry;
                    }

                    sQry = "";
                    sQry = "UPDATE  PMS_ORGHIST SET ToDate = DATEADD (dd,-1,'" + dtFromdate.ToString("yyyy/MM/dd") + "') WHERE PosID = " + iOldMgrPosID + " AND EmpID = " + empid + " AND ToDate = '" + dtFromdate + "'";
                    MyCommand.CommandText = sQry;
                    MyCommand.ExecuteNonQuery();

                    sQry = "";
                    sQry = "UPDATE PMS_OrgStruct SET MgrID = " + iNewMgrPosID + " WHERE  PosID = " + iEmpPosID;
                    MyCommand.CommandText = sQry;
                    MyCommand.ExecuteNonQuery();
                }
                else
                {
                    sQry = "";
                    sQry = "Select SrNo,EmpID,ReqDate,OldOpt,NewOpt,MgrPosID,ReqID,EmpPosID,OldMgrPosID,PtoDate,IsMovePos,LastModDateTime,ActOldMgrPosID FROM  PMS_CmpnyHrchySetup WHERE  SrNo = " + reqNo + " AND EmpID =" + empid;
                    RetVal = ConnectionFunctions.Connect_SQLDataReader(ref myReader, sQry, ref ErrMsg, ref Conn);
                    if (RetVal == false)
                        goto ExitTry;

                    if (myReader.HasRows == true)
                    {
                        myReader.Read();
                        iOldOpt = Convert.ToInt32(myReader["OldOpt"]);
                        iNewOpt = Convert.ToInt32(myReader["NewOpt"]);
                        iNewMgrPosID = Convert.ToInt32(myReader["MgrPosID"]);
                        iEmpPosID = Convert.ToInt32(myReader["EmpPosID"]);
                        iOldMgrPosID = Convert.ToInt32(myReader["OldMgrPosID"]);
                        dtptodate = Convert.ToDateTime(myReader["PtoDate"]);
                        isMovePos = Convert.ToBoolean(myReader["IsMovePos"]);
                        iActOldMgrPosID = Convert.ToInt32(myReader["ActOldMgrPosID"]);
                    }
                    else
                    {
                        ErrMsg = "No record found in the table PMS_CmpnyHrchySetup";
                        RetVal = false;
                        goto ExitTry;
                    }
                    myReader.Close();

                    if (isFromLocChng == true)
                    {
                        if (isMovePos == true)
                        {
                            // sQry = "Update PMS_OrgStruct SET " & sLocQry & ", MgrID = " & iOldMgrPosID & " WHERE posID IN(SELECT posID FROM dbo.PMS_OrgHist WHERE Empid = " & empid & " AND '" & dtptodate.ToString("yyyy/MM/dd") & "' BETWEEN FROMDATE AND TODATE)"
                            sQry = "Update PMS_OrgStruct SET " + sLocQry + ", MgrID = " + iActOldMgrPosID + " WHERE posID IN(SELECT posID FROM dbo.PMS_OrgHist WHERE Empid = " + empid + " AND '" + dtptodate.ToString("yyyy/MM/dd") + "' BETWEEN FROMDATE AND TODATE)";
                            MyCommand.CommandText = sQry;
                            lRAffected = MyCommand.ExecuteNonQuery();

                            if (lRAffected <= 0)
                            {
                                RetVal = false;
                                ErrMsg = ApprView.ShowErrorMessage(ApprView.GetLanguageType(), "GEN068");
                                goto ExitTry;
                            }
                            return true;
                        }
                    }

                    sQry = "";
                    int iCnt = 0;
                    sQry = "SELECT Count(EmpID) AS CNT FROM PMS_ORGHIST WHERE   PosID = " + iOldMgrPosID + " AND '" + dtptodate.ToString("yyyy/MM/dd") + "' BETWEEN  FromDate AND ToDate";
                    RetVal = ConnectionFunctions.Connect_SQLScalar(ref qryRslt, sQry, ref Params, ref Conn, ref ErrMsg);

                    int.TryParse(qryRslt, out iCnt);
                    if (iCnt > 0)
                    {
                        ErrMsg = "Error While updating  PMS Organisation  Hierarchy.!! " + Environment.NewLine + " There is already Employee(s) Assigned to this Position";
                        RetVal = false;
                        goto ExitTry;
                    }

                    DateTime dtFromdate = dtptodate;
                    DateTime dtTodate;
                    dtTodate = new DateTime(2099, 12, 31);

                    sQry = "";
                    sQry = "DELETE FROM  PMS_ORGHIST WHERE PosID =" + iEmpPosID + " AND EmpID = " + empid + " AND FromDate = '" + dtFromdate.ToString("yyyy/MM/dd") + "' AND ToDate = '" + dtTodate.ToString("yyyy/MM/dd") + "'";
                    MyCommand.CommandText = sQry;
                    MyCommand.ExecuteNonQuery();

                    DateTime dtTemp = dtFromdate.AddDays(-1);
                    sQry = "";
                    sQry = "UPDATE  PMS_ORGHIST SET ToDate = '" + dtTodate + "' WHERE PosID = " + iOldMgrPosID + " AND EmpID = " + empid + " AND ToDate = '" + dtTemp + "'";
                    MyCommand.CommandText = sQry;
                    MyCommand.ExecuteNonQuery();
                }



            ExitTry:;
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (myReader != null)
                {
                    if (!myReader.IsClosed)
                        myReader.Close();
                }
            }

            return RetVal;
        }

        public static bool UpdFinMastFrmFinChng(byte byType, ref CSApprovalData ApprData, ref string[] UserInfo, ref SqlConnection Conn, ref SqlCommand MyCommand, ref string ErrMsg)
        {
            bool RetVal = false;
            SqlDataReader MyReader = null/* TODO Change to default(_) if this is not a reference type */;
            SqlDataReader MyReader1 = null/* TODO Change to default(_) if this is not a reference type */;
            SqlDataReader MyReader2 = null/* TODO Change to default(_) if this is not a reference type */;

            try
            {

                // SRINI NOTE (CODE MISSING) added beloW code

                string sQry;
                string vRowsAff;
                string var;
                string dt_NextMth;
                long lCnt = 0L;
                string tableColoumStatus;
                DateTime sVal = new DateTime(1900, 1, 1);

                // Rahul Start Edit 27-06-2012 Annual Leave Type
                DateTime dtLastPaid = new DateTime(1900, 1, 1);
                string sSalProf = string.Empty;
                string sWorkComp = string.Empty;
                DateTime dtCurr = new DateTime(1900, 1, 1);
                bool bUpdated = false;
                int lStatus = 0;
                // Rahul End Edit 27-06-2012 Annual Leave Type

                string qryRslt = "";
                SqlParameter[] Params = null;

                tableColoumStatus = Common.GetTableStatus(ref ErrMsg);
                sQry = "SELECT DateAdd(dd,1,LastPaidDate) As Dt,LastPaidDate,SalProfile, Status FROM Finmast WITH (NOLOCK) WHERE EmpID = " + ApprData.m_lEmpID + " AND IsNull(Transferdate,'01/01/1900') = '01/01/1900'";

                RetVal = ConnectionFunctions.Connect_SQLDataReader(ref MyReader1, sQry, ref ErrMsg);

                if (MyReader1.HasRows)
                {
                    MyReader1.Read();
                    sVal = (MyReader1[0] == DBNull.Value ? new DateTime(1900, 1, 1) : Convert.ToDateTime(MyReader1[0]));
                    dtLastPaid = (MyReader1[1] == DBNull.Value ? new DateTime(1900, 1, 1) : Convert.ToDateTime(MyReader1[1]));
                    sSalProf = (MyReader1[2] == DBNull.Value ? "0" : MyReader1[2].ToString());
                    lStatus = Convert.ToInt32(MyReader1[3] == DBNull.Value ? 0 : MyReader1[3]);
                }
                MyReader1.Close();

                // Nishad Added 28042015 --Status Checking of FinMast
                if (lStatus > 20)
                {
                    // dt_NextMth = sVal.Year & "/" & sVal.Month & "/" & GetLastDayOfMonth(sVal.Year, sVal.Month)
                    dt_NextMth = sVal.Year + "/" + sVal.Month + "/" + Utility.General.GetLastDayOfMonth( sVal.Month,sVal.Year);
                    // dt_NextMth.SetDate(dt_NextMth.GetYear(),dt_NextMth.GetMonth(),CGeneral::GetLastDayOfMonth (dt_NextMth.GetMonth (), dt_NextMth.GetYear ()));
                    // sQry.Format(_T("SELECT Case When DateAdd(mm,1,LastPaidDate) < Effectivedate THEN 1 ELSE 0 END As Cnt FROM FinMast,FinReqMast WHERE Finmast.EmpID = FinReqMast.EmpID AND FinReqMast.[SrNo] = %ld"),ApprData.m_lReqNo );
                    sQry = "SELECT Case When '" + dt_NextMth + "' < Effectivedate THEN 1 ELSE 0 END As Cnt FROM FinMast WITH (NOLOCK) ,FinReqMast WITH (NOLOCK) WHERE Finmast.EmpID = FinReqMast.EmpID AND FinReqMast.[SrNo] = " + ApprData.m_lReqNo + "";
                    RetVal = ConnectionFunctions.Connect_SQLScalar(ref qryRslt, sQry, ref ErrMsg);

                    if (RetVal == false)
                        // Rahul Start Edit 27-04-2011
                        ErrMsg = ApprView.ShowErrorMessage(ApprView.GetLanguageType(), "GEN009");

                    long.TryParse(qryRslt, out lCnt);

                    if ((byType == 0 & lCnt == 1))
                    {
                        int Rslt = 0;
                        sQry = "Update Finreqmast Set Status = 45,ActiveStatus = 45 WHERE SrNo = " + ApprData.m_lReqNo + "";
                        //ConnectionFunctions.Connect_SQLNonQuery(Rslt, sQry, ErrMsg)     'Nishad Commented on 18122013 as this function was returning error
                        RetVal = ConnectionFunctions.Connect_SQLNonQuery(ref Rslt, sQry, ref ErrMsg, Params, ref Conn);
                        sQry = "Update FinChanges Set Status = 45,ActiveStatus = 45 WHERE SrNo = " + ApprData.m_lReqNo + "";
                        //ConnectionFunctions.Connect_SQLNonQuery(Rslt, sQry, ErrMsg)        'Nishad Commented on 18122013 as this function was returning error
                        RetVal = ConnectionFunctions.Connect_SQLNonQuery(ref Rslt, sQry, ref ErrMsg, Params, ref Conn);
                        return true;
                    }
                }

                string szElement = string.Empty;
                string szCodeFrom = string.Empty;
                string szCodeTo = string.Empty;
                string szValue = string.Empty;

                // till this

                string szValue2 = string.Empty;
                string sElemName = string.Empty;
                DateTime dtEff = new DateTime(1900, 1, 1);
                long lReqNo = 0L;
                bool bTktChk = false;
                string szFromVal = string.Empty;
                string szToVal = string.Empty;

                // Retrieving the values to be updated from the FinChange table.
                sQry = "SELECT Element,CodeFrom,CodeTo,ReqNo,Effectivedate FROM FinChanges WITH (NOLOCK),FinReqMast WITH (NOLOCK) WHERE Finchanges.Srno = Finreqmast.Srno AND FinreqMast.[SrNo] = " + ApprData.m_lReqNo;

                RetVal = ConnectionFunctions.Connect_SQLDataReader(ref MyReader, sQry, ref ErrMsg);
                if (RetVal == false)
                    goto ExitTry;

                // Rahul Start Edit 27-06-2012 Annual Leave Type
                RetVal = ConnectionFunctions.Connect_SQLScalar(ref qryRslt, "SELECT GETDATE()", ref Params, ref Conn, ref ErrMsg);
                if (RetVal == false)
                    dtCurr = DateTime.Now.Date;
                else
                    DateTime.TryParse(qryRslt, out dtCurr);

                sWorkComp = GetWorkingComp(ApprData.m_lEmpID);
                // Rahul End Edit 27-06-2012 Annual Leave Type

                while (MyReader.Read())
                {
                    szElement = (MyReader["Element"] == DBNull.Value ? string.Empty : MyReader["Element"].ToString());
                    szCodeFrom = (MyReader["CodeFrom"] == DBNull.Value ? string.Empty : MyReader["CodeFrom"].ToString());
                    szCodeTo = (MyReader["CodeTo"] == DBNull.Value ? string.Empty : MyReader["CodeTo"].ToString());

                    lReqNo = Convert.ToInt64(MyReader["ReqNo"]);
                    dtEff = (MyReader["EffectiveDate"] == DBNull.Value ? new DateTime(1900, 1, 1) : Convert.ToDateTime(MyReader["EffectiveDate"]));

                    // Setting the value to the database.
                    if (szElement != "Salary Profile")
                    {
                        if (byType == 0)
                            szValue = szCodeTo;
                        else if (byType == 1)
                            szValue = szCodeFrom;
                    }
                    else
                        // Since we need to store the previous as well as the current salary profile
                        if (byType == 0)
                    {
                        szValue = szCodeTo;
                        szValue2 = szCodeFrom;
                    }
                    else if (byType == 1)
                    {
                        // confirmed from anil about this case, setting both the SP's as Previous on disapproval
                        szValue = szCodeFrom;
                        szValue2 = szCodeFrom;
                    }

                    // Checking which Element is to be updated
                    if (szElement == "Employee Code")
                        sElemName = "EmpCode = '" + szValue + "'";
                    else if (szElement == "Employee Name")
                        sElemName = "EmpNameE = '" + szValue + "'";
                    else if (szElement == "Location")
                    {
                        string sTmp1 = "";
                        string sTmp2 = "";



                        // #NEWLOC
                        // For nCtr As Integer = 1 To CInt(UserInfo.GetValue(APPR.HierarchyLevel))
                        // If String.IsNullOrEmpty(sTmp2) = False Then
                        // sTmp2 &= ", "
                        // End If
                        // sTmp1 = "Code" & nCtr
                        // sTmp2 &= sTmp1
                        // Next

                        // 'Building the entire query.
                        // sTmp1 = "SELECT " & sTmp2 & " FROM Locations WITH (NOLOCK) WHERE Code" & UserInfo.GetValue(APPR.HierarchyLevel) & " = '" & szValue & "'"
                        // RetVal = ConnectionFunctions.Connect_SQLDataReader(MyReader2, sTmp1, ErrMsg)
                        // If RetVal = False Then
                        // ' Rahul Start Edit 27-04-2011
                        // ErrMsg = ShowErrorMessage(GetLanguageType(), "GEN037") & ErrMsg
                        // ' Rahul End Edit 27-04-2011
                        // Exit Try
                        // End If

                        // If MyReader2.HasRows = False Then
                        // RetVal = False
                        // ' Rahul Start Edit 27-04-2011
                        // ErrMsg = ShowErrorMessage(GetLanguageType(), "GEN037")
                        // ' Rahul End Edit 27-04-2011
                        // Exit Try
                        // End If

                        sTmp1 = "";
                        sTmp2 = "";

                        // MyReader2.Read()
                        // For nCtr As Integer = 0 To CInt(UserInfo.GetValue(APPR.HierarchyLevel)) - 1
                        // If Not String.IsNullOrEmpty(sTmp2) Then
                        // sTmp2 &= ", "
                        // End If
                        // sTmp1 = "LocLib" & nCtr + 1 & " = '"
                        // sTmp2 &= sTmp1
                        // sTmp1 = MyReader2.Item(nCtr)
                        // sTmp2 &= sTmp1
                        // sTmp2 &= "'"
                        // Next
                        // MyReader2.Close()
                        sElemName = "LocLib5 = '" + szValue + "'";

                        // Nishad Added 08032014
                        string SCMP = ApprView.GetCompanyProfile();
                        // If SCMP.ToUpper() = "HOKAIRHOTEL" Then
                        // If (CGeneral.HokairClearanceCheck(ApprData.m_lEmpID) <> 0) Then
                        // RetVal = False
                        // ErrMsg = Resources.EAppResource.EmployeeClearenceIsPending
                        // Exit Try
                        // End If


                        // End If

                        // Shyamjith Added for 08/08/2019 for checking the licence in case of distributed licence
                        long nLic = 0;
                        long totLicCount = 0;
                        long nEmpCount;
                        string licenceQry = string.Empty;
                        licenceQry = "SELECT Count(License) License FROM LicenseDist";
                        if ((ConnectionFunctions.Connect_SQLScalar(ref qryRslt, licenceQry, ref Params, ref Conn, ref ErrMsg)))
                        {
                            long.TryParse(qryRslt, out nLic);

                            if ((nLic > 0))
                            {
                                licenceQry = "SELECT ISNULL(License,0) AS LicenseCnt FROM LicenseDist WHERE WComp in (Select dbo.fun_GetFirstLocation(" + szCodeTo + ") )";
                                if ((ConnectionFunctions.Connect_SQLScalar(ref qryRslt, licenceQry, ref Params, ref Conn, ref ErrMsg)))
                                {
                                    long.TryParse(qryRslt, out totLicCount);

                                    if ((totLicCount > 0))
                                    {
                                        licenceQry = "SELECT ISNULL(COUNT(DISTINCT (EMPID)),0) as Cnt FROM FinMast WITH (NOLOCK) WHERE ISNULL(Transferdate,'01/01/1900') = '01/01/1900' AND Status IN (20,21,30) AND dbo.fun_GetLocLib1WithEmpID(EmpId) in  (Select dbo.fun_GetFirstLocation(" + szCodeTo + ") )";
                                        if ((ConnectionFunctions.Connect_SQLScalar(ref qryRslt, licenceQry, ref Params, ref Conn, ref ErrMsg)))
                                        {
                                            long.TryParse(qryRslt, out nEmpCount);

                                            nEmpCount = nEmpCount + 1;
                                            if ((nEmpCount > totLicCount))
                                            {
                                                ErrMsg = "Maximum License exceeded for the Working Company. Location change cannot be approved.";
                                                RetVal = false;

                                                goto ExitTry;
                                            }
                                        }
                                        else
                                        {
                                            ErrMsg = "Maximum License exceeded for the Working Company. Location change cannot be approved.";
                                            RetVal = false;
                                            goto ExitTry;
                                        }
                                    }
                                    else
                                    {
                                        ErrMsg = "Working Company / License not defined in License Distribution Setup. Location change cannot be approved.";
                                        RetVal = false;
                                        goto ExitTry;
                                    }
                                }
                                else
                                {
                                    ErrMsg = "Working Company / License not defined in License Distribution Setup.Location change cannot be approved.";
                                    RetVal = false;
                                    goto ExitTry;
                                }
                            }
                        }
                        // End Shyamjith Added for 08/08/2019 for checking the licence in case of distributed licence


                        if (SCMP.ToUpper() == "AIR ARABIA" | SCMP.ToUpper() == "BANAJA" | SCMP.ToUpper() == "BABTAIN" | SCMP.ToUpper() == "PPMDC")
                        {
                            // #NEWLOC
                            string loclvel = "5";
                            // sQry = "Select Hierarchylevel AS LVL from loclibsetup"
                            // RetVal =ConnectionFunctions.Connect_SQLScalar(loclvel, sQry, ErrMsg)
                            // If RetVal = False Then
                            // ErrMsg = ShowErrorMessage(GetLanguageType(), "GEN067")
                            // Exit Try
                            // End If

                            string sLoc = "";
                            SqlCommand MyCommand1 = Conn.CreateCommand();
                            sQry = "Select dbo.fun_GetFirstLocation(" + szCodeTo + ")";
                            RetVal =ConnectionFunctions.Connect_SQLScalar(ref qryRslt, sQry, ref ErrMsg);
                            if (RetVal == false)
                            {
                                ErrMsg = ApprView.ShowErrorMessage(ApprView.GetLanguageType(), "GEN067");
                                goto ExitTry;
                            }
                            sLoc = qryRslt;

                            string locCNT = "";
                            sQry = "Select Count (Loclib1Code) AS CNT FROM PmsCompnyTransfer WHERE  Loclib1Code = '" + sLoc + "'";
                            RetVal =ConnectionFunctions.Connect_SQLScalar(ref qryRslt, sQry, ref ErrMsg);
                            if (RetVal == false)
                                ErrMsg = ApprView.ShowErrorMessage(ApprView.GetLanguageType(), "GEN067");

                            locCNT = qryRslt;

                            if (Convert.ToInt16(locCNT) > 0)
                            {
                                if (bUpdated == false)
                                {
                                    if (UpdatePMSOrgnsnHierarchy(Convert.ToBoolean(byType) , ApprData.m_lEmpID, ApprData.m_lReqNo, true, sElemName, ref MyCommand1, ref Conn, ref ErrMsg) == true)
                                        bUpdated = true;
                                    else
                                    {
                                        ErrMsg = "Error while updating PMS Organization Hierarchy for for Location Change";
                                        ErrMsg = ApprView.ShowErrorMessage(ApprView.GetLanguageType(), "PMS01");
                                        RetVal = false;
                                        goto ExitTry;
                                    }
                                }
                            }
                            bUpdated = true;
                        }
                    }
                    else if (szElement == "Salary Profile")
                        sElemName = "SalProfile = '" + szValue + "', SalProfilePrev = '" + szValue2 + "'";
                    else if (szElement == "Category")
                    {
                        string sTmp1 = "";
                        string sTmp2 = "";
                        // Building the entire query.
                        sTmp1 = "SELECT Code, CodeMast FROM CategorySecondary WITH (NOLOCK) WHERE Code = '" + szValue + "'";
                        RetVal = ConnectionFunctions.Connect_SQLDataReader(ref MyReader2, sTmp1, ref ErrMsg);
                        if (RetVal == false)
                        {
                            // Rahul Start Edit 27-04-2011
                            ErrMsg = ApprView.ShowErrorMessage(ApprView.GetLanguageType(), "GEN037") + ErrMsg;
                            // Rahul End Edit 27-04-2011
                            break;
                        }
                        if (MyReader2.HasRows == false)
                        {
                            RetVal = false;
                            // Rahul Start Edit 27-04-2011
                            ErrMsg = ApprView.ShowErrorMessage(ApprView.GetLanguageType(), "GEN037");
                            // Rahul End Edit 27-04-2011
                            break;
                        }
                        MyReader2.Read();
                        sTmp1 = MyReader2[0].ToString();
                        sTmp2 = MyReader2[1].ToString();
                        MyReader2.Close();
                        sElemName = "CategMast = '" + sTmp2 + "', CategSec = '" + sTmp1 + "'";
                    }
                    else if (szElement == "Job Title")
                    {
                        sElemName = "JobTitle = '" + szValue + "'";

                        // Nishad Added 08032014
                        string SCMP = ApprView.GetCompanyProfile();
                        if (SCMP.ToUpper() == "AIR ARABIA" | SCMP.ToUpper() == "BANAJA" | SCMP.ToUpper() == "BABTAIN" | SCMP.ToUpper() == "PPMDC")
                        {
                            string sLoc = "";
                            SqlCommand MyCommand1 = Conn.CreateCommand();
                            sQry = "Select dbo.fun_GetLocLib1WithLocLib5(EmpId) from Finmast WITH (NOLOCK) where EmpID = " + ApprData.m_lEmpID;
                            RetVal =ConnectionFunctions.Connect_SQLScalar(ref qryRslt, sQry,ref ErrMsg);
                            if (RetVal == false)
                                ErrMsg = ApprView.ShowErrorMessage(ApprView.GetLanguageType(), "GEN067");

                            sLoc = qryRslt;

                            string iCnt = "0";
                            sQry = "Select Count (Loclib1Code) AS CNT FROM PmsCompnyTransfer WHERE  Loclib1Code = '" + sLoc + "'";
                            RetVal =ConnectionFunctions.Connect_SQLScalar(ref qryRslt, sQry, ref ErrMsg);
                            if (RetVal == false)
                                ErrMsg = ApprView.ShowErrorMessage(ApprView.GetLanguageType(), "GEN067");

                            iCnt = qryRslt;

                            if (Convert.ToInt32(iCnt) > 0)
                            {
                                if (bUpdated == false)
                                {
                                    if (UpdatePMSOrgnsnHierarchy(Convert.ToBoolean(byType), ApprData.m_lEmpID, ApprData.m_lReqNo, false, "", ref MyCommand1, ref Conn, ref ErrMsg) == true)
                                        bUpdated = true;
                                    else
                                    {
                                        ErrMsg = ApprView.ShowErrorMessage(ApprView.GetLanguageType(), "PMS02");
                                        RetVal = false;
                                        goto ExitTry;
                                    }
                                }
                            }
                            bUpdated = true;
                        }
                    }
                    else if (szElement == "Salary Grade")
                        sElemName = "SalGrade = '" + szValue + "'";
                    else if (szElement == "Salary Type")
                        sElemName = "SalType = " + szValue;
                    else if (szElement == "Basic Currency")
                        sElemName = "BSalaryCurr = '" + szValue + "'";
                    else if (szElement == "Basic Amount")
                        sElemName = "BSalaryAmt = " + szValue;
                    else if (szElement == "HRA By")
                        sElemName = "HRABy = '" + szValue + "'";
                    else if (szElement == "HRA Currency")
                        sElemName = "HRACurr = '" + szValue + "'";
                    else if (szElement == "HRA Amount")
                        sElemName = "HRAAmt = " + szValue;
                    else if (szElement == "Transport By")
                        sElemName = "TranBy = " + szValue;
                    else if (szElement == "Transport Currency")
                        sElemName = "TranCurr = '" + szValue + "'";
                    else if (szElement == "Transport Amount")
                        sElemName = "TranAmt = " + szValue;
                    else if (szElement == "Food By")
                        sElemName = "FoodBy = " + szValue;
                    else if (szElement == "Food Currency")
                        sElemName = "FoodCurr = '" + szValue + "'";
                    else if (szElement == "Food Amount")
                        sElemName = "FoodAmt = " + szValue;
                    else if (szElement == "Auxilliary Allowance 1 By")
                        sElemName = "AuxAll1By = " + szValue;
                    else if (szElement == "Auxilliary Allowance 1 Curr")
                        sElemName = "AuxAll1Curr = '" + szValue + "'";
                    else if (szElement == "Auxilliary Allowance 1 Amt")
                        sElemName = "AuxAll1Amt = " + szValue;
                    else if (szElement == "Auxilliary Allowance 2 By")
                        sElemName = "AuxAll2By = " + szValue;
                    else if (szElement == "Auxilliary Allowance 2 Curr")
                        sElemName = "AuxAll2Curr = '" + szValue + "'";
                    else if (szElement == "Auxilliary Allowance 2 Amt")
                        sElemName = "AuxAll2Amt = " + szValue;
                    else if (szElement == "Auxilliary Allowance 3 By")
                        sElemName = "AuxAll3By = " + szValue;
                    else if (szElement == "Auxilliary Allowance 3 Curr")
                        sElemName = "AuxAll3Curr = '" + szValue + "'";
                    else if (szElement == "Auxilliary Allowance 3 Amt")
                        sElemName = "AuxAll3Amt = " + szValue;
                    else if (szElement == "Auxilliary Allowance 4 By")
                        sElemName = "AuxAll4By = " + szValue;
                    else if (szElement == "Auxilliary Allowance 4 Curr")
                        sElemName = "AuxAll4Curr = '" + szValue + "'";
                    else if (szElement == "Auxilliary Allowance 4 Amt")
                        sElemName = "AuxAll4Amt = " + szValue;
                    else if (szElement == "Auxilliary Allowance 5 By")
                        sElemName = "AuxAll5By = " + szValue;
                    else if (szElement == "Auxilliary Allowance 5 Curr")
                        sElemName = "AuxAll5Curr = '" + szValue + "'";
                    else if (szElement == "Auxilliary Allowance 5 Amt")
                        sElemName = "AuxAll5Amt = " + szValue;
                    else if (szElement == "Auxilliary Allowance 6 By")
                        sElemName = "AuxAll6By = " + szValue;
                    else if (szElement == "Auxilliary Allowance 6 Curr")
                        sElemName = "AuxAll6Curr = '" + szValue + "'";
                    else if (szElement == "Auxilliary Allowance 6 Amt")
                        sElemName = "AuxAll6Amt = " + szValue;
                    else if (szElement == "Auxilliary Allowance 7 By")
                        sElemName = "AuxAll7By = " + szValue;
                    else if (szElement == "Auxilliary Allowance 7 Curr")
                        sElemName = "AuxAll7Curr = '" + szValue + "'";
                    else if (szElement == "Auxilliary Allowance 7 Amt")
                        sElemName = "AuxAll7Amt = " + szValue;
                    else if (szElement == "Auxilliary Allowance 8 By")
                        sElemName = "AuxAll8By = " + szValue;
                    else if (szElement == "Auxilliary Allowance 8 Curr")
                        sElemName = "AuxAll8Curr = '" + szValue + "'";
                    else if (szElement == "Auxilliary Allowance 8 Amt")
                        sElemName = "AuxAll8Amt = " + szValue;
                    else if (szElement == "Annual Leave Type")
                        sElemName = "ALCode = '" + szValue + "'";
                    else if (szElement == "Emp Ticket Every")
                        sElemName = "ETicketEvery = " + szValue;
                    else if (szElement == "Family Ticket")
                        sElemName = "FTicketYN = " + szValue;
                    else if (szElement == "Family Tkt Every")
                        sElemName = "FTicketEvery = " + szValue;
                    else if (szElement == "No of Full Tickets")
                        sElemName = "NoOfFullTickets = " + szValue;
                    else if (szElement == "No of Child Tickets")
                        sElemName = "NoOfChildTickets = " + szValue;
                    else if (szElement == "No of Infant Tickets")
                        sElemName = "NoOfInfantTickets = " + szValue;
                    else if (szElement == "Employee Route")
                        sElemName = "RouteEmp = '" + szValue + "'";
                    else if (szElement == "Family Route")
                        sElemName = "RouteFam = '" + szValue + "'";

                    // Check for Chronological Approval of requests
                    if (byType == 0)
                    {
                        lCnt = 0L;
                        sQry = "SELECT Count(Finreqmast.SrNo) As Cnt FROM FinReqmast WITH (NOLOCK),Finchanges WITH (NOLOCK) WHERE Finreqmast.srno = Finchanges.srno and Element = '" + szElement + "' AND Finreqmast.Status < 20 AND Effectivedate < '" + dtEff.ToString("yyyy/MM/dd") + "' AND EmpID = " + ApprData.m_lEmpID;
                        RetVal =ConnectionFunctions.Connect_SQLScalar(ref qryRslt, sQry, ref ErrMsg);

                        long.TryParse(qryRslt, out lCnt);
                        if (lCnt > 0)
                        {
                            RetVal = false;
                            ErrMsg = "Warning...Another Request for Upgrade of ' " + szElement + " ' is posted with effective date less than '" + dtEff.ToString("dd/MM/yyyy") + "'.." + Environment.NewLine + "Approve that request first to continue with the approval of the current request.";
                            goto ExitTry;
                        }
                    }
                    else if (byType == 1)
                    {
                        lCnt = 0L;
                        sQry = "SELECT Count(SrNo) As Cnt FROM FinChanges WITH (NOLOCK) WHERE SrNo > " + ApprData.m_lReqNo + " AND Element = '" + szElement + "' AND Status = 20 AND SrNo In (SELECT SRNO FROM Finreqmast WITH (NOLOCK) WHERE EmpID = " + ApprData.m_lEmpID + ")";
                        RetVal =ConnectionFunctions.Connect_SQLScalar(ref qryRslt, sQry, ref ErrMsg);

                        long.TryParse(qryRslt, out lCnt);
                        if (lCnt > 0)
                        {
                            RetVal = false;
                            ErrMsg = "Warning...Another Request for Upgrade of ' " + szElement + " ' is posted and approved later than the current request.." + Environment.NewLine + " Disapprove that request first to continue with the disapproval of the current request.";
                            goto ExitTry;
                        }
                    }

                    // Executing the query, to change the values into the FinMast table, based on Approval or disapproval.
                    long lRAffected = 0L; // for getting the rows affected... 
                    sQry = "UPDATE FinMast SET " + sElemName + " WHERE EmpID = " + ApprData.m_lEmpID;
                    MyCommand.CommandText = sQry;
                    lRAffected = MyCommand.ExecuteNonQuery();

                    if (lRAffected != 1)
                    {
                        RetVal = false;
                        // Rahul Start Edit 27-04-2011
                        ErrMsg = ApprView.ShowErrorMessage(ApprView.GetLanguageType(), "GEN038");
                        // Rahul End Edit 05-05-2011
                        goto ExitTry;
                    }

                    if (szElement == "Emp Ticket Every" & byType == 1)
                    {
                        RetVal = UpdateTktMaster(ApprData.m_lReqNo, lReqNo, ApprData.m_lEmpID, szElement, szCodeFrom, szCodeTo, ref MyCommand, ref ErrMsg);
                        if (RetVal == false)
                        {
                            // Rahul Start Edit 27-04-2011
                            ErrMsg = ApprView.ShowErrorMessage(ApprView.GetLanguageType(), "GEN039");
                            // Rahul End Edit 27-04-2011
                            goto ExitTry;
                        }
                    }
                    if (szElement == "Emp Ticket Every")
                    {
                        sQry = "Select Top 1 updtktduedate from SalaryProfile  WITH (NOLOCK) WHERE Code =(Select Top 1 SalProfile from Finmast  WITH (NOLOCK) Where EmpId =" + ApprData.m_lEmpID + ")";
                        bool iRes;
                        iRes = false;
                        RetVal =ConnectionFunctions.Connect_SQLScalar(ref qryRslt, sQry, ref ErrMsg);
                        bool.TryParse(qryRslt, out iRes);
                        if (iRes)
                        {
                            if (UpdateTktMasterwhileApprove(ApprData.m_lReqNo, lReqNo, ApprData.m_lEmpID, szCodeFrom, szCodeTo, dtEff, byType, 0, false))
                            {
                                if (byType == 0)
                                {
                                }
                                else
                                {
                                    RetVal = false;
                                    // Rahul Start Edit 27-04-2011
                                    ErrMsg = ApprView.ShowErrorMessage(ApprView.GetLanguageType(), "GEN039");
                                    goto ExitTry;
                                }
                            }
                        }
                    }
                    if (szElement == "Family Tkt Every")
                    {
                        sQry = "Select Top 1 updtktduedate from SalaryProfile  WITH (NOLOCK) WHERE Code =(Select Top 1 SalProfile from Finmast  WITH (NOLOCK) Where EmpId =" + ApprData.m_lEmpID + ")";
                        bool iRes;
                        iRes = false;
                        RetVal =ConnectionFunctions.Connect_SQLScalar(ref qryRslt, sQry, ref ErrMsg);
                        bool.TryParse(qryRslt, out iRes);
                        if (iRes)
                        {
                            if (UpdateTktMasterwhileApprove(ApprData.m_lReqNo, lReqNo, ApprData.m_lEmpID, szCodeFrom, szCodeTo, dtEff, byType, 1, false))
                            {
                                if (byType == 0)
                                {
                                }
                                else
                                {
                                    RetVal = false;
                                    // Rahul Start Edit 27-04-2011
                                    ErrMsg = ApprView.ShowErrorMessage(ApprView.GetLanguageType(), "GEN039");
                                    goto ExitTry;
                                }
                            }
                        }
                    }

                    if (szElement == "Location")
                    {
                        sQry = "UPDATE Employee SET " + sElemName + " WHERE [EmpID] = " + ApprData.m_lEmpID;
                        MyCommand.CommandText = sQry;
                        lRAffected = MyCommand.ExecuteNonQuery();

                        if (lRAffected != 1 & lRAffected != 2)
                        {
                            RetVal = false;
                            // Rahul Start Edit 27-04-2011
                            ErrMsg = ApprView.ShowErrorMessage(ApprView.GetLanguageType(), "GEN040");
                            // Rahul End Edit 27-04-2011
                            goto ExitTry;
                        }


                        #region Updating SAP Cost Centre Code for client DBS when Location updates
                        if (Common.GetCompanyProfile() == "SME")
                        {
                            //ViewNo = 116 HANDLED in HelperClass.cs OnSAPCCCUPDAction
                            //viewno = 117 future dated handled in Payrollprocessfunction.cs UpdateFinMastFrmChanges
                            //viewno = 117 current dated handled in Cgeneral.vb UpdateFinMastFrmChng
                            //viewno = 117 current dated with ByPass handled in ApprovalFlowFunctions.cs UpdateFinMastFrmChng

                            DataTable dttemp = new DataTable();
                            bool isCostCentreUpdate_Success = true;
                            string strCostCentreUpdate_ErrMsg = "";
                            string newLoclib5 = szValue;
                            string costcentrecode = "";
                            string oldcostcentrecode = "";
                            try
                            {
                                dttemp = new DataTable();
                                sQry = "SELECT SFI_Code from vw_SFI_Locations WHERE Code =" + newLoclib5;
                                if (!ConnectionFunctions.Connect_SQLDataTable(ref dttemp, sQry, ref ErrMsg))
                                {
                                    strCostCentreUpdate_ErrMsg = ErrMsg;
                                    isCostCentreUpdate_Success = false;
                                    goto CostCentreUpdateEndStep;
                                }

                                if (dttemp == null && dttemp.Rows.Count == 0)
                                {
                                    strCostCentreUpdate_ErrMsg = "New CostCentre Code could not be determined";
                                    isCostCentreUpdate_Success = false;
                                    goto CostCentreUpdateEndStep;
                                }

                                if (dttemp.Rows[0]["SFI_Code"] != DBNull.Value)
                                    costcentrecode = dttemp.Rows[0]["SFI_Code"].ToString();


                                if (string.IsNullOrEmpty(costcentrecode))
                                {
                                    strCostCentreUpdate_ErrMsg = "New CostCentre Code could not be determined";
                                    isCostCentreUpdate_Success = false;
                                    goto CostCentreUpdateEndStep;
                                }

                                dttemp = new DataTable();
                                sQry = "SELECT AuxString1 from  [Employee] WITH (NOLOCK) WHERE [EmpID] = " + ApprData.m_lEmpID;
                                ConnectionFunctions.Connect_SQLDataTable(ref dttemp, sQry, ref ErrMsg);
                                if (dttemp != null && dttemp.Rows.Count > 0)
                                {
                                    if (dttemp.Rows[0]["AuxString1"] != DBNull.Value)
                                        oldcostcentrecode = dttemp.Rows[0]["AuxString1"].ToString();
                                }



                                if (costcentrecode.EndsWith("_W") || costcentrecode.EndsWith("_B"))
                                {
                                    costcentrecode = costcentrecode.Substring(0, costcentrecode.Length - 2);
                                }

                                
                                sQry = "Update [Employee] set AuxString1 ='" + costcentrecode + "' WHERE [EmpID] = " + ApprData.m_lEmpID;
                                MyCommand.CommandText = sQry;
                                MyCommand.ExecuteNonQuery();

                            //String wLoc = AppClass.Common.GetLocLib1UsingEmpID(Convert.ToInt32(lEmpID)).ToString();
                            //DateTime currDate = DateTime.Now;
                            //string remarks = "SAP Cost Center Code Auto Updated on PayRoll Processing";
                            //newcommon.AuditSave("Employee", "Edit Record", m_apprData.m_sEmpCode, UserInfo.GetValue(Convert.ToInt16(Common.APPR.UserID)).ToString(),
                            //    ref remarks, wLoc, m_apprData.m_lEmpID, ref HRPConn, ref SQLTran);

                            CostCentreUpdateEndStep:;
                            }
                            catch (Exception ex)
                            {
                                strCostCentreUpdate_ErrMsg = ex.Message;
                                isCostCentreUpdate_Success = false;

                                Common.LogException(ex);
                                try { Common.LogAction(ex.Message); } catch { }
                            }
                            finally
                            {
                                string logmessage = "Cost Centre Update Called from Payroll Processing. Update Status:" + (isCostCentreUpdate_Success ? "Success" : "Failed") + (isCostCentreUpdate_Success ? ". Old:" + oldcostcentrecode + ", New:" + costcentrecode : strCostCentreUpdate_ErrMsg);
                                try { Common.LogAction(logmessage); } catch { }
                            }

                        }
                        #endregion



                        sQry = "UPDATE PaymentsMast SET " + sElemName + " WHERE [EmpID] = " + ApprData.m_lEmpID;
                        MyCommand.CommandText = sQry;
                        MyCommand.ExecuteNonQuery();

                        // Added by Seetha 14102021 - Update family location as well
                        sQry = "UPDATE Family SET " + sElemName + " WHERE [sponempcode] = " + ApprData.m_lEmpID;
                        MyCommand.CommandText = sQry;
                        lRAffected = MyCommand.ExecuteNonQuery();

                        // Added by Seetha 02052021 - Update adddedtran location on location transfer
                        sQry = "UPDATE AddDedTran SET " + sElemName + " WHERE [EmpID] = " + ApprData.m_lEmpID + " AND ActiveStatus <> 40 AND  ReqDate >= '" + dtEff.ToString("MM/dd/yyyy") + "'";
                        MyCommand.CommandText = sQry;
                        MyCommand.ExecuteNonQuery();

                        // Added by Seetha 11072021 - Update Leavepaytran location 
                        sQry = "UPDATE leavepaytran SET " + sElemName + " WHERE [EmpID] = " + ApprData.m_lEmpID + " AND Status <> 40 ";
                        MyCommand.CommandText = sQry;
                        MyCommand.ExecuteNonQuery();

                        // Added by Seetha 12112020 - Update Location in Exceptions table also
                        RetVal = UpdateExceptionLocation(ApprData.m_lEmpID, szValue, dtEff, ref MyCommand);

                        if (RetVal == false)
                        {
                            ErrMsg = AppResources.CannotUpdateExceptionLocation;
                            goto ExitTry;
                        }

                        // Added by Seetha 10112021 - Update security template
                        if (Common.IsUserRightsTemplateEnabled())
                        {
                            RetVal = UpdateSecurityTemplate(ApprData.m_lReqNo, ApprData.m_lEmpID, ApprData.m_sEmpCode, UserInfo.GetValue((int)Common.APPR.UserID).ToString(), ref MyCommand);

                            if (RetVal == false)
                            {
                                ErrMsg = AppResources.ErrSecTemplateUpdate;
                                goto ExitTry;
                            }
                        }
                    }

                    // Added by Seetha 10112021 - Update security template
                    if (szElement == "Job Title")
                    {
                        if ((Common.IsUserRightsTemplateEnabled()))
                        {
                            RetVal = UpdateSecurityTemplate(ApprData.m_lReqNo, ApprData.m_lEmpID, ApprData.m_sEmpCode, UserInfo.GetValue((int)Common.APPR.UserID).ToString(), ref MyCommand, szValue);

                            if (RetVal == false)
                            {
                                ErrMsg = AppResources.ErrSecTemplateUpdate;
                                goto ExitTry;
                            }
                        }
                    }

                    if (szElement == "Salary Profile")
                    {
                        sQry = "UPDATE Employee SET SalProfile = '" + szValue + "' WHERE [EmpID] = " + ApprData.m_lEmpID;
                        MyCommand.CommandText = sQry;
                        lRAffected = MyCommand.ExecuteNonQuery();

                        if (lRAffected == 0)
                        {
                            RetVal = false;
                            // Rahul Start Edit 27-04-2011
                            ErrMsg = ApprView.ShowErrorMessage(ApprView.GetLanguageType(), "GEN040");
                            // Rahul End Edit 27-04-2011
                            goto ExitTry;
                        }

                        // Added by Seetha 14102021 - Update family sal profile as well
                        sQry = "UPDATE Family SET SalProfile = '" + szValue + "' WHERE [sponempcode] = " + ApprData.m_lEmpID;
                        MyCommand.CommandText = sQry;
                        lRAffected = MyCommand.ExecuteNonQuery();

                        // Added by Seetha 02052021 - Update adddedtran salary profile
                        sQry = "UPDATE AddDedTran SET SalProfile = '" + szValue + "' WHERE [EmpID] = " + ApprData.m_lEmpID + " AND ActiveStatus <> 40 AND ReqDate >= '" + dtEff.ToString("MM/dd/yyyy") + "'";
                        MyCommand.CommandText = sQry;
                        MyCommand.ExecuteNonQuery();

                        // Added by Seetha 11072021 - Update Leavepaytran salprofile 
                        sQry = "UPDATE leavepaytran SET SalProfile = '" + szValue + "' WHERE [EmpID] = " + ApprData.m_lEmpID + " AND Status <> 40 ";
                        MyCommand.CommandText = sQry;
                        MyCommand.ExecuteNonQuery();

                        // Added by Seetha 25032021 - Update Salary Profile in Exceptions table also
                        RetVal = UpdateExceptionSalProfile(ApprData.m_lEmpID, szValue, dtEff, ref MyCommand);

                        if (RetVal == false)
                        {
                            ErrMsg = AppResources.CannotUpdateExceptionSalProfile;
                            goto ExitTry;
                        }
                    }

                    // Rahul Start Edit 27-06-2012 Annual Leave Type
                    if (szElement == "Annual Leave Type")
                    {
                        if (dtEff <= dtLastPaid)
                        {
                            decimal fTot;
                            decimal fAlBalUpgrd;
                            fTot  = 0;
                            fAlBalUpgrd = 0;
                            decimal fCalcValForAlCurrYr = 0;

                            if (byType == 0)
                            {
                                sQry = "Insert BkdtALUpgrd SELECT  EmpID, " + ApprData.m_lReqNo + ",AccountAs 'Event', SUM(ABS(Exhours)/StdSPHrs) 'NoDays', GetDate(),0.0 FROM Exceptions_New E1 WITH (NOLOCK)" + " WHERE Empid =  " + ApprData.m_lEmpID + " AND ActiveStatus = 40 AND E1.AttemptNo = (Select Max(AttemptNo) FROM Exceptions_New E2 WHERE E1.EmpId = E2.EmpId AND " + " E1.ExDate = E2.ExDate AND E2.ActiveStatus > 20) AND Exdate Between '" + dtEff.ToString("MM/dd/yyyy") + "' AND '" + dtLastPaid.ToString("MM/dd/yyyy") + "' " + " GROUP BY Empid, AccountAS HAVING SUM(ABS(Exhours)/StdSPHrs) > 0 ";
                                MyCommand.CommandText = sQry;
                                MyCommand.ExecuteNonQuery();

                                string strDefAtt = string.Empty;
                                strDefAtt = Common.RetDefAtt(sSalProf,0);

                                sQry = "Insert BkdtALUpgrd SELECT  " + ApprData.m_lEmpID + ", " + ApprData.m_lReqNo + ",'" + strDefAtt + "' 'Event', (ABS(DATEDIFF(dd,'" + dtEff.ToString("MM/dd/yyyy") + "','" + dtLastPaid.ToString("MM/dd/yyyy") + "') +1 ) - (SELECT ISNULL(SUM(NoDays),0) FROM BkdtAlUpgrd WHERE Empid = " + ApprData.m_lEmpID + " AND ReqNo = " + ApprData.m_lReqNo + "))" + " 'NoDays', GetDate(),0.0 HAVING (ABS(DATEDIFF(dd,'" + dtEff.ToString("MM/dd/yyyy") + "','" + dtLastPaid.ToString("MM/dd/yyyy") + "') +1 ) - (SELECT ISNULL(SUM(NoDays),0) FROM BkdtAlUpgrd " + " WHERE Empid = " + ApprData.m_lEmpID + " AND ReqNo = " + ApprData.m_lReqNo + " )) > 0";
                                MyCommand.CommandText = sQry;
                                MyCommand.ExecuteNonQuery();


                                sQry = "Select ISNULL(SUM(NoDays),0) As Tot From BkdtAlUpgrd WITH (NOLOCK) WHERE EmpID = " + ApprData.m_lEmpID + " AND ReqNo = " + ApprData.m_lReqNo + " AND Event In (SELECT EventCode From SalaryProfileSec WHERE SalProCode = '" + sSalProf + "' AND AccLeave = 1)";
                                RetVal =ConnectionFunctions.Connect_SQLScalar(ref qryRslt, sQry, ref ErrMsg);
                                if (RetVal == false)
                                    ErrMsg = ApprView.ShowErrorMessage(ApprView.GetLanguageType(), "GEN009");

                                decimal.TryParse(qryRslt, out fTot);

                                // sQry = "SELECT ( ((SELECT ((col1 * 1.0 / col2 * 1.0) * " & fTot & ") FROM dbo.AlEntitlementsSec WHERE Code = '" & szCodeTo & "') - (SELECT ((col1 * 1.0 / col2 * 1.0) * " & fTot & " ) FROM dbo.AlEntitlementsSec WHERE Code = '" & szCodeFrom & "')) ) As ALbalUpd"   'Nishad Commented 12012017 --As Aziz modified the query in HCMS, Same change is done in E-Approval (Below Query)
                                // sQry = "SELECT ( ((SELECT ((col1 * 1.0 / col2 * 1.0) * " & fTot & ") FROM dbo.AlEntitlementsSec WHERE Code = '" & szCodeTo & "') - ISNULL((SELECT Case WHEN Type = 4 THEN ((col2 * 1.0 / 365.0) * " & fTot & ") ELSE ((col1 * 1.0 / col2 * 1.0) * " & fTot & ") END FROM dbo.AlEntitlementsSec s join AlEntitlements a on s.code = a.Code WHERE s.Code = '" & szCodeFrom & "'  AND Slab = 1),0)) ) As ALbalUpd"
                                // Denson Changed as per VC Code 08042019
                                sQry = "SELECT ( ((SELECT Case WHEN Type = 4 THEN ((col2 * 1.0 / 365.0 ) * " + fTot + ") ELSE ((col1 * 1.0 / col2 * 1.0) * " + fTot + ") END FROM dbo.AlEntitlementsSec s join AlEntitlements a on s.code = a.Code WHERE s.Code = '" + szCodeTo + "' AND Slab = 1)  - ISNULL((SELECT Case WHEN Type = 4 THEN ((col2 * 1.0 / 365.0) * " + fTot + ") ELSE ((col1 * 1.0 / col2 * 1.0) * " + fTot + ") END FROM dbo.AlEntitlementsSec s join AlEntitlements a on s.code = a.Code WHERE s.Code = '" + szCodeFrom + "'  AND Slab = 1),0)) ) As ALbalUpd";
                                RetVal =ConnectionFunctions.Connect_SQLScalar(ref qryRslt, sQry, ref ErrMsg);
                                if (RetVal == false)
                                    ErrMsg = ApprView.ShowErrorMessage(ApprView.GetLanguageType(), "GEN009");

                                decimal.TryParse(qryRslt, out fAlBalUpgrd);

                                fAlBalUpgrd = decimal.Round(fAlBalUpgrd, 2);
                                sQry = "Update BkdtAlUpgrd Set ALBalUpd = " + fAlBalUpgrd + " WHERE EmpID = " + ApprData.m_lEmpID + " AND ReqNo = " + ApprData.m_lReqNo;
                                MyCommand.CommandText = sQry;
                                MyCommand.ExecuteNonQuery();

                                // Seetha Commented 28/11/2021 - Calculate Alcurryr logic separately
                                // sQry = "Update EmpBals Set ALBal = ALBal + " & fAlBalUpgrd & " , ALCurrYr = ALCurrYr + " & fAlBalUpgrd & " WHERE EmpID = " & ApprData.m_lEmpID
                                sQry = "Update EmpBals Set ALBal = ALBal + " + fAlBalUpgrd + " WHERE EmpID = " + ApprData.m_lEmpID;
                                MyCommand.CommandText = sQry;
                                MyCommand.ExecuteNonQuery();

                                DateTime lastDay = new DateTime(DateTime.Now.Year, 12, 31);
                                string diff2 = (lastDay - dtLastPaid).TotalDays.ToString();
                                sQry = "SELECT ( ((SELECT Case WHEN Type = 4 THEN ((col2 * 1.0 / 365.0 ) * " + diff2 + ") ELSE ((col1 * 1.0 / col2 * 1.0) * " + diff2 + ") END FROM dbo.AlEntitlementsSec s join AlEntitlements a on s.code = a.Code WHERE s.Code = '" + szCodeTo + "' AND Slab = 1)  - ISNULL((SELECT Case WHEN Type = 4 THEN ((col2 * 1.0 / 365.0) * " + diff2 + ") ELSE ((col1 * 1.0 / col2 * 1.0) * " + diff2 + ") END FROM dbo.AlEntitlementsSec s join AlEntitlements a on s.code = a.Code WHERE s.Code = '" + szCodeFrom + "'  AND Slab = 1),0)) ) As ALbalUpd";
                                RetVal =ConnectionFunctions.Connect_SQLScalar(ref qryRslt, sQry, ref ErrMsg);
                                if (RetVal == false)
                                    ErrMsg = ApprView.ShowErrorMessage(ApprView.GetLanguageType(), "GEN009");

                                decimal.TryParse(qryRslt, out fCalcValForAlCurrYr);

                                fCalcValForAlCurrYr = decimal.Round(fCalcValForAlCurrYr, 2);

                                sQry = "Update EmpBals Set ALCurrYr = ALBal + " + fCalcValForAlCurrYr + " WHERE EmpID = " + ApprData.m_lEmpID;
                                MyCommand.CommandText = sQry;
                                MyCommand.ExecuteNonQuery();


                                string sErrMsg = string.Empty;

                                sErrMsg = "Leave Entitlement Updated with " + fAlBalUpgrd;

                                RetVal = AuditSave(ApprData.m_sModuleTable, "Leave Entitlement Approved", ApprData.m_sEmpCode, UserInfo.GetValue((int)Common.APPR.UserID).ToString(), ref ErrMsg, sWorkComp, ApprData.m_lReqNo, ref dtCurr, ref MyCommand);
                            }
                            else
                            {
                                sQry = "SELECT Top 1 ALBalUpd FROM BkdtAlUpgrd WHERE EmpID = " + ApprData.m_lEmpID + " AND ReqNo = " + ApprData.m_lReqNo;
                                RetVal =ConnectionFunctions.Connect_SQLScalar(ref qryRslt, sQry, ref ErrMsg);
                                if (RetVal == false)
                                    ErrMsg = ApprView.ShowErrorMessage(ApprView.GetLanguageType(), "GEN009");

                                decimal.TryParse(qryRslt, out fAlBalUpgrd);

                                fAlBalUpgrd = decimal.Round(fAlBalUpgrd, 2);
                                sQry = "Update EmpBals Set ALBal = ALBal - " + fAlBalUpgrd + ", ALCurrYr = ALCurrYr - " + fAlBalUpgrd + " WHERE EmpID = " + ApprData.m_lEmpID;
                                MyCommand.CommandText = sQry;
                                MyCommand.ExecuteNonQuery();

                                sQry = "Delete From BkdtAlUpgrd WHERE EmpID = " + ApprData.m_lEmpID + " AND ReqNo = " + ApprData.m_lReqNo;
                                MyCommand.CommandText = sQry;
                                MyCommand.ExecuteNonQuery();


                                string sErrMsg = string.Empty;
                                sErrMsg = "Leave Entitlement Down with " + fAlBalUpgrd;
                                RetVal = AuditSave((ApprData.m_nViewNo).ToString(), "Leave Entitlement Disapproved", ApprData.m_sEmpCode, UserInfo.GetValue((int)Common.APPR.UserID).ToString(), ref sErrMsg, sWorkComp, ApprData.m_lReqNo, ref dtCurr, ref MyCommand);
                            }
                        }
                    }
                }
                // Shyamjith Added for Updating Work Agreement  from Finmast if the parameter is set in Salary Profile, on 31/07/2019 as this code was missing from VC
                bool iFlag;
                sQry = "SELECT ISNULL(UpdWorkAgrmt,0) AS UpdWorkAgrmt FROM dbo.SalaryProfile WITH (NOLOCK) WHERE Code IN (SELECT Salprofile FROM finmast WITH (NOLOCK) WHERE Empid  = " + ApprData.m_lEmpID + " )";
                RetVal = ConnectionFunctions.Connect_SQLScalar(ref qryRslt, sQry, ref ErrMsg);

                bool.TryParse(qryRslt, out iFlag);

                if ((RetVal))
                {
                    if ((iFlag))
                    {
                        sQry = "UPDATE w SET w.BSalaryAmt = f.BSalaryAmt,w.BSalaryCurr = f.BSalaryCurr, w.hraamt = f.hraamt, ";
                        sQry = sQry + " w.HRABy = f.HraBy ,w.HRACurr = f.HRACurr, w.TranBy = f.TranBy ,w.TranCurr = f.TranCurr ,w.TranAmt = f.TranAmt ,w.FoodBy = f.FoodBy, ";
                        sQry = sQry + " w.FoodCurr = f.FoodCurr ,w.FoodAmt = f.FoodAmt ,w.AuxAll1By = f.AuxAll1By ,w.AuxAll1Curr = f.AuxAll1Curr, ";
                        sQry = sQry + " w.AuxAll1Amt = f.AuxAll1Amt ,w.AuxAll2By = f.AuxAll2By ,w.AuxAll2Curr = f.AuxAll2Curr ,w.AuxAll2Amt = f.AuxAll2Amt, ";
                        sQry = sQry + " w.AuxAll3By = f.AuxAll3By ,w.AuxAll3Curr = f.AuxAll3Curr ,w.AuxAll3Amt = f.AuxAll3Amt ,w.AuxAll4By = f.AuxAll4By, ";
                        sQry = sQry + " w.AuxAll4Curr = f.AuxAll4Curr ,w.AuxAll4Amt = f.AuxAll4Amt ,w.AuxAll5By = f.AuxAll5By ,w.AuxAll5Curr = f.AuxAll5Curr, ";
                        sQry = sQry + " w.AuxAll5Amt = f.AuxAll5Amt ,w.AuxAll6By = f.AuxAll6By ,w.AuxAll6Curr = f.AuxAll6Curr ,w.AuxAll6Amt = f.AuxAll6Amt, ";
                        sQry = sQry + " w.AuxAll7By = f.AuxAll7By ,w.AuxAll7Curr = f.AuxAll7Curr ,w.AuxAll7Amt = f.AuxAll7Amt ,w.AuxAll8By = f.AuxAll8By, ";
                        sQry = sQry + " w.AuxAll8Curr = f.AuxAll8Curr ,w.AuxAll8Amt = f.AuxAll8Amt FROM  dbo.WrkAgrmntDet w , finmast f WHERE w.EmpID = f.EmpID AND w.ActiveStatus <= 30  AND f.EmpID = " + ApprData.m_lEmpID;

                        MyCommand.CommandText = sQry;
                        MyCommand.ExecuteNonQuery();
                    }
                }
                // End Shyamjith Added for Updating Work Agreement  from Finmast if the parameter is set in Salary Profile, on 31/07/2019 as this code was missing from VC


                RetVal = true;

            ExitTry:;
            }
            catch (Exception Ex)
            {
                RetVal = false;
                ErrMsg = Ex.Message;
            }
            finally
            {
                if (MyReader != null)
                {
                    if (!MyReader.IsClosed)
                        MyReader.Close();
                }

                if (MyReader2 != null)
                {
                    if (!MyReader2.IsClosed)
                        MyReader2.Close();
                }
            }

            return RetVal;
        }


        public static string GetWorkingComp(int lEmpID)
        {
            string sQry = "";
            string ErrMsg = "";
            string RetVal = "";
            bool RetValBool = false;
            // Nishad Edited 09022015 --To take from Employee
            // sQry = "Select loclib1 from FinMast  WITH (NOLOCK) where EmpID =" & lEmpID
            sQry = "Select dbo.fun_GetLocLib1WithEmpID(" + lEmpID + ")";
            RetValBool = ConnectionFunctions.Connect_SQLScalar(ref RetVal, sQry, ref ErrMsg);
            if (RetValBool == false)
                ErrMsg = ApprView.ShowErrorMessage(ApprView.GetLanguageType(), "DEF038");
            return RetVal;
        }

        public static bool UpdateBonusFormulaAmountOnFinancialChange(ref CSApprovalData ApprData, ref SqlConnection Conn, ref SqlCommand MyCommand, ref string ErrMsg)
        {
            bool RetVal = false;
            try
            {
                string sQry = string.Empty;
                int lRAffected = 0;
                int lCount = 0;

                DateTime sVal=new DateTime(1900,1,1);

                // Rahul Start Edit 27-06-2012 Annual Leave Type
                DateTime dtLastPaid;
                DateTime effectiveDate;
                string sSalProf;
                string sWorkComp = string.Empty;
                DateTime dtCurr = new DateTime(1900, 1, 1);
                bool bUpdated = false;
                int lStatus = 0;
                string dt_NextMth;

                string qryRslt = "";
                SqlParameter[] Params = null;

                DataTable MyReader;
                MyReader = new DataTable();
                SqlDataReader MyReader1 = null/* TODO Change to default(_) if this is not a reference type */;
                MyReader1 = null/* TODO Change to default(_) if this is not a reference type */;
                sQry = "SELECT DateAdd(dd,1,LastPaidDate) As Dt,LastPaidDate,SalProfile, Status FROM Finmast WITH (NOLOCK) WHERE EmpID = " + ApprData.m_lEmpID + " AND IsNull(Transferdate,'01/01/1900') = '01/01/1900'";
                RetVal = ConnectionFunctions.Connect_SQLDataReader(ref MyReader1, sQry, ref ErrMsg);

                if (MyReader1.HasRows)
                {
                    MyReader1.Read();
                    sVal = (MyReader1[0] == DBNull.Value ? new DateTime(1900, 1, 1) : Convert.ToDateTime(MyReader1[0]));
                    dtLastPaid = (MyReader1[1] == DBNull.Value ? new DateTime(1900, 1, 1) : Convert.ToDateTime(MyReader1[1]));
                    sSalProf = (MyReader1[2] == DBNull.Value ? "0" : MyReader1[2].ToString());
                    lStatus = Convert.ToInt32(MyReader1[3] == DBNull.Value ? 0 : MyReader1[3]);
                }
                MyReader1.Close();

                // Nishad Added 28042015 --Status Checking of FinMast
                if (lStatus > 20)
                {
                    dt_NextMth = sVal.Year + "/" + sVal.Month + "/" + Utility.General.GetLastDayOfMonth(sVal.Month, sVal.Year);
                    // dt_NextMth.SetDate(dt_NextMth.GetYear(),dt_NextMth.GetMonth(),CGeneral::GetLastDayOfMonth (dt_NextMth.GetMonth (), dt_NextMth.GetYear ()));
                    // sQry.Format(_T("SELECT Case When DateAdd(mm,1,LastPaidDate) < Effectivedate THEN 1 ELSE 0 END As Cnt FROM FinMast,FinReqMast WHERE Finmast.EmpID = FinReqMast.EmpID AND FinReqMast.[SrNo] = %ld"),ApprData.m_lReqNo );
                    sQry = "SELECT Case When '" + dt_NextMth + "' < Effectivedate THEN 1 ELSE 0 END As Cnt FROM FinMast WITH (NOLOCK) ,FinReqMast WITH (NOLOCK) WHERE Finmast.EmpID = FinReqMast.EmpID AND FinReqMast.[SrNo] = " + ApprData.m_lReqNo + "";
                    RetVal = ConnectionFunctions.Connect_SQLScalar(ref qryRslt, sQry, ref ErrMsg);
                    if (RetVal == false)
                    {
                        // Rahul Start Edit 27-04-2011
                        RetVal = false;

                        ErrMsg = ApprView.ShowErrorMessage(ApprView.GetLanguageType(), "GEN009");
                        return RetVal;
                    }
                    int.TryParse(qryRslt, out lCount);

                    if ((lCount == 1))
                    {
                        RetVal = true;
                        return RetVal;
                    }
                }

                lCount = 0;
                sQry = "SELECT Count(1) FROM FinChanges WITH (NOLOCK),FinReqMast WITH (NOLOCK) WHERE SalUpgrade = 1 AND Finchanges.Srno = Finreqmast.Srno AND FinreqMast.[SrNo] = " + ApprData.m_lReqNo;
                RetVal = ConnectionFunctions.Connect_SQLScalar(ref qryRslt, sQry, ref ErrMsg);
                if (RetVal == false)
                {
                    // Rahul Start Edit 27-04-2011
                    RetVal = false;

                    ErrMsg = ApprView.ShowErrorMessage(ApprView.GetLanguageType(), "GEN009");
                    return RetVal;
                }
                int.TryParse(qryRslt, out lCount);

                if ((lCount == 0))
                {
                    RetVal = true;
                    return true;
                }
                effectiveDate = DateTime.Now.Date;
                sQry = "SELECT EffectiveDate FROM FinReqMast WITH (NOLOCK) WHERE FinreqMast.[SrNo] = " + ApprData.m_lReqNo;
                RetVal = ConnectionFunctions.Connect_SQLScalar(ref qryRslt, sQry, ref ErrMsg);
                if (RetVal == false)
                {
                    // Rahul Start Edit 27-04-2011
                    RetVal = false;

                    ErrMsg = ApprView.ShowErrorMessage(ApprView.GetLanguageType(), "GEN009");
                    return RetVal;
                }
                DateTime.TryParse(qryRslt, out effectiveDate);

                sQry = "SELECT ISNULL(Formula,'') Formula,ReqNo,ISNULL(FixAmount,0) FixAmount  FROM BonusSetup WITH (NOLOCK) WHERE STATUS IN (20,30) AND BonusPayType = 3 AND EmpId = " + ApprData.m_lEmpID.ToString();
                if ((ConnectionFunctions.Connect_SQLDataTable(ref MyReader, sQry, ref ErrMsg)))
                {
                    if ((MyReader != null))
                    {
                        foreach (DataRow dr in MyReader.Rows)
                        {
                            int reqNo = 0;
                            string formula = "";
                            decimal amount = 0;
                            decimal OrgAmount = 0;
                            reqNo = Convert.ToInt32(dr["ReqNo"]);
                            formula = dr["Formula"].ToString();
                            reqNo = Convert.ToInt32(dr["ReqNo"]);
                            OrgAmount = Convert.ToDecimal(dr["FixAmount"]);
                            if ((string.IsNullOrEmpty(formula)))
                            {
                                ErrMsg = "Unable to Calculate Bonus amount as formula is blank";
                                RetVal = false;
                                goto ExitTry;
                            }
                            sQry = "Select (" + formula + ") as CalculatedAmount FROM FinMast WITH (NOLOCK) WHERE EmpId = " + ApprData.m_lEmpID;
                            if ((!ConnectionFunctions.Connect_SQLScalar(ref qryRslt, sQry, ref Params,ref  Conn, ref ErrMsg)))
                            {
                                ErrMsg = "Error Occured While Calculating Bonus Amount Details" + ErrMsg;
                                RetVal = false;
                                goto ExitTry;
                            }

                            decimal.TryParse(qryRslt, out amount);
                            string amountStr = "";
                            string orgAmountStr = "";
                            amountStr = Utility.General.Round(ApprData.m_lEmpID, amount, Conn.ConnectionString).ToString();
                            orgAmountStr = Utility.General.Round(ApprData.m_lEmpID, OrgAmount, Conn.ConnectionString).ToString();

                            if ((Convert.ToDecimal(amountStr) != Convert.ToDecimal(orgAmountStr)))
                            {
                                long newReqNo=0L;
                                RetVal = GenReqNo2(1010, ref newReqNo, ref Conn, ref ErrMsg);
                                if (RetVal == false)
                                    goto ExitTry;

                                // newReqNo = gateway.CommonServicesClient.GetRequestNo(1010).ToString()
                                // 'sQry = "UPDATE BonusSetup SET  FixAmount = '" & amountStr & "' WHERE STATUS<=20 AND BonusPayType = 3 AND EmpId = " & ApprData.m_lEmpID.ToString() & " AND  ReqNo = " & reqNo.ToString()
                                sQry = "INSERT INTO BonusSetup ( [EmpID], [BonusType], [BonusPayType], [Percentage], [EffDate], [ReqNo], [Reqdate], [LastModDateTime], [Status], [ActiveStatus], [LocLib1], [LocLib2], [LocLib3], [LocLib4], [LocLib5], [SalProfile], [ReqID], [Remarks], [Element], [FixAmount], [Formula], [Currency])";
                                sQry = sQry + " SELECT EMPID,BonusType,[BonusPayType],[Percentage],'" + effectiveDate.ToString("yyyy-MM-dd") + "'," + newReqNo + ",GETDATE(),GETDATE(),[Status],[ActiveStatus],[LocLib1], [LocLib2], [LocLib3], [LocLib4], [LocLib5], [SalProfile], [ReqID], [Remarks], [Element], '" + amountStr + "', [Formula], [Currency] FROM BonusSetup Where EmpId = " + ApprData.m_lEmpID.ToString() + " AND ReqNo = " + reqNo;

                                if ((!ConnectionFunctions.Connect_SQLNonQuery(ref lRAffected, sQry, ref ErrMsg)))
                                {
                                    ErrMsg = "Error Occured updating  calculated Bonus Amount. Details" + ErrMsg;
                                    RetVal = false;
                                    goto ExitTry;
                                }
                                sQry = "INSERT INTO ApprProcess ( [Priority], [ViewNo], [ReqNo], [RequestDate], [EmpID], [ISL], [App], [AppDate], [NoOfAppr], [Status], [Remarks], [DocAttach], [OnHold], [HoldUserNo], [Deleted], [Returned], [LastModDateTime], [LockedByUser], [ReqID], [NextApprAuth], [AsGroup], [GroupNo], [Selected], [Bypassed], [ReturnedUserNo], [ISLA], [WFCode], [RemarksApp])";
                                sQry = sQry + " SELECT [Priority], [ViewNo], " + newReqNo + ", [RequestDate], [EmpID], [ISL], [App], [AppDate], [NoOfAppr], [Status], 'Posted From Approvals', [DocAttach], [OnHold], [HoldUserNo], [Deleted], [Returned], GETDATE(), [LockedByUser], [ReqID], [NextApprAuth], [AsGroup], [GroupNo], [Selected], [Bypassed], [ReturnedUserNo], [ISLA], [WFCode], [RemarksApp] FROM ApprProcess WHERE ViewNo = 1010 AND EmpId = " + ApprData.m_lEmpID.ToString() + " AND ReqNo = " + reqNo.ToString();
                                if ((!ConnectionFunctions.Connect_SQLNonQuery(ref lRAffected, sQry, ref ErrMsg)))
                                {
                                    ErrMsg = "Error Occured updating  calculated Bonus Amount. Details" + ErrMsg;
                                    RetVal = false;
                                    goto ExitTry;
                                }
                                sQry = "UPDATE ApprProcess SET Status = 40, [LastModDateTime] = GETDATE()  WHERE ViewNo = 1010 AND EmpId = " + ApprData.m_lEmpID.ToString() + " AND ReqNo = " + reqNo.ToString();
                                if ((!ConnectionFunctions.Connect_SQLNonQuery(ref lRAffected, sQry, ref ErrMsg)))
                                {
                                    ErrMsg = "Error Occured updating  calculated Bonus Amount. Details" + ErrMsg;
                                    RetVal = false;
                                    goto ExitTry;
                                }
                                sQry = "UPDATE BonusSetup SET Status = 40 , ActiveStatus = 40 WHERE EmpId = " + ApprData.m_lEmpID.ToString() + " AND ReqNo = " + reqNo.ToString();
                                if ((!ConnectionFunctions.Connect_SQLNonQuery(ref lRAffected, sQry, ref ErrMsg)))
                                {
                                    ErrMsg = "Error Occured updating  calculated Bonus Amount. Details" + ErrMsg;
                                    RetVal = false;
                                    goto ExitTry;
                                }
                            }
                        }
                    }
                }
                else
                    RetVal = false;

                // Code here
                sQry = "SELECT ISNULL(Element,'') Formula,ReqNo,ISNULL(FixAmount,0) FixAmount  FROM BonusSetup WITH (NOLOCK) WHERE STATUS IN (20,30) AND BonusPayType = 5 AND EmpId = " + ApprData.m_lEmpID.ToString();
                if ((ConnectionFunctions.Connect_SQLDataTable(ref MyReader, sQry, ref ErrMsg)))
                {
                    if ((MyReader != null))
                    {
                        foreach (DataRow dr in MyReader.Rows)
                        {
                            int reqNo = 0;
                            string formula = "";
                            decimal amount = 0;
                            decimal OrgAmount = 0;
                            reqNo = Convert.ToInt32(dr["ReqNo"]);
                            formula = dr["Formula"].ToString();
                            reqNo = Convert.ToInt32(dr["ReqNo"]);
                            OrgAmount = Convert.ToDecimal(dr["FixAmount"]);
                            if ((string.IsNullOrEmpty(formula)))
                            {
                                ErrMsg = "Unable to Calculate Bonus amount as formula is blank";
                                RetVal = false;
                                goto ExitTry;
                            }
                            if ((formula.ToUpper() == "GROSSSALARY"))
                                formula = "TotalInBasicCurr";
                            sQry = "Select (" + formula + ") as CalculatedAmount FROM FinMast WITH (NOLOCK) WHERE EmpId = " + ApprData.m_lEmpID;
                            if ((!ConnectionFunctions.Connect_SQLScalar(ref qryRslt, sQry, ref Params, ref Conn, ref ErrMsg)))
                            {
                                ErrMsg = "Error Occured While Calculating Bonus Amount Details" + ErrMsg;
                                RetVal = false;
                                goto ExitTry;
                            }

                            decimal.TryParse(qryRslt, out amount);

                            string amountStr = "";
                            string orgAmountStr = "";
                            amountStr = Utility.General.Round(ApprData.m_lEmpID, amount, Conn.ConnectionString).ToString();
                            orgAmountStr = Utility.General.Round(ApprData.m_lEmpID, OrgAmount, Conn.ConnectionString).ToString();

                            if ((Convert.ToDecimal(amountStr) != Convert.ToDecimal(orgAmountStr)))
                            {
                                long newReqNo=0L;
                                RetVal = GenReqNo2(1010, ref newReqNo, ref Conn, ref ErrMsg);
                                if (RetVal == false)
                                    goto ExitTry;

                                // newReqNo = gateway.CommonServicesClient.GetRequestNo(1010).ToString()
                                // 'sQry = "UPDATE BonusSetup SET  FixAmount = '" & amountStr & "' WHERE STATUS<=20 AND BonusPayType = 3 AND EmpId = " & ApprData.m_lEmpID.ToString() & " AND  ReqNo = " & reqNo.ToString()
                                sQry = "INSERT INTO BonusSetup ( [EmpID], [BonusType], [BonusPayType], [Percentage], [EffDate], [ReqNo], [Reqdate], [LastModDateTime], [Status], [ActiveStatus], [LocLib1], [LocLib2], [LocLib3], [LocLib4], [LocLib5], [SalProfile], [ReqID], [Remarks], [Element], [FixAmount], [Formula], [Currency])";
                                sQry = sQry + " SELECT EMPID,BonusType,[BonusPayType],[Percentage],'" + effectiveDate.ToString("yyyy-MM-dd") + "'," + newReqNo + ",GETDATE(),GETDATE(),[Status],[ActiveStatus],[LocLib1], [LocLib2], [LocLib3], [LocLib4], [LocLib5], [SalProfile], [ReqID], [Remarks], [Element], '" + amountStr + "', [Formula], [Currency] FROM BonusSetup Where EmpId = " + ApprData.m_lEmpID.ToString() + " AND ReqNo = " + reqNo;

                                if ((!ConnectionFunctions.Connect_SQLNonQuery(ref lRAffected, sQry, ref ErrMsg)))
                                {
                                    ErrMsg = "Error Occured updating  calculated Bonus Amount. Details" + ErrMsg;
                                    RetVal = false;
                                    goto ExitTry;
                                }
                                sQry = "INSERT INTO ApprProcess ( [Priority], [ViewNo], [ReqNo], [RequestDate], [EmpID], [ISL], [App], [AppDate], [NoOfAppr], [Status], [Remarks], [DocAttach], [OnHold], [HoldUserNo], [Deleted], [Returned], [LastModDateTime], [LockedByUser], [ReqID], [NextApprAuth], [AsGroup], [GroupNo], [Selected], [Bypassed], [ReturnedUserNo], [ISLA], [WFCode], [RemarksApp])";
                                sQry = sQry + " SELECT [Priority], [ViewNo], " + newReqNo + ", [RequestDate], [EmpID], [ISL], [App], [AppDate], [NoOfAppr], [Status], 'Posted From Approvals', [DocAttach], [OnHold], [HoldUserNo], [Deleted], [Returned], GETDATE(), [LockedByUser], [ReqID], [NextApprAuth], [AsGroup], [GroupNo], [Selected], [Bypassed], [ReturnedUserNo], [ISLA], [WFCode], [RemarksApp] FROM ApprProcess_New WHERE ViewNo = 1010 AND EmpId = " + ApprData.m_lEmpID.ToString() + " AND ReqNo = " + reqNo.ToString(); //Naveen-Aziz-Apprprocess to Apprprocess_New
                                if ((!ConnectionFunctions.Connect_SQLNonQuery(ref lRAffected, sQry, ref ErrMsg)))
                                {
                                    ErrMsg = "Error Occured updating  calculated Bonus Amount. Details" + ErrMsg;
                                    RetVal = false;
                                    goto ExitTry;
                                }
                                sQry = "UPDATE ApprProcess SET Status = 40, [LastModDateTime] = GETDATE()  WHERE ViewNo = 1010 AND EmpId = " + ApprData.m_lEmpID.ToString() + " AND ReqNo = " + reqNo.ToString();
                                if ((!ConnectionFunctions.Connect_SQLNonQuery(ref lRAffected, sQry, ref ErrMsg)))
                                {
                                    ErrMsg = "Error Occured updating  calculated Bonus Amount. Details" + ErrMsg;
                                    RetVal = false;
                                    goto ExitTry;
                                }
                                sQry = "UPDATE BonusSetup SET Status = 40 , ActiveStatus = 40 WHERE EmpId = " + ApprData.m_lEmpID.ToString() + " AND ReqNo = " + reqNo.ToString();
                                if ((!ConnectionFunctions.Connect_SQLNonQuery(ref lRAffected, sQry, ref ErrMsg)))
                                {
                                    ErrMsg = "Error Occured updating  calculated Bonus Amount. Details" + ErrMsg;
                                    RetVal = false;
                                    goto ExitTry;
                                }
                            }
                        }
                    }
                }
                else
                    RetVal = false;

                RetVal = true;

            ExitTry:;
            }
            catch (Exception ex)
            {
                RetVal = false;
                ErrMsg = ex.Message;
            }
            return RetVal;
        }

        public static bool GenReqNo2(long ViewNo, ref long lReqNo, ref SqlConnection Conn, ref string ErrMsg)
        {
            bool RetVal = false;
            try
            {
                string qryRslt = "";
                SqlParameter[] Params = null;
                lReqNo = 0;
                string sQry = "SELECT ReqNo FROM GenReqNo WHERE ViewNo = " + ViewNo;
                RetVal = ConnectionFunctions.Connect_SQLScalar(ref qryRslt, sQry, ref Params, ref  Conn, ref ErrMsg);
                if (RetVal == false)
                    goto ExitTry;

                long.TryParse(qryRslt, out lReqNo);

                if (lReqNo == 0)
                {
                    lReqNo = 1;
                    RetVal = true;
                }

                sQry = "UPDATE GenReqNo SET ReqNo = " + (lReqNo + 1) + " WHERE ViewNo = " + ViewNo;
                int Result = 0;
                RetVal = ConnectionFunctions.Connect_SQLNonQuery(ref Result, sQry, ref ErrMsg, Params, ref Conn);
                if (RetVal == false)
                    goto ExitTry;

                ExitTry:;
            }
            catch (Exception Ex)
            {
                ErrMsg = Ex.Message;
                RetVal = false;
            }

            return RetVal;
        }


        public static bool AuditSave(string strTable, string strTrans, string strEC, string strUser, ref string strRetErr, string strWC, int lTranNo, ref DateTime dtCurr, ref SqlCommand MyCommand)
        {
            bool RetVal = true;
            try
            {
                string sQry = string.Empty;
                string clientIPAddress = Common.GetIPAddress + " " + Common.GetBrowserDet(); // Seetha added 21092020 - Add browser details with IP Address
                sQry = "INSERT INTO AuditTrail([Table], [Transaction], [TransactionNo], EmpCode, UserID, [Date], Errors, WComp,MachineName) ";
                sQry += "VALUES(@Table, @Transaction, @TransactionNo, @EmpCode, @UserID, @Date, @Errors, @WComp, @MacName)";
                SqlParameter[] Params = new SqlParameter[9];
                Params[0] = new SqlParameter("@Table", strTable);
                Params[1] = new SqlParameter("@Transaction", strTrans + " (Eapproval)");
                Params[2] = new SqlParameter("@TransactionNo", lTranNo);
                Params[3] = new SqlParameter("@EmpCode", strEC);
                Params[4] = new SqlParameter("@UserID", strUser);
                Params[5] = new SqlParameter("@Date", dtCurr.ToString("yyyy/MM/dd HH:mm:ss"));
                Params[6] = new SqlParameter("@Errors", strRetErr);
                Params[7] = new SqlParameter("@WComp", strWC);
                Params[8] = new SqlParameter("@MacName", clientIPAddress);

                MyCommand.Parameters.Clear();
                MyCommand.CommandType = CommandType.Text;
                MyCommand.CommandText = sQry;
                MyCommand.Parameters.AddRange(Params);
                if (MyCommand.ExecuteNonQuery() < 1)
                {
                    RetVal = false;
                    goto ExitTry;
                }

                RetVal = true;

            ExitTry:;
            }
            catch (Exception Ex)
            {
                RetVal = false;
            }
            finally
            {
                MyCommand.Parameters.Clear();
            }

            return RetVal;
        }


        protected static bool UpdateTktMaster(long lSrNo, long lReqNo, long lEmpID, string sElem, string sFromVal, string sToVal, ref SqlCommand MyCommand, ref string ErrMsg)
        {
            bool RetVal = false;
            try
            {
                string str = string.Empty;
                if (sElem == "Emp Ticket Every")
                {
                    if (Convert.ToInt32(sFromVal) == 0 & System.Convert.ToInt32(sToVal) > 0)
                    {
                        str = "Delete From TktMaster WHERE EmpID =" + lEmpID + " AND FamCode Is NULL AND RelName = 10";
                        MyCommand.CommandText = str;
                        MyCommand.ExecuteNonQuery();
                    }
                    else if (Convert.ToInt32(sFromVal) > 0)
                    {
                        str = "Update TktMaster Set TktDueDate = DateAdd(mm," + sFromVal + ",LstTktIssueDt) WHERE EmpID = " + lEmpID + " AND FamCode Is NULL AND RelName = 10";
                        MyCommand.CommandText = str;
                        MyCommand.ExecuteNonQuery();
                    }
                }
                RetVal = true;
            }
            catch (Exception Ex)
            {
                RetVal = false;
                ErrMsg = Ex.Message;
            }
            return RetVal;
        }

        public static bool UpdateTktMasterwhileApprove(long lSrNo, long lReqNo, long lEmpID, string sFromVal, string sToVal, DateTime dtEff, byte byCallType, byte IsFam, bool IsMonthClosing)
        {
            //28-06-2022: copied from newcommon UpdateTktMasterwhileApprove function, added some missing code from CGeneral UpdateTktMasterwhileApprove function

            try
            {
                string errmsg = "";
                int iResult = 0;

                string sResult = "";
                SqlDataReader myreader = null;
                String[] userinfo = Common.UserInfo;
                string userno = userinfo[Convert.ToInt16(Common.APPR.UserNo)];
                string strUser = userinfo[Convert.ToInt16(Common.APPR.UserID)];
                int m_nLevels = Convert.ToInt16(userinfo[Convert.ToInt16(Common.APPR.HierarchyLevel)]);
                double iTktEveryNew = Convert.ToDouble(sToVal);
                double iTktEveryold = Convert.ToDouble(sFromVal);
                double iDaystobeadd = 0.0;
                double sAccureddaysdiff = 0.0;
                string sqry;
                DateTime dttktduedtold = DateTime.MinValue, dtlasttktduedt = DateTime.MinValue, dttktduedtNew = DateTime.MinValue;
                string TicketEveryType = Common.GetEmpTicketEveryType(Convert.ToInt32(lEmpID)); //robin added code

                double shAccruedPer = 0.0;
                double shAccruedPerBal = 0.0;
                string sFamCode;
                string sQry;
                DateTime dtCurDuedt;

                string sAuditmessage;
                string sAudiTqry;



                if (IsMonthClosing)
                {

                    sAuditmessage = "Due Date auto Calculated and Updated while month closing as Ticket Every Changed from " + sFromVal + " to " + sToVal + " with Future Date Effective Date";

                }

                else
                {
                    sAuditmessage = "Due Date auto Calculated and Updated on Approval of Financial as Ticket Every Changed from " + sFromVal + " to  " + sToVal;


                }
                if (IsFam != 1)
                {

                    long lCnt = 0;

                    sqry = "SELECT TktDueDate,LstTktIssueDt,FamCode FROM dbo.TktMaster WHERE EmpID = " + lEmpID + " AND  ISNULL(FamCode,'')=''";

                    if (byCallType == 0)
                    {
                        sQry = "SELECT Count(EMPID) as CNT FROM TktMaster WHERE EmpID = " + lEmpID + " AND  ISNULL(FamCode,'')=''  ";

                        if (!ConnectionFunctions.Connect_SQLScalar(ref sResult, sQry, ref errmsg))
                            return false;

                        if (sResult != "")
                        {
                            lCnt = Convert.ToInt32(sResult);
                        }

                        if (iTktEveryNew != 0)
                        {
                            if (lCnt <= 0) //No Record Found 
                            {

                                DateTime dtTktDueDt, dtLastTktIssDt, dtRepTktDueDate;
                                dtTktDueDt = dtLastTktIssDt = dtRepTktDueDate = dtEff;




                                dtTktDueDt=Utility.General.RetDateSince(dtTktDueDt, Convert.ToInt32(iTktEveryNew), false, TicketEveryType);
                                if (TicketEveryType == FixedMembers.TicketEveryType_Days)
                                    dtRepTktDueDate = Utility.General.RetDateSince(dtRepTktDueDate, (2 * 365), false, TicketEveryType);
                                else
                                    dtRepTktDueDate = Utility.General.RetDateSince(dtRepTktDueDate, 24, false, TicketEveryType);
                                string sQuery;

                                if (!Common.AddTktMaster(dtTktDueDt, dtLastTktIssDt, lEmpID, dtRepTktDueDate, userinfo))//Adding to Ticket Master.
                                {
                                    errmsg = "Cannot Insert Record in Ticket Master table.";
                                    return false;
                                }

                                return true;
                            }
                        }
                        else
                        {
                            try
                            {
                                string sMessage, str, m_EmpCode;


                                sMessage = "Ticket Master Record has been Deleted while Ticket entilement [0] change Approved.";

                                str = "INSERT INTO dbo.Audit_FortktLV( TranDate, TranType,EMPID,Famcode, Fromval, Toval,TranFrom,UserID ) "
                                    + " SELECT  GETDATE(), 1,EMPID,Famcode, CONVERT(VARCHAR(15),TktDueDate,103) ,'NULL','" + sMessage + "','" + strUser + "' FROM Tktmaster WHERE Empid = " + lEmpID + " AND FamCode Is NULL AND RelName = 10";


                                if (!ConnectionFunctions.Connect_SQLNonQuery(ref iResult, str, ref errmsg))
                                    return false;

                                str = "Insert TktMasterCpy Select * From TktMaster WHERE EmpID = " + lEmpID + " AND FamCode Is NULL AND RelName = 10";

                                if (!ConnectionFunctions.Connect_SQLNonQuery(ref iResult, str, ref errmsg))
                                    return false;


                                str = "Delete From TktMaster WHERE EmpID = " + lEmpID + " AND FamCode Is NULL AND RelName = 10";
                                if (!ConnectionFunctions.Connect_SQLNonQuery(ref iResult, str, ref errmsg))
                                    return false;

                                m_EmpCode = Common.GetEmpCodeFromEmpID(lEmpID);

                                sMessage = "Ticket Master Record has been Deleted while Ticket entilement change [0] Approved.";

                                if (!Common.AuditSave("TktMaster", "Deleted Record", m_EmpCode, strUser, sMessage, "", 0))
                                {


                                }


                                return true; ;


                            }
                            catch (Exception ex)
                            {

                                return false;
                            }
                        }
                    }

                }
                else
                {
                    sqry = "SELECT TktDueDate,LstTktIssueDt,FamCode FROM dbo.TktMaster WHERE EmpID = " + lEmpID + " AND ISNULL(FamCode,'')<>'' ";
                }

                if (!ConnectionFunctions.Connect_SQLDataReader(ref myreader, sqry, ref errmsg))
                    return false;

                if (myreader.HasRows)
                {
                    while (myreader.Read())
                    {
                        dttktduedtold = Convert.ToDateTime(myreader["TktDueDate"]);
                        dtlasttktduedt = Convert.ToDateTime(myreader["LstTktIssueDt"]);
                        sFamCode = myreader["FamCode"].ToString();
                        dtCurDuedt = dttktduedtold;
                        myreader.Close();


                        if (byCallType == 0)
                        {
                            if (iTktEveryold <= 0)
                            {

                                dttktduedtNew = dtEff;
                                dttktduedtNew=Utility.General.RetDateSince(dttktduedtNew, Convert.ToInt32(iTktEveryNew), false, TicketEveryType);
                            }

                            else
                            {
                                if(iTktEveryNew == 0) 
                                {
                                    string m_EmpCode;
                                    string sMessage;
                                    string str;

                                    sMessage = "Ticket Master Record has been Deleted while Ticket entilement [0] change Approved.";

                                    str = ("INSERT INTO dbo.Audit_FortktLV( TranDate, TranType,EMPID,Famcode, Fromval, Toval,TranFrom,UserID ) " + (" SELECT  GETDATE(), 1,EMPID,Famcode, CONVERT(VARCHAR(15),TktDueDate,103) ,'NULL','"
                                            + (sMessage + ("','"
                                            + (strUser + ("' FROM Tktmaster WHERE Empid = "
                                            + (lEmpID + " AND FamCode = '" + sFamCode + "' ")))))));
                                    if (!ConnectionFunctions.Connect_SQLNonQuery(ref iResult, str, ref errmsg))
                                        continue;

                                    str = ("Insert TktMasterCpy Select * From TktMaster WHERE EmpID = "
                                            + (lEmpID + " AND FamCode = '" + sFamCode + "'"));
                                    if (!ConnectionFunctions.Connect_SQLNonQuery(ref iResult, str, ref errmsg))
                                        continue;

                                    str = ("Delete From TktMaster WHERE EmpID = "
                                          + (lEmpID + " AND FamCode = '" + sFamCode + "'"));
                                    if (!ConnectionFunctions.Connect_SQLNonQuery(ref iResult, str, ref errmsg))
                                        continue;

                                    m_EmpCode = Common.GetEmpCodeFromEmpID(lEmpID);
                                    sMessage = "Ticket Master Record has been Deleted for Family Code : " + sFamCode + " while Ticket entilement change [0] Approved.";
                                    if (!Common.AuditSave("TktMaster", "Deleted Record", m_EmpCode, strUser, sMessage, "", 0))
                                    {
                                    }
                                    continue;
                                }


                                double idays = 0;

                                if (TicketEveryType == FixedMembers.TicketEveryType_Days)
                                {
                                    idays = iTktEveryold;

                                    dtlasttktduedt = dttktduedtold.AddDays((-1 * idays));

                                    sAccureddaysdiff = dtEff.Subtract(dtlasttktduedt).Days;
                                    shAccruedPer = (sAccureddaysdiff * 100.0) /  iTktEveryold;
                                    shAccruedPerBal = (100.0 - shAccruedPer);
                                    iDaystobeadd = (iTktEveryNew * shAccruedPerBal) / 100.0;
                                }
                                else
                                {
                                    idays = (iTktEveryold * 365.0) / 12.0;

                                    dtlasttktduedt = dttktduedtold.AddDays((-1 * idays));

                                    sAccureddaysdiff = dtEff.Subtract(dtlasttktduedt).Days;
                                    shAccruedPer = (sAccureddaysdiff * 12.0 * 100.0) / (365.0 * iTktEveryold);
                                    shAccruedPerBal = (100.0 - shAccruedPer);
                                    iDaystobeadd = (iTktEveryNew * 365.0 * shAccruedPerBal) / (12.0 * 100.0);
                                }
                                   

                                dttktduedtNew = dtEff.AddDays(iDaystobeadd);
                            }
                            //Inserting into table


                            if (shAccruedPer < 100.0)
                            {

                                string sAuditTRail;
                                DateTime dtLstModDate = DateTime.Now;
                                if (IsFam != 1)
                                {
                                    sQry = "DELETE FROM TktDueDateChanges WHERE SrNo = " + lSrNo + "  AND  ISNULL (Famcode,'') ='' ";

                                    if (!ConnectionFunctions.Connect_SQLNonQuery(ref iResult, sQry, ref errmsg))
                                        return false;

                                }
                                else
                                {
                                    sQry = "DELETE FROM TktDueDateChanges WHERE SrNo = " + lSrNo + "  AND FamCode = '" + sFamCode + "' ";


                                    if (!ConnectionFunctions.Connect_SQLNonQuery(ref iResult, sQry, ref errmsg))
                                        return false;
                                }

                                sQry = "INSERT INTO TktDueDateChanges (SrNo ,EmpID,FamCode,TktDuedtFrom,TktDuedtTo,UserID ,EffectiveDate,LastModDateTime) "
                                    + " VALUES  ( " + lSrNo + " , " + lEmpID + " ,'" + sFamCode + "', '" + dtCurDuedt.ToString("yyyy/MM/dd") + "' , '" + dttktduedtNew.ToString("yyyy/MM/dd") + "' , '" + strUser + "' ,'" + dtEff.ToString("yyyy/MM/dd") + "','" + dtLstModDate.ToString("yyyy/MM/dd HH:mm:ss") + "'  ) ";







                                if (!ConnectionFunctions.Connect_SQLNonQuery(ref iResult, sQry, ref errmsg))
                                    return false;

                                if (IsFam != 1)
                                {
                                    sAudiTqry = " INSERT INTO dbo.Audit_FortktLV( TranDate, TranType,EMPID,Famcode, Fromval, Toval,TranFrom,UserID )"
                                    + " SELECT  GETDATE(),1,EMPID,Famcode, '" + dtCurDuedt.ToString("dd/MM/yyyy") + "' AS FRomVal, '" + dttktduedtNew.ToString("dd/MM/yyyy") + "' as ToVal,'" + sAuditmessage + "','" + strUser + "'  FROM Tktmaster WHERE empid = " + lEmpID + "  AND ISNULL(Famcode,'') = '' ";




                                    sAuditTRail = " For Employee";

                                    sqry = "UPDATE TktMaster SET TktDueDate = '" + dttktduedtNew.ToString("yyyy/MM/dd") + "' WHERE EMPID = " + lEmpID + "  AND FamCode IS NULL";

                                }
                                else
                                {


                                    sAuditTRail = " For Family Code '" + sFamCode + "' ";


                                    sAudiTqry = "INSERT INTO dbo.Audit_FortktLV( TranDate, TranType,EMPID,Famcode, Fromval, Toval,TranFrom,UserID )"
                                    + " SELECT  GETDATE(), 1,EMPID,Famcode, '" + dtCurDuedt.ToString("dd/MM/yyyy") + "' AS FRomVal, '" + dttktduedtNew.ToString("dd/MM/yyyy") + "' as ToVal,'" + sAuditmessage + "','" + strUser + "'  FROM Tktmaster WHERE empid = " + lEmpID + " AND FamCode  =  '" + sFamCode + "' ";






                                    sqry = "UPDATE TktMaster SET TktDueDate = '" + dttktduedtNew.ToString("yyyy/MM/dd") + "' WHERE EMPID = " + lEmpID + "  AND FamCode  =  '" + sFamCode + "' ";


                                }

                                if (!ConnectionFunctions.Connect_SQLNonQuery(ref iResult, sAudiTqry, ref errmsg))
                                    return false;

                                if (!ConnectionFunctions.Connect_SQLNonQuery(ref iResult, sqry, ref errmsg))
                                    return false;
                                string m_EmpCode;


                                m_EmpCode = Common.GetEmpCodeFromEmpID(lEmpID);

                                if (!Common.AuditSave("TktMaster", "Updated Record", m_EmpCode, strUser, sAuditmessage + sAuditTRail, "", 0))
                                {


                                }
                            }
                        }
                        else
                        {
                            if (IsMonthClosing)
                            {

                                sAuditmessage = " Due Date auto Calculated and Updated while month closing as Ticket Every Changed from " + sFromVal + " to " + sToVal + " with Future Date Effective Date";

                            }
                            else
                            {
                                sAuditmessage = "Due Date auto Calculated and Updated while Disapproval of  Financial as Ticket Every Changed from " + sToVal + " to " + sFromVal;


                            }


                            if (IsFam != 1)
                            {


                                sAudiTqry = "INSERT INTO dbo.Audit_FortktLV( TranDate, TranType,EMPID,Famcode, Fromval, Toval,TranFrom,UserID ) "
                                    + " SELECT  GETDATE(), 1,EMPID,Famcode, CONVERT(VARCHAR(15),TktDueDate,103),CONVERT(VARCHAR(15) AS  FRomVal, '" + dttktduedtNew.ToString("dd/MM/yyyy") + "' as ToVal,'" + sAuditmessage + "','" + strUser + "'  FROM Tktmaster WHERE empid = " + lEmpID + "  AND ISNULL(Famcode,'') = '' ";



                                sQry = "SELECT TktDuedtFrom FROM TktDueDateChanges WHERE Srno = " + lSrNo + " AND  Empid = " + lEmpID + " AND ISNULL(Famcode,'') = '' ";


                            }
                            else
                            {

                                sAudiTqry = "INSERT INTO dbo.Audit_FortktLV( TranDate, TranType,EMPID,Famcode, Fromval, Toval,TranFrom,UserID ) "
                                + " SELECT  GETDATE(), 1,EMPID,Famcode, CONVERT(VARCHAR(15),TktDueDate,103),CONVERT(VARCHAR(15) AS  FRomVal , '" + dttktduedtNew.ToString("dd/MM/yyyy") + "' as ToVal,'" + sAuditmessage + "','" + strUser + "'  FROM Tktmaster WHERE empid = " + lEmpID + " AND FamCode  =  '" + sFamCode + "' ";




                                sQry = "SELECT TktDuedtFrom  FROM TktDueDateChanges where Srno = " + lSrNo + " AND  Empid = " + lEmpID + " AND FamCode = '" + sFamCode + "' ";



                            }

                            sResult = "";
                            if (!ConnectionFunctions.Connect_SQLScalar(ref sResult, sQry, ref errmsg))
                                return false;

                            if (sResult != "")
                            {
                                dttktduedtNew = Convert.ToDateTime(sResult);
                                if (IsFam != 1)
                                {
                                    sqry = "UPDATE TktMaster SET TktDueDate = '" + dttktduedtNew.ToString("yyyy/MM/dd") + "' WHERE EMPID = " + lEmpID + "    AND FamCode IS NULL";
                                }
                                else
                                {
                                    sqry = "UPDATE TktMaster SET TktDueDate =  '" + dttktduedtNew.ToString("yyyy/MM/dd") + "' WHERE EMPID = " + lEmpID + "    AND FamCode = '" + sFamCode + "' ";
                                }
                                if (!ConnectionFunctions.Connect_SQLNonQuery(ref iResult, sqry, ref errmsg))
                                    return false;



                            }
                        }
                    }
                    myreader.Close();
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public static bool UpdateExceptionLocation(Int32 empID, string locLib5, DateTime effectiveDate, ref SqlCommand MyCommand)
        {
            try
            {
                int result = 0;
                SqlConnection mcon1 = new SqlConnection(ConnectionFunctions.GetConnectionString());
                SqlParameter[] Params = new SqlParameter[4];
                string Errmsg = string.Empty;
                var ExptMonTblName = string.Empty;

                if ((effectiveDate != default(DateTime)))
                    ExptMonTblName = "Expt_" + GetMonth(effectiveDate) + "_" + effectiveDate.Year;

                Params[0] = new SqlParameter("@EmpId", (empID != default(int)) ? empID : 0);
                Params[1] = new SqlParameter("@TableName", (string.IsNullOrEmpty(ExptMonTblName)) ? string.Empty : ExptMonTblName);
                Params[2] = new SqlParameter("@Loclib5", (string.IsNullOrEmpty(locLib5)) ? string.Empty : locLib5);
                Params[3] = new SqlParameter("@EffetiveDate", (effectiveDate != default(DateTime)) ? effectiveDate : new DateTime(1900,1,1));

                MyCommand.Parameters.Clear();
                MyCommand.CommandText = "Appr_UpdateExceptionsLocation";
                MyCommand.Parameters.AddRange(Params);
                MyCommand.CommandType = CommandType.StoredProcedure;
                MyCommand.ExecuteNonQuery();
                MyCommand.Parameters.Clear();
                MyCommand.CommandType = CommandType.Text; // reset the type for further use. 

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static string GetMonth(DateTime EXdate)
        {
            Int16 Ex = (Int16)EXdate.Month;

            string Exretval;
            if (Ex < 10)
                Exretval = "0" + Ex;
            else
                Exretval = Ex.ToString();
            return Exretval;
        }

        public static bool UpdateSecurityTemplate(Int32 reqNo, Int32 empID, string EmpCode, string UserId, ref SqlCommand MyCommand, string ChgJobTitle = "")
        {
            try
            {
                int result = 0;
                SqlConnection mcon1 = new SqlConnection(ConnectionFunctions.GetConnectionString());
                SqlParameter[] Params = new SqlParameter[4];
                string Errmsg = string.Empty;
                var ExptMonTblName = string.Empty;
                string sQry = string.Empty;
                string sResult = string.Empty;
                var empUserID = string.Empty;
                var empJobTitle = string.Empty;
                int templateID = 0;
                bool retVal = false;

                string sWorkComp = string.Empty;

                string qryRslt = "";
                DateTime dtCurr = DateTime.Now;

                sWorkComp = GetWorkingComp(empID);

                sQry = " SELECT UserID FROM Security WITH(NOLOCK) WHERE EmpID = " + empID + " AND Deleted = 0 AND UserID NOT IN ('EIS','AUTO') ";
                if ((ConnectionFunctions.Connect_SQLScalar(ref sResult, sQry, ref Errmsg)))
                {
                    if (!string.IsNullOrEmpty(sResult))
                    {
                        empUserID = sResult;

                        if (string.IsNullOrEmpty(ChgJobTitle))
                        {
                            sQry = " SELECT JobTitle FROM FinMast WITH(NOLOCK) WHERE EmpID = " + empID;
                            if ((ConnectionFunctions.Connect_SQLScalar(ref empJobTitle, sQry, ref Errmsg)))
                                ChgJobTitle = empJobTitle;
                        }

                        sQry = "SELECT ID FROM SecTempMast ST WHERE '" + ChgJobTitle + "' IN (select INROWS FROM dbo.CrackInRows(',',ST.JobTitleCode)) ";

                        if ((ConnectionFunctions.Connect_SQLScalar(ref qryRslt, sQry, ref Errmsg)))
                        {
                            int.TryParse(qryRslt, out templateID);

                            if (templateID > 0)
                            {
                                retVal = Common.SecurityTemplateUpdate(templateID, empUserID, ref MyCommand, ref Errmsg);

                                if (retVal == true)
                                {
                                    if (!string.IsNullOrEmpty(Errmsg) & Errmsg.Equals("1"))
                                    {
                                        Errmsg = "No User Rights to update for this Criteria (Template ID:" + templateID + ")";
                                        retVal = AuditSave("SecRights", "Security Template Update From Approval", EmpCode, UserId, ref Errmsg, sWorkComp, reqNo, ref dtCurr, ref MyCommand);
                                    }
                                    else
                                    {
                                        Errmsg = "Rights to updated for " + empUserID + " with Template ID:" + templateID;
                                        retVal = AuditSave("SecRights", "Security Template Update From Approval", EmpCode, UserId, ref Errmsg, sWorkComp, reqNo, ref dtCurr, ref MyCommand);
                                    }
                                }
                                else
                                    return false;
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool UpdateExceptionSalProfile(Int32 empID, string locLib5, DateTime effectiveDate, ref SqlCommand MyCommand)
        {
            try
            {
                int result = 0;
                SqlConnection mcon1 = new SqlConnection(ConnectionFunctions.GetConnectionString());
                SqlParameter[] Params = new SqlParameter[4];
                string Errmsg = string.Empty;
                var ExptMonTblName = string.Empty;

                if ((effectiveDate != default(DateTime)))
                    ExptMonTblName = "Expt_" + GetMonth(effectiveDate) + "_" + effectiveDate.Year;

                Params[0] = new SqlParameter("@EmpId", (empID != default(int)) ? empID : 0);
                Params[1] = new SqlParameter("@TableName", (string.IsNullOrEmpty(ExptMonTblName)) ? string.Empty : ExptMonTblName);
                Params[2] = new SqlParameter("@SalProfile", (string.IsNullOrEmpty(locLib5)) ? string.Empty : locLib5);
                Params[3] = new SqlParameter("@EffetiveDate", (effectiveDate != default(DateTime)) ? effectiveDate : new DateTime(1900,1,1));

                MyCommand.Parameters.Clear();
                MyCommand.CommandText = "Appr_UpdateExceptionsSalProfile";
                MyCommand.Parameters.AddRange(Params);
                MyCommand.CommandType = CommandType.StoredProcedure;
                MyCommand.ExecuteNonQuery();
                MyCommand.Parameters.Clear();
                MyCommand.CommandType = CommandType.Text; // reset the type for further use. 

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static CSApprovalData GetApprData(int viewNo, int reqNo)
        {
            //28-06-2022: Robin added function, derived from GetApprData function in HCMS\Areas\eApprovalPortal\Common\HelperClass.cs

            CSApprovalData apprDataObj = new CSApprovalData();
            bool retVal;
            string ErrMsg = string.Empty;

            string inClauseStmt = string.Empty;

            inClauseStmt = reqNo.ToString();

            try
            {
                string MySQL = String.Empty;
                DataTable resTable = new DataTable();

                if (viewNo == 30102) //Employee GR
                {
                    MySQL = "  SELECT AP.ViewNo,AP.ReqNo,AP.RequestDate,AP.EmpID,EmpConc4NewEmpAppr.EmpCode, EmpConc4NewEmpAppr.EmpNameE,  " +
                    "  AP.App,AP.AppDate,AP.NoOfAppr,AP.[Status],AP.DocAttach,AP.AsGroup,AP.GroupNo,AP.WFCode,ModTbl.ModuleTable            " +
                    "  FROM apprprocess AP WITH (NOLOCK)                                                                                    " +
                    "  INNER JOIN dbo.EmpConc4NewEmpAppr ON AP.EmpID = dbo.EmpConc4NewEmpAppr.EmpID                                         " +
                    "  LEFT OUTER JOIN dbo.CSModules ModTbl ON AP.ViewNo = ModTbl.ViewNo                                                    " +
                    "  WHERE AP.reqno IN ( " + inClauseStmt + ") and AP.ViewNo= " + viewNo;
                }
                else if (viewNo == 30103) //Family GR
                {
                    MySQL = "  SELECT AP.ViewNo,AP.ReqNo,AP.RequestDate,AP.EmpID,Family.FamCode AS EmpCode, Family.FamNameE AS EmpNameE,  " +
                    "  AP.App,AP.AppDate,AP.NoOfAppr,AP.[Status],AP.DocAttach,AP.AsGroup,AP.GroupNo,AP.WFCode,ModTbl.ModuleTable            " +
                    "  FROM apprprocess AP WITH (NOLOCK)                                                                                                 " +
                    "  INNER JOIN Family on AP.EmpID = Family.FamID                                          " +
                    "  LEFT OUTER JOIN dbo.CSModules ModTbl ON AP.ViewNo = ModTbl.ViewNo                                                    " +
                    "  WHERE AP.reqno IN ( " + inClauseStmt + ") and AP.ViewNo= " + viewNo;
                }
                else if (viewNo == 30104) //Visitor GR
                {
                    MySQL = "  SELECT AP.ViewNo,AP.ReqNo,AP.RequestDate,AP.EmpID,Visitor.Code AS EmpCode, Visitor.NameE AS EmpNameE,  " +
                     "  AP.App,AP.AppDate,AP.NoOfAppr,AP.[Status],AP.DocAttach,AP.AsGroup,AP.GroupNo,AP.WFCode,ModTbl.ModuleTable            " +
                     "  FROM apprprocess AP WITH (NOLOCK)                                                                                                  " +
                     "  INNER JOIN Visitor on AP.EmpID = Visitor.VisID                                          " +
                     "  LEFT OUTER JOIN dbo.CSModules ModTbl ON AP.ViewNo = ModTbl.ViewNo                                                    " +
                     "  WHERE AP.reqno IN ( " + inClauseStmt + ") and AP.ViewNo= " + viewNo;
                }
                else if (viewNo == 30105) //Sponsor GR
                {
                    MySQL = "  SELECT AP.ViewNo,AP.ReqNo,AP.RequestDate,AP.EmpID,Sponsor.Code AS EmpCode, Sponsor.NameE AS EmpNameE,  " +
                    "  AP.App,AP.AppDate,AP.NoOfAppr,AP.[Status],AP.DocAttach,AP.AsGroup,AP.GroupNo,AP.WFCode,ModTbl.ModuleTable            " +
                    "  FROM apprprocess AP WITH (NOLOCK)                                                                                                 " +
                    "  INNER JOIN Sponsor on AP.EmpID = Sponsor.SponID                                         " +
                    "  LEFT OUTER JOIN dbo.CSModules ModTbl ON AP.ViewNo = ModTbl.ViewNo                                                    " +
                    "  WHERE AP.reqno IN ( " + inClauseStmt + ") and AP.ViewNo= " + viewNo;
                }
                else
                {
                    MySQL = "  SELECT AP.ViewNo,AP.ReqNo,AP.RequestDate,AP.EmpID,EmpConc4NewEmpAppr.EmpCode, EmpConc4NewEmpAppr.EmpNameE,  " +
                     "  AP.App,AP.AppDate,AP.NoOfAppr,AP.[Status],AP.DocAttach,AP.AsGroup,AP.GroupNo,AP.WFCode,ModTbl.ModuleTable            " +
                     "  FROM apprprocess AP WITH (NOLOCK)                                                                                                 " +
                     "  INNER JOIN dbo.EmpConc4NewEmpAppr ON AP.EmpID = dbo.EmpConc4NewEmpAppr.EmpID                                         " +
                     "  LEFT OUTER JOIN dbo.CSModules ModTbl ON AP.ViewNo = ModTbl.ViewNo                                                    " +
                     "  WHERE AP.reqno IN ( " + inClauseStmt + ") and AP.ViewNo= " + viewNo;
                }

                retVal = ConnectionFunctions.Connect_SQLDataTable(ref resTable, MySQL, ref ErrMsg);

                if (resTable.Rows.Count > 0)
                {
                    foreach (DataRow row in resTable.Rows)
                    {
                        apprDataObj = new CSApprovalData();

                        apprDataObj.m_nViewNo = (row["ViewNo"] != null) ? Convert.ToInt32(row["ViewNo"].ToString()) : 0;
                        apprDataObj.m_lReqNo = (row["ReqNo"] != null) ? Convert.ToInt32(row["ReqNo"].ToString()) : 0;
                        apprDataObj.m_lEmpID = (row["EmpID"] != null) ? Convert.ToInt32(row["EmpID"].ToString()) : 0;
                        apprDataObj.m_sEmpCode = (row["EmpCode"] != null) ? row["EmpCode"].ToString() : "";
                        apprDataObj.m_sEmpName = (row["EmpNameE"] != null) ? row["EmpNameE"].ToString() : "";
                        apprDataObj.m_sApp = (row["App"] != null) ? row["App"].ToString() : "";
                        apprDataObj.m_sAppDate = (row["AppDate"] != null) ? row["AppDate"].ToString() : "";
                        apprDataObj.m_byNoOfAppr = (row["NoOfAppr"] != null) ? Convert.ToByte(row["NoOfAppr"].ToString()) : Convert.ToByte(0);
                        apprDataObj.m_byStatus = (row["Status"] != null) ? Convert.ToByte(row["Status"].ToString()) : Convert.ToByte(0);
                        apprDataObj.m_sDocAttach = (row["DocAttach"] != null) ? row["DocAttach"].ToString() : "";
                        apprDataObj.m_byAsGroup = (row["AsGroup"] != null) ? Convert.ToByte(row["AsGroup"].ToString()) : Convert.ToByte(0);
                        apprDataObj.m_nGroupNo = (row["GroupNo"] != null) ? Convert.ToInt32(row["GroupNo"].ToString()) : 0;
                        apprDataObj.m_sCodeName = (row["WFCode"] != null) ? row["WFCode"].ToString() : "";
                        apprDataObj.m_sModuleTable = (row["ModuleTable"] != null) ? row["ModuleTable"].ToString() : "";
                        //apprDataObj.m_sDeliveredDate = (row["DeliveredDate"] != null && !DBNull.Value.Equals(row["DeliveredDate"])) ? row["DeliveredDate"].ToString() : ""; ;
                        //apprDataObj.m_sSeenDate = (row["SeenDate"] != null && !DBNull.Value.Equals(row["SeenDate"])) ? row["SeenDate"].ToString() : ""; ;

                    }
                }
            }
            catch
            {

            }

            return apprDataObj;
        }


        public static bool CheckVoucherCreated(string sTableName, int lEmpID, ref SqlConnection Conn, ref string Errmsg)
        {
            // Not Same
            bool RetVal = false;
            try
            {
                int lCnt = 0;
                string sQry = "";
                string sVal = "";
                string qryRslt = "";
                SqlParameter[] Params = null;

                sQry = "Select IsNull(Val,0) As Val From MasterSetup WHERE Code = '26'";
                RetVal = ConnectionFunctions.Connect_SQLScalar(ref qryRslt, sQry, ref Errmsg);
                if (RetVal == false)
                    // Rahul Start Edit 26-04-2011
                    Errmsg = ApprView.ShowErrorMessage(ApprView.GetLanguageType(), "GEN009") + Environment.NewLine + ApprView.ShowErrorMessage(ApprView.GetLanguageType(), "GEN057");

                sVal = qryRslt;
                if (System.Convert.ToInt32(sVal) > 0)
                    // sQuery.Format("SELECT Count(ReqNo) As Cnt FROM %s WHERE IsCalcDone = 1 AND EmpID = %d AND EOSReqNo Not In (SELECT ReqNO FROM EosTran WHERE EmpID = %d AND EndofserviceType in ('5','8'))",sTableName,lEmpID,lEmpID);
                    RetVal = ConnectionFunctions.Connect_SQLScalar(ref qryRslt, "SELECT Count(ReqNo) As Cnt FROM " + sTableName + " WHERE IsCalcDone = 1 AND EmpID = " + lEmpID + " and EOSReqNo Not In (SELECT ReqNO FROM EosTran WHERE EmpID = " + lEmpID + " AND EndofserviceType in ('5','8'))", ref Params, ref Conn, ref Errmsg);
                else
                    RetVal = ConnectionFunctions.Connect_SQLScalar(ref qryRslt, "SELECT Count(ReqNo) As Cnt FROM " + sTableName + " WHERE IsCalcDone = 1 AND EmpID = " + lEmpID, ref Params, ref Conn, ref Errmsg);

                if (RetVal == true)
                {
                    int.TryParse(qryRslt, out lCnt);

                    if (lCnt > 0)
                        RetVal = true;
                    else
                        RetVal = false;
                }
            }
            catch (Exception Ex)
            {
                RetVal = false;
            }

            return RetVal;
        }


        #endregion

        //End: Robin added code for Financial bypass
    }
}
