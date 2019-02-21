using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleMan : MonoBehaviour {
    public GameObject Particle;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Particle.GetComponent<Renderer>().enabled = true;
	}
}
