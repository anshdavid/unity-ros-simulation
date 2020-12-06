

using System;
using UnityEngine;
using System.Linq;
namespace RosSharp.RosBridgeClient
{
    public class LaserScan3DPublisher : UnityPublisher<MessageTypes.Sensor.PointCloud>
    {

        public LaserScan3DReader reader;
        public string FrameId = "Unity";
        private string ChannelName = "intensity";
        private MessageTypes.Sensor.PointCloud message;
        private float scanPeriod;
        private float elapsed = 0f;

        protected override void Start()
        {
            base.Start();
            InitializeMessage();
        }

        private void FixedUpdate()
        {
            elapsed += Time.deltaTime;
            if (elapsed >= scanPeriod)
            {
                elapsed = elapsed % scanPeriod;
                UpdateMessage();
            }
        }

        private void InitializeMessage()
        {
            scanPeriod = (float)(TimeSpan.FromMilliseconds(reader.updateRate).TotalSeconds);

            message = new MessageTypes.Sensor.PointCloud
            {
                header = new MessageTypes.Std.Header()
                {
                    frame_id = FrameId
                },
                points = new MessageTypes.Geometry.Point32[reader.Length],
                // channels = new MessageTypes.Sensor.ChannelFloat32[1]
            };
            // message.channels[0] = new MessageTypes.Sensor.ChannelFloat32()
            // {
            //     name = ChannelName,
            //     values = Enumerable.Repeat(0.0f, reader.Length).ToArray()
            // };
        }

        private void UpdateMessage()
        {
            message.header.Update();

            Vector3[] hitPoints = reader.Scan();

            for (int i = 0; i < reader.Length; i++)
            {
                message.points[i] = getGeometryPoint(hitPoints[i].Unity2Ros());
            }
            Publish(message);
        }

        private static MessageTypes.Geometry.Point32 getGeometryPoint(Vector3 hitpoint)
        {
            return new MessageTypes.Geometry.Point32(hitpoint.x, hitpoint.y, hitpoint.z);
        }
    }
}
