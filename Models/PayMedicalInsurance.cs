using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_Medical_Insurance")]
    public partial class PayMedicalInsurance
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
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
        public DateTime DStartDate { get; set; }
        [Column("D_EndDate", TypeName = "datetime")]
        public DateTime DEndDate { get; set; }
        [Column("X_CardNo")]
        [StringLength(50)]
        public string XCardNo { get; set; }
        [Column("N_VendorID")]
        public int NVendorId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("X_PolicyDetails")]
        public string XPolicyDetails { get; set; }
        [Column("N_PaycodeID")]
        public int? NPaycodeId { get; set; }
    }
}
