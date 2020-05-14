using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchPromotionMaster
    {
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
        [Required]
        [Column("X_AcYear")]
        [StringLength(50)]
        public string XAcYear { get; set; }
        [Column("X_Class")]
        [StringLength(50)]
        public string XClass { get; set; }
        [Column("X_ClassType")]
        [StringLength(50)]
        public string XClassType { get; set; }
        [Required]
        [Column("X_AcYearTo")]
        [StringLength(50)]
        public string XAcYearTo { get; set; }
        [Column("X_ClassTypeTo")]
        [StringLength(50)]
        public string XClassTypeTo { get; set; }
        [Column("X_ClassTo")]
        [StringLength(50)]
        public string XClassTo { get; set; }
        [Column("X_PromotionCode")]
        [StringLength(50)]
        public string XPromotionCode { get; set; }
    }
}
