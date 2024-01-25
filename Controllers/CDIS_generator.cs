using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.ComponentModel;
using System.Collections.Generic;
using System.Net;
using System.IO;
using ZatcaIntegrationSDK;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("zatcaCSIDgenerator")]
    [ApiController]
    public class CSID_generator : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID = 1863;
        private readonly IApiFunctions api;
        private ZatcaIntegrationSDK.APIHelper.Mode mode { get; set; }
        public CSID_generator(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        [HttpGet("Details")]
        public ActionResult Details()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            Params.Add("@p1", nCompanyId);
            DataTable MasterTable = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SortedList QueryParamsList = new SortedList();
                    string Mastersql = "";
                    Mastersql = "select * from Acc_Company where N_CompanyId=" + nCompanyId + " ";
                    dt = dLayer.ExecuteDataTable(Mastersql, QueryParamsList, connection);
                    MasterTable = _api.Format(MasterTable, "Master");
                    if(dt.Rows.Count==0)
                {
                    return Ok(_api.Notice("No Results Found" ));
                }
                else
                {
                    return Ok(_api.Success(dt));
                }
                }

            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }
        }

        [HttpGet("GetAutoNo")]
        public ActionResult RefreshSerialNumber()
        {
            DataTable dt = new DataTable();
            string txt_serialNumber = "1-TST|2-TST|3-" + Guid.NewGuid().ToString();
            string input = txt_serialNumber.Trim();
            int index = input.LastIndexOf("|3-");

            if (index >= 0)
            {
                input = input.Substring(0, index + 3);
                string refreshedSerialNumber = input + Guid.NewGuid().ToString();
                dt.Columns.Add("SerialNo");
                DataRow row = dt.NewRow();
                row["SerialNo"] = (refreshedSerialNumber);
                dt.Rows.Add(row);
                dt = _api.Format(dt, "Master");
                return Ok(_api.Success(dt));
            }
            return BadRequest("Invalid input for serial number");
        }
        [HttpPost("Save")]
        public ActionResult SaveData([FromBody] DataSet ds)

        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    DataTable MasterTable;
                    DataTable dt = new DataTable();
                    DataRow row = dt.NewRow();
                    SortedList Params = new SortedList();
                    MasterTable = ds.Tables["master"];
                    DataRow MasterRow = MasterTable.Rows[0];
                    int N_CompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
                    int N_fnYearId = myFunctions.getIntVAL(MasterRow["n_FnYearId"].ToString());
                    Params.Add("@p1", N_CompanyID);
                    Params.Add("@p2", N_fnYearId);
                    string x_ZatcaName = MasterRow["X_ZatcaName"].ToString();
                    string X_ZatcaMode = MasterRow["x_ZatcaMode"].ToString();
                    string X_ZatcaSerial = MasterRow["X_ZatcaSerial"].ToString();
                    string X_ZatcaInvoicetype = MasterRow["X_ZatcaInvoicetype"].ToString();
                    string N_ZatcaOTP = MasterRow["N_ZatcaOTP"].ToString();
                    string x_CompanyName = MasterRow["x_CompanyName"].ToString();
                    string x_TaxRegistrationNo = MasterRow["x_TaxRegistrationNo"].ToString();
                    Params.Add("@p3", X_ZatcaMode);
                    Params.Add("@p4", x_ZatcaName);
                    Params.Add("@p5", X_ZatcaSerial);
                    Params.Add("@p6", X_ZatcaInvoicetype);
                    Params.Add("@p7", N_ZatcaOTP);
                    dt.Columns.Add("SecretKey");
                    dt.Columns.Add("CSID");
                    dt.Columns.Add("PrivateKey");
                    dt.Columns.Add("CSR");
                    Invoice inv = new Invoice();
                    inv.ID = "INV00001"; // مثال SME00010
                    inv.IssueDate = DateTime.Now.ToString("yyyy-MM-dd");
                    inv.IssueTime = DateTime.Now.ToString("HH:mm:ss"); // "09:32:40"
                    inv.DocumentCurrencyCode = "SAR";
                    inv.TaxCurrencyCode = "SAR";
                    if (inv.invoiceTypeCode.id == 383 || inv.invoiceTypeCode.id == 381)
                    {
                        // فى حالة ان اشعار دائن او مدين فقط هانكتب رقم الفاتورة اللى اصدرنا الاشعار ليها
                        InvoiceDocumentReference invoiceDocumentReference = new InvoiceDocumentReference();
                        invoiceDocumentReference.ID = "Invoice Number: 354; Invoice Issue Date: 2021-02-10"; // اجبارى
                        inv.billingReference.invoiceDocumentReferences.Add(invoiceDocumentReference);
                    }
                    inv.AdditionalDocumentReferencePIH.EmbeddedDocumentBinaryObject = "NWZlY2ViNjZmZmM4NmYzOGQ5NTI3ODZjNmQ2OTZjNzljMmRiYzIzOWRkNGU5MWI0NjcyOWQ3M2EyN2ZiNTdlOQ==";
                    inv.AdditionalDocumentReferenceICV.UUID = 123;
                    PaymentMeans paymentMeans = new PaymentMeans();
                    paymentMeans.PaymentMeansCode = "10";
                    paymentMeans.InstructionNote = "Payment Notes";
                    inv.paymentmeans.Add(paymentMeans);
                    // بيانات البائع
                    inv.SupplierParty.partyIdentification.ID = "2050012095"; //هنا رقم السجل التجارى للشركة
                    inv.SupplierParty.partyIdentification.schemeID = "CRN";
                    inv.SupplierParty.postalAddress.StreetName = "شارع تجربة"; // اجبارى
                    inv.SupplierParty.postalAddress.AdditionalStreetName = "شارع اضافى"; // اختيارى
                    inv.SupplierParty.postalAddress.BuildingNumber = "1234"; // اجبارى رقم المبنى
                    inv.SupplierParty.postalAddress.PlotIdentification = "9833";
                    inv.SupplierParty.postalAddress.CityName = "taif";
                    inv.SupplierParty.postalAddress.PostalZone = "12345"; // الرقم البريدي
                    inv.SupplierParty.postalAddress.CountrySubentity = "المحافظة"; // اسم المحافظة او المدينة مثال (مكة) اختيارى
                    inv.SupplierParty.postalAddress.CitySubdivisionName = "اسم المنطقة"; // اسم المنطقة او الحى
                    inv.SupplierParty.postalAddress.country.IdentificationCode = "SA";
                    inv.SupplierParty.partyLegalEntity.RegistrationName = x_CompanyName; // "شركة الصناعات الغذائية المتحده"; // اسم الشركة المسجل فى الهيئة
                    inv.SupplierParty.partyTaxScheme.CompanyID = x_TaxRegistrationNo;// "300518376300003";  // رقم التسجيل الضريبي
                    inv.CustomerParty.partyIdentification.ID = "1234567"; // رقم القومى الخاض بالمشترى
                    inv.CustomerParty.partyIdentification.schemeID = "CRN"; // الرقم القومى
                    inv.CustomerParty.postalAddress.StreetName = "شارع تجربة"; // اجبارى
                    inv.CustomerParty.postalAddress.AdditionalStreetName = "شارع اضافى"; // اختيارى
                    inv.CustomerParty.postalAddress.BuildingNumber = "1234"; // اجبارى رقم المبنى
                    inv.CustomerParty.postalAddress.PlotIdentification = "9833"; // اختيارى رقم القطعة
                    inv.CustomerParty.postalAddress.CityName = "Jeddah"; // اسم المدينة
                    inv.CustomerParty.postalAddress.PostalZone = "12345"; // الرقم البريدي
                    inv.CustomerParty.postalAddress.CountrySubentity = "Makkah"; // اسم المحافظة او المدينة مثال (مكة) اختيارى
                    inv.CustomerParty.postalAddress.CitySubdivisionName = "المحافظة"; // اسم المنطقة او الحى
                    inv.CustomerParty.postalAddress.country.IdentificationCode = "SA";
                    inv.CustomerParty.partyLegalEntity.RegistrationName = "اسم شركة المشترى"; // اسم الشركة المسجل فى الهيئة
                    inv.CustomerParty.partyTaxScheme.CompanyID = "310424415000003"; // رقم التسجيل الضريبي
                    inv.legalMonetaryTotal.PrepaidAmount = 0;
                    inv.legalMonetaryTotal.PayableAmount = 0;
                    InvoiceLine invline = new InvoiceLine();
                    invline.InvoiceQuantity = 1;
                    invline.item.Name = "منتج تجربة";
                    invline.item.classifiedTaxCategory.ID = "S"; // كود الضريبة
                    invline.taxTotal.TaxSubtotal.taxCategory.ID = "S"; // كود الضريبة
                    invline.item.classifiedTaxCategory.Percent = 15; // نسبة الضريبة
                    invline.taxTotal.TaxSubtotal.taxCategory.Percent = 15; // نسبة الضريبة
                    invline.price.PriceAmount = 120;
                    inv.InvoiceLines.Add(invline);
                    ZatcaIntegrationSDK.HelperContracts.CertificateRequest certrequest = GetCSRRequest(ds);
                    if (X_ZatcaMode == "Simulation")
                        mode = ZatcaIntegrationSDK.APIHelper.Mode.Simulation;
                    else if (X_ZatcaMode == "Production")
                        mode = ZatcaIntegrationSDK.APIHelper.Mode.Production;
                    else
                        mode = ZatcaIntegrationSDK.APIHelper.Mode.developer;
                    CSIDGenerator generator = new CSIDGenerator(mode);
                    ZatcaIntegrationSDK.HelperContracts.CertificateResponse response = generator.GenerateCSID(certrequest, inv, Directory.GetCurrentDirectory());
                    object result = "";
                    if (response.IsSuccess)
                    {
                        // get all certificate data
                        row["CSR"] = response.CSR;
                        row["PrivateKey"] = response.PrivateKey;
                        row["CSID"] = response.CSID;
                        row["SecretKey"] = response.SecretKey;
                        dt.Rows.Add(row);
                        Params.Add("@p8", response.CSR);
                        Params.Add("@p9", response.PrivateKey);
                        Params.Add("@p10", response.CSID);
                        Params.Add("@p11", response.SecretKey);

                        dt = _api.Format(dt, "Master");
                        result = dLayer.ExecuteNonQuery("update Acc_Company set X_ZatcaMode=@p3, X_ZatcaName=@p4, X_ZatcaInvoicetype=@p6, X_ZatcaSerial=@p5 , N_ZatcaOTP=@p7, X_ZatcaCSR=@p8, X_ZatcaPrivatekey=@p9, X_ZatcaPublickey=@p10, X_ZatcaSecret=@p11 where n_CompanyID=@p1  ", Params, connection, transaction);

                    }
                    if (myFunctions.getIntVAL(result.ToString()) > 0)
                    {
                        transaction.Commit();
                    }
                    if(response.IsSuccess){
                    return Ok(_api.Success(dt, "Saved"));
                    }
                    else {
                        return Ok(_api.Error("CSID Generation Failed!!"));
                    }

                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }
        public ZatcaIntegrationSDK.HelperContracts.CertificateRequest GetCSRRequest(DataSet ds)
        {
            DataTable MasterTable;
            SortedList Params = new SortedList();
            MasterTable = ds.Tables["master"];
            DataRow MasterRow = MasterTable.Rows[0];
            string x_ZatcaName = MasterRow["X_ZatcaName"].ToString();
            string X_ZatcaMode = MasterRow["x_ZatcaMode"].ToString();
            string X_ZatcaSerial = MasterRow["X_ZatcaSerial"].ToString();
            string X_ZatcaInvoicetype = MasterRow["X_ZatcaInvoicetype"].ToString();
            string N_ZatcaOTP = MasterRow["N_ZatcaOTP"].ToString();
            string x_CompanyName = MasterRow["x_CompanyName"].ToString();
            string x_TaxRegistrationNo = MasterRow["x_TaxRegistrationNo"].ToString();
            string x_ZatcaUnitName = MasterRow["x_ZatcaUnitName"].ToString();
            string x_Country = "SA";
            // string x_location = MasterRow["x_location"].ToString();
            string x_Industry = MasterRow["x_Industry"].ToString();


            ZatcaIntegrationSDK.HelperContracts.CertificateRequest certrequest = new ZatcaIntegrationSDK.HelperContracts.CertificateRequest();
            certrequest.OTP = N_ZatcaOTP;
            certrequest.CommonName = x_ZatcaName;
            certrequest.OrganizationName = x_CompanyName;
            certrequest.OrganizationUnitName = x_ZatcaUnitName;
            certrequest.CountryName = x_Country;
            certrequest.SerialNumber = X_ZatcaSerial;
            certrequest.OrganizationIdentifier = x_TaxRegistrationNo;
            certrequest.Location = x_Country;
            certrequest.BusinessCategory = x_Industry;
            certrequest.InvoiceType = "1000";
            return certrequest;
        }

    }
}