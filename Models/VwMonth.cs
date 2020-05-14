using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwMonth
    {
        [Column("X_MonthName")]
        [StringLength(30)]
        public string XMonthName { get; set; }
        [Column("N_MonthID")]
        public long? NMonthId { get; set; }
    }
}
