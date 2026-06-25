using br.com.fiap.cloudgames.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace br.com.fiap.cloudgames.Application.UseCases.User.RetrieveUser;

public class RetrieveUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<RetrieveUserUseCase> _logger;

    public RetrieveUserUseCase(IUserRepository userRepository, ILogger<RetrieveUserUseCase> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<RetrieveUserResponse> ExecuteAsync(RetrieveUserRequest request)
    {
        _logger.LogInformation("Executing {UseCase}. UserId={UserId}", nameof(RetrieveUserUseCase), request.UserId);

        try
        {
            var parseResult = Guid.TryParse(request.UserId, out var UserId);
            if (!parseResult)
            {
                _logger.LogWarning("Invalid UserId format. UserId={UserId}", request.UserId);
                throw new ApplicationException("Invalid UserId Format");
            }

            var user = await _userRepository.GetUserByIdAsync(UserId);
            if(user == null)
            {
                _logger.LogWarning("User not found. UserId={UserId}", request.UserId);
                throw new ApplicationException("User not found");
            }

            _logger.LogInformation("User retrieved successfully. UserId={UserId}, Email={Email}", user.Id, user.Email.Email);

            return new RetrieveUserResponse()
            {
                Id = user.Id.ToString(),
                FirstName = user.Name.FirstName,
                LastName = user.Name.LastName,
                Email = user.Email.Email,
                UserAccountStatus = user.UserAccountStatus,
                Role = user.Role.ToString(),
                CreationDate = user.CreationDate
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing {UseCase}. UserId={UserId}", nameof(RetrieveUserUseCase), request.UserId);
            throw;
        }
    }
}