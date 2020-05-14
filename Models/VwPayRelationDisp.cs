using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayRelationDisp
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("RelationID")]
        public int RelationId { get; set; }
        [StringLength(50)]
        public string Relation { get; set; }
        [Column("N_RelationID")]
        public int NRelationId { get; set; }
        [StringLength(30)]
        public string RelationCode { get; set; }
    }
}
