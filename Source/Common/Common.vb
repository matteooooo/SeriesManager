Imports System.IO
Public Class Common
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

    Shared Function seriefileNameCreator(SerieName As String, Season As Integer, Episode As Integer) As String
        Dim strSeason As String = Season.ToString
        Dim strEpisode As String = Episode.ToString
        If strSeason.ToString.Length = 1 Then
            strSeason = 0 & strSeason
        End If
        If strEpisode.ToString.Length = 1 Then
            strEpisode = 0 & strEpisode
        End If
        Return SerieName & " - " & strSeason & "x" & strEpisode & ".srt"
    End Function


End Class
