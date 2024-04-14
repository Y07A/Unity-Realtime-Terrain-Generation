using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using ProceduralNoise;

public class TerrainMeshGenerator : MonoBehaviour
{
    [System.Serializable]
    public struct HeightLayer{
        public float weight;
        public NoiseUtils.NoiseType noiseType;
        public int noiseSeed;
        public float noiseFreq;
        public Vector2 noiseScale;
    }

    [System.Serializable]
    public struct Prop{
        public GameObject prefab;
        public int countMin, countMax;
        public NoiseUtils.NoiseType noiseType;
        public int noiseSeed;
        public float noiseFreq;
        public Vector2 noiseScale;
        public float scaleMin, scaleMax;
    }

    public GameObject tilePrefab;

    public float seed;

    [Header("Heights")]
    public HeightLayer[] heightLayers;

    [Header("Props")]
    public Prop[] props;

    [Header("Materials")]
    public Material terrainMaterial;

    [Header("Generation")]
    public int generatingRange = 1;

    private Dictionary<Vector3, TileProfile> activeTiles = new Dictionary<Vector3, TileProfile>();

    private TileProfile tileProfile;

    private void Start()
    {
        tileProfile = tilePrefab.GetComponent<TileProfile>();

        StartCoroutine(Generating());
    }

    private IEnumerator Generating()
    {
        StartCoroutine(Generate(SnapPosition(Camera.main.transform.position), 0f));
        for (int z = -1; z <= 1; z++){
            for (int x = -1; x <= 1; x++){
                StartCoroutine(Generate(SnapPosition(Camera.main.transform.position+new Vector3(x*tileProfile.size, 0f, z*tileProfile.size)*.8f), 0f));
                yield return new WaitForSeconds(.1f);
            }
        }
        while (true){
            ClearFarTiles();
            StartCoroutine(Generate(SnapPosition(Camera.main.transform.position)));
            for (int z = -generatingRange; z <= generatingRange; z++){
                for (int x = -generatingRange; x <= generatingRange; x++){
                    if (generatingRange % 2 == 0)
                        generatingRange++;
                    StartCoroutine(Generate(SnapPosition(Camera.main.transform.position+new Vector3(x*tileProfile.size, 0f, z*tileProfile.size)*.8f)));
                    
                    yield return new WaitForSeconds(.1f);
                }
            }
            
            yield return new WaitForSeconds(.2f);
        }
    }

    private Vector3 SnapPosition(Vector3 relativePos)
    {
        Vector3 worldPos = FloatingOrigin.Apply(relativePos);
        Vector2 snap = new Vector2(Mathf.Round(worldPos.x/tileProfile.size), Mathf.Round(worldPos.z/tileProfile.size));
        return new Vector3(snap.x*tileProfile.size, 0f, snap.y*tileProfile.size);
    }


