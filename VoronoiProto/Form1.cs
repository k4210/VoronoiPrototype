using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace VoronoiProto
{
    public partial class Form1 : Form
    {
        public List<Vector2> points = new List<Vector2>();
        public Vector2 center;

        public List<PointF> included_points_sorted = new List<PointF>();
        public List<PointF> voronoi_points = new List<PointF>();

        public void Recalculate()
        {
            List<Vector2> local_points = new List<Vector2>(points);
            List<Vector2> local_voronoi_points = MathHelper.CalculateVoronoiPoints(center, local_points);

            voronoi_points.Clear();
            foreach (Vector2 v in local_voronoi_points)
            {
                voronoi_points.Add(new PointF(v.X, v.Y));
            }

            included_points_sorted.Clear();
            foreach (Vector2 v in local_points)
            {
                included_points_sorted.Add(new PointF(v.X, v.Y));
            }
        }

        public enum EState
        {
            Unselected,
            Center,
            Point
        }

        public EState state = EState.Unselected;

        int selected_point = -1;

        public Form1()
        {
            InitializeComponent();
            center = new Vector2(panel1.Width / 2.0f, panel1.Height / 2.0f);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            float point_size = 4.0f;
            e.Graphics.Clear(Color.White);
            e.Graphics.FillRectangle(Brushes.Pink
                , center.X - (point_size / 2.0f)
                , center.Y - (point_size / 2.0f)
                , point_size, point_size);

            foreach (Vector2 p in points)
            {
                e.Graphics.FillRectangle(Brushes.Black
                    , p.X - (point_size / 2.0f)
                    , p.Y - (point_size / 2.0f)
                    , point_size, point_size);
            }
            if (included_points_sorted.Count > 1)
            {
                e.Graphics.DrawLines(Pens.Black, included_points_sorted.ToArray());
                e.Graphics.DrawLine(Pens.Black, included_points_sorted[0], included_points_sorted.LastOrDefault());
            }

            if (voronoi_points.Count > 1)
            {
                e.Graphics.DrawLines(Pens.Green, voronoi_points.ToArray());
                e.Graphics.DrawLine(Pens.Green, voronoi_points[0], voronoi_points.LastOrDefault());
            }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            Vector2 spot = new Vector2(e.X, e.Y);
            float min_distance_sq = 25.0f;
            {
                float dist_Sq = center.DistanceSq(spot);
                if (dist_Sq < min_distance_sq)
                {
                    state = EState.Center;
                    return;
                }
            }

            for (int i = 0; i < points.Count; i++)
            {
                float dist_Sq = spot.DistanceSq(points[i]);
                if (dist_Sq < min_distance_sq)
                {
                    state = EState.Point;
                    selected_point = i;
                    return;
                }
            }

            points.Add(spot);
            Recalculate();
            panel1.Refresh();
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            Vector2 spot = new Vector2(e.X, e.Y);
            if (state == EState.Center)
            {
                center = spot;
            }

            if(state == EState.Point && (selected_point >= 0) && selected_point < points.Count)
            {
                points[selected_point] = spot;
            }

            state = EState.Unselected;
            Recalculate();
            panel1.Refresh();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Delete && points.Count > 0)
            {
                int index = points.Count - 1;
                points.RemoveAt(index);
                state = EState.Unselected;
                Recalculate();
                panel1.Refresh();
            }
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            Vector2 spot = new Vector2(e.X, e.Y);
            if (state == EState.Center)
            {
                center = spot;
            }

            if (state == EState.Point && (selected_point >= 0) && selected_point < points.Count)
            {
                points[selected_point] = spot;
            }

            Recalculate();
            panel1.Refresh();
        }
    }
}
