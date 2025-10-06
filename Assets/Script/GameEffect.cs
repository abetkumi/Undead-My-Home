using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GameEffect : MonoBehaviour
{
    [SerializeField] Volume m_globalVolume;
    ColorAdjustments m_colorAdjustments;

    //画面
    Color m_changeColor, m_startColor;
    float m_changeTimer = 0.0f;
    bool m_isChangeColor = false;

    //BGM
    AudioSource m_audioBGM;
    float m_changeVolume, m_startVolume;
    float m_changeVolumeTimer = 0.0f;
    bool m_isChangeVolume = false;

    // Start is called before the first frame update
    void Start()
    {
        //ColorAdjustmentsを取得
        m_globalVolume.profile.TryGet(out m_colorAdjustments);

        //自身にアタッチされたオーディオソースを取得
        m_audioBGM = GetComponent<AudioSource>();
        m_audioBGM.volume = 0.0f; //最初は無音
    }

    //画面の色を変更
    void ColorChange()
    {
        if (m_isChangeColor)
        {
            //色を設定
            m_colorAdjustments.colorFilter.
                Override(Color.Lerp(m_startColor, m_changeColor, m_changeTimer));

            m_changeTimer += Time.deltaTime;
            if(m_changeTimer >= 1.0f)
            {
                m_isChangeColor = false;
            }
        }

        //BGM音量を設定
        if (m_isChangeVolume)
        {
            //音量を設定
            m_audioBGM.volume = Mathf.Lerp(m_startVolume, m_changeVolume, m_changeVolumeTimer);

            m_changeVolumeTimer += Time.deltaTime;
            if (m_changeVolumeTimer >= 1.0f)
            {
                m_isChangeVolume = false;
            }
        }
    }

    //引数に指定した色に画面の色を緩やかに変化させる
    public void StartColorChange(Color changeColor)
    {
        //変更前の色と変更後の色を保存
        m_startColor = ((Color)m_colorAdjustments.colorFilter);
        m_changeColor = changeColor;

        m_changeTimer = 0.0f;
        m_isChangeColor = true;
    }

    //引数に指定した音量に緩やかに変化させる
    public void StartBGMVolume(float volume)
    {
        m_changeVolume = volume;
        m_startVolume = m_audioBGM.volume;

        //BGMを最初から再生する
        if (volume != 0.0f)
        {
            m_audioBGM.Play();
        }

        m_changeVolumeTimer = 0.0f;
        m_isChangeVolume = true;
    }

    //BGMを止める
    public void BGMStop()
    {
        m_audioBGM.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        ColorChange();
    }
}
