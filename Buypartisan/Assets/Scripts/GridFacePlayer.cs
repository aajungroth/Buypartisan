﻿// Michael Lee

using UnityEngine;
using System.Collections;

public class GridFacePlayer : MonoBehaviour { 
    
    void Update() 
    { 
        transform.LookAt(Camera.main.transform.position, Vector3.up); 
    } 

}