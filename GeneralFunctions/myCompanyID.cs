using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
namespace SmartxAPI
{
    static class myCompanyID
    {

        #region Account Defaults

        // public static string _FnYearName;
        // public static int _FnYearID = 1;

        public static int _TaxTypeID = 0;


        public static string _Acc_Def_Field = "Account Code";
        public static string _Acc_Ledger_ViewName = "vw_AccMastLedger";
        public static string _Acc_Group_ViewName = "vw_AccMastGroup";
        public static System.Globalization.CultureInfo _EnglishCulture = new System.Globalization.CultureInfo("en-US");
        #endregion

        #region Period

        

        public static int _PeriodID = 1;

        public static int _AcPeriodID = 1;


        #endregion

        #region Sales Defaults

        public static string _Sales_Def_Field = "Item Code";
        public static bool _Sales_OnlinePosting = true;
        public static bool _SalesReceipt_OnlinePosting = true;
        public static bool _SalesAutoInvoice = false;
        public static bool _SalesOrderAutoInvoice = false;
        public static bool _SalesQuotationAutoInvoice = false;
        public static bool _SalesReceiptAutoInvoice = false;
        #endregion

        #region Purchase Defaults

        public static string _Purchase_Def_Field = "Item Code";
        public static bool _Purchase_OnlinePosting = true;
        public static bool _Inventory_AccountPosting = true;
        public static bool _PurchaseAutoInvoice = false;
        public static bool _PurchaseOrderAutoInvoice = false;
        public static bool _PurchasePaymentAutoInvoice = false;
        #endregion

        #region Iem Begining Balance

        public static bool _Item_Begining_OnlinePosting = true;

        #endregion

        #region Company Defaults

        public static int _CompanyID = -10;
         public static string _CurrencyName="";

        #endregion

        #region School Defaults

        // public static string _AcYearName;
        public static int _AcYearID = 1;
        // public static DateTime _AcYearStartDate;
        // public static DateTime _AcYearEndDate;

        #endregion

        #region Login Defaults

        public static string _SystemDateFormat = "dd-MMM-yyyy";

        public static string _SystemDate = Convert.ToDateTime(DateTime.Now).ToShortDateString();
        public static string _FnYearStartDate = null;
        public static DateTime _MaxTransDate = DateTime.Now.AddDays(10);

        public static string _SystemName = "";
        // public static int _UserLoginId;
        public static bool _AutoLogoff=false;
        #endregion

        #region Quick Access Defaults
            // public static string _ActiveForm;
            public static object[] forms = new object[100];
            public static int N_OpenedFormCount = 0;
        #endregion

        #region Connection Defaults

        public static string _ConnectionString = "SmartxConnection";
        public static string _ServerPath = "";
        public static string _DocumtPath = "";
        public static string _ReportFolder = "olivoreports";
        public static string _TempltFolder = "olivotemplates";
        public static string _DBBkupFolder = "olivobackup";
        // public static string _DSN;

        #endregion

        public static int _UserLevel = 0;
        public static bool _OnlinePosting = true;



        


        public static int DecimalPlaces = 2;
        public static string DecimalPlaceString = "#,##0.00";
        public static string ThousandSeparatorString = "#,##0";

        public static string DecimalPlace3 = "#,##0.000";

        public static string _FontName = "Verdana";
        public static string prevVal = "";
        public static string newVal = "";

       public static string _APIVersion = "21.10.9.1";


    }
} 
