using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneShotAudioClip : MonoBehaviour
{
    AudioSource m_audioSource;
    bool m_isPlay = false;

    private void Awake()
    {
        //�V�[�����؂�ւ���Ă��폜����Ȃ��悤�ɂ���
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySE(AudioClip audioClip,
        float volume = 1.0f,
        float spatialBlend = 0.0f,
        float minDistance = float.MinValue,
        float maxDistance = float.MaxValue)
    {
        //�����ɃA�^�b�`����Ă���AudioSource���擾
        m_audioSource = GetComponent<AudioSource>();

        //�I�[�f�B�I�N���b�v��ݒ�
        m_audioSource.clip = audioClip;
        m_audioSource.volume = volume;
        m_audioSource.spatialBlend = spatialBlend;

        //������ݒ�
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
        //�Đ��t���O�������Ă��āA�I�[�f�B�I�\�[�X�̍Đ����I�������
        if (m_isPlay && m_audioSource.isPlaying == false)
        {
            //���g���폜����
            Destroy(gameObject);
        }
    }
}
