using System;
using System.Runtime.CompilerServices;
using Fami.Core.Audio;

namespace Fami.Core
{
    public enum InterruptTypeEnum
    {
        NMI,
        IRQ,
        BRK
    }

    public partial class Cpu6502State
    {
        public uint[] RAM = new uint[0x800];
        public uint A;
        public uint X;
        public uint Y;
        public uint S;

        public uint PC;
        public uint N;  // bit 7
        public uint V;  // bit 6
        public uint U;  // bit 5
        public uint B;  // bit 4
        public uint D;  // bit 3
        public uint I;  // bit 2
        public uint Z;  // bit 1
        public uint C;  // bit 0

        public uint cycles;
        public uint instructions;


        public uint dma_page;
        public uint dma_address;
        public byte dma_data;

        public bool dma_transfer;
        public bool dma_dummy;

        public uint[] Controller = new uint[2];
        public uint[] ControllerState = new uint[2];

        public uint P
        {
            get =>
                (uint)(
                    (N << 7) +
                    (V << 6) +
                    (U << 5) +
                    (B << 4) +
                    (D << 3) +
                    (I << 2) +
                    (Z << 1) +
                    (C << 0)
                );
            set
            {
                N = (value >> 7 & 1);
                V = (value >> 6 & 1);
                U = (value >> 5 & 1);
                B = (value >> 4 & 1);
                D = (value >> 3 & 1);
                I = (value >> 2 & 1);
                Z = (value >> 1 & 1);
                C = (value >> 0 & 1);
            }
        }


        public uint EffectiveAddr;
        public bool PageBoundsCrossed;

        public Cartridge Cartridge;
        public Ppu Ppu;
        public Apu Apu;

        public bool[] _interrupts = new bool[3];

        public void TriggerInterrupt(InterruptTypeEnum type)
        {
            if (I == 0 || type == InterruptTypeEnum.NMI)
            {
                _interrupts[(int)type] = true;
            }
        }

        public void WriteState(ref CpuState state)
        {
            Array.Copy(RAM, state.RAM, RAM.Length);
            state.A = A;
            state.X = X;
            state.Y = Y;
            state.S = S;

            state.PC = PC;
            state.N = N;  // bit 7
            state.V = V;  // bit 6
            state.U = U;  // bit 5
            state.B = B;  // bit 4
            state.D = D;  // bit 3
            state.I = I;  // bit 2
            state.Z = Z;  // bit 1
            state.C = C;  // bit 0

            state.cycles = cycles;
        }

        public void ReadState(CpuState state)
        {
            Array.Copy(state.RAM, RAM, RAM.Length);
            A = state.A;
            X = state.X;
            Y = state.Y;
            S = state.S;

            PC = state.PC;
            N = state.N;  // bit 7
            V = state.V;  // bit 6
            U = state.U;  // bit 5
            B = state.B;  // bit 4
            D = state.D;  // bit 3
            I = state.I;  // bit 2
            Z = state.Z;  // bit 1
            C = state.C;  // bit 0

            cycles = state.cycles;
        }

        public void Reset()
        {
            Ppu.Reset();
            // https://www.pagetable.com/?p=410
            S = 0xFD; // Actually 0xFF, but 3 Stack Pushes are done with writes supressed, 
            P = 0x24; // Just to align with nestest.log: I is set, U shouldn't exist, but...
            PC = BusRead(0xFFFC) + BusRead(0xFFFD) * 0x100; // Fetch the reset vector
            cycles = 7; // takes 7 cycles to reset
        }

        public uint BusRead(uint address)
        {
            uint data = 0;
            var (val, handled) = Cartridge.CpuRead(address);
            if (handled)
            {
                data = val;
            }
            else if (address < 0x2000)
            {
                data = RAM[address & 0x07FF];
            }
            else if (address >= 0x2000 && address <= 0x3FFF)
            {
                data = Ppu.Read(address);
            }
            else if (address == 0x4015)
            {
                data = Apu.Read(address);
            }
            else if (address >= 0x4016 && address <= 0x4017)
            {
                data = (ControllerState[address & 0x0001] & 0x80) > 0 ? 1U : 0;
                ControllerState[address & 0x0001] <<= 1;
            }
            return data;
        }


        public void BusWrite(uint address, uint value)
        {
            var handled = Cartridge.CpuWrite(address, value);
            if (handled)
            {

            }
            else if (address >= 0x0000 && address <= 0x1FFF)
            {
                RAM[address & 0x07FF] = value;
            }
            else if (address >= 0x2000 && address <= 0x3000)
            {
                Ppu.Write(address, value);
            }
            else if ((address >= 0x4000 && address <= 0x4013) || address == 0x4015 || address == 0x4017)
            {
                Apu.Write(address, value);
            }
            else if (address == 0x4014)
            {
                dma_page = value;
                dma_address = 0x00;
                dma_transfer = true;
                //dma_dummy = cycles % 2 == 1;
                //for (uint i = 0; i <= 255; i++)
                //{
                //    var _dma_data = (byte)(BusRead(dma_page << 8 | i));
                //    Ppu.pOAM[i] = _dma_data;
                //}
                //cycles += 513 + cycles % 2;
            }
            else if (address >= 0x4016 && address <= 0x4017)
            {
                ControllerState[address & 0x1] = Controller[address & 01];
            }
        }


        public uint NonMaskableInterrupt()
        {
            Push((PC >> 8) & 0xFF); // Push the high byte of the PC
            Push((PC & 0xFF)); // Push the low byte of the PC
            B = 0;
            U = 1;
            I = 1;
            Push(P);
            PC = BusRead(0xFFFA) + BusRead(0xFFFB) * 0x100; // Jump to NMI handler
            return 7;
        }


        public uint InterruptRequest()
        {
            Push((PC >> 8) & 0xFF); // Push the high byte of the PC
            Push((PC & 0xFF)); // Push the low byte of the PC
            B = 0;
            U = 1;
            I = 1;
            Push(P);
            PC = BusRead(0xFFFE) + BusRead(0xFFFF) * 0x100; // Jump to IRQ handler
            return 7;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint Push(uint value)
        {
            BusWrite(S + 0x100, value);
            S -= 1;
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint Pop()
        {
            S += 1;
            return BusRead(S + 0x100);
        }
    }
}
