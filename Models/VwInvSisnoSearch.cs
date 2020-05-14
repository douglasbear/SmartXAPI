using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvSisnoSearch
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("N_TransferId")]
        public int NTransferId { get; set; }
        [Column("SIS No")]
        [StringLength(50)]
        public string SisNo { get; set; }
        [Column("X_DepartmentCode")]
        [StringLength(50)]
        public string XDepartmentCode { get; set; }
        [Column("X_Department")]
        public string XDepartment { get; set; }
        [Column("X_CostCentreCode")]
        [StringLength(50)]
        public string XCostCentreCode { get; set; }
        [Column("N_DepartmentID")]
        public int? NDepartmentId { get; set; }
        [Column("RS No")]
        [StringLength(50)]
        public string RsNo { get; set; }
        [Column("N_PRSID")]
        public int? NPrsid { get; set; }
        [Column("D_SISDate")]
        [StringLength(8000)]
        public string DSisdate { get; set; }
        [Column("D_InvoiceDate")]
        [StringLength(8000)]
        public string DInvoiceDate { get; set; }
        [Column("B_YearEndProcess")]
        public bool? BYearEndProcess { get; set; }
        [Column("X_Purpose")]
        [StringLength(50)]
        public string XPurpose { get; set; }
    }
}
