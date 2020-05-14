using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Acc_OtherInformation")]
    public partial class AccOtherInformation
    {
        [Column("N_OtherDtlsID")]
        public int? NOtherDtlsId { get; set; }
        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_OtherCode")]
        public int NOtherCode { get; set; }
        [Key]
        [Column("N_PartyID")]
        public int NPartyId { get; set; }
        [Column("X_Information")]
        [StringLength(50)]
        public string XInformation { get; set; }
    }
}
