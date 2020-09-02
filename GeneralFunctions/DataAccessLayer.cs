using System;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace SmartxAPI.GeneralFunctions
{
    public class DataAccessLayer : IDataAccessLayer
    {

        #region Declarations



        IConfiguration config;				//Database Name

        //Database variable declarations...
        private SqlCommand commandStatement;			//SQL Statement
        public SqlConnection databaseConnection;		//SQL Connection		//Data Adapter
        private static DataTable resultTable;			//Data table to store result
        public SqlTransaction databaseTransaction;      //Transaction
        #endregion

        /// <summary>
        /// Constructor...Initializes variables...
        /// </summary>
        #region Constructor
        public DataAccessLayer(IConfiguration conf)
        {
            //isHQUtility = utilityType;

            this.databaseConnection = new SqlConnection();
            this.commandStatement = new SqlCommand();
            this.commandStatement.Connection = this.databaseConnection;
            this.config = conf;
        }
        #endregion


        #region Functions
        public static SqlTransaction TransactionScope = null;
        public void setTransaction()
        {
            OpenConnection();
            TransactionScope = this.databaseConnection.BeginTransaction();
        }
        public void commit()
        {
            TransactionScope.Commit();
        }
        public void rollBack()
        {
            TransactionScope.Rollback();
        }


        public int ExecuteNonQuery(string sqlCommandText, SqlConnection connection)
        {
            try
            {
                SqlCommand sqlCommand = new SqlCommand(sqlCommandText, connection);
                sqlCommand.CommandTimeout = 0;
                sqlCommand.CommandType = CommandType.Text;
                return sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int ExecuteNonQuery(string sqlCommandText, SqlConnection connection, SqlTransaction transaction)
        {
            try
            {
                SqlCommand sqlCommand = new SqlCommand(sqlCommandText, connection);
                sqlCommand.CommandTimeout = 0;
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.Transaction = transaction;
                return sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int ExecuteNonQuery(string sqlCommandText, SortedList paramList, SqlConnection connection)
        {
            try
            {
                SqlCommand sqlCommand = new SqlCommand(sqlCommandText, connection);
                sqlCommand.CommandTimeout = 0;
                sqlCommand.CommandType = CommandType.Text;
                if (paramList.Count > 0)
                {
                    ICollection Keys = paramList.Keys;
                    foreach (string key in Keys)
                    {
                        sqlCommand.Parameters.Add(new SqlParameter(key.ToString(), paramList[key].ToString()));
                    }
                }
                return sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int ExecuteNonQuery(string sqlCommandText, SortedList paramList, SqlConnection connection, SqlTransaction transaction)
        {
            try
            {
                SqlCommand sqlCommand = new SqlCommand(sqlCommandText, connection);
                sqlCommand.CommandTimeout = 0;
                sqlCommand.CommandType = CommandType.Text;
                if (paramList.Count > 0)
                {
                    ICollection Keys = paramList.Keys;
                    foreach (string key in Keys)
                    {
                        sqlCommand.Parameters.Add(new SqlParameter(key.ToString(), paramList[key].ToString()));
                    }
                }
                sqlCommand.Transaction = transaction;
                return sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object ExecuteScalar(string sqlCommandText)
        {
            try
            {
                OpenConnection();
                SqlCommand sqlCommand = new SqlCommand(sqlCommandText, databaseConnection);
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.Transaction = databaseTransaction;
                return sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                CloseConnection();
            }
        }

        public object ExecuteScalar(string sqlCommandText, SqlConnection connection)
        {
            try
            {
                SqlCommand sqlCommand = new SqlCommand(sqlCommandText, connection);
                sqlCommand.CommandType = CommandType.Text;
                return sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object ExecuteScalar(string sqlCommandText, SqlConnection connection, SqlTransaction transaction)
        {
            try
            {
                SqlCommand sqlCommand = new SqlCommand(sqlCommandText, connection);
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.Transaction = transaction;
                return sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public object ExecuteScalar(string sqlCommandText, SortedList paramList)
        {
            try
            {
                OpenConnection();
                SqlCommand sqlCommand = new SqlCommand(sqlCommandText, databaseConnection);
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.Transaction = databaseTransaction;
                if (paramList.Count > 0)
                {
                    ICollection Keys = paramList.Keys;
                    foreach (string key in Keys)
                    {
                        sqlCommand.Parameters.Add(new SqlParameter(key.ToString(), paramList[key].ToString()));
                    }
                }
                return sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (TransactionScope == null)
                    CloseConnection();
            }
        }

        public object ExecuteScalar(string sqlCommandText, SortedList paramList, SqlConnection connection, SqlTransaction transaction)
        {
            try
            {
                SqlCommand sqlCommand = new SqlCommand(sqlCommandText, connection);
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.Transaction = transaction;
                if (paramList.Count > 0)
                {
                    ICollection Keys = paramList.Keys;
                    foreach (string key in Keys)
                    {
                        sqlCommand.Parameters.Add(new SqlParameter(key.ToString(), paramList[key].ToString()));
                    }
                }
                return sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object ExecuteScalar(string sqlCommandText, SortedList paramList, SqlConnection connection)
        {
            try
            {
                SqlCommand sqlCommand = new SqlCommand(sqlCommandText, connection);
                sqlCommand.CommandType = CommandType.Text;
                if (paramList.Count > 0)
                {
                    ICollection Keys = paramList.Keys;
                    foreach (string key in Keys)
                    {
                        sqlCommand.Parameters.Add(new SqlParameter(key.ToString(), paramList[key].ToString()));
                    }
                }
                return sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object ExecuteScalarPro(string sqlCommandText, SortedList paramList)
        {
            try
            {

                OpenConnection();
                SqlCommand sqlCommand = new SqlCommand(sqlCommandText, databaseConnection);
                sqlCommand.CommandTimeout = 0;
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Transaction = TransactionScope;


                if (paramList.Count > 0)
                {
                    ICollection Keys = paramList.Keys;
                    foreach (string key in Keys)
                    {
                        sqlCommand.Parameters.Add(new SqlParameter(key.ToString(), paramList[key].ToString()));
                    }
                }



                return sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (TransactionScope == null)
                    CloseConnection();
            }
        }

        public object ExecuteScalarPro(string sqlCommandText, SortedList paramList, SqlConnection connection, SqlTransaction transaction)
        {
            try
            {
                SqlCommand sqlCommand = new SqlCommand(sqlCommandText, connection);
                sqlCommand.CommandTimeout = 0;
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Transaction = transaction;


                if (paramList.Count > 0)
                {
                    ICollection Keys = paramList.Keys;
                    foreach (string key in Keys)
                    {
                        sqlCommand.Parameters.Add(new SqlParameter(key.ToString(), paramList[key].ToString()));
                    }
                }

                return sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object ExecuteScalarPro(string sqlCommandText, SortedList paramList, SqlConnection connection)
        {
            try
            {
                SqlCommand sqlCommand = new SqlCommand(sqlCommandText, connection);
                sqlCommand.CommandTimeout = 0;
                sqlCommand.CommandType = CommandType.StoredProcedure;


                if (paramList.Count > 0)
                {
                    ICollection Keys = paramList.Keys;
                    foreach (string key in Keys)
                    {
                        sqlCommand.Parameters.Add(new SqlParameter(key.ToString(), paramList[key].ToString()));
                    }
                }

                return sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable ExecuteDataTable(string sqlCommandText, SqlConnection connection)
        {
            try
            {
                int recordsReturned;
                SqlDataAdapter dataAdapter = new SqlDataAdapter();
                dataAdapter.SelectCommand = new SqlCommand(sqlCommandText, connection);
                DataTable resultTable = new DataTable();
                recordsReturned = dataAdapter.Fill(resultTable);
                return resultTable;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable ExecuteDataTable(string sqlCommandText, SortedList paramList)
        {
            try
            {
                OpenConnection();
                int recordsReturned;
                SqlCommand Command = new SqlCommand(sqlCommandText, databaseConnection);
                //if(Command.Parameters.Count>0){ClearParameters();}
                if (paramList.Count > 0)
                {
                    ICollection Keys = paramList.Keys;
                    foreach (string key in Keys)
                    {
                        Command.Parameters.Add(new SqlParameter(key.ToString(), paramList[key].ToString()));
                    }
                }
                SqlDataAdapter dataAdapter = new SqlDataAdapter();
                string CmdText = Command.CommandText;
                dataAdapter.SelectCommand = Command;
                DataTable resTable = new DataTable();
                recordsReturned = dataAdapter.Fill(resTable);
                return resTable;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                CloseConnection();
            }
        }

        public DataTable ExecuteDataTable(string sqlCommandText, SortedList paramList, SqlConnection con)
        {
            try
            {
                int recordsReturned;
                SqlCommand Command = new SqlCommand(sqlCommandText, con);
                if (paramList.Count > 0)
                {
                    ICollection Keys = paramList.Keys;
                    foreach (string key in Keys)
                    {
                        Command.Parameters.Add(new SqlParameter(key.ToString(), paramList[key].ToString()));
                    }
                }
                SqlDataAdapter dataAdapter = new SqlDataAdapter();
                dataAdapter.SelectCommand = Command;
                DataTable resTable = new DataTable();
                recordsReturned = dataAdapter.Fill(resTable);
                return resTable;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable ExecuteDataTable(string sqlCommandText, SortedList paramList, SqlConnection con, SqlTransaction transaction)
        {
            try
            {
                int recordsReturned;
                SqlCommand Command = new SqlCommand(sqlCommandText, con);
                if (paramList.Count > 0)
                {
                    ICollection Keys = paramList.Keys;
                    foreach (string key in Keys)
                    {
                        Command.Parameters.Add(new SqlParameter(key.ToString(), paramList[key].ToString()));
                    }
                }
                SqlDataAdapter dataAdapter = new SqlDataAdapter();
                Command.Transaction = transaction;
                dataAdapter.SelectCommand = Command;
                DataTable resTable = new DataTable();
                recordsReturned = dataAdapter.Fill(resTable);
                return resTable;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void OpenConnection()
        {
            try
            {
                if (databaseConnection.State == ConnectionState.Closed || this.databaseConnection.State == ConnectionState.Broken)
                {
                    string _connectionString = this.config.GetConnectionString("SmartxConnection");
                    this.databaseConnection.ConnectionString = _connectionString;

                    this.databaseConnection.Open();	//Open connection to Database...
                    //SetDateFormat();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CloseConnection()
        {
            try
            {
                if (databaseConnection.State != ConnectionState.Closed || this.databaseConnection.State != ConnectionState.Broken)
                {
                    databaseConnection.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        public int ExecuteNonQueryPro(string sqlCommandText, SortedList paramList)
        {
            try
            {
                OpenConnection();
                SqlCommand sqlCommand = new SqlCommand(sqlCommandText, databaseConnection);
                sqlCommand.CommandTimeout = 0;
                sqlCommand.CommandType = CommandType.StoredProcedure;

                if (paramList.Count > 0)
                {
                    ICollection Keys = paramList.Keys;
                    foreach (string key in Keys)
                    {
                        sqlCommand.Parameters.Add(new SqlParameter(key.ToString(), paramList[key].ToString()));
                    }
                }


                sqlCommand.Transaction = TransactionScope;
                return sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (TransactionScope == null)
                    CloseConnection();
            }
        }

        public int ExecuteNonQueryPro(string sqlCommandText, SortedList paramList, SqlConnection connection, SqlTransaction transaction)
        {
            try
            {
                SqlCommand sqlCommand = new SqlCommand(sqlCommandText, connection);
                sqlCommand.CommandTimeout = 0;
                sqlCommand.CommandType = CommandType.StoredProcedure;

                if (paramList.Count > 0)
                {
                    ICollection Keys = paramList.Keys;
                    foreach (string key in Keys)
                    {
                        sqlCommand.Parameters.Add(new SqlParameter(key.ToString(), paramList[key].ToString()));
                    }
                }


                sqlCommand.Transaction = transaction;
                return sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int ExecuteNonQueryPro(string sqlCommandText, SortedList paramList, SqlConnection connection)
        {
            try
            {
                SqlCommand sqlCommand = new SqlCommand(sqlCommandText, connection);
                sqlCommand.CommandTimeout = 0;
                sqlCommand.CommandType = CommandType.StoredProcedure;

                if (paramList.Count > 0)
                {
                    ICollection Keys = paramList.Keys;
                    foreach (string key in Keys)
                    {
                        sqlCommand.Parameters.Add(new SqlParameter(key.ToString(), paramList[key].ToString()));
                    }
                }


                return sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable ExecuteDataTablePro(string sqlCommandText, SortedList paramList, SqlConnection connection)
        {
            try
            {
                int recordsReturned;
                SqlDataAdapter dataAdapter = new SqlDataAdapter();
                dataAdapter.SelectCommand = new SqlCommand(sqlCommandText, connection);
                dataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                dataAdapter.SelectCommand.CommandText = sqlCommandText;


                if (paramList.Count > 0)
                {
                    ICollection Keys = paramList.Keys;
                    foreach (string key in Keys)
                    {
                        dataAdapter.SelectCommand.Parameters.Add(new SqlParameter(key.ToString(), paramList[key].ToString()));
                    }
                }



                DataTable resultTable = new DataTable();
                recordsReturned = dataAdapter.Fill(resultTable);
                return resultTable;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public int SaveData(string TableName, string IDFieldName, int IDFieldValue, DataTable DataTable)
        {
            string FieldList = "";
            string FieldValues = "";
            int Result = 0;
            for (int i = 0; i < DataTable.Columns.Count; i++)
            {
                FieldList = FieldList + "," + DataTable.Columns[i].ColumnName.ToString();
            }
            FieldList = FieldList.Substring(1);

            for (int j = 0; j < DataTable.Rows.Count; j++)
            {
                for (int k = 0; k < DataTable.Columns.Count; k++)
                {
                    var values = DataTable.Rows[j][k].ToString();
                    values = values.Replace("|", " ");
                    FieldValues = FieldValues + "|" + values;

                }
                FieldValues = FieldValues.Substring(1);
                FieldValues = ValidateString(FieldValues);
                SortedList paramList = new SortedList();
                paramList.Add("X_TableName", TableName);
                paramList.Add("X_IDFieldName", IDFieldName);
                paramList.Add("N_IDFieldValue", IDFieldValue);
                paramList.Add("X_FieldList", FieldList);
                paramList.Add("X_FieldValue", FieldValues);
                Result = (int)ExecuteScalarPro("SAVE_DATA", paramList);
                FieldValues = "";
            }

            return Result;
        }


        public int SaveData(string TableName, string IDFieldName, int IDFieldValue, DataTable DataTable, SqlConnection connection, SqlTransaction transaction)
        {
            string FieldList = "";
            string FieldValues = "";
            int Result = 0;
            for (int i = 0; i < DataTable.Columns.Count; i++)
            {
                FieldList = FieldList + "," + DataTable.Columns[i].ColumnName.ToString();
            }
            FieldList = FieldList.Substring(1);

            for (int j = 0; j < DataTable.Rows.Count; j++)
            {
                for (int k = 0; k < DataTable.Columns.Count; k++)
                {
                    var values = DataTable.Rows[j][k].ToString();
                    values = values.Replace("|", " ");
                    FieldValues = FieldValues + "|" + values;

                }
                FieldValues = FieldValues.Substring(1);
                FieldValues = ValidateString(FieldValues);
                SortedList paramList = new SortedList();
                paramList.Add("X_TableName", TableName);
                paramList.Add("X_IDFieldName", IDFieldName);
                paramList.Add("N_IDFieldValue", IDFieldValue);
                paramList.Add("X_FieldList", FieldList);
                paramList.Add("X_FieldValue", FieldValues);
                Result = (int)ExecuteScalarPro("SAVE_DATA", paramList, connection, transaction);
                FieldValues = "";
            }

            return Result;
        }

        public string ValidateString(string InputString)
        {
            string OutputString = InputString.Replace("'", "''");
            OutputString = OutputString.Replace("|", "'|'");
            OutputString = "'" + OutputString + "'";
            return OutputString;
        }

        public int DeleteData(string TableName, string IDFieldName, int IDFieldValue, string X_Critieria)
        {
            int Result = 0;
            SortedList paramList = new SortedList();
            paramList.Add("X_TableName", TableName);
            paramList.Add("X_IDFieldName", IDFieldName);
            paramList.Add("N_IDFieldValue", IDFieldValue);
            paramList.Add("X_Critieria", X_Critieria);
            Result = (int)ExecuteNonQueryPro("DELETE_DATA", paramList);
            return Result;
        }

        public int DeleteData(string TableName, string IDFieldName, int IDFieldValue, string X_Critieria, SqlConnection connection, SqlTransaction transaction)
        {
            int Result = 0;
            SortedList paramList = new SortedList();
            paramList.Add("X_TableName", TableName);
            paramList.Add("X_IDFieldName", IDFieldName);
            paramList.Add("N_IDFieldValue", IDFieldValue);
            paramList.Add("X_Critieria", X_Critieria);
            Result = (int)ExecuteNonQueryPro("DELETE_DATA", paramList, connection, transaction);
            return Result;
        }

        public int DeleteData(string TableName, string IDFieldName, int IDFieldValue, string X_Critieria, SqlConnection connection)
        {
            int Result = 0;
            SortedList paramList = new SortedList();
            paramList.Add("X_TableName", TableName);
            paramList.Add("X_IDFieldName", IDFieldName);
            paramList.Add("N_IDFieldValue", IDFieldValue);
            paramList.Add("X_Critieria", X_Critieria);
            Result = (int)ExecuteNonQueryPro("DELETE_DATA", paramList, connection);
            return Result;
        }


        public string GetAutoNumber(string TableName, String Coloumn, SortedList Params)
        {
            string AutoNumber = "";
            string BranchId = "0";
            if (Params.Contains("N_BranchID")) { BranchId = Params["N_BranchID"].ToString(); }
            SortedList paramList = new SortedList(){
                {"N_CompanyID", Params["N_CompanyID"]},
                {"N_YearID", Params["N_YearID"]},
                {"N_FormID", Params["N_FormID"]},
                {"N_BranchID", BranchId}
                };
            while (true)
            {
                AutoNumber = (string)ExecuteScalarPro("SP_AutoNumberGenerate", paramList);

                DataTable ResultTable = new DataTable();
                string sqlCommandText = "select 1 from " + TableName + " where " + Coloumn + " = @p1 and N_CompanyID=@p2";
                SortedList SqlParams = new SortedList(){
                    {"@p1",AutoNumber},
                    {"@p2",Params["N_CompanyID"]}};
                object obj = ExecuteScalar(sqlCommandText, SqlParams);

                if (obj == null)
                    break;

            }
            return AutoNumber;
        }


        public string GetAutoNumber(string TableName, String Coloumn, SortedList Params, SqlConnection connection, SqlTransaction transaction)
        {
            string AutoNumber = "";
            string BranchId = "0";
            if (Params.Contains("N_BranchID")) { BranchId = Params["N_BranchID"].ToString(); }
            SortedList paramList = new SortedList(){
                {"N_CompanyID", Params["N_CompanyID"]},
                {"N_YearID", Params["N_YearID"]},
                {"N_FormID", Params["N_FormID"]},
                {"N_BranchID", BranchId}
                };
            while (true)
            {
                AutoNumber = (string)ExecuteScalarPro("SP_AutoNumberGenerate", paramList, connection, transaction);

                DataTable ResultTable = new DataTable();
                string sqlCommandText = "select 1 from " + TableName + " where " + Coloumn + " = @p1 and N_CompanyID=@p2";
                SortedList SqlParams = new SortedList(){
                    {"@p1",AutoNumber},
                    {"@p2",Params["N_CompanyID"]}};
                object obj = ExecuteScalar(sqlCommandText, SqlParams, connection, transaction);

                if (obj == null)
                    break;

            }
            return AutoNumber;
        }



    }

    public interface IDataAccessLayer
    {

        /*  Dep Methodes */
        public DataTable ExecuteDataTable(string sqlCommandText, SortedList paramList);
        public object ExecuteScalar(string sqlCommandText, SortedList paramList);
        public int SaveData(string TableName, string IDFieldName, int IDFieldValue, DataTable DataTable);
        public void setTransaction();
        public void commit();
        public void rollBack();
        public int DeleteData(string TableName, string IDFieldName, int IDFieldValue, string X_Critieria);
        public string GetAutoNumber(string TableName, String Coloumn, SortedList Params);

        /*  Dep Methodes End */


        public DataTable ExecuteDataTablePro(string sqlCommandText, SortedList paramList, SqlConnection connection);
        public DataTable ExecuteDataTable(string sqlCommandText, SortedList paramList, SqlConnection con);
        public DataTable ExecuteDataTable(string sqlCommandText, SortedList paramList, SqlConnection con, SqlTransaction transaction);
        public DataTable ExecuteDataTable(string sqlCommandText, SqlConnection connection);
        public object ExecuteScalar(string sqlCommandText, SqlConnection connection);
        public object ExecuteScalar(string sqlCommandText, SqlConnection connection, SqlTransaction transaction);
        public object ExecuteScalar(string sqlCommandText, SortedList paramList, SqlConnection connection);
        public object ExecuteScalar(string sqlCommandText, SortedList paramList, SqlConnection connection, SqlTransaction transaction);
        public int SaveData(string TableName, string IDFieldName, int IDFieldValue, DataTable DataTable, SqlConnection connection, SqlTransaction transaction);
        public object ExecuteScalarPro(string sqlCommandText, SortedList paramList, SqlConnection connection);
        public object ExecuteScalarPro(string sqlCommandText, SortedList paramList, SqlConnection connection, SqlTransaction transaction);
        public int DeleteData(string TableName, string IDFieldName, int IDFieldValue, string X_Critieria, SqlConnection connection);
        public int DeleteData(string TableName, string IDFieldName, int IDFieldValue, string X_Critieria, SqlConnection connection, SqlTransaction transaction);
        public int ExecuteNonQueryPro(string sqlCommandText, SortedList paramList, SqlConnection connection, SqlTransaction transaction);
        public int ExecuteNonQueryPro(string sqlCommandText, SortedList paramList, SqlConnection connection);
        public int ExecuteNonQuery(string sqlCommandText, SortedList paramList, SqlConnection connection, SqlTransaction transaction);
        public int ExecuteNonQuery(string sqlCommandText, SortedList paramList, SqlConnection connection);
        public int ExecuteNonQuery(string sqlCommandText, SqlConnection connection, SqlTransaction transaction);
        public int ExecuteNonQuery(string sqlCommandText, SqlConnection connection);
        public string GetAutoNumber(string TableName, String Coloumn, SortedList Params, SqlConnection connection, SqlTransaction transaction);
    }

}
