using System.Collections.Generic;
using UnityEngine;
using VolumetricLighting;

namespace Portals
{
    [DefaultExecutionOrder(100)]
    public class Portal : MonoBehaviour
    {
        public Portal Partner;
        [HideInInspector] public Camera Camera;

        public MeshRenderer MeshRenderer;

        private Camera MainCamera;

        public List<Light> LightsToCopy = new List<Light>();
        private Dictionary<int, VolumetricLight> VolumetricLightsToCopy = new Dictionary<int, VolumetricLight>();
        private Dictionary<int, Light> CopiedLights = new Dictionary<int, Light>();
        private Dictionary<int, VolumetricLight> CopiedVolumecticLights = new Dictionary<int, VolumetricLight>();

        public List<GameObject> ObjectsToCopy = new List<GameObject>();
        private Dictionary<int, GameObject> CopiedObjects = new Dictionary<int, GameObject>();
        private Dictionary<int, MeshRenderer> CopiedObjectsMeshRenderers = new Dictionary<int, MeshRenderer>();

        public bool DebugMode;

        private void Awake()
        {
            MainCamera = Camera.main;
        }

        private void Update()
        {
            if (Partner == null) return;

            if (Partner.Camera == null)
            {
                CreateCamera();
            }
            else
            {
                UpdateCamera();
            }
            UpdateCopiedLights();
            UpdateCopiedObjects();

            if (Input.GetKeyDown(KeyCode.F3))
            {
                DebugMode = !DebugMode;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (transform.worldToLocalMatrix.MultiplyPoint3x4(other.transform.position).z < 0)
            {
                Vector3 localPosition = transform.worldToLocalMatrix.MultiplyPoint3x4(other.transform.position);
                localPosition = new Vector3(-localPosition.x, localPosition.y, -localPosition.z);
                other.transform.position = Partner.transform.localToWorldMatrix.MultiplyPoint3x4(localPosition);

                other.transform.rotation = Partner.transform.rotation * Quaternion.Inverse(transform.rotation * Quaternion.Euler(0, 180, 0)) * other.transform.rotation;
            }
        }

        public void CreateCamera()
        {
            if (Partner.Camera != null)
            {
                Destroy(Partner.Camera.gameObject);
            }

            Partner.Camera = new GameObject(Partner.name + " Camera").AddComponent<Camera>();
            Partner.Camera.transform.SetParent(Partner.transform);
            Partner.Camera.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
            Partner.Camera.renderingPath = MainCamera.renderingPath;
            Partner.Camera.depth = MainCamera.depth;
            Partner.Camera.nearClipPlane = MainCamera.nearClipPlane;
            Partner.Camera.gameObject.AddComponent<VolumetricLightRenderer>();

            MeshRenderer.material = new Material(Shader.Find("Hidden/Portal"));
            MeshRenderer.material.mainTexture = Partner.Camera.targetTexture;
        }

        public void UpdateCamera()
        {
            Camera.clearFlags = MainCamera.clearFlags;
            Camera.backgroundColor = MainCamera.backgroundColor;
            Camera.fieldOfView = MainCamera.fieldOfView;

            Vector3 viewerPosition = Partner.transform.worldToLocalMatrix.MultiplyPoint3x4(MainCamera.transform.position);
            Camera.transform.localPosition = new Vector3(-viewerPosition.x, viewerPosition.y, -viewerPosition.z);

            Camera.transform.rotation = transform.rotation * Quaternion.Inverse(Partner.transform.rotation * Quaternion.Euler(0, 180, 0)) * MainCamera.transform.rotation;
        }

        public void UpdateCopiedLights()
        {
            for (int i = 0; i < LightsToCopy.Count; i++)
            {
                if (CopiedLights.ContainsKey(i))
                {
                    if (CopiedLights[i] == null)
                    {
                        CopiedLights[i] = Instantiate(LightsToCopy[i].gameObject).GetComponent<Light>();
                        CopiedLights[i].name = gameObject.name + " " + CopiedLights[i].name;
                        CopiedLights[i].transform.SetParent(Partner.transform);
                    }
                    else
                    {
                        Vector3 lightPosition = transform.worldToLocalMatrix.MultiplyPoint3x4(LightsToCopy[i].transform.position);
                        CopiedLights[i].transform.localPosition = new Vector3(-lightPosition.x, lightPosition.y, -lightPosition.z);

                        CopiedLights[i].transform.rotation = transform.rotation * Quaternion.Inverse(Partner.transform.rotation * Quaternion.Euler(0, 180, 0)) * LightsToCopy[i].transform.rotation;

						CopiedLights[i].enabled = LightsToCopy[i].enabled;
                        CopiedLights[i].type = LightsToCopy[i].type;
                        CopiedLights[i].range = LightsToCopy[i].range;
                        CopiedLights[i].spotAngle = LightsToCopy[i].spotAngle;
                        CopiedLights[i].color = LightsToCopy[i].color;
                        CopiedLights[i].intensity = LightsToCopy[i].intensity;
                        CopiedLights[i].shadows = LightsToCopy[i].shadows;

                        if (VolumetricLightsToCopy.ContainsKey(i))
                        {
                            if (VolumetricLightsToCopy[i] != null)
                            {
                                if (CopiedVolumecticLights.ContainsKey(i))
                                {
                                    CopiedVolumecticLights[i].enabled = VolumetricLightsToCopy[i].enabled;
                                    CopiedVolumecticLights[i].MaxRayLength = VolumetricLightsToCopy[i].MaxRayLength;
                                    CopiedVolumecticLights[i].ScatteringCoef = VolumetricLightsToCopy[i].ScatteringCoef;
                                    CopiedVolumecticLights[i].ExtinctionCoef = VolumetricLightsToCopy[i].ExtinctionCoef;
                                    CopiedVolumecticLights[i].SkyboxExtinctionCoef = VolumetricLightsToCopy[i].SkyboxExtinctionCoef;
                                    CopiedVolumecticLights[i].MieG = VolumetricLightsToCopy[i].MieG;
                                }
                                else
                                {
                                    CopiedVolumecticLights.Add(i, CopiedLights[i].GetComponent<VolumetricLight>());
                                }
                            }
                        }
                        else
                        {
                            VolumetricLightsToCopy.Add(i, LightsToCopy[i].GetComponent<VolumetricLight>());
                        }
                    }
                }
                else
                {
                    CopiedLights.Add(i, null);
                }
            }
        }

        public void UpdateCopiedObjects()
        {
            for (int i = 0; i < ObjectsToCopy.Count; i++)
            {
                if (CopiedObjects.ContainsKey(i))
                {
                    if (CopiedObjects[i] == null)
                    {
                        CopiedObjects[i] = Instantiate(ObjectsToCopy[i].gameObject);
                        CopiedObjects[i].name = gameObject.name + " " + CopiedObjects[i].name;
                        CopiedObjects[i].transform.SetParent(Partner.transform);

                        CopiedObjectsMeshRenderers.Add(i, CopiedObjects[i].GetComponent<MeshRenderer>());
                    }
                    else
                    {
                        Vector3 objectPosition = transform.worldToLocalMatrix.MultiplyPoint3x4(ObjectsToCopy[i].transform.position);
                        CopiedObjects[i].transform.localPosition = new Vector3(-objectPosition.x, objectPosition.y, -objectPosition.z);

                        CopiedObjects[i].transform.rotation = transform.rotation * Quaternion.Inverse(Partner.transform.rotation * Quaternion.Euler(0, 180, 0)) * ObjectsToCopy[i].transform.rotation;

                        if (DebugMode)
                        {
                            if (CopiedObjectsMeshRenderers.ContainsKey(i))
                            {
                                if (CopiedObjectsMeshRenderers[i] != null)
                                {
                                    CopiedObjectsMeshRenderers[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                                }
                            }
                        }
                        else
                        {
                            if (CopiedObjectsMeshRenderers.ContainsKey(i))
                            {
                                if (CopiedObjectsMeshRenderers[i] != null)
                                {
                                    CopiedObjectsMeshRenderers[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                                }
                            }
                        }
                    }
                }
                else
                {
                    CopiedObjects.Add(i, null);
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, Partner.transform.position);
        }
    }
}
