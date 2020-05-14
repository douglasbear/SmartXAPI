using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchstudentcountSummary
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_AcYearID")]
        public int? NAcYearId { get; set; }
        [Column("N_ClassID")]
        public int? NClassId { get; set; }
        [Column("X_Class")]
        [StringLength(50)]
        public string XClass { get; set; }
        [Column("ReservatnNew_Count")]
        public int? ReservatnNewCount { get; set; }
        [Column("EnrolmtNew_Count")]
        public int? EnrolmtNewCount { get; set; }
        [Column("ReservationOld_Count")]
        public int? ReservationOldCount { get; set; }
        [Column("EnrolmtOld_Count")]
        public int? EnrolmtOldCount { get; set; }
    }
}
