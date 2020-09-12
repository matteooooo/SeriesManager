Imports log4net
Imports System.IO
Public Class fileMover
    Private Structure serie
        Dim serieName As String
        Dim serieRegExp As String
        Dim seriePath As String
    End Structure
    Private serieList As List(Of serie)
    'Shared ds As DataSet
    Public Sub doCheck()
        Dim log As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)
        Dim counter As Integer = 0
        'Carico la collection con le informazioni delle serie
        'LoadSerieList()
        log.Debug("Avviata procedura")
        LoadSerieList()
        Try
            'subtitleMover.getfiles()
            'subtitleMover.extractfiles()
            'subtitleMover.movefiles()
            'subtitleMover.movearchives()
            fileMover.elaboratefileInStagingArea()
        Catch ex As Exception
            log.Error("Errore -> " & ex.Message)
        End Try
        copyFileFromSubdirectory()
        ElabSerieFiles(True)
    End Sub

    'Public Sub XXXdoCheck2()
    '    Dim log As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)
    '    Dim counter As Integer = 0
    '    'Carico la collection con le informazioni delle serie
    '    'LoadSerieList()
    '    log.Debug("Avviata procedura")
    '    LoadSerieList()
    '    Try
    '        subtitleMover.getfiles()
    '        subtitleMover.extractfiles()
    '        subtitleMover.movefiles()
    '        subtitleMover.movearchives()
    '        fileMover.elaboratefileInStagingArea()
    '    Catch ex As Exception
    '        log.Error("Errore -> " & ex.Message)
    '    End Try
    '    copyFileFromSubdirectory()
    '    ElabSerieFiles(True)
    'End Sub



    Public Sub LoadSerieList()
        Dim log As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)
        Dim cmd As OleDb.OleDbCommand = New OleDb.OleDbCommand("select * from REF_series_LIST order by seriename", DBConnection.getConnection)
        Dim dr As OleDb.OleDbDataReader = cmd.ExecuteReader
        log.Debug("Inizio caricamento attributi delle serie")
        serieList = New List(Of serie)
        Dim seriePath As String = String.Empty
        If dr.HasRows Then
            While dr.Read
                Dim s As serie
                s.serieName = dr("seriename").ToString
                s.serieRegExp = dr("regexp").ToString
                Select Case dr("directory").ToString.ToUpper
                    Case "STAGING"
                        seriePath = My.Settings.TARGET_DIR & "\@Staging"
                    Case "STANDARD"
                        seriePath = My.Settings.TARGET_DIR & "\" & dr("seriename").ToString
                End Select
                s.seriePath = seriePath
                serieList.Add(s)
            End While
        End If
        log.Debug("Caricamento caricamento attributi delle serie completato")
    End Sub

    Private Sub ElabSerieFiles(ByVal newMethod As Boolean)
        Dim iteraction = 0
        Dim log As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)
        If Not Directory.Exists(My.Settings.SOURCE_DIR) Then
            log.Error("Directory " & My.Settings.SOURCE_DIR & " non esistente.")
            Exit Sub
        End If
        Dim di As New IO.DirectoryInfo(My.Settings.SOURCE_DIR)
        log.Debug("Ricerco nuovi file")
        Dim fileList As IO.FileInfo() = di.GetFiles()
        log.Debug("Trovati " & fileList.Length & " file")
        Dim fi As IO.FileInfo
        For Each fi In fileList
            log.Debug("Elaboro file -> " & fi.Name)
            Dim estensione As String = fi.Name.Substring(fi.Name.Length - 3, 3).ToLower
            Select Case estensione
                Case "avi", "mp4", "mkv", "srt"
                    If Not FileInUse(fi.FullName) Then
                        Dim Serie As String = RicercaSerie(fi.Name)
                        Dim ep As LookupFx.StagioneEEpisodio
                        ep = LookupFx.ricercaStagioneEdEpisodio(fi.Name)
                        If Serie <> "" And ep.stagione <> 0 And ep.episodio <> 0 Then
                            Dim path As String = RicercaPath(fi.Name)
                            Dim f As New FileInfo(fi.FullName)
                            If Not Directory.Exists(path) Then
                                Directory.CreateDirectory(path)
                            End If
                            Dim NewFileName As String = Serie & " - " & LookupFx.aggiungizeri(ep.stagione) & "x" & LookupFx.aggiungizeri(ep.episodio) & "." & estensione.ToLower
                            Dim NewFileNameTimestamp As String = Serie & " - " & LookupFx.aggiungizeri(ep.stagione) & "x" & LookupFx.aggiungizeri(ep.episodio) & "_" & Date.Now.ToString("yyyyMMddhhmmss") & "." & estensione.ToLower
                            If path <> "" Then
                                Dim NewFilePath As String = path & "\" & NewFileName
                                If Not FileInUse(fi.FullName) Then
                                    Try
                                        If Not File.Exists(path & "\" & NewFileName) Then
                                            f.MoveTo(path & "\" & NewFileName)
                                            log.Info("File " & fi.Name & " spostato -> " & path & "\" & NewFileName)
                                        Else
                                            f.MoveTo(path & "\" & NewFileNameTimestamp)
                                            log.Info("File " & fi.Name & " spostato -> " & path & "\" & NewFileNameTimestamp)

                                            'If Not File.Exists(My.Settings.SOURCE_DIR & "\temp\" & NewFileName) Then
                                            'f.MoveTo(My.Settings.SOURCE_DIR & "\temp\" & NewFileName)
                                            'log.Warn("File " & fi.Name & " spostato in TEMP perchè già presente nella directory di destinazione")
                                            'Else
                                            'f.Delete()
                                            'log.Warn("File " & fi.Name & " cancellato perchè già presente nella directory TEMP")
                                        End If
                                    Catch ex As Exception
                                        log.Error("File " & fi.Name & " - " & ex.Message.Replace(ControlChars.CrLf, ""))
                                    End Try
                                Else
                                    log.Debug("Il file " & fi.Name & " è bloccato da un altro processo")
                                End If
                            Else
                                log.Debug("Nuovo path non rilevabile")
                            End If
                        Else
                            If Serie = "" Then
                                log.Debug("Serie non rilevabile")
                            End If
                            If ep.stagione = 0 Then
                                log.Debug("Stagione non rilevabile")
                            End If
                            If ep.episodio = 0 Then
                                log.Debug("Episodio non rilevabile")
                            End If
                            'Procedo rinominando il file aggiungendo l'estensione .unknown
                            'fi.MoveTo(fi.FullName & ".unknown")
                            'log.Debug("File " & fi.Name & " rinominato aggiungendo l'estensione .unknown")
                        End If
                    Else
                        log.Debug("Il file " & fi.Name & " è lockato da un'altra applicazione")
                    End If
                Case Else
                    log.Debug("File " & fi.Name & " ha un estensione non prevista")
            End Select
        Next
    End Sub

    Private Function RicercaSerie(ByVal filename As String) As String
        Dim log As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)
        log.Debug("Ricerco entry per il seguente nome file -> " & filename)
        For i = 0 To serieList.Count - 1
            log.Debug("Confronto con la seguente regex -> " & serieList.Item(i).serieRegExp)
            If LookupFx.REMatchValue(serieList.Item(i).serieRegExp, filename) Then
                log.Debug("Trovato matching")
                Return serieList.Item(i).serieName
            End If
        Next
        Return ""
    End Function
    Private Function RicercaPath(ByVal filename As String) As String
        For i = 0 To serieList.Count - 1
            If LookupFx.REMatchValue(serieList.Item(i).serieRegExp, filename) Then
                Dim Path As String = serieList.Item(i).seriePath
                Return Path
            End If
        Next
        Return ""
    End Function


    Private Sub checkAndCopy(ByVal directoryName As String)
        Dim log As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString & "." & System.Reflection.MethodBase.GetCurrentMethod().Name)
        Dim di As New IO.DirectoryInfo(directoryName)
        For Each fi In di.GetFiles
            Dim estensione As String = fi.Name.Substring(fi.Name.Length - 3, 3).ToLower
            Select Case estensione
                Case "avi", "mp4", "mkv"
                    log.Debug("Trovato file video -> " & fi.Name)
                    If Not FileInUse(fi.FullName) Then
                        Dim Serie As String = RicercaSerie(fi.Name)
                        If Serie <> "" Then
                            log.Debug("Il file " & fi.Name & " sembra essere relativo alla serie televisiva" & Serie)
                            Try
                                fi.MoveTo(My.Settings.SOURCE_DIR & "\" & fi.Name)
                                log.Debug("File " & fi.Name & " spostato nella cartella " & My.Settings.SOURCE_DIR)
                                'dopo aver spostato il file procedo a cancellare la cartella 
                                System.IO.Directory.Delete(directoryName, True)
                            Catch ex As Exception
                                log.Error("Errore durante lo spostamento del file " & fi.Name)
                                log.Error("Il messaggio di errore è il seguente  " & ex.Message)
                                log.Error(ex.StackTrace)
                            End Try
                        Else
                            log.Debug("Il file " & fi.Name & " sembra non essere relativo ad una serie televisiva")
                        End If
                    Else
                        log.Debug("Il file " & fi.Name & " è bloccato da un altro processo")
                    End If
            End Select
        Next
    End Sub

    Private Sub copyFileFromSubdirectory()
        Dim di As New IO.DirectoryInfo(My.Settings.SOURCE_DIR)
        For Each fol In di.GetDirectories
            If fol.Name <> "temp" And fol.Name <> "hist" Then
                checkAndCopy(fol.FullName)
            End If
        Next
    End Sub


    Shared Function FileInUse(ByVal sFile As String) As Boolean
        Dim thisFileInUse As Boolean = False
        If System.IO.File.Exists(sFile) Then
            Try
                Using f As New IO.FileStream(sFile, FileMode.Open, FileAccess.ReadWrite, FileShare.None)
                    ' thisFileInUse = False
                End Using
            Catch
                thisFileInUse = True
            End Try
        End If
        Return thisFileInUse
    End Function
    Shared Sub elaboratefileInStagingArea()
        Dim log As ILog = LogManager.GetLogger("Staging")
        Dim VERIFIED_FOLDER As String = My.Settings.TARGET_DIR & "\@staging\verified\"
        Dim di As New IO.DirectoryInfo(VERIFIED_FOLDER)
        For Each fi In di.GetFiles
            log.Debug("Trovato file: " & fi.Name)
            Dim serie As String = fi.Name.Substring(0, fi.Name.IndexOf("-") - 1)
            log.Debug("Directory di destinazione: " & My.Settings.TARGET_DIR & "\" & serie & "\")
            If Not Directory.Exists(My.Settings.TARGET_DIR & "\" & serie) Then
                log.Debug("Directory di destinazione: " & My.Settings.TARGET_DIR & "\" & serie & "\ NON esistente.")
                log.Debug("Creazione della directory " & My.Settings.TARGET_DIR & "\" & serie)
                Directory.CreateDirectory(My.Settings.TARGET_DIR & "\" & serie)
                log.Info("Directory " & My.Settings.TARGET_DIR & "\" & serie & " creata")
            End If
            Try
                fi.MoveTo(My.Settings.TARGET_DIR & "\" & serie & "\" & fi.Name)
                log.Info("File " & fi.Name & " spostato nella cartella " & My.Settings.TARGET_DIR & "\" & serie)
            Catch ex As Exception
                log.Error("Spostamento non riuscito per il file " & fi.Name & ".L'errore è il seguente: " & ex.Message)
            End Try
        Next
    End Sub
End Class


