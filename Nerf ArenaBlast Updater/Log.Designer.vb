<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class LogViewerForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(LogViewerForm))
        Me.logtext = New System.Windows.Forms.RichTextBox()
        Me.SuspendLayout()
        '
        'logtext
        '
        Me.logtext.Location = New System.Drawing.Point(12, 12)
        Me.logtext.Name = "logtext"
        Me.logtext.Size = New System.Drawing.Size(560, 337)
        Me.logtext.TabIndex = 0
        Me.logtext.Text = ""
        '
        'LogViewerForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(584, 361)
        Me.Controls.Add(Me.logtext)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "LogViewerForm"
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
        Me.Text = "Log"
        Me.ResumeLayout(False)

    End Sub

    Private Sub log_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        UpdaterMainForm.LogViewer = Nothing
    End Sub

    Friend WithEvents logtext As RichTextBox
End Class
