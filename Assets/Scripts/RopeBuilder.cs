using UnityEngine;

public class RopeBuilder : MonoBehaviour
{
    [Header("Rope Settings")]
    public GameObject ropeSegmentPrefab;
    public int segmentCount = 10;
    public float segmentLength = 0.25f;

    private void Start()
    {
        Rigidbody2D previousSegmentRb = GetComponent<Rigidbody2D>();

        for(int i=0;i<segmentCount;i++)
        {
            Vector3 spawnPos = transform.position - new Vector3(0, segmentLength * i, 0);
            GameObject newSegment = Instantiate(ropeSegmentPrefab, spawnPos, Quaternion.identity, transform);

            HingeJoint2D joint = newSegment.GetComponent<HingeJoint2D>();

            if (i == 0)
            {
                joint.connectedBody = previousSegmentRb;    //connect to anchor
            }
            else
            {
                joint.connectedBody = transform.GetChild(i - 1).GetComponent<Rigidbody2D>();    //connect to previous segment
            }

            //if last segment -> add trigger for player to grab

            if(i == segmentCount - 1)
            {
                CircleCollider2D grabTrigger = newSegment.AddComponent<CircleCollider2D>();
                grabTrigger.isTrigger = true;
                grabTrigger.radius = 0.2f;
                grabTrigger.gameObject.tag = "RopeGrab";
            }
        }
    }
}