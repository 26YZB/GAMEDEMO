using UnityEngine;
using UnityEngine.AI;

public enum ENEMYSTATES { ground, patrol, chase, dead }//µÐÈË×¤ÊØ£¬Ñ²Âß£¬×·»÷£¬ËÀÍö×´Ì¬
[RequireComponent(typeof(NavMeshAgent))]

public class EnemyC : MonoBehaviour
{
    private ENEMYSTATES enemystates;
    private NavMeshAgent agent;
    private float StightRadius = 5f; //µÐÈËÊÓÒ°·¶Î§
    public GameObject attacktarget;
    public bool isground;//µÐÈËÊÇ·ñÎª×¤ÊØµÐÈË
    private Animator animator;
    private Vector3 position;
    private CharactorStates charactorStates;
    private float lastAtktime;
    private bool attackDamageWindow;
    private bool hasHitThisAttackWindow;
    private bool isDeath;
    private CharactorStates playerStates;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        if (animator == null)
            animator = GetComponentInChildren<Animator>(true);
        position = transform.position;
        charactorStates = GetComponent<CharactorStates>();

        EnsureTriggerRigidbody(gameObject);
        foreach (var c in GetComponentsInChildren<Collider>())
        {
            if (c.isTrigger && c.gameObject != gameObject)
                EnsureTriggerRigidbody(c.gameObject);
        }
        var playerGo = GameObject.FindGameObjectWithTag("Player");
        if (playerGo != null)
        {
            attacktarget = playerGo;
            playerStates = playerGo.GetComponent<CharactorStates>();
        }
    }

    static bool IsAlive(CharactorStates s)
    {
        return s != null && s.charactorData != null && s.CurruntHealth > 0;
    }

    static void EnsureTriggerRigidbody(GameObject go)
    {
        var col = go.GetComponent<Collider>();
        if (col == null || !col.isTrigger)
            return;
        var rb = go.GetComponent<Rigidbody>();
        if (rb == null)
            rb = go.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    void Update()
    {
        StatesSwith();
        lastAtktime -= Time.deltaTime;
        PollProximityHitDuringWindow();
        if (charactorStates.CurruntHealth == 0)
        {
            isDeath = true;

        }
    }

    void StatesSwith()
    {
        if (isDeath)
        {
            enemystates = ENEMYSTATES.dead;
        }
        else if (!IsAlive(playerStates))
        {
            attacktarget = null;
            AttackDamageOff();
            if (agent != null && agent.enabled)
            {
                agent.isStopped = true;
                agent.ResetPath();
            }
            if (animator != null)
                animator.SetBool("Run", false);
            enemystates = isground ? ENEMYSTATES.ground : ENEMYSTATES.patrol;
        }
        else if (FoundPlayer())
        {
            enemystates = ENEMYSTATES.chase;
        }
        else if (enemystates == ENEMYSTATES.chase)
        {
            attacktarget = null;
            enemystates = isground ? ENEMYSTATES.ground : ENEMYSTATES.patrol;
            agent.SetDestination(position);
            if (animator != null)
                animator.SetBool("Run", true);
        }
        switch (enemystates)
        {
            case ENEMYSTATES.ground:
                UpdateRunByVelocity();
                break;
            case ENEMYSTATES.patrol:
                UpdateRunByVelocity();
                break;
            case ENEMYSTATES.chase:
                Echase();
                Attack();
                break;
            case ENEMYSTATES.dead:
                agent.enabled = false;
                animator.SetBool("Death",isDeath);
                Destroy(gameObject, 3);
                break;
        }
    }

    public void AttackDamageOn()
    {
        attackDamageWindow = true;
        hasHitThisAttackWindow = false;
    }

    public void AttackDamageOff()
    {
        attackDamageWindow = false;
    }

    void PollProximityHitDuringWindow()
    {
        if (!attackDamageWindow || hasHitThisAttackWindow)
            return;
        if (!IsAlive(playerStates))
            return;
        if (charactorStates == null || charactorStates.AtkData == null)
            return;
        if (attacktarget == null)
            return;
        if (!attacktarget.transform.root.CompareTag("Player"))
            return;
        if (Vector3.Distance(attacktarget.transform.position, transform.position) > charactorStates.AtkData.AtkRange)
            return;

        ApplyDamageToPlayerRoot(attacktarget.transform.root);
    }

    void OnTriggerEnter(Collider other)
    {
        TryDamagePlayer(other);
    }

    void OnTriggerStay(Collider other)
    {
        TryDamagePlayer(other);
    }

    public void NotifyHitbox(Collider other)
    {
        TryDamagePlayer(other);
    }

    void TryDamagePlayer(Collider other)
    {
        if (!attackDamageWindow || hasHitThisAttackWindow)
            return;
        if (!IsAlive(playerStates))
            return;
        if (other == null)
            return;
        if (!other.CompareTag("Player") && !other.transform.root.CompareTag("Player"))
            return;
        if (other.transform.root == transform.root)
            return;

        ApplyDamageToPlayerRoot(other.transform.root);
    }

    void ApplyDamageToPlayerRoot(Transform playerRoot)
    {
        if (charactorStates == null)
            return;
        var targetStates = playerRoot.GetComponentInChildren<CharactorStates>(true);
        if (targetStates == null)
            targetStates = playerRoot.GetComponent<CharactorStates>();
        if (!IsAlive(targetStates))
            return;

        if (targetStates != null)
        {
            targetStates.TakeDamage(charactorStates, targetStates);
            hasHitThisAttackWindow = true;
            return;
        }

        var pf = playerRoot.GetComponentInChildren<PlayerFight>(true);
        if (pf == null)
            pf = playerRoot.GetComponent<PlayerFight>();

        if (pf != null)
        {
            float amount = charactorStates.AtkData != null
                ? charactorStates.AtkData.CurruntAtk
                : 15f;
            pf.Hit(amount);
            hasHitThisAttackWindow = true;
        }
    }

    void Attack()//µÐÈË¹¥»÷×´Ì¬¶¯»­ÇÐ»»Âß¼­
    {
        if (attacktarget == null || charactorStates == null || animator == null)
            return;
        if (!IsAlive(playerStates))
            return;
        if (charactorStates.AtkData == null)
            return;
        if (TargetInAtkRange())
        {
            agent.isStopped = true;
            animator.SetBool("Run", false);
            if (lastAtktime < 0)
            {
                lastAtktime = 2;
                charactorStates.isCritical = UnityEngine.Random.value < charactorStates.AtkData.criticalChance;
                transform.LookAt(attacktarget.transform);
                animator.SetBool("IsCriticle", charactorStates.isCritical);
                AttackDamageOn();
                animator.SetTrigger("Atk");
            }
        }
        else
        {
            agent.isStopped = false;
            animator.SetBool("IsCriticle", false);
        }
    }

    bool FoundPlayer()
    {
        var Collideritems = Physics.OverlapSphere(transform.position, StightRadius);
        foreach (var item in Collideritems)
        {
            if (item.CompareTag("Player"))
            {
                var ps = item.GetComponentInParent<CharactorStates>();
                if (ps == null)
                    ps = item.GetComponent<CharactorStates>();
                if (!IsAlive(ps))
                    continue;
                attacktarget = item.gameObject;
                return true;
            }
        }
        return false;
    }

    bool TargetInAtkRange()
    {
        if (attacktarget != null && charactorStates.AtkData != null)
        {
            return Vector3.Distance(attacktarget.transform.position, transform.position) <= charactorStates.AtkData.AtkRange;
        }
        return false;
    }

    void Echase()
    {
        if (attacktarget == null)
        {
            agent.SetDestination(position);
            if (animator != null)
                animator.SetBool("Run", false);
            return;
        }

        if (TargetInAtkRange())
        {
            agent.isStopped = true;
            return;
        }

        agent.isStopped = false;
        agent.SetDestination(attacktarget.transform.position);
        if (animator != null)
            animator.SetBool("Run", true);
    }

    void UpdateRunByVelocity()
    {
        if (animator == null)
            return;
        bool isMoving = agent != null && agent.velocity.sqrMagnitude > 0.01f;
        animator.SetBool("Run", isMoving);
    }

    public void AtkOver()
    {
        AttackDamageOff();

        if (agent != null && IsAlive(playerStates))
            agent.isStopped = false;

        if (animator != null)
            animator.SetBool("IsCriticle", false);
    }
}
