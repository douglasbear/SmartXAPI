using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvLedgerAll
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("X_TransType")]
        [StringLength(50)]
        public string XTransType { get; set; }
        [Column("N_PartyId")]
        public int? NPartyId { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime? DDate { get; set; }
        [Column("N_RefID")]
        public int NRefId { get; set; }
        [Column("X_RefCode")]
        [StringLength(50)]
        public string XRefCode { get; set; }
        [Column(TypeName = "money")]
        public decimal? AmountDr { get; set; }
        [Column(TypeName = "money")]
        public decimal? AmountCr { get; set; }
        [StringLength(1000)]
        public string Notes { get; set; }
        [Required]
        [StringLength(8)]
        public string PartyType { get; set; }
    }
}
