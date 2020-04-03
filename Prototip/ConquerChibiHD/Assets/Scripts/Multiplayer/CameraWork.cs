using UnityEngine;
using System.Collections;
 
 public class CameraWork : MonoBehaviour {
 
    public float rotationSpeed = 1;
    public Transform Target, Player;

    public float height = 10f;
    public float distance = 20f;
    private Vector3 spacing;
 
    public float mouseX, mouseY;
     
    public void Start () {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        spacing = new Vector3 (0, height, -distance);
    }
     
    void LateUpdate(){
        mouseX += Input.GetAxis("Mouse X") * rotationSpeed;
        mouseY -= Input.GetAxis("Mouse Y") * rotationSpeed;
        mouseY = Mathf.Clamp(mouseY, -35, 60);

        transform.LookAt(Target);

        Camera.main.transform.rotation = Quaternion.Euler(mouseY, mouseX, 0);
        spacing = Quaternion.AngleAxis (Input.GetAxis("Mouse X") * rotationSpeed, Vector3.up) * spacing;
        Camera.main.transform.position = this.transform.position + spacing;

        //rotar al personatge en base a on estigui mirant la camara
        transform.rotation = Quaternion.Euler(0, mouseX, 0);
    }
}