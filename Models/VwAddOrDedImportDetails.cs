using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwAddOrDedImportDetails
    {
        [Column("X_refFieldName")]
        [StringLength(250)]
        public string XRefFieldName { get; set; }
        [Column("X_refFieldType")]
        [StringLength(50)]
        public string XRefFieldType { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_TransID")]
        public int NTransId { get; set; }
        [Required]
        [Column("X_Description")]
        [StringLength(100)]
        public string XDescription { get; set; }
        [Column("N_PayID")]
        public int NPayId { get; set; }
    }
}
