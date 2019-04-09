using System;

namespace noise_p
{
class Program
{
static void example()
  {
  Noise.NoiseGenPng("check.png",Noise.netGen(20,20),70,70);
  Console.WriteLine("end: st");
  Noise.NoiseGenPng("check_9x5.png",Noise.netGen(7,9),90,50);
  Console.WriteLine("end: 9x5");
  Noise.NoiseGenPng("check7oct.png",Noise.netGen(15,15),40,40,octave:7);
  Console.WriteLine("end: 7oct");
  Noise.NoiseGenPng("check3oct.png",Noise.netGen(17,26),63,63,octave:3);
  Console.WriteLine("end: 3oct");
  Console.WriteLine("_________");
  }

static void Main(string[] args)
  {
  example();
  Console.ReadKey();
  }
}
}
