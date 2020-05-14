using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_VendorAttachments")]
    public partial class InvVendorAttachments
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnyearID")]
        public int? NFnyearId { get; set; }
        [Column("N_AttachmentID")]
        public int? NAttachmentId { get; set; }
        [Column("N_VendorID")]
        public int? NVendorId { get; set; }
        [Column("N_FormID")]
        public int? NFormId { get; set; }
        [Column("N_InventoryID")]
        public int? NInventoryId { get; set; }
        [Column("N_CategoryID")]
        public int? NCategoryId { get; set; }
        [Column("X_Subject")]
        [StringLength(500)]
        public string XSubject { get; set; }
        [Column("X_Filename")]
        [StringLength(500)]
        public string XFilename { get; set; }
        [Column("X_FileType")]
        [StringLength(50)]
        public string XFileType { get; set; }
        [Column("X_File")]
        [StringLength(500)]
        public string XFile { get; set; }
        [Column("X_Extension")]
        [StringLength(50)]
        public string XExtension { get; set; }
        [Column("X_refName")]
        public string XRefName { get; set; }
        [Column("D_ExpiryDate", TypeName = "datetime")]
        public DateTime? DExpiryDate { get; set; }
    }
}
