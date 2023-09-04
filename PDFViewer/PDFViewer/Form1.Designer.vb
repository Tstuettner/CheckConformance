<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
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

    'Wird vom Windows Form-Designer benötigt.
    Private components As System.ComponentModel.IContainer

    'Hinweis: Die folgende Prozedur ist für den Windows Form-Designer erforderlich.
    'Das Bearbeiten ist mit dem Windows Form-Designer möglich.  
    'Das Bearbeiten mit dem Code-Editor ist nicht möglich.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
Me.MainMenu = New System.Windows.Forms.MenuStrip
Me.MnuFile = New System.Windows.Forms.ToolStripMenuItem
Me.MnuOpenFile = New System.Windows.Forms.ToolStripMenuItem
Me.MnuCloseFile = New System.Windows.Forms.ToolStripMenuItem
Me.MnuExit = New System.Windows.Forms.ToolStripMenuItem
Me.MnuView = New System.Windows.Forms.ToolStripMenuItem
Me.MnuViewPageDisplay = New System.Windows.Forms.ToolStripMenuItem
Me.continousToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
Me.singlePageToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
Me.MnuViewRotate = New System.Windows.Forms.ToolStripMenuItem
Me.MnuRotate0 = New System.Windows.Forms.ToolStripMenuItem
Me.rotate90ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
Me.rotate90ToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem
Me.rotate180ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
Me.MnuViewZoom = New System.Windows.Forms.ToolStripMenuItem
Me.fitWidthToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
Me.fitBestToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
Me.fitHeightToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
Me.toolStripMenuItem3 = New System.Windows.Forms.ToolStripSeparator
Me.zoomInToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
Me.zoomOutToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
Me.toolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator
Me.MnuViewBookmarks = New System.Windows.Forms.ToolStripMenuItem
Me.toolStripMenuItem2 = New System.Windows.Forms.ToolStripSeparator
Me.MnuViewDisablePageLayout = New System.Windows.Forms.ToolStripMenuItem
Me.MnuViewIngoreOpenAction = New System.Windows.Forms.ToolStripMenuItem
Me.ToolBar = New System.Windows.Forms.ToolStrip
Me.BtnFitWidth = New System.Windows.Forms.ToolStripButton
Me.BtnFitBest = New System.Windows.Forms.ToolStripButton
Me.BtnFitHeight = New System.Windows.Forms.ToolStripButton
Me.toolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator
Me.BtnViewContinous = New System.Windows.Forms.ToolStripButton
Me.BtnViewSinglePage = New System.Windows.Forms.ToolStripButton
Me.toolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator
Me.BtnZoomOut = New System.Windows.Forms.ToolStripButton
Me.BtnZoomIn = New System.Windows.Forms.ToolStripButton
Me.toolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator
Me.BtnRotate90 = New System.Windows.Forms.ToolStripButton
Me.BtnRotateM90 = New System.Windows.Forms.ToolStripButton
Me.BtnRotate180 = New System.Windows.Forms.ToolStripButton
Me.toolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator
Me.CboZoom = New System.Windows.Forms.ToolStripComboBox
Me.toolStripLabel1 = New System.Windows.Forms.ToolStripLabel
Me.TxtPageNum = New System.Windows.Forms.ToolStripTextBox
Me.LblPageCount = New System.Windows.Forms.ToolStripLabel
Me.BtnErrors = New System.Windows.Forms.ToolStripButton
Me.TabControl = New System.Windows.Forms.TabControl
Me.TabBookmarks = New System.Windows.Forms.TabPage
Me.TreeBookmarks = New System.Windows.Forms.TreeView
Me.ToolStrip2 = New System.Windows.Forms.ToolStrip
Me.BtnExpandAll = New System.Windows.Forms.ToolStripButton
Me.BtnCollapseAll = New System.Windows.Forms.ToolStripButton
Me.BtnHideTabControl = New System.Windows.Forms.ToolStripButton
Me.TabLayer = New System.Windows.Forms.TabPage
Me.Splitter = New System.Windows.Forms.Splitter
Me.OpenDialog = New System.Windows.Forms.OpenFileDialog
Me.PDFCanvas = New PDFControl.PDFCanvas
Me.MainMenu.SuspendLayout()
Me.ToolBar.SuspendLayout()
Me.TabControl.SuspendLayout()
Me.TabBookmarks.SuspendLayout()
Me.ToolStrip2.SuspendLayout()
Me.SuspendLayout()
'
'MainMenu
'
Me.MainMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MnuFile, Me.MnuView})
Me.MainMenu.Location = New System.Drawing.Point(0, 0)
Me.MainMenu.Name = "MainMenu"
Me.MainMenu.Padding = New System.Windows.Forms.Padding(8, 2, 0, 2)
Me.MainMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional
Me.MainMenu.Size = New System.Drawing.Size(1365, 26)
Me.MainMenu.TabIndex = 5
Me.MainMenu.Text = "menuStrip1"
'
'MnuFile
'
Me.MnuFile.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MnuOpenFile, Me.MnuCloseFile, Me.MnuExit})
Me.MnuFile.Name = "MnuFile"
Me.MnuFile.Size = New System.Drawing.Size(40, 22)
Me.MnuFile.Text = "File"
'
'MnuOpenFile
'
Me.MnuOpenFile.Image = CType(resources.GetObject("MnuOpenFile.Image"), System.Drawing.Image)
Me.MnuOpenFile.ImageTransparentColor = System.Drawing.Color.Fuchsia
Me.MnuOpenFile.Name = "MnuOpenFile"
Me.MnuOpenFile.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.O), System.Windows.Forms.Keys)
Me.MnuOpenFile.Size = New System.Drawing.Size(187, 22)
Me.MnuOpenFile.Text = "Open"
'
'MnuCloseFile
'
Me.MnuCloseFile.Name = "MnuCloseFile"
Me.MnuCloseFile.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.E), System.Windows.Forms.Keys)
Me.MnuCloseFile.Size = New System.Drawing.Size(187, 22)
Me.MnuCloseFile.Text = "Close"
'
'MnuExit
'
Me.MnuExit.Name = "MnuExit"
Me.MnuExit.ShortcutKeys = CType(((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Alt) _
            Or System.Windows.Forms.Keys.F4), System.Windows.Forms.Keys)
