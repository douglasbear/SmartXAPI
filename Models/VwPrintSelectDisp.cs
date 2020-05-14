using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPrintSelectDisp
    {
        [Column("N_LanguageId")]
        public int? NLanguageId { get; set; }
        [Column("N_UserCategoryID")]
        public int NUserCategoryId { get; set; }
        [Column("N_MenuID")]
        public int NMenuId { get; set; }
        [Column("B_Visible")]
        public bool? BVisible { get; set; }
        [Column("X_Text")]
        [StringLength(1000)]
        public string XText { get; set; }
        [Required]
        [Column("X_ControlNo")]
        [StringLength(50)]
        public string XControlNo { get; set; }
    }
}
