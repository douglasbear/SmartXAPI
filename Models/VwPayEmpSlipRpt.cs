using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayEmpSlipRpt
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Column("X_EmpCode")]
        [StringLength(50)]
        public string XEmpCode { get; set; }
        [Column("X_Position")]
        [StringLength(100)]
        public string XPosition { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [StringLength(50)]
        public string Batch { get; set; }
        [Column("N_PayID")]
        public int? NPayId { get; set; }
        [Column(TypeName = "money")]
        public decimal? BasicSal { get; set; }
        [Column(TypeName = "money")]
        public decimal? Additions { get; set; }
        [Column(TypeName = "money")]
        public decimal? Deductions { get; set; }
        [Column("N_TransID")]
        public int NTransId { get; set; }
        [Column("B_Inactive")]
        public bool? BInactive { get; set; }
    }
}
