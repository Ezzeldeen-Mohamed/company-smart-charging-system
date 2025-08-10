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
    public class ChargeTransactionController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ChargeTransactionController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChargeTransactionDto>>> GetAll()
        {
            var chargeTransactions = await _unitOfWork.ChargeTransactions.GetAllAsync();
            var chargeTransactionDtos = _mapper.Map<IEnumerable<ChargeTransactionDto>>(chargeTransactions);
            return Ok(chargeTransactionDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ChargeTransactionDto>> GetById(int id)
        {
            var chargeTransaction = await _unitOfWork.ChargeTransactions.GetByIdAsync(id);
            if (chargeTransaction == null)
                return NotFound();
            var chargeTransactionDto = _mapper.Map<ChargeTransactionDto>(chargeTransaction);
            return Ok(chargeTransactionDto);
        }

        [HttpPost]
        public async Task<ActionResult> Create(ChargeTransactionDto chargeTransactionDto)
        {
            var chargeTransaction = _mapper.Map<ChargeTransaction>(chargeTransactionDto);
            await _unitOfWork.ChargeTransactions.AddAsync(chargeTransaction);
            await _unitOfWork.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = chargeTransaction.Id }, _mapper.Map<ChargeTransactionDto>(chargeTransaction));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, ChargeTransactionDto chargeTransactionDto)
        {
            if (id != chargeTransactionDto.Id)
                return BadRequest();
            var chargeTransaction = _mapper.Map<ChargeTransaction>(chargeTransactionDto);
            await _unitOfWork.ChargeTransactions.UpdateAsync(chargeTransaction);
            await _unitOfWork.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var chargeTransaction = await _unitOfWork.ChargeTransactions.GetByIdAsync(id);
            if (chargeTransaction == null)
                return NotFound();
            await _unitOfWork.ChargeTransactions.DeleteAsync(chargeTransaction);
            await _unitOfWork.SaveChangesAsync();
            return NoContent();
        }
    }
} 