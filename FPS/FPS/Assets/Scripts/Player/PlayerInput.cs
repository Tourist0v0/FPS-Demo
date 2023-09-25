using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField]
    private float sped = 5f;
    [SerializeField]
    private float sensitivity = 5f; // 鼠标灵敏度
    [SerializeField]
    private float thrusterForce = 20f; // 推力
    [SerializeField]
    private PlayerController controller; // 把组件的引用拿过来？
    [SerializeField]
    private ConfigurableJoint joint;// 取组件的引用

    // Start is called before the first frame update
    void Start()// 游戏加载进去时执行的函数
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()// 游戏一秒钟会画多个图片，这个就是画每张图片之前会调用的函数，每一帧都会调用这个函数
    {
        float xMov = Input.GetAxisRaw("Horizontal"); // 每一帧都要去获取横向移动命令
        float yMov = Input.GetAxisRaw("Vertical"); // 获取纵方向的移动命令

        Vector3 velocity = (transform.right * xMov + transform.forward * yMov).normalized * sped;
        controller.Move(velocity);

        // 获取旋转信息
        float xMouse = Input.GetAxisRaw("Mouse X");
        float yMouse = Input.GetAxisRaw("Mouse Y");
        Vector3 yRotation = new Vector3(0f, xMouse, 0f) * sensitivity;
        Vector3 xRotation = new Vector3(-yMouse, 0f, 0f) * sensitivity;
        controller.Rotate(xRotation, yRotation);
        // 获取跳跃信息
        Vector3 force = Vector3.zero;
        if (Input.GetButton("Jump")) // unity 默认设置了 jump 为 space 键，判断一下有没有按住这个空格键
        {
            force = Vector3.up * thrusterForce;
            joint.yDrive = new JointDrive // 摁住空格的时候把yDrive清空，否则只降低maximumForce掉落速度太慢了
            {
                positionDamper = 0f,
                positionSpring = 0f,
                maximumForce = 0f
            };
        }
        else
        {
            joint.yDrive = new JointDrive // 不摁住空格的时候恢复原本的设置
            {
                positionDamper = 0f,
                positionSpring = 20f,
                maximumForce = 40f
            };
        }
        controller.Thrust(force);
    }
}
