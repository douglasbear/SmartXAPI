using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvDeliveryDetailsPrs
    {
        [Column("X_PRSNo")]
        [StringLength(50)]
        public string XPrsno { get; set; }
        [Column("D_PRSDate", TypeName = "datetime")]
        public DateTime? DPrsdate { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("X_Location")]
        [StringLength(50)]
        public string XLocation { get; set; }
        [Column("N_TransTypeID")]
        public int? NTransTypeId { get; set; }
        [Column("N_TotalPRSAmt", TypeName = "money")]
        public decimal? NTotalPrsamt { get; set; }
        [Column("N_Processed")]
        public int? NProcessed { get; set; }
        [Column("N_Type")]
        public int? NType { get; set; }
        [Column("X_Purpose")]
        [StringLength(50)]
        public string XPurpose { get; set; }
        [Column("N_SalesOrderId")]
        public int? NSalesOrderId { get; set; }
        [Column("N_userID")]
        public int? NUserId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("X_LocationCode")]
        public string XLocationCode { get; set; }
        [Column("N_PRSID")]
        public int NPrsid { get; set; }
        [Column("X_CustPONo")]
        [StringLength(50)]
        public string XCustPono { get; set; }
        [Column("N_DepartmentID")]
        public int? NDepartmentId { get; set; }
        [Column("N_Projectid")]
        public int? NProjectid { get; set; }
        [Column("N_TaxAmt", TypeName = "money")]
        public decimal? NTaxAmt { get; set; }
        [Column("X_ProjectName")]
        [StringLength(50)]
        public string XProjectName { get; set; }
        [Column("X_ProjectCode")]
        [StringLength(100)]
        public string XProjectCode { get; set; }
    }
}
