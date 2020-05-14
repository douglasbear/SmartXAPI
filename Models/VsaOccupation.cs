using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("vsa_Occupation")]
    public partial class VsaOccupation
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_OccupationID")]
        public int NOccupationId { get; set; }
        [Column("N_OccupationCode")]
        [StringLength(20)]
        public string NOccupationCode { get; set; }
        [Column("X_Occupation")]
        [StringLength(200)]
        public string XOccupation { get; set; }
        [Column("X_AnzscoCode")]
        [StringLength(50)]
        public string XAnzscoCode { get; set; }
        [Column("N_AuthorityID​")]
        public int? NAuthorityId { get; set; }
    }
}
