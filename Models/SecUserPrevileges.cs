using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sec_UserPrevileges")]
    public partial class SecUserPrevileges
    {
        [Column("N_InternalID")]
        public int? NInternalId { get; set; }
        [Key]
        [Column("N_UserCategoryID")]
        public int NUserCategoryId { get; set; }
        [Key]
        [Column("N_MenuID")]
        public int NMenuId { get; set; }
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
    }
}
