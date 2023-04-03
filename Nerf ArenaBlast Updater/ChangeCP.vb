Imports System.IO

Public Class ChangeCPForm
    Private PreviousVariant As String = ""
    Private NewVariant As String = ""

    Private Sub ChangeCP_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If (UpdaterMainForm.cpVariantString = "Full") Then
            PreviousVariant = "Full"
            CPFullRadioButton.Checked = True
            CPLiteRadioButton.Checked = False
            CPMinRadioButton.Checked = False
            CPOtherRadioButton.Checked = False
            OtherTextBox.Enabled = False
        ElseIf (UpdaterMainForm.cpVariantString = "Lite") Then
            PreviousVariant = "Lite"
            CPFullRadioButton.Checked = False
            CPLiteRadioButton.Checked = True
            CPMinRadioButton.Checked = False
            CPOtherRadioButton.Checked = False
            OtherTextBox.Enabled = False
        ElseIf (UpdaterMainForm.cpVariantString = "Minimal") Then
            PreviousVariant = "Minimal"
            CPFullRadioButton.Checked = False
            CPLiteRadioButton.Checked = False
            CPMinRadioButton.Checked = True
            CPOtherRadioButton.Checked = False
            OtherTextBox.Enabled = False
        Else
            PreviousVariant = UpdaterMainForm.cpVariantString
            CPFullRadioButton.Checked = False
            CPLiteRadioButton.Checked = False
            CPMinRadioButton.Checked = False
            CPOtherRadioButton.Checked = True
            OtherTextBox.Enabled = True
            OtherTextBox.Text = UpdaterMainForm.cpVariantString
        End If
    End Sub

    Private Sub CPFullRadioButton_CheckedChanged(sender As Object, e As EventArgs) Handles CPFullRadioButton.CheckedChanged
        If (CPFullRadioButton.Checked) Then
            CPLiteRadioButton.Checked = False
            CPMinRadioButton.Checked = False
            CPOtherRadioButton.Checked = False
            OtherTextBox.Enabled = False
            NewVariant = "Full"
        End If
    End Sub

    Private Sub CPLiteRadioButton_CheckedChanged(sender As Object, e As EventArgs) Handles CPLiteRadioButton.CheckedChanged
        If (CPLiteRadioButton.Checked) Then
            CPFullRadioButton.Checked = False
            CPMinRadioButton.Checked = False
            CPOtherRadioButton.Checked = False
            OtherTextBox.Enabled = False
            NewVariant = "Lite"
        End If
    End Sub

    Private Sub CPMinRadioButton_CheckedChanged(sender As Object, e As EventArgs) Handles CPMinRadioButton.CheckedChanged
        If (CPMinRadioButton.Checked) Then
            CPFullRadioButton.Checked = False
            CPLiteRadioButton.Checked = False
            CPOtherRadioButton.Checked = False
            OtherTextBox.Enabled = False
            NewVariant = "Minimal"
        End If
    End Sub

    Private Sub CPOtherRadioButton_CheckedChanged(sender As Object, e As EventArgs) Handles CPOtherRadioButton.CheckedChanged
        If (CPOtherRadioButton.Checked) Then
            CPFullRadioButton.Checked = False
            CPLiteRadioButton.Checked = False
            CPMinRadioButton.Checked = False
            OtherTextBox.Enabled = True
            NewVariant = OtherTextBox.Text
        End If
    End Sub

    Private Sub ChangeCPApply_Click(sender As Object, e As EventArgs) Handles ChangeCPApply.Click
        If (NewVariant <> PreviousVariant) Then
            UpdaterMainForm.CPVariantChanged(NewVariant)
        End If
        Close()
    End Sub

    Private Sub OtherTextBox_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles OtherTextBox.KeyPress
        If Not Char.IsLetterOrDigit(e.KeyChar) Then
            e.Handled = True
        End If
    End Sub

    Private Sub OtherTextBox_TextChanged(sender As Object, e As EventArgs) Handles OtherTextBox.TextChanged
        NewVariant = OtherTextBox.Text
    End Sub

    Private Sub ChangeCPClosing(sender As Object, e As EventArgs) Handles Me.FormClosing
        UpdaterMainForm.ChangeCP = Nothing
    End Sub
End Class