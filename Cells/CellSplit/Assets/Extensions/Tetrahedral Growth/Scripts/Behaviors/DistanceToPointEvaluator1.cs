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
    public class DistanceToPointEvaluator : FaceEvaluator
    {
        [SerializeField]private Texture2D image;

        [SerializeField] private int radius = 100;

        [SerializeField] private float _maxHeight = 20f;
        [SerializeField] private float _minHeight = -20f;
        /// <summary>
        /// 
        /// </summary>
        public override double Evalutate(HeMesh3d.Face face)
        {
            var p0 = face.GetBarycenter();
            //var p1 = (Vec3d)transform.position;

         

            //return p0.DistanceTo(p1);

            var gs = RemapTextureGrayScale(p0.X, p0.Z);

         

            var gt = Mathf.InverseLerp(0, 1f, (float)gs);
            var ht = Mathf.InverseLerp(_minHeight, _maxHeight, (float) p0.Y);

            double value;

            if (gt == 1 || gt == 0 || ht ==1 || ht == 0)
            {
                value= double.MaxValue;
            }
            else
            {
                value= Mathf.Abs(gt - ht);
            }

            //print(gs+","+gt+','+ht+","+value);

            return value;
        }

        public double RemapTextureGrayScale(double x,double z)
        {

            var h = image.height;
            var w = image.width;
            var t = h / w;

            var rh = radius * t;
            var rw = radius;


            if (x > rw||x<0 || z > rh||z<0)
            {
                return double.MaxValue;
            }
            else
            {
                var tx = Mathf.InverseLerp(0, rw, (float)x);
                var tz = Mathf.InverseLerp(0, rh, (float)z);

                var vx = Mathf.FloorToInt(Mathf.Lerp(0, w, tx));
                var vz = Mathf.FloorToInt(Mathf.Lerp(0, h, tz));

                return image.GetPixel(vx, vz).grayscale;
            }
        }
    }
}
