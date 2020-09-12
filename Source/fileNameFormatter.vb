Imports log4net
Imports System.Text
Imports System.Text.RegularExpressions
Public Class fileNameFormatter
    Shared Function formatFileName(originalFileName As String) As String
        Dim Serie As String = RicercaSerie(originalFileName)
        Dim extension As String = originalFileName.Substring(originalFileName.Length - 3, 3).ToLower
        Dim sb As StringBuilder = New StringBuilder
        Dim ep As StagioneEEpisodio = ricercaStagioneEdEpisodio(originalFileName)
        If Serie <> "" And ep.stagione <> 0 And ep.episodio <> 0 Then
            sb.Append(Serie)
            sb.Append(" - ")
            If ep.stagione.ToString.Length = 1 Then
                sb.Append("0")
            End If
            sb.Append(ep.stagione.ToString)
            sb.Append("x")
            If ep.episodio.ToString.Length = 1 Then
                sb.Append("0")
            End If
            sb.Append(ep.episodio.ToString)
            sb.Append(".")
            sb.Append(extension)
            Return sb.ToString
        Else
            Return ""
        End If
    End Function
    Shared Function RicercaSerie(ByVal filename As String) As String
        Dim log As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)
        log.Debug("Ricerco entry per il seguente nome file -> " & filename)
        For i = 0 To seriesList.getList.Count - 1
            log.Debug("Confronto con la seguente regex -> " & seriesList.getList.Item(i).serieRegExp)
            If REMatchValue(seriesList.getList.Item(i).serieRegExp, filename) Then
                log.Debug("Trovato matching")
                Return seriesList.getList.Item(i).serieName
            End If
        Next
        Return ""
    End Function


    Shared Function ricercaStagioneEdEpisodio(ByVal filename As String) As StagioneEEpisodio
        Dim se As StagioneEEpisodio
        se.status = False
        Dim str As String
        'Verifico il formato SSEE
        filename = filename.Replace("264", "").Replace("265", "").Replace("720", "")
        str = REExtractValue("[0-9][0-9][0-9][0-9]", filename)
        If str <> "" Then
            se.stagione = Integer.Parse(str.Substring(0, 2))
            se.episodio = Integer.Parse(str.Substring(2, 2))
            se.status = True
            Return se
        End If

        'Verifico il formato SEE
        str = REExtractValue("[0-9][0-9][0-9]", filename)
        If str <> "" Then
            se.stagione = Integer.Parse(str.Substring(0, 1))
            se.episodio = Integer.Parse(str.Substring(1, 2))
            se.status = True
            Return se
        End If

        'Verifico il formato SxEE
        str = REExtractValue("[0-9]x[0-9][0-9]", filename)
        If str <> "" Then
            se.stagione = Integer.Parse(str.Substring(0, 1))
            se.episodio = Integer.Parse(str.Substring(2, 2))
            se.status = True
            Return se
        End If

        'Verifico il formato SSxEE
        str = REExtractValue("[0-9][0-9]x[0-9][0-9]", filename)
        If str <> "" Then
            se.stagione = Integer.Parse(str.Substring(0, 2))
            se.episodio = Integer.Parse(str.Substring(3, 2))
            se.status = True
            Return se
        End If

        'Verifico il formato sSSeEE
        str = REExtractValue("s[0-9][0-9]e[0-9][0-9]", filename)
        If str <> "" Then
            se.stagione = Integer.Parse(str.Substring(1, 2))
            se.episodio = Integer.Parse(str.Substring(4, 2))
            se.status = True
            Return se
        End If

        'Verifico(formato)  's01.e03
        str = REExtractValue("s[0-9][0-9].e[0-9][0-9]", filename)
        If str <> "" Then
            se.stagione = Integer.Parse(str.Substring(1, 2))
            se.episodio = Integer.Parse(str.Substring(5, 2))
            se.status = True
            Return se
        End If
        Return se
    End Function


    Public Structure StagioneEEpisodio
        Dim stagione As Integer
        Dim episodio As Integer
        Dim status As Boolean
    End Structure

    Shared Function REMatchValue(ByVal myPattern As String, ByVal myString As String) As Boolean
        'Create objects.
        Dim re As Regex = New Regex(myPattern, RegexOptions.IgnoreCase)
        If re.IsMatch(myString) Then
            Return True
        Else
            Return False
        End If
    End Function
    Shared Function REExtractValue(ByVal myPattern As String, ByVal myString As String) As String
        'Create objects.
        Dim re As Regex = New Regex(myPattern, RegexOptions.IgnoreCase)
        Dim sm As MatchCollection
        sm = re.Matches(myString)
        If sm.Count > 0 Then
            Return sm.Item(0).Value
        Else
            Return ""
        End If
    End Function

End Class
