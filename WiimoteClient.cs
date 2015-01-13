/**************************************************************************************/
/**************************************************************************************/
/**                                                                                  **/
/**                         DO NOT EDIT THIS SCRIPT!!                                **/
/**                                                                                  **/
/**************************************************************************************/
/**************************************************************************************/

// This class provides a method for obtaining Wiimote data
// Directions:
// 		1. Attach to the GameObject representing the User

using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;
using System.Net;
using System.Net.Sockets;
using WiimoteLib;

public class WiimoteClient : MonoBehaviour {

	private string WiimoteServerIP = "127.0.0.1";
	private int WiimoteServerPort = 11000;
	
	Socket wiimoteSocket;
	
	class WiimoteInfo
    {
        public Wiimote wm;
        public string ID { get { return wm.ID.ToString(); } }
        private string _Name;
        public string Name
        {
            set { this._Name = value; }
            get { return ToString(); }
        }
        public WiimoteLib.ButtonState ButtonState;
        public WiimoteLib.AccelState AccelState;

        public WiimoteInfo()
        {
            Name = null;
        }

        public WiimoteInfo(Wiimote wm)
        {
            this.wm = wm;
            Name = null;
        }

        public const int BufferSize = 11 + (sizeof(double) * 3) + 32; // Buttons + accel(encode as double) + name (32 chars)
        public byte[] GetBytes()
        {
            byte[] buff = new byte[BufferSize];
            int pos = 0;
            char[] name = new char[32];
            Name.CopyTo(0, name, 0, System.Math.Min(32, Name.Length));

            for (pos = 0; pos < 32; pos++)
            {
                buff[pos] = System.BitConverter.GetBytes(name[pos])[0];
            }
            buff[pos++] = System.BitConverter.GetBytes(ButtonState.A)[0];
            buff[pos++] = System.BitConverter.GetBytes(ButtonState.B)[0];
            buff[pos++] = System.BitConverter.GetBytes(ButtonState.Up)[0];
            buff[pos++] = System.BitConverter.GetBytes(ButtonState.Left)[0];
            buff[pos++] = System.BitConverter.GetBytes(ButtonState.Down)[0];
            buff[pos++] = System.BitConverter.GetBytes(ButtonState.Right)[0];
            buff[pos++] = System.BitConverter.GetBytes(ButtonState.Minus)[0];
            buff[pos++] = System.BitConverter.GetBytes(ButtonState.Home)[0];
            buff[pos++] = System.BitConverter.GetBytes(ButtonState.Plus)[0];
            buff[pos++] = System.BitConverter.GetBytes(ButtonState.One)[0];
            buff[pos++] = System.BitConverter.GetBytes(ButtonState.Two)[0];
            System.BitConverter.GetBytes((double)AccelState.Values.X).CopyTo(buff, pos);
            pos += sizeof(double);
            System.BitConverter.GetBytes((double)AccelState.Values.Y).CopyTo(buff, pos);
            pos += sizeof(double);
            System.BitConverter.GetBytes((double)AccelState.Values.Z).CopyTo(buff, pos);
            pos += sizeof(double);
            return buff;
        }

        public void FromBytes(byte[] wi)
        {
            _Name = System.Text.Encoding.ASCII.GetString(wi, 0, 32);
            int pos = 32;
            ButtonState.A = System.BitConverter.ToBoolean(wi, pos++);
            ButtonState.B = System.BitConverter.ToBoolean(wi, pos++);
            ButtonState.Up = System.BitConverter.ToBoolean(wi, pos++);
            ButtonState.Left = System.BitConverter.ToBoolean(wi, pos++);
            ButtonState.Down = System.BitConverter.ToBoolean(wi, pos++);
            ButtonState.Right = System.BitConverter.ToBoolean(wi, pos++);
            ButtonState.Minus = System.BitConverter.ToBoolean(wi, pos++);
            ButtonState.Home = System.BitConverter.ToBoolean(wi, pos++);
            ButtonState.Plus = System.BitConverter.ToBoolean(wi, pos++);
            ButtonState.One = System.BitConverter.ToBoolean(wi, pos++);
            ButtonState.Two = System.BitConverter.ToBoolean(wi, pos++);
            AccelState.Values.X = (float)System.BitConverter.ToDouble(wi, pos);
            pos += sizeof(double);
            AccelState.Values.Y = (float)System.BitConverter.ToDouble(wi, pos);
            pos += sizeof(double);
            AccelState.Values.Z = (float)System.BitConverter.ToDouble(wi, pos);
            pos += sizeof(double);
        }

