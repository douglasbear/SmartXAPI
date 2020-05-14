using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Web_Pay_EmployeeLogin")]
    public partial class WebPayEmployeeLogin
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
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
    }
}
