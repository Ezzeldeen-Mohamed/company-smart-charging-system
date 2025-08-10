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
    public class MeterController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MeterController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MeterDto>>> GetAll()
        {
            var meters = await _unitOfWork.Meters.GetAllAsync();
            var meterDtos = _mapper.Map<IEnumerable<MeterDto>>(meters);
            return Ok(meterDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MeterDto>> GetById(int id)
        {
            var meter = await _unitOfWork.Meters.GetByIdAsync(id);
            if (meter == null)
                return NotFound();
            var meterDto = _mapper.Map<MeterDto>(meter);
            return Ok(meterDto);
        }

        [HttpPost]
        public async Task<ActionResult> Create(MeterDto meterDto)
        {
            var meter = _mapper.Map<Meter>(meterDto);
            await _unitOfWork.Meters.AddAsync(meter);
            await _unitOfWork.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = meter.Id }, _mapper.Map<MeterDto>(meter));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, MeterDto meterDto)
        {
            if (id != meterDto.Id)
                return BadRequest();
            var meter = _mapper.Map<Meter>(meterDto);
            await _unitOfWork.Meters.UpdateAsync(meter);
            await _unitOfWork.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var meter = await _unitOfWork.Meters.GetByIdAsync(id);
            if (meter == null)
                return NotFound();
            await _unitOfWork.Meters.DeleteAsync(meter);
            await _unitOfWork.SaveChangesAsync();
            return NoContent();
        }
    }
} 