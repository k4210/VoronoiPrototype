using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoronoiProto
{
    /*
    class QuadTreeBaseNode
    {
        uint level;
        uint position; //each 2 bits tells about position on a given level
    }

    class QuadTreeNode : QuadTreeBaseNode
    {
        int[] children = new int[4];
    }

    class QuadTreeLeaf : QuadTreeBaseNode
    {
        int first_point_index;
        int afer_last_point_index;
    }
    */
    class QuadTree
    {
        //List<QuadTreeBaseNode> nodes;

        public List<Vector2> sorted_points = new List<Vector2>();

        static int ZCurveIndex(int x, int y, int resolution_depth/* resolution = 2^resolution_depth */)
        {
            int index = 0;
            for (int k = 0; k < resolution_depth; k++)
            {
                int mask = 1 << k;
                index |= (x & mask) << k;
                index |= (y & mask) << (k + 1);
            }
            return index;
        }

        public void SetPoint(List<Vector2> unsorted_points, int resolution_depth /* resolution = 2^resolution_depth */)
        {
            //nodes.Clear();
            sorted_points.Clear();

            sorted_points = new List<Vector2>(unsorted_points);
            sorted_points.Sort(delegate (Vector2 a, Vector2 b)
            {
                int max_resolution = 1 << resolution_depth;
                int index_a = ZCurveIndex((int)(a.X), (int)(a.Y), resolution_depth);
                int index_b = ZCurveIndex((int)(b.X), (int)(b.Y), resolution_depth);
                return index_a.CompareTo(index_b);
            });
        }
    }
}
