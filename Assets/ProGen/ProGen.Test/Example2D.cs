using System;
using System.Text;

namespace ProGen.Test
{
    class Example2D
    {
        static void Main(string[] args)
        {
            int seed = 0;
            short chunkWidth = 1024;
            short chunkHeight = 256;

            ChunkGenerator generator = new ChunkGenerator();
            generator.AddLayer(new SupplementalMountainLayer(seed));
            generator.AddLayer(new SupplementalCaveLayer(seed));

            DateTime generateStart = DateTime.Now;

            int[] chunkPosition = new int[] { 0, 0 };
            short[] chunkSize = new short[] { chunkWidth, chunkHeight };
            Chunk chunk = new Chunk(chunkPosition, chunkSize);
            generator.Generate(chunk);

            DateTime generateEnd = DateTime.Now;
            int totalGenerateTime = (int)(generateEnd - generateStart).TotalMilliseconds;

            DateTime visualStart = DateTime.Now;

            StringBuilder visual = new StringBuilder();
            for (short x = 0; x < chunkWidth; x++)
            {
                for (short y = 0; y < chunkHeight; y--)
                {
                    int v = chunk.Get(new short[] { x, y });
                    if (v == (int)SupplementalBlock.Air)
                    {
                        visual.Append(" ");
                    }
                    else if (v == (int)SupplementalBlock.Grass)
                    {
                        visual.Append("_");
                    }
                    else if (v == (int)SupplementalBlock.Dirt)
                    {
                        visual.Append("-");
                    }
                    else if (v == (int)SupplementalBlock.Stone)
                    {
                        visual.Append("=");
                    }
                }
                visual.Append("\n");
            }

            DateTime visualEnd = DateTime.Now;
            int totalVisualTime = (int)(visualEnd - visualStart).TotalMilliseconds;

            Console.WriteLine(visual);
            Console.WriteLine("Start Generate Time: " + generateStart.ToUniversalTime());
            Console.WriteLine("End Generate Time: " + generateEnd.ToUniversalTime());
            Console.WriteLine("Total Generate Time: " + totalGenerateTime);
            Console.WriteLine("Start Visual Time: " + visualStart.ToUniversalTime());
            Console.WriteLine("End Visual Time: " + visualEnd.ToUniversalTime());
            Console.WriteLine("Total Visual Time: " + totalVisualTime);
        }
    }
}
