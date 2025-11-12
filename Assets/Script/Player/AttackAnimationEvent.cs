using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

public class AttackAnimationEvent : MonoBehaviour
{
    [SerializeField] BoxCollider m_attackCollider;
    void AttackStart()
    {
        m_attackCollider.enabled = true;
    }

    void AttackEnd()
    {
        m_attackCollider.enabled = false;
        Player m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        m_player.SetPlayerState(Player.PlayerState.Idle);
    }
}
