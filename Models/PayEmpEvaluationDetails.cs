using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_EmpEvaluationDetails")]
    public partial class PayEmpEvaluationDetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_EvalID")]
        public int? NEvalId { get; set; }
        [Key]
        [Column("N_EvalDetailsID")]
        public int NEvalDetailsId { get; set; }
        [Column("N_QuestionID")]
        public int? NQuestionId { get; set; }
        [Column("N_RatingID")]
        public int? NRatingId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("X_Reference")]
        [StringLength(100)]
        public string XReference { get; set; }
    }
}
