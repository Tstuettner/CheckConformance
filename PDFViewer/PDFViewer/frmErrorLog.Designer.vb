<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmErrorLog
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
Me.Errors = New System.Windows.Forms.ListBox
Me.button2 = New System.Windows.Forms.Button
Me.button1 = New System.Windows.Forms.Button
Me.SuspendLayout()
'
'Errors
'
Me.Errors.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
Me.Errors.FormattingEnabled = True
Me.Errors.Location = New System.Drawing.Point(0, 3)
Me.Errors.Name = "Errors"
Me.Errors.Size = New System.Drawing.Size(613, 498)
Me.Errors.TabIndex = 5
'
'button2
'
Me.button2.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
Me.button2.Location = New System.Drawing.Point(629, 48)
Me.button2.Name = "button2"
Me.button2.Size = New System.Drawing.Size(107, 30)
Me.button2.TabIndex = 4
Me.button2.Text = "Clear"
Me.button2.UseVisualStyleBackColor = True
'
'button1
'
Me.button1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
Me.button1.DialogResult = System.Windows.Forms.DialogResult.OK
Me.button1.Location = New System.Drawing.Point(629, 12)
Me.button1.Name = "button1"
Me.button1.Size = New System.Drawing.Size(107, 30)
Me.button1.TabIndex = 3
Me.button1.Text = "Close"
Me.button1.UseVisualStyleBackColor = True
'
'frmErrorLog
'
Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
Me.ClientSize = New System.Drawing.Size(748, 506)
Me.Controls.Add(Me.Errors)
Me.Controls.Add(Me.button2)
Me.Controls.Add(Me.button1)
Me.Name = "frmErrorLog"
Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
Me.Text = "Error Log"
Me.ResumeLayout(False)

End Sub
    Friend WithEvents Errors As System.Windows.Forms.ListBox
    Private WithEvents button2 As System.Windows.Forms.Button
    Private WithEvents button1 As System.Windows.Forms.Button
End Class
