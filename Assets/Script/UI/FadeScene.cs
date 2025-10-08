using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeScene : MonoBehaviour
{
    bool m_fadeStart = false;   //true�Ȃ��A�̏������J�n
    bool m_fadeMode = false;    //false�Ȃ�Â��Ȃ�true�Ȃ疾�邭�Ȃ�
    float m_alpha = 0.0f;       //�摜�̕s�����x

    [SerializeField]
    float FadeSpeed = 1.0f; //�t�F�[�h�̑��x

    // �J�ڐ�̃V�[������ۑ�
    string m_sceneName;
    //���g���g�p����Image��ۑ�
    Image m_image;
    //false�Ȃ�}�e���A�����g�ptrue�Ȃ�Image���g�p
    bool m_mode = false;

    //�t�F�[�h�J�n
    public void FadeStart(string sceneName, Color color, bool mode)
    {
        //�t�F�[�h�J�n�̏���������
        m_fadeStart = true;
        m_sceneName = sceneName;
        m_mode = mode;

        //�����̎q�I�u�W�F�N�g�ɃA�^�b�`����Ă���Image���擾����
        m_image = transform.GetChild(0).GetComponent<Image>();
        if (m_mode)
        {
            //�ʏ�t�F�[�h
            m_image.material = null;
            m_image.color = color;
        }
        else
        {
            //�}�e���A����������
            m_image.material.SetFloat("_Border", 0.0f);
            m_image.material.SetColor("_Color", color);
            //���g��RenderCamera�Ƀ��C���J������ݒ肷��
            GetComponent<Canvas>().worldCamera = Camera.main;
        }

        //���g�̓V�[�����܂����ł��폜����Ȃ��悤�ɂ���
        DontDestroyOnLoad(gameObject);
    }

    async void Fade()
    {
        //�t�F�[�h���J�n���Ă��Ȃ��Ȃ����f
        if (m_fadeStart == false)
        {
            return;
        }

        //���g��RenderCamera�Ƀ��C���J������ݒ肷��
        if (GetComponent<Canvas>().worldCamera == null &&
            m_mode == false)
        {
            GetComponent<Canvas>().worldCamera = Camera.main;
        }

        //�t�F�[�h����
        if (m_fadeMode == false)
        {
            //��ʂ��Â�����
            m_alpha += FadeSpeed * Time.deltaTime;
            //���S�ɈÂ��Ȃ����̂ŃV�[����ύX����
            if (m_alpha >= 1.0f)
            {
                //���C���Q�[���V�[���Ɉړ�����
                await SceneManager.LoadSceneAsync(m_sceneName).ToUniTask();
                Debug.Log("�Q�[���X�^�[�g!");  // ���O���o��
                //���邭���郂�[�h�ɕύX
                m_fadeMode = true;
            }
        }
        else
        {
            //��ʂ𖾂邭����
            m_alpha -= FadeSpeed * Time.deltaTime;
            //���S�ɖ��邭�Ȃ����̂Ŏ��g���폜����
            if (m_alpha <= 0.0f)
            {
                Destroy(gameObject);
            }
        }

        //�Ō�ɕs�����x�����ă`�X��
        if (m_mode)
        {
            Color nowColor = m_image.color;
            nowColor.a = m_alpha;
            m_image.color = nowColor;
        }
        else
        {
            m_image.material.SetFloat("_Border", m_alpha);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Fade();
    }
}
