using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public class PlayerSearch : MonoBehaviour
{
    GameManager m_gameManager;
    UI_SearchCreater m_UIsearchCreater;
    [SerializeField] GameObject m_searchCollider;
    [SerializeField] GameObject m_searchImageObject;

    //範囲内に収集アイテムがある場合
    private void OnTriggerEnter(Collider other)
    {
        m_UIsearchCreater = other.GetComponent<UI_SearchCreater>();
        if (m_UIsearchCreater != null)
        {
            m_UIsearchCreater.ShowValue();
        }
    }

    //サーチ用コリジョン
    async void CollisionONOFF()
    {
        m_searchCollider.SetActive(true);

        await UniTask.Delay(2200);

        m_searchCollider.SetActive(false);

        m_searchImageObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_searchCollider.gameObject.activeSelf == true)
        {
            return;
        }

        //サーチボタンが押されると
        if (Input.GetButtonDown("Search"))
        {
            CollisionONOFF();
            m_searchImageObject.SetActive(true);
        }
    }
}
