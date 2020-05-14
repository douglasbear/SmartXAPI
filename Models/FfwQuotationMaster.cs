using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Ffw_QuotationMaster")]
    public partial class FfwQuotationMaster
    {
        [Column("N_CompanyId")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Key]
        [Column("N_QuotationId")]
        public int NQuotationId { get; set; }
        [Column("X_QuotationNo")]
        [StringLength(50)]
        public string XQuotationNo { get; set; }
        [Column("D_QuotationDate", TypeName = "smalldatetime")]
        public DateTime? DQuotationDate { get; set; }
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
        [Column("N_Processed")]
        public bool? NProcessed { get; set; }
        [Column("X_ServiceLevel")]
        [StringLength(100)]
        public string XServiceLevel { get; set; }
        [Column("X_InvoiceMode")]
        [StringLength(100)]
        public string XInvoiceMode { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Column("X_Notes")]
        [StringLength(1000)]
        public string XNotes { get; set; }
        [Column("X_DeliveryTerms")]
        [StringLength(100)]
        public string XDeliveryTerms { get; set; }
        [Column("N_ServiceType")]
        public int? NServiceType { get; set; }
        [Column("X_Insurance")]
        [StringLength(50)]
        public string XInsurance { get; set; }
        [Column("B_IsEnquiry")]
        public bool? BIsEnquiry { get; set; }

        [ForeignKey(nameof(NArrivalId))]
        [InverseProperty(nameof(FfwAirport.FfwQuotationMasterNArrival))]
        public virtual FfwAirport NArrival { get; set; }
        [ForeignKey(nameof(NDepartureId))]
        [InverseProperty(nameof(FfwAirport.FfwQuotationMasterNDeparture))]
        public virtual FfwAirport NDeparture { get; set; }
    }
}
