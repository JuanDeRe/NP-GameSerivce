namespace GameService.Models;

public class PlayerState
{
    public string PlayerId { get; set; } = string.Empty;

    public PlayerRole Role { get; set; }
    
    
    public AnimatronicType AnimatronicType { get; set; } = AnimatronicType.None;

    public Position Position { get; set; } = new();

    public string CurrentRoomId { get; set; } = string.Empty;

    public bool IsConnected { get; set; } = true;
    
    public bool IsAttacking { get; set; }

    public string? ActiveAttackZoneId { get; set; }

    public float AttackProgress { get; set; }
    
    public int CollectedItems { get; set; }

    public int RequiredItems { get; set; } = 5;

    public bool IsEmpowered { get; set; }
    
    public CollectibleType? RequiredCollectibleType { get; set; }
}
