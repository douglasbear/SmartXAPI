using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Acc_Period")]
    public partial class AccPeriod
    {
        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_PeriodID")]
        public int NPeriodId { get; set; }
        [Key]
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("D_Start", TypeName = "datetime")]
        public DateTime? DStart { get; set; }
        [Column("D_End", TypeName = "datetime")]
        public DateTime? DEnd { get; set; }
        [Column("B_Blocked")]
        public bool? BBlocked { get; set; }
        [Column("X_Period")]
        [StringLength(100)]
        public string XPeriod { get; set; }
        [Column("X_PeriodCode")]
        [StringLength(50)]
        public string XPeriodCode { get; set; }
        [Column("N_Days")]
        public int? NDays { get; set; }

        [ForeignKey(nameof(NFnYearId))]
        [InverseProperty(nameof(AccFnYear.AccPeriod))]
        public virtual AccFnYear NFnYear { get; set; }
    }
}
