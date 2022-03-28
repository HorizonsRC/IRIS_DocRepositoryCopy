Imports System.IO
Imports IWshRuntimeLibrary
Imports System.Data
Imports System.Data.SqlClient
Imports System.Security.AccessControl
Imports System.Security.Cryptography

Public Class frmMain
    Private Busy As Boolean
    Private Shared FolderID As Integer = 0
    Private Shared DM_Files As DataTable
    Private Shared Loading As Boolean

    Public Sub New()
        InitializeComponent()
    End Sub

#Region "Form Load/Unload"
    Private Sub frmMain_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        SaveSetting("IRIS_DocumentMigration", "Settings", "frmMain.WindowState", Me.WindowState)
        If Me.WindowState = FormWindowState.Normal Then
            SaveSetting("IRIS_DocumentMigration", "Settings", "frmMain.Top", Me.Top)
            SaveSetting("IRIS_DocumentMigration", "Settings", "frmMain.Left", Me.Left)
            SaveSetting("IRIS_DocumentMigration", "Settings", "frmMain.Width", Me.Width)
            SaveSetting("IRIS_DocumentMigration", "Settings", "frmMain.Height", Me.Height)
        End If

        My.Settings.SourceRootFolder = txtSourceRoot.Text
        My.Settings.IRIS_RootFolder = txtIRIS_Root.Text
        My.Settings.Work_Database = cmbWork_Database.Text
        My.Settings.IRIS_Server = txtIRIS_Server.Text
        My.Settings.IRIS_Database = cmbIRIS_Database.Text
        My.Settings.FileMigrationID = cmbFileMigrationID.Text
        My.Settings.MigrationID = txtMigrationID.Text
        My.Settings.ObjectType = cmbObjectType.Text
        My.Settings.IdentifierContext = cmbIdentifierContext.Text
        My.Settings.Save()
    End Sub

    Private Sub frmMain_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim WS As FormWindowState

        Loading = True
        WS = GetSetting("IRIS_DocumentMigration", "Settings", "frmMain.WindowState", FormWindowState.Normal)
        If WS = FormWindowState.Normal Then
            Me.Top = GetSetting("IRIS_DocumentMigration", "Settings", "frmMain.Top", Me.Top)
            Me.Left = GetSetting("IRIS_DocumentMigration", "Settings", "frmMain.Left", Me.Left)
        Else
            Me.WindowState = WS
        End If

        txtIRIS_Server.Text = My.Settings.IRIS_Server
        cmbFileMigrationID.Text = My.Settings.FileMigrationID
        cmbIRIS_Database.Text = My.Settings.IRIS_Database
        cmbWork_Database.Text = My.Settings.Work_Database
        txtMigrationID.Text = My.Settings.MigrationID
        txtSourceRoot.Text = My.Settings.SourceRootFolder
        txtIRIS_Root.Text = My.Settings.IRIS_RootFolder
        cmbObjectType.Text = My.Settings.ObjectType
        cmbIdentifierContext.Text = My.Settings.IdentifierContext

        If My.Settings.IRIS_Server.Length > 0 Then
            EnsureTables()
            VerifyMigrationID()

            cmbFileMigrationID.DataSource = GetIDList("FileMigrationID")
            For i As Integer = 0 To cmbFileMigrationID.Items.Count - 1
                If cmbFileMigrationID.Items(i) = My.Settings.FileMigrationID Then
                    cmbFileMigrationID.SelectedIndex = i
                    Exit For
                End If
            Next

            cmbObjectType.DataSource = GetIDList("ObjectType")
            For i As Integer = 0 To cmbObjectType.Items.Count - 1
                If cmbObjectType.Items(i) = My.Settings.ObjectType Then
                    cmbObjectType.SelectedIndex = i
                    Exit For
                End If
            Next

            cmbIdentifierContext.DataSource = GetIDList("IdentifierContext")
            For i As Integer = 0 To cmbIdentifierContext.Items.Count - 1
                If cmbIdentifierContext.Items(i) = My.Settings.IdentifierContext Then
                    cmbIdentifierContext.SelectedIndex = i
                    Exit For
                End If
            Next

        End If
        UpdatePix()

        Loading = False
        lblStatus.Text = "Ready"
    End Sub
#End Region

