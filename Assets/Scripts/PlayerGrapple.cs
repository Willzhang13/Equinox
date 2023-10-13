using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.XR;
using static UnityEngine.GraphicsBuffer;

public class PlayerGrapple : MonoBehaviour
{
    public LayerMask Pizza;

    private Coroutine grappleCoroutine;

    //FOR ANIMATION (BLINKING)
    [SerializeField] private GameObject cinematicBarsContainerGO;
    private GameObject hand;
    float timer = -1.0f;

    public GameObject grappleGun;

    //Hiding the camera's rigidbody with the one we want
    [SerializeField] private new Rigidbody rigidbody;

    //public float MAXDISTANCE;
    //public float MAXCUBEDIST;

    
    public CrosshairCubeRayCast crc;
    //Note: This is used both when the point you are looking at is out of range and when you are already grappling
    

    //GOAL REFERENCE
    public GameObject goal;
    private Renderer goalRenderer;

    public float grappleForce;

    //AUDIO
    private AudioSource source;

    private GrappleHead grappleHead;

    private bool holding;

    [SerializeField] float dampingAngle;
    [SerializeField] float dampingSpeed;

    //GLASS MATERIAL
    //public Material glass;

    //GRAVITY/CAMERA EXPERIMENT

    //Experimenting with cube rendering

    public void ShowBars() {
        cinematicBarsContainerGO.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        crc = FindObjectOfType<CrosshairCubeRayCast>();
        //KILL BLINK IF IT PLAYS AT THE START
        hand = GameObject.Find("CinematicBlackBarsContainer");
        //print(hand);
        if (hand != null){
            hand.SetActive(false);
        }


        //GET GOAL Renderer
        goalRenderer = goal.GetComponent<Renderer>();

        //GET AUDIO SOURCE
        source = GetComponent<AudioSource>();

        //Use FindObjectsOfTypeAll so it finds inactive scripts too
        grappleHead = Resources.FindObjectsOfTypeAll(typeof(GrappleHead))[0] as GrappleHead;
        grappleHead.gameObject.SetActive(false);
        holding = Input.GetKeyDown(KeyCode.F);
    }

    // Update is called once per frame
    //MOVE DA CAMERA
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("saving");
            foreach (SaveAndLoad go in Resources.FindObjectsOfTypeAll(typeof(SaveAndLoad)))
            {
                go.Save();
            }
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("loading");
            foreach(SaveAndLoad go in Resources.FindObjectsOfTypeAll(typeof(SaveAndLoad)))
            {
                go.Load();
            }
        }
        if (Input.GetKeyDown(KeyCode.F)) {
            ToggleHold();
        }

        
        if (Input.GetMouseButtonDown(0))
        {
            //PLAY SOUND
            //source.Play();
            crc.outOfRange = true;
            if (crc.hitSomething)
            {
                grappleHead.StartMovement(grappleGun.transform.position, (crc.hit.point - grappleGun.transform.position).normalized);
            } else
            {
                grappleHead.StartMovement(grappleGun.transform.position, grappleGun.transform.forward);
            }
        }
     

        if (timer != -1.0f) { 
            timer += Time.deltaTime;
            // int seconds = timer % 60;
            //print(timer);
            if (timer >= 0.20f)
            {
                hand = GameObject.Find("CinematicBlackBarsContainer");
                //print(hand);
                if (hand != null) { 
                    hand.SetActive(false);
                }
            } 
        }
    }

    private void ToggleHold()
    {
        if (GetComponent<HingeJoint>() == null)
        {
            HoldSurface();
        }
        else
        {
            StopHolding();
        }
    }

    private void HoldSurface() {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 5f)) //not check for layer anymore
        {
            if (hit.transform.gameObject.GetComponent<Rigidbody>() != null)
            {
                Debug.Log(hit.transform.gameObject);
                HingeJoint joint = gameObject.AddComponent<HingeJoint>();
                joint.connectedBody = hit.transform.gameObject.GetComponent<Rigidbody>();
                crc.MAXDISTANCE = 100f;
            }
        }
    }

    private void StopHolding()
    {
        Destroy(GetComponent<HingeJoint>());
        crc.MAXDISTANCE = 50f;
    }

    

    public void StartGrappling(Collider collider)
    {
        if (grappleCoroutine != null)
        {
            StopCoroutine(grappleCoroutine);
        }
        StopHolding();
        grappleCoroutine = StartCoroutine(Grappling(collider));
    }

    private IEnumerator Grappling(Collider collider)
    {
        while (true) {
            Vector3 moveVector = grappleHead.transform.position - transform.position;
            if (collider.CompareTag("MoveableObject"))
            {
                if(Vector3.Angle(rigidbody.velocity.normalized, moveVector.normalized) > dampingAngle)
                {
                    Debug.Log(rigidbody.velocity);
                    rigidbody.velocity = rigidbody.velocity * (1-dampingSpeed);
                    Debug.Log(rigidbody.velocity);
                }
                collider.GetComponent<Rigidbody>().AddForceAtPosition(moveVector.normalized * -grappleForce / 2, grappleHead.transform.position);
                rigidbody.AddForce(moveVector.normalized * (grappleForce / 2));
            }
            //WE MOVING
            else if (!collider.CompareTag("Stopper"))
            {
                //Wall BUFFER
                if (collider.name != "Goal")
                {
                    if (Vector3.Angle(rigidbody.velocity.normalized, moveVector.normalized) > dampingAngle)
                    {
                        rigidbody.velocity = rigidbody.velocity * (1 - dampingSpeed);
                    }
                    rigidbody.AddForce(moveVector.normalized * grappleForce);
                }
                else
                {
                    goal.GetComponent<goal>().NextLevel();
                }
            }
            else
            {
                grappleHead.StopGrappling();
            }
            yield return new WaitForSeconds(.02f);
        }
    }

    public void StopGrappling()
    {
        if (grappleCoroutine != null)
        {
            StopCoroutine(grappleCoroutine);
        }
    }

 
}
