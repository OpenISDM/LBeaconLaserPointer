using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LBeaconLaserPointer.Modules
{
    public class Utility
    {
        public static double RotateAngle(Point CenterPoint, Point FirstPoint, Point SecondPoint)
        {
            double DotProductANS = DotProductAngle(CenterPoint, FirstPoint, SecondPoint);
            double OuterProductANS = OuterProductAngle(CenterPoint, FirstPoint, SecondPoint);

            if (OuterProductANS < 0)
                return -DotProductANS * 180 / Math.PI;
            else
                return DotProductANS * 180 / Math.PI;
        }

        private static double DotProductAngle(Point CenterPoint, Point FirstPoint, Point SecondPoint)
        {
            double Xa, Xb, Ya, Yb;
            double Angle;

            Xa = FirstPoint.X - CenterPoint.X;
            Ya = FirstPoint.Y - CenterPoint.Y;

            Xb = SecondPoint.X - CenterPoint.X;
            Yb = SecondPoint.Y - CenterPoint.Y;

            double c = Math.Sqrt(Xa * Xa + Ya * Ya) * Math.Sqrt(Xb * Xb + Yb * Yb);

            if (c == 0) return -1;

            Angle = Math.Acos((Xa * Xb + Ya * Yb) / c);

            return Angle;
        }

        private static double OuterProductAngle(Point CenterPoint, Point FirstPoint, Point SecondPoint)
        {
            double Xa, Xb, Ya, Yb;
            double Angle;

            Xa = FirstPoint.X - CenterPoint.X;
            Ya = FirstPoint.Y - CenterPoint.Y;

            Xb = SecondPoint.X - CenterPoint.X;
            Yb = SecondPoint.Y - CenterPoint.Y;

            double c = Math.Sqrt(Xa * Xa + Ya * Ya) * Math.Sqrt(Xb * Xb + Yb * Yb);

            Angle = Math.Asin((FirstPoint.X * SecondPoint.Y - SecondPoint.X * FirstPoint.Y) / c);

            return Angle;
        }
    }

    public class Point
    {
        public double X;
        public double Y;

        public Point(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }
    }
}
