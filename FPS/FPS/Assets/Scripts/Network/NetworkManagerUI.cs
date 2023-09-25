using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField]
    private Button HostBtn;
    [SerializeField]
    private Button ServerBtn;
    [SerializeField]
    private Button ClientBtn;

    // Start is called before the first frame update
    void Start() // 只需要在开始时进行一次的操作放在start中，每秒都需要进行多次的操作放到update中
    {
        HostBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
            destroyAllButton();
        });
        ServerBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartServer();
            destroyAllButton();
        });
        ClientBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
            destroyAllButton();
        });
        
    }
    // 检测一下，当按下某一个按钮之后就不再显示全部按钮
    private void destroyAllButton()
    {
        Destroy(HostBtn.gameObject);
        Destroy(ServerBtn.gameObject);
        Destroy(ClientBtn.gameObject);
    }


}
