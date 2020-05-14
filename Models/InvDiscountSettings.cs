using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_DiscountSettings")]
    public partial class InvDiscountSettings
    {
        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Key]
        [Column("N_DiscID")]
        public int NDiscId { get; set; }
        [Column("X_DiscCode")]
        [StringLength(50)]
        public string XDiscCode { get; set; }
        [Column("N_DiscPerc", TypeName = "money")]
        public decimal? NDiscPerc { get; set; }
        [Column("N_MaxDiscCount")]
        public int? NMaxDiscCount { get; set; }
        [Column("N_MaxDiscPeriod")]
        public int? NMaxDiscPeriod { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("X_DiscDescription")]
        [StringLength(500)]
        public string XDiscDescription { get; set; }
    }
}
