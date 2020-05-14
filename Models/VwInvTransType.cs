using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvTransType
    {
        [Required]
        [StringLength(3)]
        public string Type { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [StringLength(20)]
        public string TransType { get; set; }
        [StringLength(50)]
        public string TranNo { get; set; }
    }
}
