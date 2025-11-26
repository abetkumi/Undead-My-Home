using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFireCracker : MonoBehaviour
{
    [SerializeField] AudioClip m_fireCrackerSE;
    [SerializeField] GameObject m_hitCollider;

    // Start is called before the first frame update
    void Awake()
    {
        m_hitCollider.SetActive(false);
    }

    //爆竹が使われたときの処理
    async public void Fire()
    {
        //調べられないようにする
        ItemObject item = gameObject.GetComponent<ItemObject>();
        item.SetIsCheck(true);

        //1秒待機
        await UniTask.Delay(1000);
        //ItemDrop関数でもう一度拾えるようになってしまうため
        item.SetIsCheck(true);
        //攻撃判定をオンにする
        m_hitCollider.SetActive(true);
        //SE
        GameManager.PlaySE(m_fireCrackerSE);
        await UniTask.Delay(2000);
        //オブジェクトを破壊
        Destroy(gameObject);
    }
}
