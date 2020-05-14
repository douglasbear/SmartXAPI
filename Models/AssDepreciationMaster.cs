using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Ass_DepreciationMaster")]
    public partial class AssDepreciationMaster
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Key]
        [Column("N_DeprID")]
        public int NDeprId { get; set; }
        [Column("X_DepriciationNo")]
        [StringLength(50)]
        public string XDepriciationNo { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("D_RunDate", TypeName = "datetime")]
        public DateTime? DRunDate { get; set; }
    }
}
