Imports System.IO
Imports System.Data.OleDb
Imports System.Data
Imports System.Text
Imports log4net
Public Class MonitorEnginexxx
    Public Sub execute()
        Dim log As ILog = LogManager.GetLogger("MonitorMain")
        log.Info("Procedura standard avviata")
        log.Info("Carico l'elenco degli episodi dal sito epguides.com")
        LoadListOfEpisodes()
        log.Info("Carico l'elenco degli episodi e dei sottotitoli disponibili")
        ScanFolders()
        log.Info("Verifico la disponibilità di episodi e dei sottotitoli")

        Dim sc As seriesChecker = New seriesChecker
        sc.check()

        'table_subtitles.CheckSubtitleFileExistence()

        log.Info("Creo il report con gli episodi mancanti, i sottotitoli mancanti ed il palinsesto dei prossimi sette giorni")
        Dim r As Report = New Report
        r.getMissingVideo()
        r.getMissingInfo()
        'log.Info("Creo la nuova versione del file di configurazione per l'applicazione SeriesRenamer")
        'createXMLFile()
        log.Info("Procedura standard completata")
    End Sub
    Sub LoadListOfEpisodes()
        Dim log As ILog = LogManager.GetLogger("MonitorMain")
        log.Debug("Avviata procedura per la ricerca dell'elenco degli episodi delle serie")
        Dim cmd As OleDbCommand = New OleDbCommand("select * from series where URL <> '' order by serieName", DBConnection.getConnection())
        Dim dr As OleDbDataReader = cmd.ExecuteReader
        Dim EpisodeDataBase As String = ""
        If dr.HasRows Then
            While (dr.Read)
                log.Debug("Verifico elenco di episodi per la serie -> " & dr("seriename").ToString)
                If dr("url").ToString.IndexOf("epguide") > 0 Then
                    log.Debug("Ricerco l'elenco nel sito epguide.com")
                    Dim tempFileName As String = My.Application.Info.DirectoryPath & "\temp\" & dr("SerieName").ToString & ".list"
                    If Not File.Exists(tempFileName) Then
                        log.Debug("Il file non esiste quindi ricerco i dati")
                        Dim eg As epguide = New epguide(dr("SerieName").ToString, dr("URL").ToString)
                    Else
                        If File.GetLastWriteTime(tempFileName).Date <= Date.Today.AddDays(-7) Then
                            log.Debug("Il file è più vecchio di 7 giorni quindi ricerco i dati")
                            Dim eg As epguide = New epguide(dr("SerieName").ToString, dr("URL").ToString)
                        Else
                            log.Debug("Il file è aggiornato quindi NON ricerco i dati")
                        End If
                    End If
                Else
                    log.Debug("Ricerco l'elenco nel sito thetvdb.com")
                    Dim tempFileName As String = My.Application.Info.DirectoryPath & "\temp\" & dr("SerieName").ToString & ".tvdb.list"
                    If Not File.Exists(tempFileName) Then
                        log.Debug("Il file non esiste quindi ricerco i dati")
                        Dim eg As tvdb = New tvdb(dr("SerieName").ToString, dr("URL").ToString)
                    Else
                        If File.GetLastWriteTime(tempFileName).Date <= Date.Today.AddDays(-7) Then
                            log.Debug("Il file è più vecchio di 7 giorni quindi ricerco i dati")
                            Dim eg As tvdb = New tvdb(dr("SerieName").ToString, dr("URL").ToString)
                        Else
                            log.Debug("Il file è aggiornato quindi NON ricerco i dati")
                        End If
                    End If
                End If
            End While
        End If
        cmd.Dispose()
        dr.Close()
        log.Debug("Procedura per la ricerca dell'elenco degli episodi delle serie completata")
    End Sub

    Shared Sub ScanFolders()
        table_files.Delete()
        Dim nameOfDirectory As String = My.Settings.TARGET_DIR
        Dim myDirectory As DirectoryInfo
        myDirectory = New DirectoryInfo(nameOfDirectory)
        WorkWithDirectory(myDirectory)
        'inserisco in tabella tutte le entry necessarie per evitare errate segnalazioni di mancanza episodi

        table_files.addfakefile()
    End Sub
    Shared Sub WorkWithDirectory(ByVal aDir As DirectoryInfo)
        Dim nextDir As DirectoryInfo
        WorkWithFilesInDir(aDir)
        For Each nextDir In aDir.GetDirectories
            If nextDir.FullName.IndexOf("@Staging") < 0 Then
                WorkWithDirectory(nextDir)
            End If
        Next
    End Sub
    Shared Sub WorkWithFilesInDir(ByVal aDir As DirectoryInfo)
        Dim aFile As FileInfo
        For Each aFile In aDir.GetFiles()
            table_files.Insert(aFile.FullName, aFile.Name, getVideoOrSub(aFile.Name), GetNomeSerie(aFile.Name), GetStagioneSerie(aFile.Name), GetEpisodioSerie(aFile.Name))
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
    Shared Function GetStagioneSerie(ByVal Name As String) As Integer
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
    Shared Function GetEpisodioSerie(ByVal Name As String) As Integer
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
    Shared Function getVideoOrSub(ByVal name As String) As String
        Try
            Select Case name.Substring(name.LastIndexOf(".") + 1, 3).ToLower
                Case "srt"
                    Return "S"
                Case "avi"
                    Return "V"
                Case "mp4"
                    Return "V"
                Case "mkv"
                    Return "V"
                Case Else
                    Return ""
            End Select
        Catch e As Exception
            Return ""
        End Try
    End Function

End Class
