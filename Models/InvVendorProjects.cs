using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_VendorProjects")]
    public partial class InvVendorProjects
    {
        [Key]
        [Column("N_ProjectID")]
        public int NProjectId { get; set; }
        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("X_ProjectName")]
        [StringLength(100)]
        public string XProjectName { get; set; }
        [Column("X_ProjectDescription")]
        [StringLength(500)]
        public string XProjectDescription { get; set; }
        [Column("N_VendorID")]
        public int? NVendorId { get; set; }
        [Column("X_ContactPerson")]
        [StringLength(50)]
        public string XContactPerson { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime? DEntrydate { get; set; }
        [Column("N_ContractAmt", TypeName = "money")]
        public decimal? NContractAmt { get; set; }
        [Column("X_ProjectCode")]
        [StringLength(50)]
        public string XProjectCode { get; set; }
    }
}
