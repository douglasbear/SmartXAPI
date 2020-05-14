using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayDepartmentAccountsOld
    {
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
        [Column("X_EmployeeLoanDefaultAccountCode")]
        [StringLength(50)]
        public string XEmployeeLoanDefaultAccountCode { get; set; }
        [Column("X_LedgerName")]
        [StringLength(100)]
        public string XLedgerName { get; set; }
        [Column("X_EmployeeDefGroupName_Ar")]
        [StringLength(50)]
        public string XEmployeeDefGroupNameAr { get; set; }
        [Column("X_EmployeeDefGroupCode")]
        [StringLength(50)]
        public string XEmployeeDefGroupCode { get; set; }
        [Column("X_EmployeeDefGroupName")]
        [StringLength(100)]
        public string XEmployeeDefGroupName { get; set; }
        [Column("X_LedgerName_Ar")]
        [StringLength(50)]
        public string XLedgerNameAr { get; set; }
        [Column("N_EmployeeDefGroupID")]
        public int? NEmployeeDefGroupId { get; set; }
        [Column("N_EmployeeDefAccountID")]
        public int? NEmployeeDefAccountId { get; set; }
        [Column("X_EmployeeDefAccountCode")]
        [StringLength(50)]
        public string XEmployeeDefAccountCode { get; set; }
        [Column("X_EmployeeDefAccountName")]
        [StringLength(100)]
        public string XEmployeeDefAccountName { get; set; }
        [Column("X_EmployeeDefAccountName_Ar")]
        [StringLength(50)]
        public string XEmployeeDefAccountNameAr { get; set; }
        [Column("N_EmpLoanDefGroupID")]
        public int? NEmpLoanDefGroupId { get; set; }
        [Column("X_EmpLoanDefGroupCode")]
        [StringLength(50)]
        public string XEmpLoanDefGroupCode { get; set; }
        [Column("X_EmpLoanDefGroupName")]
        [StringLength(100)]
        public string XEmpLoanDefGroupName { get; set; }
        [Column("X_EmpLoanDefGroupName_Ar")]
        [StringLength(50)]
        public string XEmpLoanDefGroupNameAr { get; set; }
        [Column("N_EmpLoanDefAccountID")]
        public int? NEmpLoanDefAccountId { get; set; }
    }
}
