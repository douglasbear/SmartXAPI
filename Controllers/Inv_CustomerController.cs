using System.Collections.Generic;
using AutoMapper;
using SmartxAPI.Data;
using SmartxAPI.Dtos;
using SmartxAPI.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)]
    [Route("customer")]
    [ApiController]
    public class Inv_CustomerController : ControllerBase
    {
        private readonly IInv_CustomerRepo _repository;
        private readonly IMapper _mapper;

        public Inv_CustomerController(IInv_CustomerRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
       
        //GET api/Customer
        [HttpGet]
        public ActionResult <IEnumerable<CustomerReadDto>> GetAllCommmands()
        {
            var CustomerItems = _repository.GetAllCustomers();

            return Ok(_mapper.Map<IEnumerable<CustomerReadDto>>(CustomerItems));
        }

        //GET api/Customer/{id}
        [HttpGet("{id}", Name="GetCustomerById")]
        public ActionResult <CustomerReadDto> GetCustomerById(int id)
        {
            var CustomerItem = _repository.GetCustomerById(id);
            if(CustomerItem != null)
            {
                return Ok(_mapper.Map<CustomerReadDto>(CustomerItem));
            }
            return NotFound();
        }

        //POST api/Customer
        [HttpPost]
        public ActionResult <CustomerReadDto> CreateCustomer(CustomerCreateDto CustomerCreateDto)
        {
            var CustomerModel = _mapper.Map<InvCustomer>(CustomerCreateDto);
            _repository.CreateCustomer(CustomerModel);
            _repository.SaveChanges();

            var CustomerReadDto = _mapper.Map<CustomerReadDto>(CustomerModel);

            return CreatedAtRoute(nameof(GetCustomerById), new {Id = CustomerReadDto.NCustomerId}, CustomerReadDto);      
        }

        //PUT api/Customer/{id}
        [HttpPut("{id}")]
        public ActionResult UpdateCustomer(int id, CustomerUpdateDto CustomerUpdateDto)
        {
            var CustomerModelFromRepo = _repository.GetCustomerById(id);
            if(CustomerModelFromRepo == null)
            {
                return NotFound();
            }
            _mapper.Map(CustomerUpdateDto, CustomerModelFromRepo);

            _repository.UpdateCustomer(CustomerModelFromRepo);

            _repository.SaveChanges();

            return NoContent();
        }

        //PATCH api/Customer/{id}
        [HttpPatch("{id}")]
        public ActionResult PartialCustomerUpdate(int id, JsonPatchDocument<CustomerUpdateDto> patchDoc)
        {
            var CustomerModelFromRepo = _repository.GetCustomerById(id);
            if(CustomerModelFromRepo == null)
            {
                return NotFound();
            }

            var CustomerToPatch = _mapper.Map<CustomerUpdateDto>(CustomerModelFromRepo);
            patchDoc.ApplyTo(CustomerToPatch, ModelState);

            if(!TryValidateModel(CustomerToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(CustomerToPatch, CustomerModelFromRepo);

            _repository.UpdateCustomer(CustomerModelFromRepo);

            _repository.SaveChanges();

            return NoContent();
        }

        //DELETE api/Customer/{id}
        [HttpDelete("{id}")]
        public ActionResult DeleteCustomer(int id)
        {
            var CustomerModelFromRepo = _repository.GetCustomerById(id);
            if(CustomerModelFromRepo == null)
            {
                return NotFound();
            }
            _repository.DeleteCustomer(CustomerModelFromRepo);
            _repository.SaveChanges();

            return NoContent();
        }

        
        
    }
}