﻿using UnityEngine;
using System.Collections;
using System.IO.Ports;

public class KartController : MonoBehaviour {

	public bool showItem = false;
	SerialPort sPort = new SerialPort("COM8", 9600);
	public static double speed = 100;
	
	void Start ()
	{
		sPort.Open ();
	}

    void Update()
    {
        Debug.Log(speed);
    }
		
	void OnTriggerEnter(Collider other) 
	{
		if (other.gameObject.CompareTag ("Off Road")) {
			speed = 50;
			if (sPort.IsOpen) {
				sPort.WriteLine (speed + ",");
			}
		}
		else if (other.gameObject.CompareTag ("Item Box")) {
			showItem = true;
			StartCoroutine (respawn (other, 7));
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("Off Road")){
			speed = 100;
			if (sPort.IsOpen) {
				sPort.WriteLine (speed + ",");
			}
		}
	}

	IEnumerator respawn(Collider obj,float respawnTime)
	{
		obj.gameObject.SetActive (false);
		yield return new WaitForSeconds(respawnTime);
		obj.gameObject.SetActive (true);
	}

	void OnApplicationQuit(){
		sPort.Close();
	}


}