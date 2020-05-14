using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchReceiptprintCustom
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_AdmissionID")]
        public int NAdmissionId { get; set; }
        [Column("X_ReceiptNo")]
        [StringLength(50)]
        public string XReceiptNo { get; set; }
        [Column("D_ReceiptDate", TypeName = "datetime")]
        public DateTime? DReceiptDate { get; set; }
        [Column(TypeName = "money")]
        public decimal? TutionFee { get; set; }
        [Column(TypeName = "money")]
        public decimal? BusFee { get; set; }
        [Column(TypeName = "money")]
        public decimal? OtherFee { get; set; }
        [Column(TypeName = "money")]
        public decimal? Balance { get; set; }
        [Column("X_Name")]
        [StringLength(50)]
        public string XName { get; set; }
    }
}
