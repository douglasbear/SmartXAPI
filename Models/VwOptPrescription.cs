using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwOptPrescription
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("N_prescriptionID")]
        public int NPrescriptionId { get; set; }
        [Column("N_SalesID")]
        public int NSalesId { get; set; }
        [Column("N_SalesOrderID")]
        public int NSalesOrderId { get; set; }
        [Column("X_ReceiptNo")]
        [StringLength(50)]
        public string XReceiptNo { get; set; }
        [Column("N_CustomerId")]
        public int? NCustomerId { get; set; }
        [Column("X_CustomerCode")]
        [StringLength(50)]
        public string XCustomerCode { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
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
        [Required]
        [StringLength(100)]
        public string FrameCode { get; set; }
        [Required]
        [StringLength(800)]
        public string FrameName { get; set; }
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
        [Column("N_LensCoatingID")]
        public int? NLensCoatingId { get; set; }
        [Column("N_LensID")]
        public int? NLensId { get; set; }
        [StringLength(100)]
        public string LensCode { get; set; }
        [StringLength(800)]
        public string LensName { get; set; }
        [StringLength(50)]
        public string SupplierCode { get; set; }
        [StringLength(100)]
        public string SupplierName { get; set; }
        [StringLength(5)]
        public string LensMeterialCode { get; set; }
        [StringLength(50)]
        public string LensMeterialName { get; set; }
        [StringLength(5)]
        public string LensDesignCode { get; set; }
        [Column("N_LensDesignID")]
        public int? NLensDesignId { get; set; }
        [StringLength(50)]
        public string LensDesignName { get; set; }
        [Column("N_LensFeatureID")]
        public int? NLensFeatureId { get; set; }
        [StringLength(5)]
        public string LensFeatureCode { get; set; }
        [StringLength(50)]
        public string LensFeatureName { get; set; }
        [StringLength(5)]
        public string LensCoatingCode { get; set; }
        [StringLength(50)]
        public string LensCoatingName { get; set; }
        [Column("N_LensIndexID")]
        public int? NLensIndexId { get; set; }
        [StringLength(5)]
        public string LensIndexCode { get; set; }
        [StringLength(50)]
        public string LensIndexName { get; set; }
        [Column("X_Eye")]
        [StringLength(10)]
        public string XEye { get; set; }
    }
}
