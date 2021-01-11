using System;
using ProGen.Library;

namespace ProGen
{

    public class SupplementalMountainLayer : BaseGenerationLayer
    {
        /// The amplitude of the simplex noise. Higher values will increase max mountain height.
        private float amp;

        /// The max amount of layers of dirt between the top layer of grass and bottom layer of stone.
        private int maxDirtLayerThickness;

        /// The min amount of layers of dirt between the top layer of grass and bottom layer of stone.
        private int minDirtLayerThickness;

        /// The height at the sin wave amplitude should rise above and dip below.
        private int baseLine;

        private float frequency;

        private float fractalLacunarity;

        private float fractalGain;

        private float weightedStrength;

        private FastNoiseLite noiseGen = new FastNoiseLite();

        private void init()
        {
            noiseGen.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            noiseGen.SetFractalOctaves(5);
            SetAmp(5);
            SetBaseLine(65);
            SetMaxDirtLayerThickness(4);
            SetMinDirtLayerThickness(1);
            SetFrequency(0.02f);
            SetFractalLacunarity(10f);
            SetFractalGain(10f);
            SetWeightedStrength(0.5f);
        }

        public SupplementalMountainLayer(int seed)
        {
            SetSeed(seed);
            init();
        }

        public SupplementalMountainLayer()
        {
            SetSeed(seed);
            init();
        }


        public void SetFractalLacunarity(float lun)
        {
            fractalLacunarity = lun;
            noiseGen.SetFractalLacunarity(lun);
        }

        public void SetBaseLine(int b)
        {
            baseLine = b;
        }

        public void SetAmp(float a)
        {
            amp = a;
        }

        public void SetMinDirtLayerThickness(int m)
        {
            minDirtLayerThickness = m;
        }

        public void SetMaxDirtLayerThickness(int m)
        {
            maxDirtLayerThickness = m;
        }

        public void SetFrequency(float freq)
        {
            frequency = freq;
            noiseGen.SetFrequency(freq);
        }

        public void SetFractalGain(float g)
        {
            fractalGain = g;
            noiseGen.SetFractalGain(g);
        }

        public void SetWeightedStrength(float w)
        {
            weightedStrength = w;
            noiseGen.SetFractalWeightedStrength(w);
        }

        public override void SetSeed(int seed)
        {
            this.seed = seed;
            noiseGen.SetSeed(seed);
        }

        public override object Clone()
        {
            SupplementalMountainLayer layer = new SupplementalMountainLayer(seed);
            layer.SetSeed(seed);
            layer.SetFrequency(frequency);
            layer.SetAmp(amp);
            layer.SetBaseLine(baseLine);
            layer.SetMaxDirtLayerThickness(maxDirtLayerThickness);
            layer.SetMinDirtLayerThickness(minDirtLayerThickness);
            layer.SetBaseLine(baseLine);
            layer.SetFractalLacunarity(fractalLacunarity);
            layer.SetFractalGain(fractalGain);
            layer.SetWeightedStrength(weightedStrength);
            return layer;
        }

        private void Generate3D(ChunkHelper chunk)
        {
            int offsetX = chunk.X * chunk.Width;
            int offsetY = chunk.Y * chunk.Height;
            int offsetZ = chunk.Z * chunk.Length;
            int maxHeight = (int)amp + baseLine;

            if (offsetY > maxHeight)
            {
                return;
            }

            for (short x = 0; x < chunk.Width; x++)
            {
                for (short z = 0; z < chunk.Length; z++)
                {
                    float noise = noiseGen.GetNoise(x + offsetX, z + offsetZ);

                    int peak = (int)((amp * noise) + baseLine);
                    int dirtLayerThickness = (int)((Math.Abs(noise) * (maxDirtLayerThickness - minDirtLayerThickness)) + minDirtLayerThickness) + 1;

                    int peakDiff = peak - offsetY;
                    if (peakDiff < 0)
                    {
                        continue;
                    }

                    int upperBound = peakDiff + 1;
                    if (upperBound > offsetY + chunk.Height)
                    {
                        upperBound = chunk.Height;
                    }
                    for (short y = 0; y < upperBound; y++)
                    {
                        if (y == peakDiff)
                        {
                            chunk.Set(x, y, z, (short)SupplementalBlock.Grass);
                        }
                        else if (y > peakDiff - dirtLayerThickness)
                        {
                            chunk.Set(x, y, z, (short)SupplementalBlock.Dirt);
                        }
                        else if (y < peakDiff)
                        {
                            chunk.Set(x, y, z, (short)SupplementalBlock.Stone);
                        }
                    }
                }
            }
        }

        private void Generate2D(ChunkHelper chunk)
        {
            int offsetHeight = chunk.Y * chunk.Height;
            int maxHeight = (int)amp + baseLine;

            if (offsetHeight > maxHeight)
            {
                return;
            }

            for (short x = 0; x < chunk.Width; x++)
            {
                float noise = noiseGen.GetNoise(x, 0);
                float peak = (amp * noise) + baseLine;
                int dirtLayerThickness = (int)((Math.Abs(noise) * (maxDirtLayerThickness - minDirtLayerThickness)) + minDirtLayerThickness);

                int peakDiff = (int)peak - offsetHeight;
                if (peakDiff < 0)
                {
                    continue;
                }
                int upperBound = peakDiff;
                if (peakDiff > offsetHeight + chunk.Height)
                {
                    upperBound = chunk.Height;
                }
                for (short y = 0; y < upperBound; y++)
                {
                    if (y == (int)peakDiff)
                    {
                        chunk.Set(x, y, (int)SupplementalBlock.Grass);
                    }
                    else if (y > peakDiff - dirtLayerThickness)
                    {
                        chunk.Set(x, y, (int)SupplementalBlock.Dirt);
                    }
                    else if (y < peakDiff)
                    {
                        chunk.Set(x, y, (int)SupplementalBlock.Stone);
                    }
                }
            }
        }

        public override void Generate(Chunk internalChunk)
        {
            ChunkHelper chunk = new ChunkHelper(internalChunk);
            if (chunk.HasZ)
            {
                Generate3D(chunk);
            }
            else
            {
                Generate2D(chunk);
            }
        }
    }
}
