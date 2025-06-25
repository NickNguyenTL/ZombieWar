using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyModel : MonoBehaviour
{
    [SerializeField]
    private EnemyData enemyData;
    [SerializeField]
    private ChaserSystem chaserSystem;

    [Header("VAT Zone")]
    [SerializeField]
    public Mesh mesh;
    [SerializeField]
    private Texture2D vatTexture;
    [SerializeField]
    private Material vatMaterial;
    [SerializeField]
    private TextAsset vatMeta;    // Metadata file generated during baking

    public void Init(Transform target = null)
    {
        if (mesh != null && vatTexture != null && vatMaterial != null)
        {
            vatMaterial.SetTexture("_VATTex", vatTexture);
            vatMaterial.SetFloat("_VertexCount", mesh.vertexCount);
            vatMaterial.SetFloat("_FrameCount", vatTexture.height);

            Vector2[] texcoord1 = new Vector2[mesh.vertexCount];
            for (int i = 0; i < mesh.vertexCount; i++)
                texcoord1[i] = new Vector2(i, 0);
            mesh.uv2 = texcoord1;
        }

        // Parse metadata (format: name,startFrame,frameCount,sampleRate)
        var lines = vatMeta.text.Split('\n');
        animations = new AnimationInfo[lines.Length];
        for (int i = 0; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            var parts = lines[i].Split(',');
            animations[i] = new AnimationInfo
            {
                name = parts[0],
                startFrame = int.Parse(parts[1]),
                frameCount = int.Parse(parts[2]),
                sampleRate = int.Parse(parts[3])
            };
        }


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

    private AnimationInfo[] animations;
    private int currentAnim = 0;
    private float animTime = 0f;

    void Start()
    {
        if (chaserSystem == null)
        {
            chaserSystem = GetComponent<ChaserSystem>();
        }
        Init();
        
        Play(animations[0].name); // Start with the first animation
    }

    void Update()
    {
        if (animations == null || animations.Length == 0) return;

        var anim = animations[currentAnim];
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
        for (int i = 0; i < animations.Length; i++)
        {
            if (animations[i].name == animName)
            {
                currentAnim = i;
                animTime = 0f;
                break;
            }
        }
    }
}
