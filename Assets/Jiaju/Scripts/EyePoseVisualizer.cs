﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
#if UNITY_IOS && !UNITY_EDITOR
using UnityEngine.XR.ARKit;
#endif

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// Visualizes the eye poses for an <see cref="ARFace"/>.
    /// </summary>
    /// <remarks>
    /// Face space is the space where the origin is the transform of an <see cref="ARFace"/>.
    /// </remarks>
    [RequireComponent(typeof(ARFace))]
    public class EyePoseVisualizer : MonoBehaviour
    {
        [SerializeField]
        GameObject m_EyePrefab;

        public GameObject eyePrefab
        {
            get => m_EyePrefab;
            set => m_EyePrefab = value;
        }

        GameObject m_LeftEyeGameObject;

        ARFace m_Face;
        XRFaceSubsystem m_FaceSubsystem;

        void Awake()
        {
            m_Face = GetComponent<ARFace>();
        }

        void CreateEyeGameObjectsIfNecessary()
        {
            Vector3 ballPos = new Vector3(0f, 0f, 0f);
            if(m_Face.vertices.IsCreated)
            {
                ballPos = m_Face.vertices[0];
            }

            if (m_LeftEyeGameObject == null )
            {
                Debug.Log("Object Created at: " + transform.TransformPoint(ballPos));
                m_LeftEyeGameObject = Instantiate(m_EyePrefab, transform.TransformPoint(ballPos), Quaternion.identity);
                m_LeftEyeGameObject.SetActive(false);
            }
        }

        void SetVisible(bool visible)
        {
            if (m_LeftEyeGameObject != null)
            {
                m_LeftEyeGameObject.SetActive(visible);
            }
        }


        void OnEnable()
        {
            var faceManager = FindObjectOfType<ARFaceManager>();
            if (faceManager != null && faceManager.subsystem != null)
            {
                m_FaceSubsystem = (XRFaceSubsystem)faceManager.subsystem;
                SetVisible((m_Face.trackingState == TrackingState.Tracking) && (ARSession.state > ARSessionState.Ready));
                m_Face.updated += OnUpdated;
            }
            else
            {
                enabled = false;
            }
        }

        void OnDisable()
        {
            m_Face.updated -= OnUpdated;
            SetVisible(false);
        }

        void OnUpdated(ARFaceUpdatedEventArgs eventArgs)
        {
            CreateEyeGameObjectsIfNecessary();
            SetVisible((m_Face.trackingState == TrackingState.Tracking) && (ARSession.state > ARSessionState.Ready));
        }

        void Update()
        {
            //Vector3 ballPos = new Vector3(0f, 0f, 0f);
            if (m_Face.vertices.IsCreated)
            {
                m_LeftEyeGameObject.transform.position = transform.TransformPoint(m_Face.vertices[0]);
            }
        }
    }
}