namespace MmgEngine;

public enum Alignment : byte
{
    TopLeft = 0b_0000,
    TopCenter = 0b_0001,
    TopRight = 0b_0010,
    CenterLeft = 0b_0100,
    Center = 0b_0101,
    CenterRight = 0b_0110,
    BottomLeft = 0b_1000,
    BottomCenter = 0b_1001,
    BottomRight = 0b_1010
}