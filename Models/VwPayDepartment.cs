using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayDepartment
    {
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_DepartmentID")]
        public int NDepartmentId { get; set; }
        [Column("X_DepartmentCode")]
        [StringLength(50)]
        public string XDepartmentCode { get; set; }
        [Column("X_Department")]
        [StringLength(100)]
        public string XDepartment { get; set; }
        [Column("N_ExpenseDefAccountID")]
        public int? NExpenseDefAccountId { get; set; }
        [Column("N_EmployeeDefGroupID")]
        public int? NEmployeeDefGroupId { get; set; }
        [Column("N_EmployeeDefAccountID")]
        public int? NEmployeeDefAccountId { get; set; }
        [Column("X_DepartmentLocale")]
        [StringLength(100)]
        public string XDepartmentLocale { get; set; }
        [Column("N_EmpLoanDefGroupID")]
        public int? NEmpLoanDefGroupId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_EmpLoanDefAccountID")]
        public int? NEmpLoanDefAccountId { get; set; }
        [Column("N_ManagerID")]
        public int? NManagerId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_EvalSettingsID")]
        public int? NEvalSettingsId { get; set; }
        [Column("X_Name")]
        [StringLength(500)]
        public string XName { get; set; }
        [Column("X_EvaluationCode")]
        [StringLength(20)]
        public string XEvaluationCode { get; set; }
        [Column("X_CostCentreCode")]
        [StringLength(50)]
        public string XCostCentreCode { get; set; }
        [Column("X_CostcentreName")]
        [StringLength(100)]
        public string XCostcentreName { get; set; }
    }
}
