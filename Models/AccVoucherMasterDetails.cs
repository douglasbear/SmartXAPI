using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Acc_VoucherMaster_Details")]
    public partial class AccVoucherMasterDetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_VoucherID")]
        public int NVoucherId { get; set; }
        [Key]
        [Column("N_VoucherDetailsID")]
        public int NVoucherDetailsId { get; set; }
        [Column("N_LedgerID")]
        public int? NLedgerId { get; set; }
        [Column("X_Description")]
        [StringLength(1000)]
        public string XDescription { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("N_SalesID")]
        public int? NSalesId { get; set; }
        [Column("N_AccType")]
        public int? NAccType { get; set; }
        [Column("N_AccID")]
        public int? NAccId { get; set; }
        [Column("X_AmtInWords_Ar")]
        [StringLength(2000)]
        public string XAmtInWordsAr { get; set; }
        [Column("N_InventoryID")]
        public int? NInventoryId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_TaxCategoryID1")]
        public int? NTaxCategoryId1 { get; set; }
        [Column("N_TaxPercentage1", TypeName = "money")]
        public decimal? NTaxPercentage1 { get; set; }
        [Column("N_TaxAmt1", TypeName = "money")]
        public decimal? NTaxAmt1 { get; set; }
        [Column("X_Narration_Ar")]
        [StringLength(2000)]
        public string XNarrationAr { get; set; }
        [Column("N_TypeID")]
        public int? NTypeId { get; set; }

        [ForeignKey(nameof(NVoucherId))]
        [InverseProperty(nameof(AccVoucherMaster.AccVoucherMasterDetails))]
        public virtual AccVoucherMaster NVoucher { get; set; }
    }
}
