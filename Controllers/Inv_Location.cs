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
        public ActionResult GetLocationDetails(int? nCompanyId, string prs, bool bLocationRequired, bool bAllBranchData, int nBranchID, string xBarcode)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string xCondition = "";
            if (xBarcode != "" && xBarcode != null)
            {
                xCondition = " and X_barcode='" + xBarcode + "'";

            }

            // if(bTransferLocations)
            // {
            //     xCondition = xCondition + " and N_LocationID in ( select N_LocationID from Inv_Location where N_CompanyID=@p1 and ( isnull(N_WarehouseID,0)=0 or isnull(N_TypeID,0)=5 )) ";
            // }

            string sqlCommandText = "";
            if (prs == null || prs == "")
            {
                if (nBranchID > 0)
                    sqlCommandText = "select * from vw_InvLocation_Disp where N_CompanyID=@p1 and N_BranchID=" + nBranchID + xCondition + " order by [Location Name]";
                else
                    sqlCommandText = "select * from vw_InvLocation_Disp where N_CompanyID=@p1" + xCondition + " order by [Location Name]";
            }
            else
            {
                if (!bLocationRequired)
                {
                    if (bAllBranchData == true)
                        sqlCommandText = "select [Location Name] as x_LocationName,* from vw_InvLocation_Disp where N_MainLocationID =0 and N_CompanyID=" + nCompanyId + xCondition;

                    else
                        sqlCommandText = "select [Location Name] as x_LocationName,* from vw_InvLocation_Disp where  N_MainLocationID =0 and N_CompanyID=" + nCompanyId + " and  N_BranchID=" + nBranchID + xCondition;

                }
                else
                {
                    sqlCommandText = "select [Location Name] as x_LocationName,* from vw_InvLocation_Disp where  isnull(N_MainLocationID,0) =0 and N_CompanyID=" + nCompanyId + " and  N_BranchID=" + nBranchID + xCondition;
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
                    string LocationCode = "";
                    char x_LocationCodePattern;
                    char initialCode;
                    //Limit Validation
                    ValidateParams.Add("@N_CompanyID", MasterTable.Rows[0]["n_CompanyId"].ToString());
                    string X_Pattern = "10";
                    int N_TypeID = 0;
                    int N_MainLocationID = 0;
                    int N_LocationID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_LocationID"].ToString());
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
                        Params.Add("N_YearID", MasterTable.Rows[0]["n_FnYearId"].ToString());
                        Params.Add("N_FormID", 450);
                        // Params.Add("N_BranchID", MasterTable.Rows[0]["n_BranchId"].ToString());
                        LocationCode = dLayer.GetAutoNumber("Inv_Location", "X_LocationCode", Params, connection, transaction);
                        if (LocationCode == "") { transaction.Rollback(); return Ok(_api.Error(User, "Unable to generate Location Code")); }
                        MasterTable.Rows[0]["X_LocationCode"] = LocationCode;
                    }

                    if (N_LocationID == 0)
                    {
                        if (X_Pattern == "10" && N_MainLocationID == 0)
                        {
                            MasterTable.Rows[0]["X_Pattern"] = "10";
                        }
                        else
                        {
                            //  object xHierarchyID = dLayer.ExecuteScalar("Select N_LocationID from Inv_Location where N_CompanyID=" + myFunctions.GetCompanyID(User)  + " and N_MainLocationID=" + N_MainLocationID +  " ", connection, transaction);
                            //  object xPattern = dLayer.ExecuteScalar("Select X_Pattern  From Inv_Location Where N_CompanyID=" + myFunctions.GetCompanyID(User)  + " and N_LocationID=" + myFunctions.getIntVAL(xHierarchyID.ToString()) + " ", connection, transaction);
                            //    patternNo = xPattern.ToString();
                            //    MasterTable.Rows[0]["X_Pattern"] =patternNo + "10"; 
                            object xHierarchyID = dLayer.ExecuteScalar("Select max(N_LocationID) From Inv_Location Where N_CompanyID=" + myFunctions.GetCompanyID(User) + " and N_MainLocationID=" + N_MainLocationID + " ", connection, transaction);
                            if (myFunctions.getIntVAL(xHierarchyID.ToString()) > 0)
                            {
                                object xPattern = dLayer.ExecuteScalar("Select X_Pattern  From Inv_Location Where N_CompanyID=" + myFunctions.GetCompanyID(User) + " and N_LocationID=" + myFunctions.getIntVAL(xHierarchyID.ToString()) + " ", connection, transaction);
                                if (xPattern != null)
                                {
                                    patternNo = xPattern.ToString();
                                    int length = X_Pattern.Length;
                                    string removingPattern = patternNo.Substring(length);
                                    //string pattern = myFunctions.getIntVAL(removingPattern);
                                    string pattern = removingPattern;
                                    pattern = pattern + X_Pattern;
                                    patternNo = pattern;
                                    //patternNo = pattern.ToString();
                                    if (removingPattern.Length > (pattern.ToString().Length))
                                    {
                                        patternNo = X_Pattern + "0" + patternNo;
                                    }
                                    else
                                    {
                                        patternNo = X_Pattern + patternNo;

                                    }

                                }
                                MasterTable.Rows[0]["X_Pattern"] = patternNo;

                            }
                            else
                            {

                                MasterTable.Rows[0]["X_Pattern"] = X_Pattern + "10";


                            }




                        }
                        if (N_TypeID == 1)
                        {

                            object roomPattern = dLayer.ExecuteScalar("Select isnull(max(X_LocationCode),'')  From Inv_Location Where N_CompanyID=" + nCompanyID + " and N_TypeID=1 ", connection, transaction);

                            if (roomPattern == null || roomPattern.ToString() == "")
                            {
                                x_LocationCodePattern = 'A';
                                MasterTable.Rows[0]["X_LocationCode"] = x_LocationCodePattern.ToString();

                            }
                            else
                            {
                                x_LocationCodePattern = Convert.ToChar(roomPattern);
                                x_LocationCodePattern++;
                                MasterTable.Rows[0]["X_LocationCode"] = x_LocationCodePattern.ToString();


                            }

                        }
                        else if (N_TypeID != 0)
                        {
                            object rowPattern = dLayer.ExecuteScalar("Select isnull(max(X_LocationCode),'')  From Inv_Location Where N_CompanyID=" + nCompanyID + " and N_TypeID=" + N_TypeID + " and N_MainLocationID=" + N_MainLocationID + " ", connection, transaction);
                            object parentRomPattern = dLayer.ExecuteScalar("select TOP 1  X_LocationCode from Inv_Location where N_LocationID=" + N_MainLocationID + " and N_CompanyID=" + nCompanyID + " ", connection, transaction);
                            if (rowPattern == null || rowPattern.ToString() == "")
                            {
                                MasterTable.Rows[0]["X_LocationCode"] = parentRomPattern.ToString() + "-" + "01";

                            }
                            else
                            {
                                int hiphenLength = rowPattern.ToString().LastIndexOf("-");
                                string addingCode = rowPattern.ToString().Substring((hiphenLength), (rowPattern.ToString().Length) - hiphenLength);
                                addingCode = addingCode.Remove(0, 1);
                                if (myFunctions.getIntVAL(addingCode) <= 9) { addingCode = "0" + (myFunctions.getIntVAL(addingCode) + 1).ToString(); }
                                else { addingCode = (myFunctions.getIntVAL(addingCode) + 1).ToString(); }
                                MasterTable.Rows[0]["X_LocationCode"] = parentRomPattern.ToString() + "-" + addingCode;


                            }



                        }














                    }
                    else if (N_LocationID > 0)
                    {
                        dLayer.DeleteData("Inv_Location", "N_LocationID", N_LocationID, "", connection, transaction);
                    }

                    if (MasterTable.Columns.Contains("B_IsDefault"))
                    {
                        MasterTable.Rows[0]["b_DefaultBranch"] = true;
                      
                    }else
                    {
                        MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable,"B_IsDefault",typeof(bool),true);
                    }

                    MasterTable.Columns.Remove("n_FnYearId");
                    MasterTable.Columns.Remove("b_isSubLocation");
                    N_LocationID = dLayer.SaveData("Inv_Location", "N_LocationID", MasterTable, connection, transaction);
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
                        transaction.Commit();
                        return GetLocationDetails(int.Parse(MasterTable.Rows[0]["n_CompanyId"].ToString()), N_LocationID);
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nLocationId)
        {
            int Results = 0;
            SortedList Params = new SortedList();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Params.Add("@nLocationId", nLocationId);
                    object count = dLayer.ExecuteScalar("select count(*) as N_Count from vw_Inv_Location_Disp where N_LocationID=@nLocationId and N_CompanyID=N_CompanyID", Params, connection);
                    int N_Count = myFunctions.getIntVAL(count.ToString());
                    if (N_Count <= 0)
                    {
                        Results = dLayer.DeleteData("Inv_Location", "N_LocationID", nLocationId, "", connection);
                    }
                }
                if (Results > 0)
                {
                    return Ok(_api.Success("Location deleted"));
                }
                else
                {
                    return Ok(_api.Error(User, "Unable to delete Location"));
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