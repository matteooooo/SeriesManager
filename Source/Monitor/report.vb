Imports System.Text
Imports System.Data.OleDb
Imports System.IO
Public Class report
    Public Function createHTMLReport() As Boolean
        Dim retval As Boolean = True

        Dim cmd As OleDbCommand = New OleDbCommand("select * from HIS_SERIES_CALENDAR,REF_SERIES_LIST where HIS_SERIES_CALENDAR.serieName = REF_SERIES_LIST.serieName and HIS_SERIES_CALENDAR.serieseason >= REF_SERIES_LIST.fromseason and filename in (select replace(filename,'.srt','.???') from his_published_subtitles) and filename not in (select replace(replace(replace(filename,'.avi','.???'),'.mkv','.???'),'.mp4','.???') from HIS_VIDEO_FILES) order by HIS_SERIES_CALENDAR.seriename,serieseason, serieepisode", DBConnection.getConnection())
        Dim sb As StringBuilder = New StringBuilder
        'cmd.Parameters.Add("@today", OleDbType.Date).Value = Date.Today()
        Dim dr As OleDbDataReader = cmd.ExecuteReader
        sb.AppendLine("<html>")
        sb.AppendLine("<head>")
        sb.AppendLine("<meta http-equiv=""refresh"" content=""90"" >")
        sb.AppendLine("<link rel=""stylesheet"" type=""text/css"" href=""seriesmonitor.css"">")
        'sb.AppendLine("<title>SeriesMonitor " & Date.Now & "</title>")
        sb.AppendLine("<title>SeriesMonitor</title>")
        sb.AppendLine("</head>")
        sb.AppendLine("<body>")
        sb.AppendLine("<h2>Report generato il " & Date.Now & "</h2>")
        sb.AppendLine("<br/>")
        sb.AppendLine("<h1>Episodi mancanti</h1>")
        sb.AppendLine("<table>")
        sb.AppendLine("<tr>")
        sb.AppendLine("<th>Serie</th>")
        sb.AppendLine("<th>Stagione</th>")
        sb.AppendLine("<th>Episodio</th>")

        sb.AppendLine("<th>Pirate Bay</th>")
        sb.AppendLine("<th>Zooqle</th>")
        sb.AppendLine("<th>RAGBG</th>")
        sb.AppendLine("<th>EZTV</th>")
        sb.AppendLine("</tr>")

        If dr.HasRows Then
            While dr.Read
                sb.Append("<tr><td>")
                sb.Append(dr("REF_SERIES_LIST.seriename"))
                sb.Append("</td><td>")
                sb.Append(dr("serieSeason"))
                sb.Append("</td><td>")
                sb.Append(dr("serieEpisode"))
                sb.Append("</td><td width=""12%"">")
                Dim sb0 As StringBuilder = New StringBuilder
                sb0 = New StringBuilder
                sb0.Append("http://pirateproxy.net/search/")
                sb0.Append(dr("REF_SERIES_LIST.seriename"))
                sb0.Append(" ")
                sb0.Append("s")
                sb0.Append(addzero(Integer.Parse(dr("serieSeason").ToString)))
                sb0.Append("e")
                sb0.Append(addzero(Integer.Parse(dr("serieEpisode").ToString)))
                sb.Append("<a href=""" & sb0.ToString().Replace(" ", "%20") & """>" & "Cerca su Pirate Bay" & "</a>")
                sb.AppendLine("</td><td width=""12%"">")

                sb0 = New StringBuilder
                sb0.Append("https://zooqle.com/search?q=")
                sb0.Append(dr("REF_SERIES_LIST.seriename"))
                sb0.Append("+")
                sb0.Append("s")
                sb0.Append(addzero(Integer.Parse(dr("serieSeason").ToString)))
                sb0.Append("e")
                sb0.Append(addzero(Integer.Parse(dr("serieEpisode").ToString)))
                sb0.Append("/")
                sb.Append("<a href=""" & sb0.ToString().Replace(" ", "%20") & """>" & "Cerca su Zooqle" & "</a>")
                sb.AppendLine("</td><td width=""12%"">")


                sb0 = New StringBuilder
                sb0.Append("https://rarbg.to/torrents.php?search=")
                sb0.Append(dr("REF_SERIES_LIST.seriename").ToString.Replace(" ", "+"))
                sb0.Append("+")
                sb0.Append("s")
                sb0.Append(addzero(Integer.Parse(dr("serieSeason").ToString)))
                sb0.Append("+")
                sb0.Append("e")
                sb0.Append(addzero(Integer.Parse(dr("serieEpisode").ToString)))
                sb.Append("<a href=""" & sb0.ToString().Replace(" ", "%20") & """>" & "Cerca su RARBG" & "</a>")
                sb.AppendLine("</td><td width=""12%"">")


                sb0 = New StringBuilder
                sb0.Append("https://eztv.ag/search/")
                sb0.Append(dr("REF_SERIES_LIST.seriename"))
                sb0.Append("-")
                sb0.Append("s")
                sb0.Append(addzero(Integer.Parse(dr("serieSeason").ToString)))
                sb0.Append("e")
                sb0.Append(addzero(Integer.Parse(dr("serieEpisode").ToString)))
                sb.Append("<a href=""" & sb0.ToString().Replace(" ", "%20") & """>" & "Cerca su EZTV" & "</a>")
                sb.AppendLine("</td></tr>")




            End While
        Else
            sb.AppendLine("<tr><td colspan=""7"">Nessuna puntata mancante</td></tr>")
            retval = False
        End If
        sb.AppendLine("</table>")
        dr.Close()
        cmd = New OleDbCommand("select * from HIS_PUBLISHED_SUBTITLES,REF_SERIES_LIST where HIS_PUBLISHED_SUBTITLES.serieName = REF_SERIES_LIST.serieName and filename not in (select fileName from HIS_SRT_FILES) and filename not in (select filetargetname from HIS_SRT_TEMPORARY_FILES) and HIS_PUBLISHED_SUBTITLES.stagione >= REF_SERIES_LIST.fromseason order by HIS_PUBLISHED_SUBTITLES.seriename, stagione,episodio", DBConnection.getConnection)

        dr = cmd.ExecuteReader

        sb.AppendLine("<h1>Sottotitoli mancanti</h1>")
        sb.AppendLine("<table>")
        sb.AppendLine("<tr><th>Serie</th><th>Stagione</th><th>Episodio</th><th>Subber</th><th>Link</th></tr>")

        If dr.HasRows Then

            While dr.Read
                sb.Append("<tr>")
                sb.Append("<td>")
                sb.Append(dr("REF_SERIES_LIST.SerieName").ToString)
                sb.Append("</td>")
                sb.Append("<td>")
                sb.Append(dr("Stagione").ToString)
                sb.Append("</td>")
                sb.Append("<td>")
                sb.Append(dr("Episodio").ToString)
                sb.Append("</td>")
                sb.Append("<td>")
                sb.Append(dr("Site").ToString)
                sb.Append("</td>")
                sb.Append("<td>")
                If dr("downloadlink").ToString <> "" Then
                    sb.Append("<a href='" & dr("downloadlink").ToString & "' target='_blank'>Link</a>")
                End If
                sb.Append("</td>")
                sb.AppendLine("<tr>")
            End While

        Else
            sb.AppendLine("<tr><td colspan=""5"">Nessun sottotitolo mancante</td></tr>")

            retval = False
        End If
        sb.AppendLine("</table>")

        dr.Close()

        cmd = New OleDbCommand("select * from HIS_SERIES_CALENDAR,REF_SERIES_LIST where HIS_SERIES_CALENDAR.seriename = REF_SERIES_LIST.seriename and fromseason <> 100 and episodeairdate >= @data1 and episodeairdate <= @data2 order by episodeairdate,his_series_calendar.seriename,serieSeason,serieEpisode asc", DBConnection.getConnection())
        cmd.Parameters.Add("@data1", OleDbType.Date).Value = Date.Today.AddDays(-5)
        cmd.Parameters.Add("@data2", OleDbType.Date).Value = Date.Today.AddDays(Double.Parse(My.Settings.REPORT_FILE_NUM_OF_DAYS))
        dr = cmd.ExecuteReader
        sb.AppendLine("<h1>Prossimi episodi</h1>")
        sb.AppendLine("<table>")
        sb.AppendLine("<tr><th>Serie</th><th>Stagione</th><th>Episodio</th><th>Airdate</th></tr>")
        If dr.HasRows Then
            While dr.Read
                sb.Append("<tr><td>")
                sb.Append(dr("HIS_SERIES_CALENDAR.seriename"))
                sb.Append("</td><td>")
                sb.Append(dr("serieSeason"))
                sb.Append("</td><td>")
                sb.Append(dr("serieEpisode"))
                sb.Append("</td><td>")
                Dim app As String = Date.Parse(dr("EpisodeAirDate").ToString).ToString("dddd, dd MMMM yyyy")
                app = Replace(app, "ì", "&igrave;")
                sb.Append(app)
                sb.AppendLine("</td></tr>")
            End While
        End If
        sb.AppendLine("</table>")
        sb.AppendLine("</body></html>")
        Dim sw As StreamWriter = New StreamWriter(My.Application.Info.DirectoryPath & "\report\" & My.Settings.REPORT_FILE_NAME)
        sw.Write(sb.ToString)
        sw.Close()
        sw.Dispose()
        cmd.Dispose()
        Return retval
    End Function

    Private Function addzero(ByVal value As Integer) As String
        Dim strvalue As String = value.ToString
        If strvalue.Length = 1 Then
            Return "0" & strvalue
        Else
            Return strvalue
        End If
    End Function

End Class
