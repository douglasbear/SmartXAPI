using System.Collections.Generic;
using AutoMapper;
using SmartxAPI.Data;
using SmartxAPI.Dtos;
using SmartxAPI.Dtos.Login;
using SmartxAPI.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using SmartxAPI.Profiles;
using System;

namespace SmartxAPI.Controllers
{

    [Route("user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ISec_UserRepo _repository;

        private readonly IMapper _mapper;

        public UserController(ISec_UserRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
       
        //GET api/User
        [HttpGet]
        public ActionResult <IEnumerable<UserReadDto>> GetAllCommmands()
        {
            var UserItems = _repository.GetAllUsers();

            return Ok(_mapper.Map<IEnumerable<UserReadDto>>(UserItems));
        }


        [HttpPost("login")]
        public ActionResult Authenticate([FromBody]Sec_AuthenticateDto model)
        {
            //try{
                    var user = _repository.Authenticate(model.CompanyName,model.Username, model.Password);

                    if (user == null){ return BadRequest(new { message = "Username or password is incorrect" }); }

                    return Ok(user);
                //}
                //catch (Exception ex)
                //{
                  //  return StatusCode(403,ex.Message);
                //}
        }

        //GET api/User/{id}
        [HttpGet("{id}", Name="GetUserById")]
        public ActionResult <UserReadDto> GetUserById(int id)
        {
            var UserItem = _repository.GetUserById(id);
            if(UserItem != null)
            {
                return Ok(_mapper.Map<UserReadDto>(UserItem));
            }
            return NotFound();
        }

        //POST api/User
        [HttpPost]
        public ActionResult <UserReadDto> CreateUser(UserCreateDto UserCreateDto)
        {
            var UserModel = _mapper.Map<SecUser>(UserCreateDto);
            _repository.CreateUser(UserModel);
            _repository.SaveChanges();

            var UserReadDto = _mapper.Map<UserReadDto>(UserModel);

            return CreatedAtRoute(nameof(GetUserById), new {Id = UserReadDto.NUserId}, UserReadDto);      
        }



        //PUT api/User/{id}
        [HttpPut("{id}")]
        public ActionResult UpdateUser(int id, UserUpdateDto UserUpdateDto)
        {
            var UserModelFromRepo = _repository.GetUserById(id);
            if(UserModelFromRepo == null)
            {
                return NotFound();
            }
            _mapper.Map(UserUpdateDto, UserModelFromRepo);

            _repository.UpdateUser(UserModelFromRepo);

            _repository.SaveChanges();

            return NoContent();
        }

        //PATCH api/User/{id}
        [HttpPatch("{id}")]
        public ActionResult PartialUserUpdate(int id, JsonPatchDocument<UserUpdateDto> patchDoc)
        {
            var UserModelFromRepo = _repository.GetUserById(id);
            if(UserModelFromRepo == null)
            {
                return NotFound();
            }

            var UserToPatch = _mapper.Map<UserUpdateDto>(UserModelFromRepo);
            patchDoc.ApplyTo(UserToPatch, ModelState);

            if(!TryValidateModel(UserToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(UserToPatch, UserModelFromRepo);

            _repository.UpdateUser(UserModelFromRepo);

            _repository.SaveChanges();

            return NoContent();
        }

        //DELETE api/User/{id}
        [HttpDelete("{id}")]
        public ActionResult DeleteUser(int id)
        {
            var UserModelFromRepo = _repository.GetUserById(id);
            if(UserModelFromRepo == null)
            {
                return NotFound();
            }
            _repository.DeleteUser(UserModelFromRepo);
            _repository.SaveChanges();

            return NoContent();
        }

        
        
    }
}