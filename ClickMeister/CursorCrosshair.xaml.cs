using ClickMeister.Extension;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ClickMeister;
/// <summary>
/// Interaction logic for CursorCrosshair.xaml
/// </summary>
public partial class CursorCrosshair : ContentControl
{
    public CursorCrosshair(Point center, int size = 25)
    {
        InitializeComponent();
        // Create a path to draw a geometry with.
        var crosshair = new Path
        {
            Stroke = Brushes.Red,
            StrokeThickness = 2
        };

        // Create a StreamGeometry to use to specify myPath.
        var theGeometry = BuildCrosshair(center, size);

        // Freeze the geometry (make it unmodifiable)
        // for additional performance benefits.
        theGeometry.Freeze();

        // Use the StreamGeometry returned by the BuildRegularPolygon to
        // specify the shape of the path.
        crosshair.Data = theGeometry;

        // Add path shape to the UI.
        var panel = new StackPanel();
        _ = panel.Children.Add(crosshair);
        Content = panel;
    }

    private const double SCALE_FACTOR = 1.4;
    private StreamGeometry BuildCrosshair(Point position, int radius = 100)
    {
        var geometry = new StreamGeometry();
        using var context = geometry.Open();

        context.DrawGeomentry(new EllipseGeometry(new Point(position.X, position.Y), radius, radius));

        var horizontalStart = new Point(position.X - (radius * SCALE_FACTOR), position.Y);
        var horizontalEnd = new Point(position.X + (radius * SCALE_FACTOR), position.Y);

        var verticalStart = new Point(position.X, position.Y - (radius * SCALE_FACTOR));
        var verticalEnd = new Point(position.X, position.Y + (radius * SCALE_FACTOR));

        context.DrawGeomentry(new LineGeometry(horizontalStart, horizontalEnd));
        context.DrawGeomentry(new LineGeometry(verticalStart, verticalEnd));
        return geometry;
    }

    private StreamGeometry BuildRegularPolygon(Point c, int r, int numSides, double offsetDegree)
    {
        var geometry = new StreamGeometry();

        using (var ctx = geometry.Open())
        {
            ctx.BeginFigure(new Point(), true /* is filled */, true /* is closed */);

            double step = 2 * Math.PI / Math.Max(numSides, 3);
            var cur = c;

            double a = Math.PI * offsetDegree / 180.0;
            for (int i = 0; i < numSides; i++, a += step)
            {
                cur.X = c.X + (r * Math.Cos(a));
                cur.Y = c.Y + (r * Math.Sin(a));
                ctx.LineTo(cur, true /* is stroked */, false /* is smooth join */);
            }
        }

        return geometry;
    }
}
