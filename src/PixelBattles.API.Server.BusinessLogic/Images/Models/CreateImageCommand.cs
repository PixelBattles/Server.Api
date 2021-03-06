﻿using System;

namespace PixelBattles.API.Server.BusinessLogic.Images.Models
{
    public class CreateImageCommand
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string ContentType { get; set; }

        public string FileName { get; set; }

        public byte[] Data { get; set; }
    }
}