using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Opt_prescription")]
    public partial class OptPrescription
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Key]
        [Column("N_prescriptionID")]
        public int NPrescriptionId { get; set; }
        [Column("N_SalesID")]
        public int? NSalesId { get; set; }
        [Column("N_CustomerId")]
        public int? NCustomerId { get; set; }
        [Column("N_SPH_L")]
        public double? NSphL { get; set; }
        [Column("N_SPH_R")]
        public double? NSphR { get; set; }
        [Column("N_CYL_L")]
        public double? NCylL { get; set; }
        [Column("N_CYL_R")]
        public double? NCylR { get; set; }
        [Column("N_AXIS_L")]
        public double? NAxisL { get; set; }
        [Column("N_AXIS_R")]
        public double? NAxisR { get; set; }
        [Column("N_ADD_L")]
        public double? NAddL { get; set; }
        [Column("N_ADD_R")]
        public double? NAddR { get; set; }
        [Column("N_IPD")]
        public double? NIpd { get; set; }
        [Column("X_ColNo")]
        [StringLength(50)]
        public string XColNo { get; set; }
        [Column("N_FrameID")]
        public int? NFrameId { get; set; }
        [Column("N_LensID")]
        public int? NLensId { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("X_Notes")]
        [StringLength(1000)]
        public string XNotes { get; set; }
        [Column("X_FrameModel")]
        [StringLength(100)]
        public string XFrameModel { get; set; }
        [Column("N_SupplierID")]
        public int? NSupplierId { get; set; }
        [Column("N_LensMeterialID")]
        public int? NLensMeterialId { get; set; }
        [Column("N_LensDesignID")]
        public int? NLensDesignId { get; set; }
        [Column("N_LensFeatureID")]
        public int? NLensFeatureId { get; set; }
        [Column("N_LensCoatingID")]
        public int? NLensCoatingId { get; set; }
        [Column("N_LensIndexID")]
        public int? NLensIndexId { get; set; }
        [Column("N_SalesOrderID")]
        public int? NSalesOrderId { get; set; }
        [Column("X_Eye")]
        [StringLength(10)]
        public string XEye { get; set; }
    }
}
