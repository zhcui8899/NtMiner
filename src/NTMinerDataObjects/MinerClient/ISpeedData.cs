﻿using System;

namespace NTMiner.MinerClient {
    public interface ISpeedData {
        DateTime LocalServerMessageTimestamp { get; }
        int AutoRestartKernelTimes { get; }
        int AutoStartDelaySeconds { get; }
        DateTime BootOn { get; }
        Guid ClientId { get; }
        int CpuGETemperatureSeconds { get; }
        int CpuLETemperatureSeconds { get; }
        int CpuPerformance { get; }
        int CpuStartTemperature { get; }
        int CpuStopTemperature { get; }
        int CpuTemperature { get; }
        string DiskSpace { get; }
        string DualCoinCode { get; }
        string DualCoinPool { get; }
        string DualCoinPoolDelay { get; }
        int DualCoinRejectShare { get; }
        double DualCoinSpeed { get; }
        int DualCoinTotalShare { get; }
        string DualCoinWallet { get; }
        string GpuDriver { get; }
        string GpuInfo { get; }
        GpuSpeedData[] GpuTable { get; }
        GpuType GpuType { get; }
        bool IsAutoBoot { get; }
        bool IsAutoRestartKernel { get; }
        bool IsAutoStart { get; }
        bool IsAutoStartByCpu { get; }
        bool IsAutoStopByCpu { get; }
        bool IsDualCoinEnabled { get; }
        bool IsFoundOneGpuShare { get; }
        bool IsGotOneIncorrectGpuShare { get; }
        bool IsMining { get; }
        bool IsNoShareRestartComputer { get; }
        bool IsNoShareRestartKernel { get; }
        bool IsPeriodicRestartComputer { get; }
        bool IsPeriodicRestartKernel { get; }
        bool IsRejectOneGpuShare { get; }
        string Kernel { get; }
        string KernelCommandLine { get; }
        int KernelSelfRestartCount { get; }
        string MainCoinCode { get; }
        string MainCoinPool { get; }
        string MainCoinPoolDelay { get; }
        int MainCoinRejectShare { get; }
        double MainCoinSpeed { get; }
        int MainCoinTotalShare { get; }
        string MainCoinWallet { get; }
        string MinerIp { get; }
        /// <summary>
        /// 注意：该属性对应服务端的ClientName，而服务端的MinerName是群控矿工名。
        /// </summary>
        string MinerName { get; }
        DateTime? MineStartedOn { get; }
        Guid MineWorkId { get; }
        string MineWorkName { get; }
        int NoShareRestartComputerMinutes { get; }
        int NoShareRestartKernelMinutes { get; }
        string OSName { get; }
        int OSVirtualMemoryMb { get; }
        int PeriodicRestartComputerHours { get; }
        int PeriodicRestartComputerMinutes { get; }
        int PeriodicRestartKernelHours { get; }
        int PeriodicRestartKernelMinutes { get; }
        bool IsRaiseHighCpuEvent { get; }
        int HighCpuPercent { get; }
        int HighCpuSeconds { get; }
        string Version { get; }
    }
}