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
    public class TaskController : ITaskController
    {
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment env;
        private readonly IMyFunctions myFunctions;
        private readonly string logPath;
        private readonly IDataAccessLayer dLayer;
        public TaskController(IMapper mapper, IWebHostEnvironment envn, IMyFunctions myFun,IDataAccessLayer dl, IConfiguration conf)
        {
            _mapper = mapper;
            env = envn;
            myFunctions = myFun;
            logPath = conf.GetConnectionString("LogPath");
                        dLayer = dl;
        }
          public bool SaveGeneralTask(int nCompanyID,string taskSummary, string taskDescription, int assigneeID,int creatorID, int submitterid,int closeduserid, DateTime dueDate,DateTime startDate,DateTime entryDate,int status,int  salesOrderDetailsID,  SqlConnection connection, SqlTransaction transaction)
        {

           try
            {
              SortedList Params = new SortedList();
              SortedList ParamsAdd = new SortedList();
              Params.Add("@nCompanyID", nCompanyID);
              string due="";
              string start="";
              string entry="";
              string X_TaskCode="";
              string DocNo="";
               due=myFunctions.getDateVAL(dueDate);
               start= myFunctions.getDateVAL(startDate);
               entry= myFunctions.getDateVAL(entryDate);



                       ParamsAdd.Add("N_CompanyID", nCompanyID);
                        ParamsAdd.Add("N_FormID", 1056);
                        while (true)
                        {

                            object N_Result = dLayer.ExecuteScalar("Select 1 from Tsk_TaskMaster Where X_TaskCode ='" + DocNo + "' and N_CompanyID= " + nCompanyID, connection, transaction);
                            if (N_Result == null) DocNo = dLayer.ExecuteScalarPro("SP_AutoNumberGenerate", ParamsAdd, connection, transaction).ToString();
                            break;
                        }
                        X_TaskCode = DocNo;
                   
              DataTable TaskMaster = dLayer.ExecuteDataTable(
                        " select "+nCompanyID+" as N_CompanyID,0 as N_TaskID,"+X_TaskCode+" as X_TaskCode,'"+taskSummary+"' as X_TaskSummery,'"+taskDescription+"' as X_TaskDescription,"
                        + "'"+start+"' as D_TaskDate,'"+due+"' as D_DueDate,"+creatorID+" as N_CreatorID,0 as B_Closed,"+status+" as N_StatusID,"+assigneeID+" as N_AssigneeID,"+submitterid+" as N_SubmitterID,"
                        + ""+closeduserid+" as N_ClosedUserID, '"+entry+"' as D_EntryDate,"+assigneeID+" as N_CurrentAssigneeID ,"+salesOrderDetailsID+" as N_ServiceDetailsID,0 as N_CompletedPercentage" ,Params, connection, transaction);

              DataTable TaskDetails =dLayer.ExecuteDataTable(
                        " select "+nCompanyID+" as N_CompanyID,0 as N_TaskID,0 as N_TaskStatusID ,"+status+" as  N_Status,'"+entry+"' as D_EntryDate,"
                        + " "+assigneeID+" as N_AssigneeID,"+submitterid+" as N_SubmitterID, "+creatorID+" as N_CreaterID , "
                        + ""+closeduserid+" as N_ClosedUserID",Params, connection, transaction);
                             
                        DataRow row = TaskDetails.NewRow();
                        row["N_TaskID"] = 0;
                        row["n_TaskStatusID"] = 0;
                        row["n_CompanyId"] = nCompanyID;
                        row["n_AssigneeID"] = 0;
                        row["n_CreaterID"] =creatorID;
                        row["n_SubmitterID"] = 0;
                        row["n_ClosedUserID"] = 0;
                        row["n_Status"] = 1;
                        row["d_EntryDate"] = entryDate;
                        TaskDetails.Rows.InsertAt(row, 0);


                       int  nTaskId = dLayer.SaveData("Tsk_TaskMaster", "N_TaskID", TaskMaster, connection, transaction);
                        if (nTaskId <= 0)
                         {
                           transaction.Rollback();
                           return false;
                      
                         }
                         for (int i = 0; i < TaskDetails.Rows.Count; i++)
                          {
                            TaskDetails.Rows[i]["N_TaskID"] = nTaskId;
                          }
                        int nTaskStatusID = dLayer.SaveData("Tsk_TaskStatus", "N_TaskStatusID", TaskDetails, connection, transaction);
                        if (nTaskStatusID <= 0)
                        {
                          transaction.Rollback();
                         return false;
                        }
                        else
                        {
                             return true;

                        }
        }
           catch (Exception ex)
            {
                return false;
            }

    }




    }

    public interface ITaskController
    {
        /* Deprecated Method Don't Use */
        [Obsolete("IApiFunctions.Response is deprecated \n please use IApiFunctions.Success/ Error/ Warning/ Notice instead. \n\n Deprecate note added by Ratheesh KS-\n\n")]
        public bool SaveGeneralTask(int nCompanyID,string taskSummary, string taskDescription, int assigneeID,int creatorID, int submitterid,int closeduserid, DateTime dueDate,DateTime startDate,DateTime entryDate,int status,int salesOrderDetailsID,   SqlConnection connection, SqlTransaction transaction);
    }
}