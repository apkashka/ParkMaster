using System.Linq;
using CodeBase.Infos;
using CodeBase.Scriptable;
using CodeBase.Static;
using UniRx;
using UnityEngine;

namespace CodeBase.PseudoSystems
{
    public class PathsBuildingSystem : MonoBehaviour
    {
        private static readonly int Color = Shader.PropertyToID("_Color");

        private GameManager _game;
        private CoreConfig _config;
        private Camera _cam;
        private PathInfo _currentPath;
        private RaycastHit _hitInfo;

        private PathInfo[] _affectedComponents;

        public void Construct(GameManager game, CoreConfig config, Camera cam)
        {
            _game = game;
            _config = config;
            _cam = cam;

            //TEMP prototype solution due to lack of norm ECS
            _affectedComponents = FindObjectsOfType<PathInfo>();

            ColorLineRenderer();
            HandleDrawing();
        }

        private void ColorLineRenderer()
        {
            foreach (var pathInfo in _affectedComponents)
            {
                pathInfo.lineRenderer.material.SetColor(Color, pathInfo.color);
            }
        }

        private void HandleDrawing()
        {
            Observable.EveryUpdate().Subscribe(_ =>
            {
                TrackingStart();
                Tracking();
                TrackingStop();
            }).AddTo(this);
        }

        private void TrackingStart()
        {
            if (!Input.GetMouseButtonDown(0))
            {
                return;
            }

            if (Physics.Raycast(CastRay(), out _hitInfo, Mathf.Infinity, 1 << CashedLayerMasks.ParkingLayer))
            {
                _currentPath = _hitInfo.collider.GetComponentInParent<PathInfo>();
                if (_currentPath == null)
                {
                    Debug.LogError("no parking info on parking parent");
                    return;
                }

                _currentPath.pointsList.Clear();
                var firstPoint = _hitInfo.collider.transform.position;
                firstPoint.y += 0.01f;
                AddPoint(firstPoint);

                _game.DrawStarted.Execute();
                return;
            }

            if (!Physics.Raycast(CastRay(), out _hitInfo, Mathf.Infinity, 1 << CashedLayerMasks.CarLayer))
            {
                return;
            }

            var car = _hitInfo.collider.GetComponent<CarInfo>();
            if (car == null || car.explosiveInfo.isExploded)
            {
                return;
            }

            _currentPath = _hitInfo.collider.GetComponentInParent<PathInfo>();
            if (_currentPath == null)
            {
                Debug.LogError("no parking info on car parent");
                return;
            }

            _game.DrawStarted.Execute();
        }

        private void Tracking()
        {
            if (_currentPath == null || !Input.GetMouseButton(0) ||
                !Physics.Raycast(CastRay(), out _hitInfo, Mathf.Infinity, 1 << CashedLayerMasks.GroundLayer) ||
                Vector3.Distance(_currentPath.pointsList.Last(), _hitInfo.point) < _config.pointsDistance)
            {
                return;
            }

            AddPoint(_hitInfo.point);
        }

        private void TrackingStop()
        {
            if (_currentPath == null || !Input.GetMouseButtonUp(0))
            {
                return;
            }

            _currentPath.PointsUpdated.Execute(_currentPath.pointsList);
            _game.DrawEnded.Execute();
            _currentPath = null;
        }

        private void AddPoint(Vector3 point)
        {
            point.y += 0.01f;
            _currentPath.pointsList.Add(point);
            _currentPath.lineRenderer.positionCount = _currentPath.pointsList.Count;
            _currentPath.lineRenderer.SetPositions(_currentPath.pointsList.ToArray());
        }

        private Ray CastRay() => _cam.ScreenPointToRay(Input.mousePosition);
    }
}