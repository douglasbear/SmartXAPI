using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("FingerPrint_User")]
    public partial class FingerPrintUser
    {
        [Key]
        [Column("N_UserID")]
        public int NUserId { get; set; }
        [Column("D_Enquirydate", TypeName = "datetime")]
        public DateTime? DEnquirydate { get; set; }
    }
}
