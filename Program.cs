using Chip8_Emu;
using SDL3;

var chip = new Chip8();

chip.Initializer();
var screen = new Chip8Display();
var display = new Display();
display.Render(new byte[64 * 32]);
bool running = true;

while (running)
{

  while (SDL.PollEvent(out SDL.Event @event))
  {
    if (@event.Type == (uint)SDL.EventType.Quit)
      running = false;
  }

  chip.EmulateCycle();

  if (chip.Drawflag)
  {
    //screen.Render(chip.gfx);
    display.Render(chip.gfx);
    chip.Drawflag = false;
  }

  Thread.Sleep(1);

}

display.Cleanup();