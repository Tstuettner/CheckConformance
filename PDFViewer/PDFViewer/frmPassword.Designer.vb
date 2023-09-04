<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmPassword
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
Me.txtPassword = New System.Windows.Forms.TextBox
Me.label1 = New System.Windows.Forms.Label
Me.BtnCancel = New System.Windows.Forms.Button
Me.BtnOK = New System.Windows.Forms.Button
Me.SuspendLayout()
'
'txtPassword
'
Me.txtPassword.Location = New System.Drawing.Point(12, 59)
Me.txtPassword.Name = "txtPassword"
Me.txtPassword.Size = New System.Drawing.Size(238, 20)
Me.txtPassword.TabIndex = 3
'
'label1
'
Me.label1.AutoSize = True
Me.label1.Location = New System.Drawing.Point(9, 43)
Me.label1.Name = "label1"
Me.label1.Size = New System.Drawing.Size(53, 13)
Me.label1.TabIndex = 5
Me.label1.Text = "Password"
'
'BtnCancel
'
Me.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
Me.BtnCancel.Location = New System.Drawing.Point(270, 52)
Me.BtnCancel.Name = "BtnCancel"
Me.BtnCancel.Size = New System.Drawing.Size(93, 27)
Me.BtnCancel.TabIndex = 6
Me.BtnCancel.Text = "Cancel"
Me.BtnCancel.UseVisualStyleBackColor = True
'
'BtnOK
'
Me.BtnOK.DialogResult = System.Windows.Forms.DialogResult.OK
Me.BtnOK.Location = New System.Drawing.Point(268, 14)
Me.BtnOK.Name = "BtnOK"
Me.BtnOK.Size = New System.Drawing.Size(96, 27)
Me.BtnOK.TabIndex = 4
Me.BtnOK.Text = "OK"
Me.BtnOK.UseVisualStyleBackColor = True
'
'frmPassword
'
Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
Me.ClientSize = New System.Drawing.Size(375, 91)
Me.Controls.Add(Me.txtPassword)
Me.Controls.Add(Me.label1)
Me.Controls.Add(Me.BtnCancel)
Me.Controls.Add(Me.BtnOK)
Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
Me.Name = "frmPassword"
Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
Me.Text = "Enter Password"
Me.ResumeLayout(False)
Me.PerformLayout()

End Sub
    Private WithEvents txtPassword As System.Windows.Forms.TextBox
    Private WithEvents label1 As System.Windows.Forms.Label
    Private WithEvents BtnCancel As System.Windows.Forms.Button
    Private WithEvents BtnOK As System.Windows.Forms.Button
End Class
