using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public class PlayerSearch : MonoBehaviour
{
    GameManager m_gameManager;
    UI_SearchCreater m_UIsearchCreater;
    [SerializeField] SphereCollider m_searchCollider;

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
        m_searchCollider.enabled = true;

        await UniTask.Delay(10);

        m_searchCollider.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        //サーチボタンが押されると
        if (Input.GetButtonDown("Search"))
        {
            CollisionONOFF();
        }
    }
}
