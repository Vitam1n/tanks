using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

//дивжение ?
// координаты для оружие (не в дэд-ендах)

namespace Tanks
{
    public partial class Form1 : Form
    {
        //private int x2, y2;

        public static int ClientWidth;
        public static int ClientHeight;
        public static int FormWidth;
        public static int FormHeight;
        public static IntPtr FormHandle {get; private set;}

        Tank tank;
        Maze theMaze;

        public Form1()
        {
            InitializeComponent();
            FormHandle = this.Handle;
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
            //e.Graphics.FillEllipse(Brushes.Red, 0, 0, 10, 10);
            
            e.Graphics.RotateTransform(tank.angle);
            //e.Graphics.DrawImage(tank.tank, tank.X1, tank.Y1);
            //e.Graphics.DrawImage(tank.tank, -(float)theMaze.CellHeight/3, -(float)theMaze.CellWidth/2);
            e.Graphics.DrawImage(tank.tank, -(float)tank.size.Width / 2, -(float)tank.size.Height / 2);
            if (tank.shot)
            {
                e.Graphics.ResetTransform();
                e.Graphics.TranslateTransform(tank.shotX, tank.shotY);
                e.Graphics.DrawImage(tank.particle, 0, 0);
            }
            //e.Graphics.FillEllipse(Brushes.Red, 0, 0, 10, 10);
            //e.Graphics.DrawImage(tank.tank, -(float)tank.size.Width / 2, -(float)tank.size.Height / 2);
            e.Graphics.ResetTransform();
            theMaze.DrawMaze(e);
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
