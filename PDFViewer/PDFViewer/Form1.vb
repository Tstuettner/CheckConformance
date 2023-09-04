<System.Runtime.InteropServices.ComVisible(False)> Public Class Form1

Private Declare Unicode Sub GetICMProfileW Lib "gdi32.dll" (ByVal hDC As IntPtr, ByRef Len As Integer, ByVal Filename As System.Text.StringBuilder)

Private m_Cache As IntPtr
Private m_Bookmarks As ArrayList
Private m_ErroLog As frmErrorLog
Private m_OpenFlags As DynaPDF.TInitCacheFlags
Private m_PDF As DynaPDF.CPDF

Private Class TBmk
   Public Index As Integer
   Public Node As TreeNode
   Public Sub New(ByVal Idx As Integer, ByVal Nde As TreeNode)
      MyBase.New()
      Index = Idx
      Node = Nde
   End Sub
End Class

Private Sub BtnExpandAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnExpandAll.Click
   TreeBookmarks.ExpandAll()
End Sub

Private Sub BtnCollapseAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnCollapseAll.Click
   TreeBookmarks.CollapseAll()
End Sub

Private Sub BtnErrors_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnErrors.Click
   m_ErroLog.ShowDialog()
   If m_ErroLog.Errors.Items.Count = 0 Then BtnErrors.Visible = False
End Sub

Private Sub BtnHideTabControl_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnHideTabControl.Click
   TabControl.Visible = False
   Splitter.Visible = False
End Sub

Private Sub BtnFitBest_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnFitBest.Click, fitBestToolStripMenuItem.Click
   PDFCanvas.PageScale = DynaPDF.TPDFPageScale.psFitBest
   ' Zooming can change the page layout to plSinglePage
   If PDFCanvas.PageLayout = DynaPDF.TPageLayout.plOneColumn Then
      BtnViewContinous.Checked = True
      BtnViewSinglePage.Checked = False
   End If
   If BtnFitBest.Checked Then
      BtnFitWidth.Checked = False
      BtnFitHeight.Checked = False
   Else
      BtnFitBest.Checked = True
   End If
End Sub

Private Sub BtnFitHeight_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnFitHeight.Click, fitHeightToolStripMenuItem.Click
   PDFCanvas.PageScale = DynaPDF.TPDFPageScale.psFitHeight
   If PDFCanvas.PageLayout = DynaPDF.TPageLayout.plOneColumn Then
      BtnViewContinous.Checked = True
      BtnViewSinglePage.Checked = False
   End If
   If BtnFitHeight.Checked Then
      BtnFitWidth.Checked = False
      BtnFitBest.Checked = False
   Else
      BtnFitHeight.Checked = True
   End If
End Sub

Private Sub BtnFitWidth_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnFitWidth.Click, fitWidthToolStripMenuItem.Click
   PDFCanvas.PageScale = DynaPDF.TPDFPageScale.psFitWidth
   If PDFCanvas.PageLayout = DynaPDF.TPageLayout.plOneColumn Then
      BtnViewContinous.Checked = True
      BtnViewSinglePage.Checked = False
   End If
   If BtnFitWidth.Checked Then
      BtnFitBest.Checked = False
      BtnFitHeight.Checked = False
   Else
      BtnFitWidth.Checked = True
   End If
End Sub

Private Sub BtnRotate90_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnRotate90.Click, rotate90ToolStripMenuItem.Click
   PDFCanvas.Rotate = PDFCanvas.Rotate + 90
End Sub

Private Sub BtnRotateM90_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnRotateM90.Click, rotate90ToolStripMenuItem1.Click
   PDFCanvas.Rotate = PDFCanvas.Rotate - 90
End Sub

Private Sub BtnRotate180_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnRotate180.Click, rotate180ToolStripMenuItem.Click
   PDFCanvas.Rotate = PDFCanvas.Rotate + 180
End Sub

