using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvWarehouseDisp
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_WarehouseID")]
        public int NWarehouseId { get; set; }
        [Column("Site Code")]
        [StringLength(50)]
        public string SiteCode { get; set; }
        [Column("Site Name")]
        [StringLength(50)]
        public string SiteName { get; set; }
    }
}
