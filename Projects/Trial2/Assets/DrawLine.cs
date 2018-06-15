using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour {
    private LineRenderer rend;
    private float counter;
    private float distance;

    public Transform origin;
    public Transform destination;

    public float lineDrawSpeed = 6F;

	// Use this for initialization
	void Start ()
    {

        rend = GetComponent<LineRenderer>();
        rend.SetPosition(0, origin.position);
        rend.SetWidth(0.45f, 0.45f);


        distance = Vector3.Distance(origin.position, destination.position);



    }


    // Update is called once per frame
    void Update () {
        if(counter< distance)
        {
            counter += .1f / lineDrawSpeed;
            float x = Mathf.Lerp(0, distance, counter);

            Vector3 pointA = origin.position;
            Vector3 pointB = destination.position;

            Vector3 pointAlongline = x * Vector3.Normalize(pointB - pointA) + pointA;

            rend.SetPosition(1, pointAlongline);
        }
		
	}
}
