using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_SalaryLevel")]
    public partial class PaySalaryLevel
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_LevelID")]
        public int NLevelId { get; set; }
        [Column("X_LevelCode")]
        [StringLength(50)]
        public string XLevelCode { get; set; }
        [Column("X_LevelName")]
        [StringLength(50)]
        public string XLevelName { get; set; }
        [Column("B_Active")]
        public bool? BActive { get; set; }
    }
}
