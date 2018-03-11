using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SpatialSlur .SlurData ;

/*
 * Notes
 */

namespace RC3.Unity.Examples.DendriticGrowth
{
    /// <summary>
    /// Manages the growth process
    /// </summary>
    public partial class DirectedGrowthManager : MonoBehaviour
    {
        [SerializeField] private SharedSelection _sources;
        [SerializeField] private SharedGraph _grid;
        [SerializeField] private Transform _target;

        private Graph _graph;
        private List<VertexObject> _vertices;
        private PriorityQueue<float, int> _queue;


        /// <summary>
        /// 
        /// </summary>
        void Start()
        {
            _graph = _grid.Graph;
            _vertices = _grid.VertexObjects;
            _queue = new PriorityQueue<float, int>();
        }

        /// <summary>
        /// 
        /// </summary>
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                ResetGrowth();

            if (Input.GetKeyDown(KeyCode.C))
                ClearSources();

            UpdateGrowth();
        }

        /// <summary>
        /// 
        /// </summary>
        private void ResetGrowth()
        {

            _queue = new PriorityQueue<float, int>();

            // reset visited vertices
            foreach (var v in _vertices)
            {
                if (v.Status == VertexStatus.Visited)
                    v.Status = VertexStatus.Default;
            }

            // enqueue sources
            foreach (int v in _sources.Indices)
                _queue.Insert(GetKey(v),v);
        }

            float GetKey(int vertex)
            {
                var p0 = _vertices[vertex].transform.position;
                var p1 = _target.position;
                return Vector3.Distance( p0,p1);
            }


            /// <summary>
            /// 
            /// </summary>
            private void ClearSources()
        {  
            foreach (int v in _sources.Indices)
                _vertices[v].Status = VertexStatus.Default;

                _sources.Indices.Clear();
        }


        /// <summary>
        /// 
        /// </summary>
        private void UpdateGrowth()
        {
            if (_queue.Count == 0)
                return;

          /*  foreach (var vi in _graph.GetConnectedVertices(_queue.Dequeue()))
            {
                var v = _vertices[vi];
                if (v.Status != VertexStatus.Default) continue;

                if (CountVisitedNeighbours(vi) == 1)
                {
                    v.Status = VertexStatus.Visited;
                    _queue.Enqueue(vi);
                }
            }*/
        }


        /// <summary>
        /// Returns the number of visited or source neighbours
        /// </summary>
        private int CountVisitedNeighbours(int vertex)
        {
            int count = 0;

            foreach (var vi in _graph.GetConnectedVertices(vertex))
            {
                if (_vertices[vi].Status != VertexStatus.Default)
                    count++;
            }

            return count;
        }
    }
}
