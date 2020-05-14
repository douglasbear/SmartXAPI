using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("IDC_LocationMaster")]
    public partial class IdcLocationMaster
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("X_LocationCode")]
        [StringLength(50)]
        public string XLocationCode { get; set; }
        [Column("X_LocationName")]
        [StringLength(50)]
        public string XLocationName { get; set; }
    }
}
