<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class UpdaterMainForm
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UpdaterMainForm))
        Me.updateButton = New System.Windows.Forms.Button()
        Me.exitButton = New System.Windows.Forms.Button()
        Me.changeFilepathButton = New System.Windows.Forms.Button()
        Me.updatePathLabel = New System.Windows.Forms.Label()
        Me.updateProgressBar = New System.Windows.Forms.ProgressBar()
        Me.customCheckBox = New System.Windows.Forms.CheckBox()
        Me.selectAllButton = New System.Windows.Forms.Button()
        Me.deselectAllButton = New System.Windows.Forms.Button()
        Me.nerfNetPlugLabel = New System.Windows.Forms.LinkLabel()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ChangeBaseDirectoryToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        Me.CheckForUpdatesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SelectAllToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DeselectAllToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator()
        Me.GetLatestCommunityPackToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.HelpToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ViewHelpToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.VisitWebsiteToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.AboutToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.VersionToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CheckForNewVersionToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ViewChangelogToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OptionsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.LanguageToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.InternationalEnglishDefaultToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator5 = New System.Windows.Forms.ToolStripSeparator()
        Me.OpenGameDirectoryToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AdvancedToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AdvancedModeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ShowLatestUpdateIDToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.cleanupCheckBox = New System.Windows.Forms.CheckBox()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.lastUpdateLabel = New System.Windows.Forms.Label()
        Me.revertCheckBox = New System.Windows.Forms.CheckBox()
        Me.outputTextbox = New System.Windows.Forms.Label()
        Me.updateFilesTreeView = New Nerf_ArenaBlast_Updater.MyTreeView()
        Me.MenuStrip1.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'updateButton
        '
        resources.ApplyResources(Me.updateButton, "updateButton")
        Me.updateButton.Name = "updateButton"
        Me.updateButton.UseVisualStyleBackColor = True
        '
        'exitButton
        '
        Me.exitButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
        resources.ApplyResources(Me.exitButton, "exitButton")
        Me.exitButton.Name = "exitButton"
        Me.exitButton.UseVisualStyleBackColor = True
        '
        'changeFilepathButton
        '
        resources.ApplyResources(Me.changeFilepathButton, "changeFilepathButton")
        Me.changeFilepathButton.Name = "changeFilepathButton"
        Me.changeFilepathButton.UseVisualStyleBackColor = True
        '
        'updatePathLabel
        '
        Me.updatePathLabel.AutoEllipsis = True
        Me.updatePathLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        resources.ApplyResources(Me.updatePathLabel, "updatePathLabel")
        Me.updatePathLabel.Name = "updatePathLabel"
        '
        'updateProgressBar
        '
        resources.ApplyResources(Me.updateProgressBar, "updateProgressBar")
        Me.updateProgressBar.Name = "updateProgressBar"
        '
        'customCheckBox
        '
        resources.ApplyResources(Me.customCheckBox, "customCheckBox")
        Me.customCheckBox.Name = "customCheckBox"
        Me.customCheckBox.UseVisualStyleBackColor = True
        '
        'selectAllButton
        '
        resources.ApplyResources(Me.selectAllButton, "selectAllButton")
        Me.selectAllButton.Name = "selectAllButton"
        Me.selectAllButton.UseVisualStyleBackColor = True
        '
        'deselectAllButton
        '
        resources.ApplyResources(Me.deselectAllButton, "deselectAllButton")
        Me.deselectAllButton.Name = "deselectAllButton"
        Me.deselectAllButton.UseVisualStyleBackColor = True
        '
        'nerfNetPlugLabel
        '
        resources.ApplyResources(Me.nerfNetPlugLabel, "nerfNetPlugLabel")
        Me.nerfNetPlugLabel.Name = "nerfNetPlugLabel"
        Me.nerfNetPlugLabel.TabStop = True
        '
        'MenuStrip1
        '
        Me.MenuStrip1.BackColor = System.Drawing.Color.WhiteSmoke
        resources.ApplyResources(Me.MenuStrip1, "MenuStrip1")
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem, Me.HelpToolStripMenuItem, Me.VersionToolStripMenuItem, Me.OptionsToolStripMenuItem, Me.AdvancedToolStripMenuItem})
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
        '
        'FileToolStripMenuItem
        '
        Me.FileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ChangeBaseDirectoryToolStripMenuItem, Me.ToolStripSeparator3, Me.CheckForUpdatesToolStripMenuItem, Me.SelectAllToolStripMenuItem, Me.DeselectAllToolStripMenuItem, Me.ToolStripSeparator4, Me.GetLatestCommunityPackToolStripMenuItem, Me.ToolStripSeparator1, Me.ExitToolStripMenuItem})
        Me.FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        resources.ApplyResources(Me.FileToolStripMenuItem, "FileToolStripMenuItem")
        '
        'ChangeBaseDirectoryToolStripMenuItem
        '
        Me.ChangeBaseDirectoryToolStripMenuItem.Name = "ChangeBaseDirectoryToolStripMenuItem"
        resources.ApplyResources(Me.ChangeBaseDirectoryToolStripMenuItem, "ChangeBaseDirectoryToolStripMenuItem")
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        resources.ApplyResources(Me.ToolStripSeparator3, "ToolStripSeparator3")
        '
        'CheckForUpdatesToolStripMenuItem
        '
        Me.CheckForUpdatesToolStripMenuItem.Name = "CheckForUpdatesToolStripMenuItem"
        resources.ApplyResources(Me.CheckForUpdatesToolStripMenuItem, "CheckForUpdatesToolStripMenuItem")
        '
        'SelectAllToolStripMenuItem
        '
        Me.SelectAllToolStripMenuItem.Name = "SelectAllToolStripMenuItem"
        resources.ApplyResources(Me.SelectAllToolStripMenuItem, "SelectAllToolStripMenuItem")
        '
        'DeselectAllToolStripMenuItem
        '
        Me.DeselectAllToolStripMenuItem.Name = "DeselectAllToolStripMenuItem"
        resources.ApplyResources(Me.DeselectAllToolStripMenuItem, "DeselectAllToolStripMenuItem")
        '
        'ToolStripSeparator4
        '
        Me.ToolStripSeparator4.Name = "ToolStripSeparator4"
        resources.ApplyResources(Me.ToolStripSeparator4, "ToolStripSeparator4")
        '
        'GetLatestCommunityPackToolStripMenuItem
        '
        Me.GetLatestCommunityPackToolStripMenuItem.Name = "GetLatestCommunityPackToolStripMenuItem"
        resources.ApplyResources(Me.GetLatestCommunityPackToolStripMenuItem, "GetLatestCommunityPackToolStripMenuItem")
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        resources.ApplyResources(Me.ToolStripSeparator1, "ToolStripSeparator1")
        '
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        resources.ApplyResources(Me.ExitToolStripMenuItem, "ExitToolStripMenuItem")
        '
        'HelpToolStripMenuItem
        '
        Me.HelpToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ViewHelpToolStripMenuItem, Me.VisitWebsiteToolStripMenuItem, Me.ToolStripSeparator2, Me.AboutToolStripMenuItem})
        Me.HelpToolStripMenuItem.Name = "HelpToolStripMenuItem"
        resources.ApplyResources(Me.HelpToolStripMenuItem, "HelpToolStripMenuItem")
        '
        'ViewHelpToolStripMenuItem
        '
        Me.ViewHelpToolStripMenuItem.Name = "ViewHelpToolStripMenuItem"
        resources.ApplyResources(Me.ViewHelpToolStripMenuItem, "ViewHelpToolStripMenuItem")
        '
        'VisitWebsiteToolStripMenuItem
        '
        Me.VisitWebsiteToolStripMenuItem.Name = "VisitWebsiteToolStripMenuItem"
        resources.ApplyResources(Me.VisitWebsiteToolStripMenuItem, "VisitWebsiteToolStripMenuItem")
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        resources.ApplyResources(Me.ToolStripSeparator2, "ToolStripSeparator2")
        '
        'AboutToolStripMenuItem
        '
        Me.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem"
        resources.ApplyResources(Me.AboutToolStripMenuItem, "AboutToolStripMenuItem")
        '
        'VersionToolStripMenuItem
        '
        Me.VersionToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CheckForNewVersionToolStripMenuItem, Me.ViewChangelogToolStripMenuItem})
        Me.VersionToolStripMenuItem.Name = "VersionToolStripMenuItem"
        resources.ApplyResources(Me.VersionToolStripMenuItem, "VersionToolStripMenuItem")
        '
        'CheckForNewVersionToolStripMenuItem
        '
        Me.CheckForNewVersionToolStripMenuItem.Name = "CheckForNewVersionToolStripMenuItem"
        resources.ApplyResources(Me.CheckForNewVersionToolStripMenuItem, "CheckForNewVersionToolStripMenuItem")
        '
        'ViewChangelogToolStripMenuItem
        '
        Me.ViewChangelogToolStripMenuItem.Name = "ViewChangelogToolStripMenuItem"
        resources.ApplyResources(Me.ViewChangelogToolStripMenuItem, "ViewChangelogToolStripMenuItem")
        '
        'OptionsToolStripMenuItem
        '
        Me.OptionsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.LanguageToolStripMenuItem, Me.OpenGameDirectoryToolStripMenuItem})
        Me.OptionsToolStripMenuItem.Name = "OptionsToolStripMenuItem"
        resources.ApplyResources(Me.OptionsToolStripMenuItem, "OptionsToolStripMenuItem")
        '
        'LanguageToolStripMenuItem
        '
        Me.LanguageToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.InternationalEnglishDefaultToolStripMenuItem, Me.ToolStripSeparator5})
        Me.LanguageToolStripMenuItem.Name = "LanguageToolStripMenuItem"
        resources.ApplyResources(Me.LanguageToolStripMenuItem, "LanguageToolStripMenuItem")
        '
        'InternationalEnglishDefaultToolStripMenuItem
        '
        Me.InternationalEnglishDefaultToolStripMenuItem.Name = "InternationalEnglishDefaultToolStripMenuItem"
        resources.ApplyResources(Me.InternationalEnglishDefaultToolStripMenuItem, "InternationalEnglishDefaultToolStripMenuItem")
        '
        'ToolStripSeparator5
        '
        Me.ToolStripSeparator5.Name = "ToolStripSeparator5"
        resources.ApplyResources(Me.ToolStripSeparator5, "ToolStripSeparator5")
        '
        'OpenGameDirectoryToolStripMenuItem
        '
        Me.OpenGameDirectoryToolStripMenuItem.Name = "OpenGameDirectoryToolStripMenuItem"
        resources.ApplyResources(Me.OpenGameDirectoryToolStripMenuItem, "OpenGameDirectoryToolStripMenuItem")
        '
        'AdvancedToolStripMenuItem
        '
        Me.AdvancedToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AdvancedModeToolStripMenuItem, Me.ShowLatestUpdateIDToolStripMenuItem})
        Me.AdvancedToolStripMenuItem.Name = "AdvancedToolStripMenuItem"
        resources.ApplyResources(Me.AdvancedToolStripMenuItem, "AdvancedToolStripMenuItem")
        '
        'AdvancedModeToolStripMenuItem
        '
        Me.AdvancedModeToolStripMenuItem.Name = "AdvancedModeToolStripMenuItem"
        resources.ApplyResources(Me.AdvancedModeToolStripMenuItem, "AdvancedModeToolStripMenuItem")
        '
        'ShowLatestUpdateIDToolStripMenuItem
        '
        Me.ShowLatestUpdateIDToolStripMenuItem.Name = "ShowLatestUpdateIDToolStripMenuItem"
        resources.ApplyResources(Me.ShowLatestUpdateIDToolStripMenuItem, "ShowLatestUpdateIDToolStripMenuItem")
        '
        'cleanupCheckBox
        '
        resources.ApplyResources(Me.cleanupCheckBox, "cleanupCheckBox")
        Me.cleanupCheckBox.Name = "cleanupCheckBox"
        Me.cleanupCheckBox.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.lastUpdateLabel)
        resources.ApplyResources(Me.GroupBox1, "GroupBox1")
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.TabStop = False
        '
        'lastUpdateLabel
        '
        resources.ApplyResources(Me.lastUpdateLabel, "lastUpdateLabel")
        Me.lastUpdateLabel.Name = "lastUpdateLabel"
        '
        'revertCheckBox
        '
        resources.ApplyResources(Me.revertCheckBox, "revertCheckBox")
        Me.revertCheckBox.Name = "revertCheckBox"
        Me.revertCheckBox.UseVisualStyleBackColor = True
        '
        'outputTextbox
        '
        Me.outputTextbox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.outputTextbox, "outputTextbox")
        Me.outputTextbox.Name = "outputTextbox"
        '
        'updateFilesTreeView
        '
        resources.ApplyResources(Me.updateFilesTreeView, "updateFilesTreeView")
        Me.updateFilesTreeView.Name = "updateFilesTreeView"
        '
        'UpdaterMainForm
        '
        Me.AcceptButton = Me.updateButton
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.exitButton
        Me.Controls.Add(Me.outputTextbox)
        Me.Controls.Add(Me.revertCheckBox)
        Me.Controls.Add(Me.updateFilesTreeView)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.cleanupCheckBox)
        Me.Controls.Add(Me.nerfNetPlugLabel)
        Me.Controls.Add(Me.deselectAllButton)
        Me.Controls.Add(Me.selectAllButton)
        Me.Controls.Add(Me.customCheckBox)
        Me.Controls.Add(Me.updateProgressBar)
        Me.Controls.Add(Me.updatePathLabel)
        Me.Controls.Add(Me.changeFilepathButton)
        Me.Controls.Add(Me.exitButton)
        Me.Controls.Add(Me.updateButton)
        Me.Controls.Add(Me.MenuStrip1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MainMenuStrip = Me.MenuStrip1
        Me.MaximizeBox = False
        Me.Name = "UpdaterMainForm"
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents updateButton As Button
  Friend WithEvents exitButton As Button
  Friend WithEvents changeFilepathButton As Button
  Friend WithEvents updatePathLabel As Label
  Friend WithEvents updateProgressBar As ProgressBar
  Friend WithEvents customCheckBox As CheckBox
  Friend WithEvents selectAllButton As Button
  Friend WithEvents deselectAllButton As Button
  Friend WithEvents nerfNetPlugLabel As LinkLabel
  Friend WithEvents MenuStrip1 As MenuStrip
  Friend WithEvents FileToolStripMenuItem As ToolStripMenuItem
  Friend WithEvents ExitToolStripMenuItem As ToolStripMenuItem
  Friend WithEvents HelpToolStripMenuItem As ToolStripMenuItem
  Friend WithEvents AboutToolStripMenuItem As ToolStripMenuItem
  Friend WithEvents ChangeBaseDirectoryToolStripMenuItem As ToolStripMenuItem
  Friend WithEvents VisitWebsiteToolStripMenuItem As ToolStripMenuItem
  Friend WithEvents CheckForUpdatesToolStripMenuItem As ToolStripMenuItem
  Friend WithEvents SelectAllToolStripMenuItem As ToolStripMenuItem
  Friend WithEvents DeselectAllToolStripMenuItem As ToolStripMenuItem
  Friend WithEvents ViewHelpToolStripMenuItem As ToolStripMenuItem
  Friend WithEvents cleanupCheckBox As CheckBox
  Friend WithEvents ToolStripSeparator3 As ToolStripSeparator
  Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
  Friend WithEvents ToolStripSeparator2 As ToolStripSeparator
  Friend WithEvents GroupBox1 As GroupBox
  Friend WithEvents ToolStripSeparator4 As ToolStripSeparator
  Friend WithEvents GetLatestCommunityPackToolStripMenuItem As ToolStripMenuItem
  Friend WithEvents revertCheckBox As CheckBox
  Friend WithEvents VersionToolStripMenuItem As ToolStripMenuItem
  Friend WithEvents CheckForNewVersionToolStripMenuItem As ToolStripMenuItem
  Friend WithEvents ViewChangelogToolStripMenuItem As ToolStripMenuItem
  Friend WithEvents updateFilesTreeView As MyTreeView
  Friend WithEvents lastUpdateLabel As Label
  Friend WithEvents outputTextbox As Label
  Friend WithEvents AdvancedToolStripMenuItem As ToolStripMenuItem
  Friend WithEvents AdvancedModeToolStripMenuItem As ToolStripMenuItem
  Friend WithEvents ShowLatestUpdateIDToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents OptionsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents LanguageToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents OpenGameDirectoryToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents InternationalEnglishDefaultToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator5 As ToolStripSeparator
End Class
