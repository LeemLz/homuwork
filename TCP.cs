using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class MyTCP
{
    private Socket _socket;
    private string _ipAddress;
    private int _port;

    // Constructor
    public MyTCP(string ipAddress, string port)
    {
        _ipAddress = ipAddress;
        _port = int.Parse(port);
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    }

    // Método para iniciar el servidor
    public void StartServer()
    {
        IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(_ipAddress), _port);
        _socket.Bind(localEndPoint);
        _socket.Listen(10);

        Console.WriteLine($"Servidor iniciado en {_ipAddress}:{_port}. Esperando conexiones...");

        Socket clientSocket = _socket.Accept();
        Console.WriteLine("Conexión aceptada.");

        // Leer datos del cliente
        byte[] buffer = new byte[1024];
        int bytesRead = clientSocket.Receive(buffer);
        Console.WriteLine($"Datos recibidos: {Encoding.UTF8.GetString(buffer, 0, bytesRead)}");

        // Cerrar el socket
        clientSocket.Close();
        _socket.Close();
    }

    // Método para conectarse a un servidor
    public void Connect()
    {
        _socket.Connect(new IPEndPoint(IPAddress.Parse(_ipAddress), _port));
        Console.WriteLine($"Conectado a {_ipAddress}:{_port}");

        // Enviar datos de ejemplo al servidor
        string message = "Hola desde el cliente!";
        byte[] data = Encoding.UTF8.GetBytes(message);
        _socket.Send(data);

        // Cerrar el socket
        _socket.Close();
    }
}


// Crear instancia del servidor
MyTCP serverTCP = new MyTCP("127.0.0.1", "11000");
serverTCP.StartServer();

// Crear instancia del cliente
MyTCP clientTCP = new MyTCP("127.0.0.1", "11000");
clientTCP.Connect();
















//info@conectaconsultores.es

