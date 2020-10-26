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
    private List<Transform> _emojis = new List<Transform>();

    public Transform m_emoji_1;
    public Transform m_emoji_2;
    public Transform m_emoji_3;
    public Transform m_emoji_4;
    public Transform m_emoji_5;

    public int mode;


    protected override void Start()
    {
        base.Start();
        m_paintBall.gameObject.SetActive(false);

        _emojis.Add(m_emoji_1);
        _emojis.Add(m_emoji_2);
        _emojis.Add(m_emoji_3);
        _emojis.Add(m_emoji_4);
        _emojis.Add(m_emoji_5);
    }

    protected override void Update()
    {
        base.Update();

        DrawingHelper();
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
                                if (detalDistance > 0.006f)
                                {
                                    int id = Random.Range(0, _emojis.Count);
                                    Transform emoji = Instantiate(_emojis[id], _paintBall_prev_pos, Quaternion.identity);
                                    emoji.transform.LookAt(Camera.main.transform.position);
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
