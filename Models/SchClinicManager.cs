using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sch_ClinicManager")]
    public partial class SchClinicManager
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Key]
        [Column("N_ClinicManagerID")]
        public int NClinicManagerId { get; set; }
        [Column("X_ClinicCode")]
        [StringLength(50)]
        public string XClinicCode { get; set; }
        [Column("N_AdmissionID")]
        public int NAdmissionId { get; set; }
        [Column("N_BloodGroupID")]
        public int? NBloodGroupId { get; set; }
        [Column("X_HCNo")]
        [StringLength(50)]
        public string XHcno { get; set; }
        [Column("X_HCLocation")]
        [StringLength(50)]
        public string XHclocation { get; set; }
        [Column("D_HCExpiryDate", TypeName = "datetime")]
        public DateTime? DHcexpiryDate { get; set; }
        [Column("X_HospitalReason")]
        [StringLength(250)]
        public string XHospitalReason { get; set; }
        [Column("D_HospitalReasonDate", TypeName = "datetime")]
        public DateTime? DHospitalReasonDate { get; set; }
        [Column("X_SurgeryOperation")]
        [StringLength(250)]
        public string XSurgeryOperation { get; set; }
        [Column("D_SurgeryOperationDate", TypeName = "datetime")]
        public DateTime? DSurgeryOperationDate { get; set; }
        [Column("X_OtherDiseases")]
        [StringLength(500)]
        public string XOtherDiseases { get; set; }
        [Column("N_Height")]
        public double? NHeight { get; set; }
        [Column("N_Weight")]
        public double? NWeight { get; set; }
        [Column("N_BMI")]
        public double? NBmi { get; set; }
        [Column("X_Interpretation")]
        [StringLength(250)]
        public string XInterpretation { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
    }
}