#Region "Control Events"
    Private Sub cmdBrowseR2D2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdBrowseR2D2.Click
        Dim myFolder As String

        myFolder = GetFolder("Identify the Root Folder for R2D2 Documents", My.Settings.SourceRootFolder)
        If Len(myFolder) > 0 Then
            If Len(myFolder) > 0 AndAlso IO.Directory.Exists(myFolder) Then
                If myFolder.Substring(myFolder.Length - 1, 1) <> "\" Then myFolder = myFolder & "\"
                If Not myFolder = My.Settings.SourceRootFolder Then
                    If MsgBox("A new source folder has been selected. You will need to re-build the mapping tables after this change. Continue?", vbQuestion + vbYesNo, "Confirm Change") = vbYes Then
                        txtSourceRoot.Text = myFolder
                        My.Settings.SourceRootFolder = myFolder
                        UpdateConfig()
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub cmdBrowseIRIS_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdBrowseIRIS.Click
        Dim myFolder As String

        myFolder = GetFolder("Identify a Root Folder for IRIS Documents", My.Settings.IRIS_RootFolder)
        If myFolder.Substring(myFolder.Length - 1, 1) <> "\" Then myFolder = myFolder & "\"
        If Len(myFolder) > 0 AndAlso IO.Directory.Exists(myFolder) Then
            txtIRIS_Root.Text = myFolder
            My.Settings.IRIS_RootFolder = myFolder
            UpdateConfig()
        End If
    End Sub

    Private Sub UpdateConfig()
        Dim wks As New Workspace
        Dim SQL As String = ""
        SQL = SQL & vbCrLf & "IF EXISTS(SELECT 1 FROM HRC.DocumentMigrationProcess WHERE FileMigrationID = '" & My.Settings.FileMigrationID & "')"
        SQL = SQL & vbCrLf & "UPDATE HRC.DocumentMigrationProcess SET SourceFolder = '" & My.Settings.SourceRootFolder & "', TargetFolder = '" & My.Settings.IRIS_RootFolder & "' WHERE FileMigrationID = '" & My.Settings.FileMigrationID & "'"
        SQL = SQL & vbCrLf & "ELSE"
        SQL = SQL & vbCrLf & "INSERT INTO HRC.DocumentMigrationProcess(MigrationID,FileMigrationID,SourceFolder,TargetFolder) VALUES(" & My.Settings.MigrationID & ",'" & My.Settings.FileMigrationID & "','" & My.Settings.SourceRootFolder & "', '" & My.Settings.IRIS_RootFolder & "')"
        wks.ExecuteNonQuery(SQL)
        wks.Dispose()
    End Sub

    Private Sub FolderValidation(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtIRIS_Root.Validated, txtSourceRoot.Validated
        Dim Changed As Boolean = Not (My.Settings.IRIS_RootFolder = txtIRIS_Root.Text And My.Settings.SourceRootFolder = txtSourceRoot.Text)
        My.Settings.IRIS_RootFolder = txtIRIS_Root.Text
        My.Settings.SourceRootFolder = txtSourceRoot.Text
        If txtIRIS_Root.Text.Length > 0 And txtSourceRoot.Text.Length > 0 And Changed Then
            UpdateConfig()
        End If
    End Sub

    Private Sub txtIRIS_Server_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtIRIS_Server.KeyUp
        If e.KeyCode = Keys.Enter Then
            ValidateServer()
            UpdatePix()
        End If
    End Sub
#End Region

#Region "Shared Methods"
    Private Function ValidateServer() As String
        Dim msg As String = ""

        Dim cmb As ComboBox = cmbIRIS_Database
        My.Settings.IRIS_Server = txtIRIS_Server.Text

        If Not TestSQLServerConnection(My.Settings.IRIS_Server) Then
            cmb.DataSource = Nothing
            cmb.Text = ""
            cmb.Tag = ""
            msg = "The IRIS SQL server name you have entered can't be found"
            MsgBox("The SQL server name you have entered can't be found. Enter a valid server name and try again", vbExclamation + vbOKOnly, "Server not found")
        Else
            Dim wks As New Workspace
            Dim db_Exists As Boolean = (wks.ExecuteScalar("SELECT COUNT(*) AS ItemCount FROM sys.databases WHERE name LIKE 'IRIS%Migration'") > 0)
            If Not db_Exists Then
                msg = "The IRIS Migration database was not found on the nominated IRIS server"
                MsgBox("This application requires the IRIS Migration database to be present on the selected server and it is not found on '" & My.Settings.IRIS_Server & "'. Please select a valid IRIS server", vbExclamation + vbOKOnly, "IRIS_Migration Not Found")
            Else
                Dim i As Integer

                My.Settings.IRIS_Database = wks.ExecuteScalar("SELECT name FROM master.sys.databases WHERE name LIKE 'iris%' AND name NOT LIKE 'iris%[_]%'")

                cmbIRIS_Database.DataSource = GetDatabaseList()
                For i = 0 To cmbIRIS_Database.Items.Count - 1
                    If cmbIRIS_Database.Items(i) = My.Settings.IRIS_Database Then
                        cmbIRIS_Database.SelectedIndex = i
                        Exit For
                    End If
                Next

                cmbWork_Database.DataSource = GetDatabaseList()
                For i = 0 To cmbWork_Database.Items.Count - 1
                    If cmbWork_Database.Items(i) Like "IRIS*Migration" Then
                        cmbWork_Database.SelectedIndex = i
                        Exit For
                    End If
                Next
            End If

            wks.Dispose()
        End If
        Return msg
    End Function

    Private Function GetDatabaseList() As ArrayList
        Dim dbList As New ArrayList

        Me.Cursor = Cursors.WaitCursor

        Dim wks As New Workspace
        wks.CommandText = "SELECT LEFT(CAST(SERVERPROPERTY('productversion') as VARCHAR), 4) AS [Version]"

        Dim rdr As SqlDataReader = wks.ExecuteReader
        If rdr.HasRows Then
            rdr.Read()
            If rdr("Version") > 8 Then
                wks.CommandText = "SELECT Name FROM master.sys.databases WHERE database_id > 4 ORDER BY Name"
            Else
                wks.CommandText = "SELECT Name FROM master.dbo.sysdatabases WHERE dbid > 4 ORDER BY Name"
            End If
        End If

        rdr.Close()

        rdr = wks.ExecuteReader()
        If rdr.HasRows Then
            Do While rdr.Read
                dbList.Add(rdr("Name"))
            Loop
        End If

        rdr.Close()
        rdr = Nothing

        wks.Dispose()

        Me.Cursor = Cursors.Default

        Return dbList
    End Function

    Private Function GetIDList(ByVal ColumnName As String) As ArrayList
        Dim dbList As New ArrayList

        If My.Settings.ObjectType = "" Then My.Settings.ObjectType = "Management Site"

        Dim wks As New Workspace
        Select Case ColumnName
            Case "FileMigrationID"
                wks.CommandText = "SELECT FileMigrationID FROM hrc.DocumentMigrationProcess"
            Case "ObjectType"
                wks.CommandText = "SELECT DisplayValue AS ObjectType FROM " & My.Settings.IRIS_Database & ".dbo.ReferenceDataValue WHERE CollectionID = 1 AND IsCurrent = 1"
            Case "IdentifierContext"
                wks.CommandText = "SELECT IdentifierContext FROM (SELECT COALESCE(R3.DisplayValue, R2.DisplayValue, R1.DisplayValue, R0.DisplayValue) ObjectType, I0.DisplayValue IdentifierContext FROM " & My.Settings.IRIS_Database & ".dbo.ReferenceDataValue I0 LEFT JOIN " & My.Settings.IRIS_Database & ".dbo.ReferenceDataValue R0 ON R0.ID = I0.ParentValueID AND R0.IsCurrent = 1 LEFT JOIN " & My.Settings.IRIS_Database & ".dbo.ReferenceDataValue R1 ON R1.ID = R0.ParentValueID AND R1.IsCurrent = 1 LEFT JOIN " & My.Settings.IRIS_Database & ".dbo.ReferenceDataValue R2 ON R2.ID = R1.ParentValueID AND R2.IsCurrent = 1 LEFT JOIN " & My.Settings.IRIS_Database & ".dbo.ReferenceDataValue R3 ON R3.ID = R2.ParentValueID AND R3.IsCurrent = 1 WHERE I0.CollectionID IN (3,121,212,213) AND R0.IsCurrent = 1) I WHERE ObjectType = '" & My.Settings.ObjectType & "' ORDER BY 1"
        End Select
        Dim rdr As SqlDataReader = wks.ExecuteReader
                If rdr.HasRows Then
            Do While rdr.Read
                dbList.Add(rdr(ColumnName))
            Loop
        End If
        rdr.Close()
        rdr = Nothing

        wks.Dispose()

        Return dbList
    End Function

    Private Function GetFolder(Optional ByVal Title As String = "", Optional ByVal StartFolder As String = "") As String
        Dim myFolder As String = StartFolder

        If Directory.Exists(myFolder) Then ofdGetFolder.InitialDirectory = myFolder

        If Len(Title) > 0 Then ofdGetFolder.Title = Title
        ofdGetFolder.ValidateNames = False                              'Set validate names and check file exists to false 
        ofdGetFolder.CheckFileExists = False                            'otherwise windows will not let you select "Folder Selection"
        ofdGetFolder.CheckPathExists = True
        ofdGetFolder.FileName = "Folder Selection"                      'Always default to Folder Selection.
        ofdGetFolder.ShowDialog()
        myFolder = Path.GetDirectoryName(ofdGetFolder.FileName)

        If Directory.Exists(myFolder) Then Return myFolder Else Return ""
    End Function

    Private Sub RecurseFolder(ByVal ParentFolderID As Integer, ByVal Level As Integer, ByVal FolderName As String, ByRef FolderTable As DataTable, ByRef FileTable As DataTable)
        FolderID = FolderID + 1
        Dim ThisFolderID As Integer = FolderID

        Try
            Dim Dir As DirectoryInfo = New DirectoryInfo(FolderName)
            Dim Files As FileInfo() = Dir.GetFiles()
            Dim FolderSize As Long = 0
            For i As Integer = 0 To Files.Length - 1
                If Files(i).Name.Substring(1, 1) <> "$" AndAlso Files(i).Name <> "Thumbs.db" Then
                    FolderSize = FolderSize + Files(i).Length
                    FileTable.Rows.Add(My.Settings.MigrationID, My.Settings.FileMigrationID, FolderID, Files(i).Name, Files(i).Length, Files(i).LastWriteTime, DBNull.Value, Now())
                End If
            Next

            FolderTable.Rows.Add(My.Settings.MigrationID, My.Settings.FileMigrationID, FolderID, ParentFolderID, Level, IO.Path.GetFileName(FolderName), IO.Directory.GetDirectories(FolderName).GetLength(0), FolderSize, Files.Length, DBNull.Value)

            For Each D In IO.Directory.GetDirectories(FolderName)
                RecurseFolder(ThisFolderID, Level + 1, D, FolderTable, FileTable)
            Next
        Catch ex As Exception
            FolderTable.Rows.Add(My.Settings.MigrationID, My.Settings.FileMigrationID, FolderID, ParentFolderID, Level, IO.Path.GetFileName(FolderName), 1, 0, 0, ex.Message.Replace("'", "''"))
        End Try
    End Sub

    Private Sub SelectComboItem(ByRef Combo As ComboBox, ByVal Itemtext As String)
        Dim i As Integer

        Busy = True

        Combo.SelectedIndex = -1
        Combo.Text = ""
        For i = 0 To Combo.Items.Count - 1
            If Combo.Items(i) = Itemtext Then
                Combo.SelectedIndex = i
                Exit For
            End If
        Next

        Busy = False

    End Sub

    Private Function TestSQLServerConnection(ByVal strServerName As String) As Boolean
        If (strServerName = System.Environment.MachineName) Or (strServerName = "localhost") Then
            Return True
        End If

        Try                                                                     ' Test a connection to a SQL Server by IP address or name
            Dim MyIPHost As New System.Net.IPHostEntry()                        ' Attempt to resolve the server name
            MyIPHost = System.Net.Dns.GetHostEntry(strServerName)

            Dim TheAddress As System.Net.IPAddress                              ' Get the address object
            TheAddress = MyIPHost.AddressList(0)

            Dim MyTCPClient As System.Net.Sockets.TcpClient = New System.Net.Sockets.TcpClient()
            MyTCPClient.Connect(TheAddress, 1433)                               ' See if we can connect to port 1433. 1433 is the default port for SQL Server. If the server is configured to use a different port, change it here.

            MyTCPClient.Close()                                                 ' Everything ok so far, close the TCP connection
            MyTCPClient = Nothing

            TheAddress = Nothing                                                ' Housekeeping
            MyIPHost = Nothing

            Return True                                                         ' Return success
        Catch excServerConnection As Exception
            Return False                                                        ' If an exception is raised, let the caller know  the SQL Server could not be reached
        End Try

    End Function

    Private Sub UpdatePix()

        ToolTips.SetToolTip(btnCreateWorkspace, "Creates tables and views in the nominated workspace database to facilitate IRIS document migration")
        btnCreateWorkspace.Enabled = (My.Settings.IRIS_Server.Length > 0)
        btnCreateWorkspace.ForeColor = Color.Black

        ToolTips.SetToolTip(cmdPopulateFolderTable, "Reads all sub-folders of the nominated source documents root folder and populates a list of folders to be migrated")
        cmdPopulateFolderTable.Enabled = False
        cmdPopulateFolderTable.ForeColor = Color.Black

        ToolTips.SetToolTip(cmdIdentifyDuplicates, "Checks all likely duplicates using filename, file size and file date for files associated with each authorisation and calculates a CRC hash as a final check")
        cmdIdentifyDuplicates.Enabled = False
        cmdIdentifyDuplicates.ForeColor = Color.Black

        txtSourceRoot.Enabled = False
        txtIRIS_Root.Enabled = False
        cmdBrowseR2D2.Enabled = False
        cmdBrowseIRIS.Enabled = False

        cmdMigrateFiles.Enabled = False
        cmdMigrateFiles.ForeColor = Color.Black

        If TestSQLServerConnection(My.Settings.IRIS_Server) Then
            Try
                Dim wks As New Workspace(3)
                Try
                    Dim rdr As SqlDataReader
                    rdr = wks.ExecuteReader("SELECT TaskID, ISNULL(ErrorText,'') AS ErrorText FROM HRC.DocumentMigrationProcessTask WHERE FileMigrationID = '" & My.Settings.FileMigrationID & "'")

                    If rdr.HasRows Then
                        While rdr.Read()
                            Select Case rdr("TaskID")
                                Case 1
                                    cmdPopulateFolderTable.Enabled = True
                                    If rdr("ErrorText").ToString.Length > 0 Then
                                        ToolTips.SetToolTip(btnCreateWorkspace, rdr("ErrorText"))
                                        btnCreateWorkspace.ForeColor = Color.Red
                                    Else
                                        ToolTips.SetToolTip(btnCreateWorkspace, "Required tables and views have been created in the nominated workspace database to facilitate IRIS document migration")
                                        btnCreateWorkspace.ForeColor = Color.Green
                                    End If

                                    txtSourceRoot.Enabled = True
                                    txtIRIS_Root.Enabled = True
                                    cmdBrowseR2D2.Enabled = True
                                    cmdBrowseIRIS.Enabled = True
                                Case 2
                                    cmdIdentifyDuplicates.Enabled = True
                                    If rdr("ErrorText").ToString.Length > 0 Then
                                        ToolTips.SetToolTip(cmdPopulateFolderTable, rdr("ErrorText"))
                                        cmdPopulateFolderTable.ForeColor = Color.Red
                                    Else
                                        ToolTips.SetToolTip(cmdPopulateFolderTable, "A list of all files and folders to be migrated has been generated and stored in the workspace database")
                                        cmdPopulateFolderTable.ForeColor = Color.Green
                                    End If
                                Case 3
                                    cmdMigrateFiles.Enabled = True
                                    If rdr("ErrorText").ToString.Length > 0 Then
                                        ToolTips.SetToolTip(cmdIdentifyDuplicates, rdr("ErrorText"))
                                        cmdIdentifyDuplicates.ForeColor = Color.Red
                                    Else
                                        ToolTips.SetToolTip(cmdIdentifyDuplicates, "Duplicate files have been identified using filename, file size, file date and CRC hash")
                                        cmdIdentifyDuplicates.ForeColor = Color.Green
                                    End If
                                Case 4
                                    If rdr("ErrorText").ToString.Length > 0 Then
                                        ToolTips.SetToolTip(cmdMigrateFiles, rdr("ErrorText"))
                                        cmdMigrateFiles.ForeColor = Color.Red
                                    Else
                                        ToolTips.SetToolTip(cmdMigrateFiles, "Documents have been migrated successfully")
                                        cmdMigrateFiles.ForeColor = Color.Green
                                    End If
                            End Select

                        End While
                    End If

                    rdr = Nothing
                Catch ex As Exception
                    MsgBox("Required workspace tables have not yet been created.", vbInformation + vbOKOnly, "Workspace Not Prepared")
                End Try
                wks.Dispose()
            Catch ex As Exception
                MsgBox("Connection to nominated server '" & My.Settings.IRIS_Server & "' cannot be made due to error: " & ex.Message & vbCrLf & vbCrLf & "Please verify the IRIS server name", vbInformation + vbOKOnly, "Server Not Found")
            End Try
        End If
    End Sub
#End Region

#Region "Migration Process Buttons"
    '-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ' Task = 1  Create workspace and initialise Tasks to 1
    '-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    Private Sub btnCreateWorkspace_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCreateWorkspace.Click
        Dim msg As String = ""
        msg = ValidateServer()
        If msg.Length = 0 Then msg = EnsureTables()
        VerifyMigrationID()
        UpdateConfig()
        ResetTasksFrom(1)
        UpdateTask(1, msg)
        UpdatePix()
    End Sub
    '-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ' Task = 2  Read all folder and sub-folder names recursively and save all to DM_Folders 
    '-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    Private Sub cmdPopulateFolderTable_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdPopulateFolderTable.Click
        Dim f As Integer = 0
        Dim fMax As Integer
        Dim Percent As Integer = 0

        If MsgBox("This process will read the source folder, recursively read all subfolders and record the level, folder name and number of files contained" & vbCrLf & vbCrLf & "The process may take several minutes to complete. Continue?", vbInformation + vbYesNo, "Confirm Continue") = vbNo Then
            Exit Sub
        End If
        Application.DoEvents()
        ResetTasksFrom(2)
        UpdatePix()
        '---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        '   Create a data table in RAM to collect details for each folder (saved to hard storage on completion)
        '---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        Dim Folders As New DataTable("Folders")
        Folders.Columns.Add("MigrationID", Type.GetType("System.Int64"))
        Folders.Columns.Add("FileMigrationID", Type.GetType("System.String"))
        Folders.Columns.Add("FolderID", Type.GetType("System.Int64"))
        Folders.Columns.Add("ParentFolderID", Type.GetType("System.Int64"))
        Folders.Columns.Add("Level", Type.GetType("System.Int64"))
        Folders.Columns.Add("FolderName", Type.GetType("System.String"))
        Folders.Columns.Add("FolderCount", Type.GetType("System.Int64"))
        Folders.Columns.Add("FolderSize", Type.GetType("System.Int64"))
        Folders.Columns.Add("FileCount", Type.GetType("System.Int64"))
        Folders.Columns.Add("ErrorText", Type.GetType("System.String"))
        '---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        '   Create a data table in RAM to collect details for each file (saved to hard storage on completion)
        '---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        Dim Files As New DataTable("Files")
        Files.Columns.Add("MigrationID", Type.GetType("System.Int64"))
        Files.Columns.Add("FileMigrationID", Type.GetType("System.String"))
        Files.Columns.Add("FolderID", Type.GetType("System.Int64"))
        Files.Columns.Add("FileName", Type.GetType("System.String"))
        Files.Columns.Add("FileSize", Type.GetType("System.Int64"))
        Files.Columns.Add("Modified", Type.GetType("System.DateTime"))
        Files.Columns.Add("Checksum", Type.GetType("System.String"))
        Files.Columns.Add("DateTime", Type.GetType("System.DateTime"))
        '---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        Dim wks As New Workspace
        Dim myFolderID As Integer = wks.ExecuteScalar("SELECT ISNULL((SELECT MAX(FolderID) FROM hrc.DocumentMigrationFolders),0)") 'Reset the folder ID to start at the max of the current list of folder ID's
        Dim SQL As String = ""
        SQL = SQL & vbCrLf & "/***********************************************************************************************************************************************************************"
        SQL = SQL & vbCrLf & "*   Script:         Identify sought folder names based on authorisation business ID or legacy ID"
        SQL = SQL & vbCrLf & "*   Description:    If we're migrating R2D2 files then we need to use the legacy ID as the folder name, else the authorisation business ID"
        SQL = SQL & vbCrLf & "***********************************************************************************************************************************************************************/"
        SQL = SQL & vbCrLf & "SELECT      ''''+STUFF((SELECT ''','''+FolderName FROM ("
        SQL = SQL & vbCrLf & "            SELECT      DISTINCT"
        SQL = SQL & vbCrLf & "                        OID.IdentifierContextID,"
        SQL = SQL & vbCrLf & "                        OID.OtherIdentifierText                             FolderName"
        SQL = SQL & vbCrLf & "            FROM        IRISTEST.dbo.ManagementSite                         MGS"
        SQL = SQL & vbCrLf & "            LEFT JOIN   IRISTEST.dbo.IRISObject                             MGO"
        SQL = SQL & vbCrLf & "            ON          MGO.ID = MGS.IRISObjectID"
        SQL = SQL & vbCrLf & "            LEFT JOIN   IRISTEST.dbo.OtherIdentifier                        OID"
        SQL = SQL & vbCrLf & "            ON          OID.IRISObjectID = MGS.IRISObjectID"
        SQL = SQL & vbCrLf & "            WHERE       OID.IsDeleted = 0"
        SQL = SQL & vbCrLf & "            AND         OID.IdentifierContextID = 14411)                    MGS"
        SQL = SQL & vbCrLf & "            ORDER BY    FolderName"
        SQL = SQL & vbCrLf & "            FOR XML     PATH('')),1,3,'')+''''                              FolderList"
        SQL = SQL & vbCrLf & ""

        'SQL = SQL & vbCrLf & "DECLARE @GetLegacy BIT = " & IIf(My.Settings.SourceRootFolder Like "*E\RM\04\00*", "1", "0")
        'SQL = SQL & vbCrLf & ""
        'SQL = SQL & vbCrLf & "SELECT      ''''+STUFF((SELECT ''','''+FolderName FROM ("
        'SQL = SQL & vbCrLf & "SELECT      DISTINCT"
        'SQL = SQL & vbCrLf & "            IIF(@GetLegacy = 1,LEG.LegacyID,ATO.BusinessID)                 FolderName"
        'SQL = SQL & vbCrLf & "FROM       (SELECT      DISTINCT"
        'SQL = SQL & vbCrLf & "                        AuthorisationIRISID"
        'SQL = SQL & vbCrLf & "            FROM        hrc.MapRegime"
        'SQL = SQL & vbCrLf & "            WHERE       MigrationID = " & My.Settings.MigrationID
        'SQL = SQL & vbCrLf & "            AND         Migrate = 1)                                        MAP"
        'SQL = SQL & vbCrLf & "LEFT JOIN   IRIS[ENV].dbo.IRISObject                                          ATO"
        'SQL = SQL & vbCrLf & "ON          ATO.BusinessID = MAP.AuthorisationIRISID"
        'SQL = SQL & vbCrLf & "LEFT JOIN  (SELECT      IRISObjectID,"
        'SQL = SQL & vbCrLf & "                        OtherIdentifierText                                 LegacyID"
        'SQL = SQL & vbCrLf & "            FROM        IRIS[ENV].dbo.OtherIdentifier"
        'SQL = SQL & vbCrLf & "            WHERE       IsDeleted = 0"
        'SQL = SQL & vbCrLf & "            AND         IdentifierContextID = 427)                          LEG"
        'SQL = SQL & vbCrLf & "ON          LEG.IRISObjectID = ATO.ID)                                      LEG"
        'SQL = SQL & vbCrLf & "ORDER BY    LEG.FolderName"
        'SQL = SQL & vbCrLf & "FOR XML     PATH('')),1,3,'')+''''                                          FolderList"
        SQL = SQL.Replace("IRIS[ENV]", My.Settings.IRIS_Database)

        Dim myFolderList As String = IIf(IsDBNull(wks.ExecuteScalar(SQL)), "", wks.ExecuteScalar(SQL)) 'Get a comma-delimited list of folders to be included in the recursive scan

        If My.Settings.IRIS_Database.Length > 0 Then
            lblStatus.Text = "Processing..."
            Application.DoEvents()

            Dim DR As String() = IO.Directory.GetDirectories(My.Settings.SourceRootFolder)      'Get the list of folders to be processed
            fMax = DR.GetUpperBound(0)                                                          'Get the number of directories to be processed
            For f = 0 To fMax
                If Int(f * 100 / fMax) > Percent Then
                    Percent = f * 100 / fMax
                    lblStatus.Text = "Processing... " & f.ToString("N0") & " folders completed out of " & fMax.ToString("N0") & " total folders (" & Percent.ToString & "%)"
                    Application.DoEvents()
                End If
                If myFolderList.Contains("'" & IO.Path.GetFileName(DR(f)) & "'") Then
                    RecurseFolder(0, 0, DR(f), Folders, Files)
                End If
            Next f
        End If
        '---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        '   Save our RAM data table to hard storage
        '---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        Dim msg As String = ""

        If Folders.Rows.Count > 0 AndAlso MsgBox("Confirm that you want to overwrite existing data?", vbYesNo, "Confirm Overwrite") = MsgBoxResult.Yes Then
            Try                                                                                                             'Save the results to our workspace database
                wks.ExecuteNonQuery("ALTER INDEX idx_ID ON HRC.DocumentMigrationFolders DISABLE")                           'This is necessary for larger files as the indexes slow down the bulk copy and it may time-out
                wks.ExecuteNonQuery("ALTER INDEX idx_FolderID ON HRC.DocumentMigrationFolders DISABLE")
                wks.ExecuteNonQuery("ALTER INDEX idx_ParentFolderID ON HRC.DocumentMigrationFolders DISABLE")
                wks.ExecuteNonQuery("DELETE FROM HRC.DocumentMigrationFolders WHERE FileMigrationID = '" & My.Settings.FileMigrationID & "'")   'Delete existing records in the same source group

                wks.BulkCopy.ColumnMappings.Clear()

                For i As Integer = 0 To Folders.Columns.Count - 1
                    wks.BulkCopy.ColumnMappings.Add(Folders.Columns(i).ColumnName, Folders.Columns(i).ColumnName)
                Next

                wks.BulkCopy.DestinationTableName = "HRC.DocumentMigrationFolders"
                wks.BulkCopy.WriteToServer(Folders)

                wks.ExecuteNonQuery("ALTER INDEX idx_ID ON HRC.DocumentMigrationFolders REBUILD")                                         'And now reinstate the indexes
                wks.ExecuteNonQuery("ALTER INDEX idx_FolderID ON HRC.DocumentMigrationFolders REBUILD")
                wks.ExecuteNonQuery("ALTER INDEX idx_ParentFolderID ON HRC.DocumentMigrationFolders REBUILD")

                '-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                wks.ExecuteNonQuery("ALTER INDEX idx_ID ON HRC.DocumentMigrationFiles DISABLE")                                           'This is necessary for larger files as the indexes slow down the bulk copy and it may time-out
                wks.ExecuteNonQuery("ALTER INDEX idx_FolderID ON HRC.DocumentMigrationFiles DISABLE")
                wks.ExecuteNonQuery("DELETE FROM HRC.DocumentMigrationFiles WHERE FileMigrationID = '" & My.Settings.FileMigrationID & "'")       'Delete existing records
                wks.BulkCopy.ColumnMappings.Clear()
                wks.ExecuteNonQuery("IF COL_LENGTH('HRC.DocumentMigrationFiles', 'Checksum') IS NULL ALTER TABLE HRC.DocumentMigrationFiles ADD [Checksum] VARCHAR(100) NULL") 'Make sure the new Checksum column exists
                wks.ExecuteNonQuery("IF COL_LENGTH('HRC.DocumentMigrationFiles', 'Modified') IS NULL ALTER TABLE HRC.DocumentMigrationFiles ADD [Modified] DATETIME     NULL") 'Make sure the new Modified column exists
                For i As Integer = 0 To Files.Columns.Count - 1                                                             'NOTE: The column mapping is case sensitive
                    wks.BulkCopy.ColumnMappings.Add(Files.Columns(i).ColumnName, Files.Columns(i).ColumnName)
                Next

                wks.BulkCopy.DestinationTableName = "HRC.DocumentMigrationFiles"
                wks.BulkCopy.WriteToServer(Files)

                wks.ExecuteNonQuery("ALTER INDEX idx_ID ON HRC.DocumentMigrationFiles REBUILD")                                           'And now reinstate the indexes
                wks.ExecuteNonQuery("ALTER INDEX idx_FolderID ON HRC.DocumentMigrationFiles REBUILD")
            Catch ex As Exception
                msg = ex.Message
            End Try
        End If

        wks.Dispose()
        '---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        UpdateTask(2, msg)
        UpdatePix()

        lblStatus.Text = "Ready"
    End Sub
    '-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ' Task = 3   Identify Duplicate Files
    '-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    Private Sub cmdIdentifyDuplicates_Click(sender As Object, e As EventArgs) Handles cmdIdentifyDuplicates.Click
        Dim r As Integer
        Dim rMax As Integer
        Dim Percent As Integer

        lblStatus.Text = "Identifying and loading data for potential duplicate files"
        Application.DoEvents()
        ResetTasksFrom(3)
        UpdatePix()

        Dim wks As New Workspace
        Dim SourceFolder As String = wks.ExecuteScalar("SELECT SourceFolder FROM HRC.DocumentMigrationProcess WHERE FileMigrationID = '" & My.Settings.FileMigrationID & "'")
        '---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        '   The intention here is to identify files that will be duplicates when we copy them to the historical Regime Activity folder. Not only are there duplicate files within
        '   any individual Authorisation sub-folders in R2D2, but since a Regime can have many Authorisations associated with it, some duplicates will occur where the same file
        '   has been saved to different Authorisations in R2D2. Suspected duplicates are files with the same name, size and time-stamp. These are the ones identified by the script 
        '   below and the associated procedure calculates and saves a CRC hash for these files as a final tie-breaker. Files that have the same name but are not duplicates will be
        '   copied with a version number based on the file date
        '---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        Dim dt As New DataTable("Duplicates")
        wks.Fill(dt, GetSQL_IdentifyDuplicates)
        rMax = dt.Rows.Count - 1

        Dim myCheckSum As String
        Dim SHA As New SHA256Managed()

        lblStatus.Text = "Calculating file checksums for " & (rMax + 1).ToString & " potential duplicate files"
        Dim ErrorCount As Integer = 0
        Application.DoEvents()
        For r = 0 To rMax
            '-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            '   Update the interface with completion status if we've passed another percentage point
            '-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            If Int(r * 100 / rMax) > Percent Then
                Percent = r * 100 / rMax
                lblStatus.Text = "Calculating file checksums. " & r.ToString("N0") & " files processed out of " & (rMax + 1).ToString("N0") & " (" & Percent.ToString & "% complete)"
                Application.DoEvents()
            End If
            '-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            '   Try copying the file, but first check if a copy already exists (since this is a subsequent migration, that's a strong likelihood)
            '-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            Try
                Dim F As IO.FileInfo = New IO.DirectoryInfo(IO.Path.Combine(SourceFolder, dt.Rows(r)("FolderPath"))).GetFiles(dt.Rows(r)("FileName"))(0)
                Using cStream As Stream = F.OpenRead
                    myCheckSum = BitConverter.ToString(SHA.ComputeHash(cStream)).Replace("-", "")
                End Using
                wks.ExecuteNonQuery("UPDATE HRC.DocumentMigrationFiles SET [Checksum] = '" & myCheckSum & "' WHERE FolderID = " & dt.Rows(r)("FolderID").ToString & " AND Filename = '" & dt.Rows(r)("FileName").ToString.Replace("'", "''") & "'")
            Catch ex As Exception
                ErrorCount += 1
                wks.ExecuteNonQuery("UPDATE HRC.DocumentMigrationFiles SET ErrorText = '" & ex.Message.Replace("'", "''") & "' WHERE FolderID = " & dt.Rows(r)("FolderID").ToString & " AND Filename = '" & dt.Rows(r)("FileName") & "'")
            End Try
        Next
        '-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        dt.Dispose()
        wks.Dispose()
        '---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        UpdateTask(3, IIf(ErrorCount > 0, String.Format("{0} Errors out of {1} files tested", ErrorCount, rMax + 1), ""))
        UpdatePix()

        lblStatus.Text = "Ready"
    End Sub
    '-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ' Task = 4   Migrate Files
    '-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    Private Sub cmdMigrateFiles_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdMigrateFiles.Click
        Dim f As Integer = 0
        Dim fMax As Integer = 0
        Dim StatusText As String = ""
        Dim Percent As Integer = 0
        '---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        If MsgBox("This process uses the configuration and mapping created in the steps above to copy files from the identified source folder and sub-folders to destination folders " _
                & "suitably named so that the IRIS documents web service can locate them from the Applications and Authorisations Documents tab" _
                & vbCrLf & vbCrLf _
                & "The process will take several hours to complete. Continue?",
                vbInformation + vbYesNo, "Confirm Copy Files") = vbNo Then
            Exit Sub
        End If
        lblStatus.Text = "Reading mapping information..."
        Application.DoEvents()
        '---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        '   Query the folder tree and files table to get the mapping from source to destination file path
        '---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        Dim wks As New Workspace
        Dim SQL As String = GetSQL_DocumentMapping()
        Dim DT As New DataTable("DocumentMap")
        wks.Fill(DT, SQL)

        fMax = DT.Rows.Count - 1
        '---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        lblStatus.Text = "Copying files"
        Application.DoEvents()
        For f = 0 To fMax
            If Int(f * 100 / fMax) > Percent Then
                Percent = f * 100 / fMax
                lblStatus.Text = "Copying file " & f.ToString & " of " & fMax.ToString & " (" & Percent.ToString & "% complete)"
                Application.DoEvents()
            End If
            Try
                If Not IO.Directory.Exists(DT.Rows(f)("TargetFolder")) Then
                    IO.Directory.CreateDirectory(DT.Rows(f)("TargetFolder"))
                End If
            Catch ex As Exception
                myLogger.AddLog("Error while creating directory '" & DT.Rows(f)("TargetFolder") & "'")
            End Try

            If DT.Rows(f)("FileName").Substring(1, 1) <> "$" Then
                Try
                    If Not IO.File.Exists(IO.Path.Combine(DT.Rows(f)("TargetFolder"), DT.Rows(f)("NewFileName"))) Then
                        IO.File.Copy(IO.Path.Combine(DT.Rows(f)("SourceFolder"), DT.Rows(f)("FileName")), IO.Path.Combine(DT.Rows(f)("TargetFolder"), DT.Rows(f)("NewFileName")), True)
                    End If
                    wks.ExecuteNonQuery("UPDATE HRC.DocumentMigrationFiles SET Migrated = 1 WHERE FolderID = " & DT.Rows(f)("MinFolderID").ToString & " AND Filename = '" & DT.Rows(f)("FileName").ToString.Replace("'", "''") & "'")
                Catch ex As Exception
                    myLogger.AddLog("Error while copying file '" & IO.Path.Combine(DT.Rows(f)("SourceFolder"), "" & DT.Rows(f)("FileName")) & "' to '" & IO.Path.Combine(DT.Rows(f)("TargetFolder"), "" & DT.Rows(f)("NewFileName")) & "'. The error was " & ex.Message)
                    wks.ExecuteNonQuery("UPDATE HRC.DocumentMigrationFiles SET ErrorText = '" & ex.Message.Replace("'", "''") & "' WHERE FolderID = " & DT.Rows(f)("MinFolderID").ToString & " AND Filename = '" & DT.Rows(f)("FileName").ToString.Replace("'", "''") & "'")
                End Try
            End If
        Next
        '---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        DT.Dispose()
        wks.Dispose()
        '---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        UpdateTask(4, "")
        UpdatePix()
        '---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        lblStatus.Text = "Ready"
    End Sub
    '-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ' Helper Functions
    '-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    Private Function GetSQL_DocumentMapping() As String
        Dim SQL As String = ""
        SQL = SQL & vbCrLf & "/***********************************************************************************************************************************************************************"
        SQL = SQL & vbCrLf & "*   Script:         Identify Files to be Migrated"
        SQL = SQL & vbCrLf & "*   Description:    The purpose of this script is to identify the source files to be migrated based on the Regimes and their associated"
        SQL = SQL & vbCrLf & "*                   Authorisations. The script also identifies files that are exact copies of each other based on filename, file size and"
        SQL = SQL & vbCrLf & "*                   a predetermined CRC hash, then eliminates duplicates from the list and versions any non-exact copies of the same filename"
        SQL = SQL & vbCrLf & "***********************************************************************************************************************************************************************/"
        SQL = SQL & vbCrLf & "DECLARE @FileMigrationID VARCHAR(50) = '" & My.Settings.FileMigrationID & "'"
        SQL = SQL & vbCrLf & "DECLARE @IdentifierContextID AS INTEGER = 14411"
        SQL = SQL & vbCrLf & "DECLARE @ObjectType NVARCHAR(100) = 'Management Site'"
        SQL = SQL & vbCrLf & "-----------------------------------------------------------------------------------------------------------------------------------------------------------------------"
        SQL = SQL & vbCrLf & "--  This is the folder mapping that identifies which folders will be migrated plus sets any file prefixes that will be added when the files are copied"
        SQL = SQL & vbCrLf & "-----------------------------------------------------------------------------------------------------------------------------------------------------------------------"
        SQL = SQL & vbCrLf & "IF OBJECT_ID(N'tempdb..#Folder',N'U') IS NULL"
        SQL = SQL & vbCrLf & "BEGIN"
        SQL = SQL & vbCrLf & "    SELECT      M.FolderID,"
        SQL = SQL & vbCrLf & "                M.ParentFolderID,"
        SQL = SQL & vbCrLf & "                M.Level,"
        SQL = SQL & vbCrLf & "                M.FolderName,"
        SQL = SQL & vbCrLf & "                M.FolderCount,"
        SQL = SQL & vbCrLf & "                M.FileCount,"
        SQL = SQL & vbCrLf & "                P.SourceFolder,"
        SQL = SQL & vbCrLf & "                P.TargetFolder,"
        SQL = SQL & vbCrLf & "                R.DisplayValue                                                              ObjectType,"
        SQL = SQL & vbCrLf & "                CASE"
        SQL = SQL & vbCrLf & "                WHEN FolderName LIKE '%Rural Advi[sc]e%'        THEN 'Rural Advice, '"
        SQL = SQL & vbCrLf & "                WHEN FolderName LIKE '%appeal%'                 THEN 'Appeal, '"
        SQL = SQL & vbCrLf & "                WHEN FolderName LIKE '%hearing%'                THEN 'Hearing, '"
        SQL = SQL & vbCrLf & "                WHEN FolderName  =   'Consent Correspondence'   THEN ''"
        SQL = SQL & vbCrLf & "                WHEN FolderName LIKE '%compliance%'             THEN ''"
        SQL = SQL & vbCrLf & "                WHEN FolderName LIKE '%monitoring%'             THEN ''"
        SQL = SQL & vbCrLf & "                WHEN FolderName  =   'FlowMeterInfo'            THEN 'Flow Meter Information, '"
        SQL = SQL & vbCrLf & "                WHEN FolderName LIKE '%photos%'                 THEN 'Photo, '"
        SQL = SQL & vbCrLf & "                WHEN FolderName LIKE '%inspection%'             THEN 'Inspection, '"
        SQL = SQL & vbCrLf & "                WHEN FolderName LIKE '%test%'                   THEN NULL"
        SQL = SQL & vbCrLf & "                WHEN FolderName  =   'New Folder'               THEN NULL"
        SQL = SQL & vbCrLf & "                ELSE '' END                                                                 Prefix"
        SQL = SQL & vbCrLf & "    INTO        #Folder"
        SQL = SQL & vbCrLf & "    FROM        HRC.DocumentMigrationProcess                                                P"
        SQL = SQL & vbCrLf & "    LEFT JOIN   HRC.DocumentMigrationFolders                                                M"
        SQL = SQL & vbCrLf & "    ON          P.MigrationID = M.MigrationID"
        SQL = SQL & vbCrLf & "    AND         P.FileMigrationID = M.FileMigrationID"
        SQL = SQL & vbCrLf & "    LEFT JOIN   IRIS[ENV].dbo.ReferenceDataValue                                             R"
        SQL = SQL & vbCrLf & "    ON          UPPER(R.DisplayValue) = UPPER(P.ObjectType) COLLATE DATABASE_DEFAULT"
        SQL = SQL & vbCrLf & "    WHERE       P.FileMigrationID = @FileMigrationID"
        SQL = SQL & vbCrLf & "    -------------------------------------------------------------------------------------------------------------------------------------------------------------------"
        SQL = SQL & vbCrLf & "    CREATE INDEX idx_FolderID ON #Folder(ParentFolderID,FolderID) "
        SQL = SQL & vbCrLf & "    -------------------------------------------------------------------------------------------------------------------------------------------------------------------"
        SQL = SQL & vbCrLf & "END"
        SQL = SQL & vbCrLf & "-----------------------------------------------------------------------------------------------------------------------------------------------------------------------"
        SQL = SQL & vbCrLf & "--  FolderPath  --  This recursion builds the full path for the identified folders"
        SQL = SQL & vbCrLf & "-----------------------------------------------------------------------------------------------------------------------------------------------------------------------"
        SQL = SQL & vbCrLf & "IF OBJECT_ID(N'tempdb..#FolderPath',N'U') IS NULL"
        SQL = SQL & vbCrLf & "BEGIN"
        SQL = SQL & vbCrLf & "WITH"
        SQL = SQL & vbCrLf & "-----------------------------------------------------------------------------------------------------------------------------------------------------------------------"
        SQL = SQL & vbCrLf & "FolderPath"
        SQL = SQL & vbCrLf & "-----------------------------------------------------------------------------------------------------------------------------------------------------------------------"
        SQL = SQL & vbCrLf & "AS         (SELECT      FolderID,"
        SQL = SQL & vbCrLf & "                        ParentFolderID,"
        SQL = SQL & vbCrLf & "                        [Level],"
        SQL = SQL & vbCrLf & "                        FolderName,"
        SQL = SQL & vbCrLf & "                        FolderCount,"
        SQL = SQL & vbCrLf & "                        FileCount,"
        SQL = SQL & vbCrLf & "                        CAST(SourceFolder+'\'+FolderName+'\' AS VARCHAR(MAX))                       SourceFolder,"
        SQL = SQL & vbCrLf & "                        TargetFolder,"
        SQL = SQL & vbCrLf & "                        CAST(ObjectType AS VARCHAR(MAX))                                            ObjectType,"
        SQL = SQL & vbCrLf & "                        Prefix"
        SQL = SQL & vbCrLf & "            FROM        #Folder"
        SQL = SQL & vbCrLf & "            WHERE       [Level] = 0"
        SQL = SQL & vbCrLf & "            UNION ALL"
        SQL = SQL & vbCrLf & "            SELECT      F.FolderID,"
        SQL = SQL & vbCrLf & "                        F.ParentFolderID,"
        SQL = SQL & vbCrLf & "                        F.[Level],"
        SQL = SQL & vbCrLf & "                        F.FolderName,"
        SQL = SQL & vbCrLf & "                        F.FolderCount,"
        SQL = SQL & vbCrLf & "                        F.FileCount,"
        SQL = SQL & vbCrLf & "                        CAST(FP.SourceFolder+F.FolderName+'\' AS VARCHAR(MAX)),"
        SQL = SQL & vbCrLf & "                        F.TargetFolder,"
        SQL = SQL & vbCrLf & "                        CASE"
        SQL = SQL & vbCrLf & "                        WHEN LEN(F.ObjectType) = 0 "
        SQL = SQL & vbCrLf & "                        THEN FP.ObjectType"
        SQL = SQL & vbCrLf & "                        ELSE CAST(F.ObjectType AS VARCHAR(MAX)) END                             ObjectType,"
        SQL = SQL & vbCrLf & "                        F.Prefix"
        SQL = SQL & vbCrLf & "            FROM        #Folder             F"
        SQL = SQL & vbCrLf & "            JOIN        FolderPath          FP"
        SQL = SQL & vbCrLf & "            ON          FP.FolderID = F.ParentFolderID)"
        SQL = SQL & vbCrLf & ""
        SQL = SQL & vbCrLf & "    SELECT * INTO #FolderPath FROM FolderPath OPTION (MAXRECURSION 200)"
        SQL = SQL & vbCrLf & "    -------------------------------------------------------------------------------------------------------------------------------------------------------------------"
        SQL = SQL & vbCrLf & "    CREATE INDEX idx_FolderID      ON #FolderPath(ParentFolderID,FolderID)"
        SQL = SQL & vbCrLf & "    -------------------------------------------------------------------------------------------------------------------------------------------------------------------"
        SQL = SQL & vbCrLf & "END"
        SQL = SQL & vbCrLf & "-----------------------------------------------------------------------------------------------------------------------------------------------------------------------"
        SQL = SQL & vbCrLf & "--  ObjectType - IRISObjectID"
        SQL = SQL & vbCrLf & "-----------------------------------------------------------------------------------------------------------------------------------------------------------------------"
        SQL = SQL & vbCrLf & "IF OBJECT_ID(N'tempdb..#GetIRISObjectID',N'U') IS NULL"
        SQL = SQL & vbCrLf & "BEGIN"
        SQL = SQL & vbCrLf & "    SELECT      OBJ.ID                                                                                      IRISObjectID,"
        SQL = SQL & vbCrLf & "                OID.OtherIdentifierText                                                                     LegacyID,"
        SQL = SQL & vbCrLf & "                OBJ.BusinessID                                                                              ObjectIRISID,"
        SQL = SQL & vbCrLf & "                OBJ.ObjectTypeID"
        SQL = SQL & vbCrLf & "    INTO        #GetIRISObjectID"
        SQL = SQL & vbCrLf & "    FROM       (SELECT      IRISObjectID,"
        SQL = SQL & vbCrLf & "                            OtherIdentifierText"
        SQL = SQL & vbCrLf & "                FROM        IRIS[ENV].dbo.OtherIdentifier"
        SQL = SQL & vbCrLf & "                WHERE       IsDeleted = 0"
        SQL = SQL & vbCrLf & "                AND         IdentifierContextID = @IdentifierContextID)                                     OID"
        SQL = SQL & vbCrLf & "    LEFT JOIN   IRIS[ENV].dbo.IRISObject                                                                     OBJ"
        SQL = SQL & vbCrLf & "    ON          OID.IRISObjectID = OBJ.ID"
        SQL = SQL & vbCrLf & "    LEFT JOIN   IRIS[ENV].dbo.ReferenceDataValue                                                             REF"
        SQL = SQL & vbCrLf & "    ON          REF.ID = OBJ.ObjectTypeID"
        SQL = SQL & vbCrLf & "    WHERE       REF.CollectionID = 1"
        SQL = SQL & vbCrLf & "    AND         REF.DisplayValue = @ObjectType"
        SQL = SQL & vbCrLf & "    -------------------------------------------------------------------------------------------------------------------------------------------------------------------"
        SQL = SQL & vbCrLf & "    CREATE INDEX idx_IRISObjectID    ON #GetIRISObjectID(IRISObjectID)"
        SQL = SQL & vbCrLf & "    CREATE INDEX idx_LegacyID        ON #GetIRISObjectID(LegacyID)"
        SQL = SQL & vbCrLf & "    CREATE INDEX idx_AuthorisationID ON #GetIRISObjectID(ObjectIRISID)"
        SQL = SQL & vbCrLf & "    -------------------------------------------------------------------------------------------------------------------------------------------------------------------"
        SQL = SQL & vbCrLf & "END"
        SQL = SQL & vbCrLf & "-----------------------------------------------------------------------------------------------------------------------------------------------------------------------"
        SQL = SQL & vbCrLf & "--  Main script starts here..."
        SQL = SQL & vbCrLf & "-----------------------------------------------------------------------------------------------------------------------------------------------------------------------"
        SQL = SQL & vbCrLf & "SELECT      MAIN.*,"
        SQL = SQL & vbCrLf & "            CASE"
        SQL = SQL & vbCrLf & "            WHEN CopyCount > 1"
        SQL = SQL & vbCrLf & "            THEN ISNULL(PARSENAME(MAIN.FileName,2),PARSENAME(MAIN.FileName,3))+' v'+RIGHT('00'+CAST(MAIN.VersionNumber AS VARCHAR),2)+'.'+PARSENAME(MAIN.FileName,1)"
        SQL = SQL & vbCrLf & "            ELSE FileName END                                                                                                               NewFileName,"
        SQL = SQL & vbCrLf & "            FP.SourceFolder,"
        SQL = SQL & vbCrLf & "            FP.TargetFolder+'\'+@ObjectType+'s\'+MAIN.ObjectIRISID COLLATE DATABASE_DEFAULT+'\Legacy Monitoring Activities'                 TargetFolder"
        SQL = SQL & vbCrLf & "FROM       (SELECT      DISTINCT"
        SQL = SQL & vbCrLf & "                        FILES.ObjectIRISID,"
        SQL = SQL & vbCrLf & "                        FILES.MinFolderID,"
        SQL = SQL & vbCrLf & "                        FILES.FileName,"
        SQL = SQL & vbCrLf & "                        FILES.FileSize,"
        SQL = SQL & vbCrLf & "                        FILES.Checksum,"
        SQL = SQL & vbCrLf & "                        COUNT(*)        OVER(PARTITION BY ObjectIRISID,FileName)                                                            CopyCount,"
        SQL = SQL & vbCrLf & "                        RANK()          OVER(PARTITION BY ObjectIRISID,FileName"
        SQL = SQL & vbCrLf & "                                                 ORDER BY SourceFolder)                                                                     VersionNumber"
        SQL = SQL & vbCrLf & "            FROM       (SELECT      DISTINCT"
        SQL = SQL & vbCrLf & "                                    OID.ObjectIRISID,"
        SQL = SQL & vbCrLf & "                                    FP.FolderID,"
        SQL = SQL & vbCrLf & "                                    FP.FolderCount,"
        SQL = SQL & vbCrLf & "                                    FP.FileCount,"
        SQL = SQL & vbCrLf & "                                    FP.Prefix,"
        SQL = SQL & vbCrLf & "                                    FP.SourceFolder,"
        SQL = SQL & vbCrLf & "                                    FL.FileName,"
        SQL = SQL & vbCrLf & "                                    FL.FileSize,"
        SQL = SQL & vbCrLf & "                                    FL.[Checksum],"
        SQL = SQL & vbCrLf & "                                    MIN(FP.FolderID)    OVER(PARTITION BY OID.ObjectIRISID,FL.FileName,FL.FileSize,FL.[Checksum])           MinFolderID,"
        SQL = SQL & vbCrLf & "                                    RANK()              OVER(PARTITION BY OID.ObjectIRISID,FL.FileName,FL.FileSize,FL.[Checksum]"
        SQL = SQL & vbCrLf & "                                                                ORDER BY     SourceFolder)                                                  CopyNumber"
        SQL = SQL & vbCrLf & "                        FROM        #FolderPath                                                                                             FP"
        SQL = SQL & vbCrLf & "                        LEFT JOIN   HRC.DocumentMigrationFiles                                                                              FL"
        SQL = SQL & vbCrLf & "                        ON          FL.FolderID = FP.FolderID"
        SQL = SQL & vbCrLf & "                        ------------------------------------------------------------------------------------------------------------------------------------------------"
        SQL = SQL & vbCrLf & "                        --  Use the OtherIdentifier mapping to identify the IRIS object"
        SQL = SQL & vbCrLf & "                        ------------------------------------------------------------------------------------------------------------------------------------------------"
        SQL = SQL & vbCrLf & "                        LEFT JOIN   #GetIRISObjectID                                                                                        OID"
        SQL = SQL & vbCrLf & "                        ON          OID.LegacyID = FP.FolderName COLLATE DATABASE_DEFAULT"
        SQL = SQL & vbCrLf & "                        ------------------------------------------------------------------------------------------------------------------------------------------------"
        SQL = SQL & vbCrLf & "                        --  Identify the Regime associated with the Authorisation (Regimes are object type 12 and are linked as Regime Subject)"
        SQL = SQL & vbCrLf & "                        ------------------------------------------------------------------------------------------------------------------------------------------------"
        SQL = SQL & vbCrLf & "                        OUTER APPLY IRIS[ENV].dbo.GetSimpleLinksWithoutSecurity(OID.IRISObjectID)                                            AOR"
        SQL = SQL & vbCrLf & "                        LEFT JOIN   IRIS[ENV].dbo.IRISObject                                                                                 OBJ"
        SQL = SQL & vbCrLf & "                        ON          OBJ.ID = AOR.LinkedIRISObjectID"
        SQL = SQL & vbCrLf & "                        AND         OBJ.ObjectTypeID = OID.ObjectTypeID)                                                                    FILES"
        SQL = SQL & vbCrLf & "                        ------------------------------------------------------------------------------------------------------------------------------------------------"
        SQL = SQL & vbCrLf & "            WHERE       FILES.CopyNumber = 1)                                                                                               MAIN"
        SQL = SQL & vbCrLf & "LEFT JOIN   #FolderPath                                                                                                                     FP"
        SQL = SQL & vbCrLf & "ON          FP.FolderID = MAIN.MinFolderID"
        SQL = SQL & vbCrLf & "ORDER BY    ObjectIRISID,"
        SQL = SQL & vbCrLf & "            NewFileName"
        SQL = SQL & vbCrLf & "    -------------------------------------------------------------------------------------------------------------------------------------------------------------------"
        SQL = SQL & vbCrLf & "DROP TABLE IF EXISTS #Folder"
        SQL = SQL & vbCrLf & "DROP TABLE IF EXISTS #FolderPath"
        SQL = SQL & vbCrLf & "DROP TABLE IF EXISTS #GetIRISObjectID"
        SQL = SQL & vbCrLf & "    -------------------------------------------------------------------------------------------------------------------------------------------------------------------"
        Return SQL.Replace("IRIS[ENV]", My.Settings.IRIS_Database)
    End Function

    Private Function GetSQL_IdentifyDuplicates() As String
        Dim SQL As String = ""
        '------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        '   The intention here is to identify files that will be duplicates when we copy them to the historical Regime Activity folder
        '------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        SQL = SQL & vbCrLf & "/***********************************************************************************************************************************************"
        SQL = SQL & vbCrLf & "*   Script:         Identify Duplicates"
        SQL = SQL & vbCrLf & "*   Description:    The purpose of this script is to identify all files that are potentially duplicates of each other, using filename and"
        SQL = SQL & vbCrLf & "*                   file size, so that the application can iterate through them and calculate a CRC hash which can be used a a more robust"
        SQL = SQL & vbCrLf & "*                   discriminator. Initially, file modified date was included in the discriminator for potential duplicatesalso included but"
        SQL = SQL & vbCrLf & "*                   was found to be unreliable."
        SQL = SQL & vbCrLf & "*                   The script is based on the parent Regime that will be the final destination for the files and identifies around 5,300"
        SQL = SQL & vbCrLf & "*                   potential duplicate files out of 57,800 that will end up in a Regime. Calculating checksums for this many files takes"
        SQL = SQL & vbCrLf & "*                   about 20 minutes."
        SQL = SQL & vbCrLf & "***********************************************************************************************************************************************/"
        SQL = SQL & vbCrLf & "USE IRIS[ENV]_Migration"
        SQL = SQL & vbCrLf & "------------------------------------------------------------------------------------------------------------------------------------------------"
        SQL = SQL & vbCrLf & "DECLARE @MigrationID INTEGER =  1"
        SQL = SQL & vbCrLf & "DECLARE @FileMigrationID VARCHAR(50) = 'GROUNDWATER';"
        SQL = SQL & vbCrLf & "------------------------------------------------------------------------------------------------------------------------------------------------"
        SQL = SQL & vbCrLf & "--  Create a lookup table of all authorisation business ID's marked for migration and get their legacy ID (if any). Choose which, based on @GetLegacy"
        SQL = SQL & vbCrLf & "------------------------------------------------------------------------------------------------------------------------------------------------"
        SQL = SQL & vbCrLf & "IF OBJECT_ID(N'tempdb..#IRIS_Lookup',N'U') IS NULL"
        SQL = SQL & vbCrLf & "BEGIN"
        SQL = SQL & vbCrLf & "    --------------------------------------------------------------------------------------------------------------------------------------------"
        SQL = SQL & vbCrLf & "    SELECT      MAP.IRISObjectID,"
        SQL = SQL & vbCrLf & "                MAP.ManagementSiteIRISID,"
        SQL = SQL & vbCrLf & "                MAP.GroundwaterBoreID"
        SQL = SQL & vbCrLf & "    --------------------------------------------------------------------------------------------------------------------------------------------"
        SQL = SQL & vbCrLf & "    INTO        #IRIS_Lookup"
        SQL = SQL & vbCrLf & "    --------------------------------------------------------------------------------------------------------------------------------------------"
        SQL = SQL & vbCrLf & "    FROM       (SELECT      DISTINCT"
        SQL = SQL & vbCrLf & "                            MGS.IRISObjectID,"
        SQL = SQL & vbCrLf & "                            OBJ.BusinessID                                                                          ManagementSiteIRISID,"
        SQL = SQL & vbCrLf & "                            OID.OtherIdentifierText                                                                 GroundwaterBoreID"
        SQL = SQL & vbCrLf & "                FROM        IRIS[ENV].dbo.ManagementSite                                                             MGS"
        SQL = SQL & vbCrLf & "                LEFT JOIN   IRIS[ENV].dbo.IRISObject                                                                 OBJ"
        SQL = SQL & vbCrLf & "                ON          OBJ.ID = MGS.IRISObjectID"
        SQL = SQL & vbCrLf & "                LEFT JOIN   IRIS[ENV].dbo.OtherIdentifier                                                            OID"
        SQL = SQL & vbCrLf & "                ON          OID.IRISObjectID = OBJ.ID"
        SQL = SQL & vbCrLf & "                WHERE       OID.IsDeleted = 0"
        SQL = SQL & vbCrLf & "                AND         OID.IdentifierContextID = 14411)                                                        MAP"
        SQL = SQL & vbCrLf & "    ORDER BY    1"
        SQL = SQL & vbCrLf & "    --------------------------------------------------------------------------------------------------------------------------------------------"
        SQL = SQL & vbCrLf & "    CREATE INDEX idx_TextualID ON #IRIS_Lookup(GroundwaterBoreID)"
        SQL = SQL & vbCrLf & "    --------------------------------------------------------------------------------------------------------------------------------------------"
        SQL = SQL & vbCrLf & "END "
        SQL = SQL & vbCrLf & "------------------------------------------------------------------------------------------------------------------------------------------------"
        SQL = SQL & vbCrLf & "IF OBJECT_ID(N'tempdb..#Folders',N'U') IS NULL"
        SQL = SQL & vbCrLf & "BEGIN"
        SQL = SQL & vbCrLf & "    --------------------------------------------------------------------------------------------------------------------------------------------"
        SQL = SQL & vbCrLf & "    WITH"
        SQL = SQL & vbCrLf & "    --------------------------------------------------------------------------------------------------------------------------------------------"
        SQL = SQL & vbCrLf & "    Recurse     --  This Is the recursion that builds the path to the folder for each file"
        SQL = SQL & vbCrLf & "    --------------------------------------------------------------------------------------------------------------------------------------------"
        SQL = SQL & vbCrLf & "    AS     (SELECT      LKP.ManagementSiteIRISID,"
        SQL = SQL & vbCrLf & "                        FLD.FolderUID,"
        SQL = SQL & vbCrLf & "                        FLD.FolderID,"
        SQL = SQL & vbCrLf & "                        FLD.ParentFolderID,"
        SQL = SQL & vbCrLf & "                        FLD.GroundwaterBoreID,"
        SQL = SQL & vbCrLf & "                        REPLACE(FLD.FolderName,'''','''''')                                                         FolderName,"
        SQL = SQL & vbCrLf & "                        CAST(REPLACE(FLD.FolderPath,'''','''''') AS VARCHAR(1000))                                  FolderPath,"
        SQL = SQL & vbCrLf & "                        FLD.ErrorText"
        SQL = SQL & vbCrLf & "            FROM       (SELECT      FLD.ID                                                                          FolderUID,"
        SQL = SQL & vbCrLf & "                                    FLD.FolderID,"
        SQL = SQL & vbCrLf & "                                    FLD.ParentFolderID,"
        SQL = SQL & vbCrLf & "                                    FLD.FolderName                                                                  GroundwaterBoreID,"
        SQL = SQL & vbCrLf & "                                    FLD.FolderName,"
        SQL = SQL & vbCrLf & "                                    CAST(FLD.FolderName AS VARCHAR(1000))                                           FolderPath,"
        SQL = SQL & vbCrLf & "                                    FLD.ErrorText"
        SQL = SQL & vbCrLf & "                        FROM        HRC.DocumentMigrationFolders                                                    FLD"
        SQL = SQL & vbCrLf & "                        CROSS APPLY IRIS[ENV].dbo.NumberedSplit(FLD.FolderName,'&')                                   ITM --  Handle compound root folder names"
        SQL = SQL & vbCrLf & "                        WHERE       FLD.FileMigrationID = @FileMigrationID"
        SQL = SQL & vbCrLf & "                        AND         ParentFolderID = 0)                                                             FLD"
        SQL = SQL & vbCrLf & "            LEFT JOIN   #IRIS_Lookup                                                                                LKP"
        SQL = SQL & vbCrLf & "            ON          LKP.GroundwaterBoreID = FLD.GroundwaterBoreID"
        SQL = SQL & vbCrLf & "            UNION ALL"
        SQL = SQL & vbCrLf & "            SELECT      A.ManagementSiteIRISID,"
        SQL = SQL & vbCrLf & "                        B.ID                                                                                        FolderUID,"
        SQL = SQL & vbCrLf & "                        B.FolderID,"
        SQL = SQL & vbCrLf & "                        B.ParentFolderID,"
        SQL = SQL & vbCrLf & "                        A.GroundwaterBoreID,"
        SQL = SQL & vbCrLf & "                        REPLACE(B.FolderName,'''','''''')                                                           FolderName,"
        SQL = SQL & vbCrLf & "                        CAST(A.FolderPath+'\'+B.FolderName AS VARCHAR(1000))                                        FolderPath,"
        SQL = SQL & vbCrLf & "                        B.ErrorText"
        SQL = SQL & vbCrLf & "            FROM        Recurse                                                                                     A"
        SQL = SQL & vbCrLf & "            JOIN        HRC.DocumentMigrationFolders                                                                B"
        SQL = SQL & vbCrLf & "            ON          B.ParentFolderID = A.FolderID"
        SQL = SQL & vbCrLf & "            AND         B.FileMigrationID = @FileMigrationID)"
        SQL = SQL & vbCrLf & "    --------------------------------------------------------------------------------------------------------------------------------------------"
        SQL = SQL & vbCrLf & "    SELECT * INTO #Folders FROM Recurse"
        SQL = SQL & vbCrLf & "    --------------------------------------------------------------------------------------------------------------------------------------------"
        SQL = SQL & vbCrLf & "    CREATE INDEX idx_FolderID   ON #Folders(FolderID)"
        SQL = SQL & vbCrLf & "    CREATE INDEX idx_TextualID  ON #Folders(GroundwaterBoreID)"
        SQL = SQL & vbCrLf & "    --------------------------------------------------------------------------------------------------------------------------------------------"
        SQL = SQL & vbCrLf & "END"
        SQL = SQL & vbCrLf & "------------------------------------------------------------------------------------------------------------------------------------------------"
        SQL = SQL & vbCrLf & "IF OBJECT_ID(N'tempdb..#AllFiles',N'U') IS NULL"
        SQL = SQL & vbCrLf & "BEGIN"
        SQL = SQL & vbCrLf & "    --------------------------------------------------------------------------------------------------------------------------------------------"
        SQL = SQL & vbCrLf & "    SELECT      R.ManagementSiteIRISID,"
        SQL = SQL & vbCrLf & "                R.FolderUID,"
        SQL = SQL & vbCrLf & "                R.FolderID,"
        SQL = SQL & vbCrLf & "                R.ParentFolderID,"
        SQL = SQL & vbCrLf & "                R.GroundwaterBoreID,"
        SQL = SQL & vbCrLf & "                R.FolderName,"
        SQL = SQL & vbCrLf & "                R.FolderPath,"
        SQL = SQL & vbCrLf & "                R.ErrorText,"
        SQL = SQL & vbCrLf & "                REPLACE(F.FileName,'''','''''')                                                             FileName,"
        SQL = SQL & vbCrLf & "                F.FileSize,"
        SQL = SQL & vbCrLf & "                F.Modified"
        SQL = SQL & vbCrLf & "    --------------------------------------------------------------------------------------------------------------------------------------------"
        SQL = SQL & vbCrLf & "    INTO        #AllFiles"
        SQL = SQL & vbCrLf & "    --------------------------------------------------------------------------------------------------------------------------------------------"
        SQL = SQL & vbCrLf & "    FROM        #Folders                                                                                    R"
        SQL = SQL & vbCrLf & "    LEFT JOIN   HRC.DocumentMigrationFiles                                                                  F"
        SQL = SQL & vbCrLf & "    ON          F.FolderID = R.FolderID"
        SQL = SQL & vbCrLf & "    WHERE       F.FileMigrationID = @FileMigrationID"
        SQL = SQL & vbCrLf & "    AND         F.[Filename] IS NOT NULL"
        SQL = SQL & vbCrLf & "    AND         R.ManagementSiteIRISID IS NOT NULL"
        SQL = SQL & vbCrLf & "    --------------------------------------------------------------------------------------------------------------------------------------------"
        SQL = SQL & vbCrLf & "    CREATE INDEX idx_RegimeID ON #AllFiles(ManagementSiteIRISID,FileName,FileSize)"
        SQL = SQL & vbCrLf & "    CREATE INDEX idx_LegacyID ON #AllFiles(GroundwaterBoreID)"
        SQL = SQL & vbCrLf & "    --------------------------------------------------------------------------------------------------------------------------------------------"
        SQL = SQL & vbCrLf & "END"
        SQL = SQL & vbCrLf & "------------------------------------------------------------------------------------------------------------------------------------------------"
        SQL = SQL & vbCrLf & "--  Determine the potential duplicates by counting up files with the same name and size per Regime, then match these files to the actual"
        SQL = SQL & vbCrLf & "--  files associated with the Regime to identify all the files where we need to calculate a checksum as a final discriminator"
        SQL = SQL & vbCrLf & "------------------------------------------------------------------------------------------------------------------------------------------------"
        SQL = SQL & vbCrLf & "SELECT      DISTINCT"
        SQL = SQL & vbCrLf & "            RF.ManagementSiteIRISID,"
        SQL = SQL & vbCrLf & "            RF.FolderID,"
        SQL = SQL & vbCrLf & "            RF.FileName,"
        SQL = SQL & vbCrLf & "            RF.FileSize,"
        SQL = SQL & vbCrLf & "            RF.FolderPath"
        SQL = SQL & vbCrLf & "FROM       (SELECT      DISTINCT"
        SQL = SQL & vbCrLf & "                        ManagementSiteIRISID,"
        SQL = SQL & vbCrLf & "                        FileName,"
        SQL = SQL & vbCrLf & "                        FileSize,"
        SQL = SQL & vbCrLf & "                        COUNT(DISTINCT FolderID)                                                                DuplicateCount"
        SQL = SQL & vbCrLf & "            FROM        #AllFiles"
        SQL = SQL & vbCrLf & "            GROUP BY    ManagementSiteIRISID,"
        SQL = SQL & vbCrLf & "                        FileName,"
        SQL = SQL & vbCrLf & "                        FileSize)                                                                               MAIN"
        SQL = SQL & vbCrLf & "LEFT JOIN   #AllFiles                                                                                           RF"
        SQL = SQL & vbCrLf & "ON          RF.ManagementSiteIRISID = MAIN.ManagementSiteIRISID"
        SQL = SQL & vbCrLf & "AND         RF.FileName     = MAIN.FileName"
        SQL = SQL & vbCrLf & "AND         RF.FileSize     = MAIN.FileSize"
        SQL = SQL & vbCrLf & "WHERE       MAIN.DuplicateCount > 1"
        SQL = SQL & vbCrLf & "ORDER BY    ManagementSiteIRISID,"
        SQL = SQL & vbCrLf & "            FileName,"
        SQL = SQL & vbCrLf & "            FolderPath"
        SQL = SQL & vbCrLf & "------------------------------------------------------------------------------------------------------------------------------------------------"
        SQL = SQL & vbCrLf & "DROP TABLE IF EXISTS #Folders"
        SQL = SQL & vbCrLf & "DROP TABLE IF EXISTS #AllFiles"
        SQL = SQL & vbCrLf & "DROP TABLE IF EXISTS #RegimeFiles"
        SQL = SQL & vbCrLf & "DROP TABLE IF EXISTS #IRIS_Lookup"
        SQL = SQL & vbCrLf & "------------------------------------------------------------------------------------------------------------------------------------------------"
        Return SQL.Replace("IRIS[ENV]", My.Settings.IRIS_Database)
    End Function

    Private Sub cmbMigrationID_KeyUp(sender As Object, e As KeyEventArgs) Handles cmbFileMigrationID.KeyUp
        If e.KeyCode = Keys.Enter Then
            My.Settings.FileMigrationID = cmbFileMigrationID.Text
            ValidateFileMigrationID(cmbFileMigrationID.Text)
            UpdatePix()
        End If
    End Sub

    Private Sub ValidateFileMigrationID(ByVal FileMigrationID As String)
        Dim msg As String = ""
        Dim wks As New Workspace
        Dim CheckFileMigrationID As String = wks.ExecuteScalar("SELECT FileMigrationID FROM HRC.DocumentMigrationProcess WHERE FileMigrationID = '" & FileMigrationID & "'")
        If IsNothing(CheckFileMigrationID) Then
            If MsgBox("Create new migration?", MsgBoxStyle.OkCancel, "New migration") = MsgBoxResult.Ok Then
                wks.ExecuteNonQuery("INSERT INTO hrc.DocumentMigrationProcess(MigrationID,FileMigrationID,SourceFolder,TargetFolder) VALUES(" & My.Settings.MigrationID & ",'" & FileMigrationID.Replace("'", "''") & "','" & txtSourceRoot.Text.Replace("'", "''") & "','" & txtIRIS_Root.Text.Replace("'", "''") & "')")
                UpdateTask(1, msg)
            End If
        Else
            txtSourceRoot.Text = wks.ExecuteScalar("SELECT SourceFolder FROM hrc.DocumentMigrationProcess WHERE FileMigrationID = '" & My.Settings.FileMigrationID & "'")
            txtIRIS_Root.Text = wks.ExecuteScalar("SELECT TargetFolder FROM hrc.DocumentMigrationProcess WHERE FileMigrationID = '" & My.Settings.FileMigrationID & "'")
        End If
        wks.Dispose()
    End Sub

    Private Sub cmbMigrationID_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbFileMigrationID.SelectedIndexChanged
        If Loading Then Exit Sub

        My.Settings.FileMigrationID = cmbFileMigrationID.SelectedValue
        ValidateFileMigrationID(My.Settings.FileMigrationID)
        UpdatePix()
    End Sub

#End Region

End Class
