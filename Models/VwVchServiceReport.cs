using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwVchServiceReport
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_TruckID")]
        public int NTruckId { get; set; }
        [Column("X_PlateNumber")]
        [StringLength(100)]
        public string XPlateNumber { get; set; }
        [Column("X_Description")]
        [StringLength(100)]
        public string XDescription { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("X_TruckCode")]
        [StringLength(100)]
        public string XTruckCode { get; set; }
        [Column("N_CurrentMileage", TypeName = "numeric(18, 2)")]
        public decimal? NCurrentMileage { get; set; }
        [Column("X_ServiceTime")]
        [StringLength(50)]
        public string XServiceTime { get; set; }
        [Column("N_Ref_ID")]
        public int NRefId { get; set; }
        [Column("D_InDate", TypeName = "datetime")]
        public DateTime? DInDate { get; set; }
        [Required]
        [Column("X_Status")]
        [StringLength(3)]
        public string XStatus { get; set; }
    }
}
