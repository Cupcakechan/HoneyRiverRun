/// Implemented by anything a Pollen Dart can damage (enemies, gates).
/// Lets the dart deal a hit without knowing the concrete type.
public interface IDamageable
{
    void TakeHit();
}