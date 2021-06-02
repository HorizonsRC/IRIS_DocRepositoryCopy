Imports System.Data.SqlClient
Imports System.Data

Module Database
    Public Structure Task
        Public TaskID As Integer
        Public ErrorText As String
    End Structure

#Region "Table Definitions"
    Private Function SQL_EnsureSchema() As String
        Dim SQL As String = ""
        SQL = SQL & vbCrLf & "IF NOT EXISTS (SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = 'HRC')"
        SQL = SQL & vbCrLf & "BEGIN"
        SQL = SQL & vbCrLf & "   EXEC sp_executesql N'CREATE SCHEMA HRC AUTHORIZATION db_owner;';"
        SQL = SQL & vbCrLf & "END"
        Return SQL
    End Function

    Private Function SQL_Table_DM_Process() As String
        Dim SQL As String = ""
        SQL = SQL & vbCrLf & "IF OBJECT_ID('HRC.DocumentMigrationProcess') IS NULL"
        SQL = SQL & vbCrLf & "CREATE TABLE HRC.DocumentMigrationProcess(MigrationID       INTEGER         NOT NULL,"
        SQL = SQL & vbCrLf & "                                          FileMigrationID   VARCHAR(50)     NOT NULL,"
        SQL = SQL & vbCrLf & "                                          SourceFolder      VARCHAR(1000)   NOT NULL,"
        SQL = SQL & vbCrLf & "                                          TargetFolder      VARCHAR(1000)   NOT NULL)"
        Return SQL
    End Function

    Private Function SQL_Table_DM_ProcessTask() As String
        Dim SQL As String = ""
        SQL = SQL & vbCrLf & "IF OBJECT_ID('HRC.DocumentMigrationProcessTask') IS NULL"
        SQL = SQL & vbCrLf & "CREATE TABLE HRC.DocumentMigrationProcessTask(MigrationID       INTEGER         NOT NULL,"
        SQL = SQL & vbCrLf & "                                              FileMigrationID   VARCHAR(50)     NOT NULL,"
        SQL = SQL & vbCrLf & "                                              TaskID            INTEGER         NOT NULL,"
        SQL = SQL & vbCrLf & "                                              ErrorText         VARCHAR(MAX)    NULL)"
        Return SQL
    End Function

    Private Function SQL_Table_DM_Folders() As String
        Dim SQL As String = ""
        SQL = SQL & vbCrLf & "IF OBJECT_ID('HRC.DocumentMigrationFolders') IS NULL"
        SQL = SQL & vbCrLf & "BEGIN"
        SQL = SQL & vbCrLf & "CREATE TABLE HRC.DocumentMigrationFolders(ID                INTEGER         NOT NULL IDENTITY(1,1),"
        SQL = SQL & vbCrLf & "                                          MigrationID       INTEGER         NOT NULL,"
        SQL = SQL & vbCrLf & "                                          FileMigrationID   VARCHAR(50)     NOT NULL,"
        SQL = SQL & vbCrLf & "                                          FolderID          INTEGER         NOT NULL,"
        SQL = SQL & vbCrLf & "                                          ParentFolderID    INTEGER         NOT NULL,"
        SQL = SQL & vbCrLf & "                                          Level             INTEGER         NOT NULL,"
        SQL = SQL & vbCrLf & "                                          FolderName        VARCHAR(500)    NOT NULL,"
        SQL = SQL & vbCrLf & "                                          FolderCount       INTEGER         NULL,"
        SQL = SQL & vbCrLf & "                                          FileCount         INTEGER         NULL,"
        SQL = SQL & vbCrLf & "                                          FolderSize        BIGINT          NULL,"
        SQL = SQL & vbCrLf & "                                          ErrorText         VARCHAR(MAX)    NULL)"
        SQL = SQL & vbCrLf & "CREATE INDEX [idx_ID]             ON HRC.DocumentMigrationFolders (ID ASC)"
        SQL = SQL & vbCrLf & "CREATE INDEX [idx_FolderID]       ON HRC.DocumentMigrationFolders (MigrationID ASC,FileMigrationID ASC,FolderID   ASC)"
        SQL = SQL & vbCrLf & "CREATE INDEX [idx_ParentFolderID] ON HRC.DocumentMigrationFolders (MigrationID ASC,FileMigrationID ASC,ParentFolderID ASC,FolderID ASC)"
        SQL = SQL & vbCrLf & "END"
        Return SQL
    End Function

    Private Function SQL_Table_DM_Files() As String
        Dim SQL As String = ""
        SQL = SQL & vbCrLf & "IF OBJECT_ID('HRC.DocumentMigrationFiles') IS NULL"
        SQL = SQL & vbCrLf & "BEGIN"
        SQL = SQL & vbCrLf & "CREATE TABLE HRC.DocumentMigrationFiles  (ID                    INTEGER         NOT NULL IDENTITY(1,1),"
        SQL = SQL & vbCrLf & "                                          Migration             INTEGER         NOT NULL,"
        SQL = SQL & vbCrLf & "                                          FileMigrationID       VARCHAR(50)     NOT NULL,"
        SQL = SQL & vbCrLf & "                                          FolderID              INTEGER         NOT NULL,"
        SQL = SQL & vbCrLf & "                                          FileName              VARCHAR(1000)   NOT NULL,"
        SQL = SQL & vbCrLf & "                                          Migrated              INTEGER         NOT NULL DEFAULT 0,"
        SQL = SQL & vbCrLf & "                                          FileSize              BIGINT          NULL,"
        SQL = SQL & vbCrLf & "                                          Modified              DATETIME        NULL,"
        SQL = SQL & vbCrLf & "                                          Checksum              VARCHAR(64)     NULL,"
        SQL = SQL & vbCrLf & "                                          ErrorText             VARCHAR(MAX)    NULL,"
        SQL = SQL & vbCrLf & "                                          DateTime              DATETIME        NOT NULL)"
        SQL = SQL & vbCrLf & "CREATE INDEX [idx_ID]       ON HRC.DocumentMigrationFiles (ID ASC)"
        SQL = SQL & vbCrLf & "CREATE INDEX [idx_FolderID] ON HRC.DocumentMigrationFiles (FolderID ASC)"
        SQL = SQL & vbCrLf & "END"
        Return SQL
    End Function

    Private Function SQL_Function_DM_FileMigration() As String
        Dim SQL As String = ""
        SQL = SQL & vbCrLf & "CREATE FUNCTION HRC.FileMigration()"
        SQL = SQL & vbCrLf & ""
        SQL = SQL & vbCrLf & "RETURNS TABLE AS"
        SQL = SQL & vbCrLf & ""
        SQL = SQL & vbCrLf & "RETURN (WITH        Folder"
        SQL = SQL & vbCrLf & "        AS         (SELECT      M.FolderID,"
        SQL = SQL & vbCrLf & "                                M.ParentFolderID,"
        SQL = SQL & vbCrLf & "                                M.Level,"
        SQL = SQL & vbCrLf & "                                M.FolderName,"
        SQL = SQL & vbCrLf & "                                M.FolderCount,"
        SQL = SQL & vbCrLf & "                                M.FileCount,"
        SQL = SQL & vbCrLf & "                                P.SourceFolder,"
        SQL = SQL & vbCrLf & "                                P.TargetFolder,"
        SQL = SQL & vbCrLf & "                                CASE"
        SQL = SQL & vbCrLf & "                                WHEN [Level] = 0                                THEN 'Applications'"
        SQL = SQL & vbCrLf & "                                WHEN FolderName LIKE '%Rural Advi[sc]e%'        THEN 'Authorisations'"
        SQL = SQL & vbCrLf & "                                WHEN FolderName LIKE '%appeal%'                 THEN 'Applications'"
        SQL = SQL & vbCrLf & "                                WHEN FolderName LIKE '%hearing%'                THEN 'Applications'"
        SQL = SQL & vbCrLf & "                                WHEN FolderName  =   'Consent Correspondence'   THEN 'Applications'"
        SQL = SQL & vbCrLf & "                                WHEN FolderName LIKE '%compliance%'             THEN 'Programmes'"
        SQL = SQL & vbCrLf & "                                WHEN FolderName LIKE '%monitoring%'             THEN 'Regimes'"
        SQL = SQL & vbCrLf & "                                WHEN FolderName  =   'FlowMeterInfo'            THEN 'Regimes'"
        SQL = SQL & vbCrLf & "                                WHEN FolderName LIKE '%photos%'                 THEN 'Regimes'"
        SQL = SQL & vbCrLf & "                                WHEN FolderName LIKE '%inspection%'             THEN 'RegimeActivities'"
        SQL = SQL & vbCrLf & "                                WHEN FolderName LIKE '%test%'                   THEN NULL"
        SQL = SQL & vbCrLf & "                                WHEN FolderName IN  ('New Folder','102097')     THEN NULL"
        SQL = SQL & vbCrLf & "                                ELSE '' END                                                                 AS ObjectType,"
        SQL = SQL & vbCrLf & "                                CASE"
        SQL = SQL & vbCrLf & "                                WHEN FolderName LIKE '%Rural Advi[sc]e%'        THEN 'Rural Advice, '"
        SQL = SQL & vbCrLf & "                                WHEN FolderName LIKE '%appeal%'                 THEN 'Appeal, '"
        SQL = SQL & vbCrLf & "                                WHEN FolderName LIKE '%hearing%'                THEN 'Hearing, '"
        SQL = SQL & vbCrLf & "                                WHEN FolderName  =   'Consent Correspondence'   THEN ''"
        SQL = SQL & vbCrLf & "                                WHEN FolderName LIKE '%compliance%'             THEN ''"
        SQL = SQL & vbCrLf & "                                WHEN FolderName LIKE '%monitoring%'             THEN ''"
        SQL = SQL & vbCrLf & "                                WHEN FolderName  =   'FlowMeterInfo'            THEN 'Flow Meter Information, '"
        SQL = SQL & vbCrLf & "                                WHEN FolderName LIKE '%photos%'                 THEN 'Photo, '"
        SQL = SQL & vbCrLf & "                                WHEN FolderName LIKE '%inspection%'             THEN 'Inspection, '"
        SQL = SQL & vbCrLf & "                                WHEN FolderName LIKE '%test%'                   THEN NULL"
        SQL = SQL & vbCrLf & "                                WHEN FolderName IN  ('New Folder','102097')     THEN NULL"
        SQL = SQL & vbCrLf & "                                ELSE '' END                                                                 AS Prefix"
        SQL = SQL & vbCrLf & "                    FROM        HRC.DocumentMigrationFolders                                                AS M"
        SQL = SQL & vbCrLf & "                    LEFT JOIN   HRC.DocumentMigrationProcess                                                AS P"
        SQL = SQL & vbCrLf & "                    ON          P.MigrationID = M.MigrationID),"
        SQL = SQL & vbCrLf & ""
        SQL = SQL & vbCrLf & "                    Recurse            "
        SQL = SQL & vbCrLf & "        AS         (SELECT      FolderID,"
        SQL = SQL & vbCrLf & "                                ParentFolderID,"
        SQL = SQL & vbCrLf & "                                [Level],"
        SQL = SQL & vbCrLf & "                                REPLACE(FolderName,'-','/')                                                 AS ConsentNumber,"
        SQL = SQL & vbCrLf & "                                FolderName,"
        SQL = SQL & vbCrLf & "                                FolderCount,"
        SQL = SQL & vbCrLf & "                                FileCount,"
        SQL = SQL & vbCrLf & "                                CAST(SourceFolder+'\'+FolderName+'\' AS VARCHAR(MAX))                       AS SourceFolder,"
        SQL = SQL & vbCrLf & "                                TargetFolder,"
        SQL = SQL & vbCrLf & "                                CAST(ObjectType AS VARCHAR(MAX))                                            AS ObjectType,"
        SQL = SQL & vbCrLf & "                                Prefix"
        SQL = SQL & vbCrLf & "                    FROM        Folder"
        SQL = SQL & vbCrLf & "                    WHERE       [Level] = 0"
        SQL = SQL & vbCrLf & "                    UNION ALL"
        SQL = SQL & vbCrLf & "                    SELECT      F.FolderID,"
        SQL = SQL & vbCrLf & "                                F.ParentFolderID,"
        SQL = SQL & vbCrLf & "                                F.[Level],"
        SQL = SQL & vbCrLf & "                                R.ConsentNumber,"
        SQL = SQL & vbCrLf & "                                F.FolderName,"
        SQL = SQL & vbCrLf & "                                F.FolderCount,"
        SQL = SQL & vbCrLf & "                                F.FileCount,"
        SQL = SQL & vbCrLf & "                                CAST(R.SourceFolder+F.FolderName+'\' AS VARCHAR(MAX)),"
        SQL = SQL & vbCrLf & "                                F.TargetFolder,"
        SQL = SQL & vbCrLf & "                                CASE"
        SQL = SQL & vbCrLf & "                                WHEN LEN(F.ObjectType) = 0 "
        SQL = SQL & vbCrLf & "                                THEN R.ObjectType"
        SQL = SQL & vbCrLf & "                                ELSE CAST(F.ObjectType AS VARCHAR(MAX)) END,"
        SQL = SQL & vbCrLf & "                                F.Prefix"
        SQL = SQL & vbCrLf & "                    FROM        Folder              F"
        SQL = SQL & vbCrLf & "                    JOIN        Recurse             R"
        SQL = SQL & vbCrLf & "                    ON          F.ParentFolderID = R.FolderID)"
        SQL = SQL & vbCrLf & ""
        SQL = SQL & vbCrLf & "        SELECT      R.ConsentNumber,"
        SQL = SQL & vbCrLf & "                    M.NewApplicationID,"
        SQL = SQL & vbCrLf & "                    M.NewAuthorisationID,"
        SQL = SQL & vbCrLf & "                    R.FolderID,"
        SQL = SQL & vbCrLf & "                    R.ParentFolderID,"
        SQL = SQL & vbCrLf & "                    R.FolderCount,"
        SQL = SQL & vbCrLf & "                    R.FileCount,"
        SQL = SQL & vbCrLf & "                    R.[Level],"
        SQL = SQL & vbCrLf & "                    R.ObjectType,"
        SQL = SQL & vbCrLf & "                    R.Prefix,"
        SQL = SQL & vbCrLf & "                    R.SourceFolder,"
        SQL = SQL & vbCrLf & "                    CASE R.ObjectType"
        SQL = SQL & vbCrLf & "                    WHEN 'Applications'      THEN R.TargetFolder+'\'+ObjectType+'\'+OB1.BusinessID+'\' COLLATE DATABASE_DEFAULT"
        SQL = SQL & vbCrLf & "                    WHEN 'Authorisations'    THEN R.TargetFolder+'\'+ObjectType+'\'+OB2.BusinessID+'\' COLLATE DATABASE_DEFAULT"
        SQL = SQL & vbCrLf & "                    WHEN 'Programmes'        THEN R.TargetFolder+'\'+ObjectType+'\'+OB3.BusinessID+'\' COLLATE DATABASE_DEFAULT"
        SQL = SQL & vbCrLf & "                    WHEN 'Regimes'           THEN R.TargetFolder+'\'+ObjectType+'\'+OB4.BusinessID+'\' COLLATE DATABASE_DEFAULT"
        SQL = SQL & vbCrLf & "                    WHEN 'RegimeActivities'  THEN R.TargetFolder+'\'+ObjectType+'\'+OB5.BusinessID+'\' COLLATE DATABASE_DEFAULT"
        SQL = SQL & vbCrLf & "                    ELSE NULL END                                                  TargetFolder"
        SQL = SQL & vbCrLf & "        FROM        Recurse                                                        R"
        SQL = SQL & vbCrLf & "        LEFT JOIN  (SELECT    A.ID                                                 NewAuthorisationID,"
        SQL = SQL & vbCrLf & "                              P.ID                                                 NewApplicationID,"
        SQL = SQL & vbCrLf & "                              O.OtherIdentifierText                                LegacyID"
        SQL = SQL & vbCrLf & "                    FROM      [IRISENV].dbo.Authorisation                          A"
        SQL = SQL & vbCrLf & "                    LEFT JOIN [IRISENV].dbo.Activity                               C"
        SQL = SQL & vbCrLf & "                    ON        C.ID = A.ActivityID"
        SQL = SQL & vbCrLf & "                    LEFT JOIN [IRISENV].dbo.Application                            P"
        SQL = SQL & vbCrLf & "                    ON        P.ID = C.ApplicationID"
        SQL = SQL & vbCrLf & "                    LEFT JOIN [IRISENV].dbo.OtherIdentifier                        O"
        SQL = SQL & vbCrLf & "                    ON        O.IRISObjectID = A.IRISObjectID"
        SQL = SQL & vbCrLf & "                    WHERE     O.IsDeleted = 0"
        SQL = SQL & vbCrLf & "                    AND       O.IdentifierContextID = 427)                         M"
        SQL = SQL & vbCrLf & "        ON          M.LegacyID = R.ConsentNumber"
        SQL = SQL & vbCrLf & "        LEFT JOIN   [IRISENV].dbo.[Application]                                    APP"
        SQL = SQL & vbCrLf & "        On          APP.MigrationSourceID = M.NewApplicationID"
        SQL = SQL & vbCrLf & "        LEFT JOIN   [IRISENV].dbo.IRISObject                                       OB1"
        SQL = SQL & vbCrLf & "        On          OB1.ID = APP.IRISObjectID"
        SQL = SQL & vbCrLf & "        LEFT JOIN   [IRISENV].dbo.Authorisation                                    ATH"
        SQL = SQL & vbCrLf & "        On          ATH.MigrationSourceID = NewAuthorisationID"
        SQL = SQL & vbCrLf & "        LEFT JOIN   [IRISENV].dbo.IRISObject                                       OB2"
        SQL = SQL & vbCrLf & "        On          OB2.ID = ATH.IRISObjectID"
        SQL = SQL & vbCrLf & "        LEFT JOIN   [IRISENV].dbo.[Programme]                                      PRO"
        SQL = SQL & vbCrLf & "        On          PRO.MigrationSourceID = M.NewApplicationID"
        SQL = SQL & vbCrLf & "        LEFT JOIN   [IRISENV].dbo.IRISObject                                       OB3"
        SQL = SQL & vbCrLf & "        On          OB3.ID = PRO.IRISObjectID"
        SQL = SQL & vbCrLf & "        LEFT JOIN   [IRISENV].dbo.[Regime]                                         REG"
        SQL = SQL & vbCrLf & "        On          REG.MigrationSourceID = M.NewAuthorisationID"
        SQL = SQL & vbCrLf & "        LEFT JOIN   [IRISENV].dbo.IRISObject                                       OB4"
        SQL = SQL & vbCrLf & "        On          OB4.ID = REG.IRISObjectID"
        SQL = SQL & vbCrLf & "        LEFT JOIN   [IRISENV].dbo.[RegimeActivity]                                 RGA"
        SQL = SQL & vbCrLf & "        On          RGA.MigrationSourceID = M.NewAuthorisationID"
        SQL = SQL & vbCrLf & "        LEFT JOIN   [IRISENV].dbo.IRISObject                                       OB5"
        SQL = SQL & vbCrLf & "        On          OB5.ID = REG.IRISObjectID"
        SQL = SQL & vbCrLf & "        WHERE       R.FileCount > 0)"
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

    Private Function GetSQL_CreateFunctions() As String
        Dim SQL As String = ""

        SQL = SQL & SQL_Function_DM_FileMigration()

        Return SQL
    End Function

    Private Function GetSQL_CreateDatabase(ByVal ServerName As String, ByVal DatabaseName As String, ByVal DataFolder As String, ByVal LogFolder As String) As String
        Dim SQL As String = ""
        SQL = SQL & vbCrLf & "If (Select COUNT(*) FROM sys.databases WHERE [Name] = '" & DatabaseName & "') = 0"
        SQL = SQL & vbCrLf & "    BEGIN"
        SQL = SQL & vbCrLf & "        CREATE  DATABASE [" & DatabaseName & "] ON  PRIMARY "
        SQL = SQL & vbCrLf & "               (NAME = N'" & DatabaseName & "', "
        SQL = SQL & vbCrLf & "                FILENAME = N'" & DataFolder & DatabaseName & ".mdf' ,"
        SQL = SQL & vbCrLf & "                SIZE = 3072KB ,"
        SQL = SQL & vbCrLf & "                MAXSIZE = UNLIMITED,"
        SQL = SQL & vbCrLf & "                FILEGROWTH = 1024KB )"
        SQL = SQL & vbCrLf & "        LOG ON "
        SQL = SQL & vbCrLf & "               (NAME = N'" & DatabaseName & "_log', "
        SQL = SQL & vbCrLf & "                FILENAME = N'" & LogFolder & DatabaseName & "_log.ldf' ,"
        SQL = SQL & vbCrLf & "                SIZE = 1024KB ,"
        SQL = SQL & vbCrLf & "                MAXSIZE = 2048GB ,"
        SQL = SQL & vbCrLf & "                FILEGROWTH = 10%)"
        SQL = SQL & vbCrLf & "        ALTER DATABASE [" & DatabaseName & "] SET COMPATIBILITY_LEVEL = 100"
        SQL = SQL & vbCrLf & "        IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))"
        SQL = SQL & vbCrLf & "           BEGIN"
        SQL = SQL & vbCrLf & "              EXEC [" & DatabaseName & "].[dbo].[sp_fulltext_database] @action = 'enable'"
        SQL = SQL & vbCrLf & "           END"
        SQL = SQL & vbCrLf & "        ALTER DATABASE [" & DatabaseName & "] SET ANSI_NULL_DEFAULT OFF "
        SQL = SQL & vbCrLf & "        ALTER DATABASE [" & DatabaseName & "] SET ANSI_NULLS OFF "
        SQL = SQL & vbCrLf & "        ALTER DATABASE [" & DatabaseName & "] SET ANSI_PADDING OFF "
        SQL = SQL & vbCrLf & "        ALTER DATABASE [" & DatabaseName & "] SET ANSI_WARNINGS OFF "
        SQL = SQL & vbCrLf & "        ALTER DATABASE [" & DatabaseName & "] SET ARITHABORT OFF "
        SQL = SQL & vbCrLf & "        ALTER DATABASE [" & DatabaseName & "] SET AUTO_CLOSE OFF "
        SQL = SQL & vbCrLf & "        ALTER DATABASE [" & DatabaseName & "] SET AUTO_CREATE_STATISTICS ON "
        SQL = SQL & vbCrLf & "        ALTER DATABASE [" & DatabaseName & "] SET AUTO_SHRINK OFF "
        SQL = SQL & vbCrLf & "        ALTER DATABASE [" & DatabaseName & "] SET AUTO_UPDATE_STATISTICS ON "
        SQL = SQL & vbCrLf & "        ALTER DATABASE [" & DatabaseName & "] SET CURSOR_CLOSE_ON_COMMIT OFF "
        SQL = SQL & vbCrLf & "        ALTER DATABASE [" & DatabaseName & "] SET CURSOR_DEFAULT  GLOBAL "
        SQL = SQL & vbCrLf & "        ALTER DATABASE [" & DatabaseName & "] SET CONCAT_NULL_YIELDS_NULL OFF "
        SQL = SQL & vbCrLf & "        ALTER DATABASE [" & DatabaseName & "] SET NUMERIC_ROUNDABORT OFF "
        SQL = SQL & vbCrLf & "        ALTER DATABASE [" & DatabaseName & "] SET QUOTED_IDENTIFIER OFF "
        SQL = SQL & vbCrLf & "        ALTER DATABASE [" & DatabaseName & "] SET RECURSIVE_TRIGGERS OFF "
        SQL = SQL & vbCrLf & "        ALTER DATABASE [" & DatabaseName & "] SET  DISABLE_BROKER "
        SQL = SQL & vbCrLf & "        ALTER DATABASE [" & DatabaseName & "] SET AUTO_UPDATE_STATISTICS_ASYNC OFF "
        SQL = SQL & vbCrLf & "        ALTER DATABASE [" & DatabaseName & "] SET DATE_CORRELATION_OPTIMIZATION OFF "
        SQL = SQL & vbCrLf & "        ALTER DATABASE [" & DatabaseName & "] SET TRUSTWORTHY OFF "
        SQL = SQL & vbCrLf & "        ALTER DATABASE [" & DatabaseName & "] SET ALLOW_SNAPSHOT_ISOLATION OFF "
        SQL = SQL & vbCrLf & "        ALTER DATABASE [" & DatabaseName & "] SET PARAMETERIZATION SIMPLE "
        SQL = SQL & vbCrLf & "        ALTER DATABASE [" & DatabaseName & "] SET READ_COMMITTED_SNAPSHOT OFF "
        SQL = SQL & vbCrLf & "        ALTER DATABASE [" & DatabaseName & "] SET HONOR_BROKER_PRIORITY OFF "
        SQL = SQL & vbCrLf & "        ALTER DATABASE [" & DatabaseName & "] SET  READ_WRITE "
        SQL = SQL & vbCrLf & "        ALTER DATABASE [" & DatabaseName & "] SET RECOVERY FULL "
        SQL = SQL & vbCrLf & "        ALTER DATABASE [" & DatabaseName & "] SET  MULTI_USER "
        SQL = SQL & vbCrLf & "        ALTER DATABASE [" & DatabaseName & "] SET PAGE_VERIFY CHECKSUM  "
        SQL = SQL & vbCrLf & "        ALTER DATABASE [" & DatabaseName & "] SET DB_CHAINING OFF "
        SQL = SQL & vbCrLf & "    END"

        Return SQL
    End Function

    Public Function EnsureTables() As String
        Dim msg As String = ""
        Dim wks As New Workspace
        Dim conn As New SqlConnection("Data Source=" & My.Settings.IRIS_Server & ";Initial Catalog=" & My.Settings.Work_Database & ";User=iris_user;Password=iris_user;MultipleActiveResultSets=True")
        conn.Open()

        Dim TableCount As Integer = wks.ExecuteScalar("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'HRC' AND TABLE_NAME IN ('DocumentMigrationFiles','DocumentMigrationFolders','DocumentMigrationProcess','DocumentMigrationProcessTask')")

        If TableCount <> 4 Then
            msg = "Document migration tables have not been created in the workspace database"
            If MsgBox("The document migration tables don't exist in the workspace database. Do you want to create them?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, "Missing Data Tables") = MsgBoxResult.Yes Then
                wks.ExecuteNonQuery(SQL_EnsureSchema())
                wks.ExecuteNonQuery(GetSQL_CreateTables())
                Dim FnExists As Integer = wks.ExecuteScalar("SELECT ISNULL(OBJECT_ID(N'HRC.FileMigration',N'IF'),0)")
                If FnExists = 0 Then
                    wks.ExecuteNonQuery(GetSQL_CreateFunctions().Replace("[IRISENV]", "[" & My.Settings.IRIS_Database & "]"))
                End If
                msg = ""
            End If
        End If

        wks.Dispose()

        Return msg
    End Function

    Public Function VerifyMigrationID() As Boolean
        Dim RetVal As Boolean = False
        Dim wks As New Workspace()

        Dim conn As New SqlConnection("Data Source=" & My.Settings.IRIS_Server & ";Initial Catalog=master;Integrated Security=SSPI;MultipleActiveResultSets=True")
        conn.Open()
        Dim cmd As New SqlCommand
        cmd.Connection = conn

        If My.Settings.FileMigrationID.Length > 0 Then  'Check if exists in db
            If wks.ExecuteScalar("SELECT COUNT(*) AS ItemCount FROM HRC.DocumentMigrationProcess WHERE FileMigrationID = '" & My.Settings.FileMigrationID & "'") = 0 Then
                My.Settings.FileMigrationID = wks.ExecuteScalar("SELECT TOP 1 FileMigrationID FROM HRC.DocumentMigrationProcess")
                My.Settings.FileMigrationID = wks.ExecuteScalar("SELECT TOP 1 MigrationID FROM HRC.DocumentMigrationProcess WHERE FileMigrationID = '" & My.Settings.FileMigrationID & "'")
                My.Settings.SourceRootFolder = wks.ExecuteScalar("SELECT SourceFolder FROM HRC.DocumentMigrationProcess WHERE FileMigrationID = '" & My.Settings.FileMigrationID & "'")
                My.Settings.IRIS_RootFolder = wks.ExecuteScalar("SELECT TargetFolder FROM HRC.DocumentMigrationProcess WHERE FileMigrationID = '" & My.Settings.FileMigrationID & "'")
                RetVal = True
            Else
                If IO.Directory.Exists(My.Settings.SourceRootFolder) AndAlso IO.Directory.Exists(My.Settings.IRIS_RootFolder) Then
                    wks.ExecuteNonQuery("UPDATE HRC.DocumentMigrationProcess SET SourceFolder = '" & StripTrailingSlash(My.Settings.SourceRootFolder) & "', TargetFolder = '" & StripTrailingSlash(My.Settings.IRIS_RootFolder) & "' WHERE FileMigrationID = '" & My.Settings.FileMigrationID & "'")
                    RetVal = True
                Else
                    MsgBox("Can't find one of the migration folders. Please verify the selected folders exist", MsgBoxStyle.Information + MsgBoxStyle.OkOnly, "Missing Folder")
                End If
            End If
        End If

        wks.Dispose()
        cmd = Nothing

        conn.Close()

        Return RetVal
    End Function

    Private Function StripTrailingSlash(ByVal DirectoryName As String) As String
        Return IIf(Right(DirectoryName, 1) = "\", Left(DirectoryName, DirectoryName.Length - 1), DirectoryName)
    End Function

    Public Function GetTask(ByVal TaskID As Integer) As Task
        Dim wks As New Workspace
        Dim RetVal As Task = Nothing

        If wks.ExecuteScalar("SELECT COUNT(*) AS ItemCount FROM HRC.DocumentMigrationProcessTask WHERE MigrationID = '" & My.Settings.FileMigrationID & "' AND TaskID = " & TaskID.ToString) > 0 Then
            RetVal = New Task
            RetVal.TaskID = TaskID
            RetVal.ErrorText = wks.ExecuteScalar("SELECT ISNULL(ErrorText,'') FROM HRC.DocumentMigrationProcessTask WHERE MigrationID = '" & My.Settings.FileMigrationID & "' AND TaskID = " & TaskID.ToString)
        End If

        wks.Dispose()

        Return RetVal
    End Function

    Public Sub UpdateTask(ByVal TaskID As Integer, ByVal ErrorText As String)
        Dim wks As New Workspace

        If wks.ExecuteScalar("SELECT COUNT(*) AS ItemCount FROM HRC.DocumentMigrationProcessTask WHERE FileMigrationID = '" & My.Settings.FileMigrationID & "' AND TaskID = " & TaskID.ToString) = 0 Then
            wks.CommandText = "INSERT INTO HRC.DocumentMigrationProcessTask(MigrationID,FileMigrationID,TaskID,ErrorText) VALUES(" & My.Settings.MigrationID & ",'" & My.Settings.FileMigrationID & "'," & TaskID.ToString & "," & IIf(ErrorText.Length = 0, "NULL", "'" & ErrorText.Replace("'", "''") & "'") & ")"
        Else
            wks.CommandText = "UPDATE HRC.DocumentMigrationProcessTask SET ErrorText = " & IIf(ErrorText.Length = 0, "NULL", "'" & ErrorText.Replace("'", "''") & "'") & " WHERE FileMigrationID = '" & My.Settings.FileMigrationID & "' AND TaskID = " & TaskID.ToString
        End If
        wks.ExecuteNonQuery()

        wks.Dispose()

    End Sub

    Public Sub ResetTasksFrom(ByVal TaskID As Integer)
        Dim wks As New Workspace

        wks.CommandText = "DELETE FROM HRC.DocumentMigrationProcessTask WHERE FileMigrationID = '" & My.Settings.FileMigrationID & "' AND TaskID > " & TaskID.ToString
        wks.ExecuteNonQuery()
        wks.Dispose()

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

        Public Function GetIndex(ByVal ConsentNumber As String) As Integer
            Dim Lo As Integer = 0
            Dim Hi As Integer = DT.Rows.Count - 1
            Dim Found As Boolean = False
            Dim i As Integer = 0

            Do While Lo <= Hi
                i = (Lo + Hi) / 2
                If DT.Rows(i)(Field.ConsentNumber) = ConsentNumber Then
                    Found = True
                    Exit Do
                ElseIf DT.Rows(i)(Field.ConsentNumber) > ConsentNumber Then
                    Hi = i - 1
                Else
                    Lo = i + 1
                End If
            Loop

            If Found Then
                Return i
            Else
                Return -i - 1
            End If

        End Function

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

        Public Sub Load()
            DT.Rows.Clear()
            DT.Columns.Clear()
            Dim Conn As String = "Data Source=" & My.Settings.IRIS_Server & ";User=iris_user;Password=iris_user;Initial Catalog=" & My.Settings.IRIS_Database & ";MultipleActiveResultSets=True"
            Dim SQL As String = SQL_Query()
            Dim da As New SqlDataAdapter(SQL, Conn)
            da.Fill(DT)
            da = Nothing
        End Sub

        Private Function SQL_Query()
            Dim SQL As String = ""
            SQL = SQL & vbCrLf & "DECLARE @ObjectID AS INTEGER        = (SELECT ID FROM ReferenceDataCollection WHERE FunctionalArea = 'Object Hierarchy'     AND Code = 'ObjectType')"
            SQL = SQL & vbCrLf & "DECLARE @IdentifierID AS INTEGER    = (SELECT ID FROM ReferenceDataCollection WHERE FunctionalArea = 'Other Identifiers'    AND Code = 'IdentifierContexts')"
            SQL = SQL & vbCrLf & "DECLARE @ApplicationID AS INTEGER   = (SELECT ID FROM ReferenceDataValue      WHERE CollectionID   = @ObjectID              AND Code = 'Application')"
            SQL = SQL & vbCrLf & "DECLARE @AuthorisationID AS INTEGER = (SELECT ID FROM ReferenceDataValue      WHERE CollectionID   = @ObjectID              AND Code = 'Authorisation')"
            SQL = SQL & vbCrLf & "DECLARE @APP_ContextID AS INTEGER   = (SELECT ID FROM ReferenceDataValue      WHERE CollectionID   = @IdentifierID          AND ParentValueID = @ApplicationID   AND Code = 'LegacyNumber')"
            SQL = SQL & vbCrLf & "DECLARE @ATH_ContextID AS INTEGER   = (SELECT ID FROM ReferenceDataValue      WHERE CollectionID   = @IdentifierID          AND ParentValueID = @AuthorisationID AND Code = 'LegacyNumber')"
            SQL = SQL & vbCrLf & ""
            SQL = SQL & vbCrLf & "SELECT          APPS.R2_AuthorisationNumber,"
            SQL = SQL & vbCrLf & "                APPS.APP_BusinessID,"
            SQL = SQL & vbCrLf & "                AUTHS.ATH_BusinessID,"
            SQL = SQL & vbCrLf & "                APPS.APP_IRISID,"
            SQL = SQL & vbCrLf & "                AUTHS.ATH_IRISID"
            SQL = SQL & vbCrLf & "FROM           (SELECT      REPLACE(OtherIdentifierText,'/','-')                                    AS R2_AuthorisationNumber,"
            SQL = SQL & vbCrLf & "                            OID.IRISObjectID                                                        AS APP_IRISID,"
            SQL = SQL & vbCrLf & "                            IOB.BusinessID                                                          AS APP_BusinessID"
            SQL = SQL & vbCrLf & "                FROM        OtherIdentifier                                                         AS OID"
            SQL = SQL & vbCrLf & "                LEFT JOIN   IRISObject                                                              AS IOB"
            SQL = SQL & vbCrLf & "                ON          IOB.ID = OID.IRISObjectID"
            SQL = SQL & vbCrLf & "                WHERE       IRISObjectID "
            SQL = SQL & vbCrLf & "                IN         (SELECT ID FROM IRISObject WHERE ObjectTypeID = @ApplicationID AND IdentifierContextID = @APP_ContextID)"
            SQL = SQL & vbCrLf & "                )                                                                                   AS APPS"
            SQL = SQL & vbCrLf & "FULL OUTER JOIN(SELECT      REPLACE(OtherIdentifierText,'/','-')                                    AS R2_AuthorisationNumber,"
            SQL = SQL & vbCrLf & "                            OID.IRISObjectID                                                        AS ATH_IRISID,"
            SQL = SQL & vbCrLf & "                            IOB.BusinessID                                                          AS ATH_BusinessID"
            SQL = SQL & vbCrLf & "                            FROM        OtherIdentifier                                             AS OID"
            SQL = SQL & vbCrLf & "                            LEFT JOIN   IRISObject                                                  AS IOB"
            SQL = SQL & vbCrLf & "                            ON          IOB.ID = OID.IRISObjectID"
            SQL = SQL & vbCrLf & "                            WHERE       IRISObjectID "
            SQL = SQL & vbCrLf & "                            IN         (SELECT ID FROM IRISObject WHERE ObjectTypeID = @AuthorisationID AND IdentifierContextID = @ATH_ContextID)"
            SQL = SQL & vbCrLf & "                            )                                                                       AS AUTHS"
            SQL = SQL & vbCrLf & "ON              APPS.R2_AuthorisationNumber = AUTHS.R2_AuthorisationNumber"
            Return SQL
        End Function

        Public Sub Dispose()
            DT = Nothing
        End Sub
    End Class

End Module
