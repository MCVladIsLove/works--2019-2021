using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game1
{
    enum MovementDirection
    {
        Top,
        Bottom,
        Left,
        Right,
        TopRight,
        TopLeft,
        BottomLeft,
        BottomRight,
        Stay
    }
    
    enum CreatureState
    {
        Patrol,
        HeardSomething,
        Fight,
        RunAway
    }

    enum Priority
    {
        Ignore,
        Track,
        Check
    }

    enum CurrentWindow
    {
        Game,
        MainMenu,
        EscapeMenu,
        DeathMenu,
        WinMenu,
        AboutAuthorMenu
    }
}
