ConfigurationManager.AppSettings["nombreEnAPPCONFIG"]


//DATA ARRIVAL DE WINSOCK


Private Sub Winsock1_DataArrival(ByVal bytesTotal As Long)


	
    Dim NumCar As Long  // LONGITUD DE LA INFORMACION QUE NECESITAMOS 
    Dim Info As String  //LA INFORMACION QUE NECESITAMOS 
    Dim Peso As String  // NO SE UTILIZA EN IS 30
    Dim pesoEstable As Boolean
    
   
 
 
    
	NumCar = bytesTotal
	Winsock1.GetData Info , vbString, NumCar // INFO es la variable, VBSTRING es el tipo y NUMCAR es la longitud de la recogida de datos a guardar en INFO 

	If Info <> Chr$(ACK) Then    //ACK ES LA SEÑAL DE RECONOCIMIENTO STANDARD, POR ESO EL SER DISTINTO DE ACK IMPLICA QUE HEMOS RECIBIDO INFORMACION VALIDA (INFO) Y POR ENDE PODEMOS PROCESARLA

            //Envia datos ACK
		Winsock1.SendData Chr$(ACK)  //CONFIRMA LA RECEPCION DE LOS DATOS AL SERVIDOR 
            
		pesoEstable = RecPeso_IS30_RED(Info) //PROCESA LOS DATOS CON LA FUNCION
    
		pesoEstable = PintaPesoST_Estable()
		CalculaEstadoCanalEstable True
        
            //TrataEstadoCanal
            //objMensajeLog.GuardaMensajeGeneral Now() & " Winsock1_DataArrival " & Info & vbCrLf & Peso & vbCrLf & gblPeso
            
	End If
    
 

End Sub


