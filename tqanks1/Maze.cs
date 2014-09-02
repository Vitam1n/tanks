using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;

namespace Tanks
{
    public struct MazeCell
    {
        public bool upWall;
        public bool leftWall;
    }

    public class Maze
    {
        public const int wallThickness = 5;
        public MazeCell[,] theMaze { get; private set; }
        public double CellWidth { get; private set; }
        public double CellHeight { get; private set; }
        public int CellCount { get; private set; }
        public List<PointF> walls { get; private set; }

        Point left;
        Point right;

        public int x1 { get; private set; }
        public int y1 { get; private set; }

        public Maze()
        {
            walls = new List<PointF>();
            CreateMaze();
        }

        public void CreateMaze()
        {
            Point p1Coord, p2Coord;
            Random rnd = new Random();
            CellCount = rnd.Next(5, 16);

            CellWidth = ((Form1.ClientWidth - wallThickness) / CellCount) + 1.5;
            CellHeight = (Form1.ClientHeight + wallThickness) / CellCount;

            theMaze = new MazeCell[CellCount, CellCount];

            p1Coord = new Point(rnd.Next(0, CellCount), rnd.Next(0, CellCount));
            p2Coord = new Point(rnd.Next(0, CellCount), rnd.Next(0, CellCount));

            if (p1Coord.X == p2Coord.X && p1Coord.Y == p2Coord.Y)
                p1Coord = new Point(rnd.Next(0, CellCount), rnd.Next(0, CellCount));

            x1 = p1Coord.X * (int)CellWidth;
            y1 = p1Coord.Y * (int)CellHeight;

            left = new Point();
            right = new Point();

            if (p1Coord.X < p2Coord.X)
            {
                left.X = p1Coord.X;
                left.Y = p1Coord.Y;
                right.X = p2Coord.X;
                right.Y = p2Coord.Y;
            }
            else
            {
                left.X = p2Coord.X;
                left.Y = p2Coord.Y;
                right.X = p1Coord.X;
                right.Y = p1Coord.Y;
            }

            for (int i = 0; i < CellCount; i++)
            {
                for (int j = 0; j < CellCount; j++)
                {
                    theMaze[i, j].leftWall = true;
                    theMaze[i, j].upWall = true;
                }
            }

            float x = 0, y = 0;

            for (int i = 0; i < CellCount; i++)
            {
                for (int j = 0; j < CellCount; j++)
                {
                    if (i == CellCount || j == CellCount)
                        continue;
                    if (theMaze[i, j].upWall)
                    {
                        for (float cur = x - 2f; cur <= x + (float)(CellWidth + 1.5); cur++)
                            walls.Add(new PointF(cur, y));
                    }
                    if (theMaze[i, j].leftWall)
                    {
                        for (float cur = x - 2f; cur <= x + (float)(CellWidth + 1.5); cur++)
                            walls.Add(new PointF(cur, y));
                    }
                    x += (float)CellWidth;
                }
                x = 0;
                y += (float)CellHeight;
            }

            Stack points = new Stack();
            points.Push(left);
            MakeWay(points, left, false);

            for (int i = 0; i < CellCount; i++)
            {
                for (int j = 0; j < CellCount; j++)
                {
                    if (rnd.Next(1, 101) % 2 == 0)
                        theMaze[i, j].leftWall = false;
                    if (rnd.Next(1, 101) % 2 == 0)
                        theMaze[i, j].upWall = false;
                }
            }
        }

        public bool MakeWay(Stack points, Point left, bool goback)
        {
            if (points.Count == 0)
                return true;
            if (left.X == right.X && left.Y == right.Y)
            {
                points.Pop(); //убрали текущую ячейку
            }
            if (left.X == right.X && left.Y == right.Y || goback)
            {
                Point prev = (Point)points.Pop();
                if (prev.Y < right.Y)
                    theMaze[left.Y, left.X].upWall = false;
                if (prev.Y > right.Y)
                    theMaze[left.Y + 1, left.X].upWall = false;
                if (prev.X < right.X)
                    theMaze[left.Y, left.X].leftWall = false;
                return MakeWay(points, prev, true);
            }
            else
            {
                if (left.Y < right.Y)
                {
                    left.Y++;
                    points.Push(left);
                    return MakeWay(points, left, false);
                }
                if (left.Y > right.Y)
                {
                    left.Y--;
                    points.Push(left);
                    return MakeWay(points, left, false);
                }
                if (left.X < right.X)
                {
                    left.X++;
                    points.Push(left);
                    return MakeWay(points, left, false);
                }
            }
            return true;
        }

        public void DrawMaze(PaintEventArgs e)
        {
            AddOuterWalls(e);
            float x = 0, y = 0;

            for (int i = 0; i < CellCount; i++)
            {
                for (int j = 0; j < CellCount; j++)
                {
                    if (i == CellCount || j == CellCount)
                        continue;
                    if (theMaze[i, j].upWall)
                        e.Graphics.DrawLine(new Pen(Brushes.Gray, wallThickness), new PointF(x - 2f, y), new PointF(x + (float)(CellWidth + 1.5), y));
                    if (theMaze[i, j].leftWall)
                        e.Graphics.DrawLine(new Pen(Brushes.Gray, wallThickness), new PointF(x, y), new PointF(x, y + (float)(CellHeight + 3)));
                    x += (float)CellWidth;
                }
                x = 0;
                y += (float)CellHeight;
            }
        }

        private void AddOuterWalls(PaintEventArgs e)
        {
            e.Graphics.DrawLine(new Pen(new SolidBrush(Color.Gray), wallThickness), new Point(0, 0), new Point(Form1.FormWidth, 0));
            e.Graphics.DrawLine(new Pen(new SolidBrush(Color.Gray), wallThickness), new Point(0, 0), new Point(0, Form1.FormHeight));
            e.Graphics.DrawLine(new Pen(new SolidBrush(Color.Gray), wallThickness), new Point(0, Form1.FormHeight - 27), new Point(Form1.FormWidth, Form1.FormHeight - 27));
            e.Graphics.DrawLine(new Pen(new SolidBrush(Color.Gray), wallThickness), new Point(Form1.FormWidth - 7, 0), new Point(Form1.FormWidth - 7, Form1.FormHeight));
        }

        //public bool checkAvail()
        //{
        //    int left = p1Coord.X < p2Coord.X ? p1Coord.X : p2Coord.X;
        //    int right = p2Coord.X < p1Coord.X ? p2Coord.X : p1Coord.X;
        //    bool entry = false;

        //    for (int i = left; i < right; i++)
        //    {
        //        if (entry)
        //            break;
        //        if (theMaze[i, left].leftWall == true)
        //        {
        //            entry = true;
        //        }
        //    }
        //    if (entry)
        //        return true;
        //    return false;
        //}
    }
}
