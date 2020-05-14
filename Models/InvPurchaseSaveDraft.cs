using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_Purchase_SaveDraft")]
    public partial class InvPurchaseSaveDraft
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Key]
        [Column("N_PurchaseID")]
        public int NPurchaseId { get; set; }
        [Column("X_InvoiceNo")]
        [StringLength(50)]
        public string XInvoiceNo { get; set; }
        [Column("N_VendorID")]
        public int? NVendorId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("N_InvoiceAmt", TypeName = "money")]
        public decimal? NInvoiceAmt { get; set; }
        [Column("N_DiscountAmt", TypeName = "money")]
        public decimal? NDiscountAmt { get; set; }
        [Column("N_userID")]
        public int? NUserId { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("N_CostCentreID")]
        public int? NCostCentreId { get; set; }
        [Column("N_Cost_BranchID")]
        public int? NCostBranchId { get; set; }
    }
}
