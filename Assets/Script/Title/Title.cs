using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Title : MonoBehaviour
{
    [SerializeField] GameObject m_fadeCanvas;    //�t�F�[�h���o�p�I�u�W�F�N�g
    [SerializeField] Button m_focusButton_Start;
    [SerializeField] Button m_focusButton_End;

    bool m_sceneChange = false;
    // Start is called before the first frame update
    void Start()
    {
        m_focusButton_Start = m_focusButton_Start.GetComponent<Button>();
        m_focusButton_End = m_focusButton_End.GetComponent<Button>();
        m_focusButton_Start.Select();
    }

    public void OnClickStartButton()
    {
        //�V�[���؂�ւ����͉������Ȃ�
        if (m_sceneChange)
        {
            return;
        }

        EventSystem.current.SetSelectedGameObject(null);
        //�t�F�[�h���o�p�I�u�W�F�N�g�𐶐�
        GameObject fade = Instantiate(m_fadeCanvas);
        //���������I�u�W�F�N�g��FadeStart�֐����Ăяo��
        fade.GetComponent<FadeScene>().FadeStart("SampleScene",Color.black, false);

        m_sceneChange = true;
      
        
    }

    public void OnClickEndButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//�Q�[���v���C�I��
#else
    Application.Quit();//�Q�[���v���C�I��
#endif
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            
        }
    }
}
