using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Prj_MainProject")]
    public partial class PrjMainProject
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_MainProjectID")]
        public int NMainProjectId { get; set; }
        [Column("X_MainProjectCode")]
        [StringLength(50)]
        public string XMainProjectCode { get; set; }
        [Column("X_MainProjectName")]
        [StringLength(100)]
        public string XMainProjectName { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
    }
}
