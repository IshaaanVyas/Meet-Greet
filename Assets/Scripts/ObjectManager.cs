using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    //private static ObjectManager _instance;

    //public static ObjectManager Instance
    //{
    //    get { return _instance; }
    //}

    //private void Awake()
    //{
    //    if (_instance != null && _instance != this)
    //    {
    //        Destroy(this.gameObject);
    //    }
    //    else
    //    {
    //        _instance = this;
    //    }
    //    // Uncomment this to persist across scenes. 
    //    // DontDestroyOnLoad(this.gameObject);
    //}

    Dictionary<string, GameObject> interactiveObjects;

    // Start is called before the first frame update
    void Start()
    {
        interactiveObjects = new Dictionary<string, GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void DeleteAllObjects() {
        interactiveObjects.Clear();
    }

    public void DeleteObject(string uuid)
    {
        GameObject obj;
        if (interactiveObjects.TryGetValue(uuid, out obj))
        {
            // Found. 
            Destroy(obj);
            interactiveObjects.Remove(uuid);
        }
    }

    public void AddInteractiveObject(string uuid, GameObject obj)
    {
        interactiveObjects.Add(uuid, obj);
    }

}
