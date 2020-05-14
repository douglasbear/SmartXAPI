using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwAccOtherInformation
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FormID")]
        public int? NFormId { get; set; }
        [Column("N_OtherCode")]
        public int NOtherCode { get; set; }
        [Column("N_OtherDtlsID")]
        public int? NOtherDtlsId { get; set; }
        [Column("N_PartyID")]
        public int NPartyId { get; set; }
        [Column("X_Subject")]
        [StringLength(50)]
        public string XSubject { get; set; }
        [Column("X_Information")]
        [StringLength(50)]
        public string XInformation { get; set; }
    }
}
