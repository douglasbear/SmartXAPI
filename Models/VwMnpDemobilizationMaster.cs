using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwMnpDemobilizationMaster
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
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Column("X_MobilizationCode")]
        [StringLength(20)]
        public string XMobilizationCode { get; set; }
        [Column("X_CustomerCode")]
        [StringLength(50)]
        public string XCustomerCode { get; set; }
        [Column("N_CustomerID")]
        public int? NCustomerId { get; set; }
        [Column("N_MobilizationID")]
        public int NMobilizationId { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }
        [Column("X_ProjectName")]
        [StringLength(50)]
        public string XProjectName { get; set; }
    }
}
