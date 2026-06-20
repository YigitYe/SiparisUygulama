using Microsoft.AspNetCore.SignalR;

namespace SiparisUygulama.Hubs
{
    public class SiparisHub : Hub
    {
        public async Task SiparisDurumGuncellendi(int orderId, string yeniDurum)
        {
            await Clients.All.SendAsync("SiparisDurumGuncellendi", orderId, yeniDurum);
        }

        // Kurye anlık konumunu müşterilere push eder
        public async Task KonumGuncellendi(int orderId, double enlem, double boylam)
        {
            await Clients.All.SendAsync("KureyeKonumu", orderId, enlem, boylam);
        }
    }
}
