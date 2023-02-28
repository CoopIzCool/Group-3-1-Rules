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
    //public GameObject[] placedLocations;
    public bool isSolved;
    public bool isPositive; // For positive vs harmful items

    [SerializeField] private AudioSource src;
    [SerializeField] private AudioClip solvedClip;
    [SerializeField] public GameObject mouse;
    #endregion Fields

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isSolved)
        {
            this.gameObject.SetActive(false);
            if (isPositive)
            {
                // Make FOV more clear
            }
            else
            {
                // Make FOV less clear
            }
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z + 1);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            this.transform.position = new Vector3(this.transform.position.x - 1, this.transform.position.y, this.transform.position.z);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z-1);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            this.transform.position = new Vector3(this.transform.position.x + 1, this.transform.position.y, this.transform.position.z);
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y-1, this.transform.position.z);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y+1, this.transform.position.z);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!placedLocations.Contains(collision.gameObject)) // If the object it is currently colliding with is not in the list already, then add it to the list.
        { 
            placedLocations.Add(collision.gameObject);
            if (collision.gameObject == targetObject) // Checking to see if the object is our target object
            {
                isSolved = true;
                src.clip = solvedClip;
                src.Play();
                MouseRaycast mouseRay = mouse.GetComponent<MouseRaycast>();
                mouseRay.interactableSolvedCount++;
                if(mouseRay.interactableSolvedCount == mouseRay.interactableSolvedGoal)
                {
                    Debug.Log("completed level");
                    StartCoroutine(PlayVictory(mouseRay.VictoryClip.length));
                }
            }
        }

    }

    private IEnumerator PlayVictory(float time)
    {
        yield return new WaitForSeconds(time);
        src.clip = mouse.GetComponent<MouseRaycast>().VictoryClip;
        src.Play();
    }
}
