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
    public class SharedEdgeGraph<V, E,TV,TE,TT> : ScriptableObject
        where V : VertexObject
        where E : EdgeObject
        where TV : TensileVertex
        where TE: TensileEdge
        where TT: TensileTriangle
    {
        private EdgeGraph _graph;
        private List<V> _vertexObjs;
        private List<E> _edgeObjs;
        private List<TV> _tensileVertexObj;
        private List<TE> _tensileEdgeObj;
        private List<TT> _tensileTriangle;

        /// <summary>
        /// 
        /// </summary>
        public EdgeGraph edgeGraph
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

        public List<TV> TensileVertexObjects
        {
            get { return _tensileVertexObj; }
        }

        /// <summary>
        /// 
        /// </summary>
        public List<E> EdgeObjects
        {
            get { return _edgeObjs; }
        }


        public List<TE> TensileEdgeObjects
        {
            get { return _tensileEdgeObj; }
        }

        public List<TT> TensileTriangle
        {
            get { return _tensileTriangle; }
        }


        /// <summary>
        /// 
        /// </summary>
        public void Initialize(EdgeGraph graph)
        {
            _graph = graph;
            _vertexObjs = new List<V>(_graph.VertexCount);
            _edgeObjs = new List<E>(_graph.EdgeCount);
            _tensileEdgeObj=new List<TE>(_graph.EdgeCount);
            _tensileVertexObj=new List<TV>(_graph.VertexCount);
            _tensileTriangle=new List<TT>();
        }
    }
}