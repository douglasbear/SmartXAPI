using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Ass_Depreciation")]
    public partial class AssDepreciation
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_DeprID")]
        public int NDeprId { get; set; }
        [Column("X_DepriciationNo")]
        [StringLength(50)]
        public string XDepriciationNo { get; set; }
        [Column("X_ItemCode")]
        [StringLength(50)]
        public string XItemCode { get; set; }
        [Column("D_StartDate", TypeName = "datetime")]
        public DateTime? DStartDate { get; set; }
        [Column("D_EndDate", TypeName = "datetime")]
        public DateTime? DEndDate { get; set; }
        [Column("D_RunDate", TypeName = "datetime")]
        public DateTime? DRunDate { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("N_SuspendID")]
        public int? NSuspendId { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
    }
}
