using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvMrnfreights
    {
        [Column("X_VendorCode")]
        [StringLength(50)]
        public string XVendorCode { get; set; }
        [Column("X_VendorName")]
        [StringLength(100)]
        public string XVendorName { get; set; }
        [Column("X_Reason")]
        [StringLength(50)]
        public string XReason { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_MRNFreightID")]
        public int NMrnfreightId { get; set; }
        [Column("N_MRNID")]
        public int? NMrnid { get; set; }
        [Column("N_VendorID")]
        public int? NVendorId { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("N_ReasonID")]
        public int? NReasonId { get; set; }
        public int? Expr1 { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_AmountF", TypeName = "money")]
        public decimal? NAmountF { get; set; }
        [Column("X_Remarks")]
        [StringLength(1000)]
        public string XRemarks { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("X_ShortName")]
        [StringLength(50)]
        public string XShortName { get; set; }
        [Column("N_ExchangeRate", TypeName = "money")]
        public decimal? NExchangeRate { get; set; }
        [Column(TypeName = "money")]
        public decimal? VendorAmount { get; set; }
        [Column("N_CurrencyID")]
        public int? NCurrencyId { get; set; }
        [Column("N_TaxCategoryID1")]
        public int? NTaxCategoryId1 { get; set; }
        [Column("N_TaxAmt1", TypeName = "money")]
        public decimal? NTaxAmt1 { get; set; }
        [Column("N_TaxPercentage1", TypeName = "money")]
        public decimal? NTaxPercentage1 { get; set; }
        [Column("X_DisplayName")]
        [StringLength(100)]
        public string XDisplayName { get; set; }
    }
}
