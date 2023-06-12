using System.Collections.Generic;
using UnityEngine;
using VolumetricLighting;

namespace Mirrors
{
    [DefaultExecutionOrder(110)]
    public class Mirror : MonoBehaviour
    {
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
            if (Camera == null)
            {
                CreateCamera();
            }
            else
            {
                UpdateCamera();
            }
            //UpdateCopiedLights();
            //UpdateCopiedObjects();

            if (Input.GetKeyDown(KeyCode.F3))
            {
                DebugMode = !DebugMode;
            }
        }

        public void CreateCamera()
        {
            if (Camera != null)
            {
                Destroy(Camera.gameObject);
            }

            Camera = new GameObject(name + " Camera").AddComponent<Camera>();
            Camera.transform.SetParent(transform);
            Camera.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
            Camera.renderingPath = MainCamera.renderingPath;
            Camera.depth = MainCamera.depth;
            Camera.nearClipPlane = MainCamera.nearClipPlane;
            Camera.gameObject.AddComponent<VolumetricLightRenderer>();

            MeshRenderer.material = new Material(Shader.Find("Hidden/Mirror"));
            MeshRenderer.material.mainTexture = Camera.targetTexture;
        }

        public void UpdateCamera()
        {
            Camera.clearFlags = MainCamera.clearFlags;
            Camera.backgroundColor = MainCamera.backgroundColor;
            Camera.fieldOfView = MainCamera.fieldOfView;

            Vector3 viewerPosition = transform.worldToLocalMatrix.MultiplyPoint3x4(MainCamera.transform.position);
            Camera.transform.localPosition = new Vector3(viewerPosition.x, viewerPosition.y, -viewerPosition.z);

            Camera.transform.rotation = transform.rotation * Quaternion.Inverse(transform.rotation * Quaternion.Euler(0, 180, 0)) * MainCamera.transform.rotation;
        }
    }
}
