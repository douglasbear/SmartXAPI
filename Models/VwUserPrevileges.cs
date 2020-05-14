using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwUserPrevileges
    {
        [Column("B_Visible")]
        public bool? BVisible { get; set; }
        [Column("N_MenuID")]
        public int NMenuId { get; set; }
        [Column("N_UserCategoryID")]
        public int NUserCategoryId { get; set; }
        [Column("B_Edit")]
        public bool? BEdit { get; set; }
        [Column("B_Delete")]
        public bool? BDelete { get; set; }
        [Column("B_Save")]
        public bool? BSave { get; set; }
        [Column("B_View")]
        public bool? BView { get; set; }
        [Column("X_UserCategory")]
        [StringLength(50)]
        public string XUserCategory { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
    }
}