Private Sub BtnViewContinous_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnViewContinous.Click, continousToolStripMenuItem.Click
   If PDFCanvas.PageCount > 1 Then
      PDFCanvas.PageLayout = DynaPDF.TPageLayout.plOneColumn
      ' This can happen when we are in zoom mode
      If PDFCanvas.PageLayout = DynaPDF.TPageLayout.plSinglePage Then
         BtnViewContinous.Checked = False
         BtnViewSinglePage.Checked = True
      Else
         BtnViewContinous.Checked = True
         BtnViewSinglePage.Checked = False
      End If
   Else
      BtnViewContinous.Checked = False
      BtnViewSinglePage.Checked = True
   End If
End Sub

Private Sub BtnViewSinglePage_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnViewSinglePage.Click, singlePageToolStripMenuItem.Click
   BtnViewContinous.Checked = False
   BtnViewSinglePage.Checked = True
   PDFCanvas.PageLayout = DynaPDF.TPageLayout.plSinglePage
End Sub

Private Sub BtnZoomIn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnZoomIn.Click, zoomInToolStripMenuItem.Click
   Dim zoom As Single
   ' Consider the resolutiuon of the output device
   zoom = DynaPDF.CPDF.rasGetCurrZoom(m_Cache) * 72.0F / PDFCanvas.Resolution * 1.25F
   If (zoom <= 0.25) Then
      zoom = 0.25F
   ElseIf (zoom <= 0.5F) Then
      zoom = 0.5F
   ElseIf (zoom <= 0.75F) Then
      zoom = 0.75F
   ElseIf (zoom <= 1.0F) Then
      zoom = 1.0F
   ElseIf (zoom <= 1.25) Then
      zoom = 1.25F
   ElseIf (zoom <= 1.5F) Then
      zoom = 1.5F
   ElseIf (zoom <= 2.0F) Then
      zoom = 2.0F
   ElseIf (zoom <= 3.0F) Then
      zoom = 3.0F
   ElseIf (zoom <= 4.0F) Then
      zoom = 4.0F
   ElseIf (zoom <= 8.0F) Then
      zoom = 8.0F
   ElseIf (zoom <= 16.0F) Then
      zoom = 16.0F
   ElseIf (zoom <= 24.0F) Then
      zoom = 24.0F
   ElseIf (zoom <= 32.0F) Then
      zoom = 32.0F
   Else
      zoom = 64.0F
   End If
   PDFCanvas.Zoom(zoom * PDFCanvas.Resolution / 72.0F)
   ' If the zoom factor is large then DynaPDF falls back into zoom mode and this mode works with the
   ' page layout plSinglePage. The previous mode will be restored when the zoom factor becomes small
   ' enough to render the pages as usual.
   If PDFCanvas.PageLayout = DynaPDF.TPageLayout.plSinglePage Then
      BtnViewContinous.Checked = False
      BtnViewSinglePage.Checked = True
   End If
   UnCheckZoomButtons()
End Sub

Private Sub BtnZoomOut_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnZoomOut.Click, zoomOutToolStripMenuItem.Click
   Dim zoom As Single
   ' Consider the resolution of the ouput device
   zoom = DynaPDF.CPDF.rasGetCurrZoom(m_Cache) * 72.0F / PDFCanvas.Resolution * 100.0F
   ' 10% tolerance to avoid rounding errors
   If (zoom > 3550.0F) Then
      zoom = 32.0F
   ElseIf (zoom > 2650.0F) Then
      zoom = 24.0F
   ElseIf (zoom > 1750.0F) Then
      zoom = 16.0F
   ElseIf (zoom > 880.0F) Then
      zoom = 8.0F
   ElseIf (zoom > 440.0F) Then
      zoom = 4.0F
   ElseIf (zoom > 330.0F) Then
      zoom = 3.0F
   ElseIf (zoom > 220.0F) Then
      zoom = 2.0F
   ElseIf (zoom > 165.0F) Then
      zoom = 1.5F
   ElseIf (zoom > 132.5F) Then
      zoom = 1.25F
   ElseIf (zoom > 110.0F) Then
      zoom = 1.0F
   ElseIf (zoom > 82.5F) Then
      zoom = 0.75F
   ElseIf (zoom > 55.0F) Then
      zoom = 0.5F
   ElseIf (zoom > 27.5F) Then
      zoom = 0.25F
   Else
      zoom = 0.1F
   End If

   ' The zoom factor that we have calculated is measured in 1/72 inch units.
   ' We must multiply the value with the resolution of the output device divided by 72 to get the value we need.
   PDFCanvas.Zoom(zoom * PDFCanvas.Resolution / 72.0F)
   ' Restore the page layout if necessary
   If PDFCanvas.PageLayout = DynaPDF.TPageLayout.plOneColumn Then
      BtnViewContinous.Checked = True
      BtnViewSinglePage.Checked = False
   End If
   UnCheckZoomButtons()
