using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Acc_MastGroup")]
    public partial class AccMastGroup
    {
        public AccMastGroup()
        {
            AccMastLedger = new HashSet<AccMastLedger>();
        }

        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_GroupID")]
        public int NGroupId { get; set; }
        [Key]
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("X_GroupCode")]
        [StringLength(50)]
        public string XGroupCode { get; set; }
        [Column("X_GroupName")]
        [StringLength(100)]
        public string XGroupName { get; set; }
        [Column("X_Type")]
        [StringLength(1)]
        public string XType { get; set; }
        [Column("N_ParentGroup")]
        public int? NParentGroup { get; set; }
        [Column("N_Reserved")]
        public bool? NReserved { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("X_Level")]
        [StringLength(100)]
        public string XLevel { get; set; }
        [Column("X_GroupName_Ar")]
        [StringLength(100)]
        public string XGroupNameAr { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }

        [ForeignKey(nameof(NCompanyId))]
        [InverseProperty(nameof(AccCompany.AccMastGroup))]
        public virtual AccCompany NCompany { get; set; }
        [InverseProperty("N")]
        public virtual ICollection<AccMastLedger> AccMastLedger { get; set; }
    }
}
