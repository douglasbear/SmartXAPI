using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Acc_BranchMaster")]
    public partial class AccBranchMaster
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_BranchID")]
        public int NBranchId { get; set; }
        [Column("X_BranchName")]
        [StringLength(50)]
        public string XBranchName { get; set; }
        [Column("X_BranchCode")]
        [StringLength(50)]
        public string XBranchCode { get; set; }
        public bool? Active { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        public bool? IsCurrent { get; set; }
        [Column("B_ShowAllData")]
        public bool? BShowAllData { get; set; }
        [Column("B_DefaultBranch")]
        public bool? BDefaultBranch { get; set; }
        [Column("X_MapInfo")]
        [StringLength(100)]
        public string XMapInfo { get; set; }
        [Column("X_Address")]
        [StringLength(200)]
        public string XAddress { get; set; }
        [Column("X_PhoneNo")]
        [StringLength(20)]
        public string XPhoneNo { get; set; }

        [ForeignKey(nameof(NCompanyId))]
        [InverseProperty(nameof(AccCompany.AccBranchMaster))]
        public virtual AccCompany NCompany { get; set; }
    }
}
