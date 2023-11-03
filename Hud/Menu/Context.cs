using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Hud.Shaders;
using OpenTK.Mathematics;

namespace Hud.Menu;

/// <summary>
/// This describes how much space there is on the screen.
/// </summary>
/// <param name="Shader"></param>
/// <param name="Position">The top left corner of the Context</param>
/// <param name="Size">Width and height of the Context</param>
public record Context(HudShader Shader, Vector2 Position, Vector2 Size);
