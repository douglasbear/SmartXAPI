using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwUserMenus
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("X_UserCategory")]
        [StringLength(50)]
        public string XUserCategory { get; set; }
        [Column("N_UserCategoryID")]
        public int NUserCategoryId { get; set; }
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
        [Column("B_Visible")]
        public bool? BVisible { get; set; }
        [Column("B_Edit")]
        public bool? BEdit { get; set; }
        [Column("B_Delete")]
        public bool? BDelete { get; set; }
        [Column("B_Save")]
        public bool? BSave { get; set; }
        [Column("B_View")]
        public bool? BView { get; set; }
        [Column("N_InternalID")]
        public int? NInternalId { get; set; }
        [Column("X_ShortcutKey")]
        [StringLength(50)]
        public string XShortcutKey { get; set; }
        [Column("X_CaptionAr")]
        [StringLength(50)]
        public string XCaptionAr { get; set; }
        [Column("X_FormNameWithTag")]
        [StringLength(150)]
        public string XFormNameWithTag { get; set; }
        [Column("N_IsStartup")]
        public bool? NIsStartup { get; set; }
        [Column("B_Show")]
        public bool? BShow { get; set; }
        [Column("B_ShowOnline")]
        public bool? BShowOnline { get; set; }
        [Column("X_RouteName")]
        [StringLength(150)]
        public string XRouteName { get; set; }
        [Column("B_WShow")]
        public bool? BWShow { get; set; }
    }
}
