﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using Object = UnityEngine.Object;

public class GUIBehaviour : SGPScreen
{
    public EGuiBehaviour guiBehaviour;
    public CanvasGroup canvasGroup;
    public Image backGround;

    [Header("FadeIn / Out")]
    public bool overrideFadeValues = true;
    public bool fadeIn = true;
    public bool fadeOut = true;
    public float fadeDuration = 1;
    public float delayFade;
    public bool isFading;

    public bool FadeIn {get => overrideFadeValues ? fadeIn : GUIManager.Instance.FadeIn;}

    public bool FadeOut {get => overrideFadeValues ? fadeOut : GUIManager.Instance.FadeOut;}
    public float FadeDuration { get => overrideFadeValues ? fadeDuration : GUIManager.Instance.FadeDuration; }
    public float DelayFade { get => overrideFadeValues ? delayFade : GUIManager.Instance.DelayFade; }

    [SerializeField] private GameObject _snapVolume;
    public GameObject SnapVolume => _snapVolume;

    [SerializeField]
    private GameObject _rootPrefab;

    public GameObject RootPrefab
    {
        get => _rootPrefab;
        set => _rootPrefab = value;
    }

    public float maxAlpha = 1;
    public float minAlpha = 0;
    public float currentAlpha;
    public bool isNotClosable;
    public Vector3 snapVolumeStartPosition;

    
    
    public virtual void Fade(CanvasGroup canvasGroup, bool fadeIn, float fadeDuration = 1, float delay = 0.0f)
    {

        if (canvasGroup == null)
        {
            Debug.LogWarning("No CanvasGroup component.");
            return;
        }

        isFading = true;
        float startAlphaValue = fadeIn ? minAlpha : maxAlpha;
        float endAlphaValue = fadeIn ? maxAlpha : minAlpha;

        canvasGroup.alpha = startAlphaValue;

        StartCoroutine(FadeCoroutine(canvasGroup, startAlphaValue, endAlphaValue, fadeDuration, delay));
       
    }

    private IEnumerator FadeCoroutine(CanvasGroup canvasGroup, float startAlphaValue, float endAlphaValue, float fadeDuration, float delay)
    {
        yield return new WaitForSeconds(delay);
       
        float currentTime = 0;
        while (currentTime < fadeDuration)
        {
            canvasGroup.alpha = currentAlpha= Mathf.Lerp(startAlphaValue, endAlphaValue, currentTime / fadeDuration);
            currentTime += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = currentAlpha = endAlphaValue;
        isFading = false;
    }


    public virtual void Fade(bool state, float fadeDuration = -1) 
    {
        if (fadeDuration < 0) fadeDuration = FadeDuration;
        Fade(canvasGroup, state, fadeDuration, 0);
    }

    public virtual void FadeGuiIn() 
    {
        Fade(canvasGroup, true, FadeDuration, DelayFade);
    }

    public virtual void FadeGuiOut() 
    {
        Fade(canvasGroup, false, FadeDuration, 0);
    }

    public virtual void Start() 
    {
        GUIManager.Instance.RegisterGui(this.gameObject);
        if (FadeIn) FadeGuiIn();
       
    }

    public virtual void OnDestroy() 
    {
        GUIManager.Instance.UnRegisterGui(this.gameObject, true);
    }

    public virtual void SetMaxAlpha(float alpha)
    {
        if (maxAlpha != alpha) maxAlpha = alpha;
    }

    public virtual void SetAlpha(float alpha, bool fade = true)
    {
        if (fade)
        {
            if (canvasGroup.alpha != alpha)
                StartCoroutine(FadeCoroutine(canvasGroup, canvasGroup.alpha, alpha, .2f, 0));
        }

        else canvasGroup.alpha = alpha;
        
        //if (backGround) 
        //{
        //    Color color = backGround.color;
        //    color.a = alpha;
        //    backGround.color = color;

        //}
    }

    public override void Awake()
    {
        base.Awake();
        
        if (!SnapVolume) _snapVolume = GetComponent<XRInteractableSnapVolume>().GameObject();
       
        if (!SnapVolume) _snapVolume = GetComponentInChildren<XRInteractableSnapVolume>().GameObject();
        
    }

    
}