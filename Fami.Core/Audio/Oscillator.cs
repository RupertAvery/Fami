using System;

namespace Fami.Core.Audio
{
    public class Oscillator
    {
        public double frequency = 0;
        public double dutycycle = 0;
        public double amplitude = 1;
        double pi = 3.14159;
        double harmonics = 20;

        public double Sample(double t)
        {
            double a = 0;
            double b = 0;
            double p = dutycycle * 2.0 * pi;

            double approxsin(float t)
            {
                float j = t * 0.15915f;
                j = j - (int)j;
                return 20.785 * j * (j - 0.5) * (j - 1.0f);
            };

            for (double n = 1; n < harmonics; n++)
            {
                double c = n * frequency * 2.0 * pi * t;
                a += -approxsin((float)c) / n;
                b += -approxsin((float)(c - p * n)) / n;

                //a += -sin(c) / n;
                //b += -sin(c - p * n) / n;
            }

            return (2.0 * amplitude / pi) * (a - b);
            //return amplitude  * (a - b);
        }
    };
}