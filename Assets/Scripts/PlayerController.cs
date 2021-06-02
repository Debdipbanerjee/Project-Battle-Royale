using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform viewPoint;
    public float mouseSensitivity = 1f;
    private float verticalRotStore; //limit looking up & down
    private Vector2 mouseInput;

    public bool invertLook;
    public float moveSpeed = 5f, runSpeed = 8f;
    private float activeMoveSpeed;

    private Vector3 moveDir, movement;

    public CharacterController charCon;

    // Start is called before the first frame update
    void Start()
    {
        //Lock the cursor while playing the game
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;

        //Change the rotation accoss the Y axis (looking left & right)
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x, transform.rotation.eulerAngles.z);

        //Change the rotation of viewpoint accoss the x axis (looking Up & Down)
        verticalRotStore += mouseInput.y;
        verticalRotStore = Mathf.Clamp(verticalRotStore, -60f, 60f);

        if(invertLook)
        {
            //Mouse up: Look down + Mouse down: Look up
            viewPoint.rotation = Quaternion.Euler(verticalRotStore, viewPoint.rotation.eulerAngles.y, viewPoint.rotation.eulerAngles.z);
        }
        else
        {
            // mouse up & down will look up & down
            viewPoint.rotation = Quaternion.Euler(-verticalRotStore, viewPoint.rotation.eulerAngles.y, viewPoint.rotation.eulerAngles.z);
        }

        //getting directions from keyboards
        moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));

        //Checking if player is Running or Walking
        if(Input.GetKey(KeyCode.LeftShift))
        {
            //Running
            activeMoveSpeed = runSpeed;
        }
        else
        {
            //Not Running,just Walking
            activeMoveSpeed = moveSpeed;
        }

        // moveing directions wrt to player
        movement = ((transform.forward * moveDir.z) + (transform.right * moveDir.x)).normalized * activeMoveSpeed;

        //move using character controller
        charCon.Move(movement * Time.deltaTime); 

    }
}
