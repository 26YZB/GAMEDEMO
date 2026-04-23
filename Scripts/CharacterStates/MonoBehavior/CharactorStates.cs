using System;
using UnityEngine;

public class CharactorStates : MonoBehaviour
{
    public CharactorData_SO charactorData;
    public AtkData_SO AtkData;
    public bool isCritical;
    public event Action<int, int> updatecurruntHPbar;

    private int runtimeCurrentHealth;
    private bool runtimeHealthInitialized;

    void Awake()
    {
        InitRuntimeHealthFromData();
    }

    // SO 同步初始血量
    public void InitRuntimeHealthFromData()
    {
        if (charactorData == null)
            return;
        runtimeCurrentHealth = charactorData.CurruntHealth;
        runtimeHealthInitialized = true;
    }

    #region 从文件中读写数据
    public int MaxHealth
    {
        get
        {
            if (charactorData != null)
            {
                return charactorData.MaxHealth;
            }
            return 0;
        }

        set
        {
            if (charactorData != null)
                charactorData.MaxHealth = value;
        }
    }

    public int CurruntHealth
    {
        get
        {
            if (charactorData == null)
                return 0;
            if (Application.isPlaying && runtimeHealthInitialized)
                return runtimeCurrentHealth;
            return charactorData.CurruntHealth;
        }

        set
        {
            if (charactorData == null)
                return;
            if (Application.isPlaying)
            {
                if (!runtimeHealthInitialized)
                    InitRuntimeHealthFromData();
                runtimeCurrentHealth = Mathf.Max(0, value);
            }
            else
            {
                charactorData.CurruntHealth = value;
            }
        }
    }

    public int CurruntDefence
    {
        get
        {
            if (charactorData != null)
            {
                return charactorData.CurruntDefence;
            }
            return 0;
        }

        set
        {
            if (charactorData != null)
                charactorData.CurruntDefence = value;
        }
    }
    #endregion

    #region 角色攻击
    public void TakeDamage(CharactorStates attacker,CharactorStates defener)
    {
        if (attacker == null || attacker.AtkData == null || charactorData == null)
            return;

        int damage = attacker.CurruntDamage() - defener.CurruntDefence;
        if (damage<=0)
        {
            damage = 0;
        }
        CurruntHealth = Mathf.Max(CurruntHealth - damage, 0);

        updatecurruntHPbar?.Invoke(CurruntHealth, MaxHealth);
        if (CurruntHealth <= 0) 
            attacker.charactorData.UpdateExp(charactorData.killPoint);
    }

    private int CurruntDamage()
    {
        if (AtkData == null)
            return 0;

        float coreDamage = AtkData.CurruntAtk;
        if (isCritical)
        {
            coreDamage *= AtkData.criticalMutiplier;
        }
        return (int)coreDamage;
    }
    #endregion
}
