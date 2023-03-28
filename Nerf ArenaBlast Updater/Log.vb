Imports System.IO

Public Class LogViewerForm
    Private Sub LogLoad(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim logDirectory As String = (Directory.GetCurrentDirectory() + "\NerfUpdater.Log")
        Dim reader As StreamReader
        Text = UpdaterMainForm.locString_Window_Log
        If (My.Computer.FileSystem.FileExists(logDirectory)) Then
            reader = New StreamReader(logDirectory)
            logtext.Text = reader.ReadToEnd
            reader.Close()
        End If
    End Sub
End Class