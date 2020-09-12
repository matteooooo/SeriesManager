Imports log4net
Imports Ionic.Zip
Imports System.IO
Public Class subtitlesManager

    Shared Sub ProcessSubFiles()
        'getSubFiles()
        'extractSubFiles()
        'processVerifiedSubFile()
    End Sub


    'Procedura per copiare i file contenenti i sottotitoli salvati nella cartella SUBS_DOWNLOAD_DIR e copiarli nella cartella <TARGET_DIR>\@subtitles
    Shared Sub getSubFiles()
        Dim log As ILog = LogManager.GetLogger("FileManagerSubs")
        Dim di As New IO.DirectoryInfo(My.Settings.SUBS_DOWNLOAD_DIR)
        Dim fileList As IO.FileInfo() = di.GetFiles.Where(Function(fx) fx.Extension = ".zip" OrElse fx.Extension = ".srt").ToArray
        Dim fi As IO.FileInfo
        Dim hasToMove As Boolean
        For Each fi In fileList
            hasToMove = False
            log.Debug("File -> " & fi.Name)
            'log.Debug("ITASA -> " & fi.Name.IndexOf("italiansubs"))
            'log.Debug("subsfactory -> " & fi.Name.IndexOf("subsfactory"))
            'log.Debug("subspedia -> " & fi.Name.ToString.IndexOf("subspedia"))
            'log.Debug("subscloud -> " & fi.Name.ToString.IndexOf("subscloud"))
            'log.Debug("MyITsubs -> " & fi.Name.ToString.IndexOf("MyITsubs"))
            log.Debug("OpenSubtitles -> " & fi.Name.ToString.IndexOf("ita.1cd"))


            'If fi.Name.IndexOf("italiansubs") > 0 Then
            '    hasToMove = True
            '    log.Debug("File -> " & fi.Name & " contiene sottotitoli creati da ITASA")
            'End If

            'If fi.Name.IndexOf("subsfactory") > 0 Then
            '    hasToMove = True
            '    log.Debug("File -> " & fi.Name & " contiene sottotitoli creati da SUBSFACTORY")
            'End If

            'If fi.Name.ToUpper.IndexOf("SUBSPEDIA") > 0 Then
            '    hasToMove = True
            '    log.Debug("File -> " & fi.Name & " contiene sottotitoli creati da SUBSPEDIA")
            'End If

            'If fi.Name.IndexOf("subscloud") > 0 Then
            '    hasToMove = True
            '    log.Debug("File -> " & fi.Name & " contiene sottotitoli creati da SUBSCLOUD")
            'End If

            'If fi.Name.IndexOf("MyITsubs") > 0 Then
            '    hasToMove = True
            '    log.Debug("File -> " & fi.Name & " contiene sottotitoli creati da MyIYSubs")
            'End If

            If fi.Name.IndexOf("ita.1cd") > 0 Then
                hasToMove = True
                log.Debug("File -> " & fi.Name & " contiene sottotitoli creati da OpenSubtitles")
            End If

            If hasToMove Then
                fi.MoveTo(My.Settings.TARGET_DIR & "\@subs\" & fi.Name)
                log.Info("File -> " & fi.Name & " - Archivio spostato")
            Else
                log.Debug("File -> " & fi.Name & " - Il file non è un archivio contenente sottotitoli")
            End If
        Next
    End Sub

    Shared Sub extractSubFiles()
        Dim log As ILog = LogManager.GetLogger("FileManagerSubs")
        Dim di As New IO.DirectoryInfo(My.Settings.TARGET_DIR & "\@subs")
        Dim fileList As IO.FileInfo() = di.GetFiles("*.zip")
        Dim fi As IO.FileInfo
        For Each fi In fileList
            Dim ZipToUnpack As String = fi.FullName
            Dim UnpackDirectory As String = My.Settings.TARGET_DIR & "\@subs"
            log.Debug("Elaboro file " & fi.Name)
            Using zip1 As ZipFile = ZipFile.Read(ZipToUnpack)
                Dim e As ZipEntry
                ' here, we extract every entry, but we could extract conditionally,
                ' based on entry name, size, date, checkbox status, etc.   
                For Each e In zip1
                    e.Extract(UnpackDirectory, ExtractExistingFileAction.OverwriteSilently)
                Next
            End Using
            log.Debug("Elaboro file " & fi.Name & " - File estratto/i")
            Try
                fi.MoveTo(My.Settings.TARGET_DIR & "\@subs\hist\" & fi.Name)
            Catch ex As Exception
                log.Error("Errore durante lo spostamento del file")
                log.Error("Source file name: " & fi.FullName)
                log.Error("Target file name: " & My.Settings.TARGET_DIR & "\@subs\hist\" & fi.Name)
                log.Error("Errore -> " & ex.Message)
            End Try
        Next
    End Sub
    Shared Sub processVerifiedSubFile()
        Dim log As ILog = LogManager.GetLogger("FileManagerSubs")
        Dim di As New IO.DirectoryInfo(My.Settings.TARGET_DIR & "\@subs\Verified")
        Dim fileList As IO.FileInfo() = di.GetFiles("*.srt")
        Dim fi As IO.FileInfo
        For Each fi In fileList
            Dim targetName As String = fileNameFormatter.formatFileName(fi.Name)
            Try
                fi.MoveTo(My.Settings.TARGET_DIR & "\@staging\" & targetName)
            Catch ex As Exception
                log.Error("Errore durante lo spostamento del file")
                log.Error("Source file name: " & fi.FullName)
                log.Error("Target file name: " & My.Settings.TARGET_DIR & "\@staging\" & targetName)
                log.Error("Errore -> " & ex.Message)
            End Try
        Next
    End Sub
End Class
