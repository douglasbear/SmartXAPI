using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwEmployeeSalDetails
    {
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [StringLength(50)]
        public string Batch { get; set; }
        [Column("N_Basic", TypeName = "money")]
        public decimal? NBasic { get; set; }
        [Column("N_PayID")]
        public int? NPayId { get; set; }
        [Required]
        [StringLength(9)]
        public string TypeVal { get; set; }
    }
}
