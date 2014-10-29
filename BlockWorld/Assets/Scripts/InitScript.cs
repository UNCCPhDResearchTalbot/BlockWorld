using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text;

public class InitScript : MonoBehaviour {
	
	//string txtmuch = "45";
	string txtmuchx = "3";
	string txtmuchy = "5";
	string txtx = "2";
	string txty = "1";
	string txtsay = "Your gambols, your songs, your flashes of merriment, that were wont to set the table on a roar? Not one now to mock your own grinning? Quite chop-fallen.  Now get you to my lady's chamber, and tell her, let her paint an inch thick, to this favour she must come. Make her laugh at that.";
	string txtforward = "3";
	
	// for mode dropdowns
	private Vector2 scrollViewVector = Vector2.zero;
    public Rect dropDownRect = new Rect(125,50,125,300);
    public static string[] list = {"Choose Mode:","Baseline", "BML", "NLP", "Rules", "FDG"};
    int indexNumber;
    bool show = false;
	
	float timer = 0.0f;
	float timerMax = 1.0f; // reset to 5 when working
	bool starting = false;
	
	// for file reading
	static char quote = System.Convert.ToChar (34);
	//StreamWriter[] charFiles = null;
	public static bool started = false;
	static string path = @"";
	static string inputFileName = Application.dataPath + @"//Files//InputFile.txt";
	static StreamReader inputFile = null;
	
	// variables for legend
	public  Texture hamletT;
	public  Texture horatioT;
	public  Texture gravediggerT;
	public  Texture gravediggertwoT;
	public  Texture lanternT;
	public  Texture shovelT;
	public  Texture skull1T;
	public  Texture skull2T;
	public  Texture legendBkgrd;
	
	public float startx1 = 5f;
	public float startx2 = 110f;
	public float starty = 80f;
	public float widthtext = 90f;
	public float widthimg = 85f;
	public float heighttext = 30f;
	public float heightimg = 135f;
	public float startximg1 = 5f;
	public float startximg2 = 100f;
	public float startyimg = 100f;
	public float spacing;
	public float linex = .7f;
	public float liney = .5f;
	public Material mat;

	// Use this for initialization
	void Start () {
		spacing = heightimg+heighttext+5f;
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
		//if (started && GlobalObjs.globalQueue.Count == 0) {
		//	callNextStep();
		//}
	}
	
