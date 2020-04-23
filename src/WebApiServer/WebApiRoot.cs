﻿using Aliyun.OSS;
using LiteDB;
using NTMiner.Core;
using NTMiner.Core.Impl;
using NTMiner.Core.Mq.MqMessagePaths;
using NTMiner.Core.Mq.Senders.Impl;
using NTMiner.Core.Redis.Impl;
using NTMiner.Impl;
using NTMiner.ServerNode;
using System;
using System.Linq;
using System.Threading;

namespace NTMiner {
    public static class WebApiRoot {
        private static readonly EventWaitHandle WaitHandle = new AutoResetEvent(false);
        private static OssClient _ossClient = null;
        public static OssClient OssClient {
            get {
                return _ossClient;
            }
        }

        private static IServerContext _serverContext;

        private static Mutex _sMutexApp;
        // 该程序编译为控制台程序，如果不启用内网支持则默认设置为开机自动启动
        [STAThread]
        static void Main() {
            NTMinerConsole.DisbleQuickEditMode();
            HomePath.SetHomeDirFullName(AppDomain.CurrentDomain.BaseDirectory);
            try {
                bool mutexCreated;
                try {
                    _sMutexApp = new Mutex(true, "NTMinerServicesMutex", out mutexCreated);
                }
                catch {
                    mutexCreated = false;
                }
                if (mutexCreated) {
                    try {
                        // 用本节点的地址作为队列名，消费消息时根据路由键区分消息类型
                        string queue = $"{ServerAppType.WebApiServer.GetName()}.{ServerRoot.HostConfig.ThisServerAddress}";
                        string durableQueue = queue + MqKeyword.DurableQueueEndsWith;
                        AbstractMqMessagePath[] mqMessagePaths = new AbstractMqMessagePath[] {
                            new UserMqMessagePath(durableQueue), 
                            new MinerClientMqMessagePath(queue)
                        };
                        _serverContext = ServerContext.Create(mqClientTypeName: ServerAppType.WebApiServer.GetName(), mqMessagePaths);
                        if (_serverContext == null) {
                            Write.UserError("启动失败，无法继续，因为服务器上下文创建失败");
                            return;
                        }
                        Console.Title = $"{ServerAppType.WebApiServer.GetName()}_{ServerRoot.HostConfig.ThisServerAddress}";
                        _ossClient = new OssClient(ServerRoot.HostConfig.OssEndpoint, ServerRoot.HostConfig.OssAccessKeyId, ServerRoot.HostConfig.OssAccessKeySecret);
                        var minerClientMqSender = new MinerClientMqSender(_serverContext.Channel);
                        var userMqSender = new UserMqSender(_serverContext.Channel);
                        var wsServerNodeMqSender = new WsServerNodeMqSender(_serverContext.Channel);

                        var minerRedis = new MinerRedis(_serverContext.RedisConn);
                        var userRedis = new UserRedis(_serverContext.RedisConn);
                        var captchaRedis = new CaptchaRedis(_serverContext.RedisConn);

                        WsServerNodeSet = new WsServerNodeSet(wsServerNodeMqSender);
                        UserSet = new UserSet(userRedis, userMqSender);
                        UserAppSettingSet = new UserAppSettingSet();
                        CaptchaSet = new CaptchaSet(captchaRedis);
                        CalcConfigSet = new CalcConfigSet();
                        NTMinerWalletSet = new NTMinerWalletSet();
                        ClientDataSet clientDataSet = new ClientDataSet(minerRedis, minerClientMqSender);
                        ClientDataSet = clientDataSet;
                        CoinSnapshotSet = new CoinSnapshotSet(clientDataSet);
                        MineWorkSet = new UserMineWorkSet();
                        MinerGroupSet = new UserMinerGroupSet();
                        NTMinerFileSet = new NTMinerFileSet();
                        OverClockDataSet = new OverClockDataSet();
                        KernelOutputKeywordSet = new KernelOutputKeywordSet(SpecialPath.LocalDbFileFullName, isServer: true);
                        ServerMessageSet = new ServerMessageSet(SpecialPath.LocalDbFileFullName, isServer: true);
                        UpdateServerMessageTimestamp();
                        if (VirtualRoot.LocalAppSettingSet.TryGetAppSetting(nameof(KernelOutputKeywordTimestamp), out IAppSetting appSetting) && appSetting.Value is DateTime value) {
                            KernelOutputKeywordTimestamp = value;
                        }
                        else {
                            KernelOutputKeywordTimestamp = Timestamp.UnixBaseTime;
                        }
                    }
                    catch (Exception e) {
                        Write.UserError(e.Message);
                        Write.UserError(e.StackTrace);
                        Write.UserInfo("按任意键退出");
                        Console.ReadKey();
                        return;
                    }
                    VirtualRoot.StartTimer();
                    NTMinerRegistry.SetAutoBoot("NTMinerServices", true);
                    Type thisType = typeof(WebApiRoot);
                    Run();
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }

        private static bool _isClosed = false;
        private static void Close() {
            if (!_isClosed) {
                _isClosed = true;
                _sMutexApp?.Dispose();
            }
        }

        public static void Exit() {
            Close();
            Environment.Exit(0);
        }

        private static void Run() {
            try {
                string baseAddress = $"http://localhost:{ServerRoot.HostConfig.GetServerPort().ToString()}";
                HttpServer.Start(baseAddress);
                Windows.ConsoleHandler.Register(Close);
                WaitHandle.WaitOne();
                Close();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
            finally {
                Close();
            }
        }

        static WebApiRoot() {
        }

        public static DateTime StartedOn { get; private set; } = DateTime.Now;

        public static IUserAppSettingSet UserAppSettingSet { get; private set; }

        public static IUserSet UserSet { get; private set; }

        public static IWsServerNodeSet WsServerNodeSet { get; private set; }

        public static ICaptchaSet CaptchaSet { get; private set; }

        public static ICalcConfigSet CalcConfigSet { get; private set; }

        public static INTMinerWalletSet NTMinerWalletSet { get; private set; }

        public static IClientDataSet ClientDataSet { get; private set; }

        public static ICoinSnapshotSet CoinSnapshotSet { get; private set; }

        public static IUserMineWorkSet MineWorkSet { get; private set; }

        public static IUserMinerGroupSet MinerGroupSet { get; private set; }

        public static INTMinerFileSet NTMinerFileSet { get; private set; }

        public static IOverClockDataSet OverClockDataSet { get; private set; }

        public static IKernelOutputKeywordSet KernelOutputKeywordSet { get; private set; }

        public static IServerMessageSet ServerMessageSet { get; private set; }

        public static DateTime ServerMessageTimestamp { get; set; }

        public static DateTime KernelOutputKeywordTimestamp { get; private set; }

        public static LiteDatabase CreateLocalDb() {
            return new LiteDatabase($"filename={SpecialPath.LocalDbFileFullName}");
        }

        /// <summary>
        /// 因为客户端具有不同的ClientVersion，考虑到兼容性问题将来可能不同版本的客户端需要获取不同版本的server.json，所以
        /// 服务端用返回什么版本的server.json应由客户端自己决定，所以该方法有个jsonVersionKey入参，这个入参就是来自客户端。
        /// </summary>
        /// <param name="jsonVersionKey">server.json的版本</param>
        /// <returns></returns>
        public static ServerStateResponse ServerStateResponse(string jsonVersionKey) {
            string jsonVersion = string.Empty;
            string minerClientVersion = string.Empty;
            try {
                var fileData = NTMinerFileSet.LatestMinerClientFile;
                minerClientVersion = fileData != null ? fileData.Version : string.Empty;
                if (!VirtualRoot.LocalAppSettingSet.TryGetAppSetting(jsonVersionKey, out IAppSetting data) || data.Value == null) {
                    jsonVersion = string.Empty;
                }
                else {
                    jsonVersion = data.Value.ToString();
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
            return new ServerStateResponse {
                JsonFileVersion = jsonVersion,
                MinerClientVersion = minerClientVersion,
                Time = Timestamp.GetTimestamp(),
                MessageTimestamp = Timestamp.GetTimestamp(ServerMessageTimestamp),
                OutputKeywordTimestamp = Timestamp.GetTimestamp(KernelOutputKeywordTimestamp),
                WsStatus = WsServerNodeSet.WsStatus
            };
        }

        public static void UpdateServerMessageTimestamp() {
            var first = ServerMessageSet.AsEnumerable().OrderByDescending(a => a.Timestamp).FirstOrDefault();
            if (first == null) {
                ServerMessageTimestamp = DateTime.MinValue;
            }
            else {
                ServerMessageTimestamp = first.Timestamp;
            }
        }

        public static void UpdateKernelOutputKeywordTimestamp(DateTime timestamp) {
            KernelOutputKeywordTimestamp = timestamp;
            VirtualRoot.Execute(new SetLocalAppSettingCommand(new AppSettingData {
                Key = nameof(KernelOutputKeywordTimestamp),
                Value = timestamp
            }));
        }
    }
}