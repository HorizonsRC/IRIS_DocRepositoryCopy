Imports System
Imports System.Diagnostics.Eventing
Imports System.IO
Imports Microsoft.Data.SqlClient
Imports Microsoft.Identity.Client.Cache
Imports System.Configuration
Imports Microsoft.IdentityModel.Protocols

Module Program
    Public IRISRootFolder As String
    Public IncidentsRootFolder As String

    Sub Main(args As String())

        IRISRootFolder = ConfigurationManager.AppSettings("IRISRootFolder")
        IncidentsRootFolder = ConfigurationManager.AppSettings("IncidentsRootFolder")
        '-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        '   Folders in herman are grouped by district council under E\RM\14\08 in folders that are named prefixed with ERM05XX where XX runs from 01 to 07 (HDC,MDC,PNCC,TDC,RDC,WDC and RuDC)
        '   Within each district council folder is a subfolder for each SAHS site names prefixed with ERM05XXYY where XX is the council number above and YY is 2-character alphabetic like _A, _B... or AA, AB...
        '   In some cases the folder names are determined by a desktop.ini file and in others it is just a normal folder name so we have to cater for both possibilities.
        '-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        '   First get the mapping of SelectedLandUseSite IRIS objct business identifiers to the Herman file reference ID
        '-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        Dim Items As New List(Of Item)
        GetFolderNames(Items)
        '-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        '   Now loop through all the IRIS objects
        '-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        For Each Item In Items
            '-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            '   Get the list of top-level folders (one for each council) using the fie reference suffix, taking a second swing at it using the full file reference if no folders are returned
            '-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            Dim TopFolder As String() = IO.Directory.GetDirectories(IncidentsRootFolder, Item.FileReference.Substring(5, 2))
            If TopFolder.Length = 0 Then
                TopFolder = IO.Directory.GetDirectories(IncidentsRootFolder, Item.FileReference.Substring(0, 7) & "*")
            End If
            For t As Integer = 0 To TopFolder.Length - 1
                '---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                '   Get the list of all subfolders that match the IRIS object's file reference suffix, taking a second swing at it using the full file reference if no folders are returned
                '---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                Dim SearchKey As String = Item.FileReference.Substring(7)
                If SearchKey.Length = 1 Then SearchKey = "_" & SearchKey
                Dim SourceFolder As String() = IO.Directory.GetDirectories(TopFolder(t), SearchKey & "*", IO.SearchOption.AllDirectories)
                If SourceFolder.Length = 0 Then                                                                                             'Try the alternative folder naming convention
                    SearchKey = Item.FileReference.Substring(0, 7) & SearchKey
                    SourceFolder = IO.Directory.GetDirectories(TopFolder(t), SearchKey & "*", IO.SearchOption.AllDirectories)
                End If
                '---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                '   Iterate through all the files in the found folders and copy files for any not already existing in IRIS
                '---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                For i = 0 To SourceFolder.Length - 1
                    Dim TargetFolder As String = IO.Path.Combine(IRISRootFolder, "Selected Land Use Sites", Item.BusinessID)
                    If Not IO.Directory.Exists(TargetFolder) Then IO.Directory.CreateDirectory(TargetFolder)
                    Dim Files As String() = IO.Directory.GetFiles(SourceFolder(i))
                    For j = 0 To Files.Length - 1
                        If Not IO.File.GetAttributes(Files(j)) And FileAttributes.System Then
                            If Not IO.File.Exists(IO.Path.Combine(TargetFolder, IO.Path.GetFileName(Files(j)))) Then
                                IO.File.Copy(Files(j), IO.Path.Combine(TargetFolder, IO.Path.GetFileName(Files(j))), True)
                            End If
                        End If
                    Next
                Next

            Next
        Next
    End Sub

    Public Class Item
        Public Property IKRISObjectID As Integer
        Public Property BusinessID As String
        Public Property FileReference As String
    End Class

    Private Sub GetFolderNames(ByRef Items As List(Of Item))
        Using conn As New SqlConnection($"Data Source=iristestdb;Initial Catalog=IRISTest;Integrated Security=SSPI;TrustServerCertificate=True")
            conn.Open()
            Dim CmdText As String = "
            /***********************************************************************************************************************************************************************
            *  Script :         Identify sought folder names and associated IRIS business ID
            ***********************************************************************************************************************************************************************/
            SELECT      DISTINCT
                        I.IRISObjectID                              IRISObjectID,
                        O.BusinessID                                BusinessID,
                        OtherIdentifierText                         FileReference
            FROM        OtherIdentifier                             I
            LEFT JOIN   ReferenceDataValue                          R
            ON          R.ID = I.IdentifierContextID
            LEFT JOIN   IRISObject                                  O
            ON          O.ID = I.IRISObjectID
            WHERE       I.IsDeleted = 0
            AND         R.DisplayValue = 'File Reference'"

            Using cmd As New SqlCommand(CmdText, conn)
                Using rdr As SqlDataReader = cmd.ExecuteReader()
                    While rdr.Read

                        Dim item As New Item With {
                         .IKRISObjectID = rdr.GetInt64(0),
                         .BusinessID = rdr.GetString(1),
                         .FileReference = rdr.GetString(2)
                         }
                        Items.Add(item)
                    End While
                End Using
            End Using
        End Using
    End Sub
End Module
