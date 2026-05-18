using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF_CS.IntegrationApp.DALayer
{
    public class DBTask
    {
        string _ConnectionStrng;
        public string _Server;
        public string _DataBase;
        public string _UID;
        public string _PWD;
        public DBTask()
        {

            ////ConfigurationManager.AppSettings["ConnectionString"].ToString();

            //_Server = ConfigurationManager.AppSettings["server"].ToString();
            //_DataBase = ConfigurationManager.AppSettings["database"].ToString();
            //_UID = ConfigurationManager.AppSettings["UserID"].ToString();
            //_PWD = ConfigurationManager.AppSettings["pwd"].ToString();

            //_ConnectionStrng = "server=" + _Server + ";uid=" + _UID + ";pwd=" + _PWD + ";database=" + _DataBase;
            //_ConnectionStrng = cryptoutil.cryptoutil.Decrypt(ConfigurationManager.AppSettings.Get("SQL"));
            _ConnectionStrng = ConnectionFunctions.GetConnectionString();

        }

        public DataSet ExecuteDataset(string StrQuery)
        {
            SqlConnection SQlCon = new SqlConnection();
            DataSet DS = new DataSet();
            try
            {
                SQlCon.ConnectionString = _ConnectionStrng;
                SQlCon.Open();
                SqlDataAdapter SQLDA = new SqlDataAdapter(StrQuery, SQlCon);
                SQLDA.Fill(DS);
                SQlCon.Close();
            }
            catch (Exception ex)
            {
                SQlCon.Close();

            }

            return DS;
        }

        public bool ExecuteNonQuery_SP(string ProcedureName, object[,] ParamArrayName)
        {
            SqlConnection SQlCon = new SqlConnection();
            SQlCon.ConnectionString = _ConnectionStrng;
            SQlCon.Open();

            try
            {
                SqlCommand SQLCmd = new SqlCommand();
                SQLCmd.Connection = SQlCon;
                SQLCmd.CommandText = ProcedureName;
                SQLCmd.CommandType = CommandType.StoredProcedure;
                SQLCmd.CommandTimeout = 900000000;
                SqlTransaction SqlTran;
                SqlTran = SQlCon.BeginTransaction();

                SQLCmd.Transaction = SqlTran;

                // Attach Paremeter 

                string ParameterName;
                object ParemeterValue;

                for (int i = 0; i < ParamArrayName.Length / 2; i++)
                {

                    ParameterName = ParamArrayName[i, 0].ToString();
                    ParemeterValue = ParamArrayName[i, 1];
                    SQLCmd.Parameters.AddWithValue(ParameterName, ParemeterValue);
                }

                try
                {
                    SQLCmd.ExecuteNonQuery();
                    SqlTran.Commit();
                    SQlCon.Close();
                    return true;
                }
                catch (SqlException ex)
                {
                    SqlTran.Rollback();
                    SQlCon.Close();
                    return false;
                }

            }
            catch (Exception ex)
            {
                SQlCon.Close();

            }
            return false;
        }

        public bool ExecuteNonQuery_SPNew(string ProcedureName, object[,] ParamArrayName, ref String ErrMsg)
        {
            SqlConnection SQlCon = new SqlConnection();
            SQlCon.ConnectionString = _ConnectionStrng;
            SQlCon.Open();

            try
            {
                SqlCommand SQLCmd = new SqlCommand();
                SQLCmd.Connection = SQlCon;
                SQLCmd.CommandText = ProcedureName;
                SQLCmd.CommandType = CommandType.StoredProcedure;
                SQLCmd.CommandTimeout = 900000000;
                SqlTransaction SqlTran;
                SqlTran = SQlCon.BeginTransaction();

                SQLCmd.Transaction = SqlTran;

                // Attach Paremeter 

                string ParameterName;
                object ParemeterValue;

                for (int i = 0; i < ParamArrayName.Length / 2; i++)
                {

                    ParameterName = ParamArrayName[i, 0].ToString();
                    ParemeterValue = ParamArrayName[i, 1];
                    SQLCmd.Parameters.AddWithValue(ParameterName, ParemeterValue);
                }

                try
                {
                    SQLCmd.ExecuteNonQuery();
                    SqlTran.Commit();
                    SQlCon.Close();
                    return true;
                }
                catch (SqlException ex)
                {
                    SqlTran.Rollback();
                    SQlCon.Close();
                    ErrMsg = ex.Message;
                    return false;
                }

            }
            catch (Exception ex)
            {
                SQlCon.Close();

            }
            return false;
        }


        public bool ExecuteNonQuery_SP(string ProcedureName)
        {
            SqlConnection SQlCon = new SqlConnection();
            SQlCon.ConnectionString = _ConnectionStrng;
            SQlCon.Open();
            try
            {
                SqlCommand SQLCmd = new SqlCommand();
                SQLCmd.Connection = SQlCon;
                SQLCmd.CommandText = ProcedureName;
                SQLCmd.CommandTimeout = 6000000;
                SQLCmd.CommandType = CommandType.StoredProcedure;

                SqlTransaction SqlTran;
                SqlTran = SQlCon.BeginTransaction();

                SQLCmd.Transaction = SqlTran;

                try
                {
                    SQLCmd.ExecuteNonQuery();

                    SqlTran.Commit();
                    SQlCon.Close();
                    return true;
                }
                catch (SqlException ex)
                {
                    SqlTran.Rollback();
                    SQlCon.Close();
                    return false;

                }

            }
            catch (Exception ex)
            {
                SQlCon.Close();

            }
            return false;
        }

        public bool ExecuteNonQuery_SP_OUTPUT(string ProcedureName, object[,] ParamNameValueType, ref decimal output)
        {
            SqlConnection SQlCon = new SqlConnection();
            SQlCon.ConnectionString = _ConnectionStrng;
            try
            {
                SQlCon.Open();

                SqlCommand SQLCmd = new SqlCommand();
                SQLCmd.Connection = SQlCon;
                SQLCmd.CommandText = ProcedureName;
                SQLCmd.CommandType = CommandType.StoredProcedure;

                SqlTransaction SqlTran;
                SqlTran = SQlCon.BeginTransaction();
                SQLCmd.Transaction = SqlTran;
                string Type;

                SqlParameter SQLP;

                for (int i = 0; i < (ParamNameValueType.Length / 3) - 1; i++)
                {
                    SQLP = new SqlParameter();
                    SQLP.ParameterName = ParamNameValueType[i, 0].ToString();
                    SQLP.Value = ParamNameValueType[i, 1];

                    Type = ParamNameValueType[i, 2].ToString();

                    // Fn call for Set Parameter Type
                    this.SetParameterType(Type, ref SQLP);

                    SQLCmd.Parameters.Add(SQLP);

                }

                for (int j = ((ParamNameValueType.Length / 3) - 1); j < (ParamNameValueType.Length / 3); j++)
                {
                    SQLP = new SqlParameter();
                    SQLP.ParameterName = ParamNameValueType[j, 0].ToString();
                    SQLP.Value = ParamNameValueType[j, 1];

                    Type = ParamNameValueType[j, 2].ToString();
                    // Fn call for Set Parameter Type
                    this.SetParameterType(Type, ref SQLP);

                    SQLP.Size = 0;
                    SQLP.Direction = ParameterDirection.Output;
                    SQLP.IsNullable = false;
                    SQLP.Precision = 0;
                    SQLP.Scale = 0;
                    SQLP.SourceVersion = DataRowVersion.Default;

                    string SColoumn = ParamNameValueType[j, 0].ToString();
                    string pp = ParamNameValueType[j, 0].ToString();
                    string[] p = SColoumn.Split('@');
                    SQLP.SourceColumn = p[1].ToString();
                    SQLCmd.Parameters.Add(SQLP);

                    try
                    {

                        SQLCmd.ExecuteNonQuery();

                        output = decimal.Parse(SQLCmd.Parameters[pp].Value.ToString());

                        SqlTran.Commit();

                        SQlCon.Close();

                        return true;
                    }
                    catch (SqlException ex)
                    {
                        SqlTran.Rollback();

                        SQlCon.Close();

                        return false;

                    }
                }
            }
            catch (Exception ex)
            {
                SQlCon.Close();

            }

            return false;
        }


        public bool ExecuteNonQuery_SP_OUTPUT(string ProcedureName, object[,] ParamNameValueType, ref int output)
        {
            SqlConnection SQlCon = new SqlConnection();
            SQlCon.ConnectionString = _ConnectionStrng;
            try
            {
                SQlCon.Open();

                SqlCommand SQLCmd = new SqlCommand();
                SQLCmd.Connection = SQlCon;
                SQLCmd.CommandText = ProcedureName;
                SQLCmd.CommandType = CommandType.StoredProcedure;

                SqlTransaction SqlTran;
                SqlTran = SQlCon.BeginTransaction();
                SQLCmd.Transaction = SqlTran;
                string Type;

                SqlParameter SQLP;

                for (int i = 0; i < (ParamNameValueType.Length / 3) - 1; i++)
                {
                    SQLP = new SqlParameter();
                    SQLP.ParameterName = ParamNameValueType[i, 0].ToString();
                    SQLP.Value = ParamNameValueType[i, 1];

                    Type = ParamNameValueType[i, 2].ToString();

                    // Fn call for Set Parameter Type
                    this.SetParameterType(Type, ref SQLP);

                    SQLCmd.Parameters.Add(SQLP);

                }

                for (int j = ((ParamNameValueType.Length / 3) - 1); j < (ParamNameValueType.Length / 3); j++)
                {
                    SQLP = new SqlParameter();
                    SQLP.ParameterName = ParamNameValueType[j, 0].ToString();
                    SQLP.Value = ParamNameValueType[j, 1];

                    Type = ParamNameValueType[j, 2].ToString();
                    // Fn call for Set Parameter Type
                    this.SetParameterType(Type, ref SQLP);

                    SQLP.Size = 0;
                    SQLP.Direction = ParameterDirection.Output;
                    SQLP.IsNullable = false;
                    SQLP.Precision = 0;
                    SQLP.Scale = 0;
                    SQLP.SourceVersion = DataRowVersion.Default;

                    string SColoumn = ParamNameValueType[j, 0].ToString();
                    string pp = ParamNameValueType[j, 0].ToString();
                    string[] p = SColoumn.Split('@');
                    SQLP.SourceColumn = p[1].ToString();
                    SQLCmd.Parameters.Add(SQLP);

                    try
                    {

                        SQLCmd.ExecuteNonQuery();

                        output = int.Parse(SQLCmd.Parameters[pp].Value.ToString());

                        SqlTran.Commit();

                        SQlCon.Close();

                        return true;
                    }
                    catch (SqlException ex)
                    {
                        SqlTran.Rollback();

                        SQlCon.Close();

                        return false;

                    }
                }
            }
            catch (Exception ex)
            {
                SQlCon.Close();

            }

            return false;
        }

        private void SetParameterType(string Type, ref SqlParameter SQLP)
        {
            try
            {
                switch (Type)
                {
                    case "datetime":
                        SQLP.SqlDbType = SqlDbType.DateTime;
                        break;
                    case "decimal":
                        SQLP.SqlDbType = SqlDbType.Decimal;
                        break;
                    case "numeric":
                        SQLP.SqlDbType = SqlDbType.Decimal;
                        break;
                    case "varchar":
                        SQLP.SqlDbType = SqlDbType.VarChar;
                        break;
                    case "int":
                        SQLP.SqlDbType = SqlDbType.Int;
                        break;
                    case "integer":
                        SQLP.SqlDbType = SqlDbType.Int;
                        break;
                    case "smallint":
                        SQLP.SqlDbType = SqlDbType.Int;
                        break;

                    case "nchar":
                        SQLP.SqlDbType = SqlDbType.NChar;
                        break;
                    case "nvarchar":
                        SQLP.SqlDbType = SqlDbType.NVarChar;
                        break;
                    case "smalldatetime":
                        SQLP.SqlDbType = SqlDbType.SmallDateTime;
                        break;
                    case "short":
                        SQLP.SqlDbType = SqlDbType.TinyInt;
                        break;
                    case "bit":
                        SQLP.SqlDbType = SqlDbType.Bit;
                        break;
                    case "money":
                        SQLP.SqlDbType = SqlDbType.Money;
                        break;
                    case "tinyint":
                        SQLP.SqlDbType = SqlDbType.TinyInt;
                        break;
                    default:
                        SQLP.SqlDbType = SqlDbType.NVarChar;
                        break;
                }
            }
            catch (Exception ex)
            {

            }
        }

        public DataSet ExecuteQuery_SP(string ProcedureName)
        {
            SqlConnection SQlCon = new SqlConnection();
            DataSet DS = new DataSet();
            try
            {

                SQlCon.ConnectionString = _ConnectionStrng;
                SQlCon.Open();

                SqlCommand SQLCmd = new SqlCommand();
                SQLCmd.Connection = SQlCon;
                SQLCmd.CommandText = ProcedureName;
                SQLCmd.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter SQLDA = new SqlDataAdapter(SQLCmd);


                SQLDA.Fill(DS);
                SQlCon.Close();

            }
            catch (Exception ex)
            {
                SQlCon.Close();

            }
            return DS;
        }

        public DataSet ExecuteQuery_SP(string ProcedureName, object[,] ParameterNameValue)
        {
            SqlConnection SQlCon = new SqlConnection();
            DataSet DS = new DataSet();
            try
            {
                SQlCon.ConnectionString = _ConnectionStrng;
                SQlCon.Open();

                SqlCommand SQLCmd = new SqlCommand();
                SQLCmd.Connection = SQlCon;
                SQLCmd.CommandText = ProcedureName;
                SQLCmd.CommandType = CommandType.StoredProcedure;

                // Attach Paremeter 

                string ParameterName;
                object ParemeterValue;

                for (int i = 0; i < ParameterNameValue.Length / 2; i++)
                {

                    ParameterName = ParameterNameValue[i, 0].ToString();
                    ParemeterValue = ParameterNameValue[i, 1];
                    SQLCmd.Parameters.AddWithValue(ParameterName, ParemeterValue);
                }

                SqlDataAdapter SQLDA = new SqlDataAdapter(SQLCmd);


                SQLDA.Fill(DS);
                SQlCon.Close();
            }
            catch (Exception ex)
            {
                SQlCon.Close();

            }
            return DS;
        }


        public object ExecuteScaler_SP(string ProcedureName, object[,] ParameterNameValue)
        {
            SqlConnection SQlCon = new SqlConnection();
            object objValue = "";
            try
            {
                SQlCon.ConnectionString = _ConnectionStrng;
                SQlCon.Open();

                SqlCommand SQLCmd = new SqlCommand();
                SQLCmd.Connection = SQlCon;
                SQLCmd.CommandText = ProcedureName;
                SQLCmd.CommandType = CommandType.StoredProcedure;

                // Attach Paremeter 

                string ParameterName;
                object ParemeterValue;

                for (int i = 0; i < ParameterNameValue.Length / 2; i++)
                {

                    ParameterName = ParameterNameValue[i, 0].ToString();
                    ParemeterValue = ParameterNameValue[i, 1];
                    SQLCmd.Parameters.AddWithValue(ParameterName, ParemeterValue);
                }

                objValue = SQLCmd.ExecuteScalar();

            }
            catch (Exception ex)
            {
                SQlCon.Close();

            }
            return objValue;

        }

        public object ExecuteScaler_SP(string ProcedureName)
        {
            SqlConnection SQlCon = new SqlConnection();
            object objValue = "";
            try
            {
                SQlCon.ConnectionString = _ConnectionStrng;
                SQlCon.Open();

                SqlCommand SQLCmd = new SqlCommand();
                SQLCmd.Connection = SQlCon;
                SQLCmd.CommandText = ProcedureName;
                SQLCmd.CommandTimeout = 6000000;
                SQLCmd.CommandType = CommandType.StoredProcedure;
                objValue = SQLCmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                SQlCon.Close();

            }
            return objValue;

        }
        public object ExecuteScaler_qry(string sQry)
        {
            SqlConnection SQlCon = new SqlConnection();
            object objValue = "";
            try
            {
                SQlCon.ConnectionString = _ConnectionStrng;
                SQlCon.Open();

                SqlCommand SQLCmd = new SqlCommand();
                SQLCmd.Connection = SQlCon;
                SQLCmd.CommandText = sQry;
                SQLCmd.CommandType = CommandType.Text;
                SQLCmd.CommandTimeout = 6000000;
                objValue = SQLCmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                SQlCon.Close();

            }
            return objValue;

        }



        public DataSet ExecuteQuery_SPRptr(string ProcedureName, object[,] ParameterNameValue, DataSet DS)
        {
            SqlConnection SQlCon = new SqlConnection();

            try
            {
                SQlCon.ConnectionString = _ConnectionStrng;
                SQlCon.Open();

                SqlCommand SQLCmd = new SqlCommand();
                SQLCmd.Connection = SQlCon;
                SQLCmd.CommandText = ProcedureName;
                SQLCmd.CommandType = CommandType.StoredProcedure;
                SQLCmd.CommandTimeout = 6000000;
                // Attach Paremeter 

                string ParameterName;
                object ParemeterValue;

                for (int i = 0; i < ParameterNameValue.Length / 2; i++)
                {

                    ParameterName = ParameterNameValue[i, 0].ToString();
                    ParemeterValue = ParameterNameValue[i, 1];
                    SQLCmd.Parameters.AddWithValue(ParameterName, ParemeterValue);
                }

                SqlDataAdapter SQLDA = new SqlDataAdapter(SQLCmd);


                SQLDA.Fill(DS, "tblQustInfo");
                SQlCon.Close();
            }
            catch (Exception ex)
            {
                SQlCon.Close();

            }
            return DS;
        }

        public bool Connect_SQLNonQuery(ref int Result, string SQLQuery, ref string ErrMsg)
        {
            bool RetVal = true; SqlConnection HRPConn = new SqlConnection();
            SqlCommand catCMD = null;
            try
            {
                HRPConn.ConnectionString = ConnectionFunctions.GetConnectionString();
                catCMD = new SqlCommand(SQLQuery, HRPConn);
                HRPConn.Open(); Result = catCMD.ExecuteNonQuery();
            }
            catch (SqlException SqlEx)
            {
                if ((SqlEx.Number == 2627))
                {
                    ErrMsg = "Duplicate Request Has been Detected.";
                    RetVal = false;
                }
                else
                {
                    ErrMsg = ("SqlException Raised [Connect_SQLNonQuery]@" + SqlEx.Message);
                    RetVal = false;
                }
            }
            catch (Exception Ex)
            {
                ErrMsg = ("An Unexpected Error Has Occured.@" + Ex.Message);
                RetVal = false;
            }
            finally
            {
                if (!(catCMD == null))
                {
                    catCMD.Dispose();
                }
                if (!(HRPConn == null))
                {
                    if (!(HRPConn.State == ConnectionState.Closed))
                    {
                        HRPConn.Close();
                    }
                }
            }
            return RetVal;
        }


    }
}
