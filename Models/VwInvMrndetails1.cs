using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvMrndetails1
    {
        [Column("X_ItemCode")]
        [StringLength(100)]
        public string XItemCode { get; set; }
        [Column("X_ItemName")]
        [StringLength(800)]
        public string XItemName { get; set; }
        [Column("X_ItemUnit")]
        [StringLength(500)]
        public string XItemUnit { get; set; }
        [Column("N_UnitQty")]
        public double? NUnitQty { get; set; }
        [Column("X_BaseUnit")]
        [StringLength(500)]
        public string XBaseUnit { get; set; }
        [Column("B_YearEndProcess")]
        public bool? BYearEndProcess { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_MRNID")]
        public int NMrnid { get; set; }
        [Column("X_MRNNo")]
        [StringLength(50)]
        public string XMrnno { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_QtyToStock")]
        public double? NQtyToStock { get; set; }
        [Column("N_ReturnQty")]
        public double? NReturnQty { get; set; }
        [Column("X_Reason")]
        [StringLength(50)]
        public string XReason { get; set; }
        [Column("N_MRNDetailsID")]
        public int NMrndetailsId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_PurchaseDetailsID")]
        public int? NPurchaseDetailsId { get; set; }
        [Column("X_PartNumber")]
        [StringLength(100)]
        public string XPartNumber { get; set; }
        [Column("N_BaseUnitID")]
        public int? NBaseUnitId { get; set; }
        public double? BaseQty { get; set; }
        [Column("N_ItemUnitID")]
        public int? NItemUnitId { get; set; }
        [Column("N_PRSDetailsID")]
        public int? NPrsdetailsId { get; set; }
        [Column("N_POrderDetailsID")]
        [StringLength(100)]
        public string NPorderDetailsId { get; set; }
        [Column("N_POrderid")]
        public int? NPorderid { get; set; }
        [Column("N_PRSID")]
        public int? NPrsid { get; set; }
        [Column("N_PPrice", TypeName = "money")]
        public decimal? NPprice { get; set; }
        public int? Expr1 { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_SerialFrom")]
        [StringLength(50)]
        public string NSerialFrom { get; set; }
        [Column("N_SerialTo")]
        [StringLength(50)]
        public string NSerialTo { get; set; }
        [Column("B_IsIMEI")]
        public bool? BIsImei { get; set; }
        [Column("D_ExpiryDate", TypeName = "datetime")]
        public DateTime? DExpiryDate { get; set; }
        [Column("X_BatchCode")]
        [StringLength(50)]
        public string XBatchCode { get; set; }
        [Column("OrderID")]
        public int? OrderId { get; set; }
        [StringLength(50)]
        public string OrderNo { get; set; }
        [Column("PRSID")]
        public int? Prsid { get; set; }
        [Column("PRSNo")]
        [StringLength(50)]
        public string Prsno { get; set; }
        [Column("N_ExchangeRate", TypeName = "money")]
        public decimal? NExchangeRate { get; set; }
        [Column("X_Currency")]
        [StringLength(10)]
        public string XCurrency { get; set; }
        [Column("N_CurrencyID")]
        public int? NCurrencyId { get; set; }
        [Column("N_PpriceF", TypeName = "money")]
        public decimal? NPpriceF { get; set; }
        [Column("N_TaxCategoryID1")]
        public int? NTaxCategoryId1 { get; set; }
        [Column("N_TaxPercentage1", TypeName = "money")]
        public decimal? NTaxPercentage1 { get; set; }
        [Column("N_TaxAmt1", TypeName = "money")]
        public decimal? NTaxAmt1 { get; set; }
        [Column("N_TaxCategoryID2")]
        public int? NTaxCategoryId2 { get; set; }
        [Column("N_TaxPercentage2", TypeName = "money")]
        public decimal? NTaxPercentage2 { get; set; }
        [Column("N_TaxAmt2", TypeName = "money")]
        public decimal? NTaxAmt2 { get; set; }
        [Column("X_DisplayName2")]
        [StringLength(100)]
        public string XDisplayName2 { get; set; }
        [Column("X_DisplayName1")]
        [StringLength(100)]
        public string XDisplayName1 { get; set; }
        [Column("N_DiscountAmtF", TypeName = "money")]
        public decimal? NDiscountAmtF { get; set; }
        [Column("N_DiscountAmt", TypeName = "money")]
        public decimal? NDiscountAmt { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }
        [Column("X_ProjectName")]
        [StringLength(50)]
        public string XProjectName { get; set; }
    }
}
