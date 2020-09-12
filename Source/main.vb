Imports System.IO
Imports System.Data
Imports System.Text
Imports System.Windows.Forms
Imports System.Drawing
Imports log4net
Module main
    Dim ht As Hashtable = New Hashtable
    Friend WithEvents trayicon As NotifyIcon
    Friend WithEvents cMenu As ContextMenu
    Friend WithEvents MIFileManagerMainLog As MenuItem
    Friend WithEvents MIFileManagerSubLog As MenuItem
    Friend WithEvents MIMonitorLog As New MenuItem
    Friend WithEvents MIRSSLog As New MenuItem
    Friend WithEvents mnuContSep0 As MenuItem
    Friend WithEvents mnuContSep1 As MenuItem
    Friend WithEvents MIExit As MenuItem
    Friend WithEvents MIGenerateMonitor As MenuItem
    Friend WithEvents MIExecuteFileManager As MenuItem
    Friend WithEvents MIViewDataBase As MenuItem
    Friend WithEvents MIExecuteRSS As MenuItem

    Private Sub Init()
        trayicon = New NotifyIcon()
        trayicon.Icon = New Icon(My.Application.Info.DirectoryPath + "\rename.ico")
        trayicon.Text = "SeriesManager"
        cMenu = New ContextMenu()
        'Funzionalità
        MIGenerateMonitor = New MenuItem()
        MIExecuteFileManager = New MenuItem()
        'MIExecuteRSS = New MenuItem()
        MIViewDataBase = New MenuItem()
        'Separatori
        mnuContSep0 = New MenuItem()
        mnuContSep1 = New MenuItem()
        'Log files
        MIFileManagerMainLog = New MenuItem()
        MIFileManagerSubLog = New MenuItem()
        MIMonitorLog = New MenuItem
        MIRSSLog = New MenuItem

        'Chiusura applicazione
        MIExit = New MenuItem()
        MIExecuteFileManager.Text = "Move relevant files"
        MIGenerateMonitor.Text = "Generate Monitor report"
        MIViewDataBase.Text = "Open Database"
        mnuContSep0.Text = "-"


        MIFileManagerMainLog.Text = "View FileManager-Main log"
        MIFileManagerSubLog.Text = "View FileManager-Sub log"
        MIMonitorLog.Text = "View monitor generation log"
        MIRSSLog.Text = "View RSS subtitles log"

        mnuContSep1.Text = "-"
        MIExit.Text = "&Exit"
        cMenu.MenuItems.AddRange(New MenuItem() {MIExecuteFileManager, MIGenerateMonitor, MIViewDataBase, mnuContSep0, MIFileManagerMainLog, MIFileManagerSubLog, MIMonitorLog, mnuContSep1, MIExit})
        trayicon.ContextMenu = cMenu
    End Sub
    Sub Main()
        Select Case Command()

            Case "Debug"
            Case "Monitor"
                Dim mdc As MonitorEngine = New MonitorEngine
                mdc.collectInformation()
            Case "FileManager"
                Dim fme As fileMover = New fileMover
                fme.doCheck()
                subtitlesManager.ProcessSubFiles()
            Case Else
                Init()
                trayicon.Visible = True
                Application.Run()
        End Select
    End Sub
    Private Sub MIExit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles MIExit.Click
        trayicon.Visible = False
        trayicon.Dispose()
        Application.Exit()
    End Sub

    Private Sub MIViewDataBase_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles MIViewDataBase.Click
        Dim log As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)
        Try
            System.Diagnostics.Process.Start(My.Application.Info.DirectoryPath & "\data\seriesmanager.mdb")
        Catch ex As Exception

            log.Error("Errore durante l'apertura del database; L'errore è il seguente: " & ex.Message)
            log.Error("Nome database: " & My.Application.Info.DirectoryPath & "\data\seriesmanager.mdb")
        End Try
    End Sub

    Private Sub MIExecuteFileManager_Click(sender As Object, e As System.EventArgs) Handles MIExecuteFileManager.Click
        Dim fme As fileMover = New fileMover
        fme.doCheck()
        subtitlesManager.ProcessSubFiles()
    End Sub

    Private Sub MIGenerateMonitor_Click(sender As Object, e As System.EventArgs) Handles MIGenerateMonitor.Click
        Dim log As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)
        Dim m As MonitorEngine = New MonitorEngine
        m.collectInformation()
        'Apro il report appena generato
        Try
            Process.Start(My.Application.Info.DirectoryPath & "\report\" & My.Settings.REPORT_FILE_NAME)
        Catch ex As Exception
            log.Error("Impossibile aprire il report " & My.Application.Info.DirectoryPath & "\report\" & My.Settings.REPORT_FILE_NAME)
            log.Error("L'errore è il seguente: " & ex.Message)
        End Try
    End Sub

    Private Sub MIFileManagerMainLog_Click(sender As Object, e As System.EventArgs) Handles MIFileManagerMainLog.Click
        Try
            System.Diagnostics.Process.Start(My.Application.Info.DirectoryPath & "\logs\" & My.Settings.LOG_FILEMANAGER_MAIN_FILENAME)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub MIFileManagerSubLog_Click(sender As Object, e As System.EventArgs) Handles MIFileManagerSubLog.Click
        Try
            System.Diagnostics.Process.Start(My.Application.Info.DirectoryPath & "\logs\" & My.Settings.LOG_FILEMANAGER_SUB_FILENAME)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub MIMonitorLog_Click(sender As Object, e As System.EventArgs) Handles MIMonitorLog.Click
        Try
            System.Diagnostics.Process.Start(My.Application.Info.DirectoryPath & "\logs\" & My.Settings.LOG_MONITOR_FILENAME)
        Catch ex As Exception
        End Try
    End Sub

    'Private Sub MIRSSLog_Click(sender As Object, e As System.EventArgs) Handles MIRSSLog.Click
    '    Try
    '        System.Diagnostics.Process.Start(My.Application.Info.DirectoryPath & "\logs\" & My.Settings.LOG_RSS_FILENAME)
    '    Catch ex As Exception
    '    End Try
    'End Sub
End Module

