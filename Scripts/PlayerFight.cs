using UnityEngine;

/// <summary>
/// 玩家受击接口，供敌人武器等调用。可挂在与 CharacterController 同一物体或父物体上。
/// </summary>
public class PlayerFight : MonoBehaviour, FightInterface
{
    public float maxHp = 100f;
    public float damageFromEnemy = 15f;
    float hp;

    void Start()
    {
        hp = maxHp;
    }

    public void Hit()
    {
        Hit(damageFromEnemy);
    }

    public void Hit(float amount)
    {
        hp -= amount;
        if (hp <= 0f)
            hp = 0f;
    }

    public float CurrentHp => hp;
}
