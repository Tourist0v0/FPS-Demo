using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;

public class PlayerShooting : NetworkBehaviour
{
    private const string PLAYER_TAG = "Player";

    [SerializeField]
    private WeaponManager weaponManager; // 添加 WeaponManager 的引用

    private PlayerWeapon currentWeapon; // 用这个来找到武器信息
    [SerializeField]
    private LayerMask layerMask; // 用“层”的概念来区分敌人和队友
    [SerializeField]
    private Camera cam; // 在编辑器里获取一下摄像机的组件，也可以在 Start() 中获取 cam = GetComponentInChildren<Camera>(); // 如果有多个 Camera 会随机获取一个
    enum HitEffectMaterial
    {
        Metal,
        Stone,
    }
    // Update is called once per frame
    void Update()
    {
        if (!IsLocalPlayer) return; // 不是本地玩家则忽略
        currentWeapon = weaponManager.GetCurrentWeapon();
        if(currentWeapon.shootRate <= 0) // 判断当前为单发
        {
            if(Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        }
        else // 判断当前为连发
        {
            if(Input.GetButtonDown("Fire1"))
            {
                InvokeRepeating("Shoot", 0f, 1f / currentWeapon.shootRate);
            }
            else if(Input.GetButtonUp("Fire1") || Input.GetKeyDown(KeyCode.Q)) // 只有识别到松开鼠标时会解除连击效果，但是当我们切换武器的时候也应该解除连击效果
            {
                CancelInvoke("Shoot");
            }
        }
    }
    private void OnHit(Vector3 pos, Vector3 normal, HitEffectMaterial material) // 每次击中时需要执行的逻辑，在正确的位置上渲染出弹孔
    {
        GameObject hitEffectPrefab;
        if(material == HitEffectMaterial.Metal)
        {
            hitEffectPrefab = weaponManager.GetCurrentGraphics().metalHitEffectPrefabs; // 找到枪的攻击特效
        }
        else
        {
            hitEffectPrefab= weaponManager.GetCurrentGraphics().stoneHitEffectPrefabs;  
        }
        GameObject hitEffectObject = Instantiate(hitEffectPrefab, pos, Quaternion.LookRotation(normal));
        ParticleSystem particleSystem = hitEffectObject.GetComponent<ParticleSystem>();
        particleSystem.Emit(1);
        particleSystem.Play();
        Destroy(hitEffectObject, 2f);
    }
    [ServerRpc]
    private void OnHitServerRpc(Vector3 pos, Vector3 normal, HitEffectMaterial material)
    {
        if (!IsHost) OnHit(pos, normal, material);
        OnHitClientRpc(pos, normal, material);
    }
    [ClientRpc]
    private void OnHitClientRpc(Vector3 pos, Vector3 normal, HitEffectMaterial material)
    {
        OnHit(pos, normal, material);
    }
    private void OnShoot() // 每次射击时需要执行的逻辑，包括特效，声音等
    {
        weaponManager.GetCurrentGraphics().muzzleFlash.Play(); // 在武器特效中找到枪口特效，播放一遍
    }
    [ServerRpc]
    private void OnShootServerRpc()
    {
        if(!IsHost) OnShoot(); // 如果不是 host 才需要调用 OnShoot()
        OnShootClientRpc();
    }
    [ClientRpc]
    private void OnShootClientRpc()
    {
        OnShoot();
    }

    private void Shoot()
    {
        OnShootServerRpc(); // 不管有没有击中都应该执行，且不应该只出现在本地
        // Debug.Log("Shooting!!");
        RaycastHit hit;
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, currentWeapon.range, layerMask))
        {
            // Debug.Log(hit.collider.name);
            // shootServerRpc(hit.collider.name); // 打中了谁，把信息传递给服务器，代码在服务器上运行
            // 当命中了一个物体之后，判断这个物体是否是玩家
            if(hit.collider.tag == PLAYER_TAG)
            {
                // 打到了玩家才去调用这个函数，把攻击到的玩家, 以及对应扣除的血量信息传给服务器
                shootServerRpc(hit.collider.name, currentWeapon.damage); // 打中了谁，把信息传递给服务器，代码在服务器上运行
                // 玩家是金属材料
                OnHitServerRpc(hit.point, hit.normal, HitEffectMaterial.Metal);
            }
            else
            {
                // 其他是石头材料
                OnHitServerRpc(hit.point, hit.normal, HitEffectMaterial.Stone);
            }
        }
    }
    // 对于服务器来说，所有的玩家都是远程玩家，所以所有玩家的Playershooting脚本都被禁用掉了，而当P1开了一枪，就会调用服务器上Playershooting脚本
    [ServerRpc]
    private void shootServerRpc(string hitObjetcNmae, int damage) // 想要让 ServerRpc 起作用，函数必须加上 ServerRpc 这个后缀，函数内实现玩家被击中之后想让服务器干的事情
    {
        GameManager.UpdateInfo(transform.name + " hit " + hitObjetcNmae); // 在 server 上输出，而不是在 client 上输出，而 host 是 server 和 client ，所以会在 host 上输出
        Player player = GameManager.Singelton.GetPlayerName(hitObjetcNmae); // 在服务器端的字典里面取出来，通过被攻击的玩家的名字取出来这名被攻击的玩家
        player.UnderAttack(damage); // 服务器端的这个被攻击的玩家会调用扣血的函数
    }
}
