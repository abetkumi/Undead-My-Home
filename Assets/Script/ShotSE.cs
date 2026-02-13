using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotSE : MonoBehaviour
{
    [SerializeField] AudioClip m_shotSE, m_standUpSE;
    [SerializeField] AudioSource audioSource;
    public void Shot()
    {
        audioSource.PlayOneShot(m_shotSE);
    }

    public void StandUp()
    {
        audioSource.PlayOneShot(m_standUpSE);
    }
}
