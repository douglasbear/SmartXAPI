using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwWebPaySupervisorEmail
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Column("X_EmpCode")]
        [StringLength(50)]
        public string XEmpCode { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Column("X_Sex")]
        [StringLength(50)]
        public string XSex { get; set; }
        [Column("X_PassportNo")]
        [StringLength(50)]
        public string XPassportNo { get; set; }
        [Column("X_IqamaNo")]
        [StringLength(50)]
        public string XIqamaNo { get; set; }
        [Column("N_SupervisorEmpID")]
        public int? NSupervisorEmpId { get; set; }
        [Column("X_SupervisorEmpCode")]
        [StringLength(50)]
        public string XSupervisorEmpCode { get; set; }
        [Column("X_SupervisorEmpNme")]
        [StringLength(100)]
        public string XSupervisorEmpNme { get; set; }
        [Column("X_SupervisorEmailID")]
        [StringLength(50)]
        public string XSupervisorEmailId { get; set; }
    }
}
