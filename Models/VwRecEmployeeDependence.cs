using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwRecEmployeeDependence
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_DependenceID")]
        public int NDependenceId { get; set; }
        [Column("N_RecruitmentID")]
        public int? NRecruitmentId { get; set; }
        [Column("X_DName")]
        [StringLength(100)]
        public string XDname { get; set; }
        [Column("X_Relation")]
        [StringLength(50)]
        public string XRelation { get; set; }
        [Column("N_RelationID")]
        public int? NRelationId { get; set; }
        [Column("D_DDOB")]
        [StringLength(30)]
        public string DDdob { get; set; }
        [Column("X_DPassportNo")]
        [StringLength(50)]
        public string XDpassportNo { get; set; }
        [Column("X_DIqamaNo")]
        [StringLength(50)]
        public string XDiqamaNo { get; set; }
        [Required]
        [Column("X_RecruitmentCode")]
        [StringLength(400)]
        public string XRecruitmentCode { get; set; }
        [Column("X_LicenceNo")]
        [StringLength(50)]
        public string XLicenceNo { get; set; }
        [Column("D_IssueDate")]
        [StringLength(30)]
        public string DIssueDate { get; set; }
        [Column("D_ExpiryDate")]
        [StringLength(30)]
        public string DExpiryDate { get; set; }
    }
}
