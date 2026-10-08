using System;
namespace VoteQuest {
 public sealed class Simulation {
  public const int Population=108, Goal=76;
  public readonly int[] Votes=new int[3];
  public float Elapsed {get; private set;}
  public float Energy {get; private set;}=60;
  public bool Paused;
  public bool Closed => Elapsed>=240;
  public int Total => Votes[0]+Votes[1]+Votes[2];
  public int Minutes => 480+(int)(Math.Min(Elapsed/240,1)*840);
  public void Tick(float dt) { if(Paused||Closed||dt<=0)return; Elapsed=Math.Min(240,Elapsed+dt); Energy=Math.Min(100,Energy+dt*1.8f); }
  public bool Spend(float cost) {if(Closed||Paused||Energy<cost)return false; Energy-=cost;return true;}
  public void MockActivity() {if(!Closed&&!Paused)Energy=Math.Min(100,Energy+20);}
  public bool Arrive(int team) {if(Closed||Paused||team<0||team>2||Total>=Population)return false; Votes[team]++;return true;}
 }
}
