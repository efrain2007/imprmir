using ESC_POS_USB_NET.Printer;
using imprmir;
using Newtonsoft.Json;
using SocketIOClient.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SocketIOClient.Sample
{

    class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.ASCII;
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));

            System.Text.EncodingProvider ppp = System.Text.CodePagesEncodingProvider.Instance;
            Encoding.RegisterProvider(ppp);


            var uri = new Uri("http://localhost:3301");

            var socket = new SocketIO(uri, new SocketIOOptions
            {
                Query = new Dictionary<string, string>
                {
                    //{"token", "io" }
                    {"token", "v3" }
                },
                EIO = 4
            });
            
            try //boque try con todas las operaciones
            {
                socket.JsonSerializer = new NewtonsoftJsonSerializer(socket.Options.EIO);

                socket.GetConnectInterval = () => new MyConnectInterval();
                socket.OnConnected += Socket_OnConnected;
                socket.OnPing += Socket_OnPing;
                socket.OnPong += Socket_OnPong;
                socket.OnDisconnected += Socket_OnDisconnected;
                socket.OnReconnecting += Socket_OnReconnecting;
                await socket.ConnectAsync();
            }
            catch (Exception ex) //bloque catch para captura de error
            {
                Console.WriteLine(ex);
            }
            

            socket.On("imprimir", response =>
            {
                var data = response.GetValue<BinaryObjectResponse>();
                Console.WriteLine(data);
               // Printer printer = new Printer(data.Impresora);
                 Printer printer = new Printer(data.Impresora);
                System.IO.FileStream fs = new System.IO.FileStream(@data.Imagen, FileMode.Open, FileAccess.Read);
                Bitmap image = new Bitmap(Bitmap.FromStream(fs));
                printer.Image(image);
                fs.Close();
                printer.Append(Full());
                printer.PrintDocument();
                printer.Clear();
                string result = data.Impresora;
            });



            Console.ReadLine();
        }
        static public byte[] Full()
        {

            return new byte[] { 29, (byte)'V', 48, 0 };
        }


        private static void Socket_OnReconnecting(object sender, int e)
        {
            Console.WriteLine($"Reconnecting: attempt = {e}");
        }

        private static void Socket_OnDisconnected(object sender, string e)
        {
            Console.WriteLine("disconnect: " + e);
        }

        private static async void Socket_OnConnected(object sender, EventArgs e)
        {
            Console.WriteLine("Socket_OnConnected");
            var socket = sender as SocketIO;
            Console.WriteLine("Socket.Id:" + socket.Id);
            await socket.EmitAsync("pisos", "SocketIOClient.Sample");


        }

        private static void Socket_OnPing(object sender, EventArgs e)
        {
            Console.WriteLine("Ping");
        }

        private static void Socket_OnPong(object sender, TimeSpan e)
        {
            Console.WriteLine("Pong: " + e.TotalMilliseconds);
        }
    }

    class ByteResponse
    {
        public string ClientSource { get; set; }

        public string Source { get; set; }

        [JsonProperty("bytes")]
        public byte[] Buffer { get; set; }
    }

    class ClientCallbackResponse
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("bytes")]
        public byte[] Bytes { get; set; }
    }
}
