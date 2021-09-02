using System;
using System.Collections.Generic;
using System.Text;

namespace Fami.Core
{
    public delegate void AudioCallback(short[] samples);

    public class Sequencer
    {
        public uint Sequence;
        public int Timer;
        public int Reload;
        public uint Output;
        public bool Enabled;
        public uint NewSequence { get; set; }

        public uint Clock(Func<uint, uint> manip)
        {
            if (Enabled)
            {
                Timer--;
                if (Timer < 0)
                {
                    Timer = Reload;
                    Sequence = manip(Sequence);
                    Output = Sequence & 1;
                }
                return Output;
            }

            return 0;
        }
    }

    public class Apu
    {
        private Cpu6502State _cpu;
        private readonly AudioCallback _audioCallback;
        private Sequencer _pulse1;

        public Apu(Cpu6502State cpu, AudioCallback audioCallback)
        {
            _cpu = cpu;
            _audioCallback = audioCallback;
            _pulse1 = new Sequencer();
        }

        private uint frame_clock_counter;
        private uint clock_counter;


        private bool dmc_interrupt;
        private bool frame_interrupt;
        private uint pulse1_length;
        private uint pulse2_length;
        private uint triangle_length;
        private uint noise_length;
        private uint dmc_bytes;

        public void Reset()
        {
            // silence all channels
            // clear DMC interrupt flag
            pulse1_length = 0;
            pulse2_length = 0;
            triangle_length = 0;
            noise_length = 0;
            dmc_bytes = 0;
            dmc_interrupt = false;
        }

        private uint bufferSize = 1024;
        private short[] buffer = new short[1024];
        private uint _samples = 0;
        private int t;
        private double pulse1_sample;

        private Random rnd = new Random();
        private int ctr;
        private int value = 0;
        private double sweep = 0;
        public void Clock()
        {
            bool quarterFrame = false;
            bool halfFrame = false;

            if (clock_counter % 6 == 0)
            {
                frame_clock_counter++;

                if (frame_clock_counter == 3729)
                {
                    quarterFrame = true;
                }
                if (frame_clock_counter == 7457)
                {
                    quarterFrame = true;
                    halfFrame = true;
                }
                if (frame_clock_counter == 11186)
                {
                    quarterFrame = true;
                }

                if (quarterFrame)
                {

                }

                if (halfFrame)
                {

                }

                pulse1_sample = (double)_pulse1.Clock(u => ((u & 1) << 7) | ((u & 0xFE) >> 1)) * 100;

                //buffer[_samples++] = (short)(_pulse1.Output * 128);
                //buffer[_samples++] = (short)(value * 128);

                //pulse1_sample = (double)_pulse1.Output * 100;

                pulse1_sample = Math.Sin(sweep * frame_clock_counter * 3.141592654 / 180) * 20;

                buffer[_samples++] = (short)(pulse1_sample * 64);
                buffer[_samples++] = (short)(pulse1_sample * 64);

                if (_samples >= bufferSize)
                {
                    _audioCallback(buffer);
                    _samples = 0;
                }
            }

            clock_counter++;
        }

        public uint Read(uint address)
        {
            uint data = 0;
            if (address == 0x4015)
            {
                data = (uint)(
                    ((pulse1_length > 0 ? 1 : 0) << 0) |
                    ((pulse2_length > 0 ? 1 : 0) << 1) |
                    ((triangle_length > 0 ? 1 : 0) << 2) |
                    ((noise_length > 0 ? 1 : 0) << 3) |
                    ((dmc_bytes > 0 ? 1 : 0) << 4) |
                    ((frame_interrupt ? 1 : 0) << 6) |
                    ((dmc_interrupt ? 1 : 0) << 7)
                    );

                frame_interrupt = false;
            }
            return data;
        }

        private uint freq1;

        public void Write(uint address, uint value)
        {
            if (address == 0x4000)
            {
                switch ((value & 0xC0) >> 6)
                {
                    case 0: _pulse1.NewSequence = 0b01000000; break;
                    case 1: _pulse1.NewSequence = 0b01100000; break;
                    case 2: _pulse1.NewSequence = 0b01111000; break;
                    case 3: _pulse1.NewSequence = 0b10011111; break;
                }
                _pulse1.Sequence = _pulse1.NewSequence;
            }
            if (address == 0x4002)
            {
                _pulse1.Reload = ((_pulse1.Reload & 0xFF00) | ((int)value & 0xFF));
            }
            if (address == 0x4003)
            {
                _pulse1.Reload = (((int)(value & 7) << 8) | (_pulse1.Reload & 0x00FF));
                sweep = _pulse1.Reload / 20D;
                _pulse1.Timer = _pulse1.Reload;
                _pulse1.Sequence = _pulse1.NewSequence;
            }
            if (address == 0x4015)
            {
                _pulse1.Enabled = (value & 1) == 1;

                //if ((value & 1) == 0) pulse1_length = 0;
                //if (((value >> 1) & 1) == 0) pulse2_length = 0;
                //if (((value >> 2) & 1) == 0) triangle_length = 0;
                //if (((value >> 3) & 1) == 0) noise_length = 0;
                //switch ((value >> 4) & 1)
                //{
                //    case 0:
                //        dmc_bytes = 0;
                //        return;
                //    case 1:
                //        dmc_bytes = 0;
                //        return;
                //}

                dmc_interrupt = false;
            }
        }
    }


}
