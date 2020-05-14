using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayAccruedCodeList
    {
        [Column("N_VacTypeID")]
        public int NVacTypeId { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Date { get; set; }
        [StringLength(50)]
        public string Code { get; set; }
        [Column("X_Type")]
        [StringLength(5)]
        public string XType { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("X_Period")]
        [StringLength(50)]
        public string XPeriod { get; set; }
        [Column("N_Accrued")]
        public double? NAccrued { get; set; }
    }
}
