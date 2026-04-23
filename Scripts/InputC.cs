using UnityEngine;

public class InputC : MonoBehaviour
{
    public float XIput;
    public float YIput;
    public Vector3 MouseIput;
    public bool Mouse0;


    public static InputC instance;
    private void Awake()
    {
        instance = this;
    }

    void Update()
    {
        XIput = Input.GetAxis("Horizontal");
        YIput = Input.GetAxis("Vertical");
        MouseIput.Set(Input.GetAxis("Mouse X"),Input.GetAxis("Mouse Y"),Input.GetAxis("Mouse ScrollWheel"));
        Mouse0 = Input.GetMouseButtonDown(0);
    }
}
