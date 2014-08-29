using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Runtime.InteropServices;
using System.Reflection;

//дивжение ?
// координаты для оружие (не в дэд-ендах)

namespace Tanks
{
    public struct MazeCell
    {
        public bool upWall;
        public bool leftWall;
        public PointF start;
        public PointF end;
    }

    public partial class Form1 : Form
    {
        private int x1 = 100, y1 = 10;
        private int x2, y2;
        private const int wallThickness = 5;
        private MazeCell[,] theMaze;
        private double CellWidth;
        private double CellHeight;
        private int CellCount;

        private List<PointF> walls;

        private bool Up, Down, Left, Right;

        private Point p1Coord;
        private Point p2Coord;

        Point right;

        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hWnd);
        [DllImport("gdi32.dll")]
        static extern int GetPixel(IntPtr hDC, int x, int y);
        [DllImport("user32.dll")]
        static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        public Form1()
        {
            InitializeComponent();

            walls = new List<PointF>();
            //p1Coord = new Point(0, 0);
            //p2Coord = new Point(0, 1);

            CreateMaze();
        }

        private void CreateMaze()
        {
            Random rnd = new Random();
            CellCount = rnd.Next(5, 16);

            CellWidth = ((this.Width - wallThickness) / CellCount) + 1.5;
            CellHeight = (this.ClientSize.Height + wallThickness) / CellCount;

            theMaze = new MazeCell[CellCount, CellCount];

            p1Coord = new Point(rnd.Next(0, CellCount), rnd.Next(0, CellCount));
            p2Coord = new Point(rnd.Next(0, CellCount), rnd.Next(0, CellCount));

            if (p1Coord.X == p2Coord.X && p1Coord.Y == p2Coord.Y)
                p1Coord = new Point(rnd.Next(0, CellCount), rnd.Next(0, CellCount));
            //p1Coord.X = 0;
            //p1Coord.Y = 3;
            //p2Coord.X = 3;
            //p2Coord.Y = 4;

            //p1Coord.X = 4; //++
            //p1Coord.Y = 3;
            //p2Coord.X = 2;
            //p2Coord.Y = 1;

            x1 = p1Coord.X * (int)CellWidth;
            y1 = p1Coord.Y * (int)CellHeight;

            Point left = new Point();
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
                    //if(rnd.Next(1, 101) % 2 == 0)
                    theMaze[i, j].leftWall = true;
                    //else
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
                        //    theMaze[i, j].start = new PointF(x - 2f, y);
                        //theMaze[i, j].end = new PointF(x + (float)(CellWidth + 1.5), y);

                        //walls.Add(theMaze[i,j]);
                    }
                    if (theMaze[i, j].leftWall)
                    {
                        for (float cur = x - 2f; cur <= x + (float)(CellWidth + 1.5); cur++)
                            walls.Add(new PointF(cur, y));
                        //theMaze[i, j].start = new PointF(x, y);
                        //theMaze[i, j].end = new PointF(x, y + (float)(CellHeight + 3));
                        //walls.Add(theMaze[i, j]);
                    }
                    x += (float)CellWidth;
                }
                x = 0;
                y += (float)CellHeight;
            }

            Stack points = new Stack();
            points.Push(left);
            MakeWay(points, left, false);

            //else
            //{
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
            //}

            //if (!checkAvail())
            //{
            //    for (int i = 0; i < CellCount; i++)
            //    {
            //        if (rnd.Next(1, 101) % 2 == 0)
            //            theMaze[i, p1Coord.X].leftWall = false;
            //    }
            //}
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

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.ResetTransform();
            DrawMaze(e);

            Size size = new Size();
            if (CellWidth > CellHeight)
            {
                size.Height = (int)CellWidth - 5;
                size.Width = (int)CellHeight - 5;
            }
            else
            {
                size.Height = (int)CellHeight - 5;
                size.Width = (int)CellWidth - 5;
            }
            //Bitmap t1 = new Bitmap(Properties.Resources.lt3, size);
            Bitmap t1 = new Bitmap(Properties.Resources.bm, size);
            if (CellWidth > CellHeight)
                t1.RotateFlip(RotateFlipType.Rotate90FlipX);
            //if (Left)
            //{
            //    if (x1 > 0)
            //    {
            //        //ход мысли правилен именно на этом участке кода; суть такова: нарисовать ровный лабиринт и повернутый танк
            //        //сначала вычисляем трансформации для танка, рисуем танк; затем сбрасываем все трансформации и рисуем ровный
            //        //лабиринт. дело только за реализацие.
            //        //p.s. todo: почистить код (хотя бы отвечающий за рисование)
            //        x1--;
            //        //Graphics g = e.Graphics;
            //        //g.RotateTransform(1);
            //        //g.DrawImage(t1, new Point(x1, y1));
            //        //g.ResetTransform();
            //        //DrawMaze(e);
            //    }
            //    else
            //        x1 = 0;
            //}
            //if (Right)
            //{
            //    if (x1 < this.ClientSize.Width)
            //        x1++;
            //    else
            //        x1 = this.ClientSize.Width;
            //    //int w;
            //    //if (CellWidth > CellHeight)
            //    //    w = size.Height;
            //    //else
            //    //    w = size.Width;
            //    //if (x1 <= this.ClientRectangle.Width - w + 8)
            //    //    x1++;
            //}
            //if (Up)
            //{
            //    if (y1 > 0)
            //        y1--;
            //    else
            //        y1 = 0;
            //}
            //if (Down)
            //{
            //    //int h;
            //    //if (CellWidth > CellHeight)
            //    //    h = size.Width;
            //    //else
            //    //    h = size.Height;
            //    //if (y1 < this.ClientRectangle.Height - h)
            //    //    y1++;
            //    if (y1 < this.ClientSize.Height)
            //        y1++;
            //    else
            //        y1 = this.ClientSize.Height;
            //}

            //foreach (PointF m in walls)
            //{
                //if (x1 >= m.start.X && x1 <= m.end.X && y1 >= m.start.Y && y1 <= m.end.Y)
                //if (x1 == m.X && y1 == m.Y)
                //{
                    //MessageBox.Show("collision detected!");
                    
                    Color color1 = Color.Empty;
            Color color2 = Color.Empty;
            Color color3 = Color.Empty;
            Color color4 = Color.Empty;

                    IntPtr hDC = GetDC(this.Handle);
                    int colorRef1 = GetPixel(hDC, x1, y1);

                    int colorRef2 = GetPixel(hDC, x1+t1.Width, y1);
                    int colorRef3 = GetPixel(hDC, x1, y1+t1.Height);
                    int colorRef4 = GetPixel(hDC, x1+t1.Width, y1+t1.Height);

                    color1 = Color.FromArgb(
                    (int)(colorRef1 & 0x000000FF),
                    (int)(colorRef1 & 0x0000FF00) >> 8,
                    (int)(colorRef1 & 0x00FF0000) >> 16);
                    ReleaseDC(this.Handle, hDC);

                    color2 = Color.FromArgb(
                            (int)(colorRef2 & 0x000000FF),
                            (int)(colorRef2 & 0x0000FF00) >> 8,
                            (int)(colorRef2 & 0x00FF0000) >> 16);
                    ReleaseDC(this.Handle, hDC);
                    
            color3 = Color.FromArgb(
                            (int)(colorRef3 & 0x000000FF),
                            (int)(colorRef3 & 0x0000FF00) >> 8,
                            (int)(colorRef3 & 0x00FF0000) >> 16);
                    ReleaseDC(this.Handle, hDC);
                    
            color4 = Color.FromArgb(
                            (int)(colorRef4 & 0x000000FF),
                            (int)(colorRef4 & 0x0000FF00) >> 8,
                            (int)(colorRef4 & 0x00FF0000) >> 16);
                    ReleaseDC(this.Handle, hDC);
                    if (GetColorName(color1) == Color.Gray.Name || GetColorName(color2) == Color.Gray.Name
                        || GetColorName(color3) == Color.Gray.Name || GetColorName(color4) == Color.Gray.Name)
            {
                        this.Text = "yes!";
            }
                    else
                        this.Text = "Tanks";
            //this.Text = color.ToString();
                    //if (color == Color.Gray)
                      //  this.Text = "yes";
            //this.Text = GetColorName(color).ToString();
                //}
                //else
                //    this.Text = "ok";
            //}


            //Image tank = Properties.Resources.lt3;
            //Image tank = Bitmap.FromFile("Imgs\\lt3.png");
            //e.Graphics.DrawImage(tank, x1, y1);

            //DrawMaze(e);
            //var gr = e.Graphics.BeginContainer();
            
            //e.Graphics.DrawImage(t1, p1Coord.X * (int)CellWidth, p1Coord.Y * (int)CellHeight);
            //e.Graphics.EndContainer(gr);
            //e.Graphics.DrawImage(t1, p1Coord.X * (int)CellWidth, p1Coord.Y * (int)CellHeight);
            e.Graphics.DrawImage(t1, x1, y1);
            //if (CellWidth > CellHeight)
            //    e.Graphics.RotateTransform(90);

            //Point[] p1 = new Point[] { new Point(p1Coord.X * (int)CellWidth , p1Coord.Y * (int)CellHeight),
            //                            new Point(p1Coord.X * (int)CellWidth, p1Coord.Y * (int)CellHeight + (int)CellHeight-10),
            //                            new Point(p1Coord.X * (int)CellWidth + (int)CellWidth, p1Coord.Y * (int)CellHeight + (int)CellHeight-10),
            //                            new Point(p1Coord.X * (int)CellWidth + (int)CellWidth, p1Coord.Y * (int)CellHeight)
            //                            };
            //Point[] p1 = new Point[] { new Point(p1Coord.X * (int)CellWidth , p1Coord.Y * (int)CellHeight),
            //                            new Point(p1Coord.X * (int)CellWidth, p1Coord.Y * (int)CellHeight + (int)CellHeight-10),
            //                            new Point(p1Coord.X * (int)CellWidth+(int)CellWidth, p1Coord.Y * (int)CellHeight + (int)CellHeight-10),
            //                            new Point(p1Coord.X * (int)CellWidth + (int)CellWidth, p1Coord.Y * (int)CellHeight)
            //                            };

            //Point[] p2 = new Point[] { new Point(p2Coord.X * (int)CellWidth , p2Coord.Y * (int)CellHeight),
            //                            new Point(p2Coord.X * (int)CellWidth, p2Coord.Y * (int)CellHeight + (int)CellHeight-10),
            //                            new Point(p2Coord.X * (int)CellWidth+(int)CellWidth, p2Coord.Y * (int)CellHeight + (int)CellHeight-10),
            //                            new Point(p2Coord.X * (int)CellWidth + (int)CellWidth, p2Coord.Y * (int)CellHeight)
            //                            };
            //e.Graphics.FillPolygon(Brushes.Red, p1);
            //e.Graphics.FillPolygon(Brushes.Azure, p2);
            //e.Graphics.DrawLine(new Pen(Brushes.DarkRed, 3), new Point((p1Coord.X - 5) * (int)CellWidth, (p1Coord.Y * (int)CellHeight) - 3), new Point((p1Coord.X - 5) * (int)CellWidth, (p1Coord.Y * (int)CellHeight) - 10));
            //e.Graphics.DrawLine(new Pen(Brushes.DarkGreen, 3), new Point((p2Coord.X - 5) * (int)CellWidth, (p2Coord.Y * (int)CellHeight) - 3), new Point((p2Coord.X - 5) * (int)CellWidth, (p2Coord.Y * (int)CellHeight) - 10));
        }

        //http://stackoverflow.com/questions/10875874/c-sharp-color-name-from-argb
        Dictionary<String, Color> colors = typeof(Color).GetProperties(BindingFlags.Static | BindingFlags.Public).ToDictionary(p => p.Name, p => (Color)p.GetValue(null, null));
        public String GetColorName(Color color)
        {
            return colors.Where(c => c.Value.A == color.A && c.Value.R == color.R && c.Value.G == color.G && c.Value.B == color.B).FirstOrDefault().Key;
        }

        private void DrawMaze(PaintEventArgs e)
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
            e.Graphics.DrawLine(new Pen(new SolidBrush(Color.Gray), wallThickness), new Point(0, 0), new Point(this.Width, 0));
            e.Graphics.DrawLine(new Pen(new SolidBrush(Color.Gray), wallThickness), new Point(0, 0), new Point(0, this.Height));
            e.Graphics.DrawLine(new Pen(new SolidBrush(Color.Gray), wallThickness), new Point(0, this.Height - 27), new Point(this.Width, this.Height - 27));
            e.Graphics.DrawLine(new Pen(new SolidBrush(Color.Gray), wallThickness), new Point(this.Width - 7, 0), new Point(this.Width - 7, this.Height));
        }

        private void Shoot()
        {
 
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.A:
                case Keys.Left:
                    //if (p1Coord.X > 0)
                    //    p1Coord.X--;
                    //else
                    //    p1Coord.X = 0;
                    //if (x1 > 0)
                    //    x1--;
                    //else
                    //    x1 = 0;
                    Left = true;
                    break;
                case Keys.D:
                case Keys.Right:
                    //if (x1 <= this.ClientRectangle.Width)
                    //    x1++;
                    //else
                    //    x1 = this.ClientRectangle.Width;
                    //break;
                    //if (p1Coord.X < CellCount-1)
                    //    p1Coord.X++;
                    //else
                    //    p1Coord.X = CellCount-1;
                    //break;
                    Right = true;
                    break;
                case Keys.W:
                case Keys.Up:
                    //if (y1 > 0)
                    //    y1--;
                    //else
                    //    y1 = 0;
                    Up = true;
                    break;
                case Keys.S:
                case Keys.Down:
                    //if (y1 < this.Height - Properties.Resources.lt3.Height)
                    //    y1++;
                    //else
                    //    y1 = this.Height - Properties.Resources.lt3.Height;
                    Down = true;
                    break;
                case Keys.Control:
                    Shoot();
                    break;
            }

            if (Left)
            {
                if (x1 > 0)
                {
                    //ход мысли правилен именно на этом участке кода; суть такова: нарисовать ровный лабиринт и повернутый танк
                    //сначала вычисляем трансформации для танка, рисуем танк; затем сбрасываем все трансформации и рисуем ровный
                    //лабиринт. дело только за реализацие.
                    //p.s. todo: почистить код (хотя бы отвечающий за рисование)
                    x1--;
                    //Graphics g = e.Graphics;
                    //g.RotateTransform(1);
                    //g.DrawImage(t1, new Point(x1, y1));
                    //g.ResetTransform();
                    //DrawMaze(e);
                }
                else
                    x1 = 0;
            }
            if (Right)
            {
                if (x1 < this.ClientSize.Width)
                    x1++;
                else
                    x1 = this.ClientSize.Width;
                //int w;
                //if (CellWidth > CellHeight)
                //    w = size.Height;
                //else
                //    w = size.Width;
                //if (x1 <= this.ClientRectangle.Width - w + 8)
                //    x1++;
            }
            if (Up)
            {
                if (y1 > 0)
                    y1--;
                else
                    y1 = 0;
            }
            if (Down)
            {
                //int h;
                //if (CellWidth > CellHeight)
                //    h = size.Width;
                //else
                //    h = size.Height;
                //if (y1 < this.ClientRectangle.Height - h)
                //    y1++;
                if (y1 < this.ClientSize.Height)
                    y1++;
                else
                    y1 = this.ClientSize.Height;
            }
            
            this.Invalidate();
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.A:
                case Keys.Left:
                    Left = false;
                    break;
                case Keys.D:
                case Keys.Right:
                    Right = false;
                    break;
                case Keys.W:
                case Keys.Up:
                    Up = false;
                    break;
                case Keys.S:
                case Keys.Down:
                    Down = false;
                    break;
            }
        }
    }
}
