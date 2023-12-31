﻿using System;
using System.IO;
using System.IO.Pipes;
using System.Text;

public struct CustomData
{
    public int Id { get; set; }
    public string Message { get; set; }
}

class Program
{
    static void Main()
    {
        using (NamedPipeClientStream clientStream = new NamedPipeClientStream(".", "myPipe", PipeDirection.InOut))
        {
            Console.WriteLine("Клиент: Подключение к серверу...");
            clientStream.Connect();
            Console.WriteLine("Клиент: Введите Num и Text:");
            var sendData = new CustomData()
            {
                Id = Convert.ToInt32(Console.ReadLine()),
                Message = Console.ReadLine()
            };
            
            byte[] buffer;

            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(ms))
                {
                    writer.Write(sendData.Id);
                    writer.Write(sendData.Message);
                    buffer = ms.ToArray();
                }
            }

            Console.WriteLine($"Клиент: Отправка данных - Id: {sendData.Id}, Message: {sendData.Message}");
            clientStream.Write(buffer, 0, buffer.Length);
            clientStream.WaitForPipeDrain();

            // Получаем ответ от сервера
            byte[] responseBuffer = new byte[1024];
            int bytesRead = clientStream.Read(responseBuffer, 0, responseBuffer.Length);
            string responseMessage = Encoding.UTF8.GetString(responseBuffer, 0, bytesRead);
            Console.WriteLine($"Клиент: Получен ответ от сервера: <{responseMessage}>");
        }
    }
}