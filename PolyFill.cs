using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Media3D;
using Point = System.Drawing.Point;

namespace GrayBMP {
   class PolyFillWin {
      List<Edge> edges = new ();
      public PolyFillWin (string filePath) {
         foreach (var result in File.ReadAllLines (filePath)) {
            string[] splits = result.Split (" ");
            int x0 = Convert.ToInt32 (splits[0]), y0 = Convert.ToInt32 (splits[1]),
            x1 = Convert.ToInt32 (splits[2]), y1 = Convert.ToInt32 (splits[3]);
            var vertices = new Point[2] { new (x0, y0), new (x1, y1) };
            vertices = vertices.OrderBy (v => v.Y).ThenBy (v => v.X).ToArray ();
            edges.Add (new (vertices[0].X, vertices[0].Y, vertices[1].X, vertices[1].Y));
         }
      }
      public void Fill (GrayBMP bmp, int color) {
         List<Pair> ints = new ();
         edges = edges.OrderBy (e => e.StartPt.X).ThenBy (e => e.StartPt.Y).ToList ();
         for (double i = 0.5; i <= 599; i++) {
            Pair p1 = new (0, i),
                 p2 = new (899, i);
            foreach (var edg in edges) {
               if (Pair.Intersection (p1, p2, edg.StartPt, edg.EndPt, out var intersect) && intersect.X < 899 && intersect.Y < 599 && intersect.Y == i)
                  if ((int)(intersect.Y + 0.5) == (int)(i + 0.5))
                     ints.Add (intersect);
            }
            if (ints.Count % 2 == 0) {
               for (int j = 0; j < ints.Count - 1; j += 2) {
                  int x0 = (int)(ints[j].X + 0.5), y0 = (int)(ints[j].Y + 0.5),
                      x1 = (int)(ints[j + 1].X + 0.5), y1 = (int)(ints[j + 1].Y + 0.5);
                  bmp.DrawLine (x0, y0, x1, y1, color);
               }
               ints.Clear ();
            }
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
         double x, y;
         double x1 = A.X, x2 = B.X, x3 = C.X, x4 = D.X;
         double y1 = A.Y, y2 = B.Y, y3 = C.Y, y4 = D.Y;
         double d = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
         double pre = (x1 * y2 - y1 * x2), post = (x3 * y4 - y3 * x4);
         x = (pre * (x3 - x4) - (x1 - x2) * post) / d;
         y = (pre * (y3 - y4) - (y1 - y2) * post) / d;
         bool xWithinLine1 = x >= Math.Min (x1, x2) && x <= Math.Max (x1, x2);
         bool xWithinLine2 = x >= Math.Min (x3, x4) && x <= Math.Max (x3, x4);
         bool yWithinLine1 = y >= Math.Min (y1, y2) && y <= Math.Max (y1, y2);
         bool yWithinLine2 = y >= Math.Min (y3, y4) && y <= Math.Max (y3, y4);
         bool isIntersect = xWithinLine1 && yWithinLine1 && xWithinLine2 && yWithinLine2;
         intersect = isIntersect ? new (x, y) : new (-1, -1);
         return isIntersect;
      }
   }

   struct Edge {
      public Edge (int x0, int y0, int x1, int y1) { StartPt = new Pair (x0, y0); EndPt = new Pair (x1, y1); }
      public Pair StartPt { get; set; }
      public Pair EndPt { get; set; }
   }
}


