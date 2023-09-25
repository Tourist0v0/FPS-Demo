using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class WeaponManager : NetworkBehaviour // ������Ҫ����ͨ�ŵģ�����Ҫ�ĳ� NetworkBehaviour
{
    [SerializeField]
    private PlayerWeapon primaryWeapon; // ������
    [SerializeField]
    private PlayerWeapon viceWeapon; // ������
    private PlayerWeapon currentWeapon; // ��ǰ������
    private WeaponGraphics currentGraphics; // ��ǰ��������Ч
    [SerializeField]
    private GameObject weaponHolder;
    // private NetworkVariable<PlayerWeapon> currentWeapon = new NetworkVariable<PlayerWeapon>();
    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        EquipWeapon(primaryWeapon); // ��ʼʱĬ��װ��������
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
    }
    public void EquipWeapon(PlayerWeapon _weapon) // װ����������
    {
        currentWeapon = _weapon;
        if(weaponHolder.transform.childCount > 0)
        {
            DestroyImmediate(weaponHolder.transform.GetChild(0).gameObject); // ��֮ǰװ���õ�����ɾ��
        }
        // ������������
        GameObject weaponObject = Instantiate(currentWeapon.graphics, weaponHolder.transform.position, weaponHolder.transform.rotation); // ���ó� weaponHolder �ĳ�ʼλ��

        weaponObject.transform.SetParent(weaponHolder.transform); // ����ȥ
        currentGraphics = weaponObject.GetComponent<WeaponGraphics>();
    }
    public WeaponGraphics GetCurrentGraphics()
    {
        return currentGraphics; // ���ص�ǰ����Ч
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
        if (!IsHost) ToggleWeapon(); // ����� Host ʱ����仰�ų���ToggleWeapon()�ͻ�ִ�����Σ����ֳ�����Ч�����ǲ��л������Ե� host ģʽʱ��ע�͵���仰
        ToggleWeaponClientRpc(); // �ڷ������˵��ò�����
    }
    // Update is called once per frame
    void Update() // �����л�����
    {
        if(IsLocalPlayer)
        {
            if(Input.GetKeyDown(KeyCode.Q))
            {
                ToggleWeaponServerRpc(); // ���� bug �������и�СBUG����һ������г���ǹ�󣬵ڶ�����Ҽ���ʱ����һ����һ���M16
                // �ܲ�����ͬ��Ѫ��һ��ͬ����ҵ�ǰʹ�õ����������л����������൱��Ѫ����������ȥ����ͬ��
            }
        }
    }

}
