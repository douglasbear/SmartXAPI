using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchReservationRpt
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_Status")]
        public int? NStatus { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("D_AssessDate", TypeName = "datetime")]
        public DateTime? DAssessDate { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Required]
        [Column("X_Name")]
        [StringLength(50)]
        public string XName { get; set; }
        [Column("N_RegID")]
        public int NRegId { get; set; }
        [Column("N_AcYearID")]
        public int NAcYearId { get; set; }
        [Required]
        [Column("X_RegNo")]
        [StringLength(25)]
        public string XRegNo { get; set; }
        [Required]
        [Column("N_PaymentID")]
        [StringLength(1)]
        public string NPaymentId { get; set; }
        [Column("X_ClassType")]
        [StringLength(50)]
        public string XClassType { get; set; }
        [Column("X_Class")]
        [StringLength(50)]
        public string XClass { get; set; }
        [Required]
        [Column("X_AdmissionNo")]
        [StringLength(1)]
        public string XAdmissionNo { get; set; }
        [Column("N_AdmissionID")]
        public int? NAdmissionId { get; set; }
        [Required]
        [Column("Res_Status")]
        [StringLength(23)]
        public string ResStatus { get; set; }
        [Column("Res_StatusID")]
        public int ResStatusId { get; set; }
        [Column("X_Email")]
        [StringLength(50)]
        public string XEmail { get; set; }
        [Column("X_Phone")]
        [StringLength(20)]
        public string XPhone { get; set; }
    }
}
