using UnityEngine;
using System.Collections;
using System.IO.Ports;

public class KartController : MonoBehaviour {

	private bool showItem = false;
	// Percent of max allowed speed
	private static int speed = 100;

	void OnTriggerEnter(Collider other) 
	{
		// When entering off road collider, lower max speed
		if (other.gameObject.CompareTag ("Off Road")) {
			speed = 50;
			// Send speed to Arduino
            PortMan.Write(speed);
        }
		else if (other.gameObject.CompareTag ("Item Box")) {
			showItem = true;
			StartCoroutine (respawn (other, 7));
		}
	}

	void OnTriggerExit(Collider other)
	{
		// When leaving off road collider, reset max speed
		if (other.gameObject.CompareTag("Off Road")){
			speed = 100;
			// Send speed to Arduino
            PortMan.Write(speed);
        }
	}

	// Respawns item box after respawn time
	IEnumerator respawn(Collider obj,float respawnTime)
	{
		obj.gameObject.SetActive (false);
		yield return new WaitForSeconds(respawnTime);
		obj.gameObject.SetActive (true);
	}

}