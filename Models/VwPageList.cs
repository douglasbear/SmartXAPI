using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPageList
    {
        [Column("X_Text")]
        [StringLength(1000)]
        public string XText { get; set; }
        [Column("N_LanguageId")]
        public int? NLanguageId { get; set; }
        [Column("N_MenuID")]
        public int NMenuId { get; set; }
        [Column("X_FormNameWithTag")]
        [StringLength(150)]
        public string XFormNameWithTag { get; set; }
    }
}
