using EcommerceAPI.DTOs;
using EcommerceAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;
        private readonly ITokenService _tokenService;

        public UsersController(IUserService service, ITokenService tokenService)
        {
            _service = service;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
        {
            try
            {
                var created = await _service.RegisterAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var token = await _service.LoginAsync(dto, _tokenService);
            if (token == null)
                return Unauthorized(new { message = "Credenciales inválidas" });

            return Ok(new { token });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _service.GetByIdAsync(id);
            if (user == null) return NotFound(new { message = $"Usuario {id} no encontrado" });
            return Ok(user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] RegisterUserDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            if (!updated) return NotFound(new { message = $"Usuario {id} no encontrado" });
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted) return NotFound(new { message = $"Usuario {id} no encontrado" });
            return NoContent();
        }
    }
}