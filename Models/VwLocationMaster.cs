using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwLocationMaster
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_LocationID")]
        public int NLocationId { get; set; }
        [Column("X_LocationCode")]
        public string XLocationCode { get; set; }
        [Column("X_LocationName")]
        public string XLocationName { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("B_IsCurrent")]
        public bool? BIsCurrent { get; set; }
        [Column("N_TypeId")]
        public int? NTypeId { get; set; }
        [Column("B_IsDefault")]
        public bool? BIsDefault { get; set; }
    }
}