End Sub

Private Sub CboZoom_KeyUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles CboZoom.KeyUp
   If e.KeyCode = Keys.Return Then
      If CboZoom.Text.Length = 0 Then Return
      Try
         CboZoom.Text = CboZoom.Text.Trim(Chr(37))
         Dim zoom As Single = Convert.ToSingle(CboZoom.Text) / 100.0F
         If (zoom < 0.01F) Then
            zoom = 0.01F
         ElseIf (zoom > 64.0F) Then
            zoom = 64.0F
         End If
         PDFCanvas.Zoom(zoom * PDFCanvas.Resolution / 72.0F)
         ' If the zoom factor is large then DynaPDF falls back into zoom mode and this mode works with the
         ' page layout plSinglePage. The previous mode will be restored when the zoom factor becomes small
         ' enough to render the pages as usual.
         If PDFCanvas.PageLayout = DynaPDF.TPageLayout.plSinglePage Then
            BtnViewContinous.Checked = False
            BtnViewSinglePage.Checked = True
         End If
         UnCheckZoomButtons()
      Catch
      End Try
   End If
End Sub

Private Sub CboZoom_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CboZoom.SelectedIndexChanged
   ' The C# example contains exactly the same form, the same controls, anything is just the same but the C# example does NOT produce this
   ' event when the main form is loaded. So, why produces VB this event on start up?
   If PDFCanvas.PageCount = 0 Then Exit Sub
   Dim zoom As Single
   Select Case CboZoom.SelectedIndex
      Case 0 : zoom = 0.1F
      Case 1 : zoom = 0.25F
      Case 2 : zoom = 0.5F
      Case 3 : zoom = 0.75F
      Case 4 : zoom = 1.0F
      Case 5 : zoom = 1.25F
      Case 6 : zoom = 1.5F
      Case 7 : zoom = 2.0F
      Case 8 : zoom = 3.0F
      Case 9 : zoom = 4.0F
      Case 10 : zoom = 8.0F
      Case 11 : zoom = 16.0F
      Case 12 : zoom = 24.0F
      Case 13 : zoom = 32.0F
      Case 14 : zoom = 64.0F
      Case Else : Return
   End Select
   ' Consider the resolution of the ouput device
   PDFCanvas.Zoom(zoom * PDFCanvas.Resolution / 72.0F)
   Select Case PDFCanvas.PageLayout
      Case DynaPDF.TPageLayout.plSinglePage
         BtnViewContinous.Checked = False
         BtnViewSinglePage.Checked = True
      Case DynaPDF.TPageLayout.plOneColumn
         BtnViewContinous.Checked = True
         BtnViewSinglePage.Checked = True
   End Select
   UnCheckZoomButtons()
End Sub

Private Sub EnableControls(ByVal Enable As Boolean)
   BtnFitBest.Enabled = Enable
   BtnFitHeight.Enabled = Enable
   BtnFitWidth.Enabled = Enable
   BtnRotate90.Enabled = Enable
   BtnRotateM90.Enabled = Enable
   BtnRotate180.Enabled = Enable
   BtnViewContinous.Enabled = Enable
   BtnViewSinglePage.Enabled = Enable
   BtnZoomIn.Enabled = Enable
   BtnZoomOut.Enabled = Enable
   CboZoom.Enabled = Enable
   MnuView.Enabled = Enable
   TxtPageNum.Enabled = Enable
