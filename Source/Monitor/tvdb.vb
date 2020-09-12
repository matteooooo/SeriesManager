Imports System.IO
Imports HtmlAgilityPack
Imports log4net
Public Class tvdb
    Dim c As List(Of serieinfo)
    Private Structure serieinfo
        Dim season As Integer
        Dim episode As Integer
        Dim airDate As Date
    End Structure
    Sub New(ByVal seriename As String, ByVal serielink As String)
        Dim log As ILog = LogManager.GetLogger("MonitorMain")
        c = New List(Of serieinfo)
        log.Debug("Avviata procedura tvdb.getEpisodeList per la serie " & seriename)
        GetEpisodesList(seriename, serielink)
        log.Debug("Avviata procedura tvdb.extractEpisodeList per la serie " & seriename)
        extractEpisodeList(seriename)
        log.Debug("Avviata procedura tvdb.insertintotable per la serie " & seriename)
        InsertIntoTable(seriename)
    End Sub
    Private Sub GetEpisodesList(ByVal seriename As String, ByVal serielink As String)
        Dim coll As Collection = New Collection
        Dim log As ILog = LogManager.GetLogger("MonitorMain")
        log.Debug("Avviata analisi elenco di episodi per la serie " & seriename)
        Dim sw As StreamWriter = New StreamWriter(My.Application.Info.DirectoryPath & "\temp\" & seriename & ".tvdb.list")
        Dim c As Collection = New Collection
        Dim hw As HtmlWeb = New HtmlWeb
        Dim hd As HtmlDocument = hw.Load(serielink)
        Dim i As Integer = 0
        Dim htmlsource As String = hd.DocumentNode.InnerHtml

        htmlsource = htmlsource.Replace(ControlChars.Cr, "")
        htmlsource = htmlsource.Replace(ControlChars.Lf, "")
        htmlsource = htmlsource.Replace(ControlChars.CrLf, "")
        htmlsource = htmlsource.Replace(ControlChars.Tab, "")
        htmlsource = htmlsource.Replace(ControlChars.NullChar, "")
        htmlsource = htmlsource.Replace(" &nbsp;", "")

        'sw.Write(htmlsource)

        hd.LoadHtml(htmlsource)
        Dim nl As HtmlNodeCollection = hd.DocumentNode.SelectNodes("//table[@id='listtable']//tr")
        i = 1
        Dim strDate As String = ""
        Dim Episodio As String = ""
        Dim titolo As String = ""
        For Each nod In nl.Nodes
            Select Case i Mod 4
                Case 0
                    sw.WriteLine(Episodio & "|" & titolo & "|" & strDate)
                    log.Debug("Trovato episodio. Episodio: " & Episodio & " - Titolo: " & titolo & " - Data trasmissione: " & strDate)
                Case 1
                    Episodio = nod.InnerText.Replace(" x ", "x")
                Case 2
                    titolo = nod.InnerText
                Case 3
                    strDate = nod.InnerText
            End Select
            i = i + 1
        Next
        sw.Close()
        sw.Dispose()
    End Sub
    Private Sub extractEpisodeList(ByVal seriename As String)
        Dim log As ILog = LogManager.GetLogger("MonitorMain")
        Dim sr As StreamReader = New StreamReader(My.Application.Info.DirectoryPath & "\temp\" & seriename & ".tvdb.list")
        Dim si As serieinfo
        Do While Not sr.EndOfStream
            Dim row As String = sr.ReadLine()
            Try
                Dim info() As String = row.Split(Char.Parse("|"))
                If info(0).IndexOf("x") > 0 Then
                    Dim episodeseason As Integer = Integer.Parse(info(0).Substring(0, info(0).IndexOf("x")))
                    Dim episodenumber As Integer = Integer.Parse(info(0).Substring(info(0).IndexOf("x") + 1, info(0).Length - info(0).IndexOf("x") - 1))
                    If info(2).Length > 0 Then
                        Dim airDate As Date = Date.Parse(info(2))
                        si.season = episodeseason
                        si.episode = episodenumber
                        si.airDate = airDate
                        c.Add(si)
                    End If
                End If
            Catch ex As Exception
            End Try
        Loop
        sr.Close()
        sr.Dispose()

    End Sub
    Private Function date2date(ByVal dateIn As String) As Date
        Dim day As Integer = Integer.Parse(dateIn.Substring(0, 2))
        Dim year As Integer = Integer.Parse(dateIn.Substring(7, 2))
        If year > 70 Then
            year = year + 1900
        Else
            year = year + 2000
        End If
        Dim month As Integer = 0
        Select Case dateIn.Substring(3, 3).ToLower
            Case "jan"
                month = 1
            Case "feb"
                month = 2
            Case "mar"
                month = 3
            Case "apr"
                month = 4
            Case "may"
                month = 5
            Case "jun"
                month = 6
            Case "jul"
                month = 7
            Case "aug"
                month = 8
            Case "sep"
                month = 9
            Case "oct"
                month = 10
            Case "nov"
                month = 11
            Case "dec"
                month = 12
        End Select
        Return New Date(year, month, day)
    End Function

    Sub InsertIntoTable(ByVal SerieName As String)
        Dim tsH As table_HIS_SERIES_CALENDAR = New table_HIS_SERIES_CALENDAR
        tsH.deleteTable(SerieName)
        For i = 0 To c.Count - 1
            tsH.InsertIntoTable(SerieName, c.Item(i).season, c.Item(i).episode, c.Item(i).airDate, seriefileNameCreator(SerieName, c.Item(i).season, c.Item(i).episode))
        Next
    End Sub

    Private Function seriefileNameCreator(SerieName As String, Season As Integer, Episode As Integer) As String
        Dim strSeason As String = Season.ToString
        Dim strEpisode As String = Episode.ToString
        If strSeason.ToString.Length = 1 Then
            strSeason = 0 & strSeason
        End If
        If strEpisode.ToString.Length = 1 Then
            strEpisode = 0 & strEpisode
        End If
        Return SerieName & " - " & strSeason & "x" & strEpisode & ".???"
    End Function

End Class
