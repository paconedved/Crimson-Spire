using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Regen_Health : MonoBehaviour
{
    public static Regen_Health regenInstance;

    public bool heal;

    private void Awake()
    {
        regenInstance = this;
    }

    private void Update()
    {
        if(!heal && (PlayerController.playerInstance.health < 100 || PlayerController.playerInstance.healthImage.fillAmount < 1))
        {
            PlayerController.playerInstance.health += 0.02f * Time.deltaTime;
            PlayerController.playerInstance.healthImage.fillAmount += 0.02f * Time.deltaTime;
            if(PlayerController.playerInstance.health > 100)
            {
                PlayerController.playerInstance.health = 100;
            }
            if(PlayerController.playerInstance.healthImage.fillAmount >= 1)
            {
                PlayerController.playerInstance.healthImage.fillAmount = 1;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Skeleton") || other.CompareTag("Bear"))
        {
            heal = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Skeleton") || other.CompareTag("Bear"))
        {
            heal = false;
        }
    }
}
