using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Mnp_DeMobilization")]
    public partial class MnpDeMobilization
    {
        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
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
        [Column("N_MobilizationID")]
        public int NMobilizationId { get; set; }
        [Column("N_BranchID")]
        public int NBranchId { get; set; }
        [Column("N_UserID")]
        public int NUserId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime DEntryDate { get; set; }
        [Column("B_IsDeMobilize")]
        public bool? BIsDeMobilize { get; set; }
        [Column("N_CustomerID")]
        public int? NCustomerId { get; set; }
    }
}
