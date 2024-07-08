using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshBuild : MonoBehaviour
{
    [HideInInspector]public NavMeshData navMeshData;
    private NavMeshDataInstance navMeshDataInstance;

    [Header("以玩家为中心，生成网格")]public Transform player;
    [Header("重新生成距离")][Header("如设置为网格大小(x,z)的一半，则离开该网格时马上生成第二个网格")]public float updateRadius = 100f;

    private Vector3 lastBuildPosition;

    private NavMeshSurface navs;

    List<NavMeshBuildSource> sources;

    private Thread GoCheck;

    [Range(0,500)][Header("生成的网格大小:X")]public int Matrix_X;

    [Range(0,50)][Header("生成的网格大小:Y")]public int Matrinx_Y;

    [Range(0,500)][Header("生成的网格大小:Z")]public int Matrix_Z;

    [Header("目标图层")]public LayerMask TargetLayer;

    void Start()
    {
        lastBuildPosition = player.position;
        navs = GetComponent<NavMeshSurface>();
        StartCoroutine(WaitForPlayerPosition());
    }

    public IEnumerator WaitForPlayerPosition()
    {
        CharacterController cc = player.GetComponent<CharacterController>();
        while (!cc.isGrounded)
        {
            Debug.Log("等待生成网格.....");
            yield return null;
        }
        BuildNavMeshAtPosition(player.position);
        lastBuildPosition = player.position;
    }

    void Reset()//弃用
    {
        //navs = GetComponent<NavMeshSurface>();
        //BuildNavMeshAtPosition(this.transform.position);
        //lastBuildPosition = this.transform.position;
    }

    public IEnumerator CheckDistance()
    {
        while (true)
        {
            Debug.Log("持续检测范围....");
            if (Vector3.Distance(player.position, lastBuildPosition) > updateRadius)
            {
                lastBuildPosition = player.position;
                BuildNavMeshAtPosition(player.position);
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void BuildNavMeshAtPosition(Vector3 position)
    {
        // 删除旧的NavMesh
        if (navMeshDataInstance.valid)
        {
            NavMesh.RemoveNavMeshData(navMeshDataInstance);
        }

        StartCoroutine(CollectSourcesAsync(position));
    }

    public IEnumerator CollectSourcesAsync(Vector3 position)
    {
        sources = new List<NavMeshBuildSource>();
        MeshCollider[] meshColliders = FindObjectsOfType<MeshCollider>();
        foreach (var collider in meshColliders)
        {
            if (collider.gameObject.layer == TargetLayer)
            {
                NavMeshBuildSource source = new NavMeshBuildSource();
                source.shape = NavMeshBuildSourceShape.Mesh;
                source.sourceObject = collider.sharedMesh;
                source.transform = collider.transform.localToWorldMatrix;
                source.area = NavMesh.GetAreaFromName("Walkable");
                sources.Add(source);
                Debug.Log($"Added MeshCollider as NavMeshBuildSource: {collider.gameObject.name}");
            }
        }

        Terrain[] terrains = FindObjectsOfType<Terrain>();
        foreach (var terrain in terrains)
        {
            NavMeshBuildSource source = new NavMeshBuildSource();
            source.shape = NavMeshBuildSourceShape.Terrain;
            source.sourceObject = terrain.terrainData;
            source.transform = terrain.transform.localToWorldMatrix;
            source.area = NavMesh.GetAreaFromName("Walkable");
            sources.Add(source);
            Debug.Log($"Added Terrain as NavMeshBuildSource: {terrain.gameObject.name}");
        }
        NavMeshBuildSettings buildSettings = NavMesh.GetSettingsByID(0);
        Bounds buildBounds = new Bounds(position, new Vector3(Matrix_X, Matrinx_Y, Matrix_Z));
        navMeshData = NavMeshBuilder.BuildNavMeshData(buildSettings, sources, buildBounds, Vector3.zero, Quaternion.identity);
        navMeshDataInstance = NavMesh.AddNavMeshData(navMeshData);
        navs.navMeshData = new NavMeshData(GetInstanceID());
        var messageHandlers = FindObjectsOfType<MonoBehaviour>().OfType<NavMeshs>();

        foreach (var handler in messageHandlers)
        {
            handler.OnNavMeshDataChange();
        }
        StartCoroutine(CheckDistance());
        yield break;
    }


    public interface NavMeshs : UnityEngine.EventSystems.IEventSystemHandler
    {
        void OnNavMeshDataChange();
    }
}
