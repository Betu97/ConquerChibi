using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverScript : MonoBehaviour
{
    
    GameObject crate;
    GameObject utext;
    GameObject upanel;

    public void Start()
    {
        utext = GameObject.Find("Interact");
        utext.SetActive(false);
        upanel = GameObject.Find("Panel");
        upanel.SetActive(false);
        crate = GameObject.Find("Plataforma");
        crate.GetComponent<Crate>().state = Crate.State.hide;
    }

    private void OnTriggerStay(Collider col)
    {
        utext.SetActive(true);
        upanel.SetActive(true);
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("rotated");
            crate.GetComponent<Animator>().SetBool("rotated", crate.GetComponent<Crate>().mIsRotated);
            crate.GetComponent<Crate>().mIsRotated = !crate.GetComponent<Crate>().mIsRotated;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        utext.SetActive(false);
        upanel.SetActive(false);
    }
}
