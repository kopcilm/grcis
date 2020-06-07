using System;
using System.Collections.Generic;
using OpenTK;
using Rendering;
using Utilities;

namespace MatejKopcil
{
  /// <summary>
  /// Generates quaternion Julia set Solid for given quaternion C. 
  /// </summary>
  [Serializable]
  public class JuliaSet : DefaultSceneNode, ISolid
  {
    /// <summary>
    /// Radius of the bounding sphere, also sets the bounding box.
    /// </summary>
    public double BoundingSphereRadius
    {
      get => boundingSphereRadius;
      set
      {
        if (value <= 0)
        {
          throw new ArgumentOutOfRangeException(nameof ( value ));
        }

        bsr2 = value * value;
        boundingSphereRadius = value;
      }
    }


    /// <summary>
    /// When the iterated quaternion overgrows this radius, it's not inside the Julia set.
    /// </summary>
    public double EscapeRadius
    {
      get => escapeRadius;
      set
      {
        if (value <= 0)
        {
          throw new ArgumentOutOfRangeException(nameof ( value ));
        }

        er2 = value * value;
        escapeRadius = value;
      }
    }


    /// <summary>
    /// Maximum number of iterations of the quaternion. 
    /// </summary>
    public int MaxIterations
    {
      get => maxIterations;
      set
      {
        if (value <= 0)
        {
          throw new ArgumentOutOfRangeException(nameof ( value ));
        }

        maxIterations = value;
      }
    }


    /// <summary>
    /// If quaternion is closer then epsilon to Julia set, we count him as inside of the set.
    /// </summary>
    public double Epsilon
    {
      get => epsilon;
      set
      {
        if (value <= 0)
        {
          throw new ArgumentOutOfRangeException(nameof ( value ));
        }

        epsilon = value;
      }
    }


    /// <summary>
    /// Used to change epsilon based on the distance from ray origin. See GetEpsilon function.
    /// </summary>
    public bool EpsilonRelativeToDistance
    {
      get => epsilonRelativeToDistance;
      set => epsilonRelativeToDistance = value;
    }

    /// <summary>
    /// Used in calculating gradient.
    /// </summary>
    public double Delta
    {
      get => delta;
      set
      {
        if (value <= 0)
        {
          throw new ArgumentOutOfRangeException(nameof ( value ));
        }

        delta = value;
      }
    }

    /// <summary>
    /// Maximal number of intersections of one ray with Julia set.
    /// </summary>
    public int MaxIntersections
    {
      get => maxIntersections;
      set
      {
        if (value <= 0)
        {
          throw new ArgumentOutOfRangeException(nameof ( value ));
        }

        maxIntersections = value;
      }
    }


    /// <summary>
    /// The quaternion which gives the 3D slice.
    /// </summary>
    public Quaterniond C = new Quaterniond(0, 0, 0, 0);

    protected double boundingSphereRadius = 3;
    protected double bsr2;
    protected double escapeRadius = 3.5;
    protected double er2;
    protected int    maxIterations             = 20;
    protected double epsilon                   = 0.0001;
    protected bool   epsilonRelativeToDistance = true;
    protected double delta                     = 0.0000001;
    protected int    maxIntersections          = 128;

    public JuliaSet ()
    {
      bsr2 = boundingSphereRadius * boundingSphereRadius;
      er2 = escapeRadius * escapeRadius;
    }

    /// <summary>
    /// Initializing constructor, set the parameter quaternion c.
    /// In OpenTK.Quaterniond the real part is the last param(W)!
    /// </summary>
    /// <param name="c">quaternion c</param>
    public JuliaSet (Quaterniond c) : this() => C = c;

    /// <summary>
    /// Initializing constructor, set the parameter quaternion c.
    /// </summary>
    /// <param name="r">real part</param>
    /// <param name="i">i imaginary part</param>
    /// <param name="j">j imaginary part</param>
    /// <param name="k">k imaginary part</param>
    public JuliaSet (double r, double i, double j, double k) : this() => C = new Quaterniond(i, j, k, r);

