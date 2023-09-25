using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // 真正作用到物体上
    // 需要获取物体的属性
    [SerializeField]
    private Rigidbody rb;
    [SerializeField]
    private Camera cam;

    private Vector3 velocity = Vector3.zero; // 还未获取到用户输入时，初始为 0 ; 每秒移动多少距离
    // 旋转：左右是旋转角色，上下是旋转摄像机
    private Vector3 xRotation = Vector3.zero; // x轴动，旋转摄像机
    private Vector3 yRotation = Vector3.zero; // y轴动，旋转角色

    private float cameraRotationTotal = 0f;
    [SerializeField]
    private float cameraRotationLimit = 85f;

    private Vector3 thrusterForce = Vector3.zero; // 向上的推力

    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
    }

    public void Rotate(Vector3 _xRotation, Vector3 _yRotation)
    {
        yRotation = _yRotation;
        xRotation = _xRotation;
    }
    public void Thrust(Vector3 _thrusterForce)
    {
        thrusterForce = _thrusterForce; 
    }
    private void PerformMovement()
    {
        if(velocity != Vector3.zero)
        {
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime); // 时间为相邻两次 FixedUpdate() 的执行间隔 
        }
        if(thrusterForce != Vector3.zero)
        {
            rb.AddForce(thrusterForce); // 为刚体作用一个力，作用0.02秒，即Time.fixedDeltaTime
        }
    }
    private void PerformRotation()
    {
        if(yRotation != Vector3.zero)
        {
            rb.transform.Rotate(yRotation);
        }
        
        if(xRotation != Vector3.zero)
        {
            // cam.transform.Rotate(xRotation);
            cameraRotationTotal += xRotation.x; // 计算累计转了多少度
            cameraRotationTotal = Mathf.Clamp(cameraRotationTotal,-cameraRotationLimit, cameraRotationLimit);
            cam.transform.localEulerAngles = new Vector3(cameraRotationTotal, 0f, 0f);
        }
    }

    // FixedUpdate()每秒执行 50 次
    private void FixedUpdate() // 当要模拟的物体是刚体的时候，一般不要用update而是使用fixupdate
    {
        // 浅了解一下 fixupdate 和 update 的区别，为什么刚体使用前者？
        // fixupdate 更新时间更均匀，常用于模拟物理轨迹
        PerformMovement();
        // 旋转
        PerformRotation();
    }
}
