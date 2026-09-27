using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Animations;
using System.Linq;

public static class PigletBuild
{
    [MenuItem("PIGLET/Build Opening Scene")]
    public static void Build()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Materials")) AssetDatabase.CreateFolder("Assets", "Materials");
        if (!AssetDatabase.IsValidFolder("Assets/Animations")) AssetDatabase.CreateFolder("Assets", "Animations");
        ConfigureAnimations();
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var map = Model("Map", "Woodland clearing", Vector3.zero, 30f);
        var terrainCollider = map.GetComponentInChildren<MeshFilter>().gameObject.AddComponent<MeshCollider>();
        terrainCollider.sharedMesh = map.GetComponentInChildren<MeshFilter>().sharedMesh;
        var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground beneath clearing";
        ground.transform.position = new Vector3(0, -0.2f, 0);
        ground.transform.localScale = Vector3.one * 100f;
        ground.GetComponent<Renderer>().sharedMaterial = Solid("Ground", new Color(.24f,.34f,.18f));

        var craft = Model("Craft", "SS Acorn — crashed", new Vector3(-6.6f,0,-1.8f), 4.5f);
        craft.transform.rotation = Quaternion.Euler(8,-28,31); Feet(craft,SurfaceY(terrainCollider,-6.6f,-1.8f)+.13f);
        var pig = Model("Pig", "Piglet — playable hero", new Vector3(-4.2f,0,-1.6f), 2.15f); Feet(pig,SurfaceY(terrainCollider,-4.2f,-1.6f)+.12f);
        var bear = Model("Bear", "Bear — playable companion", new Vector3(3.9f,0,-.7f), 2.35f); Feet(bear,SurfaceY(terrainCollider,3.9f,-.7f)+.12f);
        var blue = Model("Blue", "Blue rhinoceros — enemy", new Vector3(-5.5f,0,6.8f), 1.8f); Feet(blue,SurfaceY(terrainCollider,-5.5f,6.8f)+.12f);
        var red = Model("RedAnimated", "Red bull — enemy", new Vector3(.3f,0,7.9f), 2.65f); Feet(red,SurfaceY(terrainCollider,.3f,7.9f)+.12f);
        var purple = Model("PurpleAnimated", "Purple bull — enemy", new Vector3(5.5f,0,6.8f), 2.65f); Feet(purple,SurfaceY(terrainCollider,5.5f,6.8f)+.12f);

        Face(pig.transform,bear.transform.position-pig.transform.position);
        Face(bear.transform,pig.transform.position-bear.transform.position);
        Vector3 duo=(pig.transform.position+bear.transform.position)*.5f;
        Face(blue.transform,duo-blue.transform.position);
        Face(red.transform,duo-red.transform.position);
        Face(purple.transform,duo-purple.transform.position);

        var scorch = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        scorch.name = "Scorched earth";
        scorch.transform.position = new Vector3(-6.6f,SurfaceY(terrainCollider,-6.6f,-1.8f)+.02f,-1.8f);
        scorch.transform.localScale = new Vector3(3.35f,.025f,2.35f);
        scorch.GetComponent<Renderer>().sharedMaterial = Solid("Scorch", new Color(.13f,.105f,.085f));
        Object.DestroyImmediate(scorch.GetComponent<Collider>());
        for(int i=0;i<9;i++)
        {
            float a=i*2.399f;
            var d=GameObject.CreatePrimitive(PrimitiveType.Cube);
            d.name="Crash debris " +(i+1);
            float dx=-6.6f+Mathf.Cos(a)*(2.5f+i*.19f), dz=-1.8f+Mathf.Sin(a)*(1.8f+i*.13f);
            d.transform.position=new Vector3(dx,SurfaceY(terrainCollider,dx,dz)+.18f,dz);
            d.transform.localScale=new Vector3(.28f+i%3*.1f,.16f,.32f);
            d.transform.rotation=Quaternion.Euler(18*i,37*i,11*i);
            d.GetComponent<Renderer>().sharedMaterial=Solid("Debris",new Color(.29f,.24f,.2f));
            Object.DestroyImmediate(d.GetComponent<Collider>());
        }
        var smoke=new GameObject("Crash smoke"); smoke.transform.position=new Vector3(-6.6f,SurfaceY(terrainCollider,-6.6f,-1.8f)+1.25f,-1.8f);
        var ps=smoke.AddComponent<ParticleSystem>();
        var main=ps.main; main.startLifetime=3.6f; main.startSpeed=.72f; main.startSize=.55f; main.startColor=new Color(.28f,.27f,.25f,.42f); main.maxParticles=85;
        var emission=ps.emission; emission.rateOverTime=12;
        var shape=ps.shape; shape.shapeType=ParticleSystemShapeType.Cone; shape.angle=12; shape.radius=.7f;
        ps.GetComponent<ParticleSystemRenderer>().sharedMaterial=Solid("Smoke",new Color(.3f,.3f,.3f,.55f),"Particles/Standard Unlit");

