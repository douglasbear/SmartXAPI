using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwEmpMedicalInsuranceDetails
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_MedicalInsID")]
        public int NMedicalInsId { get; set; }
        [Required]
        [Column("X_InsuranceCode")]
        [StringLength(50)]
        public string XInsuranceCode { get; set; }
        [Required]
        [Column("X_InsuranceName")]
        [StringLength(100)]
        public string XInsuranceName { get; set; }
        [Column("D_StartDate", TypeName = "datetime")]
        public DateTime? DStartDate { get; set; }
        [Column("D_EndDate", TypeName = "datetime")]
        public DateTime DEndDate { get; set; }
        [Column("N_PaycodeID")]
        public int? NPaycodeId { get; set; }
        [Column("N_FnYearId")]
        public int NFnYearId { get; set; }
        [Column("N_AdditionId")]
        public int NAdditionId { get; set; }
        [Column("N_Userid")]
        public int? NUserid { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Column("N_DependenceID")]
        public int NDependenceId { get; set; }
    }
}
