Imports System.Text.RegularExpressions
Imports log4net
Public Class LookupFx
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
    Shared Function aggiungizeri(ByVal numero As Integer) As String
        Select Case numero.ToString.Length
            Case 1
                Return "0" & numero.ToString
            Case 2
                Return numero.ToString
            Case Else
                Return "00"
        End Select
    End Function


    Shared Function ricercaStagioneEdEpisodio(ByVal filename As String) As StagioneEEpisodio
        Dim se As StagioneEEpisodio
        se.status = False
        Dim str As String
        'Verifico il formato SSEE
        filename = filename.Replace("264", "").Replace("265", "").Replace("720", "").Replace("1080", "")
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

        'Verifico il formato SxEE
        str = REExtractValue("[0-9][0-9]x[0-9]", filename)
        If str <> "" Then
            se.stagione = Integer.Parse(str.Substring(0, 2))
            se.episodio = Integer.Parse(str.Substring(3, 1))
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

End Class
