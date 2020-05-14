using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwAccProjectDetailsExpense
    {
        [Column("N_CategoryID")]
        public int NCategoryId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime? DDate { get; set; }
        [StringLength(100)]
        public string DocNo { get; set; }
        [StringLength(50)]
        public string DocName { get; set; }
        [Column("N_LedgerID")]
        public int? NLedgerId { get; set; }
        [Column("X_Remarks")]
        [StringLength(1000)]
        public string XRemarks { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }
        [Column("X_ProjectCode")]
        [StringLength(100)]
        public string XProjectCode { get; set; }
        [Column("X_ProjectName")]
        [StringLength(50)]
        public string XProjectName { get; set; }
        [Column("N_Amount")]
        public double? NAmount { get; set; }
        [Required]
        [Column("N_Days")]
        [StringLength(1)]
        public string NDays { get; set; }
        [Required]
        [Column("X_EmployeeName")]
        [StringLength(1)]
        public string XEmployeeName { get; set; }
        [Column("N_MainProjectID")]
        public int NMainProjectId { get; set; }
    }
}
