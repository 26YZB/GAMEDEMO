using UnityEngine;

//挂在玩家武器子物体 Trigger 上，转发给父级 PlayerC
public class Weapon : MonoBehaviour
{
    void Awake()
    {
        var rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    void OnTriggerEnter(Collider other)
    {
        Forward(other);
    }

    void OnTriggerStay(Collider other)
    {
        Forward(other);
    }

    void Forward(Collider other)
    {
        var pc = GetComponentInParent<PlayerC>();
        if (pc != null)
            pc.HandleWeaponTrigger(other);
    }
}
