Imports System.Data.SqlClient
Imports System.Data

Module Database
    Public Structure Task
        Public TaskID As Integer
        Public ErrorText As String
    End Structure

#Region "Table Definitions"
    Private Function SQL_EnsureSchema() As String
        Dim SQL As String = "
        IF NOT EXISTS (SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = 'HRC')
        BEGIN
           EXEC sp_executesql N'CREATE SCHEMA HRC AUTHORIZATION db_owner;';
        END"
        Return SQL
    End Function

    Private Function SQL_Table_DM_Process() As String
        Dim SQL As String = "
        IF OBJECT_ID('HRC.DocumentMigrationProcess') IS NULL
        CREATE TABLE HRC.DocumentMigrationProcess(ID                INTEGER         NOT NULL IDENTITY(1,1) PRIMARY KEY,
                                                  MigrationID       INTEGER         NOT NULL,
                                                  FileMigrationID   VARCHAR(50)     NOT NULL,
                                                  SourceFolder      VARCHAR(1000)   NOT NULL,
                                                  TargetFolder      VARCHAR(1000)   NOT NULL,
                                                  UseLegacyCode     BIT             NOT NULL DEFAULT 0)"
        Return SQL
    End Function

    Private Function SQL_Table_DM_ProcessTask() As String
        Dim SQL As String = "
        IF OBJECT_ID('HRC.DocumentMigrationProcessTask') IS NULL
        CREATE TABLE HRC.DocumentMigrationProcessTask(MigrationID       INTEGER         NOT NULL,
                                                      FileMigrationID   VARCHAR(50)     NOT NULL,
                                                      TaskID            INTEGER         NOT NULL,
                                                      ErrorText         VARCHAR(MAX)    NULL)"
        Return SQL
    End Function

    Private Function SQL_Table_DM_Folders() As String
        Dim SQL As String = "
        IF OBJECT_ID('HRC.DocumentMigrationFolders') IS NULL
        BEGIN
        CREATE TABLE HRC.DocumentMigrationFolders(ID                INTEGER         NOT NULL IDENTITY(1,1),
                                                  MigrationID       INTEGER         NOT NULL,
                                                  FileMigrationID   VARCHAR(50)     NOT NULL,
                                                  FolderID          INTEGER         NOT NULL,
                                                  ParentFolderID    INTEGER         NOT NULL,
                                                  Level             INTEGER         NOT NULL,
                                                  FolderName        VARCHAR(500)    NOT NULL,
                                                  FolderCount       INTEGER         NULL,
                                                  FileCount         INTEGER         NULL,
                                                  FolderSize        BIGINT          NULL,
                                                  ErrorText         VARCHAR(MAX)    NULL)
        CREATE INDEX [idx_ID]             ON HRC.DocumentMigrationFolders (ID ASC)
        CREATE INDEX [idx_FolderID]       ON HRC.DocumentMigrationFolders (MigrationID ASC,FileMigrationID ASC,FolderID   ASC)
        CREATE INDEX [idx_ParentFolderID] ON HRC.DocumentMigrationFolders (MigrationID ASC,FileMigrationID ASC,ParentFolderID ASC,FolderID ASC)
        END"
        Return SQL
    End Function

    Private Function SQL_Table_DM_Files() As String
        Dim SQL As String = "
        IF OBJECT_ID('HRC.DocumentMigrationFiles') IS NULL
        BEGIN
        CREATE TABLE HRC.DocumentMigrationFiles  (ID                    INTEGER         NOT NULL IDENTITY(1,1),
                                                  Migration             INTEGER         NOT NULL,
                                                  FileMigrationID       VARCHAR(50)     NOT NULL,
                                                  FolderID              INTEGER         NOT NULL,
                                                  FileName              VARCHAR(1000)   NOT NULL,
                                                  Migrated              INTEGER         NOT NULL DEFAULT 0,
                                                  FileSize              BIGINT          NULL,
                                                  Modified              DATETIME        NULL,
                                                  Checksum              VARCHAR(64)     NULL,
                                                  ErrorText             VARCHAR(MAX)    NULL,
                                                  DateTime              DATETIME        NOT NULL)
        CREATE INDEX [idx_ID]       ON HRC.DocumentMigrationFiles (ID ASC)
        CREATE INDEX [idx_FolderID] ON HRC.DocumentMigrationFiles (FolderID ASC)
        END"
        Return SQL
    End Function

    Private Function SQL_Function_DM_FileMigration(ByRef wks As Workspace) As String
        Dim SQL As String = $"
        CREATE FUNCTION HRC.FileMigration()
        
        RETURNS TABLE AS
        
        RETURN (WITH        Folder
                AS         (SELECT      M.FolderID,
                                        M.ParentFolderID,
                                        M.Level,
                                        M.FolderName,
                                        M.FolderCount,
                                        M.FileCount,
                                        P.SourceFolder,
                                        P.TargetFolder,
                                        CASE
                                        WHEN [Level] = 0                                THEN 'Applications'
                                        WHEN FolderName LIKE '%Rural Advi[sc]e%'        THEN 'Authorisations'
                                        WHEN FolderName LIKE '%appeal%'                 THEN 'Applications'
                                        WHEN FolderName LIKE '%hearing%'                THEN 'Applications'
                                        WHEN FolderName  =   'Consent Correspondence'   THEN 'Applications'
                                        WHEN FolderName LIKE '%compliance%'             THEN 'Programmes'
                                        WHEN FolderName LIKE '%monitoring%'             THEN 'Regimes'
                                        WHEN FolderName  =   'FlowMeterInfo'            THEN 'Regimes'
                                        WHEN FolderName LIKE '%photos%'                 THEN 'Regimes'
                                        WHEN FolderName LIKE '%inspection%'             THEN 'RegimeActivities'
                                        WHEN FolderName LIKE '%test%'                   THEN NULL
                                        WHEN FolderName IN  ('New Folder','102097')     THEN NULL
                                        ELSE '' END                                                                 AS ObjectType,
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
                                        WHEN FolderName IN  ('New Folder','102097')     THEN NULL
                                        ELSE '' END                                                                 AS Prefix
                            FROM        HRC.DocumentMigrationFolders                                                AS M
                            LEFT JOIN   HRC.DocumentMigrationProcess                                                AS P
                            ON          P.MigrationID = M.MigrationID),
        
                            Recurse            
                AS         (SELECT      FolderID,
                                        ParentFolderID,
                                        [Level],
                                        REPLACE(FolderName,'-','/')                                                 AS ConsentNumber,
                                        FolderName,
                                        FolderCount,
                                        FileCount,
                                        CAST(SourceFolder+'\'+FolderName+'\' AS VARCHAR(MAX))                       AS SourceFolder,
                                        TargetFolder,
                                        CAST(ObjectType AS VARCHAR(MAX))                                            AS ObjectType,
                                        Prefix
                            FROM        Folder
                            WHERE       [Level] = 0
                            UNION ALL
                            SELECT      F.FolderID,
                                        F.ParentFolderID,
                                        F.[Level],
                                        R.ConsentNumber,
                                        F.FolderName,
                                        F.FolderCount,
                                        F.FileCount,
                                        CAST(R.SourceFolder+F.FolderName+'\' AS VARCHAR(MAX)),
                                        F.TargetFolder,
                                        CASE
                                        WHEN LEN(F.ObjectType) = 0 
                                        THEN R.ObjectType
                                        ELSE CAST(F.ObjectType AS VARCHAR(MAX)) END,
                                        F.Prefix
                            FROM        Folder              F
                            JOIN        Recurse             R
                            ON          F.ParentFolderID = R.FolderID)
        
                SELECT      R.ConsentNumber,
                            M.NewApplicationID,
                            M.NewAuthorisationID,
                            R.FolderID,
                            R.ParentFolderID,
                            R.FolderCount,
                            R.FileCount,
                            R.[Level],
                            R.ObjectType,
                            R.Prefix,
                            R.SourceFolder,
                            CASE R.ObjectType
                            WHEN 'Applications'      THEN R.TargetFolder+'\'+ObjectType+'\'+OB1.BusinessID+'\' COLLATE DATABASE_DEFAULT
                            WHEN 'Authorisations'    THEN R.TargetFolder+'\'+ObjectType+'\'+OB2.BusinessID+'\' COLLATE DATABASE_DEFAULT
                            WHEN 'Programmes'        THEN R.TargetFolder+'\'+ObjectType+'\'+OB3.BusinessID+'\' COLLATE DATABASE_DEFAULT
                            WHEN 'Regimes'           THEN R.TargetFolder+'\'+ObjectType+'\'+OB4.BusinessID+'\' COLLATE DATABASE_DEFAULT
                            WHEN 'RegimeActivities'  THEN R.TargetFolder+'\'+ObjectType+'\'+OB5.BusinessID+'\' COLLATE DATABASE_DEFAULT
                            ELSE NULL END                                                  TargetFolder
                FROM        Recurse                                                        R
                LEFT JOIN  (SELECT    A.ID                                                 NewAuthorisationID,
                                      P.ID                                                 NewApplicationID,
                                      O.OtherIdentifierText                                LegacyID
                            FROM      {wks.IRISDatabase}.dbo.Authorisation                          A
                            LEFT JOIN {wks.IRISDatabase}.dbo.Activity                               C
                            ON        C.ID = A.ActivityID
                            LEFT JOIN {wks.IRISDatabase}.dbo.Application                            P
                            ON        P.ID = C.ApplicationID
                            LEFT JOIN {wks.IRISDatabase}.dbo.OtherIdentifier                        O
                            ON        O.IRISObjectID = A.IRISObjectID
                            WHERE     O.IsDeleted = 0
                            AND       O.IdentifierContextID = 427)                         M
                ON          M.LegacyID = R.ConsentNumber
                LEFT JOIN   {wks.IRISDatabase}.dbo.[Application]                                    APP
                On          APP.MigrationSourceID = M.NewApplicationID
                LEFT JOIN   {wks.IRISDatabase}.dbo.IRISObject                                       OB1
                On          OB1.ID = APP.IRISObjectID
                LEFT JOIN   {wks.IRISDatabase}.dbo.Authorisation                                    ATH
                On          ATH.MigrationSourceID = NewAuthorisationID
                LEFT JOIN   {wks.IRISDatabase}.dbo.IRISObject                                       OB2
                On          OB2.ID = ATH.IRISObjectID
                LEFT JOIN   {wks.IRISDatabase}.dbo.[Programme]                                      PRO
                On          PRO.MigrationSourceID = M.NewApplicationID
                LEFT JOIN   {wks.IRISDatabase}.dbo.IRISObject                                       OB3
                On          OB3.ID = PRO.IRISObjectID
                LEFT JOIN   {wks.IRISDatabase}.dbo.[Regime]                                         REG
                On          REG.MigrationSourceID = M.NewAuthorisationID
                LEFT JOIN   {wks.IRISDatabase}.dbo.IRISObject                                       OB4
                On          OB4.ID = REG.IRISObjectID
                LEFT JOIN   {wks.IRISDatabase}.dbo.[RegimeActivity]                                 RGA
                On          RGA.MigrationSourceID = M.NewAuthorisationID
                LEFT JOIN   {wks.IRISDatabase}.dbo.IRISObject                                       OB5
                On          OB5.ID = REG.IRISObjectID
                WHERE       R.FileCount > 0)"
        Return SQL
    End Function
#End Region

    Private Function GetSQL_CreateTables() As String
        Dim SQL As String = ""

        SQL = SQL & SQL_Table_DM_Process()
        SQL = SQL & SQL_Table_DM_ProcessTask()
        SQL = SQL & SQL_Table_DM_Files()
        SQL = SQL & SQL_Table_DM_Folders()

        Return SQL
    End Function

    Private Function GetSQL_CreateFunctions(ByRef wks As Workspace) As String
        Dim SQL As String = ""

        SQL = SQL & SQL_Function_DM_FileMigration(wks)

        Return SQL
    End Function

    Private Function GetSQL_CreateDatabase(ByVal ServerName As String, ByVal DatabaseName As String, ByVal DataFolder As String, ByVal LogFolder As String) As String
        Dim SQL As String = $"
        If (Select COUNT(*) FROM sys.databases WHERE [Name] = '{DatabaseName}') = 0
            BEGIN
                CREATE  DATABASE [{DatabaseName}] ON  PRIMARY 
                       (NAME = N'{DatabaseName}',
                        FILENAME = N'{DataFolder}{DatabaseName}.mdf',
                        SIZE = 3072KB,
                        MAXSIZE = UNLIMITED,
                        FILEGROWTH = 1024KB )
                LOG ON 
                       (NAME = N'{DatabaseName}_log', 
                        FILENAME = N'{LogFolder}{DatabaseName}_log.ldf',
                        SIZE = 1024KB,
                        MAXSIZE = 2048GB,
                        FILEGROWTH = 10%)
                ALTER DATABASE [{DatabaseName}] SET COMPATIBILITY_LEVEL = 100
                IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
                   BEGIN
                      EXEC [{DatabaseName}].[dbo].[sp_fulltext_database] @action = 'enable'
                   END
                ALTER DATABASE [{DatabaseName}] SET ANSI_NULL_DEFAULT OFF 
                ALTER DATABASE [{DatabaseName}] SET ANSI_NULLS OFF 
                ALTER DATABASE [{DatabaseName}] SET ANSI_PADDING OFF 
                ALTER DATABASE [{DatabaseName}] SET ANSI_WARNINGS OFF 
                ALTER DATABASE [{DatabaseName}] SET ARITHABORT OFF 
                ALTER DATABASE [{DatabaseName}] SET AUTO_CLOSE OFF 
                ALTER DATABASE [{DatabaseName}] SET AUTO_CREATE_STATISTICS ON 
                ALTER DATABASE [{DatabaseName}] SET AUTO_SHRINK OFF 
                ALTER DATABASE [{DatabaseName}] SET AUTO_UPDATE_STATISTICS ON 
                ALTER DATABASE [{DatabaseName}] SET CURSOR_CLOSE_ON_COMMIT OFF 
                ALTER DATABASE [{DatabaseName}] SET CURSOR_DEFAULT  GLOBAL 
                ALTER DATABASE [{DatabaseName}] SET CONCAT_NULL_YIELDS_NULL OFF 
                ALTER DATABASE [{DatabaseName}] SET NUMERIC_ROUNDABORT OFF 
                ALTER DATABASE [{DatabaseName}] SET QUOTED_IDENTIFIER OFF 
                ALTER DATABASE [{DatabaseName}] SET RECURSIVE_TRIGGERS OFF 
                ALTER DATABASE [{DatabaseName}] SET  DISABLE_BROKER 
                ALTER DATABASE [{DatabaseName}] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
                ALTER DATABASE [{DatabaseName}] SET DATE_CORRELATION_OPTIMIZATION OFF 
                ALTER DATABASE [{DatabaseName}] SET TRUSTWORTHY OFF 
                ALTER DATABASE [{DatabaseName}] SET ALLOW_SNAPSHOT_ISOLATION OFF 
                ALTER DATABASE [{DatabaseName}] SET PARAMETERIZATION SIMPLE 
                ALTER DATABASE [{DatabaseName}] SET READ_COMMITTED_SNAPSHOT OFF 
                ALTER DATABASE [{DatabaseName}] SET HONOR_BROKER_PRIORITY OFF 
                ALTER DATABASE [{DatabaseName}] SET  READ_WRITE 
                ALTER DATABASE [{DatabaseName}] SET RECOVERY FULL 
                ALTER DATABASE [{DatabaseName}] SET  MULTI_USER 
                ALTER DATABASE [{DatabaseName}] SET PAGE_VERIFY CHECKSUM  
                ALTER DATABASE [{DatabaseName}] SET DB_CHAINING OFF 
            END"

        Return SQL
    End Function

    Public Function EnsureTables(ByRef wks As Workspace) As String
        Dim msg As String = ""
        Dim TableCount As Integer = wks.ExecuteScalarMigration("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'HRC' AND TABLE_NAME IN ('DocumentMigrationFiles','DocumentMigrationFolders','DocumentMigrationProcess','DocumentMigrationProcessTask')")

        If TableCount <> 4 Then
            msg = "Document migration tables have not been created in the workspace database"
            If MsgBox("The document migration tables don't exist in the workspace database. Do you want to create them?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, "Missing Data Tables") = MsgBoxResult.Yes Then
                wks.ExecuteNonQueryMigration(SQL_EnsureSchema())
                wks.ExecuteNonQueryMigration(GetSQL_CreateTables())
                Dim FnExists As Integer = wks.ExecuteScalarMigration("SELECT ISNULL(OBJECT_ID(N'HRC.FileMigration',N'IF'),0)")
                If FnExists = 0 Then
                    wks.ExecuteNonQueryMigration(GetSQL_CreateFunctions(wks))
                End If
                msg = ""
            End If
        End If

        Return msg
    End Function

    Public Function VerifyMigrationID(ByRef wks As Workspace) As Boolean
        Dim RetVal As Boolean = False

        If My.Settings.FileMigrationID > 0 Then  'Check if exists in db
            If wks.ExecuteScalarMigration($"SELECT COUNT(*) AS ItemCount FROM {wks.MigrationDatabase}.HRC.DocumentMigrationProcess WHERE FileMigrationID = '" & My.Settings.FileMigrationID & "'") = 0 Then
                My.Settings.FileMigrationID = wks.ExecuteScalarMigration($"SELECT TOP 1 FileMigrationID FROM {wks.MigrationDatabase}.HRC.DocumentMigrationProcess")
                My.Settings.FileMigrationID = wks.ExecuteScalarMigration($"SELECT TOP 1 MigrationID FROM {wks.MigrationDatabase}.HRC.DocumentMigrationProcess WHERE FileMigrationID = '" & My.Settings.FileMigrationID & "'")
                My.Settings.SourceRootFolder = wks.ExecuteScalarMigration($"SELECT SourceFolder FROM {wks.MigrationDatabase}.HRC.DocumentMigrationProcess WHERE FileMigrationID = '" & My.Settings.FileMigrationID & "'")
                My.Settings.IRIS_RootFolder = wks.ExecuteScalarMigration($"SELECT TargetFolder FROM {wks.MigrationDatabase}.HRC.DocumentMigrationProcess WHERE FileMigrationID = '" & My.Settings.FileMigrationID & "'")
                RetVal = True
            Else
                If IO.Directory.Exists(My.Settings.SourceRootFolder) AndAlso IO.Directory.Exists(My.Settings.IRIS_RootFolder) Then
                    wks.ExecuteNonQueryMigration($"UPDATE {wks.MigrationDatabase}.HRC.DocumentMigrationProcess SET SourceFolder = '" & StripTrailingSlash(My.Settings.SourceRootFolder) & "', TargetFolder = '" & StripTrailingSlash(My.Settings.IRIS_RootFolder) & "' WHERE FileMigrationID = '" & My.Settings.FileMigrationID & "'")
                    RetVal = True
                Else
                    MsgBox("Can't find one of the migration folders. Please verify the selected folders exist", MsgBoxStyle.Information + MsgBoxStyle.OkOnly, "Missing Folder")
                End If
            End If
        End If

        Return RetVal
    End Function

    Private Function StripTrailingSlash(ByVal DirectoryName As String) As String
        Return IIf(Right(DirectoryName, 1) = "\", Left(DirectoryName, DirectoryName.Length - 1), DirectoryName)
    End Function

    Public Function GetTask(ByVal TaskID As Integer) As Task
        Dim wks As New Workspace(My.Settings.IRIS_Server)
        Dim RetVal As Task = Nothing

        If wks.ExecuteScalarMigration($"SELECT COUNT(*) AS ItemCount FROM HRC.DocumentMigrationProcessTask WHERE MigrationID = {My.Settings.FileMigrationID} AND TaskID = {TaskID.ToString}") > 0 Then
            RetVal = New Task
            RetVal.TaskID = TaskID
            RetVal.ErrorText = wks.ExecuteScalarMigration($"SELECT ISNULL(ErrorText,'') FROM HRC.DocumentMigrationProcessTask WHERE MigrationID = {My.Settings.FileMigrationID} AND TaskID = {TaskID.ToString}")
        End If
        wks.Dispose()

        Return RetVal
    End Function

    Public Sub UpdateTask(ByRef wks As Workspace, ByVal TaskID As Integer, ByVal ErrorText As String)
        If wks.ExecuteScalarMigration($"SELECT COUNT(*) AS ItemCount FROM HRC.DocumentMigrationProcessTask WHERE MigrationID = {My.Settings.MigrationID} AND TaskID = {TaskID.ToString}") = 0 Then
            wks.MigrationCommandText = $"INSERT INTO HRC.DocumentMigrationProcessTask(MigrationID,TaskID,ErrorText) VALUES({My.Settings.MigrationID},{TaskID.ToString},{IIf(ErrorText.Length = 0, "NULL", "'" & ErrorText.Replace("'", "''") & "'")})"
        Else
            wks.MigrationCommandText = $"UPDATE HRC.DocumentMigrationProcessTask SET ErrorText = {IIf(ErrorText.Length = 0, "NULL", "'" & ErrorText.Replace("'", "''") & "'")} WHERE MigrationID = {My.Settings.MigrationID} AND TaskID = {TaskID.ToString}"
        End If
        wks.ExecuteNonQueryMigration()
    End Sub

    Public Sub ResetTasksFrom(ByRef wks As Workspace, ByVal TaskID As Integer)
        wks.ExecuteNonQueryMigration($"DELETE FROM {wks.MigrationDatabase}.HRC.DocumentMigrationProcessTask WHERE MigrationID = {My.Settings.MigrationID} AND TaskID >= {TaskID.ToString}")
    End Sub

    Public Class IRIS_R2D2_Mapping
        Private DT As New DataTable("IRIS_R2D2_Mapping")

        'NOTE: Items in this enumeration MUST be in the exact ordinal position of the matching field in the below query
        Private Enum Field
            ConsentNumber
            APP_BusinessID
            ATH_BusinessID
            APP_IRISID
            ATH_IRISID
        End Enum

#Region "Properties"
        Public ReadOnly Property Count() As Integer
            Get
                Return DT.Rows.Count
            End Get
        End Property

        Public ReadOnly Property ConsentNumber(ByVal Index As Integer) As String
            Get
                Return DT.Rows(Index)(Field.ConsentNumber)
            End Get
        End Property

        Public ReadOnly Property APP_BusinessID(ByVal Index As Integer) As String
            Get
                Return DT.Rows(Index)(Field.APP_BusinessID)
            End Get
        End Property

        Public ReadOnly Property ATH_BusinessID(ByVal Index As Integer) As String
            Get
                Return DT.Rows(Index)(Field.ATH_BusinessID)
            End Get
        End Property

        Public ReadOnly Property APP_IRISID(ByVal Index As Integer) As Integer
            Get
                Return DT.Rows(Index)(Field.APP_IRISID)
            End Get
        End Property

        Public ReadOnly Property ATH_IRISID(ByVal Index As Integer) As Integer
            Get
                Return DT.Rows(Index)(Field.ATH_IRISID)
            End Get
        End Property
#End Region

        Public Sub Load(ByVal wks As Workspace)
            DT.Rows.Clear()
            DT.Columns.Clear()
            wks.FillDataTableIRIS(DT, SQL_Query())
        End Sub

        Private Function SQL_Query()
            Dim SQL As String = "
            DECLARE @ObjectID AS INTEGER        = (SELECT ID FROM ReferenceDataCollection WHERE FunctionalArea = 'Object Hierarchy'     AND Code = 'ObjectType')
            DECLARE @IdentifierID AS INTEGER    = (SELECT ID FROM ReferenceDataCollection WHERE FunctionalArea = 'Other Identifiers'    AND Code = 'IdentifierContexts')
            DECLARE @ApplicationID AS INTEGER   = (SELECT ID FROM ReferenceDataValue      WHERE CollectionID   = @ObjectID              AND Code = 'Application')
            DECLARE @AuthorisationID AS INTEGER = (SELECT ID FROM ReferenceDataValue      WHERE CollectionID   = @ObjectID              AND Code = 'Authorisation')
            DECLARE @APP_ContextID AS INTEGER   = (SELECT ID FROM ReferenceDataValue      WHERE CollectionID   = @IdentifierID          AND ParentValueID = @ApplicationID   AND Code = 'LegacyNumber')
            DECLARE @ATH_ContextID AS INTEGER   = (SELECT ID FROM ReferenceDataValue      WHERE CollectionID   = @IdentifierID          AND ParentValueID = @AuthorisationID AND Code = 'LegacyNumber')
            
            SELECT          APPS.R2_AuthorisationNumber,
                            APPS.APP_BusinessID,
                            AUTHS.ATH_BusinessID,
                            APPS.APP_IRISID,
                            AUTHS.ATH_IRISID
            FROM           (SELECT      REPLACE(OtherIdentifierText,'/','-')                                    AS R2_AuthorisationNumber,
                                        OID.IRISObjectID                                                        AS APP_IRISID,
                                        IOB.BusinessID                                                          AS APP_BusinessID
                            FROM        OtherIdentifier                                                         AS OID
                            LEFT JOIN   IRISObject                                                              AS IOB
                            ON          IOB.ID = OID.IRISObjectID
                            WHERE       IRISObjectID 
                            IN         (SELECT ID FROM IRISObject WHERE ObjectTypeID = @ApplicationID AND IdentifierContextID = @APP_ContextID)
                            )                                                                                   AS APPS
            FULL OUTER JOIN(SELECT      REPLACE(OtherIdentifierText,'/','-')                                    AS R2_AuthorisationNumber,
                                        OID.IRISObjectID                                                        AS ATH_IRISID,
                                        IOB.BusinessID                                                          AS ATH_BusinessID
                                        FROM        OtherIdentifier                                             AS OID
                                        LEFT JOIN   IRISObject                                                  AS IOB
                                        ON          IOB.ID = OID.IRISObjectID
                                        WHERE       IRISObjectID 
                                        IN         (SELECT ID FROM IRISObject WHERE ObjectTypeID = @AuthorisationID AND IdentifierContextID = @ATH_ContextID)
                                        )                                                                       AS AUTHS
            ON              APPS.R2_AuthorisationNumber = AUTHS.R2_AuthorisationNumber"
            Return SQL
        End Function

        Public Sub Dispose()
            DT = Nothing
        End Sub
    End Class

End Module
