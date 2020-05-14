using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchReservationPaymentRpt
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_Status")]
        public int? NStatus { get; set; }
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
        [Column("X_ClassType")]
        [StringLength(50)]
        public string XClassType { get; set; }
        [Column("X_Class")]
        [StringLength(50)]
        public string XClass { get; set; }
        [Column("D_TransDate", TypeName = "datetime")]
        public DateTime? DTransDate { get; set; }
        [Column("N_SessionID")]
        public int? NSessionId { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("B_IsNewStudent")]
        public bool? BIsNewStudent { get; set; }
        [Column("N_CFID")]
        public int? NCfid { get; set; }
        [Required]
        [Column("Std_Status")]
        [StringLength(3)]
        public string StdStatus { get; set; }
        [Column("N_AdmissionID")]
        public int? NAdmissionId { get; set; }
    }
}
