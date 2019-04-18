﻿using NTMiner.Controllers;
using NTMiner.MinerClient;
using NTMiner.MinerServer;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NTMiner {
    public static partial class Server {
        public partial class ReportServiceFace {
            public static readonly ReportServiceFace Instance = new ReportServiceFace();
            private static readonly string SControllerName = ControllerUtil.GetControllerName<IReportController>();

            private ReportServiceFace() { }

            public void ReportSpeedAsync(string host, SpeedData data) {
                Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = new HttpClient()) {
                            // TODO:可能超过3秒钟，查查原因
                            client.Timeout = TimeSpan.FromSeconds(10);
                            Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{host}:{WebApiConst.ControlCenterPort}/api/{SControllerName}/{nameof(IReportController.ReportSpeed)}", data);
                            Write.DevDebug($"{nameof(ReportSpeedAsync)} {message.Result.ReasonPhrase}");
                        }
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.GetInnerMessage(), e);
                    }
                });
            }

            public void ReportStateAsync(string host, Guid clientId, bool isMining) {
                Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = new HttpClient()) {
                            client.Timeout = TimeSpan.FromSeconds(10);
                            ReportState request = new ReportState {
                                ClientId = clientId,
                                IsMining = isMining
                            };
                            Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{host}:{WebApiConst.ControlCenterPort}/api/{SControllerName}/{nameof(IReportController.ReportState)}", request);
                            Write.DevDebug($"{nameof(ReportStateAsync)} {message.Result.ReasonPhrase}");
                        }
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.GetInnerMessage(), e);
                    }
                });
            }
        }
    }
}