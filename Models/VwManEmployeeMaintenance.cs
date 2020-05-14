using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwManEmployeeMaintenance
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_MaintenanceID")]
        public int NMaintenanceId { get; set; }
        [Required]
        [Column("X_MaintenanceCode")]
        [StringLength(400)]
        public string XMaintenanceCode { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("X_EmployeeCode")]
        [StringLength(400)]
        public string XEmployeeCode { get; set; }
        [Column("X_EmployeeName")]
        [StringLength(400)]
        public string XEmployeeName { get; set; }
        [Column("X_ContactNo")]
        [StringLength(20)]
        public string XContactNo { get; set; }
        [Column("X_Nationality")]
        [StringLength(100)]
        public string XNationality { get; set; }
        [Column("X_Position")]
        [StringLength(100)]
        public string XPosition { get; set; }
        [Column("X_TypeName")]
        [StringLength(50)]
        public string XTypeName { get; set; }
        [Column("N_HiredRate", TypeName = "money")]
        public decimal? NHiredRate { get; set; }
        [Column("N_SupplyRate", TypeName = "money")]
        public decimal? NSupplyRate { get; set; }
        [Column("N_OvertimeRate", TypeName = "money")]
        public decimal? NOvertimeRate { get; set; }
        [Column("N_IncentiveRate", TypeName = "money")]
        public decimal? NIncentiveRate { get; set; }
        [Column("X_IqamaNo")]
        [StringLength(400)]
        public string XIqamaNo { get; set; }
        [Column("D_Expiry", TypeName = "datetime")]
        public DateTime DExpiry { get; set; }
        [Column("N_BranchID")]
        public int NBranchId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime DEntryDate { get; set; }
        [Required]
        [Column("X_Status")]
        [StringLength(11)]
        public string XStatus { get; set; }
        [Column("X_VisaStatus")]
        [StringLength(50)]
        public string XVisaStatus { get; set; }
        [Column("B_Mobilized")]
        public bool? BMobilized { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("X_Department")]
        [StringLength(100)]
        public string XDepartment { get; set; }
    }
}
