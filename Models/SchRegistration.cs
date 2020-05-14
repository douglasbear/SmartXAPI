using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sch_Registration")]
    public partial class SchRegistration
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_AcYearID")]
        public int NAcYearId { get; set; }
        [Key]
        [Column("N_RegID")]
        public int NRegId { get; set; }
        [Required]
        [Column("X_RegNo")]
        [StringLength(25)]
        public string XRegNo { get; set; }
        [Required]
        [Column("X_Name")]
        [StringLength(50)]
        public string XName { get; set; }
        [Column("X_Email")]
        [StringLength(50)]
        public string XEmail { get; set; }
        [Column("N_ClassID")]
        public int NClassId { get; set; }
        [Column("X_Remarks")]
        [StringLength(500)]
        public string XRemarks { get; set; }
        [Column("N_ClassTypeID")]
        public int? NClassTypeId { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("X_Name_Ar")]
        [StringLength(250)]
        public string XNameAr { get; set; }
        [Column("X_MiddleName")]
        [StringLength(50)]
        public string XMiddleName { get; set; }
        [Column("X_LastName")]
        [StringLength(50)]
        public string XLastName { get; set; }
        [Column("X_Initial")]
        [StringLength(50)]
        public string XInitial { get; set; }
        [Column("X_GivenName")]
        [StringLength(50)]
        public string XGivenName { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("D_AssessDate", TypeName = "datetime")]
        public DateTime? DAssessDate { get; set; }
        [Column("D_ReserveDate", TypeName = "datetime")]
        public DateTime? DReserveDate { get; set; }
        [Column("D_TransDate", TypeName = "datetime")]
        public DateTime? DTransDate { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_QatarID")]
        [StringLength(25)]
        public string NQatarId { get; set; }
        [Column("X_Address")]
        [StringLength(100)]
        public string XAddress { get; set; }
        [Column("X_Phone")]
        [StringLength(20)]
        public string XPhone { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_Status")]
        public int? NStatus { get; set; }
        [Column("N_SessionID")]
        public int? NSessionId { get; set; }
        [Column("N_CFID")]
        public int? NCfid { get; set; }
        [Column("N_ClassDivisionID")]
        public int? NClassDivisionId { get; set; }
        [Column("N_AdmissionID")]
        public int? NAdmissionId { get; set; }
        [Column("B_IsNewStudent")]
        public bool? BIsNewStudent { get; set; }
    }
}
