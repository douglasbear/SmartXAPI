using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchParentWithMoreStudents
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_ParentID")]
        public int? NParentId { get; set; }
    }
}
