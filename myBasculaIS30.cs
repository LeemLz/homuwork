using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;


public class MyBasculaIS30
{
    private Socket _socket;
    private string _ipAddress;
    private int _port;

    //public gblpeso = 0.0;  // Variable bascular para guardar el peso

    // Constructor
    public MyBasculaIS30(string ipAddress, string port)
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

    public void conectarServer()
    {
        this.StartServer();
    }

    public void conectarClient()
    {
        this.Connect();
    }

    private void MyBasculaIS30_DataArrival(long bytesTotal)
    {
        long NumCar = bytesTotal;  // LONGITUD DE LA INFORMACION QUE NECESITAMOS
        string Info = string.Empty;  // LA INFORMACION QUE NECESITAMOS
        string Peso = string.Empty;  // NO SE UTILIZA EN IS 30
        bool pesoEstable;

        // Obtener los datos recibidos
        byte[] buffer = new byte[NumCar];
        _socket.Receive(buffer);
        Info = Encoding.UTF8.GetString(buffer);

        // ACK ES LA SEÑAL DE RECONOCIMIENTO STANDARD, POR ESO EL SER DISTINTO DE ACK IMPLICA QUE HEMOS RECIBIDO INFORMACION VALIDA (INFO) Y POR ENDE PODEMOS PROCESARLA
        if (Info != ((char)6).ToString())
        {
            // Envia datos ACK
            byte[] ack = Encoding.UTF8.GetBytes(((char)6).ToString());
            _socket.Send(ack);  // CONFIRMA LA RECEPCION DE LOS DATOS AL SERVIDOR

            pesoEstable = RecPeso_IS30_RED(Info);  // Esta función procesa los datos recibidos y determina si el peso recibido es estable
            pesoEstable = PintaPesoST_Estable();   // Esta función muestra el peso estable y realiza algunas acciones adicionales basadas en el estado de objPartida

            CalculaEstadoCanalEstable(true);
        }
    }

    public bool RecPeso_IS30_RED(string datos)   // Esta función procesa los datos recibidos y determina si el peso recibido es estable
    {
        string PesSt;
        bool pesoEstable;

        string status;
        int iStatus;
        int iMod;

        string signo;
        string sinTara;

        string patron = "^" + ((char)1) + "0" + ((char)3) + "1" + ((char)3) + "254" + ((char)3) + "I!RV04\\S*\\|GD01\\|(\\S*)\\|\\S*GD02\\S*LX02" + ((char)13) + ((char)10) + "$";

        System.Text.RegularExpressions.Regex rexpEntrada = new System.Text.RegularExpressions.Regex(patron);

        if (rexpEntrada.IsMatch(datos))
            {
             pesoEstable = true;
        
                string strPeso = rexpEntrada.Replace(datos, "$1");
                string[] wordsPeso = strPeso.Split(';');

                int nDecimales = 0;
                int iPeso = 0;
                double dPeso = 0.0;

                for (int i = 0; i <= wordsPeso.Length - 1; i++)
                {   
                    if (i == 0)
                    {
                        // No se hace nada
                    }
                    else if (i == 1)
                    {
                        nDecimales = int.Parse(wordsPeso[i]);
                        nDecimales = Math.Abs(nDecimales);
                    }
                        
                    else if (i == 2)
                    {
                        iPeso = int.Parse(wordsPeso[i]);
                    }
                }

                dPeso = iPeso / Math.Pow(10, nDecimales);
                gblPeso = dPeso;   // Aqui guarda el peso

                NetoLbl.Visible = true;
            }
            else
            {
                pesoEstable = false;
            }

            return pesoEstable;
    }

    public bool PintaPesoST_Estable()  // Esta función muestra el peso estable y realiza algunas acciones adicionales basadas en el estado de objPartida
        {
            bool mpesoEstable;
            double PesoST;

            mpesoEstable = true;
            PesoST = gblPeso;

            gblPeso = Convert.ToDouble(PesoST); // Incluido para obtener el peso como una variable global
            if (gblPeso > 10 && objPartida != null)
            {
                gblPeso -= objPartida.Tara;
                pesoDefinitivoDeVerdad = gblPeso;

                if (objPartida.EnEdicion)
                {
                    // Vuelve a permitir la edición
                    AvanceBtn.Enabled = true;
                    RetrocedeBtn.Enabled = true;

                    objUF.ReInit();
                    objRechazo.ReInit();
                    objDecomiso.ReInit();

                    if (objPartida.EsDelConsejo)
                    {
                        gblTipoSexo = TipoSexo.Macho;
                    }
                    else
                    {
                        gblTipoSexo = TipoSexo.SC;
                    }

                    // Al entrar en báscula se sale de edición
                    objPartida.EnEdicion = false;
                    BtnCambiar.Visible = false;
                    btnBorrarCanal.Visible = false;

                    objPartida.AvanceCanalFinal();

                 PantallaInicial();
                }

                WeightLbl.Caption = pesoDefinitivoDeVerdad.ToString("F3");
                KGLbl.ForeColor = ColorVerde;
            }

            // NetoLbl.Visible = (tara != 0);

            return mpesoEstable; // Incluido para saber si el peso es estable
        }
}






