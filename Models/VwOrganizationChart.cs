using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwOrganizationChart
    {
        [Column("X_PositionCode")]
        [StringLength(50)]
        public string XPositionCode { get; set; }
        [Column("X_Position")]
        [StringLength(100)]
        public string XPosition { get; set; }
        [Column("X_PositionLocale")]
        [StringLength(100)]
        public string XPositionLocale { get; set; }
        [Column("B_IsSupervisor")]
        public bool? BIsSupervisor { get; set; }
        [StringLength(50)]
        public string ParentCode { get; set; }
        [StringLength(100)]
        public string ParentName { get; set; }
        [Column("ParentID")]
        public int ParentId { get; set; }
        [Column("N_PositionID")]
        public int NPositionId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
    }
}
