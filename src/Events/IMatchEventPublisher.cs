using GameService.Contracts;

namespace GameService.Events;

public interface IMatchEventPublisher
{
    Task PublishMatchFinishedAsync(
        MatchFinishedEvent matchFinishedEvent
    );
}