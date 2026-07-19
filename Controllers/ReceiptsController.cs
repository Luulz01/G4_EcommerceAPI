using EcommerceAPI.DTOs;
using EcommerceAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers
{
    [ApiController]
    [Route("api/receipts")]
    public class ReceiptsController : ControllerBase
    {
        private readonly IReceiptService _service;

        public ReceiptsController(IReceiptService service)
        {
            _service = service;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReceiptDto dto)
        {
            try
            {
                var created = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var receipts = await _service.GetAllAsync();
            return Ok(receipts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var receipt = await _service.GetByIdAsync(id);
            if (receipt == null) return NotFound(new { message = $"Recibo {id} no encontrado" });
            return Ok(receipt);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var receipts = await _service.GetByUserIdAsync(userId);
            return Ok(receipts);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted) return NotFound(new { message = $"Recibo {id} no encontrado" });
            return NoContent();
        }
    }
}