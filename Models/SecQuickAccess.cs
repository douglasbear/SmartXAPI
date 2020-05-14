using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sec_QuickAccess")]
    public partial class SecQuickAccess
    {
        [Key]
        [Column("N_QAccessID")]
        public int NQaccessId { get; set; }
        [Key]
        [Column("N_UserID")]
        public int NUserId { get; set; }
        [Column("N_MenuID")]
        public int? NMenuId { get; set; }
        [Column("N_Visible")]
        public int? NVisible { get; set; }
    }
}
