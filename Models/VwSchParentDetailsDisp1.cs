using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchParentDetailsDisp1
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [StringLength(11)]
        public string Code { get; set; }
        [Column("Father Name")]
        [StringLength(50)]
        public string FatherName { get; set; }
        [Column("Mother Name")]
        [StringLength(50)]
        public string MotherName { get; set; }
        [Column("Family Name")]
        [StringLength(50)]
        public string FamilyName { get; set; }
    }
}
