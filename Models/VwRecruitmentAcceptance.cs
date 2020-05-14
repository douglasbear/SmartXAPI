using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwRecruitmentAcceptance
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_Accepatance")]
        public int? NAccepatance { get; set; }
        [Required]
        [Column("X_Status")]
        [StringLength(8)]
        public string XStatus { get; set; }
    }
}
