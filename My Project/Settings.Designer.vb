﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.42000
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On


Namespace My
    
    <Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute(),  _
     Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.5.0.0"),  _
     Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
    Partial Friend NotInheritable Class MySettings
        Inherits Global.System.Configuration.ApplicationSettingsBase
        
        Private Shared defaultInstance As MySettings = CType(Global.System.Configuration.ApplicationSettingsBase.Synchronized(New MySettings()),MySettings)
        
#Region "My.Settings Auto-Save Functionality"
#If _MyType = "WindowsForms" Then
    Private Shared addedHandler As Boolean

    Private Shared addedHandlerLockObject As New Object

    <Global.System.Diagnostics.DebuggerNonUserCodeAttribute(), Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)> _
    Private Shared Sub AutoSaveSettings(sender As Global.System.Object, e As Global.System.EventArgs)
        If My.Application.SaveMySettingsOnExit Then
            My.Settings.Save()
        End If
    End Sub
#End If
#End Region
        
        Public Shared ReadOnly Property [Default]() As MySettings
            Get
                
#If _MyType = "WindowsForms" Then
               If Not addedHandler Then
                    SyncLock addedHandlerLockObject
                        If Not addedHandler Then
                            AddHandler My.Application.Shutdown, AddressOf AutoSaveSettings
                            addedHandler = True
                        End If
                    End SyncLock
                End If
#End If
                Return defaultInstance
            End Get
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("h:\serieTV")>  _
        Public Property TARGET_DIR() As String
            Get
                Return CType(Me("TARGET_DIR"),String)
            End Get
            Set
                Me("TARGET_DIR") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("G:\Downloads\Torrent\Completed\")>  _
        Public Property SOURCE_DIR() As String
            Get
                Return CType(Me("SOURCE_DIR"),String)
            End Get
            Set
                Me("SOURCE_DIR") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("C:\Users\matteo\Downloads\")>  _
        Public Property SUBS_DOWNLOAD_DIR() As String
            Get
                Return CType(Me("SUBS_DOWNLOAD_DIR"),String)
            End Get
            Set
                Me("SUBS_DOWNLOAD_DIR") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("C:\Users\matteo\Downloads\SUBS\")>  _
        Public Property SUBS_WORKING_DIR() As String
            Get
                Return CType(Me("SUBS_WORKING_DIR"),String)
            End Get
            Set
                Me("SUBS_WORKING_DIR") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("seriesmonitor.shtml")>  _
        Public Property REPORT_FILE_NAME() As String
            Get
                Return CType(Me("REPORT_FILE_NAME"),String)
            End Get
            Set
                Me("REPORT_FILE_NAME") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("30")>  _
        Public Property REPORT_FILE_NUM_OF_DAYS() As String
            Get
                Return CType(Me("REPORT_FILE_NUM_OF_DAYS"),String)
            End Get
            Set
                Me("REPORT_FILE_NUM_OF_DAYS") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("filemanager_subs.log")>  _
        Public Property LOG_FILEMANAGER_SUB_FILENAME() As String
            Get
                Return CType(Me("LOG_FILEMANAGER_SUB_FILENAME"),String)
            End Get
            Set
                Me("LOG_FILEMANAGER_SUB_FILENAME") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("filemanager_main.log")>  _
        Public Property LOG_FILEMANAGER_MAIN_FILENAME() As String
            Get
                Return CType(Me("LOG_FILEMANAGER_MAIN_FILENAME"),String)
            End Get
            Set
                Me("LOG_FILEMANAGER_MAIN_FILENAME") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("monitor_main.log")>  _
        Public Property LOG_MONITOR_FILENAME() As String
            Get
                Return CType(Me("LOG_MONITOR_FILENAME"),String)
            End Get
            Set
                Me("LOG_MONITOR_FILENAME") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("monitor_rss.log")>  _
        Public Property LOG_RSS_FILENAME() As String
            Get
                Return CType(Me("LOG_RSS_FILENAME"),String)
            End Get
            Set
                Me("LOG_RSS_FILENAME") = value
            End Set
        End Property
    End Class
End Namespace

Namespace My
    
    <Global.Microsoft.VisualBasic.HideModuleNameAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute()>  _
    Friend Module MySettingsProperty
        
        <Global.System.ComponentModel.Design.HelpKeywordAttribute("My.Settings")>  _
        Friend ReadOnly Property Settings() As Global.SeriesManager.My.MySettings
            Get
                Return Global.SeriesManager.My.MySettings.Default
            End Get
        End Property
    End Module
End Namespace
