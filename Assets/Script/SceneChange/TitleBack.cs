using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleBack : MonoBehaviour
{
    [SerializeField] GameObject m_fadeCanvas;
    [SerializeField] GameObject m_gameOverLightObject;
    [SerializeField] AudioClip m_lightSE;

    async void TitleBackSence()
    {
        await UniTask.Delay(2000);
        if(m_gameOverLightObject != null)
        {
            Destroy(m_gameOverLightObject);
            GameManager.PlaySE(m_lightSE);
        }

        if (Input.anyKeyDown)
        {
            GameObject fadeObject = Instantiate(m_fadeCanvas);
            fadeObject.GetComponent<FadeScene>().FadeStart("TitleScene", Color.black, true);
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        TitleBackSence();
    }
}
