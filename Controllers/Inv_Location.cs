using AutoMapper;
using SmartxAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using ZXing;
namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("location")]
    [ApiController]
    public class Inv_Location : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly string TempFilesPath;
        public Inv_Location(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            TempFilesPath = conf.GetConnectionString("TempFilesPath");
        }
        [HttpGet("list")]
        public ActionResult GetLocationDetails(int? nCompanyId, string prs, bool bLocationRequired, bool bAllBranchData, int nBranchID, string xBarcode,bool request)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string xCondition = "";
            string xCriteria=" and  isnull(N_MainLocationID,0) =0";
            string xlocationCriteria=" ";

            if (xBarcode != "" && xBarcode != null)
            {
                xCondition = " and X_barcode='" + xBarcode + "'";

            }
            if(!request)
            {
                xlocationCriteria=" and   N_BranchID=" + nBranchID +"";
            }

            // if(bTransferLocations)
            // {
            //     xCondition = xCondition + " and N_LocationID in ( select N_LocationID from Inv_Location where N_CompanyID=@p1 and ( isnull(N_WarehouseID,0)=0 or isnull(N_TypeID,0)=5 )) ";
            // }

            string sqlCommandText = "";
            if (prs == null || prs == "")
            {
                if (nBranchID > 0)
                    sqlCommandText = "select * from vw_InvLocation_Disp where N_CompanyID=@p1 and N_BranchID=" + nBranchID + xCondition  +" order by [Location Name]";
                else
                    sqlCommandText = "select * from vw_InvLocation_Disp where N_CompanyID=@p1" + xCondition  + " order by [Location Name]";
            }
            else
            {
                if (!bLocationRequired)
                {
                    if (bAllBranchData == true)
                        sqlCommandText = "select [Location Name] as x_LocationName,* from vw_InvLocation_Disp where N_CompanyID=" + nCompanyId + xCondition ;

                    else
                        sqlCommandText = "select [Location Name] as x_LocationName,* from vw_InvLocation_Disp where   N_CompanyID=" + nCompanyId + " and  N_BranchID=" + nBranchID + xCondition ;

                }
                else
                {
                    sqlCommandText = "select [Location Name] as x_LocationName,* from vw_InvLocation_Disp where  isnull(N_MainLocationID,0) =0 and N_CompanyID=" + nCompanyId +xlocationCriteria+ xCondition;
                }
            }

            Params.Add("@p1", nCompanyId);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Warning("No Results Found"));
                }
                else
                {
                    return Ok(_api.Success(_api.Format(dt)));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }


       
   [HttpGet("dashboardList")]
        public ActionResult GetProductUnitList(int nPage ,int nSizeperpage, string xSearchkey, string xSortBy,int nCompanyId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable dt = new DataTable();
                    SortedList Params = new SortedList();
                     nCompanyId = myFunctions.GetCompanyID(User);
                    string sqlCommandCount = "", xCriteria = "";
                    int Count = (nPage - 1) * nSizeperpage;
                    string sqlCommandText = "";
                    string criteria = "";
                    string cndn = "";
                    string Searchkey = "";
                    Params.Add("@p1", nCompanyId);


                   if (xSearchkey != null && xSearchkey.Trim() != "")
                    Searchkey = "and (N_LocationID like '%" + xSearchkey + "%' OR X_LocationCode like '%" + xSearchkey + "%'  OR X_LocationName like '%" +xSearchkey + "%')";

                    
                   
                   if (xSortBy == null || xSortBy.Trim() == "")
                        xSortBy = " order by N_LocationID desc";
                      


                   if (Count == 0)
                        sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Inv_location_cloud where  N_CompanyID=@p1 "+ criteria + cndn + Searchkey + " " + xSortBy;
                    else
                        sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Inv_location_cloud where  and N_CompanyID=@p1"+ criteria + cndn + Searchkey + " " + xSortBy ;


                    SortedList OutPut = new SortedList();

                    dt = dLayer.ExecuteDataTable(sqlCommandText , Params, connection);
                   sqlCommandCount = "select count(1) as N_Count  from vw_Inv_location_cloud where N_CompanyID=@p1 ";
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    OutPut.Add("Details", _api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(_api.Warning("No Results Found"));
                    }
                    else
                    {
                        return Ok(_api.Success(OutPut));
                    }
                }
            }
              catch (Exception e)
                {
                return BadRequest(_api.Error(User, e));
                }
            

        }





        [HttpGet("listdetails")]
        public ActionResult GetLocationDetails(int? nCompanyId, int? nLocationId)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText = "select * from vw_InvLocation where N_CompanyID=@p1 and N_LocationID=@p2 order by N_LocationID DESC";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nLocationId);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Warning("No Results Found"));
                }
                else
                {
                    return Ok(_api.Success(_api.Format(dt)));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }


        [HttpPost("change")]
        public ActionResult ChangeData([FromBody] DataSet ds)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable MasterTable;
                    MasterTable = ds.Tables["master"];
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
                    int nLocationID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_LocationID"].ToString());
                    Params.Add("@nCompanyID", nCompanyID);
                    Params.Add("@nLocationID", nLocationID);

                    dLayer.ExecuteNonQuery("update Inv_Location set B_IsCurrent=0 where N_CompanyID=@nCompanyID", Params, connection);
                    dLayer.ExecuteNonQuery("update Inv_Location set B_IsCurrent=1 where N_LocationID=@nLocationID and N_CompanyID=@nCompanyID", Params, connection);

                    return Ok(_api.Success("Location Changed"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }

        //Save....
        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                MasterTable = ds.Tables["master"];
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();

                    SortedList Params = new SortedList();
                    SortedList ValidateParams = new SortedList();
                    // Auto Gen
                    String xButtonAction="";
                    string LocationCode = MasterTable.Rows[0]["x_LocationCode"].ToString();
                    char x_LocationCodePattern;
                    char initialCode;
                    String DupCriteria=" ";
                    //Limit Validation
                    ValidateParams.Add("@N_CompanyID", MasterTable.Rows[0]["n_CompanyId"].ToString());
                    string X_Pattern = "10";
                    int N_TypeID = 0;
                    int N_MainLocationID = 0;
                    string xTerminalCode ="@Auto";
                    int nFormID = 0;
                    if(!MasterTable.Columns.Contains("n_TypeId"))
                    {
                            MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable,"n_TypeId",typeof(int),0);
                            MasterTable.AcceptChanges();
                    }


                    if (myFunctions.getIntVAL(MasterTable.Rows[0]["n_TypeId"].ToString())==5)
                        nFormID = 1637;
                    else if (myFunctions.getIntVAL(MasterTable.Rows[0]["n_TypeId"].ToString())==6)
                        nFormID = 1636;
                    else
                        nFormID = 450;

                    int N_LocationID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_LocationID"].ToString());
                   // string Barcode =MasterTable.Rows[0]["X_Barcode"].ToString();
                     string barcode ="";
                      string x_LocationName = MasterTable.Rows[0]["x_LocationName"].ToString();
                      int nBranchID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_BranchID"].ToString());

                    if(N_LocationID>0)
                    xButtonAction="Update";
                    xTerminalCode="1";

                    if (MasterTable.Columns.Contains("N_MainLocationID"))
                        N_MainLocationID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_MainLocationID"].ToString());
                    if (MasterTable.Columns.Contains("n_TypeID"))
                        N_TypeID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_TypeID"].ToString());


                    object LocationCount = dLayer.ExecuteScalar("select count(N_LocationID)  from Inv_Location where N_CompanyID=@N_CompanyID", ValidateParams, connection, transaction);
                    object limit = dLayer.ExecuteScalar("select N_LocationLimit from Acc_Company where N_CompanyID=@N_CompanyID", ValidateParams, connection, transaction);
                    bool b_TransferProducts = false;
                    int n_LocationFromID = 0;
                    string TransferSql = "";
                    string patternNo = "";
                    int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    ValidateParams.Add("@N_MainLocationID", N_MainLocationID);
                    // if (LocationCount != null && limit != null)
                    // {
                    //     if (myFunctions.getIntVAL(LocationCount.ToString()) >= myFunctions.getIntVAL(limit.ToString()))
                    //     {
                    //         transaction.Rollback();
                    //         return Ok(_api.Error(User, "Location Limit exceeded!!!"));
                    //     }
                    // }
                    if (MasterTable.Columns.Contains("b_TransferProducts"))
                    {
                        b_TransferProducts = myFunctions.getBoolVAL(MasterTable.Rows[0]["b_TransferProducts"].ToString());
                        MasterTable.Columns.Remove("b_TransferProducts");
                    }

                    if (MasterTable.Columns.Contains("n_LocationFromID"))
                    {
                        n_LocationFromID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_LocationFromID"].ToString());
                        MasterTable.Columns.Remove("n_LocationFromID");
                        if (n_LocationFromID > 0)
                            TransferSql = " and N_ItemID in ( select  N_ItemID from Inv_ItemMasterWHLink where N_CompanyID=" + myFunctions.GetCompanyID(User) + " and N_WarehouseID=" + n_LocationFromID + " ) ";
                    }


                    var values = MasterTable.Rows[0]["X_LocationCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", MasterTable.Rows[0]["n_CompanyId"].ToString());
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", nFormID);
                        // Params.Add("N_BranchID", MasterTable.Rows[0]["n_BranchId"].ToString());
                        LocationCode = dLayer.GetAutoNumber("Inv_Location", "X_LocationCode", Params, connection, transaction);
                        xButtonAction="Insert";
                        if (LocationCode == "") { transaction.Rollback(); return Ok(_api.Error(User, "Unable to generate Location Code")); }
                        MasterTable.Rows[0]["X_LocationCode"] = LocationCode;
                    }

                    if (N_LocationID == 0)
                    {
                        if(N_MainLocationID==0 || N_MainLocationID ==null )
                        {
                        object mainLocationPattern=dLayer.ExecuteScalar("Select isnull(max(X_Pattern),'')  From Inv_Location Where N_CompanyID=" + nCompanyID + " and N_LocationID="+N_MainLocationID+"", connection, transaction);
                       if (mainLocationPattern == null || mainLocationPattern.ToString() == "")
                       {
                                 MasterTable.Rows[0]["X_Pattern"] = "10";

                        }
                        else
                        {
                         MasterTable.Rows[0]["X_Pattern"] = (myFunctions.getIntVAL(mainLocationPattern.ToString()) + 10).ToString() ;
                        }
                        }
                        else
                        {
                             object childPattern = dLayer.ExecuteScalar("Select isnull(max(X_Pattern),'')  From Inv_Location Where N_CompanyID=" + nCompanyID + "  and N_MainLocationID=" + N_MainLocationID + " ", connection, transaction);
                            object parentMainPattern = dLayer.ExecuteScalar("select TOP 1  X_Pattern from Inv_Location where N_LocationID=" + N_MainLocationID + " and N_CompanyID=" + nCompanyID + " ", connection, transaction);
                          if (childPattern == null || childPattern.ToString() == "")
                            {
                                
                                MasterTable.Rows[0]["X_Pattern"] = parentMainPattern.ToString() + "10";

                            }
                            else
                            {

                              
                                string lastAddedCode=childPattern.ToString().Substring(childPattern.ToString().Length-2);
                                string lengthofparent=childPattern.ToString().Substring(lastAddedCode.Length);
                                string lastString=childPattern.ToString().Remove(childPattern.ToString().Length-2);
                               string  addingCode =  (myFunctions.getIntVAL(lastAddedCode.ToString()) + 10).ToString() ;

                              MasterTable.Rows[0]["X_Pattern"] = lastString.ToString() + addingCode;



                            }
                        }
                    
                        if (N_TypeID == 1)
                        {

                            object roomPattern = dLayer.ExecuteScalar("Select isnull(max(X_LocationCode),'')  From Inv_Location Where N_CompanyID=" + nCompanyID + " and N_MainLocationID=" + N_MainLocationID + " ", connection, transaction);
                            object normalPattern = dLayer.ExecuteScalar("Select isnull(max(X_Pattern),'')  From Inv_Location Where N_CompanyID=" + nCompanyID + " and N_TypeID=1 ", connection, transaction);
                           // object mainLocationPattern=dLayer.ExecuteScalar("Select isnull(max(X_Pattern),'')  From Inv_Location Where N_CompanyID=" + nCompanyID + " and N_LocationID="+N_MainLocationID+"", connection, transaction);
                            // if (roomPattern == null || roomPattern.ToString() == "")
                            // {
                            //     x_LocationCodePattern = 'A';
                            //     // MasterTable.Rows[0]["X_LocationCode"] = x_LocationCodePattern.ToString();

                            // }
                            // else
                            // {
                            //     x_LocationCodePattern = Convert.ToChar(roomPattern);
                            //     x_LocationCodePattern++;
                            //     // MasterTable.Rows[0]["X_LocationCode"] = x_LocationCodePattern.ToString();


                            // }
                           


                        }
                        else if (N_TypeID != 0)
                        {
                            object rowPattern="";
                            object parentID= dLayer.ExecuteScalar("Select isnull(N_WareHouseID,0) From Inv_Location Where N_CompanyID=" + nCompanyID + " and N_LocationID=" + N_MainLocationID + " ", connection, transaction);
                            if(myFunctions.getIntVAL(parentID.ToString())==0)
                            {
                                if(N_TypeID==5 || N_TypeID==6)
                                {
                                    // rowPattern = dLayer.ExecuteScalar("Select isnull(max(X_LocationCode),'')  From Inv_Location Where N_CompanyID=" + nCompanyID + "  and N_MainLocationID=" + N_MainLocationID + " ", connection, transaction);
                                    // if(rowPattern==""){rowPattern="0";}
                                    // x_LocationCodePattern = Convert.ToChar(rowPattern);
                                    // x_LocationCodePattern++;
                                    // MasterTable.Rows[0]["X_LocationCode"] = x_LocationCodePattern.ToString();


                                }
                            }
                            else
                            {
                             rowPattern = dLayer.ExecuteScalar("Select isnull(max(X_LocationCode),'')  From Inv_Location Where N_CompanyID=" + nCompanyID + " and N_TypeID=" + N_TypeID + " and N_MainLocationID=" + N_MainLocationID + " ", connection, transaction);
                            object parentRomPattern = dLayer.ExecuteScalar("select TOP 1  X_LocationCode from Inv_Location where N_LocationID=" + N_MainLocationID + " and N_CompanyID=" + nCompanyID + " ", connection, transaction);
                            if (rowPattern == null || rowPattern.ToString() == "")
                            {
                                // MasterTable.Rows[0]["X_LocationCode"] = parentRomPattern.ToString() + "-" + "01";

                            }
                            else
                            {
                                int hiphenLength = rowPattern.ToString().LastIndexOf("-");
                                string addingCode = rowPattern.ToString().Substring((hiphenLength), (rowPattern.ToString().Length) - hiphenLength);
                                addingCode = addingCode.Remove(0, 1);
                                if (myFunctions.getIntVAL(addingCode) <= 9) { addingCode = "0" + (myFunctions.getIntVAL(addingCode) + 1).ToString(); }
                                else { addingCode = (myFunctions.getIntVAL(addingCode) + 1).ToString(); }
                                // MasterTable.Rows[0]["X_LocationCode"] = parentRomPattern.ToString() + "-" + addingCode;


                            }
                            }
                         


                        }
                    
                    }
                    else if (N_LocationID > 0)
                    {
                        dLayer.DeleteData("Inv_Location", "N_LocationID", N_LocationID, "", connection, transaction);
                    }

                    // if (MasterTable.Columns.Contains("B_IsDefault"))
                    // {
                    //     MasterTable.Rows[0]["B_IsDefault"] = true;
                      
                    // }else
                    // {
                    //     MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable,"B_IsDefault",typeof(bool),true);
                    // }
                    
                    if(MasterTable.Columns.Contains("X_Barcode"))
                    {

                      barcode =  MasterTable.Rows[0]["X_BarCode"].ToString();
                    if(barcode ==null || barcode =="") 
                    {
                       MasterTable.Rows[0]["X_BarCode"]= MasterTable.Rows[0]["X_LocationCode"] ;
                    }
                    }
                    else
                    {
                         MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable,"X_BarCode",typeof(string),"");
                        MasterTable.Rows[0]["X_BarCode"]= MasterTable.Rows[0]["X_LocationCode"] ;
                    }
                     if(barcode!="")
                      {
                         object barcodeCount = dLayer.ExecuteScalar("select count(N_LocationID) as Count from Inv_Location where N_CompanyID= " + nCompanyID + " and X_Barcode= '" +barcode+"'", connection, transaction);

                          if (myFunctions.getIntVAL(barcodeCount.ToString()) >0)
                          {
                                transaction.Rollback();
                                return Ok(_api.Error(User, "Barcode is already exist"));
                         }
                      }

                       DupCriteria = "N_BranchID=" + nBranchID + " and X_LocationName= '" + x_LocationName + "' and N_CompanyID=" + nCompanyID + " and N_TypeId="+N_TypeID;
             

                    MasterTable.Columns.Remove("n_FnYearId");
                    MasterTable.Columns.Remove("b_isSubLocation");
                    N_LocationID = dLayer.SaveData("Inv_Location", "N_LocationID", DupCriteria, "", MasterTable, connection, transaction);
                    if (N_LocationID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Warning("Unable to save"));
                    }
                    else
                    {
                        if (b_TransferProducts)
                        {
                            dLayer.ExecuteNonQuery("insert into Inv_ItemMasterWHLink  select ROW_NUMBER()over (Order by N_companyId)+ISNULL((Select MAX(N_RowID) from Inv_ItemMasterWHLink),0) ,N_CompanyID," + N_LocationID + ",N_ItemID,D_Entrydate from Inv_ItemMaster where  N_CompanyID=" + myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyId"].ToString()) + TransferSql, Params, connection, transaction);
                        }

                    if (xTerminalCode == "@Auto" && N_LocationID>0)
                        {
                            String sql = "select N_CompanyID,0 as N_TerminalID,'' as X_TerminalCode,X_LocationName + ' Terminal' as X_TerminalName,0 as N_UserID,getDate() as D_EntryDate,null as N_PriceTypeID,N_LocationID,N_BranchID from Inv_Location where N_CompanyID=@nCompanyID and N_LocationID=@nLocationID";
                            SortedList TerminalParams = new SortedList();
                            TerminalParams.Add("@nCompanyID", nCompanyID);
                            TerminalParams.Add("@nLocationID", N_LocationID);
                            DataTable TerminalTable = dLayer.ExecuteDataTable(sql, TerminalParams, connection, transaction);
                            if (TerminalTable.Rows.Count == 0)
                            {
                                transaction.Rollback(); return Ok(_api.Error(User, "Unable to Create location"));
                            }
                            string xLocationName = MasterTable.Rows[0]["x_LocationName"].ToString();
                            SortedList Params2 = new SortedList();
                            Params2.Add("N_CompanyID", nCompanyID);
                            Params2.Add("N_YearID", nFnYearID);
                            Params2.Add("N_FormID", 895);
                            xTerminalCode = dLayer.GetAutoNumber("Inv_Terminal", "X_TerminalCode", Params2, connection, transaction);
                            if (xTerminalCode == "") { transaction.Rollback(); return Ok(_api.Error(User, "Unable to generate location Code")); }
                            TerminalTable.Rows[0]["X_TerminalCode"] = xTerminalCode;
                            TerminalTable.AcceptChanges();
                             DupCriteria = "N_BranchID=" + nBranchID + " and X_TerminalName= '" + xLocationName + "' and N_CompanyID=" + nCompanyID;
                            int nTerminalID = dLayer.SaveData("Inv_Terminal", "N_TerminalID", DupCriteria, "", TerminalTable, connection, transaction);
                            if (nTerminalID <= 0)
                            {
                                transaction.Rollback(); return Ok(_api.Error(User, "Unable to Create terminal"));
                            }

                    
                        

                        }
                        // Activity Log
                        string ipAddress = "";
                        if (  Request.Headers.ContainsKey("X-Forwarded-For"))
                           ipAddress = Request.Headers["X-Forwarded-For"];
                        else
                           ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                           myFunctions.LogScreenActivitys(nFnYearID,N_LocationID,LocationCode,450,xButtonAction,ipAddress,"",User,dLayer,connection,transaction);

                        transaction.Commit();
                        return Ok(_api.Success("Location Saved"));
                        // return GetLocationDetails(int.Parse(MasterTable.Rows[0]["n_CompanyId"].ToString()), N_LocationID);
                    }

                

                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nLocationId,int nFnYearID)
        {
            int Results = 0;
            SortedList Params = new SortedList();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable TransData = new DataTable();
                    Params.Add("@nLocationId", nLocationId);
                    string Sql = "select N_LocationID,X_LocationCode,X_LocationName,N_BranchID,getDate() from Inv_Location where N_LocationID=@nLocationId and N_CompanyID=N_CompanyID";
                    object count = dLayer.ExecuteScalar("select count(1) as N_Count from vw_Inv_Location_Disp where N_LocationID=@nLocationId and N_CompanyID=N_CompanyID", Params, connection);
                    int N_Count = myFunctions.getIntVAL(count.ToString());
                    string xButtonAction="Delete";
                    string X_LocationCode = "";
                    TransData = dLayer.ExecuteDataTable(Sql, Params, connection);
                    SqlTransaction transaction = connection.BeginTransaction();
                    // Activity Log
                            string ipAddress = "";
                            if (  Request.Headers.ContainsKey("X-Forwarded-For"))
                                ipAddress = Request.Headers["X-Forwarded-For"];
                            else
                                ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                                myFunctions.LogScreenActivitys(myFunctions.getIntVAL( nFnYearID.ToString()),nLocationId,TransData.Rows[0]["X_LocationCode"].ToString(),450,xButtonAction,ipAddress,"",User,dLayer,connection,transaction);
                    
                    if (N_Count <= 0)
                    {
                        Results = dLayer.DeleteData("Inv_Location", "N_LocationID", nLocationId, "", connection,transaction);

                    }
                    DataRow TransRow = TransData.Rows[0];
                    
                
                
                if (Results > 0)
                {
                    transaction.Commit();
                    return Ok(_api.Success("Location deleted"));
                }
                else
                {
                    transaction.Rollback();
                    return Ok(_api.Error(User, "Unable to delete Location"));
                }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }


        }
        [HttpGet("generatebarcode")]
        public ActionResult GenerateBarcode(string name, string barcode)
        {
            try
            {
                return Ok(_api.Success(new SortedList() { { "FileName", barcode } }));

            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }

        }
        public bool CreateBarcode(string Data)
        {
            if (Data != "")
            {
                Zen.Barcode.Code128BarcodeDraw barcode = Zen.Barcode.BarcodeDrawFactory.Code128WithChecksum;
                Image img = barcode.Draw(Data, 50);
                img.Save("C://OLIVOSERVER2020/Barcode/" + Data + ".png", ImageFormat.Png);
            }
            return true;
        }
        
        // [HttpGet("printbarcode")]
        // public IActionResult GetModulePrint(string xBarcode)
        // {
        //     int nCompanyId = myFunctions.GetCompanyID(User);
        //     try
        //     {
        //         var myBitmap = new Bitmap(500, 50);
        //         var g = Graphics.FromImage(myBitmap);
        //         var jgpEncoder = GetEncoder(ImageFormat.Jpeg);

        //         g.Clear(Color.White);

        //         var strFormat = new StringFormat { Alignment = StringAlignment.Center };
        //         g.DrawString(xBarcode, new System.Drawing.Font("Free 3 of 9", 50), Brushes.Black, new RectangleF(0, 0, 500, 50), strFormat);

        //         var myEncoder = Encoder.Quality;
        //         var myEncoderParameters = new EncoderParameters(1);

        //         var myEncoderParameter = new EncoderParameter(myEncoder, 100L);
        //         myEncoderParameters.Param[0] = myEncoderParameter;
        //         string BarcodePath = this.TempFilesPath;
        //         DirectoryInfo info = new DirectoryInfo(BarcodePath);
        //         if (!info.Exists)
        //         {
        //             info.Create();
        //         }
        //         BarcodePath = BarcodePath + "/Barcode.jpg";
        //         myBitmap.Save(@"c:\Barcode.jpg", jgpEncoder, myEncoderParameters);
        //         // myBitmap.Save(@"C://OLIVOSERVER2020/Barcode/Barcode.jpg", jgpEncoder, myEncoderParameters);

        //         return Ok(_api.Success(new SortedList() { { "FileName", "Barcode.jpg" } }));

        //     }
        //     catch (Exception e)
        //     {
        //         return Ok(_api.Error(User, e));
        //     }

        // }
        // private static void CreateBarcode(string code)
        // {
        //     var myBitmap = new Bitmap(500, 50);
        //     var g = Graphics.FromImage(myBitmap);
        //     var jgpEncoder = GetEncoder(ImageFormat.Jpeg);

        //     g.Clear(Color.White);

        //     var strFormat = new StringFormat { Alignment = StringAlignment.Center };
        //     g.DrawString(code, new System.Drawing.Font("Free 3 of 9", 50), Brushes.Black, new RectangleF(0, 0, 500, 50), strFormat);

        //     var myEncoder = Encoder.Quality;
        //     var myEncoderParameters = new EncoderParameters(1);

        //     var myEncoderParameter = new EncoderParameter(myEncoder, 100L);
        //     myEncoderParameters.Param[0] = myEncoderParameter;
        //     string BarcodePath = TempFilesPath;
        //     DirectoryInfo info = new DirectoryInfo(BarcodePath);
        //     if (!info.Exists)
        //     {
        //         info.Create();
        //     }
        //     // myBitmap.Save(@"c:\Barcode.jpg", jgpEncoder, myEncoderParameters);
        //     myBitmap.Save(@"C://OLIVOSERVER2020/Barcode/Barcode.jpg", jgpEncoder, myEncoderParameters);
        // }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {

            var codecs = ImageCodecInfo.GetImageDecoders();

            foreach (var codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
    }
}