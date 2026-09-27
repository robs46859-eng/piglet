using UnityEngine;

public class PigletOpening : MonoBehaviour
{
    public Transform pig, bear, blue, red, purple, cameraTransform;
    public Collider terrain;
    float[] offsets;
    bool activePig=true;
    Animator[] animators;
    string animationPhase="";
    float time;
    Vector3 pigStart, bearStart;
    Vector3[] enemyStarts;
    GUIStyle captionStyle, smallStyle;
    void Start()
    {
        pigStart=pig.position; bearStart=bear.position;
        enemyStarts=new[]{blue.position,red.position,purple.position};
        offsets=new[]{pig.position.y-GroundY(pig.position),bear.position.y-GroundY(bear.position),blue.position.y-GroundY(blue.position),red.position.y-GroundY(red.position),purple.position.y-GroundY(purple.position)};
        animators=new[]{pig.GetComponent<Animator>(),bear.GetComponent<Animator>(),blue.GetComponent<Animator>(),red.GetComponent<Animator>(),purple.GetComponent<Animator>()};
        Restart();
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))Restart();
        time+=Time.deltaTime;
        SetAnimationPhase(time<1.6f?"Idle":time<6.4f?"Meet":time<10f?"Threat":"Chase");
        if(time>=16.8f){PlayUpdate();return;}
        float meeting=Mathf.SmoothStep(0,1,Mathf.Clamp01((time-1.6f)/3.4f));
        float escape=Mathf.SmoothStep(0,1,Mathf.Clamp01((time-10f)/5.2f));
        pig.position=Grounded(Vector3.Lerp(pigStart,new Vector3(-1.3f,pigStart.y,-.8f),meeting)+new Vector3(-5,0,-5.4f)*escape,offsets[0]);
        bear.position=Grounded(Vector3.Lerp(bearStart,new Vector3(1.1f,bearStart.y,-.8f),meeting)+new Vector3(-5,0,-5.4f)*escape,offsets[1]);
        Face(pig,time>9.8f?new Vector3(-1,0,-1):bear.position-pig.position);
        Face(bear,time>9.8f?new Vector3(-1,0,-1):pig.position-bear.position);
        Transform[] enemies={blue,red,purple};
        for(int i=0;i<3;i++)
        {
            float approach=Mathf.SmoothStep(0,1,Mathf.Clamp01((time-(6.4f+i*.55f))/3.4f));
            float chase=Mathf.SmoothStep(0,1,Mathf.Clamp01((time-10f)/6.5f));
            Vector3 reveal=new Vector3(-3.1f+i*2.9f,enemyStarts[i].y,2f);
            enemies[i].position=Grounded(Vector3.Lerp(enemyStarts[i],reveal,approach)+new Vector3(-4.2f,0,-5.4f)*chase,offsets[i+2]);
            Face(enemies[i],(pig.position+bear.position)*.5f-enemies[i].position);
        }
        float pan=Mathf.SmoothStep(0,1,Mathf.Clamp01((time-9.5f)/7f));
        cameraTransform.position=Vector3.Lerp(cameraTransform.position,new Vector3(-2.3f*pan,20,-23-2.5f*pan),Time.deltaTime*2f);
        cameraTransform.LookAt(new Vector3(-2.3f*pan,3.3f,-1.4f*pan));
    }
    void SetAnimationPhase(string phase)
    {
        if(animationPhase==phase)return;
        animationPhase=phase;
        foreach(var animator in animators)if(animator!=null)animator.Play(phase,0,0f);
    }
    void PlayUpdate()
    {
        if(Input.GetKeyDown(KeyCode.Tab))activePig=!activePig;
        Transform leader=activePig?pig:bear, companion=activePig?bear:pig;
        float leaderOffset=activePig?offsets[0]:offsets[1];
        float companionOffset=activePig?offsets[1]:offsets[0];
        Vector3 input=Vector3.zero;
        if(Input.GetKey(KeyCode.W)||Input.GetKey(KeyCode.UpArrow))input.z+=1;
        if(Input.GetKey(KeyCode.S)||Input.GetKey(KeyCode.DownArrow))input.z-=1;
        if(Input.GetKey(KeyCode.A)||Input.GetKey(KeyCode.LeftArrow))input.x-=1;
        if(Input.GetKey(KeyCode.D)||Input.GetKey(KeyCode.RightArrow))input.x+=1;
        input=input.normalized;
        Vector3 next=leader.position+input*4.2f*Time.deltaTime;
        next.x=Mathf.Clamp(next.x,-12,12); next.z=Mathf.Clamp(next.z,-11,11);
        leader.position=Grounded(next,leaderOffset);
        if(input.sqrMagnitude>.01f)Face(leader,input);
        Vector3 gap=leader.position-companion.position; gap.y=0;
        if(gap.magnitude>2.1f)
        {
            Vector3 step=companion.position+gap.normalized*3.8f*Time.deltaTime;
            companion.position=Grounded(step,companionOffset);
            Face(companion,gap);
        }
        Transform[] enemies={blue,red,purple};
        for(int i=0;i<3;i++)
        {
            Vector3 pursuit=leader.position-enemies[i].position; pursuit.y=0;
            if(pursuit.magnitude>1.7f)
            {
                Vector3 step=enemies[i].position+pursuit.normalized*(1.8f+i*.12f)*Time.deltaTime;
                enemies[i].position=Grounded(step,offsets[i+2]);
                Face(enemies[i],pursuit);
            }
        }
        Vector3 cameraGoal=leader.position+new Vector3(0,17,-18);
        cameraTransform.position=Vector3.Lerp(cameraTransform.position,cameraGoal,Time.deltaTime*2f);
        cameraTransform.LookAt(leader.position+Vector3.up*2f);
    }
    float GroundY(Vector3 p)
    {
        return terrain.Raycast(new Ray(new Vector3(p.x,20,p.z),Vector3.down),out RaycastHit hit,40)?hit.point.y:0f;
    }
    Vector3 Grounded(Vector3 p,float offset)
    {
        p.y=GroundY(p)+offset; return p;
    }
    void Restart()
    {
        time=0; animationPhase=""; activePig=true; pig.position=pigStart; bear.position=bearStart;
        if(enemyStarts!=null){blue.position=enemyStarts[0];red.position=enemyStarts[1];purple.position=enemyStarts[2];}
    }
    static void Face(Transform actor,Vector3 delta)
    {
        delta.y=0;
        if(delta.sqrMagnitude>.01f)actor.rotation=Quaternion.Slerp(actor.rotation,Quaternion.LookRotation(delta),Time.deltaTime*4f);
    }
    void OnGUI()
    {
        if(captionStyle==null)
        {
            captionStyle=new GUIStyle(GUI.skin.label){fontSize=24,fontStyle=FontStyle.Bold,alignment=TextAnchor.MiddleCenter,wordWrap=true};
            captionStyle.normal.textColor=Color.white;
            smallStyle=new GUIStyle(GUI.skin.label){fontSize=15,alignment=TextAnchor.MiddleCenter}; smallStyle.normal.textColor=Color.white;
        }
        string title=time<1.8f?"PIGLET  •  THE CRASH":time<5.4f?"A stranger in the clearing...":time<7.2f?"Piglet meets Bear":time<10f?"Three shadows emerge from the trees":time<16.8f?"RUN!":(activePig?"ESCAPE THE CLEARING  •  PIGLET":"ESCAPE THE CLEARING  •  BEAR");
        GUI.Box(new Rect(Screen.width*.19f,18,Screen.width*.62f,76),GUIContent.none);
        GUI.Label(new Rect(Screen.width*.2f,25,Screen.width*.6f,46),title,captionStyle);
        GUI.Label(new Rect(Screen.width*.25f,Screen.height-43,Screen.width*.5f,28),time<16.8f?"R  •  replay scene":"WASD / arrows  •  move     Tab  •  switch hero     R  •  replay",smallStyle);
    }
}
