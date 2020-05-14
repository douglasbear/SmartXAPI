using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Dtos
{    public partial class UserReadDto
    {
        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
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
        [Column("B_Active")]
        public bool? BActive { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("X_UserName")]
        [StringLength(60)]
        public string XUserName { get; set; }
        [Column("D_ExpireDate", TypeName = "date")]
        public DateTime? DExpireDate { get; set; }
        [Column("N_TerminalID")]
        public int? NTerminalId { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("N_LoginFlag")]
        public int? NLoginFlag { get; set; }
        [Column("N_CustomerID")]
        public int? NCustomerId { get; set; }
    }
}
