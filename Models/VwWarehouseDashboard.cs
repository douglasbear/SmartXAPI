using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwWarehouseDashboard
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
        [Required]
        [Column("X_Status")]
        [StringLength(17)]
        public string XStatus { get; set; }
        [Column("D_ExpDeliveryDate", TypeName = "smalldatetime")]
        public DateTime? DExpDeliveryDate { get; set; }
        [Column("D_ExpDelDate")]
        [StringLength(8000)]
        public string DExpDelDate { get; set; }
        [Column("X_PurchaseOrderNo")]
        [StringLength(50)]
        public string XPurchaseOrderNo { get; set; }
        [Column("N_PRSID")]
        public int NPrsid { get; set; }
        [Column("X_PRSNo")]
        [StringLength(50)]
        public string XPrsno { get; set; }
        [Required]
        [Column("X_ProjectName")]
        [StringLength(50)]
        public string XProjectName { get; set; }
        [Column("N_TranTypeID")]
        public int? NTranTypeId { get; set; }
        [Column("N_Processed")]
        public bool? NProcessed { get; set; }
        [Column("N_PRSProcessed")]
        public int NPrsprocessed { get; set; }
        [Column("N_DispatchId")]
        public int NDispatchId { get; set; }
        [Required]
        [StringLength(16)]
        public string EntryScreen { get; set; }
        [Column("X_UserName")]
        [StringLength(60)]
        public string XUserName { get; set; }
        [Column("N_Type")]
        public int NType { get; set; }
        [Column("N_Projectid")]
        public int? NProjectid { get; set; }
    }
}
