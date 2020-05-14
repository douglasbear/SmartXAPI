using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchClassDisp
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        public int Code { get; set; }
        [StringLength(50)]
        public string Class { get; set; }
        [Column("N_ClassTypeID")]
        public int? NClassTypeId { get; set; }
        [StringLength(50)]
        public string Section { get; set; }
        [Column("N_ClassDivisionID")]
        public int NClassDivisionId { get; set; }
        [Required]
        [Column("X_ClassDivision")]
        [StringLength(50)]
        public string XClassDivision { get; set; }
    }
}
