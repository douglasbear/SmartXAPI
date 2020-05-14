using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchFeeAdvanceRpt
    {
        [Required]
        [Column("X_AdmissionNo")]
        [StringLength(25)]
        public string XAdmissionNo { get; set; }
        [Column("X_Name")]
        [StringLength(200)]
        public string XName { get; set; }
        [Column("N_AdvanceID")]
        public int NAdvanceId { get; set; }
        [Required]
        [Column("X_AdvanceNo")]
        [StringLength(50)]
        public string XAdvanceNo { get; set; }
        [Column("N_AdmissionID")]
        public int NAdmissionId { get; set; }
        [Column("X_Description")]
        [StringLength(1000)]
        public string XDescription { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_UserId")]
        public int? NUserId { get; set; }
        [Column("X_ClassType")]
        [StringLength(50)]
        public string XClassType { get; set; }
        [Column("N_ClassID")]
        public int NClassId { get; set; }
        [Column("N_ClassTypeID")]
        public int? NClassTypeId { get; set; }
        [Column("X_Class")]
        [StringLength(50)]
        public string XClass { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime DDate { get; set; }
        [Column("X_Paymentmethod")]
        [StringLength(50)]
        public string XPaymentmethod { get; set; }
    }
}
