using System;
using System.Text;
using System.Text.Json;

namespace Client {
    public class Message {
        public string Type { get; set; }
        public string SentFrom { get; set; }
        public string Info { get; set; }
    }
    
    public class MessageManager {
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
            
            var readOnlySpan = new ReadOnlySpan<byte>(jsonData);
            
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
    }
}