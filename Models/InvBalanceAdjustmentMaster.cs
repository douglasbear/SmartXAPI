using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_BalanceAdjustmentMaster")]
    public partial class InvBalanceAdjustmentMaster
    {
        public InvBalanceAdjustmentMaster()
        {
            InvBalanceAdjustmentMasterDetails = new HashSet<InvBalanceAdjustmentMasterDetails>();
        }

        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("X_VoucherNo")]
        [StringLength(50)]
        public string XVoucherNo { get; set; }
        [Column("D_AdjustmentDate", TypeName = "datetime")]
        public DateTime? DAdjustmentDate { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_TransType")]
        public int? NTransType { get; set; }
        [Column("X_notes")]
        [StringLength(50)]
        public string XNotes { get; set; }
        [Column("N_PartyType")]
        public int? NPartyType { get; set; }
        [Column("N_PartyID")]
        public int? NPartyId { get; set; }
        [Key]
        [Column("N_AdjustmentId")]
        public int NAdjustmentId { get; set; }
        [Column("N_AmountF", TypeName = "money")]
        public decimal? NAmountF { get; set; }
        [Column("N_CurExchRate", TypeName = "money")]
        public decimal? NCurExchRate { get; set; }
        [Column("N_WOID")]
        public int? NWoid { get; set; }
        [Column("N_MedInsDeletionID")]
        public int? NMedInsDeletionId { get; set; }
        [Column("X_EntryFrom")]
        [StringLength(200)]
        public string XEntryFrom { get; set; }

        [InverseProperty("NAdjustment")]
        public virtual ICollection<InvBalanceAdjustmentMasterDetails> InvBalanceAdjustmentMasterDetails { get; set; }
    }
}
