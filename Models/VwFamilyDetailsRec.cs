using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwFamilyDetailsRec
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
        [Column("N_RelationID")]
        public int? NRelationId { get; set; }
        [Column("D_DDOB", TypeName = "datetime")]
        public DateTime? DDdob { get; set; }
        [Column("X_DPassportNo")]
        [StringLength(50)]
        public string XDpassportNo { get; set; }
        [Column("D_DPassportExpiry", TypeName = "datetime")]
        public DateTime? DDpassportExpiry { get; set; }
        [Column("X_DIqamaNo")]
        [StringLength(50)]
        public string XDiqamaNo { get; set; }
        [Column("D_DIqamaExpiry", TypeName = "datetime")]
        public DateTime? DDiqamaExpiry { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("X_DLName")]
        [StringLength(100)]
        public string XDlname { get; set; }
        [Column("B_IsTicketAvailable")]
        public bool? BIsTicketAvailable { get; set; }
        [Column("X_Relation")]
        [StringLength(50)]
        public string XRelation { get; set; }
    }
}
