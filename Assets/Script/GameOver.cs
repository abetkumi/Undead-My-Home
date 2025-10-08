using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    [SerializeField] GameObject m_fadeCanvas;
    [SerializeField] GameObject m_playerObject;
    [SerializeField] GameObject m_cameraObject;
    bool m_isGameOver = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        SetGameOver();
        Debug.Log("Dead");
    }

    //�Q�[���I�[�o�[����
    async void SetGameOver()
    {
        //�Q�[���}�l�[�W���[���擾
        GameManager m_gameManager = 
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        m_gameManager.SetGameState(GameManager.GameState.enGameState_GameOver);

        Vector3 m_camaraPos = m_playerObject.transform.position;
        m_camaraPos.y += 4.0f;
        m_camaraPos += m_playerObject.transform.forward * 3.0f;
        m_cameraObject.transform.position = m_camaraPos;
        Camera.main.GetComponent<GameCamera>().FocusStart(m_playerObject.transform.position, 3.0f, 5.0f);
        m_playerObject.GetComponent<Rigidbody>().velocity = Vector3.zero;

        await UniTask.Delay(1000);
        // �V�[���ؑ�
        // �t�F�[�h���o�p�I�u�W�F�N�g�𐶐�
        GameObject fadeObject = Instantiate(m_fadeCanvas);
        // ���������I�u�W�F�N�g��FadeStart�֐����Ăяo��
        fadeObject.GetComponent<FadeScene>().FadeStart("GameOverScene", Color.black, true);

        m_isGameOver = true;
        //���g�̓V�[�����܂����ł��폜����Ȃ��悤�ɂ���
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if(m_isGameOver == false)
        {
            return;
        }

        if (Input.anyKeyDown)
        {
            GameObject fadeObject = Instantiate(m_fadeCanvas);
            fadeObject.GetComponent<FadeScene>().FadeStart("TitleScene", Color.black, true);
        }
    }
}
