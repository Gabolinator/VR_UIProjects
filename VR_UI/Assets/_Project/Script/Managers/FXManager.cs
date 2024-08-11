using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public enum FXCategory 
{
    Collision,
    BodyEditing, 
    UI,
    Other
   
}

public enum FXElement
{
    Audio,
    Visual,
    All
}

[System.Serializable]
public enum SelectionMode
{
    Random,
    PlayAll,
    TakeFirst,
    TakeLast,
    None
}

[System.Serializable]
public struct FXComponents
{
    public GameObject fxPrefab;
    public AudioClip audio;
}


[System.Serializable]
public struct FX
{
  //  public List<FXComponents> fxElements; //TODO to sure yet hw to properly organise... one fx handler has all the sounds or ... ? 
    public List<GameObject> fxPrefabs;
    public SelectionMode particleSelectionMode;
    public List<AudioClip> audios;
    public SelectionMode audioSelectionMode;
    public string keyword;
    
    [HideInInspector]
    public FXCategory category;

 
}

[System.Serializable]
public struct FXDictionnairy
{
    public List<FX> fxList;
    public FXCategory category;
}

public class FXManager : MonoBehaviour
{
    private static FXManager _instance;
    public static FXManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<FXManager>();

                if (_instance == null)
                {
                    GameObject singletonGO = new GameObject("FX_Manager");
                    _instance = singletonGO.AddComponent<FXManager>();
                }
            }
            return _instance;
        }
    }

    [SerializeField] private List<FXDictionnairy> fxDictionnairy = new List<FXDictionnairy>();

    public List<FXHandler> activeFX = new List<FXHandler>();

    public void ToggleFX(FXCategory category, FXElement whatToToggle, string keyword, Vector3 position, Quaternion rotation, Transform transform ,bool parent ,bool state = true, bool destroyAfterPlay = true)
    {
        Debug.Log("[FX Manager] Keyword : " + keyword);
        
        var fxList = new List<FX>();
      
        foreach (var entry in fxDictionnairy)
        {
            if (entry.category != category) continue;

            fxList = entry.fxList;
            break;
        }

        if (fxList.Count == 0) return;

        var fx = new List<FX>();
        if (string.IsNullOrEmpty(keyword)) fx = fxList;

        foreach (var element in fxList)
        {
            if (element.keyword.Contains(keyword))
            {
                var cloneFX = new FX();
                cloneFX = element;
                cloneFX.category = category;
                fx.Add(cloneFX);
            }
        }

        ToggleFX(fx, whatToToggle, position, rotation, transform, parent, state, destroyAfterPlay);

    }

    public void ToggleFX(FXCategory category, FXElement whatToToggle, string keyword, Transform transform , bool parent , bool state = true, bool destroyAfterPlay = true ) 
    {
        if (fxDictionnairy.Count == 0) return;

        if (!state) ToggleOffFX(transform);
        if(parent) ToggleFX(category, whatToToggle, keyword, transform, state);
        else ToggleFX(category, whatToToggle, keyword, transform.position, transform.rotation, transform , parent, state);
     }

    public void ToggleOffFX(Transform transform, string keyword = null)
    {
        if (activeFX.Count == 0) return;
        List<FXHandler> fxHandlers = new List<FXHandler>(transform.GetComponentsInChildren<FXHandler>());
        List <FXHandler> activeFXClone = new List<FXHandler>(activeFX);


        if (fxHandlers.Count > 0) 
        {
            foreach (var handler in fxHandlers) 
            {

                if(!string.IsNullOrEmpty(keyword)) if(handler.fx.keyword != keyword) continue;
             
                if(activeFXClone.Contains(handler)) 
                {
                  handler.DestroySelf();
                }  
                
                
            }
        }

    }

    public void ToggleFX(List<FX> fxList, FXElement whatToToggle, Transform transform, bool parent , bool state, bool destroyAfterPlay)
    {
        
    
        ToggleFX(fxList, whatToToggle, transform.position, transform.rotation, transform, parent ,state, destroyAfterPlay);
       
    }
   

    public void ToggleFX(List<FX> fxList, FXElement whatToToggle, Vector3 position, Quaternion rotation, Transform transform, bool parent, bool state, bool destroyAfterPlay)
    {
        if (fxList.Count == 0) return;

        int index = 0;
        
        
        
        if (fxList.Count != 1) index = UnityEngine.Random.Range((int)0, (int)(fxList.Count - 1));

        
        
        ToggleFX(fxList[index], whatToToggle, position, rotation, transform ,parent, state, destroyAfterPlay);
    }

    public void ToggleFX(FX fx, FXElement whatToToggle, Vector3 position, Quaternion rotation, Transform transform, bool parent ,bool state,bool destroyAfterPlay) 
    {
        /*Create fx handler*/
       
        if(fx.particleSelectionMode == SelectionMode.None && fx.audioSelectionMode == SelectionMode.None) return; 
        
        var newFXHandlerGO = new GameObject("FXHandler_" + fx.keyword);
        FXHandler fxHandler = newFXHandlerGO.AddComponent<FXHandler>();
        var fxTransform = fxHandler.transform;
                        
        if (parent)
        {
            fxTransform.parent = transform;
            fxTransform.localPosition = Vector3.zero;
            fxTransform.localRotation = Quaternion.identity;
        }
        else
        {
            fxTransform.localPosition = position;
            fxTransform.localRotation = rotation;
        }
        
        switch (whatToToggle)
        {
            
            case FXElement.All:
                if(fx.fxPrefabs.Count == 0) return;
                
                //We toggle everything 
                //so we dont change anything in FX 
                    break;

            case FXElement.Audio:

                /*wont play any fx prefab to keep only audio*/
                fx.particleSelectionMode = SelectionMode.None;
                break;
            
            case FXElement.Visual:
                if(fx.fxPrefabs.Count == 0) return;
                /*wont play any audio to play only visual*/
                fx.audioSelectionMode = SelectionMode.None;
                    
                break;

            default:
                break;

        }
        
        fxHandler.Initialize(fx);
        activeFX.Add(fxHandler);
    }

  

    public void ToggleFX(FX fx, FXElement whatToToggle, Transform transform, bool parent ,bool state,bool destroyAfterPlay) 
    {
        if (transform == null) return;
        ToggleFX(fx, whatToToggle, transform.position, transform.rotation, transform, parent, state, destroyAfterPlay);
       
    }

   

    public void PlayAudio(AudioSource source, List<AudioClip> audioClips, bool state)
    {

    }
    public FXHandler SpawnFXPrefab(GameObject fxPrefab, Transform transform) 
    {
        var prefabClone = Instantiate(fxPrefab, transform);
        var fxHandler = prefabClone.GetComponent<FXHandler>();
        if (!fxHandler) fxHandler = prefabClone.AddComponent<FXHandler>();


        return fxHandler;
    }

    public FXHandler SpawnFXPrefab(GameObject fxPrefab, Vector3 position, Quaternion rotation)
    {
        var prefabClone = Instantiate(fxPrefab, position, rotation);
        var fxHandler = prefabClone.GetComponent<FXHandler>();
        if (!fxHandler) fxHandler = prefabClone.AddComponent<FXHandler>();

        
        return fxHandler;
    }

    public void RegisterFX(FXHandler fx)
    {
        if (!activeFX.Contains(fx)) activeFX.Add(fx);
    }

    public void UnRegisterFX(FXHandler fx)
    {
        if (activeFX.Contains(fx)) activeFX.Remove(fx);
    }

    public void DestroyFX(FXHandler fx) 
    {
        fx.DestroySelf();
    }

    public void DestroyAllFX()
    {
        if (activeFX.Count == 0) return;
        var cachedList = new List<FXHandler>();
        cachedList.AddRange(activeFX);
            
        foreach (var fx in cachedList)
        {
           DestroyFX(fx);
        }
    }

    public void Start()
    {
      // ToggleFX(FXCategory.Other, FXElement.All, "testing",transform.position,transform.rotation,transform, true, true);
    }

    private void OnEnable()
    {
    
    }

    private void OnDisable()
    {
    
    }
    
   

   
   
}
