using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwFfwInvoice
    {
        [Column("N_CompanyId")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("N_InvoiceId")]
        public int NInvoiceId { get; set; }
        [Column("X_InvoiceNo")]
        [StringLength(50)]
        public string XInvoiceNo { get; set; }
        [Required]
        [Column("X_TYPE")]
        [StringLength(20)]
        public string XType { get; set; }
        [Column("D_InvoiceDate", TypeName = "datetime")]
        public DateTime? DInvoiceDate { get; set; }
        [Column("D_SalesDate")]
        [StringLength(8000)]
        public string DSalesDate { get; set; }
        [Column("N_CustomerID")]
        public int NCustomerId { get; set; }
        [Column("X_CustomerAddress")]
        [StringLength(500)]
        public string XCustomerAddress { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Column("X_CustomerCode")]
        [StringLength(50)]
        public string XCustomerCode { get; set; }
        [Column("B_DirPosting")]
        public bool? BDirPosting { get; set; }
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
        [Column("N_ShipperID")]
        public int? NShipperId { get; set; }
        [Column("X_ShipperCode")]
        [StringLength(50)]
        public string XShipperCode { get; set; }
        [Column("X_ShipperName")]
        [StringLength(100)]
        public string XShipperName { get; set; }
        [Column("X_Shipper_Address")]
        [StringLength(250)]
        public string XShipperAddress { get; set; }
        [Column("X_ShipperPhoneNo")]
        [StringLength(20)]
        public string XShipperPhoneNo { get; set; }
        [Column("N_ConsingeeID")]
        public int? NConsingeeId { get; set; }
        [Column("X_ConsingeeCode")]
        [StringLength(50)]
        public string XConsingeeCode { get; set; }
        [Column("X_ConsingeeName")]
        [StringLength(100)]
        public string XConsingeeName { get; set; }
        [Column("X_Consingee_Address")]
        [StringLength(250)]
        public string XConsingeeAddress { get; set; }
        [Column("X_ConsingeePhoneNo")]
        [StringLength(20)]
        public string XConsingeePhoneNo { get; set; }
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
        [StringLength(50)]
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
        [StringLength(100)]
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
        [Column("D_FlightDate", TypeName = "datetime")]
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
        [Column("N_QuotationId")]
        public int? NQuotationId { get; set; }
        [Column("X_Notes")]
        [StringLength(1000)]
        public string XNotes { get; set; }
        [Column("N_ServiceType")]
        public int? NServiceType { get; set; }
        [Column("X_FreightPayment")]
        [StringLength(100)]
        public string XFreightPayment { get; set; }
        [Column("X_DeliveryTerms")]
        [StringLength(100)]
        public string XDeliveryTerms { get; set; }
        [Column("X_Insurance")]
        [StringLength(50)]
        public string XInsurance { get; set; }
        [Column("N_vehicleID")]
        public int? NVehicleId { get; set; }
        [Column("B_IsOrder")]
        public bool? BIsOrder { get; set; }
        [Column("X_TruckCode")]
        [StringLength(100)]
        public string XTruckCode { get; set; }
        [Column("X_PlateNumber")]
        [StringLength(100)]
        public string XPlateNumber { get; set; }
        [Column("X_Driver")]
        [StringLength(50)]
        public string XDriver { get; set; }
        [Column("X_Description")]
        [StringLength(100)]
        public string XDescription { get; set; }
        [Column("X_PhoneNo")]
        [StringLength(20)]
        public string XPhoneNo { get; set; }
        [Column("N_ContainedQty")]
        public double NContainedQty { get; set; }
        [Required]
        [Column("X_ContainedDescription")]
        [StringLength(100)]
        public string XContainedDescription { get; set; }
        [Column("N_StatusID")]
        public int NStatusId { get; set; }
        [Required]
        [Column("X_Status")]
        [StringLength(50)]
        public string XStatus { get; set; }
        [Column("B_Prepayment")]
        public int BPrepayment { get; set; }
    }
}
