using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SpatialSlur.Core;
using SpatialSlur.Mesh;

namespace RC3.Unity.TetrahedralGrowth
{
    

    /// <summary>
    /// 
    /// </summary>
    public class TerrainHeightEvaluator : FaceEvaluator
    {
        [SerializeField]private Terrain _terrain;
        [SerializeField] private EvaluateType _type;

        [SerializeField]
        [Range(0f,1.0f)]
        private float _aboveTerrain=0.1f;
        [SerializeField]
        [Range(0.3f,1.0f)]
        private float _tightness = 1.0f;
        /// <summary>
        /// 
        /// </summary>
        public override double Evalutate(HeMesh3d.Face face)
        {
            var p0 = face.GetBarycenter();


            switch (_type)
            {
                case EvaluateType.FollowTerrain:
                    return GetFollowTerrain((Vector3)p0);
                case EvaluateType.MinimalHeight:
                    return GetMinimalHeight((Vector3) p0);
                case EvaluateType.MaximalHeight:
                    return GetMaximalHeight((Vector3) p0);

            }
            throw new System.NotImplementedException();
        }

        public double GetFollowTerrain(Vector3 point)
        {
            double dist;

            if (!IsOutOfBound(point))
            {
                dist = Mathf.Abs(_terrain.SampleHeight(point) + (1f - _aboveTerrain) * _terrain.GetPosition().y - _tightness * point.y);
            }
            else
            {
                dist = double.MaxValue;
            }
            return dist;
        }

        public double GetMinimalHeight(Vector3 point)
        {
            double dist;

            if (!IsOutOfBound(point))
            {
                dist = _terrain.SampleHeight(point) + (1f - _aboveTerrain) * _terrain.GetPosition().y +
                       GetFollowTerrain(point);
            }
            else
            {
                dist = double.MaxValue;
            }
            return dist;
        }

        public double GetMaximalHeight(Vector3 point)
        {
            double dist;

            if (!IsOutOfBound(point))
            {
                dist = -(_terrain.SampleHeight(point) + (1f - _aboveTerrain) * _terrain.GetPosition().y) +
                       GetFollowTerrain(point);
            }
            else
            {
                dist = double.MaxValue;
            }
       
            return dist;
        }

        public bool IsOutOfBound(Vector3 point)
        {
            bool _out;

            var boundX = _terrain.terrainData.size.x + _terrain.GetPosition().x;
            var boundZ = _terrain.terrainData.size.z + _terrain.GetPosition().z;

            if (point.x > boundX || point.x < -boundX || point.z > boundZ || point.z < -boundZ)
            {
                _out = true;
            }
            else
            {
                _out = false;
            }
            return _out;
        }

       
    }

    public enum EvaluateType
    {
        MinimalHeight,
        MaximalHeight,
        FollowTerrain,
    }

   

}
