using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwVehTruckDetails
    {
        [Column("X_PlateNumber")]
        [StringLength(100)]
        public string XPlateNumber { get; set; }
        [Column("X_TruckCode")]
        [StringLength(100)]
        public string XTruckCode { get; set; }
        [Column("N_TruckID")]
        public int NTruckId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_OpeningMileage", TypeName = "numeric(18, 2)")]
        public decimal? NOpeningMileage { get; set; }
        [Column("N_CurrentMileage", TypeName = "numeric(18, 2)")]
        public decimal? NCurrentMileage { get; set; }
        [Column("X_Description")]
        [StringLength(100)]
        public string XDescription { get; set; }
    }
}
