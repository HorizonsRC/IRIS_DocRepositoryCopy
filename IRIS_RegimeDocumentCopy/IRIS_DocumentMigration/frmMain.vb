Imports System.IO
Imports IWshRuntimeLibrary
Imports System.Data
Imports System.Data.SqlClient
Imports System.Security.AccessControl
Imports System.Security.Cryptography
Imports System.Runtime.Remoting

Public Class frmMain
    Private Busy As Boolean
    Private Shared FolderID As Integer = 0
    Private Shared DM_Files As DataTable
    Private Shared Loading As Boolean
    Private wks As Workspace
    Private OriginalCellValue As String

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

        My.Settings.Save()

        wks.Dispose()
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
        ValidateServer()

        If My.Settings.IRIS_Server.Length > 0 Then
            EnsureTables(wks)
            'VerifyMigrationID(wks)
        End If

        LoadMigrationCombo()
        LoadSourceFolderList()

        txtIRIS_Root.Text = My.Settings.IRIS_RootFolder

        UpdateButtons()

        Loading = False
        lblStatus.Text = "Ready"
    End Sub
    Private Sub cmbMigrationID_SelectedIndexChanged_1(sender As Object, e As EventArgs) Handles cmbMigrationID.SelectedIndexChanged
        If cmbMigrationID.SelectedIndex() > 0 Then
            My.Settings.MigrationID = cmbMigrationID.SelectedItem("ID")
            LoadSourceFolderList()
        End If
    End Sub
    Private Sub cmbMigrationID_KeyUp(sender As Object, e As KeyEventArgs) Handles cmbMigrationID.KeyUp
        If e.KeyCode = Keys.Enter Then
            Dim MigrationName As String = cmbMigrationID.Text
            If wks.ExecuteScalarMigration($"SELECT ISNULL(ID,0) MigrationID FROM {wks.MigrationDatabase}.hrc.MigrationInstance WHERE InstanceName = '{cmbMigrationID.Text}'") = 0 Then
                My.Settings.MigrationID = wks.ExecuteScalarMigration($"INSERT {wks.MigrationDatabase}.hrc.MigrationInstance(InstanceName,InstanceDate,IsComplete) VALUES('{MigrationName}',GETDATE(),0) SELECT CAST(SCOPE_IDENTITY() AS INTEGER)")
            End If
            LoadMigrationCombo()
            LoadSourceFolderList()
        End If
    End Sub
    Private Sub txtIRIS_Server_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtIRIS_Server.KeyUp
        If e.KeyCode = Keys.Enter Then
            My.Settings.IRIS_Server = txtIRIS_Server.Text
            ValidateServer()
            UpdateButtons()
        End If
    End Sub
#End Region

#Region "Control Events"
    Private Sub cmdBrowseIRIS_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdBrowseIRIS.Click
        Dim myFolder As String

        myFolder = GetFolder("Identify a Root Folder for IRIS Documents", My.Settings.IRIS_RootFolder)
        If myFolder.Substring(myFolder.Length - 1, 1) <> "\" Then myFolder = myFolder & "\"
        If Len(myFolder) > 0 AndAlso IO.Directory.Exists(myFolder) Then
            txtIRIS_Root.Text = myFolder
            My.Settings.IRIS_RootFolder = myFolder
            wks.ExecuteNonQueryMigration($"MERGE {wks.MigrationDatabase}.hrc.DocumentMigrationProcess TGT USING (SELECT {My.Settings.MigrationID} MigrationID,'' SourceFolder,{myFolder} TargetFolder) SRC ON FLD.MigrationID = TGT.MigrationID WHEN MATCHED THEN UPDATE SET TargetFolder = SRC.TargetFolder WHEN NOT MATCHED BY TARGET THEN INSERT(MigrationID,SourceFolder,TargetFolder) VALUES(SRC.MigrationID,SRC.SourceFolder,SRC.TargetFolder);")
        End If
    End Sub
#End Region

