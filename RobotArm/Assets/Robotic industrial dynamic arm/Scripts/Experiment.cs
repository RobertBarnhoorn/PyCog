using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Experiment : MonoBehaviour {

	RoboticArm arm;
	TCPServer server;
	int iter;  // Which iteration of training are we on 

	// Use this for initialization
	void Awake() {
		DontDestroyOnLoad (this); // Dont delete when resetting the scene
		if (FindObjectsOfType(GetType()).Length > 1) {
			Destroy(gameObject);
		}
		server = GameObject.FindObjectOfType<TCPServer>();
		arm = GameObject.FindObjectOfType<RoboticArm>();

		iter = 1;
	}
	
	// Update is called once per frame
	void Update () {
		if (arm == null) {
			arm = GameObject.FindObjectOfType<RoboticArm> ();
			if (arm == null) {
				print("ARM STILL NULL");
				return;
			}
		}

		print ("Iteration " + iter + ", tick " + arm.tick);

		if (arm.next_iter == true) {
			iter++;	
			print("RESETTING SCENE FOR NEXT ITERATION...");
			SceneManager.LoadScene ("RobotBoxScene");
			server = GameObject.FindObjectOfType<TCPServer>();
			arm = GameObject.FindObjectOfType<RoboticArm>();
		}

	}
}
