using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
class BookingTicketSystem
{
    public static int totalTickets = 10; // Total tickets availabe to purchase
    public static object lockObj1 = new object(); // Object Lock for Sync
    public static object lockObj2 = new object(); // Another lock created in case of deadlock

    // A method to buy a ticket and potential deadlock
    public static void BookTicket(int ID)
    {    Thread.Sleep(new Random().Next(100, 300)); // Delay
        lock (lockObj1){
            Console.WriteLine($"Customer {ID} is waiting for a ticket");
            Thread.Sleep(100); // Processing time

            lock (lockObj2){
                if (totalTickets > 0)
                {   totalTickets--;
                    Console.WriteLine($"Customer {ID} booked a ticket! Remaining: {totalTickets}");}
                else{
                    Console.WriteLine($"Customer {ID} failed to book. No tickets left!");}
            }
        }
    }
    
// Buy a ticket with deadlock prevention 
    public static void BookTicketResolution(int ID)
    {
        Thread.Sleep(new Random().Next(100, 400)); // Simulate delay
        bool Acquired1 = false;
        bool Acquired2 = false;

        try
        { // Lock 1
            Acquired1 = Monitor.TryEnter(lockObj1, TimeSpan.FromMilliseconds(500));
            if (Acquired1)
            {
                Console.WriteLine($"Customer {ID} acquired lockObj1...");
                Thread.Sleep(150);
                //Lock 2
                Acquired2 = Monitor.TryEnter(lockObj2, TimeSpan.FromMilliseconds(500));
                if (Acquired2)
                { if (totalTickets > 0)
                    { totalTickets--;
                        Console.WriteLine($"Customer {ID} booked a ticket! Remaining: {totalTickets}");
                    }else{
                        Console.WriteLine($"Customer {ID} failed to book. No tickets left!");
                    }
                }
            }
        }
        finally
        {    // Lock Relase
            if (Acquired2) Monitor.Exit(lockObj2);
            if (Acquired1) Monitor.Exit(lockObj1);
        }
    }
}

class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Ticket booking system starting...\n");
        List<Task> customer = new List<Task>();
        
        // Deadlock situatuon with multiple customers
        for (int i = 1; i <= 5; i++) // Creating a deadlock situation
        {   int customerID = i;
            customer.Add(Task.Run(() => BookingTicketSystem.BookTicket(customerID)));}
        Task.WaitAll(customer.ToArray());// All thread to complete

        Console.WriteLine("\nResolving Deadlock...\n");
        BookingTicketSystem.totalTickets = 10; // reset ticket count
        customer.Clear();
        
        // Running the deadlock free
        for (int i = 6; i <= 10; i++) // Running deadlock-free booking
        {int customerID = i;
            customer.Add(Task.Run(() => BookingTicketSystem.BookTicketResolution(customerID))); }
        
        Task.WaitAll(customer.ToArray()); // All threads to complete
        // Final Ticket count displayed
        Console.WriteLine($"All customers processed. Final tickets left: {BookingTicketSystem.totalTickets}");
    }
}
