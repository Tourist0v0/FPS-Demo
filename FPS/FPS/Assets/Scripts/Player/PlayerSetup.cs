using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerSetup : NetworkBehaviour // 判断当前玩家是否为本地玩家需要一些api，将 MonoBehaviour 换为 NetworkBehaviour
{   // 用于判断当前玩家是否为本地玩家，如果不是本地玩家而是网络玩家，则需要禁用掉一些组件，比如网络玩家的PlayerController、PlayerInput和摄像机、监听器
    [SerializeField]
    private Behaviour[] componentToDisabled;
    private Camera sceneCamera;

    // Start is called before the first frame update
    public override void OnNetworkSpawn() // 联网同步的函数是它来实现的
    {
        base.OnNetworkSpawn();
        if(!IsLocalPlayer) // 判断如果不是本地玩家，则禁用掉“禁用列表”中的内容
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
    public override void OnNetworkDespawn() // 在一个客户端断开连接之前会调用这个函数
    {
        base.OnNetworkDespawn();
        if (sceneCamera != null)
        {
            sceneCamera.gameObject.SetActive(true);
        }
        GameManager.Singelton.UnRegisterPlayer(transform.name);
    }
    // 为每一个玩家设置一个独一无二的名字
    // private void SetPlayerName()
    // {
        // transform.name = "Player " + GetComponent<NetworkObject>().NetworkObjectId.ToString(); // 保证了在不同窗口中显示的用户名字相同
    // }
    private void RegisterPlayer()
    {
        string name = "Player " + GetComponent<NetworkObject>().NetworkObjectId.ToString();
        Player player = GetComponent<Player>();
        GameManager.Singelton.RegisterPlayer(name, player); // 使用 Singelton 时对外开放的api不需要改成static，之前通过类名来调用函数都需要将函数改为static
        player.SetUp();
    }
    private void DisableComponents() // 封装“禁用列表”函数
    {
        for (int i = 0; i < componentToDisabled.Length; i++)
        {
            componentToDisabled[i].enabled = false;
        }
    }
}
