using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Vch_Service")]
    public partial class VchService
    {
        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Key]
        [Column("N_Ref_ID")]
        public int NRefId { get; set; }
        [Column("N_Truck_ID")]
        public int? NTruckId { get; set; }
        [Column("X_TruckCode")]
        [StringLength(100)]
        public string XTruckCode { get; set; }
        [Column("X_PlateNo")]
        [StringLength(100)]
        public string XPlateNo { get; set; }
        [Column("D_InDate", TypeName = "datetime")]
        public DateTime? DInDate { get; set; }
        [Column("X_Reason")]
        [StringLength(200)]
        public string XReason { get; set; }
        [Column("D_OutDate", TypeName = "datetime")]
        public DateTime? DOutDate { get; set; }
        [Column("X_Remarks")]
        [StringLength(200)]
        public string XRemarks { get; set; }
        [Column("B_IsIn")]
        public bool? BIsIn { get; set; }
        [Column("B_IsOut")]
        public bool? BIsOut { get; set; }
        [Required]
        [Column("X_Ref_Code")]
        [StringLength(50)]
        public string XRefCode { get; set; }
        [Column("X_ServiceTime")]
        [StringLength(50)]
        public string XServiceTime { get; set; }
    }
}
