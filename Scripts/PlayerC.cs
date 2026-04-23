using UnityEngine;

public class PlayerC : MonoBehaviour
{
    private Animator animator;
    private CharacterController CC;
    public float MoveSpeed = 4.25f;
    public int combo = 0;
    private float combocurtime;
    private float combotime = 1.5f;
    private bool IsAtking;
    public CharactorStates charactorstates;
    [Tooltip("未挂 CharactorStates 时，武器对敌人造成的伤害")]
    public float weaponDamageFallback = 15f;
    [Tooltip("攻击判定球半径（以武器子物体为中心；无武器时用角色身前）")]
    [SerializeField] private float meleeHitRadius = 2f;
    [Tooltip("无 Weapon 子物体时，判定球相对角色向前偏移")]
    [SerializeField] private float meleeHitForward = 1f;
    [Tooltip("无 Weapon 子物体时，判定球相对角色向上偏移")]
    [SerializeField] private float meleeHitUp = 1f;

    private bool attackDamageWindow;
    private bool hasHitThisSwing;
    private bool dead;
    private Transform _weaponTransform;

    void Start()
    {
        animator = GetComponent<Animator>();
        CC = GetComponent<CharacterController>();
        charactorstates = GetComponent<CharactorStates>();
        var w = GetComponentInChildren<Weapon>(true);
        _weaponTransform = w != null ? w.transform : null;
    }

    void Update()
    {
        if (dead)
            return;
        if (charactorstates != null && charactorstates.charactorData != null && charactorstates.CurruntHealth <= 0)
        {
            dead = true;
            IsAtking = false;
            attackDamageWindow = false;
            if (animator != null)
            {
                animator.ResetTrigger("Atk");
                animator.SetInteger("Combo", 0);
                animator.SetFloat("Xinput", 0f);
                animator.SetFloat("Yinput", 0f);
                animator.SetBool("Death", true);
            }
            return;
        }

        Atk();
        Move();
        PollMeleeHitDuringAttack();
        ResetParmas();
    }

    private void Move()
    {
        if (animator == null || CC == null || InputC.instance == null)
            return;

        float xi = InputC.instance.XIput;
        float yi = InputC.instance.YIput;
        animator.SetFloat("Xinput", xi);
        animator.SetFloat("Yinput", yi);

        if (IsAtking)
            return;

        Vector3 dir = transform.TransformDirection(new Vector3(xi, 0, yi));
        dir.Normalize();
        CC.SimpleMove(dir * MoveSpeed);
    }

    private void Atk()
    {
        if (animator == null || InputC.instance == null)
            return;

        if (InputC.instance.Mouse0 && !IsAtking)
        {
            attackDamageWindow = true;
            hasHitThisSwing = false;
            animator.SetTrigger("Atk");
            animator.SetInteger("Combo", combo);
            combo++;
            if (combo >= 2)
                combo = 0;
            combocurtime = combotime;
            IsAtking = true;
        }
    }

    Vector3 GetMeleeProbeCenter()
    {
        if (_weaponTransform != null)
            return _weaponTransform.position;
        return transform.position + Vector3.up * meleeHitUp + transform.forward * meleeHitForward;
    }

    /// <summary>攻击窗口内每帧检测身前/武器附近敌人，避免仅依赖 Trigger 漏检。</summary>
    void PollMeleeHitDuringAttack()
    {
        if (!attackDamageWindow || hasHitThisSwing)
            return;

        var cols = Physics.OverlapSphere(GetMeleeProbeCenter(), meleeHitRadius, ~0, QueryTriggerInteraction.Collide);
        foreach (var col in cols)
        {
            if (col == null || col.transform.root == transform.root)
                continue;
            if (!col.transform.root.CompareTag("Enemy"))
                continue;
            TryApplyDamageToEnemy(col.transform.root);
            if (hasHitThisSwing)
                return;
        }
    }

    /// <summary>由 Weapon 的 Trigger 调用。</summary>
    public void HandleWeaponTrigger(Collider other)
    {
        if (!attackDamageWindow || hasHitThisSwing)
            return;
        if (other == null || other.transform.root == transform.root)
            return;
        if (!other.transform.root.CompareTag("Enemy"))
            return;

        TryApplyDamageToEnemy(other.transform.root);
    }

    void TryApplyDamageToEnemy(Transform enemyRoot)
    {
        var targetstates = enemyRoot.GetComponentInChildren<CharactorStates>(true);
        if (targetstates == null)
            targetstates = enemyRoot.GetComponent<CharactorStates>();
        if (targetstates == null)
            return;

        if (charactorstates != null && charactorstates.AtkData != null && targetstates.charactorData != null)
        {
            targetstates.TakeDamage(charactorstates, targetstates);
            hasHitThisSwing = true;
            return;
        }

        if (targetstates.charactorData != null)
        {
            int d = Mathf.RoundToInt(weaponDamageFallback);
            targetstates.CurruntHealth = Mathf.Max(targetstates.CurruntHealth - d, 0);
            hasHitThisSwing = true;
        }
    }

    public void AtkOver()
    {
        attackDamageWindow = false;
        IsAtking = false;
    }

    private void ResetParmas()
    {
        if (animator == null)
            return;

        if (combocurtime > 0f)
        {
            combocurtime -= Time.deltaTime;
            return;
        }

        if (combo != 0)
        {
            combo = 0;
            animator.SetInteger("Combo", combo);
        }
    }
}
