using System;

namespace OTFontFileVal
{
    // transform class used for rasterization testing
    public class RastTestTransform
    {
        public RastTestTransform()
        {
            stretchX = 1.0f;
            stretchY = 1.0f;

            matrix = new float[3,3];
            for (int i=0; i<3; i++)
            {
                matrix[i,i] = 1.0f;
            }
        }

        public float stretchX;
        public float stretchY;
        public float rotation;
        public float skew;
        public float [,] matrix;
    }
}
