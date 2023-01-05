using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MoveToGame : MonoBehaviour
{
    private InputAction interact;
    private InputAction quit;
    private PlayerInput playerInput;

    public GameObject player;
    public GameObject cineMachine;
    public GameObject playerCam;
    public GameObject compassCanvas;
    public GameObject controlsScreen;
    public GameObject controlCamera;
    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        interact = playerInput.actions["Interact"];
        quit = playerInput.actions["Quit"];
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if(interact.WasPressedThisFrame())
        {
            controlsScreen.SetActive(false);
            controlCamera.SetActive(false);
            player.SetActive(true);
            compassCanvas.SetActive(true);
            cineMachine.SetActive(true);
            playerCam.SetActive(true);
            this.gameObject.SetActive(false);
/*            SceneManager.LoadScene(1);*/
        }
        if(quit.WasPressedThisFrame())
        {
            Application.Quit();
        }
    }
}
