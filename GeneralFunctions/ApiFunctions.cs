using System;
using System.Collections.Generic;
using System.Data;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;

namespace SmartxAPI.GeneralFunctions
{
    public class ApiFunctions : IApiFunctions
    {
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment env;
        public ApiFunctions(IMapper mapper, IWebHostEnvironment envn)
        {
            _mapper = mapper;
            env = envn;
        }

        public object Response(int Code, string ResMessage)
        {
            return (new { StatusCode = Code, Message = ResMessage, Data = "" });
        }

        public object Error(string message)
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

        public object Success(Dictionary<DataRow,DataTable> dictionary, string message)
        {
            return (new { type = "success", Message = message, Data = dictionary });
        }
        public object Success(DataSet dataSet)
        {
            return (new { type = "success", Message = "null", Data = dataSet });
        }
        public object Success(DataSet dataSet, String message)
        {
            return (new { type = "success", Message = message, Data = dataSet });
        }
        public object Success(string message)
        {
            return (new { type = "success", Message = message, Data = "" });
        }
        public object Notice(string message)
        {
            return (new { type = "notice", Message = message, Data = "" });
        }
        public object Warning(string message)
        {
            return (new { type = "warning", Message = message, Data = "" });
        }
        public object ErrorResponse(Exception ex)
        {
            string Msg = "";
            string subString = ex.Message.Substring(8, ex.Message.Length - 8);

            switch (ex.Message.Substring(0, 8))
            {
                case "Column '":
                    Msg = ex.Message.Substring(7, subString.IndexOf("'") + 1) + " is required";
                    break;
                case "Error co":
                    Msg = ex.Message.Substring(0, 42);
                    break;
                default:
                    if (env.EnvironmentName == "Development")
                        Msg = ex.Message;
                    else
                        Msg = "Internal Server Error";
                    break;
            }


            return (new { type = "error", Message = Msg, Data = "" });


        }

        public object Error(Exception ex)
        {
            string Msg = "";
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
                        Msg = ex.Message.Substring(20, subString.IndexOf("'") + 1) + " is unknown";
                        break;
                    }
                    if (ex.Message.Contains("is specified more than once in the SET clause") == true)
                    {
                        subString = ex.Message.Substring(17, ex.Message.Length - 17);
                        Msg = ex.Message.Substring(16, subString.IndexOf("'") + 1) + "' is not required or specified more than once";
                        break;
                    }
                    if (env.EnvironmentName == "Development")
                        Msg = ex.Message;
                    else
                        Msg = "Internal Server Error";
                    break;
            }


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

    }
    public interface IApiFunctions
    {
        public object Response(int Code, string Response);
        public object ErrorResponse(Exception ex);
        public object Error(Exception ex);
        public DataTable Format(DataTable table, string tableName);
        public DataTable Format(DataTable dt);
        public object Error(string message);
        public object Success(DataTable dataTable);
        public object Success(DataTable dataTable, string message);
        public object Success(Dictionary<DataRow,DataTable> dictionary, string message);
        public object Success(DataSet dataSet);
        public object Success(string message);
        public object Success(DataSet dataSet, String message);
        public object Notice(string message);
        public object Warning(string message);
    }
}