using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwMnpMobilizationDetails
    {
        [Column("N_MobilizationID")]
        public int NMobilizationId { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Column("D_Date")]
        [StringLength(8000)]
        public string DDate { get; set; }
        [Required]
        [Column("X_MobilizationCode")]
        [StringLength(20)]
        public string XMobilizationCode { get; set; }
        [Column("X_CustomerCode")]
        [StringLength(50)]
        public string XCustomerCode { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
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
        [Column("N_MaintenanceID")]
        public int? NMaintenanceId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("X_MaintenanceCode")]
        [StringLength(400)]
        public string XMaintenanceCode { get; set; }
        [Column("X_OrderNo")]
        [StringLength(10)]
        public string XOrderNo { get; set; }
        [Column("N_CustomerID")]
        public int NCustomerId { get; set; }
        [Column("N_MobilizationDetailsID")]
        public int NMobilizationDetailsId { get; set; }
        [Column("N_HiredRate", TypeName = "money")]
        public decimal? NHiredRate { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }
        [Column("X_ProjectName")]
        [StringLength(50)]
        public string XProjectName { get; set; }
    }
}
