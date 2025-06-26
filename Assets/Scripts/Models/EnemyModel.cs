using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyModel : MonoBehaviour
{
    [SerializeField]
    private EnemyData enemyData;
    [SerializeField]
    private ChaserSystem chaserSystem;
    [SerializeField]
    private Material vatMaterial; // Material to visualize VAT

    [Header("VAT Zone")]
    [SerializeField]
    private TextAsset vatMeta;    // Metadata file generated during baking

    private void InitVAT_Data()
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
    }

    public void Init(Transform target = null)
    {
        InitVAT_Data();

        if (target != null)
        {
            chaserSystem.Init();
            chaserSystem.SetTarget(target);
        }
            
    }    

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

    void Start()
    {
        if (chaserSystem == null)
        {
            chaserSystem = GetComponent<ChaserSystem>();
        }
        Init();
        
        Play(animList[1].name); // Start with the first animation
    }

    void Update()
    {
        if (animList == null || animList.Count == 0) return;

        var anim = animList[currentAnim];
        animTime += Time.deltaTime;
        float animLength = anim.frameCount / (float)anim.sampleRate;
        if (animTime > animLength)
        {
            animTime = 0f; // Loop
        }

        // Calculate current frame within this animation
        float frameInAnim = animTime * anim.sampleRate;
        float frame = anim.startFrame + frameInAnim;

        vatMaterial.SetFloat("_CurrentFrame", frame);
    }

    // Call this to play a specific animation by name
    public void Play(string animName)
    {
        for (int i = 0; i < animList.Count; i++)
        {
            if (animList[i].name == animName)
            {
                currentAnim = i;
                animTime = 0f;
                break;
            }
        }
    }
}
