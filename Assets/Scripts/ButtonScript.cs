using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    bool buttonDown = false;
    [SerializeField] private GameObject button;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.Equals(button))
        {
            buttonDown = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.Equals(button))
        {
            buttonDown = false;
        }
    }

    private void Update()
    {
        if (buttonDown)
        {
            ButtonThing();
        }
    }
    public void ButtonThing()
    {
        Debug.Log("BUTTON");
    }
}
