using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPaySalaryGrade
    {
        [Column("N_SalaryLevel")]
        public int? NSalaryLevel { get; set; }
        [Column("X_GradeCode")]
        [StringLength(20)]
        public string XGradeCode { get; set; }
        [Column("X_Gradename")]
        [StringLength(20)]
        public string XGradename { get; set; }
        [Column("B_Active")]
        public bool? BActive { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime? DDate { get; set; }
        [Column("X_LevelName")]
        [StringLength(50)]
        public string XLevelName { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_GradeID")]
        public int NGradeId { get; set; }
        [Column("X_InsClass")]
        [StringLength(100)]
        public string XInsClass { get; set; }
        [Column("N_InsID")]
        public int? NInsId { get; set; }
        [Column("B_Edit")]
        public bool? BEdit { get; set; }
        [Column("B_FamilyStatus")]
        public bool? BFamilyStatus { get; set; }
    }
}
