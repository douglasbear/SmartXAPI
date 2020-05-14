using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sch_BusStop")]
    public partial class SchBusStop
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_StopID")]
        public int NStopId { get; set; }
        [Column("X_StopCode")]
        [StringLength(20)]
        public string XStopCode { get; set; }
        [Column("X_StopName")]
        [StringLength(20)]
        public string XStopName { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
    }
}
