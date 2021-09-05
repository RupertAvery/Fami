namespace Fami.Core.Audio
{
    public class Apu_old
    {
        private uint frame_clock_counter;
        private uint clock_counter;
        private bool dmc_interrupt;
        private bool frame_interrupt;

        private double pulse1_sample;
        private double pulse1_output;
        private bool pulse1_enable;
        private bool pulse1_halt;

        private double pulse2_sample;
        private double pulse2_output;
        private bool pulse2_enable;
        private bool pulse2_halt;

        private double noise_sample = 0;
        private double noise_output = 0;
        private bool noise_enable = false;
        private bool noise_halt = false;

        public double dGlobalTime;

        private Sequencer pulse1_seq;
        private Oscillator pulse1_osc;
        private Envelope pulse1_env;
        private LengthCounter pulse1_lc;
        private Sweeper pulse1_sweep;

        private Sequencer pulse2_seq;
        private Oscillator pulse2_osc;
        private Envelope pulse2_env;
        private LengthCounter pulse2_lc;
        private Sweeper pulse2_sweep;

        private Sequencer triangle_seq;
        private Oscillator triangle_osc;
        private Envelope triangle_env;
        private LengthCounter triangle_lc;
        private Sweeper triangle_sweep;

        private Sequencer noise_seq;
        private Envelope noise_env;
        private LengthCounter noise_lc;


        uint[] length_table = new uint[]{
             10, 254, 20,  2, 40,  4, 80,  6,
            160,   8, 60, 10, 14, 12, 26, 14,
             12,  16, 24, 18, 48, 20, 96, 22,
            192,  24, 72, 26, 16, 28, 32, 30
        };

        int[] noise_reload_table = new int[]
        {
              0,   4,   8,  16,  32,   64,   96,  128, 
            160, 202, 254, 380, 508, 1016, 2034, 4068
        };


        public Apu_old()
        {
            pulse1_seq = new Sequencer();
            pulse1_osc = new Oscillator();
            pulse1_env = new Envelope();
            pulse1_lc = new LengthCounter();
            pulse1_sweep = new Sweeper();

            pulse2_seq = new Sequencer();
            pulse2_osc = new Oscillator();
            pulse2_env = new Envelope();
            pulse2_lc = new LengthCounter();
            pulse2_sweep = new Sweeper();

            triangle_seq = new Sequencer();
            triangle_osc = new Oscillator();
            triangle_env = new Envelope();
            triangle_lc = new LengthCounter();
            triangle_sweep = new Sweeper();

            noise_seq = new Sequencer();
            noise_env = new Envelope();
            noise_lc = new LengthCounter();
            noise_seq.sequence = 0xDBDB;
        }


        public void Reset()
        {
            // silence all channels
            // clear DMC interrupt flag
            dmc_interrupt = false;
        }


        public void Clock()
        {
            bool quarterFrame = false;
            bool halfFrame = false;

            dGlobalTime += (0.3333333333f / 1789773);

            if (clock_counter % 6 == 0)
            {
                frame_clock_counter++;

                switch (frame_clock_counter)
                {
                    case 3729:
                        quarterFrame = true;
                        break;
                    case 7457:
                        quarterFrame = true;
                        halfFrame = true;
                        break;
                    case 11186:
                        quarterFrame = true;
                        break;
                    case 14916:
                        quarterFrame = true;
                        halfFrame = true;
                        frame_clock_counter = 0;
                        break;
                }

                if (quarterFrame)
                {
                    pulse1_env.Clock(pulse1_halt);
                    pulse2_env.Clock(pulse2_halt);
                    noise_env.Clock(noise_halt);
                }

                if (halfFrame)
                {
                    pulse1_lc.clock(pulse1_enable, pulse1_halt);
                    pulse2_lc.clock(pulse2_enable, pulse2_halt);
                    noise_lc.clock(noise_enable, noise_halt);
                    pulse1_sweep.clock(ref pulse1_seq.reload, 0);
                    pulse2_sweep.clock(ref pulse2_seq.reload, 1);
                }

                //pulse1_sample = (double)pulse1_seq.Clock(u => ((u & 1) << 7) | ((u & 0xFE) >> 1));
                pulse1_osc.frequency = 1789773.0 / (16.0 * (pulse1_seq.reload + 1));
                //pulse1_osc.amplitude = (double)(pulse1_env.output - 1) / 16.0;
                pulse1_osc.amplitude = pulse1_env.output - 1;
                pulse1_sample = pulse1_osc.Sample(dGlobalTime);

                if (pulse1_lc.counter > 0 && pulse1_seq.timer >= 8 && !pulse1_sweep.mute && pulse1_env.output > 2)
                    pulse1_output += (pulse1_sample - pulse1_output) * 0.5f;
                else
                    pulse1_output = 0;

                //pulse1_sample = (double)pulse1_seq.Clock(u => ((u & 1) << 7) | ((u & 0xFE) >> 1));
                pulse2_osc.frequency = 1789773.0 / (16.0 * (pulse2_seq.reload + 1));
                pulse2_osc.amplitude = (double)(pulse1_env.output - 1) / 16.0;
                pulse2_osc.amplitude = pulse1_env.output - 1;
                pulse2_sample = pulse2_osc.Sample(dGlobalTime);


                if (pulse2_lc.counter > 0 && pulse2_seq.timer >= 8 && !pulse2_sweep.mute && pulse2_env.output > 2)
                    pulse2_output += (pulse2_sample - pulse2_output) * 0.5;
                else
                    pulse2_output = 0;

                noise_seq.Clock(noise_enable, s => (((s & 0x0001) ^ ((s & 0x0002) >> 1)) << 14) | ((s & 0x7FFF) >> 1));

                if (noise_lc.counter > 0 && noise_seq.timer >= 8)
                {
                    noise_output = noise_seq.output * (noise_env.output - 1) / 16.0;
                }

                if (!pulse1_enable) pulse1_output = 0;
                if (!pulse2_enable) pulse2_output = 0;
                if (!noise_enable) noise_output = 0;

            }

            pulse1_sweep.track(pulse1_seq.reload);
            pulse2_sweep.track(pulse2_seq.reload);

            clock_counter++;
        }

        public uint Read(uint address)
        {
            uint data = 0;
            if (address == 0x4015)
            {
                data = (uint)(
                    ((pulse1_seq.timer > 0 ? 1 : 0) << 0) |
                    ((pulse2_seq.timer > 0 ? 1 : 0) << 1) |
                    //((0 > 0 ? 1 : 0) << 2) |
                    //((noise_length > 0 ? 1 : 0) << 3) |
                    //((dmc_bytes > 0 ? 1 : 0) << 4) |
                    ((frame_interrupt ? 1 : 0) << 6) |
                    ((dmc_interrupt ? 1 : 0) << 7)
                    );

                frame_interrupt = false;
            }
            return data;
        }

        public void Write(uint address, uint value)
        {
            switch (address)
            {
                case 0x4000:
                    switch ((value & 0xC0) >> 6)
                    {
                        case 0: pulse1_seq.NewSequence = 0b01000000; pulse1_osc.dutycycle = 0.125; break;
                        case 1: pulse1_seq.NewSequence = 0b01100000; pulse1_osc.dutycycle = 0.250; break;
                        case 2: pulse1_seq.NewSequence = 0b01111000; pulse1_osc.dutycycle = 0.500; break;
                        case 3: pulse1_seq.NewSequence = 0b10011111; pulse1_osc.dutycycle = 0.750; break;
                    }
                    pulse1_seq.sequence = pulse1_seq.NewSequence;
                    pulse1_halt = (value & 0x20) > 0;
                    pulse1_env.volume = (value & 0x0F);
                    pulse1_env.disable = (value & 0x10) > 0;
                    break;
                
                case 0x4001:
                    pulse1_sweep.enabled = (value & 0x80) == 0x80;
                    pulse1_sweep.period = (value & 0x70) >> 4;
                    pulse1_sweep.down = (value & 0x08) == 0x08;
                    pulse1_sweep.shift = (byte)(value & 0x07);
                    pulse1_sweep.reload = true;
                    break;
                
                case 0x4002:
                    pulse1_seq.reload = ((pulse1_seq.reload & 0xFF00) | ((int)value & 0xFF));
                    break;
                
                case 0x4003:
                    pulse1_seq.reload = (((int)(value & 7) << 8) | (pulse1_seq.reload & 0x00FF));
                    pulse1_seq.timer = pulse1_seq.reload;
                    pulse1_seq.sequence = pulse1_seq.NewSequence;
                    pulse1_lc.counter = length_table[(value & 0xF8) >> 3];
                    pulse1_env.start = true;
                    break;

                case 0x4004:
                    switch ((value & 0xC0) >> 6)
                    {
                        case 0: pulse2_seq.NewSequence = 0b01000000; pulse2_osc.dutycycle = 0.125; break;
                        case 1: pulse2_seq.NewSequence = 0b01100000; pulse2_osc.dutycycle = 0.250; break;
                        case 2: pulse2_seq.NewSequence = 0b01111000; pulse2_osc.dutycycle = 0.500; break;
                        case 3: pulse2_seq.NewSequence = 0b10011111; pulse2_osc.dutycycle = 0.750; break;
                    }
                    pulse2_seq.sequence = pulse2_seq.NewSequence;
                    pulse2_halt = (value & 0x20) > 0;
                    pulse2_env.volume = (value & 0x0F);
                    pulse2_env.disable = (value & 0x10) > 0;
                    break;

                case 0x4005:
                    pulse2_sweep.enabled = (value & 0x80) == 0x80;
                    pulse2_sweep.period = (value & 0x70) >> 4;
                    pulse2_sweep.down = (value & 0x08) == 0x08;
                    pulse2_sweep.shift = (byte)(value & 0x07);
                    pulse2_sweep.reload = true;
                    break;

                case 0x4006:
                    pulse2_seq.reload = ((pulse2_seq.reload & 0xFF00) | ((int)value & 0xFF));
                    break;

                case 0x4007:
                    pulse2_seq.reload = (((int)(value & 7) << 8) | (pulse2_seq.reload & 0x00FF));
                    pulse2_seq.timer = pulse2_seq.reload;
                    pulse2_seq.sequence = pulse2_seq.NewSequence;
                    pulse2_lc.counter = length_table[(value & 0xF8) >> 3];
                    pulse2_env.start = true;
                    break;

                case 0x400A:
                    triangle_seq.reload = ((triangle_seq.reload & 0xFF00) | ((int)value & 0xFF));
                    break;

                case 0x400B:
                    triangle_seq.reload = (((int)(value & 7) << 8) | (pulse2_seq.reload & 0x00FF));
                    triangle_seq.timer = triangle_seq.reload;
                    triangle_seq.sequence = triangle_seq.NewSequence;
                    triangle_lc.counter = length_table[(value & 0xF8) >> 3];
                    triangle_env.start = true;
                    break;

                case 0x400C:
                    noise_env.volume = (value & 0x0F);
                    noise_env.disable = (value & 0x10) == 0x10;
                    noise_halt = (value & 0x20) == 0x20;
                    break;

                case 0x400E:
                    noise_seq.reload = noise_reload_table[value & 0x0F];
                    break;
                case 0x400F:
                    pulse1_env.start = true;
                    pulse2_env.start = true;
                    noise_env.start = true;
                    noise_lc.counter = length_table[(value & 0xF8) >> 3];
                    break;
                case 0x4015:
                    pulse1_enable = (value & 1) == 1;
                    pulse2_enable = (value & 2) == 2;
                    noise_enable = (value & 4) == 4;
                    break;

            }

            dmc_interrupt = false;
        }

        public double GetOutputSample()
        {
            if (pulse1_output + pulse2_output == 0) 
                return 0;
            return
                95.88 / ((8128.0 / (pulse1_output  + pulse2_output)) + 100.0);
            //((1.0 * (pulse1_output)) - 0.8) * 0.1 +
            //((1.0 * (pulse2_output)) - 0.8) * 0.11
            ////((2.0 * (noise_output) * 64)) * 0.1
        ;
        }
    }


}
