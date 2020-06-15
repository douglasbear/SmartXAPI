using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;

namespace SmartxAPI.GeneralFunctions
{
    public class ApiFunctions:IApiFunctions
    {
        private readonly IMapper _mapper;
         public ApiFunctions(IMapper mapper)
        {
            _mapper=mapper;
        }

          public object Response(int Code,string Response)
                {
                        return (new { StatusCode = Code , Message= Response });
                }
        public object ErrorResponse(Exception ex){
            string Msg="";
            string subString =ex.Message.Substring(8,ex.Message.Length -8);
            
                switch(ex.Message.Substring(0,8)){
                    case "Column '": Msg = ex.Message.Substring(7, subString.IndexOf("'")+1) +" is required"; 
                    break;
                    case "Error co": Msg = ex.Message.Substring(0,42); 
                    break;
                    default : Msg = "Invalid Request  " + ex.Message; 
                    break;
                }


                return (new { StatusCode = 403 , Message= Msg });
           
           
        }

        public List < T > ValidateTable < T > (DataTable DataTable){
            
            for (int i = 0; i < DataTable.Columns.Count; i++)
            {
                    DataTable.Columns[i].ColumnName = DataTable.Columns[i].ColumnName.ToString().Replace("_", "");
            }

        //var columnNames = DataTable.Columns.Cast < DataColumn > ().Select(c => c.ColumnName.ToString()).ToList();  
        var properties = typeof(T).GetProperties();  
        return DataTable.AsEnumerable().Select(row => {  
            var objT = Activator.CreateInstance < T > ();  
            foreach(var pro in properties) {  
                //if (columnNames.Contains(pro.Name)) {  
                    try {  
                        pro.SetValue(objT, row[pro.Name]);  
                    } catch (Exception ex) {
                        String msg =ex.Message;
                    }  
                //}  
            }  
            return objT;  
        }).ToList();      



        }


    }
public interface IApiFunctions
    {
        public object Response(int Code,string Response);
        public object ErrorResponse(Exception ex);
        public List < T > ValidateTable < T > (DataTable DataTable);
    }    
}