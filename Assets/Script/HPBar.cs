using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBar : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

/*
using UnityEngine;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour
{
    [Header("HP Settings")]
    public int maxHP = 100;
    public int currentHP;

    [Header("UI")]
    public Image hpFillImage; // HPバーのFill部分（Image）

    void Start()
    {
        currentHP = maxHP;
        UpdateHPUI();
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        UpdateHPUI();
    }

    void UpdateHPUI()
    {
        float fill = (float)currentHP / maxHP;
        hpFillImage.fillAmount = fill;
    }
}
*/
