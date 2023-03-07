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
    #endregion Fields


    // Update is called once per frame
    void Update()
    {
        float distanceToCamera = Vector3.Distance(transform.position, camera.position);
        //Debug.Log(distanceToCamera);
        if( distanceToCamera <= 16)
        {
            meshRenderer.enabled = false;
        }
        else
        {
            meshRenderer.enabled = true;
        }
    }
}
