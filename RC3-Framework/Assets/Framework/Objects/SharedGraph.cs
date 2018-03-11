using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Notes
 */

namespace RC3.Unity
{
    /// <summary>
    /// 
    /// </summary>
    public class SharedGraph<V> : ScriptableObject
        where V : VertexObject
    {
        private Graph _graph;
        private List<V> _vertexObjs;
        private List<TensegrityObject> _tensegrityObj;


        /// <summary>
        /// 
        /// </summary>
        public Graph Graph
        {
            get { return _graph; }
        }


        /// <summary>
        /// 
        /// </summary>
        public List<V> VertexObjects
        {
            get { return _vertexObjs; }
        }

        public List<TensegrityObject> TensegrityObjects
        {
            get { return _tensegrityObj; }
        }
        /// <summary>
        /// 
        /// </summary>
        public void Initialize(Graph graph)
        {
            _graph = graph;
            _vertexObjs = new List<V>(_graph.VertexCount);
            _tensegrityObj = new List<TensegrityObject>(_graph.VertexCount);
        }
    }
}