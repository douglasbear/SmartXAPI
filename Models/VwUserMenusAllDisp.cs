using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwUserMenusAllDisp
    {
        [Column("X_Text")]
        [StringLength(500)]
        public string XText { get; set; }
        [Column("N_MenuID")]
        public int NMenuId { get; set; }
        [Column("X_MenuName")]
        [StringLength(50)]
        public string XMenuName { get; set; }
        [Column("X_Caption")]
        [StringLength(50)]
        public string XCaption { get; set; }
        [Column("N_ParentMenuID")]
        public int? NParentMenuId { get; set; }
        [Column("N_Order")]
        public double? NOrder { get; set; }
        [Column("N_HasChild")]
        public bool? NHasChild { get; set; }
        [Column("X_ShortcutKey")]
        [StringLength(50)]
        public string XShortcutKey { get; set; }
        [Column("X_CaptionAr")]
        [StringLength(50)]
        public string XCaptionAr { get; set; }
        [Column("X_FormNameWithTag")]
        [StringLength(150)]
        public string XFormNameWithTag { get; set; }
        [Required]
        [Column("X_ControlNo")]
        [StringLength(50)]
        public string XControlNo { get; set; }
        [Column("N_LanguageId")]
        public int? NLanguageId { get; set; }
    }
}
