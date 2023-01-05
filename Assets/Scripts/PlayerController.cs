using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.AI;
using TMPro;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController playerInstance;

    [Header("Player attributes")]
    public float playerWalkSpeed = 6.0f;
    public float playerSprintSpeed = 10f;
    public float jumpHeight = 1.0f;
    public float gravityValue = -9.81f;
    public float rotationSpeed = 1.0f;
    public float smoothAnimationTime = 0.1f;
    public float health;
    public float maxHealth = 100f;
    public Image healthImage;
    public bool attacking;
    public bool rolling;
    public bool gettingHit;
    public GameObject compass;
    public GameObject compassBG;
    public GameObject mushroom;
    [SerializeField]private bool groundedPlayer;
    public GameObject sword;
    public bool activateRay;
    [SerializeField] bool fragment1;
    [SerializeField] bool fragment2;
    [SerializeField] bool fragment3;
    [SerializeField] int divineFragmentCount;


    [Header("Placeholder dialouge")]
    public string[] dialouge;
    [Header("Villager1 dialouges")]
    public string[] v1_dialouge;
    [Header("Villager2 dialouges")]
    public string[] v2_dialouge;
    [Header("VillageChief dialouges")]
    public string[] vC1_dialouge;
    public string[] vC2_dialouge;
    public string[] vC3_dialouge;
    [Header("Medic dialouges")]
    public string[] medic1_dialouge;
    public string[] medic2_dialouge;
    public string[] medic3_dialouge;
    public string[] medic4_dialouge;
    [Header("Warrior dialouges")]
    public string[] war_dialouge;
    [Header("Priest dialouges")]
    public string[] priest1_dialouge;
    public string[] priest2_dialouge;

    [Header("Dialouge parameters and bools")]
    public bool talking;
    public GameObject currentNpc;
    public TextMeshProUGUI textField;
    public GameObject textPanel;
    public int index;
    public bool medicSaved;
    public bool ingredientAccquired;
    public GameObject medicAgent;
    public Transform medicVillageLocation;
    public GameObject bear1;
    public GameObject bear2;
    public int medCount;

    [Header("Scenes")]
    public GameObject cineMachine;
    public GameObject playerCam;
    public GameObject compassCanvas;
    public GameObject controlsScreen;
    public GameObject controlCamera;
    public GameObject proceed;

    private CharacterController controller;
    private PlayerInput playerInput;
    private Vector3 playerVelocity;
    private Transform camTransform;

    private InputAction movement;
    private InputAction run;
    private InputAction jump;
    public InputAction attack;
    private InputAction roll;
    private InputAction interact;
    private InputAction quit;
    private InputAction menu;

    private Animator animator;
    private int moveXAnimatorParameter;
    private int moveZAnimatorParameter;
    private int jumpAnimation;
    private int attackAnimation;

    private Vector2 animationBlendVector;
    private Vector2 animationVelocityVector;

    private void Awake()
    {
        playerInstance = this;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Start()
    {
        health = maxHealth;

        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        camTransform = Camera.main.transform;

        movement = playerInput.actions["Move"];
        run = playerInput.actions["Sprint"];
        jump = playerInput.actions["Jump"];
        attack = playerInput.actions["Attack"];
        roll = playerInput.actions["Dodge"];
        interact = playerInput.actions["Interact"];
        quit = playerInput.actions["Quit"];
        menu = playerInput.actions["Menu"];

        animator = GetComponent<Animator>();
        moveXAnimatorParameter = Animator.StringToHash("moveX");
        moveZAnimatorParameter = Animator.StringToHash("moveZ");
        jumpAnimation = Animator.StringToHash("Jump");
        attackAnimation = Animator.StringToHash("Attack");
    }

    void Update()
    {
        if (quit.WasPressedThisFrame())
        {
            Application.Quit();
        }

        if (menu.WasPressedThisFrame())
        {
            cineMachine.SetActive(false);
            playerCam.SetActive(false);
            compassCanvas.SetActive(false);
            controlsScreen.SetActive(true);
            controlCamera.SetActive(true);
            proceed.SetActive(true);
            this.gameObject.SetActive(false);
/*            SceneManager.LoadScene(0);*/
        }

        if (activateRay)
        {
            ThrowRay();
            this.GetComponent<SphereCollider>().enabled = false;
        }
        else if(!activateRay)
        {
            this.GetComponent<SphereCollider>().enabled = true;
        }

        if(health <=0)
        {
            print("Player Dead");
        }

        if(talking)
        {
            textPanel.SetActive(true);
            if (currentNpc.name != "Medic")
            {
                currentNpc.GetComponent<Animator>().SetBool("npctalk", true);
            }
            if (textField.text == dialouge[index] && index < (dialouge.Length - 1))
            {
                if(interact.WasPressedThisFrame())
                {
                    if(index < dialouge.Length - 1)
                    {
                        index++;
                        textField.text = string.Empty;
                        StartCoroutine(Typing());
                    }
                }
            }
            else if(index == (dialouge.Length - 1))
            {
                if (interact.WasPressedThisFrame())
                {
                    StopAllCoroutines();
                    textField.text = String.Empty;
                    textPanel.SetActive(false);
                    index = 0;
                    if (currentNpc.name != "Medic")
                    {
                        currentNpc.GetComponent<Animator>().SetBool("npctalk", false);
                    }
                    //Add the quest marker to find the medic
                    if (currentNpc.name == "VillageChief")
                    {
                        if(compass.GetComponent<Compass>().qMark.Count < 1)
                            compass.GetComponent<Compass>().AddQuestMarkers(GameObject.Find("MedicLocation").GetComponent<QuestMarker>());
                    }

                    //Add the quest marker for the graveyard
                    if (currentNpc.name == "Medic")
                    {
                        if (medicSaved && Mathf.Abs(Vector2.Distance(medicAgent.transform.position, medicVillageLocation.position)) > 2)
                        {
                            currentNpc.GetComponent<Animator>().SetBool("medtalk", false);
                            compass.GetComponent<Compass>().AddQuestMarkers(GameObject.Find("Graveyard").GetComponent<QuestMarker>());
                            medicAgent.transform.LookAt(medicVillageLocation.position);
                            medicAgent.GetComponent<NavMeshAgent>().SetDestination(medicVillageLocation.position);
                            currentNpc.GetComponent<Animator>().SetBool("run", true);
                        }
                        else if(!ingredientAccquired && medicSaved && Mathf.Abs(Vector2.Distance(medicAgent.transform.position, medicVillageLocation.position)) < 2)
                        {
                            currentNpc.GetComponent<Animator>().SetBool("medtalk2", false);
                            compass.GetComponent<Compass>().qMark.Clear();
                            Destroy(GameObject.Find("QuestIcon(Clone)"));
                        }
                    }
                    talking = false;
                }
            }
        }

        if (!talking)
        {
            textField.text = String.Empty;
            Dodge();

            if (!attacking && !gettingHit)
            {
/*                SwordHitReg.swordInstance.DisableCollider();*/
                //REsetting the speed values to default after player stops attacking
                playerWalkSpeed = 6f;
                playerSprintSpeed = 10f;

                groundedPlayer = controller.isGrounded;
                if (groundedPlayer && playerVelocity.y < 0)
                {
                    playerVelocity.y = 0f;
                }

                //Read Player Input
                Vector2 input = movement.ReadValue<Vector2>();
                animationBlendVector = Vector2.SmoothDamp(animationBlendVector, input, ref animationVelocityVector, smoothAnimationTime);
                Vector3 move = new Vector3(animationBlendVector.x, 0, animationBlendVector.y);
                move = move.x * camTransform.right.normalized + move.z * camTransform.forward.normalized;
                move.y = 0;

                //Set movement animations
                animator.SetFloat(moveXAnimatorParameter, animationBlendVector.x);
                animator.SetFloat(moveZAnimatorParameter, animationBlendVector.y);



                //Player sprint
                if (run.IsPressed() && movement.ReadValue<Vector2>().y > 0.8f)
                {
                    if (groundedPlayer)
                    {
                        animator.SetBool("sprint", true);
                        controller.Move(move * Time.deltaTime * playerSprintSpeed);
                    }
                    else if (animationBlendVector.y < 0.9f)
                    {
                        animator.SetBool("sprint", false);
                        controller.Move(move * Time.deltaTime * playerWalkSpeed);
                    }
                }

                if (run.WasReleasedThisFrame())
                {
                    animator.SetBool("sprint", false);
                }

                //Move the Player
                controller.Move(move * Time.deltaTime * playerWalkSpeed);

                // Player Jump
                if (jump.triggered && groundedPlayer)
                {
                    playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
                    animator.CrossFade(jumpAnimation, smoothAnimationTime);
                }

                playerVelocity.y += gravityValue * Time.deltaTime;
                controller.Move(playerVelocity * Time.deltaTime);
            }

            //Attack
            if (attack.WasPressedThisFrame())
            {
                if (groundedPlayer)
                {
                    attacking = true;
                    animator.SetTrigger("attack");
                }
            }

            if(bear1.activeInHierarchy == false && bear2.activeInHierarchy == false)
            {
                medicSaved = true;
            }


            if (medicSaved && medCount == 0)
            {
                if (Mathf.Abs(Vector2.Distance(medicAgent.transform.position, medicVillageLocation.position)) < 2)
                {
                    compass.GetComponent<Compass>().qMark.RemoveAt(0);
                    currentNpc.GetComponent<Animator>().SetTrigger("reached");
                    medCount++;
                }
            }
            //Rotate player along with the camera
            Quaternion desiredRotation = Quaternion.Euler(0, camTransform.eulerAngles.y, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, rotationSpeed * Time.deltaTime);
        }
    }

    public void Dodge()
    {
        if(roll.WasPressedThisFrame())
        {
            rolling = true;
            animator.SetTrigger("dodge");
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        healthImage.fillAmount -= (damage / 100);
        gettingHit = true;
        animator.SetTrigger("hit");
    }

    public void StartDialouge()
    {
        index = 0;
        talking = true;
        StartCoroutine(Typing());
    }

    public void ThrowRay()
    {
        RaycastHit hit;

        if(Physics.Raycast(camTransform.transform.position, transform.forward, out hit, 5f))
        {
            print(hit.transform.gameObject.name);

            //Talking to Villager1
            if (hit.transform.gameObject.name == "Villager1")
            {
                if(interact.WasPressedThisFrame() && !talking)
                {
                    dialouge = v1_dialouge;
                    currentNpc = hit.transform.gameObject;
                    StartDialouge();
                }
            }

            //Talking to Villager2
            if (hit.transform.gameObject.name == "Villager2")
            {
                if (interact.WasPressedThisFrame() && !talking)
                {
                    dialouge = v2_dialouge;
                    currentNpc = hit.transform.gameObject;
                    StartDialouge();
                }
            }

            //Talking to Village Chief
            if (hit.transform.gameObject.name == "VillageChief")
            {
                if (!medicSaved)
                {
                    if (interact.WasPressedThisFrame() && !talking)
                    {
                        if (compass.GetComponent<Compass>().qMark.Count < 1)
                        {
                            dialouge = vC1_dialouge;
                            currentNpc = hit.transform.gameObject;
                            StartDialouge();
                        }
                        else if(compass.GetComponent<Compass>().qMark.Count >= 1)
                        {
                            dialouge = vC2_dialouge;
                            currentNpc = hit.transform.gameObject;
                            StartDialouge();
                        }
                    }
                }
                else if(medicSaved)
                {
                    if (interact.WasPressedThisFrame() && !talking)
                    {
                        dialouge = vC3_dialouge;
                        currentNpc = hit.transform.gameObject;
                        if(!fragment1)
                        {
                            divineFragmentCount++;
                            fragment1 = true;
                        }
                        StartDialouge();
                    }
                }
            }

            //Talking to warrior
            if (hit.transform.gameObject.name == "Warrior")
            {
                if (interact.WasPressedThisFrame() && !talking)
                {
                    dialouge = war_dialouge;
                    currentNpc = hit.transform.gameObject;
                    if (!fragment2)
                    {
                        divineFragmentCount++;
                        fragment2 = true;
                    }
                    StartDialouge();
                }
            }

            //Talking to Medic
            if (hit.transform.gameObject.name == "Medic")
            {
                if (!medicSaved)
                {
                    if (interact.WasPressedThisFrame() && !talking)
                    {
                        dialouge = medic2_dialouge;
                        currentNpc = hit.transform.gameObject;
                        currentNpc.GetComponent<Animator>().SetBool("medtalk", true);
                        StartDialouge();
                    }
                }
                else if (medicSaved && Mathf.Abs(Vector2.Distance(medicAgent.transform.position, medicVillageLocation.position)) > 2)
                {
                    if (interact.WasPressedThisFrame() && !talking)
                    {
                        dialouge = medic1_dialouge;
                        currentNpc = hit.transform.gameObject;
                        currentNpc.GetComponent<Animator>().SetBool("medtalk", true);
                        StartDialouge();
                    }
                }
                else if (!ingredientAccquired && medicSaved && Mathf.Abs(Vector2.Distance(medicAgent.transform.position, medicVillageLocation.position)) < 2)
                {
                    if (interact.WasPressedThisFrame() && !talking)
                    {
                        dialouge = medic3_dialouge;
                        currentNpc = hit.transform.gameObject;
                        currentNpc.GetComponent<Animator>().SetBool("medtalk2", true);
                        StartDialouge();
                    }
                }
                else if (ingredientAccquired && Mathf.Abs(Vector2.Distance(medicAgent.transform.position, medicVillageLocation.position)) < 2)
                {
                    if (interact.WasPressedThisFrame() && !talking)
                    {
                        dialouge = medic4_dialouge;
                        currentNpc = hit.transform.gameObject;
                        currentNpc.GetComponent<Animator>().SetBool("medtalk2", true);
                        if (!fragment3)
                        {
                            divineFragmentCount++;
                            fragment3 = true;
                        }
                        StartDialouge();
                    }
                }
            }

            //Talking to Priest
            if (hit.transform.gameObject.name == "Priest")
            {
                if (divineFragmentCount < 3)
                {
                    if (interact.WasPressedThisFrame() && !talking)
                    {
                        dialouge = priest1_dialouge;
                        currentNpc = hit.transform.gameObject;
                        StartDialouge();
                    }
                }
                else if (divineFragmentCount == 3)
                {
                    if (interact.WasPressedThisFrame() && !talking)
                    {
                        dialouge = priest2_dialouge;
                        currentNpc = hit.transform.gameObject;
                        StartDialouge();
                    }
                }
            }

            //Picking up Mushroom
            if (hit.transform.gameObject.name == "Cross")
            {
                if (interact.WasPressedThisFrame())
                {
                    mushroom.SetActive(false);
                    ingredientAccquired = true;
                }
            }
        }
    }

    IEnumerator Typing()
    {
        foreach (char c in dialouge[index].ToCharArray())
        {
            textField.text += c;
            yield return new WaitForSeconds(0.02f);
        }
    }

    public void EnableCollider()
    {
        sword.GetComponent<BoxCollider>().enabled = true;
    }

    public void DisableCollider()
    {
        sword.GetComponent<BoxCollider>().enabled = false;
    }
}