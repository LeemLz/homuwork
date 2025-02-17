'DATA ARRIVAL DE WINSOCK


Private Sub myBasculaIS30_DataArrival(ByVal bytesTotal As Long)


	
    Dim NumCar As Long  ' LONGITUD DE LA INFORMACION QUE NECESITAMOS 
    Dim Info As String  'LA INFORMACION QUE NECESITAMOS 
    Dim Peso As String  ' NO SE UTILIZA EN IS 30
    Dim pesoEstable As Boolean
    
   
 
 
    
	NumCar = bytesTotal
	Winsock1.GetData Info , vbString, NumCar ' INFO es la variable, VBSTRING es el tipo y NUMCAR es la longitud de la recogida de datos a guardar en INFO 

	If Info <> Chr$(ACK) Then    'ACK ES LA SEÑAL DE RECONOCIMIENTO STANDARD, POR ESO EL SER DISTINTO DE ACK IMPLICA QUE HEMOS RECIBIDO INFORMACION VALIDA (INFO) Y POR ENDE PODEMOS PROCESARLA

            'Envia datos ACK
		Winsock1.SendData Chr$(ACK)  'CONFIRMA LA RECEPCION DE LOS DATOS AL SERVIDOR 
            
		pesoEstable = RecPeso_IS30_RED(Info) 'Esta función procesa los datos recibidos (Datos) y determina si el peso recibido es estable. LOS GUARDA EN GBLPESO
    
		pesoEstable = PintaPesoST_Estable()   'Esta función muestra el peso estable y realiza algunas acciones adicionales basadas en el estado de objPartida vuelve a guardar en GBLPESO .

		CalculaEstadoCanalEstable True
        
            'TrataEstadoCanal
            'objMensajeLog.GuardaMensajeGeneral Now() & " Winsock1_DataArrival " & Info & vbCrLf & Peso & vbCrLf & gblPeso
            
	End If
    
 

End Sub

'RecPeso_IS30_RED 'Esta función procesa los datos recibidos (Datos) y determina si el peso recibido es estable. LOS GUARDA EN GBLPESO

Private Function RecPeso_IS30_RED(Datos As String) As Boolean
    Dim PesSt As String
    Dim pesoEstable As Boolean
    
    Dim status As String
    Dim iStatus As Integer
    Dim iMod As Integer
    
    Dim signo As String
    Dim sinTara As String
    
    'Const SOH = Chr(1)
    'Const STX = Chr(2)
    'Const ETX = Chr(3)
    'Const ENQ = Chr(5)
    'Const ACK = Chr(6)
    'Const LF = Chr(10)
    'Const CR = Chr(13)
    'Const NACK = Chr(21)
    'Const GS = Chr(29)
    
    
    Dim patron As String
    'patron = "^" + Chr(1) & "0" & Chr(3) & "1" & Chr(3) & "254" & Chr(3) & "I!GV01\S*\|GD01\|(\S*)\|\S*GD02\S*LX02" & Chr(13) + Chr(10) + "$"
    patron = "^" + Chr(1) & "0" & Chr(3) & "1" & Chr(3) & "254" & Chr(3) & "I!RV04\S*\|GD01\|(\S*)\|\S*GD02\S*LX02" & Chr(13) + Chr(10) + "$"
    ' patron = "^" + "I!GV01\S*\|GD01\|(\S*)\|\S*GD02\S*LX02" + "$"

    Dim rexpEntrada As RegExp
    'Dim myMatches As MatchCollection
    'Dim myMatch As Match
    
    Set rexpEntrada = New RegExp
    rexpEntrada.Pattern = patron

    If rexpEntrada.Test(Datos) Then
    
        pesoEstable = True
        
        Dim strPeso As String
        strPeso = (rexpEntrada.Replace(Datos, "$1"))
        
        Dim wordsPeso() As String
        wordsPeso = Split(strPeso, ";")
        
        Dim nDecimales As Integer
        Dim iPeso As Integer
        Dim dPeso As Double
        Dim i As Integer
        For i = 0 To UBound(wordsPeso)
        
            If i = 0 Then
            
            ElseIf i = 1 Then
                nDecimales = wordsPeso(i)
                nDecimales = Abs(nDecimales)
            ElseIf i = 2 Then
                iPeso = wordsPeso(i)
            End If
                        
        Next
        
        dPeso = iPeso / (10 ^ nDecimales)


        gblPeso = dPeso   'Aqui guarda el peso


        NetoLbl.Visible = True
        
       
    Else
        pesoEstable = False
    End If
    
    RecPeso_IS30_RED = pesoEstable
    
End Function


'PintaPesosST_Estable

Private Function PintaPesoST_Estable() As Boolean 'Esta función muestra el peso estable y realiza algunas acciones adicionales basadas en el estado de objPartida.

    Dim mpesoEstable As Boolean
    Dim PesoST As Double

    
	mpesoEstable = True
	PesoST = gblPeso
        
   

    
    gblPeso = CDbl(PesoST) 'Incluido para obtener el peso como una variable global
    If gblPeso > 10 And Not objPartida Is Nothing Then
    
        gblPeso = gblPeso - objPartida.Tara
        pesoDefinitivoDeVerdad = gblPeso
        
        If objPartida.EnEdicion Then
                                
           ' Vuelve a permitir la edici?n
            AvanceBtn.Enabled = True
            RetrocedeBtn.Enabled = True
            
            objUF.ReInit
            objRechazo.ReInit
            objDecomiso.ReInit
                       
            If objPartida.esDelConsejo Then
                gblTipoSexo = TipoSexo.Macho
            Else
                gblTipoSexo = TipoSexo.SC
            End If
            
            'Al Entran en b?scula se sale de edici?n
            objPartida.EnEdicion = False
            BtnCambiar.Visible = False
            btnBorrarCanal.Visible = False
                    
            objPartida.AvanceCanalFinal
                    
            PantallaInicial
            
        End If
        
        WeightLbl.Caption = Format(pesoDefinitivoDeVerdad, "##0.000")
        KGLbl.ForeColor = ColorVerde
        
    End If
    
    ' NetoLbl.Visible = (tara <> 0)

    PintaPesoST_Estable = mpesoEstable 'Incluido para saber si el peso es estable
    
End Function
