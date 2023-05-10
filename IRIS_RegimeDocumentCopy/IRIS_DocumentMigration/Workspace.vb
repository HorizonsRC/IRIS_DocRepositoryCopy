Imports System.Data
Imports System.Data.SqlClient

Public Class Workspace
    Private connIRIS As New SqlConnection
    Private connMigration As New SqlConnection

    Private dbIRIS As String
    Private dbMigration As String
    Private dbServerName As String

    Private cmdIRIS As New SqlCommand
    Private cmdMigration As New SqlCommand

    Private blk As SqlBulkCopy

    Public Sub New(ByVal ServerName As String, Optional ByVal ConnectTimeout As Integer = 10)
        dbServerName = ServerName
        dbIRIS = ""
        dbMigration = ""
        Using conn As New SqlConnection($"Data Source={ServerName};Initial Catalog=master;Integrated Security=SSPI;MultipleActiveResultSets=True;Connection Timeout={ConnectTimeout.ToString}")
            conn.Open()
            Using cmd As New SqlCommand("SELECT TOP 1 [name] FROM sys.databases WHERE [name] LIKE 'IRIS%_Migration'", conn)
                dbMigration = cmd.ExecuteScalar()
                If Not dbMigration Is Nothing Then
                    connMigration.ConnectionString = $"Data Source={ServerName};Initial Catalog={dbMigration};User=iris_user;Password=iris_user;MultipleActiveResultSets=True;Connection Timeout={ConnectTimeout.ToString}"
                    connMigration.Open()
                    cmdMigration.Connection = connMigration
                    blk = New SqlBulkCopy(connMigration, SqlBulkCopyOptions.TableLock, Nothing)
                End If
            End Using
            Using cmd As New SqlCommand("SELECT TOP 1 [name] FROM sys.databases WHERE [name] LIKE 'IRIS%' ORDER BY LEN([name])", conn)
                dbIRIS = cmd.ExecuteScalar()
                If Not dbIRIS Is Nothing Then
                    connIRIS.ConnectionString = $"Data Source={ServerName};Initial Catalog={dbIRIS};User=iris_user;Password=iris_user;MultipleActiveResultSets=True;Connection Timeout={ConnectTimeout.ToString}"
                    connIRIS.Open()
                    cmdIRIS.Connection = connIRIS
                End If
            End Using
            conn.Close()
        End Using
    End Sub
    Public ReadOnly Property ServerName() As String
        Get
            Return dbServerName
        End Get
    End Property
    Public ReadOnly Property IRISConnectionString() As String
        Get
            Return connIRIS.ConnectionString
        End Get
    End Property
    Public ReadOnly Property MigrationConnectionString() As String
        Get
            Return connMigration.ConnectionString
        End Get
    End Property
    Public ReadOnly Property IRISDatabase() As String
        Get
            Return dbIRIS
        End Get
    End Property
    Public ReadOnly Property MigrationDatabase() As String
        Get
            Return dbMigration
        End Get
    End Property
    Public ReadOnly Property BulkCopy() As SqlBulkCopy
        Get
            Return blk
        End Get
    End Property
    Public Property IRISCommandText() As String
        Get
            Return cmdIRIS.CommandText
        End Get
        Set(ByVal value As String)
            cmdIRIS.CommandText = value
        End Set
    End Property
    Public Property MigrationCommandText() As String
        Get
            Return cmdMigration.CommandText
        End Get
        Set(ByVal value As String)
            cmdMigration.CommandText = value
        End Set
    End Property
    Public Function ExecuteNonQueryIRIS(Optional ByVal CommandText As String = Nothing) As Integer

        If Not CommandText Is Nothing Then cmdIRIS.CommandText = CommandText
        Return cmdIRIS.ExecuteNonQuery()
    End Function
    Public Function ExecuteNonQueryMigration(Optional ByVal CommandText As String = Nothing) As Integer

        If Not CommandText Is Nothing Then cmdMigration.CommandText = CommandText
        Return cmdMigration.ExecuteNonQuery()
    End Function
    Public Function ExecuteScalarIRIS(Optional ByVal CommandText As String = Nothing) As Object
        If Not CommandText Is Nothing Then cmdIRIS.CommandText = CommandText
        Return cmdIRIS.ExecuteScalar()
    End Function
    Public Function ExecuteScalarMigration(Optional ByVal CommandText As String = Nothing) As Object
        Dim cmd As New SqlCommand(CommandText, connMigration)
        Dim obj As Object = cmd.ExecuteScalar()
        cmd.Dispose()
        Return obj
    End Function
    Public Overloads Function ExecuteReaderIRIS(ByVal CommandText As String) As SqlDataReader
        cmdIRIS.CommandText = CommandText
        Return cmdIRIS.ExecuteReader()
    End Function
    Public Overloads Function ExecuteReaderMigration(ByVal CommandText As String) As SqlDataReader
        cmdMigration.CommandText = CommandText
        Return cmdMigration.ExecuteReader()
    End Function
    Public Sub FillDataTableIRIS(ByRef DT As DataTable, ByVal SQL As String)
        Using da As New SqlDataAdapter(SQL, connIRIS)
            da.SelectCommand.CommandTimeout = 120
            da.Fill(DT)
        End Using
    End Sub
    Public Sub FillDataTableMigration(ByRef DT As DataTable, ByVal SQL As String)
        Using da As New SqlDataAdapter(SQL, connMigration)
            da.SelectCommand.CommandTimeout = 120
            da.Fill(DT)
        End Using
    End Sub
    Public Sub FillDatasetMigration(ByRef DS As DataSet, ByVal SQL As String)
        Dim ad As New SqlDataAdapter()
        ad.SelectCommand = New SqlCommand(SQL, connMigration)
        ad.Fill(DS)
    End Sub
    Public Sub Dispose()
        cmdIRIS.Dispose()
        cmdIRIS = Nothing
        connIRIS.Close()
        connIRIS = Nothing
        cmdMigration.Dispose()
        cmdMigration = Nothing
        connMigration.Close()
        connMigration = Nothing
    End Sub

End Class
