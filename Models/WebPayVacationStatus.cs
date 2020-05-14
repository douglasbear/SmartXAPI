using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Web_Pay_VacationStatus")]
    public partial class WebPayVacationStatus
    {
        [Column("N_VacStatus")]
        public int? NVacStatus { get; set; }
        [Column("X_VacStatusDesc")]
        [StringLength(50)]
        public string XVacStatusDesc { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
    }
}
