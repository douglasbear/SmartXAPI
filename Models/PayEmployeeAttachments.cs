using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_EmployeeAttachments")]
    public partial class PayEmployeeAttachments
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_AttachmentID")]
        public int? NAttachmentId { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("X_Subject")]
        [StringLength(100)]
        public string XSubject { get; set; }
        [Column("I_Image", TypeName = "image")]
        public byte[] IImage { get; set; }
        [Column("X_FileName")]
        [StringLength(500)]
        public string XFileName { get; set; }
        [Column("X_Extension")]
        [StringLength(25)]
        public string XExtension { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("X_refName")]
        [StringLength(500)]
        public string XRefName { get; set; }
    }
}
