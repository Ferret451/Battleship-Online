using System;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Client {
    public class Message {
        public string Type { get; set; }
        public string SentFrom { get; set; }
        public string Info { get; set; }
    }
    
    public class MessageManager {
        private readonly ClientWebSocket _clientWebSocket;
        
        public MessageManager(ClientWebSocket clientWebSocket) {
            _clientWebSocket = clientWebSocket;
        }
        
        private static byte[] CutMessage(byte[] data) {
            for (int i = 0; i < data.Length; i++) {
                if (data[i] == 0) {
                    byte[] newData = new byte[i];
                    Array.Copy(data, newData, i);
                    return newData;
                }
            }

            return data;
        }
        
        public static Message HandleMessage(byte[] jsonData) {
            jsonData = CutMessage(jsonData);

            // foreach (var b in jsonData) {
            //     Console.Write(b + " ");    
            // }
            //
            // Console.WriteLine();
            
            Message message = JsonSerializer.Deserialize<Message>(jsonData);

            return message;
        }
        
        public static byte[] CreateMessage(string type, string sentFrom, string info) {
            Message message = new Message {
                Type = type,
                SentFrom = sentFrom,
                Info = info
            };
            
            return JsonSerializer.SerializeToUtf8Bytes(message);
        }
        
        public async Task SendMessageAsync(string type, string sendFrom, string message = "") {
            if (_clientWebSocket.State == WebSocketState.Open) {
                byte[] buffer = CreateMessage(type, sendFrom, message);
                await _clientWebSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);

                Console.WriteLine($"Sent message of type {type} with message {message}"); 
            } else {
                Console.WriteLine("Error: WebSocket connection is not open.");
            }
        }
    }
}