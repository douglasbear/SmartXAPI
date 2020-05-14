using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_LoyaltyPointOut")]
    public partial class InvLoyaltyPointOut
    {
        [Key]
        [Column("N_PointOutId")]
        public int NPointOutId { get; set; }
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_TransId")]
        public int? NTransId { get; set; }
        [Column("N_PartyId")]
        public int? NPartyId { get; set; }
        [Column("N_PointsOut")]
        public int? NPointsOut { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("X_Type")]
        [StringLength(20)]
        public string XType { get; set; }
        [Column("D_DateOut", TypeName = "datetime")]
        public DateTime? DDateOut { get; set; }
        [Column("N_AvailPoints")]
        public int? NAvailPoints { get; set; }
        [Column("N_AppliedAmt", TypeName = "money")]
        public decimal? NAppliedAmt { get; set; }
        [Column("N_SalesID")]
        public int? NSalesId { get; set; }
    }
}
