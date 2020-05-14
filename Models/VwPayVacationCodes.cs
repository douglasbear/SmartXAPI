using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayVacationCodes
    {
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Column("N_VacTypeID")]
        public int? NVacTypeId { get; set; }
        [Column("N_VacDays")]
        public double? NVacDays { get; set; }
        [Column("X_VacCode")]
        [StringLength(50)]
        public string XVacCode { get; set; }
        [Column("X_VacType")]
        [StringLength(50)]
        public string XVacType { get; set; }
        [Column("N_Accrued")]
        public double? NAccrued { get; set; }
    }
}
