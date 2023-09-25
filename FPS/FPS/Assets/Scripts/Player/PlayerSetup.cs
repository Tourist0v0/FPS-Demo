using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerSetup : NetworkBehaviour // �жϵ�ǰ����Ƿ�Ϊ���������ҪһЩapi���� MonoBehaviour ��Ϊ NetworkBehaviour
{   // �����жϵ�ǰ����Ƿ�Ϊ������ң�������Ǳ�����Ҷ���������ң�����Ҫ���õ�һЩ���������������ҵ�PlayerController��PlayerInput���������������
    [SerializeField]
    private Behaviour[] componentToDisabled;
    private Camera sceneCamera;

    // Start is called before the first frame update
    public override void OnNetworkSpawn() // ����ͬ���ĺ���������ʵ�ֵ�
    {
        base.OnNetworkSpawn();
        if(!IsLocalPlayer) // �ж�������Ǳ�����ң�����õ��������б��е�����
        {
            DisableComponents();
        }
        else
        {
            sceneCamera = Camera.main;
            if(sceneCamera != null)
            {
                sceneCamera.gameObject.SetActive(false);
            }
        }
        RegisterPlayer();
    }
    public override void OnNetworkDespawn() // ��һ���ͻ��˶Ͽ�����֮ǰ������������
    {
        base.OnNetworkDespawn();
        if (sceneCamera != null)
        {
            sceneCamera.gameObject.SetActive(true);
        }
        GameManager.Singelton.UnRegisterPlayer(transform.name);
    }
    // Ϊÿһ���������һ����һ�޶�������
    // private void SetPlayerName()
    // {
        // transform.name = "Player " + GetComponent<NetworkObject>().NetworkObjectId.ToString(); // ��֤���ڲ�ͬ��������ʾ���û�������ͬ
    // }
    private void RegisterPlayer()
    {
        string name = "Player " + GetComponent<NetworkObject>().NetworkObjectId.ToString();
        Player player = GetComponent<Player>();
        GameManager.Singelton.RegisterPlayer(name, player); // ʹ�� Singelton ʱ���⿪�ŵ�api����Ҫ�ĳ�static��֮ǰͨ�����������ú�������Ҫ��������Ϊstatic
        player.SetUp();
    }
    private void DisableComponents() // ��װ�������б�����
    {
        for (int i = 0; i < componentToDisabled.Length; i++)
        {
            componentToDisabled[i].enabled = false;
        }
    }
}
