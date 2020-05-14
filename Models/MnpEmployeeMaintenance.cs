using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Mnp_EmployeeMaintenance")]
    public partial class MnpEmployeeMaintenance
    {
        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
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
        [StringLength(400)]
        public string XNationality { get; set; }
        [Column("X_Trade")]
        [StringLength(20)]
        public string XTrade { get; set; }
        [Column("X_EmployeeType")]
        [StringLength(400)]
        public string XEmployeeType { get; set; }
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
        [Column("B_Mobilized")]
        public bool? BMobilized { get; set; }
        [Column("N_VisaStatus")]
        public int? NVisaStatus { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("N_ProjectId")]
        public int? NProjectId { get; set; }
    }
}
