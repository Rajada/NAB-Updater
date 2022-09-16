Option Strict On
Imports System.IO
Imports System.Configuration
Imports System.Net.Http
Imports System.Text
Imports System.Net
Imports System.Reflection
Imports Microsoft.VisualBasic.FileIO

Public Class UpdaterMainForm
    ' Replace folder dialogue with file dialogue?
    ' Add tristate checkboxes.
    ' Language support
    ' Checksumming
    ' Check if file modified
    Public homeDirectory As DirectoryInfo
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
    Private updaterVersion As String = "3.83"
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
    Private Upgrading As Boolean = False

    ' Strings
    ' Windows
    Private locString_Window_AboutUpdater As String = "About Nerf ArenaBlast Updater"
    Private locString_Window_AdvancedDisable As String = "Disable Advanced Mode?"
    Private locString_Window_AdvancedEnable As String = "Enable Advanced Mode?"
    Public locString_Window_Changelog As String = "Changelog"
    Private locString_Window_ConfigNotFound As String = "Updater Configuration File Not Found"
    Private locString_Window_ConfigNotRead As String = "Unable to Read Updater Configuration File"
    Private locString_Window_DeleteFolder As String = "Delete Folder?"
    Private locString_Window_ErrorDownloading As String = "Error Downloading File"
    Private locString_Window_ExitUpdater As String = "Exit Updater?"
    Private locString_Window_IniNotFound As String = "Nerf.ini File Not Found"
    Private locString_Window_NABRunning As String = "Nerf ArenaBlast Running"
    Private locString_Window_NewVersionAvailable As String = "New Version Available"
    Private locString_Window_SelectInstall As String = "Select Nerf ArenaBlast Installation Location"
    Private locString_Window_ServerNoResponse As String = "Server Did Not Respond"
    Private locString_Window_UpdateAborted As String = "Update Aborted"
    Private locString_Window_UpdateCheckCancelled As String = "Update Check Cancelled"
    Private locString_Window_UpdateCheckComplete As String = "Update Check Complete"
    Private locString_Window_UpdateComplete As String = "Update Complete"
    Private locString_Window_UpdaterName As String = "Nerf ArenaBlast Updater"
    Private locString_Window_UpdaterRunning As String = "Updater Already Running"
    Private locString_Window_VersionCheckComplete As String = "Version Check Complete"
    Private locString_Window_Warning As String = "Warning!"

    ' Captions
    Private locString_Caption_AdvancedEnableWarning As String = "Are you sure you want to switch to advanced mode? Advanced mode contains options that can cause issues with your game if used incorrectly."
    Private locString_Caption_AdvancedDisableWarning As String = "If you disable advanced mode you will lose any selections made. Are you sure you wish to disable advanced mode?"
    Public locString_Caption_ChangelogMissing As String = "Could not load changelog."
    Private locString_Caption_CleanupWarning As String = "This option shows files that have been flagged for removal by the server. If you have created files that you have named identically to one of these flagged names, they will be deleted permanently. Use caution, and only check files you are sure you want to delete. For your protection, the deletable files will not be checked by default."
    Private locString_Caption_ConfigNotFound As String = "Could not read updater configuration settings."
    Private locString_Caption_ConfigNotRead As String = "Could not read updater configuration settings. Please make sure you have permission to read and write to the game directory. You may wish to run the updater as an administrator."
    Private locString_Caption_DetectedNAB As String = "We have detected that Nerf ArenaBlast is currently running. It is highly recommended that you close Nerf ArenaBlast before updating to ensure updates are applied correctly."
    Private locString_Caption_DetectedNABCritical As String = "We have detected that Nerf ArenaBlast is currently running. It is highly recommended that you close Nerf ArenaBlast before updating to ensure updates are applied correctly. Click OK to ignore this warning or click Cancel to abort updating."
    Private locString_Caption_ErrorChecking As String = "Error checking for updates. Check has been cancelled."
    Private locString_Caption_ErrorDownloading As String = "Error downloading file <url>."
    Private locString_Caption_ErrorReport As String = "An error report follows."
    Private locString_Caption_ExitNoUpdate As String = "Are you sure you want to exit? The game will not be updated."
    Private locString_Caption_ExitMidUpdate As String = "Are you sure you want to exit? The game has not been fully updated. This may result in game instability and inability to play in multiplayer matches."
    Private locString_Caption_IniNotLocated As String = "The Nerf.ini file could not be located in the provided directory <dir>. If you have not done so, run the game once and then locate the Nerf root folder before updating."
    Private locString_Caption_NewVersionAvailable As String = "There is a newer version of the updater available (<ver>). It is highly recommended that you use the newest version of the updater. Would you like to install it now?"
    Private locString_Caption_FolderDeletionWarning As String = "<dir> is a folder that contains other files. Are you sure you wish to delete it?"
    Private locString_Caption_NoNewUpdates As String = "No new updates available at this time."
    Private locString_Caption_NoNewVersion As String = "No new version available at this time."
    Private locString_Caption_RevertWarning As String = "This option shows all files on your system that are considered up to date by the server. Typically, this is every file, and there may be significant delay listing them all. If you have files you have modified or updated on purpose, you will lose the work you did to them when reverting to the server's version. Use caution, and only check files you are sure you want to revert. For your protection, the revertable files will not be checked by default."
    Private locString_Caption_ServerNoResponse As String = "The update server did not respond at <url>. The URL may be wrong, the host may be down, or you may need to check your internet connection."
    Private locString_Caption_UpdateServerNoResponse As String = "The update server did not respond while checking for new versions. The URL may be wrong, the host may be down, or you may need to check your internet connection."
    Private locString_Caption_UpdatesWereSuccessful As String = "Updates were successful."
    Private locString_Caption_UpdaterForceClose As String = "In order to ensure update success, please only run one instance of the Nerf ArenaBlast Updater at a time. This program will now close."
    Private locString_Caption_UpdatingAborted As String = "Updating has been aborted."

    ' Outputs
    Private locString_Output_CheckingForUpdates As String = "Checking for updates..."
    Private locString_Output_DeletingDirectory As String = "Deleting directory"
    Private locString_Output_DeletingFile As String = "Deleting file"
    Private locString_Output_DownloadingFile As String = "Downloading file"
    Private locString_Output_DownloadingNewVersion As String = "Downloading new version..."
    Private locString_Output_ErrorChecking As String = "Error checking for updates."
    Private locString_Output_NoNewUpdates As String = "No new updates available."
    Private locString_Output_Refreshing As String = "Refreshing"
    Private locString_Output_ServerError As String = "Server error!"
    Private locString_Output_UpdaterReady As String = "Updater is ready to go."
    Private locString_Output_UpdatesPending As String = "updates pending"
    Private locString_Output_UpdatesSelected As String = "updates selected"
    Private locString_Output_UpdatesSuccessful As String = "Updates successful."
    Private locString_Output_UpdatingFiles As String = "Updating files..."
    Private locString_Output_UpdatingSelectedFiles As String = "Updating selected files..."

    ' GUI
    Private locString_GUI_BaseSeperator As String = "Base Game Files"
    Private locString_GUI_Change As String = "Change"
    Private locString_GUI_CheckForUpdates As String = "Check for &Updates"
    Private locString_GUI_Checking As String = "Checking..."
    Private locString_GUI_CPSeperator As String = "Community Pack <ver> Files"
    Private locString_GUI_DeselectAll As String = "&Deselect All Updates"
    Private locString_GUI_Exit As String = "E&xit"
    Private locString_GUI_LastChecked As String = "Last Checked"
    ' Private locString_GUI_Modified As String = "Modified"
    Private locString_GUI_Never As String = "Never"
    Private locString_GUI_SelectAll As String = "&Select All Updates"
    Private locString_GUI_Update As String = "&Update"
    Private locString_GUI_UpdateAvailable As String = "update available"
    Private locString_GUI_UpdatesAvailable As String = "updates available"
    Private locString_GUI_UpdateSelected As String = "&Update Selected"
    Private locString_GUI_Updating As String = "Updating..."
    Private locString_GUI_UpdatingFilesAt As String = "Updating files at"
    Private locString_GUI_CustomContent As String = "Custom Content"
    Private locString_GUI_FileCleanup As String = "File Cleanup"
    Private locString_GUI_FileReverts As String = "File Reverts"

    ' Toolbars
    Private locString_Toolbar_File As String = "&File"
    Private locString_Toolbar_ChangeDirectory As String = "Change Base Directory"
    Private locString_Toolbar_CheckForUpdates As String = "Check for &Updates"
    Private locString_Toolbar_GetCP As String = "&Get the Latest Community Pack"
    Private locString_Toolbar_Options As String = "&Options"
    Private locString_Toolbar_Language As String = "&Language"
    Private locString_Toolbar_OpenDirectory As String = "Open Game Directory"
    Private locString_Toolbar_Version As String = "&Version"
    Private locString_Toolbar_CheckVersion As String = "Chec&k for New Version"
    Private locString_Toolbar_Changelog As String = "View &Changelog"
    Private locString_Toolbar_Help As String = "&Help"
    Private locString_Toolbar_ViewHelp As String = "V&iew Help"
    Private locString_Toolbar_Website As String = "Visit Website"
    Private locString_Toolbar_About As String = "&About..."
    Private locString_Toolbar_AdvancedOptions As String = "Advanc&ed Options"
    Private locString_Toolbar_AdvancedEnable As String = "Enable Advanced M&ode"
    Private locString_Toolbar_AdvancedDisable As String = "Disable Advanced M&ode"
    Private locString_Toolbar_UpdateID As String = "Show &Latest Update ID"

    ' Log
    Private locString_Log_BootAdvanced As String = "Booting in advanced mode..."
    Private locString_Log_BootSuccess As String = "<app> <ver> booted successfully!"
    Private locString_Log_CheckingForBaseUpdates As String = "Checking for Community Pack updates at <url>..."
    Private locString_Log_CheckingForCPUpdates As String = "Checking for Community Pack updates at <url>..."
    Private locString_Log_CheckingForUpdates As String = "Checking for updates..."
    Private locString_Log_CPDetected As String = "Community Pack version <ver> detected."
    Private locString_Log_CPNotDetected As String = "Community Pack not detected."
    Private locString_Log_CPUnknown As String = "Warning: Unknown Community Pack detected."
    Private locString_Log_CreateFolder As String = "Creating directory <dir>."
    Private locString_Log_DeleteEmptyFolder As String = "Deleting directory <dir>."
    Private locString_Log_DeleteFile As String = "Deleting <dir>."
    Private locString_Log_DeleteFullFolder As String = "Deleting directory and contents <dir>."
    Private locString_Log_DetectedNABLangEnglish As String = "Detected language in Nerf ArenaBlast as English."
    Private locString_Log_DetectedNABLangGerman As String = "Detected language in Nerf ArenaBlast as German."
    Private locString_Log_DetectedNABLangItalian As String = "Detected language in Nerf ArenaBlast as Italian."
    Private locString_Log_DetectingCP As String = "Detecting Community Pack..."
    Private locString_Log_DetectingEngine As String = "Detecting engine version..."
    Private locString_Log_DownloadingNewVersion As String = "Downloading new version..."
    Private locString_Log_DownloadNewFile As String = "Downloading new file <url>."
    Private locString_Log_DownloadUpdatedFile As String = "Downloading updated file <url>."
    Private locString_Log_EngineError As String = "Error: Could not detect engine version."
    Private locString_Log_EngineImproved As String = "Improved engine version detected."
    Private locString_Log_EngineStandard As String = "Standard engine version detected."
    Private locString_Log_EngineUnknown As String = "Warning: Unknown engine version <ver> detected."
    Private locString_Log_ErrorChecking As String = "Error: Error checking for updates."
    Private locString_Log_ErrorDownloading As String = "Error: Could not download file <url>."
    Private locString_Log_ErrorLoadingConfig As String = "Error: Could not read updater configuration settings for loading."
    Private locString_Log_ErrorSavingConfig As String = "Error: Could not read updater configuration settings for saving."
    Private locString_Log_ExitNoUpdate As String = "Closing without updating."
    Private locString_Log_ExitMidUpdate As String = "Warning: Closing without fully updating."
    Private locString_Log_FoundUpdate As String = "Found <num> update."
    Private locString_Log_FoundUpdates As String = "Found <num> updates."
    Private locString_Log_IniNotLocated As String = "Warning: Could not locate Nerf.ini in <dir>."
    Private locString_Log_InstanceUpdater As String = "Instancing updater at <dir>."
    Private locString_Log_LogClose As String = "Log closed <date> at <time>."
    Private locString_Log_LogOpen As String = "Log opened <date> at <time>."
    Private locString_Log_NewVersion As String = "Found a new version (<ver1>), your version (<ver2>)."
    Private locString_Log_NoNewVersion As String = "No new version available at this time."
    Private locString_Log_NoNewUpdates As String = "No new updates available at this time."
    Private locString_Log_PathChanged As String = "Update path changed to <dir>."
    Private locString_Log_RefreshFile As String = "Refreshing <dir>."
    Private locString_log_ServerNoResponse As String = "Error: Could not reach the update server at <url>."
    Private locString_Log_Shutdown As String = "Updater shutting down."
    Private locString_Log_UpdateFile As String = "Updating file <dir>."
    Private locString_Log_UpdateServerNoResponse As String = "Warning: Could not reach the update server while checking for new versions."
    Private locString_Log_UpdatesWereSuccessful As String = "Updates were successful."
    Private locString_Log_UpdatingFiles As String = "Updating files..."
    Private locString_Log_UpdatingSelectedFiles As String = "Updating selected files..."

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
                If (MessageBox.Show(locString_Caption_DetectedNABCritical, locString_Window_NABRunning, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) = DialogResult.Cancel) Then
                    Return True
                End If
            Else
                MessageBox.Show(locString_Caption_DetectedNAB, locString_Window_NABRunning, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            End If
        End If
        Return False
    End Function

    Public Function CheckUpdaterRunning() As Boolean
        If (CheckForProcess("Nerf ArenaBlast Updater") > 2) Then
            silentClose = True
            Return True
        ElseIf (CheckForProcess("Nerf ArenaBlast Updater") > 1) Then
            MessageBox.Show(locString_Caption_UpdaterForceClose, locString_Window_UpdaterRunning, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
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
        GetPrivateProfileString(sSection, sKey, fallbackValue, sReturn, sReturn.Capacity, sINIFile)
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
            If (MessageBox.Show(locString_Caption_ExitNoUpdate, locString_Window_ExitUpdater, MessageBoxButtons.YesNo, MessageBoxIcon.None) = DialogResult.Yes) Then
                Log(locString_Log_ExitNoUpdate, True)
                Close()
                Exit Sub
            End If
        Else
            If (updateDiff > 0) Then
                If (MessageBox.Show(locString_Caption_ExitMidUpdate, locString_Window_ExitUpdater, MessageBoxButtons.YesNo, MessageBoxIcon.None) = DialogResult.Yes) Then
                    Log(locString_Log_ExitMidUpdate, True)
                    Close()
                    Exit Sub
                End If
            Else
                Close()
                Exit Sub
            End If
        End If
    End Sub

    Private Sub IterateLanguageOptions()
        Dim ldi As New DirectoryInfo(Directory.GetCurrentDirectory().ToString)
        Dim FileArray As FileInfo() = ldi.GetFiles()
        Dim LFI As FileInfo
        Dim LTSMI As ToolStripMenuItem

        For Each LFI In FileArray
            If ((LFI.Name Like "*.lang") And (Not LFI.Name Like "International English.lang")) Then
                LTSMI = New ToolStripMenuItem()
                LTSMI.Name = LFI.Name.Replace(".lang", "ToolStripMenuItem")
                LTSMI.Text = LFI.Name.Replace(".lang", "")
                AddHandler LTSMI.Click, AddressOf MenuItemClicked
                LanguageToolStripMenuItem.DropDownItems.Add(LTSMI)
            End If
        Next LFI
    End Sub

    Private Sub MenuItemClicked(ByVal sender As System.Object, ByVal e As System.EventArgs)
        SetLanguage(DirectCast(sender, ToolStripItem).Text)
    End Sub

    Private Sub UpdaterMainForm_Load(sender As Object, e As EventArgs) Handles Me.Load
        IterateLanguageOptions()
        Text = locString_Window_UpdaterName + " " + updaterVersion

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
            Log(locString_Log_ErrorLoadingConfig, True)
        End Try

        Log(locString_Log_LogOpen.Replace("<date>", thisDate.ToShortDateString).Replace("<time>", thisTime.ToShortTimeString), CBool(InitialAppend))
        LoadConfigSettings()
        CheckIniLocation(sender, e)
        updateFilesTreeView.DrawMode = TreeViewDrawMode.OwnerDrawAll
        CheckNABRunning(False)
    End Sub

    Private Sub UpdaterMainForm_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        If (newVersion) Then
            Process.Start(Path.Combine(Directory.GetCurrentDirectory, "update.bat"))
        End If
    End Sub

    Private Sub UpdaterMainForm_Loaded(sender As Object, e As EventArgs) Handles Me.Shown
        ShowLatestUpdateIDToolStripMenuItem.Enabled = False
        SelectAllToolStripMenuItem.Visible = False
        DeselectAllToolStripMenuItem.Visible = False
        Log(locString_Log_BootSuccess.Replace("<app>", locString_Window_UpdaterName).Replace("<ver>", updaterVersion), True)
        If Not (updaterDirectory = "") Then
            CheckUpdaterVersion(False)
        End If

        If (BootAdvanced > 0) Then
            If (Not Upgrading) Then
                Log(locString_Log_BootAdvanced, True)
            End If
            ToggleAdvanced(sender, e, True)
        End If
    End Sub

    Private Sub MainFormClosing(sender As Object, e As EventArgs) Handles Me.Closing
        If (Changelog IsNot Nothing) Then
            Changelog.Close()
        End If

        If (Not silentClose) Then
            Log(locString_Log_Shutdown, True)
            Log(locString_Log_LogClose.Replace("<date>", thisDate.ToShortDateString).Replace("<time>", thisTime.ToShortTimeString), True)
        End If
    End Sub

    Private Async Sub CheckUpdaterVersion(ShowUpToDateMessage As Boolean)
        Dim latestVersion As String = updaterVersion
        Dim downPath As String = Path.Combine(updaterDirectory, "version.txt")
        Dim filePath As String = Path.Combine(Directory.GetCurrentDirectory, "version.txt")
        Dim updaterPath As String = Directory.GetCurrentDirectory
        Dim myWebClient As New WebClient()

        If Not (My.Computer.FileSystem.DirectoryExists(updaterPath)) Then
            My.Computer.FileSystem.CreateDirectory(updaterPath)
        End If

        Try
            myWebClient.DownloadFile(downPath, filePath)
        Catch ex As Exception
            Log(locString_Log_UpdateServerNoResponse, True)
            If (ex.InnerException IsNot Nothing) Then
                MessageBox.Show(locString_Caption_UpdateServerNoResponse & ControlChars.NewLine & ControlChars.NewLine & locString_Caption_ErrorReport & ControlChars.NewLine & ControlChars.NewLine & ex.InnerException.Message, locString_Window_ServerNoResponse, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Else
                MessageBox.Show(locString_Caption_UpdateServerNoResponse & ControlChars.NewLine & ControlChars.NewLine & locString_Caption_ErrorReport & ControlChars.NewLine & ControlChars.NewLine & ex.Message, locString_Window_ServerNoResponse, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
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
            Log(locString_Log_NewVersion.Replace("<ver1>", latestVersion).Replace("<ver2>", updaterVersion), True)
            If (MessageBox.Show(locString_Caption_NewVersionAvailable.Replace("<ver>", latestVersion), locString_Window_NewVersionAvailable, MessageBoxButtons.YesNo, MessageBoxIcon.None) = DialogResult.Yes) Then
                Upgrading = True
                changeFilepathButton.Enabled = False
                ChangeBaseDirectoryToolStripMenuItem.Enabled = False
                VersionToolStripMenuItem.Enabled = False
                updateFilesTreeView.Enabled = False
                cleanupCheckBox.Enabled = False
                customCheckBox.Enabled = False
                revertCheckBox.Enabled = False
                selectAllButton.Enabled = False
                SelectAllToolStripMenuItem.Enabled = False
                deselectAllButton.Enabled = False
                DeselectAllToolStripMenuItem.Enabled = False
                updateFilesTreeView.Nodes.Clear()
                outputTextbox.Text = locString_Output_DownloadingNewVersion
                Log(locString_Log_DownloadingNewVersion, True)
                UpdateSettings("InitialAppend", "1")
                updateButton.Enabled = False
                CheckForUpdatesToolStripMenuItem.Enabled = False
                AdvancedModeToolStripMenuItem.Enabled = False

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
                    If (My.Computer.FileSystem.FileExists(Path.Combine(Directory.GetCurrentDirectory, "Nerf ArenaBlast Updater.exe.new"))) Then
                        newVersion = True
                    End If
                End If
                myWebClient.Dispose()
                Close()
                Exit Sub
            End If
            myWebClient.Dispose()
        Else
            Log(locString_Log_NoNewVersion, True)
            UpdateSettings("InitialAppend", "0")
            If (ShowUpToDateMessage) Then
                MessageBox.Show(locString_Caption_NoNewVersion, locString_Window_VersionCheckComplete, MessageBoxButtons.OK, MessageBoxIcon.None)
                outputTextbox.Text = "."
            End If
        End If
    End Sub

    Private Function DetectLanguage() As String
        Dim tempLang As StringBuilder = New StringBuilder
        Dim lang As String
        readIni(iniDirectory.ToString, "Engine.Engine", "Langauge", tempLang, "int")

        lang = "International English"

        If (tempLang.ToString = "int") Then
            lang = "International English"
            Log(locString_Log_DetectedNABLangEnglish, True)
        ElseIf (tempLang.ToString = "det") Then
            lang = "German"
            Log(locString_Log_DetectedNABLangGerman, True)
        ElseIf (tempLang.ToString = "itt") Then
            lang = "Italian"
            Log(locString_Log_DetectedNABLangItalian, True)
        End If

        Return lang
    End Function

    Private Sub LoadConfigSettings()

        If Not (My.Computer.FileSystem.FileExists(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile)) Then
            homeDirectory = New DirectoryInfo(Directory.GetParent(Directory.GetCurrentDirectory()).FullName)
            iniDirectory = New DirectoryInfo(Path.Combine(homeDirectory.FullName, "System\Nerf.ini"))
            Dim lang As String = DetectLanguage()
            SetLanguage(lang)
            Log(locString_Log_InstanceUpdater.Replace("<dir>", homeDirectory.FullName), True)
            UpdateSettings("GamePath", Directory.GetParent(Directory.GetCurrentDirectory()).FullName)
            UpdateSettings("BaseQueryURL", selectedBaseDirectory)
            UpdateSettings("CommunityQueryURL", selectedCustomDirectory)
            UpdateSettings("UpdateQueryURL", updaterDirectory)
            UpdateSettings("LastDate", "Never")
            UpdateSettings("LastTime", String.Empty)
            UpdateSettings("InitialAppend", "0")
            UpdateSettings("BootAdvanced", "0")
            UpdateSettings("Language", lang)
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
                SetLanguage(ConfigurationManager.AppSettings("Language"))
            Catch e As ConfigurationErrorsException
                Log(locString_Log_ErrorLoadingConfig, True)
                MessageBox.Show(locString_Caption_ConfigNotRead, locString_Window_ConfigNotRead, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Close()
                Exit Sub
            End Try
        End If
    End Sub

    Private Sub CheckIniLocation(sender As Object, e As EventArgs)
        While (Not (File.Exists(iniDirectory.FullName)) And Not (Aborting))
            Log(locString_Log_IniNotLocated.Replace("<dir>", iniDirectory.FullName), True)
            MessageBox.Show(locString_Caption_IniNotLocated.Replace("<dir>", iniDirectory.FullName), locString_Window_IniNotFound, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            ShowGamePathSelector(sender, e)
        End While
        If (Not Aborting) Then
            UpdateSettings("GamePath", homeDirectory.FullName)
            updatePathLabel.Text = locString_GUI_UpdatingFilesAt & homeDirectory.FullName
            CheckEngineVersion()
            CheckForCP(True)
            updateFilesTreeView.Nodes.Clear()
            updateButton.Text = locString_GUI_CheckForUpdates
            outputTextbox.Text = locString_Output_UpdaterReady
            querying = False
            DoUpdate(querying)
        End If
    End Sub

    Private Sub CheckEngineVersion()
        Dim engineVersion As StringBuilder = New StringBuilder(64)

        Log(locString_Log_DetectingEngine, True)
        readIni(iniDirectory.ToString, "Engine.Engine", "GameEngine", engineVersion, "ERROR")

        If (engineVersion.ToString = "Engine.GameEngine") Then
            Log(locString_Log_EngineStandard, True)
            engineVersionNumber = 1
            selectedBaseDirectory = onlineOldBaseDirectory
            selectedCustomDirectory = onlineOldCustomDirectory
            UpdateSettings("BaseQueryURL", onlineOldBaseDirectory)
            UpdateSettings("CommunityQueryURL", onlineOldCustomDirectory)
            GetLatestCommunityPackToolStripMenuItem.Enabled = True
        ElseIf (engineVersion.ToString = "EngineI.GameEngineI") Then
            Log(locString_Log_EngineImproved, True)
            engineVersionNumber = 2
            selectedBaseDirectory = onlineBaseDirectory
            selectedCustomDirectory = onlineCustomDirectory
            UpdateSettings("BaseQueryURL", onlineBaseDirectory)
            UpdateSettings("CommunityQueryURL", onlineCustomDirectory)
            GetLatestCommunityPackToolStripMenuItem.Enabled = True
        ElseIf (engineVersion.ToString = "ERROR") Then
            Log(locString_Log_EngineError, True)
            engineVersionNumber = -1
            selectedBaseDirectory = onlineBaseDirectory
            selectedCustomDirectory = onlineCustomDirectory
            UpdateSettings("BaseQueryURL", onlineBaseDirectory)
            UpdateSettings("CommunityQueryURL", onlineCustomDirectory)
            GetLatestCommunityPackToolStripMenuItem.Enabled = False
        Else
            Log(locString_Log_EngineUnknown.Replace("<ver>", engineVersion.ToString), True)
            engineVersionNumber = 0
            selectedBaseDirectory = onlineBaseDirectory
            selectedCustomDirectory = onlineCustomDirectory
            UpdateSettings("BaseQueryURL", onlineBaseDirectory)
            UpdateSettings("CommunityQueryURL", onlineCustomDirectory)
            GetLatestCommunityPackToolStripMenuItem.Enabled = False
        End If
    End Sub

    Private Sub CheckForCP(ChangeStates As Boolean)
        Log(locString_Log_DetectingCP, True)

        If (File.Exists(Path.Combine(homeDirectory.FullName, "System\CommunityPack.ini"))) Then
            customCheckBox.Enabled = True
            If (ChangeStates) Then
                customCheckBox.Checked = True
            End If
            Dim cpVersion As String
            Dim sb As StringBuilder = New StringBuilder(64)

            cpVersion = readIni(Path.Combine(homeDirectory.FullName, "System\CommunityPack.ini"), "Community Pack", "Version", sb, "ERROR")

            If (cpVersion = "ERROR") Then
                Log(locString_Log_CPUnknown, True)
                hasCP = False
                cpVersionString = ""
            Else
                Log(locString_Log_CPDetected.Replace("<ver>", cpVersion), True)
                hasCP = True
                cpVersionString = cpVersion
            End If

        Else
            customCheckBox.Enabled = False
            Log(locString_Log_CPNotDetected, True)
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
            Log(locString_Log_ErrorSavingConfig, True)
            MessageBox.Show(locString_Caption_ConfigNotFound, locString_Window_ConfigNotFound, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End Try
    End Sub

    Private Function ShowGamePathSelector(sender As Object, e As EventArgs) As Boolean
        Dim selGamePathDialogue As FolderBrowserDialog = New FolderBrowserDialog()
        selGamePathDialogue.Description = locString_Window_SelectInstall
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
            Log(locString_Log_PathChanged.Replace("<dir>", homeDirectory.ToString), True)
            CheckIniLocation(sender, e)
        End If
    End Sub

    Private Sub DoUpdate(isQuerying As Boolean)
        updateFilesTreeView.Enabled = Not isQuerying
        changeFilepathButton.Enabled = Not isQuerying
        ChangeBaseDirectoryToolStripMenuItem.Enabled = Not isQuerying
        AdvancedModeToolStripMenuItem.Enabled = Not isQuerying
        VersionToolStripMenuItem.Enabled = Not isQuerying
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
            'updateButton.Enabled = True
            CheckForUpdatesToolStripMenuItem.Enabled = True
            customCheckBox.Enabled = hasCP
        Else
            updateFilesTreeView.Nodes.Clear()
            updateButton.Enabled = False
            CheckForUpdatesToolStripMenuItem.Enabled = False
        End If
    End Sub

    Private Sub updateButton_Click(sender As Object, e As EventArgs) Handles updateButton.Click
        updateButton.Focus()

        If (updateButton.Text = locString_GUI_CheckForUpdates) Then
            CheckNABRunning(False)
            updateSuccess = False
            updateButton.Text = locString_GUI_Checking
            outputTextbox.Text = locString_Output_CheckingForUpdates
            Log(locString_Log_CheckingForUpdates, True)
            updateCount = 0
            querying = True
            DoUpdate(querying)
            updateProgressBar.Maximum = 1
            updateProgressBar.Minimum = 0
            updateProgressBar.Value = 0
            updateProgressBar.Step = 1
            QueryGameFiles(sender, e)
        ElseIf ((updateButton.Text = locString_GUI_UpdateSelected) Or (updateButton.Text = locString_GUI_Update)) Then
            If (CheckNABRunning(True)) Then
                MessageBox.Show(locString_Caption_UpdatingAborted, locString_Window_UpdateAborted, MessageBoxButtons.OK, MessageBoxIcon.None)
            Else
                Updating = True
                updateButton.Text = locString_GUI_Updating
                If (AdvancedMode) Then
                    outputTextbox.Text = locString_Output_UpdatingSelectedFiles
                    Log(locString_Log_UpdatingSelectedFiles, True)
                Else
                    outputTextbox.Text = locString_Output_UpdatingFiles
                    Log(locString_Log_UpdatingFiles, True)
                End If
                updateButton.Enabled = False
                CheckForUpdatesToolStripMenuItem.Enabled = False
                CopyFilesOver()
                updateSuccess = True
                'writeINI(iniDirectory.ToString, "Update", "UpdateID", "")
                'Log("Update ID is " + "", True)
                updateButton.Text = locString_GUI_CheckForUpdates
                updateButton.Enabled = True
                CheckForUpdatesToolStripMenuItem.Enabled = True
                Log(locString_Log_UpdatesWereSuccessful, True)
                MessageBox.Show(locString_Caption_UpdatesWereSuccessful, locString_Window_UpdateComplete, MessageBoxButtons.OK, MessageBoxIcon.None)
                outputTextbox.Text = locString_Output_UpdatesSuccessful
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
                                        folderNode.Tag = dir + entry.FileName.Replace(".delete", "/")
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
            Log(locString_log_ServerNoResponse.Replace("<url>", parentNode.Tag.ToString), True)
            If (ex.InnerException IsNot Nothing) Then
                MessageBox.Show(locString_Caption_ServerNoResponse.Replace("<url>", parentNode.Tag.ToString) & ControlChars.NewLine & ControlChars.NewLine & locString_Caption_ErrorReport & ControlChars.NewLine & ControlChars.NewLine & ex.InnerException.Message, locString_Window_ServerNoResponse, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Else
                MessageBox.Show(locString_Caption_ServerNoResponse.Replace("<url>", parentNode.Tag.ToString) & ControlChars.NewLine & ControlChars.NewLine & locString_Caption_ErrorReport & ControlChars.NewLine & ControlChars.NewLine & ex.Message, locString_Window_ServerNoResponse, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            End If
            'updateFilesTreeView.Nodes.Clear()
            outputTextbox.Text = locString_Output_ServerError
            'querying = False
            'updateFilesTreeView.Enabled = True
            'selectAllButton.Enabled = False
            'SelectAllToolStripMenuItem.Enabled = False
            'deselectAllButton.Enabled = False
            'DeselectAllToolStripMenuItem.Enabled = False
            'updateButton.Text = locGUICheckForUpdates
            'DoUpdate(querying)
            updateCount = -1
            Exit Function
        End Try
    End Function

    Private Async Sub QueryGameFiles(sender As Object, e As EventArgs)
        Dim i As Integer = 0
        Dim root As TreeNode = updateFilesTreeView.Nodes.Add(locString_GUI_BaseSeperator)
        Dim myNode As TreeNode
        Dim tempCount As Integer = 0

        nodesToDelete.Clear()
        root.Tag = selectedBaseDirectory
        updateFilesTreeView.BeginUpdate()
        Log(locString_Log_CheckingForBaseUpdates.Replace("<url>", ConfigurationManager.AppSettings("BaseQueryURL")), True)
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
            root = updateFilesTreeView.Nodes.Add(locString_GUI_CPSeperator.Replace("<ver>", cpVersionString))
            root.Tag = selectedCustomDirectory
            Log(locString_Log_CheckingForCPUpdates.Replace("<url>", ConfigurationManager.AppSettings("CommunityQueryURL")), True)
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
            If (updateCount > 0) Then
                If (AdvancedMode) Then
                    updateButton.Text = locString_GUI_UpdateSelected

                    If (tempCount <= 0) Then
                        updateButton.Enabled = False
                    Else
                        updateButton.Enabled = True
                    End If
                Else
                    updateButton.Text = locString_GUI_Update
                End If
            Else
                updateButton.Text = locString_GUI_CheckForUpdates
            End If
            updateSuccess = False
            selectAllButton.Enabled = True
            SelectAllToolStripMenuItem.Enabled = True
            deselectAllButton.Enabled = True
            DeselectAllToolStripMenuItem.Enabled = True
            DoUpdate(querying)

            If (updateCount = 1) Then
                Log(locString_Log_FoundUpdate.Replace("<num>", updateCount.ToString), True)
                outputTextbox.Text = updateCount.ToString + " " + locString_GUI_UpdateAvailable + "."
            Else
                Log(locString_Log_FoundUpdates.Replace("<num>", updateCount.ToString), True)
                outputTextbox.Text = updateCount.ToString + " " + locString_GUI_UpdatesAvailable + "."
            End If

            If (AdvancedMode) Then
                If (tempCount = 1) Then
                    outputTextbox.Text += ControlChars.NewLine + ControlChars.NewLine + tempCount.ToString + " " + locString_Output_UpdatesSelected + "."
                Else
                    outputTextbox.Text += ControlChars.NewLine + ControlChars.NewLine + tempCount.ToString + " " + locString_Output_UpdatesSelected + "."
                End If
            End If

            updateDiff = updateCount
            updateProgressBar.Value = 0
            UpdateLastTime()
            customCheckBox.Enabled = hasCP
        ElseIf (updateCount < 0) Then
            selectAllButton.Enabled = False
            SelectAllToolStripMenuItem.Enabled = False
            deselectAllButton.Enabled = False
            DeselectAllToolStripMenuItem.Enabled = False
            DoUpdate(querying)
            updateButton.Text = locString_GUI_CheckForUpdates
            updateSuccess = True
            Log(locString_Log_ErrorChecking, True)
            MessageBox.Show(locString_Caption_ErrorChecking, locString_Window_UpdateCheckCancelled, MessageBoxButtons.OK, MessageBoxIcon.None)
            outputTextbox.Text = locString_Output_ErrorChecking
            updateDiff = 0
            updateProgressBar.Value = 0
            updateButton.Enabled = True
            CheckForUpdatesToolStripMenuItem.Enabled = True
            changeFilepathButton.Enabled = True
            ChangeBaseDirectoryToolStripMenuItem.Enabled = True
            VersionToolStripMenuItem.Enabled = True
            AdvancedModeToolStripMenuItem.Enabled = True
            customCheckBox.Enabled = hasCP
        Else
            selectAllButton.Enabled = False
            SelectAllToolStripMenuItem.Enabled = False
            deselectAllButton.Enabled = False
            DeselectAllToolStripMenuItem.Enabled = False
            DoUpdate(querying)
            updateButton.Text = locString_GUI_CheckForUpdates
            updateSuccess = True
            Log(locString_Log_NoNewUpdates, True)
            MessageBox.Show(locString_Caption_NoNewUpdates, locString_Window_UpdateCheckComplete, MessageBoxButtons.OK, MessageBoxIcon.None)
            outputTextbox.Text = locString_Output_NoNewUpdates
            UpdateLastTime()
            updateDiff = 0
            updateProgressBar.Value = 0
            updateButton.Enabled = True
            CheckForUpdatesToolStripMenuItem.Enabled = True
            changeFilepathButton.Enabled = True
            ChangeBaseDirectoryToolStripMenuItem.Enabled = True
            VersionToolStripMenuItem.Enabled = True
            AdvancedModeToolStripMenuItem.Enabled = True
            customCheckBox.Enabled = hasCP
        End If

        cleanupCheckBox.Enabled = True
        revertCheckBox.Enabled = True
    End Sub

    Private Sub UpdateLastTime()
        thisDate = Today
        thisTime = TimeOfDay
        lastUpdateLabel.Text = thisDate.ToShortDateString & ControlChars.NewLine & thisTime.ToShortTimeString
        UpdateSettings("LastDate", thisDate.ToShortDateString)
        UpdateSettings("LastTime", thisTime.ToShortTimeString)
    End Sub

    Private Function RecurseCountFiles(treeNode As TreeNode, fileCount As Integer) As Integer
        Dim node As TreeNode
        Dim cleanupNode As TreeNode
        Dim tempFileCount As Integer

        If ((treeNode.Nodes.Count <= 0) And (Not treeNode.ForeColor = Color.Red)) Then
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
                    If (node.ForeColor = Color.Red) Then
                        fileCount += 1
                    Else
                        If (Not nodesToDelete.Contains(node)) Then
                            nodesToDelete.Add(node)
                        End If


                        If (node.Parent IsNot Nothing) Then
                            tempFileCount = 0
                            For Each cleanupNode In node.Parent.Nodes
                                If (cleanupNode.ForeColor = Color.Red) Then
                                    tempFileCount += 1
                                ElseIf (Not nodesToDelete.Contains(cleanupNode)) AndAlso (cleanupNode.Nodes.Count > 0) Then
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
                ElseIf ((node.ForeColor = Color.Red) And (node.Checked)) Then
                    selCount += 1
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

                If ((Not (actualOnlinePath Like "*/")) Or ((actualOnlinePath Like "*/") And (node.ForeColor = Color.Red))) Then
                    If (node.ForeColor = Color.Red) Then
                        If (node.Text Like "*.*") Then
                            If (My.Computer.FileSystem.FileExists(actualPath)) Then
                                outputTextbox.Text = locString_Output_DeletingFile & ControlChars.NewLine & ControlChars.NewLine & node.Text
                                Log(locString_Log_DeleteFile.Replace("<dir>", actualPath), True)
                                My.Computer.FileSystem.DeleteFile(actualPath)
                            End If
                            node.ForeColor = Color.DarkRed
                        Else
                            outputTextbox.Text = locString_Output_DeletingDirectory & ControlChars.NewLine & ControlChars.NewLine & node.Text
                            If (actualDirectoryInfo.EnumerateFiles().Any() <> False) Or (actualDirectoryInfo.EnumerateDirectories().Any() <> False) Then
                                If (MessageBox.Show(locString_Caption_FolderDeletionWarning.Replace("<dir>", actualPath), locString_Window_DeleteFolder, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) = DialogResult.Yes) Then
                                    Log(locString_Log_DeleteFullFolder.Replace("<dir>", actualPath), True)
                                    My.Computer.FileSystem.DeleteDirectory(actualPath, DeleteDirectoryOption.DeleteAllContents)
                                    node.ForeColor = Color.DarkRed
                                End If
                            Else
                                Log(locString_Log_DeleteEmptyFolder.Replace("<dir>", actualPath), True)
                                My.Computer.FileSystem.DeleteDirectory(actualPath, DeleteDirectoryOption.DeleteAllContents)
                                node.ForeColor = Color.DarkRed
                            End If
                        End If
                    Else
                        Try
                            If Not (My.Computer.FileSystem.DirectoryExists(actualPath.Substring(0, actualPath.LastIndexOf("\")))) Then
                                Log(locString_Log_CreateFolder.Replace("<dir>", actualPath), True)
                                My.Computer.FileSystem.CreateDirectory(actualPath.Substring(0, actualPath.LastIndexOf("\")))
                            End If

                            If (node.ForeColor = Color.Blue) Then
                                Log(locString_Log_DownloadUpdatedFile.Replace("<url>", actualOnlinePath), True)
                            ElseIf (node.ForeColor = Color.Orange) Then
                                Log(locString_Log_DownloadNewFile.Replace("<url>", actualOnlinePath), True)
                            End If

                            outputTextbox.Text = locString_Output_DownloadingFile & ControlChars.NewLine & ControlChars.NewLine & node.Text
                            myWebClient.DownloadFile(actualOnlinePath, actualPath)

                            If (node.Text Like ("Default.ini*")) Then
                                If (My.Computer.FileSystem.FileExists(actualPath.Replace("Default.ini", "Nerf.ini"))) Then
                                    Log(locString_Log_DeleteFile.Replace("<dir>", actualPath), True)
                                    outputTextbox.Text = locString_Output_DeletingFile & ControlChars.NewLine & ControlChars.NewLine & "Nerf.ini"
                                    My.Computer.FileSystem.DeleteFile(actualPath.Replace("Default.ini", "Nerf.ini"))
                                End If
                                If (My.Computer.FileSystem.FileExists(actualPath)) Then
                                    Log(locString_Log_RefreshFile.Replace("<dir>", actualPath), True)
                                    outputTextbox.Text = locString_Output_Refreshing & ControlChars.NewLine & ControlChars.NewLine & "Nerf.ini"
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
                            Log(locString_Log_UpdateFile.Replace("<dir>", actualPath), True)
                        Catch e As Exception
                            Log(locString_Log_ErrorDownloading.Replace("<url>", actualOnlinePath), True)
                            MessageBox.Show(locString_Caption_ErrorDownloading.Replace("<url>", actualOnlinePath) & ControlChars.NewLine & ControlChars.NewLine & locString_Caption_ErrorReport & ControlChars.NewLine & ControlChars.NewLine & e.ToString, locString_Window_ErrorDownloading, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
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
                    outputTextbox.Text = updateProgressBar.Maximum & locString_Output_UpdatesSelected
                Else
                    outputTextbox.Text = updateProgressBar.Maximum & locString_Output_UpdatesPending
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
        Dim selCount As Integer = 0
        Dim totalCount As Integer = 0
        Dim oldMessage As String
        Dim lineSeperator As Integer = -1

        If (updateFilesTreeView.Nodes.Count > 0) Then
            For Each myNode In updateFilesTreeView.Nodes
                totalCount = RecurseCountFiles(myNode, totalCount)
                selCount = RecurseCountSelected(myNode, selCount)
            Next
        End If

        If (totalCount <= 0) Then
            updateButton.Text = locString_GUI_CheckForUpdates
        Else
            If (selCount <= 0) Then
                updateButton.Enabled = False
            Else
                If (AdvancedMode) Then
                    updateButton.Text = locString_GUI_UpdateSelected
                Else
                    updateButton.Text = locString_GUI_Update
                End If
                updateButton.Enabled = True
            End If
        End If

        lineSeperator = outputTextbox.Text.IndexOf(ControlChars.NewLine)
        If (lineSeperator > -1) Then
            oldMessage = outputTextbox.Text.Substring(0, lineSeperator)
        Else
            oldMessage = outputTextbox.Text
        End If
        If (AdvancedMode) Then
            outputTextbox.Text = oldMessage + ControlChars.NewLine + ControlChars.NewLine + selCount.ToString + " " + locString_Output_UpdatesSelected + "."
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
        MessageBox.Show(locString_Window_UpdaterName & " " & updaterVersion & ControlChars.NewLine & ControlChars.NewLine & "© Jared Petersen " & Date.Today.Year, locString_Window_AboutUpdater, MessageBoxButtons.OK, MessageBoxIcon.None)
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
        CheckNABRunning(False)
        updateSuccess = False
        updateButton.Text = locString_GUI_Checking
        outputTextbox.Text = locString_Output_CheckingForUpdates
        Log(locString_Log_CheckingForUpdates, True)
        updateCount = 0
        querying = True
        DoUpdate(querying)
        updateProgressBar.Maximum = 1
        updateProgressBar.Minimum = 0
        updateProgressBar.Value = 0
        updateProgressBar.Step = 1
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
        updateButton.Text = locString_GUI_CheckForUpdates
        outputTextbox.Text = locString_Output_UpdaterReady
        querying = False
        DoUpdate(querying)
    End Sub

    Private Sub cleanupCheckBox_CheckedChanged(sender As Object, e As EventArgs) Handles cleanupCheckBox.CheckedChanged
        updateFilesTreeView.Nodes.Clear()
        updateButton.Text = locString_GUI_CheckForUpdates
        outputTextbox.Text = locString_Output_UpdaterReady
        querying = False
        DoUpdate(querying)

        If (cleanupCheckBox.Checked) Then
            MessageBox.Show(locString_Caption_CleanupWarning, locString_Window_Warning, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
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
        updateButton.Text = locString_GUI_CheckForUpdates
        outputTextbox.Text = locString_Output_UpdaterReady
        querying = False
        DoUpdate(querying)

        If (revertCheckBox.Checked) Then
            MessageBox.Show(locString_Caption_RevertWarning, locString_Window_Warning, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
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
                If (MessageBox.Show(locString_Caption_AdvancedDisableWarning, locString_Window_AdvancedDisable, MessageBoxButtons.YesNo, MessageBoxIcon.None) = DialogResult.Yes) Then
                    ToggleAdvanced(sender, e, False)
                End If
            Else
                ToggleAdvanced(sender, e, False)
            End If

        Else
            If (MessageBox.Show(locString_Caption_AdvancedEnableWarning, locString_Window_AdvancedEnable, MessageBoxButtons.YesNo, MessageBoxIcon.None) = DialogResult.Yes) Then
                ToggleAdvanced(sender, e, True)
            End If
        End If
    End Sub
    Private Sub ToggleAdvanced(sender As Object, e As EventArgs, Advanced As Boolean)
        Dim myNode As TreeNode

        AdvancedMode = Advanced

        If (Advanced) Then
            AdvancedModeToolStripMenuItem.Text = locString_Toolbar_AdvancedDisable
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
                updateButton.Text = locString_GUI_UpdateSelected
            End If
        Else
            UpdateSettings("BootAdvanced", "0")
            AdvancedModeToolStripMenuItem.Text = locString_Toolbar_AdvancedEnable
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
            updateButton.Text = locString_GUI_CheckForUpdates
            updateButton.Enabled = True
            CheckForUpdatesToolStripMenuItem.Enabled = True
            updateCount = 0
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

    Private Sub OpenGameDirectoryToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenGameDirectoryToolStripMenuItem.Click
        Process.Start(homeDirectory.FullName)
    End Sub

    Private Sub LoadLanguageStrings(Lang As String)
        Dim ldi As New DirectoryInfo(Directory.GetCurrentDirectory().ToString)
        Dim FileArray As FileInfo() = ldi.GetFiles()
        Dim LFI As FileInfo
        Dim SB As StringBuilder = New StringBuilder(512)

        For Each LFI In FileArray
            If (LFI.Name Like (Lang + ".lang")) Then
                ldi = New DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory().ToString, LFI.Name))

                ' Windows
                locString_Window_AboutUpdater = readIni(ldi.FullName, "Windows", "locString_Window_AboutUpdater", SB, "About Nerf ArenaBlast Updater")
                locString_Window_AdvancedDisable = readIni(ldi.FullName, "Windows", "locString_Window_AdvancedDisable", SB, "Disable Advanced Mode?")
                locString_Window_AdvancedEnable = readIni(ldi.FullName, "Windows", "locString_Window_AdvancedEnable", SB, "Enable Advanced Mode?")
                locString_Window_Changelog = readIni(ldi.FullName, "Windows", "locString_Window_Changelog", SB, "Changelog")
                locString_Window_ConfigNotFound = readIni(ldi.FullName, "Windows", "locString_Window_ConfigNotFound", SB, "Updater Configuration File Not Found")
                locString_Window_ConfigNotRead = readIni(ldi.FullName, "Windows", "locString_Window_ConfigNotRead", SB, "Unable to Read Updater Configuration File")
                locString_Window_DeleteFolder = readIni(ldi.FullName, "Windows", "locString_Window_DeleteFolder", SB, "Delete Folder?")
                locString_Window_ErrorDownloading = readIni(ldi.FullName, "Windows", "locString_Window_ErrorDownloading", SB, "Error Downloading File")
                locString_Window_ExitUpdater = readIni(ldi.FullName, "Windows", "locString_Window_ExitUpdater", SB, "Exit Updater?")
                locString_Window_IniNotFound = readIni(ldi.FullName, "Windows", "locString_Window_IniNotFound", SB, "Nerf.ini File Not Found")
                locString_Window_NABRunning = readIni(ldi.FullName, "Windows", "locString_Window_NABRunning", SB, "Nerf ArenaBlast Running")
                locString_Window_NewVersionAvailable = readIni(ldi.FullName, "Windows", "locString_Window_NewVersionAvailable", SB, "New Version Available")
                locString_Window_SelectInstall = readIni(ldi.FullName, "Windows", "locString_Window_SelectInstall", SB, "Select Nerf ArenaBlast Installation Location")
                locString_Window_ServerNoResponse = readIni(ldi.FullName, "Windows", "locString_Window_ServerNoResponse", SB, "Server Did Not Respond")
                locString_Window_UpdateAborted = readIni(ldi.FullName, "Windows", "locString_Window_UpdateAborted", SB, "Update Aborted")
                locString_Window_UpdateCheckCancelled = readIni(ldi.FullName, "Windows", "locString_Window_UpdateCheckCancelled", SB, "Update Check Cancelled")
                locString_Window_UpdateCheckComplete = readIni(ldi.FullName, "Windows", "locString_Window_UpdateCheckComplete", SB, "Update Check Complete")
                locString_Window_UpdateComplete = readIni(ldi.FullName, "Windows", "locString_Window_UpdateComplete", SB, "Update Complete")
                locString_Window_UpdaterName = readIni(ldi.FullName, "Windows", "locString_Window_UpdaterName", SB, "Nerf ArenaBlast Updater")
                Me.Text = locString_Window_UpdaterName + " " + updaterVersion
                locString_Window_UpdaterRunning = readIni(ldi.FullName, "Windows", "locString_Window_UpdaterRunning", SB, "Updater Already Running")
                locString_Window_VersionCheckComplete = readIni(ldi.FullName, "Windows", "locString_Window_VersionCheckComplete", SB, "Version Check Complete")
                locString_Window_Warning = readIni(ldi.FullName, "Windows", "locString_Window_Warning", SB, "Warning!")
                Exit For
            End If
        Next LFI
    End Sub

    Private Sub CheckboxLanguage(lang As String)
        Dim LTSI As ToolStripItem
        Dim LMI As ToolStripMenuItem

        If (lang = "International English") Then
            For Each LTSI In LanguageToolStripMenuItem.DropDownItems
                If TypeOf (LTSI) Is ToolStripMenuItem Then
                    LMI = CType(LTSI, ToolStripMenuItem)
                    LMI.Checked = (LTSI.Text = lang)
                End If
            Next LTSI
        Else
            For Each LTSI In LanguageToolStripMenuItem.DropDownItems
                If TypeOf (LTSI) Is ToolStripMenuItem Then
                    LMI = CType(LTSI, ToolStripMenuItem)
                    LMI.Checked = (LTSI.Text = lang)
                End If
            Next
        End If
    End Sub

    Private Sub SetLanguage(Lang As String)
        Dim LanguageDirectory As DirectoryInfo = New DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), Lang + ".lang"))

        If ((Lang = "") Or (Lang = "International English")) Then
            UpdateSettings("Language", "International English")
            Log("Setting language to International English", True)
            LoadLanguageStrings("International English")
            CheckboxLanguage("International English")
        Else
            If (My.Computer.FileSystem.FileExists(LanguageDirectory.FullName)) Then
                UpdateSettings("Language", Lang)
                Log("Setting language to " & Lang, True)
                LoadLanguageStrings(Lang)
                CheckboxLanguage(Lang)
            Else
                UpdateSettings("Language", "International English")
                Log("Failed to change language to " & Lang & ", defaulting to International English", True)
                LoadLanguageStrings("International English")
                CheckboxLanguage("International English")
            End If
        End If
    End Sub

    Private Sub InternationalEnglishDefaultToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles InternationalEnglishDefaultToolStripMenuItem.Click
        SetLanguage("International English")
    End Sub
End Class

Class MyTreeView
    Inherits TreeView
    Protected Overrides Sub WndProc(ByRef m As Message)
        If (m.Msg = 515) Then
            m.Msg = 513
        End If
        Try
            MyBase.WndProc(m)
        Catch ex As NullReferenceException
        End Try
    End Sub

    Private Declare Function FindWindow Lib "user32.dll" Alias "FindWindowA" (ByVal lpClassName As String, ByVal lpWindowName As String) As Long
    Public Function FindWindowHandle(Caption As String) As Long
        FindWindowHandle = FindWindow(vbNullString, Caption)
    End Function
End Class