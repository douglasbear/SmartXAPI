using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvSalesAnalysis
    {
        public long? Srl { get; set; }
        [Column("X_ReceiptNo")]
        [StringLength(50)]
        public string XReceiptNo { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_SalesId")]
        public int? NSalesId { get; set; }
        [Column("N_Sprice", TypeName = "money")]
        public decimal? NSprice { get; set; }
        [Column("Selling_Price")]
        public double? SellingPrice { get; set; }
        [Column("X_InvoiceNo")]
        [StringLength(50)]
        public string XInvoiceNo { get; set; }
        [Column("N_PPrice")]
        public double? NPprice { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [Column("N_QtyDisplay")]
        public double? NQtyDisplay { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_CustomerId")]
        public int? NCustomerId { get; set; }
        [Column("N_SalesDetailsID")]
        public int NSalesDetailsId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column(@"D_InvoiceDat
e", TypeName = "datetime")]
        public DateTime? DInvoiceDatE { get; set; }
        [Column("D_SalesDate", TypeName = "datetime")]
        public DateTime? DSalesDate { get; set; }
        [Column("N_BillAmt", TypeName = "money")]
        public decimal? NBillAmt { get; set; }
        [Column("N_DiscountAmt", TypeName = "money")]
        public decimal? NDiscountAmt { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("N_SalesmanID")]
        public int? NSalesmanId { get; set; }
        [Column("X_SalesmanName")]
        [StringLength(100)]
        public string XSalesmanName { get; set; }
        [Column("X_SalesUnit")]
        [StringLength(500)]
        public string XSalesUnit { get; set; }
        [Column("N_Cost", TypeName = "decimal(38, 6)")]
        public decimal? NCost { get; set; }
        [Column("N_SalesItemUnitID")]
        public int? NSalesItemUnitId { get; set; }
        [Column("N_ItemDiscAmt", TypeName = "money")]
        public decimal? NItemDiscAmt { get; set; }
        [Column("N_ItemSelPrice")]
        public double? NItemSelPrice { get; set; }
        [Column("N_FreightAmt", TypeName = "money")]
        public decimal? NFreightAmt { get; set; }
        [Column("N_SalesComm", TypeName = "money")]
        public decimal? NSalesComm { get; set; }
        [Column("N_AvgCost")]
        public double? NAvgCost { get; set; }
        [Required]
        [Column("Trans_Type")]
        [StringLength(2)]
        public string TransType { get; set; }
        [Column("N_PaymentID")]
        public int? NPaymentId { get; set; }
        [StringLength(50)]
        public string TransNo { get; set; }
        [Column("Remarks_No")]
        [StringLength(50)]
        public string RemarksNo { get; set; }
        [Column("X_CustPONo")]
        [StringLength(50)]
        public string XCustPono { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
    }
}
