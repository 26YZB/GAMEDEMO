using UnityEngine;

[CreateAssetMenu(fileName = "NewData", menuName = "Charactor States/Atk")]
public class AtkData_SO : ScriptableObject
{
    public int CurruntAtk;

    public float AtkRange;

    public float criticalMutiplier;//±©»÷¼Ó³É

    public float criticalChance;//±©»÷ÂÊ
}
