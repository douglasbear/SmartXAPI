using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwVsaOccupationDisp
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_OccupationID")]
        public int NOccupationId { get; set; }
        [Column("N_OccupationCode")]
        [StringLength(20)]
        public string NOccupationCode { get; set; }
        [Column("X_AnzscoCode")]
        [StringLength(50)]
        public string XAnzscoCode { get; set; }
        [Column("X_Authority")]
        [StringLength(50)]
        public string XAuthority { get; set; }
        [Column("N_AuthorityID")]
        public int NAuthorityId { get; set; }
        [Column("X_Occupation")]
        [StringLength(200)]
        public string XOccupation { get; set; }
    }
}
