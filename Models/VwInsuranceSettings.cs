using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInsuranceSettings
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_InsuranceSettingsID")]
        public int NInsuranceSettingsId { get; set; }
        [Required]
        [Column("X_InsuranceSettingsCode")]
        [StringLength(50)]
        public string XInsuranceSettingsCode { get; set; }
        [Column("X_InsuranceSettingsDesc")]
        [StringLength(200)]
        public string XInsuranceSettingsDesc { get; set; }
        [Column("N_InsuranceClassID")]
        public int NInsuranceClassId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_EntryUserID")]
        public int? NEntryUserId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_InsuranceID")]
        public int? NInsuranceId { get; set; }
        [Column("N_InsuranceSettingsDetailsID")]
        public int? NInsuranceSettingsDetailsId { get; set; }
        [Column("N_Cost", TypeName = "money")]
        public decimal? NCost { get; set; }
        [Column("N_Price", TypeName = "money")]
        public decimal? NPrice { get; set; }
        [StringLength(50)]
        public string InsuranceClass { get; set; }
        [Column("CategoryID")]
        public int? CategoryId { get; set; }
        [StringLength(50)]
        public string CategoryCode { get; set; }
        [StringLength(150)]
        public string CategoryName { get; set; }
        [Column("X_InsuranceCode")]
        [StringLength(50)]
        public string XInsuranceCode { get; set; }
        [Column("X_InsuranceName")]
        [StringLength(100)]
        public string XInsuranceName { get; set; }
        [Column("X_CardNo")]
        [StringLength(50)]
        public string XCardNo { get; set; }
        [Required]
        [StringLength(1)]
        public string Agent { get; set; }
        [Column("D_StartDate", TypeName = "datetime")]
        public DateTime? DStartDate { get; set; }
        [Column("D_EndDate", TypeName = "datetime")]
        public DateTime? DEndDate { get; set; }
    }
}
