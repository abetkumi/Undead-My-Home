using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCulling : MonoBehaviour
{
    [SerializeField] Camera m_cameraObject;
    [SerializeField] private string m_cullingLayerName = "Culling Layer";
    int m_cullingLayerMask;
    int m_defaultCullingMask;

    void Start()
    {
        m_cullingLayerMask = 1 << LayerMask.NameToLayer(m_cullingLayerName);

        m_defaultCullingMask = m_cameraObject.cullingMask;

        HidePlayerBody();
    }

    public void HidePlayerBody()
    {
        m_cameraObject.cullingMask = m_defaultCullingMask & ~m_cullingLayerMask;
    }

    public void ShowPlayerBody()
    {
        m_cameraObject.cullingMask = m_defaultCullingMask | m_cullingLayerMask;
    }
}
