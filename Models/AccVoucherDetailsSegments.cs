using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Acc_VoucherDetails_Segments")]
    public partial class AccVoucherDetailsSegments
    {
        [Column("N_SegmentsTransID")]
        public int NSegmentsTransId { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_VoucherID")]
        public int NVoucherId { get; set; }
        [Column("N_VoucherDetailsID")]
        public int NVoucherDetailsId { get; set; }
        [Column("N_Segment_1")]
        public int? NSegment1 { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("N_LedgerID")]
        public int NLedgerId { get; set; }
        [Column("N_Segment_2")]
        public int? NSegment2 { get; set; }
        [Column("X_Naration")]
        [StringLength(1000)]
        public string XNaration { get; set; }
        [Column("X_TransType")]
        [StringLength(25)]
        public string XTransType { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_Segment_3")]
        public int? NSegment3 { get; set; }
        [Column("N_Segment_4")]
        public int? NSegment4 { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_AccTransID")]
        public int? NAccTransId { get; set; }
        [Column("N_LineNo")]
        public int? NLineNo { get; set; }

        [ForeignKey("NCompanyId,NLedgerId,NFnYearId")]
        public virtual AccMastLedger N { get; set; }
    }
}
