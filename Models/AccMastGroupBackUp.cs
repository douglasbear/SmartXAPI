using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Acc_MastGroup_BackUp")]
    public partial class AccMastGroupBackUp
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_GroupID")]
        public int NGroupId { get; set; }
        [Column("X_GroupCode")]
        [StringLength(10)]
        public string XGroupCode { get; set; }
        [Column("X_GroupName")]
        [StringLength(50)]
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
        [StringLength(200)]
        public string XLevel { get; set; }
    }
}
