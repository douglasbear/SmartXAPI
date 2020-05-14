using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Acc_MastLedger")]
    public partial class AccMastLedger
    {
        public AccMastLedger()
        {
            AccVoucherDetails = new HashSet<AccVoucherDetails>();
            AccYearWiseLedgerSettings = new HashSet<AccYearWiseLedgerSettings>();
            AssAssetCategory = new HashSet<AssAssetCategory>();
            AssTransactions = new HashSet<AssTransactions>();
            InvCostCentreTransactions = new HashSet<InvCostCentreTransactions>();
        }

        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_GroupID")]
        public int NGroupId { get; set; }
        [Key]
        [Column("N_LedgerID")]
        public int NLedgerId { get; set; }
        [Column("X_LedgerCode")]
        [StringLength(50)]
        public string XLedgerCode { get; set; }
        [Column("X_LedgerName")]
        [StringLength(500)]
        public string XLedgerName { get; set; }
        [Column("N_Reserved")]
        public int? NReserved { get; set; }
        [Column("B_Inactive")]
        public bool? BInactive { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("X_Level")]
        [StringLength(100)]
        public string XLevel { get; set; }
        [Column("X_CashTypeBehaviour")]
        [StringLength(50)]
        public string XCashTypeBehaviour { get; set; }
        [Column("X_LedgerName_Ar")]
        [StringLength(500)]
        public string XLedgerNameAr { get; set; }
        [Column("B_CostCenterEnabled")]
        public bool? BCostCenterEnabled { get; set; }
        [Key]
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_CashBahavID")]
        public int? NCashBahavId { get; set; }
        [Column("N_TransBehavID")]
        public int? NTransBehavId { get; set; }
        [Column("N_LedgerBehavID")]
        public int? NLedgerBehavId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("X_Form")]
        [StringLength(100)]
        public string XForm { get; set; }
        [Column("N_PostingBahavID")]
        public int? NPostingBahavId { get; set; }

        [ForeignKey("NCompanyId,NGroupId,NFnYearId")]
        [InverseProperty(nameof(AccMastGroup.AccMastLedger))]
        public virtual AccMastGroup N { get; set; }
        [InverseProperty("N")]
        public virtual ICollection<AccVoucherDetails> AccVoucherDetails { get; set; }
        [InverseProperty("N")]
        public virtual ICollection<AccYearWiseLedgerSettings> AccYearWiseLedgerSettings { get; set; }
        [InverseProperty("N")]
        public virtual ICollection<AssAssetCategory> AssAssetCategory { get; set; }
        [InverseProperty("N")]
        public virtual ICollection<AssTransactions> AssTransactions { get; set; }
        [InverseProperty("N")]
        public virtual ICollection<InvCostCentreTransactions> InvCostCentreTransactions { get; set; }
    }
}
