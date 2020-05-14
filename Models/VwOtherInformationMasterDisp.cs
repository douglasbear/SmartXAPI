using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwOtherInformationMasterDisp
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FormID")]
        public int? NFormId { get; set; }
        [Column("N_OtherCode")]
        public int NOtherCode { get; set; }
        [Column("X_Subject")]
        [StringLength(50)]
        public string XSubject { get; set; }
        [Column("X_OtherCode")]
        [StringLength(50)]
        public string XOtherCode { get; set; }
        [Column("X_Text")]
        [StringLength(500)]
        public string XText { get; set; }
        [Column("N_LanguageId")]
        public int? NLanguageId { get; set; }
    }
}
