using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwEmployeesBasicSalary
    {
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("N_Payrate", TypeName = "money")]
        public decimal? NPayrate { get; set; }
        [Column("N_TransDetailsID")]
        public int NTransDetailsId { get; set; }
        [Column("N_PayTypeID")]
        public int? NPayTypeId { get; set; }
    }
}
