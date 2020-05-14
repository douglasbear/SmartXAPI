using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_ParticipateComp")]
    public partial class InvParticipateComp
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_participateID")]
        public int NParticipateId { get; set; }
        [Column("N_TenderID")]
        public int NTenderId { get; set; }
        [Column("X_TenderCode")]
        [StringLength(50)]
        public string XTenderCode { get; set; }
        [Column("X_CompanyName")]
        [StringLength(200)]
        public string XCompanyName { get; set; }
        [Column("X_QuoteValue", TypeName = "money")]
        public decimal? XQuoteValue { get; set; }
        [Column("X_Rank")]
        [StringLength(200)]
        public string XRank { get; set; }
    }
}
