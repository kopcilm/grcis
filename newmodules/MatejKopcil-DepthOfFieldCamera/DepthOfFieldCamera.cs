using System;
using OpenTK;
using Rendering;

namespace MatejKopcil
{
  /// <summary>
  /// Depth-of-field camera.
  /// </summary>
  [Serializable]
  class DepthOfFieldCamera : StaticCamera, ICamera
  {
    protected double focalLength   = 1.0;
    protected double apertureAngle = 1.0;

    /// <summary>
    /// Aperture radius.
    /// </summary>
    protected double aRadius;

    public DepthOfFieldCamera () {}

    /// <summary>
    /// Initializing constructor, able to set all camera parameters.
    /// </summary>
    /// <param name="fLen">Focal length, determines how far objects must be from the camera to be in focus; this number should always be positive.</param>
    /// <param name="aAng">Aperture angle(in degrees) will determine how blurry objects that are out of focus will appear.</param>
    public DepthOfFieldCamera (double fLen, double aAng)
    {
      FocalLength = fLen;
      ApertureAngle = aAng;
    }

    /// <summary>
    /// Initializing constructor, able to set all camera parameters.
    /// </summary>
    /// <param name="fPoint">Focal point, focal point's distance from camera center determines how far objects must be from the camera to be in focus</param>
    /// <param name="aAng">Aperture angle(in degrees) will determine how blurry objects that are out of focus will appear.</param>
    public DepthOfFieldCamera (Vector3d fPoint, double aAng)
    {
      FocalLength = (center - fPoint).Length;
      ApertureAngle = aAng;
    }

    /// <summary>
    /// Initializing constructor, able to set all camera parameters.
    /// </summary>
    /// <param name="fLen">Focal length, determines how far objects must be from the camera to be in focus; this number should always be positive.</param>
    /// <param name="aAng">Aperture angle(in degrees) will determine how blurry objects that are out of focus will appear.</param>
    /// <param name="cen">Center of the projection.</param>
    /// <param name="dir">View direction (must not be zero).</param>
    /// <param name="ang">Horizontal viewing angle in degrees.</param>
    public DepthOfFieldCamera (double fLen, double aAng, Vector3d cen, Vector3d dir, double ang) : base(cen, dir, ang)
    {
      FocalLength = fLen;
      ApertureAngle = aAng;
    }

    /// <summary>
    /// Initializing constructor, able to set all camera parameters.
    /// </summary>
    /// <param name="fPoint">Focal point, focal point's distance from camera center determines how far objects must be from the camera to be in focus</param>
    /// <param name="aAng">Aperture angle(in degrees) will determine how blurry objects that are out of focus will appear.</param>
    /// <param name="cen">Center of the projection.</param>
    /// <param name="dir">View direction (must not be zero).</param>
    /// <param name="ang">Horizontal viewing angle in degrees.</param>
    public DepthOfFieldCamera (Vector3d fPoint, double aAng, Vector3d cen, Vector3d dir, double ang) :
      base(cen, dir, ang)
    {
      FocalLength = (center - fPoint).Length;
      ApertureAngle = aAng;
    }

    /// <summary>
    /// Initializing constructor, able to set all camera parameters.
    /// </summary>
    /// <param name="fLen">Focal length, determines how far objects must be from the camera to be in focus; this number should always be positive.</param>
    /// <param name="aAng">Aperture angle(in degrees) will determine how blurry objects that are out of focus will appear.</param>
    /// <param name="cen">Center of the projection.</param>
    /// <param name="dir">View direction (must not be zero).</param>
    /// <param name="u">Up vector.</param>
    /// <param name="ang">Horizontal viewing angle in degrees.</param>
    public DepthOfFieldCamera (double fLen, double aAng, Vector3d cen, Vector3d dir, Vector3d u, double ang) :
      base(cen, dir, u, ang)
    {
      FocalLength = fLen;
      ApertureAngle = aAng;
    }

    /// <summary>
    /// Initializing constructor, able to set all camera parameters.
    /// </summary>
    /// <param name="fPoint">Focal point, focal point's distance from camera center determines how far objects must be from the camera to be in focus</param>
    /// <param name="aAng">Aperture angle(in degrees) will determine how blurry objects that are out of focus will appear.</param>
    /// <param name="cen">Center of the projection.</param>
    /// <param name="dir">View direction (must not be zero).</param>
    /// <param name="u">Up vector.</param>
    /// <param name="ang">Horizontal viewing angle in degrees.</param>
    public DepthOfFieldCamera (Vector3d fPoint, double aAng, Vector3d cen, Vector3d dir, Vector3d u, double ang) :
      base(cen, dir, u, ang)
    {
      FocalLength = (center - fPoint).Length;
      ApertureAngle = aAng;
    }

    /// <summary>
    /// Focal length, determines how far objects must be from the camera to be in focus; this number should always be positive.
    /// </summary>
    public double FocalLength
    {
      get => focalLength;
      set
      {
        if (value <= 0)
        {
          throw new ArgumentOutOfRangeException(nameof ( value ));
        }

        focalLength = value;
        prepare();
      }
    }

    /// <summary>
    /// Sets focal length from point.
    /// </summary>
    /// <param name="p"></param>
    public void SetFocalLengthFromPoint (Vector3d p)
    {
      FocalLength = (p - center).Length;
    }

    /// <summary>
    /// Aperture angle(in degrees) will determine how blurry objects that are out of focus will appear.
    /// </summary>
    public double ApertureAngle
    {
      get => apertureAngle;
      set
      {
        apertureAngle = value;
        prepare();
      }
    }

    /// <summary>
    /// Should be called after every parameter change..
    /// </summary>
    protected new void prepare ()
    {
      base.prepare();

      aRadius = Math.Tan(0.5 * MathHelper.DegreesToRadians((float)apertureAngle));
    }

    /// <summary>
    /// Ray-generator. Creates depth-of-field effect.
    /// </summary>
    /// <param name="x">Origin position within a viewport (horizontal coordinate).</param>
    /// <param name="y">Origin position within a viewport (vertical coordinate).</param>
    /// <param name="p0">Ray origin.</param>
    /// <param name="p1">Ray direction vector.</param>
    /// <returns>True if the ray (viewport position) is valid.</returns>
    public new bool GetRay (double x, double y, out Vector3d p0, out Vector3d p1)
    {
      bool res = base.GetRay(x, y, out p0, out p1);

      Vector3d focusPoint = p0 + p1 * focalLength;
      MT.rnd.UniformDirection(-1.0, 1.0, out Vector3d v);
      p0 += aRadius * v;
      p1 = focusPoint - p0;
      p1.Normalize();
      return res;
    }
  }
}
