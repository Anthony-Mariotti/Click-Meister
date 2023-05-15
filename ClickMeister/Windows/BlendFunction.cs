using System.Runtime.InteropServices;

namespace ClickMeister.Windows;

[StructLayout(LayoutKind.Sequential)]
public struct BlendFunction
{
    public byte BlendOp;
    public byte BlendFlags;
    public byte SourceConstantAlpha;
    public byte AlphaFormat;

    public BlendFunction(byte blendOp, byte blendFlags, byte sourceConstantAlpha, byte alphaFormat)
    {
        BlendOp = blendOp;
        BlendFlags = blendFlags;
        SourceConstantAlpha = sourceConstantAlpha;
        AlphaFormat = alphaFormat;
    }
}