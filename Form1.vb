' Regatta Mate
' Arduino Nano and Visual Basic
' Capture of finish margins, storing and editing.

Imports System.IO
Imports System.IO.Ports

Public Class FrmMain
    ' Global variables.
    ' Anything defined here is available in the whole app
    Dim ArduinoConnected As Boolean

    ' This called when the app first starts. Used to initial what ever needs initialising.
    Private Sub FrmMain_Load(sender As Object, e As EventArgs) Handles Me.Load
        btnConnect.BackColor = Color.IndianRed
        TimerConnect.Enabled = False
        ArduinoConnected = False
        autoconnect() 'Search for Finish control
        'Me.Show()
    End Sub

    ' Try to open the com port or close the port if already open
    ' Note: There is no error catching. If the connection attampt fails the app will crash!
    Private Sub btnConnect_Click(sender As Object, e As EventArgs) Handles btnConnect.Click
        If (btnConnect.Text = "CONNECT") Then
            autoconnect()
        Else
            Try
                SerialPort1.Close()
                btnConnect.Text = "CONNECT"
                ArduinoConnected = False
            Catch ex As Exception
                MessageBox.Show("Serial Port is already closed!")
            End Try
        End If
    End Sub
    'Seach open serial port for connection, send "3/" and listen for reply
    Private Sub autoconnect()
        ' Dafault Port parameters
        SerialPort1.BaudRate = 9600                         '**
        SerialPort1.DataBits = 8                            '**
        SerialPort1.Parity = Parity.None                    '**
        SerialPort1.StopBits = StopBits.One                 '**
        SerialPort1.Handshake = Handshake.None              '**
        SerialPort1.Encoding = System.Text.Encoding.Default '**

        SerialPort1.DtrEnable = True
        SerialPort1.RtsEnable = True
        SerialPort1.ReadTimeout = 10000

        For Each sp As String In My.Computer.Ports.SerialPortNames
            Try
                ' Try each port found
                SerialPort1.PortName = sp
                SerialPort1.Open()
                'Sent msg and wait for response
                SerialPort1.Write("3")
                TimerConnect.Interval = 500
                TimerConnect.Start()
                Dim str As String = SerialPort1.ReadExisting()
                If str.Contains("51") Then
                    'MsgBox("Control Found " + SerialPort1.PortName + " - " + str)
                    ArduinoConnected = True
                    btnConnect.Text = "Connected !" + SerialPort1.PortName
                End If
                While TimerConnect.Enabled And Not (ArduinoConnected)
                    Application.DoEvents()
                End While
                If ArduinoConnected Then
                    Exit For
                End If
                SerialPort1.Close()
            Catch ex As Exception
                MsgBox(ex.Message)
            End Try
        Next
        ' Format Label and Button depending on connection status
        If ArduinoConnected Then
            lblConnect.Text = "Connectet on: " + SerialPort1.PortName
            btnConnect.Text = "DIS-CONNECT"
            btnConnect.BackColor = SystemColors.Control
            'TimerConnect.Enabled = True
            btnConnect.Text = "Disconnect" '' old button
        Else
            lblConnect.Text = "Control Not connected"
            btnConnect.Text = "CONNECT"
            btnConnect.BackColor = Color.IndianRed
            Timer1.Enabled = False
            MsgBox("Timing control failed to connect. Please check that it is plugged in.")
        End If
    End Sub

    ' Timer for arduino search
    Public Sub TimerConnect_Tick(sender As Object, e As EventArgs) Handles TimerConnect.Tick
        TimerConnect.Stop()
    End Sub
End Class