        var sunObj=new GameObject("Warm afternoon light");
        var sun=sunObj.AddComponent<Light>(); sun.type=LightType.Directional; sun.intensity=1.05f; sun.color=new Color(1f,.88f,.69f);
        sunObj.transform.rotation=Quaternion.Euler(48,-35,0);
        RenderSettings.ambientMode=UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight=new Color(.48f,.53f,.58f);
        RenderSettings.fog=true; RenderSettings.fogColor=new Color(.66f,.76f,.72f); RenderSettings.fogMode=FogMode.Linear; RenderSettings.fogStartDistance=21; RenderSettings.fogEndDistance=52;
        var camObj=new GameObject("Opening camera"); camObj.tag="MainCamera";
        var cam=camObj.AddComponent<Camera>(); cam.clearFlags=CameraClearFlags.SolidColor; cam.backgroundColor=new Color(.65f,.77f,.76f); cam.fieldOfView=51;
        camObj.AddComponent<AudioListener>(); camObj.transform.position=new Vector3(0,20,-23); camObj.transform.LookAt(new Vector3(0,3.3f,.5f));
        var director=new GameObject("Opening sequence director").AddComponent<PigletOpening>();
        director.terrain=terrainCollider; director.pig=pig.transform; director.bear=bear.transform; director.blue=blue.transform; director.red=red.transform; director.purple=purple.transform; director.cameraTransform=camObj.transform;
        EditorSceneManager.SaveScene(scene,"Assets/Scenes/TheCrash.unity");
        EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene("Assets/Scenes/TheCrash.unity",true)};
        AssetDatabase.SaveAssets();
        Debug.Log("PIGLET_SCENE_BUILT Assets/Scenes/TheCrash.unity");
    }
    static float SurfaceY(Collider terrain,float x,float z)
    {
        var ray=new Ray(new Vector3(x,20,z),Vector3.down);
        return terrain.Raycast(ray,out RaycastHit hit,40)?hit.point.y:0f;
    }
    static void Face(Transform actor,Vector3 toward)
    {
        toward.y=0;
        if(toward.sqrMagnitude>.001f)actor.rotation=Quaternion.LookRotation(toward);
    }
    static void ConfigureAnimations()
    {
        foreach(var name in new[]{"Pig","Bear","Blue","RedAnimated","PurpleAnimated"})
        {
            var path="Assets/Models/"+name+".fbx";
            var importer=AssetImporter.GetAtPath(path) as ModelImporter;
            if(importer==null)throw new System.Exception("Missing animated model "+name);
            importer.animationType=ModelImporterAnimationType.Generic;
            importer.avatarSetup=ModelImporterAvatarSetup.NoAvatar;
            importer.importAnimation=true;
            var clips=importer.defaultClipAnimations;
            foreach(var c in clips)c.loopTime=true;
            importer.clipAnimations=clips;
            importer.SaveAndReimport();
        }
    }
    static RuntimeAnimatorController Controller(string name)
    {
        string path="Assets/Animations/"+name+".controller";
        AssetDatabase.DeleteAsset(path);
        var controller=AnimatorController.CreateAnimatorControllerAtPath(path);
        string[] clips=name=="Pig" ? new[]{"","agree","angry",""}:
            name=="Bear" ? new[]{"","","angry",""}:
            name=="Blue" ? new[]{"wait","turn","angry","swagger"}:
            name=="RedAnimated" ? new[]{"","greet","angry_01","angry_02"}:
            new[]{"idle","look_around","angry","angry"};
        string[] states={"Idle","Meet","Threat","Chase"};
        var machine=controller.layers[0].stateMachine;
        var all=AssetDatabase.LoadAllAssetsAtPath("Assets/Models/"+name+".fbx").OfType<AnimationClip>()
            .Where(c=>!c.name.StartsWith("__preview__")).ToArray();
        for(int i=0;i<states.Length;i++)
        {
            var state=machine.AddState(states[i]);
            if(clips[i]!="")state.motion=all.FirstOrDefault(c=>c.name.StartsWith(clips[i]));
            if(i==0)machine.defaultState=state;
        }
        return controller;
    }
    static GameObject Model(string file,string label,Vector3 pos,float scale)
    {
        var prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/"+file+".fbx");
        if(!prefab)throw new System.Exception("Missing model "+file);
        var go=(GameObject)PrefabUtility.InstantiatePrefab(prefab);
        go.name=label; go.transform.position=pos; go.transform.localScale=Vector3.one*scale;
        var mat=TextureMat(file,AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Models/"+file+".jpg"));
        foreach(var r in go.GetComponentsInChildren<Renderer>())
        {
            var mats=r.sharedMaterials; for(int i=0;i<mats.Length;i++)mats[i]=mat; r.sharedMaterials=mats;
        }
        if(file!="Map" && file!="Craft")
        {
            var animator=go.GetComponent<Animator>();
            if(animator==null)animator=go.AddComponent<Animator>();
            animator.runtimeAnimatorController=Controller(file);
            animator.applyRootMotion=false;
        }
        return go;
    }
    static void Feet(GameObject go,float y)
    {
        var rs=go.GetComponentsInChildren<Renderer>(); var b=rs[0].bounds;
        for(int i=1;i<rs.Length;i++)b.Encapsulate(rs[i].bounds);
        go.transform.position+=Vector3.up*(y-b.min.y);
    }
    static Material TextureMat(string name,Texture2D tex)
    {
        string path="Assets/Materials/"+name+".mat";
        var mat=AssetDatabase.LoadAssetAtPath<Material>(path);
        if(!mat){mat=new Material(Shader.Find("Standard"));AssetDatabase.CreateAsset(mat,path);}
        mat.mainTexture=tex; mat.color=Color.white; EditorUtility.SetDirty(mat); return mat;
    }
    static Material Solid(string name,Color color,string shader="Standard")
    {
        string path="Assets/Materials/"+name+".mat";
        var mat=AssetDatabase.LoadAssetAtPath<Material>(path);
        if(!mat){mat=new Material(Shader.Find(shader));AssetDatabase.CreateAsset(mat,path);}
        mat.color=color; EditorUtility.SetDirty(mat); return mat;
    }
    [MenuItem("PIGLET/Render Preview")]
    public static void Preview()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/TheCrash.unity");
        var cam=Camera.main;
        var rt=new RenderTexture(1280,720,24);
        cam.targetTexture=rt;
        cam.Render();
        RenderTexture.active=rt;
        var tex=new Texture2D(1280,720,TextureFormat.RGB24,false);
        tex.ReadPixels(new Rect(0,0,1280,720),0,0); tex.Apply();
        System.IO.File.WriteAllBytes(System.IO.Path.Combine(Application.dataPath,"../Preview.png"),tex.EncodeToPNG());
        cam.targetTexture=null; RenderTexture.active=null;
        Object.DestroyImmediate(rt); Object.DestroyImmediate(tex);
        Debug.Log("PIGLET_PREVIEW_RENDERED");
    }

    [MenuItem("PIGLET/Inspect Terrain")]
    public static void InspectTerrain()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/TheCrash.unity");
        var map=GameObject.Find("Woodland clearing");
        foreach(var f in map.GetComponentsInChildren<MeshFilter>())
        {
            var c=f.gameObject.AddComponent<MeshCollider>(); c.sharedMesh=f.sharedMesh;
        }
        foreach(var p in new[]{new Vector2(-6.6f,-1.8f),new Vector2(-4.2f,-1.6f),new Vector2(3.9f,-.7f),new Vector2(-5.5f,6.8f),new Vector2(.3f,7.9f),new Vector2(5.5f,6.8f),Vector2.zero})
        {
            var hits=Physics.RaycastAll(new Ray(new Vector3(p.x,20,p.y),Vector3.down),30);
            System.Array.Sort(hits,(a,b)=>a.point.y.CompareTo(b.point.y));
            string ys=""; foreach(var h in hits) ys+=h.point.y.ToString("F2")+"/"+h.normal.y.ToString("F2")+",";
            Debug.Log("TERRAIN "+p+" hits="+ys);
        }
    }

}
