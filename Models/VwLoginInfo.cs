using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwLoginInfo
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("N_LoginID")]
        public int NLoginId { get; set; }
        [Column("N_UserId")]
        public int? NUserId { get; set; }
        [Column("X_UserID")]
        [StringLength(100)]
        public string XUserId { get; set; }
        [Column("X_UserName")]
        [StringLength(1000)]
        public string XUserName { get; set; }
        [Column("D_LoginDate")]
        [StringLength(30)]
        public string DLoginDate { get; set; }
        [Column("D_LogoutDate")]
        [StringLength(30)]
        public string DLogoutDate { get; set; }
        [Column("D_LoginTime")]
        [StringLength(30)]
        public string DLoginTime { get; set; }
        [Column("D_LogoutTime")]
        [StringLength(30)]
        public string DLogoutTime { get; set; }
        [Column("X_SystemName")]
        [StringLength(1000)]
        public string XSystemName { get; set; }
        [Column("N_Status")]
        public int? NStatus { get; set; }
        [Column("X_UserCategory")]
        [StringLength(50)]
        public string XUserCategory { get; set; }
        [Required]
        [Column("X_Status")]
        [StringLength(8)]
        public string XStatus { get; set; }
    }
}
