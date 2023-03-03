using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SmartxAPI.GeneralFunctions;
using System;
using System.Data;
using System.Collections;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;


namespace SmartxAPI.Controllers

{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("whgrn")]
    [ApiController]
    public class WhGRN : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly IMyAttachments myAttachments;
        private readonly string connectionString;
        private readonly int N_FormID;
        public WhGRN(IApiFunctions api, IDataAccessLayer dl, IMyFunctions fun, IConfiguration conf, IMyAttachments myAtt)
        {
            _api = api;
            dLayer = dl;
            myFunctions = fun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
            N_FormID = 1407;
        }
        [HttpGet("list")]
        public ActionResult GetWhGRNList(int? nCompanyId, int nFnYearId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable dt = new DataTable();
                    DataTable CountTable = new DataTable();
                    SortedList Params = new SortedList();
                    DataSet dataSet = new DataSet();
                    string sqlCommandText = "";
                    string sqlCommandCount = "";
                    string Searchkey = "";

                    int nUserID = myFunctions.GetUserID(User);


                    if (xSearchkey != null && xSearchkey.Trim() != "")
                        Searchkey = "and ([X_GRNNo] like '%" + xSearchkey + "%' or N_GRNID like '%" + xSearchkey + "%')";

                    if (xSortBy == null || xSortBy.Trim() == "")
                        xSortBy = " order by N_GRNID desc";
                    else
                    {
                        switch (xSortBy.Split(" ")[0])
                        {
                            case "X_GRNNo":
                                xSortBy = "X_GRNNo " + xSortBy.Split(" ")[1];
                                break;
                            case "N_GRNID":
                                xSortBy = "N_GRNID " + xSortBy.Split(" ")[1];
                                break;
                            default: break;
                        }
                        xSortBy = " order by " + xSortBy;
                    }

                    int Count = (nPage - 1) * nSizeperpage;
                    if (Count == 0)
                        sqlCommandText = "select top(" + nSizeperpage + ") [X_GRNNo] AS X_GRNNo,* from vw_Wh_GRN_Disp where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey + " " + xSortBy;
                    else
                        sqlCommandText = "select top(" + nSizeperpage + ") [X_GRNNo] AS X_GRNNo,* from vw_Wh_GRN_Disp where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey + " and N_GRNID not in (select top(" + Count + ") N_GRNID from vw_Wh_GRN_Disp where N_CompanyID=@p1 and N_FnYearID=@p2 " + xSortBy + " ) " + xSortBy;

                    // sqlCommandText = "select * from Inv_MRNDetails where N_CompanyID=@p1";
                    Params.Add("@p1", nCompanyId);
                    Params.Add("@p2", nFnYearId);
                    SortedList OutPut = new SortedList();


                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCommandCount = "select count(1) as N_Count from vw_Wh_GRN_Disp where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey + "";
                    DataTable Summary = dLayer.ExecuteDataTable(sqlCommandCount, Params, connection);
                    string TotalCount = "0";
                    if (Summary.Rows.Count > 0)
                    {
                        DataRow drow = Summary.Rows[0];
                        TotalCount = drow["N_Count"].ToString();
                    }
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
                return Ok(_api.Error(User, e));
            }
        }



        [HttpGet("details")]
        public ActionResult GetDetails(string XGRNNo, int nFnYearID, int nCompanyID, int nBranchID, bool bShowAllBranchData, string xAsnDocNo)
        {
            DataTable Master = new DataTable();
            DataTable Detail = new DataTable();
            DataSet ds = new DataSet();
            SortedList Params = new SortedList();
            SortedList QueryParams = new SortedList();
            DataTable Attachments = new DataTable();
            int companyid = myFunctions.GetCompanyID(User);

            QueryParams.Add("@nCompanyID", nCompanyID);

            QueryParams.Add("@nBranchID", nBranchID);
            QueryParams.Add("@nFnYearID", nFnYearID);
            string Condition = "";
            string _sqlQuery = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();


                    if (xAsnDocNo != "" && xAsnDocNo != null)
                    {
                        QueryParams.Add("@xAsnDocNo", xAsnDocNo);
                        _sqlQuery = "Select * from Vw_AsnMasterToGRNMaster Where n_Companyid=@nCompanyID and X_AsnDocNo =@xAsnDocNo and N_FnYearID=@nFnYearID";
                    }
                    else
                    {
                        QueryParams.Add("@XGRNNo", XGRNNo);
                        if (bShowAllBranchData == true)
                            Condition = " n_Companyid=@nCompanyID and X_GRNNo =@XGRNNo and N_FnYearID=@nFnYearID";
                        else
                            Condition = " n_Companyid=@nCompanyID and X_GRNNo =@XGRNNo and N_FnYearID=@nFnYearID and N_BranchID=@nBranchID";

                        _sqlQuery = "Select * from vw_Wh_GRN_Disp Where " + Condition + "";
                    }


                    Master = dLayer.ExecuteDataTable(_sqlQuery, QueryParams, connection);


                    Master = _api.Format(Master, "master");

                    if (Master.Rows.Count == 0)
                    {
                        return Ok(_api.Notice("No Results Found"));
                    }
                    else
                    {

                        ds.Tables.Add(Master);
                        if (xAsnDocNo != null)
                        {
                            QueryParams.Add("@nAsnID", Master.Rows[0]["N_AsnID"].ToString());

                            _sqlQuery = "Select * from Vw_AsnDetailsToGRNDetails Where N_CompanyID=@nCompanyID and N_AsnID=@nAsnID";

                        }
                        else
                        {
                            QueryParams.Add("@N_GRNID", Master.Rows[0]["N_GRNID"].ToString());

                            _sqlQuery = "Select * from vw_Wh_GRNDetails Where N_CompanyID=@nCompanyID and N_GRNID=@N_GRNID";
                        }

                        Detail = dLayer.ExecuteDataTable(_sqlQuery, QueryParams, connection);

                        Detail = _api.Format(Detail, "details");
                        DataTable Attachements = myAttachments.ViewAttachment(dLayer, myFunctions.getIntVAL(Master.Rows[0]["N_CustomerID"].ToString()), myFunctions.getIntVAL(Master.Rows[0]["N_GRNID"].ToString()), this.N_FormID, myFunctions.getIntVAL(Master.Rows[0]["N_FnYearID"].ToString()), User, connection);
                        Attachements = _api.Format(Attachements, "attachments");
                        if (Detail.Rows.Count == 0)
                        {
                            return Ok(_api.Notice("No Results Found"));
                        }
                        ds.Tables.Add(Detail);
                      
                         ds.Tables.Add(Attachements);


                        return Ok(_api.Success(ds));
                    }


                }


            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }


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
                    DataRow MasterRow = MasterTable.Rows[0];
                    DataTable DetailTable;
                    MasterTable = ds.Tables["master"];
                    DetailTable = ds.Tables["details"];
                    DataTable Attachment = ds.Tables["attachments"];
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_CompanyID"].ToString());
                    int nGrnID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_GRNID"].ToString());
                    int N_UserID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_UserID"].ToString());
                    int N_FnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_FnYearID"].ToString());
                    int N_CustomerID = myFunctions.getIntVAL(MasterRow["n_CustomerID"].ToString());
                    string X_GRNNo = "";
                    var values = MasterTable.Rows[0]["X_GRNNo"].ToString();
                    string i_Signature = "";
                    bool SigEnable = false;
                    string i_signature2="";


                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_FormID", 1407);
                        Params.Add("N_YearID", N_FnYearID);

                        X_GRNNo = dLayer.GetAutoNumber("wh_GRN", "X_GRNNo", Params, connection, transaction);
                        if (X_GRNNo == "")
                        {
                            transaction.Rollback();
                            return Ok(_api.Warning("Unable to generate"));
                        }
                        MasterTable.Rows[0]["X_GRNNo"] = X_GRNNo;
                    }

                    if (nGrnID > 0)
                    {
                        SortedList DeleteParams = new SortedList(){
                                {"N_CompanyID",nCompanyID},
                                {"X_TransType","WHGRN"},
                                {"N_VoucherID",nGrnID},
                                {"N_UserID",N_UserID},
                                {"X_SystemName","WebRequest"},
                                {"B_MRNVisible","0"}};

                        try
                        {
                            dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_PurchaseAccounts", DeleteParams, connection, transaction);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, ex));
                        }
                    }

                    //Signature
                    Byte[] ImageBitmap = new Byte[i_Signature.Length];
                    if (MasterTable.Columns.Contains("i_signature"))
                    {
                        if (!MasterRow["i_signature"].ToString().Contains("undefined") && !MasterRow["i_signature"].ToString().Contains("ZGF0YTppbWFnZS9wbmc7YmFzZTY0LHVuZGVmaW5lZA=="))
                        {
                            i_Signature = Regex.Replace(MasterRow["i_signature"].ToString(), @"^data:image\/[a-zA-Z]+;base64,", string.Empty);
                            if (myFunctions.ContainColumn("i_signature", MasterTable))
                                MasterTable.Columns.Remove("i_signature");
                            ImageBitmap = new Byte[i_Signature.Length];
                            ImageBitmap = Convert.FromBase64String(i_Signature);
                            SigEnable = true;
                        }
                    }

                         Byte[] ImageBitmap2 = new Byte[i_signature2.Length];
                         if (MasterTable.Columns.Contains("i_signature2"))
                    {
                        if (!MasterRow["i_signature2"].ToString().Contains("undefined") && !MasterRow["i_signature2"].ToString().Contains("ZGF0YTppbWFnZS9wbmc7YmFzZTY0LHVuZGVmaW5lZA=="))
                        {
                            i_signature2 = Regex.Replace(MasterRow["i_signature2"].ToString(), @"^data:image\/[a-zA-Z]+;base64,", string.Empty);
                            if (myFunctions.ContainColumn("i_signature2", MasterTable))
                                MasterTable.Columns.Remove("i_signature2");
                            ImageBitmap2 = new Byte[i_signature2.Length];
                            ImageBitmap2 = Convert.FromBase64String(i_signature2);
                            SigEnable = true;
                        }
                    }


                    nGrnID = dLayer.SaveData("wh_GRN", "N_GRNID", MasterTable, connection, transaction);
                    //Saving Signature
                    if (SigEnable)
                    {
                        if (i_Signature.Length > 0)
                        {
                             MemoryStream stream = new MemoryStream(ImageBitmap);
                            Image Sign = Image.FromStream(stream);

                            using (var b = new Bitmap(Sign.Width, Sign.Height))
                            {
                                b.SetResolution(Sign.HorizontalResolution, Sign.VerticalResolution);

                                using (var g = Graphics.FromImage(b))
                                {
                                    g.Clear(Color.White);
                                    g.DrawImageUnscaled(Sign, 0, 0);
                                }
                                b.Save(stream,ImageFormat.Png);
                               
                            }

                               byte[] bmpBytes = stream.ToArray();
                                dLayer.SaveImage("wh_GRN", "i_signature", bmpBytes, "N_GRNID", nGrnID, connection, transaction);
                        }

                        if(i_signature2.Length > 0){
                            MemoryStream stream2 = new MemoryStream(ImageBitmap2);
                            Image Sign2 = Image.FromStream(stream2);

                            using (var b = new Bitmap(Sign2.Width, Sign2.Height))
                            {
                                b.SetResolution(Sign2.HorizontalResolution, Sign2.VerticalResolution);

                                using (var g = Graphics.FromImage(b))
                                {
                                    g.Clear(Color.White);
                                    g.DrawImageUnscaled(Sign2, 0, 0);
                                }
                                b.Save(stream2,ImageFormat.Png);
                               
                            }

                             byte[] bmpBytes2 = stream2.ToArray();
                           
                             dLayer.SaveImage("wh_GRN", "i_signature2", bmpBytes2, "N_GRNID", nGrnID, connection, transaction);

                          
                        }   
                            
                        }
                    


                    if (nGrnID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to save"));
                    }
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["N_GrnID"] = nGrnID;
                    }
                    int N_GRNDetailsID = dLayer.SaveData("wh_GRNDetails", "N_GRNDetailsID", DetailTable, connection, transaction);
                    if (N_GRNDetailsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to save"));

                    }


                    try
                    {

                        SortedList StockPosting = new SortedList();
                        StockPosting.Add("N_CompanyID", nCompanyID);
                        StockPosting.Add("N_GRNID", nGrnID);
                        StockPosting.Add("N_UserID", N_UserID);
                        StockPosting.Add("X_SystemName", "ERP Cloud");
                        dLayer.ExecuteNonQueryPro("[SP_Inv_AllocateNegStock_WHGRN]", StockPosting, connection, transaction);

                        SortedList PostingParam = new SortedList();
                        PostingParam.Add("N_CompanyID", nCompanyID);
                        PostingParam.Add("X_InventoryMode", "GRN");
                        PostingParam.Add("N_InternalID", nGrnID);
                        PostingParam.Add("N_UserID", N_UserID);
                        PostingParam.Add("X_SystemName", "ERP Cloud");
                        dLayer.ExecuteNonQueryPro("SP_Acc_Inventory_Purchase_Posting", PostingParam, connection, transaction);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, ex));
                    }
                    SortedList WareHouseParams = new SortedList();
                    WareHouseParams.Add("@nCustomerID", N_CustomerID);
                    DataTable CustomerInfo = dLayer.ExecuteDataTable("Select X_CustomerCode,X_CustomerName from Inv_Customer where N_CustomerID=@nCustomerID", WareHouseParams, connection, transaction);
                    if (CustomerInfo.Rows.Count > 0)
                    {
                        try
                        {

                            myAttachments.SaveAttachment(dLayer, Attachment, X_GRNNo, nGrnID, CustomerInfo.Rows[0]["X_CustomerName"].ToString().Trim(), CustomerInfo.Rows[0]["X_CustomerCode"].ToString(), myFunctions.getIntVAL(MasterTable.Rows[0]["n_CustomerID"].ToString()), "WareHouse Document", User, connection, transaction);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, ex));
                        }
                    }
                    transaction.Commit();
                    SortedList Result = new SortedList();
                    Result.Add("N_GRNDetailsID", nGrnID);
                    Result.Add("X_GRNNo", X_GRNNo);
                    return Ok(_api.Success(Result, "Saved"));
                }
            }

            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }

        }




        [HttpDelete("delete")]
        public ActionResult DeleteData(int nGRNID, int nCustomerID, int nFnYearID)
        {
            int Results = 0;
            try
            {
                int nCompanyID = myFunctions.GetCompanyID(User);
                SortedList QueryParams = new SortedList();
                QueryParams.Add("@nCompanyID", nCompanyID);
                QueryParams.Add("@nFnYearID", nFnYearID);
                QueryParams.Add("@nGRNID", nGRNID);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();

                    SortedList DeleteParams = new SortedList(){
                                {"N_CompanyID",nCompanyID},
                                {"X_TransType","WHGRN"},
                                {"N_VoucherID",nGRNID},
                                {"N_UserID",myFunctions.GetUserID(User)},
                                // {"@nPartyID",nCustomerID},
                                {"X_SystemName","WebRequest"},
                                {"@B_MRNVisible","0"}};

                    Results = dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_PurchaseAccounts", DeleteParams, connection, transaction);
                    if (Results <= 0)
                    {
                        myAttachments.DeleteAttachment(dLayer, 1, nGRNID, nCustomerID, nFnYearID, this.N_FormID, User, transaction, connection);
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to Delete Goods Receive Note"));
                    }
                    transaction.Commit();
                    return Ok(_api.Success("WhGRN deleted"));


                }

            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }


        }

        [HttpGet("grnLocation")]
        public ActionResult AccruedTypeList(int nItemID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nComapnyID", nCompanyID);
            SortedList OutPut = new SortedList();
            string sqlCommandText = "select * from Vw_ItemWiseLocation where N_ItemID=" + nItemID + " and N_CompanyID=" + nCompanyID + "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                dt = _api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(_api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }




    }
}





