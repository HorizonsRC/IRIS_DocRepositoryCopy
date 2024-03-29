﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.fbdGetFolder = New System.Windows.Forms.FolderBrowserDialog()
        Me.ofdGetFolder = New System.Windows.Forms.OpenFileDialog()
        Me.ToolTips = New System.Windows.Forms.ToolTip(Me.components)
        Me.cmbWork_Database = New System.Windows.Forms.ComboBox()
        Me.txtIRIS_Server = New System.Windows.Forms.TextBox()
        Me.cmbIRIS_Database = New System.Windows.Forms.ComboBox()
        Me.txtIRIS_Root = New System.Windows.Forms.TextBox()
        Me.txtSourceRoot = New System.Windows.Forms.TextBox()
        Me.btnCreateWorkspace = New System.Windows.Forms.Button()
        Me.cmdPopulateFolderTable = New System.Windows.Forms.Button()
        Me.cmdMigrateFiles = New System.Windows.Forms.Button()
        Me.cmdIdentifyDuplicates = New System.Windows.Forms.Button()
        Me.txtMigrationID = New System.Windows.Forms.TextBox()
        Me.cmbObjectType = New System.Windows.Forms.ComboBox()
        Me.img = New System.Windows.Forms.ImageList(Me.components)
        Me.cmdBrowseIRIS = New System.Windows.Forms.Button()
        Me.lblRootFolder = New System.Windows.Forms.Label()
        Me.cmdBrowseR2D2 = New System.Windows.Forms.Button()
        Me.lblSourceRootFolder = New System.Windows.Forms.Label()
        Me.status = New System.Windows.Forms.StatusStrip()
        Me.lblStatus = New System.Windows.Forms.ToolStripStatusLabel()
        Me.lblHeader3 = New System.Windows.Forms.Label()
        Me.lblIRIS_Database = New System.Windows.Forms.Label()
        Me.lblIRIS_Server = New System.Windows.Forms.Label()
        Me.lblHeader2 = New System.Windows.Forms.Label()
        Me.lblHeader1 = New System.Windows.Forms.Label()
        Me.lblHeader0 = New System.Windows.Forms.Label()
        Me.lblWorkspaceDatabase = New System.Windows.Forms.Label()
        Me.cmbFileMigrationID = New System.Windows.Forms.ComboBox()
        Me.lblFileMigrationID = New System.Windows.Forms.Label()
        Me.lblMigrationID = New System.Windows.Forms.Label()
        Me.lblObjectType = New System.Windows.Forms.Label()
        Me.cmbIdentifierContext = New System.Windows.Forms.ComboBox()
        Me.lblIdentifierContext = New System.Windows.Forms.Label()
        Me.status.SuspendLayout()
        Me.SuspendLayout()
        '
        'ofdGetFolder
        '
        Me.ofdGetFolder.FileName = "OpenFileDialog1"
        '
        'cmbWork_Database
        '
        Me.cmbWork_Database.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest
        Me.cmbWork_Database.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.cmbWork_Database.Enabled = False
        Me.cmbWork_Database.Font = New System.Drawing.Font("Calibri", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmbWork_Database.FormattingEnabled = True
        Me.cmbWork_Database.Location = New System.Drawing.Point(284, 218)
        Me.cmbWork_Database.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.cmbWork_Database.MaxDropDownItems = 20
        Me.cmbWork_Database.Name = "cmbWork_Database"
        Me.cmbWork_Database.Size = New System.Drawing.Size(250, 32)
        Me.cmbWork_Database.TabIndex = 47
        Me.cmbWork_Database.TabStop = False
        Me.ToolTips.SetToolTip(Me.cmbWork_Database, "The name of the database on the above server where the necessary workspace tables" &
        " can be created")
        '
        'txtIRIS_Server
        '
        Me.txtIRIS_Server.Font = New System.Drawing.Font("Calibri", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtIRIS_Server.Location = New System.Drawing.Point(284, 54)
        Me.txtIRIS_Server.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.txtIRIS_Server.Name = "txtIRIS_Server"
        Me.txtIRIS_Server.Size = New System.Drawing.Size(250, 31)
        Me.txtIRIS_Server.TabIndex = 1
        Me.ToolTips.SetToolTip(Me.txtIRIS_Server, "The name of the SQL server holding the IRIS database")
        '
        'cmbIRIS_Database
        '
        Me.cmbIRIS_Database.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest
        Me.cmbIRIS_Database.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.cmbIRIS_Database.Enabled = False
        Me.cmbIRIS_Database.Font = New System.Drawing.Font("Calibri", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmbIRIS_Database.FormattingEnabled = True
        Me.cmbIRIS_Database.Location = New System.Drawing.Point(284, 95)
        Me.cmbIRIS_Database.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.cmbIRIS_Database.MaxDropDownItems = 20
        Me.cmbIRIS_Database.Name = "cmbIRIS_Database"
        Me.cmbIRIS_Database.Size = New System.Drawing.Size(250, 32)
        Me.cmbIRIS_Database.TabIndex = 56
        Me.cmbIRIS_Database.TabStop = False
        Me.ToolTips.SetToolTip(Me.cmbIRIS_Database, "The name of the IRIS database holding the Contacts, Applications and Authorisatio" &
        "ns data")
        '
        'txtIRIS_Root
        '
        Me.txtIRIS_Root.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtIRIS_Root.Font = New System.Drawing.Font("Calibri", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtIRIS_Root.Location = New System.Drawing.Point(284, 431)
        Me.txtIRIS_Root.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.txtIRIS_Root.Name = "txtIRIS_Root"
        Me.txtIRIS_Root.Size = New System.Drawing.Size(930, 31)
        Me.txtIRIS_Root.TabIndex = 3
        Me.ToolTips.SetToolTip(Me.txtIRIS_Root, "Root folder where new folders will be created and documents copied")
        '
        'txtSourceRoot
        '
        Me.txtSourceRoot.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtSourceRoot.Font = New System.Drawing.Font("Calibri", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtSourceRoot.Location = New System.Drawing.Point(284, 345)
        Me.txtSourceRoot.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.txtSourceRoot.Name = "txtSourceRoot"
        Me.txtSourceRoot.Size = New System.Drawing.Size(930, 31)
        Me.txtSourceRoot.TabIndex = 2
        Me.ToolTips.SetToolTip(Me.txtSourceRoot, "Root folder where the source documents are stored")
        '
        'btnCreateWorkspace
        '
        Me.btnCreateWorkspace.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnCreateWorkspace.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnCreateWorkspace.Location = New System.Drawing.Point(282, 495)
        Me.btnCreateWorkspace.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnCreateWorkspace.Name = "btnCreateWorkspace"
        Me.btnCreateWorkspace.Size = New System.Drawing.Size(226, 68)
        Me.btnCreateWorkspace.TabIndex = 4
        Me.btnCreateWorkspace.Text = "1: Create Workspace"
        Me.ToolTips.SetToolTip(Me.btnCreateWorkspace, "Creates workspace tables in the nominated database (used for managing file transf" &
        "ers and logging)")
        Me.btnCreateWorkspace.UseVisualStyleBackColor = True
        '
        'cmdPopulateFolderTable
        '
        Me.cmdPopulateFolderTable.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdPopulateFolderTable.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdPopulateFolderTable.Location = New System.Drawing.Point(518, 495)
        Me.cmdPopulateFolderTable.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.cmdPopulateFolderTable.Name = "cmdPopulateFolderTable"
        Me.cmdPopulateFolderTable.Size = New System.Drawing.Size(226, 68)
        Me.cmdPopulateFolderTable.TabIndex = 5
        Me.cmdPopulateFolderTable.Text = "2: Compile Mapping Data"
        Me.ToolTips.SetToolTip(Me.cmdPopulateFolderTable, "Recursively reads source folders and compiles file-lists to be used for the final" &
        " file copying process")
        Me.cmdPopulateFolderTable.UseVisualStyleBackColor = True
        '
        'cmdMigrateFiles
        '
        Me.cmdMigrateFiles.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdMigrateFiles.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdMigrateFiles.Location = New System.Drawing.Point(988, 495)
        Me.cmdMigrateFiles.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.cmdMigrateFiles.Name = "cmdMigrateFiles"
        Me.cmdMigrateFiles.Size = New System.Drawing.Size(226, 68)
        Me.cmdMigrateFiles.TabIndex = 6
        Me.cmdMigrateFiles.Text = "4: Copy to New Location"
        Me.ToolTips.SetToolTip(Me.cmdMigrateFiles, "Uses the compiled mapping data to copy the files and log progress")
        Me.cmdMigrateFiles.UseVisualStyleBackColor = True
        '
        'cmdIdentifyDuplicates
        '
        Me.cmdIdentifyDuplicates.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdIdentifyDuplicates.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdIdentifyDuplicates.Location = New System.Drawing.Point(753, 495)
        Me.cmdIdentifyDuplicates.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.cmdIdentifyDuplicates.Name = "cmdIdentifyDuplicates"
        Me.cmdIdentifyDuplicates.Size = New System.Drawing.Size(226, 68)
        Me.cmdIdentifyDuplicates.TabIndex = 60
        Me.cmdIdentifyDuplicates.Text = "3: Identify Duplicates"
        Me.ToolTips.SetToolTip(Me.cmdIdentifyDuplicates, "Uses the compiled mapping data to copy the files and log progress")
        Me.cmdIdentifyDuplicates.UseVisualStyleBackColor = True
        '
        'txtMigrationID
        '
        Me.txtMigrationID.Font = New System.Drawing.Font("Calibri", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtMigrationID.Location = New System.Drawing.Point(837, 94)
        Me.txtMigrationID.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.txtMigrationID.Name = "txtMigrationID"
        Me.txtMigrationID.Size = New System.Drawing.Size(376, 31)
        Me.txtMigrationID.TabIndex = 63
        Me.ToolTips.SetToolTip(Me.txtMigrationID, "An ID number for this migration (used to separate multiple migrations for the sam" &
        "e File Migration Code)")
        '
        'cmbObjectType
        '
        Me.cmbObjectType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest
        Me.cmbObjectType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.cmbObjectType.Font = New System.Drawing.Font("Calibri", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmbObjectType.FormattingEnabled = True
        Me.cmbObjectType.Location = New System.Drawing.Point(837, 135)
        Me.cmbObjectType.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.cmbObjectType.MaxDropDownItems = 20
        Me.cmbObjectType.Name = "cmbObjectType"
        Me.cmbObjectType.Size = New System.Drawing.Size(376, 32)
        Me.cmbObjectType.TabIndex = 67
        Me.cmbObjectType.TabStop = False
        Me.ToolTips.SetToolTip(Me.cmbObjectType, "The name of the object whose data files are being migrated")
        '
        'img
        '
        Me.img.ImageStream = CType(resources.GetObject("img.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.img.TransparentColor = System.Drawing.Color.Transparent
        Me.img.Images.SetKeyName(0, "Folder - Explorer.png")
        '
        'cmdBrowseIRIS
        '
        Me.cmdBrowseIRIS.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdBrowseIRIS.Font = New System.Drawing.Font("Tahoma", 8.25!)
        Me.cmdBrowseIRIS.Location = New System.Drawing.Point(1224, 431)
        Me.cmdBrowseIRIS.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.cmdBrowseIRIS.Name = "cmdBrowseIRIS"
        Me.cmdBrowseIRIS.Size = New System.Drawing.Size(40, 32)
        Me.cmdBrowseIRIS.TabIndex = 40
        Me.cmdBrowseIRIS.TabStop = False
        Me.cmdBrowseIRIS.Text = "..."
        Me.cmdBrowseIRIS.UseVisualStyleBackColor = True
        '
        'lblRootFolder
        '
        Me.lblRootFolder.Font = New System.Drawing.Font("Calibri", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblRootFolder.ForeColor = System.Drawing.SystemColors.ControlDark
        Me.lblRootFolder.Location = New System.Drawing.Point(36, 432)
        Me.lblRootFolder.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblRootFolder.Name = "lblRootFolder"
        Me.lblRootFolder.Size = New System.Drawing.Size(238, 28)
        Me.lblRootFolder.TabIndex = 39
        Me.lblRootFolder.Text = "DOCUMENTS ROOT FOLDER"
        Me.lblRootFolder.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'cmdBrowseR2D2
        '
        Me.cmdBrowseR2D2.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdBrowseR2D2.Font = New System.Drawing.Font("Tahoma", 8.25!)
        Me.cmdBrowseR2D2.Location = New System.Drawing.Point(1224, 345)
        Me.cmdBrowseR2D2.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.cmdBrowseR2D2.Name = "cmdBrowseR2D2"
        Me.cmdBrowseR2D2.Size = New System.Drawing.Size(40, 32)
        Me.cmdBrowseR2D2.TabIndex = 37
        Me.cmdBrowseR2D2.TabStop = False
        Me.cmdBrowseR2D2.Text = "..."
        Me.cmdBrowseR2D2.UseVisualStyleBackColor = True
        '
        'lblSourceRootFolder
        '
        Me.lblSourceRootFolder.Font = New System.Drawing.Font("Calibri", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSourceRootFolder.ForeColor = System.Drawing.SystemColors.ControlDark
        Me.lblSourceRootFolder.Location = New System.Drawing.Point(34, 346)
        Me.lblSourceRootFolder.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblSourceRootFolder.Name = "lblSourceRootFolder"
        Me.lblSourceRootFolder.Size = New System.Drawing.Size(238, 28)
        Me.lblSourceRootFolder.TabIndex = 36
        Me.lblSourceRootFolder.Text = "DOCUMENTS ROOT FOLDER"
        Me.lblSourceRootFolder.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'status
        '
        Me.status.GripMargin = New System.Windows.Forms.Padding(0)
        Me.status.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.status.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.lblStatus})
        Me.status.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow
        Me.status.Location = New System.Drawing.Point(0, 606)
        Me.status.Name = "status"
        Me.status.Padding = New System.Windows.Forms.Padding(2, 0, 21, 0)
        Me.status.Size = New System.Drawing.Size(1281, 28)
        Me.status.SizingGrip = False
        Me.status.TabIndex = 35
        Me.status.Text = "Status"
        '
        'lblStatus
        '
        Me.lblStatus.BackColor = System.Drawing.SystemColors.Control
        Me.lblStatus.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.lblStatus.Margin = New System.Windows.Forms.Padding(0, 2, 0, 1)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(60, 25)
        Me.lblStatus.Spring = True
        Me.lblStatus.Text = "Ready"
        Me.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblHeader3
        '
        Me.lblHeader3.AutoSize = True
        Me.lblHeader3.Font = New System.Drawing.Font("Verdana", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblHeader3.ForeColor = System.Drawing.SystemColors.ActiveCaption
        Me.lblHeader3.Location = New System.Drawing.Point(20, 23)
        Me.lblHeader3.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblHeader3.Name = "lblHeader3"
        Me.lblHeader3.Size = New System.Drawing.Size(158, 22)
        Me.lblHeader3.TabIndex = 58
        Me.lblHeader3.Text = "IRIS Database"
        '
        'lblIRIS_Database
        '
        Me.lblIRIS_Database.Font = New System.Drawing.Font("Calibri", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblIRIS_Database.ForeColor = System.Drawing.SystemColors.ControlDark
        Me.lblIRIS_Database.Location = New System.Drawing.Point(38, 95)
        Me.lblIRIS_Database.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblIRIS_Database.Name = "lblIRIS_Database"
        Me.lblIRIS_Database.Size = New System.Drawing.Size(238, 32)
        Me.lblIRIS_Database.TabIndex = 57
        Me.lblIRIS_Database.Text = "DATABASE"
        Me.lblIRIS_Database.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblIRIS_Server
        '
        Me.lblIRIS_Server.Font = New System.Drawing.Font("Calibri", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblIRIS_Server.ForeColor = System.Drawing.SystemColors.ControlDark
        Me.lblIRIS_Server.Location = New System.Drawing.Point(38, 54)
        Me.lblIRIS_Server.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblIRIS_Server.Name = "lblIRIS_Server"
        Me.lblIRIS_Server.Size = New System.Drawing.Size(238, 31)
        Me.lblIRIS_Server.TabIndex = 54
        Me.lblIRIS_Server.Text = "SERVER"
        Me.lblIRIS_Server.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblHeader2
        '
        Me.lblHeader2.AutoSize = True
        Me.lblHeader2.Font = New System.Drawing.Font("Verdana", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblHeader2.ForeColor = System.Drawing.SystemColors.ActiveCaption
        Me.lblHeader2.Location = New System.Drawing.Point(20, 178)
        Me.lblHeader2.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblHeader2.Name = "lblHeader2"
        Me.lblHeader2.Size = New System.Drawing.Size(223, 22)
        Me.lblHeader2.TabIndex = 51
        Me.lblHeader2.Text = "Workspace Database"
        '
        'lblHeader1
        '
        Me.lblHeader1.AutoSize = True
        Me.lblHeader1.Font = New System.Drawing.Font("Verdana", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblHeader1.ForeColor = System.Drawing.SystemColors.ActiveCaption
        Me.lblHeader1.Location = New System.Drawing.Point(20, 394)
        Me.lblHeader1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblHeader1.Name = "lblHeader1"
        Me.lblHeader1.Size = New System.Drawing.Size(199, 22)
        Me.lblHeader1.TabIndex = 50
        Me.lblHeader1.Text = "Destination (IRIS)"
        '
        'lblHeader0
        '
        Me.lblHeader0.AutoSize = True
        Me.lblHeader0.Font = New System.Drawing.Font("Verdana", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblHeader0.ForeColor = System.Drawing.SystemColors.ActiveCaption
        Me.lblHeader0.Location = New System.Drawing.Point(20, 303)
        Me.lblHeader0.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblHeader0.Name = "lblHeader0"
        Me.lblHeader0.Size = New System.Drawing.Size(170, 22)
        Me.lblHeader0.TabIndex = 49
        Me.lblHeader0.Text = "Source (R2-D2)"
        '
        'lblWorkspaceDatabase
        '
        Me.lblWorkspaceDatabase.Font = New System.Drawing.Font("Calibri", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblWorkspaceDatabase.ForeColor = System.Drawing.SystemColors.ControlDark
        Me.lblWorkspaceDatabase.Location = New System.Drawing.Point(38, 218)
        Me.lblWorkspaceDatabase.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblWorkspaceDatabase.Name = "lblWorkspaceDatabase"
        Me.lblWorkspaceDatabase.Size = New System.Drawing.Size(238, 32)
        Me.lblWorkspaceDatabase.TabIndex = 48
        Me.lblWorkspaceDatabase.Text = "DATABASE"
        Me.lblWorkspaceDatabase.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'cmbFileMigrationID
        '
        Me.cmbFileMigrationID.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmbFileMigrationID.Font = New System.Drawing.Font("Calibri", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmbFileMigrationID.FormattingEnabled = True
        Me.cmbFileMigrationID.Location = New System.Drawing.Point(837, 52)
        Me.cmbFileMigrationID.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.cmbFileMigrationID.Name = "cmbFileMigrationID"
        Me.cmbFileMigrationID.Size = New System.Drawing.Size(376, 32)
        Me.cmbFileMigrationID.TabIndex = 61
        Me.ToolTips.SetToolTip(Me.cmbFileMigrationID, "A name for the migration indicating the kind of files being moved")
        '
        'lblFileMigrationID
        '
        Me.lblFileMigrationID.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblFileMigrationID.Font = New System.Drawing.Font("Calibri", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblFileMigrationID.ForeColor = System.Drawing.SystemColors.ControlDark
        Me.lblFileMigrationID.Location = New System.Drawing.Point(613, 52)
        Me.lblFileMigrationID.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblFileMigrationID.Name = "lblFileMigrationID"
        Me.lblFileMigrationID.Size = New System.Drawing.Size(216, 31)
        Me.lblFileMigrationID.TabIndex = 62
        Me.lblFileMigrationID.Text = "FILE MIGRATION ID"
        Me.lblFileMigrationID.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblMigrationID
        '
        Me.lblMigrationID.Font = New System.Drawing.Font("Calibri", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblMigrationID.ForeColor = System.Drawing.SystemColors.ControlDark
        Me.lblMigrationID.Location = New System.Drawing.Point(617, 94)
        Me.lblMigrationID.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblMigrationID.Name = "lblMigrationID"
        Me.lblMigrationID.Size = New System.Drawing.Size(212, 31)
        Me.lblMigrationID.TabIndex = 64
        Me.lblMigrationID.Text = "MIGRATION ID"
        Me.lblMigrationID.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblObjectType
        '
        Me.lblObjectType.Font = New System.Drawing.Font("Calibri", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblObjectType.ForeColor = System.Drawing.SystemColors.ControlDark
        Me.lblObjectType.Location = New System.Drawing.Point(617, 135)
        Me.lblObjectType.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblObjectType.Name = "lblObjectType"
        Me.lblObjectType.Size = New System.Drawing.Size(212, 31)
        Me.lblObjectType.TabIndex = 66
        Me.lblObjectType.Text = "OBJECT TYPE"
        Me.lblObjectType.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'cmbIdentifierContext
        '
        Me.cmbIdentifierContext.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest
        Me.cmbIdentifierContext.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.cmbIdentifierContext.Font = New System.Drawing.Font("Calibri", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmbIdentifierContext.FormattingEnabled = True
        Me.cmbIdentifierContext.Location = New System.Drawing.Point(837, 177)
        Me.cmbIdentifierContext.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.cmbIdentifierContext.MaxDropDownItems = 20
        Me.cmbIdentifierContext.Name = "cmbIdentifierContext"
        Me.cmbIdentifierContext.Size = New System.Drawing.Size(376, 32)
        Me.cmbIdentifierContext.TabIndex = 69
        Me.cmbIdentifierContext.TabStop = False
        Me.ToolTips.SetToolTip(Me.cmbIdentifierContext, "The identifier to be used for matching folder names in the source folders")
        '
        'lblIdentifierContext
        '
        Me.lblIdentifierContext.Font = New System.Drawing.Font("Calibri", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblIdentifierContext.ForeColor = System.Drawing.SystemColors.ControlDark
        Me.lblIdentifierContext.Location = New System.Drawing.Point(617, 177)
        Me.lblIdentifierContext.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblIdentifierContext.Name = "lblIdentifierContext"
        Me.lblIdentifierContext.Size = New System.Drawing.Size(212, 31)
        Me.lblIdentifierContext.TabIndex = 68
        Me.lblIdentifierContext.Text = "IDENTIFIER CONTEXT"
        Me.lblIdentifierContext.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Window
        Me.ClientSize = New System.Drawing.Size(1281, 634)
        Me.Controls.Add(Me.cmbIdentifierContext)
        Me.Controls.Add(Me.lblIdentifierContext)
        Me.Controls.Add(Me.cmbObjectType)
        Me.Controls.Add(Me.lblObjectType)
        Me.Controls.Add(Me.txtMigrationID)
        Me.Controls.Add(Me.lblMigrationID)
        Me.Controls.Add(Me.lblFileMigrationID)
        Me.Controls.Add(Me.cmbFileMigrationID)
        Me.Controls.Add(Me.cmdIdentifyDuplicates)
        Me.Controls.Add(Me.status)
        Me.Controls.Add(Me.btnCreateWorkspace)
        Me.Controls.Add(Me.txtIRIS_Server)
        Me.Controls.Add(Me.cmdPopulateFolderTable)
        Me.Controls.Add(Me.lblSourceRootFolder)
        Me.Controls.Add(Me.cmdBrowseIRIS)
        Me.Controls.Add(Me.cmdBrowseR2D2)
        Me.Controls.Add(Me.lblHeader3)
        Me.Controls.Add(Me.lblRootFolder)
        Me.Controls.Add(Me.cmbIRIS_Database)
        Me.Controls.Add(Me.txtIRIS_Root)
        Me.Controls.Add(Me.lblIRIS_Database)
        Me.Controls.Add(Me.lblIRIS_Server)
        Me.Controls.Add(Me.txtSourceRoot)
        Me.Controls.Add(Me.cmdMigrateFiles)
        Me.Controls.Add(Me.lblWorkspaceDatabase)
        Me.Controls.Add(Me.lblHeader2)
        Me.Controls.Add(Me.cmbWork_Database)
        Me.Controls.Add(Me.lblHeader1)
        Me.Controls.Add(Me.lblHeader0)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.Name = "frmMain"
        Me.Text = "IRIS Consents Document Migration"
        Me.status.ResumeLayout(False)
        Me.status.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents fbdGetFolder As System.Windows.Forms.FolderBrowserDialog
    Friend WithEvents ofdGetFolder As System.Windows.Forms.OpenFileDialog
    Friend WithEvents ToolTips As System.Windows.Forms.ToolTip
    Friend WithEvents img As System.Windows.Forms.ImageList
    Friend WithEvents cmdBrowseIRIS As System.Windows.Forms.Button
    Friend WithEvents lblRootFolder As System.Windows.Forms.Label
    Friend WithEvents txtIRIS_Root As System.Windows.Forms.TextBox
    Friend WithEvents cmdBrowseR2D2 As System.Windows.Forms.Button
    Friend WithEvents lblSourceRootFolder As System.Windows.Forms.Label
    Friend WithEvents txtSourceRoot As System.Windows.Forms.TextBox
    Friend WithEvents status As System.Windows.Forms.StatusStrip
    Friend WithEvents lblStatus As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents lblHeader2 As System.Windows.Forms.Label
    Friend WithEvents lblHeader1 As System.Windows.Forms.Label
    Friend WithEvents lblHeader0 As System.Windows.Forms.Label
    Friend WithEvents cmbWork_Database As System.Windows.Forms.ComboBox
    Friend WithEvents lblWorkspaceDatabase As System.Windows.Forms.Label
    Friend WithEvents cmdMigrateFiles As System.Windows.Forms.Button
    Friend WithEvents lblHeader3 As System.Windows.Forms.Label
    Friend WithEvents txtIRIS_Server As System.Windows.Forms.TextBox
    Friend WithEvents lblIRIS_Server As System.Windows.Forms.Label
    Friend WithEvents cmdPopulateFolderTable As System.Windows.Forms.Button
    Friend WithEvents cmbIRIS_Database As System.Windows.Forms.ComboBox
    Friend WithEvents lblIRIS_Database As System.Windows.Forms.Label
    Friend WithEvents btnCreateWorkspace As System.Windows.Forms.Button
    Friend WithEvents cmdIdentifyDuplicates As Button
    Friend WithEvents cmbFileMigrationID As ComboBox
    Friend WithEvents lblFileMigrationID As Label
    Friend WithEvents txtMigrationID As TextBox
    Friend WithEvents lblMigrationID As Label
    Friend WithEvents lblObjectType As Label
    Friend WithEvents cmbObjectType As ComboBox
    Friend WithEvents cmbIdentifierContext As ComboBox
    Friend WithEvents lblIdentifierContext As Label
End Class
