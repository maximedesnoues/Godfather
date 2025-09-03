using UnityEngine;

public interface IFighter 
{
    public abstract void Attack();
    public abstract void Damage(Vector2 dir, float force);
}
