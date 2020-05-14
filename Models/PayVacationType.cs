﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_VacationType")]
    public partial class PayVacationType
    {
        public PayVacationType()
        {
            PayEmpAccruls = new HashSet<PayEmpAccruls>();
        }

        [Key]
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
        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("B_IsReturn")]
        public bool? BIsReturn { get; set; }
        [Column("N_MinAccrued")]
        public double? NMinAccrued { get; set; }
        [Column("B_HolidayFlag")]
        public bool? BHolidayFlag { get; set; }
        [Column("B_DeductSalary")]
        public bool? BDeductSalary { get; set; }
        [Column("N_AccruelID")]
        public int? NAccruelId { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }

        [InverseProperty("N")]
        public virtual ICollection<PayEmpAccruls> PayEmpAccruls { get; set; }
    }
}