Me.MnuExit.Size = New System.Drawing.Size(187, 22)
Me.MnuExit.Text = "Exit"
'
'MnuView
'
Me.MnuView.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MnuViewPageDisplay, Me.MnuViewRotate, Me.MnuViewZoom, Me.toolStripMenuItem1, Me.MnuViewBookmarks, Me.toolStripMenuItem2, Me.MnuViewDisablePageLayout, Me.MnuViewIngoreOpenAction})
Me.MnuView.Enabled = False
Me.MnuView.Name = "MnuView"
Me.MnuView.Size = New System.Drawing.Size(49, 22)
Me.MnuView.Text = "View"
'
'MnuViewPageDisplay
'
Me.MnuViewPageDisplay.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.continousToolStripMenuItem, Me.singlePageToolStripMenuItem})
Me.MnuViewPageDisplay.Name = "MnuViewPageDisplay"
Me.MnuViewPageDisplay.Size = New System.Drawing.Size(288, 22)
Me.MnuViewPageDisplay.Text = "Page Display"
'
'continousToolStripMenuItem
'
Me.continousToolStripMenuItem.Image = CType(resources.GetObject("continousToolStripMenuItem.Image"), System.Drawing.Image)
Me.continousToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Silver
Me.continousToolStripMenuItem.Name = "continousToolStripMenuItem"
Me.continousToolStripMenuItem.Size = New System.Drawing.Size(149, 22)
Me.continousToolStripMenuItem.Text = "Continous"
'
'singlePageToolStripMenuItem
'
Me.singlePageToolStripMenuItem.Image = CType(resources.GetObject("singlePageToolStripMenuItem.Image"), System.Drawing.Image)
Me.singlePageToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Silver
Me.singlePageToolStripMenuItem.Name = "singlePageToolStripMenuItem"
Me.singlePageToolStripMenuItem.Size = New System.Drawing.Size(149, 22)
Me.singlePageToolStripMenuItem.Text = "Single Page"
'
'MnuViewRotate
'
Me.MnuViewRotate.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MnuRotate0, Me.rotate90ToolStripMenuItem, Me.rotate90ToolStripMenuItem1, Me.rotate180ToolStripMenuItem})
Me.MnuViewRotate.Name = "MnuViewRotate"
Me.MnuViewRotate.Size = New System.Drawing.Size(288, 22)
Me.MnuViewRotate.Text = "Rotate"
'
'MnuRotate0
'
Me.MnuRotate0.Image = CType(resources.GetObject("MnuRotate0.Image"), System.Drawing.Image)
Me.MnuRotate0.ImageTransparentColor = System.Drawing.Color.Silver
Me.MnuRotate0.Name = "MnuRotate0"
Me.MnuRotate0.Size = New System.Drawing.Size(148, 22)
Me.MnuRotate0.Text = "Rotate 0"
'
'rotate90ToolStripMenuItem
'
Me.rotate90ToolStripMenuItem.Image = CType(resources.GetObject("rotate90ToolStripMenuItem.Image"), System.Drawing.Image)
Me.rotate90ToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Silver
Me.rotate90ToolStripMenuItem.Name = "rotate90ToolStripMenuItem"
Me.rotate90ToolStripMenuItem.Size = New System.Drawing.Size(148, 22)
Me.rotate90ToolStripMenuItem.Text = "Rotate 90"
'
'rotate90ToolStripMenuItem1
'
Me.rotate90ToolStripMenuItem1.Image = CType(resources.GetObject("rotate90ToolStripMenuItem1.Image"), System.Drawing.Image)
Me.rotate90ToolStripMenuItem1.ImageTransparentColor = System.Drawing.Color.Silver
Me.rotate90ToolStripMenuItem1.Name = "rotate90ToolStripMenuItem1"
Me.rotate90ToolStripMenuItem1.Size = New System.Drawing.Size(148, 22)
Me.rotate90ToolStripMenuItem1.Text = "Rotate -90"
'
'rotate180ToolStripMenuItem
'
Me.rotate180ToolStripMenuItem.Image = CType(resources.GetObject("rotate180ToolStripMenuItem.Image"), System.Drawing.Image)
Me.rotate180ToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Silver
Me.rotate180ToolStripMenuItem.Name = "rotate180ToolStripMenuItem"
Me.rotate180ToolStripMenuItem.Size = New System.Drawing.Size(148, 22)
Me.rotate180ToolStripMenuItem.Text = "Rotate 180"
'
'MnuViewZoom
'
Me.MnuViewZoom.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.fitWidthToolStripMenuItem, Me.fitBestToolStripMenuItem, Me.fitHeightToolStripMenuItem, Me.toolStripMenuItem3, Me.zoomInToolStripMenuItem, Me.zoomOutToolStripMenuItem})
Me.MnuViewZoom.Name = "MnuViewZoom"
Me.MnuViewZoom.Size = New System.Drawing.Size(288, 22)
Me.MnuViewZoom.Text = "Zoom"
'
'fitWidthToolStripMenuItem
'
Me.fitWidthToolStripMenuItem.Image = CType(resources.GetObject("fitWidthToolStripMenuItem.Image"), System.Drawing.Image)
Me.fitWidthToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Silver
Me.fitWidthToolStripMenuItem.Name = "fitWidthToolStripMenuItem"
Me.fitWidthToolStripMenuItem.Size = New System.Drawing.Size(142, 22)
Me.fitWidthToolStripMenuItem.Text = "Fit Width"
'
'fitBestToolStripMenuItem
'
Me.fitBestToolStripMenuItem.Image = CType(resources.GetObject("fitBestToolStripMenuItem.Image"), System.Drawing.Image)
Me.fitBestToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Silver
Me.fitBestToolStripMenuItem.Name = "fitBestToolStripMenuItem"
Me.fitBestToolStripMenuItem.Size = New System.Drawing.Size(142, 22)
Me.fitBestToolStripMenuItem.Text = "Fit Best"
'
'fitHeightToolStripMenuItem
'
Me.fitHeightToolStripMenuItem.Image = CType(resources.GetObject("fitHeightToolStripMenuItem.Image"), System.Drawing.Image)
Me.fitHeightToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Silver
Me.fitHeightToolStripMenuItem.Name = "fitHeightToolStripMenuItem"
Me.fitHeightToolStripMenuItem.Size = New System.Drawing.Size(142, 22)
Me.fitHeightToolStripMenuItem.Text = "Fit Height"
'
'toolStripMenuItem3
'
Me.toolStripMenuItem3.Name = "toolStripMenuItem3"
Me.toolStripMenuItem3.Size = New System.Drawing.Size(139, 6)
'
'zoomInToolStripMenuItem
'
Me.zoomInToolStripMenuItem.Image = CType(resources.GetObject("zoomInToolStripMenuItem.Image"), System.Drawing.Image)
Me.zoomInToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Silver
Me.zoomInToolStripMenuItem.Name = "zoomInToolStripMenuItem"
Me.zoomInToolStripMenuItem.Size = New System.Drawing.Size(142, 22)
Me.zoomInToolStripMenuItem.Text = "Zoom In"
'
'zoomOutToolStripMenuItem
'
Me.zoomOutToolStripMenuItem.Image = CType(resources.GetObject("zoomOutToolStripMenuItem.Image"), System.Drawing.Image)
Me.zoomOutToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Silver
Me.zoomOutToolStripMenuItem.Name = "zoomOutToolStripMenuItem"
Me.zoomOutToolStripMenuItem.Size = New System.Drawing.Size(142, 22)
Me.zoomOutToolStripMenuItem.Text = "Zoom Out"
'
'toolStripMenuItem1
'
Me.toolStripMenuItem1.Name = "toolStripMenuItem1"
Me.toolStripMenuItem1.Size = New System.Drawing.Size(285, 6)
'
'MnuViewBookmarks
'
Me.MnuViewBookmarks.Name = "MnuViewBookmarks"
Me.MnuViewBookmarks.Size = New System.Drawing.Size(288, 22)
Me.MnuViewBookmarks.Text = "Bookmarks"
'
'toolStripMenuItem2
'
Me.toolStripMenuItem2.Name = "toolStripMenuItem2"
Me.toolStripMenuItem2.Size = New System.Drawing.Size(285, 6)
'
'MnuViewDisablePageLayout
'
Me.MnuViewDisablePageLayout.Checked = True
Me.MnuViewDisablePageLayout.CheckOnClick = True
Me.MnuViewDisablePageLayout.CheckState = System.Windows.Forms.CheckState.Checked
Me.MnuViewDisablePageLayout.Name = "MnuViewDisablePageLayout"
Me.MnuViewDisablePageLayout.Size = New System.Drawing.Size(288, 22)
Me.MnuViewDisablePageLayout.Text = "File cannot override Page Layout"
'
'MnuViewIngoreOpenAction
'
Me.MnuViewIngoreOpenAction.Checked = True
Me.MnuViewIngoreOpenAction.CheckOnClick = True
Me.MnuViewIngoreOpenAction.CheckState = System.Windows.Forms.CheckState.Checked
Me.MnuViewIngoreOpenAction.Name = "MnuViewIngoreOpenAction"
Me.MnuViewIngoreOpenAction.Size = New System.Drawing.Size(288, 22)
Me.MnuViewIngoreOpenAction.Text = "Igore File Open Action"
'
'ToolBar
'
Me.ToolBar.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BtnFitWidth, Me.BtnFitBest, Me.BtnFitHeight, Me.toolStripSeparator1, Me.BtnViewContinous, Me.BtnViewSinglePage, Me.toolStripSeparator2, Me.BtnZoomOut, Me.BtnZoomIn, Me.toolStripSeparator3, Me.BtnRotate90, Me.BtnRotateM90, Me.BtnRotate180, Me.toolStripSeparator4, Me.CboZoom, Me.toolStripLabel1, Me.TxtPageNum, Me.LblPageCount, Me.BtnErrors})
Me.ToolBar.Location = New System.Drawing.Point(0, 26)
Me.ToolBar.Name = "ToolBar"
Me.ToolBar.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional
Me.ToolBar.Size = New System.Drawing.Size(1365, 41)
Me.ToolBar.TabIndex = 6
Me.ToolBar.Text = "toolStrip1"
'
'BtnFitWidth
'
Me.BtnFitWidth.Checked = True
Me.BtnFitWidth.CheckOnClick = True
Me.BtnFitWidth.CheckState = System.Windows.Forms.CheckState.Checked
Me.BtnFitWidth.Enabled = False
Me.BtnFitWidth.Image = CType(resources.GetObject("BtnFitWidth.Image"), System.Drawing.Image)
Me.BtnFitWidth.ImageTransparentColor = System.Drawing.Color.Silver
Me.BtnFitWidth.Margin = New System.Windows.Forms.Padding(1, 1, 0, 2)
Me.BtnFitWidth.Name = "BtnFitWidth"
Me.BtnFitWidth.Size = New System.Drawing.Size(69, 38)
Me.BtnFitWidth.Text = "Fit Width"
Me.BtnFitWidth.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
'
'BtnFitBest
'
Me.BtnFitBest.CheckOnClick = True
Me.BtnFitBest.Enabled = False
Me.BtnFitBest.Image = CType(resources.GetObject("BtnFitBest.Image"), System.Drawing.Image)
Me.BtnFitBest.ImageTransparentColor = System.Drawing.Color.Silver
Me.BtnFitBest.Margin = New System.Windows.Forms.Padding(1, 1, 0, 2)
Me.BtnFitBest.Name = "BtnFitBest"
Me.BtnFitBest.Size = New System.Drawing.Size(61, 38)
Me.BtnFitBest.Text = "Fit Best"
Me.BtnFitBest.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
'
'BtnFitHeight
'
Me.BtnFitHeight.CheckOnClick = True
Me.BtnFitHeight.Enabled = False
Me.BtnFitHeight.Image = CType(resources.GetObject("BtnFitHeight.Image"), System.Drawing.Image)
Me.BtnFitHeight.ImageTransparentColor = System.Drawing.Color.Silver
Me.BtnFitHeight.Margin = New System.Windows.Forms.Padding(1, 1, 0, 2)
Me.BtnFitHeight.Name = "BtnFitHeight"
Me.BtnFitHeight.Size = New System.Drawing.Size(73, 38)
Me.BtnFitHeight.Text = "Fit Height"
Me.BtnFitHeight.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
'
'toolStripSeparator1
'
Me.toolStripSeparator1.Name = "toolStripSeparator1"
Me.toolStripSeparator1.Size = New System.Drawing.Size(6, 41)
'
'BtnViewContinous
'
Me.BtnViewContinous.Checked = True
Me.BtnViewContinous.CheckOnClick = True
Me.BtnViewContinous.CheckState = System.Windows.Forms.CheckState.Checked
Me.BtnViewContinous.Enabled = False
Me.BtnViewContinous.Image = CType(resources.GetObject("BtnViewContinous.Image"), System.Drawing.Image)
Me.BtnViewContinous.ImageTransparentColor = System.Drawing.Color.Silver
Me.BtnViewContinous.Name = "BtnViewContinous"
Me.BtnViewContinous.Size = New System.Drawing.Size(75, 38)
Me.BtnViewContinous.Text = "Continous"
Me.BtnViewContinous.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
'
'BtnViewSinglePage
'
Me.BtnViewSinglePage.CheckOnClick = True
Me.BtnViewSinglePage.Enabled = False
Me.BtnViewSinglePage.Image = CType(resources.GetObject("BtnViewSinglePage.Image"), System.Drawing.Image)
Me.BtnViewSinglePage.ImageTransparentColor = System.Drawing.Color.Silver
Me.BtnViewSinglePage.Name = "BtnViewSinglePage"
Me.BtnViewSinglePage.Size = New System.Drawing.Size(85, 38)
Me.BtnViewSinglePage.Text = "Single Page"
Me.BtnViewSinglePage.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
'
'toolStripSeparator2
'
Me.toolStripSeparator2.Name = "toolStripSeparator2"
Me.toolStripSeparator2.Size = New System.Drawing.Size(6, 41)
'
'BtnZoomOut
'
Me.BtnZoomOut.Enabled = False
Me.BtnZoomOut.Image = CType(resources.GetObject("BtnZoomOut.Image"), System.Drawing.Image)
Me.BtnZoomOut.ImageTransparentColor = System.Drawing.Color.Silver
Me.BtnZoomOut.Name = "BtnZoomOut"
Me.BtnZoomOut.Size = New System.Drawing.Size(78, 38)
Me.BtnZoomOut.Text = "Zoom Out"
Me.BtnZoomOut.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
'
'BtnZoomIn
'
Me.BtnZoomIn.Enabled = False
Me.BtnZoomIn.Image = CType(resources.GetObject("BtnZoomIn.Image"), System.Drawing.Image)
Me.BtnZoomIn.ImageTransparentColor = System.Drawing.Color.Silver
Me.BtnZoomIn.Name = "BtnZoomIn"
Me.BtnZoomIn.Size = New System.Drawing.Size(68, 38)
Me.BtnZoomIn.Text = "Zoom In"
Me.BtnZoomIn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
'
'toolStripSeparator3
'
Me.toolStripSeparator3.Name = "toolStripSeparator3"
Me.toolStripSeparator3.Size = New System.Drawing.Size(6, 41)
'
'BtnRotate90
'
Me.BtnRotate90.Enabled = False
Me.BtnRotate90.Image = CType(resources.GetObject("BtnRotate90.Image"), System.Drawing.Image)
Me.BtnRotate90.ImageTransparentColor = System.Drawing.Color.Silver
Me.BtnRotate90.Name = "BtnRotate90"
Me.BtnRotate90.Size = New System.Drawing.Size(76, 38)
Me.BtnRotate90.Text = "Rotate 90"
Me.BtnRotate90.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
'
'BtnRotateM90
'
Me.BtnRotateM90.Enabled = False
Me.BtnRotateM90.Image = CType(resources.GetObject("BtnRotateM90.Image"), System.Drawing.Image)
Me.BtnRotateM90.ImageTransparentColor = System.Drawing.Color.Silver
Me.BtnRotateM90.Name = "BtnRotateM90"
Me.BtnRotateM90.Size = New System.Drawing.Size(81, 38)
Me.BtnRotateM90.Text = "Rotate -90"
Me.BtnRotateM90.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
'
'BtnRotate180
'
Me.BtnRotate180.Enabled = False
Me.BtnRotate180.Image = CType(resources.GetObject("BtnRotate180.Image"), System.Drawing.Image)
Me.BtnRotate180.ImageTransparentColor = System.Drawing.Color.Silver
Me.BtnRotate180.Name = "BtnRotate180"
Me.BtnRotate180.Size = New System.Drawing.Size(84, 38)
Me.BtnRotate180.Text = "Rotate 180"
Me.BtnRotate180.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
'
'toolStripSeparator4
'
Me.toolStripSeparator4.Name = "toolStripSeparator4"
Me.toolStripSeparator4.Size = New System.Drawing.Size(6, 41)
'
'CboZoom
'
Me.CboZoom.Enabled = False
Me.CboZoom.Items.AddRange(New Object() {"10%", "25%", "50%", "75%", "100%", "125%", "150%", "200%", "300%", "400%", "800%", "1600%", "2400%", "3200%", "6400 %"})
Me.CboZoom.MaxDropDownItems = 20
Me.CboZoom.MaxLength = 8
Me.CboZoom.Name = "CboZoom"
Me.CboZoom.Size = New System.Drawing.Size(105, 41)
Me.CboZoom.Text = "100%"
'
'toolStripLabel1
'
Me.toolStripLabel1.Name = "toolStripLabel1"
Me.toolStripLabel1.Size = New System.Drawing.Size(40, 38)
Me.toolStripLabel1.Text = "Page"
'
'TxtPageNum
'
Me.TxtPageNum.BackColor = System.Drawing.SystemColors.Window
Me.TxtPageNum.BorderStyle = System.Windows.Forms.BorderStyle.None
Me.TxtPageNum.Enabled = False
Me.TxtPageNum.HideSelection = False
Me.TxtPageNum.Name = "TxtPageNum"
Me.TxtPageNum.Size = New System.Drawing.Size(67, 41)
Me.TxtPageNum.Text = "0"
'
'LblPageCount
'
Me.LblPageCount.Name = "LblPageCount"
Me.LblPageCount.Size = New System.Drawing.Size(34, 38)
Me.LblPageCount.Text = "of 0"
'
'BtnErrors
'
Me.BtnErrors.Image = CType(resources.GetObject("BtnErrors.Image"), System.Drawing.Image)
Me.BtnErrors.ImageTransparentColor = System.Drawing.Color.Silver
Me.BtnErrors.Name = "BtnErrors"
Me.BtnErrors.Size = New System.Drawing.Size(72, 38)
Me.BtnErrors.Text = "Warnings"
Me.BtnErrors.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
Me.BtnErrors.Visible = False
'
'TabControl
'
Me.TabControl.Alignment = System.Windows.Forms.TabAlignment.Left
Me.TabControl.Controls.Add(Me.TabBookmarks)
Me.TabControl.Controls.Add(Me.TabLayer)
Me.TabControl.Dock = System.Windows.Forms.DockStyle.Left
Me.TabControl.Location = New System.Drawing.Point(0, 67)
Me.TabControl.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
Me.TabControl.Multiline = True
Me.TabControl.Name = "TabControl"
Me.TabControl.SelectedIndex = 0
Me.TabControl.Size = New System.Drawing.Size(319, 772)
Me.TabControl.TabIndex = 7
Me.TabControl.Visible = False
'
'TabBookmarks
'
Me.TabBookmarks.Controls.Add(Me.TreeBookmarks)
Me.TabBookmarks.Controls.Add(Me.ToolStrip2)
Me.TabBookmarks.Location = New System.Drawing.Point(25, 4)
Me.TabBookmarks.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
Me.TabBookmarks.Name = "TabBookmarks"
Me.TabBookmarks.Padding = New System.Windows.Forms.Padding(4, 4, 4, 4)
Me.TabBookmarks.Size = New System.Drawing.Size(290, 764)
Me.TabBookmarks.TabIndex = 0
Me.TabBookmarks.Text = "Bookmarks"
Me.TabBookmarks.UseVisualStyleBackColor = True
'
'TreeBookmarks
'
Me.TreeBookmarks.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
Me.TreeBookmarks.Dock = System.Windows.Forms.DockStyle.Fill
Me.TreeBookmarks.Font = New System.Drawing.Font("Arial Unicode MS", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
Me.TreeBookmarks.ForeColor = System.Drawing.SystemColors.WindowText
Me.TreeBookmarks.HideSelection = False
Me.TreeBookmarks.HotTracking = True
Me.TreeBookmarks.ItemHeight = 20
Me.TreeBookmarks.Location = New System.Drawing.Point(4, 29)
Me.TreeBookmarks.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
Me.TreeBookmarks.Name = "TreeBookmarks"
Me.TreeBookmarks.ShowNodeToolTips = True
Me.TreeBookmarks.Size = New System.Drawing.Size(282, 731)
Me.TreeBookmarks.TabIndex = 8
'
'ToolStrip2
'
Me.ToolStrip2.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BtnExpandAll, Me.BtnCollapseAll, Me.BtnHideTabControl})
Me.ToolStrip2.Location = New System.Drawing.Point(4, 4)
Me.ToolStrip2.Name = "ToolStrip2"
Me.ToolStrip2.Size = New System.Drawing.Size(282, 25)
Me.ToolStrip2.TabIndex = 7
Me.ToolStrip2.Text = "ToolStrip2"
'
'BtnExpandAll
'
Me.BtnExpandAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
Me.BtnExpandAll.Image = CType(resources.GetObject("BtnExpandAll.Image"), System.Drawing.Image)
Me.BtnExpandAll.ImageTransparentColor = System.Drawing.Color.Magenta
Me.BtnExpandAll.Name = "BtnExpandAll"
Me.BtnExpandAll.Size = New System.Drawing.Size(60, 22)
Me.BtnExpandAll.Text = "Expand"
'
'BtnCollapseAll
'
Me.BtnCollapseAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
Me.BtnCollapseAll.Image = CType(resources.GetObject("BtnCollapseAll.Image"), System.Drawing.Image)
Me.BtnCollapseAll.ImageTransparentColor = System.Drawing.Color.Magenta
Me.BtnCollapseAll.Name = "BtnCollapseAll"
Me.BtnCollapseAll.Size = New System.Drawing.Size(64, 22)
Me.BtnCollapseAll.Text = "Collapse"
'
'BtnHideTabControl
'
Me.BtnHideTabControl.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
Me.BtnHideTabControl.Image = CType(resources.GetObject("BtnHideTabControl.Image"), System.Drawing.Image)
Me.BtnHideTabControl.ImageTransparentColor = System.Drawing.Color.Magenta
Me.BtnHideTabControl.Name = "BtnHideTabControl"
Me.BtnHideTabControl.Size = New System.Drawing.Size(79, 22)
Me.BtnHideTabControl.Text = "Hide Panel"
'
'TabLayer
'
Me.TabLayer.Location = New System.Drawing.Point(25, 4)
Me.TabLayer.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
Me.TabLayer.Name = "TabLayer"
Me.TabLayer.Padding = New System.Windows.Forms.Padding(4, 4, 4, 4)
Me.TabLayer.Size = New System.Drawing.Size(290, 758)
Me.TabLayer.TabIndex = 1
Me.TabLayer.Text = "Layer"
Me.TabLayer.UseVisualStyleBackColor = True
'
'Splitter
'
Me.Splitter.Location = New System.Drawing.Point(319, 67)
Me.Splitter.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
Me.Splitter.MinExtra = 0
Me.Splitter.MinSize = 0
Me.Splitter.Name = "Splitter"
Me.Splitter.Size = New System.Drawing.Size(8, 772)
Me.Splitter.TabIndex = 8
Me.Splitter.TabStop = False
Me.Splitter.Visible = False
'
'OpenDialog
'
Me.OpenDialog.Filter = "PDF Files |*.pdf"
Me.OpenDialog.Title = "Open PDF Files"
'
'PDFCanvas
'
Me.PDFCanvas.AccessibleRole = System.Windows.Forms.AccessibleRole.None
Me.PDFCanvas.AutoScroll = True
Me.PDFCanvas.AutoSize = True
Me.PDFCanvas.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange
Me.PDFCanvas.BackColor = System.Drawing.Color.FromArgb(CType(CType(60, Byte), Integer), CType(CType(60, Byte), Integer), CType(CType(60, Byte), Integer))
Me.PDFCanvas.CausesValidation = False
Me.PDFCanvas.ColorManagement = True
Me.PDFCanvas.Dock = System.Windows.Forms.DockStyle.Fill
Me.PDFCanvas.Location = New System.Drawing.Point(327, 67)
Me.PDFCanvas.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
Me.PDFCanvas.MaxErrCount = 100
Me.PDFCanvas.MinimumSize = New System.Drawing.Size(67, 62)
Me.PDFCanvas.Name = "PDFCanvas"
Me.PDFCanvas.PageLayout = DynaPDF.TPageLayout.plOneColumn
Me.PDFCanvas.PageScale = DynaPDF.TPDFPageScale.psFitWidth
Me.PDFCanvas.Rotate = 0
Me.PDFCanvas.Size = New System.Drawing.Size(1038, 772)
Me.PDFCanvas.TabIndex = 9
Me.PDFCanvas.UseScrollWindow = True
'
'Form1
'
Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
Me.ClientSize = New System.Drawing.Size(1365, 839)
Me.Controls.Add(Me.PDFCanvas)
Me.Controls.Add(Me.Splitter)
Me.Controls.Add(Me.TabControl)
Me.Controls.Add(Me.ToolBar)
Me.Controls.Add(Me.MainMenu)
Me.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
Me.Name = "Form1"
Me.Text = "Form1"
Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
Me.MainMenu.ResumeLayout(False)
Me.MainMenu.PerformLayout()
Me.ToolBar.ResumeLayout(False)
Me.ToolBar.PerformLayout()
Me.TabControl.ResumeLayout(False)
Me.TabBookmarks.ResumeLayout(False)
Me.TabBookmarks.PerformLayout()
Me.ToolStrip2.ResumeLayout(False)
Me.ToolStrip2.PerformLayout()
Me.ResumeLayout(False)
Me.PerformLayout()

