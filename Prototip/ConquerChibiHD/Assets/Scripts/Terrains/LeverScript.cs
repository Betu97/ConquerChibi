using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverScript : MonoBehaviour
{
    
    GameObject crate;

    //Make it as a singleton
    private static LeverScript instance;

    public static LeverScript Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<LeverScript>();
            }

            return instance;
        }
    }

    public void Start()
    {
        crate = GameObject.Find("PlataformaNew");
        crate.GetComponent<Crate>().state = Crate.State.hide;
    }

    public void rotatePlatform()
    {
        crate.GetComponent<Crate>().mIsRotated = !crate.GetComponent<Crate>().mIsRotated;
        crate.GetComponent<Animator>().SetBool("rotated", crate.GetComponent<Crate>().mIsRotated);
    }

}
