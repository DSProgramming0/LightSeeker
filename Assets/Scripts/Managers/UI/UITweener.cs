using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITweener : MonoBehaviour
{

    public LeanTweenType inType;
    public LeanTweenType outType;
    public float duration;
    public float delay;
    
    public void showUI()
    {
        LeanTween.scale(gameObject, new Vector3(1, 1, 1), duration).setDelay(delay).setOnComplete(OnComplete).setEase(inType);
    }

    public void hideUI()
    {
        LeanTween.scale(gameObject, new Vector3(0, 0, 0), duration).setDelay(delay).setOnComplete(OnComplete).setEase(outType);
    }


    private void OnComplete()
    {

    }
}
