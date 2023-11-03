using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using OpenTK.Mathematics;

namespace Hud.Menu;
public interface IWidget
{
    Vector2 GetSize();

    void Render(Context context);
}
