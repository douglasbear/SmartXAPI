using System;
using System.Collections;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace SmartxAPI.GeneralFunctions
{
    public class DataAccessLayer:IDataAccessLayer
    {
        
        private readonly IConfiguration _config;
        private SqlTransaction _transaction;
        private readonly SqlConnection _conn;
                    
        public DataAccessLayer(IConfiguration config){
            _config = config;
            _conn = new SqlConnection( _config.GetConnectionString("SmartxConnection")); 
            _conn.Open();
        }
      
        public void StartTransaction(){
            _transaction = _conn.BeginTransaction("SmartxTransaction");}
        public void Commit(){
            _transaction.Commit();
            _conn.Close();}
        public void Rollback(){
            _transaction.Rollback();
            _conn.Close();}
  
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


        
        public int DeleteData(string TableName,string IDFieldName,int IDFieldValue,string X_Critieria)
        {
            int Result =0;

            SqlCommand Command  = new SqlCommand("DELETE_DATA", _conn);
            Command.Transaction=_transaction;
            Command.CommandType = CommandType.StoredProcedure;
            Command.Parameters.Add(new SqlParameter("@X_TableName", TableName));
            Command.Parameters.Add(new SqlParameter("@X_IDFieldName", IDFieldName));
            Command.Parameters.Add(new SqlParameter("@N_IDFieldValue", IDFieldValue));
            Command.Parameters.Add(new SqlParameter("@X_Critieria", X_Critieria));
   
            Result = (int)Command.ExecuteNonQuery();
            return Result;
        }
        public int UpdateData(string TableName,string UpdateFiels,string X_Critieria,SortedList Params)
        {
            int Result =0;
            if(!X_Critieria.Contains("@")){return Result;}
            if(!UpdateFiels.Contains("@")){return Result;}
            SqlCommand Command  = new SqlCommand("Update "+TableName+" Set "+UpdateFiels+" where "+ X_Critieria, _conn);
            if(Params.Count>0){
                ICollection Keys = Params.Keys;
                foreach (string key in Keys) {
                    Command.Parameters.Add(new SqlParameter(key.ToString(),Params[key].ToString()));
                }
            }

            Command.CommandType = CommandType.StoredProcedure;
            Result = (int)Command.ExecuteScalar();
            return Result;
        }
        

        public DataTable Select(string TableName,string FieldName,string X_Critieria,SortedList Params,string X_OrderBy)
        {
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand Command;
            DataTable ds = new DataTable();

            if(FieldName==""){FieldName="*";}

            if(X_OrderBy!=""){X_OrderBy=" order by "+X_OrderBy;}

            if(X_Critieria!="" && Params.Count>0){X_Critieria=" where "+X_Critieria;}

            string Sql = "Select "+FieldName+" from "+TableName +" "+ X_Critieria+" "+X_OrderBy;

            Command  = new SqlCommand(Sql, _conn);

            if(Params.Count>0){
                ICollection Keys = Params.Keys;
                foreach (string key in Keys) {
                    Command.Parameters.Add(new SqlParameter(key.ToString(),Params[key].ToString()));
                }
            }

            if(_transaction!=null){
                Command.Transaction=_transaction;
            }
            
            da.SelectCommand = Command;
           
            da.Fill(ds);

            return ds;
        }
        public Object ExecuteProcedure(string ProcedureName,string Params)
            {
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand Command;
            DataSet ds = new DataSet();
            Command  = new SqlCommand("Exec "+ProcedureName +" "+Params, _conn);

            da.SelectCommand = Command;

            return da;
        }


        public string GetAutoNumber(string TableName,String Coloumn,SortedList Params)
            {   
                string Result="0";
                string AutoNumber="";
                while(true){
                    SqlCommand Command  = new SqlCommand("SP_AutoNumberGenerate", _conn);
                    Command.Transaction=_transaction;
                    Command.CommandType = CommandType.StoredProcedure;

                    Command.Parameters.Add(new SqlParameter("@N_CompanyID", Params["N_CompanyID"]));
                    Command.Parameters.Add(new SqlParameter("@N_YearID", Params["N_YearID"]));
                    Command.Parameters.Add(new SqlParameter("@N_FormID", Params["N_FormID"]));
                    Command.Parameters.Add(new SqlParameter("@N_BranchID", Params["N_BranchID"]));
                    
                    AutoNumber = (string)Command.ExecuteScalar();

                    DataTable ResultTable=new DataTable();
                    string X_Crieteria = Coloumn+" = @p1 and N_CompanyID=@p2";
                    SortedList SqlParams =new SortedList();
                    SqlParams.Add("@p1",AutoNumber);
                    SqlParams.Add("@p2",Params["N_CompanyID"]);
                    ResultTable = Select(TableName,"1",X_Crieteria,SqlParams,"");
                    if(ResultTable.Rows.Count==0){
                            break;
                    }else{
                        Result=ResultTable.Rows[0][0].ToString();
                        if(Result==null)
                        break;
                    }
                }
                return AutoNumber;
            }


        public string ValidateString(string InputString){
            string OutputString=InputString.Replace("'","''");
            OutputString = OutputString.Replace("|","'|'");
            OutputString="'"+OutputString+"'";
            return OutputString;
        }
        public string ValidateSql(string InputString){
            string OutputString=InputString.Replace("'","''");
            return OutputString;
        }
    
}

    public interface IDataAccessLayer
    {
        public void StartTransaction();
        public void Commit();
        public void Rollback();
        public int SaveData(string TableName,string IDFieldName,int IDFieldValue,DataTable DataTable); 
        public int DeleteData(string TableName,string IDFieldName,int IDFieldValue,string X_Critieria); 
        public int UpdateData(string TableName,string UpdateFields,string X_Critieria,SortedList Params);
        public DataTable Select(string TableName,string FieldName,string X_Critieria,SortedList Params,string X_OrderBy);
        public Object ExecuteProcedure(string ProcedureName,string Params);
        public string GetAutoNumber(string TableName,String Coloumn,SortedList Params);
    }
    
}