using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayAddDedTimeSheet
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_SheetID")]
        public int? NSheetId { get; set; }
        [Column("D_Date", TypeName = "date")]
        public DateTime? DDate { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_TransID")]
        public int? NTransId { get; set; }
        [Column("N_PayRunID")]
        public int? NPayRunId { get; set; }
        [Column("X_Batch")]
        [StringLength(100)]
        public string XBatch { get; set; }
        [Column("D_TransDate", TypeName = "datetime")]
        public DateTime? DTransDate { get; set; }
        [StringLength(50)]
        public string PayrunText { get; set; }
        [Column("B_PostedAccount")]
        public bool? BPostedAccount { get; set; }
        [Column("D_CreatedDate", TypeName = "datetime")]
        public DateTime? DCreatedDate { get; set; }
        [Column("D_ModifiedDate", TypeName = "datetime")]
        public DateTime? DModifiedDate { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime? DEntrydate { get; set; }
        [Column("N_PayID")]
        public int? NPayId { get; set; }
        [Column("N_Payrate", TypeName = "money")]
        public decimal? NPayrate { get; set; }
        [Column("EmpID")]
        public int? EmpId { get; set; }
        [Column("N_HrsOrDays")]
        public double? NHrsOrDays { get; set; }
        [Column("B_IsApproved")]
        public bool? BIsApproved { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_EntryDate")]
        public int? NEntryDate { get; set; }
        [Column("X_PayrunText")]
        [StringLength(50)]
        public string XPayrunText { get; set; }
    }
}
