using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvItemClass
    {
        [Column("N_ClassID")]
        public int NClassId { get; set; }
        [Column("X_ClassName")]
        [StringLength(25)]
        public string XClassName { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("X_Description")]
        [StringLength(50)]
        public string XDescription { get; set; }
        [Column("ClassID")]
        [StringLength(30)]
        public string ClassId { get; set; }
    }
}
