using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvMrnnoSearch
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_MRNID")]
        public int NMrnid { get; set; }
        [Column("MRN No")]
        [StringLength(50)]
        public string MrnNo { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("D_MRNDate", TypeName = "datetime")]
        public DateTime? DMrndate { get; set; }
        [Column("N_userID")]
        public int? NUserId { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_VendorID")]
        public int? NVendorId { get; set; }
        [Column("B_YearEndProcess")]
        public bool? BYearEndProcess { get; set; }
        [Column("MRN Date")]
        [StringLength(8000)]
        public string MrnDate { get; set; }
        [Column("N_PRSID")]
        public int? NPrsid { get; set; }
        [Column("MRNDate", TypeName = "datetime")]
        public DateTime? Mrndate1 { get; set; }
        [Column("N_POrderid")]
        public int? NPorderid { get; set; }
        [Column("X_VendorCode")]
        [StringLength(50)]
        public string XVendorCode { get; set; }
        [Column("X_VendorName")]
        [StringLength(100)]
        public string XVendorName { get; set; }
        [Required]
        [StringLength(50)]
        public string OrderNo { get; set; }
        [StringLength(50)]
        public string PurchaseInvoice { get; set; }
        [Column("X_Description")]
        public string XDescription { get; set; }
        [Required]
        [Column("X_VendorInvoice")]
        [StringLength(50)]
        public string XVendorInvoice { get; set; }
        [Required]
        [Column("X_VendorDeliveryNote")]
        [StringLength(15)]
        public string XVendorDeliveryNote { get; set; }
        [Required]
        [StringLength(50)]
        public string FileNo { get; set; }
        [Column("N_FreightAmt", TypeName = "money")]
        public decimal? NFreightAmt { get; set; }
        [Column("N_TaxAmt", TypeName = "money")]
        public decimal? NTaxAmt { get; set; }
        [Column("B_IsDirectMRN")]
        public bool? BIsDirectMrn { get; set; }
        [Column("B_IsSaveDraft")]
        public bool? BIsSaveDraft { get; set; }
        [Column("N_Processed")]
        public bool? NProcessed { get; set; }
        [Required]
        [StringLength(5)]
        public string TransType { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }
    }
}
