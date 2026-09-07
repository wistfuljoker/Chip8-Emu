namespace Chip8_Emu;


class Chip8
{
  ushort opcode; // Chip8 has 35 opcodes, 2 bytes long each
  byte[] memory = new byte[4096]; // Chip8  has 4k memory in total

  // CPU Registers, 15 8-bit registers, from V0 to VE
  byte[] V = new byte[16];

  // Index register and the program counter
  ushort I, pc;

  // The screen is black and white and has 2048 pixels (64x32)
  byte[] gfx = new byte[64 * 32];

  // delay timer and sound timer
  byte delay_timer, sound_timer;

  // the stack and a stack pointer
  ushort[] stack = new ushort[16];
  ushort sp;

  // Chip8 keypad (HEX)
  byte[] keypad = new byte[16];

  // fontset

  byte[] chip8_fontset =
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


  void read_rom()
  {

    using (FileStream fs = File.OpenRead("rom.ch8"))
    {
      int bytesRead = fs.Read(memory, 512, 3584);

      if (bytesRead == 0)
        Console.WriteLine("ROM failed to load");
      else
        Console.WriteLine("ROM successfully loaded");
    }

  }

  internal void initializer()
  {

    pc = 0x200; // program counter starts at 0x200
    opcode = 0; // reset current opcode
    I = 0;      // reset index register
    sp = 0;     // reset stack pointer

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

    read_rom();
  }

  void emulateCycle() { }
}
