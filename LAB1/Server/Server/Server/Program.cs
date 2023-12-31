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
        using (NamedPipeServerStream serverStream = new NamedPipeServerStream("myPipe", PipeDirection.InOut))
        {
            Console.WriteLine("Сервер запущен:");
            serverStream.WaitForConnection();
            Console.WriteLine("Клиент подключен.");

            byte[] buffer = new byte[1024];

            while (true)
            {
                int bytesRead = serverStream.Read(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    CustomData receivedData = new CustomData(); 

                    using (MemoryStream ms = new MemoryStream(buffer, 0, bytesRead))
                    {
                        using (BinaryReader reader = new BinaryReader(ms))
                        {
                            receivedData.Id = reader.ReadInt32();
                            receivedData.Message = reader.ReadString();
                            Console.WriteLine($"Сервер: Получено - Num: {receivedData.Id}, Text: {receivedData.Message}");

                            // Отправляем ответ клиенту
                            string responseMessage = "Сервер: Принято!";
                            byte[] responseBytes = Encoding.UTF8.GetBytes(responseMessage);
                            serverStream.Write(responseBytes, 0, responseBytes.Length);
                            serverStream.WaitForPipeDrain();
                            Console.WriteLine(responseMessage);
                        }
                    }
                }
            }
        }
    }
}