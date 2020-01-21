using UnityEngine;

public class TextureDetector
{
    private TerrainData terrainData;
    private int alphamapWidth;
    private int alphamapHeight;
    private static float[,,] splatmapData;
    private static int numTextures;

    public TextureDetector()
    {
        terrainData = Terrain.activeTerrain.terrainData;
        alphamapWidth = terrainData.alphamapWidth;
        alphamapHeight = terrainData.alphamapHeight;

        splatmapData = terrainData.GetAlphamaps(0, 0, alphamapWidth, alphamapHeight);
        numTextures = splatmapData.Length / (alphamapWidth * alphamapHeight);
    }

    private static Vector3 ConvertToSplatMapCoordinate(Vector3 worldPosition)
    {
        Vector3 splatPosition = new Vector3();
        Terrain ter = Terrain.activeTerrain;
        Vector3 terPosition = ter.transform.position;
        splatPosition.x = ((worldPosition.x - terPosition.x) / ter.terrainData.size.x) * ter.terrainData.alphamapWidth;
        splatPosition.z = ((worldPosition.z - terPosition.z) / ter.terrainData.size.z) * ter.terrainData.alphamapHeight;
        return splatPosition;
    }

    public static int GetActiveTerrainTextureIdx(Vector3 position)
    {
        Vector3 terrainCord = ConvertToSplatMapCoordinate(position);
        int activeTerrainIndex = 0;
        float largestOpacity = 0f;

        for (int i = 0; i < numTextures; i++)
        {
            if (largestOpacity < splatmapData[(int)terrainCord.z, (int)terrainCord.x, i])
            {
                activeTerrainIndex = i;
                largestOpacity = splatmapData[(int)terrainCord.z, (int)terrainCord.x, i];
            }
        }

        return activeTerrainIndex;
    }

    public static Texture GetMeshMaterialOnHitCollider(Collider colliderHit)
    {
        if(colliderHit.GetType().Equals(typeof(TerrainCollider)))
        {
            Debug.Log("Hit terrain collider!");
        }
        else
        {
            var a = colliderHit.GetComponentInChildren<Renderer>();
            
            return a.material.mainTexture;
        }

        return null;
    }

    //fix
    public void CheckGround(Transform transform, float groundCheckHeight, float groundCheckRadius, float groundCheckDistance, LayerMask groundLayers, out RaycastHit currentGroundInfo)
    {
        Ray ray = new Ray(transform.position + Vector3.up * groundCheckHeight, Vector3.down);

        Debug.DrawRay(ray.origin, ray.direction);

        if (Physics.SphereCast(ray, groundCheckRadius, out currentGroundInfo, groundCheckDistance, groundLayers, QueryTriggerInteraction.Ignore))
        {
            
        }
    }

    public Texture2D GetSurfaceTexture(Collider col, Vector3 worldPos)
    {
        if(col.GetType().Equals(typeof(TerrainCollider)))
        {
            Terrain terrain = col.GetComponent<Terrain>();
            TerrainData terrainData = terrain.terrainData;
            var vec = ConvertToSplatMapCoordinate(worldPos);

        }

        return null;
    }
}