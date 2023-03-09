using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleWalls : MonoBehaviour
{
    #region Fields
    [SerializeField]
    private Transform camera;
    [SerializeField]
    private MeshRenderer meshRenderer;
    private MeshRenderer[] childRenderers;
    private int childrenSize;
    #endregion Fields

    private void Start()
    {
        childrenSize = transform.childCount;
        Debug.Log(childrenSize);
        childRenderers = transform.GetComponentsInChildren<MeshRenderer>();
    }
    // Update is called once per frame
    void Update()
    {
        float distanceToCamera = Vector3.Distance(transform.position, camera.position);
        
        //Debug.Log(distanceToCamera);
        if( distanceToCamera <= 16)
        {
            meshRenderer.enabled = false;
            foreach(MeshRenderer childRenderer in childRenderers)
            {

                childRenderer.enabled = false;
            }
        }
        else
        {
            meshRenderer.enabled = true;
            foreach (MeshRenderer childRenderer in childRenderers)
            {
                childRenderer.enabled = true;
            }
        }

    }
}
