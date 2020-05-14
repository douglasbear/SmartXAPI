using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayEmpHistoryRpt
    {
        [Column("N_Salary", TypeName = "money")]
        public decimal? NSalary { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Column("X_EmpCode")]
        [StringLength(50)]
        public string XEmpCode { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Column("X_Nationality")]
        [StringLength(100)]
        public string XNationality { get; set; }
        [Column("X_Position")]
        [StringLength(100)]
        public string XPosition { get; set; }
        [Column("N_DepartmentID")]
        public int? NDepartmentId { get; set; }
        [Column("D_HireDate", TypeName = "datetime")]
        public DateTime? DHireDate { get; set; }
        [Column("X_Address")]
        [StringLength(250)]
        public string XAddress { get; set; }
        [Column("X_City")]
        [StringLength(50)]
        public string XCity { get; set; }
        [Column("X_IqamaNo")]
        [StringLength(50)]
        public string XIqamaNo { get; set; }
        [Column("X_BankName")]
        [StringLength(100)]
        public string XBankName { get; set; }
        [Column("X_DepartmentCode")]
        [StringLength(50)]
        public string XDepartmentCode { get; set; }
        [Column("X_Department")]
        [StringLength(100)]
        public string XDepartment { get; set; }
        [Column("N_BranchID")]
        public int NBranchId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
    }
}
