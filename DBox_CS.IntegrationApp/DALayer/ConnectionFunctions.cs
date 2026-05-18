using SF_CS.IntegrationApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF_CS.IntegrationApp.DALayer
{
    public class ConnectionFunctions
    {

        public static bool bTranStarted = false;
        public static SqlTransaction SQLTran;

        #region "Execute DataReader"

        public static bool Connect_SQLDataReader(ref SqlDataReader MyReader, string SQLQuery, ref string ErrMsg)
        {

            SqlParameter[] Params = null;
            return Connect_SQLDataReader(ref MyReader, SQLQuery, ref ErrMsg, Params, CommandType.Text);

        }

        //    public static bool Connect_SQLDataReader(ref SqlDataReader MyReader, string SQLQuery, ref string ErrMsg, SqlParameter[] Params, [System.Runtime.InteropServices.OptionalAttribute, System.Runtime.InteropServices.DefaultParameterValueAttribute(CommandType.Text)] // ERROR: Optional parameters aren't supported in C# CommandType cmdType) 
        public static bool Connect_SQLDataReader(ref SqlDataReader MyReader, string SQLQuery, ref string ErrMsg, SqlParameter[] Params, CommandType cmdType)
        {

            bool RetVal = true;

            SqlConnection HRPConn = new SqlConnection();
            try
            {

                HRPConn.ConnectionString = GetConnectionString();
                SqlCommand catCMD = new SqlCommand(SQLQuery, HRPConn);
                catCMD.CommandType = cmdType;
                if ((Params != null))
                {
                    foreach (SqlParameter sqlParam in Params)
                    {
                        catCMD.Parameters.Add(sqlParam);
                    }
                }
                HRPConn.Open();
                MyReader = catCMD.ExecuteReader(CommandBehavior.CloseConnection);
            }

            catch (SqlException SQLEx)
            {
                if (HRPConn.State != 0) HRPConn.Close();
                ErrMsg = "SqlException Raised [Connect_SQLDataReader]@" + SQLEx.Message;
                RetVal = false;
            }
            catch (Exception Ex)
            {
                if (HRPConn.State != 0) HRPConn.Close();
                ErrMsg = "An Unexpected Error Has Occured.@" + Ex.Message;
                RetVal = false;
            }

            return RetVal;

        }

        public static bool Connect_SQLDataReader(ref SqlDataReader MyReader, string SQLQuery, ref string ErrMsg, ref SqlConnection sqlConn)
        {

            SqlParameter[] Params = null;
            return Connect_SQLDataReader(ref MyReader, SQLQuery, ref ErrMsg, Params, ref sqlConn, CommandType.Text);

        }

        public static bool Connect_SQLDataReader(ref SqlDataReader MyReader, string StoredProcedure, ref string ErrMsg, SqlParameter[] Params, bool IsStoredProc)
        {
            bool RetVal = true;
            SqlConnection HRPConn = new SqlConnection();
            try
            {
                HRPConn.ConnectionString = GetConnectionString();
                SqlCommand catCMD = new SqlCommand(StoredProcedure, HRPConn);
                catCMD.CommandType = CommandType.StoredProcedure;
                if ((Params != null))
                {
                    foreach (SqlParameter sqlParam in Params)
                    {
                        catCMD.Parameters.Add(sqlParam);
                    }
                }
                HRPConn.Open();
                MyReader = catCMD.ExecuteReader(CommandBehavior.CloseConnection);
            }

            catch (SqlException SQLEx)
            {
                if (HRPConn.State != 0) HRPConn.Close();
                ErrMsg = "SqlException Raised [Connect_SQLDataReader]@" + SQLEx.Message;
                RetVal = false;
            }
            catch (Exception Ex)
            {
                if (HRPConn.State != 0) HRPConn.Close();
                ErrMsg = "An Unexpected Error Has Occured.@" + Ex.Message;
                RetVal = false;
            }

            return RetVal;

        }

        //    public static bool Connect_SQLDataReader(ref SqlDataReader MyReader, string SQLQuery, ref string ErrMsg, SqlParameter[] Params, ref SqlConnection sqlConn, [System.Runtime.InteropServices.OptionalAttribute, System.Runtime.InteropServices.DefaultParameterValueAttribute(CommandType.Text)] // ERROR: Optional parameters aren't supported in C# CommandType cmdType) 
        public static bool Connect_SQLDataReader(ref SqlDataReader MyReader, string SQLQuery, ref string ErrMsg, SqlParameter[] Params, ref SqlConnection sqlConn, CommandType cmdType)
        {
            bool RetVal = true;
            try
            {


                SqlCommand catCMD = new SqlCommand(SQLQuery, sqlConn);
                catCMD.CommandTimeout = 500;
                catCMD.CommandType = cmdType;
                if ((SQLTran != null))
                {
                    if (bTranStarted == true)
                    {
                        catCMD.Transaction = SQLTran;
                    }
                }
                if ((Params != null))
                {
                    foreach (SqlParameter sqlParam in Params)
                    {
                        catCMD.Parameters.Add(sqlParam);
                    }
                }
                MyReader = catCMD.ExecuteReader();
            }
            catch (SqlException SQLEx)
            {
                ErrMsg = "SqlException Raised [Connect_SQLDataReader]@" + SQLEx.Message;
                RetVal = false;
            }
            catch (Exception Ex)
            {
                ErrMsg = "An Unexpected Error Has Occured.@" + Ex.Message;
                RetVal = false;
            }
            return RetVal;
        }
        #endregion

        #region "Execute DataTable"

        //    public static bool Connect_SQLDataTable(ref DataTable MyTable, string SQLQuery, ref string ErrMsg, SqlParameter[] Params, [System.Runtime.InteropServices.OptionalAttribute, System.Runtime.InteropServices.DefaultParameterValueAttribute(CommandType.Text)] // ERROR: Optional parameters aren't supported in C# CommandType cmdType) 
        public static bool Connect_SQLDataTable(ref DataTable MyTable, string SQLQuery, ref string ErrMsg, SqlParameter[] Params, CommandType cmdType)
        {
            bool RetVal = true;
            SqlConnection HRPConn = new SqlConnection();


            try
            {
                HRPConn.ConnectionString = GetConnectionString();
                SqlCommand catCMD = new SqlCommand(SQLQuery, HRPConn);
                catCMD.CommandType = cmdType;
                if ((Params != null))
                {
                    foreach (SqlParameter sqlParam in Params)
                    {
                        catCMD.Parameters.Add(sqlParam);
                    }
                }
                HRPConn.Open();
                SqlDataAdapter MyAdapter = new SqlDataAdapter(catCMD);
                MyAdapter.Fill(MyTable);
                MyAdapter.Dispose();
            }

            catch (SqlException SQLEx)
            {
                if (HRPConn.State != 0) HRPConn.Close();
                ErrMsg = "SqlException Raised [Connect_SQLDataReader]@" + SQLEx.Message;
                RetVal = false;
            }
            catch (Exception Ex)
            {
                if (HRPConn.State != 0) HRPConn.Close();
                ErrMsg = "An Unexpected Error Has Occured.@" + Ex.Message;
                RetVal = false;
            }
            finally
            {
                if (HRPConn.State != 0) HRPConn.Close();
            }

            return RetVal;

        }

        public static bool Connect_SQLDataTable(ref DataTable MyTable, string SQLQuery, ref SqlConnection sqlConn, SqlParameter[] Params, ref string ErrMsg)
        {

            bool RetVal = true;
            CommandType cmdType = CommandType.Text;
            try
            {
                SqlCommand catCMD = new SqlCommand(SQLQuery, sqlConn);
                if ((Params != null))
                {
                    foreach (SqlParameter sqlParam in Params)
                    {
                        catCMD.Parameters.Add(sqlParam);
                    }
                }
                SqlDataAdapter MyAdapter = new SqlDataAdapter(catCMD);
                MyAdapter.Fill(MyTable);
                MyAdapter.Dispose();
            }

            catch (SqlException SQLEx)
            {
                ErrMsg = "SqlException Raised [Connect_SQLDataReader]@" + SQLEx.Message;
                RetVal = false;
            }
            catch (Exception Ex)
            {
                ErrMsg = "An Unexpected Error Has Occured.@" + Ex.Message;
                RetVal = false;
            }

            return RetVal;

        }

        public static bool Connect_SQLDataTable(ref DataTable MyTable, string SQLQuery, ref SqlConnection sqlConn, ref string ErrMsg)
        {

            bool RetVal = true;
            try
            {
                SqlCommand catCMD = new SqlCommand(SQLQuery, sqlConn);
                SqlDataAdapter MyAdapter = new SqlDataAdapter(catCMD);
                MyAdapter.Fill(MyTable);
                MyAdapter.Dispose();
            }

            catch (SqlException SQLEx)
            {
                ErrMsg = "SqlException Raised [Connect_SQLDataReader]@" + SQLEx.Message;
                RetVal = false;
            }
            catch (Exception Ex)
            {
                ErrMsg = "An Unexpected Error Has Occured.@" + Ex.Message;
                RetVal = false;
            }

            return RetVal;

        }
        public static bool Connect_SQLDataTable(ref DataTable MyTable, string SQLQuery, ref string ErrMsg)
        {

            bool RetVal = true;
            SqlCommand catCMD = null;
            SqlConnection sqlConn = new SqlConnection();
            try
            {
                sqlConn.ConnectionString = GetConnectionString();
                catCMD = new SqlCommand(SQLQuery, sqlConn);
                catCMD.CommandTimeout = 0;
                SqlDataAdapter MyAdapter = new SqlDataAdapter(catCMD);
                MyAdapter.Fill(MyTable);
                MyAdapter.Dispose();
            }

            catch (SqlException SQLEx)
            {
                ErrMsg = "SqlException Raised [Connect_SQLDataReader]@" + SQLEx.Message;
                RetVal = false;





                //denson added 26/08/2014


                //if ((Params != null))
                //{
                //    foreach (SqlParameter sqlParam in Params)
                //    {
                //        catCMD.Parameters.Add(sqlParam);
                //    }
                //}


            }
            catch (Exception Ex)
            {
                ErrMsg = "An Unexpected Error Has Occured.@" + Ex.Message;
                RetVal = false;
            }
            finally
            {
                catCMD.Dispose();
                if (sqlConn.State != 0) sqlConn.Close();
            }

            return RetVal;

        }

        #endregion

        #region "Execute NonQuery"
        public static bool Connect_SQLNonQuery(ref int Result, string SQLQuery, ref string ErrMsg)
        {

            SqlParameter[] Params = null;
            return Connect_SQLNonQuery(ref Result, SQLQuery, ref ErrMsg, Params);

        }

        //public static bool Connect_SQLNonQuery(ref int Result, string SQLQuery, ref string ErrMsg, SqlParameter[] Params, [System.Runtime.InteropServices.OptionalAttribute, System.Runtime.InteropServices.DefaultParameterValueAttribute(CommandType.Text)] // ERROR: Optional parameters aren't supported in C# CommandType cmdType) 
        public static bool Connect_SQLNonQuery(ref int Result, string SQLQuery, ref string ErrMsg, SqlParameter[] Params)
        {

            bool RetVal = true;
            SqlConnection HRPConn = new SqlConnection();
            CommandType cmdType = CommandType.Text;
            SqlCommand catCMD = null;
            try
            {
                HRPConn.ConnectionString = GetConnectionString();
                catCMD = new SqlCommand(SQLQuery, HRPConn);
                catCMD.CommandType = cmdType;
                if ((Params != null))
                {
                    foreach (SqlParameter sqlParam in Params)
                    {
                        catCMD.Parameters.Add(sqlParam);
                    }
                }
                HRPConn.Open();
                Result = catCMD.ExecuteNonQuery();
            }

            catch (SqlException SqlEx)
            {
                if (SqlEx.Number == 2627)
                {
                    ErrMsg = "Duplicate Request Has been Detected.";
                    RetVal = false;
                }
                else
                {
                    ErrMsg = "SqlException Raised [Connect_SQLNonQuery]@" + SqlEx.Message;
                    RetVal = false;
                }
            }
            catch (Exception Ex)
            {
                ErrMsg = "An Unexpected Error Has Occured.@" + Ex.Message;
                RetVal = false;
            }
            finally
            {
                catCMD.Dispose();
                if (HRPConn.State != 0) HRPConn.Close();
            }

            return RetVal;

        }

        public static bool Connect_SQLNonQuery(ref int Result, string SQLQuery, ref string ErrMsg, SqlParameter[] Params, CommandType cmdType)
        {

            bool RetVal = true;
            SqlConnection HRPConn = new SqlConnection();
            SqlCommand catCMD = null;
            try
            {
                HRPConn.ConnectionString = GetConnectionString();
                catCMD = new SqlCommand(SQLQuery, HRPConn);
                catCMD.CommandType = cmdType;
                if ((Params != null))
                {
                    foreach (SqlParameter sqlParam in Params)
                    {
                        catCMD.Parameters.Add(sqlParam);
                    }
                }
                HRPConn.Open();
                Result = catCMD.ExecuteNonQuery();
            }

            catch (SqlException SqlEx)
            {
                if (SqlEx.Number == 2627)
                {
                    ErrMsg = "Duplicate Request Has been Detected.";
                    RetVal = false;
                }
                else
                {
                    ErrMsg = "SqlException Raised [Connect_SQLNonQuery]@" + SqlEx.Message;
                    RetVal = false;
                }
            }
            catch (Exception Ex)
            {
                ErrMsg = "An Unexpected Error Has Occured.@" + Ex.Message;
                RetVal = false;
            }
            finally
            {
                catCMD.Dispose();
                if (HRPConn.State != 0) HRPConn.Close();
            }

            return RetVal;

        }

        public static bool Connect_SQLNonQuery(ref int Result, string SQLQuery, ref string ErrMsg, SqlParameter[] Params, ref SqlConnection Conn)
        {

            bool RetVal = true;
            SqlCommand catCMD = null;
            try
            {

                catCMD = new SqlCommand(SQLQuery, Conn);
                catCMD.CommandType = CommandType.StoredProcedure;
                if ((SQLTran != null))
                {
                    if (bTranStarted == true)
                    {
                        catCMD.Transaction = SQLTran;
                    }
                }
                if ((Params != null))
                {
                    foreach (SqlParameter sqlParam in Params)
                    {
                        catCMD.Parameters.Add(sqlParam);
                    }
                }
                Result = catCMD.ExecuteNonQuery();
            }

            catch (SqlException SqlEx)
            {
                if (SqlEx.Number == 2627)
                {
                    ErrMsg = "Duplicate Request Has been Detected.";
                    RetVal = false;
                }
                else
                {
                    ErrMsg = "SqlException Raised [Connect_SQLNonQuery]@" + SqlEx.Message;
                    RetVal = false;
                }
            }
            catch (Exception Ex)
            {
                ErrMsg = "An Unexpected Error Has Occured.@" + Ex.Message;
                RetVal = false;
            }
            finally
            {
                catCMD.Dispose();
            }

            return RetVal;

        }

        #endregion

        #region "Execute Scalar"

        public static bool Connect_SQLScalar(ref string Result, string SQLQuery, ref string ErrMsg)
        {

            bool RetVal = true;
            SqlConnection HRPConn = new SqlConnection();
            SqlCommand catCMD = null;
            try
            {
                HRPConn.ConnectionString = GetConnectionString();
                catCMD = new SqlCommand(SQLQuery, HRPConn);
                HRPConn.Open();
                Result = Convert.ToString(catCMD.ExecuteScalar());
            }
            catch (SqlException SQLEx)
            {
                ErrMsg = "SqlException Raised [Connect_SQLScalar]@" + SQLEx.Message;
                RetVal = false;
            }
            catch (Exception Ex)
            {
                ErrMsg = "An Unexpected Error Has Occured.@" + Ex.Message;
                RetVal = false;
            }
            finally
            {
                catCMD.Dispose();
                if (HRPConn.State != 0) HRPConn.Close();
            }

            return RetVal;

        }

        public static bool Connect_SQLScalar(ref Int32 Result, string SQLQuery, ref string ErrMsg)
        {
            bool RetVal = true;
            SqlConnection HRPConn = new SqlConnection();
            SqlCommand catCMD = null;
            try
            {
                HRPConn.ConnectionString = GetConnectionString();
                catCMD = new SqlCommand(SQLQuery, HRPConn);
                HRPConn.Open();
                Result = Convert.ToInt32(catCMD.ExecuteScalar());
            }
            catch (SqlException SQLEx)
            {
                ErrMsg = "SqlException Raised [Connect_SQLScalar]@" + SQLEx.Message;
                RetVal = false;
            }
            catch (Exception Ex)
            {
                ErrMsg = "An Unexpected Error Has Occured.@" + Ex.Message;
                RetVal = false;
            }
            finally
            {
                catCMD.Dispose();
                if (HRPConn.State != 0) HRPConn.Close();
            }

            return RetVal;
        }

        //    public static bool Connect_SQLScalar(ref string Result, string SQLQuery, ref SqlParameter[] Params, ref SqlConnection sqlConn, ref string ErrMsg, [System.Runtime.InteropServices.OptionalAttribute, System.Runtime.InteropServices.DefaultParameterValueAttribute(CommandType.Text)] // ERROR: Optional parameters aren't supported in C# CommandType cmdType) 
        public static bool Connect_SQLScalar(ref string Result, string SQLQuery, ref SqlParameter[] Params, ref SqlConnection sqlConn, ref string ErrMsg)
        {

            bool RetVal = true;
            SqlCommand catCMD = null;
            CommandType cmdType = CommandType.Text;

            try
            {
                catCMD = new SqlCommand(SQLQuery, sqlConn);
                catCMD.CommandType = cmdType;
                if ((SQLTran != null))
                {
                    if (bTranStarted == true)
                    {
                        catCMD.Transaction = SQLTran;
                    }
                }

                if ((Params != null))
                {
                    foreach (SqlParameter sqlParam in Params)
                    {
                        catCMD.Parameters.Add(sqlParam);
                    }
                }
                Result = Convert.ToString(catCMD.ExecuteScalar());
            }
            catch (SqlException SQLEx)
            {
                ErrMsg = "SqlException Raised [Connect_SQLScalar]@" + SQLEx.Message;
                RetVal = false;
            }
            catch (Exception Ex)
            {
                Result = "0";
                ErrMsg = "An Unexpected Error Has Occured.@" + Ex.Message;
                RetVal = false;
            }
            finally
            {
                catCMD.Dispose();
            }

            return RetVal;

        }

        //public static bool Connect_SQLScalar(ref string Result, string SQLQuery, ref SqlParameter[] Params, ref string ErrMsg, [System.Runtime.InteropServices.OptionalAttribute, System.Runtime.InteropServices.DefaultParameterValueAttribute(CommandType.Text)] // ERROR: Optional parameters aren't supported in C# CommandType cmdType) 
        public static bool Connect_SQLScalar(ref string Result, string SQLQuery, ref SqlParameter[] Params, ref string ErrMsg)
        {
            bool RetVal = true;
            SqlConnection HRPConn = new SqlConnection();
            SqlCommand catCMD = null;
            CommandType cmdType = CommandType.Text;
            try
            {
                HRPConn.ConnectionString = GetConnectionString();
                catCMD = new SqlCommand(SQLQuery, HRPConn);
                HRPConn.Open();
                catCMD.CommandType = cmdType;
                if ((Params != null))
                {
                    foreach (SqlParameter sqlParam in Params)
                    {
                        catCMD.Parameters.Add(sqlParam);
                    }
                }
                Result = Convert.ToString(catCMD.ExecuteScalar());
            }
            catch (SqlException SQLEx)
            {
                ErrMsg = "SqlException Raised [Connect_SQLScalar]@" + SQLEx.Message;
                RetVal = false;
            }
            catch (Exception Ex)
            {
                Result = "0";
                ErrMsg = "An Unexpected Error Has Occured.@" + Ex.Message;
                RetVal = false;
            }
            finally
            {
                catCMD.Dispose();
                if (HRPConn.State != 0) HRPConn.Close();
            }

            return RetVal;

        }


        #endregion

        public static string GetConnectionString()
        {

            String str = System.Configuration.ConfigurationSettings.AppSettings["connect"];
            str =
            str = cryptoutil.Decrypt(str);
            return str;
        }

        public ObservableCollection<SftpFile> LoadFtpFilesFromDatabase()
        {
            ObservableCollection<SftpFile> SftpFiles = new ObservableCollection<SftpFile>();

            try
            {
                using (SqlConnection connection = new SqlConnection("connect"))
                {
                    connection.Open();
                    string query = "SELECT FtpUrl, LocalPath FROM SftpDetails";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                SftpFiles.Add(new SftpFile
                                {
                                    SftpUrl = reader.GetString(0),
                                    LocalPath = reader.GetString(1),
                                    Status = "Pending"
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
               // MessageBox.Show($"Error fetching data: {ex.Message}");
            }

            return SftpFiles;
        }

        public static bool ExecuteQuery(string query, Dictionary<string, object> parameters, ref string errMsg)
        {
            try
            {
                SqlConnection HRPConn = new SqlConnection();
                HRPConn.ConnectionString = cryptoutil.Decrypt(ConfigurationManager.AppSettings.Get("connect"));
                using (SqlConnection conn = new SqlConnection(HRPConn.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();

                        if (parameters != null)
                        {
                            foreach (var param in parameters)
                            {
                                cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                            }
                        }

                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return false;
            }
        }

        public static object ExecuteScalar(string query, Dictionary<string, object> parameters = null)
        {

            SqlConnection HRPConn = new SqlConnection();
            HRPConn.ConnectionString = cryptoutil.Decrypt(ConfigurationManager.AppSettings.Get("connect"));
            using (SqlConnection conn = new SqlConnection(HRPConn.ConnectionString))
            {
                try
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (parameters != null)
                        {
                            foreach (var param in parameters)
                            {
                                cmd.Parameters.AddWithValue(param.Key, param.Value);
                            }
                        }

                        return cmd.ExecuteScalar();
                    }
                }
                catch (Exception ex)
                {
                    //errMsg = ex.Message;
                    return false;
                }
            }
        }


        public static bool ExecuteQueryScalar(string query, Dictionary<string, object> parameters, ref int result, ref string errorMessage)
        {
            try
            {
                SqlConnection HRPConn = new SqlConnection();
                HRPConn.ConnectionString = cryptoutil.Decrypt(ConfigurationManager.AppSettings.Get("connect"));
                using (SqlConnection conn = new SqlConnection(HRPConn.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();

                        if (parameters != null)
                        {
                            foreach (var param in parameters)
                            {
                                cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                            }
                        }

                        result = (int)cmd.ExecuteScalar();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        public static DataTable ExecuteQueryToDataTable(string query, Dictionary<string, object> parameters = null)
        {
            DataTable dataTable = new DataTable();

            try
            {
                SqlConnection HRPConn = new SqlConnection();
                HRPConn.ConnectionString = cryptoutil.Decrypt(ConfigurationManager.AppSettings.Get("connect"));
                using (SqlConnection connection = new SqlConnection(HRPConn.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            foreach (var param in parameters)
                            {
                                command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                            }
                        }

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return dataTable;
        }
    }
}
