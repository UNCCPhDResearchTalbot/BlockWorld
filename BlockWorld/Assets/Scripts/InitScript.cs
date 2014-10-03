using UnityEngine;
using System.Collections;

public class InitScript : MonoBehaviour {
	
	//string txtmuch = "45";
	string txtmuchx = "3";
	string txtmuchy = "5";
	string txtx = "2";
	string txty = "1";
	string txtsay = "This is what I want to say here";
	string txtforward = "3";
	
	// for mode dropdowns
	private Vector2 scrollViewVector = Vector2.zero;
    public Rect dropDownRect = new Rect(125,50,125,300);
    public static string[] list = {"Choose Mode:","Baseline", "BML", "NLP", "Rules", "FDG"};
    int indexNumber;
    bool show = false;
	
	float timer = 0.0f;
	float timerMax = 5.0f;
	bool starting = false;
	
	//bool shrinking = false;
	//bool growing = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (starting) {
			timer += Time.deltaTime;
			if (timer >= timerMax) {
				// ready to start
				RunPlay ();
				timer = 0.0f;
				starting = false;
			}
		}
		/*if (shrinking) {
			if (!GlobalObjs.TesterChar.animation.IsPlaying("Shrink")) {
				// attach object
				GlobalObjs.Box.transform.parent = GlobalObjs.TesterCharParent.transform;
				// do I want it to always be on the right/left of the char?  By how much?
				
				GlobalObjs.Box.transform.localPosition=new Vector3(.5f, 0f, 0f);
				GlobalObjs.Box.transform.localRotation=Quaternion.identity;
				shrinking = false;
				growing = true;
				// stand back up
				GlobalObjs.TesterChar.animation.Play ("Grow");
			}
		}
		if (growing) {
			if (!GlobalObjs.TesterChar.animation.IsPlaying("Grow")) {
				Debug.Log ("Done running anim");
				growing = false;
				GlobalObjs.TesterCharParent.transform.position += -1*3*GlobalObjs.TesterChar.transform.forward;
				
			}
		}*/
		
	}
	
	void OnGUI() {
		
		//txtmuch = GUI.TextField(new Rect(780, 30, 40, 30), txtmuch, 4);
		txtmuchx = GUI.TextField (new Rect(780, 30, 40, 30), txtmuchx, 4);
		txtmuchy = GUI.TextField (new Rect(830, 30, 40, 30), txtmuchy, 4);
		if (GUI.Button(new Rect(500,30,250,30),"Click to Rotate Hamlet")) {
			float howmuchx;
			float howmuchy;
			bool success = float.TryParse(txtmuchx, out howmuchx);
			success = float.TryParse (txtmuchy, out howmuchy);
			GlobalObjs.HamletFunc.doRotate(howmuchx, howmuchy, null);
			Debug.Log("Clicked the button to rotate");	
		}
		txtx = GUI.TextField (new Rect(780, 70, 40, 30), txtx, 4);
		txty = GUI.TextField (new Rect(830, 70, 40, 30), txty, 4);
		if (GUI.Button (new Rect(500, 70, 250, 30), "Click to Move Hamlet FWD")) {
			float x;
			float y;
			bool success = float.TryParse (txtx, out x);
			success = float.TryParse (txty, out y);
			GlobalObjs.HamletFunc.doWalk(x, y, null);
			Debug.Log ("Clicked the button to walk");
		}
		txtsay = GUI.TextField (new Rect(780, 110, 100, 30), txtsay, 100);
		if (GUI.Button (new Rect(500, 110, 250, 30), "Speak")) {
			GlobalObjs.HamletFunc.doSpeak(txtsay);
			Debug.Log ("Said something");
		}
		if (GUI.Button (new Rect(500, 150, 250, 30), "Stop")) {
			GlobalObjs.HamletFunc.doStopAll();
			Debug.Log ("Stopped everything");
		}
		txtforward = GUI.TextField (new Rect(780, 190, 100, 30), txtforward, 4);
		if (GUI.Button (new Rect(500, 190, 250, 30), "Move Forward")) {
			float thisamt;
			bool success = float.TryParse (txtforward, out thisamt);
			GlobalObjs.HamletFunc.doForward(thisamt);
			Debug.Log ("Moved forward "+thisamt);
		}
		if (GUI.Button (new Rect(25, 150, 100, 30), "Pickup")) {
			Debug.Log ("Pickup");
			//shrinking = true;
			GlobalObjs.HamletFunc.doPickup(GlobalObjs.Box);//.animation.Play("Shrink");
		}
		if (GUI.Button (new Rect(25, 190, 100, 30), "Putdown")) {
			Debug.Log ("Putdown");
			GlobalObjs.HamletFunc.doPutDown();
		}
		
		//bool useBML = GUI.Toggle(new Rect(500, 30, 100, 30), BML, "Use BML File?");
		if (GUI.Button (new Rect(25, 20, 100, 30), "Start Play")) {
			Debug.Log ("Starting Play");	
			//RunPlay();
			starting = true;
			timer = 0.0f;
		}
		
		
		// shows dropdown to choose what to run
		if(GUI.Button(new Rect((dropDownRect.x - 100), dropDownRect.y, dropDownRect.width, 25), ""))
        {
            if(!show)
            {
                show = true;
            }
            else
            {
                show = false;
            }
        }
        if(show)
        {
            scrollViewVector = GUI.BeginScrollView(new Rect((dropDownRect.x - 100), (dropDownRect.y + 25), dropDownRect.width, dropDownRect.height),scrollViewVector,new Rect(0, 0, dropDownRect.width, Mathf.Max(dropDownRect.height, (list.Length*25))));
            GUI.Box(new Rect(0, 0, dropDownRect.width, Mathf.Max(dropDownRect.height, (list.Length*25))), "");           
            for(int index = 0; index < list.Length; index++)
            {               
                if(GUI.Button(new Rect(0, (index*25), dropDownRect.height, 25), ""))
                {
                    show = false;
                    indexNumber = index;
                }
                GUI.Label(new Rect(5, (index*25), dropDownRect.height, 25), list[index]);               
            }
            GUI.EndScrollView();   
        }
        else
        {
            GUI.Label(new Rect((dropDownRect.x - 95), dropDownRect.y, 300, 25), list[indexNumber]);
        }
       
	}
	
	

	
	void RunPlay() {
		// check Mode & run based on that setting
		// use indexNumber, where 1=Baseline, 2=BML, etc
		
		Debug.Log ("Run in mode #"+indexNumber);
		starting = false;
	}
}
