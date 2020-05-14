using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_SalesOrder")]
    public partial class InvSalesOrder
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Key]
        [Column("N_SalesOrderId")]
        public int NSalesOrderId { get; set; }
        [Column("X_OrderNo")]
        [StringLength(50)]
        public string XOrderNo { get; set; }
        [Column("D_OrderDate", TypeName = "smalldatetime")]
        public DateTime? DOrderDate { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("N_CustomerId")]
        public int? NCustomerId { get; set; }
        [Column("N_BillAmt", TypeName = "money")]
        public decimal? NBillAmt { get; set; }
        [Column("N_DiscountAmt", TypeName = "money")]
        public decimal? NDiscountAmt { get; set; }
        [Column("N_FreightAmt", TypeName = "money")]
        public decimal? NFreightAmt { get; set; }
        [Column("N_CashReceived", TypeName = "money")]
        public decimal? NCashReceived { get; set; }
        [Column("x_Notes")]
        [StringLength(1000)]
        public string XNotes { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_QuotationID")]
        public int? NQuotationId { get; set; }
        [Column("N_SalesID")]
        public int? NSalesId { get; set; }
        [Column("N_Processed")]
        public bool? NProcessed { get; set; }
        [Column("X_PurchaseOrderNo")]
        [StringLength(50)]
        public string XPurchaseOrderNo { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_SalesmanID")]
        public int? NSalesmanId { get; set; }
        [Column("B_CancelOrder")]
        public bool? BCancelOrder { get; set; }
        [Column("X_FileNo")]
        [StringLength(50)]
        public string XFileNo { get; set; }
        [Column("N_ApproveLevel")]
        public int? NApproveLevel { get; set; }
        [Column("N_ProcStatus")]
        public int? NProcStatus { get; set; }
        [Column("B_IsSaveDraft")]
        public bool? BIsSaveDraft { get; set; }
        [Column("N_AddlAmount", TypeName = "money")]
        public decimal? NAddlAmount { get; set; }
        [Column("D_ExpDeliveryDate", TypeName = "smalldatetime")]
        public DateTime? DExpDeliveryDate { get; set; }
        [Column("X_RfqRefNo")]
        [StringLength(100)]
        public string XRfqRefNo { get; set; }
        [Column("D_RfqRefDate", TypeName = "smalldatetime")]
        public DateTime? DRfqRefDate { get; set; }
        [Column("X_TandC")]
        [StringLength(2500)]
        public string XTandC { get; set; }
        [Column("N_TaxAmt", TypeName = "money")]
        public decimal? NTaxAmt { get; set; }
        [Column("N_JobOrderEmpID")]
        public int? NJobOrderEmpId { get; set; }
        [Column("N_JobOrderPriority")]
        public int? NJobOrderPriority { get; set; }
        [Column("N_JobOrderStatus")]
        public int? NJobOrderStatus { get; set; }
        [Column("X_JobOrderNotes")]
        [StringLength(500)]
        public string XJobOrderNotes { get; set; }
        [Column("N_OthTaxAmt", TypeName = "money")]
        public decimal? NOthTaxAmt { get; set; }
        [Column("N_OthTaxCategoryID")]
        public int? NOthTaxCategoryId { get; set; }
        [Column("N_OthTaxPercentage", TypeName = "money")]
        public decimal? NOthTaxPercentage { get; set; }
        [Column("N_Type")]
        public int? NType { get; set; }
        [Column("N_TranTypeID")]
        public int? NTranTypeId { get; set; }
        [Column("N_OrderTypeID")]
        public int? NOrderTypeId { get; set; }
        [Column("D_ContractEndDate", TypeName = "datetime")]
        public DateTime? DContractEndDate { get; set; }
        [Column("N_WSID")]
        public int? NWsid { get; set; }
        [Column("N_POId")]
        public int? NPoid { get; set; }
        [Column("D_CustPODate", TypeName = "datetime")]
        public DateTime? DCustPodate { get; set; }

        [ForeignKey("NCompanyId,NCustomerId,NFnYearId")]
        [InverseProperty(nameof(InvCustomer.InvSalesOrder))]
        public virtual InvCustomer N { get; set; }
    }
}
