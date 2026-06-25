using br.com.fiap.cloudgames.Application.UseCases.User.ChangeUserRole;
using br.com.fiap.cloudgames.Application.UseCases.User.RegisterUser;
using br.com.fiap.cloudgames.Application.UseCases.User.RetrieveUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace br.com.fiap.cloudgames.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly RegisterUserUseCase _registerUserUseCase;
        private readonly ChangeUserRoleUseCase _changeUserRoleUseCase;
        private readonly RetrieveUserUseCase _retrieveUserUseCase;
        private const string ADMIN_ROLE = "admin";
        public UserController(RegisterUserUseCase registerUserUseCase,
            ChangeUserRoleUseCase changeUserRoleUseCase,
            RetrieveUserUseCase retrieveUserUseCase)
        {
            _registerUserUseCase = registerUserUseCase;
            _changeUserRoleUseCase = changeUserRoleUseCase;
            _retrieveUserUseCase = retrieveUserUseCase;
        }
        
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RegisterUserRequest request)
        {
            var result = await _registerUserUseCase.ExecuteAsync(request);
            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }

        [Authorize(Roles = ADMIN_ROLE)]
        [HttpPatch("role")]
        public async Task<IActionResult> Patch([FromBody] ChangeUserRoleRequest request)
        {
            var result = await _changeUserRoleUseCase.ExecuteAsync(request);
            return Ok(result);
        }
        
        [Authorize(Roles = ADMIN_ROLE)]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] String Id)
        {
            var result = await _retrieveUserUseCase.ExecuteAsync(new RetrieveUserRequest() { UserId = Id });
            return Ok(result);
        }
    }
}
