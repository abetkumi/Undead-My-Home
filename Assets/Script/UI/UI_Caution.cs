using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class UI_Caution : MonoBehaviour
{
    [SerializeField] GameObject m_cautionUI;
    [SerializeField] GameManager m_gameManager;
    [SerializeField] Player m_player;
    public Button m_yesButton;
    
    // Start is called before the first frame update
    void Start()
    {
        m_player = m_player.GetComponent<Player>();
        m_gameManager = m_gameManager.GetComponent<GameManager>();
    }

    public void SetActiveCautionUI(bool active)
    {
        m_cautionUI.SetActive(active);
    }

    public void YesButton()
    {
        SetActiveCautionUI(false);
        m_gameManager.SetGameState(GameManager.GameState.enGameState_Play);
        m_player.SetPlayerState(Player.PlayerState.Dead);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        Time.timeScale = 1.0f;
    }

    public void NoButton()
    {
        m_gameManager.SetGameState(GameManager.GameState.enGameState_Play);
        SetActiveCautionUI(false);
        Time.timeScale = 1.0f;
    }
}
