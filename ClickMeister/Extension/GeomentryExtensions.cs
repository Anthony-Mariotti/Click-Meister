using System.Windows.Media;

namespace ClickMeister.Extension;
public static class GeomentryExtensions
{
    public static void DrawGeomentry(this StreamGeometryContext context, Geometry geo)
    {
        var pathGeomentry = geo as PathGeometry ?? PathGeometry.CreateFromGeometry(geo);
        foreach(var figure in pathGeomentry.Figures)
        {
            context.DrawFigure(figure);
        }
    }

    public static void DrawFigure(this StreamGeometryContext context, PathFigure figure)
    {
        context.BeginFigure(figure.StartPoint, figure.IsFilled, figure.IsClosed);
        foreach(var segment in figure.Segments)
        {
            if (segment is LineSegment lineSegment)
            {
                context.LineTo(lineSegment.Point, lineSegment.IsStroked, lineSegment.IsSmoothJoin);
            }

            if (segment is BezierSegment bezierSegment)
            {
                context.BezierTo(bezierSegment.Point1, bezierSegment.Point2, bezierSegment.Point3, bezierSegment.IsStroked, bezierSegment.IsSmoothJoin);
                continue;
            }

            if (segment is QuadraticBezierSegment quadraticSegment)
            {
                context.QuadraticBezierTo(quadraticSegment.Point1, quadraticSegment.Point2, quadraticSegment.IsStroked, quadraticSegment.IsSmoothJoin);
                continue;
            }

            if (segment is PolyLineSegment polyLineSegment)
            {
                context.PolyLineTo(polyLineSegment.Points, polyLineSegment.IsStroked, polyLineSegment.IsSmoothJoin);
                continue;
            }

            if (segment is PolyBezierSegment polyBezierSegment)
            {
                context.PolyBezierTo(polyBezierSegment.Points, polyBezierSegment.IsStroked, polyBezierSegment.IsSmoothJoin);
                continue;
            }

            if (segment is PolyQuadraticBezierSegment polyQuadraticSegment)
            {
                context.PolyQuadraticBezierTo(polyQuadraticSegment.Points, polyQuadraticSegment.IsStroked, polyQuadraticSegment.IsSmoothJoin);
                continue;
            }

            if (segment is ArcSegment arcSegment)
            {
                context.ArcTo(arcSegment.Point, arcSegment.Size, arcSegment.RotationAngle, arcSegment.IsLargeArc, arcSegment.SweepDirection, arcSegment.IsStroked, arcSegment.IsSmoothJoin);
                continue;
            }
        }
    }
}
