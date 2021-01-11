using System;
using System.Collections.Generic;

using ProGen.Library;

namespace ProGen
{
    public class SupplementalCaveLayer : BaseGenerationLayer
    {
        private double stoneFillPercentage;

        private double dirtFillPercentage;

        private float fractalLacunarity;

        private float fractalGain;

        private float frequency;

        private int maxHeight;

        private int minHeight;


        private FastNoiseLite noiseGen = new FastNoiseLite();

        private void init()
        {
            noiseGen.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            noiseGen.SetFractalType(FastNoiseLite.FractalType.Ridged);
            SetFractalLacunarity(2f);
            SetFrequency(0.005f);
            SetFractalGain(0.4f);
            stoneFillPercentage = 0.2;
            dirtFillPercentage = 0.15;
            SetMinHeight(-1);
            SetMaxHeight(260);
        }

        public SupplementalCaveLayer(int seed)
        {
            SetSeed(seed);
            init();
        }

        public SupplementalCaveLayer()
        {
            init();
        }

        public void SetMinHeight(int min)
        {
            minHeight = min;
        }

        public void SetMaxHeight(int max)
        {
            maxHeight = max;
        }

        public void SetStoneFillPercentage(double p)
        {
            stoneFillPercentage = p;
        }

        public void SetDirtFillPercentage(double p)
        {
            dirtFillPercentage = p;
        }

        public void SetFrequency(float freq)
        {
            frequency = freq;
            noiseGen.SetFrequency(freq);
        }

        public void SetFractalLacunarity(float lun)
        {
            fractalLacunarity = lun;
            noiseGen.SetFractalLacunarity(lun);
        }
        public void SetFractalGain(float gain)
        {
            fractalGain = gain;
            noiseGen.SetFractalGain(gain);
        }

        public override void SetSeed(int seed)
        {
            this.seed = seed;
            noiseGen.SetSeed(seed);
        }

        public override object Clone()
        {
            SupplementalCaveLayer layer = new SupplementalCaveLayer(seed);
            layer.SetDirtFillPercentage(dirtFillPercentage);
            layer.SetFractalGain(fractalGain);
            layer.SetFractalLacunarity(fractalLacunarity);
            layer.SetStoneFillPercentage(stoneFillPercentage);
            layer.SetDirtFillPercentage(dirtFillPercentage);
            layer.SetFrequency(frequency);
            return layer;
        }

        public override void Generate(Chunk internalChunk)
        {
            ChunkHelper chunk = new ChunkHelper(internalChunk);
            int offsetX = chunk.X * chunk.Width;
            int offsetY = chunk.Y * chunk.Height;
            int offsetZ = chunk.Z * chunk.Length;
            if (offsetY > maxHeight)
            {
                return;
            }

            int heightBound = maxHeight - offsetY;
            if (heightBound > chunk.Height)
            {
                heightBound = chunk.Height;
            }

            for (short x = 0; x < chunk.Width; x++)
            {
                for (short z = 0; z < chunk.Length; z++)
                {
                    for (short y = 0; y < heightBound; y++)
                    {
                        short mat = chunk.Get(x, y, z);
                        if (mat != (short)SupplementalBlock.Dirt &&
                            mat != (short)SupplementalBlock.Stone &&
                            mat != (short)SupplementalBlock.Grass)
                        {
                            continue;
                        }

                        double fillPercentage;
                        if (mat == (short)SupplementalBlock.Stone)
                        {
                            fillPercentage = stoneFillPercentage;
                        }
                        else
                        {
                            fillPercentage = dirtFillPercentage;
                        }

                        double fill = Math.Abs(noiseGen.GetNoise(x + offsetX, y + offsetY, z + offsetZ));
                        if (fill >= 1 - fillPercentage)
                        {
                            if (chunk.HasZ)
                            {
                                chunk.Set(x, y, z, (short)SupplementalBlock.Air);
                            }
                            else
                            {
                                chunk.Set(x, y, (short)SupplementalBlock.Air);
                            }
                        }
                    }
                }
            }
        }
    }
}