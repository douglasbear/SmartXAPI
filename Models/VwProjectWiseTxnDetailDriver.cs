using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwProjectWiseTxnDetailDriver
    {
        [Column("N_ProjectID")]
        public int NProjectId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("X_ProjectName")]
        [StringLength(50)]
        public string XProjectName { get; set; }
        [Column("N_CustomerID")]
        public int? NCustomerId { get; set; }
        [Column("N_Sprice")]
        public double NSprice { get; set; }
        [Column("N_Cost")]
        public double NCost { get; set; }
        [Column("X_RefNo")]
        [StringLength(50)]
        public string XRefNo { get; set; }
    }
}
