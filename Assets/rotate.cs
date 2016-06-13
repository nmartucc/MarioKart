using UnityEngine;
using System.Collections;

public class rotate : MonoBehaviour {

	void Update () 
	{
		transform.Rotate (new Vector3 (15, 30, 45) * Time.deltaTime);
	}
}
