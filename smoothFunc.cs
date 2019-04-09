using static System.Math;

namespace noise_p
{
public static class smoothFunc
  {
  public delegate double SFunc(double x);
  
  /// <param name="x">[0:1]</param>
  public static double smFunction_null(double x)
    {
    double x2=x*x; double x3=x2*x;
    return 6*x3*x2-15*x2*x2+10*x3;
    }

  /// <param name="x">[0:1]</param>
  public static double smFunction_line(double x)
    {return x;}

  /// <param name="x">[0:1]</param>
  public static double smFunction_sin(double x)
    {return 0.5+0.5*Sin(PI*(x-0.5));}

  /// <param name="x">[0:1]</param>
  public static double smFunction_sq(double x)
    {
    if(x<0.5)return 2*x*x;
    double d=2*x-2;
    return 1-0.5*d*d;
    }
  }
}
