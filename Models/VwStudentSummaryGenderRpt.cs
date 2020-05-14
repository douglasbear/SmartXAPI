using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwStudentSummaryGenderRpt
    {
        [Column("boys")]
        public int? Boys { get; set; }
        [Column("girls")]
        public int? Girls { get; set; }
        [Column("X_Class")]
        [StringLength(50)]
        public string XClass { get; set; }
        [Column("X_ClassDivision")]
        [StringLength(50)]
        public string XClassDivision { get; set; }
        [Column("X_ClassType")]
        [StringLength(50)]
        public string XClassType { get; set; }
        [Column("N_ClassID")]
        public int NClassId { get; set; }
        [Column("N_ClassDivisionID")]
        public int? NClassDivisionId { get; set; }
        [Column("N_ClassTypeID")]
        public int NClassTypeId { get; set; }
        [Column("X_Gender")]
        [StringLength(10)]
        public string XGender { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
    }
}
