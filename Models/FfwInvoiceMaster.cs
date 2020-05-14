using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Ffw_InvoiceMaster")]
    public partial class FfwInvoiceMaster
    {
        [Column("N_CompanyId")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Key]
        [Column("N_InvoiceId")]
        public int NInvoiceId { get; set; }
        [Column("X_InvoiceNo")]
        [StringLength(50)]
        public string XInvoiceNo { get; set; }
        [Column("D_InvoiceDate", TypeName = "smalldatetime")]
        public DateTime? DInvoiceDate { get; set; }
        [Column("D_EntryDate", TypeName = "smalldatetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("N_CustomerID")]
        public int? NCustomerId { get; set; }
        [Column("N_InvoiceModeID")]
        public int? NInvoiceModeId { get; set; }
        [Column("N_InsuranceID")]
        public int? NInsuranceId { get; set; }
        [Column("X_PONO")]
        [StringLength(100)]
        public string XPono { get; set; }
        [Column("X_Comm_InvoiceNo")]
        [StringLength(100)]
        public string XCommInvoiceNo { get; set; }
        [Column("N_SrviceLevelID")]
        public int? NSrviceLevelId { get; set; }
        [Column("N_DeliveryTermID")]
        public int? NDeliveryTermId { get; set; }
        [Column("X_FileRefNo")]
        [StringLength(50)]
        public string XFileRefNo { get; set; }
        [Column("N_ShipperID")]
        public int? NShipperId { get; set; }
        [Column("N_ConsingerID")]
        public int? NConsingerId { get; set; }
        [Column("X_AwbNo")]
        [StringLength(50)]
        public string XAwbNo { get; set; }
        [Column("D_FlightDate", TypeName = "smalldatetime")]
        public DateTime? DFlightDate { get; set; }
        [Column("N_FlightNo")]
        public int? NFlightNo { get; set; }
        [Column("N_DepartureID")]
        public int? NDepartureId { get; set; }
        [Column("N_ArrivalID")]
        public int? NArrivalId { get; set; }
        [Column("N_FrightPaymentID")]
        public int? NFrightPaymentId { get; set; }
        [Column("N_IncotermsID")]
        public int? NIncotermsId { get; set; }
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
        [Column("B_IsSaveDraft")]
        public bool? BIsSaveDraft { get; set; }
        [Column("N_FlightId")]
        public int NFlightId { get; set; }
        [Column("B_IsExport")]
        public bool? BIsExport { get; set; }
        [Column("N_BillAmount", TypeName = "money")]
        public decimal? NBillAmount { get; set; }
        [Column("N_CurrencyId")]
        public int? NCurrencyId { get; set; }
        [Column("X_FlightNo")]
        [StringLength(20)]
        public string XFlightNo { get; set; }
        [Column("N_TaxAmount", TypeName = "money")]
        public decimal? NTaxAmount { get; set; }
        [Column("N_ModeOfTransaction")]
        public int? NModeOfTransaction { get; set; }
        [Column("X_ServiceLevel")]
        [StringLength(100)]
        public string XServiceLevel { get; set; }
        [Column("X_InvoiceMode")]
        [StringLength(100)]
        public string XInvoiceMode { get; set; }
        [Column("N_QuotationId")]
        public int? NQuotationId { get; set; }
        [Column("X_Notes")]
        [StringLength(1000)]
        public string XNotes { get; set; }
        [Column("X_DeliveryTerms")]
        [StringLength(100)]
        public string XDeliveryTerms { get; set; }
        [Column("X_FreightPayment")]
        [StringLength(100)]
        public string XFreightPayment { get; set; }
        [Column("N_ServiceType")]
        public int? NServiceType { get; set; }
        [Column("X_Insurance")]
        [StringLength(50)]
        public string XInsurance { get; set; }
        [Column("B_IsOrder")]
        public bool? BIsOrder { get; set; }
        [Column("N_vehicleID")]
        public int? NVehicleId { get; set; }
        [Column("N_StatusID")]
        public int? NStatusId { get; set; }
        [Column("B_Prepayment")]
        public bool? BPrepayment { get; set; }
        [Column("X_ContainedDescription")]
        [StringLength(100)]
        public string XContainedDescription { get; set; }
        [Column("N_ContainedQty")]
        public double? NContainedQty { get; set; }
        [Column("N_ProjecID")]
        public int? NProjecId { get; set; }
        [Column("D_OrderDate", TypeName = "datetime")]
        public DateTime? DOrderDate { get; set; }
        [Column("X_PlateNumber")]
        [StringLength(20)]
        public string XPlateNumber { get; set; }
        [Column("X_Driver")]
        [StringLength(50)]
        public string XDriver { get; set; }
        [Column("X_PhoneNo")]
        [StringLength(20)]
        public string XPhoneNo { get; set; }

        [ForeignKey(nameof(NArrivalId))]
        [InverseProperty(nameof(FfwAirport.FfwInvoiceMasterNArrival))]
        public virtual FfwAirport NArrival { get; set; }
        [ForeignKey(nameof(NDepartureId))]
        [InverseProperty(nameof(FfwAirport.FfwInvoiceMasterNDeparture))]
        public virtual FfwAirport NDeparture { get; set; }
    }
}
