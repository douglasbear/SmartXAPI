using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwWebPaySupervisorEmployeeList
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_VacRequestID")]
        public int NVacRequestId { get; set; }
        [Column("X_RequestNo")]
        [StringLength(50)]
        public string XRequestNo { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("D_VacReqestDate", TypeName = "datetime")]
        public DateTime? DVacReqestDate { get; set; }
        [Column("D_VacDateFrom", TypeName = "datetime")]
        public DateTime? DVacDateFrom { get; set; }
        [Column("D_VacDateTo", TypeName = "datetime")]
        public DateTime? DVacDateTo { get; set; }
        [Column("N_VacTypeID")]
        public int? NVacTypeId { get; set; }
        [Column("X_VacType")]
        [StringLength(50)]
        public string XVacType { get; set; }
        [Column("N_VacDays")]
        public int? NVacDays { get; set; }
        [Column("X_VacRemarks")]
        [StringLength(250)]
        public string XVacRemarks { get; set; }
        [Column("N_VacStatus")]
        public int? NVacStatus { get; set; }
        [Column("X_VacApprovedDate", TypeName = "datetime")]
        public DateTime? XVacApprovedDate { get; set; }
        [Column("X_VacApprovedComments")]
        [StringLength(250)]
        public string XVacApprovedComments { get; set; }
        [Column("X_EmpCode")]
        [StringLength(50)]
        public string XEmpCode { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Column("X_Sex")]
        [StringLength(50)]
        public string XSex { get; set; }
        [Column("X_Position")]
        [StringLength(100)]
        public string XPosition { get; set; }
        [Column("X_Department")]
        [StringLength(100)]
        public string XDepartment { get; set; }
        [Column("X_PassportNo")]
        [StringLength(50)]
        public string XPassportNo { get; set; }
        [Column("X_IqamaNo")]
        [StringLength(50)]
        public string XIqamaNo { get; set; }
        [Column("N_SupervisorEmpID")]
        public int? NSupervisorEmpId { get; set; }
    }
}
