/// Implemented by anything the generic Spawner activates from its pool.
/// Called right after the object is set active so it can configure itself
/// (position, variant, animation) — kept out of OnEnable on purpose.
public interface ISpawnable
{
    void OnSpawned();
}