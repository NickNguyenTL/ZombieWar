using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class VATBaker : EditorWindow
{
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public Animator animator;
    public int sampleRate = 30;

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
    }

    void BakeVAT()
    {
        var mesh = skinnedMeshRenderer.sharedMesh;
        var clips = animator.runtimeAnimatorController.animationClips;
        int vertexCount = mesh.vertexCount;

        // Calculate total frames
        int totalFrames = 0;
        Dictionary<string, int> clipFrameOffsets = new Dictionary<string, int>();
        foreach (var clip in clips)
        {
            clipFrameOffsets[clip.name] = totalFrames;
            totalFrames += Mathf.CeilToInt(clip.length * sampleRate);
        }

        // Prepare texture
        Texture2D vatTexture = new Texture2D(vertexCount, totalFrames, TextureFormat.RGBAHalf, false, true);
        vatTexture.wrapMode = TextureWrapMode.Clamp;

        // Bake
        Mesh bakedMesh = new Mesh();
        int frameIndex = 0;
        foreach (var clip in clips)
        {
            animator.Play(clip.name, 0, 0);
            animator.Update(0);

            int frames = Mathf.CeilToInt(clip.length * sampleRate);
            for (int f = 0; f < frames; f++)
            {
                float time = (float)f / sampleRate;
                animator.Play(clip.name, 0, time / clip.length);
                animator.Update(0);

                skinnedMeshRenderer.BakeMesh(bakedMesh);
                var vertices = bakedMesh.vertices;

                for (int v = 0; v < vertexCount; v++)
                {
                    Vector3 pos = vertices[v];
                    vatTexture.SetPixel(v, frameIndex, new Color(pos.x, pos.y, pos.z, 1));
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
            foreach (var clip in clips)
            {
                int offset = clipFrameOffsets[clip.name];
                int frames = Mathf.CeilToInt(clip.length * sampleRate);
                sw.WriteLine($"{clip.name},{offset},{frames},{sampleRate}");
            }
        }
        Debug.Log("VAT metadata saved to: " + metaPath);
    }
}
