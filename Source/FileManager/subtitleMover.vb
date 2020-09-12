Imports System.IO
Imports log4net
Imports Ionic.Zip
Public Class subtitleMover

    Shared Sub getfiles()
        'Dim log As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)
        Dim log As ILog = LogManager.GetLogger("FileManagerSubs")
        Dim di As New IO.DirectoryInfo(My.Settings.SUBS_DOWNLOAD_DIR)
        'Dim fileList As IO.FileInfo() = di.GetFiles("*.zip")

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
                fi.MoveTo(My.Settings.SUBS_WORKING_DIR & fi.Name)
                log.Info("File -> " & fi.Name & " - Archivio spostato")
            Else
                log.Debug("File -> " & fi.Name & " - Il file non è un archivio contenente sottotitoli")
            End If

        Next
    End Sub

    Shared Sub extractfiles()
        Dim log As ILog = LogManager.GetLogger("FileManagerSubs")
        Dim di As New IO.DirectoryInfo(My.Settings.SUBS_WORKING_DIR)
        Dim fileList As IO.FileInfo() = di.GetFiles("*.zip")
        Dim fi As IO.FileInfo
        For Each fi In fileList
            Dim ZipToUnpack As String = fi.FullName
            Dim UnpackDirectory As String = My.Settings.SUBS_WORKING_DIR
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
        Next
    End Sub

    Shared Sub movefiles()
        Dim log As ILog = LogManager.GetLogger("FileManagerSubs")
        Dim di As New IO.DirectoryInfo(My.Settings.SUBS_WORKING_DIR)
        Dim fileList As IO.FileInfo() = di.GetFiles("*.srt")
        Dim fi As IO.FileInfo
        For Each fi In fileList
            log.Debug("Elaboro file " & fi.Name)
            If fi.Length > 500 Then
                If fi.Name.IndexOf("720") > 0 Then
                    log.Debug("Il file " & fi.Name & " contiene la stringa 720 percui non sarà elaborato")
                    Try
                        fi.MoveTo(My.Settings.SUBS_WORKING_DIR & "\" & fi.Name & ".720")
                        log.Debug("File " & fi.Name & " rinominato come " & fi.Name & ".720")
                    Catch IOex As IOException
                        Try
                            log.Error("Errore durante la rinomina del file ")
                            fi.Delete()
                            log.Debug("Errore durante la cancellazione del file ")
                        Catch ex As Exception
                            log.Error("Errore durante la cancellazione del file " & fi.Name)
                        End Try
                    End Try
                Else
                    fi.MoveTo(My.Settings.SOURCE_DIR & "\" & fi.Name)
                    log.Info("Spostato file " & fi.Name & " nella cartella " & My.Settings.SOURCE_DIR)
                End If
            Else
                fi.Delete()
            End If
        Next
    End Sub
    Shared Sub movearchives()
        Dim log As ILog = LogManager.GetLogger("FileManagerSubs")
        Dim di As New IO.DirectoryInfo(My.Settings.SUBS_WORKING_DIR)
        Dim fileList As IO.FileInfo() = di.GetFiles("*.zip")
        log.Debug("Trovati " & fileList.Length & " file")
        Dim fi As IO.FileInfo
        For Each fi In fileList
            Try
                fi.MoveTo(My.Settings.SUBS_WORKING_DIR & "\hist\" & fi.Name)
                log.Debug("Spostato archivio " & fi.Name & " nella cartella hist")
            Catch e As IOException
                If File.Exists(My.Settings.SUBS_WORKING_DIR & "\hist\" & fi.Name) Then
                    log.Debug("File " & My.Settings.SUBS_WORKING_DIR & "\hist\" & fi.Name & " già esistente")
                    fi.Delete()
                    log.Debug("File " & fi.Name & " cancellato")
                End If
            End Try

        Next
    End Sub

End Class
