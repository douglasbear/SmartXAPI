using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_Jwl_RateMaster")]
    public partial class InvJwlRateMaster
    {
        [Key]
        [Column("N_RateID")]
        public int NRateId { get; set; }
        [Column("D_PriceDate", TypeName = "datetime")]
        public DateTime? DPriceDate { get; set; }
        [Column("N_TypeID")]
        public int? NTypeId { get; set; }
        [Column("N_Rate", TypeName = "money")]
        public decimal? NRate { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
    }
}
