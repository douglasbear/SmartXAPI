using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwStudentStatusRpt
    {
        [Column("n_companyid")]
        public int NCompanyid { get; set; }
        [Column("N_admissionid")]
        public int NAdmissionid { get; set; }
        [Column(TypeName = "money")]
        public decimal? Balance { get; set; }
    }
}
