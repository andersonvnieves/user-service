using br.com.fiap.cloudgames.Application.Services;
using br.com.fiap.cloudgames.Domain.Repositories;
using br.com.fiap.cloudgames.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace br.com.fiap.cloudgames.Application.UseCases.User.LogIn;

public class LogInUseCase
{
    private readonly IUserAuthService _userAuthService;
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly ILogger<LogInUseCase> _logger;

    public LogInUseCase(
        IUserAuthService userAuthService,
        IUserRepository userRepository,
        ITokenService tokenService,
        ILogger<LogInUseCase> logger)
    {
        _userAuthService = userAuthService;
        _userRepository = userRepository;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<LogInResponse> ExecuteAsync(LogInRequest request)
    {
        _logger.LogInformation("Executing {UseCase}. Email={Email}", nameof(LogInUseCase), request.email);

        try
        {
            var email = new EmailAddress(request.email);
            var identityUserId = await _userAuthService.AuthenticateUserAsync(email.Email, request.password);
            var user = await _userRepository.GetByIdentityIdAsync(identityUserId);
            var token = await _tokenService.GenerateTokenAsync(user);

            _logger.LogInformation("Login succeeded. UserId={UserId}", user.Id);

            return new LogInResponse()
            {
                Token =  token
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing {UseCase}. Email={Email}", nameof(LogInUseCase), request.email);
            throw;
        }
    }
}