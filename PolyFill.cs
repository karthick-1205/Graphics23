using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Media3D;
using Point = System.Drawing.Point;

namespace GrayBMP {
   class PolyFillWin {
      List<Line> lines = new ();
      List<Pair> pairs = new ();
      List<Edge> edges = new ();
      public PolyFillWin (string filePath) {
         foreach (var result in File.ReadAllLines (filePath)) {
            string[] splits = result.Split (" ");
            int x0 = Convert.ToInt32 (splits[0]), y0 = Convert.ToInt32 (splits[1]),
            x1 = Convert.ToInt32 (splits[2]), y1 = Convert.ToInt32 (splits[3]);
            edges.Add (new (x0, y0, x1, y1));
         }
      }
      public void AddLine (int x0, int y0, int x1, int y1) {
         lines.Add (new (x0, y0, x1, y1));
      }

      public void Fill (GrayBMP bmp, int color) {
         List<Pair> ints = new List<Pair> ();
         for (double i = 0.5; i <= 599; i++) {
            Pair p1 = new (0, i),
                 p2 = new (899, i);
            foreach (var edg in edges) {
               if (Pair.Intersection (p1, p2, edg.StartPt, edg.EndPt, out var intersect) && intersect.X < 899 && intersect.Y < 599)
                  ints.Add (intersect);
            }
         }
         ints = ints.OrderBy (pair => pair.X).ToList ();
         for (int i = 0; i < ints.Count - 1; i += 2) {
            bmp.DrawLine ((int)ints[i].X, (int)ints[i].Y, (int)ints[i + 1].X, (int)ints[i + 1].Y, 255);
         }

      }
   }

   public struct Pair {
      public double X { get; set; }
      public double Y { get; set; }
      public Pair (double first, double second) {
         X = first;
         Y = second;
      }
      public static bool Intersection (Pair A, Pair B, Pair C, Pair D, out Pair intersect) {
         double px1 = ((A.X * B.Y) - (A.Y * B.X)) * (C.X - D.X);
         double px2 = (A.X - B.X) * ((C.X * D.Y) - (C.Y * D.X));
         double px3 = (A.X - B.X) * (C.Y - D.Y);
         double px4 = (A.Y - B.Y) * (C.X - D.X);
         double denominator = (px3 - px4);
         double res1 = (px1 - px2) / denominator;

         double py1 = ((A.X * B.Y) - (A.Y * B.Y)) * (C.Y - D.Y);
         double py2 = (A.Y - B.Y) * ((C.X * D.Y) - (C.Y * D.X));
         double res2 = (py1 - py2) /denominator;

         bool isIntersect = denominator != 0 ;
         intersect = isIntersect ? new (res1, res2) : new (-1, -1);
         return isIntersect;
      }

   }

   class Line {
      public Line (int x0, int y0, int x1, int y1) { StartPt = new Point (x0, y0); EndPt = new Point (x1, y1); }
      public Point StartPt { get; set; }
      public Point EndPt { get; set; }
   }
   struct Edge {
      public Edge (int x0, int y0, int x1, int y1) { StartPt = new Pair (x0, y0); EndPt = new Pair (x1, y1); }
      public Pair StartPt { get; set; }
      public Pair EndPt { get; set; }
   }
}


