using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchstudentcountRpt
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_AcYearID")]
        public int NAcYearId { get; set; }
        [Column("Res_OldCount")]
        public int? ResOldCount { get; set; }
        [Column("Res_NewCount")]
        public int? ResNewCount { get; set; }
        [Column("Res_OldEnrolledCount")]
        public int? ResOldEnrolledCount { get; set; }
        [Column("Res_NewEnrolledCount")]
        public int? ResNewEnrolledCount { get; set; }
        [Column("N_ClassID")]
        public int? NClassId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
    }
}
