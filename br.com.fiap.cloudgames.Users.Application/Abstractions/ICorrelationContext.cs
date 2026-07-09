namespace br.com.fiap.cloudgames.Users.Application.Abstractions;

public interface ICorrelationContext
{
    public string CorrelationId { get; }
}