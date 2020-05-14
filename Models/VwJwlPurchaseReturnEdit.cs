using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwJwlPurchaseReturnEdit
    {
        [Column("X_InvoiceNo")]
        [StringLength(50)]
        public string XInvoiceNo { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_PurchaseID")]
        public int? NPurchaseId { get; set; }
        [Column("N_PurchaseDetailsID")]
        public int NPurchaseDetailsId { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("X_Model")]
        [StringLength(100)]
        public string XModel { get; set; }
        [Column("N_Karat")]
        public double? NKarat { get; set; }
        [Column("N_GoldWeight")]
        public double? NGoldWeight { get; set; }
        [Column("N_StoneWeight")]
        public double? NStoneWeight { get; set; }
        [Column("N_LessWeight")]
        public double? NLessWeight { get; set; }
        [Column("N_NetWeight")]
        public double? NNetWeight { get; set; }
        [Column("N_MCPerc")]
        public double? NMcperc { get; set; }
        [Column("N_MCRate", TypeName = "money")]
        public decimal? NMcrate { get; set; }
        [Column("N_WastagePerc")]
        public double? NWastagePerc { get; set; }
        [Column("N_WastageRate", TypeName = "money")]
        public decimal? NWastageRate { get; set; }
        [Column("N_StoneProfitPerc")]
        public double? NStoneProfitPerc { get; set; }
        [Column("N_StoneRate", TypeName = "money")]
        public decimal? NStoneRate { get; set; }
        [Column("N_UnitRate", TypeName = "money")]
        public decimal? NUnitRate { get; set; }
        [Column("N_Quantity")]
        public int? NQuantity { get; set; }
        [Column("N_GoldAmount", TypeName = "money")]
        public decimal? NGoldAmount { get; set; }
        [Column("N_StoneAmount", TypeName = "money")]
        public decimal? NStoneAmount { get; set; }
        [Column("N_MakingCharge", TypeName = "money")]
        public decimal? NMakingCharge { get; set; }
        [Column("N_Discount", TypeName = "money")]
        public decimal? NDiscount { get; set; }
        [Column("N_TotalAmount", TypeName = "money")]
        public decimal? NTotalAmount { get; set; }
        [Column("N_MCMinimumPerc")]
        public double? NMcminimumPerc { get; set; }
        [Column("N_OtherExp", TypeName = "money")]
        public decimal? NOtherExp { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("X_CreditNoteNo")]
        [StringLength(50)]
        public string XCreditNoteNo { get; set; }
        [Column("D_RetDate", TypeName = "datetime")]
        public DateTime? DRetDate { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        public double? RetQty { get; set; }
        [Column("X_ItemCode")]
        [StringLength(50)]
        public string XItemCode { get; set; }
        [Column("X_ItemName")]
        [StringLength(600)]
        public string XItemName { get; set; }
        [Column("N_RetQty")]
        public double? NRetQty { get; set; }
        [Column("N_RetAmount", TypeName = "money")]
        public decimal? NRetAmount { get; set; }
        [Column("N_CreditNoteId")]
        public int NCreditNoteId { get; set; }
        [Column("N_TotalReceived", TypeName = "money")]
        public decimal? NTotalReceived { get; set; }
        [Column("N_TotalReturnAmount", TypeName = "money")]
        public decimal? NTotalReturnAmount { get; set; }
    }
}
