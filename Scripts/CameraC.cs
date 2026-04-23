using UnityEngine;

public class CameraC : MonoBehaviour
{
    public Transform target; //摄像机跟随对象
    public Vector3 offset; //摄像机偏移
    public float Xspeed = 95f;
    public float Yspeed = 55f;
    public float distance = 4;//跟随距离
    public float zoomRate = 80;//鼠标滚动速度
    public float YMinlim = -15f;
    public float YMaxlim = 45f;
    public float disMinlim = 1.5f;//跟随最小距离
    public float disMaxlim = 5;//跟随最大距离
    public Vector3 rotateoffset = new Vector3(0, 0, -1);//旋转偏移
    float x;
    float y;
    Quaternion rotation;
    
    void Update()
    {
        if (InputC.instance == null || target == null)
            return;

        x += InputC.instance.MouseIput.x * Xspeed * Time.deltaTime;
        y -= InputC.instance.MouseIput.y * Yspeed * Time.deltaTime;

        //限制摄像机视角移动
        y = clampAngle(y, YMinlim, YMaxlim);
        rotation = Quaternion.Euler(y, x, 0);
        transform.rotation = rotation;

        //角色随鼠标移动进行视角变化
        if (InputC.instance.XIput != 0 || InputC.instance.YIput != 0)
        {
            target.rotation = Quaternion.Euler(0, x, 0);
        }

        //摄像机视角大小随鼠标滚轮滑动进行缩放
        distance -= InputC.instance.MouseIput.z * zoomRate * Time.deltaTime * Mathf.Abs(distance);
        distance = Mathf.Clamp(distance, disMinlim, disMaxlim);

        //摄像机视角跟随角色移动调整
        transform.position = target.position + offset + rotation * rotateoffset * distance;
    }

    float clampAngle(float angle,float min,float max) //限制角度
    {
        if (angle>360)
        {
            angle -= 360;
        }
        else if (angle<-360)
        {
            angle += 360;
        }
        return Mathf.Clamp(angle, min, max);
    }
}
