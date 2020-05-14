using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwFileCancellationRpt
    {
        [Column("X_RegCode")]
        [StringLength(50)]
        public string XRegCode { get; set; }
        [Required]
        [Column("X_Name")]
        [StringLength(50)]
        public string XName { get; set; }
        [Column("X_Profession")]
        [StringLength(100)]
        public string XProfession { get; set; }
        [Column("X_mobileNo")]
        [StringLength(20)]
        public string XMobileNo { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime DDate { get; set; }
        [Column("X_FileNo")]
        [StringLength(50)]
        public string XFileNo { get; set; }
        [Column("D_Dob", TypeName = "smalldatetime")]
        public DateTime DDob { get; set; }
        [StringLength(11)]
        public string Age { get; set; }
        [Column("D_CancelDate", TypeName = "smalldatetime")]
        public DateTime? DCancelDate { get; set; }
        [Column("X_CancelNo")]
        [StringLength(50)]
        public string XCancelNo { get; set; }
        [Column("N_RfdAmt", TypeName = "money")]
        public decimal NRfdAmt { get; set; }
    }
}
