using System.Collections.Generic;
using UnityEngine;

namespace ProceduralWorlds.Flora
{
    [System.Serializable]
    public class TreeColliderData
    {
        public string m_treeName;
        public string m_vegetationType;
        public int m_id;
        public GameObject m_prefab;
        public bool m_buildCollider = true;
    }
    [System.Serializable]
    public class BakerColliderData
    {
        public bool m_drawMeshPreview = false;
        public Material m_defaultMaterial;
        public Mesh m_box;
        public Mesh m_capsule;
        public Mesh m_sphere;
    }

    public class TreeBakerProfile : ScriptableObject
    {
        public Terrain m_terrain;
        public GameObject m_parentObject;
        public BakerColliderData m_colliderData = new BakerColliderData();
        public List<TreeColliderData> Data = new List<TreeColliderData>();

        private List<GameObject> m_tempGeneratedPrefabs = new List<GameObject>();

        public bool CheckLastTreePrototypes(TreePrototype[] lastTreePrototypes)
        {
            if (lastTreePrototypes == null)
            {
                return true;
            }

            if (m_terrain != null)
            {
                TreePrototype[] treePrototypes = m_terrain.terrainData.treePrototypes;
                if (treePrototypes.Length > 0)
                {
                    if (lastTreePrototypes.Length != treePrototypes.Length)
                    {
                        return true;
                    }

                    for (int i = 0; i < treePrototypes.Length; i++)
                    {
                        TreePrototype prototype = treePrototypes[i];
                        TreePrototype lastPrototype = lastTreePrototypes[i];
                        if (prototype.prefab != lastPrototype.prefab)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
        public void BakeTerrainTreeColliders(Terrain terrain)
        {
            if (terrain == null)
            {
                return;
            }

            m_terrain = terrain;
            if (m_terrain != null && Data.Count > 0)
            {
                ClearBakedColliders();
                GetOrCreateRootParentObject(m_terrain);
                if (m_terrain.terrainData.treeInstances.Length < 1)
                {
                    Debug.Log("No tree instances we're found to spawn from");
                    return;
                }

                //Build instance list
                List<TreeInstance> allInstances = new List<TreeInstance>();
                foreach (TreeInstance instance in m_terrain.terrainData.treeInstances)
                {
                    allInstances.Add(instance);
                }

                for (int i = 0; i < Data.Count; i++)
                {
                    Collider dataCollider = GetCollider(Data[i].m_prefab);
                    if (Data[i].m_buildCollider && dataCollider != null)
                    {                
                        List<TreeInstance> currentTreeInstances = allInstances.FindAll(instance => instance.prototypeIndex == Data[i].m_id);
                        GameObject treeInstanceParent = CreateTreeInstanceParent(Data[i].m_treeName);
                        treeInstanceParent.transform.SetParent(m_parentObject.transform);
                        for (int j = 0; j < currentTreeInstances.Count; j++)
                        {
                            TreeInstance instance = currentTreeInstances[j];
                            GameObject treeColliderObject = new GameObject(Data[i].m_treeName + " " + currentTreeInstances[j].position);

                            MeshRenderer meshRenderer = null;
                            MeshFilter meshFilter = null;
                            if (m_colliderData.m_drawMeshPreview)
                            {
                                meshRenderer = treeColliderObject.AddComponent<MeshRenderer>();
                                meshFilter = treeColliderObject.AddComponent<MeshFilter>();
                            }

                            treeColliderObject.isStatic = true;
                            treeColliderObject.transform.SetParent(treeInstanceParent.transform);
                            if (dataCollider.GetType() == typeof(BoxCollider))
                            {
                                BoxCollider dataBoxCollider = (BoxCollider) dataCollider;
                                BoxCollider newBoxCollider = treeColliderObject.AddComponent<BoxCollider>();
                                //SetData
                                treeColliderObject.transform.position = TranslatePosition(instance.position, m_terrain);
                                treeColliderObject.transform.eulerAngles = new Vector3(0f, instance.rotation, 0f);
                                newBoxCollider.center = dataBoxCollider.center;
                                newBoxCollider.size = dataBoxCollider.size;
                                //Setup Mesh
                                treeColliderObject.transform.localScale = Vector3.one;
                                if (m_colliderData.m_drawMeshPreview)
                                {
                                    ConvertDefaultPreviewMaterial(m_colliderData.m_defaultMaterial);
                                    meshRenderer.sharedMaterial = m_colliderData.m_defaultMaterial;
                                    meshFilter.sharedMesh = m_colliderData.m_box;
                                }
                            }
                            else if (dataCollider.GetType() == typeof(SphereCollider))
                            {
                                SphereCollider dataSphereCollider = (SphereCollider)dataCollider;
                                SphereCollider newSphereCollider = treeColliderObject.AddComponent<SphereCollider>();
                                //SetData
                                treeColliderObject.transform.position = TranslatePosition(instance.position, m_terrain);
                                treeColliderObject.transform.eulerAngles = new Vector3(0f, instance.rotation, 0f);
                                newSphereCollider.center = dataSphereCollider.center;
                                newSphereCollider.radius = dataSphereCollider.radius;
                                //Setup Mesh
                                treeColliderObject.transform.localScale = Vector3.one;
                                if (m_colliderData.m_drawMeshPreview)
                                {
                                    ConvertDefaultPreviewMaterial(m_colliderData.m_defaultMaterial);
                                    meshRenderer.sharedMaterial = m_colliderData.m_defaultMaterial;
                                    meshFilter.sharedMesh = m_colliderData.m_sphere;
                                }
                            }
                            else if (dataCollider.GetType() == typeof(CapsuleCollider))
                            {
                                CapsuleCollider dataCapsuleCollider = (CapsuleCollider)dataCollider;
                                CapsuleCollider newCapsuleCollider = treeColliderObject.AddComponent<CapsuleCollider>();
                                //SetData
                                treeColliderObject.transform.position = TranslatePosition(instance.position, m_terrain);
                                treeColliderObject.transform.eulerAngles = new Vector3(0f, instance.rotation, 0f);
                                newCapsuleCollider.center = dataCapsuleCollider.center;
                                newCapsuleCollider.radius = dataCapsuleCollider.radius;
                                newCapsuleCollider.direction = dataCapsuleCollider.direction;
                                newCapsuleCollider.height = dataCapsuleCollider.height;
                                //Setup Mesh
                                treeColliderObject.transform.localScale = Vector3.one;
                                if (m_colliderData.m_drawMeshPreview)
                                {
                                    ConvertDefaultPreviewMaterial(m_colliderData.m_defaultMaterial);
                                    meshRenderer.sharedMaterial = m_colliderData.m_defaultMaterial;
                                    meshFilter.sharedMesh = m_colliderData.m_capsule;
                                }
                            }
                            else if (dataCollider.GetType() == typeof(MeshCollider))
                            {
                                MeshCollider dataMeshCollider = (MeshCollider)dataCollider;
                                MeshCollider newMeshCollider = treeColliderObject.AddComponent<MeshCollider>();
                                //SetData
                                treeColliderObject.transform.localScale = Data[i].m_prefab.transform.localScale;
                                treeColliderObject.transform.position = TranslatePosition(instance.position, m_terrain);
                                treeColliderObject.transform.eulerAngles = new Vector3(0f, instance.rotation, 0f);
                                newMeshCollider.convex = dataMeshCollider.convex;
                                newMeshCollider.cookingOptions = dataMeshCollider.cookingOptions;
                                newMeshCollider.sharedMesh = dataMeshCollider.sharedMesh;
                                //Setup Mesh
                                if (m_colliderData.m_drawMeshPreview)
                                {
                                    ConvertDefaultPreviewMaterial(m_colliderData.m_defaultMaterial);
                                    meshRenderer.sharedMaterial = m_colliderData.m_defaultMaterial;
                                    meshFilter.sharedMesh = dataMeshCollider.sharedMesh;
                                }
                            }
                        }

                        treeInstanceParent.transform.localPosition = Vector3.zero;
                    }
                    else
                    {
                        Debug.Log("Collision Will Be Skipped: " + Data[i].m_treeName + " does not have a collider present (Box, Capsule, Sphere or Mesh Collider)");
                    }
                }

                m_parentObject.transform.localPosition = Vector3.zero;
                CleanUpTemps();
            }
        }
        public void ClearBakedColliders()
        {
            if (m_parentObject != null)
            {
                DestroyImmediate(m_parentObject);
            }

            GameObject[] colliderObjects = GameObject.FindGameObjectsWithTag("Flora Tree Collision");
            if (colliderObjects.Length > 0)
            {
                foreach (GameObject colliderObject in colliderObjects)
                {
                    DestroyImmediate(colliderObject);
                }
            }
        }
        public void SetBuildState(bool build)
        {
            if (Data.Count > 0)
            {
                foreach (TreeColliderData data in Data)
                {
                    data.m_buildCollider = build;
                }
            }
        }
        /// <summary>
        /// Insures that the default render material works in the current render pipeline
        /// </summary>
        /// <param name="material"></param>
        private static void ConvertDefaultPreviewMaterial(Material material)
        {
            if (material != null)
            {
                switch (FloraTerrainTile.GetRenderPipline())
                {
                    case FloraTerrainTile.FLORARP.Builtin:
                    {
                        material.shader = Shader.Find("Standard");
                        break;
                    }
                    case FloraTerrainTile.FLORARP.URP:
                    {
                        material.shader = Shader.Find("Universal Render Pipeline/Lit");
                        break;
                    }
                    case FloraTerrainTile.FLORARP.HDRP:
                    {
                        material.shader = Shader.Find("HDRP/Lit");
                        break;
                    }
                }
            }
        }
        private void GetOrCreateRootParentObject(Terrain terrain)
        {
            if (m_parentObject == null && terrain != null)
            {
                m_parentObject = new GameObject("Generated Tree Colliders " + terrain.name);
                m_parentObject.transform.SetParent(terrain.transform);
                m_parentObject.tag = "Flora Tree Collision";
            }
        }
        private GameObject CreateTreeInstanceParent(string name)
        {
            return new GameObject(name);
        }
        private Collider GetCollider(GameObject treeObject)
        {
            GameObject tempObject = Instantiate(treeObject);
            m_tempGeneratedPrefabs.Add(tempObject);
            Collider collider = null;
            if (tempObject != null)
            {
                collider = tempObject.GetComponent<Collider>();
                if (collider == null)
                {
                    collider = tempObject.GetComponentInChildren<Collider>();
                }
            }

            return collider;
        }
        private void CleanUpTemps()
        {
            if (m_tempGeneratedPrefabs.Count > 0)
            {
                foreach (GameObject tempGeneratedPrefab in m_tempGeneratedPrefabs)
                {
                    DestroyImmediate(tempGeneratedPrefab);
                }

                m_tempGeneratedPrefabs.Clear();
            }
        }
        private Vector3 TranslatePosition(Vector3 treePosition, Terrain terrain)
        {
            TerrainData data = terrain.terrainData;
            float width = data.size.x;
            float height = data.size.z;
            float y = data.size.y;
            return new Vector3(treePosition.x * width, treePosition.y * y - 0.05f, treePosition.z * height);
        }
    }
}