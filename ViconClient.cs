/**************************************************************************************/
/**************************************************************************************/
/**                                                                                  **/
/**                         DO NOT EDIT THIS SCRIPT!!                                **/
/**                                                                                  **/
/**************************************************************************************/
/**************************************************************************************/

// This class provides a method for obtaining Vicon data
// Directions:
// 		1. Attach to any GameObject

using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;

public class ViconClient : MonoBehaviour {

	private string ViconServerIP = "127.0.0.1";
	private int ViconServerPort = 11001;
	
	public struct Translation_
	{
	    public float x;
	    public float y;
	    public float z;
	}
	
	public struct Quaternion_
	{
	    public float x;
	    public float y;
	    public float z;
	    public float w;
	}

	public struct Euler_ {
		public float h;
		public float p;
		public float r;
	}
	
	public class Body_
        {
            public string name;
            public Translation_ translation;
            public Quaternion_ quaternion;
			public Euler_ euler;

            public Body_() {
                name = "";
                translation = new Translation_();
                quaternion = new Quaternion_();
				euler = new Euler_();
            }

            public const int BufferSize = sizeof(int) + 32 + (sizeof(double)*7);
            public byte[] GetBytes()
            {
                byte[] buff = new byte[BufferSize];
                int pos = 0;
                char[] n = new char[32];
                name.CopyTo(0, n, 0, Math.Min(32, name.Length));
                for (pos = 0; pos < Math.Min(32, name.Length); pos++)
                {
                    byte[] chars = System.BitConverter.GetBytes(name[pos]);
                    if (chars.Length > 0)
                        buff[pos] = System.BitConverter.GetBytes(name[pos])[0];
                    else
                        buff[pos] = 0;
                }
                System.BitConverter.GetBytes((double)translation.x).CopyTo(buff, pos);
                pos += sizeof(double);
                System.BitConverter.GetBytes((double)translation.y).CopyTo(buff, pos);
                pos += sizeof(double);
                System.BitConverter.GetBytes((double)translation.z).CopyTo(buff, pos);
                pos += sizeof(double);
                System.BitConverter.GetBytes((double)quaternion.x).CopyTo(buff, pos);
                pos += sizeof(double);
                System.BitConverter.GetBytes((double)quaternion.y).CopyTo(buff, pos);
                pos += sizeof(double);
                System.BitConverter.GetBytes((double)quaternion.z).CopyTo(buff, pos);
                pos += sizeof(double);
                System.BitConverter.GetBytes((double)quaternion.w).CopyTo(buff, pos);
                pos += sizeof(double);

                return buff;
            }

            public void FromBytes(byte[] buf)
            {
                int pos = 0;
                int length = (int)System.BitConverter.ToInt32(buf, pos);
                pos += sizeof(int);
                name = System.Text.Encoding.ASCII.GetString(buf, pos, length);
                pos += 32;
                translation.x = (float)System.BitConverter.ToDouble(buf, pos);
                pos += sizeof(double);
                translation.y = (float)System.BitConverter.ToDouble(buf, pos);
                pos += sizeof(double);
                translation.z = (float)System.BitConverter.ToDouble(buf, pos);
                pos += sizeof(double);
                quaternion.x = (float)System.BitConverter.ToDouble(buf, pos);
                pos += sizeof(double);
                quaternion.y = (float)System.BitConverter.ToDouble(buf, pos);
                pos += sizeof(double);
                quaternion.z = (float)System.BitConverter.ToDouble(buf, pos);
                pos += sizeof(double);
                quaternion.w = (float)System.BitConverter.ToDouble(buf, pos);
                pos += sizeof(double);
            }
        }
	
	private Socket viconSocket;
	private System.Collections.Generic.Dictionary<string, Body_> Bodies_ = new System.Collections.Generic.Dictionary<string, Body_>();
	private System.Collections.Generic.Dictionary<string, Body_> BodyAdjustments_ = new System.Collections.Generic.Dictionary<string, Body_>();
	
	public class ReceiveStateObject
	{
		public Socket sock = null;
		public int BufferSize;
		public byte[] buffer;
	}
		
