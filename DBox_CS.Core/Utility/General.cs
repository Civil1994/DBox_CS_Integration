using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBox_CS.Core.Utility
{
    public class General
    {



        #region Date Functions
        public static string GetDate(DateTime dtSource)
        {
            int day = 0, month = 0, year = 0;
            string sday = string.Empty, smonth = string.Empty, syear = string.Empty;
            string sDate = string.Empty;
            day = dtSource.Day;
            month = dtSource.Month;
            year = dtSource.Year;
            sday = day.ToString();
            smonth = month.ToString();
            syear = year.ToString();
            sDate = sday + "/" + smonth + "/" + syear;
            return sDate;
        }

        public static string GetDateYMD(DateTime dtSource)
        {
            int day = 0, month = 0, year = 0;
            string sday = string.Empty, smonth = string.Empty, syear = string.Empty;
            string sDate = string.Empty;
            day = dtSource.Day;
            month = dtSource.Month;
            year = dtSource.Year;
            sday = day.ToString();
            smonth = month.ToString();
            syear = year.ToString();
            sDate = syear + "/" + smonth + "/" + sday;
            return sDate;
        }

        public static DateTime SetDate(string strDate)
        {
            int day = 0, month = 0, year = 0;
            string sday = string.Empty, smonth = string.Empty, syear = string.Empty;
            DateTime dtDate = new DateTime(1900, 1, 1);

            if (!string.IsNullOrEmpty(strDate))
            {

                char[] delimiter = { '/' };
                string[] sArr = strDate.Split(delimiter);
                sday = sArr[0];
                smonth = sArr[1];
                syear = sArr[2];

                day = Convert.ToInt16(sday);
                month = Convert.ToInt16(smonth);
                year = Convert.ToInt16(syear);
                dtDate = new DateTime(year, month, day);
            }

            return dtDate;
        }

        public static DateTime SetDateYMD(string strDate)
        {
            int day = 0, month = 0, year = 0;
            string sday = string.Empty, smonth = string.Empty, syear = string.Empty;
            DateTime dtDate = new DateTime(1900, 1, 1);

            char[] delimiter = { '/' };
            string[] sArr = strDate.Split(delimiter);
            syear = sArr[0];
            smonth = sArr[1];
            sday = sArr[2];

            day = Convert.ToInt16(sday);
            month = Convert.ToInt16(smonth);
            year = Convert.ToInt16(syear);

            dtDate = new DateTime(year, month, day);
            return dtDate;
        }

        public static string GetHijriDate(DateTime p_oGregDate)
        {
            DateTime lDateTime = p_oGregDate;
            System.Globalization.DateTimeFormatInfo lHijriDateInfo = null;
            try
            {
                lDateTime = Convert.ToDateTime(p_oGregDate);
                lHijriDateInfo = new System.Globalization.CultureInfo("ar-SA", false).DateTimeFormat;
                lHijriDateInfo.Calendar = new System.Globalization.HijriCalendar();
                lHijriDateInfo.ShortDatePattern = "dd/MM/yyyy";
                return ((string)lDateTime.Date.ToString(lHijriDateInfo)).Substring(0, 10);
            }
            catch (Exception ex)
            {

                return p_oGregDate.ToShortDateString();
            }
        }

        #endregion

        #region"String functions"

        public static string Left(string param, int length)
        {
            int strLength = param.Length;
            length = (strLength < length ? strLength : length);
            //we start at 0 since we want to get the characters starting from the
            //left and with the specified lenght and assign it to a variable


            string result = param.Substring(0, length);
            //return the result of the operation
            return result;
        }

        public static string Right(string param, int length)
        {
            int strLength = param.Length;
            length = (strLength < length ? strLength : length);
            //start at the index based on the lenght of the sting minus
            //the specified lenght and assign it a variable
            string result = param.Substring(param.Length - length, length);
            //return the result of the operation
            return result;
        }

        public static string Mid(string param, int startIndex, int length)
        {
            //start at the specified index in the string ang get N number of
            //characters depending on the lenght and assign it to a variable
            string result = param.Substring(startIndex, length);
            //return the result of the operation
            return result;
        }

        public static string Mid(string param, int startIndex)
        {
            //start at the specified index and return all characters after it
            //and assign it to a variable
            string result = param.Substring(startIndex - 1);
            //return the result of the operation
            return result;
        }
        public static string Trim_Loc(string locstr, int length)
        {
            try
            {
                locstr = locstr.Trim();
                if (locstr.Length > length)
                {
                    // locstr = "..." + Trim(Right(locstr, length - 3))
                    //locstr = "..." + locstr.Substring(3, locstr.Length).Trim();
                    locstr = "..." + Right(locstr, length - 3);

                }
            }
            catch (Exception)
            {

            }
            return locstr;
        }

        #endregion

        #region R O U N D I N G    F U N C T I O N S

        public static byte GetCurrencyDecimalPlaces(int empId, string strConn)
        {
            byte DecimalPlaces;
            try
            {
                string sQry = "SELECT [dbo].[fn_GetCurrencyDecimalPlaces](" + empId + ")";

                SqlConnection sqlConn = new SqlConnection(strConn);
                SqlCommand sqlCmd = new SqlCommand(sQry, sqlConn);

                sqlConn.Open();

                DecimalPlaces = Convert.ToByte(sqlCmd.ExecuteScalar());

                sqlCmd.Dispose();
                sqlConn.Close();
            }
            catch
            {
                DecimalPlaces = 2;
            }
            return DecimalPlaces;
        }

        public static string Round(string amount, byte decimalPlaces)
        {
            Decimal AmtDec;
            if (Decimal.TryParse(amount, out AmtDec))
            {
                string format = "";
                if (decimalPlaces <= 0)
                {
                    return Math.Round(AmtDec, decimalPlaces, MidpointRounding.AwayFromZero).ToString();
                }
                else
                {
                    if (decimalPlaces == 1)
                    {
                        format = "0.0";
                    }
                    else if (decimalPlaces == 2)
                    {
                        format = "0.00";
                    }
                    else if (decimalPlaces == 2)
                    {
                        format = "0.00";
                    }
                    else if (decimalPlaces == 3)
                    {
                        format = "0.000";
                    }
                    else if (decimalPlaces == 4)
                    {
                        format = "0.0000";
                    }
                    else if (decimalPlaces == 5)
                    {
                        format = "0.00000";
                    }
                }
                return Math.Round(AmtDec, decimalPlaces, MidpointRounding.AwayFromZero).ToString(format);
            }
            else
                return string.Empty;
        }
        public static string RoundWithCommas(int empId, Decimal amount, string strConn)
        {

            string sAmt = "";
            try
            {
                byte DecimalPlaces = GetCurrencyDecimalPlaces(empId, strConn);
                amount = Math.Round(amount, DecimalPlaces, MidpointRounding.AwayFromZero);
                switch (DecimalPlaces)
                {
                    case 1:
                        sAmt = Convert.ToDecimal(amount).ToString("#,##0.0");
                        break;
                    case 2:
                        sAmt = Convert.ToDecimal(amount).ToString("#,##0.00");
                        break;
                    case 3:
                        sAmt = Convert.ToDecimal(amount).ToString("#,##0.000");
                        break;
                    case 4:
                        sAmt = Convert.ToDecimal(amount).ToString("#,##0.0000");
                        break;
                    default:
                        sAmt = Convert.ToDecimal(amount).ToString("#,##0.00");
                        break;

                }
            }
            catch
            {

            }

            return sAmt;
        }
        public static string Round(int empId, string amount, string strConn)
        {
            byte DecimalPlaces = GetCurrencyDecimalPlaces(empId, strConn);

            return Round(amount, DecimalPlaces);
        }

        public static Decimal Round(int empId, Decimal amount, string strConn)
        {
            string RoundedStr = Round(empId, amount.ToString(), strConn);
            return Decimal.Parse(RoundedStr);
        }

        public static Single Round(int empId, Single amount, string strConn)
        {
            string RoundedStr = Round(empId, amount.ToString(), strConn);
            return Single.Parse(RoundedStr);
        }

        public static Double Round(int empId, Double amount, string strConn)
        {
            string RoundedStr = Round(empId, amount.ToString(), strConn);
            return Double.Parse(RoundedStr);
        }

        public static string RoundBalance(string balance)
        {
            return Round(balance, 2);
        }

        #endregion


        public static int GetLastDayOfMonth(int nMonth, int nYear)
        {
            int nDay;

            if (nYear > 9999 || nYear < 0)
                return 0;

            switch (nMonth)
            {
                case 1:
                case 3:
                case 5:
                case 7:
                case 8:
                case 10:
                case 12:
                    nDay = 31;
                    break;
                case 4:
                case 6:
                case 9:
                case 11:
                    nDay = 30;
                    break;
                case 2:
                    {
                        if (nYear % 400 == 0 || (nYear % 100 != 0 && nYear % 4 == 0)) //Leap Year Checking......

                            nDay = 29;
                        else
                            nDay = 28;
                    }
                    break;
                default:
                    nDay = 0;
                    break;
            }
            return nDay;
        }

        public static DateTime RetDateSince(DateTime nCurrDate, int nTicketEvery, Boolean nFwdBack, string TicketEveryType)
        {

            //robin modified code for TicketEveryType
            if (TicketEveryType == AppClass.FixedMembers.TicketEveryType_Days)
            {
                if (nFwdBack)
                {
                    //deduct the days
                    nCurrDate = nCurrDate.AddDays(-nTicketEvery);
                }
                else
                {
                    nCurrDate = nCurrDate.AddDays(nTicketEvery);
                }
            }
            else
            {
                //If Variable is True, then we have to go back the same span since we started
                int nYy = 0; int nMm = 0;
                nYy = nCurrDate.Year;
                nMm = nCurrDate.Month;

                if (nFwdBack)
                {
                    //Loop those many months, as many as u want to go back
                    for (int i = 0; i < nTicketEvery; i++)
                    {
                        //First deduct the days in the current month and then go to previous month
                        nCurrDate = nCurrDate.AddDays(-GetLastDayOfMonth(nMm, nYy));
                        if (nMm == 1)
                        {
                            nMm = 12;
                            nYy--;
                        }
                        else
                            nMm--;
                    }
                }
                else
                {
                    for (int i = 0; i < nTicketEvery; i++)
                    {
                        nCurrDate = nCurrDate.AddDays(GetLastDayOfMonth(nMm, nYy));

                        if (nMm == 12)
                        {
                            nMm = 1;
                            nYy++;
                        }
                        else
                            nMm++;
                    }
                }
            }



            return nCurrDate;
        }
    }
}
