using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_AutoSaveSettings")]
    public partial class InvAutoSaveSettings
    {
        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_FormID")]
        public int NFormId { get; set; }
        [Column("N_TimeSlot")]
        public int? NTimeSlot { get; set; }
        [Column("N_SavedAtInterval")]
        public int? NSavedAtInterval { get; set; }
        [Column("B_AutoSaveEnabled")]
        public bool? BAutoSaveEnabled { get; set; }
        [Column("N_MenuID")]
        public int? NMenuId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
    }
}
