Imports System
Imports System.Drawing
Imports System.Windows.Forms
Imports System.IO
Imports IWshRuntimeLibrary
Imports Microsoft.VisualBasic
Imports System.Data.OleDb

Module modStartup
    Public myLogger As clsLogger

    Public Sub Main(ByVal args() As String)
        Dim Silent As Boolean

        System.Windows.Forms.Application.EnableVisualStyles()

        myLogger = New clsLogger(Application.StartupPath & "\EventLog.log")
        myLogger.AddSeparator(False)
        myLogger.AddSeparator(True)
        myLogger.AddLog(Path.GetFileName(Application.ExecutablePath) & " application started")

        Dim s As String
        For Each s In args
            Select Case LCase(s)
                Case "-s"
                    Silent = True
                Case Else
                    'Do something here
            End Select
        Next

        If Not Silent Then
            System.Windows.Forms.Application.Run(New frmMain)
        Else
            myLogger.AddLog("Running in silent mode")
        End If

        myLogger.AddLog(Path.GetFileName(Application.ExecutablePath) & " application exited")
        myLogger.Dispose()
        myLogger = Nothing
    End Sub
End Module



