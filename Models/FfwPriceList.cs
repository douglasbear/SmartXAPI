using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Ffw_PriceList")]
    public partial class FfwPriceList
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_PriceID")]
        public int NPriceId { get; set; }
        [Column("X_PriceCode")]
        [StringLength(100)]
        public string XPriceCode { get; set; }
        [Column("N_PartyID")]
        public int? NPartyId { get; set; }
        [Column("N_TypeID")]
        public int? NTypeId { get; set; }
        [Column("D_DateFrom", TypeName = "datetime")]
        public DateTime? DDateFrom { get; set; }
        [Column("D_DateTo", TypeName = "datetime")]
        public DateTime? DDateTo { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
    }
}
