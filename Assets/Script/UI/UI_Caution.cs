using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Caution : MonoBehaviour
{
    [SerializeField] GameObject m_cautionUI;
    [SerializeField] TextMeshProUGUI m_cautionText;
    [SerializeField] GameManager m_gameManager;
    [SerializeField] Player m_player;
    public Button m_yesButton;
    [SerializeField] AudioClip m_buttonSE;
    
    // Start is called before the first frame update
    void Start()
    {
        m_player = m_player.GetComponent<Player>();
        m_gameManager = m_gameManager.GetComponent<GameManager>();
        m_yesButton = m_yesButton.GetComponent<Button>();
    }

    public void SetActiveCautionUI(bool active)
    {
        m_cautionUI.SetActive(active);
    }

    public void SetCautionText(string text)
    {
        m_cautionText.text = text;
    }

    public void SetYesButton(int i)
    {
        m_yesButton.onClick.RemoveAllListeners();

        m_yesButton.onClick.AddListener(ClickSE);
        switch (i)
        {
            case 0:
                m_yesButton.onClick.AddListener(YesButtonMainScene);
                break;
            case 1:
                m_yesButton.onClick.AddListener(YesButtonStoreScene);
                break;
            default:
                m_yesButton.onClick.RemoveAllListeners();
                break;
        }
    }

    void ClickSE()
    {
        GameManager.PlaySE(m_buttonSE);
    }

    public void YesButtonStoreScene()
    {
        SetActiveCautionUI(false);
        m_gameManager.SetGameState(GameManager.GameState.enGameState_Play);
        m_player.SetPlayerState(Player.PlayerState.Dead);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1.0f;
    }

    public void YesButtonMainScene()
    {
        m_gameManager.SetGameState(GameManager.GameState.enGameState_Play);
        GameClear m_clear = GameObject.FindWithTag("GameClear").GetComponent<GameClear>();
        SetActiveCautionUI(false);
        m_clear.SetStoreScene();
        m_clear.m_isWait = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1.0f;
    }

    public void NoButton()
    {
        m_gameManager.SetGameState(GameManager.GameState.enGameState_Play);
        SetActiveCautionUI(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1.0f;
    }
}
