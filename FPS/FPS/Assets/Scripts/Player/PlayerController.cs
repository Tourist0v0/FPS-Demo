using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // �������õ�������
    // ��Ҫ��ȡ���������
    [SerializeField]
    private Rigidbody rb;
    [SerializeField]
    private Camera cam;

    private Vector3 velocity = Vector3.zero; // ��δ��ȡ���û�����ʱ����ʼΪ 0 ; ÿ���ƶ����پ���
    // ��ת����������ת��ɫ����������ת�����
    private Vector3 xRotation = Vector3.zero; // x�ᶯ����ת�����
    private Vector3 yRotation = Vector3.zero; // y�ᶯ����ת��ɫ

    private float cameraRotationTotal = 0f;
    [SerializeField]
    private float cameraRotationLimit = 85f;

    private Vector3 thrusterForce = Vector3.zero; // ���ϵ�����

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
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime); // ʱ��Ϊ�������� FixedUpdate() ��ִ�м�� 
        }
        if(thrusterForce != Vector3.zero)
        {
            rb.AddForce(thrusterForce); // Ϊ��������һ����������0.02�룬��Time.fixedDeltaTime
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
            cameraRotationTotal += xRotation.x; // �����ۼ�ת�˶��ٶ�
            cameraRotationTotal = Mathf.Clamp(cameraRotationTotal,-cameraRotationLimit, cameraRotationLimit);
            cam.transform.localEulerAngles = new Vector3(cameraRotationTotal, 0f, 0f);
        }
    }

    // FixedUpdate()ÿ��ִ�� 50 ��
    private void FixedUpdate() // ��Ҫģ��������Ǹ����ʱ��һ�㲻Ҫ��update����ʹ��fixupdate
    {
        // ǳ�˽�һ�� fixupdate �� update ������Ϊʲô����ʹ��ǰ�ߣ�
        // fixupdate ����ʱ������ȣ�������ģ������켣
        PerformMovement();
        // ��ת
        PerformRotation();
    }
}
