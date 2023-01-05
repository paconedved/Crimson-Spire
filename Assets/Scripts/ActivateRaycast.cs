using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateRaycast : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            this.transform.LookAt(other.transform.position);
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
