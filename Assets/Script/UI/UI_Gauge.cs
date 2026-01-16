using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class UI_Gauge : MonoBehaviour
{
    [SerializeField] Image m_hpBar;
    [SerializeField] Image m_hpAnderBar;
    [SerializeField] Image m_staminaBar;
    [SerializeField] GameObject m_playerObject;
    Player m_player;

    bool m_active = false;

    // Start is called before the first frame update
    void Start()
    {
        m_player = m_playerObject.GetComponent<Player>();
    }

    async public void UpdateHPGauge()
    {
        m_hpBar.fillAmount = m_player.GetPlayerHP() / m_player.GetMaxHP();

        await UniTask.Delay(1000);
        m_active = true;
    }

    void UpdateHPAnderBauge()
    {
        m_hpAnderBar.fillAmount -= 0.1f *Time.deltaTime;
        if(m_hpAnderBar.fillAmount < m_hpBar.fillAmount)
        {
            m_hpAnderBar.fillAmount = m_hpBar.fillAmount;
            m_active = false;
        }
    }

    public void UpdateStaminaGauge()
    {
        m_staminaBar.fillAmount = m_player.GetStamina() / m_player.GetMaxStamina();
    }

    private void FixedUpdate()
    {
        if (m_active == false)
        {
            return;
        }

        UpdateHPAnderBauge();
    }
}
