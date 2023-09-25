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
    void Start() // ֻ��Ҫ�ڿ�ʼʱ����һ�εĲ�������start�У�ÿ�붼��Ҫ���ж�εĲ����ŵ�update��
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
    // ���һ�£�������ĳһ����ť֮��Ͳ�����ʾȫ����ť
    private void destroyAllButton()
    {
        Destroy(HostBtn.gameObject);
        Destroy(ServerBtn.gameObject);
        Destroy(ClientBtn.gameObject);
    }


}
