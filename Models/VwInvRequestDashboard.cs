using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvRequestDashboard
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("N_SalesOrderId")]
        public int NSalesOrderId { get; set; }
        [Column("X_OrderNo")]
        [StringLength(50)]
        public string XOrderNo { get; set; }
        [Column("D_OrderDate")]
        public string DOrderDate { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("N_CustomerId")]
        public int? NCustomerId { get; set; }
        [Column("N_BillAmt")]
        [StringLength(30)]
        public string NBillAmt { get; set; }
        [Column("N_Discount_Amt")]
        [StringLength(30)]
        public string NDiscountAmt { get; set; }
        [Column("N_NetAmt")]
        [StringLength(30)]
        public string NNetAmt { get; set; }
        [Column("N_BalanceAmt", TypeName = "money")]
        public decimal? NBalanceAmt { get; set; }
        [Column("N_CashReceived")]
        [StringLength(30)]
        public string NCashReceived { get; set; }
        [Column("x_Notes")]
        [StringLength(1000)]
        public string XNotes { get; set; }
        [Column("N_Processed")]
        public bool? NProcessed { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_DueDays")]
        [StringLength(11)]
        public string NDueDays { get; set; }
        [Column("B_IsSaveDraft")]
        public bool? BIsSaveDraft { get; set; }
        [Required]
        [Column("X_FileNo")]
        [StringLength(50)]
        public string XFileNo { get; set; }
        [Column("D_ExpDeliveryDate", TypeName = "smalldatetime")]
        public DateTime? DExpDeliveryDate { get; set; }
        [Column("D_ExpDelDate")]
        [StringLength(8000)]
        public string DExpDelDate { get; set; }
        [Required]
        [Column("X_PurchaseOrderNo")]
        [StringLength(50)]
        public string XPurchaseOrderNo { get; set; }
        [Column("N_PRSID")]
        public int NPrsid { get; set; }
        [Required]
        [Column("X_PRSNo")]
        [StringLength(50)]
        public string XPrsno { get; set; }
        [Required]
        [Column("D_PRSDate")]
        public string DPrsdate { get; set; }
        [Column(TypeName = "smalldatetime")]
        public DateTime? EntryDate { get; set; }
        [Column("D_PRSEntryDate", TypeName = "datetime")]
        public DateTime? DPrsentryDate { get; set; }
        [Required]
        [Column("Request_type")]
        [StringLength(18)]
        public string RequestType { get; set; }
        [Column("X_POrderNo")]
        [StringLength(50)]
        public string XPorderNo { get; set; }
        [Column("N_POrderID")]
        public int? NPorderId { get; set; }
        [Column("N_MRNID")]
        public int? NMrnid { get; set; }
        [Column("X_MRNNo")]
        [StringLength(50)]
        public string XMrnno { get; set; }
        [Column("N_VendorID")]
        public int? NVendorId { get; set; }
        [Column("X_ReceiptNo")]
        [StringLength(50)]
        public string XReceiptNo { get; set; }
        [Column("PRSDateRpt", TypeName = "datetime")]
        public DateTime? PrsdateRpt { get; set; }
        [Column("X_Status")]
        [StringLength(9)]
        public string XStatus { get; set; }
        [Required]
        [Column("X_EntryFrom")]
        [StringLength(13)]
        public string XEntryFrom { get; set; }
    }
}
