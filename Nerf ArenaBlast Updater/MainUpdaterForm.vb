Option Strict On
Imports System.IO
Imports System.Configuration
Imports System.Net.Http
Imports System.Text
Imports System.Net
Imports System.Reflection
Imports Microsoft.VisualBasic.FileIO
Imports System.Deployment.Application
Imports System.Drawing.Drawing2D
Imports System.ComponentModel.Design
Imports System.Web.UI

Public Class UpdaterMainForm
    ' Replace folder dialogue with file dialogue?
    ' Add tristate checkboxes.
    Private Const BaseSeperator As String = "Base Game Files:"
    Private Const CPSeperator As String = "Community Pack <ver> Files:"
    Private Const MapsSeperator As String = "Maps:"
    Public homeDirectory As DirectoryInfo = New DirectoryInfo("C:\Program Files\Atari\Nerf\")
    Private iniDirectory As DirectoryInfo
    Private logDirectory As String = (Directory.GetCurrentDirectory() + "\NerfUpdater.Log")
    Private onlineOldBaseDirectory As String = "https://www.update.nerfarena.net/original/basegame/"
    Private onlineOldCustomDirectory As String = "https://www.update.nerfarena.net/original/community/"
    Private onlineBaseDirectory As String = "https://www.update.nerfarena.net/basegame/"
    Private onlineCustomDirectory As String = "https://www.update.nerfarena.net/community/"
    Private selectedBaseDirectory As String = "https://www.update.nerfarena.net/basegame/"
    Private selectedCustomDirectory As String = "https://www.update.nerfarena.net/community/"
    Private updaterDirectory As String = "https://www.update.nerfarena.net/nerfupdate/"
    Private communityDirectory As String = "https://www.nerfarena.net/"
    Private communityPackDirectory As String = "https://www.nerfarena.net/index.php/downloads/download/8-community-packs-and-utilities/4-nab-community-pack-lite"
    Private communityPack300Directory As String = "https://www.nerfarena.net/"
    Private thisDate As Date
    Private thisTime As Date
    Private updateSuccess As Boolean = True
    Private baseFiles As FileInfo()
    Private customFiles As FileInfo()
    Private querying As Boolean = False
    Private nodesToDelete As New List(Of TreeNode)
    Private filesToDelete As New List(Of String)
    Private updaterVersion As String = "3.81"
    Private updateDiff As Integer = 0
    Private newVersion As Boolean = False
    Private updateCount As Integer = 0
    Private InitialAppend As Integer = 0
    Public Changelog As ChangelogForm
    Private AdvancedMode As Boolean = False
    Private Aborting As Boolean = False
    Private Updating As Boolean = False
    Private BootAdvanced As Integer = 0
    Private engineVersionNumber As Integer = -1
    Private cpVersionString As String = String.Empty
    Private hasCP As Boolean = False
    Private silentClose As Boolean = False

    Dim resFilestream As Stream
    Dim Exeassembly As Assembly = Assembly.GetExecutingAssembly
    Dim Mynamespace As String = Exeassembly.GetName().Name.ToString()

    Private Declare Auto Function GetPrivateProfileString Lib "kernel32" (ByVal lpAppName As String,
              ByVal lpKeyName As String,
              ByVal lpDefault As String,
              ByVal lpReturnedString As StringBuilder,
              ByVal nSize As Integer,
              ByVal lpFileName As String) As Integer

    Private Declare Auto Function WritePrivateProfileString Lib "kernel32" (ByVal lpApplicationName As String,
                 ByVal lpKeyName As String,
                 ByVal lpString As String,
                 ByVal lpFileName As String) As Integer

    Public Function CheckForProcess(pName As String) As Integer
        Dim psList() As Process
        Dim pCount As Integer = 0
        Try
            psList = Process.GetProcesses()

            For Each p As Process In psList
                If (pName = p.ProcessName) Then
                    pCount += 1
                End If
            Next p

        Catch ex As Exception

        End Try

        Return pCount
    End Function

    Public Function CheckNABRunning(isCritical As Boolean) As Boolean
        If (CheckForProcess("Nerf") > 0) Then
            If (isCritical) Then
                If (MessageBox.Show("We have detected that Nerf ArenaBlast is currently running. It is highly recommended that you close Nerf ArenaBlast before updating to ensure updates are applied correctly. Click OK to ignore this warning or click Cancel to abort updating.", "Nerf ArenaBlast Running", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) = DialogResult.Cancel) Then
                    Return True
                End If
            Else
                MessageBox.Show("We have detected that Nerf ArenaBlast is currently running. It is highly recommended that you close Nerf ArenaBlast before updating to ensure updates are applied correctly.", "Nerf ArenaBlast Running", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            End If
        End If
        Return False
    End Function

    Public Function CheckUpdaterRunning() As Boolean
        If (CheckForProcess("Nerf ArenaBlast Updater") > 2) Then
            silentClose = True
            Return True
        ElseIf (CheckForProcess("Nerf ArenaBlast Updater") > 1) Then
            MessageBox.Show("In order to ensure update success, please only run one instance of the Nerf ArenaBlast Updater at a time. This program will now close.", "Updater Already Running", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            silentClose = True
            Return True
        End If
        silentClose = False
        Return False
    End Function

    Public Sub writeINI(sINIFile As String, sSection As String, sKey As String, sValue As String)
        Dim n As Integer
        Dim sTemp As String
        sTemp = sValue
        'Replace any CR/LF characters with spaces
        For n = 1 To Len(sValue)
            If Mid$(sValue, n, 1) = vbCr Or Mid$(sValue, n, 1) = vbLf _
              Then Mid$(sValue, n) = " "
        Next n
        n = WritePrivateProfileString(sSection, sKey, sTemp, sINIFile)
    End Sub

    Public Function readIni(sINIFile As String, sSection As String, sKey As String, sReturn As StringBuilder, fallbackValue As String) As String
        GetPrivateProfileString(sSection, sKey, "", sReturn, sReturn.Capacity, sINIFile)
        Return sReturn.ToString
    End Function

    Private Sub treeView_DrawNode(sender As Object, e As DrawTreeNodeEventArgs) Handles updateFilesTreeView.DrawNode
        e.DrawDefault = True
    End Sub

    Private Sub treeView_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles updateFilesTreeView.AfterSelect
        updateFilesTreeView.SelectedNode = Nothing
    End Sub

    Private Sub exitButton_Click(sender As Object, e As EventArgs) Handles exitButton.Click
        PromptForExit(sender, e)
    End Sub

    Private Sub PromptForExit(sender As Object, e As EventArgs)

        If (Not updateSuccess) Then
            If (MessageBox.Show("Are you sure you want to exit? The game will not be updated.", "Exit Updater?", MessageBoxButtons.YesNo, MessageBoxIcon.None) = DialogResult.Yes) Then
                Log("Closing without updating", True)
                Close()
                Exit Sub
            End If
        Else
            If (updateDiff > 0) Then
                If (MessageBox.Show("Are you sure you want to exit? The game has not been fully updated. This may result in game instability and inability to play in multiplayer matches.", "Exit Updater?", MessageBoxButtons.YesNo, MessageBoxIcon.None) = DialogResult.Yes) Then
                    Log("Closing without fully updating", True)
                    Close()
                    Exit Sub
                End If
            Else
                Close()
                Exit Sub
            End If
        End If
    End Sub

    Private Sub UpdaterMainForm_Load(sender As Object, e As EventArgs) Handles Me.Load
        Text = "Nerf ArenaBlast Updater " + updaterVersion

        If (CheckUpdaterRunning()) Then
            Close()
            Exit Sub
        End If

        ServicePointManager.Expect100Continue = True
        ServicePointManager.DefaultConnectionLimit = 9999
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
        thisDate = Today
        thisTime = TimeOfDay

        Try
            InitialAppend = CInt(ConfigurationManager.AppSettings("InitialAppend"))
        Catch err As ConfigurationErrorsException
            Log("Error: Could not read application configuration settings for loading", True)
        End Try

        Log("Log opened " + thisDate.ToShortDateString + " at " + thisTime.ToShortTimeString, CBool(InitialAppend))
        LoadConfigSettings()
        CheckIniLocation(sender, e)
        updateFilesTreeView.DrawMode = TreeViewDrawMode.OwnerDrawAll
        CheckNABRunning(False)
    End Sub

    Private Sub UpdaterMainForm_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        If (newVersion) Then
            Process.Start(Path.Combine(homeDirectory.FullName, "Nerf ArenaBlast Updater\update.bat"))
        End If
    End Sub

    Private Sub UpdaterMainForm_Loaded(sender As Object, e As EventArgs) Handles Me.Shown
        ShowLatestUpdateIDToolStripMenuItem.Enabled = False
        SelectAllToolStripMenuItem.Visible = False
        DeselectAllToolStripMenuItem.Visible = False
        Log("Nerf ArenaBlast Updater version " + updaterVersion + " booted successfully!", True)
        If Not (updaterDirectory = "") Then
            CheckUpdaterVersion(False)
        End If

        If (BootAdvanced > 0) Then
            Log("Booting in advanced mode", True)
            ToggleAdvanced(sender, e, True)
        End If
    End Sub

    Private Sub MainFormClosing(sender As Object, e As EventArgs) Handles Me.Closing
        If (Changelog IsNot Nothing) Then
            Changelog.Close()
        End If

        If (Not silentClose) Then
            Log("Updater shutting down", True)
            Log("Log closing " + thisDate.ToShortDateString + " at " + thisTime.ToShortTimeString, True)
        End If
    End Sub

    Private Async Sub CheckUpdaterVersion(ShowUpToDateMessage As Boolean)
        Dim latestVersion As String = updaterVersion
        Dim downPath As String = Path.Combine(updaterDirectory, "version.txt")
        Dim filePath As String = Path.Combine(homeDirectory.FullName, "Nerf ArenaBlast Updater\version.txt")
        Dim updaterPath As String = Path.Combine(homeDirectory.FullName, "Nerf ArenaBlast Updater\")
        Dim myWebClient As New WebClient()

        If Not (My.Computer.FileSystem.DirectoryExists(updaterPath)) Then
            My.Computer.FileSystem.CreateDirectory(updaterPath)
        End If

        Try
            myWebClient.DownloadFile(downPath, filePath)
        Catch ex As Exception
            Log("Warning: Could not reach the update server while checking for new versions", True)
            If (ex.InnerException IsNot Nothing) Then
                MessageBox.Show("The update server did not respond at while checking for new versions. The URL may be wrong, the host may be down, or you may need to check your internet connection." & ControlChars.NewLine & ControlChars.NewLine & "An error report follows:" & ControlChars.NewLine & ControlChars.NewLine & ex.InnerException.Message, "Server did not Respond", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Else
                MessageBox.Show("The update server did not respond at while checking for new versions. The URL may be wrong, the host may be down, or you may need to check your internet connection." & ControlChars.NewLine & ControlChars.NewLine & "An error report follows:" & ControlChars.NewLine & ControlChars.NewLine & ex.Message, "Server did not Respond", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            End If
            myWebClient.Dispose()
            Exit Sub
        End Try

        Dim SR As StreamReader = New StreamReader(filePath)
        Do While SR.Peek() >= 0
            latestVersion = SR.ReadLine()
        Loop
        SR.Close()

        My.Computer.FileSystem.DeleteFile(filePath)

        If (Convert.ToDouble(latestVersion) > Convert.ToDouble(updaterVersion)) Then
            Log("Found a new version (" + latestVersion + "), your version (" + updaterVersion + ")", True)
            If (MessageBox.Show("There is a newer version of the updater available. It is highly recommended that you use the newest version of the updater. Would you like to download it now?", "New Version", MessageBoxButtons.YesNo, MessageBoxIcon.None) = DialogResult.Yes) Then
                changeFilepathButton.Enabled = False
                updateFilesTreeView.Enabled = False
                cleanupCheckBox.Enabled = False
                customCheckBox.Enabled = False
                revertCheckBox.Enabled = False
                selectAllButton.Enabled = False
                SelectAllToolStripMenuItem.Enabled = False
                deselectAllButton.Enabled = False
                DeselectAllToolStripMenuItem.Enabled = False
                updateFilesTreeView.Nodes.Clear()
                outputTextbox.Text = "Downloading new version..."
                Log("Downloading new version...", True)
                UpdateSettings("InitialAppend", "1")
                updateButton.Enabled = False

                Dim fileList = Await GetRemoteFileInfos(updaterDirectory)

                If (fileList.Count > 0) Then
                    For Each entry In fileList
                        If Not (entry.FileName Like "version.txt") Then
                            If (entry.FileName Like "Nerf ArenaBlast Updater.exe") Then
                                myWebClient.DownloadFile(Path.Combine(updaterDirectory, entry.FileName), Path.Combine(updaterPath, entry.FileName + ".new"))
                            Else
                                myWebClient.DownloadFile(Path.Combine(updaterDirectory, entry.FileName), Path.Combine(updaterPath, entry.FileName))
                            End If
                        End If
                    Next
                    If (My.Computer.FileSystem.FileExists(Path.Combine(homeDirectory.FullName, "Nerf ArenaBlast Updater\Nerf ArenaBlast Updater.exe.new"))) Then
                        newVersion = True
                    End If
                End If
                myWebClient.Dispose()
                Close()
                Exit Sub
            End If
            myWebClient.Dispose()
        Else
            Log("No newer version available", True)
            UpdateSettings("InitialAppend", "0")
            If (ShowUpToDateMessage) Then
                MessageBox.Show("No newer version available.", "Version Check", MessageBoxButtons.OK, MessageBoxIcon.None)
                outputTextbox.Text = "."
            End If
        End If
    End Sub

    Private Sub LoadConfigSettings()

        If Not (My.Computer.FileSystem.FileExists(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile)) Then
            iniDirectory = New DirectoryInfo(Path.Combine(homeDirectory.FullName, "System\Nerf.ini"))
            UpdateSettings("GamePath", homeDirectory.FullName)
            UpdateSettings("BaseQueryURL", selectedBaseDirectory)
            UpdateSettings("CommunityQueryURL", selectedCustomDirectory)
            UpdateSettings("UpdateQueryURL", updaterDirectory)
            UpdateSettings("LastDate", "Never")
            UpdateSettings("LastTime", String.Empty)
            UpdateSettings("InitialAppend", "0")
            UpdateSettings("BootAdvanced", "0")
        Else
            Try
                homeDirectory = New DirectoryInfo(ConfigurationManager.AppSettings("GamePath"))
                iniDirectory = New DirectoryInfo(Path.Combine(homeDirectory.FullName, "System\Nerf.ini"))
                selectedBaseDirectory = ConfigurationManager.AppSettings("BaseQueryURL")
                selectedCustomDirectory = ConfigurationManager.AppSettings("CommunityQueryURL")
                updaterDirectory = ConfigurationManager.AppSettings("UpdateQueryURL")
                lastUpdateLabel.Text = ConfigurationManager.AppSettings("LastDate") & ControlChars.NewLine & ConfigurationManager.AppSettings("LastTime")
                InitialAppend = CInt(ConfigurationManager.AppSettings("InitialAppend"))
                BootAdvanced = CInt(ConfigurationManager.AppSettings("BootAdvanced"))
            Catch e As ConfigurationErrorsException
                Log("Error: Could not read application configuration settings for loading", True)
                MessageBox.Show("Could not read application configuration settings. Please make sure you have permission to read and write to the game directory. You may wish to run the updater as an administrator.", "Unable to Read Application Configuration File", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Close()
                Exit Sub
            End Try
        End If
    End Sub

    Private Sub CheckIniLocation(sender As Object, e As EventArgs)
        While (Not (File.Exists(iniDirectory.FullName)) And Not (Aborting))
            Log("Warning: Could not locate Nerf.ini in " + iniDirectory.FullName, True)
            MessageBox.Show("The Nerf.ini file could not be located in the provided directory " & iniDirectory.FullName & ". If you have not done so, run the game once and then locate the Nerf root folder before updating.", "Nerf.ini Not Found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            ShowGamePathSelector(sender, e)
        End While
        If (Not Aborting) Then
            UpdateSettings("GamePath", homeDirectory.FullName)
            updatePathLabel.Text = "Updating files at: " & homeDirectory.FullName
            CheckEngineVersion()
            CheckForCP(True)
            updateFilesTreeView.Nodes.Clear()
            updateButton.Text = "Check for &Updates"
            outputTextbox.Text = "Updater is ready to go."
            querying = False
            DoUpdate(querying)
        End If
    End Sub

    Private Sub CheckEngineVersion()
        Dim engineVersion As StringBuilder = New StringBuilder(64)

        Log("Detecting engine version...", True)
        readIni(iniDirectory.ToString, "Engine.Engine", "GameEngine", engineVersion, "ERROR")

        If (engineVersion.ToString = "Engine.GameEngine") Then
            Log("Standard engine version detected", True)
            engineVersionNumber = 1
            selectedBaseDirectory = onlineOldBaseDirectory
            selectedCustomDirectory = onlineOldCustomDirectory
            UpdateSettings("BaseQueryURL", onlineOldBaseDirectory)
            UpdateSettings("CommunityQueryURL", onlineOldCustomDirectory)
            GetLatestCommunityPackToolStripMenuItem.Enabled = True
        ElseIf (engineVersion.ToString = "EngineI.GameEngineI") Then
            Log("Improved engine version detected", True)
            engineVersionNumber = 2
            selectedBaseDirectory = onlineBaseDirectory
            selectedCustomDirectory = onlineCustomDirectory
            UpdateSettings("BaseQueryURL", onlineBaseDirectory)
            UpdateSettings("CommunityQueryURL", onlineCustomDirectory)
            GetLatestCommunityPackToolStripMenuItem.Enabled = True
        ElseIf (engineVersion.ToString = "ERROR") Then
            Log("Could not detect engine version", True)
            engineVersionNumber = -1
            selectedBaseDirectory = onlineBaseDirectory
            selectedCustomDirectory = onlineCustomDirectory
            UpdateSettings("BaseQueryURL", onlineBaseDirectory)
            UpdateSettings("CommunityQueryURL", onlineCustomDirectory)
            GetLatestCommunityPackToolStripMenuItem.Enabled = False
        Else
            Log("Unknown engine version " + engineVersion.ToString + " detected", True)
            engineVersionNumber = 0
            selectedBaseDirectory = onlineBaseDirectory
            selectedCustomDirectory = onlineCustomDirectory
            UpdateSettings("BaseQueryURL", onlineBaseDirectory)
            UpdateSettings("CommunityQueryURL", onlineCustomDirectory)
            GetLatestCommunityPackToolStripMenuItem.Enabled = False
        End If
    End Sub

    Private Sub CheckForCP(ChangeStates As Boolean)
        Log("Detecting Community Pack...", True)

        If (File.Exists(Path.Combine(homeDirectory.FullName, "System\CommunityPack.ini"))) Then
            customCheckBox.Enabled = True
            If (ChangeStates) Then
                customCheckBox.Checked = True
            End If
            Dim cpVersion As String
            Dim sb As StringBuilder = New StringBuilder(64)

            cpVersion = readIni(Path.Combine(homeDirectory.FullName, "System\CommunityPack.ini"), "Community Pack", "Version", sb, "ERROR")

            If (cpVersion = "ERROR") Then
                Log("Unknown Community Pack detected", True)
                hasCP = False
                cpVersionString = ""
            Else
                Log("Community Pack version " + cpVersion + " detected", True)
                hasCP = True
                cpVersionString = cpVersion
            End If

        Else
            customCheckBox.Enabled = False
            Log("Community Pack not detected", True)
            hasCP = False
            cpVersionString = ""

            If (ChangeStates) Then
                customCheckBox.Checked = False
            End If
        End If
    End Sub

    Private Sub UpdateSettings(key As String, value As String)
        Try
            Dim configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None)
            Dim settings = configFile.AppSettings.Settings
            If IsNothing(settings(key)) Then
                settings.Add(key, value)
            Else
                settings(key).Value = value
            End If
            configFile.Save(ConfigurationSaveMode.Modified)
            ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name)
        Catch e As ConfigurationErrorsException
            Log("Error: Could not read application configuration settings for saving", True)
            MessageBox.Show("Could not read application configuration settings.", "Application Configuration File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End Try
    End Sub

    Private Function ShowGamePathSelector(sender As Object, e As EventArgs) As Boolean
        Dim selGamePathDialogue As FolderBrowserDialog = New FolderBrowserDialog()
        selGamePathDialogue.Description = "Select Nerf ArenaBlast Installation Location"
        selGamePathDialogue.RootFolder = Environment.SpecialFolder.MyComputer

        If (iniDirectory.FullName IsNot Nothing) Then
            selGamePathDialogue.SelectedPath = iniDirectory.FullName
        ElseIf (homeDirectory.FullName IsNot Nothing) Then
            selGamePathDialogue.SelectedPath = homeDirectory.FullName
        End If

        If (selGamePathDialogue.ShowDialog() = DialogResult.OK) Then

            If (selGamePathDialogue.SelectedPath Like "*\System") Then
                selGamePathDialogue.SelectedPath = selGamePathDialogue.SelectedPath.Substring(0, selGamePathDialogue.SelectedPath.Length - 7)
            End If

            homeDirectory = New DirectoryInfo(selGamePathDialogue.SelectedPath)
            iniDirectory = New DirectoryInfo(Path.Combine(homeDirectory.FullName, "System\Nerf.ini"))
            Return True
        Else
            If (Not (sender Is changeFilepathButton) And Not (sender Is ChangeBaseDirectoryToolStripMenuItem)) Then
                Aborting = True
                PromptForExit(sender, e)
            Else
                LoadConfigSettings()
            End If
            Return False
        End If
    End Function

    Private Sub changeFilepathButton_Click(sender As Object, e As EventArgs) Handles changeFilepathButton.Click
        Dim ChangedPath As Boolean = False

        changeFilepathButton.Focus()
        ChangedPath = ShowGamePathSelector(sender, e)

        If (ChangedPath) Then
            Log("Update path changed to " + homeDirectory.ToString, True)
            CheckIniLocation(sender, e)
        End If
    End Sub

    Private Sub DoUpdate(isQuerying As Boolean)
        updateFilesTreeView.Enabled = Not isQuerying
        changeFilepathButton.Enabled = Not isQuerying
        customCheckBox.Enabled = False
        cleanupCheckBox.Enabled = Not isQuerying
        revertCheckBox.Enabled = Not isQuerying

        If (updateFilesTreeView.Nodes.Count > 0) Then
            selectAllButton.Enabled = Not isQuerying
            SelectAllToolStripMenuItem.Enabled = Not isQuerying
            deselectAllButton.Enabled = Not isQuerying
            DeselectAllToolStripMenuItem.Enabled = Not isQuerying
        Else
            selectAllButton.Enabled = False
            SelectAllToolStripMenuItem.Enabled = False
            deselectAllButton.Enabled = False
            DeselectAllToolStripMenuItem.Enabled = False
        End If
        If (Not isQuerying) Then
            customCheckBox.Enabled = hasCP
            updateButton.Enabled = True
        Else
            updateFilesTreeView.Nodes.Clear()
            updateButton.Enabled = False
        End If
    End Sub

    Private Sub updateButton_Click(sender As Object, e As EventArgs) Handles updateButton.Click
        updateButton.Focus()

        If (updateButton.Text = "Check for &Updates") Then
            CheckNABRunning(False)
            updateSuccess = False
            updateButton.Text = "Checking..."
            outputTextbox.Text = "Checking for updates..."
            Log("Checking for updates...", True)
            updateCount = 0
            querying = True
            DoUpdate(querying)
            updateProgressBar.Maximum = 1
            updateProgressBar.Minimum = 0
            updateProgressBar.Value = 0
            updateProgressBar.Step = 1
            QueryGameFiles(sender, e)
        ElseIf ((updateButton.Text = "&Update Selected") Or (updateButton.Text = "&Update")) Then
            If (CheckNABRunning(True)) Then
                MessageBox.Show("Updating has been aborted.", "Update Aborted", MessageBoxButtons.OK, MessageBoxIcon.None)
            Else
                Updating = True
                updateButton.Text = "Updating..."
                If (AdvancedMode) Then
                    outputTextbox.Text = "Updating selected files..."
                    Log("Updating selected files...", True)
                Else
                    outputTextbox.Text = "Updating files..."
                    Log("Updating files...", True)
                End If
                updateButton.Enabled = False
                CopyFilesOver()
                updateSuccess = True
                'writeINI(iniDirectory.ToString, "Update", "UpdateID", "")
                'Log("Update ID is " + "", True)
                updateButton.Text = "Check for &Updates"
                updateButton.Enabled = True
                Log("Updates were successfull.", True)
                MessageBox.Show("Updates were successful.", "Update Complete", MessageBoxButtons.OK, MessageBoxIcon.None)
                outputTextbox.Text = "Updates successful."
                updateProgressBar.Value = 0
                selectAllButton.Enabled = False
                SelectAllToolStripMenuItem.Enabled = False
                deselectAllButton.Enabled = False
                DeselectAllToolStripMenuItem.Enabled = False
            End If
        End If
    End Sub

    Public Class RemoteFileInfo
        Public Property Url As String
        Public Property FileName As String
        Public Property FileSize As String
        Public Property LastModified As String
        Public Property Description As String

        Public Overrides Function ToString() As String
            Return $"{FileName}".PadRight(20, "."c) & $"Modified: {LastModified}"
        End Function
    End Class

    'Function to get list of items from URL
    Public Async Function GetRemoteFileInfos(remoteAddress As String) As Task(Of IEnumerable(Of RemoteFileInfo))
        Dim results As New List(Of RemoteFileInfo)
        Dim htmlText As String

        Using client As New HttpClient
            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml")
            client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0")
            htmlText = Await client.GetStringAsync(remoteAddress)

            Dim lines() As String = htmlText.Split(ControlChars.Lf)
            Dim index As Integer = -1
            Dim line As String = String.Empty

            If (lines.Length <= 0) Then
                Exit Function
            End If

            Do Until line.StartsWith("<a")
                index += 1
                If (index >= lines.Length) Then
                    Exit Do
                End If
                line = lines(index).Trim
            Loop

            Do While index < lines.Length AndAlso index >= 0 AndAlso line.StartsWith("<a")
                line = lines(index).Trim
                Dim sepIndex As Integer = line.IndexOf("/a>")

                Dim test As Integer = line.Length
                Dim test2 As String = line.Substring(0, sepIndex + 3)

                If sepIndex > -1 Then
                    Dim currentInfo As New RemoteFileInfo

                    Dim a As XElement = XElement.Parse(line.Substring(0, sepIndex + 3))

                    currentInfo = New RemoteFileInfo
                    currentInfo.Url = Path.Combine(remoteAddress, a.@href)
                    currentInfo.FileName = a.Value

                    Dim parts() As String = line.Substring(sepIndex + 3).TrimStart.Split({"  "}, StringSplitOptions.RemoveEmptyEntries)

                    currentInfo.LastModified = parts(0).Trim

                    If parts.Length > 1 Then currentInfo.FileSize = parts(1).Trim

                    If parts.Length > 2 Then currentInfo.Description = parts(2).Trim

                    results.Add(currentInfo)

                End If

                index += 1
            Loop
        End Using

        Return results.ToArray
    End Function

    Private Async Function PopulateTreeView(ByVal dir As String, ByVal parentNode As TreeNode) As Task
        Dim infoReader As FileInfo
        Dim tempFilePath As String
        Dim tempDiff As Long

        Try
            Dim fileList = Await GetRemoteFileInfos(parentNode.Tag.ToString)

            If (fileList.Count > 0) Then
                For Each entry In fileList
                    If Not (entry.FileName Like "*/") Then
                        If (entry.FileName Like "*.delete") Then
                            If (cleanupCheckBox.Checked) Then
                                tempFilePath = Path.Combine(homeDirectory.FullName, dir.Replace(selectedBaseDirectory, "").Replace(selectedCustomDirectory, "").Replace("/", "\") + entry.FileName.Replace(".delete", ""))
                                If Not (tempFilePath Like "*.*") Then
                                    tempFilePath = tempFilePath + "\"
                                    If (My.Computer.FileSystem.DirectoryExists(tempFilePath)) Then
                                        updateProgressBar.Maximum += 1
                                        Dim folderNode As TreeNode = Nothing
                                        folderNode = parentNode.Nodes.Add(entry.FileName.Replace(".delete", ":"))
                                        folderNode.Tag = dir + entry.FileName.Replace(".delete", "\")
                                        folderNode.ForeColor = Color.Red
                                        updateProgressBar.PerformStep()
                                    End If
                                Else
                                    If (My.Computer.FileSystem.FileExists(tempFilePath)) Then
                                        updateProgressBar.Maximum += 1
                                        Dim fileNode As TreeNode = Nothing
                                        fileNode = parentNode.Nodes.Add(entry.FileName.Replace(".delete", ""))
                                        fileNode.Tag = dir + entry.FileName.Replace(".delete", "")
                                        fileNode.ForeColor = Color.Red
                                        updateProgressBar.PerformStep()
                                    End If
                                End If
                            End If
                        Else
                            tempFilePath = Path.Combine(homeDirectory.FullName, dir.Replace(selectedBaseDirectory, "").Replace(selectedCustomDirectory, "").Replace("/", "\") + entry.FileName)

                            If (My.Computer.FileSystem.FileExists(tempFilePath)) Then
                                infoReader = My.Computer.FileSystem.GetFileInfo(tempFilePath)



                                tempDiff = DateDiff("n", TimeZoneInfo.ConvertTime(infoReader.LastWriteTime, TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time")).ToString(), entry.LastModified)
                            End If

                            If (revertCheckBox.Checked) AndAlso (My.Computer.FileSystem.FileExists(tempFilePath)) AndAlso (tempDiff < 0) Then
                                updateProgressBar.Maximum += 1
                                Dim fileNode As TreeNode = Nothing
                                fileNode = parentNode.Nodes.Add(entry.FileName + " (" + entry.FileSize + "B)")
                                fileNode.Tag = dir + entry.FileName
                                fileNode.ForeColor = Color.DarkOrchid
                                updateProgressBar.PerformStep()
                            End If

                            If ((tempDiff > 0) Or (Not My.Computer.FileSystem.FileExists(tempFilePath))) Then
                                updateProgressBar.Maximum += 1
                                Dim fileNode As TreeNode = Nothing
                                fileNode = parentNode.Nodes.Add(entry.FileName + " (" + entry.FileSize + "B)")
                                fileNode.Tag = dir + entry.FileName
                                If File.Exists(tempFilePath) Then
                                    fileNode.ForeColor = Color.Blue
                                Else
                                    fileNode.ForeColor = Color.Orange
                                    updateProgressBar.PerformStep()
                                End If
                            End If
                        End If
                    End If
                Next
                For Each entry In fileList
                    If (entry.FileName Like "*/") Then
                        Dim folderNode As TreeNode = Nothing
                        Dim folderName As String = String.Empty
                        folderName = entry.FileName.Replace("/", "")
                        folderNode = parentNode.Nodes.Add(folderName + ":")
                        folderNode.Tag = dir + entry.FileName
                        Await PopulateTreeView(folderNode.Tag.ToString, folderNode)
                    End If
                Next
            End If

        Catch ex As HttpRequestException
            Log("Error: Could not reach the update server while checking for updates", True)
            If (ex.InnerException IsNot Nothing) Then
                MessageBox.Show("The update server did not respond at " & parentNode.Tag.ToString & ". The URL may be wrong, the host may be down, or you may need to check your internet connection." & ControlChars.NewLine & ControlChars.NewLine & "An error report follows:" & ControlChars.NewLine & ControlChars.NewLine & ex.InnerException.Message, "Server did not Respond", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Else
                MessageBox.Show("The update server did not respond at " & parentNode.Tag.ToString & ". The URL may be wrong, the host may be down, or you may need to check your internet connection." & ControlChars.NewLine & ControlChars.NewLine & "An error report follows:" & ControlChars.NewLine & ControlChars.NewLine & ex.Message, "Server did not Respond", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            End If
            'updateFilesTreeView.Nodes.Clear()
            outputTextbox.Text = "Server error."
            'querying = False
            'updateFilesTreeView.Enabled = True
            'selectAllButton.Enabled = False
            'SelectAllToolStripMenuItem.Enabled = False
            'deselectAllButton.Enabled = False
            'DeselectAllToolStripMenuItem.Enabled = False
            'updateButton.Text = "Check for &Updates"
            'DoUpdate(querying)
            updateCount = -1
            Exit Function
        End Try
    End Function

    Private Async Sub QueryGameFiles(sender As Object, e As EventArgs)
        Dim i As Integer = 0
        Dim root As TreeNode = updateFilesTreeView.Nodes.Add(BaseSeperator)
        Dim myNode As TreeNode
        Dim tempCount As Integer = 0

        nodesToDelete.Clear()
        root.Tag = selectedBaseDirectory
        updateFilesTreeView.BeginUpdate()
        Log("Checking for base game updates (" + ConfigurationManager.AppSettings("BaseQueryURL") + ")...", True)
        Await PopulateTreeView(selectedBaseDirectory, updateFilesTreeView.Nodes(i))

        If (updateCount < 0) Then
            RecurseCountFiles(root, 0)
        Else
            updateCount = RecurseCountFiles(root, 0)
        End If

        CleanUpTree()
        If (root.Nodes.Count <= 0) Then
            nodesToDelete.Add(root)
            i = 0
            CleanUpTree()
        Else
            root.ExpandAll()
            i = 1
        End If

        If (customCheckBox.Checked) Then
            root = updateFilesTreeView.Nodes.Add(CPSeperator.Replace("<ver>", cpVersionString))
            root.Tag = selectedCustomDirectory
            Log("Checking for Community Pack updates (" + ConfigurationManager.AppSettings("CommunityQueryURL") + ")...", True)
            Await PopulateTreeView(selectedCustomDirectory, updateFilesTreeView.Nodes(i))
            If (updateCount < 0) Then
                RecurseCountFiles(root, 0)
            Else
                updateCount = RecurseCountFiles(root, updateCount)
            End If
            CleanUpTree()
            If (root.Nodes.Count <= 0) Then
                nodesToDelete.Add(root)
                CleanUpTree()
            Else
                root.ExpandAll()
            End If
        End If

        updateFilesTreeView.EndUpdate()
        updateProgressBar.PerformStep()

        If (updateCount > 0) Then
            For Each myNode In updateFilesTreeView.Nodes
                myNode.Checked = True
                If myNode.Nodes.Count > 0 Then
                    CheckAllChildNodes(myNode, myNode.Checked, False)
                    tempCount = RecurseCountSelected(myNode, tempCount)
                End If
            Next
            querying = False
            If (tempCount > 0) Then
                If (AdvancedMode) Then
                    updateButton.Text = "&Update Selected"
                Else
                    updateButton.Text = "&Update"
                End If
            Else
                updateButton.Text = "Check for &Updates"
            End If
            updateSuccess = False
            selectAllButton.Enabled = True
            SelectAllToolStripMenuItem.Enabled = True
            deselectAllButton.Enabled = True
            DeselectAllToolStripMenuItem.Enabled = True
            DoUpdate(querying)

            If (updateCount = 1) Then
                Log("Found " + updateCount.ToString + " update", True)
                outputTextbox.Text = updateCount.ToString + " update available."
            Else
                Log("Found " + updateCount.ToString + " updates", True)
                outputTextbox.Text = updateCount.ToString + " updates available."
            End If

            If (AdvancedMode) Then
                If (tempCount = 1) Then
                    outputTextbox.Text += ControlChars.NewLine + ControlChars.NewLine + tempCount.ToString + " update selected."
                Else
                    outputTextbox.Text += ControlChars.NewLine + ControlChars.NewLine + tempCount.ToString + " updates selected."
                End If
            End If

            updateDiff = updateCount
            updateProgressBar.Value = 0
            UpdateLastTime()
        ElseIf (updateCount < 0) Then
            selectAllButton.Enabled = False
            SelectAllToolStripMenuItem.Enabled = False
            deselectAllButton.Enabled = False
            DeselectAllToolStripMenuItem.Enabled = False
            DoUpdate(querying)
            updateButton.Text = "Check for &Updates"
            updateSuccess = True
            Log("Error checking for updates", True)
            MessageBox.Show("Error checking for updates.", "Check Cancelled", MessageBoxButtons.OK, MessageBoxIcon.None)
            outputTextbox.Text = "Error checking for updates."
            updateDiff = 0
            updateProgressBar.Value = 0
            updateButton.Enabled = True
            changeFilepathButton.Enabled = True
        Else
            selectAllButton.Enabled = False
            SelectAllToolStripMenuItem.Enabled = False
            deselectAllButton.Enabled = False
            DeselectAllToolStripMenuItem.Enabled = False
            DoUpdate(querying)
            updateButton.Text = "Check for &Updates"
            updateSuccess = True
            Log("No newer updates availible", True)
            MessageBox.Show("No newer updates available.", "Check Complete", MessageBoxButtons.OK, MessageBoxIcon.None)
            outputTextbox.Text = "No newer updates available."
            UpdateLastTime()
            updateDiff = 0
            updateProgressBar.Value = 0
            updateButton.Enabled = True
            changeFilepathButton.Enabled = True
        End If

        cleanupCheckBox.Enabled = True
        revertCheckBox.Enabled = True
    End Sub

    Private Sub UpdateLastTime()
        lastUpdateLabel.Text = thisDate.ToShortDateString & ControlChars.NewLine & thisTime.ToShortTimeString
        UpdateSettings("LastDate", thisDate.ToShortDateString)
        UpdateSettings("LastTime", thisTime.ToShortTimeString)
    End Sub

    Private Function RecurseCountFiles(treeNode As TreeNode, fileCount As Integer) As Integer
        Dim node As TreeNode
        Dim cleanupNode As TreeNode
        Dim tempFileCount As Integer

        If (treeNode.Nodes.Count <= 0) Then
            If (Not nodesToDelete.Contains(treeNode)) Then
                nodesToDelete.Add(treeNode)
            End If
            Return fileCount
        Else
            tempFileCount = 0
            For Each node In treeNode.Nodes
                If (Not nodesToDelete.Contains(node)) Then
                    tempFileCount += 1
                End If
            Next
            If (tempFileCount <= 0) Then
                If (Not nodesToDelete.Contains(treeNode)) Then
                    nodesToDelete.Add(treeNode)
                End If
                Return fileCount
            End If
        End If

        For Each node In treeNode.Nodes
            If Not (node.Tag.ToString Like "*/") Then
                fileCount += 1
            Else
                If (node.Nodes.Count > 0) Then
                    fileCount = RecurseCountFiles(node, fileCount)
                Else
                    If (Not nodesToDelete.Contains(node)) Then
                        nodesToDelete.Add(node)
                    End If

                    If (node.Parent IsNot Nothing) Then
                        tempFileCount = 0
                        For Each cleanupNode In node.Parent.Nodes
                            If (Not nodesToDelete.Contains(cleanupNode)) AndAlso (cleanupNode.Nodes.Count > 0) Then
                                tempFileCount += cleanupNode.Nodes.Count
                            End If
                        Next
                        If (tempFileCount <= 0) Then

                            For Each cleanupNode In node.Parent.Nodes
                                If (Not nodesToDelete.Contains(cleanupNode)) Then
                                    nodesToDelete.Add(cleanupNode)
                                End If
                            Next

                            If (Not nodesToDelete.Contains(node.Parent)) Then
                                nodesToDelete.Add(node.Parent)
                            End If

                            If (node.Parent.Parent IsNot Nothing) Then
                                RecurseCountFiles(node.Parent.Parent, fileCount)
                            End If
                        End If
                    End If
                End If
            End If
        Next node

        Return fileCount
    End Function

    Private Function RecurseCountSelected(treeNode As TreeNode, selCount As Integer) As Integer
        Dim node As TreeNode

        For Each node In treeNode.Nodes
            If Not (node.Tag.ToString Like "*/") Then
                If (node.Checked) Then
                    selCount += 1
                End If
            Else
                If node.Nodes.Count > 0 Then
                    selCount = RecurseCountSelected(node, selCount)
                End If
            End If
        Next node

        Return selCount
    End Function

    Private Sub CleanUpTree()
        For Each node In nodesToDelete
            updateFilesTreeView.Nodes.Remove(node)
        Next
    End Sub

    Private Sub RecurseCopy(treeNode As TreeNode)
        Dim node As TreeNode
        Dim actualOnlinePath As String
        Dim actualPath As String
        Dim temp As String
        Dim myWebClient As New WebClient()

        For Each node In treeNode.Nodes

            If node.Checked Then

                actualOnlinePath = node.Tag.ToString
                actualPath = actualOnlinePath.Replace(selectedBaseDirectory, "")
                actualPath = actualPath.Replace(selectedCustomDirectory, "")
                actualPath = actualPath.Replace("/", "\")
                temp = actualPath
                actualPath = Path.Combine(homeDirectory.FullName, temp)
                Dim actualDirectoryInfo As DirectoryInfo = New DirectoryInfo(actualPath)

                If Not (actualOnlinePath Like "*/") Then
                    If (node.ForeColor = Color.Red) Then
                        If (node.Text Like "*.*") Then
                            If (My.Computer.FileSystem.FileExists(actualPath)) Then
                                outputTextbox.Text = "Deleting file:" & ControlChars.NewLine & ControlChars.NewLine & node.Text
                                My.Computer.FileSystem.DeleteFile(actualPath)
                            End If
                            node.ForeColor = Color.DarkRed
                        Else
                            outputTextbox.Text = "Deleting directory:" & ControlChars.NewLine & ControlChars.NewLine & node.Text
                            If (actualDirectoryInfo.EnumerateFiles().Any() <> False) Or (actualDirectoryInfo.EnumerateDirectories().Any() <> False) Then
                                If (MessageBox.Show(actualPath + " is a folder that contains other files. Are you sure you wish to delete it?", "Delete Folder?", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) = DialogResult.Yes) Then
                                    Log("Deleting directory and contents (" + actualPath + ")", True)
                                    My.Computer.FileSystem.DeleteDirectory(actualPath, DeleteDirectoryOption.DeleteAllContents)
                                    node.ForeColor = Color.DarkRed
                                End If
                            Else
                                Log("Deleting directory (" + actualPath + ")", True)
                                My.Computer.FileSystem.DeleteDirectory(actualPath, DeleteDirectoryOption.DeleteAllContents)
                                node.ForeColor = Color.DarkRed
                            End If
                        End If
                    Else
                        Try
                            If Not (My.Computer.FileSystem.DirectoryExists(actualPath.Substring(0, actualPath.LastIndexOf("\")))) Then
                                Log("Creating directory (" + actualPath + ")", True)
                                My.Computer.FileSystem.CreateDirectory(actualPath.Substring(0, actualPath.LastIndexOf("\")))
                            End If

                            If (node.ForeColor = Color.Blue) Then
                                Log("Downloading updated file " + actualOnlinePath, True)
                            ElseIf (node.ForeColor = Color.Orange) Then
                                Log("Downloading new file " + actualOnlinePath, True)
                            End If

                            outputTextbox.Text = "Downloading file:" & ControlChars.NewLine & ControlChars.NewLine & node.Text
                            myWebClient.DownloadFile(actualOnlinePath, actualPath)

                            If (node.Text Like ("Default.ini*")) Then
                                If (My.Computer.FileSystem.FileExists(actualPath.Replace("Default.ini", "Nerf.ini"))) Then
                                    Log("Deleting Nerf.ini", True)
                                    outputTextbox.Text = "Deleting file:" & ControlChars.NewLine & ControlChars.NewLine & "Nerf.ini"
                                    My.Computer.FileSystem.DeleteFile(actualPath.Replace("Default.ini", "Nerf.ini"))
                                End If
                                If (My.Computer.FileSystem.FileExists(actualPath)) Then
                                    Log("Refreshing Nerf.ini", True)
                                    outputTextbox.Text = "Refreshing:" & ControlChars.NewLine & ControlChars.NewLine & "Nerf.ini"
                                    My.Computer.FileSystem.CopyFile(actualPath, actualPath.Replace("Default.ini", "Nerf.ini"))
                                End If
                                'ElseIf (node.Text Like ("DefUser.ini*")) Then
                                '  Log("Deleting User.ini", True)
                                '  If (My.Computer.FileSystem.FileExists(actualPath.Replace("DefUser.ini", "User.ini"))) Then
                                '    outputTextbox.Text = "Deleting file:" & ControlChars.NewLine & ControlChars.NewLine & "User.ini"
                                '    My.Computer.FileSystem.DeleteFile(actualPath.Replace("DefUser.ini", "User.ini"))
                                '  End If
                                '  If (My.Computer.FileSystem.FileExists(actualPath)) Then
                                '    Log("Refreshing User.ini", True)
                                '    outputTextbox.Text = "Refreshing:" & ControlChars.NewLine & ControlChars.NewLine & "User.ini"
                                '    My.Computer.FileSystem.CopyFile(actualPath, actualPath.Replace("DefUser.ini", "User.ini"))
                                '  End If
                            End If

                            node.ForeColor = Color.Green
                            Log("Updated file: " + actualPath, True)
                        Catch e As Exception
                            Log("Error: Could not download file " + actualOnlinePath, True)
                            MessageBox.Show("Error downloading file. " + e.ToString, "Error Downloading File", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                            myWebClient.Dispose()
                            Updating = False
                        End Try
                    End If
                    myWebClient.Dispose()
                    updateProgressBar.PerformStep()
                End If
            End If

            If node.Nodes.Count > 0 Then
                RecurseCopy(node)
            End If
        Next node
        'Catch ex As UnauthorizedAccessException
        '  parentNode.Nodes.Add("Access Denied")
        'End Try
    End Sub

    Private Sub CopyFilesOver()
        Dim myNode As TreeNode

        updateProgressBar.Maximum = 0

        If (updateFilesTreeView.Nodes.Count > 0) Then
            For Each myNode In updateFilesTreeView.Nodes
                updateProgressBar.Maximum = RecurseCountSelected(myNode, updateProgressBar.Maximum)
                If (AdvancedMode) Then
                    outputTextbox.Text = updateProgressBar.Maximum & "updates selected"
                Else
                    outputTextbox.Text = updateProgressBar.Maximum & "updates pending"
                End If
                updateDiff -= updateProgressBar.Maximum
            Next
            updateProgressBar.Minimum = 0
            updateProgressBar.Value = 0
            updateProgressBar.Step = 1

            For Each myNode In updateFilesTreeView.Nodes
                If myNode.Nodes.Count > 0 Then
                    RecurseCopy(myNode)
                End If
            Next

            Updating = False
        End If
    End Sub

    Private Sub treeNodeCheckChanged(sender As Object, e As TreeViewEventArgs) Handles updateFilesTreeView.AfterCheck
        Dim myNode As TreeNode
        Dim tempCount As Integer = 0
        Dim oldMessage As String
        Dim lineSeperator As Integer = -1

        If (updateFilesTreeView.Nodes.Count > 0) Then
            For Each myNode In updateFilesTreeView.Nodes
                tempCount = RecurseCountSelected(myNode, tempCount)
            Next
        End If

        If (tempCount <= 0) Then
            updateButton.Text = "Check for &Updates"
        Else
            If (AdvancedMode) Then
                updateButton.Text = "&Update Selected"
            Else
                updateButton.Text = "&Update"
            End If
        End If

        lineSeperator = outputTextbox.Text.IndexOf(ControlChars.NewLine)
        If (lineSeperator > -1) Then
            oldMessage = outputTextbox.Text.Substring(0, lineSeperator)
        Else
            oldMessage = outputTextbox.Text
        End If
        If (AdvancedMode) Then
            outputTextbox.Text = oldMessage + ControlChars.NewLine + ControlChars.NewLine + tempCount.ToString + " updates selected."
        Else
            outputTextbox.Text = oldMessage
        End If
    End Sub

    Private Sub selectAllButton_Click(sender As Object, e As EventArgs) Handles selectAllButton.Click
        selectAllButton.Focus()
        Dim myNode As TreeNode
        If (updateFilesTreeView.Nodes.Count > 0) Then
            For Each myNode In updateFilesTreeView.Nodes
                myNode.Checked = True
                If myNode.Nodes.Count > 0 Then
                    CheckAllChildNodes(myNode, myNode.Checked, True)
                End If
            Next
        End If
    End Sub

    Private Sub deselectAllButton_Click(sender As Object, e As EventArgs) Handles deselectAllButton.Click
        deselectAllButton.Focus()
        Dim myNode As TreeNode
        If (updateFilesTreeView.Nodes.Count > 0) Then
            For Each myNode In updateFilesTreeView.Nodes
                myNode.Checked = False
                If myNode.Nodes.Count > 0 Then
                    CheckAllChildNodes(myNode, myNode.Checked, True)
                End If
            Next
        End If
    End Sub

    Private Sub AboutToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AboutToolStripMenuItem.Click
        MessageBox.Show("Nerf ArenaBlast Updater " & updaterVersion & ControlChars.NewLine & ControlChars.NewLine & "© Jared Petersen " & Date.Today.Year, "About Nerf ArenaBlast Updater", MessageBoxButtons.OK, MessageBoxIcon.None)
    End Sub

    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        PromptForExit(sender, e)
    End Sub

    Private Sub ChangeBaseDirectoryToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ChangeBaseDirectoryToolStripMenuItem.Click
        Dim ChangedPath As Boolean = False

        ChangedPath = ShowGamePathSelector(sender, e)

        If (ChangedPath) Then
            CheckIniLocation(sender, e)
        End If
    End Sub

    Private Sub VisitWebsiteToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles VisitWebsiteToolStripMenuItem.Click
        Process.Start(communityDirectory)
    End Sub

    Private Sub nerfNetPlugLabel_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles nerfNetPlugLabel.LinkClicked
        Process.Start(communityDirectory)
    End Sub

    Private Sub updateFilesTreeView_AfterCheck(sender As Object, e As TreeViewEventArgs) Handles updateFilesTreeView.AfterCheck
        If e.Action <> TreeViewAction.Unknown Then
            If e.Node.Nodes.Count > 0 Then
                CheckAllChildNodes(e.Node, e.Node.Checked, True)
            End If
            ToggleParentChecks(e.Node, e.Node.Checked)
        End If
    End Sub

    Private Sub updateFilesTreeView_AfterCheck(sender As Object, e As TreeNodeMouseClickEventArgs) Handles updateFilesTreeView.NodeMouseDoubleClick

    End Sub

    Private Sub CheckAllChildNodes(treeNode As TreeNode, nodeChecked As Boolean, checkAllTypes As Boolean)
        Dim node As TreeNode
        For Each node In treeNode.Nodes
            If (checkAllTypes) Then
                node.Checked = nodeChecked
            Else
                If (node.ForeColor = Color.DarkOrchid) Or (node.ForeColor = Color.Red) Then
                    node.Checked = False
                    ToggleParentChecks(node, node.Checked)
                Else
                    node.Checked = nodeChecked
                End If
            End If
            If node.Nodes.Count > 0 Then
                CheckAllChildNodes(node, nodeChecked, checkAllTypes)
            End If
        Next node
    End Sub

    Private Sub ToggleParentChecks(treeNode As TreeNode, nodeChecked As Boolean)
        Dim node As TreeNode
        If (Not nodeChecked) Then
            If Not treeNode.Parent Is Nothing Then
                treeNode.Parent.Checked = False
                ToggleParentChecks(treeNode.Parent, treeNode.Parent.Checked)
            End If
        Else
            If Not treeNode.Parent Is Nothing Then
                For Each node In treeNode.Parent.Nodes
                    If Not node.Checked Then
                        Exit Sub
                    End If
                Next
                treeNode.Parent.Checked = True
                ToggleParentChecks(treeNode.Parent, treeNode.Parent.Checked)
            End If
        End If
    End Sub

    Private Sub CheckForUpdatesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CheckForUpdatesToolStripMenuItem.Click
        querying = True
        DoUpdate(querying)
        QueryGameFiles(sender, e)
    End Sub

    Private Sub SelectAllToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SelectAllToolStripMenuItem.Click
        Dim myNode As TreeNode
        If (updateFilesTreeView.Nodes.Count > 0) Then
            For Each myNode In updateFilesTreeView.Nodes
                myNode.Checked = True
                If myNode.Nodes.Count > 0 Then
                    CheckAllChildNodes(myNode, myNode.Checked, True)
                End If
            Next
        End If
    End Sub

    Private Sub DeselectAllToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeselectAllToolStripMenuItem.Click
        Dim myNode As TreeNode
        If (updateFilesTreeView.Nodes.Count > 0) Then
            For Each myNode In updateFilesTreeView.Nodes
                myNode.Checked = False
                If myNode.Nodes.Count > 0 Then
                    CheckAllChildNodes(myNode, myNode.Checked, True)
                End If
            Next
        End If
    End Sub

    Private Sub ViewHelpToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ViewHelpToolStripMenuItem.Click
        MessageBox.Show("Help file coming soon.", "Coming Soon", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
    End Sub

    Private Sub customCheckBox_CheckedChanged(sender As Object, e As EventArgs) Handles customCheckBox.CheckedChanged
        updateFilesTreeView.Nodes.Clear()
        updateButton.Text = "Check for &Updates"
        outputTextbox.Text = "Updater is ready to go."
        querying = False
        DoUpdate(querying)
    End Sub

    Private Sub cleanupCheckBox_CheckedChanged(sender As Object, e As EventArgs) Handles cleanupCheckBox.CheckedChanged
        updateFilesTreeView.Nodes.Clear()
        updateButton.Text = "Check for &Updates"
        outputTextbox.Text = "Updater is ready to go."
        querying = False
        DoUpdate(querying)

        If (cleanupCheckBox.Checked) Then
            MessageBox.Show("This option shows files that have been flagged for removal by the server. If you have created files that you have named identically to one of these flagged names, they will be deleted permanently. Use caution, and only check files you are sure you want to delete. For your protection, the deletable files will not be checked by default.", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End If
    End Sub

    Private Sub GetLatestCommunityPackToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GetLatestCommunityPackToolStripMenuItem.Click
        If (engineVersionNumber = 1) Then
            Process.Start(communityPackDirectory)
        ElseIf (engineVersionNumber = 2) Then
            Process.Start(communityPack300Directory)
        Else
            Process.Start(communityPack300Directory)
        End If
    End Sub

    Private Sub CheckForNewVersionToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CheckForNewVersionToolStripMenuItem.Click
        CheckUpdaterVersion(True)
    End Sub

    Private Sub revertCheckBox_CheckedChanged(sender As Object, e As EventArgs) Handles revertCheckBox.CheckedChanged
        updateFilesTreeView.Nodes.Clear()
        updateButton.Text = "Check for &Updates"
        outputTextbox.Text = "Updater is ready to go."
        querying = False
        DoUpdate(querying)

        If (revertCheckBox.Checked) Then
            MessageBox.Show("This option shows all files on your system that are considered up to date by the server. Typically, this is every file, and there may be significant delay listing them all. If you have files you have modified or updated on purpose, you will lose the work you did to them when reverting to the server's version. Use caution, and only check files you are sure you want to revert. For your protection, the revertable files will not be checked by default.", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End If
    End Sub

    Private Sub ViewChangelogToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ViewChangelogToolStripMenuItem.Click
        If (Changelog Is Nothing) Then
            Changelog = New ChangelogForm()
            Changelog.Show()
        End If
    End Sub

    Private Sub Log(Info As String, Append As Boolean)
        Using LogWriter As New StreamWriter(logDirectory, Append)
            LogWriter.WriteLine(Info)
            LogWriter.Flush()
            LogWriter.Close()
        End Using
    End Sub

    Private Sub AdvancedModeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AdvancedModeToolStripMenuItem.Click

        If (AdvancedMode) Then
            If (updateCount > 0) Then
                If (MessageBox.Show("If you disable advanced mode you will lose any selections made. Are you sure you wish to disable advanced mode?", "Disable Advanced Mode?", MessageBoxButtons.YesNo, MessageBoxIcon.None) = DialogResult.Yes) Then
                    ToggleAdvanced(sender, e, False)
                End If
            Else
                ToggleAdvanced(sender, e, False)
            End If

        Else
            If (MessageBox.Show("Are you sure you want to swith to advanced mode? Advanced mode contains options that can cause issues with your game.", "Enable Advanced Mode?", MessageBoxButtons.YesNo, MessageBoxIcon.None) = DialogResult.Yes) Then
                ToggleAdvanced(sender, e, True)
            End If
        End If
    End Sub
    Private Sub ToggleAdvanced(sender As Object, e As EventArgs, Advanced As Boolean)
        Dim myNode As TreeNode

        AdvancedMode = Advanced

        If (Advanced) Then
            AdvancedModeToolStripMenuItem.Text = "Disable Advanced M&ode"
            updateFilesTreeView.CheckBoxes = True
            selectAllButton.Visible = True
            deselectAllButton.Visible = True
            customCheckBox.Visible = True
            cleanupCheckBox.Visible = True
            revertCheckBox.Visible = True
            SelectAllToolStripMenuItem.Visible = True
            DeselectAllToolStripMenuItem.Visible = True
            UpdateSettings("BootAdvanced", "1")

            If (updateCount > 0) Then
                If (updateFilesTreeView.Nodes.Count > 0) Then
                    For Each myNode In updateFilesTreeView.Nodes
                        myNode.Checked = True
                        If myNode.Nodes.Count > 0 Then
                            CheckAllChildNodes(myNode, myNode.Checked, True)
                        End If
                    Next
                End If

                updateButton.Text = "&Update Selected"
            End If
        Else
            UpdateSettings("BootAdvanced", "0")
            AdvancedModeToolStripMenuItem.Text = "Enable Advanced M&ode"
            updateFilesTreeView.CheckBoxes = False
            selectAllButton.Visible = False
            deselectAllButton.Visible = False
            customCheckBox.Visible = False

            If (cleanupCheckBox.Checked) Then
                cleanupCheckBox.Checked = False
            End If
            cleanupCheckBox.Visible = False

            If (revertCheckBox.Checked) Then
                revertCheckBox.Checked = False
            End If
            revertCheckBox.Visible = False

            SelectAllToolStripMenuItem.Visible = False
            DeselectAllToolStripMenuItem.Visible = False

            CheckForCP(True)

            If (updateCount > 0) Then
                updateButton.Text = "&Update"
            End If
        End If
    End Sub

    Private Sub updateFilesTreeView_BeforeCheck(sender As Object, e As TreeViewCancelEventArgs) Handles updateFilesTreeView.BeforeCheck

        If ((updateSuccess) Or (Updating)) Then
            e.Cancel = True
        End If
    End Sub

    Private Sub ShowLatestUpdateIDToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowLatestUpdateIDToolStripMenuItem.Click
        MessageBox.Show("The latest update ID is " + "<PH>", "Latest Update ID", MessageBoxButtons.OK, MessageBoxIcon.None)
    End Sub
End Class

Class MyTreeView
    Inherits TreeView
    Protected Overrides Sub WndProc(ByRef m As Message)
        If (m.Msg = 515) Then
            m.Msg = 513
        End If
        MyBase.WndProc(m)
    End Sub

    Private Declare Function FindWindow Lib "user32.dll" Alias "FindWindowA" (ByVal lpClassName As String, ByVal lpWindowName As String) As Long
    Public Function FindWindowHandle(Caption As String) As Long
        FindWindowHandle = FindWindow(vbNullString, Caption)
    End Function
End Class