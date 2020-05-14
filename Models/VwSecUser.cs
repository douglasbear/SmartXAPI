using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSecUser
    {
        [Column("N_UserID")]
        public int NUserId { get; set; }
        [Column("X_UserID")]
        [StringLength(50)]
        public string XUserId { get; set; }
        [Column("X_Password")]
        [StringLength(50)]
        public string XPassword { get; set; }
        [Column("N_UserCategoryID")]
        public int? NUserCategoryId { get; set; }
        [Column("X_UserCategory")]
        [StringLength(50)]
        public string XUserCategory { get; set; }
        [Column("B_Active")]
        public bool? BActive { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_BranchID")]
        public int NBranchId { get; set; }
        [Column("User ID")]
        [StringLength(10)]
        public string UserId { get; set; }
        [Required]
        [Column("X_UserName")]
        [StringLength(60)]
        public string XUserName { get; set; }
    }
}
