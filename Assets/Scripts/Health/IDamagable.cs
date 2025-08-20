using UnityEngine;

public interface IDamagable
{
    float MaxHealth { get; }

    public void TakeDamage(float _damage);

    public void TakeHealing(float _heal);

    public void Die();
}
