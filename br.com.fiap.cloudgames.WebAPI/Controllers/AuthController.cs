using br.com.fiap.cloudgames.Application.UseCases.User.LogIn;
using Microsoft.AspNetCore.Mvc;

namespace br.com.fiap.cloudgames.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly LogInUseCase _logInUseCase;

        public AuthController(LogInUseCase logInUseCase)
        {
            _logInUseCase = logInUseCase;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LogInRequest request)
        {
            var result = await  _logInUseCase.ExecuteAsync(request);
            return Ok(result);
        }
    }
}
