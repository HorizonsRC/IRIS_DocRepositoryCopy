Imports System.Windows.Forms

Module modConfigAndLog
    Public Const ConfigVersion As String = "1.0.0"

    Public ConfigFilename As String

    Public Sub ReadConfig()
        Dim myDOM As New System.Xml.XmlDocument
        Dim myRoot As Xml.XmlElement
        Dim myNode As Xml.XmlNode
        Dim myVersion As String

        ConfigFilename = IO.Path.GetDirectoryName(Application.ExecutablePath.ToString) & "\" & IO.Path.GetFileNameWithoutExtension(Application.ExecutablePath) & ".config.xml"
        If IO.File.Exists(ConfigFilename) Then
            myDOM.Load(ConfigFilename)

            myNode = myDOM.SelectSingleNode("//Source")
            myVersion = "" & GetAttributeValue(myNode, "ConfigVersion")

            If myVersion <> ConfigVersion Then
                MsgBox("This XML file is version " & myVersion & ". The current version is " & ConfigVersion & ". Please ensure your file version is compatible.", vbInformation + vbOKOnly, "Version Mismatch")
            Else
                myNode = myDOM.SelectSingleNode("//FilePaths")
                SaveSetting("FileClassFolderBuilder", "Settings", "FileClassFile", GetAttributeValue(myNode, "FileClassFile", GetSetting("FileClassFolderBuilder", "Settings", "FileClassFile", "")))
                SaveSetting("FileClassFolderBuilder", "Settings", "FileClassRootFolder", GetAttributeValue(myNode, "FileClassRootFolder", GetSetting("FileClassFolderBuilder", "Settings", "FileClassRootFolder", "")))
                SaveSetting("FileClassFolderBuilder", "Settings", "ShortcutHostFolder", GetAttributeValue(myNode, "ShortcutHostFolder", GetSetting("FileClassFolderBuilder", "Settings", "ShortcutHostFolder", "")))
                myNode = Nothing
                myRoot = Nothing
                myDOM = Nothing
            End If
        Else
            SaveConfig()
        End If
    End Sub

    Public Sub SaveConfig()
        Dim myDOM As New System.Xml.XmlDocument
        Dim myRoot As Xml.XmlElement
        Dim myElement As Xml.XmlElement
        Dim myNode As Xml.XmlNode

        ConfigFilename = IO.Path.GetDirectoryName(Application.ExecutablePath.ToString) & "\" & IO.Path.GetFileNameWithoutExtension(Application.ExecutablePath) & ".config.xml"
        myNode = myDOM.CreateProcessingInstruction("xml", "version='1.0'")
        myDOM.AppendChild(myNode)

        myRoot = myDOM.CreateElement("Source")
        AddAttribute(myDOM, myRoot, "Name", IO.Path.GetFileNameWithoutExtension(Application.ExecutablePath))
        AddAttribute(myDOM, myRoot, "Version", Application.ProductVersion.ToString)
        AddAttribute(myDOM, myRoot, "ConfigVersion", ConfigVersion)

        myElement = myDOM.CreateElement("FilePaths")
        AddAttribute(myDOM, myElement, "FileClassFile", GetSetting("FileClassFolderBuilder", "Settings", "FileClassFile", ""))
        AddAttribute(myDOM, myElement, "FileClassRootFolder", GetSetting("FileClassFolderBuilder", "Settings", "FileClassRootFolder", ""))
        AddAttribute(myDOM, myElement, "ShortcutHostFolder", GetSetting("FileClassFolderBuilder", "Settings", "ShortcutHostFolder", ""))
        myRoot.AppendChild(myElement)

        myDOM.AppendChild(myRoot)
        myDOM.Save(ConfigFilename)
        myNode = Nothing
        myRoot = Nothing
        myDOM = Nothing
    End Sub

    Private Sub AddAttribute(ByVal Doc As Xml.XmlDocument, ByVal Element As Xml.XmlElement, ByVal AttributeName As String, ByVal AttributeValue As String)
        Dim myAttr As Xml.XmlAttribute

        myAttr = Doc.CreateAttribute(AttributeName)
        myAttr.Value = CStr("" & AttributeValue)
        Element.SetAttributeNode(myAttr)
    End Sub

    Private Function GetAttributeValue(ByVal Node As Xml.XmlNode, ByVal AttributeName As String, Optional ByVal mDefault As String = "") As String
        Dim myNode As Xml.XmlNode

        myNode = Node.SelectSingleNode("@" & AttributeName)
        If Not myNode Is Nothing Then
            GetAttributeValue = myNode.Value.ToString
        Else
            GetAttributeValue = mDefault
        End If
    End Function
End Module
