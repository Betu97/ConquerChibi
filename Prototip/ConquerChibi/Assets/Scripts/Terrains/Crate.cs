using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour
{
    public enum State
    {
        hide,
        rotated,
        inbetween
    }

    public State state;
    public string InteractText = "";
    public bool mIsRotated = false;

    // Start is called before the first frame update
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {

    }

    public void OnMouseEnter()
    {
    }

    public void OnMouseExit()
    {
    }

    public void OnMouseUp()
    {
        InteractText = "Press F to ";
        
        InteractText += mIsRotated ? "to damage" : "to hide";
       
    }
}
