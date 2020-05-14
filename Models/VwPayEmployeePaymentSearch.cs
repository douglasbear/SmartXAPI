using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayEmployeePaymentSearch
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_ReceiptID")]
        public int NReceiptId { get; set; }
        [Column("Receipt No")]
        [StringLength(50)]
        public string ReceiptNo { get; set; }
        [Column("X_EmpCode")]
        [StringLength(50)]
        public string XEmpCode { get; set; }
        [Column("Employee Name")]
        [StringLength(100)]
        public string EmployeeName { get; set; }
        [StringLength(8000)]
        public string Date { get; set; }
        [Column("X_Remarks")]
        [StringLength(150)]
        public string XRemarks { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("D_ReceiptDate", TypeName = "datetime")]
        public DateTime? DReceiptDate { get; set; }
        [Column("N_AcYearID")]
        public int NAcYearId { get; set; }
        [Column("B_YearEndProcess")]
        public bool? BYearEndProcess { get; set; }
    }
}
