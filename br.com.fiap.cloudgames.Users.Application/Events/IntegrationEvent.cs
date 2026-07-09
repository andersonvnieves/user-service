namespace br.com.fiap.cloudgames.Users.Application.Events;

public abstract class IntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public string CorrelationId { get; init; } = String.Empty;
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}