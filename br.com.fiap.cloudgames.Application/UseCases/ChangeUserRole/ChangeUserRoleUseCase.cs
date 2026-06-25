using br.com.fiap.cloudgames.Application.Services;
using br.com.fiap.cloudgames.Application.UnitsOfWork;
using br.com.fiap.cloudgames.Domain.Enums;
using br.com.fiap.cloudgames.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace br.com.fiap.cloudgames.Application.UseCases.User.ChangeUserRole;

public class ChangeUserRoleUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IUserAuthService _userAuthService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ChangeUserRoleUseCase> _logger;
    
    public ChangeUserRoleUseCase(
        IUserRepository userRepository,
        IUserAuthService userAuthService,
        IUnitOfWork unitOfWork,
        ILogger<ChangeUserRoleUseCase> logger)
    {
        _userRepository = userRepository;
        _userAuthService = userAuthService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ChangeUserRoleResponse> ExecuteAsync(ChangeUserRoleRequest request)
    {
        _logger.LogInformation("Executing {UseCase}. UserId={UserId}, Role={Role}", nameof(ChangeUserRoleUseCase), request.UserId, request.Role);

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var parseResult = Guid.TryParse(request.UserId, out var UserId);
            if (!parseResult)
            {
                _logger.LogWarning("Invalid UserId format. UserId={UserId}", request.UserId);
                throw new ApplicationException("Invalid UserId Format");
            }
            
            var user = await _userRepository.GetUserByIdAsync(UserId);
            if (user == null)
            {
                _logger.LogWarning("User not found. UserId={UserId}", request.UserId);
                throw new ApplicationException("User not found.");
            }

            var role = Enum.Parse<UserRoles>(request.Role, ignoreCase: true);
            await _userAuthService.ReplaceUserRoleAsync(user.IdentityId, role.ToString().ToLowerInvariant());

            user.Role = role;
            _userRepository.Update(user);

            await _unitOfWork.CommitAsync();

            _logger.LogInformation(
                "Role changed successfully. UserId={UserId}, NewRole={Role}",
                user.Id,
                role.ToString());

            return new ChangeUserRoleResponse()
            {
                Id =  user.Id.ToString(),
                FirstName = user.Name.FirstName,
                LastName = user.Name.LastName,
                Email = user.Email.Email,
                Role = role.ToString()
            };
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            _logger.LogError(ex, "Error executing {UseCase}. UserId={UserId}, Role={Role}", nameof(ChangeUserRoleUseCase), request.UserId, request.Role);
            throw;
        }
    }
}