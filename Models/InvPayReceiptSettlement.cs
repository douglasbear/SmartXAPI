using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_PayReceiptSettlement")]
    public partial class InvPayReceiptSettlement
    {
        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Key]
        [Column("N_SettlementID")]
        public int NSettlementId { get; set; }
        [Column("N_PartyID")]
        public int? NPartyId { get; set; }
        [Column("N_TransType")]
        public int? NTransType { get; set; }
        [Column("X_VoucherNo")]
        [StringLength(50)]
        public string XVoucherNo { get; set; }
        [Column("D_VoucherDate", TypeName = "datetime")]
        public DateTime? DVoucherDate { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("X_Notes")]
        public string XNotes { get; set; }
        [Column("N_CurrencyID")]
        public int? NCurrencyId { get; set; }
        [Column("N_ExchangeRate", TypeName = "money")]
        public decimal? NExchangeRate { get; set; }
    }
}
