using UnityEngine;
using System.Collections;

public class KartController : MonoBehaviour {

	public bool showItem = false;

	
	void Start ()
	{

	}
	

	
	void OnTriggerEnter(Collider other) 
	{
		if (other.gameObject.CompareTag ("Item Box"))
		{
			showItem = true;
			StartCoroutine(respawn(other,7));
		}
	}

	IEnumerator respawn(Collider obj,float respawnTime)
	{
		obj.gameObject.SetActive (false);
		yield return new WaitForSeconds(respawnTime);
		obj.gameObject.SetActive (true);
	}


}