using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayVacationList
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_VacTypeID")]
        public int NVacTypeId { get; set; }
        [Column("X_VacCode")]
        [StringLength(50)]
        public string XVacCode { get; set; }
        [Column("X_VacType")]
        [StringLength(50)]
        public string XVacType { get; set; }
        [Column("N_Accrued")]
        public double? NAccrued { get; set; }
        [Column("B_IsReturn")]
        public bool? BIsReturn { get; set; }
        [Column("X_Type")]
        [StringLength(5)]
        public string XType { get; set; }
        [Column("N_EmpId")]
        public int NEmpId { get; set; }
    }
}
