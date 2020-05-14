using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Acc_VoucherMaster_Details_Segments")]
    public partial class AccVoucherMasterDetailsSegments
    {
        [Key]
        [Column("N_VoucherSegmentID")]
        public int NVoucherSegmentId { get; set; }
        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_VoucherID")]
        public int? NVoucherId { get; set; }
        [Column("N_VoucherDetailsID")]
        public int? NVoucherDetailsId { get; set; }
        [Column("N_Segment_1")]
        public int? NSegment1 { get; set; }
        [Column("N_Segment_2")]
        public int? NSegment2 { get; set; }
        [Column("N_Segment_3")]
        public int? NSegment3 { get; set; }
        [Column("N_Segment_4")]
        public int? NSegment4 { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("X_Naration")]
        [StringLength(250)]
        public string XNaration { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime? DEntrydate { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_Segment_5")]
        public int? NSegment5 { get; set; }
        [Column("D_RepaymentDate", TypeName = "datetime")]
        public DateTime? DRepaymentDate { get; set; }
        [Column("N_InstNo")]
        public int? NInstNo { get; set; }
        [Column("N_PayID")]
        public int? NPayId { get; set; }
    }
}
