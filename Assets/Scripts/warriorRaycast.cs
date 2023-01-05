using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class warriorRaycast : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController.playerInstance.activateRay = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController.playerInstance.activateRay = false;
        }
    }
}
