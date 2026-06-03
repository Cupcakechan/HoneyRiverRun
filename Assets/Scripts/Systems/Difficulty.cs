/// Single source of truth for difficulty multipliers. Systems read these; the
/// DifficultyManager raises them per gate. Mirrors the WorldScroll pattern.
public static class Difficulty
{
    public static float ScrollMultiplier    = 1f;   // >1 = faster river
    public static float EnemyRateMultiplier  = 1f;   // >1 = enemies spawn more often
    public static float OrbRateMultiplier     = 1f;   // <1 = orbs spawn rarer

    public static void Reset()
    {
        ScrollMultiplier   = 1f;
        EnemyRateMultiplier = 1f;
        OrbRateMultiplier   = 1f;
    }
}