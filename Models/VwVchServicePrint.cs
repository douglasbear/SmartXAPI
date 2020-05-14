using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwVchServicePrint
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("X_TruckCode")]
        [StringLength(100)]
        public string XTruckCode { get; set; }
        [Column("X_PlateNo")]
        [StringLength(100)]
        public string XPlateNo { get; set; }
        [Column("D_InDate", TypeName = "datetime")]
        public DateTime? DInDate { get; set; }
        [Column("D_OutDate", TypeName = "datetime")]
        public DateTime? DOutDate { get; set; }
        [Column("X_Reason")]
        [StringLength(200)]
        public string XReason { get; set; }
        [Column("X_Remarks")]
        [StringLength(200)]
        public string XRemarks { get; set; }
        [Column("N_Ref_ID")]
        public int NRefId { get; set; }
        [Column("X_Description")]
        [StringLength(100)]
        public string XDescription { get; set; }
    }
}
