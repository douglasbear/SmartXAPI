using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayEvaluationDetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_EvalID")]
        public int NEvalId { get; set; }
        [Column("X_EvalCode")]
        [StringLength(20)]
        public string XEvalCode { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("N_EvalSettingsID")]
        public int? NEvalSettingsId { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_QuestionID")]
        public int? NQuestionId { get; set; }
        [Column("N_RatingID")]
        public int? NRatingId { get; set; }
        [Column("X_Question")]
        [StringLength(200)]
        public string XQuestion { get; set; }
        [Column("N_Weightage")]
        public double? NWeightage { get; set; }
        [Column("X_Name")]
        [StringLength(50)]
        public string XName { get; set; }
        [Column("N_ReferId")]
        public int? NReferId { get; set; }
        [Column("X_Name_Ar")]
        [StringLength(250)]
        public string XNameAr { get; set; }
        [Column("X_Reference")]
        [StringLength(100)]
        public string XReference { get; set; }
    }
}
