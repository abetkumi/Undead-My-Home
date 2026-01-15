using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleBack : MonoBehaviour
{
    [SerializeField] GameObject m_fadeCanvas;
    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            GameObject fadeObject = Instantiate(m_fadeCanvas);
            fadeObject.GetComponent<FadeScene>().FadeStart("TitleScene", Color.black, true);
            Destroy(gameObject);
        }
    }
}
