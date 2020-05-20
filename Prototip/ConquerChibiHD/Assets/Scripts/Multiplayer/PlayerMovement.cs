using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

	public Rigidbody rb;
	public Camera mainCamera;
    public bool isGrounded = true;
    public bool hasDoubleJumped = false;
    public float horizontalSpeed = 2.0F;
    public float verticalSpeed = 2.0F;
    public Animator animator;
//    public GameObject myGameObject;


/*
En un futur, per balancejar el moviment de cada personatge es pot fer un switch on les variables es mofifiquin en base al personatge
    public readonly int NINJA = 0;
    public readonly int VALKYRIE = 1;
    public readonly int KNIGHT = 2;
    public readonly int ESKIMO = 3;
    public readonly int VIKING = 4;
    public readonly int AMAZON = 5;
*/

    // Start is called before the first frame update
    void Start()
    {
        //rb = GetComponent<Rigidbody>(); 
        mainCamera = Camera.main;
        //Rigidbody 
        rb = gameObject.AddComponent<Rigidbody>(); // Add the rigidbody.
		rb.mass = 1;
    }

    // Update is called once per frame
    void Update()
    {
    	// Cache the inputs.
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 moveDirection = Vector3.zero;

        //salt i doble salt
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded){
        	print(rb);
            rb.AddForce(new Vector3(0, 5, 0), ForceMode.Impulse);
            isGrounded = false;
        } else if (Input.GetKeyDown(KeyCode.Space) && !isGrounded && !hasDoubleJumped) {
			rb.AddForce(new Vector3(0, 5, 0), ForceMode.Impulse);
        	hasDoubleJumped = true;
        }


        //rotacio de la camara en X. 
        //TODO: El moviment Y per ara no es necesari i s'ha d'arreglar
        float h = horizontalSpeed * Input.GetAxis("Mouse X");
        float v = verticalSpeed * Input.GetAxis("Mouse Y");
        //camara sense mirar adalt i abaix:
        transform.Rotate(0, h, 0);
        //camara amb moviment adalt i abaix:
        //transform.Rotate(-v/2, h, 0);


        //per a fer el moviment del personatge mes lent
        int slower = 8;

        //moviment en estrella WSAD
        if(Input.GetKey(KeyCode.A)){
            Vector3 direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
			direction = Camera.main.transform.TransformDirection(direction);
			direction.y = 0.0f;
			transform.position += Vector3.Normalize(direction)/slower;
        }else if(Input.GetKey(KeyCode.D)){
            Vector3 direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
			direction = Camera.main.transform.TransformDirection(direction);
			direction.y = 0.0f;
			transform.position += Vector3.Normalize(direction)/slower;
        }else if(Input.GetKey(KeyCode.W)){
            Debug.Log("caca");
            Vector3 direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
			direction = Camera.main.transform.TransformDirection(direction);
			direction.y = 0.0f;
			transform.position += Vector3.Normalize(direction)/slower;
        }else if(Input.GetKey(KeyCode.S)){
        	Vector3 direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
			direction = Camera.main.transform.TransformDirection(direction);
			direction.y = 0.0f;
			transform.position += Vector3.Normalize(direction)/slower;
        }else if (Input.GetKeyDown(KeyCode.LeftShift)){
        	//TODO: Arreglar el teleport
        	print(Camera.main.transform.position);
        	//Vector3 direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0f + 1, Input.GetAxisRaw("Vertical"));
			//direction = Camera.main.transform.TransformDirection(direction);
        	//transform.position = Vector3.Normalize(direction);
        	Vector3 aux = new Vector3(0, 2, 0);
			transform.position = (Camera.main.transform.forward * 5) + aux;
        }
    }

    //comprova si el objecte esta tocant a terra per a fer o no fer el double jump. El terra ha de tenir el tag "Ground"
    void OnCollisionEnter(Collision col){
     	if (col.gameObject.tag == ("Ground") && !isGrounded){
         	isGrounded = true;
         	hasDoubleJumped = false;
     	}
 	}
}