    /// <summary>
    /// Returns an epsilon with the current distance setting. Distance is given by two vectors;
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <returns>epsilon</returns>
    private double GetEpsilon (Vector3d v1, Vector3d v2)
    {
      return EpsilonRelativeToDistance ? (v1 - v2).Length * epsilon : epsilon;
    }

    public void GetBoundingBox (out Vector3d corner1, out Vector3d corner2)
    {
      corner1 = new Vector3d(-boundingSphereRadius, -boundingSphereRadius, -boundingSphereRadius);
      corner2 = new Vector3d(boundingSphereRadius, boundingSphereRadius, boundingSphereRadius);
    }

    /// <summary>
    /// Approximate the lower bound to the closest point inside the Julia set.
    /// </summary>
    /// <param name="v">current position on the ray</param>
    /// <returns>lower bound to the closest point inside the Julia set</returns>
    protected double ExteriorDistanceEstimation (Vector3d v)
    {
      Quaterniond z = new Quaterniond(v.Y, v.Z, 0, v.X);
      Quaterniond dz = new Quaterniond(0, 0, 0, 1);
      for (int i = 0; i < maxIterations; i++)
      {
        dz = 2 * Quaterniond.Multiply(z, dz);
        z = Quaterniond.Multiply(z, z) + C;

        double zMagnitude = z.LengthSquared;
        if (zMagnitude > er2 || double.IsInfinity(zMagnitude) || double.IsNaN(zMagnitude))
        {
          break;
        }
      }

      double zl = z.Length;
      return 0.5 * zl * Math.Log(zl) / dz.Length;
    }

    /// <summary>
    /// Approximate the lower bound to the closest point outside the Julia set.
    /// </summary>
    /// <param name="v">current position on the ray</param>
    /// <param name="outside"></param>
    /// <returns>lower bound to the closest point outside the Julia set</returns>
    protected double InteriorDistanceEstimation (Vector3d v, out bool outside)
    {
      Quaterniond z = new Quaterniond(0, v.X, v.Y, v.Z);
      Quaterniond dz = new Quaterniond(0, 0, 0, 1);
      Quaterniond z0 = z;
      for (int i = 0; i < MaxIterations; i++)
      {
        dz = 2 * Quaterniond.Multiply(z, dz);
        z = Quaterniond.Multiply(z, z) + C;

        double zMagnitude = z.LengthSquared;
        if (zMagnitude > er2 || double.IsInfinity(zMagnitude) || double.IsNaN(zMagnitude))
        {
          outside = true;
          double zl = z.Length;
          return 0.5 * zl * Math.Log(zl) / dz.Length;
        }
      }

      outside = false;
      return (z - z0).Length / dz.Length;
    }

    public override LinkedList<Intersection> Intersect (Vector3d p0, Vector3d p1)
    {
      BoundingSphere bs = new BoundingSphere(boundingSphereRadius);

      // start from point on the bounding sphere
      double t = bs.Intersect(p0, p1);
      if (t == -1.0)
      {
        return null;
      }

      Vector3d v = p0 + t * p1;

      bool inside = false;
      bool front = true;
      LinkedList<Intersection> results = new LinkedList<Intersection>();
      while (true)
      {
        if (!inside)
        {
          double distance = ExteriorDistanceEstimation(v);
          v += distance * p1;
          if (distance < GetEpsilon(v, p0))
          {
            Vector3d u = v - p0;
            t = u.X / p1.X;

            results.AddLast(new Intersection(this) {T = t, Enter = true, Front = front, CoordLocal = p0 + t * p1});

            inside = true;
            front = false;
          }
        }
        else
        {
          double distance = InteriorDistanceEstimation(v, out bool isOut);
          double e = GetEpsilon(v, p0);
          if (isOut && distance >= e)
          {
            Vector3d u = v - p0;
            t = u.X / p1.X;

            results.AddLast(new Intersection(this) {T = t, Enter = false, Front = false, CoordLocal = p0 + t * p1});
            if (results.Count > MaxIntersections)
            {
              break;
            }

            inside = false;
          }

          v += Math.Max(distance, e) * p1;
        }

        if (Vector3d.Dot(v, v) > bsr2)
        {
          break;
        }

      }

      return results;
    }

