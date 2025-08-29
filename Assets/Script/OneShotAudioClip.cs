using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneShotAudioClip : MonoBehaviour
{
    AudioSource m_audioSource;
    bool m_isPlay = false;

    private void Awake()
    {
        //シーンが切り替わっても削除されないようにする
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySE(AudioClip audioClip,
        float volume = 1.0f,
        float spatialBlend = 0.0f,
        float minDistance = float.MinValue,
        float maxDistance = float.MaxValue)
    {
        //自分にアタッチされているAudioSourceを取得
        m_audioSource = GetComponent<AudioSource>();

        //オーディオクリップを設定
        m_audioSource.clip = audioClip;
        m_audioSource.volume = volume;
        m_audioSource.spatialBlend = spatialBlend;

        //距離を設定
        if (minDistance != float.MinValue)
        {
            m_audioSource.minDistance = minDistance;
        }
        if (maxDistance != float.MinValue)
        {
            m_audioSource.maxDistance = maxDistance;
        }

        m_audioSource.Play();

        m_isPlay = true;
    }

    // Update is called once per frame
    void Update()
    {
        //再生フラグが立っていて、オーディオソースの再生が終わったら
        if (m_isPlay && m_audioSource.isPlaying == false)
        {
            //自身を削除する
            Destroy(gameObject);
        }
    }
}
