using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace CodeBase.Infos
{
    public class PathInfo : MonoBehaviour
    {
        public readonly ReactiveCommand<IEnumerable<Vector3>> PointsUpdated =
            new ReactiveCommand<IEnumerable<Vector3>>();

        public LineRenderer lineRenderer;
        public Color color;

        public List<Vector3> pointsList = new List<Vector3>();
    }
}