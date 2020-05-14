using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchSessionReservationCount
    {
        [Column("N_SessionID")]
        public int? NSessionId { get; set; }
        [Column("X_TypeName")]
        [StringLength(50)]
        public string XTypeName { get; set; }
        [Column("N_ClassTypeID")]
        public int? NClassTypeId { get; set; }
        [Column("N_AcYearID")]
        public int NAcYearId { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("N_ClassID")]
        public int NClassId { get; set; }
    }
}
