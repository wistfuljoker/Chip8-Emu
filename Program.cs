using Chip8_Emu;

var chip = new Chip8();

chip.Initializer();
var screen = new Chip8Display();

while (true)
{

  chip.EmulateCycle();
  if (chip.Drawflag)
  {
    screen.Render(chip.gfx);
    chip.Drawflag = false;
  }
}