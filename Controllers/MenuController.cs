using System.Collections.Generic;
using AutoMapper;
using SmartxAPI.Data;
using SmartxAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;


namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)]
    [Route("menu")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly IMenuRepo _repository;

        public MenuController(IMenuRepo repository, IMapper mapper)
        {
            _repository = repository;
        }
       
        //GET api/menu/list
        [HttpGet("list")]
        public ActionResult <IEnumerable<VwUserMenus>> GetAllMenu ()
        {
            var Menu = _repository.GetAllMenus();

            return Ok(Menu);
        }

       
    }
}