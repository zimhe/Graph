using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * Notes
 */  

namespace RC3
{
    /// <summary>
    /// Contains static methods for creating instances of EdgeGraph
    /// </summary>
    public abstract class GraphFactoryBase<G>
        where G : IGraph
    {
        /// <summary>
        /// 
        /// </summary>
        protected abstract G Create();


        /// <summary>
        /// 
        /// </summary>
        public G CreateGrid(int countX, int countY)
        {
            var g = Create();
            int n = countX * countY;

            // add vertices
            for (int i = 0; i < n; i++)
                g.AddVertex();

            // add edges
            for (int y = 0; y < countY; y++)
            {
                for (int x = 0; x < countX; x++)
                {
                    int i = x + y * countX;
                    if (x > 0) g.AddEdge(i, i - 1); // x-1
                    if (y > 0) g.AddEdge(i, i - countX); // y-1
                }
            }

            return g;
        }
        public G CreateCubicGrid(int countX, int countY,int countZ)
        {
            var g = Create();
            int n = countX * countY*countZ;

            // add vertices
            for (int i = 0; i < n; i++)
                g.AddVertex();
            // add edges
            for (int z = 0; z < countZ; z++)
            {
                for (int y = 0; y < countY; y++)
                {
                    for (int x = 0; x < countX; x++)
                    {
                        int i = x + y * countX + z * countX * countY;
                        if (x > 0) g.AddEdge(i, i - 1); // x-1
                        if (y > 0) g.AddEdge(i, i - countX); // y-1
                        if (z > 0) g.AddEdge(i, i - countX * countY);//z-1
                    }
                }
            }
            return g;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <returns></returns>
        public G CreateTriangleGrid(int countX, int countY)
        {
            var g = Create();
            int n = countX * countY;

            // add vertices
            for (int i = 0; i < n; i++)
                g.AddVertex();

            // add even row edges
            for (int y = 0; y < countY; y += 2)
            {
                for (int x = 0; x < countX; x++)
                {
                    int i = x + y * countX;
                    if (x > 0) g.AddEdge(i, i - 1); // x-1
                    if (y > 0) g.AddEdge(i, i - countX); // y-1
                    if (y > 0 && x > 0) g.AddEdge(i, i - countX - 1);// y-1, x-1
                }
            }

            // add odd row edges
            for (int y = 1; y < countY; y += 2)
            {
                for (int x = 0; x < countX; x++)
                {
                    int i = x + y * countX;
                    if (x > 0) g.AddEdge(i, i - 1); // x-1
                    if (y > 0) g.AddEdge(i, i - countX); // y-1
                    if (y > 0 && x < countX - 1) g.AddEdge(i, i - countX + 1); // y-1, x+1
                }
            }

            return g;
        }
        public G CreateTriangleGrid3D(int countX, int countZ,int countY)
        {
            var g = Create();
            int n = countX * countZ * countY;

            // add vertices
            for (int i = 0; i < n; i++)
                g.AddVertex();


            for (int y = 0; y < countY; y++)
            {
                // add even row edges
                for (int z = 0; z < countZ; z += 2)
                {
                    for (int x = 0; x < countX; x++)
                    {
                        int i = x + z * countX+y*countX*countZ;
                        if (x > 0) g.AddEdge(i, i - 1); // x-1
                        if (z > 0 && x > 0) g.AddEdge(i, i - countX - 1);// z-1, x-1
                        if (z > 0) g.AddEdge(i, i - countX); // z-1
                        if(y>0) g.AddEdge(i,i-countX*countZ);//y-1
                    }
                }

                // add odd row edges
                for (int z = 1; z < countZ; z += 2)
                {
                    for (int x = 0; x < countX; x++)
                    {
                        int i = x + z * countX+y * countX * countZ; ;
                        if (x > 0) g.AddEdge(i, i - 1); // x-1
                        if (z > 0) g.AddEdge(i, i - countX); // z-1
                        if (z > 0 && x < countX - 1) g.AddEdge(i, i - countX + 1); // y-1, x+1
                        if (y>0) g.AddEdge(i,i-countX*countZ);//y-1
                    }
                }
            }

            return g;
        }
        /// <summary>
        /// Create a grid in a boundry of a triangle
        /// </summary>
        /// <param name="SegmentCount"></param>
        /// <param name="LayerCount"></param>
        /// <returns></returns>
        public G CreateDecreasingTriangleGrid(int SegmentCount,  int LayerCount)
        {
            var g = Create();

            int VertextIndex = 0;

            for (int y = 0; y < LayerCount; y++)
            {
                for (int z = 0; z < SegmentCount; z++)
                {
                    for (int x = 0; x < SegmentCount - z; x++)
                    {
                       
                        g.AddVertex();
                      
                    }
                }
            }
            for (int y = 0; y < LayerCount; y++)
            {
                for (int z = 0; z < SegmentCount; z++)
                {
                    for (int x = 0; x < SegmentCount - z; x++)
                    {
                        VertextIndex++;
                      
                        var layerVertexCount = y * ((SegmentCount * SegmentCount - SegmentCount) / 2 + SegmentCount);

                      
                        var row = SegmentCount - z + 1;

                        int i = VertextIndex - 1;

                        if (x > 0) g.AddEdge(i, i - 1); // x-1
                        if (z > 0) g.AddEdge(i, i - row); // z-1
                        if (z > 0 && x < SegmentCount - 1) g.AddEdge(i, i - row + 1); // z-1, x+1
                        if (y > 0) g.AddEdge(i, i - layerVertexCount);//y-1

                    }
                }
            }


            return g;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="countZ"></param>
        /// <returns></returns>
        public G CreateTruncatedOctahedronGrid(int countX, int countY, int countZ)
        {
            var g = Create();
            int countXY = countX * countY;
            int count = countXY * countZ;

            // add vertices
            for (int i = 0; i < count; i++)
            {
                g.AddVertex();
                g.AddVertex();
            }

            // add primal edges
            for (int z = 0; z < countZ; z++)
            {
                for (int y = 0; y < countY; y++)
                {
                    for (int x = 0; x < countX; x++)
                    {
                        int i = x + y * countX + z * countXY;
                        int j = i + count;

                        // primal to primal
                        if (x > 0) g.AddEdge(i, i - 1); // x-1
                        if (y > 0) g.AddEdge(i, i - countX); // y-1
                        if (z > 0) g.AddEdge(i, i - countXY); // z-1

                        // dual to dual
                        if (z > 0) g.AddEdge(i, j - countXY); // z-1
                        if (x > 0 && z > 0) g.AddEdge(i, j - countXY - 1); // x-1,z-1
                        if (y > 0 && z > 0) g.AddEdge(i, j - countXY - countX); // y-1,z-1
                        if (x > 0 && y > 0 && z > 0) g.AddEdge(i, j - countXY - countX - 1); // x-1,y-1,z-1
                    }
                }
            }

            // add dual edges
            for (int z = 0; z < countZ; z++)
            {
                for (int y = 0; y < countY; y++)
                {
                    for (int x = 0; x < countX; x++)
                    {
                        int i = x + y * countX + z * countXY;
                        int j = i + count;

                        // dual to dual
                        if (x > 0) g.AddEdge(j, j - 1); // x-1
                        if (y > 0) g.AddEdge(j, j - countX); // y-1
                        if (z > 0) g.AddEdge(j, j - countXY); // z-1

                        // dual to primal
                        g.AddEdge(j, i);
                        if (x < countX - 1) g.AddEdge(j, i + 1); // x+1
                        if (y < countY - 1) g.AddEdge(j, i + countX); // y+1
                        if (x < countY - 1 && y < countY - 1) g.AddEdge(j, i + countX + 1); // x+1,y+1
                    }
                }
            }

            return g;
        }
    }
}