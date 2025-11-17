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

    async public void Fire()
    {
        await UniTask.Delay(1000);
        m_hitCollider.SetActive(true);
        GameManager.PlaySE(m_fireCrackerSE);
        await UniTask.Delay(2000);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
