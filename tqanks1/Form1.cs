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
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hWnd);
        [DllImport("gdi32.dll")]
        static extern int GetPixel(IntPtr hDC, int x, int y);
        [DllImport("user32.dll")]
        static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        //private int x2, y2;

        public static int ClientWidth;
        public static int ClientHeight;
        public static int FormWidth;
        public static int FormHeight;

        Tank tank;
        Maze theMaze;

        public Form1()
        {
            InitializeComponent();
            ClientWidth = this.ClientSize.Width; //this.Width
            ClientHeight = this.ClientSize.Height;

            FormWidth = this.Width;
            FormHeight = this.Height;

            theMaze = new Maze();
            tank = new Tank(theMaze.x1, theMaze.y1, theMaze.CellWidth, theMaze.CellHeight);
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            //вычислить координатную сетку либо отказаться от идеи поворачивать танк на градус; срочно добавить оружие и начать делать сервак
            e.Graphics.ResetTransform();
            e.Graphics.TranslateTransform(tank.X1, tank.Y1);
            e.Graphics.FillEllipse(Brushes.AliceBlue, 0, 0, 0, 0);
            if (tank.shot)
            {
                e.Graphics.DrawImage(tank.particle, 30, 30);
            }
            e.Graphics.RotateTransform(tank.angle);
            //e.Graphics.DrawImage(tank.tank, tank.X1, tank.Y1);
            e.Graphics.DrawImage(tank.tank, -tank.size.Width/1.5f, -20);
            e.Graphics.ResetTransform();
            theMaze.DrawMaze(e);
            
            //Color color1 = Color.Empty;
            //Color color2 = Color.Empty;
            //Color color3 = Color.Empty;
            //Color color4 = Color.Empty;

            //IntPtr hDC = GetDC(this.Handle);
            //int colorRef1 = GetPixel(hDC, tank.X1, tank.Y1);

            //int colorRef2 = GetPixel(hDC, tank.X1 + tank.size.Width, tank.Y1);
            //int colorRef3 = GetPixel(hDC, tank.X1, tank.Y1 + tank.size.Height);
            //int colorRef4 = GetPixel(hDC, tank.X1 + tank.size.Width, tank.Y1 + tank.size.Height);

            //color1 = Color.FromArgb(
            //(int)(colorRef1 & 0x000000FF),
            //(int)(colorRef1 & 0x0000FF00) >> 8,
            //(int)(colorRef1 & 0x00FF0000) >> 16);
            //ReleaseDC(this.Handle, hDC);

            //color2 = Color.FromArgb(
            //        (int)(colorRef2 & 0x000000FF),
            //        (int)(colorRef2 & 0x0000FF00) >> 8,
            //        (int)(colorRef2 & 0x00FF0000) >> 16);
            //ReleaseDC(this.Handle, hDC);

            //color3 = Color.FromArgb(
            //                (int)(colorRef3 & 0x000000FF),
            //                (int)(colorRef3 & 0x0000FF00) >> 8,
            //                (int)(colorRef3 & 0x00FF0000) >> 16);
            //ReleaseDC(this.Handle, hDC);

            //color4 = Color.FromArgb(
            //                (int)(colorRef4 & 0x000000FF),
            //                (int)(colorRef4 & 0x0000FF00) >> 8,
            //                (int)(colorRef4 & 0x00FF0000) >> 16);
            //ReleaseDC(this.Handle, hDC);
            //if (GetColorName(color1) == Color.Gray.Name || GetColorName(color2) == Color.Gray.Name
            //    || GetColorName(color3) == Color.Gray.Name || GetColorName(color4) == Color.Gray.Name)
            //{
            //    this.Text = "yes!";
            //}
            //else
            //    this.Text = "Tanks";
        }

        //http://stackoverflow.com/questions/10875874/c-sharp-color-name-from-argb
        Dictionary<String, Color> colors = typeof(Color).GetProperties(BindingFlags.Static | BindingFlags.Public).ToDictionary(p => p.Name, p => (Color)p.GetValue(null, null));
        public String GetColorName(Color color)
        {
            return colors.Where(c => c.Value.A == color.A && c.Value.R == color.R && c.Value.G == color.G && c.Value.B == color.B).FirstOrDefault().Key;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            #region comment
            //switch (e.KeyCode)
            //{
            //    case Keys.A:
            //    case Keys.Left:
            //        left = true;
            //        tank.Move("left");
            //        break;
            //    case Keys.D:
            //    case Keys.Right:
            //        right = true;
            //        tank.Move("right");
            //        break;
            //    case Keys.W:
            //    case Keys.Up:
            //        up = true;
            //        tank.Move("up");
            //        break;
            //    case Keys.S:
            //    case Keys.Down:
            //        down = true;
            //        tank.Move("down");
            //        break;
            //    case Keys.ControlKey:
            //        tank.Shoot();
            //        break;
            //}

            //if (left)
            //{
            //    tank.angle--;
            //    if (tank.X1 > 0)
            //    {
            //        //ход мысли правилен именно на этом участке кода; суть такова: нарисовать ровный лабиринт и повернутый танк
            //        //сначала вычисляем трансформации для танка, рисуем танк; затем сбрасываем все трансформации и рисуем ровный
            //        //лабиринт. дело только за реализацие.
            //        //p.s. todo: почистить код (хотя бы отвечающий за рисование)
            //        tank.X1--;
            //        //Graphics g = e.Graphics;
            //        //g.RotateTransform(1);
            //        //g.DrawImage(t1, new Point(tank.X1, tank.Y1));
            //        //g.ResetTransform();
            //        //DrawMaze(e);
            //    }
            //    else
            //        tank.X1 = 0;
            //}
            //if (right)
            //{
            //    tank.angle++;
            //    if (tank.X1 < this.ClientSize.Width)
            //        tank.X1++;
            //    else
            //        tank.X1 = this.ClientSize.Width;
            //    //int w;
            //    //if (CellWidth > CellHeight)
            //    //    w = size.Height;
            //    //else
            //    //    w = size.Width;
            //    //if (tank.X1 <= this.ClientRectangle.Width - w + 8)
            //    //    tank.X1++;
            //}
            //if (up)
            //{
            //    if (tank.Y1 > 0)
            //        tank.Y1--;
            //    else
            //        tank.Y1 = 0;
            //}
            //if (down)
            //{
            //    //int h;
            //    //if (CellWidth > CellHeight)
            //    //    h = size.Width;
            //    //else
            //    //    h = size.Height;
            //    //if (tank.Y1 < this.ClientRectangle.Height - h)
            //    //    tank.Y1++;
            //    if (tank.Y1 < this.ClientSize.Height)
            //        tank.Y1++;
            //    else
            //        tank.Y1 = this.ClientSize.Height;
            //}

            #endregion
            tank.Move(e);
            this.Invalidate();
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.A:
                case Keys.Left:
                    tank.left = false;
                    break;
                case Keys.D:
                case Keys.Right:
                    tank.right = false;
                    break;
                case Keys.W:
                case Keys.Up:
                    tank.up = false;
                    break;
                case Keys.S:
                case Keys.Down:
                    tank.down = false;
                    break;
            }
        }
    }
}
