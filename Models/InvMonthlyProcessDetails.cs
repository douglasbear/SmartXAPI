using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_MonthlyProcessDetails")]
    public partial class InvMonthlyProcessDetails
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_ProcessID")]
        public int NProcessId { get; set; }
        [Key]
        [Column("N_ProcessDetailID")]
        public int NProcessDetailId { get; set; }
        [Required]
        [Column("X_RunMonth")]
        [StringLength(20)]
        public string XRunMonth { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("N_RefID")]
        public int? NRefId { get; set; }
        [Column("N_FormID")]
        public int? NFormId { get; set; }
        [Column("N_RefDetailID")]
        public int? NRefDetailId { get; set; }
    }
}
