using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwAccCompanyDisp
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [StringLength(50)]
        public string Code { get; set; }
        [Column("Company Name")]
        [StringLength(250)]
        public string CompanyName { get; set; }
        [StringLength(50)]
        public string Country { get; set; }
        [Column("B_Inactive")]
        public bool? BInactive { get; set; }
    }
}
