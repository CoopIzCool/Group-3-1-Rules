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
    [SerializeField] private MeshRenderer placedMesh;
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
                placedMesh.enabled = true;
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
