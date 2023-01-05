using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class SkeletonAI : MonoBehaviour
{
    public static SkeletonAI skeletonInstance;

    NavMeshAgent agent;
    Animator aiAnim;

    public bool playerInSight;

    [SerializeField] bool canAttack;

    public int comboNumber;

    [SerializeField] float max_health = 100;

    [SerializeField] float b_health;

    [SerializeField] Image healthBar;

    [SerializeField] GameObject armature;

    public List<Transform> patrolPoints = new List<Transform>();
    int nextPatrolPoint = 0;

    private int IdleAnim;

    public GameObject player;

    private void Awake()
    {
        skeletonInstance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        aiAnim = GetComponent<Animator>();

        IdleAnim = Animator.StringToHash("Idle");

        b_health = max_health;
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerInSight)
        {
            aiAnim.SetBool("playerdetected", false);
            aiAnim.SetBool("attack", false);
            canAttack = false;

            Patrol();
            if (Vector3.Distance(transform.position, patrolPoints[nextPatrolPoint].position) < 1f)
            {
                nextPatrolPoint += 1; //change the patrol position 
                agent.isStopped = true; //the AI stops moving
                aiAnim.CrossFade(IdleAnim, 0.1f); //change the animation from walking to eating
                StartCoroutine(WaitToEat()); //start the coroutine so that eating animation can take place 
            }
        }

        else if (playerInSight)
        {
            aiAnim.SetBool("playerdetected", true);
            MoveTowardsPlayer();

            if (Mathf.Abs(Vector3.Distance(transform.position, player.transform.position)) < 3f)
            {
                canAttack = true;
                aiAnim.SetBool("inattackrange", true);
            }
            else
            {
                agent.isStopped = false;
                canAttack = false;
                aiAnim.SetBool("attack", false);
                aiAnim.SetBool("inattackrange", false);
            }

            if (canAttack)
            {
                agent.isStopped = true;
                AttackPlayer();
            }
        }

        if (b_health > 100)
        {
            b_health = 100f;
        }
    }

    //Make the AI turn and face the target location
    private void FaceTarget(Vector3 destination)
    {
        Vector3 lookPos = destination - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 0.5f);
    }

    #region Patrol and Eat
    //Make the AI move between predetermined patrol positions
    void Patrol()
    {
        if (nextPatrolPoint == patrolPoints.Count)
        {
            nextPatrolPoint = 0;
        }
        FaceTarget(patrolPoints[nextPatrolPoint].position);
        agent.SetDestination(patrolPoints[nextPatrolPoint].position);
    }

    IEnumerator WaitToEat()
    {
        if (playerInSight)
            yield break;
        yield return new WaitForSeconds(1.4f);
        agent.isStopped = false;
        Patrol();
    }
    #endregion

    #region Player Detection
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInSight = true;
            PlayerController.playerInstance.compass.SetActive(false);
            PlayerController.playerInstance.compassBG.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInSight = false;
            PlayerController.playerInstance.compass.SetActive(true);
            PlayerController.playerInstance.compassBG.SetActive(true);
        }
    }

    void MoveTowardsPlayer()
    {
        FaceTarget(player.transform.position);
        agent.SetDestination(player.transform.position);
    }
    #endregion

    #region Attack
    void AttackPlayer()
    {
        comboNumber = UnityEngine.Random.Range(1, 4);
        aiAnim.SetInteger("comboNumber", comboNumber);
        aiAnim.SetBool("attack", true);
    }

    /*    IEnumerator WaitForAttack(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            aiAnim.SetBool("attack", false);
        }*/
    #endregion

    public void TakeDamage(float damage)
    {
        if (b_health > 0f)
        {
            b_health -= damage;
            healthBar.fillAmount -= (damage / 100);
        }

        if (b_health <= 0)
        {
            armature.GetComponent<CapsuleCollider>().enabled = false;
            armature.tag.Replace("Skeleton", "Dead");
            this.gameObject.tag.Replace("Skeleton", "Dead");
            aiAnim.SetTrigger("death");
        }

        else if (b_health % 20 == 0 && b_health > 0)
        {
            aiAnim.SetTrigger("hit");
        }
    }

    public void Death()
    {
        PlayerController.playerInstance.compass.SetActive(true);
        PlayerController.playerInstance.compassBG.SetActive(true);
        Regen_Health.regenInstance.heal = false;
        this.gameObject.SetActive(false);
    }
}
