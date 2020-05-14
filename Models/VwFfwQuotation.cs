using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwFfwQuotation
    {
        [Column("N_CompanyId")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("N_QuotationId")]
        public int NQuotationId { get; set; }
        [Column("X_QuotationNo")]
        [StringLength(50)]
        public string XQuotationNo { get; set; }
        [Required]
        [Column("X_TYPE")]
        [StringLength(9)]
        public string XType { get; set; }
        [Column("D_QuotationDate", TypeName = "smalldatetime")]
        public DateTime? DQuotationDate { get; set; }
        [Column("D_SalesDate")]
        [StringLength(8000)]
        public string DSalesDate { get; set; }
        [Column("N_CustomerID")]
        public int? NCustomerId { get; set; }
        [Column("X_CustomerAddress")]
        [StringLength(250)]
        public string XCustomerAddress { get; set; }
        [Column("X_CustomerCode")]
        [StringLength(50)]
        public string XCustomerCode { get; set; }
        [Column("X_CustomerPhoneNo")]
        [StringLength(20)]
        public string XCustomerPhoneNo { get; set; }
        [Column("N_InvDueDays")]
        public int? NInvDueDays { get; set; }
        [Column("N_CurrencyID")]
        public int? NCurrencyId { get; set; }
        [Column("X_CurrencyCode")]
        [StringLength(50)]
        public string XCurrencyCode { get; set; }
        [Column("N_ExchangeRate", TypeName = "money")]
        public decimal? NExchangeRate { get; set; }
        [Column("X_ShortName")]
        [StringLength(50)]
        public string XShortName { get; set; }
        [Column("N_FlightID")]
        public int? NFlightId { get; set; }
        [Column("X_FlightCode")]
        [StringLength(50)]
        public string XFlightCode { get; set; }
        [Column("X_FlightName")]
        [StringLength(100)]
        public string XFlightName { get; set; }
        [Column("X_FlightNumber")]
        public int? XFlightNumber { get; set; }
        [Column("X_PONO")]
        [StringLength(100)]
        public string XPono { get; set; }
        [Column("X_Comm_InvoiceNo")]
        [StringLength(100)]
        public string XCommInvoiceNo { get; set; }
        [Column("X_FileRefNo")]
        [StringLength(50)]
        public string XFileRefNo { get; set; }
        [Column("X_AwbNo")]
        [StringLength(50)]
        public string XAwbNo { get; set; }
        [Column("X_Marks")]
        [StringLength(50)]
        public string XMarks { get; set; }
        [Column("N_Qty")]
        public int? NQty { get; set; }
        [Column("X_Packages")]
        [StringLength(50)]
        public string XPackages { get; set; }
        [Column("X_GoodsDescription")]
        [StringLength(500)]
        public string XGoodsDescription { get; set; }
        [Column("X_GrossWt")]
        [StringLength(50)]
        public string XGrossWt { get; set; }
        [Column("X_ChgWt")]
        [StringLength(50)]
        public string XChgWt { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_InvoiceModeID")]
        public int? NInvoiceModeId { get; set; }
        [Column("X_InvoiceMode")]
        [StringLength(50)]
        public string XInvoiceMode { get; set; }
        [Column("N_InsuranceID")]
        public int? NInsuranceId { get; set; }
        [Column("X_InsuranceName")]
        [StringLength(50)]
        public string XInsuranceName { get; set; }
        [Column("N_SrviceLevelID")]
        public int? NSrviceLevelId { get; set; }
        [Column("X_Srvice")]
        [StringLength(100)]
        public string XSrvice { get; set; }
        [Column("N_FrightPaymentID")]
        public int? NFrightPaymentId { get; set; }
        [Column("X_FrightPayment")]
        [StringLength(50)]
        public string XFrightPayment { get; set; }
        [Column("N_IncotermsID")]
        public int? NIncotermsId { get; set; }
        [Column("X_Incoterms")]
        [StringLength(50)]
        public string XIncoterms { get; set; }
        [Column("N_AirportArrival_id")]
        public int? NAirportArrivalId { get; set; }
        [Column("X_AirportArrival_Code")]
        [StringLength(50)]
        public string XAirportArrivalCode { get; set; }
        [Column("N_DeliveryTermID")]
        public int? NDeliveryTermId { get; set; }
        [Column("X_DeliveryTerm")]
        [StringLength(50)]
        public string XDeliveryTerm { get; set; }
        [Column("X_ArrivalAirportName")]
        [StringLength(100)]
        public string XArrivalAirportName { get; set; }
        [Column("X_DepartureAirport")]
        public int? XDepartureAirport { get; set; }
        [Column("X_DepartureAirportCode")]
        [StringLength(50)]
        public string XDepartureAirportCode { get; set; }
        [Column("X_AirportDepartureName")]
        [StringLength(100)]
        public string XAirportDepartureName { get; set; }
        [Column("N_AirportDep_id")]
        public int? NAirportDepId { get; set; }
        [Column("D_FlightDate", TypeName = "smalldatetime")]
        public DateTime? DFlightDate { get; set; }
        [Column("B_IsExport")]
        public bool? BIsExport { get; set; }
        [Column("N_BillAmount", TypeName = "money")]
        public decimal? NBillAmount { get; set; }
        [Column("X_UserID")]
        [StringLength(50)]
        public string XUserId { get; set; }
        [Column("B_IsSaveDraft")]
        public bool? BIsSaveDraft { get; set; }
        [Required]
        [Column("type")]
        [StringLength(5)]
        public string Type { get; set; }
        [Column("N_TaxAmount", TypeName = "money")]
        public decimal? NTaxAmount { get; set; }
        [Column("N_ModeOfTransaction")]
        public int? NModeOfTransaction { get; set; }
        [Column("X_ModeOfTransaction")]
        [StringLength(50)]
        public string XModeOfTransaction { get; set; }
        [Column("D_EntryDate", TypeName = "smalldatetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("N_Processed")]
        public bool? NProcessed { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Column("X_Notes")]
        [StringLength(1000)]
        public string XNotes { get; set; }
        [Column("N_ServiceType")]
        public int? NServiceType { get; set; }
        [Column("X_DeliveryTerms")]
        [StringLength(100)]
        public string XDeliveryTerms { get; set; }
        [Column("X_Insurance")]
        [StringLength(50)]
        public string XInsurance { get; set; }
        [Column("B_IsEnquiry")]
        public bool BIsEnquiry { get; set; }
    }
}
