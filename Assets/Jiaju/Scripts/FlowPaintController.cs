using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.iOS;
using Portalble;


public class FlowPaintController : PortalbleGeneralController
{
    public Transform placePrefab;
    public float offset = 0.00002f;
    public Transform m_paintPrefab;
    public Transform m_paintBall;
    private Vector3 _paintBall_pos = Vector3.zero;
    private Vector3 _paintBall_prev_pos = Vector3.zero;
    private float _fac = 0.12f * 2f;

    public List<Transform> _emojis = new List<Transform>();
    private Queue<Transform> _emojiWindow = new Queue<Transform>();

    //public Transform m_emoji_1;
    //public Transform m_emoji_2;
    //public Transform m_emoji_3;
    //public Transform m_emoji_4;
    //public Transform m_emoji_5;

    public int mode;

    private int _id = 0;
    private int _counter = 0;

    private Queue<Vector3> _burstPoses = new Queue<Vector3>();
    private bool _isBursting = false;


    protected override void Start()
    {
        base.Start();
        m_paintBall.gameObject.SetActive(false);

        _id = Random.Range(0, _emojis.Count);
    }

    protected override void Update()
    {
        base.Update();

        DrawingHelper();

        if (_isBursting)
        {
            IEnumerator<Vector3> ienum = _burstPoses.GetEnumerator();

            while (ienum.MoveNext())
            {
                Rigidbody rb = Instantiate(_emojis[Random.Range(0, _emojis.Count)], ienum.Current, Quaternion.identity).GetComponent<Rigidbody>();
                rb.gameObject.transform.LookAt(Camera.main.transform.position);
                rb.isKinematic = false;
                rb.AddForce(new Vector3(0, 30, 0));
            }

        }
    }

    private void DrawingHelper()
    {
        // Handle screen touches.
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    _paintBall_pos = m_FirstPersonCamera.transform.position + _fac * m_FirstPersonCamera.transform.forward;
                    m_paintBall.position = _paintBall_pos;

                    switch (mode)
                    {
                        case 0:
                            m_paintBall.gameObject.SetActive(true);
                            break;

                        case 1:
                            m_paintBall.gameObject.SetActive(false);
                            break;
                    }

                    break;

                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    _paintBall_prev_pos = _paintBall_pos;
                    _paintBall_pos = m_FirstPersonCamera.transform.position + _fac * m_FirstPersonCamera.transform.forward;

                    float detalDistance = Vector3.Distance(_paintBall_prev_pos, _paintBall_pos);
                   
                        switch (mode)
                        {
                            case 0: // draw more balls
                                if (detalDistance > 0.002f)
                                {
                                    Instantiate(m_paintPrefab, _paintBall_prev_pos, Quaternion.identity);
                                }
                                break;

                            case 1: // draw emojis
                                if (detalDistance > 0.0015f)
                                {
                                    Transform emoji = Instantiate(_emojis[_id], _paintBall_prev_pos, Quaternion.identity);
                                    emoji.transform.LookAt(Camera.main.transform.position);

                                    _counter++;

                                    if (_counter > Random.Range(3, 10))
                                    {
                                        _id = Random.Range(0, _emojis.Count);
                                        _counter = 0;
                                    }


                                    if (_emojiWindow.Count > 20)
                                    {
                                        Transform fall = _emojiWindow.Dequeue();
                                        fall.gameObject.GetComponent<Rigidbody>().isKinematic = false;
                                    }

                                    _emojiWindow.Enqueue(emoji);
                                 }
                                break;
                        }
                    break;

                case TouchPhase.Ended:
                    m_paintBall.gameObject.SetActive(false);
                    break;

            }
        }
    }

    public override void OnARPlaneHit(PortalbleHitResult hit)
    {
        if (mode == 0) return;

        base.OnARPlaneHit(hit);

        if(_burstPoses.Count > 2)
        {
            _burstPoses.Dequeue();
        }
        _burstPoses.Enqueue(hit.Pose.position);
        _isBursting = true;
    }

    //public override void OnARPlaneHit(PortalbleHitResult hit)
    //{
    //    base.OnARPlaneHit(hit);
    //    Transform poop = null;

    //    if (placePrefab != null)
    //    {
    //        poop = Instantiate(placePrefab, hit.Pose.position + hit.Pose.rotation * Vector3.up * offset, hit.Pose.rotation);
    //    }

    //    Vector3 targetPostition = new Vector3(Camera.main.transform.position.x,
    //                                poop.position.y,
    //                                Camera.main.transform.position.z);

    //    poop.transform.LookAt(2 * poop.position - targetPostition);
    //}
}
