using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_MedicalInsuranceAddition")]
    public partial class PayMedicalInsuranceAddition
    {
        [Column("N_CompanyId")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int NFnYearId { get; set; }
        [Key]
        [Column("N_AdditionId")]
        public int NAdditionId { get; set; }
        [Column("X_PolicyCode")]
        [StringLength(500)]
        public string XPolicyCode { get; set; }
        [Column("X_PolicyNo")]
        [StringLength(500)]
        public string XPolicyNo { get; set; }
        [Column("N_MedicalInsID")]
        public int NMedicalInsId { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime? DDate { get; set; }
        [Column("N_VendorID")]
        public int? NVendorId { get; set; }
        [Column("N_Userid")]
        public int? NUserid { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }
    }
}
