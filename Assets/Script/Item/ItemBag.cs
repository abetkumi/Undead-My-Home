using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBag : MonoBehaviour
{
    [SerializeField] ItemData m_itemData;

    // Start is called before the first frame update
    void Start()
    {
        //©g‚ÍƒV[ƒ“‚ğ‚Ü‚½‚¢‚Å‚àíœ‚³‚ê‚È‚¢‚æ‚¤‚É‚·‚é
        DontDestroyOnLoad(gameObject);
    }
}
