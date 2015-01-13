using UnityEngine;
using System.Collections;

// This class provides an interface for simulating Vicon and Wiimote input
// Directions:
// 		1. Attach to any GameObject
//		2. Manipulate the public variables in the Inspector before or during Play
// Note:
//		In the MOCAP Lab, you must disable the InputSimulator or it will override the 
//		actual Vicon and Wiimote data.
public class InputSimulator : MonoBehaviour {

	public bool Marching = false;
	public float MarchingSpeed = 2.0f;
	// 1 = RightUP
	// 2 = RightDOWN
	// 3 = LeftUP
	// 4 = LeftDOWN
	private int MarchingState = 1;

	// Transformation values for Vicon RiftDK1
	private string RiftDK1Vicon = "RiftDK1";
	public float RiftDK1X = 0.0f;
	public float RiftDK1Y = 1.75f;
	public float RiftDK1Z = 0.0f;
	public float RiftDK1H = 0.0f;
	public float RiftDK1P = 15.0f;
	public float RiftDK1R = 0.0f;
	
	// Transformation values for Vicon RightGlove
	private string RightGloveVicon = "RightGlove";
	public float RightGloveX = 0.25f;
	public float RightGloveY = 1.0f;
	public float RightGloveZ = 0.5f;
	public float RightGloveH = 0.0f;
	public float RightGloveP = 0.0f;
	public float RightGloveR = 0.0f;

	// Transformation values for Vicon RightWiimote
	private string RightWiimoteVicon = "RightWiimote";
	public float RightWiimoteX = 0.25f;
	public float RightWiimoteY = 1.0f;
	public float RightWiimoteZ = 0.5f;
	public float RightWiimoteH = 0.0f;
	public float RightWiimoteP = 0.0f;
	public float RightWiimoteR = 0.0f;
	
	// Button values for right Wiimote
	private string RightWiimoteName = "RightWiimote";
	public bool RightWiimoteUp = false;
	public bool RightWiimoteDown = false;
	public bool RightWiimoteLeft = false;
	public bool RightWiimoteRight = false;
	public bool RightWiimoteA = false;
	public bool RightWiimoteB = false;
	public bool RightWiimoteMinus = false;
	public bool RightWiimotePlus = false;
	public bool RightWiimote1 = false;
	public bool RightWiimote2 = false;
	
	// Transformation values for Vicon LeftGlove
	private string LeftGloveVicon = "LeftGlove";
	public float LeftGloveX = -0.25f;
	public float LeftGloveY = 1.0f;
	public float LeftGloveZ = 0.5f;
	public float LeftGloveH = 0.0f;
	public float LeftGloveP = 0.0f;
	public float LeftGloveR = 0.0f;
	
	// Transformation values for Vicon LeftWiimote
	private string LeftWiimoteVicon = "LeftWiimote";
	public float LeftWiimoteX = -0.25f;
	public float LeftWiimoteY = 1.0f;
	public float LeftWiimoteZ = 0.5f;
	public float LeftWiimoteH = 0.0f;
	public float LeftWiimoteP = 0.0f;
	public float LeftWiimoteR = 0.0f;
	
	// Button values for left Wiimote
	private string LeftWiimoteName = "LeftWiimote";
	public bool LeftWiimoteUp = false;
	public bool LeftWiimoteDown = false;
	public bool LeftWiimoteLeft = false;
	public bool LeftWiimoteRight = false;
	public bool LeftWiimoteA = false;
	public bool LeftWiimoteB = false;
	public bool LeftWiimoteMinus = false;
	public bool LeftWiimotePlus = false;
	public bool LeftWiimote1 = false;
	public bool LeftWiimote2 = false;

	// Transformation values for Vicon RightShoe
	private string RightShoeVicon = "RightShoe";
	public float RightShoeX = 0.25f;
	public float RightShoeY = 0.0f;
	public float RightShoeZ = 0.0f;
	public float RightShoeH = 0.0f;
	public float RightShoeP = 0.0f;
	public float RightShoeR = 0.0f;

	// Transformation values for Vicon LeftShoe
	private string LeftShoeVicon = "LeftShoe";
	public float LeftShoeX = -0.25f;
	public float LeftShoeY = 0.0f;
	public float LeftShoeZ = 0.0f;
	public float LeftShoeH = 0.0f;
	public float LeftShoeP = 0.0f;
	public float LeftShoeR = 0.0f;
	
