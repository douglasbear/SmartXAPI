using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPrsDashboard
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Required]
        [StringLength(50)]
        public string FileNo { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_PRSID")]
        public int NPrsid { get; set; }
        [Column("N_Cost")]
        [StringLength(30)]
        public string NCost { get; set; }
        [Column(TypeName = "money")]
        public decimal? NetAmount { get; set; }
        [Column("X_PRSNo")]
        [StringLength(50)]
        public string XPrsno { get; set; }
        [Column("N_Processed")]
        public int? NProcessed { get; set; }
        [Column("D_Date")]
        [StringLength(8000)]
        public string DDate { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("X_LocationName")]
        public string XLocationName { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("X_Purpose")]
        [StringLength(50)]
        public string XPurpose { get; set; }
        [Column("X_Status")]
        [StringLength(20)]
        public string XStatus { get; set; }
        [Column("X_StatusComplete")]
        [StringLength(20)]
        public string XStatusComplete { get; set; }
        [Required]
        [Column("X_Department")]
        public string XDepartment { get; set; }
        [Column("N_POrderID")]
        public int NPorderId { get; set; }
        [Required]
        [Column("X_POrderNo")]
        [StringLength(50)]
        public string XPorderNo { get; set; }
        [Required]
        [Column("X_VendorName")]
        [StringLength(100)]
        public string XVendorName { get; set; }
        [Required]
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Column("N_MRNID")]
        public int NMrnid { get; set; }
        [Required]
        [Column("X_MRNNo")]
        [StringLength(50)]
        public string XMrnno { get; set; }
        [Required]
        [Column("X_SalesOrderNo")]
        [StringLength(50)]
        public string XSalesOrderNo { get; set; }
        [Column("N_TransTypeID")]
        public int? NTransTypeId { get; set; }
        [Column("N_SalesOrderId")]
        public int? NSalesOrderId { get; set; }
        [Required]
        [StringLength(16)]
        public string EntryScreen { get; set; }
        [StringLength(100)]
        public string CustomerName { get; set; }
        [Column("X_UserName")]
        [StringLength(60)]
        public string XUserName { get; set; }
        [Required]
        [Column("X_ProjectName")]
        [StringLength(50)]
        public string XProjectName { get; set; }
        [Column("N_userID")]
        public int? NUserId { get; set; }
    }
}
