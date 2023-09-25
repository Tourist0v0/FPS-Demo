using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField]
    private int maxHealth = 100;
    //[SerializeField]
    //private PlayerWeapon weapon;
    [SerializeField]
    private Behaviour[] componentToDisabled;
    private bool[] componentEnabled;
    private bool colliderEnabled; // �����洢 collider �Ľ���״̬
    // ��һ��ȫ��ͬ���ı������洢��ҵ�Ѫ��
    private NetworkVariable<int> currentHealth = new NetworkVariable<int>(); // NetworkVariable ��ֻҪ���ڷ��������޸��������ֵ��Unity �ͻ��Զ�ͬ�����������пͻ��˵ı���
    // ��һ��ȫ��ͬ���ı����洢����Ƿ�ȥ����
    private NetworkVariable<bool> isDead = new NetworkVariable<bool>();
    // �ܲ��ܿ�һ��ȫ��ͬ���ı������洢��ҵĵ�ǰ������
    // private NetworkVariable<PlayerWeapon> currentWeapon = new NetworkVariable<PlayerWeapon>();
    public void SetUp()
    {
        // ��¼��ʼ״̬
        componentEnabled = new bool[componentToDisabled.Length];
        for(int i = 0; i < componentToDisabled.Length; i++)
        {
            componentEnabled[i] = componentToDisabled[i].enabled;
        }
        Collider collider = GetComponent<Collider>();
        colliderEnabled = collider.enabled;

        SetDefault();
    }
    private void SetDefault() // ������ʱ��Ҳ�����������
    {
        // �ָ�ԭ�ȵĿ���״̬
        for(int i = 0; i < componentToDisabled.Length; i++)
        {
            componentToDisabled[i].enabled = componentEnabled[i];
        }
        Collider collider = GetComponent<Collider>();
        collider.enabled = colliderEnabled;

        if (IsServer) // ֻ�ڷ��������޸����ֵ����Чͬ��������Ƿ������˵Ļ����³�ʼ�����Ѫ��
        {
            currentHealth.Value = maxHealth;
            isDead.Value = false; // ��ʼʱ����������ʱ������ǻ��ŵģ�
        }
    }
    public void UnderAttack(int damage) // ������ܵ��˺�ʱ���Լ����ã���ֻ�ڷ������˱����ã���Ϊ�������ֻ�ڷ������˱��޸Ĳ���Ч
    {
        if (isDead.Value) return; // ����������Ѿ������˾Ͳ�Ҫ�������ܵ��˺���
        currentHealth.Value -= damage;
        if (currentHealth.Value <= 0)
        {
            currentHealth.Value = 0; // ������ֵС����ʱ���������³� 0 
            isDead.Value = true;
            if(!IsHost) DieOnServer(); // �����host�Ļ����ͻ��ظ�ִ����
            DieClientRpc();// ֻд����Ļ� Die() �����ٷ�������ִ��
        }
    }
    private void DieOnServer()
    {
        Die();
    }
    [ClientRpc] // �ڷ������˵��� ClientRpc �ĺ����Ļ����ͻ���ÿһ���ͻ�����ִ���������
    private void DieClientRpc()
    {
        Die();
    }
    private void Die()
    {
        for (int i = 0; i < componentToDisabled.Length; i++)
        {
            componentToDisabled[i].enabled = false;
        }
        Collider collider = GetComponent<Collider>();
        collider.enabled = false;

        // ����ʱ��ֻ���������Ǹ�������ڵĿͻ��˿������»������Ŀ���Ȩ��ʣ�µ���ҵĿͻ���ֻ�ָܻ���ײ��⣬��Ϊԭ�Ⱦ��������ģ����ص�ԭ�ȵ�״̬
        StartCoroutine(Respawn()); // ��һ�����߳���ִ�� Respawn() ����
    }
    private IEnumerator Respawn()// ����
    {
        yield return new WaitForSeconds(5f); // ������ C++ ���е� sleep ����
        SetDefault();
        if (IsLocalPlayer) // ��Ϊʹ�õ��Ǳ��ؿͻ�������������ƶ������Ƿ������˿���������꣬���Բ������ؿͻ��˲���Ч
        {
            transform.position = new Vector3(0f, 10f, 0f); // ����ʱ�������
        }
    }

    public int GetHealth()
    {
        return currentHealth.Value;
    }

}
