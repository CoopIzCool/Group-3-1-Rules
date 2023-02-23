using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactables : MonoBehaviour
{
    // Establish necessary Fields
    public GameObject targetObject;
    public List<GameObject> placedLocations;
    //public GameObject[] placedLocations;
    public bool isSolved;

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
            // Change the fov to be clearer
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject == targetObject)
        {
            // Add to the list, then solve it
            isSolved = true;
        }
    }
}
