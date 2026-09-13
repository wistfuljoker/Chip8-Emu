using SDL3;

namespace Chip8_Emu;

public class Display
{
  nint window;
  nint renderer;
  nint texture;
  uint[] pixels = new uint[64 * 32];

  internal Display()
  {


    if (!SDL.Init(SDL.InitFlags.Video))
    {
      SDL.Log("SDL Failed!");
      Environment.Exit(1);
    }
    window = SDL.CreateWindow("Chip8 Intepreter", 640, 320, 0);
    renderer = SDL.CreateRenderer(window, null);
    texture = SDL.CreateTexture(renderer, SDL.PixelFormat.ABGR8888, SDL.TextureAccess.Streaming, 64, 32);

  }



  internal void Render(byte[] gfx)
  {
    for (int i = 0; i < gfx.Length; i++)
      pixels[i] = gfx[i] != 0 ? 0xFFFFFFFFu : 0xFF000000u;

    unsafe
    {
      fixed (uint* ptr = pixels)
      {
        SDL.UpdateTexture(texture, IntPtr.Zero, (IntPtr)ptr, 64 * sizeof(uint));
      }
    }

    SDL.RenderClear(renderer);
    SDL.RenderTexture(renderer, texture, IntPtr.Zero, IntPtr.Zero);
    SDL.RenderPresent(renderer);
  }

  internal void Cleanup()
  {
    SDL.DestroyTexture(texture);
    SDL.DestroyRenderer(renderer);
    SDL.DestroyWindow(window);
    SDL.Quit();
  }

}