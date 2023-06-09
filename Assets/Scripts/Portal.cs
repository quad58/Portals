using System.Collections.Generic;
using UnityEngine;
using VolumetricLighting;

namespace Portals
{
    [RequireComponent(typeof(MeshRenderer))]
    public class Portal : MonoBehaviour
    {
        public Portal Partner;
        [HideInInspector] public Camera Camera;

        private MeshRenderer MeshRenderer;

        private Camera MainCamera;

        public List<Light> Lights = new List<Light>();
        private Dictionary<int, Light> LightsBeyondThePortal = new Dictionary<int, Light>();

        private void Awake()
        {
            MeshRenderer = GetComponent<MeshRenderer>();

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
            UpdateLights();
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

            Vector3 relativePosition = transform.InverseTransformPoint(MainCamera.transform.position);
            relativePosition = Quaternion.Euler(0, 180, 0) * relativePosition;
            Camera.transform.position = Partner.transform.TransformPoint(relativePosition);

            Camera.transform.rotation = transform.rotation * Quaternion.Inverse(Partner.transform.rotation * Quaternion.Euler(0, 180, 0)) * MainCamera.transform.rotation;
        }

        public void UpdateLights()
        {
            for (int i = 0; i < Lights.Count; i++)
            {
                if (LightsBeyondThePortal.ContainsKey(i))
                {
                    if (LightsBeyondThePortal[i] == null)
                    {
                        LightsBeyondThePortal[i] = Instantiate(Lights[i].gameObject).GetComponent<Light>();
                        LightsBeyondThePortal[i].name = gameObject.name + " " + LightsBeyondThePortal[i].name;
                        LightsBeyondThePortal[i].transform.SetParent(Partner.transform);
                    }
                    else
                    {
                        Vector3 relativePosition = transform.InverseTransformPoint(Lights[i].transform.position);
                        relativePosition = Quaternion.Euler(0, 180, 0) * relativePosition;
                        LightsBeyondThePortal[i].transform.position = Partner.transform.TransformPoint(relativePosition);

                        LightsBeyondThePortal[i].transform.rotation = transform.rotation * Quaternion.Inverse(Partner.transform.rotation * Quaternion.Euler(0, 180, 0)) * Lights[i].transform.rotation;

						LightsBeyondThePortal[i].enabled = Lights[i].enabled;
                        LightsBeyondThePortal[i].type = Lights[i].type;
                        LightsBeyondThePortal[i].range = Lights[i].range;
                        LightsBeyondThePortal[i].spotAngle = Lights[i].spotAngle;
                        LightsBeyondThePortal[i].color = Lights[i].color;
                        LightsBeyondThePortal[i].intensity = Lights[i].intensity;
                        LightsBeyondThePortal[i].shadows = Lights[i].shadows;

                        VolumetricLight volumetricLightOriginal = Lights[i].GetComponent<VolumetricLight>();
                        VolumetricLight volumetricLightBeyondThePortal = LightsBeyondThePortal[i].GetComponent<VolumetricLight>();
                        volumetricLightBeyondThePortal.enabled = volumetricLightOriginal.enabled;
                        volumetricLightBeyondThePortal.MaxRayLength = volumetricLightOriginal.MaxRayLength;
                        volumetricLightBeyondThePortal.ScatteringCoef = volumetricLightOriginal.ScatteringCoef;
                        volumetricLightBeyondThePortal.ExtinctionCoef = volumetricLightOriginal.ExtinctionCoef;
                        volumetricLightBeyondThePortal.SkyboxExtinctionCoef = volumetricLightOriginal.SkyboxExtinctionCoef;
                        volumetricLightBeyondThePortal.MieG = volumetricLightOriginal.MieG;
                    }
                }
                else
                {
                    LightsBeyondThePortal.Add(i, null);
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
