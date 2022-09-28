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
    Private updateStatus As String = "Ready"
    Private baseFiles As FileInfo()
    Private customFiles As FileInfo()
    Private querying As Boolean = False
    Private nodesToDelete As New List(Of TreeNode)
    Private filesToDelete As New List(Of String)
    Private updaterVersion As String = "3.84"
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
    Const constString_Window_AboutUpdater As String = "About <app>"
    Const constString_Window_AdvancedDisable As String = "Disable Advanced Mode?"
    Const constString_Window_AdvancedEnable As String = "Enable Advanced Mode?"
    Const constString_Window_Changelog As String = "Changelog"
    Const constString_Window_ConfigNotFound As String = "<app> Configuration File Not Found"
    Const constString_Window_ConfigNotRead As String = "Unable to Read <app> Configuration File"
    Const constString_Window_DeleteFolder As String = "Delete Folder?"
    Const constString_Window_ErrorDownloading As String = "Error Downloading File"
    Const constString_Window_ExitUpdater As String = "Exit <app>?"
    Const constString_Window_IniNotFound As String = "Nerf.ini File Not Found"
    Const constString_Window_NABRunning As String = "Nerf ArenaBlast Running"
    Const constString_Window_NewVersionAvailable As String = "New Version Available"
    Const constString_Window_SelectInstall As String = "Select Nerf ArenaBlast Installation Location"
    Const constString_Window_ServerNoResponse As String = "Server Did Not Respond"
    Const constString_Window_UpdateAborted As String = "Update Aborted"
    Const constString_Window_UpdateCheckCancelled As String = "Update Check Cancelled"
    Const constString_Window_UpdateCheckComplete As String = "Update Check Complete"
    Const constString_Window_UpdateComplete As String = "Update Complete"
    Const constString_Window_UpdaterName As String = "Nerf ArenaBlast Updater"
    Const constString_Window_UpdaterRunning As String = "<app> Already Running"
    Const constString_Window_VersionCheckComplete As String = "Version Check Complete"
    Const constString_Window_Warning As String = "Warning!"

    ' Captions
    Const constString_Caption_AdvancedEnableWarning As String = "Are you sure you want to switch to advanced mode? Advanced mode contains options that can cause issues with your game if used incorrectly."
    Const constString_Caption_AdvancedDisableWarning As String = "If you disable advanced mode you will lose any selections made. Are you sure you want to disable advanced mode?"
    Const constString_Caption_ChangelogMissing As String = "Could not load changelog."
    Const constString_Caption_CleanupWarning As String = "This option shows files that have been flagged for removal by the server. If you have created files that you have named identically to one of these flagged names, they will be deleted permanently. Use caution, and only check files you are sure you want to delete. For your protection, the deletable files will not be checked by default."
    Const constString_Caption_ConfigNotFound As String = "Could not read <app> configuration settings."
    Const constString_Caption_ConfigNotRead As String = "Could not read <app> configuration settings. Please make sure you have permission to read and write to the game directory. You may want to run the <app> as an administrator."
    Const constString_Caption_DetectedNAB As String = "We have detected that Nerf ArenaBlast is currently running. It is highly recommended that you close Nerf ArenaBlast before updating to ensure updates are applied correctly."
    Const constString_Caption_DetectedNABCritical As String = "We have detected that Nerf ArenaBlast is currently running. It is highly recommended that you close Nerf ArenaBlast before updating to ensure updates are applied correctly. Click OK to ignore this warning or click Cancel to abort updating."
    Const constString_Caption_ErrorChecking As String = "Error checking for updates. Check has been cancelled."
    Const constString_Caption_ErrorDownloading As String = "Error downloading file <url>."
    Const constString_Caption_ErrorReport As String = "An error report follows."
    Const constString_Caption_ExitNoUpdate As String = "Are you sure you want to exit? The game will not be updated."
    Const constString_Caption_ExitMidUpdate As String = "Are you sure you want to exit? The game has not been fully updated. This may result in game instability and inability to play in multiplayer matches."
    Const constString_Caption_IniNotLocated As String = "The Nerf.ini file could not be located in the provided directory <dir>. If you have not done so, run the game once and then locate the Nerf root folder before updating."
    Const constString_Caption_NewVersionAvailable As String = "There is a newer version of the <app> available (<ver>). It is highly recommended that you use the newest version of the <app>. Would you like to install it now?"
    Const constString_Caption_FolderDeletionWarning As String = "<dir> is a folder that contains other files. Are you sure you want to delete it?"
    Const constString_Caption_NoNewUpdates As String = "No new updates available at this time."
    Const constString_Caption_NoNewVersion As String = "No new version available at this time."
    Const constString_Caption_RevertWarning As String = "This option shows all files on your system that are considered up to date by the server. Typically, this is every file, and there may be significant delay listing them all. If you have files you have modified or updated on purpose, you will lose the work you did to them when reverting to the server's version. Use caution, and only check files you are sure you want to revert. For your protection, the revertable files will not be checked by default."
    Const constString_Caption_ServerNoResponse As String = "The update server did not respond at <url>. The URL may be wrong, the host may be down, or you may need to check your internet connection."
    Const constString_Caption_UpdateServerNoResponse As String = "The update server did not respond while checking for new versions. The URL may be wrong, the host may be down, or you may need to check your internet connection."
    Const constString_Caption_UpdatesWereSuccessful As String = "Updates were successful."
    Const constString_Caption_UpdaterForceClose As String = "In order to ensure update success, please only run one instance of the <app> at a time. This program will now close."
    Const constString_Caption_UpdatingAborted As String = "Updating has been aborted."

    ' Outputs
    Const constString_Output_CheckingForUpdates As String = "Checking for updates..."
    Const constString_Output_DeletingDirectory As String = "Deleting directory"
    Const constString_Output_DeletingFile As String = "Deleting file"
    Const constString_Output_DownloadingFile As String = "Downloading file"
    Const constString_Output_DownloadingNewVersion As String = "Downloading new version..."
    Const constString_Output_ErrorChecking As String = "Error checking for updates."
    Const constString_Output_NoNewUpdates As String = "No new updates available."
    Const constString_Output_NoNewVersion As String = "No new version available."
    Const constString_Output_Refreshing As String = "Refreshing"
    Const constString_Output_ServerError As String = "Server error!"
    Const constString_Output_UpdateAvailable As String = "update available"
    Const constString_Output_UpdaterReady As String = "<app> is ready to go."
    Const constString_Output_UpdatesAvailable As String = "updates available"
    Const constString_Output_UpdatesPending As String = "updates pending"
    Const constString_Output_UpdatesSelected As String = "updates selected"
    Const constString_Output_UpdatesSuccessful As String = "Updates successful."
    Const constString_Output_UpdatingFiles As String = "Updating files..."
    Const constString_Output_UpdatingSelectedFiles As String = "Updating selected files..."

    ' GUI
    Const constString_GUI_BaseSeperator As String = "Base Game Files"
    Const constString_GUI_Change As String = "Change"
    Const constString_GUI_CheckForUpdates As String = "Check for &Updates"
    Const constString_GUI_Checking As String = "Checking..."
    Const constString_GUI_CPSeperator As String = "Community Pack <ver> Files"
    Const constString_GUI_DeselectAll As String = "&Deselect All Updates"
    Const constString_GUI_Exit As String = "E&xit"
    Const constString_GUI_LastChecked As String = "Last Checked"
    Const constString_GUI_Modified As String = "Modified"
    Const constString_GUI_Never As String = "Never"
    Const constString_GUI_SelectAll As String = "&Select All Updates"
    Const constString_GUI_Update As String = "&Update"
    Const constString_GUI_UpdateSelected As String = "&Update Selected"
    Const constString_GUI_Updating As String = "Updating..."
    Const constString_GUI_UpdatingFilesAt As String = "Updating files at"
    Const constString_GUI_CustomContent As String = "Custom Content"
    Const constString_GUI_FileCleanup As String = "File Cleanup"
    Const constString_GUI_FileReverts As String = "File Reverts"

    ' Toolbars
    Const constString_Toolbar_File As String = "&File"
    Const constString_Toolbar_ChangeDirectory As String = "Change Game Directory"
    Const constString_Toolbar_CheckForUpdates As String = "Check for &Updates"
    Const constString_Toolbar_SelectAll As String = "&Select All Updates"
    Const constString_Toolbar_DeselectAll As String = "&Deselect All Updates"
    Const constString_Toolbar_GetCP As String = "&Get the Latest Community Pack"
    Const constString_Toolbar_Exit As String = "E&xit"
    Const constString_Toolbar_Options As String = "&Options"
    Const constString_Toolbar_Language As String = "&Language"
    Const constString_Toolbar_OpenDirectory As String = "Open Game Directory"
    Const constString_Toolbar_Version As String = "&Version"
    Const constString_Toolbar_CheckVersion As String = "Chec&k for New Version"
    Const constString_Toolbar_Changelog As String = "View &Changelog"
    Const constString_Toolbar_Help As String = "&Help"
    Const constString_Toolbar_ViewHelp As String = "V&iew Help"
    Const constString_Toolbar_Website As String = "Visit Website"
    Const constString_Toolbar_About As String = "&About..."
    Const constString_Toolbar_AdvancedOptions As String = "Advanc&ed Options"
    Const constString_Toolbar_AdvancedEnable As String = "Enable Advanced M&ode"
    Const constString_Toolbar_AdvancedDisable As String = "Disable Advanced M&ode"
    Const constString_Toolbar_UpdateID As String = "Show &Latest Update ID"

    ' Log
    Const constString_Log_BootAdvanced As String = "Booting in advanced mode..."
    Const constString_Log_BootSuccess As String = "<app> <ver> booted successfully!"
    Const constString_Log_CheckingForBaseUpdates As String = "Checking for base game updates at <url>..."
    Const constString_Log_CheckingForCPUpdates As String = "Checking for Community Pack updates at <url>..."
    Const constString_Log_CheckingForUpdates As String = "Checking for updates..."
    Const constString_Log_CPDetected As String = "Community Pack version <ver> detected."
    Const constString_Log_CPNotDetected As String = "Community Pack not detected."
    Const constString_Log_CPUnknown As String = "Warning: Unknown Community Pack detected."
    Const constString_Log_CreateFolder As String = "Creating directory <dir>."
    Const constString_Log_DeleteEmptyFolder As String = "Deleting directory <dir>."
    Const constString_Log_DeleteFile As String = "Deleting <dir>."
    Const constString_Log_DeleteFullFolder As String = "Deleting directory and contents <dir>."
    Const constString_Log_DetectedNABLangEnglish As String = "Detected language in Nerf ArenaBlast as English."
    Const constString_Log_DetectedNABLangGerman As String = "Detected language in Nerf ArenaBlast as German."
    Const constString_Log_DetectedNABLangItalian As String = "Detected language in Nerf ArenaBlast as Italian."
    Const constString_Log_DetectingCP As String = "Detecting Community Pack..."
    Const constString_Log_DetectingEngine As String = "Detecting engine version..."
    Const constString_Log_DownloadingNewVersion As String = "Downloading new version..."
    Const constString_Log_DownloadNewFile As String = "Downloading new file <url>."
    Const constString_Log_DownloadUpdatedFile As String = "Downloading updated file <url>."
    Const constString_Log_EngineError As String = "Error: Could not detect engine version."
    Const constString_Log_EngineImproved As String = "Improved engine version detected."
    Const constString_Log_EngineStandard As String = "Standard engine version detected."
    Const constString_Log_EngineUnknown As String = "Warning: Unknown engine version <ver> detected."
    Const constString_Log_ErrorChecking As String = "Error: Error checking for updates."
    Const constString_Log_ErrorDownloading As String = "Error: Could not download file <url>."
    Const constString_Log_ErrorLoadingConfig As String = "Error: Could not read <app> configuration settings for loading."
    Const constString_Log_ErrorSavingConfig As String = "Error: Could not read <app> configuration settings for saving."
    Const constString_Log_ExitNoUpdate As String = "Closing without updating."
    Const constString_Log_ExitMidUpdate As String = "Warning: Closing without fully updating."
    Const constString_Log_FoundUpdate As String = "Found <num> update."
    Const constString_Log_FoundUpdates As String = "Found <num> updates."
    Const constString_Log_IniNotLocated As String = "Warning: Could not locate Nerf.ini in <dir>."
    Const constString_Log_InstanceUpdater As String = "Instancing <app> at <dir>."
    Const constString_Log_LangNotSet As String = "Failed to change language to <lang>, language not changed."
    Const constString_Log_LangNotSetDef As String = "Failed to change language to <lang>, defaulting to International English."
    Const constString_Log_LangSet As String = "Setting language to <lang>."
    Const constString_Log_LogClose As String = "Log closed <date> at <time>."
    Const constString_Log_LogOpen As String = "Log opened <date> at <time>."
    Const constString_Log_NewVersion As String = "Found a new version (<ver1>), current version (<ver2>)."
    Const constString_Log_NoNewVersion As String = "No new version available at this time."
    Const constString_Log_NoNewUpdates As String = "No new updates available at this time."
    Const constString_Log_PathChanged As String = "Game path changed to <dir>."
    Const constString_Log_RefreshFile As String = "Refreshing <dir>."
    Const constString_log_ServerNoResponse As String = "Error: Could not reach the update server at <url>."
    Const constString_Log_Shutdown As String = "<app> shutting down."
    Const constString_Log_UpdateFile As String = "Updating file <dir>."
    Const constString_Log_UpdateServerNoResponse As String = "Warning: Could not reach the update server while checking for new versions."
    Const constString_Log_UpdatesWereSuccessful As String = "Updates were successful."
    Const constString_Log_UpdatingFiles As String = "Updating files..."
    Const constString_Log_UpdatingSelectedFiles As String = "Updating selected files..."

    ' Windows
    Private locString_Window_AboutUpdater As String = constString_Window_AboutUpdater
    Private locString_Window_AdvancedDisable As String = constString_Window_AdvancedDisable
    Private locString_Window_AdvancedEnable As String = constString_Window_AdvancedEnable
    Public locString_Window_Changelog As String = constString_Window_Changelog
    Private locString_Window_ConfigNotFound As String = constString_Window_ConfigNotFound
    Private locString_Window_ConfigNotRead As String = constString_Window_ConfigNotRead
    Private locString_Window_DeleteFolder As String = constString_Window_DeleteFolder
    Private locString_Window_ErrorDownloading As String = constString_Window_ErrorDownloading
    Private locString_Window_ExitUpdater As String = constString_Window_ExitUpdater
    Private locString_Window_IniNotFound As String = constString_Window_IniNotFound
    Private locString_Window_NABRunning As String = constString_Window_NABRunning
    Private locString_Window_NewVersionAvailable As String = constString_Window_NewVersionAvailable
    Private locString_Window_SelectInstall As String = constString_Window_SelectInstall
    Private locString_Window_ServerNoResponse As String = constString_Window_ServerNoResponse
    Private locString_Window_UpdateAborted As String = constString_Window_UpdateAborted
    Private locString_Window_UpdateCheckCancelled As String = constString_Window_UpdateCheckCancelled
    Private locString_Window_UpdateCheckComplete As String = constString_Window_UpdateCheckComplete
    Private locString_Window_UpdateComplete As String = constString_Window_UpdateComplete
    Private locString_Window_UpdaterName As String = constString_Window_UpdaterName
    Private locString_Window_UpdaterRunning As String = constString_Window_UpdaterRunning
    Private locString_Window_VersionCheckComplete As String = constString_Window_VersionCheckComplete
    Private locString_Window_Warning As String = constString_Window_Warning

    ' Captions
    Private locString_Caption_AdvancedEnableWarning As String = constString_Caption_AdvancedEnableWarning
    Private locString_Caption_AdvancedDisableWarning As String = constString_Caption_AdvancedDisableWarning
    Public locString_Caption_ChangelogMissing As String = constString_Caption_ChangelogMissing
    Private locString_Caption_CleanupWarning As String = constString_Caption_CleanupWarning
    Private locString_Caption_ConfigNotFound As String = constString_Caption_ConfigNotFound
    Private locString_Caption_ConfigNotRead As String = constString_Caption_ConfigNotRead
    Private locString_Caption_DetectedNAB As String = constString_Caption_DetectedNAB
    Private locString_Caption_DetectedNABCritical As String = constString_Caption_DetectedNABCritical
    Private locString_Caption_ErrorChecking As String = constString_Caption_ErrorChecking
    Private locString_Caption_ErrorDownloading As String = constString_Caption_ErrorDownloading
    Private locString_Caption_ErrorReport As String = constString_Caption_ErrorReport
    Private locString_Caption_ExitNoUpdate As String = constString_Caption_ExitNoUpdate
    Private locString_Caption_ExitMidUpdate As String = constString_Caption_ExitMidUpdate
    Private locString_Caption_IniNotLocated As String = constString_Caption_IniNotLocated
    Private locString_Caption_NewVersionAvailable As String = constString_Caption_NewVersionAvailable
    Private locString_Caption_FolderDeletionWarning As String = constString_Caption_FolderDeletionWarning
    Private locString_Caption_NoNewUpdates As String = constString_Caption_NoNewUpdates
    Private locString_Caption_NoNewVersion As String = constString_Caption_NoNewVersion
    Private locString_Caption_RevertWarning As String = constString_Caption_RevertWarning
    Private locString_Caption_ServerNoResponse As String = constString_Caption_ServerNoResponse
    Private locString_Caption_UpdateServerNoResponse As String = constString_Caption_UpdateServerNoResponse
    Private locString_Caption_UpdatesWereSuccessful As String = constString_Caption_UpdatesWereSuccessful
    Private locString_Caption_UpdaterForceClose As String = constString_Caption_UpdaterForceClose
    Private locString_Caption_UpdatingAborted As String = constString_Caption_UpdatingAborted

    ' Outputs
    Private locString_Output_CheckingForUpdates As String = constString_Output_CheckingForUpdates
    Private locString_Output_DeletingDirectory As String = constString_Output_DeletingDirectory
    Private locString_Output_DeletingFile As String = constString_Output_DeletingFile
    Private locString_Output_DownloadingFile As String = constString_Output_DownloadingFile
    Private locString_Output_DownloadingNewVersion As String = constString_Output_DownloadingNewVersion
    Private locString_Output_ErrorChecking As String = constString_Output_ErrorChecking
    Private locString_Output_NoNewUpdates As String = constString_Output_NoNewUpdates
    Private locString_Output_NoNewVersion As String = constString_Output_NoNewVersion
    Private locString_Output_Refreshing As String = constString_Output_Refreshing
    Private locString_Output_ServerError As String = constString_Output_ServerError
    Private locString_Output_UpdateAvailable As String = constString_Output_UpdateAvailable
    Private locString_Output_UpdaterReady As String = constString_Output_UpdaterReady
    Private locString_Output_UpdatesAvailable As String = constString_Output_UpdatesAvailable
    Private locString_Output_UpdatesPending As String = constString_Output_UpdatesPending
    Private locString_Output_UpdatesSelected As String = constString_Output_UpdatesSelected
    Private locString_Output_UpdatesSuccessful As String = constString_Output_UpdatesSuccessful
    Private locString_Output_UpdatingFiles As String = constString_Output_UpdatingFiles
    Private locString_Output_UpdatingSelectedFiles As String = constString_Output_UpdatingSelectedFiles

    ' GUI
    Private locString_GUI_BaseSeperator As String = constString_GUI_BaseSeperator
    Private locString_GUI_Change As String = constString_GUI_Change
    Private locString_GUI_CheckForUpdates As String = constString_GUI_CheckForUpdates
    Private locString_GUI_Checking As String = constString_GUI_Checking
    Private locString_GUI_CPSeperator As String = constString_GUI_CPSeperator
    Private locString_GUI_DeselectAll As String = constString_GUI_DeselectAll
    Private locString_GUI_Exit As String = constString_GUI_Exit
    Private locString_GUI_LastChecked As String = constString_GUI_LastChecked
    Public locString_GUI_Modified As String = constString_GUI_Modified
    Private locString_GUI_Never As String = constString_GUI_Never
    Private locString_GUI_SelectAll As String = constString_GUI_SelectAll
    Private locString_GUI_Update As String = constString_GUI_Update
    Private locString_GUI_UpdateSelected As String = constString_GUI_UpdateSelected
    Private locString_GUI_Updating As String = constString_GUI_Updating
    Private locString_GUI_UpdatingFilesAt As String = constString_GUI_UpdatingFilesAt
    Private locString_GUI_CustomContent As String = constString_GUI_CustomContent
    Private locString_GUI_FileCleanup As String = constString_GUI_FileCleanup
    Private locString_GUI_FileReverts As String = constString_GUI_FileReverts

    ' Toolbars
    Private locString_Toolbar_File As String = constString_Toolbar_File
    Private locString_Toolbar_ChangeDirectory As String = constString_Toolbar_ChangeDirectory
    Private locString_Toolbar_CheckForUpdates As String = constString_Toolbar_CheckForUpdates
    Private locString_Toolbar_SelectAll As String = constString_Toolbar_SelectAll
    Private locString_Toolbar_DeselectAll As String = constString_Toolbar_DeselectAll
    Private locString_Toolbar_GetCP As String = constString_Toolbar_GetCP
    Private locString_Toolbar_Exit As String = constString_Toolbar_Exit
    Private locString_Toolbar_Options As String = constString_Toolbar_Options
    Private locString_Toolbar_Language As String = constString_Toolbar_Language
    Private locString_Toolbar_OpenDirectory As String = constString_Toolbar_OpenDirectory
    Private locString_Toolbar_Version As String = constString_Toolbar_Version
    Private locString_Toolbar_CheckVersion As String = constString_Toolbar_CheckVersion
    Private locString_Toolbar_Changelog As String = constString_Toolbar_Changelog
    Private locString_Toolbar_Help As String = constString_Toolbar_Help
    Private locString_Toolbar_ViewHelp As String = constString_Toolbar_ViewHelp
    Private locString_Toolbar_Website As String = constString_Toolbar_Website
    Private locString_Toolbar_About As String = constString_Toolbar_About
    Private locString_Toolbar_AdvancedOptions As String = constString_Toolbar_AdvancedOptions
    Private locString_Toolbar_AdvancedEnable As String = constString_Toolbar_AdvancedEnable
    Private locString_Toolbar_AdvancedDisable As String = constString_Toolbar_AdvancedDisable
    Private locString_Toolbar_UpdateID As String = constString_Toolbar_UpdateID

    ' Log
    Private locString_Log_BootAdvanced As String = constString_Log_BootAdvanced
    Private locString_Log_BootSuccess As String = constString_Log_BootSuccess
    Private locString_Log_CheckingForBaseUpdates As String = constString_Log_CheckingForBaseUpdates
    Private locString_Log_CheckingForCPUpdates As String = constString_Log_CheckingForCPUpdates
    Private locString_Log_CheckingForUpdates As String = constString_Log_CheckingForUpdates
    Private locString_Log_CPDetected As String = constString_Log_CPDetected
    Private locString_Log_CPNotDetected As String = constString_Log_CPNotDetected
    Private locString_Log_CPUnknown As String = constString_Log_CPUnknown
    Private locString_Log_CreateFolder As String = constString_Log_CreateFolder
    Private locString_Log_DeleteEmptyFolder As String = constString_Log_DeleteEmptyFolder
    Private locString_Log_DeleteFile As String = constString_Log_DeleteFile
    Private locString_Log_DeleteFullFolder As String = constString_Log_DeleteFullFolder
    Private locString_Log_DetectedNABLangEnglish As String = constString_Log_DetectedNABLangEnglish
    Private locString_Log_DetectedNABLangGerman As String = constString_Log_DetectedNABLangGerman
    Private locString_Log_DetectedNABLangItalian As String = constString_Log_DetectedNABLangItalian
    Private locString_Log_DetectingCP As String = constString_Log_DetectingCP
    Private locString_Log_DetectingEngine As String = constString_Log_DetectingEngine
    Private locString_Log_DownloadingNewVersion As String = constString_Log_DownloadingNewVersion
    Private locString_Log_DownloadNewFile As String = constString_Log_DownloadNewFile
    Private locString_Log_DownloadUpdatedFile As String = constString_Log_DownloadUpdatedFile
    Private locString_Log_EngineError As String = constString_Log_EngineError
    Private locString_Log_EngineImproved As String = constString_Log_EngineImproved
    Private locString_Log_EngineStandard As String = constString_Log_EngineStandard
    Private locString_Log_EngineUnknown As String = constString_Log_EngineUnknown
    Private locString_Log_ErrorChecking As String = constString_Log_ErrorChecking
    Private locString_Log_ErrorDownloading As String = constString_Log_ErrorDownloading
    Private locString_Log_ErrorLoadingConfig As String = constString_Log_ErrorLoadingConfig
    Private locString_Log_ErrorSavingConfig As String = constString_Log_ErrorSavingConfig
    Private locString_Log_ExitNoUpdate As String = constString_Log_ExitNoUpdate
    Private locString_Log_ExitMidUpdate As String = constString_Log_ExitMidUpdate
    Private locString_Log_FoundUpdate As String = constString_Log_FoundUpdate
    Private locString_Log_FoundUpdates As String = constString_Log_FoundUpdates
    Private locString_Log_IniNotLocated As String = constString_Log_IniNotLocated
    Private locString_Log_InstanceUpdater As String = constString_Log_InstanceUpdater
    Private locString_Log_LangNotSet As String = constString_Log_LangNotSet
    Private locString_Log_LangNotSetDef As String = constString_Log_LangNotSetDef
    Private locString_Log_LangSet As String = constString_Log_LangSet
    Private locString_Log_LogClose As String = constString_Log_LogClose
    Private locString_Log_LogOpen As String = constString_Log_LogOpen
    Private locString_Log_NewVersion As String = constString_Log_NewVersion
    Private locString_Log_NoNewVersion As String = constString_Log_NoNewVersion
    Private locString_Log_NoNewUpdates As String = constString_Log_NoNewUpdates
    Private locString_Log_PathChanged As String = constString_Log_PathChanged
    Private locString_Log_RefreshFile As String = constString_Log_RefreshFile
    Private locString_log_ServerNoResponse As String = constString_log_ServerNoResponse
    Private locString_Log_Shutdown As String = constString_Log_Shutdown
    Private locString_Log_UpdateFile As String = constString_Log_UpdateFile
    Private locString_Log_UpdateServerNoResponse As String = constString_Log_UpdateServerNoResponse
    Private locString_Log_UpdatesWereSuccessful As String = constString_Log_UpdatesWereSuccessful
    Private locString_Log_UpdatingFiles As String = constString_Log_UpdatingFiles
    Private locString_Log_UpdatingSelectedFiles As String = constString_Log_UpdatingSelectedFiles

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
            MessageBox.Show(locString_Caption_UpdaterForceClose.Replace("<app>", locString_Window_UpdaterName), locString_Window_UpdaterRunning.Replace("<app>", locString_Window_UpdaterName), MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
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
            If (MessageBox.Show(locString_Caption_ExitNoUpdate, locString_Window_ExitUpdater.Replace("<app>", locString_Window_UpdaterName), MessageBoxButtons.YesNo, MessageBoxIcon.None) = DialogResult.Yes) Then
                Log(locString_Log_ExitNoUpdate, True)
                Close()
                Exit Sub
            End If
        Else
            If (updateDiff > 0) Then
                If (MessageBox.Show(locString_Caption_ExitMidUpdate, locString_Window_ExitUpdater.Replace("<app>", locString_Window_UpdaterName), MessageBoxButtons.YesNo, MessageBoxIcon.None) = DialogResult.Yes) Then
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
    Private Sub SetUpdateStatus(StatusCode As String)
        If (StatusCode = "") Then
            StatusCode = updateStatus
        Else
            updateStatus = StatusCode
        End If

        If (StatusCode = "Ready") Then
            updateButton.Text = locString_GUI_CheckForUpdates
        ElseIf (StatusCode = "Checking") Then
            updateButton.Text = locString_GUI_Checking
        ElseIf (StatusCode = "Update") Then
            If (AdvancedMode) Then
                updateButton.Text = locString_GUI_UpdateSelected
            Else
                updateButton.Text = locString_GUI_Update
            End If
        ElseIf (StatusCode = "Updating") Then
            updateButton.Text = locString_GUI_Updating
        Else
            updateButton.Text = locString_GUI_CheckForUpdates
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
        SetLanguage(DirectCast(sender, ToolStripItem).Text, False)
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
            Log(locString_Log_ErrorLoadingConfig.Replace("<app>", locString_Window_UpdaterName), True)
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
            Log(locString_Log_Shutdown.Replace("<app>", locString_Window_UpdaterName), True)
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
            If (MessageBox.Show(locString_Caption_NewVersionAvailable.Replace("<app>", locString_Window_UpdaterName).Replace("<ver>", latestVersion), locString_Window_NewVersionAvailable, MessageBoxButtons.YesNo, MessageBoxIcon.None) = DialogResult.Yes) Then
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
                LanguageToolStripMenuItem.Enabled = False
                CheckForNewVersionToolStripMenuItem.Enabled = False
                ViewChangelogToolStripMenuItem.Enabled = False
                ExitToolStripMenuItem.Enabled = False
                exitButton.Enabled = False

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
                If (updateCount <= 0) Then
                    outputTextbox.Text = locString_Output_NoNewVersion
                End If
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
            SetLanguage(lang, True)
            Log(locString_Log_InstanceUpdater.Replace("<app>", locString_Window_UpdaterName).Replace("<dir>", homeDirectory.FullName), True)
            UpdateSettings("GamePath", Directory.GetParent(Directory.GetCurrentDirectory()).FullName)
            UpdateSettings("BaseQueryURL", selectedBaseDirectory)
            UpdateSettings("CommunityQueryURL", selectedCustomDirectory)
            UpdateSettings("UpdateQueryURL", updaterDirectory)
            UpdateSettings("LastDate", locString_GUI_Never)
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
                If (ConfigurationManager.AppSettings("LastTime") = String.Empty) Then
                    lastUpdateLabel.Text = locString_GUI_Never
                Else
                    lastUpdateLabel.Text = ConfigurationManager.AppSettings("LastDate") & ControlChars.NewLine & ConfigurationManager.AppSettings("LastTime")
                End If
                InitialAppend = CInt(ConfigurationManager.AppSettings("InitialAppend"))
                BootAdvanced = CInt(ConfigurationManager.AppSettings("BootAdvanced"))
                SetLanguage(ConfigurationManager.AppSettings("Language"), True)
            Catch e As ConfigurationErrorsException
                Log(locString_Log_ErrorLoadingConfig.Replace("<app>", locString_Window_UpdaterName), True)
                MessageBox.Show(locString_Caption_ConfigNotRead.Replace("<app>", locString_Window_UpdaterName), locString_Window_ConfigNotRead.Replace("<app>", locString_Window_UpdaterName), MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
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
            updatePathLabel.Text = locString_GUI_UpdatingFilesAt & " " & homeDirectory.FullName
            CheckEngineVersion()
            CheckForCP(True)
            updateFilesTreeView.Nodes.Clear()
            SetUpdateStatus("Ready")
            outputTextbox.Text = locString_Output_UpdaterReady.Replace("<app>", locString_Window_UpdaterName)
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
            Log(locString_Log_ErrorSavingConfig.Replace("<app>", locString_Window_UpdaterName), True)
            MessageBox.Show(locString_Caption_ConfigNotFound.Replace("<app>", locString_Window_UpdaterName), locString_Window_ConfigNotFound.Replace("<app>", locString_Window_UpdaterName), MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
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
        LanguageToolStripMenuItem.Enabled = Not isQuerying
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

        If (updateStatus = "Ready") Then
            CheckNABRunning(False)
            updateSuccess = False
            SetUpdateStatus("Checking")
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
        ElseIf (updateStatus = "Update") Then
            If (CheckNABRunning(True)) Then
                MessageBox.Show(locString_Caption_UpdatingAborted, locString_Window_UpdateAborted, MessageBoxButtons.OK, MessageBoxIcon.None)
            Else
                Updating = True
                SetUpdateStatus("Update")
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
                SetUpdateStatus("Ready")
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
            Return $"{FileName}".PadRight(20, "."c) & UpdaterMainForm.locString_GUI_Modified & $": {LastModified}"
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
            'setUpdateStatus(0)
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
                SetUpdateStatus("Update")
                If (AdvancedMode) Then
                    If (tempCount <= 0) Then
                        updateButton.Enabled = False
                    Else
                        updateButton.Enabled = True
                    End If
                End If
            Else
                SetUpdateStatus("Ready")
            End If
            updateSuccess = False
            selectAllButton.Enabled = True
            SelectAllToolStripMenuItem.Enabled = True
            deselectAllButton.Enabled = True
            DeselectAllToolStripMenuItem.Enabled = True
            DoUpdate(querying)

            If (updateCount = 1) Then
                Log(locString_Log_FoundUpdate.Replace("<num>", updateCount.ToString), True)
                outputTextbox.Text = updateCount.ToString + " " + locString_Output_UpdateAvailable + "."
            Else
                Log(locString_Log_FoundUpdates.Replace("<num>", updateCount.ToString), True)
                outputTextbox.Text = updateCount.ToString + " " + locString_Output_UpdatesAvailable + "."
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
            SetUpdateStatus("Ready")
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
            LanguageToolStripMenuItem.Enabled = True
            customCheckBox.Enabled = hasCP
        Else
            selectAllButton.Enabled = False
            SelectAllToolStripMenuItem.Enabled = False
            deselectAllButton.Enabled = False
            DeselectAllToolStripMenuItem.Enabled = False
            DoUpdate(querying)
            SetUpdateStatus("Ready")
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
            LanguageToolStripMenuItem.Enabled = True
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
            SetUpdateStatus("Ready")
        Else
            If (selCount <= 0) Then
                updateButton.Enabled = False
            Else
                SetUpdateStatus("Update")
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
        MessageBox.Show(locString_Window_UpdaterName & " " & updaterVersion & ControlChars.NewLine & ControlChars.NewLine & "© Jared Petersen " & Date.Today.Year, locString_Window_AboutUpdater.Replace("<app>", locString_Window_UpdaterName), MessageBoxButtons.OK, MessageBoxIcon.None)
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
        SetUpdateStatus("Checking")
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
        SetUpdateStatus("Ready")
        outputTextbox.Text = locString_Output_UpdaterReady.Replace("<app>", locString_Window_UpdaterName)
        querying = False
        DoUpdate(querying)
    End Sub

    Private Sub cleanupCheckBox_CheckedChanged(sender As Object, e As EventArgs) Handles cleanupCheckBox.CheckedChanged
        updateFilesTreeView.Nodes.Clear()
        SetUpdateStatus("Ready")
        outputTextbox.Text = locString_Output_UpdaterReady.Replace("<app>", locString_Window_UpdaterName)
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
        SetUpdateStatus("Ready")
        outputTextbox.Text = locString_Output_UpdaterReady.Replace("<app>", locString_Window_UpdaterName)
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
                SetUpdateStatus("Update")
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
            SetUpdateStatus("Ready")
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
                locString_Window_AboutUpdater = readIni(ldi.FullName, "Windows", "locString_Window_AboutUpdater", SB, constString_Window_AboutUpdater)
                locString_Window_AdvancedDisable = readIni(ldi.FullName, "Windows", "locString_Window_AdvancedDisable", SB, constString_Window_AdvancedDisable)
                locString_Window_AdvancedEnable = readIni(ldi.FullName, "Windows", "locString_Window_AdvancedEnable", SB, constString_Window_AdvancedEnable)
                locString_Window_Changelog = readIni(ldi.FullName, "Windows", "locString_Window_Changelog", SB, constString_Window_Changelog)
                If (Changelog IsNot Nothing) Then
                    Changelog.Text = locString_Window_Changelog
                End If
                locString_Window_ConfigNotFound = readIni(ldi.FullName, "Windows", "locString_Window_ConfigNotFound", SB, constString_Window_ConfigNotFound)
                locString_Window_ConfigNotRead = readIni(ldi.FullName, "Windows", "locString_Window_ConfigNotRead", SB, constString_Window_ConfigNotRead)
                locString_Window_DeleteFolder = readIni(ldi.FullName, "Windows", "locString_Window_DeleteFolder", SB, constString_Window_DeleteFolder)
                locString_Window_ErrorDownloading = readIni(ldi.FullName, "Windows", "locString_Window_ErrorDownloading", SB, constString_Window_ErrorDownloading)
                locString_Window_ExitUpdater = readIni(ldi.FullName, "Windows", "locString_Window_ExitUpdater", SB, constString_Window_ExitUpdater)
                locString_Window_IniNotFound = readIni(ldi.FullName, "Windows", "locString_Window_IniNotFound", SB, constString_Window_IniNotFound)
                locString_Window_NABRunning = readIni(ldi.FullName, "Windows", "locString_Window_NABRunning", SB, constString_Window_NABRunning)
                locString_Window_NewVersionAvailable = readIni(ldi.FullName, "Windows", "locString_Window_NewVersionAvailable", SB, constString_Window_NewVersionAvailable)
                locString_Window_SelectInstall = readIni(ldi.FullName, "Windows", "locString_Window_SelectInstall", SB, constString_Window_SelectInstall)
                locString_Window_ServerNoResponse = readIni(ldi.FullName, "Windows", "locString_Window_ServerNoResponse", SB, constString_Window_ServerNoResponse)
                locString_Window_UpdateAborted = readIni(ldi.FullName, "Windows", "locString_Window_UpdateAborted", SB, constString_Window_UpdateAborted)
                locString_Window_UpdateCheckCancelled = readIni(ldi.FullName, "Windows", "locString_Window_UpdateCheckCancelled", SB, constString_Window_UpdateCheckCancelled)
                locString_Window_UpdateCheckComplete = readIni(ldi.FullName, "Windows", "locString_Window_UpdateCheckComplete", SB, constString_Window_UpdateCheckComplete)
                locString_Window_UpdateComplete = readIni(ldi.FullName, "Windows", "locString_Window_UpdateComplete", SB, constString_Window_UpdateComplete)
                locString_Window_UpdaterName = readIni(ldi.FullName, "Windows", "locString_Window_UpdaterName", SB, constString_Window_UpdaterName)
                Me.Text = locString_Window_UpdaterName + " " + updaterVersion
                locString_Window_UpdaterRunning = readIni(ldi.FullName, "Windows", "locString_Window_UpdaterRunning", SB, constString_Window_UpdaterRunning)
                locString_Window_VersionCheckComplete = readIni(ldi.FullName, "Windows", "locString_Window_VersionCheckComplete", SB, constString_Window_VersionCheckComplete)
                locString_Window_Warning = readIni(ldi.FullName, "Windows", "locString_Window_Warning", SB, constString_Window_Warning)

                ' Captions
                locString_Caption_AdvancedEnableWarning = readIni(ldi.FullName, "Captions", "locString_Caption_AdvancedEnableWarning", SB, constString_Caption_AdvancedEnableWarning)
                locString_Caption_AdvancedDisableWarning = readIni(ldi.FullName, "Captions", "locString_Caption_AdvancedDisableWarning", SB, constString_Caption_AdvancedDisableWarning)
                locString_Caption_ChangelogMissing = readIni(ldi.FullName, "Captions", "locString_Caption_ChangelogMissing", SB, constString_Caption_ChangelogMissing)
                locString_Caption_CleanupWarning = readIni(ldi.FullName, "Captions", "locString_Caption_CleanupWarning", SB, constString_Caption_CleanupWarning)
                locString_Caption_ConfigNotFound = readIni(ldi.FullName, "Captions", "locString_Caption_ConfigNotFound", SB, constString_Caption_ConfigNotFound)
                locString_Caption_ConfigNotRead = readIni(ldi.FullName, "Captions", "locString_Caption_ConfigNotRead", SB, constString_Caption_ConfigNotRead)
                locString_Caption_DetectedNAB = readIni(ldi.FullName, "Captions", "locString_Caption_DetectedNAB", SB, constString_Caption_DetectedNAB)
                locString_Caption_DetectedNABCritical = readIni(ldi.FullName, "Captions", "locString_Caption_DetectedNABCritical", SB, constString_Caption_DetectedNABCritical)
                locString_Caption_ErrorChecking = readIni(ldi.FullName, "Captions", "locString_Caption_ErrorChecking", SB, constString_Caption_ErrorChecking)
                locString_Caption_ErrorDownloading = readIni(ldi.FullName, "Captions", "locString_Caption_ErrorDownloading", SB, constString_Caption_ErrorDownloading)
                locString_Caption_ErrorReport = readIni(ldi.FullName, "Captions", "locString_Caption_ErrorReport", SB, constString_Caption_ErrorReport)
                locString_Caption_ExitNoUpdate = readIni(ldi.FullName, "Captions", "locString_Caption_ExitNoUpdate", SB, constString_Caption_ExitNoUpdate)
                locString_Caption_ExitMidUpdate = readIni(ldi.FullName, "Captions", "locString_Caption_ExitMidUpdate", SB, constString_Caption_ExitMidUpdate)
                locString_Caption_IniNotLocated = readIni(ldi.FullName, "Captions", "locString_Caption_IniNotLocated", SB, constString_Caption_IniNotLocated)
                locString_Caption_NewVersionAvailable = readIni(ldi.FullName, "Captions", "locString_Caption_NewVersionAvailable", SB, constString_Caption_NewVersionAvailable)
                locString_Caption_FolderDeletionWarning = readIni(ldi.FullName, "Captions", "locString_Caption_FolderDeletionWarning", SB, constString_Caption_FolderDeletionWarning)
                locString_Caption_NoNewUpdates = readIni(ldi.FullName, "Captions", "locString_Caption_NoNewUpdates", SB, constString_Caption_NoNewUpdates)
                locString_Caption_NoNewVersion = readIni(ldi.FullName, "Captions", "locString_Caption_NoNewVersion", SB, constString_Caption_NoNewVersion)
                locString_Caption_RevertWarning = readIni(ldi.FullName, "Captions", "locString_Caption_RevertWarning", SB, constString_Caption_RevertWarning)
                locString_Caption_ServerNoResponse = readIni(ldi.FullName, "Captions", "locString_Caption_ServerNoResponse", SB, constString_Caption_ServerNoResponse)
                locString_Caption_UpdateServerNoResponse = readIni(ldi.FullName, "Captions", "locString_Caption_UpdateServerNoResponse", SB, constString_Caption_UpdateServerNoResponse)
                locString_Caption_UpdatesWereSuccessful = readIni(ldi.FullName, "Captions", "locString_Caption_UpdatesWereSuccessful", SB, constString_Caption_UpdatesWereSuccessful)
                locString_Caption_UpdaterForceClose = readIni(ldi.FullName, "Captions", "locString_Caption_UpdaterForceClose", SB, constString_Caption_UpdaterForceClose)
                locString_Caption_UpdatingAborted = readIni(ldi.FullName, "Captions", "locString_Caption_UpdatingAborted", SB, constString_Caption_UpdatingAborted)

                ' Outputs
                locString_Output_CheckingForUpdates = readIni(ldi.FullName, "Outputs", "locString_Output_CheckingForUpdates", SB, constString_Output_CheckingForUpdates)
                locString_Output_DeletingDirectory = readIni(ldi.FullName, "Outputs", "locString_Output_DeletingDirectory", SB, constString_Output_DeletingDirectory)
                locString_Output_DeletingFile = readIni(ldi.FullName, "Outputs", "locString_Output_DeletingFile", SB, constString_Output_DeletingFile)
                locString_Output_DownloadingFile = readIni(ldi.FullName, "Outputs", "locString_Output_DownloadingFile", SB, constString_Output_DownloadingFile)
                locString_Output_DownloadingNewVersion = readIni(ldi.FullName, "Outputs", "locString_Output_DownloadingNewVersion", SB, constString_Output_DownloadingNewVersion)
                locString_Output_ErrorChecking = readIni(ldi.FullName, "Outputs", "locString_Output_ErrorChecking", SB, constString_Output_ErrorChecking)
                locString_Output_NoNewUpdates = readIni(ldi.FullName, "Outputs", "locString_Output_NoNewUpdates", SB, constString_Output_NoNewUpdates)
                locString_Output_NoNewVersion = readIni(ldi.FullName, "Outputs", "locString_Output_NoNewVersion", SB, constString_Output_NoNewVersion)
                locString_Output_Refreshing = readIni(ldi.FullName, "Outputs", "locString_Output_Refreshing", SB, constString_Output_Refreshing)
                locString_Output_ServerError = readIni(ldi.FullName, "Outputs", "locString_Output_ServerError", SB, constString_Output_ServerError)
                locString_Output_UpdateAvailable = readIni(ldi.FullName, "Outputs", "locString_Output_UpdateAvailable", SB, constString_Output_UpdateAvailable)
                locString_Output_UpdaterReady = readIni(ldi.FullName, "Outputs", "locString_Output_UpdaterReady", SB, constString_Output_UpdaterReady)
                locString_Output_UpdatesAvailable = readIni(ldi.FullName, "Outputs", "locString_Output_UpdatesAvailable", SB, constString_Output_UpdatesAvailable)
                locString_Output_UpdatesPending = readIni(ldi.FullName, "Outputs", "locString_Output_UpdatesPending", SB, constString_Output_UpdatesPending)
                locString_Output_UpdatesSelected = readIni(ldi.FullName, "Outputs", "locString_Output_UpdatesSelected", SB, constString_Output_UpdatesSelected)
                locString_Output_UpdatesSuccessful = readIni(ldi.FullName, "Outputs", "locString_Output_UpdatesSuccessful", SB, constString_Output_UpdatesSuccessful)
                locString_Output_UpdatingFiles = readIni(ldi.FullName, "Outputs", "locString_Output_UpdatingFiles", SB, constString_Output_UpdatingFiles)
                locString_Output_UpdatingSelectedFiles = readIni(ldi.FullName, "Outputs", "locString_Output_UpdatingSelectedFiles", SB, constString_Output_UpdatingSelectedFiles)

                ' GUI
                locString_GUI_BaseSeperator = readIni(ldi.FullName, "GUI", "locString_GUI_BaseSeperator", SB, constString_GUI_BaseSeperator)
                locString_GUI_Change = readIni(ldi.FullName, "GUI", "locString_GUI_Change", SB, constString_GUI_Change)
                changeFilepathButton.Text = locString_GUI_Change
                locString_GUI_CheckForUpdates = readIni(ldi.FullName, "GUI", "locString_GUI_CheckForUpdates", SB, constString_GUI_CheckForUpdates)
                locString_GUI_Checking = readIni(ldi.FullName, "GUI", "locString_GUI_Checking", SB, constString_GUI_Checking)
                locString_GUI_CPSeperator = readIni(ldi.FullName, "GUI", "locString_GUI_CPSeperator", SB, constString_GUI_CPSeperator)
                locString_GUI_DeselectAll = readIni(ldi.FullName, "GUI", "locString_GUI_DeselectAll", SB, constString_GUI_DeselectAll)
                deselectAllButton.Text = locString_GUI_DeselectAll
                locString_GUI_Exit = readIni(ldi.FullName, "GUI", "locString_GUI_Exit", SB, constString_GUI_Exit)
                exitButton.Text = locString_GUI_Exit
                locString_GUI_LastChecked = readIni(ldi.FullName, "GUI", "locString_GUI_LastChecked", SB, constString_GUI_LastChecked)
                lastCheckedBox.Text = locString_GUI_LastChecked
                locString_GUI_Modified = readIni(ldi.FullName, "GUI", "locString_GUI_Modified", SB, constString_GUI_Modified)
                locString_GUI_Never = readIni(ldi.FullName, "GUI", "locString_GUI_Never", SB, constString_GUI_Never)
                locString_GUI_SelectAll = readIni(ldi.FullName, "GUI", "locString_GUI_SelectAll", SB, constString_GUI_SelectAll)
                selectAllButton.Text = locString_GUI_SelectAll
                locString_GUI_Update = readIni(ldi.FullName, "GUI", "locString_GUI_Update", SB, constString_GUI_Update)
                locString_GUI_UpdateSelected = readIni(ldi.FullName, "GUI", "locString_GUI_UpdateSelected", SB, constString_GUI_UpdateSelected)
                locString_GUI_Updating = readIni(ldi.FullName, "GUI", "locString_GUI_Updating", SB, constString_GUI_Updating)
                locString_GUI_UpdatingFilesAt = readIni(ldi.FullName, "GUI", "locString_GUI_UpdatingFilesAt", SB, constString_GUI_UpdatingFilesAt)
                updatePathLabel.Text = locString_GUI_UpdatingFilesAt & " " & homeDirectory.FullName
                locString_GUI_CustomContent = readIni(ldi.FullName, "GUI", "locString_GUI_CustomContent", SB, constString_GUI_CustomContent)
                customCheckBox.Text = locString_GUI_CustomContent
                locString_GUI_FileCleanup = readIni(ldi.FullName, "GUI", "locString_GUI_FileCleanup", SB, constString_GUI_FileCleanup)
                cleanupCheckBox.Text = locString_GUI_FileCleanup
                locString_GUI_FileReverts = readIni(ldi.FullName, "GUI", "locString_GUI_FileReverts", SB, constString_GUI_FileReverts)
                revertCheckBox.Text = locString_GUI_FileReverts

                SetUpdateStatus("")

                ' Toolbars
                locString_Toolbar_File = readIni(ldi.FullName, "Toolbars", "locString_Toolbar_File", SB, constString_Toolbar_File)
                FileToolStripMenuItem.Text = locString_Toolbar_File
                locString_Toolbar_ChangeDirectory = readIni(ldi.FullName, "Toolbars", "locString_Toolbar_ChangeDirectory", SB, constString_Toolbar_ChangeDirectory)
                ChangeBaseDirectoryToolStripMenuItem.Text = locString_Toolbar_ChangeDirectory
                locString_Toolbar_CheckForUpdates = readIni(ldi.FullName, "Toolbars", "locString_Toolbar_CheckForUpdates", SB, constString_Toolbar_CheckForUpdates)
                CheckForUpdatesToolStripMenuItem.Text = locString_Toolbar_CheckForUpdates
                locString_Toolbar_SelectAll = readIni(ldi.FullName, "Toolbars", "locString_Toolbar_SelectAll", SB, constString_Toolbar_SelectAll)
                SelectAllToolStripMenuItem.Text = locString_Toolbar_SelectAll
                locString_Toolbar_DeselectAll = readIni(ldi.FullName, "Toolbars", "locString_Toolbar_DeselectAll", SB, constString_Toolbar_DeselectAll)
                DeselectAllToolStripMenuItem.Text = locString_Toolbar_DeselectAll
                locString_Toolbar_GetCP = readIni(ldi.FullName, "Toolbars", "locString_Toolbar_GetCP", SB, constString_Toolbar_GetCP)
                GetLatestCommunityPackToolStripMenuItem.Text = locString_Toolbar_GetCP
                locString_Toolbar_Exit = readIni(ldi.FullName, "Toolbars", "locString_Toolbar_Exit", SB, constString_Toolbar_Exit)
                ExitToolStripMenuItem.Text = locString_Toolbar_Exit
                locString_Toolbar_Options = readIni(ldi.FullName, "Toolbars", "locString_Toolbar_Options", SB, constString_Toolbar_Options)
                OptionsToolStripMenuItem.Text = locString_Toolbar_Options
                locString_Toolbar_Language = readIni(ldi.FullName, "Toolbars", "locString_Toolbar_Language", SB, constString_Toolbar_Language)
                LanguageToolStripMenuItem.Text = locString_Toolbar_Language
                locString_Toolbar_OpenDirectory = readIni(ldi.FullName, "Toolbars", "locString_Toolbar_OpenDirectory", SB, constString_Toolbar_OpenDirectory)
                OpenGameDirectoryToolStripMenuItem.Text = locString_Toolbar_OpenDirectory
                locString_Toolbar_Version = readIni(ldi.FullName, "Toolbars", "locString_Toolbar_Version", SB, constString_Toolbar_Version)
                VersionToolStripMenuItem.Text = locString_Toolbar_Version
                locString_Toolbar_CheckVersion = readIni(ldi.FullName, "Toolbars", "locString_Toolbar_CheckVersion", SB, constString_Toolbar_CheckVersion)
                CheckForNewVersionToolStripMenuItem.Text = locString_Toolbar_CheckVersion
                locString_Toolbar_Changelog = readIni(ldi.FullName, "Toolbars", "locString_Toolbar_Changelog", SB, constString_Toolbar_Changelog)
                ViewChangelogToolStripMenuItem.Text = locString_Toolbar_Changelog
                locString_Toolbar_Help = readIni(ldi.FullName, "Toolbars", "locString_Toolbar_Help", SB, constString_Toolbar_Help)
                HelpToolStripMenuItem.Text = locString_Toolbar_Help
                locString_Toolbar_ViewHelp = readIni(ldi.FullName, "Toolbars", "locString_Toolbar_ViewHelp", SB, constString_Toolbar_ViewHelp)
                ViewHelpToolStripMenuItem.Text = locString_Toolbar_ViewHelp
                locString_Toolbar_Website = readIni(ldi.FullName, "Toolbars", "locString_Toolbar_Website", SB, constString_Toolbar_Website)
                VisitWebsiteToolStripMenuItem.Text = locString_Toolbar_Website
                locString_Toolbar_About = readIni(ldi.FullName, "Toolbars", "locString_Toolbar_About", SB, constString_Toolbar_About)
                AboutToolStripMenuItem.Text = locString_Toolbar_About
                locString_Toolbar_AdvancedOptions = readIni(ldi.FullName, "Toolbars", "locString_Toolbar_AdvancedOptions", SB, constString_Toolbar_AdvancedOptions)
                AdvancedToolStripMenuItem.Text = locString_Toolbar_AdvancedOptions
                locString_Toolbar_AdvancedEnable = readIni(ldi.FullName, "Toolbars", "locString_Toolbar_AdvancedEnable", SB, constString_Toolbar_AdvancedEnable)
                locString_Toolbar_AdvancedDisable = readIni(ldi.FullName, "Toolbars", "locString_Toolbar_AdvancedDisable", SB, constString_Toolbar_AdvancedDisable)
                If (AdvancedMode) Then
                    AdvancedModeToolStripMenuItem.Text = locString_Toolbar_AdvancedDisable
                Else
                    AdvancedModeToolStripMenuItem.Text = locString_Toolbar_AdvancedEnable
                End If
                locString_Toolbar_UpdateID = readIni(ldi.FullName, "Toolbars", "locString_Toolbar_UpdateID", SB, constString_Toolbar_UpdateID)
                ShowLatestUpdateIDToolStripMenuItem.Text = locString_Toolbar_UpdateID

                ' Log
                locString_Log_BootAdvanced = readIni(ldi.FullName, "Log", "locString_Log_BootAdvanced", SB, constString_Log_BootAdvanced)
                locString_Log_BootSuccess = readIni(ldi.FullName, "Log", "locString_Log_BootSuccess", SB, constString_Log_BootSuccess)
                locString_Log_CheckingForBaseUpdates = readIni(ldi.FullName, "Log", "locString_Log_CheckingForBaseUpdates", SB, constString_Log_CheckingForBaseUpdates)
                locString_Log_CheckingForCPUpdates = readIni(ldi.FullName, "Log", "locString_Log_CheckingForCPUpdates", SB, constString_Log_CheckingForCPUpdates)
                locString_Log_CheckingForUpdates = readIni(ldi.FullName, "Log", "locString_Log_CheckingForUpdates", SB, constString_Log_CheckingForUpdates)
                locString_Log_CPDetected = readIni(ldi.FullName, "Log", "locString_Log_CPDetected", SB, constString_Log_CPDetected)
                locString_Log_CPNotDetected = readIni(ldi.FullName, "Log", "locString_Log_CPNotDetected", SB, constString_Log_CPNotDetected)
                locString_Log_CPUnknown = readIni(ldi.FullName, "Log", "locString_Log_CPUnknown", SB, constString_Log_CPUnknown)
                locString_Log_CreateFolder = readIni(ldi.FullName, "Log", "locString_Log_CreateFolder", SB, constString_Log_CreateFolder)
                locString_Log_DeleteEmptyFolder = readIni(ldi.FullName, "Log", "locString_Log_DeleteEmptyFolder", SB, constString_Log_DeleteEmptyFolder)
                locString_Log_DeleteFile = readIni(ldi.FullName, "Log", "locString_Log_DeleteFile", SB, constString_Log_DeleteFile)
                locString_Log_DeleteFullFolder = readIni(ldi.FullName, "Log", "locString_Log_DeleteFullFolder", SB, constString_Log_DeleteFullFolder)
                locString_Log_DetectedNABLangEnglish = readIni(ldi.FullName, "Log", "locString_Log_DetectedNABLangEnglish", SB, constString_Log_DetectedNABLangEnglish)
                locString_Log_DetectedNABLangGerman = readIni(ldi.FullName, "Log", "locString_Log_DetectedNABLangGerman", SB, constString_Log_DetectedNABLangGerman)
                locString_Log_DetectedNABLangItalian = readIni(ldi.FullName, "Log", "locString_Log_DetectedNABLangItalian", SB, constString_Log_DetectedNABLangItalian)
                locString_Log_DetectingCP = readIni(ldi.FullName, "Log", "locString_Log_DetectingCP", SB, constString_Log_DetectingCP)
                locString_Log_DetectingEngine = readIni(ldi.FullName, "Log", "locString_Log_DetectingEngine", SB, constString_Log_DetectingEngine)
                locString_Log_DownloadingNewVersion = readIni(ldi.FullName, "Log", "locString_Log_DownloadingNewVersion", SB, constString_Log_DownloadingNewVersion)
                locString_Log_DownloadNewFile = readIni(ldi.FullName, "Log", "locString_Log_DownloadNewFile", SB, constString_Log_DownloadNewFile)
                locString_Log_DownloadUpdatedFile = readIni(ldi.FullName, "Log", "locString_Log_DownloadUpdatedFile", SB, constString_Log_DownloadUpdatedFile)
                locString_Log_EngineError = readIni(ldi.FullName, "Log", "locString_Log_EngineError", SB, constString_Log_EngineError)
                locString_Log_EngineImproved = readIni(ldi.FullName, "Log", "locString_Log_EngineImproved", SB, constString_Log_EngineImproved)
                locString_Log_EngineStandard = readIni(ldi.FullName, "Log", "locString_Log_EngineStandard", SB, constString_Log_EngineStandard)
                locString_Log_EngineUnknown = readIni(ldi.FullName, "Log", "locString_Log_EngineUnknown", SB, constString_Log_EngineUnknown)
                locString_Log_ErrorChecking = readIni(ldi.FullName, "Log", "locString_Log_ErrorChecking", SB, constString_Log_ErrorChecking)
                locString_Log_ErrorDownloading = readIni(ldi.FullName, "Log", "locString_Log_ErrorDownloading", SB, constString_Log_ErrorDownloading)
                locString_Log_ErrorLoadingConfig = readIni(ldi.FullName, "Log", "locString_Log_ErrorLoadingConfig", SB, constString_Log_ErrorLoadingConfig)
                locString_Log_ErrorSavingConfig = readIni(ldi.FullName, "Log", "locString_Log_ErrorSavingConfig", SB, constString_Log_ErrorSavingConfig)
                locString_Log_ExitNoUpdate = readIni(ldi.FullName, "Log", "locString_Log_ExitNoUpdate", SB, constString_Log_ExitNoUpdate)
                locString_Log_ExitMidUpdate = readIni(ldi.FullName, "Log", "locString_Log_ExitMidUpdate", SB, constString_Log_ExitMidUpdate)
                locString_Log_FoundUpdate = readIni(ldi.FullName, "Log", "locString_Log_FoundUpdate", SB, constString_Log_FoundUpdate)
                locString_Log_FoundUpdates = readIni(ldi.FullName, "Log", "locString_Log_FoundUpdates", SB, constString_Log_FoundUpdates)
                locString_Log_IniNotLocated = readIni(ldi.FullName, "Log", "locString_Log_IniNotLocated", SB, constString_Log_IniNotLocated)
                locString_Log_InstanceUpdater = readIni(ldi.FullName, "Log", "locString_Log_InstanceUpdater", SB, constString_Log_InstanceUpdater)
                locString_Log_LangNotSet = readIni(ldi.FullName, "Log", "locString_Log_LangNotSet", SB, constString_Log_LangNotSet)
                locString_Log_LangNotSetDef = readIni(ldi.FullName, "Log", "locString_Log_LangNotSetDef", SB, constString_Log_LangNotSetDef)
                locString_Log_LangSet = readIni(ldi.FullName, "Log", "locString_Log_LangSet", SB, constString_Log_LangSet)
                locString_Log_LogClose = readIni(ldi.FullName, "Log", "locString_Log_LogClose", SB, constString_Log_LogClose)
                locString_Log_LogOpen = readIni(ldi.FullName, "Log", "locString_Log_LogOpen", SB, constString_Log_LogOpen)
                locString_Log_NewVersion = readIni(ldi.FullName, "Log", "locString_Log_NewVersion", SB, constString_Log_NewVersion)
                locString_Log_NoNewVersion = readIni(ldi.FullName, "Log", "locString_Log_NoNewVersion", SB, constString_Log_NoNewVersion)
                locString_Log_NoNewUpdates = readIni(ldi.FullName, "Log", "locString_Log_NoNewUpdates", SB, constString_Log_NoNewUpdates)
                locString_Log_PathChanged = readIni(ldi.FullName, "Log", "locString_Log_PathChanged", SB, constString_Log_PathChanged)
                locString_Log_RefreshFile = readIni(ldi.FullName, "Log", "locString_Log_RefreshFile", SB, constString_Log_RefreshFile)
                locString_log_ServerNoResponse = readIni(ldi.FullName, "Log", "locString_log_ServerNoResponse", SB, constString_log_ServerNoResponse)
                locString_Log_Shutdown = readIni(ldi.FullName, "Log", "locString_Log_Shutdown", SB, constString_Log_Shutdown)
                locString_Log_UpdateFile = readIni(ldi.FullName, "Log", "locString_Log_UpdateFile", SB, constString_Log_UpdateFile)
                locString_Log_UpdateServerNoResponse = readIni(ldi.FullName, "Log", "locString_Log_UpdateServerNoResponse", SB, constString_Log_UpdateServerNoResponse)
                locString_Log_UpdatesWereSuccessful = readIni(ldi.FullName, "Log", "locString_Log_UpdatesWereSuccessful", SB, constString_Log_UpdatesWereSuccessful)
                locString_Log_UpdatingFiles = readIni(ldi.FullName, "Log", "locString_Log_UpdatingFiles", SB, constString_Log_UpdatingFiles)
                locString_Log_UpdatingSelectedFiles = readIni(ldi.FullName, "Log", "locString_Log_UpdatingSelectedFiles", SB, constString_Log_UpdatingSelectedFiles)
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

    Private Sub SetLanguage(Lang As String, ReloadonFail As Boolean)
        Dim LanguageDirectory As DirectoryInfo = New DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), Lang + ".lang"))

        If ((Lang = "") Or (Lang = "International English")) Then
            Lang = "International English"
            UpdateSettings("Language", "International English")
            Log(locString_Log_LangSet.Replace("<lang>", Lang), True)
            LoadLanguageStrings("International English")
            CheckboxLanguage("International English")
        Else
            If (My.Computer.FileSystem.FileExists(LanguageDirectory.FullName)) Then
                UpdateSettings("Language", Lang)
                Log(locString_Log_LangSet.Replace("<lang>", Lang), True)
                LoadLanguageStrings(Lang)
                CheckboxLanguage(Lang)
            ElseIf (ReloadonFail) Then
                UpdateSettings("Language", "International English")
                Log(locString_Log_LangNotSetDef.Replace("<lang>", Lang), True)
                LoadLanguageStrings("International English")
                CheckboxLanguage("International English")
            Else
                Log(locString_Log_LangNotSet.Replace("<lang>", Lang), True)
            End If
        End If
    End Sub

    Private Sub InternationalEnglishDefaultToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles InternationalEnglishDefaultToolStripMenuItem.Click
        SetLanguage("International English", False)
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