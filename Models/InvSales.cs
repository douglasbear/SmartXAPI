using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_Sales")]
    public partial class InvSales
    {
        public InvSales()
        {
            InvSalesDetails = new HashSet<InvSalesDetails>();
        }

        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Key]
        [Column("N_SalesId")]
        public int NSalesId { get; set; }
        [Column("X_ReceiptNo")]
        [StringLength(50)]
        public string XReceiptNo { get; set; }
        [Column("D_SalesDate", TypeName = "smalldatetime")]
        public DateTime? DSalesDate { get; set; }
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
        [Column("N_SalesOrderID")]
        public int? NSalesOrderId { get; set; }
        [Column("X_POrderNo")]
        [StringLength(50)]
        public string XPorderNo { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }
        [Column("B_BeginingBalEntry")]
        public bool? BBeginingBalEntry { get; set; }
        [Column("N_SalesType")]
        public int? NSalesType { get; set; }
        [Column("N_SalesRefID")]
        public int? NSalesRefId { get; set; }
        [Column("N_SalesmanID")]
        public int? NSalesmanId { get; set; }
        [Column("N_SalesmanAmt", TypeName = "money")]
        public decimal? NSalesmanAmt { get; set; }
        [Column("N_SalesmanPerc")]
        public double? NSalesmanPerc { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("X_TransType")]
        [StringLength(50)]
        public string XTransType { get; set; }
        [Column("N_CreditReceived", TypeName = "money")]
        public decimal? NCreditReceived { get; set; }
        [Column("N_ChequeReceived", TypeName = "money")]
        public decimal? NChequeReceived { get; set; }
        [Column("B_IsSaveDraft")]
        public bool? BIsSaveDraft { get; set; }
        [Column("X_QuotationNo")]
        [StringLength(50)]
        public string XQuotationNo { get; set; }
        [Column("N_PaymentMethodId")]
        public int? NPaymentMethodId { get; set; }
        [Column("N_DeliveryNoteId")]
        public int? NDeliveryNoteId { get; set; }
        [Column("N_ApproveLevel")]
        public int? NApproveLevel { get; set; }
        [Column("N_ProcStatus")]
        public int? NProcStatus { get; set; }
        [Column("X_CustPONo")]
        [StringLength(50)]
        public string XCustPono { get; set; }
        [Column("X_TandC")]
        [StringLength(2500)]
        public string XTandC { get; set; }
        [Column("N_TaxAmt", TypeName = "money")]
        public decimal? NTaxAmt { get; set; }
        [Column("N_DriverID")]
        public int? NDriverId { get; set; }
        [Column("N_TruckID")]
        public int? NTruckId { get; set; }
        [Column("X_DriverName")]
        [StringLength(200)]
        public string XDriverName { get; set; }
        [Column("X_FreeText")]
        [StringLength(100)]
        public string XFreeText { get; set; }
        [Column("X_CustomerDetails")]
        [StringLength(100)]
        public string XCustomerDetails { get; set; }
        [Column("X_GSTIN")]
        [StringLength(100)]
        public string XGstin { get; set; }
        [Column("B_POS")]
        public bool? BPos { get; set; }
        [Column("N_Hold")]
        public int? NHold { get; set; }
        [Column("D_WorkOrderDate", TypeName = "datetime")]
        public DateTime? DWorkOrderDate { get; set; }
        [Column("X_WorkOrderNo")]
        [StringLength(50)]
        public string XWorkOrderNo { get; set; }
        [Column("N_TenderAmount", TypeName = "money")]
        public decimal? NTenderAmount { get; set; }
        [Column("N_UpdatedBy")]
        public int? NUpdatedBy { get; set; }
        [Column("D_UpdatedDate", TypeName = "datetime")]
        public DateTime? DUpdatedDate { get; set; }
        [Column("X_CardNo")]
        [StringLength(20)]
        public string XCardNo { get; set; }
        [Column("N_OthTaxAmt", TypeName = "money")]
        public decimal? NOthTaxAmt { get; set; }
        [Column("N_OthTaxCategoryID")]
        public int? NOthTaxCategoryId { get; set; }
        [Column("N_OthTaxPercentage", TypeName = "money")]
        public decimal? NOthTaxPercentage { get; set; }
        [Column("N_CessAmt", TypeName = "money")]
        public decimal? NCessAmt { get; set; }
        [Column("N_DiscID")]
        public int? NDiscId { get; set; }
        [Column("N_TermsID")]
        public int? NTermsId { get; set; }
        [Column("N_SavedPoints")]
        public int? NSavedPoints { get; set; }
        [Column("D_PrintDate", TypeName = "datetime")]
        public DateTime? DPrintDate { get; set; }

        [ForeignKey("NCompanyId,NCustomerId,NFnYearId")]
        [InverseProperty(nameof(InvCustomer.InvSales))]
        public virtual InvCustomer N { get; set; }
        [ForeignKey(nameof(NCompanyId))]
        [InverseProperty(nameof(AccCompany.InvSales))]
        public virtual AccCompany NCompany { get; set; }
        [InverseProperty("NSales")]
        public virtual ICollection<InvSalesDetails> InvSalesDetails { get; set; }
    }
}
