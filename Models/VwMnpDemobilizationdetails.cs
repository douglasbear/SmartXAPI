using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwMnpDemobilizationdetails
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_DeMobilizationID")]
        public int NDeMobilizationId { get; set; }
        [Required]
        [Column("X_DeMobilizationCode")]
        [StringLength(20)]
        public string XDeMobilizationCode { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime DDate { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("X_EmployeeCode")]
        [StringLength(400)]
        public string XEmployeeCode { get; set; }
        [Column("X_EmployeeName")]
        [StringLength(400)]
        public string XEmployeeName { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Required]
        [Column("X_MobilizationCode")]
        [StringLength(20)]
        public string XMobilizationCode { get; set; }
        [Column("X_Position")]
        [StringLength(100)]
        public string XPosition { get; set; }
        [Column("X_Nationality")]
        [StringLength(100)]
        public string XNationality { get; set; }
        [Column("X_CustomerCode")]
        [StringLength(50)]
        public string XCustomerCode { get; set; }
        [Column("N_HiredRate", TypeName = "money")]
        public decimal? NHiredRate { get; set; }
        [Column("N_MobilizationID")]
        public int NMobilizationId { get; set; }
        [Column("N_MaintenanceID")]
        public int NMaintenanceId { get; set; }
    }
}
