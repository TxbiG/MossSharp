using System;
using System.Runtime.InteropServices;

namespace MossSharp.Native.GUI
{
    internal enum MossGuiDataType
    {
        S32,
        U32,
        Float,
        Double
    }

    internal enum MossGuiDrawCmdType
    {
        Triangles,
        Text,
        Callback
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Float2
    {
        public float X;
        public float Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Color
    {
        public float R;
        public float G;
        public float B;
        public float A;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Rect
    {
        public float X;
        public float Y;
        public float W;
        public float H;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct Mat44
    {
        public fixed float M[16];
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossGuiStyle
    {
        public float WindowRounding;
        public float FrameRounding;
        public float ItemSpacing;
        public float WindowPadding;
        public float FramePaddingX;
        public float FramePaddingY;
        public float FontSize;
        public float ScrollbarSize;
        public Color ColorText;
        public Color ColorWindowBg;
        public Color ColorPanelBg;
        public Color ColorFrame;
        public Color ColorFrameHovered;
        public Color ColorFrameActive;
        public Color ColorAccent;
        public Color ColorBorder;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossGuiDrawVert
    {
        public Float2 Pos;
        public Float2 Uv;
        public Color Color;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossGuiDrawCmd
    {
        public MossGuiDrawCmdType Type;
        public uint ElemOffset;
        public uint ElemCount;
        public Rect ClipRect;
        public IntPtr Texture;
        public IntPtr Callback;
        public IntPtr CallbackUserData;
        public uint TextOffset;
        public uint TextCount;
        public Float2 TextPos;
        public Color TextColor;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossGuiDrawList
    {
        public IntPtr Vertices;
        public uint VertexCount;
        public IntPtr Indices;
        public uint IndexCount;
        public IntPtr Commands;
        public uint CommandCount;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossGuiDrawData
    {
        public IntPtr Lists;
        public uint ListCount;
        public IntPtr Text;
        public uint TextSize;
        public uint TotalVertexCount;
        public uint TotalIndexCount;
        public Float2 DisplaySize;
        public Float2 FramebufferScale;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct MossGuiInput
    {
        public Float2 MousePos;
        public fixed byte MouseDown[5];
        public fixed byte MousePressed[5];
        public fixed byte MouseReleased[5];
        public fixed byte KeysDown[512];
        public fixed byte KeysPressed[512];
        public fixed byte KeysReleased[512];
        public float MouseWheel;
        public float DeltaTime;
        public fixed byte InputChars[32];
        public uint InputCharCount;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossGuiFontAtlasDesc
    {
        public uint Width;
        public uint Height;
        public float FontSize;
        public IntPtr Pixels;
        public uint PixelSize;
        [MarshalAs(UnmanagedType.I1)] public bool UploadTexture;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MossGuiStyleBlob
    {
        public IntPtr Data;
        public nuint Size;
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate int MossGuiInputTextCallback(IntPtr userData);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate void MossGuiDrawCallback(IntPtr drawCmd, IntPtr userData);

    internal static unsafe class MossGUINative
    {
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GUICreate(IntPtr renderer, IntPtr gpuDevice);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_GUIDestroy(IntPtr gui);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_GUISetCurrent(IntPtr gui);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GUIGetCurrent();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GUIGetStyle(IntPtr gui);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_GUISetDisplaySize(IntPtr gui, float width, float height);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_GUISetInput(IntPtr gui, in MossGuiInput input);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GUIFontAtlasCreate(IntPtr gui, in MossGuiFontAtlasDesc desc);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_GUIFontAtlasDestroy(IntPtr gui, IntPtr atlas);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GUIFontAtlasGetTexture(IntPtr atlas);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GUIFontAtlasGetPixels(IntPtr atlas, out uint width, out uint height);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_GUISetFontAtlas(IntPtr gui, IntPtr atlas);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_GUISetKeyboardFocus(IntPtr gui, uint id);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern uint Moss_GUIGetKeyboardFocus(IntPtr gui);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_GUINewFrame(IntPtr gui);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GUIEndFrame(IntPtr gui);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr Moss_GUIGetDrawData(IntPtr gui);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_GUIRender(IntPtr gui, IntPtr renderer, IntPtr cmd);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern nuint Moss_GUIStyleSerialize(IntPtr style, IntPtr buffer, nuint bufferSize);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern int Moss_GUIStyleDeserialize(IntPtr style, [MarshalAs(UnmanagedType.LPUTF8Str)] string text);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool BeginWindow([MarshalAs(UnmanagedType.LPUTF8Str)] string name, IntPtr pOpen, uint flags);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void EndWindow();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool BeginWindowChild([MarshalAs(UnmanagedType.LPUTF8Str)] string strId, in Float2 size, uint childFlags, uint windowFlags);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "BeginWindowChild")] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool BeginWindowChildId(uint id, in Float2 size, uint childFlags, uint windowFlags);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void EndWindowChild();

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void BeginContainer(ref Rect rect);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void EndContainer();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void BeginVContainer(ref Rect rect, float spacing);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void EndVContainer();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void BeginHContainer(ref Rect rect, float spacing);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void EndHContainer();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void BeginCenterContainer(ref Rect rect);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void EndCenterContainer();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void BeginMarginContainer(float leftMargin, float rightMargin, float topMargin, float bottomMargin);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void EndMarginContainer();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void BeginVScrollContainer();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void EndVScrollContainer();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void BeginHScrollContainer();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void EndHScrollContainer();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void BeginGridContainer();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void EndGridContainer();

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void ColorRect(Color color, in Float2 size);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void RichText([MarshalAs(UnmanagedType.LPUTF8Str)] string label);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Text([MarshalAs(UnmanagedType.LPUTF8Str)] string fmt);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void SliderDouble(double minValue, double maxValue, double value);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void SliderInt(int minValue, int maxValue, int value);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ProgressBar")] internal static extern void ProgressBarSimple();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ProgressBar")] internal static extern void ProgressBar(float fraction, in Float2 size, [MarshalAs(UnmanagedType.LPUTF8Str)] string? overlay);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool TextLink([MarshalAs(UnmanagedType.LPUTF8Str)] string label);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void TextLinkOpenURL([MarshalAs(UnmanagedType.LPUTF8Str)] string label, [MarshalAs(UnmanagedType.LPUTF8Str)] string? url);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Button([MarshalAs(UnmanagedType.LPUTF8Str)] string label, IntPtr rectPtr, in Mat44 vpMatrix, float defaultWidth, float defaultHeight);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool ToggleSwitch([MarshalAs(UnmanagedType.I1)] bool active);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool RadioButton([MarshalAs(UnmanagedType.LPUTF8Str)] string label, [MarshalAs(UnmanagedType.I1)] bool active);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Checkbox([MarshalAs(UnmanagedType.LPUTF8Str)] string label, [MarshalAs(UnmanagedType.I1)] ref bool value, IntPtr rectPtr, in Mat44 vpMatrix, float defaultSize);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool SliderFloat([MarshalAs(UnmanagedType.LPUTF8Str)] string label, ref float value, float minValue, float maxValue, [MarshalAs(UnmanagedType.LPUTF8Str)] string format, uint flags);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool DragFloat([MarshalAs(UnmanagedType.LPUTF8Str)] string label, ref float value, float speed, float minValue, float maxValue, [MarshalAs(UnmanagedType.LPUTF8Str)] string format, uint flags);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool DragInt([MarshalAs(UnmanagedType.LPUTF8Str)] string label, ref int value, float speed, int minValue, int maxValue, [MarshalAs(UnmanagedType.LPUTF8Str)] string format, uint flags);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool InputText([MarshalAs(UnmanagedType.LPUTF8Str)] string label, IntPtr buffer, nuint bufferSize, uint flags, MossGuiInputTextCallback? callback, IntPtr userData);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool InputTextWithHint([MarshalAs(UnmanagedType.LPUTF8Str)] string label, [MarshalAs(UnmanagedType.LPUTF8Str)] string hint, IntPtr buffer, nuint bufferSize, uint flags, MossGuiInputTextCallback? callback, IntPtr userData);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool InputFloat([MarshalAs(UnmanagedType.LPUTF8Str)] string label, ref float value, float step, float stepFast, [MarshalAs(UnmanagedType.LPUTF8Str)] string format, uint flags);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool InputInt([MarshalAs(UnmanagedType.LPUTF8Str)] string label, ref int value, int step, int stepFast, uint flags);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool InputDouble([MarshalAs(UnmanagedType.LPUTF8Str)] string label, ref double value, double step, double stepFast, [MarshalAs(UnmanagedType.LPUTF8Str)] string format, uint flags);

        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_GUICombo([MarshalAs(UnmanagedType.LPUTF8Str)] string label, ref int currentItem, IntPtr items, uint itemCount, uint flags);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_GUIOpenPopup([MarshalAs(UnmanagedType.LPUTF8Str)] string id);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_GUIBeginPopup([MarshalAs(UnmanagedType.LPUTF8Str)] string id, uint flags);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_GUIEndPopup();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_GUIBeginMenu([MarshalAs(UnmanagedType.LPUTF8Str)] string label, [MarshalAs(UnmanagedType.I1)] bool enabled);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_GUIEndMenu();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_GUIMenuItem([MarshalAs(UnmanagedType.LPUTF8Str)] string label, [MarshalAs(UnmanagedType.I1)] bool selected, [MarshalAs(UnmanagedType.I1)] bool enabled);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_GUITabBar([MarshalAs(UnmanagedType.LPUTF8Str)] string label, ref int activeTab, IntPtr tabs, uint tabCount);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_GUITableBegin([MarshalAs(UnmanagedType.LPUTF8Str)] string label, uint columnCount);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_GUITableNextRow();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_GUITableNextColumn();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_GUITableEnd();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_GUITreeNode([MarshalAs(UnmanagedType.LPUTF8Str)] string label, uint flags);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_GUITreePop();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_GUIListBox([MarshalAs(UnmanagedType.LPUTF8Str)] string label, ref int currentItem, IntPtr items, uint itemCount, uint visibleItems);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_GUIColorPicker4([MarshalAs(UnmanagedType.LPUTF8Str)] string label, float* rgba, uint flags);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_GUITooltip([MarshalAs(UnmanagedType.LPUTF8Str)] string text);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_GUIBeginDragDropSource([MarshalAs(UnmanagedType.LPUTF8Str)] string type, IntPtr data, nuint size);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern void Moss_GUIEndDragDropSource();
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] [return: MarshalAs(UnmanagedType.I1)] internal static extern bool Moss_GUIAcceptDragDropPayload([MarshalAs(UnmanagedType.LPUTF8Str)] string type, out IntPtr data, out nuint size);
        [DllImport(MossNativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)] internal static extern float Moss_GUI_VirtualJoystick();
    }
}
