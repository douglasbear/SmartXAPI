using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_MRNDetails")]
    public partial class InvMrndetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_MRNID")]
        public int? NMrnid { get; set; }
        [Key]
        [Column("N_MRNDetailsID")]
        public int NMrndetailsId { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_ItemUnitID")]
        public int? NItemUnitId { get; set; }
        [Column("N_QtyToStock")]
        public double? NQtyToStock { get; set; }
        [Column("N_ReturnQty")]
        public double? NReturnQty { get; set; }
        [Column("X_Reason")]
        [StringLength(50)]
        public string XReason { get; set; }
        [Column("N_PurchaseDetailsID")]
        public int? NPurchaseDetailsId { get; set; }
        [Column("X_PartNumber")]
        [StringLength(100)]
        public string XPartNumber { get; set; }
        [Column("N_PRSDetailsID")]
        public int? NPrsdetailsId { get; set; }
        [Column("N_POrderDetailsID")]
        [StringLength(100)]
        public string NPorderDetailsId { get; set; }
        [Column("N_PPrice", TypeName = "money")]
        public decimal? NPprice { get; set; }
        [Column("N_SPrice", TypeName = "money")]
        public decimal? NSprice { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [Column("X_BatchCode")]
        [StringLength(50)]
        public string XBatchCode { get; set; }
        [Column("D_ExpiryDate", TypeName = "datetime")]
        public DateTime? DExpiryDate { get; set; }
        [Column("N_SerialFrom")]
        [StringLength(50)]
        public string NSerialFrom { get; set; }
        [Column("N_SerialTo")]
        [StringLength(50)]
        public string NSerialTo { get; set; }
        [Column("X_PONo")]
        [StringLength(50)]
        public string XPono { get; set; }
        [Column("N_PONo")]
        public int? NPono { get; set; }
        [Column("X_PRSNo")]
        [StringLength(50)]
        public string XPrsno { get; set; }
        [Column("N_PRSID")]
        public int? NPrsid { get; set; }
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
        [Column("N_Cost", TypeName = "decimal(20, 6)")]
        public decimal? NCost { get; set; }
        [Column("N_CostF", TypeName = "decimal(20, 6)")]
        public decimal? NCostF { get; set; }
        [Column("N_DiscountAmtF", TypeName = "money")]
        public decimal? NDiscountAmtF { get; set; }
        [Column("N_DiscountAmt", TypeName = "money")]
        public decimal? NDiscountAmt { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }
    }
}
