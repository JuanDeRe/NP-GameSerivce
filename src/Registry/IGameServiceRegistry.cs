using GameService.Models;

namespace GameService.Registry;

public interface IGameServiceRegistry
{
    void RegisterOrRefresh(
        GameServiceInstance instance
    );

    void RemoveInstance(string instanceId);
}