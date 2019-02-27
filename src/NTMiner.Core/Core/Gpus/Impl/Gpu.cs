﻿namespace NTMiner.Core.Gpus.Impl {
    internal class Gpu : IGpu {
        public static readonly IGpu Total = new Gpu {
            Index = NTMinerRoot.GpuAllId,
            Name = "全部显卡",
            Temperature = 0,
            FanSpeed = 0,
            PowerUsage = 0,
            CoreClockDelta = 0,
            MemoryClockDelta = 0,
            GpuClockDelta = new GpuClockDelta(0, 0, 0, 0),
            OverClock = new GpuAllOverClock()
        };

        public Gpu() {
        }

        public IOverClock OverClock { get; set; }

        public int Index { get; set; }

        public string Name { get; set; }

        public uint Temperature { get; set; }

        public uint FanSpeed { get; set; }

        public uint PowerUsage { get; set; }
        public int CoreClockDelta { get; set; }
        public int MemoryClockDelta { get; set; }

        public GpuClockDelta GpuClockDelta { get; set; }
    }
}
