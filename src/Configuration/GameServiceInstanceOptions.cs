namespace GameService.Configuration;

public class GameServiceInstanceOptions
{
    public string InstanceId { get; set; } =
        "game-service-local-01";

    public string InternalUrl { get; set; } =
        "http://localhost:5002";

    public int MaxMatches { get; set; } = 20;

    public int HeartbeatIntervalSeconds { get; set; } = 5;
}