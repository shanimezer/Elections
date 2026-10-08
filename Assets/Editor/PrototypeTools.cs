using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using VoteQuest;
public static class PrototypeTools {
 [InitializeOnLoadMethod] static void EnsureScene(){EditorApplication.delayCall+=()=>{if(!File.Exists("Assets/Scenes/City.unity"))CreateScene();};}
 [MenuItem("Vote Quest/Create City Scene")] public static void CreateScene(){var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);new GameObject("Vote Quest").AddComponent<CityGame>();EditorSceneManager.SaveScene(scene,"Assets/Scenes/City.unity");EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene("Assets/Scenes/City.unity",true)};AssetDatabase.SaveAssets();}
 [MenuItem("Vote Quest/Run Simulation Checks")] public static void Validate(){
  var s=new Simulation();Check(s.Minutes==480,"opens at 08:00");Check(s.Spend(8)&&s.Energy==52,"energy spending");Check(!s.Spend(100),"cannot overspend");s.Paused=true;s.Tick(10);Check(s.Elapsed==0,"pause clock");Check(!s.Arrive(0),"pause arrivals");s.Paused=false;s.MockActivity();Check(s.Energy==72,"mock activity");s.Tick(10);Check(s.Energy<=100,"energy cap");Check(!s.Arrive(-1)&&!s.Arrive(3),"team validation");for(int i=0;i<108;i++)Check(s.Arrive(i%3),"arrivals");Check(!s.Arrive(0)&&s.Total==108,"population cap");s.Tick(1000);Check(s.Closed&&s.Minutes==1320,"22:00 close");Check(!s.Spend(1)&&!s.Arrive(1),"closed actions");
  CreateScene();Debug.Log("VOTEQUEST_CHECKS_PASSED");File.WriteAllText("verification.txt","Unity compilation and simulation checks passed. City scene generated.\n");
 }
 static void Check(bool ok,string name){if(!ok)throw new Exception("Failed: "+name);}
 [MenuItem("Vote Quest/Build macOS Player")] public static void BuildMac(){CreateScene();var r=BuildPipeline.BuildPlayer(EditorBuildSettings.scenes,"Builds/VoteQuest.app",BuildTarget.StandaloneOSX,BuildOptions.None);if(r.summary.result!=UnityEditor.Build.Reporting.BuildResult.Succeeded)throw new Exception("Build failed");}
}
