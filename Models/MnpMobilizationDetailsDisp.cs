using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class MnpMobilizationDetailsDisp
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_MobilizationID")]
        public int? NMobilizationId { get; set; }
        [Column("N_MobilizationDetailsID")]
        public int? NMobilizationDetailsId { get; set; }
        [Column("N_MaintenanceID")]
        public int? NMaintenanceId { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("X_CustomerCode")]
        [StringLength(50)]
        public string XCustomerCode { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Column("X_EmployeeCode")]
        [StringLength(400)]
        public string XEmployeeCode { get; set; }
        [Column("X_EmployeeName")]
        [StringLength(400)]
        public string XEmployeeName { get; set; }
        [Column("X_Position")]
        [StringLength(100)]
        public string XPosition { get; set; }
        [Column("X_Nationality")]
        [StringLength(100)]
        public string XNationality { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("X_MobilizationCode")]
        [StringLength(20)]
        public string XMobilizationCode { get; set; }
        [Column("N_HiredRate", TypeName = "money")]
        public decimal? NHiredRate { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime? DDate { get; set; }
    }
}
