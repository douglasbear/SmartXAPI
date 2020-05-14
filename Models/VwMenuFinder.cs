using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwMenuFinder
    {
        [Column("FormIDMain")]
        public int FormIdmain { get; set; }
        [Column("FormIDSub")]
        public int FormIdsub { get; set; }
        [Column("MainID")]
        [StringLength(50)]
        public string MainId { get; set; }
        [Column("SubID")]
        [StringLength(50)]
        public string SubId { get; set; }
        [StringLength(500)]
        public string MainMenuName { get; set; }
        [StringLength(500)]
        public string SubMenuName { get; set; }
        [Column("ProgrammerFormID")]
        [StringLength(150)]
        public string ProgrammerFormId { get; set; }
    }
}