	// Variables for simulating the accuracy of the tracking system
	private float positionalAccuracy = 0.001f;
	private float rotationalAccuracy = 0.1f;
	private float riftDK1XYZNoise = 0.0f;
	private float riftDK1HPRNoise = 0.0f;
	private float rightGloveXYZNoise = 0.0f;
	private float rightGloveHPRNoise = 0.0f;
	private float rightWiimoteXYZNoise = 0.0f;
	private float rightWiimoteHPRNoise = 0.0f;
	private float leftGloveXYZNoise = 0.0f;
	private float leftGloveHPRNoise = 0.0f;
	private float leftWiimoteXYZNoise = 0.0f;
	private float leftWiimoteHPRNoise = 0.0f;
	private float rightShoeXYZNoise = 0.0f;
	private float rightShoeHPRNoise = 0.0f;
	private float leftShoeXYZNoise = 0.0f;
	private float leftShoeHPRNoise = 0.0f;
	

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
		// Only use input simulator if not in MOCAP Lab
		// Simulate tracking noise
		SimulateTrackingNoise();
		
		// Update InputBroker
		UpdateInputBroker();
	}
	
	void SimulateMarching() {
	
		if(Marching) {
			// right up
			if(MarchingState == 1) {
				RightShoeY += 0.3f * Time.deltaTime * MarchingSpeed;
				if(RightShoeY >= 0.3f) {
					MarchingState = 2;
				}
			}
			else if(MarchingState == 2) {
				RightShoeY -= 0.3f * Time.deltaTime * MarchingSpeed;
				if(RightShoeY <= 0.0f) {
					MarchingState = 3;
					RightShoeY = 0.0f;
				}
			}
			else if(MarchingState == 3) {
				LeftShoeY += 0.3f * Time.deltaTime * MarchingSpeed;
				if(LeftShoeY >= 0.3f) {
					MarchingState = 4;
				}
			}
			else if(MarchingState == 4) {
				LeftShoeY -= 0.3f * Time.deltaTime * MarchingSpeed;
				if(LeftShoeY <= 0.0f) {
					MarchingState = 1;
					LeftShoeY = 0.0f;
				}
			}
		}
	}
	
	// Simulate tracking noise based on positional and rotational accuracy
	void SimulateTrackingNoise() {
		
		// First determine actual values by removing last frame's noise
		// For the RiftDK1
		RiftDK1X -= riftDK1XYZNoise;
		RiftDK1Y -= riftDK1XYZNoise;
		RiftDK1Z -= riftDK1XYZNoise;
		RiftDK1H -= riftDK1HPRNoise;
		RiftDK1P -= riftDK1HPRNoise;
		RiftDK1R -= riftDK1HPRNoise;
		// For the RightGlove
		RightGloveX -= rightGloveXYZNoise;
		RightGloveY -= rightGloveXYZNoise;
		RightGloveZ -= rightGloveXYZNoise;
		RightGloveH -= rightGloveHPRNoise;
		RightGloveP -= rightGloveHPRNoise;
		RightGloveR -= rightGloveHPRNoise;
		// For the RightWiimote
		RightWiimoteX -= rightWiimoteXYZNoise;
		RightWiimoteY -= rightWiimoteXYZNoise;
		RightWiimoteZ -= rightWiimoteXYZNoise;
		RightWiimoteH -= rightWiimoteHPRNoise;
		RightWiimoteP -= rightWiimoteHPRNoise;
		RightWiimoteR -= rightWiimoteHPRNoise;
		// For the LeftGlove
		LeftGloveX -= leftGloveXYZNoise;
		LeftGloveY -= leftGloveXYZNoise;
		LeftGloveZ -= leftGloveXYZNoise;
		LeftGloveH -= leftGloveHPRNoise;
		LeftGloveP -= leftGloveHPRNoise;
		LeftGloveR -= leftGloveHPRNoise;
		// For the LeftWiimote
		LeftWiimoteX -= leftWiimoteXYZNoise;
		LeftWiimoteY -= leftWiimoteXYZNoise;
		LeftWiimoteZ -= leftWiimoteXYZNoise;
		LeftWiimoteH -= leftWiimoteHPRNoise;
		LeftWiimoteP -= leftWiimoteHPRNoise;
		LeftWiimoteR -= leftWiimoteHPRNoise;
		// For RightShoe
		RightShoeX -= rightShoeXYZNoise;
		RightShoeY -= rightShoeXYZNoise;
		RightShoeZ -= rightShoeXYZNoise;
		RightShoeH -= rightShoeHPRNoise;
		RightShoeP -= rightShoeHPRNoise;
		RightShoeR -= rightShoeHPRNoise;
		// For LeftShoe
		LeftShoeX -= leftShoeXYZNoise;
		LeftShoeY -= leftShoeXYZNoise;
		LeftShoeZ -= leftShoeXYZNoise;
		LeftShoeH -= leftShoeHPRNoise;
		LeftShoeP -= leftShoeHPRNoise;
		LeftShoeR -= leftShoeHPRNoise;
		
		// Now determine this frame's noise based on positional and rotational accuracy
		riftDK1XYZNoise = Random.Range(-positionalAccuracy/2.0f, positionalAccuracy/2.0f);
		riftDK1HPRNoise = Random.Range(-rotationalAccuracy/2.0f, rotationalAccuracy/2.0f);
		rightGloveXYZNoise = Random.Range(-positionalAccuracy/2.0f, positionalAccuracy/2.0f);
		rightGloveHPRNoise = Random.Range(-rotationalAccuracy/2.0f, rotationalAccuracy/2.0f);
		rightWiimoteXYZNoise = Random.Range(-positionalAccuracy/2.0f, positionalAccuracy/2.0f);
		rightWiimoteHPRNoise = Random.Range(-rotationalAccuracy/2.0f, rotationalAccuracy/2.0f);
		leftGloveXYZNoise = Random.Range(-positionalAccuracy/2.0f, positionalAccuracy/2.0f);
		leftGloveHPRNoise = Random.Range(-rotationalAccuracy/2.0f, rotationalAccuracy/2.0f);
		leftWiimoteXYZNoise = Random.Range(-positionalAccuracy/2.0f, positionalAccuracy/2.0f);
		leftWiimoteHPRNoise = Random.Range(-rotationalAccuracy/2.0f, rotationalAccuracy/2.0f);
		rightShoeXYZNoise = Random.Range(-positionalAccuracy/2.0f, positionalAccuracy/2.0f);
		rightShoeHPRNoise = Random.Range(-rotationalAccuracy/2.0f, rotationalAccuracy/2.0f);
		leftShoeXYZNoise = Random.Range(-positionalAccuracy/2.0f, positionalAccuracy/2.0f);
		leftShoeHPRNoise = Random.Range(-rotationalAccuracy/2.0f, rotationalAccuracy/2.0f);
		
		SimulateMarching();
		
		// Finally add this frame's noise to actual values
		// For the RiftDK1
		RiftDK1X += riftDK1XYZNoise;
		RiftDK1Y += riftDK1XYZNoise;
		RiftDK1Z += riftDK1XYZNoise;
		RiftDK1H += riftDK1HPRNoise;
		RiftDK1P += riftDK1HPRNoise;
		RiftDK1R += riftDK1HPRNoise;
		// For the RightGlove
		RightGloveX += rightGloveXYZNoise;
		RightGloveY += rightGloveXYZNoise;
		RightGloveZ += rightGloveXYZNoise;
		RightGloveH += rightGloveHPRNoise;
		RightGloveP += rightGloveHPRNoise;
		RightGloveR += rightGloveHPRNoise;
		// For the RightWiimote
		RightWiimoteX += rightWiimoteXYZNoise;
		RightWiimoteY += rightWiimoteXYZNoise;
		RightWiimoteZ += rightWiimoteXYZNoise;
		RightWiimoteH += rightWiimoteHPRNoise;
		RightWiimoteP += rightWiimoteHPRNoise;
		RightWiimoteR += rightWiimoteHPRNoise;
		// For the LeftGlove
		LeftGloveX += leftGloveXYZNoise;
		LeftGloveY += leftGloveXYZNoise;
		LeftGloveZ += leftGloveXYZNoise;
		LeftGloveH += leftGloveHPRNoise;
		LeftGloveP += leftGloveHPRNoise;
		LeftGloveR += leftGloveHPRNoise;
		// For the LeftWiimote
		LeftWiimoteX += leftWiimoteXYZNoise;
		LeftWiimoteY += leftWiimoteXYZNoise;
		LeftWiimoteZ += leftWiimoteXYZNoise;
		LeftWiimoteH += leftWiimoteHPRNoise;
		LeftWiimoteP += leftWiimoteHPRNoise;
		LeftWiimoteR += leftWiimoteHPRNoise;
		// For RightShoe
		RightShoeX += rightShoeXYZNoise;
		RightShoeY += rightShoeXYZNoise;
		RightShoeZ += rightShoeXYZNoise;
		RightShoeH += rightShoeHPRNoise;
		RightShoeP += rightShoeHPRNoise;
		RightShoeR += rightShoeHPRNoise;
		// For LeftShoe
		LeftShoeX += leftShoeXYZNoise;
		LeftShoeY += leftShoeXYZNoise;
		LeftShoeZ += leftShoeXYZNoise;
		LeftShoeH += leftShoeHPRNoise;
		LeftShoeP += leftShoeHPRNoise;
		LeftShoeR += leftShoeHPRNoise;
	}
	
	// Store InputSimulator values in InputBroker 
	void UpdateInputBroker () {

		// Update RiftDK1's Vicon
		InputBroker.SetPosition(RiftDK1Vicon, new Vector3(RiftDK1X, RiftDK1Y, RiftDK1Z));
		Quaternion OculusRiftQuaternion = Quaternion.Euler(RiftDK1P, RiftDK1H, RiftDK1R);
		InputBroker.SetRotation(RiftDK1Vicon, OculusRiftQuaternion);

		// Update RightGlove's Vicon
		InputBroker.SetPosition(RightGloveVicon, new Vector3(RightGloveX, RightGloveY, RightGloveZ));
		Quaternion RightGloveQuaternion = Quaternion.Euler(RightGloveP, RightGloveH, RightGloveR);
		InputBroker.SetRotation(RightGloveVicon, RightGloveQuaternion);
		
		// Update RightWiimote's Vicon
		InputBroker.SetPosition(RightWiimoteVicon, new Vector3(RightWiimoteX, RightWiimoteY, RightWiimoteZ));
		Quaternion RightWiimoteQuaternion = Quaternion.Euler(RightWiimoteP, RightWiimoteH, RightWiimoteR);
		InputBroker.SetRotation(RightWiimoteVicon, RightWiimoteQuaternion);
		
		// Update RightWiimote's Buttons
		InputBroker.SetKey(RightWiimoteName + ":Up", RightWiimoteUp);
		InputBroker.SetKey(RightWiimoteName + ":Down", RightWiimoteDown);
		InputBroker.SetKey(RightWiimoteName + ":Left", RightWiimoteLeft);
		InputBroker.SetKey(RightWiimoteName + ":Right", RightWiimoteRight);
		InputBroker.SetKey(RightWiimoteName + ":A", RightWiimoteA);
		InputBroker.SetKey(RightWiimoteName + ":B", RightWiimoteB);
		InputBroker.SetKey(RightWiimoteName + ":Minus", RightWiimoteMinus);
		InputBroker.SetKey(RightWiimoteName + ":Plus", RightWiimotePlus);
		InputBroker.SetKey(RightWiimoteName + ":One", RightWiimote1);
		InputBroker.SetKey(RightWiimoteName + ":Two", RightWiimote2);

		// Update LeftGlove's Vicon
		InputBroker.SetPosition(LeftGloveVicon, new Vector3(LeftGloveX, LeftGloveY, LeftGloveZ));
		Quaternion LeftGloveQuaternion = Quaternion.Euler(LeftGloveP, LeftGloveH, LeftGloveR);
		InputBroker.SetRotation(LeftGloveVicon, LeftGloveQuaternion);

		// Update LeftWiimote's Vicon
		InputBroker.SetPosition(LeftWiimoteVicon, new Vector3(LeftWiimoteX, LeftWiimoteY, LeftWiimoteZ));
		Quaternion LeftWiimoteQuaternion = Quaternion.Euler(LeftWiimoteP, LeftWiimoteH, LeftWiimoteR);
		InputBroker.SetRotation(LeftWiimoteVicon, LeftWiimoteQuaternion);
		
		// Update LeftWiimote's Buttons
		InputBroker.SetKey(LeftWiimoteName + ":Up", LeftWiimoteUp);
		InputBroker.SetKey(LeftWiimoteName + ":Down", LeftWiimoteDown);
		InputBroker.SetKey(LeftWiimoteName + ":Left", LeftWiimoteLeft);
		InputBroker.SetKey(LeftWiimoteName + ":Right", LeftWiimoteRight);
		InputBroker.SetKey(LeftWiimoteName + ":A", LeftWiimoteA);
		InputBroker.SetKey(LeftWiimoteName + ":B", LeftWiimoteB);
		InputBroker.SetKey(LeftWiimoteName + ":Minus", LeftWiimoteMinus);
		InputBroker.SetKey(LeftWiimoteName + ":Plus", LeftWiimotePlus);
		InputBroker.SetKey(LeftWiimoteName + ":One", LeftWiimote1);
		InputBroker.SetKey(LeftWiimoteName + ":Two", LeftWiimote2);
		
		// Update RightShoe's Vicon
		InputBroker.SetPosition(RightShoeVicon, new Vector3(RightShoeX, RightShoeY, RightShoeZ));
		Quaternion RightShoeQuaternion = Quaternion.Euler(RightShoeP, RightShoeH, RightShoeR);
		InputBroker.SetRotation(RightShoeVicon, RightShoeQuaternion);
		
		// Update LeftShoe's Vicon
		InputBroker.SetPosition(LeftShoeVicon, new Vector3(LeftShoeX, LeftShoeY, LeftShoeZ));
		Quaternion LeftShoeQuaternion = Quaternion.Euler(LeftShoeP, LeftShoeH, LeftShoeR);
		InputBroker.SetRotation(LeftShoeVicon, LeftShoeQuaternion);
	}
}
