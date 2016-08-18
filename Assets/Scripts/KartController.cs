using UnityEngine;
using System.Collections;
using System.IO.Ports;

public class KartController : MonoBehaviour {

    private bool hasItem = false;
    private bool usingMush = false;
	// Percent of max allowed speed
	private static int speed = 100;

    void Update()
    {
        if (hasItem)
        {
			// grab button data from Arduino
            double[] data = MYOVRDebugInfo.sensordata;
			// if button is pressed
            if (data[6] == 1)
            {
                hasItem = false;
             	MYOVRDebugInfo.showMushroom = false;
                StartCoroutine(useMush());
            }
        }
    }

	void OnTriggerEnter(Collider other) 
	{
		// When entering off road collider, lower max speed unless mushroom is being used
		if (other.gameObject.CompareTag ("Off Road") && !usingMush) {
			speed = 50;
			// Send speed to Arduino
            PortMan.Write(speed);
        }
		else if (other.gameObject.CompareTag ("Item Box")) {
			hasItem = true;
            MYOVRDebugInfo.showMushroom = true;
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

	// increases max speed for 2 seconds
    IEnumerator useMush()
    {
        usingMush = true;
        speed = 125;
        PortMan.Write(speed);
        yield return new WaitForSeconds(2);
        speed = 100;
        PortMan.Write(speed);
        usingMush = false;
        
    }

}