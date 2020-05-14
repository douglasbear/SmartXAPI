using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_Position")]
    public partial class PayPosition
    {
        public PayPosition()
        {
            PayEmployee = new HashSet<PayEmployee>();
        }

        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_PositionID")]
        public int NPositionId { get; set; }
        [Column("X_PositionCode")]
        [StringLength(50)]
        public string XPositionCode { get; set; }
        [Column("X_Position")]
        [StringLength(100)]
        public string XPosition { get; set; }
        [Column("X_PositionLocale")]
        [StringLength(100)]
        public string XPositionLocale { get; set; }
        [Column("B_IsSupervisor")]
        public bool? BIsSupervisor { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_SalaryGradeID")]
        public int? NSalaryGradeId { get; set; }
        [Column("N_HigherLevelID")]
        public int? NHigherLevelId { get; set; }

        [InverseProperty("NPosition")]
        public virtual ICollection<PayEmployee> PayEmployee { get; set; }
    }
}
