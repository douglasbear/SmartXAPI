using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SmartxAPI.GeneralFunctions;
using System;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Security.Claims;

namespace SmartxAPI.Controllers

{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("Requestforquotation")]
    [ApiController]
    public class Inv_RequestForQuotation : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly IMyAttachments myAttachments;
        private readonly string connectionString;
        private readonly int FormID = 198;
        public Inv_RequestForQuotation(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf, IMyAttachments myAtt)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
           // N_FormID = 64;
        }

        [HttpGet("list")]
        public ActionResult GetSalesInvoiceList(int nFnYearId, int nPage, bool bAllBranchData, int nBranchID,int N_RequestId, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    int nCompanyId = myFunctions.GetCompanyID(User);
                    DataTable dt = new DataTable();
                    SortedList Params = new SortedList();
                    
                    int Count = (nPage - 1) * nSizeperpage;
                    string sqlCommandText = "";
                    string sqlCommandCount = "";
                    string Searchkey = "";
                    string X_TransType = "";
                    
                 
                    if (xSortBy == null || xSortBy.Trim() == "")
                        xSortBy = " order by D_RequestDate  desc";
                    else
                    {
                        switch (xSortBy.Split(" ")[0])
                        {
                            case "invoiceNo":
                                xSortBy = "D_RequestDate " + xSortBy.Split(" ")[1];
                                break;
                            default: break;
                        }
                        xSortBy = " order by " + xSortBy;
                    }

                    if (myCompanyID._B_AllBranchData == true)
                        sqlCommandText = "select top(" + nSizeperpage + ") * from vw_RequestQuotation_Disp where N_CompanyID=@p1 and N_FnYearID=@p2  " + Searchkey + " " + xSortBy;
                    else
                        sqlCommandText = "select top(" + nSizeperpage + ") * from vw_RequestQuotation_Disp where N_CompanyID=@p1 and N_FnYearID=@p2 and n_branchid= "   + xSearchkey + xSortBy + " ) " + xSortBy;

                    Params.Add("@p1", nCompanyId);
                    Params.Add("@p2", nFnYearId);
                    Params.Add("@p3",nBranchID);

                    SortedList OutPut = new SortedList();

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
                return Ok(_api.Error(e));
            }
        }



          [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {

                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    DataTable MasterTable;
                    DataTable DetailTable;
                    DataTable MultiVendorTabe;
                    string DocNo = "";
                    MasterTable = ds.Tables["master"];
                    DetailTable = ds.Tables["details"];
                    MultiVendorTabe = ds.Tables["vendorDetails"];
                    DataRow MasterRow = MasterTable.Rows[0];
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
                    int nQuotationID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_QuotationID"].ToString());
                    int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                    string X_QuotationNo = MasterTable.Rows[0]["x_QuotationNo"].ToString();
                    int nBranchID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_BranchID"].ToString());

                    if (nQuotationID > 0)
                    {
                        // SortedList deleteParams = new SortedList()
                        //     {
                        //         {"N_CompanyID",nCompanyID},

                        //         {"N_ReceiptId",nReceiptID}
                        //     };
                        dLayer.DeleteData("Inv_VendorRequestDetails", "N_QuotationID", nQuotationID, "N_CompanyID = " + nCompanyID, connection, transaction);
                        dLayer.DeleteData("Inv_VendorRequest", "N_QuotationID", nQuotationID, "N_CompanyID = " + nCompanyID, connection, transaction);
                    }
                    DocNo = MasterRow["x_QuotationNo"].ToString();
                    if (X_QuotationNo == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_FormID", FormID);
                        Params.Add("N_YearID", nFnYearID);

                        while (true)
                        {
                            DocNo = dLayer.ExecuteScalarPro("SP_AutoNumberGenerate", Params, connection, transaction).ToString();
                            object N_Result = dLayer.ExecuteScalar("Select 1 from Inv_VendorRequest Where X_ReceiptNo ='" + DocNo + "' and N_CompanyID= " + nCompanyID, connection, transaction);
                            if (N_Result == null)
                                break;
                        }
                        X_QuotationNo = DocNo;


                        if (X_QuotationNo == "") { transaction.Rollback(); return Ok(_api.Error("Unable to generate")); }
                        MasterTable.Rows[0]["x_QuotationNo"] = X_QuotationNo;

                    }
                    string DupCriteria = "N_CompanyID=" + nCompanyID + " and N_AcYearID=" + nFnYearID + " and X_ReceiptNo='" + X_QuotationNo + "'";
                    string X_Criteria = "N_CompanyID=" + nCompanyID + " and N_AcYearID=" + nFnYearID;
                    nQuotationID = dLayer.SaveData("Inv_VendorRequest", "N_QuotationID", DupCriteria, X_Criteria, MasterTable, connection, transaction);
                    if (nQuotationID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable To Save"));
                    }
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["n_QuotationID"] = nQuotationID;
                    }
                    if (FormID == 1049)
                    {
                        for (int j = 0; j < DetailTable.Rows.Count; j++)
                        {

                            int n_QuotationDetailsID = dLayer.SaveDataWithIndex("Inv_VendorRequestDetails", "n_QuotationDetailsID", "", "", j, DetailTable, connection, transaction);
                            if (n_QuotationDetailsID <= 0)
                            {
                                transaction.Rollback();
                                return Ok("Unable to save Accrual Code");
                            }

                            if (MultiVendorTabe.Rows.Count > 0)
                            {
                                // MultiVendorTabe.Rows[ ]

                            }
                        }
                    }
                    transaction.Commit();
                    return Ok(_api.Success("Asset Purchase Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }

    }
}
