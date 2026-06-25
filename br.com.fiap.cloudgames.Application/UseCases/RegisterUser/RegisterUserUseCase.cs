using br.com.fiap.cloudgames.Application.Services;
using br.com.fiap.cloudgames.Domain.Repositories;
using br.com.fiap.cloudgames.Application.UnitsOfWork;
using br.com.fiap.cloudgames.Domain.Enums;
using br.com.fiap.cloudgames.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using br.com.fiap.cloudgames.Application.Publishers;

namespace br.com.fiap.cloudgames.Application.UseCases.User.RegisterUser;

public class RegisterUserUseCase
{
    private readonly IUserAuthService _userAuthService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly IUserCreatedEventPublisher _userCreatedEventPublisher;
    private readonly ILogger<RegisterUserUseCase> _logger;

    public RegisterUserUseCase(IUserAuthService userAuthService,
        IUnitOfWork unitOfWork,
        IUserRepository userRepository,
        IUserCreatedEventPublisher userCreatedEventPublisher,
        ILogger<RegisterUserUseCase> logger)
    {
        _userAuthService = userAuthService;
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
        _userCreatedEventPublisher = userCreatedEventPublisher;
        _logger = logger;
    }

    public async Task<RegisterUserResponse> ExecuteAsync(RegisterUserRequest request)
    {
        try
        {
            _logger.LogInformation("Executing {UseCase}. Email={Email}", nameof(RegisterUserUseCase), request.Email);

            var name = new Name(request.FirstName, request.LastName);
            var email = new EmailAddress(request.Email);
            var role = UserRoles.user.ToString();
            await _unitOfWork.BeginTransactionAsync();
            
            var identityUserId = await _userAuthService.CreateUserAsync(request.Email, request.Password, role);
            
            var user = br.com.fiap.cloudgames.Domain.Aggregates.User.Create(name, email, identityUserId);
            await _userRepository.AddAsync(user);
        
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("User registered successfully. UserId={UserId}, Email={Email}", user.Id, user.Email.Email);

            _logger.LogInformation("Publishing UserCreated event. UserId={UserId}, Email={Email}", user.Id, user.Email.Email);
            await _userCreatedEventPublisher.PublishAsync(new Events.UserCreatedEvent() { 
                EventId = user.Id,
                UserId = user.Id,
                Name = user.Name.FullName,
                Email = user.Email.Email,
            });

            return new RegisterUserResponse()
            {
                Id = user.Id.ToString(),
                FirstName = user.Name.FirstName,
                LastName = user.Name.LastName,
                Email = user.Email.Email,
                Role = user.Role.ToString()
            };
        }
        catch(Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            _logger.LogError(ex, "Error executing {UseCase}. Email={Email}", nameof(RegisterUserUseCase), request.Email);
            throw;
        }
    }
    
    
}