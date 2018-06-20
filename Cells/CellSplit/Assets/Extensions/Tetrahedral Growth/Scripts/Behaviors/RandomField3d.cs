using System;
using System.Collections;
using System.Collections.Generic;
using RC3.Unity.SDFDemo;
using UnityEngine;

using SpatialSlur.Core;
using UnityEngine.Assertions.Comparers;
using Random = UnityEngine.Random;


namespace RC3.Unity.TetrahedralGrowth
{
    /// <summary>
    /// 
    /// </summary>
    public class RandomField3d : ScalarField
    {
        [SerializeField] private float _min;
        [SerializeField] private float _max;

        public override void BeforeEvaluate()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public override float Evaluate(Vector3 point)
        {
            var _value = Random.Range(_min, _max);
            return _value;
        }
    }
}
