using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSrnInvDepartmentDisp
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_DepartmentID")]
        public int NDepartmentId { get; set; }
        [Column("Department Code")]
        [StringLength(50)]
        public string DepartmentCode { get; set; }
        public string Department { get; set; }
        [Column("N_CostCentreID")]
        public int? NCostCentreId { get; set; }
        [Column("Cost Center Code")]
        [StringLength(50)]
        public string CostCenterCode { get; set; }
        [Column("X_CostcentreName")]
        [StringLength(100)]
        public string XCostcentreName { get; set; }
    }
}
