using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable] // �ܹ����л�
public class PlayerWeapon
{
    public string name = "QBZ";
    public int damage = 10;
    public float range = 100f;

    public float shootRate = 10f; // 1 ���ӿ�������ٷ��ӵ�����������С�ڵ��� 0 ����ʾ����

    public GameObject graphics;
}
