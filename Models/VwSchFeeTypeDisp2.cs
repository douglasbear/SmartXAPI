using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchFeeTypeDisp2
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [StringLength(30)]
        public string Code { get; set; }
        [Required]
        [Column("Fee Type")]
        [StringLength(50)]
        public string FeeType { get; set; }
        [Required]
        [StringLength(15)]
        public string Frequency { get; set; }
        [StringLength(20)]
        public string Amount { get; set; }
        [Column("N_FeeCategoryID")]
        public int? NFeeCategoryId { get; set; }
        [Column("N_FeeTypeID")]
        public int? NFeeTypeId { get; set; }
        [Column("N_Type")]
        public int? NType { get; set; }
    }
}
