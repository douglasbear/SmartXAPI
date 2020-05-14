using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Gen_LoginDetails")]
    public partial class GenLoginDetails
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Key]
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
        [Column("D_LoginDate", TypeName = "datetime")]
        public DateTime? DLoginDate { get; set; }
        [Column("D_LogoutDate", TypeName = "datetime")]
        public DateTime? DLogoutDate { get; set; }
        [Column("D_LoginTime")]
        [StringLength(10)]
        public string DLoginTime { get; set; }
        [Column("D_LogoutTime")]
        [StringLength(10)]
        public string DLogoutTime { get; set; }
        [Column("X_SystemName")]
        [StringLength(1000)]
        public string XSystemName { get; set; }
        [Column("N_Status")]
        public int? NStatus { get; set; }
    }
}
