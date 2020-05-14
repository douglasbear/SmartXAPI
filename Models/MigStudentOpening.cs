using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("_Mig_StudentOpening")]
    public partial class MigStudentOpening
    {
        [Column("PKey_Code")]
        public int? PkeyCode { get; set; }
        [StringLength(200)]
        public string Date { get; set; }
        [Column("Student_Name")]
        [StringLength(300)]
        public string StudentName { get; set; }
        [Column("Invoice_No")]
        [StringLength(300)]
        public string InvoiceNo { get; set; }
        [StringLength(300)]
        public string Amount { get; set; }
        [StringLength(300)]
        public string Class { get; set; }
    }
}
