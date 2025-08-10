using CompanySmartChargingSystem.Application.DTOs;
using CompanySmartChargingSystem.Application.Services.IService;
using CompanySmartChargingSystem.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;

namespace company_smart_charging_system.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContractController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ContractController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContractDto>>> GetAll()
        {
            var contracts = await _unitOfWork.Contracts.GetAllAsync();
            var contractDtos = _mapper.Map<IEnumerable<ContractDto>>(contracts);
            return Ok(contractDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ContractDto>> GetById(int id)
        {
            var contract = await _unitOfWork.Contracts.GetByIdAsync(id);
            if (contract == null)
                return NotFound();
            var contractDto = _mapper.Map<ContractDto>(contract);
            return Ok(contractDto);
        }

        [HttpPost]
        public async Task<ActionResult> Create(ContractDto contractDto)
        {
            var contract = _mapper.Map<Contract>(contractDto);
            await _unitOfWork.Contracts.AddAsync(contract);
            await _unitOfWork.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = contract.Id }, _mapper.Map<ContractDto>(contract));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, ContractDto contractDto)
        {
            if (id != contractDto.Id)
                return BadRequest();
            var contract = _mapper.Map<Contract>(contractDto);
            await _unitOfWork.Contracts.UpdateAsync(contract);
            await _unitOfWork.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var contract = await _unitOfWork.Contracts.GetByIdAsync(id);
            if (contract == null)
                return NotFound();
            await _unitOfWork.Contracts.DeleteAsync(contract);
            await _unitOfWork.SaveChangesAsync();
            return NoContent();
        }
    }
} 