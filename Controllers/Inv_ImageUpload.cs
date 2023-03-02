using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SmartxAPI.GeneralFunctions;
using System;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace SmartxAPI.Controllers

{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("imageUpload")]
    [ApiController]
    public class Inv_ImageUpload : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;


        public Inv_ImageUpload(IApiFunctions apiFun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apiFun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }



        [HttpGet("list")]
        public ActionResult GetImageList()
        {
            int nCompanyId= myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText = "select Code,[Unit Code],Description from vw_InvItemUnit_Disp where N_CompanyID=@p1 and N_ItemID is null order by ItemCode,[Unit Code]";
            Params.Add("@p1", nCompanyId);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                dt = api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(api.Warning("No Results Found"));
                }
                else
                {
                    return Ok(api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }

        [HttpGet("listdetails")]
        public ActionResult GetImageDetails(int? nCompanyId,int? nItemUnitID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText = "select * from Inv_ItemUnit where N_CompanyID=@p1 and N_ItemUnitID=@p2 ";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nItemUnitID);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                dt = api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(api.Warning("No Results Found"));
                }
                else
                {
                    return Ok(api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }

        //Save....
        [HttpPost("Save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                MasterTable = ds.Tables["master"];

                SortedList Params = new SortedList();
                
            
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                     string image = "";
                  
                     string itemtype1 = "";
                       if (MasterTable.Rows.Count > 0)
                        {
                            MasterTable.Columns.Add("X_ImageName", typeof(System.String));
                            MasterTable.Columns.Add("X_ImageLocation", typeof(System.String));
                            MasterTable.Columns.Add("N_ImageID", typeof(System.Int32));
                            MasterTable.Columns.Add("N_ItemID", typeof(System.Int32));
                            int i = 0;
                            int type=myFunctions.getIntVAL(MasterTable.Rows[0]["n_Type"].ToString());
                          
                            foreach (DataRow dRow in MasterTable.Rows)
                            {
                                object xItemCode = MasterTable.Rows[i]["ItemCode"].ToString();
                                object N_ItemID = dLayer.ExecuteScalar("Select N_ItemID from Inv_ItemMaster where X_ItemCode='" + xItemCode + "' and N_CompanyID="+myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString())+"", Params, connection,transaction);
                                int NItemID=0;
                                
                                if(N_ItemID!=null)
                                {
                                
                                 NItemID=myFunctions.getIntVAL(N_ItemID.ToString());
                                }
                                // else 
                                // {
                                //   int NItemID=0;  
                                // }
                               
                            if(type==1)
                           {
                              myFunctions.writeImageFile(dRow["I_Image"].ToString(), myFunctions.GetUploadsPath(User, "PosProductImages"), xItemCode + "-POS-" + i);
                              dRow["X_ImageName"] =xItemCode + "-POS-" + i + ".jpg";
                              dRow["X_ImageLocation"] = myFunctions.GetUploadsPath(User, "PosProductImages");
                           }
                           else   if(type==2)
                           {
                             myFunctions.writeImageFile(dRow["I_Image"].ToString(), myFunctions.GetUploadsPath(User, "EcomProductImages"), xItemCode + "-ECOM-" + i);
                             dRow["X_ImageName"] =xItemCode + "-ECOM-" + i + ".jpg";
                             dRow["X_ImageLocation"] = myFunctions.GetUploadsPath(User, "EcomProductImages");
                           }
                               
                               
                                dRow["N_ImageID"] = MasterTable.Rows[i]["N_ImageID"] ;
                                dRow["N_ItemID"] = NItemID ;
                                i++;

                            }
                           
                            MasterTable.Columns.Remove("I_Image");
                            MasterTable.Columns.Remove("ItemCode");
                            MasterTable.Columns.Remove("fileName");
                            int N_ImageID=dLayer.SaveData("Inv_DisplayImages", "N_ImageID", MasterTable, connection, transaction);
                            if (N_ImageID <= 0)
                    {
                        transaction.Rollback();
                        return Ok( api.Warning("no result fount"));
                    }
                    else
                    {
                        transaction.Commit();
                    }
                        }
                    // if (MasterTable.Columns.Contains("i_Image"))
                    //     {

                    //         image = MasterTable.Rows[0]["i_Image"].ToString();

                    //         MasterTable.Rows[0]["i_Image"] = "";

                    //     }
                    //     Byte[] imageBitmap = new Byte[image.Length];
                    //     imageBitmap = Convert.FromBase64String(image);
                    //     MasterTable.Columns.Remove("i_Image");
                    //     int N_ImageID = dLayer.SaveData("Inv_DisplayImages","N_ImageID", MasterTable, connection, transaction);
                    //    if (image.Length > 0)
                    //    dLayer.SaveImage("Inv_DisplayImages","X_ImageName",imageBitmap,"N_ImageID",N_ImageID,connection, transaction);
                 
                    // if (N_ImageID <= 0)
                    // {
                    //     transaction.Rollback();
                    //     return Ok( api.Warning("no result fount"));
                    // }
                    // else
                    // {
                    //     transaction.Commit();
                    // }
                    return Ok( api.Success("Upload sucessfully"));
                }
                

            }

            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }
        }


     

        
    }
}