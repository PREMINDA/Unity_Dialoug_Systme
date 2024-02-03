using UnityEngine;
using Color = System.Drawing.Color;

namespace Editor.DLSystem.Data.Error
{
    public class DLSystemErrorData
    {
        public Color32 Color { get; set; }

        public DLSystemErrorData()
        {
            GenerateColor();
        }

        private void GenerateColor()
        {
            Color = new Color32(
                (byte) Random.Range(65,256),
                (byte) Random.Range(65,176),
                (byte) Random.Range(65,176),
                255
                );


        }
    }
}