    public override void CompleteIntersection (Intersection inter)
    {
      // approximating  the normal vector, with gradient 
      Quaterniond q = new Quaterniond(inter.CoordLocal.Y, inter.CoordLocal.Z, 0, inter.CoordLocal.X);

      Quaterniond gx1 = q - new Quaterniond(0, 0, 0, Delta);
      Quaterniond gx2 = q + new Quaterniond(0, 0, 0, Delta);
      Quaterniond gy1 = q - new Quaterniond(Delta, 0, 0, 0);
      Quaterniond gy2 = q + new Quaterniond(Delta, 0, 0, 0);
      Quaterniond gz1 = q - new Quaterniond(0, Delta, 0, 0);
      Quaterniond gz2 = q + new Quaterniond(0, Delta, 0, 0);
      Quaterniond dgx1 = new Quaterniond(0, 0, 0, 1);
      Quaterniond dgx2 = new Quaterniond(0, 0, 0, 1);
      Quaterniond dgy1 = new Quaterniond(0, 0, 0, 1);
      Quaterniond dgy2 = new Quaterniond(0, 0, 0, 1);
      Quaterniond dgz1 = new Quaterniond(0, 0, 0, 1);
      Quaterniond dgz2 = new Quaterniond(0, 0, 0, 1);
      Vector3d n = new Vector3d();
      for (int i = 0; i < maxIterations; i++)
      {
        dgx1 = 2 * Quaterniond.Multiply(gx1, dgx1);
        dgx2 = 2 * Quaterniond.Multiply(gx2, dgx2);
        dgy1 = 2 * Quaterniond.Multiply(gy1, dgy1);
        dgy2 = 2 * Quaterniond.Multiply(gy2, dgy2);
        dgz1 = 2 * Quaterniond.Multiply(gz1, dgz1);
        dgz2 = 2 * Quaterniond.Multiply(gz2, dgz2);
        gx1 = Quaterniond.Multiply(gx1, gx1) + C;
        gx2 = Quaterniond.Multiply(gx2, gx2) + C;
        gy1 = Quaterniond.Multiply(gy1, gy1) + C;
        gy2 = Quaterniond.Multiply(gy2, gy2) + C;
        gz1 = Quaterniond.Multiply(gz1, gz1) + C;
        gz2 = Quaterniond.Multiply(gz2, gz2) + C;

        double dgx = dgx2.Length - dgx1.Length;
        double dgy = dgy2.Length - dgy1.Length;
        double dgz = dgz2.Length - dgz1.Length;
        if (double.IsInfinity(dgx) || double.IsNaN(dgx) ||
            double.IsInfinity(dgy) || double.IsNaN(dgy) ||
            double.IsInfinity(dgz) || double.IsNaN(dgz))
        {
          break;
        }

        n = new Vector3d(dgx, dgy, dgz);
      }

      n.Normalize();
      inter.Normal = n;
    }
  }


  /// <summary>
  /// Texture with color given by normal vector.
  /// </summary>
  public class NormalTexture : ITexture
  {
    public long Apply (Intersection inter)
    {
      Vector3d n = inter.Normal;
      Util.ColorCopy(new[] {1 - (n.X + 1) / 2, 1 - (n.Y + 1) / 2, 1 - (n.Z + 1) / 2}, inter.SurfaceColor);

      inter.textureApplied = true;

      ulong x = (ulong)((n.X + 1.0) * 524288);
      ulong y = (ulong)((n.Y + 1.0) * 524288);
      ulong z = (ulong)((n.Z + 1.0) * 524288);
      var ret = (long)((x << 42) | (y << 21) | z);
      return ret;
    }
  }
}
