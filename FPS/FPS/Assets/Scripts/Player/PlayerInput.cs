using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField]
    private float sped = 5f;
    [SerializeField]
    private float sensitivity = 5f; // ���������
    [SerializeField]
    private float thrusterForce = 20f; // ����
    [SerializeField]
    private PlayerController controller; // ������������ù�����
    [SerializeField]
    private ConfigurableJoint joint;// ȡ���������

    // Start is called before the first frame update
    void Start()// ��Ϸ���ؽ�ȥʱִ�еĺ���
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()// ��Ϸһ���ӻử���ͼƬ��������ǻ�ÿ��ͼƬ֮ǰ����õĺ�����ÿһ֡��������������
    {
        float xMov = Input.GetAxisRaw("Horizontal"); // ÿһ֡��Ҫȥ��ȡ�����ƶ�����
        float yMov = Input.GetAxisRaw("Vertical"); // ��ȡ�ݷ�����ƶ�����

        Vector3 velocity = (transform.right * xMov + transform.forward * yMov).normalized * sped;
        controller.Move(velocity);

        // ��ȡ��ת��Ϣ
        float xMouse = Input.GetAxisRaw("Mouse X");
        float yMouse = Input.GetAxisRaw("Mouse Y");
        Vector3 yRotation = new Vector3(0f, xMouse, 0f) * sensitivity;
        Vector3 xRotation = new Vector3(-yMouse, 0f, 0f) * sensitivity;
        controller.Rotate(xRotation, yRotation);
        // ��ȡ��Ծ��Ϣ
        Vector3 force = Vector3.zero;
        if (Input.GetButton("Jump")) // unity Ĭ�������� jump Ϊ space �����ж�һ����û�а�ס����ո��
        {
            force = Vector3.up * thrusterForce;
            joint.yDrive = new JointDrive // ��ס�ո��ʱ���yDrive��գ�����ֻ����maximumForce�����ٶ�̫����
            {
                positionDamper = 0f,
                positionSpring = 0f,
                maximumForce = 0f
            };
        }
        else
        {
            joint.yDrive = new JointDrive // ����ס�ո��ʱ��ָ�ԭ��������
            {
                positionDamper = 0f,
                positionSpring = 20f,
                maximumForce = 40f
            };
        }
        controller.Thrust(force);
    }
}
