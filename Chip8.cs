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

  void read_rom() { }

  void initializer() { }

  void emulateCycle() { }
}
