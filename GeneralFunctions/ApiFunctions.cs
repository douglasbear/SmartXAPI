using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace SmartxAPI.GeneralFunctions
{
    public class ApiFunctions : IApiFunctions
    {
        private readonly IMapper _mapper;
        public ApiFunctions(IMapper mapper)
        {
            _mapper = mapper;
        }

        public object Response(int Code, string ResMessage)
        {
            return (new { StatusCode = 404, Message = ResMessage, Data = "" });
        }

        public object BadRequest(string message)
        {
            return (new { type = "error", Message = message, Data = "" });
        }
        public object Ok(DataTable dataTable)
        {
            return (new { type = "success", Message = "null", Data = dataTable });
        }
        public object Ok(DataSet dataSet)
        {
            return (new { type = "success", Message = "null", Data = dataSet });
        }
        public object Ok(string message)
        {
            return (new { type = "success", Message = message, Data = "" });
        }
        public object NotFound(string message)
        {
            return (new { type = "error", Message = message, Data = "" });
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
                    Msg = "invalid request parameters or server error occurred" + ex.Message;
                    break;
            }


            return (new { StatusCode = 403, Message = Msg });


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
        public DataTable Format(DataTable table, string tableName);
        public DataTable Format(DataTable dt);
        public object BadRequest(string message);
        public object Ok(DataTable dataTable);
        public object Ok(DataSet dataSet);
        public object Ok(string message);
        public object NotFound(string message);
    }
}