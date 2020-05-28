using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace SmartxAPI.GeneralFunctions
{
    public class DataAccessLayer:IDataAccessLayer{
        
        private readonly IConfiguration _config;
        private SqlTransaction _transaction;
        private readonly SqlConnection _conn;
                    
        public DataAccessLayer(IConfiguration config){
            _config = config;
            _conn = new SqlConnection( _config.GetConnectionString("SmartxConnection")); 
            _conn.Open();
        }
      
        public void StartTransaction(){
            _transaction = _conn.BeginTransaction("SmartxTransaction");
        }
        public void Commit(){
            _transaction.Commit();
            _conn.Close();        }
        public void Rollback(){
            _transaction.Rollback();
            _conn.Close();
        }
        public int SaveData(string TableName,string IDFieldName,int IDFieldValue,DataTable DataTable)
        {
            string FieldList="";
            string FieldValues="";
            int Result =0;
            for (int i = 0; i < DataTable.Columns.Count; i++)
            {
                if(i==0){
                    FieldList = DataTable.Columns[i].ColumnName.ToString();
                }else{
                    FieldList = FieldList +","+ DataTable.Columns[i].ColumnName.ToString();
                }
            }

            for (int j = 0 ;j < DataTable.Rows.Count;j++)
            {
                for (int k = 0; k < DataTable.Columns.Count; k++)
                {
                    var values = DataTable.Rows[j][k].ToString();
                    if(values==""){values="''";}
                    if(k==0){
                        FieldValues = values;
                    }else{
                        FieldValues = FieldValues +"|"+values;
                    }
                }
                    SqlDataReader DataReader  = null;
                    SqlCommand Command  = new SqlCommand("SAVE_DATA", _conn);
                        Command.Transaction=_transaction;
                        Command.CommandType = CommandType.StoredProcedure;

                        Command.Parameters.Add(new SqlParameter("@X_TableName", TableName));
                        Command.Parameters.Add(new SqlParameter("@X_IDFieldName", IDFieldName));
                        Command.Parameters.Add(new SqlParameter("@N_IDFieldValue", IDFieldValue));
                        Command.Parameters.Add(new SqlParameter("@X_FieldList", FieldList));
                        Command.Parameters.Add(new SqlParameter("@X_FieldValue", FieldValues));

                       
                        Result = (int)Command.ExecuteScalar();

                FieldValues="";
            }       
            
            return Result;
        }
    
}

    public interface IDataAccessLayer
    {
        public void StartTransaction();
        public void Commit();
        public void Rollback();
        public int SaveData(string TableName,string IDFieldName,int IDFieldValue,DataTable DataTable); 
    }
    
}