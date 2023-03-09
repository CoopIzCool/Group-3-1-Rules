using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactables : MonoBehaviour
{
    #region Fields
    // Establish necessary Fields
    public GameObject targetObject;
    public List<GameObject> placedLocations;
    public List<float> timedLocations;
    [SerializeField] public Dictionary<string, int> placedInteractablesDict = new Dictionary<string, int>();
    //public GameObject[] placedLocations;
    public bool isSolved;
    public bool isPositive; // For positive vs harmful items
    [SerializeField] public GameObject secondaryTarget;
    [SerializeField] public bool isPreReqMet = true;
    [SerializeField] private bool isRequired;
    [SerializeField] private AudioSource src;
    [SerializeField] private AudioClip solvedClip;
    [SerializeField] public GameObject mouse;
    [SerializeField] private MeshRenderer placedMesh;

    // JSON Fields
    public string intName;
    private Vector3 startingLoc;
    public bool trackTime;
    private float timer; // tracks the active time between first movement and solving
    public bool paused = false;
    #endregion Fields
    public float Timer
    {
        get { return timer; }
        set { timer = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        startingLoc = transform.position;
        intName = gameObject.name;
    }

    // Update is called once per frame
    void Update()
    {
        if (isSolved)
        {
            this.gameObject.SetActive(false);
        }

        trackTime = (!paused && !startingLoc.Equals(transform.position) && !isSolved ? true : false); // if the object has been moved and hasnt been solved, track active time
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!placedInteractablesDict.ContainsKey(collision.gameObject.name)) // If the object it is currently colliding with is not in the list already, then add it to the list.
        {
            //Debug.Log(collision.gameObject.name);
            placedInteractablesDict.Add(collision.gameObject.name, 1);
            if (collision.gameObject == targetObject) // Checking to see if the object is our target object
            {
                if (targetObject.GetComponent<Interactables>() != null)
                {
                    //if(targetObject.GetComponent<Rigidbody>().isKinematic == true)
                    //{
                    //    targetObject.GetComponent<Rigidbody>().isKinematic = false;
                    //}
                    if (secondaryTarget != null)
                    {
                        secondaryTarget.GetComponent<Interactables>().isPreReqMet = true;
                        //secondaryTarget.GetComponent<Rigidbody>().isKinematic = true;
                    }
                    else
                    {
                        targetObject.GetComponent<Interactables>().isPreReqMet = true;
                        targetObject.GetComponent<Rigidbody>().isKinematic = true;
                    }

                }

                isSolved = true;
                src.clip = solvedClip;
                src.Play();
                MouseRaycast mouseRay = mouse.GetComponent<MouseRaycast>();
                if (this.isRequired)
                {
                    mouseRay.interactableSolvedCount++;
                }
                else
                {
                    mouseRay.optionalInteractableSolvedCount++;

                }
                if(placedMesh != null)
                {
                    placedMesh.enabled = true;
                }
                if (mouseRay.interactableSolvedCount == mouseRay.interactableSolvedGoal)
                {
                    //Debug.Log("completed level");
                    StartCoroutine(PlayVictory(mouseRay.VictoryClip.length));
                }
            }
        }
        else
        {
            placedInteractablesDict[collision.gameObject.name] = placedInteractablesDict[collision.gameObject.name] + 1;
            //placedInteractablesDict.Add(collision.gameObject, placedInteractablesDict[collision.gameObject] + 1);
        }
        //if (!placedLocations.Contains(collision.gameObject)) // If the object it is currently colliding with is not in the list already, then add it to the list.
        //{ 
        //    placedLocations.Add(collision.gameObject);
        //    if (collision.gameObject == targetObject) // Checking to see if the object is our target object
        //    {
        //        if(targetObject.GetComponent<Interactables>() != null)
        //        {
        //            //if(targetObject.GetComponent<Rigidbody>().isKinematic == true)
        //            //{
        //            //    targetObject.GetComponent<Rigidbody>().isKinematic = false;
        //            //}
        //            targetObject.GetComponent<Interactables>().isPreReqMet = true;
        //            targetObject.GetComponent<Rigidbody>().isKinematic= true;
        //        }
        //        isSolved = true;
        //        src.clip = solvedClip;
        //        src.Play();
        //        MouseRaycast mouseRay = mouse.GetComponent<MouseRaycast>();
        //        if(this.isRequired)
        //        {
        //            mouseRay.interactableSolvedCount++;
        //        }
        //        else
        //        {
        //            mouseRay.optionalInteractableSolvedCount++;

        //        }
        //        placedMesh.enabled = true;
        //        if(mouseRay.interactableSolvedCount == mouseRay.interactableSolvedGoal)
        //        {
        //            Debug.Log("completed level");
        //            StartCoroutine(PlayVictory(mouseRay.VictoryClip.length));
        //        }
        //    }
        //}

    }

    private IEnumerator PlayVictory(float time)
    {
        yield return new WaitForSeconds(time);
        src.clip = mouse.GetComponent<MouseRaycast>().VictoryClip;
        src.Play();
    }
}
