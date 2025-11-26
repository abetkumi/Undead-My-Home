using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public class PlayerSearch : MonoBehaviour
{
    //UIを作る側のスクリプト用変数
    UI_SearchCreater m_UIsearchCreater;
    //アイテムが範囲内にあるかを取得するコリジョン
    [SerializeField] GameObject m_searchCollider;
    //サーチする際のエフェクト
    [SerializeField] GameObject m_searchImageObject;

    private void Awake()
    {
        m_searchCollider.SetActive(false);
        m_searchImageObject.SetActive(false);
    }

    //範囲内に収集アイテムがある場合
    private void OnTriggerEnter(Collider other)
    {
        m_UIsearchCreater = other.GetComponent<UI_SearchCreater>();
        if (m_UIsearchCreater != null)
        {
            //金額を表示する
            m_UIsearchCreater.ShowValue();
        }
    }

    //サーチ用コリジョン
    async void CollisionONOFF()
    {
        //コリジョンとエフェクトをONにする
        m_searchCollider.SetActive(true);
        m_searchImageObject.SetActive(true);

        await UniTask.Delay(2200);

        //コリジョンとエフェクトをOFFにする
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
        if (Input.GetButtonUp("Search"))
        {
            //コリジョンをONにする
            CollisionONOFF();
        }
    }
}
