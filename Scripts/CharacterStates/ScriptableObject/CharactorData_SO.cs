using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Data",menuName = "Charactor States/Data")]
public class CharactorData_SO : ScriptableObject
{
    [Header("States Info")]

    public int MaxHealth;

    public int CurruntHealth;

    public int CurruntDefence;

    [Header("Kill")]

    public int killPoint;//获得经验值

    [Header("Level")]

    public int currentLevel;//当前等级
    
    public int maxLevel;//最大等级
    
    public int baseExp;//当前升级经验
    
    public int currentExp;//当前经验
    
    public float levelBuff;//等级数值提升倍率

    public void UpdateExp(int point) 
    {
        currentExp += point;
        if (currentExp >= baseExp)
            LeveUp();
    }

    public float LevelMultiplier
    { 
        get { return 1 + (currentLevel - 1) * levelBuff; } 
    }
    private void LeveUp()
    {
        //提升的数据方法
        currentLevel = Mathf.Clamp(currentLevel + 1,1, maxLevel); 
        baseExp += (int)(baseExp * LevelMultiplier);
        MaxHealth = (int)(MaxHealth * LevelMultiplier);
        CurruntHealth = MaxHealth;
    }
}
