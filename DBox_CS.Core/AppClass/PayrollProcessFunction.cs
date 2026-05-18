using DBox_CS.Core.DALayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBox_CS.Core.AppClass
{
    public class PayrollProcessFunction
    {
        public static bool ReturnSLReInitDate(ref int EmpID, ref DateTime dtTranDate, ref DateTime dt_SLReInitCurr, ref DateTime dt_SLReInitPrev, ref string cs_CoProfileCode, ref SqlConnection Conn, ref string ErrMsg)
        {

            bool RetVal = false;
            SqlDataReader MyReader = null/* TODO Change to default(_) if this is not a reference type */;
            try
            {
                string sResult = "";
                SqlParameter[] Params = null;
                DateTime dt_JoiningDate = new DateTime(1900, 1, 1);
                DateTime dt_FirstSLDate = new DateTime(1900, 1, 1);
                string cs_Temp = string.Empty;
                short nSLOption = 0;
                short SLReInitMth = 0;
                short SLReInitDay = 0;
                bool ReInit = false;
                string TableColumnStatus = "0";
                //TableColumnStatus = GetTableStatus(ErrMsg);


                // SRINI NOTE PROFILE CODE added by srini  WHERE ProfileCode = '%s'"
                // Nishad Edited 26092016
                // If cs_CoProfileCode <> "" And TableColumnStatus = "1" Then
                // cs_Temp = "Select SLReInitDateAs, FiscalYrStartDay, FiscalYrStartMnth from PrgDefault1 WHERE ProfileCode = '" & cs_CoProfileCode & "'"
                // Else
                // cs_Temp = "Select SLReInitDateAs, FiscalYrStartDay, FiscalYrStartMnth from PrgDefault"
                // End If

                if (cs_CoProfileCode != "" & cs_CoProfileCode != string.Empty)
                    cs_Temp = "Select SLReInitDateAs, FiscalYrStartDay, FiscalYrStartMnth from PrgDefault1 WHERE ProfileCode = '" + cs_CoProfileCode + "'";
                else
                    cs_Temp = "Select SLReInitDateAs, FiscalYrStartDay, FiscalYrStartMnth from PrgDefault";
                // Nishad End Edit 26092016

                RetVal = ConnectionFunctions.Connect_SQLDataReader(ref MyReader, cs_Temp, ref ErrMsg, ref Conn);
                if (RetVal == true)
                {
                    if (MyReader.HasRows)
                    {
                        MyReader.Read();
                        nSLOption = Convert.ToInt16(MyReader[0]);
                        SLReInitDay = Convert.ToInt16(MyReader[1]);
                        SLReInitMth = Convert.ToInt16(MyReader[2]);
                    }
                    MyReader.Close();
                }
                // SRINI NOTE (Transferdate IS MISSING) added by srini
                if (nSLOption == 0)
                {
                    // If TableColumnStatus = "1" Then
                    cs_Temp = "SELECT JoiningDate FROM FinMast WITH (NOLOCK) WHERE EmpID = " + EmpID + " AND IsNULL(Transferdate,'01/01/1900') = '01/01/1900'";
                    // Nishad End Edit 28052014
                    // cs_Temp = "SELECT JoiningDate FROM FinMast WHERE EmpID = " & EmpID
                    RetVal = ConnectionFunctions.Connect_SQLScalar(ref sResult, cs_Temp, ref Params, ref Conn, ref ErrMsg);
                    DateTime.TryParse(sResult, out dt_JoiningDate);

                    if (Common.IsSLReinintilalization3Years())
                    {
                        DateTime dt_JoiningDt = dt_JoiningDate.Date;
                        while (dt_JoiningDt.AddYears(3).Year <= dtTranDate.Year)
                            dt_JoiningDt = dt_JoiningDt.AddYears(3);

                        dt_SLReInitPrev = new DateTime(dt_JoiningDt.Year, dt_JoiningDt.Month, dt_JoiningDt.Day);
                        dt_SLReInitCurr = dt_SLReInitPrev.AddYears(3);
                        if (dt_SLReInitPrev.Year == dtTranDate.Year & dt_SLReInitPrev.Month >= dtTranDate.Month)
                        {
                            dt_SLReInitCurr = new DateTime(dt_JoiningDt.Year, dt_JoiningDt.Month, dt_JoiningDt.Day);
                            dt_SLReInitPrev = dt_SLReInitPrev.AddYears(-3);
                        }
                    }
                    else
                    {
                        dt_SLReInitCurr = new DateTime(dtTranDate.Year, dt_JoiningDate.Month, dt_JoiningDate.Day);
                        // Shyamjith Modified on 10/03/2020 to fix leap year issue
                        // dt_SLReInitPrev = New Date(dtTranDate.Year - 1, dt_JoiningDate.Month, dt_JoiningDate.Day)
                        dt_SLReInitPrev = dt_SLReInitCurr.AddYears(-1);

                        // If this years re-initialization has already been passed in the previous months then reverse the above cycle
                        if (dt_SLReInitCurr.Month < dtTranDate.Month & dt_SLReInitCurr.Year <= dtTranDate.Year)
                        {
                            // dt_SLReInitCurr = New Date(dtTranDate.Year + 1, dt_JoiningDate.Month, dt_JoiningDate.Day)
                            dt_SLReInitPrev = new DateTime(dtTranDate.Year, dt_JoiningDate.Month, dt_JoiningDate.Day);
                            // Shyamjith Modified on 10/03/2020 to fix leap year issue
                            dt_SLReInitCurr = dt_SLReInitPrev.AddYears(1);
                        }
                        if (dt_SLReInitPrev < dt_JoiningDate)
                            dt_SLReInitPrev = dt_JoiningDate;
                    }
                }
                else if (nSLOption == 1)
                {
                    dt_SLReInitCurr = new DateTime(dtTranDate.Year, SLReInitMth, SLReInitDay);
                    // Shyamjith Modified on 10/03/2020 to fix leap year issue
                    // dt_SLReInitPrev = New Date(dtTranDate.Year - 1, SLReInitMth, SLReInitDay)
                    dt_SLReInitPrev = dt_SLReInitCurr.AddYears(-1);
                    // If this years re-initialization has already been passed in the previous months then reverse the above cycle
                    if (dt_SLReInitCurr.Month < dtTranDate.Month & dt_SLReInitCurr.Year <= dtTranDate.Year)
                    {
                        dt_SLReInitCurr = new DateTime(dtTranDate.Year + 1, SLReInitMth, SLReInitDay);
                        // Shyamjith Modified on 10/03/2020 to fix leap year issue
                        // dt_SLReInitPrev = New Date(dtTranDate.Year, SLReInitMth, SLReInitDay)
                        dt_SLReInitPrev = dt_SLReInitCurr.AddYears(-1);
                    }
                }
                else if (nSLOption == 2)
                {
                    cs_Temp = "Select  IsNULL(FirstSLDate,'01/01/1900') FirstSLDate from FirstSLDet where EmpId = " + EmpID + "";
                    RetVal = ConnectionFunctions.Connect_SQLScalar(ref sResult, cs_Temp, ref Params, ref Conn, ref ErrMsg);
                    DateTime.TryParse(sResult, out dt_FirstSLDate);

                    SLReInitMth = Convert.ToInt16(dt_FirstSLDate.Month);
                    SLReInitDay = Convert.ToInt16(dt_FirstSLDate.Day);
                    dt_SLReInitCurr = new DateTime(dtTranDate.Year, SLReInitMth, SLReInitDay);
                    dt_SLReInitPrev = dt_SLReInitCurr.AddYears(-1);
                    // If this years re-initialization has already been passed in the previous months then reverse the above cycle
                    if (dt_SLReInitCurr.Month < dtTranDate.Month & dt_SLReInitCurr.Year <= dtTranDate.Year)
                    {
                        dt_SLReInitPrev = new DateTime(dtTranDate.Year, SLReInitMth, SLReInitDay);
                        dt_SLReInitCurr = dt_SLReInitPrev.AddYears(1);
                    }
                }


                RetVal = true;
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

        public static bool CalculateLeaveBal(long lEmpID, ref double nAbsentDays, ref DateTime EffDt, ref double fALBalCaltd, double fAlTaken, ref string errmsg, double dSBD = 0.0, double dSBD_Bkd = 0.0, int nNpDays = 0)
        {
            List<double> nALBalEntitleArr = new List<double>();
            string m_ALCode = "";
            if (!Get_ALBalEntitle(lEmpID, EffDt, ref m_ALCode, ref errmsg))
                return false;
            double fALBalAccd = 0.0;
            double fLimit = 0.0;
            string sQry;
            string sResult = "";
            sQry = "Select ALBal From EmpBals  with(Nolock) Where EmpID = " + lEmpID + " AND RecordNo In (Select RecordNo From Finmast WHERE Empid = " + lEmpID + " AND IsNULL(Transferdate,'01/01/1900') = '01/01/1900')";
            if (!ConnectionFunctions.Connect_SQLScalar(ref sResult, sQry, ref errmsg))
            {
                errmsg += sQry;
                return false;
            }
            else if (!string.IsNullOrEmpty(sResult))
            {
                fALBalAccd = Convert.ToDouble(sResult);
            }
            sResult = "";
            sQry = "Select LIMITLEAVEBAL FROM SALARYPROFILE with(Nolock)  Where Code in (Select Salprofile from Finmast WHERE EmpID = " + lEmpID + "  AND IsNULL(Transferdate,'01/01/1900') = '01/01/1900')";
            if (!ConnectionFunctions.Connect_SQLScalar(ref sResult, sQry, ref errmsg))
            {
                errmsg += sQry;
                return false;
            }
            else if (!string.IsNullOrEmpty(sResult))
            {
                fLimit = Convert.ToDouble(sResult);
            }
            SqlDataReader dr = null;

            DateTime dtLPDate = DateTime.MinValue, dtJoiningDate = DateTime.MinValue;
            //Denson commented and added below code  added 08042021 
            //sQry = "Select LastPaidDate, JoiningDate From Finmast with(Nolock)  Where EmpID = " + lEmpID + " AND IsNULL(Transferdate,'01/01/1900') = '01/01/1900'";
            //if (!ConnectionFunctions.Connect_SQLDataReader(ref dr, sQry, ref errmsg))
            //{
            //    errmsg += sQry;
            //    return false;
            //}
            //else
            //{
            //    if (dr.HasRows)
            //    {
            //        dr.Read();
            //        dtLPDate = Convert.ToDateTime(dr["LastPaidDate"]);
            //        dtJoiningDate = Convert.ToDateTime(dr["JoiningDate"]);

            //    }
            //    dr.Close();
            //}
            //double fBalanceDays = Convert.ToDouble((EffDt.Subtract(dtLPDate)).Days);
            DateTime dtFirstFinEff = new DateTime(1900, 1, 1);
            int nstatus = 0, nFinFlag = 0;
            sQry = "Select LastPaidDate, JoiningDate,Status,IsNull(FinFlag,0) FinFlag,FirstFinEffdt From Finmast with(Nolock)  Where EmpID = " + lEmpID + " AND IsNULL(Transferdate,'01/01/1900') = '01/01/1900'";
            if (!ConnectionFunctions.Connect_SQLDataReader(ref dr, sQry, ref errmsg))
            {
                errmsg += sQry;
                return false;
            }
            else
            {
                if (dr.HasRows)
                {
                    dr.Read();
                    if (!dr.IsDBNull(dr.GetOrdinal("LastPaidDate")))
                        dtLPDate = Convert.ToDateTime(dr["LastPaidDate"]);
                    if (!dr.IsDBNull(dr.GetOrdinal("JoiningDate")))
                        dtJoiningDate = Convert.ToDateTime(dr["JoiningDate"]);
                    if (!dr.IsDBNull(dr.GetOrdinal("Status")))
                        nstatus = Convert.ToInt32(dr["Status"]);
                    if (!dr.IsDBNull(dr.GetOrdinal("FinFlag")))
                        nFinFlag = Convert.ToInt32(dr["FinFlag"]);
                    if (!dr.IsDBNull(dr.GetOrdinal("FirstFinEffdt")))
                        dtFirstFinEff = Convert.ToDateTime(dr["FirstFinEffdt"]);

                }
                dr.Close();
            }
            double fBalanceDays = 0.0;
            if ((nstatus == 20 || nstatus == 21) && nFinFlag == 1)
            {
                //fBalanceDays = Convert.ToDouble((EffDt.Subtract(dtFirstFinEff)).Days);
                fBalanceDays = Convert.ToDouble((EffDt.Subtract(dtJoiningDate)).Days) + 1;
            }
            else
            {
                fBalanceDays = Convert.ToDouble((EffDt.Subtract(dtLPDate)).Days);
            }
            if (fBalanceDays == 366)
                fBalanceDays = 365;

            fBalanceDays -= nAbsentDays;
            //Denson added 30/05/2023 Gisco
            fBalanceDays -= dSBD;
            fBalanceDays += nNpDays;//Denson added 13/07/2023 Yas Case

            DateTime dtTemp = DateTime.MinValue;
            DateTime dtTemp2 = DateTime.MinValue;

            string strQry;
            long nType = 0;
            //strQry = "Select Type from AlEntitlements with(Nolock)  Where Code='" + m_ALCode + "'";
            strQry = "Select IsNULL(Type,1) from AlEntitlements with(Nolock)  Where Code='" + m_ALCode + "'"; //Neppolian added 15/01/2025 Ssun case
            sResult = "";
            if (!ConnectionFunctions.Connect_SQLScalar(ref sResult, strQry, ref errmsg))
            {
                errmsg += strQry;
                return false;
            }
            else if (!string.IsNullOrEmpty(sResult))
            {
                nType = Convert.ToInt32(sResult);
            }
            switch (nType)
            {
                case 2:  //  Std No. of Days
                    {
                        strQry = "";
                        strQry = "Select col1,col2 From alentitlementssec with(Nolock)  where Code= '" + m_ALCode + "'";
                        double noDays, wrkDays;
                        noDays = wrkDays = 0.0;
                        //Denson added 30/05/2023 Gisco
                        double dSBDbal, dSBD_BkdOld, dSBD_BkdNew;
                        dSBDbal = dSBD_BkdOld = dSBD_BkdNew = 0.0;
                        if (!ConnectionFunctions.Connect_SQLDataReader(ref dr, strQry, ref errmsg))
                        {
                            return false;
                        }
                        else
                        {
                            if (dr.HasRows)
                            {
                                dr.Read();
                                noDays = Convert.ToDouble(dr["col1"]);
                                wrkDays = Convert.ToDouble(dr["col2"]);

                            }
                            dr.Close();
                        }
                        if ((noDays == 0.0) || (wrkDays == 0.0))
                            fALBalCaltd = 0.0;
                        else
                        {
                            fALBalCaltd = (((noDays) / (wrkDays)) * fBalanceDays);
                            //Denson added 30/05/2023 Gisco
                            if (dSBD != 0.0)
                                dSBDbal = ((30.0 / 365.0) * dSBD);
                            if (dSBD_Bkd != 0.0)
                            {
                                dSBD_BkdOld = (((noDays) / (wrkDays)) * dSBD_Bkd);
                                dSBD_BkdNew = ((30.0 / 365.0) * dSBD_Bkd);
                            }
                            fALBalCaltd = fALBalCaltd + dSBDbal - dSBD_BkdOld + dSBD_BkdNew;
                            //Denson Stopped 30/05/2023 Gisco
                            if (fLimit > 0.0)
                            {
                                double fCAPPEDAlbal = 0.0;
                                fCAPPEDAlbal = (noDays * fLimit / 100.0);

                                if ((fALBalAccd + fALBalCaltd - fAlTaken) > fCAPPEDAlbal)
                                {
                                    if ((fALBalAccd - fAlTaken) <= fCAPPEDAlbal)
                                    {
                                        //fALBalCaltd = fCAPPEDAlbal - fALBalAccd;
                                        fALBalCaltd = fCAPPEDAlbal - (fALBalAccd - fAlTaken);
                                    }
                                    else
                                        fALBalCaltd = 0.0;
                                }
                            }
                        }
                        break;
                    }
                //Alternate Years	
                case 3:     // 17 Days/1st Year & 30 Days/2nd Year
                    {
                        strQry = "";
                        strQry = "Select col2 From alentitlementssec with(Nolock)  where Code= '" + m_ALCode + "' And Slab = 1";
                        double nDays1 = 0, nDays2 = 0;
                        if (!ConnectionFunctions.Connect_SQLDataReader(ref dr, strQry, ref errmsg))
                        {
                            return false;
                        }
                        else
                        {
                            if (dr.HasRows)
                            {
                                dr.Read();
                                nDays1 = Convert.ToDouble(dr["col2"]);

                            }
                            dr.Close();
                        }
                        strQry = "Select col2 From alentitlementssec with(Nolock)  where Code= '" + m_ALCode + "' And Slab = 2";
                        if (!ConnectionFunctions.Connect_SQLDataReader(ref dr, strQry, ref errmsg))
                        {
                            return false;
                        }
                        else
                        {
                            if (dr.HasRows)
                            {
                                dr.Read();
                                nDays2 = Convert.ToDouble(dr["col2"]);

                            }
                            dr.Close();
                        }
                        if (dtJoiningDate.Day == 1)
                        {
                            if (dtJoiningDate.Month == 1)
                            {
                                dtTemp = new DateTime(dtJoiningDate.Year, 12, 31);
                            }
                            else
                            {
                                if (dtJoiningDate.Month == 2 || dtJoiningDate.Month == 4 || dtJoiningDate.Month == 6 || dtJoiningDate.Month == 8 || dtJoiningDate.Month == 9 || dtJoiningDate.Month == 11)
                                    dtTemp = new DateTime(dtJoiningDate.Year + 1, dtJoiningDate.Month - 1, 31);
                                else if (dtJoiningDate.Month == 3)
                                {
                                    if ((dtJoiningDate.Year + 1) % 4 == 0)
                                        dtTemp = new DateTime(dtJoiningDate.Year + 1, dtJoiningDate.Month - 1, 29);
                                    else
                                        dtTemp = new DateTime(dtJoiningDate.Year + 1, dtJoiningDate.Month - 1, 28);
                                }
                                else
                                    dtTemp = new DateTime(dtJoiningDate.Year + 1, dtJoiningDate.Month - 1, 30);
                            }
                        }
                        else
                        {
                            dtTemp = new DateTime(dtJoiningDate.Year + 1, dtJoiningDate.Month, dtJoiningDate.Day - 1);
                        }
                        if (EffDt <= dtTemp)
                        {
                            fALBalCaltd = ((nDays1 / 365.0) * fBalanceDays);
                        }
                        else
                        {
                            //Anil 16092000
                            double ftotdays, fTotYears, fDiffMod;
                            ftotdays = fTotYears = fDiffMod = 0.0;
                            ftotdays = (EffDt - dtJoiningDate).Days;//will return the no. of days
                            fTotYears = ftotdays / 365.0;
                            fDiffMod = fTotYears % 2.0;
                            // fDiffMod = fmodl(fTotYears,2.0);
                            //now if the variable fdiffMod contains more than 1
                            if (fDiffMod <= 1.0)
                                fALBalCaltd = ((nDays1 / 365.0) * fBalanceDays);
                            else
                                fALBalCaltd = ((nDays2 / 365.0) * fBalanceDays);
                            //Anil 16092000
                        }
                        break;
                    }
                //Slabs
                case 4:		// 21 Days/1st-10th Year & 24 Days/Year After
                    {
                        strQry = "";
                        strQry = "Select col1,col2 From AlEntitlementsSec  with(Nolock) Where Code = '" + m_ALCode + "' And Slab = 1";
                        double noDays1 = 0.0, noDays2 = 0.0, noDays3 = 0.0, noDays = 0.0;
                        double years = 0, years2 = 0;
                        if (!ConnectionFunctions.Connect_SQLDataReader(ref dr, strQry, ref errmsg))
                        {
                            return false;
                        }
                        else
                        {
                            if (dr.HasRows)
                            {
                                dr.Read();
                                years = Convert.ToDouble(dr["col1"]);
                                noDays1 = Convert.ToDouble(dr["col2"]);

                            }
                            dr.Close();
                        }
                        dr.Dispose();
                        strQry = "Select col1,col2 From AlEntitlementsSec with(Nolock)  Where Code = '" + m_ALCode + "' And Slab = 3";
                        if (!ConnectionFunctions.Connect_SQLDataReader(ref dr, strQry, ref errmsg))
                        {
                            return false;
                        }
                        else
                        {
                            if (dr.HasRows)
                            {
                                dr.Read();
                                years2 = Convert.ToDouble(dr["col1"]);
                                noDays2 = Convert.ToDouble(dr["col2"]);
                                dr.Close();
                            }
                        }
                        strQry = "Select col2 From AlEntitlementsSec  with(Nolock) Where Code = '" + m_ALCode + "' And Slab = 2";
                        sResult = "";
                        if (!ConnectionFunctions.Connect_SQLScalar(ref sResult, strQry, ref errmsg))
                        {
                            errmsg += strQry;
                            return false;
                        }
                        else if (!string.IsNullOrEmpty(sResult))
                        {
                            noDays3 = Convert.ToDouble(sResult);
                        }

                        //Denson modified 05/07/2021
                        int Mnth = 0;
                        int Mnth2 = 0;
                        if (years > 0.0)
                        {
                            Mnth = Convert.ToInt32(years * 12);
                            dtTemp = dtJoiningDate.AddMonths(Mnth).AddDays(-1);
                        }
                        if (years2 > 0)
                        {
                            Mnth2 = Convert.ToInt32(years2 * 12);
                            dtTemp2 = dtJoiningDate.AddMonths(Mnth + Mnth2).AddDays(-1);
                        }

                        /*
                        if (dtJoiningDate.Day == 1)
                        {
                            if (dtJoiningDate.Month == 1)
                            {
                                if (years == 0.5)
                                {
                                    sResult = "";
                                    ConnectionFunctions.Connect_SQLScalar(ref sResult, "SELECT DATEADD(mm, 6, '" + dtJoiningDate.ToString("yyyy/MM/dd") + "') - 1", ref errmsg);
                                    if (!string.IsNullOrEmpty(sResult))
                                        dtTemp = Convert.ToDateTime(sResult);
                                }
                                else
                                    dtTemp = new DateTime(dtJoiningDate.Year + ((int)years - 1), 12, 31);

                                if (years2 > 0)
                                {
                                    if (years2 == 0.5 && years == 0.5)
                                    {
                                        dtTemp2 = new DateTime(dtJoiningDate.Year, 12, 31);
                                    }
                                    else
                                        dtTemp2 = new DateTime(dtJoiningDate.Year + ((int)years + (int)years2 - 1), 12, 31);
                                }
                            }
                            else
                            {
                                if (dtJoiningDate.Month == 2 || dtJoiningDate.Month == 4 || dtJoiningDate.Month == 6 || dtJoiningDate.Month == 8 || dtJoiningDate.Month == 9 || dtJoiningDate.Month == 11)
                                {
                                    if (years == 0.5)
                                    {
                                        sResult = "";
                                        ConnectionFunctions.Connect_SQLScalar(ref sResult, "SELECT DATEADD(mm, 6, '" + dtJoiningDate.ToString("yyyy/MM/dd") + "') - 1", ref errmsg);
                                        if (!string.IsNullOrEmpty(sResult))
                                            dtTemp = Convert.ToDateTime(sResult);
                                    }
                                    else
                                        dtTemp = new DateTime(dtJoiningDate.Year + (int)years, dtJoiningDate.Month - 1, 31);

                                    if (years2 > 0)
                                    {
                                        if (years2 == 0.5 && years == 0.5)
                                        {
                                            dtTemp2 = new DateTime(dtJoiningDate.Year + 1, dtJoiningDate.Month - 1, 31);
                                        }
                                        else
                                            dtTemp2 = new DateTime(dtJoiningDate.Year + (int)years + (int)years2, dtJoiningDate.Month - 1, 31);
                                    }
                                }
                                else if (dtJoiningDate.Month == 3)
                                {
                                    if (years == 0.5)
                                    {
                                        sResult = "";
                                        ConnectionFunctions.Connect_SQLScalar(ref sResult, "SELECT DATEADD(mm, 6, '" + dtJoiningDate.ToString("yyyy/MM/dd") + "') - 1", ref errmsg);
                                        if (!string.IsNullOrEmpty(sResult))
                                            dtTemp = Convert.ToDateTime(sResult);
                                    }
                                    else
                                    {
                                        if ((dtJoiningDate.Year + years) % 4 == 0)
                                            dtTemp = new DateTime(dtJoiningDate.Year + (int)years, dtJoiningDate.Month - 1, 29);
                                        else
                                            dtTemp = new DateTime(dtJoiningDate.Year + (int)years, dtJoiningDate.Month - 1, 28);
                                    }

                                    if (years2 > 0)
                                    {
                                        if (years2 == 0.5 && years == 0.5)
                                        {
                                            if ((dtJoiningDate.Year + years + years2) % 4 == 0)
                                                dtTemp2 = new DateTime(dtJoiningDate.Year + 1, dtJoiningDate.Month - 1, 29);
                                            else
                                                dtTemp2 = new DateTime(dtJoiningDate.Year + 1, dtJoiningDate.Month - 1, 28);
                                        }
                                        else
                                        {
                                            if ((dtJoiningDate.Year + years + years2) % 4 == 0)
                                                dtTemp2 = new DateTime(dtJoiningDate.Year + (int)years + (int)years2, dtJoiningDate.Month - 1, 29);
                                            else
                                                dtTemp2 = new DateTime(dtJoiningDate.Year + (int)years + (int)years2, dtJoiningDate.Month - 1, 28);
                                        }

                                    }
                                }
                                else
                                {
                                    if (years == 0.5)
                                    {
                                        sResult = "";
                                        ConnectionFunctions.Connect_SQLScalar(ref sResult, "SELECT DATEADD(mm, 6, '" + dtJoiningDate.ToString("yyyy/MM/dd") + "') - 1", ref errmsg);
                                        if (!string.IsNullOrEmpty(sResult))
                                            dtTemp = Convert.ToDateTime(sResult);
                                    }
                                    else
                                        dtTemp = new DateTime(dtJoiningDate.Year + (int)years, dtJoiningDate.Month - 1, 30);

                                    if (years2 > 0)
                                    {
                                        if (years2 == 0.5 && years == 0.5)
                                        {
                                            dtTemp2 = new DateTime(dtJoiningDate.Year + 1, dtJoiningDate.Month - 1, 30);
                                        }
                                        else
                                            dtTemp2 = new DateTime(dtJoiningDate.Year + (int)years + (int)years2, dtJoiningDate.Month - 1, 30);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (years == 0.5)
                            {
                                sResult = "";
                                ConnectionFunctions.Connect_SQLScalar(ref sResult, "SELECT DATEADD(mm, 6, '" + dtJoiningDate.ToString("yyyy/MM/dd") + "') - 1", ref errmsg);
                                if (!string.IsNullOrEmpty(sResult))
                                    dtTemp = Convert.ToDateTime(sResult);
                            }
                            else
                                dtTemp = new DateTime(dtJoiningDate.Year + (int)years, dtJoiningDate.Month, dtJoiningDate.Day - 1);

                            if (years2 > 0)
                            {
                                if (years2 == 0.5 && years == 0.5)
                                {
                                    dtTemp2 = new DateTime(dtJoiningDate.Year + 1, dtJoiningDate.Month, dtJoiningDate.Day - 1);
                                }
                                else
                                    dtTemp2 = new DateTime(dtJoiningDate.Year + (int)years + (int)years2, dtJoiningDate.Month, dtJoiningDate.Day - 1);
                            }
                        }

                        */
                        if (EffDt <= dtTemp)
                        {
                            fALBalCaltd = ((noDays1 / 365.0) * fBalanceDays);
                            noDays = noDays1;
                        }
                        else if (EffDt > dtTemp && EffDt <= dtTemp2)
                        {
                            fALBalCaltd = ((noDays2 / 365.0) * fBalanceDays);
                            noDays = noDays2;
                        }
                        else
                        {
                            fALBalCaltd = ((noDays3 / 365.0) * fBalanceDays);
                            noDays = noDays3;
                        }
                        if (fLimit > 0.0)
                        {
                            double fCAPPEDAlbal = 0.0;
                            fCAPPEDAlbal = (noDays * fLimit / 100.0);

                            if ((fALBalAccd + fALBalCaltd - fAlTaken) > fCAPPEDAlbal)
                            {
                                if ((fALBalAccd - fAlTaken) <= fCAPPEDAlbal)
                                {
                                    //fALBalCaltd = fCAPPEDAlbal - fALBalAccd;
                                    fALBalCaltd = fCAPPEDAlbal - (fALBalAccd - fAlTaken);
                                }
                                else
                                    fALBalCaltd = 0.0;
                            }
                        }

                        break;
                    }
            }

            return true;
        }


        public static bool Get_ALBalEntitle(long lEmpID, DateTime dt_EffDate, ref string m_ALCode, ref string errmsg)
        {

            try
            {
                DataTable dt = new DataTable();
                string sqry = "Exec Eff_ALBalEntitle '" + lEmpID + "','" + dt_EffDate.ToString("yyyy/MM/dd") + "'";
                if (!ConnectionFunctions.Connect_SQLDataTable(ref dt, sqry, ref errmsg))
                {
                    errmsg += " Code:XGAEX";
                    return false;
                }
                if (dt.Rows.Count > 0)
                {
                    m_ALCode = dt.Rows[0]["ALCode"].ToString();
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message + " Code: XGAEX";
                return false;
            }

            return true;
        }
    }
}
