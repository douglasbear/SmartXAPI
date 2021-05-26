using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using SmartxAPI.Models;

namespace SmartxAPI.Dtos.Login
{
    public class LoginResponseDto
    {
		public TokenDto Token { get; set; }
		public DataTable GlobalUserInfo { get; set; }
		public UserDto UserData { get; set; } // Company User
		public CompanyDto CompanyData {get; set;}
		public FnYearDto FnYearData {get; set;}
		public IEnumerable<MenuDto> MenuList {get;set;}
    }
}
