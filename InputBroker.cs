/**************************************************************************************/
/**************************************************************************************/
/**                                                                                  **/
/**                         DO NOT EDIT THIS SCRIPT!!                                **/
/**                                                                                  **/
/**************************************************************************************/
/**************************************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputBroker {
	
	private static Dictionary<string, bool> _Keys;
	private static Dictionary<string, bool> _KeyPresses;
	private static Dictionary<string, float> _Axes;
	private static Dictionary<string, Vector3> _Positions;
	private static Dictionary<string, Quaternion> _Rotations;
	private static bool Initialized = false;
	
	static Dictionary<string, bool> Keys
	{
		get
		{
			if( !Initialized ) init();
			return _Keys;
		}
	}
	static Dictionary<string, bool> KeyPresses
	{
		get
		{
			if( !Initialized ) init();
			return _KeyPresses;
		}
	}
	static Dictionary<string, float> Axes
	{
		get
		{
			if( !Initialized ) init();
			return _Axes;
		}
	}
	static Dictionary<string, Vector3> Positions
	{
		get
		{
			if( !Initialized ) init();
			return _Positions;
		}
	}
	static Dictionary<string, Quaternion> Rotations
	{
		get
		{
			if( !Initialized ) init();
			return _Rotations;
		}
	}
	
	
	static void init()
	{
		_Keys = new Dictionary<string, bool>();
		_KeyPresses = new Dictionary<string, bool>();
		_Axes = new Dictionary<string, float>();
		_Positions = new Dictionary<string, Vector3>();
		_Rotations = new Dictionary<string, Quaternion>();
		Initialized = true;
	}
	
	public static void SetKeyDown(string name)
	{
		if( !Keys.ContainsKey(name.ToLower()) )
			Keys[name.ToLower()] = false;
		if( !Keys[name.ToLower()] )
			KeyPresses[name.ToLower()] = true;
		Keys[name.ToLower()] = true;
	}
	
	public static void SetKeyUp(string name)
	{
		Keys[name.ToLower()] = false;
	}
	
	public static void SetKey(string name, bool val)
	{
		if(val)
			SetKeyDown (name);
		else
			SetKeyUp (name);
	}
	
	public static bool GetKeyDown(string name)
	{
		try
		{
			return Keys[name.ToLower()];
		}
		catch(KeyNotFoundException)
		{
			Keys[name.ToLower ()] = false;
			return false;
		}
	}
	
	public static bool GetKeyPress(string name)
	{
		try
		{	
			bool val = KeyPresses[name.ToLower()];
			KeyPresses[name.ToLower()] = false;
			return val;
		}
		catch(KeyNotFoundException)
		{
			KeyPresses[name.ToLower()] = false;
			return false;
		}
	}
	
	public static void SetAxis(string axis, float val)
	{
		Axes[axis.ToLower()] = val;
	}
	
	public static float GetAxis(string axis)
	{
		try
		{
			return Axes[axis.ToLower()];
		}
		catch(KeyNotFoundException)
		{
			Axes[axis.ToLower()] = 0;
			return 0;
		}
	}
	
	public static void SetPosition(string name, Vector3 v)
	{
		Positions[name.ToLower ()] = v;
	}
	
	public static Vector3 GetPosition(string name)
	{
		try
		{
			return Positions[name.ToLower ()];
		}
		catch(KeyNotFoundException)
		{
			Positions[name.ToLower ()] = new Vector3();
			return new Vector3();
		}
	}
	
	public static void SetRotation(string name, Quaternion q)
	{
		Rotations[name.ToLower ()] = q;
	}
	
	public static Quaternion GetRotation(string name)
	{
		try
		{
			return Rotations[name.ToLower ()];
		}
		catch(KeyNotFoundException)
		{
			Rotations[name.ToLower ()] = new Quaternion();
			return new Quaternion();
		}
	}
}
