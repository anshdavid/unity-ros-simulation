using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class LaserScan3DReader : MonoBehaviour
    {

        public float maxAngle = 10;
        public float minAngle = -10;
        public int layers = 16;
        public int samples = 360;
        public float maxRange = 10f;
        public int updateRate = 1800;

        float vertIncrement;
        float azimutIncrAngle;
        private float azimuthAngle;
        private float[] distances;
        private Vector3[] direction;
        public Vector3[] hitPoints;
        public float[] intensities;
        public int Length;

        // Use this for initialization
        private void Awake()
        {
            vertIncrement = (float)(maxAngle - minAngle) / (float)(layers - 1);
            azimutIncrAngle = (float)(360.0f / samples);
            Length = layers * samples;
            distances = new float[Length];
            direction = new Vector3[Length];
            hitPoints = new Vector3[Length];
            intensities = new float[Length];
        }

        public Vector3[] Scan()
        {
            MeasureDistance();

            return hitPoints;
        }

        private void FixedUpdate()
        {
            MeasureDistance();
        }

        // Update is called once per frame
        private void MeasureDistance()
        {
            Vector3 forward = new Vector3(0, 0, 1);
            RaycastHit hit;
            float vAngle;
            int idx;

            //azimut angles
            for (int i = 0; i < samples; i++)
            {
                for (int y = 0; y < layers; y++)
                {

                    idx = y + i * layers;
                    vAngle = minAngle + (float)y * vertIncrement;
                    azimuthAngle = i * azimutIncrAngle;
                    direction[idx] = transform.rotation * Quaternion.Euler(-vAngle, azimuthAngle, 0) * forward;

                    if (Physics.Raycast(transform.position, direction[idx], out hit, maxRange))
                    {
                        distances[idx] = (float)hit.distance;
                        hitPoints[idx] = transform.InverseTransformPoint(hit.point);

                        Debug.DrawRay(transform.position, direction[idx] * distances[idx], Color.red);
                    }
                    else
                    {
                        hitPoints[idx] = new Vector3(0, 0, 0);
                        distances[idx] = 0.0f;
                    }
                }
            }

        }
    }
}