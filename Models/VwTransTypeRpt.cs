using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwTransTypeRpt
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Required]
        [StringLength(4)]
        public string Type { get; set; }
        [Column("N_TransType")]
        [StringLength(20)]
        public string NTransType { get; set; }
    }
}
