using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwMenuDisp
    {
        [Column("Menu Id")]
        [StringLength(100)]
        public string MenuId { get; set; }
        [Column("Menu Name")]
        [StringLength(500)]
        public string MenuName { get; set; }
        [Column("Rpt File")]
        [StringLength(50)]
        public string RptFile { get; set; }
        [Column("N_LanguageId")]
        public int? NLanguageId { get; set; }
        [Column("N_ParentMenuID")]
        public int? NParentMenuId { get; set; }
    }
}
