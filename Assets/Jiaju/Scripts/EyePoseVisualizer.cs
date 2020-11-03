using System.Collections;
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

        GameObject m_PaintBallGameObject;
        private Vector3 _paintBall_pos = Vector3.zero;
        private Vector3 _paintBall_prev_pos = Vector3.zero;

        public List<Transform> _emojis = new List<Transform>();
        private Queue<Transform> _emojiWindow = new Queue<Transform>();

        private int _id = 0;
        private int _counter = 0;
        private bool _canPaint = false;


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

            if (m_PaintBallGameObject == null )
            {
                m_PaintBallGameObject = Instantiate(m_EyePrefab, transform.TransformPoint(ballPos), Quaternion.identity);
                m_PaintBallGameObject.SetActive(false);
                _paintBall_pos = m_PaintBallGameObject.transform.position;
            }
        }

        void SetVisible(bool visible)
        {
            if (m_PaintBallGameObject != null)
            {
                m_PaintBallGameObject.SetActive(false);
            }
            _canPaint = visible;
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
            if (m_Face.vertices.IsCreated)
            {
                if(_canPaint)
                {
                    _paintBall_prev_pos = _paintBall_pos;

                    m_PaintBallGameObject.transform.position = transform.TransformPoint(m_Face.vertices[0]);
                    _paintBall_pos = m_PaintBallGameObject.transform.position;
                    //m_PaintBallGameObject.transform.forward = transform.TransformVector(m_Face.normals[0]);

                    if (Vector3.Distance(_paintBall_prev_pos, _paintBall_pos) > 0.0015f)
                    {
                        //Instantiate(m_EyePrefab, _paintBall_prev_pos, Quaternion.identity);
                        Transform emoji = Instantiate(_emojis[_id], _paintBall_prev_pos, Quaternion.identity);
                        emoji.transform.LookAt(Camera.main.transform.position);
                        emoji.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);

                        _counter++;

                        if (_counter > Random.Range(3, 10))
                        {
                            _id = Random.Range(0, _emojis.Count);
                            _counter = 0;
                        }


                        if (_emojiWindow.Count > 40)
                        {
                            Transform fall = _emojiWindow.Dequeue();
                            fall.gameObject.GetComponent<Rigidbody>().isKinematic = false;
                        }

                        _emojiWindow.Enqueue(emoji);

                    }
                }
                
            }
        }
    }
}