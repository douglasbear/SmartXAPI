using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwCostCentreMaster
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_CostCentreID")]
        public int NCostCentreId { get; set; }
        [Column("X_CostCentreCode")]
        [StringLength(50)]
        public string XCostCentreCode { get; set; }
        [Column("X_CostcentreName")]
        [StringLength(100)]
        public string XCostcentreName { get; set; }
        [Column("N_GroupID")]
        public int? NGroupId { get; set; }
        [Required]
        [Column("X_GroupName")]
        [StringLength(100)]
        public string XGroupName { get; set; }
        [Required]
        [Column("X_GroupCode")]
        [StringLength(50)]
        public string XGroupCode { get; set; }
    }
}