	void Start() {

		// START EDITABLE SECTION: Here we declare a body adjustment for each Vicon tracker to address Blade's limitations for creating objects

		// Adjustment for RiftDK1
		Body_ RiftDK1 = new Body_();
		RiftDK1.name = "RiftDK1";
		RiftDK1.translation.x = -0.022f;
		RiftDK1.translation.y = -0.095f;
		RiftDK1.translation.z = -0.018f;
		RiftDK1.euler.h = 342.5f;
		RiftDK1.euler.p = 347.0f;
		RiftDK1.euler.r = 196.5f;
		BodyAdjustments_[RiftDK1.name] = RiftDK1;
		
		// Adjustment for RightGlove
		Body_ RightGlove = new Body_();
		RightGlove.name = "RightGlove";
		RightGlove.translation.x = 0f;
		RightGlove.translation.y = 0f;
		RightGlove.translation.z = 0f;
		RightGlove.euler.h = 87.83496f;
		RightGlove.euler.p = 7.07246f;
		RightGlove.euler.r = 86.11337f;
		BodyAdjustments_[RightGlove.name] = RightGlove;

		// Adjustment for RightWiimote
		Body_ RightWiimote = new Body_();
		RightWiimote.name = "RightWiimote";
		RightWiimote.translation.x = 0f;
		RightWiimote.translation.y = 0f;
		RightWiimote.translation.z = 0f;
		RightWiimote.euler.h = 90f;
		RightWiimote.euler.p = 5f;
		RightWiimote.euler.r = 90f;
		BodyAdjustments_[RightWiimote.name] = RightWiimote;
		
		// Adjustment for LeftGlove
		Body_ LeftGlove = new Body_();
		LeftGlove.name = "LeftGlove";
		LeftGlove.translation.x = 0f;
		LeftGlove.translation.y = 0f;
		LeftGlove.translation.z = 0f;
		LeftGlove.euler.h = 86.82818f;
		LeftGlove.euler.p = 359.3969f;
		LeftGlove.euler.r = 271.8313f;
		BodyAdjustments_[LeftGlove.name] = LeftGlove;
		
		// Adjustment for LeftWiimote
		Body_ LeftWiimote = new Body_();
		LeftWiimote.name = "LeftWiimote";
		LeftWiimote.translation.x = 0f;
		LeftWiimote.translation.y = 0f;
		LeftWiimote.translation.z = 0f;
		LeftWiimote.euler.h = 90f;
		LeftWiimote.euler.p = -10f;
		LeftWiimote.euler.r = 74f;
		BodyAdjustments_[LeftWiimote.name] = LeftWiimote;
		
		// Adjustment for RightShoe
		Body_ RightShoe = new Body_ ();
		RightShoe.name = "RightShoe";
		RightShoe.translation.x = 0.0f;
		RightShoe.translation.y = -0.015f;
		RightShoe.translation.z = 0.0f;
		RightShoe.euler.h = 232.4361f;
		RightShoe.euler.p = 18.79926f;
		RightShoe.euler.r = 112.5233f;
		BodyAdjustments_ [RightShoe.name] = RightShoe;
		
		// Adjustment for LeftShoe
		Body_ LeftShoe = new Body_ ();
		LeftShoe.name = "LeftShoe";
		LeftShoe.translation.x = 0.0f;
		LeftShoe.translation.y = -0.005f;
		LeftShoe.translation.z = 0.0f;
		LeftShoe.euler.h = 310.6974f;
		LeftShoe.euler.p = 17.13592f;
		LeftShoe.euler.r = 255.4526f;
		BodyAdjustments_ [LeftShoe.name] = LeftShoe;

		// END EDITABLE SECTION

		viconSocket = new Socket(AddressFamily.InterNetwork, 
                    SocketType.Stream, ProtocolType.Tcp );
		//Debug.Log ("Trying to open a connection to: " + ViconServerIP);
        IPAddress ipAddress;
		IPAddress.TryParse (ViconServerIP, out ipAddress);
        
		System.IAsyncResult result = viconSocket.BeginConnect(ipAddress, ViconServerPort, null, null);
		result.AsyncWaitHandle.WaitOne(1000, true);
		if(!viconSocket.Connected)
		{
			//Debug.Log ("Closing the socket");
			viconSocket.Close ();
			Destroy (this);
			return;
		}
		
		ReceiveStateObject state = new ReceiveStateObject();
		state.sock = viconSocket;
		state.BufferSize = Body_.BufferSize;
		state.buffer = new byte[state.BufferSize];
		viconSocket.BeginReceive(state.buffer, 0, state.BufferSize, 0, new System.AsyncCallback(ReceiveCallback), state);
	}
	
	System.Threading.Mutex StateMutex = new System.Threading.Mutex();
	
	void Update() {
		StateMutex.WaitOne();
		if(Bodies_ != null && BodyAdjustments_ != null)
		{
			foreach(System.Collections.Generic.KeyValuePair<string,Body_> kv in Bodies_)
			{
				bool adjustmentMade = false;
				foreach(System.Collections.Generic.KeyValuePair<string,Body_> adjustment in BodyAdjustments_) {
					if(kv.Key == adjustment.Key) {
						InputBroker.SetPosition(kv.Key, new Vector3(
							kv.Value.translation.x + adjustment.Value.translation.x,
							kv.Value.translation.y + adjustment.Value.translation.y,
							kv.Value.translation.z + adjustment.Value.translation.z));
						Quaternion q = new Quaternion(kv.Value.quaternion.x, kv.Value.quaternion.y, kv.Value.quaternion.z, kv.Value.quaternion.w) *
							Quaternion.Euler(adjustment.Value.euler.p, adjustment.Value.euler.h, adjustment.Value.euler.r);
						InputBroker.SetRotation(kv.Key, q);
						adjustmentMade = true;
					}
					if(!adjustmentMade) {
						InputBroker.SetPosition(kv.Key, new Vector3(kv.Value.translation.x, kv.Value.translation.y, kv.Value.translation.z));
						Quaternion q = new Quaternion(kv.Value.quaternion.x, kv.Value.quaternion.y, kv.Value.quaternion.z, kv.Value.quaternion.w);
						InputBroker.SetRotation(kv.Key, q);
					}
				}
			}
		}
		StateMutex.ReleaseMutex();
	}
	
	private void ReceiveCallback( System.IAsyncResult ar )
	{
		ReceiveStateObject state = (ReceiveStateObject) ar.AsyncState;
		byte[] buff = state.buffer;
		
		try{
			Body_ b = new Body_();
			b.FromBytes(buff);
			//Debug.Log ("Received " + b.name + " translation, (x,y,z)=" + b.translation.x + "," + b.translation.y + "," + b.translation.z + "\n");
			StateMutex.WaitOne();
			Bodies_[b.name] = b;
			StateMutex.ReleaseMutex();
		}
		catch(Exception ex)
		{
			Debug.Log ("Caught: " + ex.Message);
		}
		viconSocket.BeginReceive(state.buffer, 0, state.BufferSize, 0, new System.AsyncCallback(ReceiveCallback), state);
	}
}