End Sub

Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
   Dim path As String
   m_Cache = PDFCanvas.GetCacheInstance()
   m_PDF = PDFCanvas.GetPDFInstance()
   m_OpenFlags = DynaPDF.TInitCacheFlags.icfIgnoreOpenAction Or DynaPDF.TInitCacheFlags.icfIgnorePageLayout
   If PDFCanvas.ColorManagement Then
      ' Because .Net applications require an installer before they can be executed outside of Visual Studio, we never know
      ' where the app will be installed, and hence, we cannot access the directory /examples/test_files/. However, it should
      ' at least be possible to activate color management in the demo app. To archive this, the default CMYK profile is embedded
      ' in the assembly. This is of course not what you should do in a real application because CMYK profiles are rather large
      ' and we need a lot of additional files, i.e. the CMap files. If you work with VB .Net then you should have a license of a
      ' professional installer so that you don't need to embed such files into the assembly.

      ' Very important: All profile paths must be absolute paths! This is required because the open file dialog changes the
      ' current directory and if DynaPDF must reload a profile later then it is maybe no longer possible since the current
      ' directory was changed.
      Dim file As System.IO.FileStream = Nothing
      path = System.IO.Path.GetFullPath("deficcprofile.icm")
      If Not System.IO.File.Exists(path) Then
         Try
            ' Extract the CMYK profile from the assembly and store it in the application folder.
            Dim a As System.Reflection.Assembly = System.Reflection.Assembly.GetExecutingAssembly()
            Dim strm As System.IO.Stream = a.GetManifestResourceStream("PDFViewer.ISOcoated_v2_bas.ICC")
            file = New System.IO.FileStream("deficcprofile.icm", System.IO.FileMode.CreateNew)

            Dim len As Integer = 4096
            Dim buffer As Byte()
            ReDim buffer(len)
            Dim bytesRead As Integer = strm.Read(buffer, 0, len)
            ' write the required bytes
            Do While (bytesRead > 0)
               file.Write(buffer, 0, bytesRead)
               bytesRead = strm.Read(buffer, 0, len)
            Loop
            strm.Close()
            file.Close()
         Catch ex As Exception
            file.Close()
            PDFCanvas.AddError(ex.Message)
         End Try
      End If
      ' Gray and RGB profiles are automatically created if not provided. The default RGB profile is sRGB.
      ' It is possible to set all profiles just to Nothing. In this case, CMYK colors will only be rendered with
      ' color management if the PDF file contains ICCBased color spaces or a CMYK Output Intent.
      ' If you want to disable color management (if it was enabled) then set the parameter Profiles to Nothing, e.g.
      ' DynaPDF.CPDF.rasInitColorManagement(m_Cache, Nothing, TPDFInitCMFlags.icmBPCompensation)
      Dim profiles As DynaPDF.TPDFColorProfiles = New DynaPDF.TPDFColorProfiles()
      profiles.StructSize = System.Runtime.InteropServices.Marshal.SizeOf(profiles)
      profiles.DefInCMYKW = path

      ' Get the monitor profile
      Dim size As Integer = 0
      ' This is the private dc of the PDF Canvas. It is not required to release this dc.
      Dim dc As IntPtr = PDFCanvas.GetDC()
      Dim iccFile As System.Text.StringBuilder = New System.Text.StringBuilder()
      GetICMProfileW(dc, size, Nothing)
      If size > 0 Then
         iccFile.Length = size * 2 + 2 ' 2 bytes per character plus 2 bytes for the null-terminator.
         GetICMProfileW(dc, size, iccFile)
         profiles.DeviceProfileW = iccFile.ToString()
      End If

      If DynaPDF.CPDF.rasInitColorManagement(m_Cache, profiles, DynaPDF.TPDFInitCMFlags.icmBPCompensation) = 0 Then
         PDFCanvas.AddError("Failed to initialize color management!")
      End If
   End If
   ' Skip anything that is not required to display a page.
   ' Independent of the used flags this instance is not used to load pages! The page cache uses
   ' this instance only to fetch the page's bounding boxes and orientation.
   m_PDF.SetImportFlags(DynaPDF.TImportFlags.ifContentOnly Or DynaPDF.TImportFlags.ifImportAsPage Or DynaPDF.TImportFlags.ifAllAnnots Or DynaPDF.TImportFlags.ifFormFields)
   m_PDF.SetImportFlags2(DynaPDF.TImportFlags2.if2UseProxy Or DynaPDF.TImportFlags2.if2NoResNameCheck)
   ' Load external CMaps delayed. Note that the cache contains its own functions to load the CMaps. Loading the
   ' CMaps into the main PDF instance is useless because this instance is not used to render pages...

   ' In a real application make sure that you copy the files into a directory that DynaPDF can access at runtime.
      path = System.IO.Path.GetFullPath("../../../../../../../Resource/CMap/")
   DynaPDF.CPDF.rasSetCMapDir(m_Cache, path, DynaPDF.TLoadCMapFlags.lcmRecursive Or DynaPDF.TLoadCMapFlags.lcmDelayed)
