using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Singelton; // 单例模式是什么？
    private static string info;
    // 为了方便调试，并且为了能在服务器上找到每个玩家，开一个字典来存储每个玩家和名字的映射关系
    // 这个字典是在每个窗口都会有的，服务器和每个客户端都有一个自己的储存玩家名字信息的字典，字典在新玩家联网后在每个客户端都会更新
    private static Dictionary<string, Player> players = new Dictionary<string, Player>();

    private void Awake()
    {
        Singelton = this;
    }
    public void RegisterPlayer(string name, Player player) // 向字典里加入一个玩家
    {
        player.name = name;
        players.Add(name, player);
    }
    public void UnRegisterPlayer(string name) // 删除一个玩家
    {
        players.Remove(name);
    }
    // 创建辅助函数修改info值
    public static void UpdateInfo(string _info)
    {
        info = _info;
    }
    public Player GetPlayerName(string name) // 根据名称，返回这名玩家
    {
        return players[name];
    }
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(200f, 200f, 200, 400));
        GUILayout .BeginVertical(); // 竖排展示

        GUI.color = Color.red; // 调试信息的颜色改为红色
        foreach(string name in players.Keys)
        {
            Player player = GetPlayerName(name);
            GUILayout.Label(name + " Current Health: " + player.GetHealth());
        }

        GUILayout .EndVertical(); // 有始有终hh
        GUILayout.EndArea();
    }
}
