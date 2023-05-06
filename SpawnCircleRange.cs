using System.Collections.Generic;
using UnityEngine;

namespace Models.Common
{
    public interface ISpawnCircleRangeHelper<T>
    {
        void DestroyT(T go);
        T OnInstantiate(Vector3 position, Quaternion quaternion);
    }
    public class SpawnCircleRange<T>
    {
        private float _agentRadius;
        private Vector3 _originPoint;
        private List<Vector3> _freePosList = new List<Vector3>();
        private List<T> _spawnedCircles = new List<T>();
        private ISpawnCircleRangeHelper<T> _spawnCircleRangeHelper;
        public IReadOnlyList<T> SpawnedCircles => _spawnedCircles;

        public void Init(ISpawnCircleRangeHelper<T> spawnCircleRangeHelper, float agentRadius, Vector3 originPoint)
        {
            _spawnCircleRangeHelper = spawnCircleRangeHelper;
            _agentRadius = agentRadius;
            _originPoint = originPoint;
        }

        public void OnDisable()
        {
            ClearCircles();
            _freePosList.Clear();
        }

        public void InstantiateCircleWithEuler(float radius)
        {
            FindPlaceWIthCircle(radius);
            ClearCircles();
            float eulerStep = 360f / _freePosList.Count;
            
            for (int i = 0; i < _freePosList.Count; i++)
            {
                _spawnedCircles.Add(_spawnCircleRangeHelper.OnInstantiate(_freePosList[i], Quaternion.Euler(new Vector3(0f, 0f, eulerStep * i))));
            }
            _freePosList.Clear();
        }
        
        public void InstantiateCircle(float radius)
        {
            FindPlaceWIthCircle(radius);
            ClearCircles();
            foreach (var position in _freePosList)
            {
                _spawnedCircles.Add(_spawnCircleRangeHelper.OnInstantiate(position, Quaternion.identity));
            }
            _freePosList.Clear();
        }

        private void FindPlaceWIthCircle(float radius)
        {
            Vector3 point = _originPoint;
            float distRing = 2 * 3.14f * radius;

            var angle = 360 * Mathf.Deg2Rad;

            var possibleCount = distRing / _agentRadius;
            for (int i = 1; i <= possibleCount; i++)
            {
                point.y = _originPoint.y + Mathf.Cos(angle / possibleCount * i) * radius;
                point.x = _originPoint.x + Mathf.Sin(angle / possibleCount * i) * radius;
                
                _freePosList.Add(point);
            }
        }
        
        private void ClearCircles()
        {
            foreach (var spawnedCircle in _spawnedCircles)
            {
                _spawnCircleRangeHelper.DestroyT(spawnedCircle);
            }
            
            _spawnedCircles.Clear();
        }
    }
}