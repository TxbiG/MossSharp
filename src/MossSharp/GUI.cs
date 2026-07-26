using System;
using MossSharp.Native.GUI;

namespace Moss.GUI
{
    public static class Gui
    {
        public static void Text(string text) => MossGUINative.Text(text);
        public static void RichText(string text) => MossGUINative.RichText(text);
        public static bool Button(string label) => MossGUINative.Button(label, IntPtr.Zero, default, 0.0f, 0.0f);
        public static bool Checkbox(string label, ref bool value) => MossGUINative.Checkbox(label, ref value, IntPtr.Zero, default, 0.0f);
        public static void Slider(int minValue, int maxValue, int value) => MossGUINative.SliderInt(minValue, maxValue, value);
        public static void Slider(double minValue, double maxValue, double value) => MossGUINative.SliderDouble(minValue, maxValue, value);
    }
}