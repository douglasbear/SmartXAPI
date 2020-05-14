using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sch_BookDistibution")]
    public partial class SchBookDistibution
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Key]
        [Column("N_DistibutionId")]
        public int NDistibutionId { get; set; }
        [Column("X_DistibutionNo")]
        [StringLength(50)]
        public string XDistibutionNo { get; set; }
        [Column("X_BookCde")]
        [StringLength(20)]
        public string XBookCde { get; set; }
        [Column("N_BookID")]
        public int? NBookId { get; set; }
        [Column("D_DistibutioDate", TypeName = "smalldatetime")]
        public DateTime? DDistibutioDate { get; set; }
        [Column("D_EntryDate", TypeName = "smalldatetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("N_StudentId")]
        public int? NStudentId { get; set; }
        [Column("N_BillAmt", TypeName = "money")]
        public decimal? NBillAmt { get; set; }
        [Column("N_DiscountAmt", TypeName = "money")]
        public decimal? NDiscountAmt { get; set; }
        [Column("N_CashReceived", TypeName = "money")]
        public decimal? NCashReceived { get; set; }
        [Column("x_Notes")]
        [StringLength(250)]
        public string XNotes { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("B_IsSaveDraft")]
        public bool? BIsSaveDraft { get; set; }
        [Column("N_RefForm")]
        public int? NRefForm { get; set; }
    }
}
