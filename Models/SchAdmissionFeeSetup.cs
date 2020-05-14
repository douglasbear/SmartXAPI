using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sch_AdmissionFeeSetup")]
    public partial class SchAdmissionFeeSetup
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_AdmissionFeeSetupID")]
        public int NAdmissionFeeSetupId { get; set; }
        [Column("N_AdmissionID")]
        public int NAdmissionId { get; set; }
        [Column("N_FeeTypeID")]
        public int NFeeTypeId { get; set; }
        [Column("D_DateFrom", TypeName = "datetime")]
        public DateTime? DDateFrom { get; set; }
        [Column("D_DateTill", TypeName = "datetime")]
        public DateTime? DDateTill { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("B_Issued")]
        public bool? BIssued { get; set; }
        [Column("D_DateIssued", TypeName = "datetime")]
        public DateTime? DDateIssued { get; set; }
        [Column("B_Paid")]
        public bool? BPaid { get; set; }
        [Column("D_DatePaid", TypeName = "datetime")]
        public DateTime? DDatePaid { get; set; }
        [Column("N_AcYearID")]
        public int? NAcYearId { get; set; }
        [Column("N_ClassID")]
        public int? NClassId { get; set; }
        [Column("N_ReceiptID")]
        public int? NReceiptId { get; set; }
        [Column("N_FeeProcessingID")]
        public int? NFeeProcessingId { get; set; }
        [Column("N_ClassTypeID")]
        public int? NClassTypeId { get; set; }
        [Column("N_FrequencyID")]
        public int? NFrequencyId { get; set; }
        [Column("N_BalAmount", TypeName = "money")]
        public decimal? NBalAmount { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }

        [ForeignKey(nameof(NAdmissionId))]
        [InverseProperty(nameof(SchAdmission.SchAdmissionFeeSetup))]
        public virtual SchAdmission NAdmission { get; set; }
    }
}
