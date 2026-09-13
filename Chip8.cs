using SDL3;

namespace Chip8_Emu;

public class Chip8
{
  ushort opcode; // Chip8 has 35 opcodes, 2 bytes long each
  byte[] memory = new byte[4096]; // Chip8  has 4k memory in total

  // CPU Registers, 15 8-bit registers, from V0 to VE
  byte[] V = new byte[16];

  // Index register and the program counter
  ushort I,
    pc;

  // The screen is black and white and has 2048 pixels (64x32)
  internal byte[] gfx = new byte[64 * 32];

  // delay timer and sound timer
  byte delay_timer,
    sound_timer;

  // the stack and a stack pointer
  ushort[] stack = new ushort[16];
  ushort sp;

  // Chip8 keypad (HEX)
  byte[] keypad = new byte[16];

  // fontset

  readonly byte[] chip8_fontset =
  [
    0xF0, 0x90, 0x90, 0x90, 0xF0, // 0
    0x20, 0x60, 0x20, 0x20, 0x70, // 1
    0xF0, 0x10, 0xF0, 0x80, 0xF0, // 2
    0xF0, 0x10, 0xF0, 0x10, 0xF0, // 3
    0x90, 0x90, 0xF0, 0x10, 0x10, // 4
    0xF0, 0x80, 0xF0, 0x10, 0xF0, // 5
    0xF0, 0x80, 0xF0, 0x90, 0xF0, // 6
    0xF0, 0x10, 0x20, 0x40, 0x40, // 7
    0xF0, 0x90, 0xF0, 0x90, 0xF0, // 8
    0xF0, 0x90, 0xF0, 0x10, 0xF0, // 9
    0xF0, 0x90, 0xF0, 0x90, 0x90, // A
    0xE0, 0x90, 0xE0, 0x90, 0xE0, // B
    0xF0, 0x80, 0x80, 0x80, 0xF0, // C
    0xE0, 0x90, 0x90, 0x90, 0xE0, // D
    0xF0, 0x80, 0xF0, 0x80, 0xF0, // E
    0xF0, 0x80, 0xF0, 0x80, 0x80  // F
  ];

  // Drawflag
  internal bool Drawflag;

  // Random num generator
  Random rand = new Random();

  void Read_rom()
  {
    try
    {
      using FileStream fs = File.OpenRead("rom.ch8");
      int bytesRead = fs.Read(memory, 512, 3584);
      Console.WriteLine("ROM successfully loaded");
    }
    catch (Exception ex)
    {
      Console.Error.WriteLine($"Error: {ex.Message}");
      Console.Error.WriteLine("ROM failed to load");
      Environment.Exit(1);
    }
  }

  internal void Initializer()
  {
    pc = 0x200; // program counter starts at 0x200
    opcode = 0; // reset current opcode
    I = 0; // reset index register
    sp = 0; // reset stack pointer

    // clear display, stack, registers and memory

    Array.Clear(gfx, 0, gfx.Length);
    Array.Clear(stack, 0, stack.Length);
    Array.Clear(V, 0, V.Length);
    Array.Clear(memory, 0, memory.Length);

    // load fontset

    for (int i = 0; i < 80; i++)
    {
      memory[i] = chip8_fontset[i];
    }

    Read_rom();

    
  }

