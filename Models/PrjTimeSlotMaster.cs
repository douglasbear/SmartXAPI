using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("prj_TimeSlotMaster")]
    public partial class PrjTimeSlotMaster
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_TimeSlotID")]
        public int NTimeSlotId { get; set; }
        [Column("X_TimeSlot")]
        [StringLength(50)]
        public string XTimeSlot { get; set; }
        [Column("N_Hours")]
        public double? NHours { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
    }
}
