using System.Collections.Generic;
using UnityEngine;

namespace Portals
{
    [RequireComponent(typeof(MeshRenderer))]
    public class Portal : MonoBehaviour
    {
        public Portal Partner;
        [HideInInspector] public Camera Camera;

        private MeshRenderer MeshRenderer;

        private Vector2Int LastScreenSize;

        private Camera MainCamera;

        public List<Light> Lights = new List<Light>();
        private Dictionary<int, Light> LightsBeyondThePortal = new Dictionary<int, Light>();

        private void Awake()
        {
            MeshRenderer = GetComponent<MeshRenderer>();

            LastScreenSize = new Vector2Int(Screen.width, Screen.height);

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

            Vector2Int currentScreenSize = new Vector2Int(Screen.width, Screen.height);
            if (LastScreenSize != currentScreenSize)
            {
                UpdateRenderTextureResolution(currentScreenSize);
            }
            LastScreenSize = currentScreenSize;
        }

        public void CreateCamera()
        {
            if (Partner.Camera != null)
            {
                Destroy(Partner.Camera.gameObject);
            }

            Partner.Camera = new GameObject(Partner.name + " Camera").AddComponent<Camera>();
            Partner.Camera.targetTexture = new RenderTexture(LastScreenSize.x, LastScreenSize.y, 24);
            Partner.Camera.renderingPath = MainCamera.renderingPath;
            Partner.Camera.depth = MainCamera.depth;

            MeshRenderer.material = new Material(Shader.Find("Hidden/Portal"));
            MeshRenderer.material.mainTexture = Partner.Camera.targetTexture;
        }

        public void UpdateCamera()
        {
            Camera.fieldOfView = MainCamera.fieldOfView;

            Vector3 relativePosition = transform.InverseTransformPoint(MainCamera.transform.position);
            relativePosition = Quaternion.Euler(0, 180, 0) * relativePosition;
            Camera.transform.position = Partner.transform.TransformPoint(relativePosition);

            Camera.transform.rotation = transform.rotation * Quaternion.Inverse(Partner.transform.rotation * Quaternion.Euler(0, 180, 0)) * MainCamera.transform.rotation;
            //Camera.nearClipPlane = Vector3.Distance(transform.position, Camera.transform.position);
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
                    }
                }
                else
                {
                    LightsBeyondThePortal.Add(i, null);
                }
            }
        }

        public void UpdateRenderTextureResolution(Vector2Int newResolution)
        {
            Partner.Camera.targetTexture.Release();
            Partner.Camera.targetTexture.width = newResolution.x;
            Partner.Camera.targetTexture.height = newResolution.y;
            Partner.Camera.targetTexture.Create();
        }
    }
}
