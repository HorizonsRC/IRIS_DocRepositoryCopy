'Imports System.IO
'Imports System.Collections.Generic
'Imports System.Security.Permissions
'Imports System.Runtime.InteropServices
'Imports Microsoft.Win32.SafeHandles

'Public Class FastDirectoryEnumerator
'    Public Class FileData
'        Private Sub FileData(ByVal Dir As String, ByVal FindData As WIN32_FIND_DATA)

'        End Sub

'        Private Class WIN32_FIND_DATA
'            Public dxFileAttributes As FileAttributes
'            Public ftCreationTime_dwLowDateTime As UInt32
'            Public ftCreationTime_dwHighDateTime As UInt32
'            Public ftLastAccessTime_dwLowDateTime As UInt32
'            Public ftLastAccessTime_dwHighDateTime As UInt32
'            Public ftLastWriteTime_dwLowDateTime As UInt32
'            Public ftLastWriteTime_dwHighDateTime As UInt32
'            Public nFileSizeLow As UInt32
'            Public nFileSizeHigh As UInt32
'            Public dwReserved0 As UInt32
'            Public dwReserved1 As UInt32

'        End Class
'        Private _Attributes
'        Public ReadOnly Property Attributes As FileAttributes
'            Get
'                Return _Attributes
'            End Get
'        End Property

'        Public ReadOnly Property LastAccessTimeUTC
'            Get
'                Return
'            End Get
'        End Property

'    End Class
'    Implements IEnumerable(Of String)

'    Private myPath As String = "*"

'    Public Property EnumerateFiles(ByVal Path As String)

'        Get
'            Return myPath
'        End Get
'        Set(ByVal value)
'            myPath = value
'        End Set
'    End Property

'    Public Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of String) Implements System.Collections.Generic.IEnumerable(Of String).GetEnumerator

'    End Function
'End Class
