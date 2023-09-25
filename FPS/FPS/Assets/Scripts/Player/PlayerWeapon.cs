using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable] // 能够串行化
public class PlayerWeapon
{
    public string name = "QBZ";
    public int damage = 10;
    public float range = 100f;

    public float shootRate = 10f; // 1 秒钟可以射多少发子弹，如果这个数小于等于 0 ，表示单发

    public GameObject graphics;
}