  internal void EmulateCycle()
  {
    // fetch opcode
    opcode = (ushort)(memory[pc] << 8 | memory[pc + 1]);
    pc += 2;

    // decode opcode

    switch (opcode & 0xF000)
    {
      case 0x0000:
        switch (opcode & 0x000F)
        {
          case 0x0000: // (00E0) clear the display
            Array.Clear(gfx, 0, gfx.Length);
            Drawflag = true;

            break;

          case 0x000E: // (00EE) return from a subroutine
            pc = stack[sp];
            sp--;

            break;

          default:
            Console.Error.WriteLine($"Unknown opcode [0x0000]: 0x{opcode}X");

            break;
        }

        break;

      case 0x1000: // (1NNN) jump to location nnn
        pc = (ushort)(opcode & 0xFFF);
        break;

      case 0x2000: // (2NNN) call subroutine at nnn
        sp++;
        stack[sp] = pc;
        pc = (ushort)(opcode & 0x0FFF);
        break;

      case 0x3000: // (3XKK) skip next instruction if Vx = kk
        if (V[(opcode & 0x0F00) >> 8] == (opcode & 0x00FF))
          pc += 2;

        break;

      case 0x4000: // (4XKK) skip next instruction if Vx != kk
        if (V[(opcode & 0x0F00) >> 8] != (opcode & 0x00FF))
          pc += 2;

        break;

      case 0x5000: // (5XY0) skip next instruction if Vx = Vy
        if (V[(opcode & 0x0F00) >> 8] == V[(opcode & 0x00F0) >> 4])
          pc += 2;

        break;

      case 0x6000: // (6XKK) set Vx = kk
        V[(opcode & 0x0F00) >> 8] = (byte)(opcode & 0x00FF);

        break;

      case 0x7000: // (set Vx = Vx + kk)
        V[(opcode & 0x0F00) >> 8] += (byte)(opcode & 0x00FF);

        break;

      case 0x8000:

        switch (opcode & 0x000F)
        {
          case 0x0000: // (8XY0) set Vx = Vy
            V[(opcode & 0x0F00) >> 8] = V[(opcode & 0x00F0) >> 4];

            break;

          case 0x0001: // (8XY1) set Vx = Vx OR Vy
            V[(opcode & 0x0F00) >> 8] = (byte)(
              V[(opcode & 0x0F00) >> 8] | V[(opcode & 0x00F0) >> 4]
            );


            break;

          case 0x0002: // (8XY2) set Vx = Vx AND Vy
            V[(opcode & 0x0F00) >> 8] = (byte)(
              V[(opcode & 0x0F00) >> 8] & V[(opcode & 0x00F0) >> 4]
            );


            break;

          case 0x0003: // (8XY3) set Vx = Vx XOR Vy
            V[(opcode & 0x0F00) >> 8] = (byte)(
              V[(opcode & 0x0F00) >> 8] ^ V[(opcode & 0x00F0) >> 4]
            );


            break;

          case 0x0004: // (8XY4) set Vx = Vx+ Vy, set VF = carry
            // creates 2 variables to simplify the overflow check
            // could have used try catch
            byte X = V[(opcode & 0x0F00) >> 8];
            byte Y = V[(opcode & 0x00F0) >> 4];
            V[(opcode & 0x0F00) >> 8] += V[(opcode & 0x00F0) >> 4];
            if ((X / 2) + (Y / 2) > 254 / 2)
              V[0xF] = 1;
            else
              V[0xF] = 0;

            break;

          case 0x0005: // (8XY5) set Vx = Vx - Vy, set VF = NOT borrow
            if (V[(opcode & 0x0F00) >> 8] >= V[(opcode & 0x00F0) >> 4])
            {
              V[(opcode & 0x0F00) >> 8] -= V[(opcode & 0x00F0) >> 4];
              V[0xF] = 1;
            }
            else
            {
              V[(opcode & 0x0F00) >> 8] -= V[(opcode & 0x00F0) >> 4];
              V[0xF] = 0;
            }

            break;

          case 0x0006: // (8XY6) set Vx = Vx SHR
            if ((V[(opcode & 0x0F00) >> 8] & 0x01) == 1)
            {
              V[(opcode & 0x0F00) >> 8] = (byte)(V[(opcode & 0x0F00) >> 8] >> 1);
              V[0xF] = 1;
            }
            else
            {
              V[(opcode & 0x0F00) >> 8] = (byte)(V[(opcode & 0x0F00) >> 8] >> 1);
              V[0xF] = 0;
            }


            break;

          case 0x0007: // (8XY7) set Vx = Vy - Vx, set VF = NOT borrow
            if (V[(opcode & 0x0F00) >> 8] <= V[(opcode & 0x00F0) >> 4])
            {
              V[(opcode & 0x0F00) >> 8] = (byte)(V[(opcode & 0x00F0) >> 4] - V[(opcode & 0x0F00) >> 8]);
              V[0xF] = 1;
            }
            else
            {
              V[(opcode & 0x0F00) >> 8] = (byte)(V[(opcode & 0x00F0) >> 4] - V[(opcode & 0x0F00) >> 8]);
              V[0xF] = 0;
            }


            break;

          case 0x000E: // (8XYE) set Vx = Vx SHL 1
            if ((V[(opcode & 0x0F00) >> 8] >> 7) == 1)
            {
              V[(opcode & 0x0F00) >> 8] = (byte)(V[(opcode & 0x0F00) >> 8] << 1);
              V[0xF] = 1;
            }
            else
            {
              V[(opcode & 0x0F00) >> 8] = (byte)(V[(opcode & 0x0F00) >> 8] << 1);
              V[0xF] = 0;
            }

            break;

          default:
            Console.Error.WriteLine($"Unknown opcode [0x8000]: 0x{opcode}X");

            break;
        }

        break;

      case 0x9000: // (9XY0) set Vx = Vx SHL 1
        if (V[(opcode & 0x0F00) >> 8] != V[(opcode & 0x00F0) >> 4])
          pc += 2;

        break;

      case 0xA000: // (ANNN) set I = nnn
        I = (ushort)(opcode & 0x0FFF);

        break;

      case 0xB000: // (BNNN) jump to location nnn + V0
        pc = (ushort)((opcode & 0x0FFF) + V[0]);
        break;

      case 0xC000: // (CXKK) set Vx = random byte and kk
        V[(opcode & 0x0F00) >> 8] = (byte)((byte)rand.Next() & (opcode & 0x0FFF));

        break;

      case 0xD000: // (DYNX) display n-byte sprite starting at
        // memory location I at (Vx, Vy), set VF = collision
        ushort x = V[(opcode & 0x0F00) >> 8];
        ushort y = V[(opcode & 0x00F0) >> 4];
        ushort height = (ushort)(opcode & 0x000F);
        ushort pixel;

        V[0xF] = 0;
        for (byte yline = 0; yline < height; yline++)
        {
          pixel = memory[I + yline];
          for (int xline = 0; xline < 8; xline++)
          {
            if ((pixel & (0x80 >> xline)) != 0)
            {
              if (gfx[x + xline + ((y + yline) * 64)] == 1)
                V[0xF] = 1;
              gfx[x + xline + ((y + yline) * 64)] ^= 1;
            }
          }
        }

        Drawflag = true;

        break;

      case 0xE000:
        switch (opcode & 0x000F)
        {
          case 0x000E: // (EX9E) skip next instruction if a key with the value of Vx is pressed.

            break;

          case 0x0001: // (EXA1) skip next instruction if a key with the value of Vx is not pressed.

            break;


          default:
            Console.Error.WriteLine($"Unknown opcode [0xE000]: 0x{opcode}X");

            break;
        }

        break;

      case 0xF000:
        switch (opcode & 0x00FF)
        {
          case 0x0007: // (FX07) set Vx = delay timer value
            V[(opcode & 0x0F00) >> 8] = delay_timer;

            break;

          case 0x000A: // (FX0A) wait for a key press, store the value of the key in Vx

            break;

          case 0x0015: // (FX15) set delay timer = Vx
            delay_timer = V[(opcode & 0x0F00) >> 8];

            break;

          case 0x0018: // (FX18) set sound timer = Vx
            sound_timer = V[(opcode & 0x0F00) >> 8];

            break;

          case 0x001E: // (FX1E) set I = I + Vx
            I += V[(opcode & 0x0F00) >> 8];

            break;

          case 0x0029: // (FX29) set I location of sprite digit Vx
            I = gfx[V[(opcode & 0x0F00) >> 8]];

            break;

          case 0x0033: // (FX33) store bcd representation of Vx in memory locations I, I+1 and I+2

            memory[I] = (byte)(V[(opcode & 0x0F00) >> 8] / 100);
            memory[I + 1] = (byte)(V[(opcode & 0x0F00) >> 8] / 10 % 10);
            memory[I + 2] = (byte)(V[(opcode & 0x0F00) >> 8] % 100 % 10);

            break;

          case 0x0055: // (FX55) store registers V0 through Vx in memory starting at location I
            for (int i = 0; i <= (opcode & 0x0F00) >> 8; i++)
              memory[I + i] = V[i];

            break;

          case 0x0065: // (FX65) read registers V0 through Vx from memory starting at location I

            for (byte i = 0; i <= (opcode & 0x0F00) >> 8; i++)
              V[i] = memory[I + i];

            break;

          default:
            Console.Error.WriteLine($"Unknown opcode [0xF000]: 0x{opcode}X");

            break;
        }

        break;

      default:
        Console.Error.WriteLine($"Unknown opcode: 0x{opcode}X");

        break;
    }

    // update timers
    if (delay_timer > 0)
      delay_timer--;
    if (sound_timer > 0)
    {
      if (sound_timer == 1)
        Console.WriteLine("BEEP!");
      sound_timer--;
    }
  }
}

// claude vibecoded screen terminal test
class Chip8Display
{
  const int Width = 64;
  const int Height = 32;
  bool initialized = false;

  public void Render(byte[] display) // 64*32 array, 0 or 1 per pixel
  {
    if (!initialized)
    {
      Console.CursorVisible = false;
      Console.Clear();
      initialized = true;
    }

    for (int y = 0; y < Height; y++)
    {
      Console.SetCursorPosition(0, y);
      var row = new System.Text.StringBuilder(Width);
      for (int x = 0; x < Width; x++)
        row.Append(display[y * Width + x] != 0 ? '#' : ' ');
      Console.Write(row.ToString());
    }
  }
}
