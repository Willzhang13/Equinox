using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SaveAndLoad : MonoBehaviour
{
    private Vector3 position;
    private Quaternion rotation;
    private Vector3 velocity;
    private Vector3 angularVelocity;

    public virtual void Save()
    {
        position = transform.localPosition;
        rotation = transform.localRotation;
        velocity = GetComponent<Rigidbody>().velocity;
        angularVelocity = GetComponent<Rigidbody>().angularVelocity;
        Debug.Log("Saved " + gameObject.name);
    }

    public virtual void Load()
    {
        transform.localPosition = position;
        transform.localRotation = rotation;
        GetComponent<Rigidbody>().velocity = velocity;
        GetComponent<Rigidbody>().angularVelocity = angularVelocity;
        Debug.Log(gameObject.name + position);
    }
}
