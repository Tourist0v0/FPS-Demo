using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponGraphics : MonoBehaviour
{
    public ParticleSystem muzzleFlash; // 开枪特效，在开枪时播放一次
    public GameObject metalHitEffectPrefabs; // 后面需要动态生成的击中金属的特效
    public GameObject stoneHitEffectPrefabs; // 后面需要动态生成的击中石头的特效
}
