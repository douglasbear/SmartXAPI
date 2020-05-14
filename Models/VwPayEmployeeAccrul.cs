using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayEmployeeAccrul
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_VacTypeID")]
        public int NVacTypeId { get; set; }
        [Column("N_EmpAccID")]
        public int NEmpAccId { get; set; }
        [Column("X_VacType")]
        [StringLength(50)]
        public string XVacType { get; set; }
        [Column("N_EmpId")]
        public int NEmpId { get; set; }
        [Column("N_Value", TypeName = "money")]
        public decimal? NValue { get; set; }
        [Column("N_Accrued")]
        public double? NAccrued { get; set; }
        [Column("N_AccruedValue")]
        public double? NAccruedValue { get; set; }
    }
}
