using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class GlobalDistanceCulling : MonoBehaviour
{
    public Transform player;
    public float maxDistance = 200f;
    public float checkInterval = 0.5f;

    public string targetTag = "Map";
    public string playerTag = "Player";

    private List<Renderer> renderers = new List<Renderer>();
    private List<Collider> colliders = new List<Collider>();

    void Start()
    {
        FindPlayer();

        GameObject[] objs = GameObject.FindGameObjectsWithTag(targetTag);

        foreach (var obj in objs)
        {
            renderers.AddRange(obj.GetComponentsInChildren<Renderer>());
            colliders.AddRange(obj.GetComponentsInChildren<Collider>());
        }

        InvokeRepeating(nameof(CheckDistance), 0f, checkInterval);
    }

    void FindPlayer()
    {
        if (player != null) return;

        GameObject p = GameObject.FindGameObjectWithTag(playerTag);

        if (p != null)
        {
            player = p.transform;
        }
        else
        {
            // fallback: sahnadagi birinchi camera yoki rigidbody
            if (Camera.main != null)
                player = Camera.main.transform;
        }
    }

    void CheckDistance()
    {
        if (player == null)
        {
            FindPlayer();
            return;
        }

        Vector3 playerPos = player.position;

        foreach (var r in renderers)
        {
            if (r == null) continue;

            float dist = Vector3.Distance(playerPos, r.transform.position);
            bool visible = dist < maxDistance;

            if (r.enabled != visible)
                r.enabled = visible;
        }

        foreach (var c in colliders)
        {
            if (c == null) continue;

            float dist = Vector3.Distance(playerPos, c.transform.position);
            bool active = dist < maxDistance;

            if (c.enabled != active)
                c.enabled = active;
        }
    }
}