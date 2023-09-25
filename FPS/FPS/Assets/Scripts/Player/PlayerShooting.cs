using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;

public class PlayerShooting : NetworkBehaviour
{
    private const string PLAYER_TAG = "Player";

    [SerializeField]
    private WeaponManager weaponManager; // ��� WeaponManager ������

    private PlayerWeapon currentWeapon; // ��������ҵ�������Ϣ
    [SerializeField]
    private LayerMask layerMask; // �á��㡱�ĸ��������ֵ��˺Ͷ���
    [SerializeField]
    private Camera cam; // �ڱ༭�����ȡһ��������������Ҳ������ Start() �л�ȡ cam = GetComponentInChildren<Camera>(); // ����ж�� Camera �������ȡһ��
    enum HitEffectMaterial
    {
        Metal,
        Stone,
    }
    // Update is called once per frame
    void Update()
    {
        if (!IsLocalPlayer) return; // ���Ǳ�����������
        currentWeapon = weaponManager.GetCurrentWeapon();
        if(currentWeapon.shootRate <= 0) // �жϵ�ǰΪ����
        {
            if(Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        }
        else // �жϵ�ǰΪ����
        {
            if(Input.GetButtonDown("Fire1"))
            {
                InvokeRepeating("Shoot", 0f, 1f / currentWeapon.shootRate);
            }
            else if(Input.GetButtonUp("Fire1") || Input.GetKeyDown(KeyCode.Q)) // ֻ��ʶ���ɿ����ʱ��������Ч�������ǵ������л�������ʱ��ҲӦ�ý������Ч��
            {
                CancelInvoke("Shoot");
            }
        }
    }
    private void OnHit(Vector3 pos, Vector3 normal, HitEffectMaterial material) // ÿ�λ���ʱ��Ҫִ�е��߼�������ȷ��λ������Ⱦ������
    {
        GameObject hitEffectPrefab;
        if(material == HitEffectMaterial.Metal)
        {
            hitEffectPrefab = weaponManager.GetCurrentGraphics().metalHitEffectPrefabs; // �ҵ�ǹ�Ĺ�����Ч
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
    private void OnShoot() // ÿ�����ʱ��Ҫִ�е��߼���������Ч��������
    {
        weaponManager.GetCurrentGraphics().muzzleFlash.Play(); // ��������Ч���ҵ�ǹ����Ч������һ��
    }
    [ServerRpc]
    private void OnShootServerRpc()
    {
        if(!IsHost) OnShoot(); // ������� host ����Ҫ���� OnShoot()
        OnShootClientRpc();
    }
    [ClientRpc]
    private void OnShootClientRpc()
    {
        OnShoot();
    }

    private void Shoot()
    {
        OnShootServerRpc(); // ������û�л��ж�Ӧ��ִ�У��Ҳ�Ӧ��ֻ�����ڱ���
        // Debug.Log("Shooting!!");
        RaycastHit hit;
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, currentWeapon.range, layerMask))
        {
            // Debug.Log(hit.collider.name);
            // shootServerRpc(hit.collider.name); // ������˭������Ϣ���ݸ��������������ڷ�����������
            // ��������һ������֮���ж���������Ƿ������
            if(hit.collider.tag == PLAYER_TAG)
            {
                // ������Ҳ�ȥ��������������ѹ����������, �Լ���Ӧ�۳���Ѫ����Ϣ����������
                shootServerRpc(hit.collider.name, currentWeapon.damage); // ������˭������Ϣ���ݸ��������������ڷ�����������
                // ����ǽ�������
                OnHitServerRpc(hit.point, hit.normal, HitEffectMaterial.Metal);
            }
            else
            {
                // ������ʯͷ����
                OnHitServerRpc(hit.point, hit.normal, HitEffectMaterial.Stone);
            }
        }
    }
    // ���ڷ�������˵�����е���Ҷ���Զ����ң�����������ҵ�Playershooting�ű��������õ��ˣ�����P1����һǹ���ͻ���÷�������Playershooting�ű�
    [ServerRpc]
    private void shootServerRpc(string hitObjetcNmae, int damage) // ��Ҫ�� ServerRpc �����ã������������ ServerRpc �����׺��������ʵ����ұ�����֮�����÷������ɵ�����
    {
        GameManager.UpdateInfo(transform.name + " hit " + hitObjetcNmae); // �� server ��������������� client ��������� host �� server �� client �����Ի��� host �����
        Player player = GameManager.Singelton.GetPlayerName(hitObjetcNmae); // �ڷ������˵��ֵ�����ȡ������ͨ������������ҵ�����ȡ�������������������
        player.UnderAttack(damage); // �������˵��������������һ���ÿ�Ѫ�ĺ���
    }
}