        public override string ToString()
        {
            if (_Name != null)
                return _Name.TrimEnd('\0');
            else
                return wm.ID.ToString();
        }
    }
	
	public class ReceiveStateObject
	{
		public Socket sock = null;
		public int BufferSize;
		public byte[] buffer;
	}
	
	System.Collections.Generic.Dictionary<string, WiimoteInfo> Wiimotes = new System.Collections.Generic.Dictionary<string, WiimoteInfo>();
	
	void Start () {	
		wiimoteSocket = new Socket(AddressFamily.InterNetwork, 
                    SocketType.Stream, ProtocolType.Tcp );
		IPHostEntry ipHostInfo = Dns.GetHostEntry(WiimoteServerIP);
        IPAddress ipAddress = ipHostInfo.AddressList[0];
        
		System.IAsyncResult result = wiimoteSocket.BeginConnect(ipAddress, WiimoteServerPort, null, null);
		result.AsyncWaitHandle.WaitOne(1000, true);
		if(!wiimoteSocket.Connected)
		{
			wiimoteSocket.Close ();
			Destroy (this);
			return;
		}
		
		ReceiveStateObject state = new ReceiveStateObject();
		state.sock = wiimoteSocket;
		state.BufferSize = WiimoteInfo.BufferSize;
		state.buffer = new byte[state.BufferSize];
		wiimoteSocket.BeginReceive(state.buffer, 0, state.BufferSize, 0, new System.AsyncCallback(ReceiveCallback), state);
	}
	
	System.Threading.Mutex StateMutex = new System.Threading.Mutex();
	
	// Update is called once per frame
	void Update () {
		StateMutex.WaitOne();
		foreach(System.Collections.Generic.KeyValuePair<string, WiimoteInfo> kv in Wiimotes)
		{
			parsekeys (kv.Key, kv.Value.ButtonState);
			parseaccel(kv.Key, kv.Value.AccelState);
		}
		StateMutex.ReleaseMutex();
	}
	
	private void parsekeys(string name, ButtonState buttons)
	{
		InputBroker.SetKey (name + ":B", buttons.B);
		InputBroker.SetKey (name + ":A", buttons.A);
		InputBroker.SetKey (name + ":Up", buttons.Up);
		InputBroker.SetKey (name + ":Left", buttons.Left);
		InputBroker.SetKey (name + ":Down", buttons.Down);
		InputBroker.SetKey (name + ":Right", buttons.Right);
		InputBroker.SetKey (name + ":Minus", buttons.Minus);
		InputBroker.SetKey (name + ":Home", buttons.Home);
		InputBroker.SetKey (name + ":Plus", buttons.Plus);
		InputBroker.SetKey (name + ":One", buttons.One);
		InputBroker.SetKey (name + ":Two", buttons.Two);
	}
	
	private void parseaccel(string name, AccelState accel)
	{
		InputBroker.SetAxis (name + ":X", accel.Values.X);	
		InputBroker.SetAxis (name + ":Y", accel.Values.Y);
		InputBroker.SetAxis (name + ":Z", accel.Values.Z);
	}
	
	private void ReceiveCallback( System.IAsyncResult ar )
	{
		ReceiveStateObject state = (ReceiveStateObject) ar.AsyncState;
		int bytesreceived = state.sock.EndReceive(ar);
		if( bytesreceived != state.BufferSize )
		{
			throw new System.Exception( "Failed to get an entire response!" );
		}
		byte[] buff = state.buffer;
		WiimoteInfo info = new WiimoteInfo();
		info.FromBytes(buff);
		StateMutex.WaitOne();
		Wiimotes[info.Name] = info;
		StateMutex.ReleaseMutex();
		wiimoteSocket.BeginReceive(state.buffer, 0, state.BufferSize, 0, new System.AsyncCallback(ReceiveCallback), state);
	}
}
