using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reflection;

namespace Tanks
{
    public enum Directions { Left = 0, Right, Up, Down };
    public class Tank
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hWnd);
        [DllImport("gdi32.dll")]
        static extern int GetPixel(IntPtr hDC, int x, int y);
        [DllImport("user32.dll")]
        static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        public Timer bulletTimer = new Timer();
        private Directions dir = Directions.Right;
        public Directions bulletDir = Directions.Right;
        float go = 0;
        //public float X1 = 0;
        //public float Y1 = 0;
        public int X1 = 0;
        public int Y1 = 0;
        public Bitmap tank { get; private set; }
        public Size size;
        public float angle = 90;
        public bool shot;
        public float shotX;
        public float shotY;
        public Bitmap particle;
        public bool rotate1;
        public bool rotate2;
        public bool rotate3;
        public bool rotate4;

        public bool up, down, left, right; //enum? get-set? в класс tank?

        public Tank(int x, int y, double CellWidth, double CellHeight)
        {
            //rotate = false;
            bulletTimer.Interval = 5000;
            bulletTimer.Tick += new EventHandler(bulletTimer_Tick);
            size = new Size();
            size.Height = (int)CellWidth - 20;
            size.Width = (int)CellHeight - 20;
            this.X1 = x + (size.Height / 2) + Maze.wallThickness;
            this.Y1 = y + (size.Width / 2) + Maze.wallThickness;
            this.tank = new Bitmap(Properties.Resources.bm, size);
            //this.tank = new Bitmap(Properties.Resources.lt3, size);
            if (CellWidth > CellHeight)
            {
                //tank.RotateFlip(RotateFlipType.Rotate90FlipX);
                angle = 90;
            }
        }

        public void Move(KeyEventArgs e)
        {
            //if (angle > 365)
            //{
            //    angle = 0;
            //    go = 0;
            //}
            switch (e.KeyCode)
            {
                case Keys.A:
                case Keys.Left:
                    left = true;
                    angle = -90;
                    dir = Directions.Left;
                    break;
                case Keys.D:
                case Keys.Right:
                    right = true;
                    angle = 90;
                    dir = Directions.Right;
                    break;
                case Keys.W:
                case Keys.Up:
                    up = true;
                    angle = 0;
                    dir = Directions.Up;
                    break;
                case Keys.S:
                case Keys.Down:
                    down = true;
                    angle = -180;
                    dir = Directions.Down;
                    break;
                case Keys.ControlKey:
                    Shoot();
                    break;
            }

            if (up)
            {
                if (!CheckCollision("up", "tank"))
                    Y1--;
            }
            if (down)
            {
                if (!CheckCollision("down", "tank"))
                    Y1++;
            }
            if (right)
            {
                if (!CheckCollision("right", "tank"))
                    X1++;
            }
            if (left)
            {
                if (!CheckCollision("left", "tank"))
                    X1--;
            }

            #region SmoothMove
            //if (left)
            //{
            //    angle--;
            //    go -= 0.01f;
            //}
            //if (right)
            //{
            //    angle++;
            //    go += 0.01f;
            //}
            //if (up)
            //{
            //    //сместить центр и разюить на больше "делений"
            //    if (angle >= 85 && angle <= 95)
            //        X1++;
            //    else if (angle >= 91 && angle <= 120)
            //    {
            //        Y1+=go;
            //        X1++;
            //    }
            //    else if (angle >= 121 && angle <= 179)
            //    {
            //        Y1 += go;
            //        X1+=0.5f;
            //    }
            //    else if (Math.Abs(angle) == 180)
            //        Y1++;
            //    else if (angle >= 181 && angle <= 270)
            //    {
            //        Y1 += go;
            //        X1--;
            //    }
            //    else if (angle == 271)
            //        X1--;
            //    else if (angle >= 271 && angle <= 365)
            //    {
            //        Y1-=go;
            //        X1--;
            //    }
            //    //else
            //    //{
            //    //    if (Y1 > 0)
            //    //        Y1--;
            //    //    else
            //    //        Y1 = 0;
            //    //}
            //}
            //if (down)
            //{
            //    if (angle >= 85 && angle <= 95)
            //        X1--;
            //    else
            //    {
            //        if (Y1 < Form1.ClientHeight)
            //            Y1++;
            //        else
            //            Y1 = Form1.ClientHeight;
            //    }
            //}
            #endregion
            #region SwitchDirection
            //switch (direction)
            //{
            //    case "left":
            //        angle--;
            //        if (X1 > 0)
            //        {
            //            X1--;
            //        }
            //        else
            //            X1 = 0;
            //        break;

            //    case "right":
            //        angle++;
            //        if (X1 < Form1.ClientWidth)
            //            X1++;
            //        else
            //            X1 = Form1.ClientWidth;
            //        break;

            //    case "up":
            //        if (angle >= 85 && angle <= 95)
            //            X1++;
            //        else
            //        {
            //            if (Y1 > 0)
            //                Y1--;
            //            else
            //                Y1 = 0;
            //        }
            //        break;

            //    case "down":
            //        if (Y1 < Form1.ClientHeight)
            //            Y1++;
            //        else
            //            Y1 = Form1.ClientHeight;
            //        break;
            //}
            #endregion
        }

        private bool CheckCollision(string direction, string type)
        {
            int tx = 0, ty = 0;
            Size checker = new Size();
            if (type == "tank")
            {
                tx = X1;
                ty = Y1;
                checker = this.size;
            }
            if (type == "bullet")
            {
                tx = (int)shotX;
                ty = (int)shotY;
                checker = new Size(this.size.Width / 2, this.size.Height / 2);
                checker.Height /= 2;
                checker.Width /= 2;
            }
            if ((shotX >= this.X1 - this.size.Height / 2 && shotX <= this.X1 + this.size.Height / 2) && (shotY >= this.Y1 - this.size.Height / 2 && shotY <= this.Y1 + this.size.Height / 2))
            {
                //MessageBox.Show("destroy");
                this.tank.Tag = "destroyed";
            }

            Color color1 = Color.Empty;
            Color color2 = Color.Empty;
            //Color color3 = Color.Empty;
            //Color color4 = Color.Empty;

            IntPtr hDC = GetDC(Form1.FormHandle);
            int colorRef1 = 0;
            //int colorRef2 = 0;
            switch (direction)
            {
                case "up":
                    colorRef1 = GetPixel(hDC, tx, ty - checker.Height / 2);
                    //colorRef2 = GetPixel(hDC, tx+checker.Width, ty - checker.Height / 2);
                    break;
                case "down":
                    colorRef1 = GetPixel(hDC, tx, ty + checker.Height / 2);
                    break;
                case "left":
                    colorRef1 = GetPixel(hDC, tx - checker.Width, ty);
                    //colorRef2 = GetPixel(hDC, tx - checker.Height, ty + checker.Height / 2);
                    break;
                case "right":
                    colorRef1 = GetPixel(hDC, tx + checker.Width, ty);
                    //colorRef2 = GetPixel(hDC, tx + checker.Width, ty - checker.Height / 2);
                    break;
            }
            //int colorRef3 = GetPixel(hDC, X1, Y1 + size.Height);
            //int colorRef4 = GetPixel(hDC, X1 + size.Width, Y1 + size.Height);

            color1 = Color.FromArgb(
            (int)(colorRef1 & 0x000000FF),
            (int)(colorRef1 & 0x0000FF00) >> 8,
            (int)(colorRef1 & 0x00FF0000) >> 16);
            ReleaseDC(Form1.FormHandle, hDC);

            //color2 = Color.FromArgb(
            //        (int)(colorRef2 & 0x000000FF),
            //        (int)(colorRef2 & 0x0000FF00) >> 8,
            //        (int)(colorRef2 & 0x00FF0000) >> 16);
            //ReleaseDC(Form1.FormHandle, hDC);

            //color3 = Color.FromArgb(
            //                (int)(colorRef3 & 0x000000FF),
            //                (int)(colorRef3 & 0x0000FF00) >> 8,
            //                (int)(colorRef3 & 0x00FF0000) >> 16);
            //ReleaseDC(Form1.FormHandle, hDC);

            //color4 = Color.FromArgb(
            //                (int)(colorRef4 & 0x000000FF),
            //                (int)(colorRef4 & 0x0000FF00) >> 8,
            //                (int)(colorRef4 & 0x00FF0000) >> 16);
            ReleaseDC(Form1.FormHandle, hDC);
            //if (GetColorName(color1) == Color.Gray.Name || GetColorName(color2) == Color.Gray.Name
            //    || GetColorName(color3) == Color.Gray.Name || GetColorName(color4) == Color.Gray.Name)
            if (GetColorName(color1) == Color.Gray.Name)
            {
                return true;
            }
            return false;
        }

        //http://stackoverflow.com/questions/10875874/c-sharp-color-name-from-argb
        Dictionary<String, Color> colors = typeof(Color).GetProperties(BindingFlags.Static | BindingFlags.Public).ToDictionary(p => p.Name, p => (Color)p.GetValue(null, null));
        public String GetColorName(Color color)
        {
            return colors.Where(c => c.Value.A == color.A && c.Value.R == color.R && c.Value.G == color.G && c.Value.B == color.B).FirstOrDefault().Key;
        }

        public void Shoot()
        {
            if (shot)
                return;
            Size size = new Size(this.size.Width / 2, this.size.Height / 2);
            particle = new Bitmap(Properties.Resources.bullets_PNG1459, size);

            this.shot = true;
            //this.rotate = false;
            switch (dir)
            {
                case Directions.Left:
                    shotX = X1 - this.size.Height;
                    shotY = Y1 - this.size.Width / 2;
                    bulletDir = Directions.Left;
                    particle.RotateFlip(RotateFlipType.Rotate180FlipY);
                    rotate3 = false;
                    break;
                case Directions.Right:
                    shotX = X1 + this.size.Height;
                    shotY = Y1 - this.size.Width / 2;
                    bulletDir = Directions.Right;
                    rotate4 = false;
                    break;
                case Directions.Up:
                    shotX = X1 - (this.size.Width / 2) - 2;
                    shotY = Y1 - (this.size.Height / 2) - 2;
                    rotate1 = false;
                    bulletDir = Directions.Up;
                    break;
                case Directions.Down:
                    shotX = X1 + this.size.Width / 2;
                    shotY = Y1 + this.size.Height;
                    rotate2 = false;
                    bulletDir = Directions.Down;
                    break;
                default:
                    break;
            }
            bulletTimer.Enabled = true;
        }

        public void BulletFly()
        {
            switch (bulletDir)
            {
                case Directions.Left:
                    if (!CheckCollision("left", "bullet"))
                        shotX--;
                    else
                    {
                        particle.RotateFlip(RotateFlipType.Rotate180FlipY);
                        bulletDir = Directions.Right;
                    }
                    break;
                case Directions.Right:
                    if (!CheckCollision("right", "bullet"))
                        shotX++;
                    else
                    {
                        particle.RotateFlip(RotateFlipType.Rotate180FlipY);
                        bulletDir = Directions.Left;
                    }
                    break;
                case Directions.Up:
                    if (!CheckCollision("up", "bullet"))
                        shotY--;
                    else
                    {
                        //particle.RotateFlip(RotateFlipType.Rotate180FlipX);
                        bulletDir = Directions.Down;
                    }
                    break;
                case Directions.Down:
                    if (!CheckCollision("down", "bullet"))
                        shotY++;
                    else
                    {
                        //particle.RotateFlip(RotateFlipType.Rotate180FlipX);
                        bulletDir = Directions.Up;
                    }
                    break;
            }
        }

        private void bulletTimer_Tick(object sender, EventArgs e)
        {
            bulletTimer.Enabled = false;
            shot = false;
            rotate1 = true;
            rotate2 = true;
            rotate3 = true;
            rotate4 = true;
            //rotate = false;
        }
    }
}
