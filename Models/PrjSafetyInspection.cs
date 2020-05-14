using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Prj_SafetyInspection")]
    public partial class PrjSafetyInspection
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Key]
        [Column("N_InspectionID")]
        public int NInspectionId { get; set; }
        [Column("X_InspectionCode")]
        [StringLength(100)]
        public string XInspectionCode { get; set; }
        [Column("X_InspectionName")]
        [StringLength(100)]
        public string XInspectionName { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime DDate { get; set; }
        [Column("N_InspectorID")]
        public int? NInspectorId { get; set; }
        [Column("X_Supervisor")]
        [StringLength(100)]
        public string XSupervisor { get; set; }
    }
}