End Sub

Private Sub MnuCloseFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MnuCloseFile.Click
   BtnErrors.Visible = False
   PDFCanvas.CloseFile()
   TreeBookmarks.Nodes.Clear()
   m_Bookmarks = Nothing
   EnableControls(False)
   Splitter.Visible = False
   TabControl.Visible = False
   TxtPageNum.Text = "0"
   LblPageCount.Text = "of 0"
End Sub

Private Sub MnuExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MnuExit.Click
   Application.Exit()
End Sub

Private Sub MnuOpenFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MnuOpenFile.Click
   If OpenDialog.ShowDialog() = DialogResult.OK Then
      OpenPDFFile(OpenDialog.FileName)
   End If
End Sub

Private Sub MnuRotate0_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MnuRotate0.Click
   PDFCanvas.Rotate = 0
End Sub

Private Sub MnuViewBookmarks_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MnuViewBookmarks.Click
   ShowBookmarks()
   Splitter.Visible = True
   TabControl.Visible = True
   TabControl.TabIndex = 0
End Sub

Private Sub MnuViewDisablePageLayout_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MnuViewDisablePageLayout.Click
   If MnuViewDisablePageLayout.Checked Then
      m_OpenFlags = m_OpenFlags Or DynaPDF.TInitCacheFlags.icfIgnorePageLayout
   Else
      m_OpenFlags = m_OpenFlags And Not DynaPDF.TInitCacheFlags.icfIgnorePageLayout
   End If
End Sub

Private Sub MnuViewIngoreOpenAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MnuViewIngoreOpenAction.Click
   If MnuViewIngoreOpenAction.Checked Then
      m_OpenFlags = m_OpenFlags Or DynaPDF.TInitCacheFlags.icfIgnoreOpenAction
   Else
      m_OpenFlags = m_OpenFlags And Not DynaPDF.TInitCacheFlags.icfIgnoreOpenAction
   End If
End Sub

Private Sub OpenPDFFile(ByVal FileName As String)
   Dim retval As Integer
   ' Clean up if necessary
   MnuCloseFile_Click(Nothing, Nothing)
   m_PDF.CreateNewPDF(Nothing)
   retval = m_PDF.OpenImportFileW(FileName, DynaPDF.TPwdType.ptOpen, Nothing)
   If retval < 0 Then
      Do While m_PDF.IsWrongPwd(retval) = True
         m_PDF.ClearErrorLog() ' Remove the "Wrong password error message"
         Dim pwdDlg As frmPassword = New frmPassword()
         If pwdDlg.ShowDialog() = DialogResult.OK Then
            retval = m_PDF.OpenImportFileW(FileName, DynaPDF.TPwdType.ptOpen, pwdDlg.Password)
            If m_PDF.IsWrongPwd(retval) Then
               m_PDF.ClearErrorLog() ' Remove the "Wrong password error message"
               retval = m_PDF.OpenImportFileW(FileName, DynaPDF.TPwdType.ptOwner, pwdDlg.Password)
            End If
         Else
            Exit Do
         End If
      Loop
      If retval < 0 Then
         MnuCloseFile_Click(Nothing, Nothing)
         ' At this point the page cache doesn't notice when an error occurs.
         ' The error messages are still available after the file was already closed.
         If (m_PDF.GetErrLogMessageCount() > 0) Then
            Dim err As DynaPDF.TPDFError = New DynaPDF.TPDFError()
            If m_PDF.GetErrLogMessage(0, err) Then
               PDFCanvas.AddError(err.Message)
               BtnErrors.Visible = True
            End If
         End If
         Return
      End If
   End If
   ' Display the outline panel if necessary before initializing the PDFCanvas.
   ' This should be done here because displaying the outline panel causes a resize event.
   If m_PDF.GetPageMode() = DynaPDF.TPageMode.pmUseOutlines Then
      Splitter.Visible = True
      TabControl.Visible = True
      TabControl.TabIndex = 0
   End If
   If Not PDFCanvas.InitBaseObjects(m_OpenFlags) Then
      MnuCloseFile_Click(Nothing, Nothing)
      Return
   End If
   If m_PDF.GetInRepairMode() Then
      PDFCanvas.AddError("Opened damaged PDF file in repair mode!")
   End If
   ' Process the errors of the top level objects if any
   PDFCanvas.ProcessErrors(False)
   TxtPageNum.Text = String.Format("{0}", PDFCanvas.FirstPage)
   LblPageCount.Text = String.Format(" of {0} ", PDFCanvas.PageCount)
   ActiveForm.Text = FileName
   EnableControls(True)
   If PDFCanvas.PageLayout = DynaPDF.TPageLayout.plSinglePage Then
      BtnViewContinous.Checked = False
      BtnViewSinglePage.Checked = True
   Else
      BtnViewContinous.Checked = True
      BtnViewSinglePage.Checked = False
   End If
   PDFCanvas.DisplayFirstPage()
   ' Show the outline tree if necessary
   If m_PDF.GetPageMode() = DynaPDF.TPageMode.pmUseOutlines Then
      ShowBookmarks()
   End If
