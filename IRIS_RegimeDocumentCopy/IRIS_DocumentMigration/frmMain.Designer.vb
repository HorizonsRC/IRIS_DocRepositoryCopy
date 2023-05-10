<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.fbdGetFolder = New System.Windows.Forms.FolderBrowserDialog()
        Me.ofdGetFolder = New System.Windows.Forms.OpenFileDialog()
        Me.ToolTips = New System.Windows.Forms.ToolTip(Me.components)
        Me.txtIRIS_Server = New System.Windows.Forms.TextBox()
        Me.txtIRIS_Root = New System.Windows.Forms.TextBox()
        Me.btnCreateWorkspace = New System.Windows.Forms.Button()
        Me.cmdPopulateFolderTable = New System.Windows.Forms.Button()
        Me.cmdMigrateFiles = New System.Windows.Forms.Button()
        Me.cmdIdentifyDuplicates = New System.Windows.Forms.Button()
        Me.img = New System.Windows.Forms.ImageList(Me.components)
        Me.cmdBrowseIRIS = New System.Windows.Forms.Button()
        Me.lblRootFolder = New System.Windows.Forms.Label()
        Me.status = New System.Windows.Forms.StatusStrip()
        Me.lblStatus = New System.Windows.Forms.ToolStripStatusLabel()
        Me.lblHeader3 = New System.Windows.Forms.Label()
        Me.lblIRIS_Database = New System.Windows.Forms.Label()
        Me.lblIRIS_Server = New System.Windows.Forms.Label()
        Me.lblHeader1 = New System.Windows.Forms.Label()
        Me.lblWorkspaceDatabase = New System.Windows.Forms.Label()
        Me.lblMigrationID = New System.Windows.Forms.Label()
        Me.cmbMigrationID = New System.Windows.Forms.ComboBox()
        Me.txtIRIS_Database = New System.Windows.Forms.TextBox()
        Me.txtMigration_Database = New System.Windows.Forms.TextBox()
        Me.btnAdd = New System.Windows.Forms.Button()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.lstSourceRootFolder = New System.Windows.Forms.DataGridView()
        Me.status.SuspendLayout()
        Me.Panel1.SuspendLayout()
        CType(Me.lstSourceRootFolder, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'ofdGetFolder
        '
        Me.ofdGetFolder.FileName = "OpenFileDialog1"
        '
        'txtIRIS_Server
        '
        Me.txtIRIS_Server.Font = New System.Drawing.Font("Calibri", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtIRIS_Server.Location = New System.Drawing.Point(276, 62)
        Me.txtIRIS_Server.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.txtIRIS_Server.Name = "txtIRIS_Server"
        Me.txtIRIS_Server.Size = New System.Drawing.Size(250, 31)
        Me.txtIRIS_Server.TabIndex = 1
        Me.ToolTips.SetToolTip(Me.txtIRIS_Server, "The name of the SQL server holding the IRIS database")
        '
        'txtIRIS_Root
        '
        Me.txtIRIS_Root.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtIRIS_Root.Font = New System.Drawing.Font("Calibri", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtIRIS_Root.Location = New System.Drawing.Point(277, 496)
        Me.txtIRIS_Root.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.txtIRIS_Root.Name = "txtIRIS_Root"
        Me.txtIRIS_Root.Size = New System.Drawing.Size(993, 31)
        Me.txtIRIS_Root.TabIndex = 3
        Me.ToolTips.SetToolTip(Me.txtIRIS_Root, "Root folder where new folders will be created and documents copied")
        '
        'btnCreateWorkspace
        '
        Me.btnCreateWorkspace.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnCreateWorkspace.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnCreateWorkspace.Location = New System.Drawing.Point(277, 552)
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
        Me.cmdPopulateFolderTable.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdPopulateFolderTable.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdPopulateFolderTable.Location = New System.Drawing.Point(549, 552)
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
        Me.cmdMigrateFiles.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdMigrateFiles.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdMigrateFiles.Location = New System.Drawing.Point(1095, 552)
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
        Me.cmdIdentifyDuplicates.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdIdentifyDuplicates.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdIdentifyDuplicates.Location = New System.Drawing.Point(822, 552)
        Me.cmdIdentifyDuplicates.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.cmdIdentifyDuplicates.Name = "cmdIdentifyDuplicates"
        Me.cmdIdentifyDuplicates.Size = New System.Drawing.Size(226, 68)
        Me.cmdIdentifyDuplicates.TabIndex = 60
        Me.cmdIdentifyDuplicates.Text = "3: Identify Duplicates"
        Me.ToolTips.SetToolTip(Me.cmdIdentifyDuplicates, "Uses the compiled mapping data to copy the files and log progress")
        Me.cmdIdentifyDuplicates.UseVisualStyleBackColor = True
        '
        'img
        '
        Me.img.ImageStream = CType(resources.GetObject("img.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.img.TransparentColor = System.Drawing.Color.Transparent
        Me.img.Images.SetKeyName(0, "Folder - Explorer.png")
        '
        'cmdBrowseIRIS
        '
        Me.cmdBrowseIRIS.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdBrowseIRIS.Font = New System.Drawing.Font("Tahoma", 8.25!)
        Me.cmdBrowseIRIS.Location = New System.Drawing.Point(1285, 496)
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
        Me.lblRootFolder.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblRootFolder.Font = New System.Drawing.Font("Calibri", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblRootFolder.ForeColor = System.Drawing.SystemColors.ControlDark
        Me.lblRootFolder.Location = New System.Drawing.Point(34, 490)
        Me.lblRootFolder.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblRootFolder.Name = "lblRootFolder"
        Me.lblRootFolder.Size = New System.Drawing.Size(238, 46)
        Me.lblRootFolder.TabIndex = 39
        Me.lblRootFolder.Text = "DOCUMENTS ROOT FOLDER"
        Me.lblRootFolder.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'status
        '
        Me.status.GripMargin = New System.Windows.Forms.Padding(0)
        Me.status.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.status.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.lblStatus})
        Me.status.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow
        Me.status.Location = New System.Drawing.Point(0, 642)
        Me.status.Name = "status"
        Me.status.Padding = New System.Windows.Forms.Padding(2, 0, 21, 0)
        Me.status.Size = New System.Drawing.Size(1345, 28)
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
        Me.lblIRIS_Database.Location = New System.Drawing.Point(34, 112)
        Me.lblIRIS_Database.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblIRIS_Database.Name = "lblIRIS_Database"
        Me.lblIRIS_Database.Size = New System.Drawing.Size(238, 46)
        Me.lblIRIS_Database.TabIndex = 57
        Me.lblIRIS_Database.Text = "IRIS DATABASE"
        Me.lblIRIS_Database.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblIRIS_Server
        '
        Me.lblIRIS_Server.Font = New System.Drawing.Font("Calibri", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblIRIS_Server.ForeColor = System.Drawing.SystemColors.ControlDark
        Me.lblIRIS_Server.Location = New System.Drawing.Point(30, 55)
        Me.lblIRIS_Server.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblIRIS_Server.Name = "lblIRIS_Server"
        Me.lblIRIS_Server.Size = New System.Drawing.Size(238, 45)
        Me.lblIRIS_Server.TabIndex = 54
        Me.lblIRIS_Server.Text = "SERVER"
        Me.lblIRIS_Server.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblHeader1
        '
        Me.lblHeader1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblHeader1.AutoSize = True
        Me.lblHeader1.Font = New System.Drawing.Font("Verdana", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblHeader1.ForeColor = System.Drawing.SystemColors.ActiveCaption
        Me.lblHeader1.Location = New System.Drawing.Point(20, 467)
        Me.lblHeader1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblHeader1.Name = "lblHeader1"
        Me.lblHeader1.Size = New System.Drawing.Size(199, 22)
        Me.lblHeader1.TabIndex = 50
        Me.lblHeader1.Text = "Destination (IRIS)"
        '
        'lblWorkspaceDatabase
        '
        Me.lblWorkspaceDatabase.Font = New System.Drawing.Font("Calibri", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblWorkspaceDatabase.ForeColor = System.Drawing.SystemColors.ControlDark
        Me.lblWorkspaceDatabase.Location = New System.Drawing.Point(34, 157)
        Me.lblWorkspaceDatabase.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblWorkspaceDatabase.Name = "lblWorkspaceDatabase"
        Me.lblWorkspaceDatabase.Size = New System.Drawing.Size(238, 46)
        Me.lblWorkspaceDatabase.TabIndex = 48
        Me.lblWorkspaceDatabase.Text = "MIGRATION DATABASE"
        Me.lblWorkspaceDatabase.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblMigrationID
        '
        Me.lblMigrationID.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblMigrationID.Font = New System.Drawing.Font("Calibri", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblMigrationID.ForeColor = System.Drawing.SystemColors.ControlDark
        Me.lblMigrationID.Location = New System.Drawing.Point(699, 54)
        Me.lblMigrationID.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblMigrationID.Name = "lblMigrationID"
        Me.lblMigrationID.Size = New System.Drawing.Size(238, 46)
        Me.lblMigrationID.TabIndex = 64
        Me.lblMigrationID.Text = "MIGRATION ID"
        Me.lblMigrationID.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'cmbMigrationID
        '
        Me.cmbMigrationID.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmbMigrationID.Font = New System.Drawing.Font("Calibri", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmbMigrationID.FormattingEnabled = True
        Me.cmbMigrationID.Location = New System.Drawing.Point(945, 61)
        Me.cmbMigrationID.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.cmbMigrationID.Name = "cmbMigrationID"
        Me.cmbMigrationID.Size = New System.Drawing.Size(376, 32)
        Me.cmbMigrationID.TabIndex = 65
        '
        'txtIRIS_Database
        '
        Me.txtIRIS_Database.Enabled = False
        Me.txtIRIS_Database.Font = New System.Drawing.Font("Calibri", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtIRIS_Database.Location = New System.Drawing.Point(276, 120)
        Me.txtIRIS_Database.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.txtIRIS_Database.Name = "txtIRIS_Database"
        Me.txtIRIS_Database.Size = New System.Drawing.Size(250, 31)
        Me.txtIRIS_Database.TabIndex = 66
        '
        'txtMigration_Database
        '
        Me.txtMigration_Database.Enabled = False
        Me.txtMigration_Database.Font = New System.Drawing.Font("Calibri", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtMigration_Database.Location = New System.Drawing.Point(276, 165)
        Me.txtMigration_Database.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.txtMigration_Database.Name = "txtMigration_Database"
        Me.txtMigration_Database.Size = New System.Drawing.Size(250, 31)
        Me.txtMigration_Database.TabIndex = 67
        '
        'btnAdd
        '
        Me.btnAdd.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnAdd.Location = New System.Drawing.Point(1271, 204)
        Me.btnAdd.Name = "btnAdd"
        Me.btnAdd.Size = New System.Drawing.Size(49, 30)
        Me.btnAdd.TabIndex = 70
        Me.btnAdd.Text = "Add"
        Me.btnAdd.UseVisualStyleBackColor = True
        '
        'Panel1
        '
        Me.Panel1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Panel1.BackColor = System.Drawing.SystemColors.Control
        Me.Panel1.Controls.Add(Me.lstSourceRootFolder)
        Me.Panel1.Location = New System.Drawing.Point(276, 241)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Padding = New System.Windows.Forms.Padding(1)
        Me.Panel1.Size = New System.Drawing.Size(1045, 207)
        Me.Panel1.TabIndex = 71
        '
        'lstSourceRootFolder
        '
        Me.lstSourceRootFolder.AllowUserToAddRows = False
        Me.lstSourceRootFolder.AllowUserToResizeColumns = False
        Me.lstSourceRootFolder.AllowUserToResizeRows = False
        Me.lstSourceRootFolder.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        Me.lstSourceRootFolder.BackgroundColor = System.Drawing.SystemColors.Window
        Me.lstSourceRootFolder.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.lstSourceRootFolder.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None
        Me.lstSourceRootFolder.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        Me.lstSourceRootFolder.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.lstSourceRootFolder.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstSourceRootFolder.EnableHeadersVisualStyles = False
        Me.lstSourceRootFolder.GridColor = System.Drawing.SystemColors.Control
        Me.lstSourceRootFolder.Location = New System.Drawing.Point(1, 1)
        Me.lstSourceRootFolder.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.lstSourceRootFolder.MultiSelect = False
        Me.lstSourceRootFolder.Name = "lstSourceRootFolder"
        Me.lstSourceRootFolder.RowHeadersVisible = False
        Me.lstSourceRootFolder.RowHeadersWidth = 62
        Me.lstSourceRootFolder.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.lstSourceRootFolder.Size = New System.Drawing.Size(1043, 205)
        Me.lstSourceRootFolder.TabIndex = 70
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Window
        Me.ClientSize = New System.Drawing.Size(1345, 670)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.btnAdd)
        Me.Controls.Add(Me.txtMigration_Database)
        Me.Controls.Add(Me.txtIRIS_Database)
        Me.Controls.Add(Me.cmbMigrationID)
        Me.Controls.Add(Me.lblMigrationID)
        Me.Controls.Add(Me.cmdIdentifyDuplicates)
        Me.Controls.Add(Me.status)
        Me.Controls.Add(Me.btnCreateWorkspace)
        Me.Controls.Add(Me.txtIRIS_Server)
        Me.Controls.Add(Me.cmdPopulateFolderTable)
        Me.Controls.Add(Me.cmdBrowseIRIS)
        Me.Controls.Add(Me.lblHeader3)
        Me.Controls.Add(Me.lblRootFolder)
        Me.Controls.Add(Me.txtIRIS_Root)
        Me.Controls.Add(Me.lblIRIS_Database)
        Me.Controls.Add(Me.lblIRIS_Server)
        Me.Controls.Add(Me.cmdMigrateFiles)
        Me.Controls.Add(Me.lblWorkspaceDatabase)
        Me.Controls.Add(Me.lblHeader1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.Name = "frmMain"
        Me.Text = "IRIS Consents Document Migration"
        Me.status.ResumeLayout(False)
        Me.status.PerformLayout()
        Me.Panel1.ResumeLayout(False)
        CType(Me.lstSourceRootFolder, System.ComponentModel.ISupportInitialize).EndInit()
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
    Friend WithEvents status As System.Windows.Forms.StatusStrip
    Friend WithEvents lblStatus As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents lblHeader1 As System.Windows.Forms.Label
    Friend WithEvents lblWorkspaceDatabase As System.Windows.Forms.Label
    Friend WithEvents cmdMigrateFiles As System.Windows.Forms.Button
    Friend WithEvents lblHeader3 As System.Windows.Forms.Label
    Friend WithEvents txtIRIS_Server As System.Windows.Forms.TextBox
    Friend WithEvents lblIRIS_Server As System.Windows.Forms.Label
    Friend WithEvents cmdPopulateFolderTable As System.Windows.Forms.Button
    Friend WithEvents lblIRIS_Database As System.Windows.Forms.Label
    Friend WithEvents btnCreateWorkspace As System.Windows.Forms.Button
    Friend WithEvents cmdIdentifyDuplicates As Button
    Friend WithEvents lblMigrationID As Label
    Friend WithEvents cmbMigrationID As ComboBox
    Friend WithEvents txtIRIS_Database As TextBox
    Friend WithEvents txtMigration_Database As TextBox
    Friend WithEvents btnAdd As Button
    Friend WithEvents Panel1 As Panel
    Friend WithEvents lstSourceRootFolder As DataGridView
End Class
