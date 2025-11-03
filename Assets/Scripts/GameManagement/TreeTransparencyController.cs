using UnityEngine;
using System.Collections.Generic;

public class TreeTransparencyController : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Camera mainCamera;

    [Header("Settings")]
    public float fadeSpeed = 5f;         // how quickly fade happens
    [Range(0f, 1f)] public float transparentAlpha = 0.3f;
    public LayerMask obstacleLayers;     // set this to include "Tree"

    private readonly Dictionary<Renderer, float> fadedRenderers = new();

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    void Update()
    {
        if (player == null || mainCamera == null) return;

        Vector3 camPos = mainCamera.transform.position;
        Vector3 dir = player.position - camPos;
        float distance = Vector3.Distance(player.position, camPos);

        // Find all colliders between camera and player
        RaycastHit[] hits = Physics.RaycastAll(camPos, dir, distance, obstacleLayers);

        HashSet<Renderer> hitRenderers = new();

        foreach (RaycastHit hit in hits)
        {
            Renderer r = hit.collider.GetComponentInChildren<Renderer>();
            if (r == null) continue;

            hitRenderers.Add(r);

            if (!fadedRenderers.ContainsKey(r))
                fadedRenderers[r] = r.material.color.a;

            // 🔹 Fade all materials on this renderer
            Material[] mats = r.materials;
            for (int i = 0; i < mats.Length; i++)
            {
                Color c = mats[i].color;
                c.a = Mathf.Lerp(c.a, transparentAlpha, Time.deltaTime * fadeSpeed);
                mats[i].color = c;
            }
            r.materials = mats;
        }

        // Restore any that are no longer blocking
        List<Renderer> toRestore = new();
        foreach (var kvp in fadedRenderers)
        {
            if (!hitRenderers.Contains(kvp.Key))
            {
                Renderer r = kvp.Key;
                Material[] mats = r.materials;
                for (int i = 0; i < mats.Length; i++)
                {
                    Color c = mats[i].color;
                    c.a = Mathf.Lerp(c.a, kvp.Value, Time.deltaTime * fadeSpeed);
                    mats[i].color = c;
                }
                r.materials = mats;

                if (Mathf.Abs(mats[0].color.a - kvp.Value) < 0.01f)
                    toRestore.Add(r);
            }
        }

        foreach (Renderer r in toRestore)
            fadedRenderers.Remove(r);
    }
}
