using UnityEngine;

public interface IFighter 
{
    public abstract void Attack(bool isChargeAttack);
    public abstract void Damage(Vector2 dir, float force, int playerID = 0);
}
