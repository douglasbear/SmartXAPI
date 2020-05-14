using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("vsa_CV_Registration")]
    public partial class VsaCvRegistration
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime DDate { get; set; }
        [Key]
        [Column("N_RegId")]
        public int NRegId { get; set; }
        [Column("X_RegCode")]
        [StringLength(50)]
        public string XRegCode { get; set; }
        [Column("X_Place")]
        [StringLength(50)]
        public string XPlace { get; set; }
        [Required]
        [Column("X_Name")]
        [StringLength(50)]
        public string XName { get; set; }
        [Column("X_mobileNo")]
        [StringLength(20)]
        public string XMobileNo { get; set; }
        [Column("N_contryid")]
        public int? NContryid { get; set; }
        [Column("N_Nationality")]
        public int? NNationality { get; set; }
        [Required]
        [Column("X_PassportNo")]
        [StringLength(20)]
        public string XPassportNo { get; set; }
        [Column("X_Qualification")]
        [StringLength(100)]
        public string XQualification { get; set; }
        [Column("X_University")]
        [StringLength(100)]
        public string XUniversity { get; set; }
        [Column("X_Profession")]
        [StringLength(100)]
        public string XProfession { get; set; }
        [Column("D_Dob", TypeName = "smalldatetime")]
        public DateTime DDob { get; set; }
        [Column("X_Email")]
        [StringLength(100)]
        public string XEmail { get; set; }
        [Column("X_Address")]
        [StringLength(200)]
        public string XAddress { get; set; }
        [Column("X_SpouseName")]
        [StringLength(30)]
        public string XSpouseName { get; set; }
        [Column("D_SpouseDob", TypeName = "smalldatetime")]
        public DateTime? DSpouseDob { get; set; }
        [Column("X_SpouseQualification")]
        [StringLength(200)]
        public string XSpouseQualification { get; set; }
        [Column("X_SpouseUniversity")]
        [StringLength(200)]
        public string XSpouseUniversity { get; set; }
        [Column("X_SpouseProfession")]
        [StringLength(200)]
        public string XSpouseProfession { get; set; }
        [Column("X_StatusDescription")]
        [StringLength(500)]
        public string XStatusDescription { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_UserId")]
        public int? NUserId { get; set; }
        [Column("N_WebId")]
        public int? NWebId { get; set; }
    }
}
