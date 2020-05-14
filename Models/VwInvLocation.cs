using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvLocation
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_LocationID")]
        public int NLocationId { get; set; }
        [Column("X_LocationCode")]
        public string XLocationCode { get; set; }
        [Column("X_LocationName")]
        public string XLocationName { get; set; }
        [Column("X_BranchName")]
        [StringLength(50)]
        public string XBranchName { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("X_BranchCode")]
        [StringLength(50)]
        public string XBranchCode { get; set; }
        [Column("N_TypeId")]
        public int? NTypeId { get; set; }
        [Column("X_TypeName")]
        [StringLength(50)]
        public string XTypeName { get; set; }
        [Column("N_DefaultId")]
        public int? NDefaultId { get; set; }
        [Column("B_IsDefault")]
        public bool? BIsDefault { get; set; }
        [Column("X_Address")]
        [StringLength(500)]
        public string XAddress { get; set; }
        [Column("X_PhoneNo")]
        [StringLength(20)]
        public string XPhoneNo { get; set; }
        [Column("B_PhysicalSite")]
        public bool? BPhysicalSite { get; set; }
    }
}
