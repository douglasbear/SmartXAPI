using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Ass_Transfer")]
    public partial class AssTransfer
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Key]
        [Column("N_TransferID")]
        public int NTransferId { get; set; }
        [Key]
        [Column("N_LineNo")]
        public int NLineNo { get; set; }
        [Column("N_ItemID")]
        [StringLength(50)]
        public string NItemId { get; set; }
        [Column("N_BranchFromID")]
        public int? NBranchFromId { get; set; }
        [Column("N_BranchToID")]
        public int? NBranchToId { get; set; }
        [Column("D_TransDate", TypeName = "datetime")]
        public DateTime? DTransDate { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_CostCenterFromID")]
        public int? NCostCenterFromId { get; set; }
        [Column("N_CostCenterToID")]
        public int? NCostCenterToId { get; set; }
        [Column("X_TransCode")]
        [StringLength(50)]
        public string XTransCode { get; set; }
        [Column("N_ProjectFromID")]
        public int? NProjectFromId { get; set; }
        [Column("N_ProjectToID")]
        public int? NProjectToId { get; set; }
        [Column("N_EmployeeFromID")]
        public int? NEmployeeFromId { get; set; }
        [Column("N_EmployeeToID")]
        public int? NEmployeeToId { get; set; }
    }
}
