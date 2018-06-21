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
    public class TerrainEvaluator : FaceEvaluator
    {
        [SerializeField]private Terrain _terrain;

        /// <summary>
        /// 
        /// </summary>
        public override double Evalutate(HeMesh3d.Face face)
        {
            var p0 = face.GetBarycenter();
         
            var th = GetToTerrainDistance((Vector3) p0);

            return th;
        }

        public double GetToTerrainDistance(Vector3 point)
        {
            double dist;
            RaycastHit hit;
            Ray ray=new Ray(point,Vector3.down);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == _terrain.transform)
                {
                    dist = hit.distance;
                }
                else
                {
                    dist = double.MaxValue;
                }
            }
            else
            {
                dist = double.MaxValue;
            }



            return dist;
        }
    }
}
