using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;

namespace OnlineBookStore.Infrastructure
{
    public static class LocalNetworkHelper
    {
        /// <summary>
        /// 返回可靠的本机内网 IPv4 地址（首选用于对外发包的地址）。
        /// 若失败则回退到第一个可用的非回环 IPv4 地址。
        /// 若均失败则返回 "127.0.0.1"。
        /// </summary>
        public static string GetLocalIPv4()
        {
            // 方法A：通过 UDP 连接外部地址来获取当前出口地址（最快且常用）
            try
            {
                using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                {
                    // 目标地址不必可达，只用于让系统选出一条出口接口
                    socket.Connect("8.8.8.8", 53);
                    if (socket.LocalEndPoint is IPEndPoint endPoint)
                    {
                        if (endPoint.Address.AddressFamily == AddressFamily.InterNetwork)
                            return endPoint.Address.ToString();
                    }
                }
            }
            catch
            {
                // 忽略并退回到网卡遍历
            }

            // 方法B：遍历网卡，选一个合适的 IPv4 地址
            try
            {
                var interfaces = NetworkInterface.GetAllNetworkInterfaces()
                    .Where(nic => nic.OperationalStatus == OperationalStatus.Up &&
                                  nic.NetworkInterfaceType != NetworkInterfaceType.Loopback);

                foreach (var nic in interfaces)
                {
                    var props = nic.GetIPProperties();
                    foreach (var addr in props.UnicastAddresses)
                    {
                        if (addr.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            var ip = addr.Address.ToString();
                            // 排除自动私有链路地址 (169.254.x.x) 等
                            if (!ip.StartsWith("169.254") && !IPAddress.IsLoopback(addr.Address))
                                return ip;
                        }
                    }
                }
            }
            catch
            {
                // 忽略
            }

            return "127.0.0.1";
        }
    }
}
