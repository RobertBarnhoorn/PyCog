using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoboticArm : MonoBehaviour {

	public const int QUIT = 0;
	public const int ROTATE_JOINT = 1;

	//this are the parts of the robotic arm
	public GameObject box;
	public Transform part0;
	public Transform part1;
	public Transform part2;
	public Transform part3;
	public Transform gripLeft;
	public Transform gripRight;
	private TCPServer java_interface;

	void Start () {
		java_interface = GameObject.FindObjectOfType<TCPServer>();
	}
	
	// Update is called once per frame
	void Update () {
		//HandleInput(); // Keyboard control for debugging the arm's movement
		TCPMessage message = java_interface.GetNextMessage();
		if (message != null) {
			InterpretAgentMessage (message);
		}
	}

    public void InterpretAgentMessage(TCPMessage message) {
		print (message.id + "$" + message.getData () [0] + "$" + message.getData () [1]);
        switch (message.id) {
		case ROTATE_JOINT: // ROTATE_JOINT
			int joint = Int32.Parse (message.getData () [0]);
			int degrees = Int32.Parse (message.getData () [1]);
			print ("Rotating joint " + joint + " by " + degrees + " degrees.");
			if (joint == 0)
				rotatePart0 (degrees);
			else if (joint == 1)
				rotatePart1 (degrees);
			else if (joint == 2)
				rotatePart2 (degrees);
			else if (joint == 3)
				rotatePart3 (degrees);
			else if (joint == 4)
				grip (degrees);
			break;
		default:
        	break;
        }
    }

	void FixedUpdate () {
	}

	// For testing: allows arm to be controlled by mouse and keyboard
	void HandleInput() {
		float t = Time.deltaTime; // Used to make rotation speed frame-rate independent
		float horizontal = Input.GetAxis("Horizontal");
		float vertical   = Input.GetAxis ("Vertical");
		float mouseX     = Input.GetAxis("Mouse X");
		float mouseY     = Input.GetAxis("Mouse Y");
		bool leftClick   = Input.GetMouseButton(0);
		bool rightClick  = Input.GetMouseButton(1);

		if (horizontal != 0) {
			rotatePart0 (t * 50 * horizontal);
		}
		if (vertical != 0) {
			rotatePart1 (t * 50 * vertical);
		}
		if (mouseX != 0) {
			rotatePart2 (t * 80 * mouseX);
		}
		if (mouseY != 0) {
			rotatePart3 (t * 500 * mouseY);
		}
		if (leftClick) {
			grip (t * 100);
		}
		if (rightClick) {
			grip (-t * 100);
		}
	}

	// Rotate part by val degrees
	public void rotatePart0(float val) {
		part0.Rotate(0f, 0f, val);
	}

	// Rotate part by val degrees
	public void rotatePart1(float val) {
		part1.Rotate(0f, 0f, val);
	}

	// Rotate part by val degrees
	public void rotatePart2(float val) {
		part2.Rotate(0f, 0f, val);
	}

	// Rotate part by val degrees
	public void rotatePart3(float val) {
		part3.Rotate(-val, 0f, 0f);

	}

	// Close/open grip by val degrees
	public void grip(float val) {
		gripLeft.Rotate (0f, 0f, val);
		gripRight.Rotate (0f, 0f, val);
	}

}