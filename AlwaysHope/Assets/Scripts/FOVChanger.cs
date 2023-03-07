using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class FOVChanger : Image
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override Color color {

        get {
            return base.color;
        }

        set
        {
            base.color = value;
        }
    }

    public override Material materialForRendering // Learned to do this form https://www.youtube.com/watch?v=XJJl19N2KFM
    {
        get
        {
            Material newMat = new Material(base.materialForRendering);
            newMat.SetInt("_StencilComp", (int)CompareFunction.NotEqual);
            Debug.Log(newMat.color);
            return newMat;
        }
    }
}
