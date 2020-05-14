using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayVacationTypeDisplay
    {
        [Column("N_VacTypeID")]
        public int NVacTypeId { get; set; }
        [Column("X_VacType")]
        [StringLength(50)]
        public string XVacType { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_StartType")]
        public int? NStartType { get; set; }
        [Column("X_Period")]
        [StringLength(10)]
        public string XPeriod { get; set; }
        [Column("N_Accrued")]
        public double? NAccrued { get; set; }
        [Column("N_MaxAccrued")]
        public double? NMaxAccrued { get; set; }
        [Column("N_CarryFlag")]
        public int? NCarryFlag { get; set; }
        [Column("N_ProportionalFlag")]
        public int? NProportionalFlag { get; set; }
        [Column("X_VacCode")]
        [StringLength(50)]
        public string XVacCode { get; set; }
        [Column("X_Type")]
        [StringLength(5)]
        public string XType { get; set; }
        [Column("N_PayID")]
        public int? NPayId { get; set; }
        [Column("N_PerPayMethod")]
        public int? NPerPayMethod { get; set; }
        [Column("X_PayType")]
        [StringLength(100)]
        public string XPayType { get; set; }
        [Column("X_TypeName")]
        [StringLength(50)]
        public string XTypeName { get; set; }
        [Column("N_DefaultId")]
        public int? NDefaultId { get; set; }
        [Column("X_TypeCode")]
        [StringLength(5)]
        public string XTypeCode { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
    }
}
