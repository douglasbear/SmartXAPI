using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace SmartxAPI.GeneralFunctions
{
    public class ApiFunctions : IApiFunctions
    {
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment env;
        private readonly IMyFunctions myFunctions;
        private readonly string logPath;
        public ApiFunctions(IMapper mapper, IWebHostEnvironment envn, IMyFunctions myFun, IConfiguration conf)
        {
            _mapper = mapper;
            env = envn;
            myFunctions = myFun;
            logPath = conf.GetConnectionString("LogPath");
        }

        public object Response(int Code, string ResMessage)
        {
            return (new { StatusCode = Code, Message = ResMessage, Data = "" });
        }

        public object Error( ClaimsPrincipal User,string message)
        {
            return (new { type = "error", Message = message, Data = "" });
        }
        public object Success(DataTable dataTable)
        {
            return (new { type = "success", Message = "null", Data = dataTable });
        }
        public object Success(DataTable dataTable, string msg)
        {
            return (new { type = "success", Message = msg, Data = dataTable });
        }

        public object Success(Dictionary<DataRow, DataTable> dictionary, string message)
        {
            return (new { type = "success", Message = message, Data = dictionary });
        }
        public object Success(Dictionary<string, string> dictionary, string message)
        {
            return (new { type = "success", Message = message, Data = dictionary });
        }
        public object Success(Dictionary<string, string> dictionary)
        {
            return (new { type = "success", Message = "null", Data = dictionary });
        }
        public object Success(DataSet dataSet)
        {
            return (new { type = "success", Message = "null", Data = dataSet });
        }
        public object Success(string[] json)
        {
            return (new { type = "success", Message = "null", Data = json });
        }
        public object Success(SortedList result)
        {
            return (new { type = "success", Message = "null", Data = result });
        }
        public object Success(SortedList result, String message)
        {
            return (new { type = "success", Message = message, Data = result });
        }
        public object Success(DataSet dataSet, String message)
        {
            return (new { type = "success", Message = message, Data = dataSet });
        }
        public object Success(DataRow dataRow, String message)
        {
            return (new { type = "success", Message = message, Data = dataRow });
        }
        public object Success(string message)
        {
            return (new { type = "success", Message = message, Data = "" });
        }
        public object Success(dynamic data)
        {
            return (new { type = "success", Message = "", Data = data });
        }
        public object Notice(string message)
        {
            return (new { type = "notice", Message = message, Data = "" });
        }
        public object Warning(string message)
        {
            return (new { type = "warning", Message = message, Data = "" });
        }


        public object Error( ClaimsPrincipal User, Exception ex)
        {
            string Msg = "";
            if (ex.Message.Length < 8)
                return (new { type = "error", Message = ex.Message, Data = "" });
            string subString = ex.Message.Substring(8, ex.Message.Length - 8);

            switch (ex.Message.Substring(0, 8))
            {
                case "Column '":
                    Msg = ex.Message.Substring(8, subString.IndexOf("'") + 1) + " is required";
                    break;
                case "Error co":
                    Msg = ex.Message.Substring(0, 42);
                    break;
                case "Invalid co":
                    Msg = ex.Message.Substring(0, 42);
                    break;
                default:
                    if (ex.Message.Contains("Invalid column name '") == true)
                    {
                        Msg = subString + " is unknown";
                        break;
                    }
                    if (ex.Message.Contains("is specified more than once in the SET clause") == true)
                    {
                        subString = ex.Message.Substring(17, ex.Message.Length - 17);
                        Msg = ex.Message.Substring(16, subString.IndexOf("'") + 1) + "' is not required or specified more than once";
                        break;
                    }
                    if (ex.Message.Contains("Some accounts may not properly set. Please check the  Account Mapping !") == true)
                    {
                        Msg = "Some accounts may not properly set. Please check the  Account Mapping !";
                        break;
                    }
                    if (ex.Message.Contains("Transaction Processed"))
                    {
                        Msg = ex.Message;
                        break;
                    }
                    break;
                    // if (env.EnvironmentName == "Development")
                    // {
                    //     Msg = ex.Message;
                    //     break;
                    // }
                    // else
                    // {
                    //     Msg = "Internal Server Error";
                    //     break;
                    // }
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(ex.Message);
            File.AppendAllText( logPath + myFunctions.GetCompanyName(User) + " - " + "log.txt", sb.ToString());
            sb.Clear();
            return (new { type = "error", Message = Msg, Data = "" });


        }

        public DataTable Format(DataTable dt, string tableName)
        {
            foreach (DataColumn c in dt.Columns)
            {
                c.ColumnName = String.Join("", c.ColumnName.Split());
            }
            dt.TableName = tableName;
            return dt;
        }
        public DataTable Format(DataTable dt)
        {
            foreach (DataColumn c in dt.Columns)
            {
                c.ColumnName = String.Join("", c.ColumnName.Split());
            }
            return dt;
        }
        public string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            if(types.ContainsKey(ext))
            return types[ext];
            else
            return "unknow";
        }

        public Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }


                public string GetConnectionString(ClaimsPrincipal User,SqlConnection connection)
        {
            try
            {
                
                return "";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }






    }

    public interface IApiFunctions
    {
        /* Deprecated Method Don't Use */
        [Obsolete("IApiFunctions.Response is deprecated \n please use IApiFunctions.Success/ Error/ Warning/ Notice instead. \n\n Deprecate note added by Ratheesh KS-\n\n")]
        public object Response(int Code, string Response);
        /*  End Of Deprecated Method  */

        public object Error( ClaimsPrincipal User,Exception ex);
        public DataTable Format(DataTable table, string tableName);
        public DataTable Format(DataTable dt);
        public object Error( ClaimsPrincipal User,string message);
        public object Success(DataTable dataTable);
        public object Success(dynamic data);
        public object Success(DataTable dataTable, string message);
        public object Success(Dictionary<DataRow, DataTable> dictionary, string message);
        public object Success(Dictionary<string, string> dictionary, string message);
        public object Success(Dictionary<string, string> dictionary);
        public object Success(SortedList data);
        public object Success(SortedList result, String message);
        public object Success(DataSet dataSet);
        public object Success(string[] json);
        public object Success(string message);
        public object Success(DataSet dataSet, String message);
        public object Success(DataRow dataRow, String message);
        public object Notice(string message);
        public object Warning(string message);
        public string GetContentType(string path);
        public Dictionary<string, string> GetMimeTypes();
    }
}