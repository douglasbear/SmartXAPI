using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwWebPayEmployeeUserProfile
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Column("X_UserName")]
        [StringLength(25)]
        public string XUserName { get; set; }
        [Column("X_Password")]
        [StringLength(15)]
        public string XPassword { get; set; }
        [Column("D_PwdExpireDate", TypeName = "datetime")]
        public DateTime? DPwdExpireDate { get; set; }
        [Column("B_Active")]
        public bool? BActive { get; set; }
        [Column("X_EmpCode")]
        [StringLength(50)]
        public string XEmpCode { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Column("X_EmailID")]
        [StringLength(50)]
        public string XEmailId { get; set; }
    }
}
