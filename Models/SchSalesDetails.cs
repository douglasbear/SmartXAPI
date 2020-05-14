using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sch_SalesDetails")]
    public partial class SchSalesDetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_SalesID")]
        public int? NSalesId { get; set; }
        [Column("N_SalesDetailsID")]
        public int NSalesDetailsId { get; set; }
        [Column("N_ClassTypeID")]
        public int NClassTypeId { get; set; }
        [Column("N_ClassID")]
        public int NClassId { get; set; }
        [Column("N_ItemID")]
        public int NItemId { get; set; }
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
        [Column("N_ReceiptID")]
        public int? NReceiptId { get; set; }
        [Column("N_FeeProcessingID")]
        public int? NFeeProcessingId { get; set; }
        [Column("N_FrequencyID")]
        public int? NFrequencyId { get; set; }
        [Column("N_BalAmount", TypeName = "money")]
        public decimal? NBalAmount { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
    }
}
