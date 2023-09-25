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
    private bool colliderEnabled; // 用来存储 collider 的禁用状态
    // 开一个全局同步的变量来存储玩家的血量
    private NetworkVariable<int> currentHealth = new NetworkVariable<int>(); // NetworkVariable 是只要你在服务器端修改这个变量值，Unity 就会自动同步给其他所有客户端的变量
    // 开一个全局同步的变量存储玩家是否去世了
    private NetworkVariable<bool> isDead = new NetworkVariable<bool>();
    // 能不能开一个全局同步的变量来存储玩家的当前的武器
    // private NetworkVariable<PlayerWeapon> currentWeapon = new NetworkVariable<PlayerWeapon>();
    public void SetUp()
    {
        // 记录初始状态
        componentEnabled = new bool[componentToDisabled.Length];
        for(int i = 0; i < componentToDisabled.Length; i++)
        {
            componentEnabled[i] = componentToDisabled[i].enabled;
        }
        Collider collider = GetComponent<Collider>();
        colliderEnabled = collider.enabled;

        SetDefault();
    }
    private void SetDefault() // 重生的时候也调用这个函数
    {
        // 恢复原先的控制状态
        for(int i = 0; i < componentToDisabled.Length; i++)
        {
            componentToDisabled[i].enabled = componentEnabled[i];
        }
        Collider collider = GetComponent<Collider>();
        collider.enabled = colliderEnabled;

        if (IsServer) // 只在服务器端修改这个值会有效同步，如果是服务器端的话重新初始化最大血量
        {
            currentHealth.Value = maxHealth;
            isDead.Value = false; // 初始时，或者重生时，玩家是活着的！
        }
    }
    public void UnderAttack(int damage) // 当玩家受到伤害时被自己调用，且只在服务器端被调用，因为这个变量只在服务器端被修改才有效
    {
        if (isDead.Value) return; // 如果这个玩家已经死亡了就不要再让他受到伤害了
        currentHealth.Value -= damage;
        if (currentHealth.Value <= 0)
        {
            currentHealth.Value = 0; // 当生命值小于零时，将它更新成 0 
            isDead.Value = true;
            if(!IsHost) DieOnServer(); // 如果是host的话，就会重复执行了
            DieClientRpc();// 只写这个的话 Die() 不会再服务器上执行
        }
    }
    private void DieOnServer()
    {
        Die();
    }
    [ClientRpc] // 在服务器端调用 ClientRpc 的函数的话，就会在每一个客户端上执行这个函数
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

        // 重生时，只有死亡的那个玩家所在的客户端可以重新获得人物的控制权，剩下的玩家的客户端只能恢复碰撞检测，因为原先就是这样的，返回到原先的状态
        StartCoroutine(Respawn()); // 开一个新线程来执行 Respawn() 函数
    }
    private IEnumerator Respawn()// 重生
    {
        yield return new WaitForSeconds(5f); // 类似于 C++ 当中的 sleep 函数
        SetDefault();
        if (IsLocalPlayer) // 因为使用的是本地客户端来控制玩家移动，而非服务器端控制玩家坐标，所以操作本地客户端才有效
        {
            transform.position = new Vector3(0f, 10f, 0f); // 出生时从天而降
        }
    }

    public int GetHealth()
    {
        return currentHealth.Value;
    }

}
