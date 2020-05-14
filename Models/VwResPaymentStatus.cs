using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwResPaymentStatus
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Required]
        [Column("Std_Status")]
        [StringLength(3)]
        public string StdStatus { get; set; }
    }
}
