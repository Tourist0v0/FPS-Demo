using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class WeaponManager : NetworkBehaviour // 凡是需要网络通信的，都需要改成 NetworkBehaviour
{
    [SerializeField]
    private PlayerWeapon primaryWeapon; // 主武器
    [SerializeField]
    private PlayerWeapon viceWeapon; // 副武器
    private PlayerWeapon currentWeapon; // 当前的武器
    private WeaponGraphics currentGraphics; // 当前的武器特效
    [SerializeField]
    private GameObject weaponHolder;
    // private NetworkVariable<PlayerWeapon> currentWeapon = new NetworkVariable<PlayerWeapon>();
    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        EquipWeapon(primaryWeapon); // 开始时默认装备主武器
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
    }
    public void EquipWeapon(PlayerWeapon _weapon) // 装备武器函数
    {
        currentWeapon = _weapon;
        if(weaponHolder.transform.childCount > 0)
        {
            DestroyImmediate(weaponHolder.transform.GetChild(0).gameObject); // 将之前装备好的武器删掉
        }
        // 将武器画出来
        GameObject weaponObject = Instantiate(currentWeapon.graphics, weaponHolder.transform.position, weaponHolder.transform.rotation); // 设置成 weaponHolder 的初始位置

        weaponObject.transform.SetParent(weaponHolder.transform); // 挂上去
        currentGraphics = weaponObject.GetComponent<WeaponGraphics>();
    }
    public WeaponGraphics GetCurrentGraphics()
    {
        return currentGraphics; // 返回当前的特效
    }
    public PlayerWeapon GetCurrentWeapon()
    {
        return currentWeapon;
    }
    private void ToggleWeapon()
    {
        if (currentWeapon == primaryWeapon)
        {
            EquipWeapon(viceWeapon);
        }
        else
        {
            EquipWeapon(primaryWeapon);
        }
    }
    [ClientRpc]
    private void ToggleWeaponClientRpc()
    {
        ToggleWeapon();
    }
    [ServerRpc]
    private void ToggleWeaponServerRpc()
    {
        if (!IsHost) ToggleWeapon(); // 如果是 Host 时把这句话放出来ToggleWeapon()就会执行两次，呈现出来的效果就是不切换，所以当 host 模式时，注释掉这句话
        ToggleWeaponClientRpc(); // 在服务器端调用才有用
    }
    // Update is called once per frame
    void Update() // 监听切换操作
    {
        if(IsLocalPlayer)
        {
            if(Input.GetKeyDown(KeyCode.Q))
            {
                ToggleWeaponServerRpc(); // 存在 bug ：这里有个小BUG？第一个玩家切成手枪后，第二个玩家加入时看第一个玩家还是M16
                // 能不能像同步血量一样同步玩家当前使用的武器？当切换武器，就相当于血量减少那样去网络同步
            }
        }
    }

}
