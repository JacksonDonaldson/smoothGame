using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class horitontalCamera : MonoBehaviour
{
    [SerializeField] private float minX;
    [SerializeField] private float maxX;

    [SerializeField] private float lookahead = 5;
    [SerializeField] private Transform playerT;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float camX = Math.Max(Math.Min(maxX, playerT.position.x + lookahead), minX);
        transform.position = new Vector3(camX, transform.position.y, transform.position.z);
    }
}
