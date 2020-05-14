using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_SRN")]
    public partial class InvSrn
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Key]
        [Column("N_SRNID")]
        public int NSrnid { get; set; }
        [Column("X_SRNNo")]
        [StringLength(50)]
        public string XSrnno { get; set; }
        [Column("D_SRNDate", TypeName = "datetime")]
        public DateTime? DSrndate { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("X_Reason")]
        public string XReason { get; set; }
        [Column("N_DepartmentID")]
        public int? NDepartmentId { get; set; }
    }
}
