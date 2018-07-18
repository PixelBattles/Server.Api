﻿namespace PixelBattles.Shared.DataTransfer.Api.Battle
{
    public class BattleSettingsDTO
    {
        public int ChunkHeight { get; set; }
        public int ChunkWidth { get; set; }
        public int MaxHeightIndex { get; set; }
        public int MaxWidthIndex { get; set; }
        public int MinHeightIndex { get; set; }
        public int MinWidthIndex { get; set; }
        public int CenterX { get; set; }
        public int CenterY { get; set; }
        public int Cooldown { get; set; }
    }
}