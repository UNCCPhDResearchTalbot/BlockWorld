using UnityEngine;
using System.Collections;

public class miniQueueObj : MonoBehaviour {
	
	public GameObject target;
	public Vector3 targetpt;
	public int msgnum;
	
	public miniQueueObj(Vector3 p, GameObject t, int n) {
		target = t;
		targetpt = p;
		msgnum = n;
	}
	
	public string getTargetType() {
		if (target == null) {
			return "Vector3";
		} else {
			return "GameObject";
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
