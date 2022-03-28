Imports System.Reflection

Public Class frmBrowseNetwork

    Private Sub btnBrowse_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBrowse.Click
        '====== Create folder dialog box object
        Dim objFolderDialog As New FolderBrowserDialog()
        '===== Pass object as Parameter and get Selected network folder
        txtPath.Text = GetNetworkFolders(objFolderDialog)
    End Sub

    ''' <summary>
    ''' This function will get the Folderdialog as parameter and return the Selected
    ''' Folders UNC path. 
    ''' Ex: \\Server1\TestFolder
    ''' </summary>
    ''' <param name="oFolderBrowserDialog"></param>
    ''' <returns>it will return the Selected Folders UNC Path</returns>
    ''' <remarks></remarks>
    Public Shared Function GetNetworkFolders(ByVal oFolderBrowserDialog As FolderBrowserDialog) As String
        '======= Get type of Folder Dialog bog
        Dim type As Type = oFolderBrowserDialog.[GetType]
        '======== Get Fieldinfo for rootfolder.
        Dim fieldInfo As Reflection.FieldInfo = type.GetField("rootFolder", BindingFlags.NonPublic Or BindingFlags.Instance)
        '========= Now set the value for Folder Dialog using DirectCast
        '=== 18 = Network Neighborhood is the root
        fieldInfo.SetValue(oFolderBrowserDialog, DirectCast(18, Environment.SpecialFolder))
        '==== if user click on Ok, then it will return the selected folder.
        '===== otherwise return the blank string.
        If oFolderBrowserDialog.ShowDialog() = DialogResult.OK Then
            Return oFolderBrowserDialog.SelectedPath
        Else
            Return ""
        End If
    End Function

End Class
