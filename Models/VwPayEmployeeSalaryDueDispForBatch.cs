using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayEmployeeSalaryDueDispForBatch
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Column("Employee Code")]
        [StringLength(50)]
        public string EmployeeCode { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
        [Column("N_PositionID")]
        public int? NPositionId { get; set; }
        [Column("N_DepartmentID")]
        public int? NDepartmentId { get; set; }
        [Column("N_ReportToID")]
        public int? NReportToId { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("B_Inactive")]
        public bool? BInactive { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_InvoiceDueAmt", TypeName = "money")]
        public decimal NInvoiceDueAmt { get; set; }
        [Column("N_BranchID")]
        public int NBranchId { get; set; }
        [Column("N_TransID")]
        public int NTransId { get; set; }
    }
}
