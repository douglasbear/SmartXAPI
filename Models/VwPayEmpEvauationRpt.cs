using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayEmpEvauationRpt
    {
        [Column("N_EvalID")]
        public int NEvalId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("X_EvalCode")]
        [StringLength(20)]
        public string XEvalCode { get; set; }
        [Column("X_EmpCode")]
        [StringLength(50)]
        public string XEmpCode { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("N_EvalSettingsID")]
        public int NEvalSettingsId { get; set; }
        [Column("D_EvalDate", TypeName = "datetime")]
        public DateTime? DEvalDate { get; set; }
        [Column("N_DepartmentID")]
        public int? NDepartmentId { get; set; }
        [Column("X_Question")]
        [StringLength(200)]
        public string XQuestion { get; set; }
        [StringLength(50)]
        public string Rating { get; set; }
        [Column("N_QuestionID")]
        public int? NQuestionId { get; set; }
        [Column("X_Department")]
        [StringLength(100)]
        public string XDepartment { get; set; }
        [Column("X_DepartmentCode")]
        [StringLength(50)]
        public string XDepartmentCode { get; set; }
    }
}
