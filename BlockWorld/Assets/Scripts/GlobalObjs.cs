using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlobalObjs : MonoBehaviour
{
	
	public static GameObject Hamlet = null;
	public static CharFuncs HamletFunc = null;
	public static GameObject Horatio = null;
	public static CharFuncs HoratioFunc = null;
	public static GameObject GraveDigger = null;
	public static CharFuncs GraveDiggerFunc = null;
	public static GameObject GraveDiggerTwo = null;
	public static CharFuncs GraveDiggerTwoFunc = null;
	GameObject[] templist = null;
	
	public static List<QueueObj> globalQueue = new List<QueueObj>();
	
//	public static Queue<QueueObj> globalQueue = new Queue<QueueObj>();
	public static GameObject Skull1 = null;
	public static GameObject Skull2 = null;
	public static GameObject Shovel = null;
	public static GameObject Lantern = null;
	public static GameObject Box = null;
	
	// Use this for initialization
	void Start () {
		if (Hamlet == null) {
			templist = GameObject.FindGameObjectsWithTag("Hamlet");
			Hamlet = templist[0];
			HamletFunc = (CharFuncs) Hamlet.GetComponent (typeof(CharFuncs));
			templist = null;
		}
		if (Horatio == null) {
			templist = GameObject.FindGameObjectsWithTag("Horatio");
			Horatio = templist[0];
			HoratioFunc = (CharFuncs) Horatio.GetComponent (typeof(CharFuncs));
			templist = null;
		}
		if (GraveDigger == null) {
			templist = GameObject.FindGameObjectsWithTag("GraveDigger");
			GraveDigger = templist[0];
			GraveDiggerFunc = (CharFuncs) GraveDigger.GetComponent (typeof(CharFuncs));
			templist = null;
		}
		if (GraveDiggerTwo == null) {
			templist = GameObject.FindGameObjectsWithTag("GraveDiggerTwo");
			GraveDiggerTwo = templist[0];
			GraveDiggerTwoFunc = (CharFuncs) GraveDiggerTwo.GetComponent (typeof(CharFuncs));
			templist = null;
		}
		if (Skull1 == null) {
			templist = GameObject.FindGameObjectsWithTag ("Skull1");
			Skull1 = templist[0];
			templist = null;
		}
		if (Skull2 == null) {
			templist = GameObject.FindGameObjectsWithTag ("Skull2");
			Skull2 = templist[0];
			templist = null;
		}
		if (Lantern == null) {
			templist = GameObject.FindGameObjectsWithTag ("Lantern");
			Lantern = templist[0];
			templist = null;
		}
		if (Shovel == null) {
			templist = GameObject.FindGameObjectsWithTag ("Shovel");
			Shovel = templist[0];
			templist = null;
		}
		if (Box == null) {
			templist = GameObject.FindGameObjectsWithTag("Box");
			for (int i=0; i<templist.Length; i++) {
				if (templist[i].name == "Skull1") {
					Box = templist[i];
				}
			}
			//Box = templist[0];
			templist = null;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Hamlet == null) {
			templist = GameObject.FindGameObjectsWithTag("Hamlet");
			Hamlet = templist[0];
			HamletFunc = (CharFuncs) Hamlet.GetComponent (typeof(CharFuncs));
			templist = null;
		}
		if (Horatio == null) {
			templist = GameObject.FindGameObjectsWithTag("Horatio");
			Horatio = templist[0];
			HoratioFunc = (CharFuncs) Horatio.GetComponent (typeof(CharFuncs));
			templist = null;
		}
		if (GraveDigger == null) {
			templist = GameObject.FindGameObjectsWithTag("GraveDigger");
			GraveDigger = templist[0];
			GraveDiggerFunc = (CharFuncs) GraveDigger.GetComponent (typeof(CharFuncs));
			templist = null;
		}
		if (GraveDiggerTwo == null) {
			templist = GameObject.FindGameObjectsWithTag("GraveDiggerTwo");
			GraveDiggerTwo = templist[0];
			GraveDiggerTwoFunc = (CharFuncs) GraveDiggerTwo.GetComponent (typeof(CharFuncs));
			templist = null;
		}
		if (Skull1 == null) {
			templist = GameObject.FindGameObjectsWithTag ("Skull1");
			Skull1 = templist[0];
			templist = null;
		}
		if (Skull2 == null) {
			templist = GameObject.FindGameObjectsWithTag ("Skull2");
			Skull2 = templist[0];
			templist = null;
		}
		if (Lantern == null) {
			templist = GameObject.FindGameObjectsWithTag ("Lantern");
			Lantern = templist[0];
			templist = null;
		}
		if (Shovel == null) {
			templist = GameObject.FindGameObjectsWithTag ("Shovel");
			Shovel = templist[0];
			templist = null;
		}
		if (Box == null) {
			templist = GameObject.FindGameObjectsWithTag("Box");
			for (int i=0; i<templist.Length; i++) {
				if (templist[i].name == "Skull1") {
					Box = templist[i];
				}
			}
			//Box = templist[0];
			templist = null;
		}
	
	}
	
	public static GameObject getObject(string name) {
		name = name.ToLower();
		Debug.Log ("Getting object for:"+name);
		switch (name) {
		case "hamlet":
			return Hamlet;
			break;
		case "horatio":
			return Horatio;
			break;
		case "gravedigger":
			return GraveDigger;
			break;
		case "gravediggertwo":
			return GraveDiggerTwo;
			break;
		case "skull1":
			return Skull1;
			break;
		case "skull2":
			return Skull2;
			break;
		case "shovel":
			return Shovel;
			break;
		case "lantern":
			return Lantern;
			break;
		default:
			return null;
			break;
		}
	}
	
	public static CharFuncs getCharFunc(string name) {
		name = name.ToLower();
		switch (name) {
		case "hamlet":
			return HamletFunc;
			break;
		case "horatio":
			return HoratioFunc;
			break;
		case "gravedigger":
			return GraveDiggerFunc;
			break;
		case "gravediggertwo":
			return GraveDiggerTwoFunc;
			break;
		default:
			return null;
			break;
		}
		
	}
	
	public static CharFuncs getCharFunc(GameObject o) {
		switch (o.name) {
		case "Hamlet":
			return HamletFunc;
			break;
		case "Horatio":
			return HoratioFunc;
			break;
		case "GraveDigger":
			return GraveDiggerFunc;
			break;
		case "GraveDiggerTwo":
			return GraveDiggerTwoFunc;
			break;
		default:
			return null;
			break;
		}
	}
	
	public static void removeOne(int which) {
		int removethis = -1;
		for (int i=0; i< GlobalObjs.globalQueue.Count; i++) {
			if (GlobalObjs.globalQueue[i].msgNum == which) {
				// then remove this one
				removethis = i;
				break;
			}
		}
		Debug.Log ("Removed msg="+which+", item="+removethis);
		GlobalObjs.globalQueue.RemoveAt(removethis);
		if (GlobalObjs.globalQueue.Count == 0) {
			// read next set of lines
			Debug.Log ("Calling next Step, no items in queue");
			InitScript.callNextStep();
		}
	}
	
}

