<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ChangelogForm
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
    Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ChangelogForm))
    Me.ChangelogTextbox = New System.Windows.Forms.RichTextBox()
    Me.SuspendLayout()
    '
    'ChangelogTextbox
    '
    Me.ChangelogTextbox.Cursor = System.Windows.Forms.Cursors.Default
    Me.ChangelogTextbox.DetectUrls = False
    Me.ChangelogTextbox.Location = New System.Drawing.Point(12, 12)
    Me.ChangelogTextbox.Name = "ChangelogTextbox"
    Me.ChangelogTextbox.ReadOnly = True
    Me.ChangelogTextbox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical
    Me.ChangelogTextbox.Size = New System.Drawing.Size(560, 340)
    Me.ChangelogTextbox.TabIndex = 1
    Me.ChangelogTextbox.Text = ""
    '
    'ChangelogForm
    '
    Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
    Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
    Me.ClientSize = New System.Drawing.Size(584, 364)
    Me.Controls.Add(Me.ChangelogTextbox)
    Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
    Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
    Me.MaximizeBox = False
    Me.MaximumSize = New System.Drawing.Size(600, 400)
    Me.MinimizeBox = False
    Me.MinimumSize = New System.Drawing.Size(600, 400)
    Me.Name = "ChangelogForm"
    Me.Text = "Changelog"
    Me.ResumeLayout(False)

  End Sub

  Friend WithEvents ChangelogTextbox As RichTextBox
End Class
