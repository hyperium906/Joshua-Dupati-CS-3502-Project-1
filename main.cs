using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

// Ticket System that communicates logger proccess using pipes 

class TicketBookingSystem
{
    public static void Run()
    {
        Console.WriteLine("[Booking System] Starting...");
        Task.Run(() => Logger.Run()); // Start the process in a separate task
        Thread.Sleep(1500); // logger time to start
        
        // piper sever for the IPC
        using (NamedPipeServerStream Serverpipe = new NamedPipeServerStream("TicketPipe"))
        {   Console.WriteLine("[Booking System] Waiting for logger to connect...");
            Serverpipe.WaitForConnection();
            Console.WriteLine("[Booking System] Logger connected!");
           // Write the details into the pipe
            using (StreamWriter writer = new StreamWriter(Serverpipe))
            {
                writer.AutoFlush = true;
                for (int i = 1; i <= 5; i++)
                {
                    string Info = $"Customer {i} booked a ticket.";
                    Console.WriteLine("[Booking System] Sending: " + Info);
                    writer.WriteLine(Info); // Send booking
                    Thread.Sleep(800); // delay between bookings
                }
            }
        }
        Console.WriteLine("[Booking System] All bookings sent. Exiting...");
    }
}

// Read the information from the pipe
class Logger
{
    public static void Run()
    {
        Console.WriteLine("[Logger] Waiting for Booking System...");
        // Connect the pipe
        using (NamedPipeClientStream Client = new NamedPipeClientStream("TicketPipe"))
        {
            Client.Connect();// Connect to the pipe
            Console.WriteLine("Logger: Connected to Booking System!");
            
            // Read then display the messages
            using (StreamReader reader = new StreamReader(Client))
            {   string message;
                while ((message = reader.ReadLine()) != null)
                {
                    Console.WriteLine("Logger: Received: " + message);
                }
            }
        }
        Console.WriteLine("Logger: Exiting...");
    }
}

class Program
{
    public static void Main()
    {
        TicketBookingSystem.Run();
    }
}
