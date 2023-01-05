using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordHitReg : MonoBehaviour
{
/*    public static SwordHitReg swordInstance;

    public bool alreadyhit;

    private void Awake()
    {
        swordInstance = this;
    }
*/
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Bear"))
        {
            if (PlayerController.playerInstance.attacking)
            {
                other.transform.gameObject.GetComponentInParent<EnemyAI>().TakeDamage(4f);
            }
        }
        if (other.CompareTag("Skeleton"))
        {
            if (PlayerController.playerInstance.attacking)
            {
                other.transform.gameObject.GetComponentInParent<SkeletonAI>().TakeDamage(6f);
            }
        }
    }
}
