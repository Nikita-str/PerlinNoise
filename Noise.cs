using System;
using static System.Math;

using static noise_p.smoothFunc;

using Bitmap=System.Drawing.Bitmap;
using GR=System.Drawing.Graphics;
using COL=System.Drawing.Color;
using imSpace=System.Drawing.Imaging;

namespace noise_p
{
public struct Vector2
  {
  private readonly static Random rand=new Random();
  public double x{get; private set;}
  public double y{get; private set;}
  public readonly double len;

  public Vector2(double x, double y,double len=-1)
    {
    this.x=x; this.y=y; 
    this.len=(len>=0)?len:Sqrt(x*x+y*y); 
    }

  public static Vector2 RandVector(double len=1)
    {
    double ang=rand.NextDouble()*2*PI;
    double x=Cos(ang),y=Sin(ang);
    //double x=(rand.NextDouble()*2-1.0);
    //double y=rand.Next(0,2)==1?Sqrt(1-x*x):-Sqrt(1-x*x);
    return new Vector2(x,y,len);
    }
  
  /// <summary>псевдоскаляроне произведение</summary>
  public static double operator ^(Vector2 a,Vector2 b)
    {return a.x*b.y+b.x*a.y;}
  
  /// <summary>псевдоскаляроне произведение</summary>
  public static double dotMul(Vector2 a,double x,double y)
    {return a.x*y+x*a.y;}

  public override string ToString()
    {return "("+String.Format("{0:N3}",x)+" : "+String.Format("{0:N3}",y)+")";}
  }

public static class Noise
{

static double interpolationLine(double one,double two,double proc)
  {return ((1-proc)*one+proc*two);}

public static Vector2[,] netGen(int w,int h)
  {
  Vector2[,] ret=new Vector2[h,w];
  for(int i=0; i<h; i++)
  for(int j=0; j<w; j++)
    ret[i,j]=Vector2.RandVector();
  return ret;
  }

public static void NoiseGenPng(string nameSave,Vector2[,] net,int cellW,int cellH,int octave=5,SFunc smoothFunc=null)
  {
  if(smoothFunc==null)smoothFunc=smFunction_null;

  int H=net.GetLength(0);
  int W=net.GetLength(1);
  
  int allH=H*cellH;
  int allW=W*cellW;
  Bitmap bmp=new Bitmap(allW,allH);

  using(var g=GR.FromImage(bmp)) g.Clear(COL.White);

  var data=bmp.LockBits(new System.Drawing.Rectangle(0,0, allW,allH),imSpace.ImageLockMode.ReadWrite, imSpace.PixelFormat.Format32bppArgb);
  IntPtr scan=data.Scan0;

  double _div=Max(cellW,cellH);
  
  double [,,,] values=new double[H,W,cellH,cellW];//[allH*allW];
  #region precount
  ///helpful if octave>1
  for(int h=0; h<H; h++)
  for(int w=0; w<W; w++)
    for(int y=0; y<cellH; y++)
    for(int x=0; x<cellW; x++)
      values[h,w,y,x]=
        interpolationLine(
        interpolationLine(Vector2.dotMul(net[h%H    ,w%W    ],        x/_div,         y/_div),
                          Vector2.dotMul(net[h%H    ,(w+1)%W],(x-cellW)/_div,         y/_div),
                          smoothFunc(x/(double)cellW)),
        interpolationLine(Vector2.dotMul(net[(h+1)%H,w%W    ],        x/_div, (y-cellH)/_div),
                          Vector2.dotMul(net[(h+1)%H,(w+1)%W],(x-cellW)/_div, (y-cellH)/_div),
                          smoothFunc(x/(double)cellW)),
        smoothFunc(y/(double)cellH));
  #endregion precount
  
  unsafe
  {
  byte*ptr=(byte*)scan.ToPointer();
  byte*ptr0=ptr;
  byte*ptrW=ptr;
  int deltaPtr=4*allW;

  for(int h=0; h<H; h++,ptrW=ptr=ptr0+=cellH*deltaPtr)
  for(int w=0; w<W; w++,ptr=ptrW+=cellW*4)
    {
    byte* ptrsave=ptr;
    for(int y=0; y<cellH; y++,ptr=ptrsave+=deltaPtr)
    for(int x=0; x<cellW; x++,ptr+=4)
      {
      double bnow=0;
      double sumCoef=0;
      for(int i=0,k=1; i<octave; i++,k<<=1)
        {
        int _x=(x*k)%cellW;
        int _y=(y*k)%cellH;
        int hk=h*k+(y*k)/cellH;
        int wk=w*k+(x*k)/cellW;
        
        double coef=(1.0/k);
        bnow+=coef*values[hk%H,wk%W,_y,_x];

        sumCoef+=coef;
        }
      
      bnow=bnow/sumCoef;
      bnow+=1.0; bnow/=2;
      *(ptr)=*(ptr+1)=*(ptr+2)=(byte)(bnow*255);
      }
    }
  }

  bmp.UnlockBits(data);
  bmp.Save(nameSave,imSpace.ImageFormat.Png);
  bmp.Dispose(); bmp=null;
  values=null;
  }
}
}
