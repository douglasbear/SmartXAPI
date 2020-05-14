using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sch_PromotionMaster")]
    public partial class SchPromotionMaster
    {
        public SchPromotionMaster()
        {
            SchStudentHistory = new HashSet<SchStudentHistory>();
        }

        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
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

        [InverseProperty("NPromotion")]
        public virtual ICollection<SchStudentHistory> SchStudentHistory { get; set; }
    }
}
