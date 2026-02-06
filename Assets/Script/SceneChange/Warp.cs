using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warp : MonoBehaviour
{
    [SerializeField] GameObject m_fadeCanvas;
    [SerializeField] GameObject m_warpPosition;
    [SerializeField] AudioClip m_warpSE;
    bool m_inArea = false;
    public bool m_fade = false;

    // Start is called before the first frame update
    void Start()
    {
        m_inArea = false;
        m_fade = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_inArea = true;
            m_fade = true;
        }
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    m_inArea = false;
    //}

    async void WarpPosition()
    {
        m_fade = false;
        // シーン切替
        // フェード演出用オブジェクトを生成
        GameObject fadeObject = Instantiate(m_fadeCanvas);
        // 生成したオブジェクトのFadeStart関数を呼び出す
        fadeObject.GetComponent<FadeScene>().FadeStart(m_warpPosition.transform.position,Color.black, true);
        GameManager.PlaySE(m_warpSE);
        //自身はシーンをまたいでも削除されないようにする
        DontDestroyOnLoad(fadeObject);
        await UniTask.Delay(1000);
        PlayerRotation();
    }

    public void PlayerRotation()
    {
        Rigidbody rb = GameObject.FindWithTag("Player").GetComponent<Rigidbody>();
        if (gameObject == null) return;

        // 相手の水平角度（Y軸だけ）を取得
        float targetY = m_warpPosition.transform.eulerAngles.y;

        // 現在のプレイヤー角度
        Vector3 currentEuler = rb.rotation.eulerAngles;

        // Y軸だけ置き換える
        Quaternion newRot = Quaternion.Euler(
            currentEuler.x,   // Xそのまま
            targetY,          // Yだけ相手と同じ
            currentEuler.z    // Zそのまま
        );

        rb.MoveRotation(newRot);
    }

    // Update is called once per frame
    void Update()
    {
        if(m_inArea && m_fade)
        {
            WarpPosition();
        }  
    }
}
