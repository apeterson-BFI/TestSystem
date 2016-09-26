using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestSystem
{
    // 16 bit registers
    // 8 bit instruction word size
    // 16 bit data word size
    // 16 bit address size
    // A register (accumulator)
    // B register (secondary)
    // I register (index)
    // IC instruction counter address
    public class RegisterContext
    {
        Int16 A;
        Int16 B;
        Int16 I;
        UInt16 IC;
        Byte[] memory;
        UInt16 MAX_MEMORY_INDEX;
        // bool terminate;
        // UInt16 returnVal;
        List<object> outs;

        public RegisterContext(Byte[] minit)
        {
            A = 0;
            B = 0;
            I = 0;
            IC = 0;

            MAX_MEMORY_INDEX = ((ushort)(memory.Length - 1));

            memory = new byte[memory.Length];

            for (int i = 0; i < memory.Length; i++)
            {
                memory[i] = minit[i];
            }

            outs = new List<object>();
        }

        public void Run()
        {
            //while (!terminate)
            {
                Step();
            }
        }

        public void Step()
        {
            byte opcode = memory[IC];
            ExecuteOpcode(opcode);
            IC++;
        }

        public void ExecuteOpcode(byte opcode)
        {
            // Legend:
            // * = contents at address (16b contents unless specified otherwise)
            // * on left hand side of an assignment, means the address's contents become
            // Indexing Scale: 1 -> 8 bits
            // B|A 32 Joined Register Value -> B is high 16 bits, A is low 16 bits

            switch (opcode)
            {
                // 0x00 NOP                      [0b]   "No Effect"
                case 0x00:
                    break;
                // 0x01 INCA                     [0b]   "A = A + 1"
                case 0x01:
                    A = A++;
                    break;
                // 0x02 LDA IMMEDIATE           [16b]   "A = IMMEDIATE"
                case 0x02:
                    A = GetImmediate();
                    break;
                // 0x03 LDA ADDRESS16           [16b]   "A = *ADDRESS16"
                case 0x03:
                    {
                        UInt16 uadr = GetAddressOp();
                        A = GetWordAtAddress(uadr);
                        break;
                    }
                // 0x04 LDA ADDRESS16 + I       [16b]   "A = *(ADDRESS16 + I)"
                case 0x04:
                    {
                        UInt16 uadr = GetAddressOp();
                        A = GetWordAtAddress((ushort)(uadr + I));
                        break;
                    }
                // 0x05 LDA DIRECT               [0b]   "A = *A"
                case 0x05:
                    {
                        UInt16 address = SignedToUnsigned(A);
                        A = GetWordAtAddress(address);
                        break;
                    }
                // 0x06 MOVA ADDRESS16          [16b]   "*ADDRESS16 = A"
                case 0x06:
                    {
                        UInt16 address = GetAddressOp();

                        if (!CheckAddressRange(address, 16))
                            throw new ArgumentException("Invalid Indirection Target");

                        byte[] aby = BitConverter.GetBytes(A);

                        memory[address] = aby[0];
                        memory[address + 1] = aby[1];
                        break;
                    }
                // 0x07 MOVA ADDRESS16 + I      [16b]   "*(ADDRESS16 + I] = A"
                case 0x07:
                    {
                        UInt16 address = (ushort)(GetAddressOp() + I);

                        if (!CheckAddressRange(address, 16))
                            throw new ArgumentException("Invalid Indirection Target");

                        byte[] aby = BitConverter.GetBytes(A);

                        memory[address] = aby[0];
                        memory[address + 1] = aby[1];
                        break;
                    }
                // 0x08 LDB IMMEDIATE           [16b]   "B = IMMEDIATE"
                case 0x08:
                    B = GetImmediate();
                    break;
                // 0x09 LDB ADDRESS16           [16b]   "B = *ADDRESS16"
                case 0x09:
                    {
                        UInt16 uadr = GetAddressOp();
                        B = GetWordAtAddress(uadr);
                        break;
                    }
                // 0x0A LDB ADDRESS16 + I       [16b]   "B = *(ADDRESS16 + I)"
                case 0x0A:
                    {
                        UInt16 uadr = GetAddressOp();
                        B = GetWordAtAddress((ushort)(uadr + I));
                        break;
                    }
                // 0x0B LDB DIRECT               [0b]   "B = *B"
                case 0x0B:
                    {
                        UInt16 address = SignedToUnsigned(B);
                        B = GetWordAtAddress(address);
                        break;
                    }
                // 0x0C MOVB ADDRESS16          [16b]   "*ADDRESS16 = B"
                case 0x0C:
                    {
                        UInt16 address = GetAddressOp();

                        if (!CheckAddressRange(address, 16))
                            throw new ArgumentException("Invalid Indirection Target");

                        byte[] aby = BitConverter.GetBytes(B);

                        memory[address] = aby[0];
                        memory[address + 1] = aby[1];
                        break;
                    }
                // 0x0D MOVB ADDRESS16 + I      [16b]   "*(ADDRESS16 + I) = B"
                case 0x0D:
                    {
                        UInt16 address = (ushort)(GetAddressOp() + I);

                        if (!CheckAddressRange(address, 16))
                            throw new ArgumentException("Invalid Indirection Target");

                        byte[] aby = BitConverter.GetBytes(B);

                        memory[address] = aby[0];
                        memory[address + 1] = aby[1];
                        break;
                    }
                // 0x0E MOVAB                    [0b]   "B = A"
                case 0x0E:
                    B = A;
                    break;
                // 0x0F MOVAI                    [0b]   "I = A"
                case 0x0F:
                    I = A;
                    break;
                // 0x10 MOVBA                    [0b]   "A = B"
                case 0x10:
                    A = B;
                    break;
                // 0x11 MOVBI                    [0b]   "I = B"
                case 0x11:
                    I = B;
                    break;
                // 0x12 MOVIA                    [0b]   "A = I"
                case 0x12:
                    A = I;
                    break;
                // 0x13 MOVIB                    [0b]   "B = I"
                case 0x13:
                    B = I;
                    break;
                // 0x14 ADDAB                    [0b]   "A = A + B"
                case 0x14:
                    A = (short)(A + B);
                    break;
                // 0x15 SUBAB                    [0b]   "A = A - B"
                case 0x15:
                    A = (short)(A - B);
                    break;
                // 0x16 MULAB                    [0b]   "BA = A * B : B -> 16 bits high order, A -> 16 bits low order"
                case 0x16:
                    {
                        // If a * b < 0, and not overflowing into B, then B must = -1
                        // if a * b > 0, and not overflowing, then B = 0
                        int product = A * B;
                        byte[] pby = BitConverter.GetBytes(product);

                        A = BitConverter.ToInt16(pby, 0);   // low order bytes
                        B = BitConverter.ToInt16(pby, 2);   // high order bytes

                        break;
                    }
                // 0x17 DIVAB                    [0b]   "A = quotient of A \ B, B = remainder A % B"
                case 0x17:
                    {
                        B = (short)(A % B);
                        A = (short)(A / B);
                        break;
                    }
                // 0x18 IFAZ ADDRESS16          [16b]   "IF(A == 0) IC = ADDRESS16"
                case 0x18:
                    {
                        UInt16 add = GetAddressOp();

                        if (A == 0)
                            IC = (ushort)(add - 1);

                        break;
                    }
                // 0x19 IFAZ ADDRESS16 + I      [16b]   "IF(A == 0) IC = ADDRESS16 + I"
                // 0x1A IFAZ [ADDRESS16]        [16b]   "IF(A == 0) IC = *(ADDRESS16)"
                // 0x1B IFAZ ADDRESS16 RELATIVE [16b]   "IF(A == 0) IC = IC + ((SIGNED INT16) ADDRESS16)"
                // 0x1C IFANZ ADDRESS16         [16b]   "IF(A != 0) IC = ADDRESS16"
                // 0x1D IFANZ ADDRESS16 + I     [16b]   "IF(A != 0) IC = ADDRESS16 + I"
                // 0x1E IFANZ [ADDRESS16]       [16b]   "IF(A != 0) IC = *(ADDRESS16)"
                // 0x1F IFANZ ADDRESS16 RELATIVE[16b]   "IF(A != 0) IC = IC + ((SIGNED INT16) ADDRESS16)"
                // 0x20 IFIZ ADDRESS16          [16b]   "IF(I == 0) IC = ADDRESS16"
                // 0x21 IFIZ [ADDRESS16]        [16b]   "IF(I == 0) IC = *(ADDRESS16)"
                // 0x22 IFIZ ADDRESS16 RELATIVE [16b]   "IF(I == 0) IC = IC + ((SIGNED INT16) ADDRESS16)"
                // 0x23 IFINZ ADDRESS16         [16b]   "IF(I != 0) IC = ADDRESS16"
                // 0x24 IFINZ [ADDRESS16]       [16b]   "IF(I != 0) IC = *(ADDRESS16)"
                // 0x25 IFINZ ADDRESS16 RELATIVE[16b]   "IF(I != 0) IC = IC + ((SIGNED INT16) ADDRESS16)"
                // 0x26 TOGA                     [0b]   "IF(A == 0) A = 1 ELSE A = 0"
                // 0x27 TOGB                     [0b]   "IF(B == 0) B = 1 ELSE B = 0"
                // 0x28 TOGI                     [0b]   "IF(I == 0) I = 1 ELSE I = 0"
                // 0x29 MULF ADDRESS16[READ32]  [16b]   "(IEEE FLOAT): B|A = B|A * ((*ADDRESS16) [READ32])"
                // 0x2A DIVF ADDRESS16[READ32]  [16b]   "(IEEE FLOAT): B|A = B|A / ((*ADDRESS16) [READ32])"
                // 0x2B ADDF ADDRESS16[READ32]  [16b]   "(IEEE FLOAT): B|A = B|A + ((*ADDRESS16) [READ32])"
                // 0x2C SUBF ADDRESS16[READ32]  [16b]   "(IEEE FLOAT): B|A = B|A + ((*ADDRESS16) [READ32])"
                // 0x2D POWF ADDRESS16[READ32]  [16b]   "(IEEE FLOAT): B|A = B|A ^ ((*ADDRESS16) [READ32])"
                // 0x2E POWAB                    [0b]   "A = A ^ B"
                // 0x2F LNF                      [0b]   "(IEEE FLOAT): B|A = Ln(B|A)"
                // 0x30 EXPF                     [0b]   "(IEEE FLOAT): B|A = e ^ B|A"
                // 0x31 SINF                     [0b]   "(IEEE FLOAT): B|A = Sin(B|A)"
                // 0x32 COSF                     [0b]   "(IEEE FLOAT): B|A = Cos(B|A)"
                // 0x33 TANF                     [0b]   "(IEEE FLOAT): B|A = Tan(B|A)"
                // 0x34 ASINF                    [0b]   "(IEEE FLOAT): B|A = Asin(B|A)"
                // 0x35 ACOSF                    [0b]   "(IEEE FLOAT): B|A = Acos(B|A)"
                // 0x36 ATANF                    [0b]   "(IEEE FLOAT): B|A = Atan(B|A)"
                // 0x37 SINHF                    [0b]   "(IEEE FLOAT): B|A = Sinh(B|A)"
                // 0x38 COSHF                    [0b]   "(IEEE FLOAT): B|A = Cosh(B|A)"
                // 0x39 TANHF                    [0b]   "(IEEE FLOAT): B|A = Tanh(B|A)"
                // 0x3A ATANHF                   [0b]   "(IEEE FLOAT): B|A = Atanh(B|A)"
                // 0x3B ASINHF                   [0b]   "(IEEE FLOAT): B|A = Asinh(B|A)"
                // 0x3C ACOSHF                   [0b]   "(IEEE FLOAT): B|A = Acosh(B|A)"
                // 0x3D LOGF                     [0b]   "(IEEE FLOAT): B|A = Log10(B|A)"
                // 0x3E LOGF ADDRESS16[READ32]  [16b]   "(IEEE FLOAT): B|A = Ln(B|A) / Ln((*ADDRESS16) [READ32])"
                // 0x3F FAC                      [0b]   "A = A!"
                // 0x40 OUTC                     [0b]   "Console.Print('A') : ascii low order on A"
                // 0x41 OUTI                     [0b]   "Console.Print(A) : integer printout"
                // 0x42 OUTF                     [0b]   "(IEEE FLOAT): Console.Print(A) : float printout"
                // 0x43 OUTSZ ADDRESS16         [16b]   "Console.Print(*A) : as zero-terminated string"
                // 0x44 FTOI                     [0b]   "A = ROUND(B|A) TO INT"
                // 0x45 ITOF                     [0b]   "B|A = A TO FLOAT"
                // 0x46 NEGA                     [0b]   "A = -A"
                // 0x47 NEGB                     [0b]   "B = -B"
                // 0x48 NEGFA                    [0b]   "(IEEE FLOAT): A = -A"
                // 0x49 NEGFB                    [0b]   "(IEEE FLOAT): B = -B"
                // 0x4A ABSA                     [0b]   "A = |A|"
                // 0x4B ABSB                     [0b]   "B = |B|"
                // 0x4C ABSFA                    [0b]   "(IEEE FLOAT): A = |A|"
                // 0x4D ABSFB                    [0b]   "(IEEE FLOAT): B = |B|"
                // 0x4E CAT ADDRESS16 ADDRESS16 [32b]   "*(ADDRESS16) + *(ADDRESS16). Adds the contents of the second zero-term string to the first, consecutively"
                // 0x4F SHLAB                    [0b]   "A = A << B"
                // 0x50 SHRAB                    [0b]   "A = A >> B"
                // 0x51 FLF                      [0b]   "(IEEE FLOAT): B|A = [_B|A_]"
                // 0x52 CLF                      [0b]   "(IEEE FLOAT): B|A = [=B|A=]"
                // 0x53 EXIT                     [0b]   "System.Exit(0)"
                // 0x54 EXIT WITH IMMEDIATE      [0b]   "System.Exit(IMMEDIATE)"
                // 0x55 EXIT WITH A              [0b]   "System.Exit(A)"
                // 0x56 NEWL                     [0b]   "Console.Out.WriteLine()"
                // 0x57 EXPUNGE                  [0b]   "Remove last character written to Console Out"
                // 0x58 FLUSH                    [0b]   "Empty the console out stream"
                // 0x59 INCI                     [0b]   "I++"
                // 0x5A INCB                     [0b]   "B++"

                default:
                    break;
            }
        }

        public Int16 GetImmediate()
        {
            byte[] imm = new byte[2];
            imm[0] = memory[IC + 1];
            imm[1] = memory[IC + 2];
            return(BitConverter.ToInt16(imm, 0));
        }

        public UInt16 GetAddressOp()
        {
            byte[] adr = new byte[2];
            adr[0] = memory[IC + 1];
            adr[1] = memory[IC + 2];
            return(BitConverter.ToUInt16(adr, 0));
        }

        public Int16 GetWordAtAddress(UInt16 address)
        {
            if (!CheckAddressRange(address))
                throw new ArgumentException("Invalid Indirection Target");

            byte[] imm = new byte[2];
            imm[0] = memory[(int)address];
            imm[1] = memory[((int)address) + 1];
            return(BitConverter.ToInt16(imm, 0));
        }

        public bool CheckAddressRange(UInt16 address)
        {
            if (address > MAX_MEMORY_INDEX)
                return false;
            else
                return true;
        }

        public bool CheckAddressRange(UInt16 address, int bytes)
        {
            int temp = address + bytes / 8 - 1;

            if (temp > MAX_MEMORY_INDEX)
                return false;
            else
                return true;
        }

        public UInt16 SignedToUnsigned(Int16 s)
        {
            byte[] ub = BitConverter.GetBytes(s);
            return(BitConverter.ToUInt16(ub, 2));
        }

        public Int16 UnsignedToSigned(UInt16 u)
        {
            byte[] sb = BitConverter.GetBytes(u);
            return (BitConverter.ToInt16(sb, 2));
        }
    }
}
