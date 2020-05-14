using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayVacationRemaining
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Column("N_VacTypeID")]
        public int? NVacTypeId { get; set; }
        [Column("N_VacDays")]
        public double? NVacDays { get; set; }
    }
}
