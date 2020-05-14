using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSalesOrderDashboard
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
        [Column("D_SalesOrderDate", TypeName = "smalldatetime")]
        public DateTime? DSalesOrderDate { get; set; }
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
        [Column("M_NetAmt", TypeName = "money")]
        public decimal? MNetAmt { get; set; }
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
        [Column("X_Status")]
        [StringLength(20)]
        public string XStatus { get; set; }
        [Column("D_ExpDeliveryDate", TypeName = "smalldatetime")]
        public DateTime? DExpDeliveryDate { get; set; }
        [Column("D_ExpDelDate")]
        [StringLength(8000)]
        public string DExpDelDate { get; set; }
        [Column("X_PurchaseOrderNo")]
        [StringLength(50)]
        public string XPurchaseOrderNo { get; set; }
        [Column("X_SalesNumber")]
        [StringLength(20)]
        public string XSalesNumber { get; set; }
        [Required]
        [Column("X_SalesmanName")]
        [StringLength(100)]
        public string XSalesmanName { get; set; }
        [Column("N_Type")]
        public int? NType { get; set; }
        [Required]
        [StringLength(13)]
        public string EntryFrom { get; set; }
        [Required]
        [Column("X_CurrentStatus")]
        [StringLength(50)]
        public string XCurrentStatus { get; set; }
        [Column("X_UserName")]
        [StringLength(60)]
        public string XUserName { get; set; }
        [Required]
        [Column("D_LastInvoiceDate")]
        public string DLastInvoiceDate { get; set; }
    }
}
