
using System;
using System.Collections.Generic;

public struct Vector2
{
    public float X;
    public float Y;

    public Vector2(float x, float y)
    {
        X = x;
        Y = y;
    }
    public Vector2(System.Drawing.Point p)
    {
        X = p.X;
        Y = p.Y;
    }

    public System.Drawing.Point ToPoint()
    {
        return new System.Drawing.Point((int)X, (int)Y);
    }

    public static Vector2 operator *(float f, Vector2 vec)
    {
        return vec * f;
    }
    public static Vector2 operator *(Vector2 vec, float f)
    {
        return new Vector2(vec.X * f, vec.Y * f);
    }
    public static Vector2 operator +(Vector2 v1, Vector2 v2)
    {
        return new Vector2(v1.X + v2.X, v1.Y + v2.Y);
    }
    public static Vector2 operator -(Vector2 v1, Vector2 v2)
    {
        return new Vector2(v1.X - v2.X, v1.Y - v2.Y);
    }

    public static bool Equal(Vector2 v1, Vector2 v2)
    {
        return v1.X == v2.X && v1.Y == v2.Y;
    }

    public override string ToString()
    {
        return " {X: " + X.ToString() + " \tY: " + Y.ToString() + " }";
    }

    public float DistanceSq(Vector2 v)
    {
        return ((X - v.X) * (X - v.X)) + ((Y - v.Y) * (Y - v.Y));
    }

    public float Dot(Vector2 v)
    {
        return X * v.X + Y * v.Y;
    }

    public void Normalize()
    {
        float length = (float)Math.Sqrt(Dot(this));
        this *= (1.0f / length);
    }
}

public struct MathHelper
{
    public static bool TwoLinesIntersection(Vector2 v0, Vector2 v1, Vector2 w0, Vector2 w1, out Vector2 out_r)
    {
        out_r = new Vector2();
        Vector2 v_diff = v0 - v1;
        Vector2 w_diff = w0 - w1;
        if (v_diff.X == 0.0f)
        {
            if (w_diff.X == 0.0f)
            {
                return false;
            }
            out_r.X = v0.X;
            float aw = w_diff.Y / w_diff.X;
            float bw = w0.Y - aw * w0.X;
            out_r.Y = aw * out_r.X + bw;
            return true;
        }

        if (w_diff.X == 0.0f)
        {
            out_r.X = w0.X;
            float av = v_diff.Y / v_diff.X;
            float bv = v0.Y - av * v0.X;
            out_r.Y = av * out_r.X + bv;
            return true;
        }

        {
            float av = v_diff.Y / v_diff.X;
            float bv = v0.Y - av * v0.X;

            float aw = w_diff.Y / w_diff.X;
            float bw = w0.Y - aw * w0.X;

            if (av == aw)
            {
                return false;
            }

            out_r.X = (bw - bv) / (av - aw);
            out_r.Y = aw * out_r.X + bw;
        }
        return true;
    }

    public static bool ClockwiseLess(Vector2 a, Vector2 b, Vector2 center)
    {
        if (a.X - center.X >= 0 && b.X - center.X < 0)
            return true;
        if (a.X - center.X < 0 && b.X - center.X >= 0)
            return false;
        if (a.X - center.X == 0 && b.X - center.X == 0)
        {
            if (a.Y - center.Y >= 0 || b.Y - center.Y >= 0)
                return a.Y > b.Y;
            return b.Y > a.Y;
        }

        // compute the cross product of vectors (center -> a) x (center -> b)
        float det = (a.X - center.X) * (b.Y - center.Y) - (b.X - center.X) * (a.Y - center.Y);
        if (det < 0)
            return true;
        if (det > 0)
            return false;

        // points a and b are on the same line from the center
        // check which point is closer to the center
        return a.DistanceSq(center) < b.DistanceSq(center);
    }

    public static void ClockwiseBubbleSort(List<Vector2> p, Vector2 center)
    {
        for (int i = 1; i < p.Count; i++)
        {
            int j = i;
            while ((j > 0) && ClockwiseLess(p[j - 1], p[j], center))
            {
                Vector2 temp = p[j - 1];
                p[j - 1] = p[j];
                p[j] = temp;
                j--;
            }
        }
    }

    public static Vector2 FindClosestPoint(List<Vector2> local_points, Vector2 center)
    {
        Vector2 closest = local_points[0];
        float best_distance_sq = center.DistanceSq(local_points[0]);
        for (int i = 1; i < local_points.Count; i++)
        {
            float distance_sq = center.DistanceSq(local_points[i]);
            if (distance_sq < best_distance_sq)
            {
                best_distance_sq = distance_sq;
                closest = local_points[i];
            }
        }

        return closest;
    }

    public static List<Vector2> CalculateVoronoiPoints(Vector2 center, List<Vector2> local_points)
    {
        List<Vector2> voronoi_points = new List<Vector2>();
        if (local_points.Count > 2)
        {
            ClockwiseBubbleSort(local_points, center);
            Vector2 closest = FindClosestPoint(local_points, center);

            for (int i = 0; i < local_points.Count;)
            {
                int closest_index = local_points.IndexOf(closest);
                int index1 = (i + closest_index) % local_points.Count;
                int index2 = (i + closest_index + 1) % local_points.Count;
                Circle c = Circle.FromPoints(center, local_points[index1], local_points[index2]);
                if ((!c.valid) || c.ContainsPoint(local_points, index1, index2))
                {
                    if (index2 == closest_index)
                    {
                        local_points.RemoveAt(index1);
                        voronoi_points.RemoveAt(voronoi_points.Count - 1);
                        i--;
                    }
                    else
                    {
                        local_points.RemoveAt(index2);
                    }
                }
                else
                {
                    voronoi_points.Add(c.center);
                    i++;
                }
            }
        }
        return voronoi_points;
    }
}

public struct Circle
{
    public Vector2 center;
    public float radiusSQ;
    public bool valid;

    public bool IsPointInside(Vector2 p)
    {
        return p.DistanceSq(center) < radiusSQ;
    }

    public static Circle FromPoints(Vector2 p0, Vector2 p1, Vector2 p2)
    {
        Vector2 v1 = p1 - p0;
        Vector2 q1 = p0 + (v1 * 0.5f);
        v1 = new Vector2(-v1.Y, v1.X);

        Vector2 v2 = p1 - p2;
        Vector2 q2 = p2 + (v2 * 0.5f);
        v2 = new Vector2(-v2.Y, v2.X);

        Circle c = new Circle();
        c.valid = MathHelper.TwoLinesIntersection(q1, v1 + q1, q2, v2 + q2, out c.center);
        c.radiusSQ = c.valid ? p1.DistanceSq(c.center) : 0.0f;

        return c;
    }

    public bool ContainsPoint(List<Vector2> points, int exclude1, int exclude2)
    {
        for (int i = 0; i < points.Count; i++)
        {
            if (i != exclude1 && i != exclude2 && IsPointInside(points[i]))
            {
                return true;
            }
        }
        return false;
    }
};