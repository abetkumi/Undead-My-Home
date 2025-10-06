using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GameEffect : MonoBehaviour
{
    [SerializeField] Volume m_globalVolume;
    ColorAdjustments m_colorAdjustments;

    //���
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
        //ColorAdjustments���擾
        m_globalVolume.profile.TryGet(out m_colorAdjustments);

        //���g�ɃA�^�b�`���ꂽ�I�[�f�B�I�\�[�X���擾
        m_audioBGM = GetComponent<AudioSource>();
        m_audioBGM.volume = 0.0f; //�ŏ��͖���
    }

    //��ʂ̐F��ύX
    void ColorChange()
    {
        if (m_isChangeColor)
        {
            //�F��ݒ�
            m_colorAdjustments.colorFilter.
                Override(Color.Lerp(m_startColor, m_changeColor, m_changeTimer));

            m_changeTimer += Time.deltaTime;
            if(m_changeTimer >= 1.0f)
            {
                m_isChangeColor = false;
            }
        }

        //BGM���ʂ�ݒ�
        if (m_isChangeVolume)
        {
            //���ʂ�ݒ�
            m_audioBGM.volume = Mathf.Lerp(m_startVolume, m_changeVolume, m_changeVolumeTimer);

            m_changeVolumeTimer += Time.deltaTime;
            if (m_changeVolumeTimer >= 1.0f)
            {
                m_isChangeVolume = false;
            }
        }
    }

    //�����Ɏw�肵���F�ɉ�ʂ̐F���ɂ₩�ɕω�������
    public void StartColorChange(Color changeColor)
    {
        //�ύX�O�̐F�ƕύX��̐F��ۑ�
        m_startColor = ((Color)m_colorAdjustments.colorFilter);
        m_changeColor = changeColor;

        m_changeTimer = 0.0f;
        m_isChangeColor = true;
    }

    //�����Ɏw�肵�����ʂɊɂ₩�ɕω�������
    public void StartBGMVolume(float volume)
    {
        m_changeVolume = volume;
        m_startVolume = m_audioBGM.volume;

        //BGM���ŏ�����Đ�����
        if (volume != 0.0f)
        {
            m_audioBGM.Play();
        }

        m_changeVolumeTimer = 0.0f;
        m_isChangeVolume = true;
    }

    //BGM���~�߂�
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
