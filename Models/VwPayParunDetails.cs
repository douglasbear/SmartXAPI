using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayParunDetails
    {
        [Column("N_TransID")]
        public int NTransId { get; set; }
        [Column("N_PayRunID")]
        public int? NPayRunId { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("X_PayrunText")]
        [StringLength(50)]
        public string XPayrunText { get; set; }
        [Column("B_PostedAccount")]
        public bool? BPostedAccount { get; set; }
    }
}