#Region "Shared Methods"
    Private Function ValidateServer() As String
        Dim msg As String = ""

        If Not TestSQLServerConnection(My.Settings.IRIS_Server) Then
            msg = "The IRIS SQL server name you have entered can't be found"
            MsgBox("The SQL server name you have entered can't be found. Enter a valid server name and try again", vbExclamation + vbOKOnly, "Server not found")
            txtIRIS_Database.Text = ""
            txtMigration_Database.Text = ""
        Else
            wks = New Workspace(My.Settings.IRIS_Server)
            If wks.MigrationDatabase = "" Then
                msg = "The IRIS Migration database was not found on the nominated IRIS server"
                MsgBox("This application requires the IRIS Migration database to be present on the selected server and it is not found on '" & My.Settings.IRIS_Server & "'. Please select a valid IRIS server", vbExclamation + vbOKOnly, "IRIS_Migration Not Found")
            Else
                txtIRIS_Database.Text = wks.IRISDatabase
                txtMigration_Database.Text = wks.MigrationDatabase
            End If
        End If
        Return msg
    End Function
    Private Sub LoadMigrationCombo()
        Dim ds As New DataSet
        wks.FillDatasetMigration(ds, "SELECT ID, InstanceName from hrc.MigrationInstance")
        cmbMigrationID.ValueMember = "ID"
        cmbMigrationID.DisplayMember = "InstanceName"
        cmbMigrationID.DataSource = ds.Tables(0)
        Dim q = From r In ds.Tables(0).Rows Where r("ID") = My.Settings.MigrationID Select r
        If cmbMigrationID.Items.Count > 0 AndAlso q.Count > 0 Then
            cmbMigrationID.SelectedValue = q(0)("ID")
        End If
    End Sub
    Private Sub btnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        wks.ExecuteNonQueryMigration($"INSERT {wks.MigrationDatabase}.hrc.DocumentMigrationProcess(SourceFolder,TargetFolder,MigrationID) VALUES('','{My.Settings.IRIS_RootFolder}',{My.Settings.MigrationID})")
        LoadSourceFolderList()
    End Sub
    Private Sub lstSourceRootFolder_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles lstSourceRootFolder.CellClick
        If e.RowIndex < 0 Then Exit Sub

        Dim FileMigrationID As Integer = lstSourceRootFolder.DataSource.Rows(e.RowIndex)("FileMigrationID")
        Select Case True
            Case e.ColumnIndex = lstSourceRootFolder.Columns("Browse").Index
                Dim myFolder As String
                Dim CellFolder As String = lstSourceRootFolder.DataSource.Rows(e.RowIndex)("SourceFolder")
                myFolder = GetFolder("Identify the Root Folder for Documents", CellFolder)
                If Len(myFolder) > 0 AndAlso IO.Directory.Exists(myFolder) AndAlso myFolder <> CellFolder Then
                    If MsgBox("A new source folder has been selected or added. You will need to re-build the mapping tables after this change. Continue?", vbQuestion + vbYesNo, "Confirm Change") = vbYes Then
                        lstSourceRootFolder.DataSource.Rows(e.RowIndex)("SourceFolder") = myFolder
                        wks.ExecuteNonQueryMigration($"UPDATE {wks.MigrationDatabase}.hrc.DocumentMigrationProcess SET SourceFolder = '{lstSourceRootFolder.DataSource.Rows(e.RowIndex)("SourceFolder")}' WHERE FileMigrationID = {FileMigrationID}")
                    End If
                End If
            Case e.ColumnIndex = lstSourceRootFolder.Columns("Delete").Index
                If MsgBox("Delete this row and associated file and folder data?", MsgBoxStyle.YesNo, "Confirm deletion") = MsgBoxResult.Yes Then
                    wks.ExecuteNonQueryMigration($"DELETE FROM {wks.MigrationDatabase}.hrc.DocumentMigrationFiles WHERE FileMigrationID = {FileMigrationID}")
                    wks.ExecuteNonQueryMigration($"DELETE FROM {wks.MigrationDatabase}.hrc.DocumentMigrationFolders WHERE FileMigrationID = {FileMigrationID}")
                    wks.ExecuteNonQueryMigration($"DELETE FROM {wks.MigrationDatabase}.hrc.DocumentMigrationProcess WHERE FileMigrationID = {FileMigrationID}")
                    LoadSourceFolderList()
                End If
            Case e.ColumnIndex = lstSourceRootFolder.Columns("UseLegacyCode").Index
                Dim UseLegacyCode As Boolean = lstSourceRootFolder.DataSource.Rows(e.RowIndex)("UseLegacyCode")
                wks.ExecuteNonQueryMigration($"UPDATE {wks.MigrationDatabase}.hrc.DocumentMigrationProcess SET UseLegacyCode = {IIf(UseLegacyCode, 0, 1)} WHERE FileMigrationID = {FileMigrationID}")
        End Select
    End Sub
    Private Sub lstSourceRootFolder_CellBeginEdit(sender As Object, e As DataGridViewCellCancelEventArgs) Handles lstSourceRootFolder.CellBeginEdit
        If e.RowIndex < 0 Then Exit Sub
        OriginalCellValue = lstSourceRootFolder.Rows(e.RowIndex).Cells(e.ColumnIndex).Value
    End Sub
    Private Sub lstSourceRootFolder_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles lstSourceRootFolder.CellEndEdit
        If e.RowIndex < 0 Then Exit Sub

        Dim FileMigrationID As Integer = lstSourceRootFolder.DataSource.Rows(e.RowIndex)("FileMigrationID")
        Select Case True
            Case e.ColumnIndex = lstSourceRootFolder.Columns("SourceFolder").Index
                Dim CellFolder As String = lstSourceRootFolder.DataSource.Rows(e.RowIndex)("SourceFolder")
                If Len(CellFolder) > 0 AndAlso IO.Directory.Exists(CellFolder) AndAlso CellFolder <> OriginalCellValue Then
                    wks.ExecuteNonQueryMigration($"UPDATE {wks.MigrationDatabase}.hrc.DocumentMigrationProcess SET SourceFolder = '{CellFolder}', TargetFolder = '{My.Settings.IRIS_RootFolder}' WHERE FileMigrationID = {FileMigrationID}")
                End If
        End Select
    End Sub
    Private Sub LoadSourceFolderList()
        lstSourceRootFolder.DataSource = Nothing
        lstSourceRootFolder.Columns.Clear()

        Dim dt As New DataTable
        wks.FillDataTableMigration(dt, $"SELECT FileMigrationID,SourceFolder,UseLegacyCode FROM hrc.DocumentMigrationProcess WHERE MigrationID = {My.Settings.MigrationID}")
        lstSourceRootFolder.DataSource = dt
        For c = 0 To lstSourceRootFolder.Columns.Count - 1
            If c = 0 Then lstSourceRootFolder.Columns(c).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            If c = 1 Then
                lstSourceRootFolder.Columns(c).AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            Else
                lstSourceRootFolder.Columns(c).AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
            End If
        Next
        Dim btn As New DataGridViewButtonColumn()
        btn.HeaderText = ""
        btn.Name = "Browse"
        btn.Text = "..."
        btn.UseColumnTextForButtonValue = True
        lstSourceRootFolder.Columns.Add(btn)

        Dim del As New DataGridViewButtonColumn()
        del.HeaderText = ""
        del.Name = "Delete"
        del.Text = "X"
        del.UseColumnTextForButtonValue = True
        lstSourceRootFolder.Columns.Add(del)
    End Sub
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

    Private Sub RecurseFolder(ByVal FileMigrationID As Integer, ByVal ParentFolderID As Integer, ByVal Level As Integer, ByVal FolderName As String, ByRef FolderTable As DataTable, ByRef FileTable As DataTable)
        FolderID = FolderID + 1
        Dim ThisFolderID As Integer = FolderID

        Try
            Dim Dir As DirectoryInfo = New DirectoryInfo(FolderName)
            Dim Files As FileInfo() = Dir.GetFiles()
            Dim FolderSize As Long = 0
            For i As Integer = 0 To Files.Length - 1
                If Files(i).Name.Substring(1, 1) <> "$" AndAlso Files(i).Name <> "Thumbs.db" Then
                    FolderSize = FolderSize + Files(i).Length
                    FileTable.Rows.Add(FileMigrationID, FolderID, Files(i).Name, Files(i).Length, Files(i).LastWriteTime, DBNull.Value, Now())
                End If
            Next

            FolderTable.Rows.Add(FileMigrationID, FolderID, ParentFolderID, Level, IO.Path.GetFileName(FolderName), IO.Directory.GetDirectories(FolderName).GetLength(0), FolderSize, Files.Length, DBNull.Value)

            For Each D In IO.Directory.GetDirectories(FolderName)
                RecurseFolder(FileMigrationID, ThisFolderID, Level + 1, D, FolderTable, FileTable)
            Next
        Catch ex As Exception
            FolderTable.Rows.Add(FileMigrationID, FolderID, ParentFolderID, Level, IO.Path.GetFileName(FolderName), 1, 0, 0, ex.Message.Replace("'", "''"))
        End Try
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

    Private Sub UpdateButtons()

        ToolTips.SetToolTip(btnCreateWorkspace, "Creates tables and views in the nominated workspace database to facilitate IRIS document migration")
        btnCreateWorkspace.Enabled = (My.Settings.IRIS_Server.Length > 0)
        btnCreateWorkspace.ForeColor = Color.Black

        ToolTips.SetToolTip(cmdPopulateFolderTable, "Reads all sub-folders of the nominated source documents root folder and populates a list of folders to be migrated")
        cmdPopulateFolderTable.Enabled = False
        cmdPopulateFolderTable.ForeColor = Color.Black

        ToolTips.SetToolTip(cmdIdentifyDuplicates, "Checks all likely duplicates using filename, file size and file date for files associated with each authorisation and calculates a CRC hash as a final check")
        cmdIdentifyDuplicates.Enabled = False
        cmdIdentifyDuplicates.ForeColor = Color.Black

        txtIRIS_Root.Enabled = False
        cmdBrowseIRIS.Enabled = False

        cmdMigrateFiles.Enabled = False
        cmdMigrateFiles.ForeColor = Color.Black

        If TestSQLServerConnection(My.Settings.IRIS_Server) Then
            Try
                Try
                    Dim rdr As SqlDataReader
                    rdr = wks.ExecuteReaderMigration($"SELECT TaskID, ISNULL(ErrorText,'') AS ErrorText FROM HRC.DocumentMigrationProcessTask WHERE MigrationID = {My.Settings.MigrationID} ORDER BY TaskID")

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

                                    txtIRIS_Root.Enabled = True
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
                    rdr.Close()
                    rdr = Nothing
                Catch ex As Exception
                    MsgBox("Required workspace tables have not yet been created.", vbInformation + vbOKOnly, "Workspace Not Prepared")
                End Try
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
        If msg.Length = 0 Then msg = EnsureTables(wks)
        VerifyMigrationID(wks)
        ResetTasksFrom(wks, 1)
        UpdateTask(wks, 1, msg)
        UpdateButtons()
    End Sub
    '-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ' Task = 2  Read all folder and sub-folder names recursively and save all to DM_Folders 
    '-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    Private Sub cmdPopulateFolderTable_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdPopulateFolderTable.Click
        Dim f As Integer = 0
        Dim fMax As Integer
        Dim Percent As Integer = 0

        If MsgBox("This process will recursively read the source folders and subfolders, recording the level, folder name and number of files contained" & vbCrLf & vbCrLf & "The process will replace existing data and may take several minutes to complete. Continue?", vbInformation + vbYesNo, "Confirm Continue") = vbNo Then
            Exit Sub
        End If
        Application.DoEvents()
        ResetTasksFrom(wks, 2)
        UpdateButtons()
        '---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        '   Create a data table in RAM to collect details for each folder (saved to hard storage on completion)
        '---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        Dim Folders As New DataTable("Folders")
        Folders.Columns.Add("FileMigrationID", Type.GetType("System.Int64"))
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
        Files.Columns.Add("FileMigrationID", Type.GetType("System.Int64"))
        Files.Columns.Add("FolderID", Type.GetType("System.Int64"))
        Files.Columns.Add("FileName", Type.GetType("System.String"))
        Files.Columns.Add("FileSize", Type.GetType("System.Int64"))
        Files.Columns.Add("Modified", Type.GetType("System.DateTime"))
        Files.Columns.Add("Checksum", Type.GetType("System.String"))
        Files.Columns.Add("DateTime", Type.GetType("System.DateTime"))
        '---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        Dim myFolderID As Integer = wks.ExecuteScalarMigration($"SELECT ISNULL((SELECT MAX(FolderID) FROM hrc.DocumentMigrationFolders),0)") 'Reset the folder ID to start at the max of the current list of folder ID's
        Dim msg As String = ""
        Dim rdr As SqlDataReader = wks.ExecuteReaderMigration($"SELECT FileMigrationID FROM {wks.MigrationDatabase}.hrc.DocumentMigrationProcess WHERE MigrationID = {My.Settings.MigrationID}")
        If rdr.HasRows Then
            While rdr.Read()
                Dim FileMigrationID As Integer = rdr("FileMigrationID")
                Dim SQL As String = $"
                /***********************************************************************************************************************************************************************
                *  Script :         Identify sought folder names based on authorisation business ID or legacy ID
                *  Description:     If we're migrating R2D2 files then we need to use the legacy ID as the folder name, else the authorisation business ID
                ***********************************************************************************************************************************************************************/
                DECLARE @UseLegacyCode BIT = (SELECT UseLegacyCode FROM {wks.MigrationDatabase}.hrc.DocumentMigrationProcess WHERE FileMigrationID = {FileMigrationID})
                SELECT      ''''+STUFF((SELECT ''','''+FolderName FROM (
                            SELECT      DISTINCT
                                        CASE 
                                        WHEN @UseLegacyCode = 1 
                                        THEN OID.OtherIdentifierText 
                                        ELSE ATO.BusinessID END                     FolderName
                            FROM        {wks.MigrationDatabase}.hrc.MapRegime             MAP
                            LEFT JOIN   IRISObject                                  ATO
                            ON          ATO.BusinessID = MAP.AuthorisationIRISID
                            LEFT JOIN   OtherIdentifier                             OID
                            ON          OID.IRISObjectID = ATO.ID
                            WHERE       OID.IsDeleted    = 0
                            AND         OID.IdentifierContextID = 427
                            AND         MAP.MigrationID = 5)                        FLD
                            ORDER BY    FolderName
                            FOR XML     PATH('')),1,3,'')+''''                      FolderList"

                Dim myFolderList As String = IIf(IsDBNull(wks.ExecuteScalarIRIS(SQL)), "", wks.ExecuteScalarIRIS(SQL))              'Get a comma-delimited list of folders to be included in the recursive scan

                lblStatus.Text = "Processing..."
                Application.DoEvents()
                Dim SourceRootFolder As String = wks.ExecuteScalarMigration($"SELECT SourceFolder FROM {wks.MigrationDatabase}.hrc.DocumentMigrationProcess WHERE FileMigrationID = {FileMigrationID}")
                Dim DR As String() = IO.Directory.GetDirectories(SourceRootFolder)                                                  'Get the list of folders to be processed
                fMax = DR.GetUpperBound(0)                                                                                          'Get the number of directories to be processed
                For f = 0 To fMax
                    If Int(f * 100 / fMax) > Percent Then
                        Percent = f * 100 / fMax
                        lblStatus.Text = "Processing... " & f.ToString("N0") & " folders completed out of " & fMax.ToString("N0") & " total folders (" & Percent.ToString & "%)"
                        Application.DoEvents()
                    End If
                    If myFolderList.Contains("'" & IO.Path.GetFileName(DR(f)) & "'") Then
                        RecurseFolder(FileMigrationID, 0, 0, DR(f), Folders, Files)
                    End If
                Next f
            End While
            rdr.Close()
        End If
        '---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        '   Save our RAM data table to hard storage
        '---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        If Folders.Rows.Count > 0 AndAlso MsgBox("Confirm that you want to overwrite existing data?", vbYesNo, "Confirm Overwrite") = MsgBoxResult.Yes Then
            Try                                                                                                                     'Save the results to our workspace database
                wks.ExecuteNonQueryMigration($"ALTER INDEX idx_ID ON HRC.DocumentMigrationFolders DISABLE")                         'This is necessary for larger files as the indexes slow down the bulk copy and it may time-out
                wks.ExecuteNonQueryMigration($"ALTER INDEX idx_FolderID ON HRC.DocumentMigrationFolders DISABLE")
                wks.ExecuteNonQueryMigration($"ALTER INDEX idx_ParentFolderID ON HRC.DocumentMigrationFolders DISABLE")
                wks.ExecuteNonQueryMigration($"DELETE FROM HRC.DocumentMigrationFolders WHERE FileMigrationID IN (SELECT FileMigrationID FROM {wks.MigrationDatabase}.hrc.DocumentMigrationProcess WHERE MigrationID = {My.Settings.MigrationID})")     'Delete existing records in the same source group

                wks.BulkCopy.ColumnMappings.Clear()

                For i As Integer = 0 To Folders.Columns.Count - 1
                    wks.BulkCopy.ColumnMappings.Add(Folders.Columns(i).ColumnName, Folders.Columns(i).ColumnName)
                Next

                wks.BulkCopy.DestinationTableName = $"HRC.DocumentMigrationFolders"
                wks.BulkCopy.WriteToServer(Folders)

                wks.ExecuteNonQueryMigration($"ALTER INDEX idx_ID ON HRC.DocumentMigrationFolders REBUILD")                         'And now reinstate the indexes
                wks.ExecuteNonQueryMigration($"ALTER INDEX idx_FolderID ON HRC.DocumentMigrationFolders REBUILD")
                wks.ExecuteNonQueryMigration($"ALTER INDEX idx_ParentFolderID ON HRC.DocumentMigrationFolders REBUILD")

                '-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                wks.ExecuteNonQueryMigration($"ALTER INDEX idx_ID ON HRC.DocumentMigrationFiles DISABLE")                           'This is necessary for larger files as the indexes slow down the bulk copy and it may time-out
                wks.ExecuteNonQueryMigration($"ALTER INDEX idx_FolderID ON HRC.DocumentMigrationFiles DISABLE")
                wks.ExecuteNonQueryMigration($"DELETE FROM HRC.DocumentMigrationFiles WHERE FileMigrationID IN (SELECT FileMigrationID FROM {wks.MigrationDatabase}.hrc.DocumentMigrationProcess WHERE MigrationID = {My.Settings.MigrationID})")       'Delete existing records
                wks.BulkCopy.ColumnMappings.Clear()
                wks.ExecuteNonQueryMigration($"IF COL_LENGTH('HRC.DocumentMigrationFiles', 'Checksum') IS NULL ALTER TABLE HRC.DocumentMigrationFiles ADD [Checksum] VARCHAR(100) NULL") 'Make sure the new Checksum column exists
                wks.ExecuteNonQueryMigration($"IF COL_LENGTH('HRC.DocumentMigrationFiles', 'Modified') IS NULL ALTER TABLE HRC.DocumentMigrationFiles ADD [Modified] DATETIME     NULL") 'Make sure the new Modified column exists
                For i As Integer = 0 To Files.Columns.Count - 1                                                                     'NOTE: The column mapping is case sensitive
                    wks.BulkCopy.ColumnMappings.Add(Files.Columns(i).ColumnName, Files.Columns(i).ColumnName)
                Next

                wks.BulkCopy.DestinationTableName = $"HRC.DocumentMigrationFiles"
                wks.BulkCopy.WriteToServer(Files)

                wks.ExecuteNonQueryMigration($"ALTER INDEX idx_ID ON HRC.DocumentMigrationFiles REBUILD")                           'And now reinstate the indexes
                wks.ExecuteNonQueryMigration($"ALTER INDEX idx_FolderID ON HRC.DocumentMigrationFiles REBUILD")
            Catch ex As Exception
                msg = ex.Message
            End Try
        End If
        '---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        UpdateTask(wks, 2, msg)
        UpdateButtons()

        lblStatus.Text = "Ready"
    End Sub
    '-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ' Task = 3   Identify Duplicate Files
    '-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    Private Sub cmdIdentifyDuplicates_Click(sender As Object, e As EventArgs) Handles cmdIdentifyDuplicates.Click
        Dim r As Integer
        Dim rMax As Integer
        Dim Percent As Integer
        Dim ErrorCount As Integer = 0

        lblStatus.Text = "Identifying and loading data for potential duplicate files"
        Application.DoEvents()
        ResetTasksFrom(wks, 3)
        UpdateButtons()

        Dim rdr As SqlDataReader = wks.ExecuteReaderMigration($"SELECT FileMigrationID, SourceFolder FROM {wks.MigrationDatabase}.hrc.DocumentMigrationProcess WHERE MigrationID = {My.Settings.MigrationID}")
        If rdr.HasRows Then
            While rdr.Read()
                Dim FileMigrationID As Integer = rdr("FileMigrationID")
                Dim SourceFolder As String = rdr("SourceFolder")
                '---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                '   The intention here is to identify files that will be duplicates when we copy them to the historical Regime Activity folder. Not only are there duplicate files within
                '   any individual Authorisation sub-folders in R2D2, but since a Regime can have many Authorisations associated with it, some duplicates will occur where the same file
                '   has been saved to different Authorisations in R2D2. Suspected duplicates are files with the same name, size and time-stamp. These are the ones identified by the script 
                '   below and the associated procedure calculates and saves a CRC hash for these files as a final tie-breaker. Files that have the same name but are not duplicates will be
                '   copied with a version number based on the file date
                '---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                Dim dt As New DataTable("Duplicates")
                wks.FillDataTableMigration(dt, GetSQL_IdentifyDuplicates(FileMigrationID))
                rMax = dt.Rows.Count - 1

                Dim myCheckSum As String
                Dim SHA As New SHA256Managed()

                lblStatus.Text = "Calculating file checksums for " & (rMax + 1).ToString & " potential duplicate files"
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
                        wks.ExecuteScalarMigration($"UPDATE HRC.DocumentMigrationFiles SET [Checksum] = '{myCheckSum}' WHERE FolderID = {dt.Rows(r)("FolderID").ToString} AND Filename = '{dt.Rows(r)("FileName").ToString.Replace("'", "''")}'")
                    Catch ex As Exception
                        ErrorCount += 1
                        wks.ExecuteScalarMigration($"UPDATE HRC.DocumentMigrationFiles SET ErrorText = '{ex.Message.Replace("'", "''")}' WHERE FolderID = {dt.Rows(r)("FolderID").ToString} AND Filename = '{dt.Rows(r)("FileName")}'")
                    End Try
                Next
                '-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                dt.Dispose()
            End While
            rdr.Close()
        End If
        '---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        UpdateTask(wks, 3, IIf(ErrorCount > 0, String.Format("{0} Errors out of {1} files tested", ErrorCount, rMax + 1), ""))
        UpdateButtons()

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
                & "The process may take several hours to complete depending on the number and size of files. Continue?",
                vbInformation + vbYesNo, "Confirm Copy Files") = vbNo Then
            Exit Sub
        End If
        lblStatus.Text = "Reading mapping information..."
        Application.DoEvents()
        '---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        '   Query the folder tree and files table to get the mapping from source to destination file path
        '---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        Dim SQL As String = GetSQL_DocumentMapping()
        Dim DT As New DataTable("DocumentMap")
        wks.FillDataTableMigration(DT, SQL)

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
                    wks.ExecuteScalarMigration($"UPDATE HRC.DocumentMigrationFiles SET Migrated = 1 WHERE FolderID = {DT.Rows(f)("MinFolderID").ToString} AND Filename = '{DT.Rows(f)("FileName").ToString.Replace("'", "''")}'")
                Catch ex As Exception
                    myLogger.AddLog("Error while copying file '" & IO.Path.Combine(DT.Rows(f)("SourceFolder"), "" & DT.Rows(f)("FileName")) & "' to '" & IO.Path.Combine(DT.Rows(f)("TargetFolder"), "" & DT.Rows(f)("NewFileName")) & "'. The error was " & ex.Message)
                    wks.ExecuteScalarMigration($"UPDATE HRC.DocumentMigrationFiles SET ErrorText = '{ex.Message.Replace("'", "''")}' WHERE FolderID = {DT.Rows(f)("MinFolderID").ToString} AND Filename = '{DT.Rows(f)("FileName").ToString.Replace("'", "''")}'")
                End Try
            End If
        Next
        '---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        DT.Dispose()
        '---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        UpdateTask(wks, 4, "")
        UpdateButtons()
        '---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        lblStatus.Text = "Ready"
    End Sub
    '-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ' Helper Functions
    '-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    Private Function GetSQL_DocumentMapping() As String
        Dim SQL As String = $"
        /***********************************************************************************************************************************************************************
        *   Script:         Identify Files to be Migrated
        *   Description:    The purpose of this script is to identify the source files to be migrated based on the Regimes and their associated
        *                   Authorisations. The script also identifies files that are exact copies of each other based on filename, file size and
        *                   a predetermined CRC hash, then eliminates duplicates from the list and versions any non-exact copies of the same filename
        ***********************************************************************************************************************************************************************/
        -----------------------------------------------------------------------------------------------------------------------------------------------------------------------
        --  This is the folder mapping that identifies which folders will be migrated plus sets any file prefixes that will be added when the files are copied
        -----------------------------------------------------------------------------------------------------------------------------------------------------------------------
        IF OBJECT_ID(N'tempdb..#Folder',N'U') IS NULL
        BEGIN
            SELECT      M.FolderID,
                        M.ParentFolderID,
                        M.Level,
                        M.FolderName,
                        M.FolderCount,
                        M.FileCount,
                        P.SourceFolder,
                        P.TargetFolder,
                        CASE
                        WHEN FolderName LIKE 'ATH-%'                    THEN 'Regimes'
                        WHEN [Level] = 0                                THEN 'Applications'
                        WHEN FolderName LIKE '%Rural Advi[sc]e%'        THEN 'Authorisations'
                        WHEN FolderName LIKE '%appeal%'                 THEN 'Applications'
                        WHEN FolderName LIKE '%hearing%'                THEN 'Applications'
                        WHEN FolderName  =   'Consent Correspondence'   THEN 'Applications'
                        WHEN FolderName LIKE '%compliance%'             THEN 'Regimes'
                        WHEN FolderName LIKE '%monitoring%'             THEN 'Regimes'
                        WHEN FolderName  =   'FlowMeterInfo'            THEN 'Regimes'
                        WHEN FolderName LIKE '%photos%'                 THEN 'Regimes'
                        WHEN FolderName LIKE '%inspection%'             THEN 'Regimes'
                        WHEN FolderName LIKE '%test%'                   THEN NULL
                        WHEN FolderName  =   'New Folder'               THEN NULL
                        ELSE '' END                                                                 ObjectType,
                        CASE
                        WHEN FolderName LIKE '%Rural Advi[sc]e%'        THEN 'Rural Advice, '
                        WHEN FolderName LIKE '%appeal%'                 THEN 'Appeal, '
                        WHEN FolderName LIKE '%hearing%'                THEN 'Hearing, '
                        WHEN FolderName  =   'Consent Correspondence'   THEN ''
                        WHEN FolderName LIKE '%compliance%'             THEN ''
                        WHEN FolderName LIKE '%monitoring%'             THEN ''
                        WHEN FolderName  =   'FlowMeterInfo'            THEN 'Flow Meter Information, '
                        WHEN FolderName LIKE '%photos%'                 THEN 'Photo, '
                        WHEN FolderName LIKE '%inspection%'             THEN 'Inspection, '
                        WHEN FolderName LIKE '%test%'                   THEN NULL
                        WHEN FolderName  =   'New Folder'               THEN NULL
                        ELSE '' END                                                                 Prefix
            INTO        #Folder
            FROM        HRC.DocumentMigrationProcess                                                P
            LEFT JOIN   HRC.DocumentMigrationFolders                                                M
            ON          P.FileMigrationID = M.FileMigrationID
            WHERE       P.FileMigrationID IN (SELECT FileMigrationID FROM hrc.DocumentMigrationProcess WHERE MigrationID = {My.Settings.MigrationID})
        
            CREATE INDEX idx_FolderID ON #Folder(ParentFolderID,FolderID) 
        END
        -----------------------------------------------------------------------------------------------------------------------------------------------------------------------
        --  FolderPath  --  This recursion builds the full path for the identified folders
        -----------------------------------------------------------------------------------------------------------------------------------------------------------------------
        IF OBJECT_ID(N'tempdb..#FolderPath',N'U') IS NULL
        BEGIN
        WITH
        -----------------------------------------------------------------------------------------------------------------------------------------------------------------------
        FolderPath
        -----------------------------------------------------------------------------------------------------------------------------------------------------------------------
        AS         (SELECT      FolderID,
                                ParentFolderID,
                                [Level],
                                CAST(REPLACE(REPLACE(FolderName,'-','/'),'ATH/','ATH-') AS VARCHAR(50))     ConsentNumber,
                                FolderName,
                                FolderCount,
                                FileCount,
                                CAST(SourceFolder+'\'+FolderName+'\' AS VARCHAR(MAX))                       SourceFolder,
                                TargetFolder,
                                CAST(ObjectType AS VARCHAR(MAX))                                            ObjectType,
                                Prefix
                    FROM        #Folder
                    WHERE       [Level] = 0
                    UNION ALL
                    SELECT      F.FolderID,
                                F.ParentFolderID,
                                F.[Level],
                                FP.ConsentNumber,
                                F.FolderName,
                                F.FolderCount,
                                F.FileCount,
                                CAST(FP.SourceFolder+F.FolderName+'\' AS VARCHAR(MAX)),
                                F.TargetFolder,
                                CASE
                                WHEN LEN(F.ObjectType) = 0 
                                THEN FP.ObjectType
                                ELSE CAST(F.ObjectType AS VARCHAR(MAX)) END                             ObjectType,
                                F.Prefix
                    FROM        #Folder             F
                    JOIN        FolderPath          FP
                    ON          FP.FolderID = F.ParentFolderID)
        
            SELECT * INTO #FolderPath FROM FolderPath OPTION (MAXRECURSION 200)
        
            CREATE INDEX idx_FolderID      ON #FolderPath(ParentFolderID,FolderID)
            CREATE INDEX idx_ConsentNumber ON #FolderPath(ConsentNumber)
        END
        -----------------------------------------------------------------------------------------------------------------------------------------------------------------------
        --  Authorisation - IRISObjectID
        -----------------------------------------------------------------------------------------------------------------------------------------------------------------------
        IF OBJECT_ID(N'tempdb..#GetIRISObjectID',N'U') IS NULL
        BEGIN
            SELECT      ATH.IRISObjectID,
                        OID.OtherIdentifierText                                                                     LegacyID,
                        ATO.BusinessID                                                                              AuthorisationIRISID
            INTO        #GetIRISObjectID
            FROM        {wks.IRISDatabase}.dbo.Authorisation                                                                  ATH
            LEFT JOIN   {wks.IRISDatabase}.dbo.IRISObject                                                                     ATO
            ON          ATO.ID = ATH.IRISObjectID
            LEFT JOIN  (SELECT      IRISObjectID,
                                    OtherIdentifierText
                        FROM        {wks.IRISDatabase}.dbo.OtherIdentifier
                        WHERE       IsDeleted = 0
                        AND         IdentifierContextID = 427)                                                      OID
            ON          OID.IRISObjectID = ATH.IRISObjectID
            WHERE       ATH.IsDeleted = 0
        
            CREATE INDEX idx_IRISObjectID    ON #GetIRISObjectID(IRISObjectID)
            CREATE INDEX idx_LegacyID        ON #GetIRISObjectID(LegacyID)
            CREATE INDEX idx_AuthorisationID ON #GetIRISObjectID(AuthorisationIRISID)
        END
        -----------------------------------------------------------------------------------------------------------------------------------------------------------------------
        --  Main script starts here...
        -----------------------------------------------------------------------------------------------------------------------------------------------------------------------
        SELECT      MAIN.*,
                    CASE
                    WHEN CopyCount > 1
                    THEN ISNULL(PARSENAME(MAIN.FileName,2),PARSENAME(MAIN.FileName,3))+' v'+RIGHT('00'+CAST(MAIN.VersionNumber AS VARCHAR),2)+'.'+PARSENAME(MAIN.FileName,1)
                    ELSE FileName END                                                                                                               NewFileName,
                    FP.SourceFolder,
                    FP.TargetFolder+'\Regimes\'+MAIN.RegimeIRISID COLLATE DATABASE_DEFAULT+'\Legacy Monitoring Activities'                          TargetFolder
        FROM       (SELECT      DISTINCT
                                FILES.RegimeIRISID,
                                FILES.MinFolderID,
                                FILES.FileName,
                                FILES.FileSize,
                                FILES.Checksum,
                                COUNT(*)        OVER(PARTITION BY RegimeIRISID,FileName)                                                            CopyCount,
                                RANK()          OVER(PARTITION BY RegimeIRISID,FileName
                                                        ORDER BY     SourceFolder)                                                                  VersionNumber
                    FROM       (SELECT      DISTINCT
                                            RGO.BusinessID                                                                                          RegimeIRISID,
                                            FP.FolderID,
                                            FP.FolderCount,
                                            FP.FileCount,
                                            FP.Prefix,
                                            FP.SourceFolder,
                                            FL.FileName,
                                            FL.FileSize,
                                            FL.[Checksum],
                                            MIN(FP.FolderID)    OVER(PARTITION BY RGO.BusinessID,FL.FileName,FL.FileSize,FL.[Checksum])             MinFolderID,
                                            RANK()              OVER(PARTITION BY RGO.BusinessID,FL.FileName,FL.FileSize,FL.[Checksum]
                                                                        ORDER BY     SourceFolder)                                                  CopyNumber
                                FROM        #FolderPath                                                                                             FP
                                LEFT JOIN   HRC.DocumentMigrationFiles                                                                              FL
                                ON          FL.FolderID = FP.FolderID
                                -----------------------------------------------------------------------------------------------------------------------------------------------
                                --  Use Consent Number to identify the IRIS Authorisation via the OtherIdentifier mapping
                                -----------------------------------------------------------------------------------------------------------------------------------------------
                                LEFT JOIN   #GetIRISObjectID                                                                                        LEG
                                ON          LEG.LegacyID            = FP.ConsentNumber COLLATE DATABASE_DEFAULT
                                OR          LEG.AuthorisationIRISID = FP.ConsentNumber COLLATE DATABASE_DEFAULT
                                -----------------------------------------------------------------------------------------------------------------------------------------------
                                --  Identify the Regime associated with the Authorisation (Regimes are object type 12 and are linked as Regime Subject)
                                -----------------------------------------------------------------------------------------------------------------------------------------------
                                OUTER APPLY {wks.IRISDatabase}.dbo.GetSimpleLinksWithoutSecurity(LEG.IRISObjectID)                                           AOR
                                LEFT JOIN   {wks.IRISDatabase}.dbo.IRISObject                                                                                RGO
                                ON          RGO.ID = AOR.LinkedIRISObjectID
                                AND         RGO.ObjectTypeID = 12
                                -----------------------------------------------------------------------------------------------------------------------------------------------
                                --  Get the associated IRIS object so we can work out the Regime Activity Type (RC2) and SubType (RC3)
                                --  Also, identify the associated Regime Activity (the Regime Activity connects to the Regime via a Regime Activity Schedule)
                                -----------------------------------------------------------------------------------------------------------------------------------------------
                                LEFT JOIN  (SELECT      RAC.IRISObjectID                                                                            RegimeActivityIRISObjectID,
                                                        RAC.ID                                                                                      RegimeActivityID,
                                                        RAO.BusinessID                                                                              RegimeActivityIRISID,
                                                        RAS.ID                                                                                      RegimeActivityScheduleID,
                                                        RAS.RegimeID
                                            FROM        {wks.IRISDatabase}.dbo.RegimeActivity                                                                RAC
                                            LEFT JOIN   {wks.IRISDatabase}.dbo.IRISObject                                                                    RAO
                                            ON          RAO.ID = RAC.IRISObjectID
                                            LEFT JOIN   {wks.IRISDatabase}.dbo.ReferenceDataValue                                                            SC2
                                            ON          SC2.ID = RAO.SubClass2ID
                                            LEFT JOIN   {wks.IRISDatabase}.dbo.ReferenceDataValue                                                            SC3
                                            ON          SC3.ID = RAO.SubClass3ID
                                            LEFT JOIN   {wks.IRISDatabase}.dbo.RegimeActivitySchedule                                                        RAS
                                            ON          RAS.ID = RAC.RegimeActivityScheduleID
                                            WHERE       RAO.ObjectTypeID = 13
                                            AND         SC2.DisplayValue = 'Historic'
                                            AND         SC3.DisplayValue = 'Legacy Monitoring Activities')                                          HRA
                                ON          HRA.RegimeID = RGO.LinkID
                                -----------------------------------------------------------------------------------------------------------------------------------------------
                                WHERE       FL.FileMigrationID IN (SELECT FileMigrationID FROM hrc.DocumentMigrationProcess WHERE MigrationID = {My.Settings.MigrationID})
                                AND         FL.Migrated = 0
                                AND         FP.FileCount > 0
                                AND         FL.FileName IS NOT NULL
                                AND         FP.ObjectType = 'Regimes'
                                AND         AOR.LinkedAsRelationshipType = 'Regime Subject')                                                        FILES
                    WHERE       FILES.CopyNumber = 1)                                                                                               MAIN
        LEFT JOIN   #FolderPath                                                                                                                     FP
        ON          FP.FolderID = MAIN.MinFolderID
        ORDER BY    RegimeIRISID,
        NewFileName
        -----------------------------------------------------------------------------------------------------------------------------------------------------------------------
        DROP TABLE IF EXISTS #Folder
        DROP TABLE IF EXISTS #FolderPath
        DROP TABLE IF EXISTS #GetIRISObjectID
        -----------------------------------------------------------------------------------------------------------------------------------------------------------------------"
        Return SQL
    End Function

    Private Function GetSQL_IdentifyDuplicates(ByVal FileMigrationID As Integer) As String
        '------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        '   The intention here is to identify files that will be duplicates when we copy them to the historical Regime Activity folder
        '------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        Dim SQL As String = $"
        /***********************************************************************************************************************************************
        *   Script:         Identify Duplicates
        *   Description:    The purpose of this script is to identify all files that are potentially duplicates of each other, using filename and
        *                   file size, so that the application can iterate through them and calculate a CRC hash which can be used a a more robust
        *                   discriminator. Initially, file modified date was included in the discriminator for potential duplicatesalso included but
        *                   was found to be unreliable.
        *                   The script is based on the parent Regime that will be the final destination for the files and identifies around 5,300
        *                   potential duplicate files out of 57,800 that will end up in a Regime. Calculating checksums for this many files takes
        *                   about 20 minutes.
        ***********************************************************************************************************************************************/
        USE {wks.MigrationDatabase}
        ------------------------------------------------------------------------------------------------------------------------------------------------
        DECLARE @GetLegacy BIT =  (SELECT UseLegacyCode FROM hrc.DocumentMigrationProcess WHERE FileMigrationID = {FileMigrationID})
        ------------------------------------------------------------------------------------------------------------------------------------------------
        --  Create a lookup table of all authorisation business ID's marked for migration and get their legacy ID (if any). Choose which, based on @GetLegacy
        ------------------------------------------------------------------------------------------------------------------------------------------------
        IF OBJECT_ID(N'tempdb..#IRIS_Lookup',N'U') IS NULL
        BEGIN
            --------------------------------------------------------------------------------------------------------------------------------------------
            SELECT      RGM.IRISObjectID                                                                                    RegimeIRISObjectID,
                        RGO.BusinessID                                                                                      RegimeIRISID,
                        IIF(@GetLegacy = 1,LEG.LegacyID,ATO.BusinessID)                                                     TextualID
            --------------------------------------------------------------------------------------------------------------------------------------------
            INTO        #IRIS_Lookup
            --------------------------------------------------------------------------------------------------------------------------------------------
            FROM       (SELECT      DISTINCT
                                    RegimeID,
                                    AuthorisationIRISID
                        FROM        HRC.MapRegime
                        WHERE       MigrationID = {My.Settings.MigrationID}
                        AND         Migrate = 1)                                                                            MAP
            LEFT JOIN   {wks.IRISDatabase}.dbo.IRISObject                                                                            ATO
            ON          ATO.BusinessID = MAP.AuthorisationIRISID
            LEFT JOIN   {wks.IRISDatabase}.dbo.Regime                                                                                RGM
            ON          RGM.ID = MAP.RegimeID
            LEFT JOIN   {wks.IRISDatabase}.dbo.IRISObject                                                                            RGO
            ON          RGO.ID = RGM.IRISObjectID
            LEFT JOIN  (SELECT      IRISObjectID,
                                    CAST(OtherIdentifierText AS NVARCHAR(100))                                              LegacyID
                        FROM        {wks.IRISDatabase}.dbo.OtherIdentifier
                        WHERE       IsDeleted = 0
                        AND         IdentifierContextID = 427)                                                              LEG
            ON          LEG.IRISObjectID = ATO.ID
            ORDER BY    1
            --------------------------------------------------------------------------------------------------------------------------------------------
            CREATE INDEX idx_TextualID ON #IRIS_Lookup(TextualID)
            --------------------------------------------------------------------------------------------------------------------------------------------
        END
        ------------------------------------------------------------------------------------------------------------------------------------------------
        IF OBJECT_ID(N'tempdb..#Folders',N'U') IS NULL
        BEGIN
            --------------------------------------------------------------------------------------------------------------------------------------------
            WITH
            --------------------------------------------------------------------------------------------------------------------------------------------
            Recurse     --  This Is the recursion that builds the path to the folder for each file
            --------------------------------------------------------------------------------------------------------------------------------------------
            AS     (SELECT      LKP.RegimeIRISID,
                                FLD.FolderUID,
                                FLD.FolderID,
                                FLD.ParentFolderID,
                                FLD.TextualID,
                                REPLACE(FLD.FolderName,'''','''''')                                                         FolderName,
                                CAST(REPLACE(FLD.FolderPath,'''','''''') AS VARCHAR(1000))                                  FolderPath,
                                FLD.ErrorText
                    FROM       (SELECT      FLD.ID                                                                          FolderUID,
                                            FLD.FolderID,
                                            FLD.ParentFolderID,
                                            CASE
                                            WHEN PATINDEX('[0-9]%',FLD.FolderName)=1                             THEN REPLACE(FLD.FolderName,'-','/')
                                            WHEN LEFT(FLD.FolderName+'  ' ,2) IN ('CR','MC','MW','NU','PA','RT') THEN RTRIM(LTRIM(ITM.Item))
                                            WHEN LEFT(FLD.FolderName+'   ',3) IN ('ATH','APP','RGM','PGM')       THEN RTRIM(LTRIM(ITM.Item))
                                            ELSE NULL END                                                                   TextualID,
                                            FLD.FolderName,
                                            CAST(FLD.FolderName AS VARCHAR(1000))                                           FolderPath,
                                            FLD.ErrorText
                                FROM        HRC.DocumentMigrationFolders                                                    FLD
                                CROSS APPLY {wks.IRISDatabase}.dbo.NumberedSplit(FLD.FolderName,'&')                                 ITM --  Handle compound root folder names
                                WHERE       FLD.FileMigrationID = {FileMigrationID}
                                AND         ParentFolderID = 0)                                                             FLD
                    LEFT JOIN   #IRIS_Lookup                                                                                LKP
                    ON          LKP.TextualID = FLD.TextualID
                    UNION ALL
                    SELECT      A.RegimeIRISID,
                                B.ID                                                                                        FolderUID,
                                B.FolderID,
                                B.ParentFolderID,
                                A.TextualID,
                                REPLACE(B.FolderName,'''','''''')                                                           FolderName,
                                CAST(A.FolderPath+'\'+B.FolderName AS VARCHAR(1000))                                        FolderPath,
                                B.ErrorText
                    FROM        Recurse                                                                                     A
                    JOIN        HRC.DocumentMigrationFolders                                                                B
                    ON          B.ParentFolderID = A.FolderID
                    AND         B.FileMigrationID = {FileMigrationID})
            --------------------------------------------------------------------------------------------------------------------------------------------
            SELECT * INTO #Folders FROM Recurse
            --------------------------------------------------------------------------------------------------------------------------------------------
            CREATE INDEX idx_FolderID   ON #Folders(FolderID)
            CREATE INDEX idx_TextualID  ON #Folders(TextualID)
            --------------------------------------------------------------------------------------------------------------------------------------------
        END
        ------------------------------------------------------------------------------------------------------------------------------------------------
        IF OBJECT_ID(N'tempdb..#AllFiles',N'U') IS NULL
        BEGIN
            --------------------------------------------------------------------------------------------------------------------------------------------
            SELECT      R.RegimeIRISID,
                        R.FolderUID,
                        R.FolderID,
                        R.ParentFolderID,
                        R.TextualID,
                        R.FolderName,
                        R.FolderPath,
                        R.ErrorText,
                        REPLACE(F.FileName,'''','''''')                                                             FileName,
                        F.FileSize,
                        F.Modified
            --------------------------------------------------------------------------------------------------------------------------------------------
            INTO        #AllFiles
            --------------------------------------------------------------------------------------------------------------------------------------------
            FROM        #Folders                                                                                    R
            LEFT JOIN   HRC.DocumentMigrationFiles                                                                  F
            ON          F.FolderID = R.FolderID
            WHERE       F.FileMigrationID = {FileMigrationID}
            AND         F.Filename IS NOT NULL
            AND         R.RegimeIRISID IS NOT NULL
            --------------------------------------------------------------------------------------------------------------------------------------------
            CREATE INDEX idx_RegimeID ON #AllFiles(RegimeIRISID,FileName,FileSize)
            CREATE INDEX idx_LegacyID ON #AllFiles(TextualID)
            --------------------------------------------------------------------------------------------------------------------------------------------
        END
        ------------------------------------------------------------------------------------------------------------------------------------------------
        --  Determine the potential duplicates by counting up files with the same name and size per Regime, then match these files to the actual
        --  files associated with the Regime to identify all the files where we need to calculate a checksum as a final discriminator
        ------------------------------------------------------------------------------------------------------------------------------------------------
        SELECT      DISTINCT
                    RF.RegimeIRISID,
                    RF.FolderID,
                    RF.FileName,
                    RF.FileSize,
                    RF.FolderPath
        FROM       (SELECT      DISTINCT
                                RegimeIRISID,
                                FileName,
                                FileSize,
                                COUNT(DISTINCT FolderID)                                                                DuplicateCount
                    FROM        #AllFiles
                    GROUP BY    RegimeIRISID,
                                FileName,
                                FileSize)                                                                               MAIN
        LEFT JOIN   #AllFiles                                                                                           RF
        ON          RF.RegimeIRISID = MAIN.RegimeIRISID
        AND         RF.FileName     = MAIN.FileName
        AND         RF.FileSize     = MAIN.FileSize
        WHERE       MAIN.DuplicateCount > 1
        ORDER BY    RegimeIRISID,
                    FileName,
                    FolderPath
        ------------------------------------------------------------------------------------------------------------------------------------------------
        DROP TABLE IF EXISTS #Folders
        DROP TABLE IF EXISTS #AllFiles
        DROP TABLE IF EXISTS #RegimeFiles
        DROP TABLE IF EXISTS #IRIS_Lookup
        ------------------------------------------------------------------------------------------------------------------------------------------------"
        Return SQL
    End Function

#End Region

End Class
