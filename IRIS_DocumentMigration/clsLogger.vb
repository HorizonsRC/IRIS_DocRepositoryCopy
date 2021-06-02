Imports System.IO

Public Class clsLogger
    Private myFolder As String
    Private myFilename As String
    Private myWriter As StreamWriter
    Private myDay As Integer                    'Current day of month
    Private Const MaxLength As Long = 5 * 1024 * 1024

    Public Sub New(ByVal Filename As String)
        myFilename = Filename
        StartLog()
    End Sub

    Public Sub Dispose()
        StopLog()
    End Sub

    Private Sub StartLog()
        myFolder = Path.GetDirectoryName(myFilename)
        If IO.Directory.Exists(myFolder) Then
            myWriter = New StreamWriter(myFilename, True)
            myWriter.AutoFlush = True
        End If
        myDay = Now.Day
    End Sub

    Private Sub StopLog()
        myWriter.Close()
        myWriter.Dispose()
        myWriter = Nothing

        Dim myInfo As New FileInfo(myFilename)                                              'Handle archiving
        If myInfo.Length > MaxLength Then                                                   'If our current archive exceeeds the maximum size
            Dim i As Integer
            Dim MaxIndex As Integer
            Dim myArchive As String = myFolder & "\Archive"
            Dim ShortName As String
            Dim ThisFile As String
            Dim ThatFile As String

            If Not IO.Directory.Exists(myArchive) Then                                      'Make sure the archive directory exists
                IO.Directory.CreateDirectory(myArchive)
            End If
            ShortName = Path.GetFileNameWithoutExtension(myFilename)

            MaxIndex = 0
            For i = 0 To 10                                                                 'Work out how many archive files exist already
                ThatFile = myArchive & "\" & ShortName & Right("00" & i, 2) & ".log"        'Identify the maximum index in our archive sequence
                If IO.File.Exists(ThatFile) Then MaxIndex = i
            Next
            If MaxIndex = 10 Then                                                           'If there are 10 items in the index it's full
                ThisFile = myArchive & "\" & ShortName & "00.log"                           'Delete the oldest archive
                If IO.File.Exists(ThisFile) Then IO.File.Delete(ThisFile)
                For i = 1 To 10                                                             'Rename each archive to the next older one
                    ThatFile = myArchive & "\" & myFilename & Right("00" & i, 2) & ".log"
                    If IO.File.Exists(ThatFile) Then IO.File.Move(ThatFile, ThisFile)
                    ThisFile = ThatFile
                Next
            Else
                MaxIndex = MaxIndex + 1
            End If
            IO.File.Move(myFilename, myArchive & "\" & ShortName & Right("00" & MaxIndex, 2) & ".log") 'Finally move our current log file into the archive
        End If
        myInfo = Nothing
    End Sub

    Public Property Filename() As String
        Get
            Filename = myFilename
        End Get
        Set(ByVal value As String)
            myFilename = value
        End Set
    End Property

    Public ReadOnly Property Writer() As StreamWriter
        Get
            Writer = myWriter
        End Get
    End Property

    Public Sub AddLog(ByVal Text As String, Optional ByVal Separator As Boolean = False)

        If Now.Day <> myDay Then                                                    'If the date has changed to the next day, place as date separator
            AddSeparator(True)
            myDay = Now.Day                                                         'And remember the new day
        End If

        If Separator Then                                                           'If a separator requested, add it
            AddSeparator(True)
        End If
        myWriter.WriteLine(GetLogTime() & Text)                                     'Write the log text
    End Sub

    Public Sub AddSeparator(Optional ByVal WithDate As Boolean = False)
        Dim myFont As New Font("Tahoma", 8)
        Dim d As Integer
        Dim G2 As Graphics
        Dim s1 As String
        Dim s2 As String
        Dim w1 As SizeF
        Dim w2 As SizeF
        Dim n As Integer
        Dim m As Integer

        G2 = frmMain.CreateGraphics                     'This next bit attempts to get the two strings to be the same length (no reason except it looks tidier)

        n = 50                                          'Length of string (character count)
        s1 = New String("─", n)                         'Make a string of n single-dash characters
        w1 = G2.MeasureString(s1, myFont, 2000)         'Measure it using the Tahoma font

        m = n                                           'This is how many of the double-dash characters are needed to be the same length
        d = 0                                           'Shrinking / enlarging factor
        Do
            m = m + d                                   'Add the shrinking/enlarging factor
            s2 = New String("═", m)                     'Create a string of m double-dash characters
            w2 = G2.MeasureString(s2, myFont, 2000)     'Measure it's width
            Select Case w2.Width - w1.Width             'Compare to the other string
                Case Is < 0                             'Shorter - need to enlarge it
                    If d = -1 Then
                        Exit Do                         'Except if we were previously shrinking (we're done in that case)
                    End If
                    d = 1
                Case Is > 0                             'Longer - need to shrink it
                    If d = 1 Then
                        Exit Do                         'Except if we were previously enlarging (we're done in that case)
                    End If
                    d = -1
                Case Is = 0                             'Same length - joy!, we're done
                    Exit Do
            End Select
        Loop

        If WithDate Then
            myWriter.WriteLine(Format(Now, "dd MMM yyyy"))
            myWriter.WriteLine(New String("─", n))
        Else
            myWriter.WriteLine(New String("═", m))
        End If

        G2.Dispose()
        G2 = Nothing
        myFont.Dispose()
        myFont = Nothing
    End Sub

    Public Function GetCompleteLog() As String
        If IO.File.Exists(myFilename) Then
            StopLog()
            Dim sr As New StreamReader(myFilename, False)
            GetCompleteLog = sr.ReadToEnd
            sr.Close()
            sr.Dispose()
            sr = Nothing
            StartLog()
        Else
            GetCompleteLog = ""
        End If
    End Function

    Private Function GetLogTime() As String
        GetLogTime = Format(Now, "HH:mm:ss") & vbTab & vbTab
    End Function
End Class
