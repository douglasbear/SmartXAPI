using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_Department")]
    public partial class PayDepartment
    {
        public PayDepartment()
        {
            PayEmployee = new HashSet<PayEmployee>();
        }

        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
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
        [Key]
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_ManagerID")]
        public int? NManagerId { get; set; }
        [Column("N_EvalSettingsID")]
        public int? NEvalSettingsId { get; set; }
        [Column("N_CostCentreID")]
        public int? NCostCentreId { get; set; }

        [InverseProperty("N")]
        public virtual ICollection<PayEmployee> PayEmployee { get; set; }
    }
}
