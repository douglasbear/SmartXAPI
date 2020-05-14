using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_InsuranceSettings")]
    public partial class PayInsuranceSettings
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Key]
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
    }
}
