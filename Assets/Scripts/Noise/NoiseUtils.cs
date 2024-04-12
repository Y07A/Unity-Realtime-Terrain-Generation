using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProceduralNoise;

public class NoiseUtils : MonoBehaviour
{
    public enum NoiseType{
        Perlin,
        Simplex,
        Value,
        Voronoi,
        Worley
    }

    public static Noise NewNoise(NoiseType noiseType, int seed, float frequency)
    {
        switch(noiseType){
            case NoiseType.Perlin:
                return new PerlinNoise(seed, frequency);
                break;
            case NoiseType.Simplex:
                return new SimplexNoise(seed, frequency);
                break;
            case NoiseType.Value:
                return new ValueNoise(seed, frequency);
                break;
            case NoiseType.Voronoi:
                return new VoronoiNoise(seed, frequency);
                break;
            case NoiseType.Worley:
                return new WorleyNoise(seed, frequency, .5f);
                break;
        }
        return new PerlinNoise(seed, frequency); 
    }

    public static float Correct(float f){
        return (f+1f)*.5f;
    }

    public static float Sample1D(Noise noise, float x)
    {
        return Correct(noise.Sample1D(x));
    }

    public static float Sample2D(Noise noise, float x, float y)
    {
        return Correct(noise.Sample2D(x, y));
    }

    public static float Sample3D(Noise noise, float x, float y, float z)
    {
        return Correct(noise.Sample3D(x, y, z));
    }

    
}
