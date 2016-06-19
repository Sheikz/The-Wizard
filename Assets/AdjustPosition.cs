using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class AdjustPosition : MonoBehaviour 
{
    private AdjustPosition adjustPosParent;

    void Awake()
    {
        if (transform.parent)
            adjustPosParent = transform.parent.GetComponent<AdjustPosition>();
    }

    void Start()
    {
        fixPosition();
    }

	void Update () 
	{
        if (Application.isEditor)
            fixPosition();
    }

    void fixPosition()
    {
        if (adjustPosParent)
            return;

        transform.position = new Vector3(Mathf.RoundToInt(transform.position.x*2)/2f,
                                        Mathf.RoundToInt(transform.position.y*2)/2f,
                                        Mathf.RoundToInt(transform.position.z*2)/2f);
    }


}
