using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_HealthSafety")]
    public partial class PayHealthSafety
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_ReferenceID")]
        public int NReferenceId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("X_ReferenceCode")]
        [StringLength(50)]
        public string XReferenceCode { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Column("X_Location")]
        [StringLength(200)]
        public string XLocation { get; set; }
        [Column("X_InsPolicy")]
        [StringLength(200)]
        public string XInsPolicy { get; set; }
        [Column("X_Remarks")]
        [StringLength(500)]
        public string XRemarks { get; set; }
        [Column("N_BranchID")]
        public int NBranchId { get; set; }
        [Column("N_UserID")]
        public int NUserId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("D_AccidentDate", TypeName = "datetime")]
        public DateTime? DAccidentDate { get; set; }
        [Column("X_Situation")]
        public string XSituation { get; set; }
        [Column("X_Witness")]
        [StringLength(500)]
        public string XWitness { get; set; }
        [Column("X_Coverage")]
        [StringLength(500)]
        public string XCoverage { get; set; }
    }
}