End Sub

Private Sub PDFCanvas_Error(ByVal sender As System.Object, ByVal e As PDFControl.ErrorArgs) Handles PDFCanvas.Error
   Dim i As Integer
   If m_ErroLog Is Nothing Then
      m_ErroLog = New frmErrorLog()
   Else
      m_ErroLog.Errors.Items.Clear()
   End If
   For i = 0 To e.Errors.Count - 1
      m_ErroLog.Errors.Items.Add(e.Errors(i))
   Next i
   BtnErrors.Visible = True
End Sub

Private Sub PDFCanvas_NewPage(ByVal sender As System.Object, ByVal e As PDFControl.NewPageArgs) Handles PDFCanvas.NewPage
   TxtPageNum.Text = e.PageNum.ToString()
   CboZoom.Text = String.Format("{0:f1}%", (DynaPDF.CPDF.rasGetCurrZoom(m_Cache) * 72.0F / PDFCanvas.Resolution) * 100.0) ' Consider the resolution of the ouput device
End Sub

Private Function RemoveControls(ByVal Value As String) As String
   Dim i As Integer
   Dim retval As Char() = Value.ToCharArray()
   For i = 0 To Value.Length - 1
      If Value(i) < Chr(32) Then
         retval(i) = Chr(32)
      End If
   Next i
   Return New String(retval)
End Function

