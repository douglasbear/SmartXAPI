using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("vsa_Authority")]
    public partial class VsaAuthority
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_AuthorityID")]
        public int NAuthorityId { get; set; }
        [Column("N_OccupationCode")]
        [StringLength(20)]
        public string NOccupationCode { get; set; }
        [Column("X_Authority")]
        [StringLength(50)]
        public string XAuthority { get; set; }
    }
}