	void OnGUI() {
		
		// legend
		GUI.BeginGroup(new Rect(1200, -3, 200, 900));
			GUI.Box (new Rect(0,-3, 200,900), legendBkgrd);
		
			GUIStyle mystyle = new GUIStyle();
			mystyle.fontSize = 30;
			mystyle.normal.textColor = Color.white;
			GUI.Label (new Rect(20, startx1+20, widthtext*2, heighttext*2), "LEGEND", mystyle);
		
			GUI.Label (new Rect(startx2, starty, widthtext, heighttext), "Hamlet:");
			GUI.Label(new Rect(startximg2, startyimg,widthimg,heightimg), new GUIContent(hamletT));
			
			GUI.Label (new Rect(startx2, starty+(spacing*1), widthtext, heighttext), "Horatio:");
			GUI.Label(new Rect(startximg2, startyimg+(spacing*1),widthimg,heightimg), new GUIContent(horatioT));
		
			GUI.Label (new Rect(startx1, starty, widthtext, heighttext), "GraveDigger 1:");
			GUI.Label(new Rect(startximg1, startyimg,widthimg,heightimg), new GUIContent(gravediggerT));
		
			GUI.Label (new Rect(startx1, starty+(spacing*1), widthtext, heighttext), "GraveDigger 2:");
			GUI.Label(new Rect(startximg1, startyimg+(spacing*1),widthimg,heightimg), new GUIContent(gravediggertwoT));
		
			GUI.Label (new Rect(startx1, starty+(spacing*2), widthtext*2, heighttext*2), "--------------------------------------------");
		
			GUI.Label (new Rect(startx1, starty+30+(spacing*2), widthtext, heighttext), "Shovel:");
			GUI.Label(new Rect(startximg1, startyimg+30+(spacing*2),widthimg,heightimg*2), new GUIContent(shovelT));
		
			GUI.Label (new Rect(startx1, starty+30+(spacing*2.5f), widthtext, heighttext), "Lantern:");
			GUI.Label(new Rect(startximg1, startyimg+30+(spacing*2.5f),widthimg,heightimg*2), new GUIContent(lanternT));
		
			GUI.Label (new Rect(startx2, starty+30+(spacing*2), widthtext, heighttext), "Skull 1:");
			GUI.Label(new Rect(startximg2, startyimg+30+(spacing*2),widthimg,heightimg), new GUIContent(skull1T));
		
			GUI.Label (new Rect(startx2, starty+30+(spacing*2.5f), widthtext, heighttext), "Skull 2:");
			GUI.Label(new Rect(startximg2, startyimg+30+(spacing*2.5f),widthimg,heightimg), new GUIContent(skull2T));
		
		
		GUI.EndGroup();
		//GUI.DrawTexture(new Rect(100,60, 50,50), hamletT, ScaleMode.ScaleToFit, true, 0);
		// end legend
		
		if (started) {
			// show nothing
		} else {
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
				GlobalObjs.HamletFunc.doWalk(x, y, null, false);
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
				GlobalObjs.HamletFunc.doPickup(GlobalObjs.Lantern);//.animation.Play("Shrink");
			}
			if (GUI.Button (new Rect(25, 190, 100, 30), "Putdown")) {
				Debug.Log ("Putdown");
				GlobalObjs.HamletFunc.doPutDown();
			}
			if (GUI.Button (new Rect(25, 230, 100, 30), "Follow")) {
				Debug.Log ("Following");
				GlobalObjs.GraveDiggerFunc.doWalk (GlobalObjs.Grave.transform.position.x, GlobalObjs.Grave.transform.position.z, GlobalObjs.Grave, false);
				GlobalObjs.GraveDiggerTwoFunc.doWalk (GlobalObjs.GraveDigger.transform.position.x, GlobalObjs.GraveDigger.transform.position.z, GlobalObjs.GraveDigger, true);
			}
			if (GUI.Button (new Rect(25, 270, 100, 30), "Point")) {
				Debug.Log ("Pointing");
				GlobalObjs.HamletFunc.doPoint (GlobalObjs.Skull1);
			}
			if (GUI.Button(new Rect(25, 310, 100, 30), "Check Visible")) {
				Debug.Log ("Checking if Grave is visible");
				GlobalObjs.HamletFunc.moveTo = GlobalObjs.Grave.transform.position;
				Debug.Log ("Grave="+GlobalObjs.Grave.transform.position+", Hamlet="+GlobalObjs.Hamlet.transform.position);
				Debug.Log (GlobalObjs.HamletFunc.isVisible());
			}
			if (GUI.Button (new Rect(25, 350, 100, 30), "Look at")) {
				Debug.Log ("Look at Hamlet");
				GlobalObjs.GraveDiggerFunc.doRotate(GlobalObjs.Hamlet.transform.position.x, GlobalObjs.Hamlet.transform.position.z, GlobalObjs.Hamlet);
			}
			
			//bool useBML = GUI.Toggle(new Rect(500, 30, 100, 30), BML, "Use BML File?");
			if (GUI.Button (new Rect(25, 20, 100, 30), "Start Play")) {
				Debug.Log ("Starting Play "+Time.time);	
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
       
	}
	
	

	
	void RunPlay() {
		// check Mode & run based on that setting
		// use indexNumber, where 1=Baseline, 2=BML, etc
		
		Debug.Log ("Run in mode #"+indexNumber);
		starting = false;
		started = true;
		// need to add logic to do different actions based on mode chosen!!
		inputFile = File.OpenText(inputFileName);
		callNextStep ();
	}
	
	public static void callNextStep() {
		
		string curLine = null;// = inputFile.ReadLine ();
		string[] parsedLine = null;
		bool firstiteration = true;
		
		
		while (firstiteration || (curLine != null && parsedLine[0] != "N")) {
			firstiteration = false;
       		curLine = inputFile.ReadLine ();
	        if (curLine != null) {
	           
	//            currentMessageNum++;
	            parsedLine = curLine.Split ('\t');
	            Debug.Log ("CJT LINE="+curLine);
				//Debug.Log ("First item=" +parsedLine[0]);
	            switch (parsedLine [1]) {
	                case "MOVE":
	                    //Debug.Log ("CJT MESSAGE="+parsedLine [1] + " " + parsedLine [2] + " CJT" + currentMessageNum + " " + parsedLine [3]);
	                    //vhmsg.SendVHMsg ("vrExpress", parsedLine [1] + " " + parsedLine [2] + " CJT" + currentMessageNum + " " + parsedLine [3]);
	                //Debug.Log ("Doing movement for "+parsedLine[2]+" doing:"+parsedLine[4]);	
					parseMovement(parsedLine[2], parsedLine[4]);    
					
					break;
	                case "SPEAK":
	                    //if (parsedLine [1] == actor) {
	                        // find the speech tags & display only that text, start listening for enter key or mouse click?   
	                    //    showtext = findSpeech (parsedLine [3]);
	                    //} else {
	                        // else send the message to be spoken by the character
	                    //Debug.Log ("CJT MESSAGE="+parsedLine [1] + " " + parsedLine [2] + " CJT" + currentMessageNum + " " + parsedLine [3]);
						
						CharFuncs who = GlobalObjs.getCharFunc(parsedLine[2]);
						string saywhat = findSpeech(parsedLine[4]);
						//Debug.Log (parsedLine[2]+" says: "+saywhat);
						who.doSpeak (saywhat);
	                    //vhmsg.SendVHMsg ("vrSpeak", parsedLine [1] + " " + parsedLine [2] + " CJT" + currentMessageNum + " " + parsedLine [3]);
	                    //}
	                    break;
	                default:
	                    // bad command, ignore
					Debug.Log ("Bad command");
	                    break;
	            }
	            //curLine = null;
	            //parsedLine = null;
	        } else {
	            // exit - nothing left to do
	            Debug.Log ("CJT MESSAGE=DONE!!");
	            inputFile.Close ();
	            started = false;
	            inputFile = null;
	            //currentMessageNum = 0;
	           // Application.Quit ();
	        }

		} //while (curLine != null && parsedLine[0] != "N");
		
	}
	
	
	static string findSpeech(string xml) {
        string myText = null;
        int startPos = 0;
        int endPos = 0;
        startPos = xml.IndexOf("application/ssml+xml"+quote+">");
        endPos = xml.IndexOf("</speech>");
        myText = xml.Substring(startPos+22,endPos-startPos-22);
        return myText;
    }
	
	static void parseMovement(string name, string xmltxt) {
		CharFuncs who = GlobalObjs.getCharFunc(name);
		Debug.Log (who.thisChar.name);
		//string action;
		float targetx = -1;
		float targety = -1;
		float targetx2 = -1;
		float targety2 = -1;
		GameObject target = null;
		bool following = false;
		
		string myText = null;
		int startPos = 0;
		int endPos = 0;
		string targetstr;
		
		if (xmltxt.Contains ("follow=")) {
			startPos = xmltxt.IndexOf ("follow="+quote);
			following = true;
		} else {
			startPos = xmltxt.IndexOf ("target="+quote);
		}
		//Debug.Log ("start="+startPos);
		//Debug.Log (xmltxt.Substring (startPos));
		endPos = xmltxt.Substring (startPos+8).IndexOf (quote);
		//Debug.Log (xmltxt.Substring(startPos)[7]);
		//Debug.Log (xmltxt.Substring(startPos)[7] == quote);
		//Debug.Log ("end="+endPos);
		targetstr = xmltxt.Substring(startPos+8, endPos);
		
		
		Debug.Log("Parsed target="+targetstr);
		if (targetstr.IndexOf(" ") > 0) {
			// this is a vector position, not an object
			string[] position = targetstr.Split (' ');
			bool success = float.TryParse(position[0], out targetx);
			success = float.TryParse (position[1], out targety);
			if (position.Length > 2) {
				success = float.TryParse(position[2], out targetx2);
				success = float.TryParse(position[3], out targety2);
			}
			
		} else {
			// this is an object
			target = GlobalObjs.getObject(targetstr);
			
		}
		
		// find out what action to take
		if (xmltxt.Contains ("pick-up")) {
			Debug.Log ("Action=pickup");
			who.doPickup(target);	
		} else if (xmltxt.Contains("put-down")) {
			Debug.Log ("Action=putdown");
			who.doPutDown();
		} else if (xmltxt.Contains ("locomotion")) {
			Debug.Log ("Action=move");
			if (target != null) {
				who.doWalk (target.transform.position.x, target.transform.position.z, target, following);
			} else {
				who.doWalk (targetx, targety, null, following);
				if (targetx2 != -1) {
					who.doWalk (targetx2, targety2, null, following); // this one should get queued
				}
			}
		} else if (xmltxt.Contains ("gaze")) {
			Debug.Log ("Action=turn");
			if (target != null) {
				who.doRotate(target.transform.position.x, target.transform.position.z, target);
			} else {
				who.doRotate(targetx, targety, null);
			}
		} else if (xmltxt.Contains ("POINT")) {
			Debug.Log ("Action=point");
			if (target != null) {
				who.doPoint(target);
			} else {
				Debug.Log ("Error no target");
			}
		} else {
			Debug.Log ("Error - unknown command");
		}
		
		
	}
}
