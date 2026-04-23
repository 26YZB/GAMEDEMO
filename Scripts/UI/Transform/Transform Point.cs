using UnityEngine;

public class TransformPoint : MonoBehaviour
{
    public enum TransitionType
    { SameScene, DifferentScene }
    [Header("Transition Info")]

    public string sceneName;

    public TransitionType transitionType;
    public TransformDestination.DestinationTag destinationTag;

    private bool canTrans;

    void Update() //SceneControiler ´«ËÍ
    {
        if (Input.GetKeyUp(KeyCode.E) && canTrans && ScenesC.IsInitialized)
            ScenesC.Instance.TransitionToDestination(this);
    }


    void OnTriggerStay(Collider other)
    { 
        if (other.CompareTag("Player")) 
            canTrans = true; 
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            canTrans = false;
    }
}
