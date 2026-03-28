using Dreamteck.Splines;
using UnityEngine;
using RubyCase.Pools;

namespace RubyCase.Game
{
    public class ConveyorArrow : MonoBehaviour, IPoolObject
    {
        [SerializeField] private SplineFollower splineFollower;

        public void SetSpline(SplineComputer spline, double percent, float followSpeed)
        {
            splineFollower.spline = spline;
            splineFollower.RebuildImmediate();
            splineFollower.follow = true;
            splineFollower.followSpeed = followSpeed;
            splineFollower.SetPercent(percent);
        }

        private void Update()
        {
            if (!splineFollower.follow) return;

            if (splineFollower.GetPercent() >= 1)
            {
                splineFollower.SetPercent(0);
                splineFollower.Rebuild();
            }
        }

        public void Stop()
        {
            splineFollower.follow = false;
        }

        public void Reset()
        {
            splineFollower.follow = false;
            splineFollower.spline = null;
        }
    }
}
