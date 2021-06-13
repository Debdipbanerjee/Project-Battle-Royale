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

    //Accessing main camera
    private Camera cam;

    public float jumpForce = 12f, gravityMod = 2.5f;

    public Transform groundCheckPoint;
    private bool isGrounded;
    public LayerMask groundLayers;

    public GameObject bulletImpact;
    public float timeBetweenShots = 0.1f;
    private float shotCounter;

    public float maxHeat = 10f, heatPerShot = 1f, coolRate = 4f, overheatCoolRate = 5f;
    private float heatCounter;
    private bool overHeated;



    // Start is called before the first frame update
    void Start()
    {
        //Lock the cursor while playing the game
        Cursor.lockState = CursorLockMode.Locked;

        //Assigning main camera
        cam = Camera.main;

        //Weapon temparature
        UIController.instance.weaponTempSlider.maxValue = maxHeat;
    }

    // Update is called once per frame
    void Update()
    {
        //Looking
        //Getting mouse input for looking
        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;

        //Change the rotation accoss the Y axis (looking left & right)
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x, transform.rotation.eulerAngles.z);

        //Change the rotation of viewpoint accoss the x axis (looking Up & Down)
        verticalRotStore += mouseInput.y;
        verticalRotStore = Mathf.Clamp(verticalRotStore, -60f, 60f);

        //Invert Looking up & down
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

        //Moving
        //getting keyboard inputs for moving direction
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

        //Gravity
        float yVel = movement.y;
        movement = (((transform.forward * moveDir.z) + (transform.right * moveDir.x)).normalized) * activeMoveSpeed; // moveing directions wrt to player
        movement.y = yVel; //player Y position

        //gravity 0 when grounded
        if(charCon.isGrounded)
        {
            movement.y = 0f;
        }

        //custom way to check is the player grounded or not
        isGrounded = Physics.Raycast(groundCheckPoint.position, Vector3.down, 0.25f, groundLayers);

        //Jump
        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            movement.y = jumpForce;
        }

        movement.y += Physics.gravity.y * Time.deltaTime * gravityMod;


        if(!overHeated)
        {
            //Shooting
            if (Input.GetMouseButtonDown(0))
            {
                Shoot();
            }

            //Autofiring
            if (Input.GetMouseButton(0))
            {
                shotCounter -= Time.deltaTime;

                if (shotCounter <= 0)
                {
                    Shoot();
                }
            }

            //Cooling down when NOT overheated
            heatCounter -= coolRate * Time.deltaTime;
        }
        else
        {
            //Cooling down when overheated
            heatCounter -= overheatCoolRate * Time.deltaTime;

            if (heatCounter <= 0)
            {
                overHeated = false;

                //Disable overheated message when it's cools down
                UIController.instance.overheatedMessage.gameObject.SetActive(false);
            }
        }

        //Resetting the heat counter
        if(heatCounter < 0)
        {
            heatCounter = 0f;
        }

        //Weapon temparature value per frame
        UIController.instance.weaponTempSlider.value = heatCounter;


        //move using character controller
        charCon.Move(movement * Time.deltaTime); 

        //Locking of cursor while Game
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else if (Cursor.lockState == CursorLockMode.None)
        {
            if(Input.GetMouseButtonDown(0))
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

    }

    private void Shoot()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        ray.origin = cam.transform.position;

        if(Physics.Raycast(ray,out RaycastHit hit))
        {
            //Instantiate bullet impact
            GameObject bulletImpactObject = Instantiate(bulletImpact, hit.point + (hit.normal * 0.002f), Quaternion.LookRotation(hit.normal, Vector3.up));

            //Destroy bullet impact
            Destroy(bulletImpactObject, 10f);
        }

        shotCounter = timeBetweenShots;

        //heat ill increase per shot
        heatCounter += heatPerShot;

        if(heatCounter >= maxHeat)
        {
            heatCounter = maxHeat;

            overHeated = true;
            
            //show overheated message 
            UIController.instance.overheatedMessage.gameObject.SetActive(true);
        }
    }

    private void LateUpdate()
    {
        //Using Main camera as FPS camera without assigning as a child of player
        cam.transform.position = viewPoint.position;
        cam.transform.rotation = viewPoint.rotation;
    }
}