End Sub
    Private WithEvents MainMenu As System.Windows.Forms.MenuStrip
    Private WithEvents MnuFile As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents MnuOpenFile As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents MnuCloseFile As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents MnuExit As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents MnuView As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents MnuViewPageDisplay As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents continousToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents singlePageToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents MnuViewRotate As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents MnuRotate0 As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents rotate90ToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents rotate90ToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents rotate180ToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents MnuViewZoom As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents fitWidthToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents fitBestToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents fitHeightToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents toolStripMenuItem3 As System.Windows.Forms.ToolStripSeparator
    Private WithEvents zoomInToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents zoomOutToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents toolStripMenuItem1 As System.Windows.Forms.ToolStripSeparator
    Private WithEvents MnuViewBookmarks As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents toolStripMenuItem2 As System.Windows.Forms.ToolStripSeparator
    Private WithEvents MnuViewDisablePageLayout As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents MnuViewIngoreOpenAction As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents ToolBar As System.Windows.Forms.ToolStrip
    Private WithEvents BtnFitWidth As System.Windows.Forms.ToolStripButton
    Private WithEvents BtnFitBest As System.Windows.Forms.ToolStripButton
    Private WithEvents BtnFitHeight As System.Windows.Forms.ToolStripButton
    Private WithEvents toolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Private WithEvents BtnViewContinous As System.Windows.Forms.ToolStripButton
    Private WithEvents BtnViewSinglePage As System.Windows.Forms.ToolStripButton
    Private WithEvents toolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
    Private WithEvents BtnZoomOut As System.Windows.Forms.ToolStripButton
    Private WithEvents BtnZoomIn As System.Windows.Forms.ToolStripButton
    Private WithEvents toolStripSeparator3 As System.Windows.Forms.ToolStripSeparator
    Private WithEvents BtnRotate90 As System.Windows.Forms.ToolStripButton
    Private WithEvents BtnRotateM90 As System.Windows.Forms.ToolStripButton
    Private WithEvents BtnRotate180 As System.Windows.Forms.ToolStripButton
    Private WithEvents toolStripSeparator4 As System.Windows.Forms.ToolStripSeparator
    Private WithEvents CboZoom As System.Windows.Forms.ToolStripComboBox
    Private WithEvents toolStripLabel1 As System.Windows.Forms.ToolStripLabel
    Private WithEvents TxtPageNum As System.Windows.Forms.ToolStripTextBox
    Private WithEvents LblPageCount As System.Windows.Forms.ToolStripLabel
    Private WithEvents BtnErrors As System.Windows.Forms.ToolStripButton
    Friend WithEvents TabControl As System.Windows.Forms.TabControl
    Friend WithEvents TabBookmarks As System.Windows.Forms.TabPage
    Private WithEvents TreeBookmarks As System.Windows.Forms.TreeView
    Private WithEvents ToolStrip2 As System.Windows.Forms.ToolStrip
    Private WithEvents BtnExpandAll As System.Windows.Forms.ToolStripButton
    Private WithEvents BtnCollapseAll As System.Windows.Forms.ToolStripButton
    Private WithEvents BtnHideTabControl As System.Windows.Forms.ToolStripButton
    Friend WithEvents TabLayer As System.Windows.Forms.TabPage
    Private WithEvents Splitter As System.Windows.Forms.Splitter
    Private WithEvents PDFCanvas As PDFControl.PDFCanvas
    Private WithEvents OpenDialog As System.Windows.Forms.OpenFileDialog

End Class
