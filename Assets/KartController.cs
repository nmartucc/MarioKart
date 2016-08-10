using UnityEngine;
using System.Collections;
using System.IO.Ports;

public class KartController : MonoBehaviour {

	private bool showItem = false;
	private static int speed = 100;

	void OnTriggerEnter(Collider other) 
	{
		if (other.gameObject.CompareTag ("Off Road")) {
			speed = 50;
            PortMan.Write(speed);
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
            PortMan.Write(speed);
        }
	}

	IEnumerator respawn(Collider obj,float respawnTime)
	{
		obj.gameObject.SetActive (false);
		yield return new WaitForSeconds(respawnTime);
		obj.gameObject.SetActive (true);
	}

}