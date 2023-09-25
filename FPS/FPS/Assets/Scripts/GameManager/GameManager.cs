using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Singelton; // ����ģʽ��ʲô��
    private static string info;
    // Ϊ�˷�����ԣ�����Ϊ�����ڷ��������ҵ�ÿ����ң���һ���ֵ����洢ÿ����Һ����ֵ�ӳ���ϵ
    // ����ֵ�����ÿ�����ڶ����еģ���������ÿ���ͻ��˶���һ���Լ��Ĵ������������Ϣ���ֵ䣬�ֵ����������������ÿ���ͻ��˶������
    private static Dictionary<string, Player> players = new Dictionary<string, Player>();

    private void Awake()
    {
        Singelton = this;
    }
    public void RegisterPlayer(string name, Player player) // ���ֵ������һ�����
    {
        player.name = name;
        players.Add(name, player);
    }
    public void UnRegisterPlayer(string name) // ɾ��һ�����
    {
        players.Remove(name);
    }
    // �������������޸�infoֵ
    public static void UpdateInfo(string _info)
    {
        info = _info;
    }
    public Player GetPlayerName(string name) // �������ƣ������������
    {
        return players[name];
    }
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(200f, 200f, 200, 400));
        GUILayout .BeginVertical(); // ����չʾ

        GUI.color = Color.red; // ������Ϣ����ɫ��Ϊ��ɫ
        foreach(string name in players.Keys)
        {
            Player player = GetPlayerName(name);
            GUILayout.Label(name + " Current Health: " + player.GetHealth());
        }

        GUILayout .EndVertical(); // ��ʼ����hh
        GUILayout.EndArea();
    }
}
