using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Data : MonoBehaviour {
	public int sensorNum;
	public double minDist;
	public Image caution;
	Text txt;
	private double dist = 0;//double.PositiveInfinity;
	
	// Use this for initialization
	void Start () {
		Color c = caution.color;
		c.a = 0;
		caution.color = c;
		txt = gameObject.GetComponent<Text>();
		txt.text = "Distance: "+dist;
	}
	
	// Update is called once per frame
	void Update () {
		if (sensorNum == 1) {
			//dist = double.PositiveInfinity; // get dist from sensor 1
		} 
		else if (sensorNum == 2) {
			//dist = double.PositiveInfinity; // get dist from sensor 2
		} 
		else if (sensorNum == 3) {
			//dist = double.PositiveInfinity; // get dist from sensor 3
		}
		if (dist < minDist) {
			Color c = caution.color;
			c.a = 0.6f;
			caution.color = c;
		}
		txt.text = "Distance: "+dist;
	}
}
