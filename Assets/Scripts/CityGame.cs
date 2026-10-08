using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace VoteQuest {
 public sealed class CityGame:MonoBehaviour {
  sealed class House {public Vector3 Pos;public int Team,Remaining=6;public bool Known;public GameObject Fog;public Renderer Body;}
  sealed class Walker {public Transform Model;public int Team;public Vector3[] Route;public int Step;}
  readonly Color[] colors={new Color(.95f,.29f,.33f),new Color(.22f,.62f,.96f),new Color(1,.76f,.2f)};
  readonly string[] names={"Coral","Azure","Amber"};
  readonly List<House> houses=new List<House>(); readonly List<Walker> walkers=new List<Walker>();
  readonly Vector3[] polls={new Vector3(-12,0,0),new Vector3(0,0,0),new Vector3(12,0,0)};
  public Simulation Sim=new Simulation();
  Camera cam; House selected;float botTimer,activityCooldown;int team;bool started,activity;string notice="Choose a fictional team to begin."; GUIStyle title,body,small;Vector3 focus=Vector3.zero;
  Material Mat(Color c){var m=new Material(Shader.Find("Standard"));m.color=c;return m;}
  GameObject Shape(string name,PrimitiveType kind,Vector3 pos,Vector3 scale,Color c){var g=GameObject.CreatePrimitive(kind);g.name=name;g.transform.position=pos;g.transform.localScale=scale;g.GetComponent<Renderer>().sharedMaterial=Mat(c);return g;}
  void Label(string s,Vector3 pos,float size=1){var g=new GameObject(s);g.transform.position=pos;g.transform.rotation=Quaternion.Euler(55,0,0);var t=g.AddComponent<TextMesh>();t.text=s;t.fontSize=48;t.characterSize=.065f*size;t.anchor=TextAnchor.MiddleCenter;t.color=Color.white;}
  void Start(){
   Random.InitState(17); RenderSettings.ambientLight=new Color(.66f,.72f,.8f);RenderSettings.fog=false;
   var l=new GameObject("Sun").AddComponent<Light>();l.type=LightType.Directional;l.intensity=1.15f;l.transform.rotation=Quaternion.Euler(50,-30,0);l.shadows=LightShadows.Soft;
   cam=new GameObject("Isometric camera").AddComponent<Camera>();cam.orthographic=true;cam.orthographicSize=28;cam.backgroundColor=new Color(.10f,.16f,.23f);cam.farClipPlane=200;cam.transform.rotation=Quaternion.Euler(55,0,0);PositionCamera();
   Shape("City island",PrimitiveType.Cube,new Vector3(0,-.6f,0),new Vector3(45,1,37),new Color(.31f,.54f,.44f));
   for(int x=-12;x<=12;x+=12)Shape("Avenue",PrimitiveType.Cube,new Vector3(x,-.04f,0),new Vector3(3,.1f,35),new Color(.23f,.29f,.34f));
   for(int z=-10;z<=10;z+=10)Shape("Street",PrimitiveType.Cube,new Vector3(0,-.03f,z),new Vector3(43,.1f,2.5f),new Color(.23f,.29f,.34f));
   for(int i=0;i<3;i++){Shape("Polling hall",PrimitiveType.Cube,polls[i]+Vector3.up*.8f,new Vector3(2.7f,1.6f,2.3f),new Color(.91f,.9f,.81f));Shape("Polling roof",PrimitiveType.Cube,polls[i]+Vector3.up*1.7f,new Vector3(3.2f,.25f,2.8f),new Color(.2f,.34f,.38f));Label("POLL "+(i+1),polls[i]+new Vector3(0,2.5f,0));}
   int n=0;for(int row=0;row<3;row++)for(int col=0;col<6;col++){
    var p=new Vector3(-18+col*7.2f,0,-14+row*13);var h=new House{Pos=p,Team=n++%3};
    h.Body=Shape("House "+n,PrimitiveType.Cube,p+Vector3.up*.9f,new Vector3(3.8f,1.8f,3),colors[h.Team]).GetComponent<Renderer>();
    var roof=Shape("Roof",PrimitiveType.Cylinder,p+Vector3.up*2,new Vector3(3.1f,.65f,2.6f),colors[h.Team]*.7f);roof.transform.rotation=Quaternion.Euler(0,45,0);
    Shape("Door",PrimitiveType.Cube,p+new Vector3(0,.55f,-1.53f),new Vector3(.6f,1.1f,.1f),new Color(.15f,.21f,.27f));
    h.Fog=Shape("Unexplored block",PrimitiveType.Cube,p+Vector3.up*2.5f,new Vector3(5.5f,5.8f,5.5f),new Color(.37f,.44f,.53f));Label("?",p+new Vector3(0,5.5f,0));houses.Add(h);
   }
   foreach(var h in houses)if(h.Pos.z< -10&&Mathf.Abs(h.Pos.x)<11)Reveal(h);
   for(int i=0;i<18;i++){float x=i%2==0?-21:21;Shape("Tree trunk",PrimitiveType.Cylinder,new Vector3(x,.6f,-15+i*1.7f),new Vector3(.25f,.6f,.25f),new Color(.4f,.27f,.18f));Shape("Tree crown",PrimitiveType.Sphere,new Vector3(x,1.8f,-15+i*1.7f),new Vector3(1.5f,2,1.5f),new Color(.18f,.4f,.29f));}
  }
  void PositionCamera(){cam.transform.position=focus+new Vector3(0,38,-27);}
  void Reveal(House h){h.Known=true;h.Fog.SetActive(false);// Remove this block's question marker.
   foreach(var t in FindObjectsOfType<TextMesh>())if(t.text=="?"&&Vector3.Distance(t.transform.position,h.Pos+new Vector3(0,5.5f,0))<.1f) t.gameObject.SetActive(false);
  }
  House NearestUnknown(){House best=null;float d=float.MaxValue;foreach(var h in houses)if(!h.Known){float v=Vector3.Distance(selected==null?Vector3.zero:selected.Pos,h.Pos);if(v<d){d=v;best=h;}}return best;}
  void Explore(){var h=NearestUnknown();if(h!=null&&Sim.Spend(10)){Reveal(h);selected=h;notice="Block discovered. Select a house and invite its residents.";}}
  void Dispatch(House h){if(h.Remaining==0)return;h.Remaining--;int poll=Mathf.Clamp(Mathf.RoundToInt((h.Pos.x+12)/12),0,2);float street=-10+Mathf.Clamp(Mathf.RoundToInt((h.Pos.z+10)/10),0,2)*10;
   var g=Shape("Resident",PrimitiveType.Capsule,h.Pos+new Vector3(0,.65f,-2),new Vector3(.55f,.65f,.55f),colors[h.Team]);
   walkers.Add(new Walker{Model=g.transform,Team=h.Team,Route=new[]{new Vector3(h.Pos.x,.65f,street),new Vector3(polls[poll].x,.65f,street),polls[poll]+new Vector3(0,.65f,-1.5f)}});
  }
  public void Advance(float dt){if(!started||Sim.Paused||Sim.Closed)return;Sim.Tick(dt);activityCooldown=Mathf.Max(0,activityCooldown-dt);botTimer+=dt;
   if(botTimer>=3){botTimer=0;for(int t=0;t<3;t++)if(t!=team){var options=houses.FindAll(h=>h.Team==t&&h.Remaining>0);if(options.Count>0)Dispatch(options[Random.Range(0,options.Count)]);}}
   for(int i=walkers.Count-1;i>=0;i--){var w=walkers[i];w.Model.position=Vector3.MoveTowards(w.Model.position,w.Route[w.Step],dt*3.5f);if(Vector3.Distance(w.Model.position,w.Route[w.Step])<.05f){w.Step++;if(w.Step==w.Route.Length){Sim.Arrive(w.Team);Destroy(w.Model.gameObject);walkers.RemoveAt(i);}}}
  }
  void Update(){Advance(Time.deltaTime);if(!started)return;if(Input.GetKeyDown(KeyCode.Space)&&!Sim.Closed)Sim.Paused=!Sim.Paused;
   float speed=Time.unscaledDeltaTime*18;focus+=new Vector3(((Input.GetKey(KeyCode.D)||Input.GetKey(KeyCode.RightArrow)?1:0)-(Input.GetKey(KeyCode.A)||Input.GetKey(KeyCode.LeftArrow)?1:0)),0,((Input.GetKey(KeyCode.W)||Input.GetKey(KeyCode.UpArrow)?1:0)-(Input.GetKey(KeyCode.S)||Input.GetKey(KeyCode.DownArrow)?1:0)))*speed;focus.x=Mathf.Clamp(focus.x,-15,15);focus.z=Mathf.Clamp(focus.z,-10,10);cam.orthographicSize=Mathf.Clamp(cam.orthographicSize-Input.mouseScrollDelta.y*2,17,34);PositionCamera();
   if(Input.GetMouseButtonDown(0)&&Input.mousePosition.x>305*Mathf.Min(Screen.width/1100f,Screen.height/720f)&&Input.mousePosition.y<Screen.height-110*Mathf.Min(Screen.width/1100f,Screen.height/720f)){RaycastHit hit;if(Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition),out hit)){foreach(var h in houses)if(Vector3.Distance(hit.point,h.Pos)<5&&h.Known){selected=h;break;}}}
  }
  void Styles(){if(title!=null)return;title=new GUIStyle(GUI.skin.label){fontSize=25,fontStyle=FontStyle.Bold};title.normal.textColor=Color.white;body=new GUIStyle(GUI.skin.label){fontSize=17,wordWrap=true};body.normal.textColor=new Color(.88f,.92f,.96f);small=new GUIStyle(body){fontSize=13};GUI.skin.button.fontSize=16;}
  void OnGUI(){Styles();float scale=Mathf.Min(Screen.width/1100f,Screen.height/720f);GUI.matrix=Matrix4x4.Scale(new Vector3(scale,scale,1));float width=Screen.width/scale,height=Screen.height/scale;
   GUI.Box(new Rect(12,12,width-24,88),GUIContent.none);GUI.Label(new Rect(30,22,430,32),"VOTE QUEST / a city in motion",title);
   int m=Sim.Minutes;GUI.Label(new Rect(width-300,25,280,30),$"{m/60:00}:{m%60:00}  /  closes 22:00",body);
   GUI.Label(new Rect(30,59,width-60,30),$"Shared virtual turnout  {Sim.Total} / {Simulation.Population}     •     Community goal  {Simulation.Goal}     •     Energy  {Sim.Energy:0}/100",body);
   GUI.Box(new Rect(12,112,280,height-124),GUIContent.none);GUILayout.BeginArea(new Rect(28,128,248,height-155));
   GUILayout.Label("THE CITY TEAMS",title);for(int i=0;i<3;i++){GUI.contentColor=colors[i];GUILayout.Label($"{names[i]}     {Sim.Votes[i]} virtual votes",body);}GUI.contentColor=Color.white;GUILayout.Space(18);
   if(!started){GUILayout.Label("Choose your team. Invite residents of any color; their fictional affiliation stays fixed.",body);for(int i=0;i<3;i++)if(GUILayout.Button("Play as "+names[i],GUILayout.Height(36))){team=i;started=true;notice="Explore blocks, then click a colored house.";}}
   else {
    GUILayout.Label("You lead "+names[team],body);GUI.enabled=!Sim.Closed&&!Sim.Paused;
    if(GUILayout.Button("Explore a block • 10 energy",GUILayout.Height(38)))Explore();
    GUILayout.Space(12);GUILayout.Label(selected==null?"Select a revealed house.":$"{names[selected.Team]} house\n{selected.Remaining} residents at home",body);
    GUI.enabled=!Sim.Closed&&!Sim.Paused&&selected!=null&&selected.Remaining>0&&Sim.Energy>=8;
    if(GUILayout.Button("Invite a resident • 8 energy",GUILayout.Height(38))&&Sim.Spend(8)){Dispatch(selected);notice="Resident is walking to a virtual poll.";}
    GUI.enabled=true;GUILayout.Space(14);activity=GUILayout.Toggle(activity," Optional activity mock");
    if(activity){GUILayout.Label("Manual simulation only. No location or real voting data.",small);GUI.enabled=!Sim.Closed&&!Sim.Paused&&activityCooldown<=0;if(GUILayout.Button("Simulate 100 steps • +20 energy",GUILayout.Height(42))){Sim.MockActivity();activityCooldown=15;notice="Mock activity added energy. You can also wait for energy to refill.";}GUI.enabled=true;}
    GUILayout.Space(12);if(GUILayout.Button(Sim.Paused?"Resume":"Pause",GUILayout.Height(30))&&!Sim.Closed)Sim.Paused=!Sim.Paused;
    if(GUILayout.Button("Restart city",GUILayout.Height(30)))SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
   }
   GUILayout.Space(14);GUILayout.Label(notice,small);GUILayout.FlexibleSpace();GUILayout.Label("WASD / arrows: pan\nScroll: zoom • Space: pause\n4 minutes = one fictional election day",small);GUILayout.EndArea();
   if(Sim.Closed){GUI.Box(new Rect(width/2-230,height/2-110,460,220),GUIContent.none);GUI.Label(new Rect(width/2-210,height/2-90,420,45),Sim.Total>=Simulation.Goal?"COMMUNITY GOAL REACHED":"THE VIRTUAL POLLS ARE CLOSED",title);GUI.Label(new Rect(width/2-210,height/2-35,420,100),$"{Sim.Total} residents participated ({100f*Sim.Total/Simulation.Population:0}%).\nCoral {Sim.Votes[0]}  •  Azure {Sim.Votes[1]}  •  Amber {Sim.Votes[2]}\nTry another strategy with Restart city.",body);}
   GUI.Label(new Rect(315,height-36,width-330,30),"Fictional simulation • Local shared goal • No real vote rewards or polling check-ins",small);
  }
 }
}
