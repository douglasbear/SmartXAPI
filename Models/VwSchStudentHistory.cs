using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchStudentHistory
    {
        [Required]
        [Column("X_AcYear")]
        [StringLength(50)]
        public string XAcYear { get; set; }
        [Column("D_YearFrom", TypeName = "datetime")]
        public DateTime? DYearFrom { get; set; }
        [Column("D_YearTo", TypeName = "datetime")]
        public DateTime? DYearTo { get; set; }
        [Column("N_AdmissionID")]
        public int? NAdmissionId { get; set; }
        [Column("N_DivisionID")]
        public int? NDivisionId { get; set; }
        [Column("X_Class")]
        [StringLength(50)]
        public string XClass { get; set; }
        [Column("X_ClassType")]
        [StringLength(50)]
        public string XClassType { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_PromotionID")]
        public int NPromotionId { get; set; }
        [Column("N_AcYearID")]
        public int? NAcYearId { get; set; }
        [Column("N_AcYearIDTo")]
        public int? NAcYearIdto { get; set; }
        [Column("N_ClassTypeID")]
        public int? NClassTypeId { get; set; }
        [Column("N_ClassTypeIDTo")]
        public int? NClassTypeIdto { get; set; }
        [Column("N_ClassID")]
        public int? NClassId { get; set; }
        [Column("N_ClassIDTo")]
        public int? NClassIdto { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime? DDate { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("X_PromotionCode")]
        [StringLength(50)]
        public string XPromotionCode { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("N_Stud_HistoryID")]
        public int NStudHistoryId { get; set; }
    }
}
