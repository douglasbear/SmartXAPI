using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvSalesOrderNoGridSearch
    {
        [Column("N_SalesOrderId")]
        public int NSalesOrderId { get; set; }
        [Column("Order Date")]
        [StringLength(8000)]
        public string OrderDate { get; set; }
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("Order No")]
        [StringLength(50)]
        public string OrderNo { get; set; }
        [Column("Customer Code")]
        [StringLength(50)]
        public string CustomerCode { get; set; }
        [StringLength(100)]
        public string Customer { get; set; }
        [Column("N_CustomerId")]
        public int? NCustomerId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("D_OrderDate", TypeName = "smalldatetime")]
        public DateTime? DOrderDate { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("B_YearEndProcess")]
        public bool? BYearEndProcess { get; set; }
        [Column("N_Processed")]
        public int NProcessed { get; set; }
        [Column("B_CancelOrder")]
        public bool? BCancelOrder { get; set; }
        [Column("X_FileNo")]
        [StringLength(50)]
        public string XFileNo { get; set; }
        [Column("B_IsSaveDraft")]
        public bool? BIsSaveDraft { get; set; }
        [Required]
        [StringLength(5)]
        public string TransType { get; set; }
        [Column("N_ProcStatus")]
        public int? NProcStatus { get; set; }
        [Column("N_ApproveLevel")]
        public int? NApproveLevel { get; set; }
        [Column("X_PurchaseOrderNo")]
        [StringLength(50)]
        public string XPurchaseOrderNo { get; set; }
        [Column("X_BranchName")]
        [StringLength(50)]
        public string XBranchName { get; set; }
        [Column("N_Amount")]
        [StringLength(30)]
        public string NAmount { get; set; }
        [Column("N_OrderProcessed")]
        public int NOrderProcessed { get; set; }
        [Column("x_Notes")]
        [StringLength(1000)]
        public string XNotes { get; set; }
        [Column("N_ProcessedQty")]
        public double? NProcessedQty { get; set; }
        [Column("N_QtyDisplay")]
        public double? NQtyDisplay { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_SalesOrderDetailsID")]
        public int NSalesOrderDetailsId { get; set; }
    }
}
