using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverScript : MonoBehaviour
{
    
    GameObject crate;
    GameObject uinteract;
    bool col = false;

    public void Start()
    {
        uinteract = GameObject.Find("Interaction");
        uinteract.SetActive(false);
        crate = GameObject.Find("Plataforma");
        crate.GetComponent<Crate>().state = Crate.State.hide;
    }

    private void OnTriggerEnter(Collider other)
    {
        col = true;
    }

    public void Update()
    {
        if (col == true)
        {
            uinteract.SetActive(true);
            if (Input.GetButtonDown("MapInteraction"))
            {
                rotatePlatform();
            }
        }
    }

    private void rotatePlatform()
    {
        crate.GetComponent<Crate>().mIsRotated = !crate.GetComponent<Crate>().mIsRotated;
        crate.GetComponent<Animator>().SetBool("rotated", crate.GetComponent<Crate>().mIsRotated);
    }

    private void OnTriggerExit(Collider other)
    {
        col = false;
        uinteract.SetActive(false);
    }
}
