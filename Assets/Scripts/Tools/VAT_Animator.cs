using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class VAT_Animator : MonoBehaviour
{
    public Action<int> OnAnimEnd; // Callback when animation changes

    [SerializeField]
    private MeshRenderer meshRenderer; // MeshRenderer to visualize VAT
    [SerializeField]
    private TextAsset vatMeta; // Metadata file generated during baking
    [SerializeField]
    private Material vatMaterial; // Material to visualize VAT

    private class AnimationInfo
    {
        public string name;
        public int startFrame;
        public int frameCount;
        public int sampleRate;
    }

    private List<AnimationInfo> animList;
    private int currentAnim = 0;
    private float animTime = 0f;

    private MaterialPropertyBlock mpb;
    private Tween dissolveTween;

    public void Init()
    {
        // Parse metadata (format: name,startFrame,frameCount,sampleRate)
        var lines = vatMeta.text.Split('\n');
        animList = new List<AnimationInfo>();
        int vertexCount = int.Parse(lines[0].Split(',')[0]);
        for (int i = 2; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            var parts = lines[i].Split(',');
            if (parts.Length >= 4)
            {
                animList.Add(new AnimationInfo
                {
                    name = parts[0],
                    startFrame = int.Parse(parts[1]),
                    frameCount = int.Parse(parts[2]),
                    sampleRate = int.Parse(parts[3])
                });
            }
        }

        meshRenderer.sharedMaterial = vatMaterial;
        mpb = new MaterialPropertyBlock();
    }

    // Update is called once per frame
    void Update()
    {
        if (animList == null || animList.Count == 0) return;

        UpdateVAT_AnimFrame();
    }

    #region VAT Process
    private void UpdateVAT_AnimFrame()
    {
        var anim = animList[currentAnim];
        float animLength = anim.frameCount / (float)anim.sampleRate;
        if (animTime < animLength)
        {
            animTime = Mathf.Clamp(animTime + Time.deltaTime, 0, animLength);            
        }
        else
        {
            if(isRepeating)
            {
                animTime = 0; // Loop 
            }
            else
            {
                animTime = animLength; // Stop at the end
                OnAnimEnd?.Invoke(currentAnim);
                return;
            }
        }

        // Calculate current frame within this animation
        float frameInAnim = animTime * anim.sampleRate;
        float frame = Mathf.Clamp(anim.startFrame + frameInAnim, anim.startFrame, anim.startFrame + anim.frameCount - 1) ;

        //Debug.Log("Update: " + anim.startFrame + " | " + anim.frameCount + " | " + frameInAnim + " | " + frame);

        mpb.SetFloat("_CurrentFrame", frame);
        meshRenderer.SetPropertyBlock(mpb);
    }

    // Call this to play a specific animation by name
    public void Play(string animName, bool repeat = true)
    {
        for (int i = 0; i < animList.Count; i++)
        {
            if (animList[i].name == animName)
            {
                Play(i, repeat);
                break;
            }
        }
    }

    bool isRepeating;
    public void Play(int Id, bool repeat = true, bool forceRepeat = true)
    {
        if(Id < animList.Count)
        {
            if(currentAnim != Id || forceRepeat)
            {
                currentAnim = Id;
                animTime = 0f;
            }
            isRepeating = repeat;
        }
    }
    #endregion

    #region Other Material Process
    public void SetTintColor(Color32 color)
    {
        if (mpb == null)
        {
            mpb = new MaterialPropertyBlock();
        }

        mpb.SetColor("_Color", color);
        meshRenderer.SetPropertyBlock(mpb);
    }

    float dissolveValue = 0f;   
    /// <summary>
    /// Set dissolve state of the VAT material.
    /// </summary>
    /// <param name="state">
    /// true = Start Disolve, false = Revert Disolve
    /// </param>
    /// <param name="time">
    /// 0 = Instant change, > 0 = Smooth transition over time
    /// </param>
    public void SetDisolve(bool state, float time = 0)
    {
        float endDissolveValue = state ? 1f : 0f;
        if (time <= 0)
        {
            mpb.SetFloat("_Amount", endDissolveValue);
            meshRenderer.SetPropertyBlock(mpb);
        }

        
        dissolveTween = DOTween.To(
            () => dissolveValue,
            x => {
                dissolveValue = x;
                mpb.SetFloat("_Amount", dissolveValue);
                meshRenderer.SetPropertyBlock(mpb);
            },
            endDissolveValue,
            time
        );
    }

    public float GetAnimationTime(int id)
    {
        return animList[id].frameCount / (float)animList[id].sampleRate;
    }
    #endregion
}
