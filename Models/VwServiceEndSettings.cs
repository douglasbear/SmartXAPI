using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwServiceEndSettings
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_ServiceEndID")]
        public int NServiceEndId { get; set; }
        [Column("X_ServiceEndCode")]
        [StringLength(20)]
        public string XServiceEndCode { get; set; }
        [Column("N_ServiceEndStatusID")]
        public int? NServiceEndStatusId { get; set; }
        [Column("X_ServiceEndStatusDesc")]
        [StringLength(200)]
        public string XServiceEndStatusDesc { get; set; }
        [Column("N_EntryUserID")]
        public int? NEntryUserId { get; set; }
        [Column("N_UpdatedUserID")]
        public int? NUpdatedUserId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("B_Active")]
        public bool? BActive { get; set; }
        [Column("N_EndSettiingsID")]
        public int NEndSettiingsId { get; set; }
        public int? YearFrom { get; set; }
        public int? YearTo { get; set; }
        public double? SalaryRange { get; set; }
        [Column("N_Status")]
        public int? NStatus { get; set; }
        [Column("N_ApplyFrom")]
        public int? NApplyFrom { get; set; }
        [Column(TypeName = "money")]
        public decimal? AmtPerc { get; set; }
        [StringLength(50)]
        public string ServiceEndStatus { get; set; }
    }
}