    private IEnumerator Generate(Vector3 offset, float delay = 0.0005f)
    {
        if (activeTiles.ContainsKey(offset))
            yield break;
        

        TileProfile tile = Instantiate(tilePrefab, FloatingOrigin.Invert(offset), Quaternion.identity, transform).GetComponent<TileProfile>();
        activeTiles.Add(offset, tile);

        if (terrainMaterial != null)
            tile.ChangeMaterial(terrainMaterial);
        

        // Heights
        List<TileProcessing> tilesProcessing = new List<TileProcessing>();
        foreach(TileProfile.tileMesh map in tile.tiles){
            tilesProcessing.Add(new TileProcessing(map.filter, map.resolution));
        }

        foreach(TileProcessing tp in tilesProcessing){
            float texelRatio = (tp.resolution-1f)/tile.size;
            Vector2 texelOffset = new Vector2(seed+offset.x*texelRatio, seed+offset.z*texelRatio);
            
            foreach(HeightLayer hl in heightLayers){
                Noise noise = NoiseUtils.NewNoise(hl.noiseType, hl.noiseSeed, hl.noiseFreq);

                for (int y = 0; y < tp.resolution; y++)
                {
                    for (int x = 0; x < tp.resolution; x++)
                    {
                        float xCoord = x / (tp.resolution - 1f)*tile.size+offset.x;
                        float yCoord = y / (tp.resolution - 1f)*tile.size+offset.z;
                        float height = NoiseUtils.Sample2D(noise, xCoord*hl.noiseScale.x, yCoord*hl.noiseScale.y)*hl.weight;
                    
                        tp.vertices[y * tp.resolution + x].y += height;
                    }
                    yield return new WaitForSecondsRealtime(.001f);
                }
            }
            tp.Apply();
        }

        float propsRange = tile.GetComponent<TileLod>().propsRange*(tileProfile.size-.1f);

        Vector3 camPosition = FloatingOrigin.Apply(Camera.main.transform.position);
        camPosition.y = 0f;
        while (Vector3.Distance(offset, camPosition) > tileProfile.size*propsRange){
            yield return new WaitForSecondsRealtime(.1f);
            camPosition = FloatingOrigin.Apply(Camera.main.transform.position);
            camPosition.y = 0f;
        }
        
        // Props
        foreach(Prop p in props){
            StartCoroutine(GeneratingProps(p, tile));
            yield return new WaitForSecondsRealtime(.1f);
        }

    }

    private IEnumerator GeneratingProps(Prop p, TileProfile tile)
    {
        Vector3 offset = FloatingOrigin.Apply(tile.transform.position);
        Noise noise = NoiseUtils.NewNoise(p.noiseType, (int)seed+p.noiseSeed, p.noiseFreq);

        int count = (int)Mathf.Lerp(p.countMin, p.countMax, NoiseUtils.Sample2D(noise, offset.x*p.noiseScale.x, offset.z*p.noiseScale.y));

        for (int i = 0; i < count; i++){
            
            Transform prop = Instantiate(p.prefab).transform;
            

            prop.localScale = Vector3.one *Mathf.Clamp(NoiseUtils.Sample1D(noise, (offset.x*offset.z+i*i)*p.noiseScale.x)*p.scaleMax, p.scaleMin, p.scaleMax);
            
            Vector3 pos = new Vector3(noise.Sample1D((offset.x+i*50f)*p.noiseScale.x)*tile.size*.5f, 0f, noise.Sample1D((offset.z-i)*p.noiseScale.y)*tile.size*.5f);
            prop.position = tile.collider.SnapToSurface(tile.transform.position+pos);

            prop.localRotation = Quaternion.Euler(new Vector3(0, NoiseUtils.Sample1D(noise, (offset.z+offset.x*i-i*2f)*p.noiseScale.x)*360f, 0));
            prop.parent = tile.propsHandle;
            yield return new WaitForSecondsRealtime(.01f);
        }
    }

    private void ClearFarTiles()
    {
        var offsets = activeTiles.Keys.ToList();

        Vector3 camPosition = Camera.main.transform.position;
        camPosition.y = 0f;

        foreach(Vector3 pos in offsets){
            float distance = Vector3.Distance(FloatingOrigin.Invert(pos), camPosition);
            if (distance > tileProfile.size*generatingRange*1.3f){
                Destroy(activeTiles[pos].gameObject);
                activeTiles.Remove(pos);
            }
        }
    }
}

public class TileProcessing{
    public MeshFilter filter;
    public Mesh mesh;
    public Vector3[] vertices;
    public int resolution;

    public TileProcessing(MeshFilter m, int r)
    {
        filter = m;
        mesh = MeshCloner.Clone(m.mesh);
        vertices = mesh.vertices;
        resolution = r;
    }

    public void Apply()
    {
        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        filter.mesh = mesh;
        if (filter.GetComponent<TileCollider>() != null)
            filter.GetComponent<TileCollider>().Init();
    }
}
