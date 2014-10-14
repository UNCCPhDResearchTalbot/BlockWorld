using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

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
	public static GameObject Audience = null;
	public static GameObject Grave = null;
	public static GameObject StageRight = null;
	public static GameObject CenterBackStage = null;
	
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
		if (Audience == null) {
			templist = GameObject.FindGameObjectsWithTag ("Audience");
			Audience = templist[0];
			templist = null;
		}
		if (Grave == null) {
			templist = GameObject.FindGameObjectsWithTag ("Grave");
			Grave = templist[0];
			templist = null;
		}
		if (StageRight == null) {
			templist = GameObject.FindGameObjectsWithTag ("StageRight");
			StageRight = templist[0];
			templist = null;
		}
		if (CenterBackStage == null) {
			templist = GameObject.FindGameObjectsWithTag ("CenterBackStage");
			CenterBackStage = templist[0];
			templist = null;
		}
		/*if (Box == null) {
			templist = GameObject.FindGameObjectsWithTag("Box");
			for (int i=0; i<templist.Length; i++) {
				if (templist[i].name == "Skull1") {
					Box = templist[i];
				}
			}
			//Box = templist[0];
			templist = null;
		}*/
		
		#if UNITY_EDITOR
            Material m = AssetDatabase.LoadAssetAtPath("Assets/Materials/Hamletmat.mat", typeof(Material)) as Material;
            m = AssetDatabase.LoadAssetAtPath("Assets/Materials/Horatiomat.mat", typeof(Material)) as Material;
            m = AssetDatabase.LoadAssetAtPath("Assets/Materials/GraveDiggermat.mat", typeof(Material)) as Material;
            m = AssetDatabase.LoadAssetAtPath("Assets/Materials/GraveDiggerTwomat.mat", typeof(Material)) as Material;
			#endif
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
		if (Audience == null) {
			templist = GameObject.FindGameObjectsWithTag ("Audience");
			Audience = templist[0];
			templist = null;
		}
		if (Grave == null) {
			templist = GameObject.FindGameObjectsWithTag ("Grave");
			Grave = templist[0];
			templist = null;
		}
		if (StageRight == null) {
			templist = GameObject.FindGameObjectsWithTag ("StageRight");
			StageRight = templist[0];
			templist = null;
		}
		if (CenterBackStage == null) {
			templist = GameObject.FindGameObjectsWithTag ("CenterBackStage");
			CenterBackStage = templist[0];
			templist = null;
		}
		/*if (Box == null) {
			templist = GameObject.FindGameObjectsWithTag("Box");
			for (int i=0; i<templist.Length; i++) {
				if (templist[i].name == "Skull1") {
					Box = templist[i];
				}
			}
			//Box = templist[0];
			templist = null;
		}*/
		
		if (InitScript.started && GlobalObjs.globalQueue.Count == 0) {
			// read next set of lines
			Debug.Log ("Calling next Step, no items in queue");
			InitScript.callNextStep();
		} 
	
	}
	
	public static GameObject getObject(string name) {
		name = name.ToLower();
		//Debug.Log ("Getting object for:"+name);
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
		case "audience":
			return Audience;
			break;
		case "grave":
			return Grave;
			break;
		case "stageright":
			return StageRight;
			break;
		case "centerbackstage":
			return CenterBackStage;
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
		if (GlobalObjs.globalQueue[removethis].action == QueueObj.actiontype.speak) {
			GlobalObjs.globalQueue[removethis].actorFunc.speakNum = -1;
		} else if (GlobalObjs.globalQueue[removethis].action == QueueObj.actiontype.point) {
			GlobalObjs.globalQueue[removethis].actorFunc.pointnum = -1;
		} else {
			GlobalObjs.globalQueue[removethis].actorFunc.workingNum = -1;
		}
		Debug.Log ("Removed msg="+which+", item="+removethis);
		GlobalObjs.globalQueue.RemoveAt(removethis);
/*		if (GlobalObjs.globalQueue.Count == 0) {
			// read next set of lines
			Debug.Log ("Calling next Step, no items in queue");
			InitScript.callNextStep();
		} else {*/
			//Debug.Log ("*****Left in Queue:");
			//for (int j=0; j < GlobalObjs.globalQueue.Count; j++) {
			//	Debug.Log (GlobalObjs.globalQueue[j].msgNum+" - " +GlobalObjs.globalQueue[j].actorObj.name + " " + GlobalObjs.globalQueue[j].action.ToString());
			//}
			//Debug.Log ("*****End of Left in Queue");
//		}
		//Debug.Log ("REMOVED "+which);
	}
	
	public static void printQueue(string msg) {
		Debug.Log ("*****Left in Queue:"+msg);
			for (int j=0; j < GlobalObjs.globalQueue.Count; j++) {
				Debug.Log (GlobalObjs.globalQueue[j].msgNum+" - " +GlobalObjs.globalQueue[j].actorObj.name + " " + GlobalObjs.globalQueue[j].action.ToString());
			}
			Debug.Log ("*****End of Left in Queue"+msg);
	}
	
	public static Material getMaterial(string name) {
		foreach(Material myMaterial in  Resources.FindObjectsOfTypeAll(typeof(Material))) {
            //Debug.Log ("Material="+myMaterial.name);
            if (myMaterial.name == name+"mat") {
                return myMaterial;
                //Debug.Log ("Found "+findWhichMaterialmb1);
            }
        }
		return null;
	}
}

