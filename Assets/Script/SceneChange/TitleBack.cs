using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleBack : MonoBehaviour
{
    [SerializeField] GameObject m_fadeCanvas;
    [SerializeField] GameObject m_gameOverLightObject;
    [SerializeField] AudioClip m_lightSE;
    bool m_titleBack = false;

    //ƒ^ƒCƒgƒ‹‚É–ß‚é
    async void TitleBackSence()
    {
        await UniTask.Delay(2000);
        if(m_gameOverLightObject != null)
        {
            Destroy(m_gameOverLightObject);
            GameManager.PlaySE(m_lightSE);
        }

        GameObject fadeObject = Instantiate(m_fadeCanvas);
        fadeObject.GetComponent<FadeScene>().FadeStart("TitleScene", Color.black, true);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_titleBack)
        {
            return;
        }
        if (Input.anyKeyDown)
        {
            m_titleBack = true;
            TitleBackSence();
        }
    }
}
