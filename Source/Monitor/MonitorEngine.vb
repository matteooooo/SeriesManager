Imports System.IO
Imports log4net
Imports System.Data.OleDb
Imports System.Text

Public Class MonitorEngine
    Public Sub collectInformation()
        Dim log As ILog = LogManager.GetLogger("MonitorMain")
        log.Debug("Carico l'elenco dei file presenti nel file system - Video e sottotitoli")
        CollectConsolidatedFiles()
        log.Info("Carico l'elenco degli episodi dal sito TVMaze")
        Dim tm As TVMaze = New TVMaze
        tm.searchForInformation()
        log.Info("Carico i sottotitoli disponibili su OpenSubTitles")
        Dim os As openSubtitles = New openSubtitles
        os.searchForNewSubtitles()
        log.Info("Creo il report")
        Dim r As report = New report
        r.createHTMLReport()
    End Sub
    Private Sub CollectConsolidatedFiles()
        Dim log As ILog = LogManager.GetLogger("MonitorMain")
        log.Debug("Cancello le tabelle HIST_SRT_FILES e HIST_VIDEO_FILES")
        table_HIS_SRT_FILES.Delete()
        table_HIS_VIDEO_FILES.Delete()
        Dim nameOfDirectory As String = My.Settings.TARGET_DIR
        Dim myDirectory As DirectoryInfo
        log.Debug("Inizio l'analisi dalla cartella " & My.Settings.TARGET_DIR)
        myDirectory = New DirectoryInfo(nameOfDirectory)
        WorkWithDirectory(myDirectory)

        table_HIS_SRT_FILES.addFakeFiles()
        table_HIS_VIDEO_FILES.addFakeFiles()
    End Sub
    Private Sub WorkWithDirectory(ByVal aDir As DirectoryInfo)
        Dim nextDir As DirectoryInfo
        WorkWithFilesInDir(aDir)
        For Each nextDir In aDir.GetDirectories
            WorkWithDirectory(nextDir)
        Next
    End Sub
    Private Sub WorkWithFilesInDir(ByVal aDir As DirectoryInfo)
        Dim aFile As FileInfo
        Dim folder As String
        For Each aFile In aDir.GetFiles()
            If aDir.FullName.IndexOf("@Staging") > 0 Then
                folder = "Staging"
            Else
                folder = "Standard"
            End If
            Select Case aFile.Extension
                Case ".srt"
                    table_HIS_SRT_FILES.Insert(aFile.FullName, aFile.Name, GetNomeSerie(aFile.Name), GetStagioneSerie(aFile.Name), GetEpisodioSerie(aFile.Name), folder)
                Case ".mp4", ".mkv", ".avi"
                    table_HIS_VIDEO_FILES.Insert(aFile.FullName, aFile.Name, GetNomeSerie(aFile.Name), GetStagioneSerie(aFile.Name), GetEpisodioSerie(aFile.Name), folder)
            End Select
        Next
    End Sub
    Shared Function GetNomeSerie(ByVal Name As String) As String
        Try
            Dim nomeSerie As String = Name.Substring(0, Name.IndexOf("-") - 1).Trim
            Return nomeSerie
        Catch e As Exception
            Return ""
        End Try
    End Function
    Private Function GetStagioneSerie(ByVal Name As String) As Integer
        Try
            Dim app As String = Name.Substring(Name.IndexOf("-") + 1, Name.Length - (Name.IndexOf("-") + 1)).Trim
            app = app.Substring(0, app.IndexOf("."))
            app = app.Substring(0, app.IndexOf("x"))
            Dim iapp As Integer = Integer.Parse(app.Trim)
            Return iapp
        Catch e As Exception
            Return -1
        End Try
    End Function
    Private Function GetEpisodioSerie(ByVal Name As String) As Integer
        Try
            Dim app As String = Name.Substring(Name.IndexOf("-") + 1, Name.Length - (Name.IndexOf("-") + 1)).Trim
            app = app.Substring(0, app.IndexOf("."))
            app = app.Substring(app.IndexOf("x") + 1, app.Length - app.IndexOf("x") - 1)
            Dim iapp As Integer = Integer.Parse(app.Trim)
            Return iapp
        Catch e As Exception
            Return -1
        End Try
    End Function
End Class
