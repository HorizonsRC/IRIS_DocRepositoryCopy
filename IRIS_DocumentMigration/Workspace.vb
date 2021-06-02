Imports System.Data
Imports System.Data.SqlClient

Public Class Workspace

    Private conn As New SqlConnection
    Private cmd As New SqlCommand
    Private blk As SqlBulkCopy

    Public Sub New(Optional ByVal ConnectTimeout As Integer = 10)
        conn.ConnectionString = "Data Source=" & My.Settings.IRIS_Server & ";Initial Catalog=master;Integrated Security=SSPI;MultipleActiveResultSets=True;Connection Timeout=" & ConnectTimeout.ToString
        conn.Open()
        cmd.Connection = conn
        cmd.CommandText = "SELECT TOP 1 [name] FROM sys.databases WHERE [name] LIKE 'IRIS%_Migration'"
        Dim db As String = cmd.ExecuteScalar()
        conn.Close()
        If Not db Is Nothing Then
            My.Settings.Work_Database = db
            My.Settings.Save()
            conn.ConnectionString = "Data Source=" & My.Settings.IRIS_Server & ";Initial Catalog=" & db & ";User=iris_user;Password=iris_user;MultipleActiveResultSets=True;Connection Timeout=" & ConnectTimeout.ToString
            conn.Open()
            cmd.Connection = conn
            blk = New SqlBulkCopy(conn, SqlBulkCopyOptions.TableLock, Nothing)
        End If
    End Sub

    Public ReadOnly Property BulkCopy() As SqlBulkCopy
        Get
            Return blk
        End Get
    End Property

    Public Property CommandText() As String
        Get
            Return cmd.CommandText
        End Get
        Set(ByVal value As String)
            cmd.CommandText = value
        End Set
    End Property

    Public Function ExecuteNonQuery(Optional ByVal CommandText As String = Nothing) As Integer

        If Not CommandText Is Nothing Then cmd.CommandText = CommandText
        Return cmd.ExecuteNonQuery()
    End Function

    Public Function ExecuteScalar(Optional ByVal CommandText As String = Nothing) As Object
        If Not CommandText Is Nothing Then cmd.CommandText = CommandText
        Return cmd.ExecuteScalar()
    End Function

    Public Overloads Function ExecuteReader(Optional ByVal CommandText As String = Nothing) As SqlDataReader
        If Not CommandText Is Nothing Then cmd.CommandText = CommandText
        Return cmd.ExecuteReader()
    End Function

    Public Overloads Function ExecuteReader(ByVal Behaviour As System.Data.CommandBehavior, Optional ByVal CommandText As String = Nothing) As SqlDataReader
        If Not CommandText Is Nothing Then cmd.CommandText = CommandText
        Return cmd.ExecuteReader(Behaviour)
    End Function

    Public Sub Fill(ByRef DT As DataTable, ByVal SQL As String)
        Using da As New SqlDataAdapter(SQL, conn)
            da.SelectCommand.CommandTimeout = 120
            da.Fill(DT)
        End Using
    End Sub
    Public Sub Dispose()
        cmd.Dispose()
        cmd = Nothing
        conn.Close()
        conn = Nothing
    End Sub

End Class
