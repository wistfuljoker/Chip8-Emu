using Chip8_Emu;

var chip = new Chip8();

chip.Initializer();

while (true)
  chip.EmulateCycle();
