using UnityEngine;
using System.Collections;
 
 public class CameraWork : MonoBehaviour {
 
    public float rotationSpeed = 1;
    public Transform Target, Player;

    public float height = 10f;
    public float distance = 20f;
    private Vector3 spacing;
 
    public float mouseX, mouseY;

    bool sideViewFlag = false;

    // cached transform of the target
    Transform cameraTransform;

    // maintain a flag internally to reconnect if target is lost or camera is switched
    bool isFollowing;
 
    public void Start () {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        height = 3;
        distance = 6;
        spacing = new Vector3 (0, height, -distance);
        Debug.Log("setting initial spacing value: " + spacing + " with height = " + height + " and distance = " + -distance);
    }
     
    void LateUpdate(){
        // The transform target may not destroy on level load,
        // so we need to cover corner cases where the Main Camera is different everytime we load a new scene, and reconnect when that happens
        if (cameraTransform == null && isFollowing){
            OnStartFollowing();
        }
        // only follow is explicitly declared
        if (isFollowing){
            moveCamera();
        }
    }

    void moveCamera(){
        mouseX += Input.GetAxis("Mouse X") * rotationSpeed;
        mouseY -= Input.GetAxis("Mouse Y") * rotationSpeed;
        mouseY = Mathf.Clamp(mouseY, -35, 60);

        transform.LookAt(Target);

        cameraTransform.rotation = Quaternion.Euler(mouseY, mouseX, 0);
        spacing = Quaternion.AngleAxis (Input.GetAxis("Mouse X") * rotationSpeed, Vector3.up) * spacing;
       
        // original vector:
        Vector3 forward = cameraTransform.forward;
        // up direction:
        Vector3 up = new Vector3(0.0f, 1.0f, 0.0f);
        // find right vector:
        Vector3 right = Vector3.Cross(forward.normalized, up.normalized);

        if (Input.GetAxis("Mouse ScrollWheel") > 0f ) { //forward
            sideViewFlag = false;
        }else if (Input.GetAxis("Mouse ScrollWheel") < 0f ) { //backwards
            sideViewFlag = true;
        }

        if(sideViewFlag){
            cameraTransform.position = transform.position + spacing + right;
        }else{
            cameraTransform.position = transform.position + spacing - right;
        }

        //rotar al personatge en base a on estigui mirant la camara
        transform.rotation = Quaternion.Euler(0, mouseX, 0);
    }

    // Raises the start following event.
    // Use this when you don't know at the time of editing what to follow, typically instances managed by the photon network.
    public void OnStartFollowing(){
        cameraTransform = Camera.main.transform;
        isFollowing = true;
        // we don't smooth anything, we go straight to the right camera shot
        moveCamera();
    }
}