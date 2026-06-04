/// Shared river state read by spawners. True while the river is in a narrow
/// checkpoint stretch (taper-in -> checkpoint -> taper-out), where the wide-channel
/// spawn ranges would drop things into the grass. The RiverStreamer maintains it.
public static class RiverState
{
    public static bool InCheckpointStretch;
}