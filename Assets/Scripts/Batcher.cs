using UnityEngine;
using System.Collections;

// A Batcher works at a layer level, only the children of the batcher are taken into account
public class Batcher : MonoBehaviour
{

    private MeshRenderer mMeshRenderer;
    private Mesh mMesh;
    private MeshFilter mMeshFilter;

    private bool areChildrenVisible = false;

    [SerializeField]
    bool EnableBatching;

    [SerializeField]
    SpriteSheet mSpriteSheet;

    void Awake()
    {
        mMesh = new UnityEngine.Mesh();
        mMesh.MarkDynamic();

        mMeshFilter = gameObject.AddComponent<MeshFilter>();
        mMeshFilter.mesh = mMesh;

        mMeshRenderer = gameObject.AddComponent<MeshRenderer>();
        mMeshRenderer.castShadows = false;
        mMeshRenderer.useLightProbes = false;
        mMeshRenderer.receiveShadows = false;
    }

    // Use this for initialization
    void Start()
    {
        mMeshRenderer.material = new Material(mSpriteSheet.Shader);
        mMeshRenderer.material.mainTexture = mSpriteSheet.Texture;
    }

    // LateUpdate is called once per frame
    void LateUpdate()
    {
        if (EnableBatching)
        {
            // As we are batching, we disable the standard draw process of any children
            if (!areChildrenVisible)
                areChildrenVisible = showChildren(false);

            MeshFilter spriteMeshFilter;
            int verticesCount = 0;
            int uvsCount = 0;
            int trianglesCount = 0;

            Mesh mesh;

            // First pass, count the relevant meshes to merge in our mesh.
            foreach (Sprite s in gameObject.GetComponentsInChildren<Sprite>())
            {
                spriteMeshFilter = s.GetComponent<MeshFilter>();

                if (spriteMeshFilter != null)
                {
                    mesh = spriteMeshFilter.sharedMesh;

                    // Using only sprites, our meshes are complete with 4 vertices, 4 uvs and 6 integers (2 triangles)
                    if (mesh.vertices.Length != 4 || mesh.uv.Length != 4 || mesh.triangles.Length != 6)
                    {
                        continue;
                    }

                    verticesCount += 4;
                    uvsCount += 4;
                    trianglesCount += 6;
                }
            }

            Vector3[] vertices = new Vector3[verticesCount];
            Vector2[] uvs = new Vector2[uvsCount];
            int[] triangles = new int[trianglesCount];

            int verticesOffset = 0;
            int uvsOffset = 0;
            int trianglesOffset = 0;
            int meshCount = 0;

            foreach (Sprite s in gameObject.GetComponentsInChildren<Sprite>())
            {
                spriteMeshFilter = s.GetComponent<MeshFilter>();

                if (spriteMeshFilter != null)
                {
                    // We get the sharedMesh to avoid copying the mesh and getting only a reference
                    // on the first frame if the sprite is animated.
                    mesh = spriteMeshFilter.sharedMesh;

                    // If this is an incomplete mesh, we avoid it
                    if (mesh.vertices.Length != 4 || mesh.uv.Length != 4 || mesh.triangles.Length != 6)
                    {
                        continue;
                    }

                    // We copy the vertices adding an offset evaluated as the transform position of the sprite as
                    // the sprite vertices are on a local position and the sprite position defines its position in the world
                    // UVs are copied as if, as the material is the same (in our case in particular)
                    for (int index = 0; index < 4; index++)
                    {
                        vertices[verticesOffset++] = mesh.vertices[index] + s.transform.position;
                        uvs[uvsOffset++] = mesh.uv[index];
                    }
                    // We copy the triangles adding an offset of 4 times the meshCount to it
                    // Indeed, the integers indicated by the triangles are not offseted in the mesh (all from 0 to 3)
                    // Whereas in our mesh the first mesh uses [0-3], the second [4-7] etc.
                    for (int index = 0; index < 6; index++)
                    {
                        triangles[trianglesOffset++] = mesh.triangles[index] + meshCount * 4;
                    }

                    meshCount++;
                }
            }

            // All the calculation is done, let's clear the mesh and change its content
            // Bounds are automaticaly calculated after setting the triangles
            mMesh.Clear();
            mMesh.vertices = vertices;
            mMesh.uv = uvs;
            mMesh.triangles = triangles;

        }
        else
        {
            // We ended batching, if the children are still hidden, we have to show them again
            if (areChildrenVisible)
                areChildrenVisible = showChildren(true);
        }
    }

    bool showChildren(bool value)
    {
        // Just disable the render, nothing more nothing less
        foreach (Sprite s in gameObject.GetComponentsInChildren<Sprite>())
        {
            s.gameObject.renderer.enabled = value;
        }

        return value;
    }
}