using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchFeeTypeDisp1
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        public int Code { get; set; }
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
    }
}
