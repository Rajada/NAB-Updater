<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ChangeCPForm
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ChangeCPForm))
        Me.CPFullRadioButton = New System.Windows.Forms.RadioButton()
        Me.CPLiteRadioButton = New System.Windows.Forms.RadioButton()
        Me.CPMinRadioButton = New System.Windows.Forms.RadioButton()
        Me.OtherTextBox = New System.Windows.Forms.TextBox()
        Me.SelectCPLabel = New System.Windows.Forms.Label()
        Me.CPOtherRadioButton = New System.Windows.Forms.RadioButton()
        Me.ChangeCPApply = New System.Windows.Forms.Button()
        Me.CPArchiveRadioButton = New System.Windows.Forms.RadioButton()
        Me.SuspendLayout()
        '
        'CPFullRadioButton
        '
        Me.CPFullRadioButton.AutoSize = True
        Me.CPFullRadioButton.Location = New System.Drawing.Point(18, 58)
        Me.CPFullRadioButton.Name = "CPFullRadioButton"
        Me.CPFullRadioButton.Size = New System.Drawing.Size(41, 17)
        Me.CPFullRadioButton.TabIndex = 1
        Me.CPFullRadioButton.TabStop = True
        Me.CPFullRadioButton.Text = "Full"
        Me.CPFullRadioButton.UseVisualStyleBackColor = True
        '
        'CPLiteRadioButton
        '
        Me.CPLiteRadioButton.AutoSize = True
        Me.CPLiteRadioButton.Location = New System.Drawing.Point(18, 81)
        Me.CPLiteRadioButton.Name = "CPLiteRadioButton"
        Me.CPLiteRadioButton.Size = New System.Drawing.Size(42, 17)
        Me.CPLiteRadioButton.TabIndex = 2
        Me.CPLiteRadioButton.TabStop = True
        Me.CPLiteRadioButton.Text = "Lite"
        Me.CPLiteRadioButton.UseVisualStyleBackColor = True
        '
        'CPMinRadioButton
        '
        Me.CPMinRadioButton.AutoSize = True
        Me.CPMinRadioButton.Location = New System.Drawing.Point(18, 102)
        Me.CPMinRadioButton.Name = "CPMinRadioButton"
        Me.CPMinRadioButton.Size = New System.Drawing.Size(60, 17)
        Me.CPMinRadioButton.TabIndex = 3
        Me.CPMinRadioButton.TabStop = True
        Me.CPMinRadioButton.Text = "Minimal"
        Me.CPMinRadioButton.UseVisualStyleBackColor = True
        '
        'OtherTextBox
        '
        Me.OtherTextBox.Location = New System.Drawing.Point(100, 125)
        Me.OtherTextBox.MaxLength = 16
        Me.OtherTextBox.Name = "OtherTextBox"
        Me.OtherTextBox.Size = New System.Drawing.Size(100, 20)
        Me.OtherTextBox.TabIndex = 5
        '
        'SelectCPLabel
        '
        Me.SelectCPLabel.AutoSize = True
        Me.SelectCPLabel.Location = New System.Drawing.Point(15, 9)
        Me.SelectCPLabel.Name = "SelectCPLabel"
        Me.SelectCPLabel.Size = New System.Drawing.Size(182, 13)
        Me.SelectCPLabel.TabIndex = 0
        Me.SelectCPLabel.Text = "Select a Community Pack type below"
        '
        'CPOtherRadioButton
        '
        Me.CPOtherRadioButton.AutoSize = True
        Me.CPOtherRadioButton.Location = New System.Drawing.Point(18, 125)
        Me.CPOtherRadioButton.Name = "CPOtherRadioButton"
        Me.CPOtherRadioButton.Size = New System.Drawing.Size(51, 17)
        Me.CPOtherRadioButton.TabIndex = 4
        Me.CPOtherRadioButton.TabStop = True
        Me.CPOtherRadioButton.Text = "Other"
        Me.CPOtherRadioButton.UseVisualStyleBackColor = True
        '
        'ChangeCPApply
        '
        Me.ChangeCPApply.Location = New System.Drawing.Point(18, 158)
        Me.ChangeCPApply.Name = "ChangeCPApply"
        Me.ChangeCPApply.Size = New System.Drawing.Size(75, 23)
        Me.ChangeCPApply.TabIndex = 6
        Me.ChangeCPApply.Text = "Apply"
        Me.ChangeCPApply.UseVisualStyleBackColor = True
        '
        'CPArchiveRadioButton
        '
        Me.CPArchiveRadioButton.AutoSize = True
        Me.CPArchiveRadioButton.Location = New System.Drawing.Point(18, 35)
        Me.CPArchiveRadioButton.Name = "CPArchiveRadioButton"
        Me.CPArchiveRadioButton.Size = New System.Drawing.Size(61, 17)
        Me.CPArchiveRadioButton.TabIndex = 7
        Me.CPArchiveRadioButton.TabStop = True
        Me.CPArchiveRadioButton.Text = "Archive"
        Me.CPArchiveRadioButton.UseVisualStyleBackColor = True
        '
        'ChangeCPForm
        '
        Me.AcceptButton = Me.ChangeCPApply
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(224, 196)
        Me.Controls.Add(Me.CPArchiveRadioButton)
        Me.Controls.Add(Me.ChangeCPApply)
        Me.Controls.Add(Me.CPOtherRadioButton)
        Me.Controls.Add(Me.SelectCPLabel)
        Me.Controls.Add(Me.OtherTextBox)
        Me.Controls.Add(Me.CPMinRadioButton)
        Me.Controls.Add(Me.CPLiteRadioButton)
        Me.Controls.Add(Me.CPFullRadioButton)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "ChangeCPForm"
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Change Community Pack Type"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents CPFullRadioButton As RadioButton
    Friend WithEvents CPLiteRadioButton As RadioButton
    Friend WithEvents CPMinRadioButton As RadioButton
    Friend WithEvents OtherTextBox As TextBox
    Friend WithEvents SelectCPLabel As Label
    Friend WithEvents CPOtherRadioButton As RadioButton
    Friend WithEvents ChangeCPApply As Button
    Friend WithEvents CPArchiveRadioButton As RadioButton
End Class
