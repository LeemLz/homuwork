ConfigurationManager.AppSettings["nombreEnAPPCONFIG"]


//DATA ARRIVAL DE WINSOCK


Private Sub Winsock1_DataArrival(ByVal bytesTotal As Long)


	
    Dim NumCar As Long
    Dim Info As String
    Dim Peso As String
    Dim pesoEstable As Boolean
    
   
 
 
    
	NumCar = bytesTotal
	Winsock1.GetData Info, vbString, NumCar

	If Info <> Chr$(ACK) Then

            //Envia datos ACK
		Winsock1.SendData Chr$(ACK)
            
		pesoEstable = RecPeso_IS30_RED(Info)
    
		pesoEstable = PintaPesoST_Estable()
		CalculaEstadoCanalEstable True
        
            //TrataEstadoCanal
            //objMensajeLog.GuardaMensajeGeneral Now() & " Winsock1_DataArrival " & Info & vbCrLf & Peso & vbCrLf & gblPeso
            
	End If
    
 

End Sub