Private Sub ShowBookmarks()
   If TreeBookmarks.Nodes.Count > 0 Then Return
   Me.Cursor = Cursors.WaitCursor

   Dim count As Integer = m_PDF.ImportBookmarks()
   ' Add a dummy node so that we don't import the bookmarks multiple times
   If (count <= 0) Then
      TreeBookmarks.Nodes.Add(" ")
   Else
      Try
         Dim bm As TBmk
         Dim i As Integer
         Dim style As FontStyle
         Dim node, parent As TreeNode
         Dim bmk As DynaPDF.TBookmark = Nothing
         m_Bookmarks = New ArrayList(count)
         Splitter.Visible = True
         TabControl.Visible = True
         TabControl.TabIndex = 0
         ' The treeview is relatively fast. It is of course possible to optimize the code so that only the visible nodes
         ' will be loaded.
         For i = 0 To count - 1
            If m_PDF.GetBookmark(i, bmk) Then
               If bmk.Parent > -1 Then
                  ' We must use a mapping table to map bookmark indexes to the TreeView nodes and vice versa. The check
                  ' whether the parent index is valid is just for safety. You can be sure that the children of a bookmark
                  ' occur after the parent bookmark.
                  If (bmk.Parent < m_Bookmarks.Count) Then
                     bm = m_Bookmarks(bmk.Parent)
                     parent = bm.Node
                     ' Bookmark titles contain often control characters especially if the file was created on Mac OS X.
                     ' Acrobat doesn't display characters below 32, so, we do the same...
                     node = parent.Nodes.Add(RemoveControls(bmk.Title))
                     style = FontStyle.Regular
                     If (bmk.Style And DynaPDF.TBmkStyle.bmsBold) = DynaPDF.TBmkStyle.bmsBold Then
                        style = style Or FontStyle.Bold
                     End If
                     If (bmk.Style And DynaPDF.TBmkStyle.bmsItalic) = DynaPDF.TBmkStyle.bmsItalic Then
                        style = style Or FontStyle.Italic
                     End If
                     If style <> FontStyle.Regular Then
                        node.NodeFont = New Font(TreeBookmarks.Font, style)
                     End If
                     node.ForeColor = Color.FromArgb(bmk.Color)
                     ' Check whether the node must be expandet but expand it only one time!
                     If bmk.Open And parent.Nodes.Count = 1 Then
                        parent.Expand()
                     End If
                     bm = New TBmk(i, node)
                     node.Tag = bm
                     m_Bookmarks.Add(bm)
                  End If
               Else
                  node = TreeBookmarks.Nodes.Add(RemoveControls(bmk.Title))
                  style = FontStyle.Regular
                  If (bmk.Style And DynaPDF.TBmkStyle.bmsBold) = DynaPDF.TBmkStyle.bmsBold Then
                     style = style Or FontStyle.Bold
                  End If
                  If ((bmk.Style And DynaPDF.TBmkStyle.bmsItalic) = DynaPDF.TBmkStyle.bmsItalic) Then
                     style = style Or FontStyle.Italic
                  End If
                  If style <> FontStyle.Regular Then
                     node.NodeFont = New Font(TreeBookmarks.Font, style)
                  End If
                  node.ForeColor = Color.FromArgb(bmk.Color)
                  bm = New TBmk(i, node)
                  node.Tag = bm
                  m_Bookmarks.Add(bm)
               End If
            End If
         Next i
      Catch e As Exception
         MessageBox.Show(e.Message)
      End Try
   End If
   Me.Cursor = Cursors.Default
   ' Check for errors. It is not allowed to access the error log directly with pdfGetErrLogMessage()
   ' because this can cause collusions with the rendering thread!
   PDFCanvas.ProcessErrors(True)
End Sub

Private Sub TreeBookmarks_AfterSelect(ByVal sender As System.Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles TreeBookmarks.AfterSelect
   Dim bmk As TBmk = e.Node.Tag
   If Not bmk Is Nothing Then
      Dim retval As DynaPDF.TUpdBmkAction = PDFCanvas.ExecBookmark(bmk.Index)
      If (retval And DynaPDF.TUpdBmkAction.ubaZoom) = DynaPDF.TUpdBmkAction.ubaZoom Then
         UnCheckZoomButtons()
      ElseIf (retval And DynaPDF.TUpdBmkAction.ubaPageScale) = DynaPDF.TUpdBmkAction.ubaPageScale Then
         Select Case PDFCanvas.PageScale
            Case DynaPDF.TPDFPageScale.psFitWidth
               BtnFitWidth.Checked = True
               BtnFitHeight.Checked = False
               BtnFitBest.Checked = False
            Case DynaPDF.TPDFPageScale.psFitHeight
               BtnFitHeight.Checked = True
               BtnFitWidth.Checked = False
               BtnFitBest.Checked = False
            Case DynaPDF.TPDFPageScale.psFitBest
               BtnFitBest.Checked = True
               BtnFitWidth.Checked = False
               BtnFitHeight.Checked = False
         End Select
      End If
   End If
End Sub

Private Sub TxtPageNum_KeyUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles TxtPageNum.KeyUp
   If e.KeyCode = Keys.Return Then
      If TxtPageNum.Text.Length = 0 Then Return
      Try
         Dim pageNum As Integer = Convert.ToInt32(TxtPageNum.Text)
         If pageNum > PDFCanvas.PageCount Then
            pageNum = PDFCanvas.PageCount
         ElseIf pageNum < 1 Then
            pageNum = 1
         End If
         PDFCanvas.ScrollTo(pageNum)
      Catch
      End Try
   End If
End Sub

Private Sub UnCheckZoomButtons()
   BtnFitWidth.Checked = False
   BtnFitBest.Checked = False
   BtnFitHeight.Checked = False
End Sub

End Class
