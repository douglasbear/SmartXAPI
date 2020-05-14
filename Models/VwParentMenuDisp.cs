using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwParentMenuDisp
    {
        [Column("Menu Id")]
        [StringLength(30)]
        public string MenuId { get; set; }
        [Column("Menu Name")]
        [StringLength(500)]
        public string MenuName { get; set; }
        [Column("N_LanguageId")]
        public int? NLanguageId { get; set; }
    }
}
