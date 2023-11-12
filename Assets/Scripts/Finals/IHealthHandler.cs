namespace Finals {
public interface IHealthHandler {
    public Team TeamType { get; }
    public float HP { get; }
    public float Damage { get; }
    public void ApplyDamage(float damage);
}
}