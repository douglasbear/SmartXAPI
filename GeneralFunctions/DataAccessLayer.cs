using System;
using System.Data.SqlClient;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace SmartxAPI.GeneralFunctions
{
    public class DataAccessLayer:IDataAccessLayer
    {
        /// <summary>
        /// Class Variables for Data Layer...
        /// </summary>
        #region Declarations
        //Common Variable Declarations...
        bool isHQUtility;								//True -> HQ, False -> Store
        public static string connectionString;			//Connection String
        public int formatCode;							//Format Code of Date Format
        public string formatString;						//Format string of Date Format

        //Database variable declarations...
        public static string serverName;				//Server Name
        public static string databaseUser;				//Database User Name
        public static string databasePassword;			//Database Password
        public static string databaseName;
        
        IConfiguration config;				//Database Name

        //Database variable declarations...
        private SqlCommand commandStatement;			//SQL Statement
        public SqlConnection databaseConnection;		//SQL Connection		//Data Adapter
        private static DataTable resultTable;			//Data table to store result
        private static DataSet resultSet;
        public SqlTransaction databaseTransaction;		//Transaction
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
            this.config =conf;
        }
        #endregion

        /// <summary>
        /// Server and Database name can be passed in these properties...
        /// </summary>
        #region Properties
        public string server
        {
            get
            {
                return serverName;
            }
            set
            {
                serverName = value;
            }
        }

        public string database
        {
            get
            {
                return databaseName;
            }
            set
            {
                databaseName = value;
            }
        }

        #endregion

        /// <summary>
        /// Database functions for Database...
        /// </summary>
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
        public int ExecuteNonQuery(string sqlCommandText)
        {
            try
            {
                OpenConnection();
                SqlCommand sqlCommand=new SqlCommand(sqlCommandText,databaseConnection);
                sqlCommand.CommandTimeout = 0;
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.Transaction = databaseTransaction;
                return sqlCommand.ExecuteNonQuery();
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

        public int ExecuteNonQuery(string sqlCommandText, System.Data.CommandType sqlCommandType)
        {
            try
            {
                OpenConnection();
                commandStatement.CommandTimeout = 0;
                commandStatement.CommandType = sqlCommandType;
                commandStatement.CommandText = sqlCommandText;
                commandStatement.Transaction = databaseTransaction;
                return this.commandStatement.ExecuteNonQuery();
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

        public int ExecuteNonQueryLinkDB(string sqlCommandText, System.Data.CommandType sqlCommandType)
        {
            try
            {
                OpenConnection();
                commandStatement.CommandType = sqlCommandType;
                commandStatement.CommandText = sqlCommandText;
                return this.commandStatement.ExecuteNonQuery();
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

        public object ExecuteScalar(string sqlCommandText)
        {
            try
            {
                OpenConnection();
                SqlCommand sqlCommand =new SqlCommand(sqlCommandText, databaseConnection);
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

    public object ExecuteScalar(string sqlCommandText,SortedList paramList)
        {
            try
            {
                OpenConnection();
                SqlCommand sqlCommand =new SqlCommand(sqlCommandText, databaseConnection);
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.Transaction = TransactionScope;
                if(paramList.Count>0){
                ICollection Keys = paramList.Keys;
                foreach (string key in Keys) {
                        sqlCommand.Parameters.Add(new SqlParameter(key.ToString(),paramList[key].ToString()));
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
                if(TransactionScope==null)
                CloseConnection();
            }
        }
        public object ExecuteScalar(string connectionString, string sqlCommandText)
        {
            try
            {
                if (databaseConnection.State != ConnectionState.Closed || this.databaseConnection.State != ConnectionState.Broken)
                {
                    databaseConnection.Close();
                }
                this.databaseConnection.ConnectionString = connectionString;
                this.databaseConnection.Open();
                commandStatement.CommandType = CommandType.Text;
                commandStatement.CommandText = sqlCommandText;
                commandStatement.Transaction = TransactionScope;
                return this.commandStatement.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (databaseConnection.State != ConnectionState.Closed || this.databaseConnection.State != ConnectionState.Broken)
                {
                    databaseConnection.Close();
                }
            }
        }

        public object ExecuteScalar(string sqlCommandText, System.Data.CommandType sqlCommandType)
        {
            try
            {
                OpenConnection();
                commandStatement.CommandType = sqlCommandType;
                commandStatement.CommandText = sqlCommandText;
                commandStatement.Transaction = databaseTransaction;
                return this.commandStatement.ExecuteScalar();
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

        public object ExecuteScalarPro(string sqlCommandText, SortedList paramList)
        {
            try
            {

                OpenConnection();
                SqlCommand sqlCommand=new SqlCommand(sqlCommandText,databaseConnection);
                sqlCommand.CommandTimeout = 0;
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Transaction = TransactionScope;
                

                if(paramList.Count>0){
                ICollection Keys = paramList.Keys;
                foreach (string key in Keys) {
                        sqlCommand.Parameters.Add(new SqlParameter(key.ToString(),paramList[key].ToString()));
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
                if(TransactionScope==null)
                CloseConnection();
            }
        }

        public IDataReader ExecuteReader(string sqlCommandText)
        {
            try
            {
                OpenConnection();
                commandStatement.CommandType = CommandType.Text;
                commandStatement.CommandText = sqlCommandText;
                commandStatement.Transaction = databaseTransaction;
                return (IDataReader)this.commandStatement.ExecuteReader();
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

        public IDataReader ExecuteReader(string sqlCommandText, System.Data.CommandType sqlCommandType)
        {
            try
            {
                OpenConnection();
                commandStatement.CommandType = sqlCommandType;
                commandStatement.CommandText = sqlCommandText;
                commandStatement.Transaction = databaseTransaction;
                return (IDataReader)this.commandStatement.ExecuteReader();
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

        public DataTable ExecuteDataTable(string sqlCommandText)
        {
            try
            {
                OpenConnection();
                int recordsReturned;
                SqlDataAdapter dataAdapter =new SqlDataAdapter();
                dataAdapter.SelectCommand = new SqlCommand(sqlCommandText, databaseConnection);
                resultTable = new DataTable();
                recordsReturned = dataAdapter.Fill(resultTable);
                return resultTable;
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

        public  DataTable ExecuteDataTable(string sqlCommandText,SortedList paramList)
        {
            try
            {
                OpenConnection();
                int recordsReturned;
                SqlCommand Command =new SqlCommand(sqlCommandText, databaseConnection);
                //if(Command.Parameters.Count>0){ClearParameters();}
                if(paramList.Count>0){
                ICollection Keys = paramList.Keys;
                foreach (string key in Keys) {
                        Command.Parameters.Add(new SqlParameter(key.ToString(),paramList[key].ToString()));
                    }
                }
                SqlDataAdapter dataAdapter =new SqlDataAdapter();
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

        // public  async Task<DataTable> ExecuteDataTableAsync(string sqlCommandText,SortedList paramList)
        // {
            
        //         try
        //         {
        //             await Task.Run(() => 
        //             {
        //                 OpenConnection();
        //                 int recordsReturned;
        //                 SqlCommand Command =new SqlCommand(sqlCommandText, databaseConnection);
        //                 if(commandStatement.Parameters.Count>0){ClearParameters();}
        //                 if(paramList.Count>0){
        //                 ICollection Keys = paramList.Keys;
        //                 foreach (string key in Keys) {
        //                         Command.Parameters.Add(new SqlParameter(key.ToString(),paramList[key].ToString()));
        //                     }
        //                 }
        //                 dataAdapter.SelectCommand = Command;
        //                 resultTable = new DataTable();
        //                 recordsReturned = dataAdapter.Fill(resultTable);
        //                 return resultTable;
        //             });
        //             DataTable dataTable=new DataTable();
        //             return dataTable;
        //         }
        //         catch (Exception ex)
        //         {
        //             throw ex;
        //         }
        //         finally
        //         {
        //             CloseConnection();
        //         }
        // }

        
        public DataSet FillDataSet(string sqlCommandText)
        {
            try
            {
                OpenConnection();
                int recordsReturned;
                SqlDataAdapter dataAdapter =new SqlDataAdapter();
                dataAdapter.SelectCommand = new SqlCommand(sqlCommandText, databaseConnection);
                resultSet = new DataSet();
                recordsReturned = dataAdapter.Fill(resultTable);
                return resultSet;
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
        public DataTable ExecuteDataTable(string sqlCommandText, string dataTableName)
        {
            try
            {
                OpenConnection();
                int recordsReturned;
                SqlDataAdapter dataAdapter =new SqlDataAdapter();
                dataAdapter.SelectCommand = new SqlCommand(sqlCommandText, databaseConnection);
                if (dataTableName.Trim().Length > 0)
                {
                    resultTable = new DataTable(dataTableName);
                }
                else
                {
                    resultTable = new DataTable();
                }
                recordsReturned = dataAdapter.Fill(resultTable);
                return resultTable;
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

        public DataTable ExecuteDataTable(string sqlCommandText, System.Data.CommandType sqlCommandType)
        {
            try
            {
                OpenConnection();
                commandStatement.CommandTimeout = 0;
                commandStatement.CommandType = sqlCommandType;
                commandStatement.CommandText = sqlCommandText;
                commandStatement.Connection = databaseConnection;
                SqlDataAdapter dataAdapter =new SqlDataAdapter();
                dataAdapter.SelectCommand = commandStatement;
                commandStatement.Transaction = databaseTransaction;

                int recordsReturned;
                dataAdapter.SelectCommand = commandStatement;

                resultTable = new DataTable();
                recordsReturned = dataAdapter.Fill(resultTable);
                return resultTable;
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

        public DataTable ExecuteDataTable(string sqlCommandText, System.Data.CommandType sqlCommandType, string dataTableName)
        {
            try
            {
                OpenConnection();
                commandStatement.CommandTimeout = 0;
                commandStatement.CommandType = sqlCommandType;
                commandStatement.CommandText = sqlCommandText;
                commandStatement.Connection = databaseConnection;
                SqlDataAdapter dataAdapter =new SqlDataAdapter();
                dataAdapter.SelectCommand = commandStatement;
                commandStatement.Transaction = databaseTransaction;

                int recordsReturned;
                dataAdapter.SelectCommand = commandStatement;

                if (dataTableName.Trim().Length > 0)
                {
                    resultTable = new DataTable(dataTableName);
                }
                else
                {
                    resultTable = new DataTable();
                }
                recordsReturned = dataAdapter.Fill(resultTable);
                return resultTable;
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

        public string ExecuteForXML(string sqlCommandText, System.Data.CommandType sqlCommandType)
        {
            try
            {
                OpenConnection();
                commandStatement.CommandType = sqlCommandType;
                commandStatement.CommandText = sqlCommandText;
                SqlDataReader readerResultSet;
                readerResultSet = this.commandStatement.ExecuteReader();
                System.Text.StringBuilder resultData = new System.Text.StringBuilder();
                do
                {
                    resultData.Append(readerResultSet.GetString(0));
                }
                while (readerResultSet.Read());
                return Convert.ToString(resultData);
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

        public void ClearParameters()
        {
            try
            {
                this.commandStatement.Parameters.Clear();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IDataParameter AddParameter()
        {
            try
            {
                SqlParameter sqlParam = new SqlParameter();
                this.commandStatement.Parameters.Add(sqlParam);
                return sqlParam;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IDataParameter AddParameter(string parameterName, SqlDbType sqlDatabaseType, int parameterLength, ParameterDirection parameterDirection)
        {
            try
            {
                SqlParameter sqlParam = new SqlParameter(parameterName, sqlDatabaseType, parameterLength);
                sqlParam.Direction = parameterDirection;
                return this.commandStatement.Parameters.Add(sqlParam);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IDataParameter AddParameter(string parameterName, SqlDbType sqlDatabaseType, int parameterLength, ParameterDirection parameterDirection, object parameterValue)
        {
            try
            {
                SqlParameter sqlParam = (SqlParameter)this.AddParameter(parameterName, sqlDatabaseType, parameterLength, parameterDirection);
                sqlParam.Value = parameterValue;
                return sqlParam;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        /// <summary>
        /// Functions for date formatting...
        /// </summary>
        #region Date Formatting

        /// <summary>
        /// Converts supplied date to SQL universal format...
        /// </summary>
        /// <param name="rawDate"> Date in DateTime </param>
        public string UniversalDateFormat(DateTime rawDate)
        {
            try
            {
                return string.Format("{0:s}", rawDate);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Converts supplied date to SQL universal format...
        /// </summary>
        /// <param name="rawDate"> Date in string </param>
        public string UniversalDateFormat(string rawDate)
        {
            try
            {
                return string.Format("{0:s}", Convert.ToDateTime(rawDate));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// To set the date format of SQL Server...
        /// </summary>
        private void SetDateFormat()
        {
            try
            {
                string sqlQuery = "";
                string dateFormat = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
                string dateSeparator = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.DateSeparator;

                dateFormat = dateFormat.ToUpper();

                //Getting Date Format...
                if (dateFormat.Substring(0, 1) == "M" && dateFormat.Substring(dateFormat.Length - 1, 1) == "Y")
                {
                    formatCode = 101;
                    formatString = "MM" + dateSeparator + "dd" + dateSeparator + "yyy";
                    sqlQuery = "SET DATEFORMAT mdy";
                }
                else if (dateFormat.Substring(0, 1) == "M" && dateFormat.Substring(dateFormat.Length - 1, 1) == "D")
                {
                    formatString = "MM" + dateSeparator + "yyy" + dateSeparator + "dd";
                    sqlQuery = "SET DATEFORMAT myd";
                }
                else if (dateFormat.Substring(0, 1) == "D" && dateFormat.Substring(dateFormat.Length - 1, 1) == "Y")
                {
                    formatCode = 103;
                    formatString = "dd" + dateSeparator + "MM" + dateSeparator + "yyy";
                    sqlQuery = "SET DATEFORMAT dmy";
                }
                else if (dateFormat.Substring(0, 1) == "D" && dateFormat.Substring(dateFormat.Length - 1, 1) == "M")
                {
                    formatString = "dd" + dateSeparator + "yyy" + dateSeparator + "MM";
                    sqlQuery = "SET DATEFORMAT dym";
                }
                else if (dateFormat.Substring(0, 1) == "Y" && dateFormat.Substring(dateFormat.Length - 1, 1) == "M")
                {
                    formatString = "yyy" + dateSeparator + "dd" + dateSeparator + "MM";
                    sqlQuery = "SET DATEFORMAT ydm";
                }
                else if (dateFormat.Substring(0, 1) == "Y" && dateFormat.Substring(dateFormat.Length - 1, 1) == "D")
                {
                    formatString = "yyy" + dateSeparator + "MM" + dateSeparator + "dd";
                    sqlQuery = "SET DATEFORMAT ymd";
                }

                this.commandStatement.CommandType = CommandType.Text;
                this.commandStatement.CommandText = sqlQuery;
                this.commandStatement.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        /// <summary>
        /// Functions to connect to database...and to set date format...
        /// </summary>
        #region Database Connection
        /// <summary>
        /// Opens connection to database without transaction...
        /// </summary>

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

        /// <summary>
        /// Opens database connection with transaction...
        /// </summary>
        /// <param name="transactionType"> true = Begin Transaction, false = No Transaction </param>
       
        public void OpenConnectionLinkServer()
        {
            try
            {
                if (databaseConnection.State == ConnectionState.Closed || this.databaseConnection.State == ConnectionState.Broken)
                {
                    this.databaseConnection.ConnectionString = "Integrated Security=SSPI;Persist Security Info=False;"
                                                                    + "Server=" + serverName
                                                                    + ";Database=" + databaseName
                                                                    + ";User ID=" + databaseUser
                                                                    + ";Password=" + databasePassword
                                                                    + ";Trusted_Connection=False";

                    this.databaseConnection.Open();	//Open connection to Database...
                    SetDateFormat();					//Server date format...
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Close connection to server for connection having no transaction...
        /// </summary>
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

        /// <summary>
        /// Close connection to server for connection having transaction...
        /// </summary>
        /// <param name="commitRollBack"> true = Commit, false = RollBack </param>
        public void CloseConnection(bool commitRollBack)
        {
            try
            {
                if (databaseConnection.State != ConnectionState.Closed || this.databaseConnection.State != ConnectionState.Broken)
                {
                    if (commitRollBack == true)
                    {
                        databaseTransaction.Commit();
                    }
                    else
                    {
                        databaseTransaction.Rollback();
                    }
                    databaseConnection.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CloseConnectionLinkServer()
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
                SqlCommand sqlCommand=new SqlCommand(sqlCommandText,databaseConnection);
                sqlCommand.CommandTimeout = 0;
                sqlCommand.CommandType = CommandType.StoredProcedure;

                if(paramList.Count>0){
                ICollection Keys = paramList.Keys;
                foreach (string key in Keys) {
                        sqlCommand.Parameters.Add(new SqlParameter(key.ToString(),paramList[key].ToString()));
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
                if(TransactionScope==null)
                CloseConnection();
            }
        }


        public DataTable ExecuteDataTablePro(string sqlCommandText, SortedList paramList)
        {
            try
            {
                OpenConnection();
                int recordsReturned;
                SqlDataAdapter dataAdapter =new SqlDataAdapter();
                dataAdapter.SelectCommand = new SqlCommand(sqlCommandText, databaseConnection);
                dataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                dataAdapter.SelectCommand.CommandText = sqlCommandText;
                

                if(paramList.Count>0){
                ICollection Keys = paramList.Keys;
                foreach (string key in Keys) {
                    dataAdapter.SelectCommand.Parameters.Add(new SqlParameter(key.ToString(),paramList[key].ToString()));
                }
            }



                resultTable = new DataTable();
                recordsReturned = dataAdapter.Fill(resultTable);
                return resultTable;
                
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


    public int SaveData(string TableName,string IDFieldName,int IDFieldValue,DataTable DataTable)
        {
            string FieldList="";
            string FieldValues="";
            int Result =0;
            for (int i = 0; i < DataTable.Columns.Count; i++)
            {
                    FieldList = FieldList +","+ DataTable.Columns[i].ColumnName.ToString();
            }
            FieldList=FieldList.Substring(1);

            for (int j = 0 ;j < DataTable.Rows.Count;j++)
            {
                for (int k = 0; k < DataTable.Columns.Count; k++)
                {
                    var values = DataTable.Rows[j][k].ToString();
                    values = values.Replace("|"," ");
                    FieldValues = FieldValues +"|"+values;
                   
                }
                FieldValues=FieldValues.Substring(1);
                FieldValues=ValidateString(FieldValues);
                SortedList paramList = new SortedList();
                paramList.Add("X_TableName", TableName);
                paramList.Add("X_IDFieldName", IDFieldName);
                paramList.Add("N_IDFieldValue", IDFieldValue);
                paramList.Add("X_FieldList", FieldList);
                paramList.Add("X_FieldValue", FieldValues);
                Result =(int)ExecuteScalarPro("SAVE_DATA", paramList);
                FieldValues="";
            }       
            
            return Result;
        }
        public string ValidateString(string InputString){
            string OutputString=InputString.Replace("'","''");
            OutputString = OutputString.Replace("|","'|'");
            OutputString="'"+OutputString+"'";
            return OutputString;
        }

        public int DeleteData(string TableName,string IDFieldName,int IDFieldValue,string X_Critieria)
        {
            int Result =0;
                SortedList paramList = new SortedList();
                paramList.Add("X_TableName", TableName);
                paramList.Add("X_IDFieldName", IDFieldName);
                paramList.Add("N_IDFieldValue", IDFieldValue);
                paramList.Add("X_Critieria", X_Critieria);
                Result =(int) ExecuteNonQueryPro("DELETE_DATA", paramList);
            return Result;
        }


        public string GetAutoNumber(string TableName,String Coloumn,SortedList Params)
            {   
                string AutoNumber="";
                string BranchId ="0";
                if(Params.Contains("N_BranchID")){BranchId=Params["N_BranchID"].ToString();}
                SortedList paramList=new SortedList(){
                {"N_CompanyID", Params["N_CompanyID"]},
                {"N_YearID", Params["N_YearID"]},
                {"N_FormID", Params["N_FormID"]},
                {"N_BranchID", BranchId}
                };
                while(true){
                    AutoNumber = (string)ExecuteScalarPro("SP_AutoNumberGenerate",paramList);

                    DataTable ResultTable=new DataTable();
                    string sqlCommandText ="select 1 from "+TableName +" where "+ Coloumn+" = @p1 and N_CompanyID=@p2";
                    SortedList SqlParams =new SortedList(){
                    {"@p1",AutoNumber},
                    {"@p2",Params["N_CompanyID"]}};
                    object obj = ExecuteScalar(sqlCommandText,SqlParams);
                
                        if(obj==null)
                        break;
                
                }
                return AutoNumber;
            }



    }

     public interface IDataAccessLayer
    {
        public DataTable ExecuteDataTablePro(string sqlCommandText, SortedList paramList);
        public DataTable ExecuteDataTable(string sqlCommandText,SortedList paramList);
        //public  DataTable ExecuteDataTableAsync(string sqlCommandText,SortedList paramList);
        public DataTable ExecuteDataTable(string sqlCommandText);
        public object ExecuteScalar(string sqlCommandText);
        public object ExecuteScalar(string sqlCommandText,SortedList paramList);
        public int SaveData(string TableName,string IDFieldName,int IDFieldValue,DataTable DataTable);
        public object ExecuteScalarPro(string sqlCommandText, SortedList paramList);
        public void setTransaction();
        public void commit();
        public void rollBack();
        public int DeleteData(string TableName,string IDFieldName,int IDFieldValue,string X_Critieria);
        public int ExecuteNonQueryPro(string sqlCommandText, SortedList paramList);
        public string GetAutoNumber(string TableName,String Coloumn,SortedList Params);
    }
 
}
