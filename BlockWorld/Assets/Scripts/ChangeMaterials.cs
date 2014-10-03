using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

public class ChangeMaterials : MonoBehaviour {

    public string whichChar;
    public static bool doInit = false;
	int runtest = 0;

	// Use this for initialization
	void Start () {
        if (!doInit) {
            doInit = true;
            #if UNITY_EDITOR
            Material m = AssetDatabase.LoadAssetAtPath("Assets/Materials/BLUEMat.mat", typeof(Material)) as Material;
            m = AssetDatabase.LoadAssetAtPath("Assets/Materials/PURPLEMat.mat", typeof(Material)) as Material;
            m = AssetDatabase.LoadAssetAtPath("Assets/Materials/REDMat.mat", typeof(Material)) as Material;
            m = AssetDatabase.LoadAssetAtPath("Assets/Materials/GREENMat.mat", typeof(Material)) as Material;
			#endif
        }
        //Debug.Log("Starting Coloring for "+whichChar+"!!!!");
        if (whichChar == "") {
            whichChar = this.name;
            //Debug.Log("Changed name to:"+this.name);
        }
        string findWhichMaterial = "BLUEMat";

		GameObject mbody = this.gameObject;

        if (!mbody) Debug.Log ("Couldn't find body for "+whichChar);

        // calculate the material name to be searched for
        switch (whichChar) {
            case "GraveDigger":
            findWhichMaterial = "PURPLEMat";
            //Debug.Log ("Changed GraveDigger coloring");
            break;
            case "GraveDiggerTwo":
            findWhichMaterial = "REDMat";
            //Debug.Log ("Changed GraveDiggerTwo coloring");
            break;
            case "Hamlet":
            findWhichMaterial = "BLUEMat";
            //Debug.Log ("Changed Hamlet coloring");
            break;
            case "Horatio":
            findWhichMaterial = "GREENMat";
            //Debug.Log ("Changed Horatio coloring");
            break;
            default:
            // do nothing
            Debug.Log ("No match!");
            break;

        }

        foreach(Material myMaterial in  Resources.FindObjectsOfTypeAll(typeof(Material))) {
            //Debug.Log ("Material="+myMaterial.name);
            if (myMaterial.name == findWhichMaterial) {
                mbody.renderer.material = myMaterial;
                //Debug.Log ("Found "+findWhichMaterialmb1);
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
//		if (runtest < 2 && whichChar == "Hamlet") {
//			Debug.Log ("Hamlet's name is="+GlobalObjs.Hamlet.name);
//			//CharFuncs cf = (CharFuncs) GlobalObjs.Hamlet.GetComponent (typeof(CharFuncs));
//		//	GlobalObjs.HamletFunc.doRotate(45);
//			//GlobalObjs.doRotate(GlobalObjs.Hamlet, 45);
//			ChangeMaterials c = (ChangeMaterials) GlobalObjs.Hamlet.GetComponent (typeof(ChangeMaterials));
//			//Debug.Log (c.getmymessage(GlobalObjs.Hamlet));
//			runtest++;
//		}
	}
				
	/*public string getmymessage(GameObject who) {
		string temp = "This is my message for " + who.name;
		return temp;
	}*/
}
