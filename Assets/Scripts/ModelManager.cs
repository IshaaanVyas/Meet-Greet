/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

using UnityEngine;
using NRKernal;

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Utilities;

public class ModelManager : MonoBehaviour
{
    /// <summary> A model to place when a raycast from a user touch hits a plane. </summary>
    public GameObject SlatePrefab;

    public GameObject SlateSquarePrefab;

    public GameObject SlateTitlePrefab;

    /// <summary>
    ///  A model for chevron marker guide. 
    /// </summary>
    public GameObject FollowPairPrefab;

    public GameObject Bounded3DObjPrefab;

    public ObjectManager m_ObjectManager;

    private bool HasLoadedModels = false;


    // 1 = duck, 2 = avo, 0 == no place. 
    // private int modelType = 0;

    private uint SerialUID = 0;

    private static ModelManager _instance;

    public static ModelManager Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        // Uncomment this to persist across scenes. 
        // DontDestroyOnLoad(this.gameObject);
    }
    
    private async void Start()
    {
        //// var url = "https://raw.githubusercontent.com/KhronosGroup/glTF-Sample-Models/master/2.0/Duck/glTF/Duck.gltf";
        //// var url = "https://raw.githubusercontent.com/KhronosGroup/glTF-Sample-Models/master/2.0/Avocado/glTF/Avocado.gltf";

        //// Create a settings object and configure it accordingly
        //var settings = new ImportSettings
        //{
        //    // generateMipMaps = true,
        //    // anisotropicFilterLevel = 3,
        //    // nodeNameMethod = ImportSettings.NameImportMethod.OriginalUnique
        //};

        //gltfDuck = new GLTFast.GltfImport();
        //var url = "https://raw.githubusercontent.com/KhronosGroup/glTF-Sample-Models/master/2.0/Duck/glTF/Duck.gltf";
        //var success = await gltfDuck.Load(url, settings);
        //if (success)
        //{
        //    gltfAvocado = new GLTFast.GltfImport();
        //    url = "https://raw.githubusercontent.com/KhronosGroup/glTF-Sample-Models/master/2.0/Avocado/glTF/Avocado.gltf";
        //    success = await gltfAvocado.Load(url, settings);
        //    HasLoadedModels = true;
        //}

    }

    //public void OnClickModelTypeDuck()
    //{
    //    // modelType = 1;
    //    ObjectManager.Instance.SetModelType(1);
    //}

    //public void OnClickModelTypeAvo()
    //{
    //    ObjectManager.Instance.SetModelType(2);
    //    // modelType = 2;
    //}

    //public void OnClickModelTypeNone()
    //{
    //    ObjectManager.Instance.SetModelType(0);
    //    // modelType = 0;
    //}

    //public void OnClickOpRotateMode()
    //{
    //    ObjectManager.Instance.SetOpMode(1);
    //}

    //public void OnClickOpDeleteMode()
    //{
    //    ObjectManager.Instance.SetOpMode(2);
    //}

    //public void OnClickDeleteModel()
    //{
    //    // FInd all models and delete them. 
    //    var additions = GameObject.FindGameObjectsWithTag("addedobj");
    //    foreach (GameObject addition in additions)
    //    {
    //        Destroy(addition);
    //    }
    //    // Delete all from handler; 
    //    ObjectManager.Instance.DeleteAllObjects();
    //}

    private GameObject addGameObjectWithUID(string uuid, Vector3 pos, Vector3 forward, int type, string msg)
    {
        //var gameobj = new GameObject("glTFmodel" + uid);
        //gameobj.tag = "addedobj";

        //var tf = gameobj.transform;
        //tf.Translate(pos); // new Vector3(1.0f, 0.0f, 0.0f));

        // Debug.Log(transform.position);
        // behaviour.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);

        // tf.Translate(hitResult.point);
        // tf.localScale = new Vector3(0.25f, 0.25f, 0.25f);

        //// Instantiate object. 
        //var modelType = ObjectManager.Instance.GetModelType();
        //if (modelType == 1)
        //{
        //    //var objInstantiator = new CustomGameObjectInstantiator(TestPrefab, tf);
        //    //objInstantiator.assignUID(uid);            
        //    //tf.localScale = new Vector3(0.25f, 0.25f, 0.25f);
        //    //gltfDuck.InstantiateMainScene(objInstantiator);

        //}
        //else if (modelType == 2)
        //{
        //    //var objInstantiator = new CustomGameObjectInstantiator(gltfAvocado, tf);
        //    //objInstantiator.assignUID(uid);            
        //    //tf.localScale = new Vector3(5, 5, 5);
        //    //gltfAvocado.InstantiateMainScene(objInstantiator);
        //}
        //ObjectManager.Instance.AddInteractiveObject(uid, gameobj);


        // Slate as default. 
        GameObject instantiateObj = SlatePrefab;
        if (type == 2)
        {
            instantiateObj = SlateSquarePrefab;
        }
        else if (type == 3)
        {
            instantiateObj = SlateTitlePrefab;
	    }
        else if (type == 10)
        {
            instantiateObj = Bounded3DObjPrefab;
	    }
        else if (type == 20)
        {
            instantiateObj = FollowPairPrefab;
	    }
        GameObject gameobj = Instantiate(instantiateObj, pos, Quaternion.identity);
        initGameObjProps(gameobj, forward, msg, uuid);
        return gameobj;
    }

    private void initGameObjProps(GameObject gameobj, Vector3 forward, string msg, string uuid)
    {
        gameobj.transform.forward = forward;
	    // Set other props. 
        GeneratedObject genObject = gameobj.GetComponentInChildren<GeneratedObject>();
        if (genObject != null)
        {
            genObject.UpdateProps(msg);
        }
        m_ObjectManager.AddInteractiveObject(uuid, gameobj);
    }

    //public void TestSpawnObject()
    //{
    //    SerialUID += 1;

    //    var headTran = NRSessionManager.Instance.NRHMDPoseTracker.centerAnchor;
    //    //quad.name = "picture";
    //    //quad.transform.localPosition = headTran.position + headTran.forward * 3f;
    //    //quad.transform.forward = headTran.forward;
    //    //quad.transform.localScale = new Vector3(1.6f, 0.9f, 0);
    //    //quad.transform.localPosition = headTran.position + headTran.forward * 3f;

    //    var cam = CameraCache.Main;
    //    Debug.Log(cam.pixelHeight + ", " + cam.pixelWidth);
    //    Debug.Log(cam.nearClipPlane + ", " + cam.farClipPlane);
    //    var center = CameraCache.Main.ScreenToWorldPoint(new Vector3(0, 0.0f, 2.0f)); //  608.0f, 541.0f, 2.0f)); //  cam.transform.position.z)); //  Vector3.zero);

    //    var mapped = center; //  + headTran.forward * 1.0f;
    //    //var mapped = headTran.position + headTran.forward * 2.0f;

    //    // CHeck with using scren to WorldTransformation. 



    //    // Compute (x, y) offset from forward vector, and depth (
    //    var gameobj = addGameObjectWithUID(SerialUID, mapped, headTran.forward, "Test");

    //}

    public void DeleteObject(string uuid)
    {
        m_ObjectManager.DeleteObject(uuid);
    }

    public void SpawnObject(string uuid, int x, int y, int type, string msg)
    {
        SerialUID += 1;

        var headTran = NRSessionManager.Instance.NRHMDPoseTracker.centerAnchor;
        //quad.name = "picture";
        //quad.transform.localPosition = headTran.position + headTran.forward * 3f;
        //quad.transform.forward = headTran.forward;
        //quad.transform.localScale = new Vector3(1.6f, 0.9f, 0);
        //quad.transform.localPosition = headTran.position + headTran.forward * 3f;
        
        
        // CHeck with using scren to WorldTransformation. 
        var cam = CameraCache.Main;
        //Debug.Log(cam.pixelHeight + ", " + cam.pixelWidth);
        //Debug.Log(cam.nearClipPlane + ", " + cam.farClipPlane);
        var center = CameraCache.Main.ScreenToWorldPoint(new Vector3((x + 100) / 200.0f * cam.pixelWidth, (y + 100) / 200.0f * cam.pixelHeight, 2.0f));

        var mapped = center; //  headTran.position + headTran.forward * 2.0f;

        // Compute (x, y) offset from forward vector, and depth (
        var gameobj = addGameObjectWithUID(uuid, mapped, headTran.forward, type, msg);
    }

    /// <summary> Updates this object. </summary>
    void Update()
    {
        //// If the player doesn't click the trigger button, we are done with this update.
        //if (!NRInput.GetButtonDown(ControllerButton.TRIGGER))
        //{
        //    return;
        //}

        //if (ObjectManager.Instance.GetModelType() == 0)
        //{
        //    return;
        //}

        //// Get controller laser origin.
        //var handControllerAnchor = NRInput.DomainHand == ControllerHandEnum.Left ? ControllerAnchorEnum.LeftLaserAnchor : ControllerAnchorEnum.RightLaserAnchor;
        //Transform laserAnchor = NRInput.AnchorsHelper.GetAnchor(NRInput.RaycastMode == RaycastModeEnum.Gaze ? ControllerAnchorEnum.GazePoseTrackerAnchor : handControllerAnchor);

        //RaycastHit hitResult;
        //if (Physics.Raycast(new Ray(laserAnchor.transform.position, laserAnchor.transform.forward), out hitResult, 10))
        //{
        //    if (hitResult.collider.gameObject != null && hitResult.collider.gameObject.GetComponent<NRTrackableBehaviour>() != null)
        //    {
        //        var behaviour = hitResult.collider.gameObject.GetComponent<NRTrackableBehaviour>();
        //        if (behaviour.Trackable.GetTrackableType() != TrackableType.TRACKABLE_PLANE)
        //        {
        //            return;
        //        }

        //        if (HasLoadedModels)
        //        {
        //            SerialUID += 1;
        //            addGameObjectWithUID(SerialUID, hitResult.point);

        //            // var gameobj = new GameObject("glTFmodel");
        //            // gameobj.tag = "addedobj";
        //            // var tf = gameobj.transform;
        //            // // Debug.Log(transform.position);
        //            // // behaviour.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);

        //            // tf.Translate(hitResult.point);
        //            // if (modelType == 1)
        //            // {
        //            //     tf.localScale = new Vector3(0.25f, 0.25f, 0.25f);
        //            //     gltfDuck.InstantiateMainScene(tf);
        //            // }
        //            // else if (modelType == 2)
        //            // {
        //            //     tf.localScale = new Vector3(6, 6, 6);
        //            //     gltfAvocado.InstantiateMainScene(tf);
        //            // }
        //        }
        //        // else
        //        // {
        //        //     // Instantiate Andy model at the hit point / compensate for the hit point rotation.
        //        //     Instantiate(AndyPlanePrefab, hitResult.point, Quaternion.identity, behaviour.transform);
        //        // }
        //    }
        //}
    }
}
