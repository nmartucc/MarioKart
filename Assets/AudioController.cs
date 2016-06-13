using UnityEngine;
using System.Collections;

public class AudioController : MonoBehaviour {

	public AudioSource loadup;
	public AudioSource countdown;
	public AudioSource start;
	public AudioSource loop;
	// Use this for initialization
	void Start () {
		loadup.Play ();
		countdown.PlayDelayed (loadup.clip.length-1);
		start.PlayDelayed(loadup.clip.length+countdown.clip.length-2);
		loop.PlayDelayed (loadup.clip.length+countdown.clip.length+start.clip.length-2);
	}
	
	// Update is called once per frame
	void Update () {

	}
}
