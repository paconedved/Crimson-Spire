using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearHits : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(PlayerController.playerInstance.rolling == false)
            {
                PlayerController.playerInstance.TakeDamage(5f);
            }
        }
    }
}
