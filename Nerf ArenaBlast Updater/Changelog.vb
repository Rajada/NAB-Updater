Option Strict On
Imports System.IO

Public Class ChangelogForm
  Private Sub Changelog_Load(sender As Object, e As EventArgs) Handles Me.Load
    Dim changelogDirectory As String = (Directory.GetCurrentDirectory() + "\changelog.txt")
    Dim reader As StreamReader
        Text = UpdaterMainForm.locString_Window_Changelog
        If Not (My.Computer.FileSystem.FileExists(changelogDirectory)) Then
            ChangelogTextbox.Text = UpdaterMainForm.locString_Caption_ChangelogMissing
        Else
            reader = New StreamReader(changelogDirectory)
            ChangelogTextbox.Text = reader.ReadToEnd
            reader.Close()
        End If
    End Sub
  Private Sub ChangelogClosing(sender As Object, e As EventArgs) Handles Me.FormClosing
    UpdaterMainForm.Changelog = Nothing
  End Sub
End Class