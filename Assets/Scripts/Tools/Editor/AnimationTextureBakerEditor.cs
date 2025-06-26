using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using Codice.Client.BaseCommands;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class VATBaker : EditorWindow
{
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public Animator animator;
    public int sampleRate = 30;

    public Material vatMaterial; // Optional: Material to visualize VAT
    public Texture2D vatTexture; // Optional: Texture to visualize VAT
    public TextAsset vatMeta; // Optional: Metadata file generated during baking

    [MenuItem("Tools/VAT Baker")]
    public static void ShowWindow()
    {
        GetWindow<VATBaker>("VAT Baker");
    }

    void OnGUI()
    {
        skinnedMeshRenderer = (SkinnedMeshRenderer)EditorGUILayout.ObjectField("Skinned Mesh Renderer", skinnedMeshRenderer, typeof(SkinnedMeshRenderer), true);
        animator = (Animator)EditorGUILayout.ObjectField("Animator", animator, typeof(Animator), true);
        sampleRate = EditorGUILayout.IntField("Sample Rate (FPS)", sampleRate);

        if (GUILayout.Button("Bake VAT"))
        {
            if (skinnedMeshRenderer && animator)
            {
                BakeVAT();
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Assign both SkinnedMeshRenderer and Animator.", "OK");
            }
        }

        GUILayout.Label("VAT Texture:");
        vatTexture = (Texture2D)EditorGUILayout.ObjectField(vatTexture, typeof(Texture2D), false);

        GUILayout.Label("VAT Metadata File:");
        vatMeta = (TextAsset)EditorGUILayout.ObjectField(vatMeta, typeof(TextAsset), false);

        GUILayout.Label("VAT Material:");
        vatMaterial = (Material)EditorGUILayout.ObjectField(vatMaterial, typeof(Material), false);

        if (GUILayout.Button("Set VAT Data"))
        {
            if (vatMaterial != null && vatTexture != null && vatMeta != null)
            {
                SetVATData();
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Assign vatMaterial, vatTexture and vatMeta.", "OK");
            }
        }
    }

    void BakeVAT()
    {
        var clips = animator.runtimeAnimatorController.animationClips;
        int vertexCount = skinnedMeshRenderer.sharedMesh.vertexCount;

        var skinnedMeshTrans = skinnedMeshRenderer.transform;
        skinnedMeshTrans.position = Vector3.zero;
        skinnedMeshTrans.rotation = Quaternion.identity;
        skinnedMeshTrans.localScale = Vector3.one;

        // Calculate total frames
        int totalFrames = 0;
        Dictionary<string, int> clipFrameOffsets = new Dictionary<string, int>();
        foreach (var clip in clips)
        {
            clipFrameOffsets[clip.name] = totalFrames;
            int frame = Mathf.CeilToInt(clip.length * sampleRate);
            Debug.Log(clip.name + " - Lenght: " + clip.length + " - Frame: " + frame);
            totalFrames += frame;
        }

        // Prepare texture
        Debug.Log("Initialize Texture: " + vertexCount + " x " + totalFrames);
        Texture2D vatTexture = new Texture2D(vertexCount, totalFrames, TextureFormat.RGBAHalf, false, true);
        vatTexture.wrapMode = TextureWrapMode.Clamp;

        Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

        
        Mesh bakedMesh = new Mesh();
        int frameIndex = 0;

        // First pass: find min/max
        foreach (var clip in clips)
        {
            int frames = Mathf.CeilToInt(clip.length * sampleRate);
            for (int f = 0; f < frames; f++)
            {
                float time = (float)f / sampleRate;
                clip.SampleAnimation(animator.gameObject, time);
                skinnedMeshRenderer.BakeMesh(bakedMesh);
                var vertices = bakedMesh.vertices;
                for (int v = 0; v < vertexCount; v++)
                {
                    Vector3 pos = vertices[v];
                    min = Vector3.Min(min, pos);
                    max = Vector3.Max(max, pos);
                }
            }
        }
        Debug.Log("Min: " + min.ToString() + " | Max: " + max.ToString());

        // Second pass: write normalized positions
        foreach (var clip in clips)
        {
            int frames = Mathf.CeilToInt(clip.length * sampleRate);
            Debug.Log("Clip: " + clip.name + " | Frame: " + frames);

            for (int f = 0; f < frames; f++)
            {
                float time = (float)f / sampleRate;
                clip.SampleAnimation(animator.gameObject, time);
                skinnedMeshRenderer.BakeMesh(bakedMesh);

                var vertices = bakedMesh.vertices;
                for (int v = 0; v < vertexCount; v++)
                {
                    Vector3 pos = vertices[v];
                    Vector3 normalized = new Vector3(
                        (pos.x - min.x) / (max.x - min.x),
                        (pos.y - min.y) / (max.y - min.y),
                        (pos.z - min.z) / (max.z - min.z)
                    );
                    vatTexture.SetPixel(v, frameIndex, new Color(normalized.x, normalized.y, normalized.z, 1));
                }
                frameIndex++;
            }
        }

        vatTexture.Apply();

        // Save texture
        string path = EditorUtility.SaveFilePanel("Save VAT Texture", "Assets", "VAT_Texture.png", "png");
        if (!string.IsNullOrEmpty(path))
        {
            File.WriteAllBytes(path, vatTexture.EncodeToPNG());
            AssetDatabase.Refresh();
            Debug.Log("VAT Texture saved to: " + path);
        }

        // Save metadata
        string metaPath = Path.ChangeExtension(path, ".txt");
        using (StreamWriter sw = new StreamWriter(metaPath))
        {
            sw.WriteLine($"{vertexCount},{totalFrames}");
            sw.WriteLine($"AABB,{min.x},{min.y},{min.z},{max.x},{max.y},{max.z}");
            foreach (var clip in clips)
            {
                int offset = clipFrameOffsets[clip.name];
                int frames = Mathf.CeilToInt(clip.length * sampleRate);
                sw.WriteLine($"{clip.name},{offset},{frames},{sampleRate}");                
            }
        }
        Debug.Log("VAT metadata saved to: " + metaPath);

        // Create or update VAT material
        if (vatMaterial != null)
        {            
            //Update existing material with new Data: _VertexCount, _VATTex, _FrameCount, _AABBMin, _AABBMax
            Debug.Log("VAT Material updated.");
            vatMaterial.SetTexture("_VATTex", vatTexture);
            vatMaterial.SetFloat("_VertexCount", vertexCount);
            vatMaterial.SetFloat("_FrameCount", totalFrames);
            vatMaterial.SetVector("_AABBMin", min);
            vatMaterial.SetVector("_AABBMax", max);
        }
    }

    void SetVATData()
    {
        if (vatMaterial == null || vatTexture == null || vatMeta == null)
        {
            EditorUtility.DisplayDialog("Error", "Assign vatMaterial, vatTexture and vatMeta.", "OK");
            return;
        }

        // Parse metadata (format: name,startFrame,frameCount,sampleRate)
        var lines = vatMeta.text.Split('\n');
        int vertexCount = int.Parse(lines[0].Split(',')[0]);
        var parts = lines[1].Split(',');
        if (parts[0] == "AABB" && parts.Length >= 7)
        {
            // Parse min and max
            Vector3 min = new Vector3(
                float.Parse(parts[1]),
                float.Parse(parts[2]),
                float.Parse(parts[3])
            );
            Vector3 max = new Vector3(
                float.Parse(parts[4]),
                float.Parse(parts[5]),
                float.Parse(parts[6])
            );
            if (vatMaterial != null)
            {
                vatMaterial.SetVector("_AABBMin", min);
                vatMaterial.SetVector("_AABBMax", max);
            }
        }

        vatMaterial.SetTexture("_VATTex", vatTexture);
        vatMaterial.SetFloat("_VertexCount", vertexCount);
        vatMaterial.SetFloat("_FrameCount", vatTexture.height);
        
    }
}
