﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvItemUnit
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_ItemUnitID")]
        public int NItemUnitId { get; set; }
        [Column("X_ItemUnit")]
        [StringLength(500)]
        public string XItemUnit { get; set; }
        [Column("B_BaseUnit")]
        public bool? BBaseUnit { get; set; }
    }
}
