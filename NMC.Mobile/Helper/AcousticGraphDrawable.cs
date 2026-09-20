using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Maui.Graphics;

namespace NMC.Mobile.Helper
{
    public class AcousticGraphDrawable : IDrawable
    {
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.StrokeColor = Colors.White;
            canvas.StrokeSize = 2;
            canvas.Antialias = true;

            canvas.DrawLine(4, dirtyRect.Height - 4, dirtyRect.Width - 4, dirtyRect.Height - 4);
            canvas.DrawLine(4, 4, 4, dirtyRect.Height - 4);

            var path = new PathF();
            path.MoveTo(4, dirtyRect.Height - 8);
            path.CurveTo(
                dirtyRect.Width * 0.3f, dirtyRect.Height - 8,
                dirtyRect.Width * 0.6f, 12,
                dirtyRect.Width - 4, 4
            );

            canvas.DrawPath(path);
        }
    }
}