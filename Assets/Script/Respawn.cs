using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlayerRespawn();
    }

    public void PlayerRespawn()
    {
        GameObject player = GameObject.FindWithTag("Player");
        player.transform.position = gameObject.transform.position;
    }
}
