using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeManager : MonoBehaviour
{
    public static BiomeManager Instance;

    [SerializeField] Biome[] biomes;
    [SerializeField] float lineLength;

    private void Awake()
    {
        Instance = this;
    }
    
    public Biome GetBiomOnPosition(Vector3 objectPosition)
    {
        Vector2 posToCheckPosVector = objectPosition - transform.position;
        float angle = Vector2.SignedAngle(posToCheckPosVector, transform.up);
        if (angle < 0f) angle += 360f;
        
        foreach (Biome biome in biomes)
        {
            if (angle >= biome.fromAngle && angle < biome.toAngle)
            {
                return biome;
            }
        }
        return null;
    }
    private void OnDrawGizmos()
    {
        if (biomes == null || biomes.Length == 0)
        {
            return;
        }

        foreach (Biome biome in biomes)
        {
            Gizmos.color = biome.color;
            Gizmos.DrawLine(transform.position, transform.position + Quaternion.Euler(0, 0f, -biome.fromAngle) * transform.up * lineLength);
            Gizmos.DrawLine(transform.position, transform.position + Quaternion.Euler(0, 0f, -biome.toAngle) * transform.up * lineLength);
        }
    }
}
